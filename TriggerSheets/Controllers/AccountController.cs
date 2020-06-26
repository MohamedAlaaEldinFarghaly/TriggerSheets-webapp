using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using TriggerSheets.Models;
using System.Web.Security;
using System.Configuration;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.DirectoryServices.Protocols;
using System.Security;
using System.Net;

namespace TriggerSheets.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        public AccountController()
            : this(new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext())))
        {
        }

        public AccountController(UserManager<ApplicationUser> userManager)
        {
            UserManager = userManager;
        }

        public UserManager<ApplicationUser> UserManager { get; private set; }


        public bool AuthenticateAD(string username, string password)

        {
            
            //using (var ldap = new LdapConnection(new LdapDirectoryIdentifier("ldaps://ed.pg.com:636")))
            using (var ldap = new LdapConnection(new LdapDirectoryIdentifier("www.zflexldap.com:389")))
            {
                ldap.SessionOptions.ProtocolVersion = 3;
                ldap.AuthType = AuthType.Basic;
                try
                {
                    //ldap.Bind(new NetworkCredential("uid = " + username + ",ou=people,ou=pg,o=world", password));
                     ldap.Bind(new NetworkCredential("uid = " + username + ", ou = users, ou = guests, dc = zflexsoftware, dc = com", password));
                    return true;
                }
                catch (LdapException)
                {
                    return false;
                }
            }
           
            
          /*  using (var context = new PrincipalContext(ContextType.Domain, "LDAP://www.zflexldap.com:389/", "uid=guest1,ou=users,ou=guests,dc=zflexsoftware,dc=com", "guest1password"))
            {
            //Username and password for authentication.
                return context.ValidateCredentials("uid=guest1,ou=users,ou=guests,dc=zflexsoftware,dc=com", "guest1password"); 
        }
            */
        }

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel model, string returnUrl)
        {
            if (!this.ModelState.IsValid)

          {

            return this.View(model);
          }

        //Check LDAP Authentication

        if (this.AuthenticateAD(model.UserName, model.Password))

          {

        //Save credentials to use while accessing reports.

        Session["Username"] = model.UserName;

        Session["Password"] = model.Password;

        FormsAuthentication.SetAuthCookie(model.UserName, model.RememberMe);
        if (this.Url.IsLocalUrl(returnUrl) &&returnUrl.Length> 1 && returnUrl.StartsWith("/")&& !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))

        {

         return this.Redirect(returnUrl);
        }

        return this.RedirectToAction("Index", "Home");
        }

        this.ModelState.AddModelError(string.Empty, "The Username or Password is incorrect.");
        return this.View(model);
        }

        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            HttpContext.GetOwinContext().Authentication.SignOut();
            Session.Abandon(); // it will clear the session at the end of request
            Session["Username"] = null;

            Session["Password"] = null;
            return RedirectToAction( "Login", "Account", null);
        }

        

     
    }
}