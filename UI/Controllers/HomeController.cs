using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UI.Services;

namespace UI.Controllers
{
  public class HomeController : Controller
  {
    public ActionResult Index()
    {
      return View();
    }

    public ActionResult Estatisticas()
    {
      var configs = new[] { "config1", "config2", "config3" };

      return View(configs);
    }

    public JsonResult GetEstatisticas(string config)
    {

      var statsReader = new StatsReader();
      var stats = statsReader.readConfigStats(config);

      return Json(stats, JsonRequestBehavior.AllowGet);
    }


    public ActionResult Som()
    {
      ViewBag.Message = "Your contact page.";

      return View();
    }
  }
}