using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using EleWise.ELMA.CRM.Telephony.Managers;
using Newtonsoft.Json;

namespace Maxim5579.ELMA.Beeline_net452.BeelineAPI
{
    /// <summary>
    /// Класс реализующий API общения с Билайн АТС
    /// </summary>
    public class BeeConnect
    {
        private readonly string _host;
        private readonly string _token;
        private readonly BeelineAPI _api;
        public bool init;

        private TelephonyManager TelephonyManager { get; } = TelephonyManager.Instance;

        /// <summary>
        /// Инициализация нового класса
        /// </summary>
        /// <param name="host">Адрес хоста API (без последнего знака "/")</param>
        /// <param name="token">Токен приложения</param>
        public BeeConnect(string host, string token)
        {
            _host = host;
            _token = token;
            _api = new BeelineAPI(host);
            init = true;
        }

        /// <summary>
        /// Совершает звонок от имени абонента
        /// </summary>
        /// <param name="idAbonent"></param>
        /// <param name="numContact"></param>
        /// <returns></returns>
        public async Task<OutCallInfo> MakeCall(string idAbonent, string numContact)
        {
            //Словарь для передачи параметров в POST
            var values = new Dictionary<string, string>();
            values.Add("phoneNumber", numContact);
            var content = new FormUrlEncodedContent(values);

            HttpClient httpClient = new HttpClient();
            var headers = httpClient.DefaultRequestHeaders;
            headers.Add("X-MPBX-API-AUTH-TOKEN", _token);

            Uri requestUri = new Uri(_api.call(idAbonent).uri);
            HttpResponseMessage httpResponse = new HttpResponseMessage();
            try
            {
                httpResponse = await httpClient.PostAsync(requestUri, content);
            }
            catch (OperationCanceledException)
            { }
            httpClient.Dispose();
            return new OutCallInfo(await httpResponse.Content.ReadAsStringAsync(), httpResponse.StatusCode);
        }

        /// <summary>
        /// Перечень всех зарегистрированных абонентов системы
        /// </summary>
        /// <returns>Список абонентов системы</returns>
        public async Task<List<Abonent>> GetAllAbonents()
        {
            HttpClient httpClient = new HttpClient();

            var headers = httpClient.DefaultRequestHeaders;
            headers.Add("X-MPBX-API-AUTH-TOKEN", _token);
            Uri requestUri = new Uri(_api.allAbonents().uri);
            HttpResponseMessage httpResponse = new HttpResponseMessage();
            string httpResponseBody = "";
            List<Abonent> abonents;
            try
            {
                // отправляем запрос
                httpResponse = await httpClient.GetAsync(requestUri);
                httpResponse.EnsureSuccessStatusCode();
                httpResponseBody = await httpResponse.Content.ReadAsStringAsync();
                abonents = JsonConvert.DeserializeObject<List<Abonent>>(httpResponseBody);
            }
            catch (Exception ex)
            {
                abonents = null;
            }
            httpClient.Dispose();
            return abonents;
        }

        /// <summary>
        /// Подписка на события телефонии
        /// </summary>
        /// <param name="req">Параметры подписки</param>
        /// <returns>Информация о подписке</returns>
        public async Task<SubscriptionResult> Subscription(SubscriptionRequest req)
        {
            string values = JsonConvert.SerializeObject(req);
            var httpContent = new StringContent(values, Encoding.UTF8, "application/json");

            HttpClient httpClient = new HttpClient();
            var headers = httpClient.DefaultRequestHeaders;
            headers.Add("X-MPBX-API-AUTH-TOKEN", _token);
            Uri requestUri = new Uri(_api.subscript().uri);
            HttpResponseMessage httpResponse = new HttpResponseMessage();

            try
            {
                httpResponse = await httpClient.PutAsync(requestUri, httpContent);
            }
            catch (OperationCanceledException)
            { }
            httpClient.Dispose();
            string str = await httpResponse.Content.ReadAsStringAsync();
            SubscriptionResult SR = JsonConvert.DeserializeObject<SubscriptionResult>(str);

            return SR;
        }

        /// <summary>
        /// Получение информации о подписке
        /// </summary>
        /// <param name="subscriptId">ИД подписки</param>
        /// <returns></returns>
        public async Task<SubscriptionInfo> SubscriptionInformation(string subscriptId)
        {
            HttpClient httpClient = new HttpClient();

            var headers = httpClient.DefaultRequestHeaders;
            headers.Add("X-MPBX-API-AUTH-TOKEN", _token);
            Uri requestUri = new Uri(_api.getInfoSubscript(subscriptId).uri);
            HttpResponseMessage httpResponse = new HttpResponseMessage();
            string httpResponseBody = "";
            try
            {
                // отправляем запрос
                httpResponse = await httpClient.GetAsync(requestUri);
                httpResponse.EnsureSuccessStatusCode();
                httpResponseBody = await httpResponse.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                httpResponseBody = "Error: " + ex.HResult.ToString("X") + " Message: " + ex.Message;
            }
            SubscriptionInfo subscription = JsonConvert.DeserializeObject<SubscriptionInfo>(httpResponseBody);
            httpClient.Dispose();
            return subscription;
        }

