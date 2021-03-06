﻿using System.Threading.Tasks;
using System.Web.Mvc;

using SetLocale.Client.Web.Models;
using SetLocale.Client.Web.Services;

namespace SetLocale.Client.Web.Controllers
{
    public class UserController : BaseController
    {
        private readonly IUserService _userService;

        public UserController(
            IUserService userService,
            IFormsAuthenticationService formsAuthenticationService,
            IDemoDataService demoDataService)
            : base(formsAuthenticationService, demoDataService)
        {
            _userService = userService;
        }

        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Apps()
        {
            var model = _demoDataService.GetUsersApps();
            return View(model);
        }


        #region Membership
        [HttpGet]
        public ActionResult New()
        {
            var model = new UserModel();
            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> New(UserModel model)
        {
            if (!model.IsValidForNewDeveloper())
            {
                model.Msg = "bir sorun oluştu";
                return View(model);
            }

            var userId = await _userService.Create(model);
            if (userId == null)
            {
                model.Msg = "bir sorun oluştu";
                return View(model);
            }

            return Redirect("/user/apps");
        }

        [HttpGet]
        public ActionResult Reset()
        {
            var model = new ResetModel()
            {
                Email = "dev@test.com"
            };

            return View(model);
        }

        [HttpGet]
        public ActionResult Login()
        {
            var model = new LoginModel();
            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginModel model)
        {
            if (!model.IsValid())
            {
                model.Msg = "bir sorun oluştu";
                return View(model);
            }

            var authenticated = await _userService.Authenticate(model.Email, model.Password);
            if (!authenticated)
            {
                model.Msg = "bir sorun oluştu";
                return View(model);
            }

            var user = await _userService.GetByEmail(model.Email);
            _formsAuthenticationService.SignIn(string.Format("{0}|{1}", user.Id, user.Name), true);

            return Redirect("/user/apps");
        }

        [HttpGet]
        public ActionResult Logout()
        {
            _formsAuthenticationService.SignOut();
            return RedirectToHome();
        }
        #endregion
    }
}