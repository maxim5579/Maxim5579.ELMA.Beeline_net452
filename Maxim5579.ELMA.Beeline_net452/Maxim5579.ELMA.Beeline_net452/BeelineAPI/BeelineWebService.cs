using System;
using System.IO;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using EleWise.ELMA.ComponentModel;
using EleWise.ELMA.CRM.Telephony.Managers;
using EleWise.ELMA.Logging;
using EleWise.ELMA.Model.Attributes;
using EleWise.ELMA.Services.Public;
using EleWise.ELMA.Web.Service;
using Maxim5579.ELMA.Beeline_net452.Managers;

namespace Maxim5579.ELMA.Beeline_net452.BeelineAPI
{
    [ServiceContract(Namespace = APIRouteProvider.ApiServiceNamespaceRoot)]
    [Description("Сервис-прием событий Билайн АТС")]
    [WsdlDocumentation("Сервис-прием событий Билайн АТС")]
    public interface ITelephonyWebService
    {
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/SetEvents")]
        //[WebGet(UriTemplate = "/SetEvents")]
        //[AuthorizeOperationBehavior]
        //[FaultContract(typeof(PublicServiceException))] //*
        [Description("Информация о событиях телефонии")]
        [WsdlDocumentation("Информация о событиях телефонии")]
        void SetEvents(Stream param);
    }

    /// <summary>
    /// Класс, позволяющий добавить публичный веб сервис уровня модуля
    /// </summary>
    //[ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall, MaxItemsInObjectGraph = int.MaxValue)] //*
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    //[ServiceKnownType("GetEntityKnownTypes", typeof(ServiceKnownTypeHelper))] //*
    [Component]
    [Uid(GuidS)]
    class BeelineWebService : ITelephonyWebService, IPublicAPIWebService
    {
        public const string GuidS = "BD997CA7-8F3A-4DA4-AF8C-C98FA8401DC7";

        static private object obj = new object();
        public void SetEvents(Stream param)
        {
            StreamReader sr = new StreamReader(param);
            string res = sr.ReadToEnd();
            lock(obj)
                BeelineManager.Instance.BeelineConnect.EventProcessing(res);
            //TelephonyManager.TelephonyLog.Debug("Метод вызван с параметром "+res);
        }
    }
}
