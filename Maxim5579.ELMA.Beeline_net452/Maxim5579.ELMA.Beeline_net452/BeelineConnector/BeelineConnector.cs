using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using EleWise.ELMA.Security.Models;
using EleWise.ELMA.Security.Managers;
using EleWise.ELMA.API;
using EleWise.ELMA.CRM.Telephony.Managers;
using EleWise.ELMA.Logging;
using EleWise.ELMA.Security.Services;
using EleWise.ELMA.Services;
using Maxim5579.ELMA.Beeline_net452.BeelineAPI;
using Maxim5579.ELMA.Beeline_net452.Schema;

namespace Maxim5579.ELMA.Beeline_net452
{
    /// <summary>
    /// Класс управления общением системы с телефонией Билайн
    /// </summary>
    public class BeelineConnector
    {
        private readonly string _subscriptionUrl;
        private readonly BeeConnect _apiConnect;
        public Dictionary<User, Abonent> AbonentsConnector;
        public Dials ActualDials;
        public TypesEvent typesEvents;
        private FileLogsWriter LogEvents;

        //private TelephonyManager TelephonyManager { get; } = TelephonyManager.Instance;

        #region События класса
        public delegate void NewInnerCall(Dial newDial);

        public event NewInnerCall onNewInnerCall;

        public delegate void NewOuterCall(Dial newDial);

        public event NewOuterCall onNewOuterCall;

        public delegate void CallAnswered(Dial newDial);

        public event CallAnswered onCallAnswered;

        public delegate void CallEnding(Dial newDial);

        public event CallEnding onCallEnding;

        public delegate void CallCenterInnerCall(Dial newDial);

        public event CallCenterInnerCall onCallCenterInnerCall;
        #endregion

        /// <summary>
        /// Конструктор класса
        /// </summary>
        /// <param name="apiConnect"></param>
        /// <param name="subscriptionUrl">Урл подписки на события</param>
        public BeelineConnector(BeeConnect apiConnect, string subscriptionUrl)
        {
            this._apiConnect = apiConnect;
            this._subscriptionUrl = subscriptionUrl;
            typesEvents = new TypesEvent();
            ActualDials = new Dials();
            LogEvents = new FileLogsWriter();
        }

        /// <summary>
        /// Инициализация параметров класса
        /// </summary>
        public void Initialize()
        {
            List<IUser> Users = FindUsersForTelephony("Rusdecking_Телефония продажи"); //TODO Название группы. Надо придумать
            //TelephonyManager.TelephonyLog.Debug(String.Format("Count Users = {0}/ UserName = {1}",Users.Count.ToString(),Users[0].UserName));
            CreateDictionary(Users);
            
            AbonentsSubscription();
            //TelephonyManager.TelephonyLog.Debug("All abonents subscription is OK");
        }

        /// <summary>
        /// Деинициализация параметров класса
        /// </summary>
        public void Deinitialize()
        {
            throw new System.NotImplementedException();
        }

        #region Приватные методы класса
        /// <summary>
        /// Поиск всех пользователей системы, с ролью использования телефонии
        /// </summary>
        /// <param name="NameGroup">Название группы пользователей телефонии</param>
        /// <returns>Список пользователей телефонии</returns>
        private List<IUser> FindUsersForTelephony(string NameGroup)
        {
            List<IUser> UserList = new List<IUser>();
            IUserGroupFilter filter = Locator.GetServiceNotNull<IUserGroupFilter>();
            filter.ShowOnlyGroups = true;
            filter.SearchString = NameGroup;
            var grp = UserGroupManager.Instance.Find(filter, null);
            foreach (var item in grp.First().Users.ToList())
            {
                var user = (IUser) item;
                UserList.Add(user);
            }
            return UserList;
        }

        /// <summary>
        /// Сопоставление пользователей системы, абонентам телефонии, с формированием словаря соответствий
        /// </summary>
        /// <param name="users">Список пользователей системы, с ролью работы с телефонией</param>
        private void CreateDictionary(List<IUser> users)
        {
            List<Abonent> abonents = _apiConnect.GetAllAbonents().Result;
            //TelephonyManager.TelephonyLog.Debug(String.Format("Count Abonents = {0}", abonents.Count.ToString()));
            //lock (AbonentsConnector)
            //{
                //AbonentsConnector = null;
                AbonentsConnector = new Dictionary<User, Abonent>();
            //}
            //TelephonyManager.TelephonyLog.Debug(String.Format("AbonentsConnector = {0}", AbonentsConnector.GetType().ToString()));
            foreach (User user in users)
            {
                //DEBUG: сравнение пользователей ELMA  и абонентов Билайн проходит по полю Рабочий телефон(ELMA) и Добавочный номер (Билайн)
                Abonent ab = abonents.Find(x => x.extension == user.WorkPhone);
                if (ab != default(Abonent))
                {
                    lock (AbonentsConnector)
                    {
                        AbonentsConnector.Add(user, ab);
                    }
                }
            }
            //TelephonyManager.TelephonyLog.Debug(String.Format("Count Abonents = {0}", AbonentsConnector.Count.ToString()));
        }

