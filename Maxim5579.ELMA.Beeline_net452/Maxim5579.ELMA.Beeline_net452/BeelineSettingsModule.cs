using System;
using EleWise.ELMA.ComponentModel;
using EleWise.ELMA.Runtime.Settings;
using EleWise.ELMA;
using EleWise.ELMA.CRM.Telephony;
using EleWise.ELMA.CRM.Telephony.Managers;
using EleWise.ELMA.Services;
using Maxim5579.ELMA.Beeline_net452.Components;
using Maxim5579.ELMA.Beeline_net452.Managers;

namespace Maxim5579.ELMA.Beeline_net452
{
    [Component]
    public class BeelineSettingsModule : GlobalSettingsModuleBase<BeelineSettings>
    {
        public static Guid _ModuleGuid = new Guid("{23B1C4A4-3D30-4795-A7E3-73A96BC345E1}");
        public override Guid ModuleGuid{get{ return _ModuleGuid; } }
        public override string ModuleName { get { return SR.T("Настройки модуля Интеграция с Beeline"); } }

        public override void SaveSettings()
        {
            base.SaveSettings();
            var TelephonySettingsModule = Locator.GetServiceNotNull<TelephonySettingsModule>();
            if (TelephonySettingsModule.Settings.TelephonyProviderUid == new Guid(BeelineTelephonyProvider.UID_S))
            {
                BeelineManager.Instance.Initial();
            }
        }
    }
}
