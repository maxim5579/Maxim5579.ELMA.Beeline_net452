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
        public Dictionary<IUser, Abonent> AbonentsConnector;
        public TypesEvent typesEvents;

        #region События класса
        public delegate Dial CallReleased();

        public event CallReleased onReleased;

        public delegate Dial CallReleasing();

        public event CallReleasing onReleasing;

        public delegate Dial CallOriginated();

        public event CallOriginated onOriginated;

        public delegate Dial CallOriginating();

        public delegate Dial CallReceived();

        public delegate Dial CallAnswered();
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
        }

        /// <summary>
        /// Инициализация параметров класса
        /// </summary>
        public void Initialize()
        {
            List<IUser> Users = FindUsersForTelephony("Rusdecking_Телефония продажи"); //TODO Создать группу "Rusdecking_Телефония продажи"
            CreateDictionary(Users);
            AbonentsSubscription();
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
            lock (AbonentsConnector)
            {
                AbonentsConnector = null;
                AbonentsConnector = new Dictionary<IUser, Abonent>();
            }
            foreach (IUser user in users)
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
                foreach (KeyValuePair<IUser, Abonent> kvp in AbonentsConnector)
                {
                    sr.pattern = kvp.Value.extension;
                    kvp.Value.Sinfo1 = _apiConnect.Subscription(sr).Result; //TODO: Использовать async await
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
                KeyValuePair<IUser, Abonent> abonsInfo = FindAbonentsBy(subscriptionID);
                sr.pattern = abonsInfo.Value.extension;
                abonsInfo.Value.Sinfo1 = _apiConnect.Subscription(sr).Result;
            }
        }

        /// <summary>
        /// Поиск абонента телефонии по идентификатору подписки
        /// </summary>
        /// <param name="subscriptionId">Идентификатор подписки</param>
        private KeyValuePair<IUser, Abonent> FindAbonentsBy(string subscriptionId)
        {
            lock (AbonentsConnector)
            {
                foreach (KeyValuePair<IUser, Abonent> item in AbonentsConnector)
                {
                    if (item.Value.Sinfo1.subscriptionId == subscriptionId) return item;
                }
            }
            return default(KeyValuePair<IUser, Abonent>);
        }

        #endregion

        /// <summary>
        /// Обработка полученных событий телефонии
        /// </summary>
        /// <param name="XMLString">XML событие в виде строки string</param>
        public void EventProcessing(string XMLString)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(BaseEvent));
            BaseEvent XMLObject = serializer.Deserialize(XmlReader.Create(new StringReader(XMLString))) as BaseEvent;
            if (XMLObject == null) throw new ArgumentNullException(nameof(XMLObject));//TODO Определиться с этим исключением (может не нужно);

            SubscriptionEvent se = XMLObject as SubscriptionEvent;
            if (se == null) throw new ArgumentNullException(nameof(se)); //TODO Не забыть про исключение
            EventData eventData = se.eventData;
            string NameEvent=eventData.ToString().Remove(0, eventData.ToString().LastIndexOf(".") + 1);

            if (typesEvents.CallTypesSet.Contains(NameEvent))
            {
                //TODO Обработка события. ...RemoteParty.callType=Group - внутренние звоннки (не учитывать). Учитывать только Network
                switch (NameEvent)
                {
                    case "SubscriptionTerminatedEvent":
                        AbonentsSubscription(se.subscriptionId);
                        break;
                    case "CallReleasedEvent": //Звонок завершен
                        CallReleasedEvent CRlE = eventData as CallReleasedEvent;
                        if (CRlE != null && CRlE.call.remoteParty.callType == CallType.Network)
                        {
                            //TODO Реализовать "Звонок завершен"
                        }
                        break;
                    case "CallReleasingEvent": //
                        CallReleasingEvent CRiE = eventData as CallReleasingEvent;
                        if (CRiE != null && CRiE.call.remoteParty.callType == CallType.Network)
                        {
                            
                        }
                        break;
                    case "CallOriginatedEvent": //Исходящий вызов. Номер набран на телефоне. Ждет ответа, слушает гудки.
                        CallOriginatedEvent COE = eventData as CallOriginatedEvent;
                        if (COE != null && COE.call.remoteParty.callType == CallType.Network)
                        {
                            
                        }//TODO Реализовать "Исходящий вызов"
                        break;
                    case "CallOriginatingEvent": //Исходящий. Номер набран программой. Идет вызов. State "Alerting" (когда из программы начинаешь звонить себе)
                        CallOriginatingEvent COrE = eventData as CallOriginatingEvent;
                        if (COrE != null)
                        {
                            //TODO Реализовать "Исходящий вызов"
                        }
                        break;
                    case "CallReceivedEvent": //Входящий вызов до поднятия трубки
                        CallReceivedEvent CRE = eventData as CallReceivedEvent;
                        if (CRE != null && CRE.call.remoteParty.callType == CallType.Network) //Если это не внутренние звонки
                        {
                            //TODO Реализация события входящего звонка
                        }
                        break;
                    case "CallAnsweredEvent": //Поднятие трубки, ответ на вызов (исх и вх)
                        CallAnsweredEvent CAE = eventData as CallAnsweredEvent;
                        if (CAE != null && CAE.call.remoteParty.callType==CallType.Network)
                        {
                            //TODO Реализовать "Поднятие трубки"
                        }
                        break;
                }
            }
            //TODO Если событие не из списка что делаем? Пока ничего
        }

        
    }


}