using System;
using System.Collections.Generic;
using System.Linq;
using EleWise.ELMA.CRM.Models;
using EleWise.ELMA.Model.Services;
using EleWise.ELMA.Services;
using EleWise.ELMA.Model.Managers;
using EleWise.ELMA.Security.Models;
using Maxim5579.ELMA.Beeline_net452.BeelineAPI;
using Maxim5579.ELMA.Beeline_net452.Managers;
using EleWise.ELMA.Common.Models;
using EleWise.ELMA.Messages.Models;
using EleWise.ELMA.Scheduling;
using EleWise.ELMA.Tasks.Models;
using EleWise.ELMA.CRM.Telephony.Managers;
using EleWise.ELMA.Logging;

namespace Maxim5579.ELMA.Beeline_net452
{
    public class CallEventsController
    {
        //private RelationshipCallManager CallManager;
        public User sysAdmin, system;
        private TelephonyManager TelephonyManager { get; } = TelephonyManager.Instance;

        //private object dbLock;
        
        public CallEventsController()
        {
            sysAdmin = EntityManager<User>.Instance.Load(Guid.Parse("c218a7eb-1e45-4056-bf84-c6818a1dabd6"));
            system = EntityManager<User>.Instance.Load(Guid.Parse("f0e949bf-4da4-4c02-b04c-ca61964b4fe8"));
            //dbLock = new object();
        }


        /// <summary>
        /// Создать новый входящий звонок
        /// </summary>
        /// <param name="newDial">Параметры звонка</param>
        /// <returns>true - если успешно, false - неудача</returns>
        public bool CreateNewInnerCall(Dial newDial)
        {
            try
            {
                User Abonent = BeelineManager.Instance.BeelineConnect.FindAbonentsBy(newDial.SubscriptionID).Key;
                TelephonyManager.TelephonyLog.Debug("Начало обработки события для " + Abonent.UserName + " CallEventsController.CreateNewInnerCall");
                RelationshipCall call = CreateRelationshipCall(newDial);
                call.Theme = "Входящий звонок от " + newDial.RemoteAddress;
                call.Type = EleWise.ELMA.CRM.Enums.RelationshipCallType.Input;
                call.KollCentr = false;
                call.Save();
                TelephonyManager.TelephonyLog.Debug("Обработано успешно для " + Abonent.UserName + " CallEventsController.CreateNewInnerCall");
            }
            catch(Exception ex)
            {
                SendMessage(sysAdmin, "error in CreateNewInnerCall", string.Format("{0} || {1}", ex.Message, ex.TargetSite));
                return false;
            }
            return true;
        }

        /// <summary>
        /// Обработка события входящего звонка на колл центр
        /// </summary>
        /// <param name="newDial">Структура информация о звонке</param>
        public void ProcessingNewInnerCallInCallCenter(Dial newDial)
        {
            try
            {
                User Abonent = BeelineManager.Instance.BeelineConnect.FindAbonentsBy(newDial.SubscriptionID).Key;
                TelephonyManager.TelephonyLog.Debug("Входящий звонок на Колл центр "+Abonent.UserName+ "; CallEventsController.ProcessingNewInnerCallInCallCenter");
                RelationshipCall call;
                //lock (dbLock)
                    call = FindLastRelationsByCallID(newDial.TrackingId);
                if (call != null)
                {
                    call.RelationshipUsers.Add(CreateParticipant(Abonent));
                }
                else
                {
                    call = CreateRelationshipCall(newDial);
                    call.Theme = string.Format("Входящий звонок '{0}' от {1}", newDial.CallOutEvent.acdCallInfo.acdName, newDial.RemoteAddress);
                    call.NazvanieKollCentra = newDial.CallOutEvent.acdCallInfo.acdName;
                    call.Type = EleWise.ELMA.CRM.Enums.RelationshipCallType.Input;
                    call.KollCentr = true;
                }
                call.Save();
                TelephonyManager.TelephonyLog.Debug("Отработано успешно для " + Abonent.UserName + " CallEventsController.ProcessingNewInnerCallInCallCenter");
            }
            catch (Exception ex)
            {
                SendMessage(sysAdmin, "error in ProcessingNewInnerCallInCallCenter", string.Format("{0} || {1}", ex.Message, ex.TargetSite));
            }
        }

