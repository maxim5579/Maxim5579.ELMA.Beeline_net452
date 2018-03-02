using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EleWise.ELMA.BPM.Mvc.Controllers;
using EleWise.ELMA.CRM.Models;
using EleWise.ELMA.Messages.Models;
using EleWise.ELMA.Model.Managers;
using EleWise.ELMA.Model.Services;
using EleWise.ELMA.Security.Models;
using EleWise.ELMA.Security.Services;
using EleWise.ELMA.Services;
using EleWise.ELMA.Web.Service.v1;
using Maxim5579.ELMA.Beeline_net452.BeelineAPI;
using Maxim5579.ELMA.Beeline_net452.Managers;

namespace Maxim5579.ELMA.Beeline_net452.Web.Controllers
{
    public class BeeOpenCallController : BPMController
    {
        //
        // GET: /BeeOpenCall/

        public ActionResult Index(string trackNum)
        {
            
            
            RelationshipCall call = EntityManager<RelationshipCall>.Instance.Find(
                    string.Format("UniqueId='{0}'", trackNum))
                .LastOrDefault();
            //return Redirect(string.Format("~/CRM/RelationshipCall/Details/{0}", call.Id));
            if (call.StatusRazgovora == EleWise.ELMA.ConfigurationModel.SostoyanieVyzova.RazgovorNachat)
            {
                ViewData["status"] = "Запуск процесса обработки клиента";
                ViewData["callTheme"] = call.Theme;
                return View();
            }
            else
                return Redirect(string.Format("~/CRM/RelationshipCall/Details/{0}", call.Id));
            
            //if (call != null)
            //    return Redirect(string.Format("~/CRM/RelationshipCall/Details/{0}", call.Id));
            //else
            //    return View();
        }

    }

}
