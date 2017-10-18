using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EleWise.ELMA.ComponentModel;
using EleWise.ELMA.CRM.Telephony;
using EleWise.ELMA.Scheduling;
using EleWise.ELMA.Services;
using Maxim5579.ELMA.Beeline_net452.Managers;

namespace Maxim5579.ELMA.Beeline_net452.Components
{
    [Component]
    public class BeelineConnectionSheduler : ISweepHandler
    {
        public void Execute()
        {
            var telephonySettingsModule = Locator.GetServiceNotNull<TelephonySettingsModule>();
            if (telephonySettingsModule.Settings.TelephonyProviderUid == new Guid(BeelineTelephonyProvider.UID_S))
            {
                BeelineManager.Instance.Initial();
            }
        }
    }
}
