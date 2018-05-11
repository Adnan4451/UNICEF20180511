using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using WMS.Models;

namespace WMS.CustomClass
{
    public class CustomControllerAttributesForSession
    {
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            string controllerName = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;
            bool HavePermission = false;
            try
            {
                HttpSessionStateBase session = filterContext.HttpContext.Session;
                filterContext.HttpContext.Session["SelectedMenu"] = controllerName;
                User loggedUser = session["LoggedUser"] as User;
                if (HavePermission == false)
                {
                    //filterContext.Result = new HttpUnauthorizedResult();
                    filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Home", action = "VDSContainer", area = "" }));
                    filterContext.Result.ExecuteResult(filterContext.Controller.ControllerContext);
                }
            }
            catch (Exception ex)
            {
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Home", action = "VDSContainer", area = "" }));
                filterContext.Result.ExecuteResult(filterContext.Controller.ControllerContext);
            }

        }
    }
}