using AutoMapper;
// using DocumentFormat.OpenXml.Office.Word;
using K4os.Compression.LZ4.Internal;
// using Microsoft.CodeAnalysis.Recommendations;
using Microsoft.EntityFrameworkCore;
using VisssStock.Application.DTOs;
using VisssStock.Application.Models;
using VisssStock.Application.Models.Pagination;
using VisssStock.Application.Interfaces;
using VisssStock.Infrastructure.Data;
using VisssStock.Domain.DataObjects;

namespace VisssStock.Application.Services.IntervalServices
{
    public class IntervalServiceImpl : IIntervalService
    {
        public DataContext _context;
        public IMapper _mapper;
        public IConfiguration _configuration;

        public IntervalServiceImpl()
        {

        }

        public IntervalServiceImpl(DataContext context, IMapper mapper, IConfiguration configuration)
        {
            _context = context;
            _mapper = mapper;
            _configuration = configuration;
        }

        public async Task<ServiceResponse<IntervalResponseDTO>> createInterval(CreateIntervalDTO createIntervalDTO)
        {
            var interval = _mapper.Map<Interval>(createIntervalDTO);
            await _context.Intervals.AddAsync(interval);
            await _context.SaveChangesAsync();
            return ServiceResponse<IntervalResponseDTO>.Success(_mapper.Map<IntervalResponseDTO>(interval));
        }

        public async Task<ServiceResponse<IntervalResponseDTO>> deleteInterval(int id)
        {
            var interval = await _context.Intervals.FirstOrDefaultAsync(x => x.Id == id);
            if (interval == null)
            {
                return ServiceResponse<IntervalResponseDTO>.Failure(404, "Interval not found.");
            }
            interval.IsDeleted = 1;
            interval.UpdateDate = DateTime.Now;
            _context.Intervals.Update(interval);

            await _context.SaveChangesAsync();
            return ServiceResponse<IntervalResponseDTO>.Success(_mapper.Map<IntervalResponseDTO>(interval));
        }

        public async Task<ServiceResponse<PagedListResponseDTO<IntervalResponseDTO>>> getAllIntervals(OwnerParameters ownerParameters, string searchByName)
        {
            try
            {
                // Truy vấn dữ liệu từ bảng Intervals
                var dbIntervals = _context.Intervals
                    .Where(c => c.IsDeleted == 0);

                // Lọc dữ liệu theo tên nếu có
                if (!string.IsNullOrEmpty(searchByName))
                {
                    dbIntervals = dbIntervals
                        .Where(x => x.Symbol.ToUpper().Contains(searchByName.ToUpper()));
                }

                // Áp dụng phân trang với PagedList
                var intervals = await dbIntervals.ToListAsync();
                var intervalResponseDTOs = _mapper.Map<List<IntervalResponseDTO>>(intervals);
                var intervalsPagedList = PagedList<IntervalResponseDTO>.ToPagedList(intervalResponseDTOs, ownerParameters.pageIndex, ownerParameters.pageSize);

                // Sử dụng PagedListResponseDTO
                var pagedResponse = new PagedListResponseDTO<IntervalResponseDTO>(intervalsPagedList);
                return ServiceResponse<PagedListResponseDTO<IntervalResponseDTO>>.Success(pagedResponse);
            }
            catch (Exception ex)
            {
                return ServiceResponse<PagedListResponseDTO<IntervalResponseDTO>>.Failure(500, ex.Message);
            }
        }

        public async Task<ServiceResponse<IntervalResponseDTO>> getIntervalById(int id)
        {
            var interval = await _context.Intervals.FirstOrDefaultAsync(x => x.Id == id);
            if (interval == null)
            {
                return ServiceResponse<IntervalResponseDTO>.Failure(404, "Interval not found.");
            }
            return ServiceResponse<IntervalResponseDTO>.Success(_mapper.Map<IntervalResponseDTO>(interval));
        }

