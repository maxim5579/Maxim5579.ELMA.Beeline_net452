using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EleWise.ELMA.ComponentModel;
using EleWise.ELMA.Web.Mvc.Models.Settings;
using System.Web.Mvc.Html;

namespace Maxim5579.ELMA.Beeline_net452.Web.Components
{
    [Component(Order = 270)]
    public class BeelineSettingsModuleController : GlobalSettingsModuleControllerBase<BeelineSettings, BeelineSettingsModule>
    {
        #region Overrides of GlobalSettingsModuleBase<SecuritySettings>
        public BeelineSettingsModuleController(BeelineSettingsModule module) : base(module)
        {
        }

        public override MvcHtmlString RenderDisplay(HtmlHelper html)
        {
            return html.Action("View", "BeelineSettings", new { area = RouteProvider.AreaName });
        }

        public override MvcHtmlString RenderEdit(HtmlHelper html)
        {
            return html.Action("Edit", "BeelineSettings", new { area = RouteProvider.AreaName });
        }
        #endregion
    }
}