using MadWin.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Stimulsoft.Report;
using Stimulsoft.Report.Mvc;

namespace Shop2City.WebHost.Controllers
{
    public class ReportController : Controller
    {
        private readonly IReportService _reportService;

        public ReportController(IReportService reportService)
        {
            _reportService = reportService;
        }

        public IActionResult Viewer()
        {
            return View();
        }

        public async Task<IActionResult> GetReportForOrder(int orderId)
        {
            var report = StiReport.CreateNewReport();
            report.Load(StiNetCoreHelper.MapPath(this, "/wwwroot/Reports/ReportOrderForPrint.mrt"));

            var result = await _reportService.GetUserOrderPrintByOrderId(orderId);

            report.RegData("dtOrder", result);

            return StiNetCoreViewer.GetReportResult(this, report);
        }

        public async Task<IActionResult> GetReportForFactor(int factorId)
        {
            var report = StiReport.CreateNewReport();
            report.Load(StiNetCoreHelper.MapPath(this, "/wwwroot/Reports/ReportFactorForPrint.mrt"));

            var result = await _reportService.GetUserFactorPrintByFactorId(factorId);

            report.RegData("dtFactor", result);

            return StiNetCoreViewer.GetReportResult(this, report);
        }


        public IActionResult ViewerEvent()
        {
            return StiNetCoreViewer.ViewerEventResult(this);
        }
    }
}
