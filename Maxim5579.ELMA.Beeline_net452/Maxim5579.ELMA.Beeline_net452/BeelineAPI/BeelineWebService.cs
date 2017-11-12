using System;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using EleWise.ELMA.ComponentModel;
using EleWise.ELMA.CRM.Telephony.Managers;
using EleWise.ELMA.Logging;
using EleWise.ELMA.Model.Attributes;
using EleWise.ELMA.Services.Public;
using EleWise.ELMA.Web.Service;

namespace Maxim5579.ELMA.Beeline_net452.BeelineAPI
{
    [ServiceContract(Namespace = APIRouteProvider.ApiServiceNamespaceRoot)]
    [Description("Сервис-прием событий Билайн АТС")]
    [WsdlDocumentation("Сервис-прием событий Билайн АТС")]
    public interface ITelephonyWebService
    {
        [OperationContract]
        [WebInvoke(Method ="POST",UriTemplate = "/SetEvents")]
        //[WebGet(UriTemplate = "/SetEvents")]
        //[AuthorizeOperationBehavior]
        [FaultContract(typeof(PublicServiceException))]
        [Description("Удаление объекта IExampleObject")]
        [WsdlDocumentation("Удаление объекта IExampleObject")]
        void SetEvents(string param);
    }

    /// <summary>
    /// Класс, позволяющий добавить публичный веб сервис уровня модуля
    /// </summary>
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall, MaxItemsInObjectGraph = int.MaxValue)]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    [ServiceKnownType("GetEntityKnownTypes", typeof(ServiceKnownTypeHelper))]
    [Component]
    [Uid(GuidS)]
    class BeelineWebService : ITelephonyWebService, IPublicAPIWebService
    {
        public const string GuidS = "BD997CA7-8F3A-4DA4-AF8C-C98FA8401DC7";

        public void SetEvents(string param)
        {
            TelephonyManager.TelephonyLog.Debug("Метод вызван с параметром "+param);
        }
    }
}
