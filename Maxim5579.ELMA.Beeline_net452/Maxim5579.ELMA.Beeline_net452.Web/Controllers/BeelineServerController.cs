using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EleWise.ELMA.Web.Mvc.Controllers;
using Maxim5579.ELMA.Beeline_net452.BeelineAPI;
using Maxim5579.ELMA.Beeline_net452.Managers;

namespace Maxim5579.ELMA.Beeline_net452.Web.Controllers
{
    public class BeelineServerController : BaseController
    {
        //
        // GET: /BeelineServer/

            /// <summary>
            /// Совершение звонка
            /// </summary>
            /// <param name="phone">Номер телефона</param>
            /// <returns></returns>
        public ActionResult MakeCall(string phone)
        {
            try
            {
                // если пользователь не взял тубку, он переключается в режим "Отключен".
                // для совершения звонка устанавливаем статус "Активен".
                //var response = BeelineManager.Instance.MakeCall(phone);
                OutCallInfo response = BeelineManager.Instance.MakeCall(phone);
                return Json(
                    new
                    {
                        success = true,
                        calling = response.callerID,
                        message = response.statusCode.ToString(),
                        //actionid = response.callerID
                    }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { success = false, error = e.Message }, JsonRequestBehavior.AllowGet);
            }
        }

    }
}
