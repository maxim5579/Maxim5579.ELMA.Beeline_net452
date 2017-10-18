using System;
using EleWise.ELMA.ComponentModel;
using EleWise.ELMA.CRM.Telephony;
using EleWise.ELMA.CRM.Telephony.Extensions;
using Maxim5579.ELMA.Beeline_net452.Managers;

namespace Maxim5579.ELMA.Beeline_net452.Components
{
    [Component]
    public class BeelineSettingsEventHandler : ITelephonySettingsEventHandler
    {
        public void OnSaveSettings(TelephonySettings settings)
        {
            if (settings.TelephonyProviderUid == new Guid(BeelineTelephonyProvider.UID_S))
            {
                BeelineManager.Instance.Initial();
            }
        }
    }
}