        /// <summary>
        /// Создать новый исходящий звонок
        /// </summary>
        /// <param name="newDial">Параметры звонка</param>
        /// <returns>true - если успешно, false - неудача</returns>
        public bool CreateNewOuterCall(Dial newDial)
        {
            try
            {
                User Abonent = BeelineManager.Instance.BeelineConnect.FindAbonentsBy(newDial.SubscriptionID).Key;
                TelephonyManager.TelephonyLog.Debug("Начало исходящего звонка для " + Abonent.UserName + " CallEventsController.CreateNewOuterCall");
                RelationshipCall call;
                call = CreateRelationshipCall(newDial);
                call.Theme = string.Format("Исходящий звонок от {0} на номер {1}", call.CreationAuthor.FullName, newDial.RemoteAddress);
                call.Type = EleWise.ELMA.CRM.Enums.RelationshipCallType.Output;
                call.Lead.Responsible = Abonent;
                call.Lead.Status = EleWise.ELMA.CRM.Enums.LeadStatus.InHand;
                call.Lead.Save();
                call.KollCentr = false;
                call.Save();
                TelephonyManager.TelephonyLog.Debug("Исходящий вызов выполнен для " + Abonent.UserName + " CallEventsController.CreateNewOuterCall");
            }
            catch(Exception ex)
            {
                SendMessage(sysAdmin, "error in CreateNewOuterCall", string.Format("{0} || {1}", ex.Message, ex.TargetSite));
                return false;
            }
            return true;
        }

        /// <summary>
        /// Обработка события начала разговора
        /// </summary>
        /// <param name="newDial"></param>
        public void ProcessingAnswerCall(Dial newDial)
        {
            try
            {
                User Abonent = BeelineManager.Instance.BeelineConnect.FindAbonentsBy(newDial.SubscriptionID).Key;
                TelephonyManager.TelephonyLog.Debug("Начало разговора для "+Abonent.UserName+" CallEventsController.ProcessingAnswerCall");
                RelationshipCall call = FindLastRelationsByCallID(newDial.TrackingId);
                if (call == null)
                {
                    call = CreateRelationshipCall(newDial);
                    call.Theme = "Входящий звонок от " + newDial.RemoteAddress;
                    call.Type = EleWise.ELMA.CRM.Enums.RelationshipCallType.Input;
                    call.KollCentr = false;
                }
                call.StatusRazgovora = EleWise.ELMA.ConfigurationModel.SostoyanieVyzova.RazgovorNachat;
                
                if (call.Lead != null && call.Lead.Status != EleWise.ELMA.CRM.Enums.LeadStatus.Qualified)
                {
                    call.Lead.Responsible = Abonent;
                    call.Lead.Status = EleWise.ELMA.CRM.Enums.LeadStatus.InHand;
                    call.Lead.Save();
                }
                call.Save();
                TelephonyManager.TelephonyLog.Debug("Начало разговора выполнено для " + Abonent.UserName + " CallEventsController.ProcessingAnswerCall");
            }
            catch(Exception ex)
            {
                SendMessage(sysAdmin, "error in ProcessingAnswerCall", string.Format("{0} || {1}", ex.Message, ex.TargetSite));
            }
        }

