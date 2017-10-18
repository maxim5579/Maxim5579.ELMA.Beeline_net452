using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using EleWise.ELMA.ComponentModel;
using EleWise.ELMA.CRM.Telephony;
using EleWise.ELMA.Services;
using EleWise.ELMA.Web.Mvc.ExtensionPoints;
using Maxim5579.ELMA.Beeline_net452.Components;

namespace Maxim5579.ELMA.Beeline_net452.Web.Components
{
    [Component(EnableInterceptiors = false, InjectProerties = false, Order = 20)]
    public class MakeCallScriptExtensionZone : IExtensionZone
    {
        const string ZONE_ID = "Telephony-MakeCallScript";
        public bool CanRenderInZone(string zoneId, HtmlHelper html)
        {
            return zoneId == ZONE_ID;
        }

        public void RenderZone(string zoneId, HtmlHelper html)
        {
            if (zoneId == ZONE_ID)
            {
                var telephonySettings = Locator.GetServiceNotNull<TelephonySettingsModule>().Settings;
                if (telephonySettings.TelephonyProviderUid == new Guid(BeelineTelephonyProvider.UID_S))
                    html.RenderPartial("~/Modules/Maxim5579.ELMA.Beeline_net452.Web/Views/MakeCallScript.cshtml");
            }
        }
    }
}