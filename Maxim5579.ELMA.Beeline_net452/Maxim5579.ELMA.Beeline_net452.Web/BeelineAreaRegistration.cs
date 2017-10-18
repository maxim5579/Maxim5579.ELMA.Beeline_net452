using System.Web.Mvc;

namespace Maxim5579.ELMA.Beeline_net452.Web
{
    public class BeelineAreaRegistration : AreaRegistration
    {
        public const string AREA_NAME = "Maxim5579.ELMA.Beeline_net452.Web";
        public override string AreaName
        {
            get { return AREA_NAME; }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Beeline_default",
                "Beeline/{controller}/{action}/{id}",
                new { controller = "Home", action = "Index", id = UrlParameter.Optional }
                //new { controller = "BeelineSettings", action = "View", id = UrlParameter.Optional }
            );
        }
    }
}