        /// <summary>
        /// Обработка события окончания разговора
        /// </summary>
        /// <param name="newDial"></param>
        public void ProcessingReleaseCall(Dial newDial)
        {
            try
            {
                User Abonent = BeelineManager.Instance.BeelineConnect.FindAbonentsBy(newDial.SubscriptionID)
                    .Key;
                TelephonyManager.TelephonyLog.Debug("Окончание разговора для "+Abonent.UserName+" CallEventsController.ProcessingReleaseCall");
                RelationshipCall call = FindLastRelationsByCallID(newDial.TrackingId);
                if (call == null)
                    return;
                if (call.StatusRazgovora == EleWise.ELMA.ConfigurationModel.SostoyanieVyzova.Novyy)
                {
                    if (call.KollCentr)
                    {
                        LogicEndeCallCenterCall(call, newDial);
                        #region Block for Debug
                        //SendMessage(sysAdmin,
                        //    string.Format("Пропущенный вызов {0}", newDial.CallOutEvent?.acdCallInfo.acdName),
                        //    string.Format("Неотвечен вызов Колл-центра абонентом - {0}", Abonent.FullName));
                        #endregion
                        #region Block for release
                        foreach (var item in call.RelationshipUsers)
                            SendMessage(item.User,
                                string.Format("Пропущенный вызов {0}", newDial.CallOutEvent?.acdCallInfo.acdName),
                                string.Format("Неотвечен вызов Колл-центра абонентом - {0}", item.User.FullName));
                        #endregion
                    }
                    else
                    {
                        if (call.Type == EleWise.ELMA.CRM.Enums.RelationshipCallType.Input)
                            call.Theme = "Пропущенный вызов от " + newDial.RemoteAddress;
                        else
                            call.Theme = "Несостоявшийся разговор " + newDial.RemoteAddress;
                        call.StatusRazgovora = EleWise.ELMA.ConfigurationModel.SostoyanieVyzova.VypolnenNeUspeshno;
                        call.Completed = true;
                        //SendMessage(sysAdmin, "3-e Событие личный звонок", "Надо запланировать звонок");
                        PlaningFutureCall(call);
                        call.Save();
                    }
                }
                else if (call.StatusRazgovora == EleWise.ELMA.ConfigurationModel.SostoyanieVyzova.RazgovorNachat)
                {
                    call.StatusRazgovora = EleWise.ELMA.ConfigurationModel.SostoyanieVyzova.VypolnenUspeshno;
                    call.EndDate = DateTime.Now;
                    call.Completed = true;
                    call.Save();
                }
                TelephonyManager.TelephonyLog.Debug("Окончание разговора для " + Abonent.UserName + " выполнено CallEventsController.ProcessingReleaseCall");
            }
            catch(Exception ex)
            {
                SendMessage(sysAdmin, "error in ProcessingReleaseCall", string.Format("{0} || {1}", ex.Message, ex.TargetSite));
            }
        }


        ///// <summary>
        ///// Новая задача
        ///// </summary>
        ///// <param name="Call"></param>
        ///// <param name="subject"></param>
        ///// <returns></returns>
        //private Task NewTask(RelationshipCall Call, string subject)
        //{
        //    Task task = EntityManager<Task>.Instance.Create();
        //    task.CreationDate = DateTime.Now;
        //    task.CreationAuthor = Call.CreationAuthor;
        //    task.Subject = subject;
        //    task.Executor = Call.CreationAuthor;
        //    task.StartDate = DateTime.Now;
        //    task.EndDate = DateTime.Now.AddDays(1);
        //    task.Status = TaskBaseStatus.NewOrder;
        //    task.Save();
        //    return task;
        //}
        //private ICollection<Task> FindAllTaskByCall(RelationshipCall Call, User Executor)
        //{
        //    string eqlFindStr;
        //    if (Call.Contractor != null)
        //        eqlFindStr = string.Format("Contractor in({0}) AND Executor in({1})", Call.Contractor.Id.ToString(), Executor.Id.ToString());
        //    else
        //        eqlFindStr = string.Format("Lead in(Id={0}) AND Executor in({1})", Call.Lead.Id.ToString(), Executor.Id.ToString());

        //    try
        //    {
        //        var tmp = EntityManager<Task>.Instance.Find(eqlFindStr);
        //        SendMessage(sysAdmin, "Поиск задач", "Количество задач " + tmp.Count.ToString());
        //    }
        //    catch
        //    {
        //        SendMessage(sysAdmin, "Поиск задач", "Error find of Task");
        //    }

        //    return EntityManager<Task>.Instance.Find(eqlFindStr);
        //}

