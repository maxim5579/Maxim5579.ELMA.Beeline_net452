using System;
using EleWise.ELMA.ComponentModel;
using EleWise.ELMA.CRM.Telephony.Extensions;
using Maxim5579.ELMA.Beeline_net452.Managers;
using EleWise.ELMA;
using EleWise.ELMA.CRM.Telephony.Managers;
using EleWise.ELMA.Logging;

namespace Maxim5579.ELMA.Beeline_net452.Components
{
    [Component]
    public class BeelineTelephonyProvider : ITelephonyProvider
    {
        public static string UID_S { get { return "{CDC4B225-F359-4F7E-B93F-62701743968E}"; } }
        private BeelineManager manager { get { return BeelineManager.Instance; } }
        private TelephonyManager TelephonyManager { get; } = TelephonyManager.Instance;

        public string DisplayName => SR.T("Билайн АТС");

        public Guid ProviderUid => new Guid(UID_S);

        public void MakeCall(string phone)
        {
            TelephonyManager.TelephonyLog.Info("MakeCall into TelephonyProvider - start");
            manager.MakeCall(phone);
            TelephonyManager.TelephonyLog.Info("MakeCall into TelephonyProvider - OK"); ;
        }
    }
}