        /// <summary>
        /// Подписка всех пользователей в словаре на события телефонии
        /// </summary>
        private void AbonentsSubscription()
        {
            SubscriptionRequest sr = new SubscriptionRequest();
            sr.subscriptionType = SubscriptionType.BASIC_CALL;
            sr.expires = 36000;
            sr.url = _subscriptionUrl;
            lock (AbonentsConnector)
            {
                foreach (KeyValuePair<User, Abonent> kvp in AbonentsConnector)
                {
                    sr.pattern = kvp.Value.extension;
                    kvp.Value.Sinfo = _apiConnect.Subscription(sr).Result; //TODO: Использовать async await
                }
            }
        }

        /// <summary>
        /// Переподписка одного абонента
        /// </summary>
        /// <param name="subscriptionID">Идентификатор старой подписки</param>
        private void AbonentsSubscription(string subscriptionID)
        {
            SubscriptionRequest sr = new SubscriptionRequest();
            sr.subscriptionType = SubscriptionType.BASIC_CALL;
            sr.expires = 36000;
            sr.url = _subscriptionUrl;
            lock (AbonentsConnector)
            {
                KeyValuePair<User, Abonent> abonsInfo = FindAbonentsBy(subscriptionID);
                sr.pattern = abonsInfo.Value.extension;
                abonsInfo.Value.Sinfo = _apiConnect.Subscription(sr).Result;
            }
        }

        /// <summary>
        /// Поиск абонента телефонии по идентификатору подписки
        /// </summary>
        /// <param name="subscriptionId">Идентификатор подписки</param>
        public KeyValuePair<User, Abonent> FindAbonentsBy(string subscriptionId)
        {
            lock (AbonentsConnector)
            {
                return AbonentsConnector.LastOrDefault(s => s.Value.Sinfo.subscriptionId == subscriptionId);
                //foreach (KeyValuePair<User, Abonent> item in AbonentsConnector)
                //{
                //    if (item.Value.Sinfo.subscriptionId == subscriptionId) return item;
                //}
            }
            //return default(KeyValuePair<User, Abonent>);
        }

        #endregion