        /// <summary>
        /// Запланировать звонок
        /// </summary>
        /// <param name="calls"></param>
        private void PlaningFutureCall(RelationshipCall calls)
        {
            RelationshipCall call = EntityManager<RelationshipCall>.Instance.Create();
            call.CreationDate = DateTime.Now;
            call.CreationAuthor =
                EntityManager<User>.Instance.LoadOrNull(Guid.Parse("f0e949bf-4da4-4c02-b04c-ca61964b4fe8")); //System
            call.Completed = false;
            call.Contact = calls.Contact;
            call.Contractor = calls.Contractor;

            var calendar = Locator.GetServiceNotNull<IProductionCalendarService>();//TODO Планирование звонков на час вперед
            call.StartDate = calendar.EvalTargetTime(DateTime.Now, 1);
            call.EndDate = calendar.EvalTargetTime(DateTime.Now, 1);
            //call.StartDate= DateTime.Now.AddDays(1);
            //call.EndDate = DateTime.Now.AddDays(1);

            if (calls.Contractor != null)
                call.Theme = "Перезвонить абоненту " + calls.Contractor.Name;
            else
                call.Theme = "Перезвонить абоненту " + calls.Lead.Name;
            //SendMessage(sysAdmin, "Plane call", "Создаем участников");
            call.RelationshipUsers.Add(CreateParticipant(calls.CreationAuthor));
            call.Save();
            //SendMessage(sysAdmin, "Планирование звонка", "Звонок запланирован успешно");
        }

        /// <summary>
        /// Отправление сообщения пользователю
        /// </summary>
        /// <param name="user">Кому направляется</param>
        /// <param name="subj">Тема</param>
        /// <param name="mes">Текст сообщения</param>
        public void SendMessage(User user, string subj, string mes)
        {
            //создание нового сообщения:
            var m = InterfaceActivator.Create<ChannelMessage>();
            //добавление в список получателей пользователя, который хранится в переменной context.Poljzovatelj типа Пользователь
            m.Recipients.Add(user);
            m.Subject = subj;
            m.MessageType = ChannelMessageType.Post;
            //указание автора сообщения
            m.CreationAuthor = system;
            //указание даты создания сообщения:
            m.CreationDate = DateTime.Now;
            //содержание сообщения:
            m.FullMessage = mes;
            //создание статуса сообщения (новое или прочитанное)
            var s = InterfaceActivator.Create<RecipientMessageStatus>();
            s.Message = m;
            s.Recipient = user;
            //статус сообщения: новое
            s.Status = MessageStatus.New;
            s.Save();
            //добавление статуса в сообщение
            m.Statuses.Add(s);
            //сохранение сообщения
            m.Save();
        }

        /// <summary>
        /// Создать новый звонок
        /// </summary>
        /// <param name="newDial">Структура параметров вызова</param>
        /// <returns></returns>
        public RelationshipCall CreateRelationshipCall(Dial newDial)
        {
            User abonent = BeelineManager.Instance.BeelineConnect.FindAbonentsBy(newDial.SubscriptionID).Key;
            Abonent ab = BeelineManager.Instance.BeelineConnect.FindAbonentsBy(newDial.SubscriptionID).Value;
            RelationshipCall call = EntityManager<RelationshipCall>.Instance.Create();
            call.CreationDate = DateTime.Now;
            call.CreationAuthor = abonent;
            call = FindOrCreateContacts(call, newDial.RemoteAddress);
            call.UniqueId = newDial.TrackingId;
            call.StatusRazgovora = EleWise.ELMA.ConfigurationModel.SostoyanieVyzova.Novyy;
            call.SsylkaNaZapisj = new Uri(BeelineManager.Instance.BeeConnect.Api.getRecordFile(newDial.TrackingId, ab.phone).uri);
            //lock (dbLock)
                call.Save();
            call.RelationshipUsers.Add(CreateParticipant(abonent));
            return call;
        }

        /// <summary>
        /// Поиск звонка в системе по ИД
        /// </summary>
        /// <param name="callID">Идентификатор звонка</param>
        /// <returns></returns>
        public RelationshipCall FindLastRelationsByCallID(string callID)
        {
            return EntityManager<RelationshipCall>.Instance
                .Find(s => s.UniqueId == callID)
                .LastOrDefault();
        }

