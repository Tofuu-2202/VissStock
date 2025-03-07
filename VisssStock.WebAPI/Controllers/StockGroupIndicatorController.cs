using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;
using VisssStock.Application.DTOs;
using VisssStock.Application.DTOs.ProjectDTOs;
using VisssStock.Application.Models;
using VisssStock.Application.Models.Pagination;
using VisssStock.Application.Services.StockGroupIndicatorServices;
using VisssStock.Infrastructure.Data;

namespace VisssStock.WebAPI.Controllers
{
    [Authorize]
    [ApiController]
    [EnableCors("AllowAll")]
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class StockGroupIndicatorController : ControllerBase
    {
        private readonly IStockGroupIndicatorService _stockGroupIndicator;
        private readonly DataContext _context;
        private readonly IConfiguration _configuration;

        public StockGroupIndicatorController(IStockGroupIndicatorService stockGroupIndicator, DataContext context, IConfiguration configuration)
        {
            _stockGroupIndicator = stockGroupIndicator;
            _context = context;
            _configuration = configuration;
        }
        // get all stock group indicators
        [HttpGet("getAllStockGroupIndicators")]
        public async Task<ActionResult<ServiceResponse<PagedListResponseDTO<StockGroupIndicatorResponseDTO>>>> getAllStockGroupIndicators([FromQuery] OwnerParameters ownerParameters, string searchByName)
        {
            var response = await _stockGroupIndicator.getAllStockGroupIndicators(ownerParameters, searchByName);
            if (response.Status == false)
            {
                return BadRequest(new ProblemDetails
                {
                    Status = response.ErrorCode,
                    Title = response.Message
                });
            }
            return Ok(response);
        }

        // create stock group indicators
        //[HttpPost("createStockGroupIndicator")]
        //public async Task<ActionResult<ServiceResponse<StockGroupIndicatorResponseDTO>>> createStockGroupIndicator([FromBody] List<CreateStockGroupIndicatorDTO> createStockGroupIndicatorDTOs)
        //{
        //    var response = await _stockGroupIndicator.CreateStockGroupIndicator(createStockGroupIndicatorDTOs);
        //    if (response.Status == false)
        //    {
        //        return BadRequest(new ProblemDetails
        //        {
        //            Status = response.ErrorCode,
        //            Title = response.Message
        //        });
        //    }
        //    return Ok(response);
        //}

        // update stock group indicators
        [HttpPut("updateStockGroupIndicator/{id}")]
        public async Task<ActionResult<ServiceResponse<StockGroupIndicatorResponseDTO>>> updateStockGroupIndicator(UpdateStockGroupIndicatorDTO updateStockGroupIndicatorDTO, int id)
        {
            var response = await _stockGroupIndicator.updateStockGroupIndicator(updateStockGroupIndicatorDTO, id);
            if (response.Status == false)
            {
                return BadRequest(new ProblemDetails
                {
                    Status = response.ErrorCode,
                    Title = response.Message
                });
            }
            return Ok(response);
        }

        // delete stock group indicators
        [HttpDelete("deleteStockGroupIndicator/{id}")]
        public async Task<ActionResult<ServiceResponse<StockGroupIndicatorResponseDTO>>> deleteStockGroupIndicator(int id)
        {
            var response = await _stockGroupIndicator.deleteStockGroupIndicator(id);
            if (response.Status == false)
            {
                return BadRequest(new ProblemDetails
                {
                    Status = response.ErrorCode,
                    Title = response.Message
                });
            }
            return Ok(response);
        }

        // get stock group indicators by id
        [HttpGet("getStockGroupIndicatorById/{id}")]
        public async Task<ActionResult<ServiceResponse<StockGroupIndicatorResponseDTO>>> getStockGroupIndicatorById(int id)
        {
            var response = await _stockGroupIndicator.getStockGroupIndicatorById(id);
            if (response.Status == false)
            {
                return BadRequest(new ProblemDetails
                {
                    Status = response.ErrorCode,
                    Title = response.Message
                });
            }
            return Ok(response);
        }


        [HttpPost("CreateUpdateStockGroupIndicatorAsync/{stockGroupId}")]
        public async Task<ActionResult<ServiceResponse<List<StockGroupIndicatorResponseDTO>>>> CreateUpdateStockGroupIndicatorAsync(int stockGroupId, [FromBody] List<StockGroupIndicatorRequestDTO> stockGroupIndicatorRequestDTOs)
        {
            var response = await _stockGroupIndicator.CreateUpdateStockGroupIndicatorAsync(stockGroupId, stockGroupIndicatorRequestDTOs);
            if (!response.Status)
            {
                return StatusCode(response.ErrorCode, response);
            }
            return Ok(response);
        }

        // GET: api/StockGroupIndicator/GetAllIndicatorByStockGroupId/{stockGroupId}
        [HttpGet("GetAllStockGroupIndicatorByStockGroupId/{stockGroupId}")]
        public async Task<ActionResult<ServiceResponse<List<StockGroupIndicatorResponseDTO2>>>> GetAllIndicatorByStockGroupId(int stockGroupId)
        {
            var response = await _stockGroupIndicator.GetAllStockGroupIndicatorByStockGroupId(stockGroupId);
            if (!response.Status)
            {
                return StatusCode(response.ErrorCode, response);
            }
            return Ok(response);
        }

        // GET: api/StockGroupIndicator/GetAllIndicatorByStockGroupId/{stockGroupId}
        [HttpGet("ValidateFormula")]
        public ActionResult<ServiceResponse<bool>> ValidateFormula([FromQuery] string formula)
        {
            var response = _stockGroupIndicator.CheckFormula(formula);
            if (!response.Status)
            {
                return StatusCode(response.ErrorCode, response);
            }
            return Ok(response);
        }

        //public static bool EvaluateLogicalExpression(string expression)
        //{

        //    //var evaluator = new Expression(expression);
        //    //var result = evaluator.Evaluate();
        //    //return Convert.ToBoolean(result);
        //}
    }
}
