using AutoMapper;
using Microsoft.EntityFrameworkCore;
using NCalc;
using System.Linq;
using System.Security.Claims;
using System.Text.RegularExpressions;
using VisssStock.Application.DTOs;
using VisssStock.Application.Models;
using VisssStock.Application.Models.Pagination;
using VisssStock.Application.Interfaces;
using VisssStock.Infrastructure.Data;
using VisssStock.Domain.DataObjects;

namespace VisssStock.Application.Services.IndicatorServices
{
    public class IndicatorServiceImpl : IIndicatorService
    {

        private readonly DataContext _context;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public IndicatorServiceImpl(DataContext context, IMapper mapper, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _mapper = mapper;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<ServiceResponse<IndicatorResponseDTO>> createIndicator(CreateIndicatorDTO createIndicatorDTO)
        {
            var userId = GetCurrentUserId();
            var indicator = _mapper.Map<Indicator>(createIndicatorDTO);
            var checkFormula = await CheckFormula(createIndicatorDTO.Formula);

            if (!checkFormula)
            {
                return ServiceResponse<IndicatorResponseDTO>.Failure(400, "Invalid formula");
            }
            indicator.CreateBy = userId;
            indicator.CreateDate = DateTime.Now;

            await _context.Indicators.AddAsync(indicator);
            await _context.SaveChangesAsync();
            return ServiceResponse<IndicatorResponseDTO>.Success(_mapper.Map<IndicatorResponseDTO>(indicator));
        }

        public async Task<ServiceResponse<IndicatorResponseDTO>> deleteIndicator(int id)
        {
            var indicator = await _context.Indicators.FirstOrDefaultAsync(x => x.Id == id);
            if (indicator == null)
            {
                return ServiceResponse<IndicatorResponseDTO>.Failure(404, "Indicator not found.");
            }

            indicator.IsDeleted = 1;
            indicator.UpdateDate = DateTime.Now;
            _context.Indicators.Update(indicator);

            await _context.SaveChangesAsync();
            return ServiceResponse<IndicatorResponseDTO>.Success(_mapper.Map<IndicatorResponseDTO>(indicator));
        }

        public async Task<ServiceResponse<PagedListResponseDTO<IndicatorResponseDTO>>> getAllIndicators(OwnerParameters ownerParameters, string searchByName)
        {
            try
            {
                // Truy vấn dữ liệu từ bảng Indicators
                var dbIndicator = _context.Indicators
                    .Where(c => c.IsDeleted == 0);

                // Lọc dữ liệu theo tên nếu có
                if (!string.IsNullOrEmpty(searchByName))
                {
                    dbIndicator = dbIndicator
                        .Where(x => x.Name.ToUpper().Contains(searchByName.ToUpper()));
                }

                // Áp dụng phân trang với PagedList
                var indicators = await dbIndicator.ToListAsync();
                var indicatorResponseDTOs = _mapper.Map<List<IndicatorResponseDTO>>(indicators);
                var pagedIndicators = PagedList<IndicatorResponseDTO>.ToPagedList(indicatorResponseDTOs, ownerParameters.pageIndex, ownerParameters.pageSize);

                // Sử dụng PagedListResponseDTO
                var pagedResponse = new PagedListResponseDTO<IndicatorResponseDTO>(pagedIndicators);
                return ServiceResponse<PagedListResponseDTO<IndicatorResponseDTO>>.Success(pagedResponse);
            }
            catch (Exception ex)
            {
                return ServiceResponse<PagedListResponseDTO<IndicatorResponseDTO>>.Failure(500, ex.Message);
            }
        }


        public async Task<ServiceResponse<IndicatorResponseDTO>> getIndicatorById(int id)
        {

            var indicator = await _context.Indicators.FirstOrDefaultAsync(x => x.Id == id);
            if (indicator == null)
            {
                return ServiceResponse<IndicatorResponseDTO>.Failure(404, "Indicator not found.");
            }
            return ServiceResponse<IndicatorResponseDTO>.Success(_mapper.Map<IndicatorResponseDTO>(indicator));
        }

        public async Task<ServiceResponse<IndicatorResponseDTO>> updateIndicator(UpdateIndicatorDTO updateIndicatorDTO, int id)
        {

            var userId = GetCurrentUserId();

            var indicator = await _context.Indicators.FirstOrDefaultAsync(x => x.Id == id);

            if(indicator.CreateBy != userId)
            {
                return ServiceResponse<IndicatorResponseDTO>.Failure(403, "You do not have permission to update this indicator.");
            }

            if (indicator == null)
            {
                return ServiceResponse<IndicatorResponseDTO>.Failure(404, "Indicator not found.");
            }

            var checkFormula = await CheckFormula(updateIndicatorDTO.Formula);

            if (!checkFormula)
            {
                return ServiceResponse<IndicatorResponseDTO>.Failure(400, "Invalid formula");
            }

            // Sử dụng AutoMapper để cập nhật các thuộc tính của đối tượng hiện có
            _mapper.Map(updateIndicatorDTO, indicator);

            // Cập nhật thời gian cập nhật
            indicator.UpdateDate = DateTime.Now;

            // Đánh dấu đối tượng là đã bị thay đổi
            _context.Indicators.Update(indicator);

            // Lưu thay đổi vào cơ sở dữ liệu
            await _context.SaveChangesAsync();

            return ServiceResponse<IndicatorResponseDTO>.Success(_mapper.Map<IndicatorResponseDTO>(indicator));
        }

        private async Task<bool> CheckFormula(string formula)
        {
            try
            {
                if (!formula.StartsWith("="))
                {
                    throw new Exception("Invalid formula");
                }
                formula = formula.Substring(1);
                if (string.IsNullOrEmpty(formula))
                {
                    throw new Exception("Invalid formula");
                }
                var regex = new Regex(@"@(\w+(\.\w+)*)");
                var placeholders = new HashSet<string>();

                foreach (Match match in regex.Matches(formula))
                {
                    placeholders.Add(match.Value.Substring(1)); // Remove '@'
                }

                foreach (string match in placeholders)
                {
                    var check = await _context.Indicators.FirstOrDefaultAsync(x => x.Name.ToLower() == match.ToLower());
                    if (check == null)
                    {
                        throw new Exception("Not found indicator name" + match +" in list indicator of the System");
                    }
                }

                var formulaWithValues = formula;
                foreach (var placeholder in placeholders)
                {
                    formulaWithValues = formulaWithValues.Replace($"@{placeholder}", "1");
                }

                var result = EvaluateFormula(formulaWithValues);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private static double EvaluateFormula(string formula)
        {
            try
            {
                // Xử lý các phép toán không được hỗ trợ trực tiếp (như Abs) trước khi truyền vào NCalc
                formula = ProcessSpecialFunctions(formula);

                // Tạo biểu thức NCalc
                var expression = new Expression(formula);

                // Đánh giá công thức
                var result = expression.Evaluate();

                // Trả về kết quả dưới dạng double
                return Convert.ToDouble(result);
            }
            catch (Exception ex)
            {
                // Xử lý lỗi đánh giá công thức
                throw new Exception($"Error evaluating formula: {formula}. Exception: {ex.Message}");
            }
        }

        private static string ProcessSpecialFunctions(string formula)
        {
            try
            {
                //if (formula.Contains("Abs"))
                //{
                //    Console.WriteLine($"Processing Abs function in formula: {formula}");
                //}
                // Xử lý hàm Abs
                var regexAbs = new Regex(@"Abs\(([^)]+)\)");
                formula = regexAbs.Replace(formula, match =>
                {
                    // Lấy biểu thức bên trong hàm Abs
                    var innerExpression = match.Groups[1].Value;

                    // Đánh giá biểu thức bên trong hàm Abs
                    var expression = new Expression(innerExpression);
                    var result = expression.Evaluate();

                    // Tính giá trị tuyệt đối
                    return Math.Abs(Convert.ToDouble(result)).ToString();
                });

                // Trả về công thức đã xử lý
                return formula;
            }
            catch
            {
                throw new Exception("Invalid formula");
            }
        }

        private int GetCurrentUserId()
        {
            return int.Parse(_httpContextAccessor?.HttpContext?.User?.Claims
                    .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value);
        }
    }

}