        /// <summary>
        /// Обработка полученных событий телефонии
        /// </summary>
        /// <param name="XMLString">XML событие в виде строки string</param>
        public void EventProcessing(string XMLString)
        {
            //TODO логирование всех событий телефонии
            LogEvents.addText(XMLString);
            ////////////////////////////////////////

            XmlSerializer serializer = new XmlSerializer(typeof(BaseEvent));
            BaseEvent XMLObject = serializer.Deserialize(XmlReader.Create(new StringReader(XMLString))) as BaseEvent;
            if (XMLObject == null)
            {
                TelephonyManager.TelephonyLog.Error("Десериализация XML строки вернула NULL");
                return;
            }

            SubscriptionEvent se = XMLObject as SubscriptionEvent;
            if (se == null) {
                TelephonyManager.TelephonyLog.Error("Событие телефонии не является типом SubscriptionEvent");
                return;
            }
            EventData eventData = se.eventData;
            string NameEvent=eventData.ToString().Remove(0, eventData.ToString().LastIndexOf(".") + 1);

            if (typesEvents.CallTypesSet.Contains(NameEvent))
            {
                switch (NameEvent)
                {
                    case "SubscriptionTerminatedEvent":
                        User us = FindAbonentsBy(se.subscriptionId).Key;
                        TelephonyManager.TelephonyLog.Debug("Событие сброс подписки для "+us.UserName+" SubscriptionTerminatedEvent");
                        AbonentsSubscription(se.subscriptionId);
                        TelephonyManager.TelephonyLog.Debug("Событие переподписки для " + us.UserName + " выполнено");
                        break;
                    case "CallReleasedEvent": //Звонок завершен TODO Реализовать формирование ссылки на запись разговора
                        TelephonyManager.TelephonyLog.Debug("Событие Звонок завершен CallReleasedEvent");
                        CallReleasedEvent CRlE = eventData as CallReleasedEvent;
                        if (CRlE != null && CRlE.call.remoteParty.callType == CallType.Network)
                        {
                            Dial newDial = ActualDials.findBySubscriptionID(se.subscriptionId);
                            if (newDial != default(Dial))
                            {
                                newDial.State = DialState.CallEnding;
                                lock (ActualDials)
                                {
                                    ActualDials.RemoveAt(ActualDials.FindIndexBySubscriptionID(se.subscriptionId));
                                }
                                onCallEnding?.Invoke(newDial);
                            }
                        }
                        break;
                    case "CallReleasingEvent": //
                        CallReleasingEvent CRiE = eventData as CallReleasingEvent;
                        if (CRiE != null && CRiE.call.remoteParty.callType == CallType.Network)
                        {
                            
                        }
                        break;
                    case "CallOriginatedEvent": //Исходящий вызов. Номер набран на телефоне. Ждет ответа, слушает гудки.
                        TelephonyManager.TelephonyLog.Debug("Событие Исходящий вызов CallOriginatedEvent");
                        CallOriginatedEvent COE = eventData as CallOriginatedEvent;
                        if (COE != null && COE.call.remoteParty.callType == CallType.Network)
                        {
                            if (COE != null && COE.call.remoteParty.callType == CallType.Network) //Если это не внутренние звонки
                            {
                                Dial newDial = new Dial();
                                Call tmpCall = COE.call;
                                newDial.SubscriptionID = se.subscriptionId;
                                newDial.TrackingId = tmpCall.extTrackingId;
                                newDial.RemoteAddress = tmpCall.remoteParty.address.Value.Remove(0, 4);
                                newDial.StartTime = tmpCall.startTime;
                                newDial.EndTime = 0;
                                newDial.State = DialState.OuterCall_originated;
                                lock (ActualDials)
                                {
                                    ActualDials.Add(newDial);
                                }
                                onNewOuterCall?.Invoke(newDial);
                            }
                        }
                        break;
                    case "CallOriginatingEvent": //Исходящий. Номер набран программой. Идет вызов. State "Alerting" (когда из программы начинаешь звонить себе)
                        CallOriginatingEvent COrE = eventData as CallOriginatingEvent;
                        if (COrE != null)
                        {
                            //TODO Реализовать "Исходящий вызов"
                        }
                        break;
                    case "CallReceivedEvent": //Входящий вызов до поднятия трубки
                        TelephonyManager.TelephonyLog.Debug("Событие Входящий вызов CallReceivedEvent");
                        CallReceivedEvent CRE = eventData as CallReceivedEvent;
                        if (CRE != null && CRE.call.remoteParty.callType == CallType.Network) //Если это не внутренние звонки
                        {
                            Dial newDial = new Dial();
                            Call tmpCall = CRE.call;
                            newDial.CallOutEvent = CRE.call;
                            newDial.SubscriptionID = se.subscriptionId;
                            newDial.TrackingId = tmpCall.extTrackingId;
                            newDial.RemoteAddress = tmpCall.remoteParty.address.Value.Remove(0, 4);
                            newDial.StartTime = tmpCall.startTime;
                            newDial.EndTime = 0;
                            newDial.State = DialState.InnerCall_waiting;
                            lock (ActualDials)
                            {
                                ActualDials.Add(newDial);
                            }
                           
                            if (CRE.call.acdCallInfo != null)
                            {
                                newDial.CallOutEvent = CRE.call;
                                onCallCenterInnerCall?.Invoke(newDial);
                            }
                            else
                                onNewInnerCall?.Invoke(newDial);
                            /////////////////////////////////////////////////
                            //if (CRE.call.remoteParty.name != "CallCenter 4007")
                            //    onNewInnerCall?.Invoke(newDial);
                            //else
                            //    onCallCenterInnerCall?.Invoke(newDial);
                        }
                        break;
                    case "CallAnsweredEvent": //Поднятие трубки, ответ на вызов (исх и вх)
                        TelephonyManager.TelephonyLog.Debug("Событие Поднятие трубки CallAnsweredEvent");
                        CallAnsweredEvent CAE = eventData as CallAnsweredEvent;
                        if (CAE != null && CAE.call.remoteParty.callType==CallType.Network)
                        {
                            Dial newDial = ActualDials.findBySubscriptionID(se.subscriptionId);
                            if (newDial != default(Dial))
                            {
                                newDial.State = DialState.CallAnswered;
                                lock (ActualDials)
                                {
                                    ActualDials[ActualDials.FindIndexBySubscriptionID(se.subscriptionId)] = newDial;
                                }
                                onCallAnswered?.Invoke(newDial);
                            }
                        }
                        break;
                }
            }
            //Если событие не из списка что делаем? Пока ничего
        }

        
    }
}