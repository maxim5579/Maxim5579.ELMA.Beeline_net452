using System;
using EleWise.ELMA.Model.Attributes;
using EleWise.ELMA.Runtime.Settings;
using EleWise.ELMA;
using EleWise.ELMA.CRM.Telephony.Managers;
using EleWise.ELMA.Logging;

namespace Maxim5579.ELMA.Beeline_net452
{
    public class BeelineSettings:GlobalSettingsBase
    {
        public BeelineSettings()
        {
            BeelineHost = "https://cloudpbx.beeline.ru/apis/portal";
            //BeelineToken = " 7a0cc164-8dcf-4b1f-8091-c1a412c18c06";
            BeelineToken = "3fce0597-b0f4-43f1-ac38-70735ec26aea";
            //TelephonyManager.TelephonyLog.Debug(String.Format("Конструктор настроек. Токен - {0}", BeelineToken));
        }

        private string token;
        [DisplayName(typeof(@__Resources_BeelineSettings), "BeelineHost")]
        [Required(true)] //Обязательность заполнения поля
        public string BeelineHost { get; set; }

        [DisplayName(typeof(@__Resources_BeelineSettings), "BeelineToken")]
        [Required(true)] //Обязательность заполнения поля
        public string BeelineToken {
            get { return token; }
            set
            {
                token = value;
                //TelephonyManager.TelephonyLog.Debug(String.Format("Новый Токен - {0}", BeelineToken));
            } }

        #region Resources
        internal class @__Resources_BeelineSettings
        {
            public static string BeelineHost { get { return SR.T("Адрес сервера Билайн АТС"); } }
            public static string BeelineToken { get { return SR.T("Токен сервера телефонии для приложения"); } }
        }
        #endregion
    }
}