        /// <summary>
        /// Поиск имеющейся в системе информации по номеру телефона
        /// </summary>
        /// <param name="call">Объект типа "Звонок"</param>
        /// <param name="PhoneNum">Номер телефона</param>
        /// <returns></returns>
        private RelationshipCall FindOrCreateContacts(RelationshipCall call, string PhoneNum)
        {
            Lead lead = EntityManager<Lead>.Instance.Find(string.Format("Phone in(PhoneString='{0}')", PhoneNum))?.FirstOrDefault();
            Contact contact = EntityManager<Contact>.Instance.Find(string.Format("Phone in(PhoneString='{0}')", PhoneNum))?.FirstOrDefault();
            Contractor contractor = EntityManager<Contractor>.Instance.Find(string.Format("Phone in(PhoneString='{0}')", PhoneNum))?.FirstOrDefault();
            if (lead == null && contact == null && contractor == null)
            {
                lead = EntityManager<Lead>.Instance.Create();
                lead.Name = "Телефонный контакт с " + PhoneNum;
                lead.CreationAuthor = call.CreationAuthor;
                lead.CreationDate = DateTime.Now;
                lead.Status = EleWise.ELMA.CRM.Enums.LeadStatus.New;
                lead.Source =
                    EntityManager<LeadSource>.Instance
                        .Find(f => f.Uid == Guid.Parse("e03a36f5-58d6-4be0-9b70-64b55902c529"))
                        .FirstOrDefault();
                Phone phone = EntityManager<Phone>.Instance.Create();
                phone.PhoneString = PhoneNum;
                phone.Save();
                lead.Phone.Add(phone);
                lead.Save();
            }
            call.Lead = lead;
            call.Contact = contact;
            call.Contractor = contractor;
            return call;
        }

        /// <summary>
        /// Создание участников телефонного звонка
        /// </summary>
        /// <param name="participantAbonent">Пользователь системы, участник телефонного звонка</param>
        /// <returns></returns>
        private RelationshipUser CreateParticipant(User participantAbonent)
        {
            RelationshipUser u = EntityManager<RelationshipUser>.Create();//.Instance.Create();
            //u.Relationship = call;
            u.Status = EleWise.ELMA.CRM.Enums.RelationshipUserStatus.Participant;
            u.User = participantAbonent;
            u.Save();
            //call.RelationshipUsers.Add(u);
            return u;
        }

        /// <summary>
        /// Обработка логики события завершения звонка на колл-центр
        /// </summary>
        /// <param name="call">Объект "Звонок"</param>
        /// <param name="newDial">Информация о вызове тип Dial</param>
        private void LogicEndeCallCenterCall(RelationshipCall call, Dial newDial) //TODO Придумать с заверщениями звонка Колл-центра
        {
            //string tmp = string.Format("{0} {1}",
            //    newDial.CallOutEvent.releasingParty != null ? newDial.CallOutEvent.releasingParty.ToString() : "",
            //    newDial.CallOutEvent.releasingParty);
            //SendMessage(sysAdmin, "TEST 1 clothed Call-Center", tmp);
            if (/*newDial.CallOutEvent.releasingParty != null && */newDial.CallOutEvent.releasingParty==Schema.ReleasingParty.localRelease)
            {
                //SendMessage(sysAdmin, "TEST 2 clothed Call-Center", tmp);
                if (call.StatusRazgovora == EleWise.ELMA.ConfigurationModel.SostoyanieVyzova.Novyy)
                {
                    call.StatusRazgovora = EleWise.ELMA.ConfigurationModel.SostoyanieVyzova.VypolnenNeUspeshno;
                    //call.Theme = string.Format("Неотвеченный звонок '{0}' от {1}", newDial.CallOutEvent.acdCallInfo.acdName, newDial.RemoteAddress);
                }
                else if (call.StatusRazgovora == EleWise.ELMA.ConfigurationModel.SostoyanieVyzova.RazgovorNachat)
                    call.StatusRazgovora = EleWise.ELMA.ConfigurationModel.SostoyanieVyzova.VypolnenUspeshno;
                call.EndDate = DateTime.Now;
                call.Completed = true;
                call.Save();
            }
        }
    }
}