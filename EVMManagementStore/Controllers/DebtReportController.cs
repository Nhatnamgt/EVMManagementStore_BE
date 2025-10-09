using EVMManagementStore.Models;
using EVMManagementStore.Service.DTO;
using EVMManagementStore.Service.Interface.Dealer;
using EVMManagementStore.Service.Service.Dealer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EVMManagementStore.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DebtReportController : ControllerBase
    {
        private readonly IDebtReportService _debtReportService;

        public DebtReportController(IDebtReportService debtReportService)
        {
            _debtReportService = debtReportService;
        }

              [Authorize(Roles = "dealer")]
        [HttpGet("Dealers")]
        public async Task<IActionResult> GetDealerDebtReport()
        {
            var debt = await _debtReportService.GetDealerDebtReportAsync();
            if (debt == null || !debt.Any())
            {
                return NotFound(ApiResponse<string>.NotFoundResponse("Không có công nợ dealer"));
            }
            return Ok(ApiResponse<List<DebtReportDTO>>.OkResponse(debt.ToList(), "Lấy công nợ dealer thành công"));
        }
              [Authorize(Roles = "dealer")]
        [HttpGet("Customers")]
        public async Task<IActionResult> GetCustomerDebtReport()
        {
            var debt = await _debtReportService.GetCustomerDebtReportAsync();
            if (debt == null)
            {
                return NotFound(ApiResponse<string>.NotFoundResponse("Không có công nợ khách hàng"));
            }
            return Ok(ApiResponse<List<DebtReportDTO>>.OkResponse(debt.ToList(), "Lấy công nợ khách hàng thành công"));
        }
    }
}