        /// <summary>
        /// Удаление подписки на сервере
        /// </summary>
        /// <param name="subscriptId"></param>
        public async void SubscriptionDelete(string subscriptId)
        {
            HttpClient httpClient = new HttpClient();

            var headers = httpClient.DefaultRequestHeaders;
            headers.Add("X-MPBX-API-AUTH-TOKEN", _token);
            Uri requestUri = new Uri(_api.closeSubscript(subscriptId).uri);
            HttpResponseMessage httpResponse = new HttpResponseMessage();
            string httpResponseBody = "";
            try
            {
                // отправляем запрос
                httpResponse = await httpClient.DeleteAsync(requestUri);
                httpResponse.EnsureSuccessStatusCode();
                //httpResponseBody = await httpResponse.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                httpResponseBody = "Error: " + ex.HResult.ToString("X") + " Message: " + ex.Message;
            }
        }
    }

    /// <summary>
    /// Класс формирование URI запросов АПИ Билайна
    /// </summary>
    public class BeelineAPI
    {
        private readonly string host;
        public BeelineAPI(string host)
        {
            this.host = host;
        }

        /// <summary>
        /// Возвращает список всех абонентов
        /// </summary>
        /// <returns></returns>
        public RestAPIBeeline allAbonents()
        {
            return new RestAPIBeeline(Methods.Get, host + "/abonents");
        }

        /// <summary>
        /// Совершает звонок от имени абонента
        /// </summary>
        /// <param name="idAbonent">Идентификатор, мобильный или добавочный номер абонента</param>
        /// <param name="numContact">Номер телефона - 10 цифр</param>
        /// <returns></returns>
        public RestAPIBeeline call(string idAbonent)
        {
            return new RestAPIBeeline(Methods.Post, host + String.Format("/abonents/{0}/call", idAbonent));
        }

        /// <summary>
        /// Возвращает файл записи разговора
        /// </summary>
        /// <param name="recordId">Идентификатор записи разговора</param>
        /// <returns></returns>
        public RestAPIBeeline recordDownload(string recordId)
        {
            return new RestAPIBeeline(Methods.Get, host + String.Format("/v2/records/{0}/download", recordId));
        }

        /// <summary>
        /// Формирует подписку на Xsi-Events
        /// </summary>
        /// <returns></returns>
        public RestAPIBeeline subscript()
        {
            return new RestAPIBeeline(Methods.Put, host + "/subscription");
        }

        /// <summary>
        /// Возвращает информацию о подписке на Xsi-Events
        /// </summary>
        /// <param name="idSubscript">Идентификатор подписки</param>
        /// <returns></returns>
        public RestAPIBeeline getInfoSubscript(string idSubscript)
        {
            return new RestAPIBeeline(Methods.Get, host + String.Format("/subscription?subscriptionId={0}", idSubscript));
        }

        /// <summary>
        /// Отключает подписку на Xsi-Events
        /// </summary>
        /// <param name="idSubscript">Идентификатор подписки</param>
        /// <returns></returns>
        public RestAPIBeeline closeSubscript(string idSubscript)
        {
            return new RestAPIBeeline(Methods.Delete, host + String.Format("/subscription?subscriptionId={0}", idSubscript));
        }
    }

    #region Вспомогательные классы
    /// <summary>
    /// Структура REST API
    /// </summary>
    public class RestAPIBeeline
    {
        public string method { get; set; }
        public string uri { get; set; }

        /// <summary>
        /// Конструктор класса
        /// </summary>
        /// <param name="method">Метод HTTP запроса</param>
        /// <param name="URI">URI запроса</param>
        public RestAPIBeeline(string method, string URI)
        {
            this.method = method;
            this.uri = URI;
        }
    }

    /// <summary>
    /// Методы HTTP запросов
    /// </summary>
    public static class Methods
    {
        //private static string _get;
        //private static string _put;
        //private static string _delete;
        //private static string _post;

        public static string Get { get { return "GET"; } }
        public static string Put { get { return "PUT"; } }
        public static string Delete { get { return "DELETE"; } }
        public static string Post { get { return "POST"; } }

    }

    /// <summary>
    /// Структура с информацией о вызываемом абоненте
    /// </summary>
    public class OutCallInfo
    {
        private Task<string> task;

        public OutCallInfo(Task<string> task, HttpStatusCode statusCode)
        {
            this.task = task;
            this.statusCode = statusCode;
        }

        public OutCallInfo(string callerId, HttpStatusCode code)
        {
            callerID = callerId;
            statusCode = code;
        }
        public string callerID { get; set; }
        public HttpStatusCode statusCode { get; set; }
    }

    /// <summary>
    /// Класс описания внутренних абонентов Билайн
    /// </summary>
    public class Abonent
    {
        public string userID { get; set; }
        public string phone { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string email { get; set; }
        public string department { get; set; }
        public string extension { get; set; }
    }

    /// <summary>
    /// Запрос для подписки на события
    /// </summary>
    public class SubscriptionRequest
    {
        public string pattern { get; set; }
        public int expires { get; set; }
        public string subscriptionType { get; set; }
        public string url { get; set; }
    }

    /// <summary>
    /// Перечень типов подписки
    /// </summary>
    public static class SubscriptionType
    {
        public static string BASIC_CALL { get { return "BASIC_CALL"; } }
        public static string ADVANCED_CALL { get { return "ADVANCED_CALL"; } }
    }

    /// <summary>
    /// Результат подписки
    /// </summary>
    public class SubscriptionResult
    {
        public string subscriptionId { get; set; }
        public string expires { get; set; }
    }

    /// <summary>
    /// Информация о подписке
    /// </summary>
    public class SubscriptionInfo
    {
        public string subscriptionId { get; set; }
        public string targetType { get; set; }
        public string targetId { get; set; }
        public string subscriptionType { get; set; }
        public string expires { get; set; }
        public string url { get; set; }
    }
    #endregion
}
