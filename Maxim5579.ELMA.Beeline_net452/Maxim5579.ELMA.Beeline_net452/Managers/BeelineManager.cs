using System;
using EleWise.ELMA.Cache;
using EleWise.ELMA.ComponentModel;
using EleWise.ELMA.CRM.Telephony.Managers;
using EleWise.ELMA.Logging;
using EleWise.ELMA.Security;
using EleWise.ELMA.Security.Services;
using EleWise.ELMA.Services;
using Maxim5579.ELMA.Beeline_net452.BeelineAPI;

namespace Maxim5579.ELMA.Beeline_net452.Managers
{
    [Service(Scope = ServiceScope.Application)]
    public class BeelineManager
    {
        private static BeelineSettings BeelineSettings { get; set; }
        private BeeConnect BeeConnect;
        //private OutCallInfo OutCallInfo;

        public static BeelineManager Instance { get; } = Locator.GetServiceNotNull<BeelineManager>();
        private TelephonyManager TelephonyManager { get; } = TelephonyManager.Instance;
        public static ICacheService CacheService { get; } = Locator.GetServiceNotNull<ICacheService>();
        public static ISecurityService SecurityService { get; } = Locator.GetServiceNotNull<ISecurityService>();
        public static IAuthenticationService AuthenticationService { get; }=Locator.GetServiceNotNull<IAuthenticationService>();

        public void Initial()
        {
            BeelineSettings = Locator.GetServiceNotNull<BeelineSettingsModule>().Settings;
            if (string.IsNullOrEmpty(BeelineSettings.BeelineHost))
            {
                TelephonyManager.TelephonyLog.Error("Manager.Init: Адрес сервера недоступен");
                return;
            }
            if (string.IsNullOrEmpty(BeelineSettings.BeelineToken))
            {
                TelephonyManager.TelephonyLog.Error("Manager.Init: Токен сервера пустой");
                return;
            }
            BeeConnect = new BeeConnect(BeelineSettings.BeelineHost, BeelineSettings.BeelineToken);
            TelephonyManager.TelephonyLog.Info(string.Format("Beeline initialized: {0}! Host: {1}/// token: {2}",BeeConnect.init.ToString(),BeelineSettings.BeelineHost,BeelineSettings.BeelineToken));
        }
        public OutCallInfo MakeCall(string phone)
        {
            OutCallInfo outCall;
            EleWise.ELMA.Security.Models.IUser user = AuthenticationService.GetCurrentUser<EleWise.ELMA.Security.Models.IUser>();
            //var user=PublicAPI.Portal.Security.User.GetCurrentUser();
            TelephonyManager.TelephonyLog.Debug(
                String.Format("{0} Пользователь {1} (тел {2}) звонит абоненту {3}",BeeConnect.init, user.UserName,user.WorkPhone, phone));
            outCall = BeeConnect.MakeCall(user.WorkPhone,phone).Result;
            TelephonyManager.TelephonyLog.Debug(String.Format("Статус {0} ID исходящего звонка - {1}",outCall.statusCode ,outCall.callerID));
            return outCall;
        }

        
    }
}
