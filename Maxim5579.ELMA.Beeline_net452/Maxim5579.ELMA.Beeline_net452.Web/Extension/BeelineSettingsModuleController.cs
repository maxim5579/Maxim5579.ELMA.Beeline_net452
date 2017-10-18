using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EleWise.ELMA.ComponentModel;
using EleWise.ELMA.Web.Mvc.Models.Settings;
using Microsoft.Web.Mvc.Html;

namespace Maxim5579.ELMA.Beeline_net452.Web.Extension
{
    //[Component(Order = 260)]
    [Component]
    public class BeelineSettingsModuleController : GlobalSettingsModuleControllerBase<BeelineSettings,BeelineSettingsModule>
    {
        public BeelineSettingsModuleController(BeelineSettingsModule module) : base(module)
        {
        }

        //
        // GET: /BeelineSettingsModule/

        public override MvcHtmlString RenderDisplay(HtmlHelper html)
        {
            return html.Action("View", "Home", new { area = RouteProvider.AreaName });
        }

        public override MvcHtmlString RenderEdit(HtmlHelper html)
        {
            return html.Action("Edit", "Home", new { area = RouteProvider.AreaName });
        }

    }
}
