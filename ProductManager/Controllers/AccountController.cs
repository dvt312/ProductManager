using System;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Microsoft.AspNet.Identity.Owin;
using ProductManager.Models;

namespace ProductManager.Controllers
{
    /// <summary>
    /// Account controller class.
    /// </summary>
    [Authorize]
    public class AccountController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public AccountController() { }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager )
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set 
            { 
                _signInManager = value; 
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        /// <summary>
        /// Login method using GET.
        /// </summary>
        /// <param name="returnUrl">Return URL to redirect user</param>
        /// <returns>Action result instance.</returns>
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl != null && returnUrl.Contains("/Account/Logout") ? null: returnUrl;
            return View();
        }

        /// <summary>
        /// Login method using POST, perform login action.
        /// </summary>
        /// <param name="model">LoginViewModel object with login screen data</param>
        /// <param name="returnUrl">Return URL to redirect user</param>
        /// <returns>Action result instance.</returns>
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            returnUrl = returnUrl != null && returnUrl.Contains("/Account/Logout") ? null : returnUrl;

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (model.UsePersistentStorage)
            {
                var result = await SignInManager.PasswordSignInAsync(
                    model.UserName, model.Password, isPersistent: false, shouldLockout: false);
                switch (result)
                {
                    case SignInStatus.Success:
                        FormsAuthentication.SetAuthCookie(model.UserName, false);
                        TempData[ProductController.USE_PERSISTENT_STORAGE_KEY] = model.UsePersistentStorage;
                        TempData[ProductController.LOGGED_USER_NAME_KEY] = model.UserName;
                        return RedirectToLocal(returnUrl);
                    case SignInStatus.Failure:
                    default:
                        ModelState.AddModelError(
                            "", "Invalid username or password, please check information and try again");
                        return View(model);
                }
            }

            var userNames = Convert.ToString(ConfigurationManager.AppSettings["UserNames"]);
            var pass = Convert.ToString(ConfigurationManager.AppSettings["DefaultPassword"]);
            if (!string.IsNullOrWhiteSpace(userNames) && !string.IsNullOrWhiteSpace(pass))
            {
                var users = userNames.Split(',');
                var user = users.FirstOrDefault(x => x == model.UserName);
                if (!string.IsNullOrWhiteSpace(user) && pass == model.Password)
                {
                    FormsAuthentication.SetAuthCookie(model.UserName, false);
                    TempData[ProductController.USE_PERSISTENT_STORAGE_KEY] = model.UsePersistentStorage;
                    TempData[ProductController.LOGGED_USER_NAME_KEY] = user;
                    return RedirectToLocal(returnUrl);
                }
            }

            ModelState.AddModelError(
                "", "Invalid username or password, please check information and try again");
            return View(model);
        }

        /// <summary>
        /// Logout action, SignOut authentication and resets Session. Redirect to Login page
        /// </summary>
        /// <returns>Action result instance.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            Session.Abandon();
            return RedirectToAction("Login", "Account", new {ReturnUrl = string.Empty});
        }

        /// <summary>
        /// Dispose implementation to dispose identity objects.
        /// </summary>
        /// <param name="disposing">Dispose flag</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
            }

            base.Dispose(disposing);
        }

        /// <summary>
        /// Redirects to returnURL, in case returnURL is not valid local url, redirect to product list.
        /// </summary>
        /// <param name="returnUrl">Return URL to redirect user</param>
        /// <returns>Action result instance.</returns>
        #region Helpers
        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("List", "Product");
        }
        #endregion
    }
}