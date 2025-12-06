using MadWin.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Stimulsoft.Report;
using Stimulsoft.Report.Mvc;

namespace Shop2City.WebHost.Controllers
{
    public class ReportController : Controller
    {
        private readonly IOrderService _orderService;

        public ReportController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        public IActionResult Viewer()
        {
            return View();
        }

        public IActionResult GetReportForOrder()
        {
            var report = StiReport.CreateNewReport();
            var path = StiNetCoreHelper.MapPath(this, "/wwwroot/Reports/ReportForPrint.mrt");
            report.Load(path);

            return StiNetCoreViewer.GetReportResult(this, report);
        }

        public IActionResult ViewerEvent()
        {
            return StiNetCoreViewer.ViewerEventResult(this);
        }
    }
}
