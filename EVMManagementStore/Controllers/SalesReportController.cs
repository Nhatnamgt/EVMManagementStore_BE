using EVMManagementStore.Models;
using EVMManagementStore.Service.DTO;
using EVMManagementStore.Service.Interface.EVM;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace EVMManagementStore.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SalesReportController : ControllerBase
    {
        private readonly ISalesReportService _salesReportService;

        public SalesReportController(ISalesReportService salesReportService)
        {
            _salesReportService = salesReportService;
        }

        [Authorize(Roles = "admin,evm_staff")]
        [HttpGet]
        public async Task<IActionResult> GetAllSalesReports()
        {
            var reports = await _salesReportService.GetAllSalesReportsAsync();

            if (reports == null || !reports.Any())
                return NotFound(ApiResponse<string>.NotFoundResponse("Không có dữ liệu báo cáo doanh thu"));

            return Ok(ApiResponse<List<SalesReportDTO>>.OkResponse(reports.ToList(), "Lấy báo cáo doanh thu thành công"));
        }
    }
}