        public async Task<ServiceResponse<IntervalResponseDTO>> updateInterval(UpdateIntervalDTO updateIntervalDTO, int id)
        {
            var interval = await _context.Intervals.FirstOrDefaultAsync(x => x.Id == id);
            if (interval == null)
            {
                return ServiceResponse<IntervalResponseDTO>.Failure(404, "Interval not found.");
            }

            // Sử dụng AutoMapper để cập nhật các thuộc tính của đối tượng hiện có
            _mapper.Map(updateIntervalDTO, interval);

            // Cập nhật thời gian cập nhật
            interval.UpdateDate = DateTime.Now;

            // Đánh dấu đối tượng là đã bị thay đổi
            _context.Intervals.Update(interval);

            // Lưu thay đổi vào cơ sở dữ liệu
            await _context.SaveChangesAsync();

            return ServiceResponse<IntervalResponseDTO>.Success(_mapper.Map<IntervalResponseDTO>(interval));
        }

        public async Task<ServiceResponse<IntervalResponseDTO>> insertIndicatorsTool()
        {
            List<string> list = new List<string>
            {
                "Recommend.Other", "Recommend.All", "Recommend.MA", "RSI", "RSI[1]", "Stoch.K", "Stoch.D", "Stoch.K[1]", "Stoch.D[1]", "CCI20", "CCI20[1]", "ADX", "ADX+DI", "ADX-DI", "ADX+DI[1]", "ADX-DI[1]", "AO", "AO[1]", "Mom", "Mom[1]", "MACD.macd", "MACD.signal", "Rec.Stoch.RSI", "Stoch.RSI.K", "Rec.WR", "W.R", "Rec.BBPower", "BBPower", "Rec.UO", "UO", "close", "EMA5", "SMA5", "EMA10", "SMA10", "EMA20", "SMA20", "EMA30", "SMA30", "EMA50", "SMA50", "EMA100", "SMA100", "EMA200", "SMA200", "Rec.Ichimoku", "Ichimoku.BLine", "Rec.VWMA", "VWMA", "Rec.HullMA9", "HullMA9", "Pivot.M.Classic.S3", "Pivot.M.Classic.S2", "Pivot.M.Classic.S1", "Pivot.M.Classic.Middle", "Pivot.M.Classic.R1",
                  "Pivot.M.Classic.R2", "Pivot.M.Classic.R3", "Pivot.M.Fibonacci.S3", "Pivot.M.Fibonacci.S2", "Pivot.M.Fibonacci.S1", "Pivot.M.Fibonacci.Middle", "Pivot.M.Fibonacci.R1", "Pivot.M.Fibonacci.R2", "Pivot.M.Fibonacci.R3", "Pivot.M.Camarilla.S3", "Pivot.M.Camarilla.S2", "Pivot.M.Camarilla.S1", "Pivot.M.Camarilla.Middle", "Pivot.M.Camarilla.R1", "Pivot.M.Camarilla.R2", "Pivot.M.Camarilla.R3", "Pivot.M.Woodie.S3", "Pivot.M.Woodie.S2", "Pivot.M.Woodie.S1", "Pivot.M.Woodie.Middle", "Pivot.M.Woodie.R1", "Pivot.M.Woodie.R2", "Pivot.M.Woodie.R3", "Pivot.M.Demark.S1", "Pivot.M.Demark.Middle", "Pivot.M.Demark.R1", "open", "P.SAR", "BB.lower", "BB.upper", "AO[2]", "volume", "change", "low", "high"
            };

            foreach (var item in list)
            {
                var indicator = new Indicator
                {
                    Name = item,
                    CreateBy = 20,
                    UpdateBy = 20,
                    CreateDate = DateTime.Now,
                    UpdateDate = DateTime.Now,
                    IsDeleted = 0
                };

                await _context.Indicators.AddAsync(indicator);
            }

            await _context.SaveChangesAsync();

            return ServiceResponse<IntervalResponseDTO>.Success(null);
        }
    }
}
