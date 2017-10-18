using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EleWise.ELMA.BPM.Mvc.Controllers;

namespace Maxim5579.ELMA.Beeline_net452.Web.Controllers
{
    public class BeelineSettingsController : BPMController
    {
        public BeelineSettingsModule SettingsModule { get; set; }

        public BeelineSettings Settings
        {
            get { return SettingsModule.Settings; }
        }


        [HttpGet]
        public ActionResult View()
        {
            return PartialView(Settings);
        }

        [HttpGet]
        public ActionResult Edit()
        {
            return PartialView(Settings);
        }

    }
}
