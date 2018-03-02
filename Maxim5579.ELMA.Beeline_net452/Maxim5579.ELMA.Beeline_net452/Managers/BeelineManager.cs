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
        //private static object _lockForNewInnerCall = new object();
        //private object _lockForNewInnerCallCenterInnerCall;
        //private static object _lockForCallCenterReleasedCall = new object();
        //private static object _lockForNewOuterCall = new object();
        //private static object _lockForCallAnswer = new object();

        //public object LockCallCenterInnerCall
        //{
        //    get { return _lockForNewInnerCallCenterInnerCall; }
        //}
        //public static object LockForCallCenterReleasedCall
        //{
        //    get{return _lockForCallCenterReleasedCall;}
        //}

        private string TestSubscriptionUri =
            "http://138.201.120.143/PublicAPI/REST/EleWise.ELMA.SDK.Web/BeelineWeb/SetEvents"; //TODO после тестирования внести в BeelineSettings

        public BeeConnect BeeConnect;

        public BeelineConnector BeelineConnect;

        public CallEventsController CallController;
        //private OutCallInfo OutCallInfo;

        public static BeelineManager Instance { get; } = Locator.GetServiceNotNull<BeelineManager>();
        private TelephonyManager TelephonyManager { get; } = TelephonyManager.Instance;
        public static ICacheService CacheService { get; } = Locator.GetServiceNotNull<ICacheService>();
        public static ISecurityService SecurityService { get; } = Locator.GetServiceNotNull<ISecurityService>();
        public static IAuthenticationService AuthenticationService { get; }=Locator.GetServiceNotNull<IAuthenticationService>();

        

        public void Initial()
        {
            CallController = new CallEventsController();
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
            TelephonyManager.TelephonyLog.Debug(string.Format("Beeline initialized: {0}! Host: {1}/// token: {2}", BeeConnect.init.ToString(), BeelineSettings.BeelineHost, BeelineSettings.BeelineToken));
            BeelineConnect = new BeelineConnector(BeeConnect, TestSubscriptionUri);
            TelephonyManager.TelephonyLog.Debug("BeelineConnectConnector construction OK");
            BeelineConnect.Initialize();
            TelephonyManager.TelephonyLog.Debug("BeelineConnectConnector Initialization OK");

            #region Подписка на события телефонии
            BeelineConnect.onNewInnerCall += eventNewInnerCall;
            BeelineConnect.onNewOuterCall += eventNewOuterCall;
            BeelineConnect.onCallAnswered += eventCallAnswer;
            BeelineConnect.onCallEnding += eventCallReleased;
            BeelineConnect.onCallCenterInnerCall += eventNewCallCenterCall;
            #endregion

            //_lockForCallCenterReleasedCall = new object();
            
            TelephonyManager.TelephonyLog.Debug(
                string.Format("Total {0} users subscription", BeelineConnect.AbonentsConnector.Count.ToString()));
        }
        public OutCallInfo MakeCall(string phone)
        {
            OutCallInfo outCall;
                EleWise.ELMA.Security.Models.IUser user = AuthenticationService.GetCurrentUser<EleWise.ELMA.Security.Models.IUser>();
                //TelephonyManager.TelephonyLog.Debug(
                //    String.Format("{0} Пользователь {1} (тел {2}) звонит абоненту {3}", BeeConnect.init, user.UserName, user.WorkPhone, phone));
                outCall = BeeConnect.MakeCall(user.WorkPhone, phone).Result;
                //TelephonyManager.TelephonyLog.Debug(String.Format("Статус {0} ID исходящего звонка - {1}", outCall.statusCode, outCall.callerID));
            return outCall;
        }

        private void eventNewInnerCall(Dial newDial)
        {
                CallController.CreateNewInnerCall(newDial);
                TelephonyManager.PushCommand(newDial.RemoteAddress,
                    BeelineConnect.FindAbonentsBy(newDial.SubscriptionID).Key.UserName,
                    newDial.TrackingId);
        }

        private void eventNewCallCenterCall(Dial newDial)
        {
            CallController.ProcessingNewInnerCallInCallCenter(newDial);
            TelephonyManager.PushCommand(newDial.RemoteAddress,
                    BeelineConnect.FindAbonentsBy(newDial.SubscriptionID).Key.UserName,
                    newDial.TrackingId);
        }

        private void eventNewOuterCall(Dial newDial)
        {
            CallController.CreateNewOuterCall(newDial);
            TelephonyManager.PushCommand(newDial.RemoteAddress,
                    BeelineConnect.FindAbonentsBy(newDial.SubscriptionID).Key.UserName,
                    newDial.TrackingId);
        }

        private void eventCallAnswer(Dial newDial)
        {
            CallController.ProcessingAnswerCall(newDial);
            TelephonyManager.PushCommand(newDial.RemoteAddress,
                BeelineConnect.FindAbonentsBy(newDial.SubscriptionID).Key.UserName,
                newDial.TrackingId);
        }

        private void eventCallReleased(Dial newDial)
        {
            CallController.ProcessingReleaseCall(newDial);
        }
        
    }
}
