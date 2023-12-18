using EofficeBNILWEB.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Newtonsoft.Json;
using Microsoft.Extensions.Configuration;
using EofficeBNILWEB.DataAccess;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.AspNetCore.Localization;
using DocumentFormat.OpenXml.Office2010.Excel;

namespace EofficeBNILWEB.Controllers
{
    
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _config;
        private readonly IDataAccessProvider _dataAccessProvider;
        private readonly string urlApi;
        private readonly IHtmlLocalizer<HomeController> _localiza;

        public HomeController(ILogger<HomeController> logger, IConfiguration config, IDataAccessProvider dataAccessProvider, IHtmlLocalizer<HomeController> localiza)
        {
            _logger = logger;
            _config = config;
            _dataAccessProvider = dataAccessProvider;
            urlApi = _config.GetValue<string>("AppSettings:apiUrl");
            _localiza = localiza;
        }

        [Authorize]
        public async Task<IActionResult> Index()
      {
            DashboardOutput dahsboarOutput = new DashboardOutput();
            GeneralOutputModel generalOutput = new GeneralOutputModel();
            var token = HttpContext.Session.GetString("token");
            generalOutput = await _dataAccessProvider.GetDashboardContent("General/GetDashboardContent/", token);
            var jsonApiResponseSerialize = JsonConvert.SerializeObject(generalOutput.Result);
            dahsboarOutput = JsonConvert.DeserializeObject<DashboardOutput>(jsonApiResponseSerialize);

            if (generalOutput.Status == "UA")
            {
                TempData["status"] = "NG";
                TempData["pesan"] = "Session habis silahkan login kembali";
                return RedirectToAction("Logout", "Home");
            }
            var testtranslate = _localiza["Mail"].Value;
            ViewBag.srtmsk = testtranslate;
            var param = _localiza["Parameter"].Value;
            ViewBag.Param = param;


            return View(dahsboarOutput);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Login()
        {
            return View();
        }
        public IActionResult LoginNew()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginParam pr)
        {
            LoginResponse loginResponse = new LoginResponse();
            GeneralOutputModel generalOutput = new GeneralOutputModel();
            generalOutput = await _dataAccessProvider.LoginAsync(pr, "Token");
            var jsonApiResponseSerialize = JsonConvert.SerializeObject(generalOutput.Result);
            loginResponse = JsonConvert.DeserializeObject<LoginResponse>(jsonApiResponseSerialize);
            if (generalOutput.Status == "OK")
            {
                Response.Cookies.Append(CookieRequestCultureProvider.DefaultCookieName, CookieRequestCultureProvider.MakeCookieValue(new RequestCulture("id")),
                new CookieOptions { Expires = DateTimeOffset.Now.AddDays(15) });
                HttpContext.Session.Remove("en");
                HttpContext.Session.SetString("ID", "id");
                // coookie authentication with Owin, nomore check for Session
                var claims = new List<Claim>(){ new  Claim(ClaimTypes.Name, loginResponse.Fullname) ,
                            new Claim(ClaimTypes.NameIdentifier, loginResponse.Nip)
                            };
                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);
                HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                if (loginResponse.Password =="MTIz")
                {
                    HttpContext.Session.SetString("token", loginResponse.Token);
                    HttpContext.Session.SetString("idUser", loginResponse.IdUser.ToString().ToUpper());
                    HttpContext.Session.SetString("nip", loginResponse.Nip);
                    HttpContext.Session.SetString("password", loginResponse.Password);
                    HttpContext.Session.SetString("fullname", loginResponse.Fullname);
                    HttpContext.Session.SetString("idPosition", loginResponse.IdPosition.ToString().ToUpper());
                    HttpContext.Session.SetString("positionName", loginResponse.PositionName);
                    HttpContext.Session.SetString("parentIdPosition", loginResponse.parentIdPosition.ToString().ToUpper());
                    HttpContext.Session.SetString("idGroup", loginResponse.IdGroup);
                    HttpContext.Session.SetString("idUnit", loginResponse.IdUnit.ToString().ToUpper());
                    HttpContext.Session.SetString("unitName", loginResponse.UnitName);
                    HttpContext.Session.SetString("idBranch", loginResponse.IdBranch.ToString().ToUpper());
                    HttpContext.Session.SetString("branchName", loginResponse.BranchName);
                    HttpContext.Session.SetString("email", loginResponse.email);
                    HttpContext.Session.SetString("phone", loginResponse.phone.ToString());
                    HttpContext.Session.SetString("ina", "id".ToString());
                    HttpContext.Session.SetString("indo", "indo".ToString());
                    HttpContext.Session.SetString("directorIdUnit", loginResponse.directorIdUnit.ToString());
                    HttpContext.Session.SetString("parentIdUser", loginResponse.parentIdUser.ToString().ToUpper());
                    TempData["status"] = "LOGIN";
                    TempData["title"] = "Login Berhasil";
                    TempData["pesan"] = generalOutput.Message;
                    TempData["passDetect"] = pr.Password;
                    ViewBag.pass=loginResponse.Password;
                    return Redirect("~/SettingUser/FirtsTimeLogin");
                }
                else
                {
                     HttpContext.Session.SetString("token", loginResponse.Token);
                    HttpContext.Session.SetString("idUser", loginResponse.IdUser.ToString().ToUpper());
                    HttpContext.Session.SetString("nip", loginResponse.Nip);
                    HttpContext.Session.SetString("password", loginResponse.Password);
                    HttpContext.Session.SetString("fullname", loginResponse.Fullname);
                    HttpContext.Session.SetString("idPosition", loginResponse.IdPosition.ToString().ToUpper());
                    HttpContext.Session.SetString("positionName", loginResponse.PositionName);
                    HttpContext.Session.SetString("parentIdPosition", loginResponse.parentIdPosition.ToString().ToUpper());
                    HttpContext.Session.SetString("idGroup", loginResponse.IdGroup);
                    HttpContext.Session.SetString("idUnit", loginResponse.IdUnit.ToString().ToUpper());
                    HttpContext.Session.SetString("unitName", loginResponse.UnitName);
                    HttpContext.Session.SetString("idBranch", loginResponse.IdBranch.ToString().ToUpper());
                    HttpContext.Session.SetString("branchName", loginResponse.BranchName);
                    HttpContext.Session.SetString("email", loginResponse.email);
                    HttpContext.Session.SetString("phone", loginResponse.phone.ToString());
                    HttpContext.Session.SetString("ina", "id".ToString());
                    HttpContext.Session.SetString("indo", "indo".ToString());
                    HttpContext.Session.SetString("directorIdUnit", loginResponse.directorIdUnit.ToString());
                    HttpContext.Session.SetString("parentIdUser", loginResponse.parentIdUser.ToString().ToUpper());
                    TempData["status"] = "LOGIN";
                    TempData["title"] = "Login Berhasil";
                    TempData["pesan"] = generalOutput.Message;
                    TempData["passDetect"] = pr.Password;
					ViewBag.pass=loginResponse.Password;
					return Redirect("~/Home/Index");
                }
                return Redirect("~/Home/Index");

            }
            //else if (generalOutput.Status =="updpssd")
            //{
            //    //return UpdatePasswordLogin();
            //    return RedirectToAction("Login", "Home");
            //}
            else
            {
                TempData["status"] = "NG";
                TempData["pesan"] = generalOutput.Message;
                return Redirect("~/");
            }

        }

        public IActionResult Logout()
        {
            //Response.Cookies.Append(CookieRequestCultureProvider.DefaultCookieName, CookieRequestCultureProvider.MakeCookieValue(new RequestCulture("en")),
            //new CookieOptions { Expires = DateTimeOffset.Now.AddDays(-1) });

            //Response.Cookies.Append(CookieRequestCultureProvider.DefaultCookieName, CookieRequestCultureProvider.MakeCookieValue(new RequestCulture("id")),
            //new CookieOptions { Expires = DateTimeOffset.Now.AddDays(-1) });

            Response.Cookies.Delete("en");
            Response.Cookies.Delete("ID");
            HttpContext.Session.Clear();
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            
            return RedirectToAction("Login","Home");
        }

        public IActionResult UpdatePasswordLogin()
        {
           
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> UpdatePasswordLogin(ParamUpdatePasswordLogin pr)
        {
            try
            {
                GeneralOutputModel generalOutput = new GeneralOutputModel();

                generalOutput = await _dataAccessProvider.UpdatePasswordLogin("User/UpdatePasswordLoginUser", pr);
                var jsonApiResponseSerialize = JsonConvert.SerializeObject(generalOutput.Result);

                if (generalOutput.Status == "OK")
                {
                    TempData["status"] = "OK";
                    TempData["title"] = "Berhasil merubah password";
                    TempData["pesan"] = generalOutput.Message;
                }
                else if (generalOutput.Status == "UA")
                {
                    TempData["status"] = "NG";
                    TempData["pesan"] = "Session habis silahkan login kembali";
                    return RedirectToAction("Logout", "Home");
                }
                else
                {
                    TempData["status"] = "NG";
                    TempData["title"] = "Gagal merubah password";
                    TempData["pesan"] = generalOutput.Message;
                }


                return RedirectToAction("Login", "Home");
            }
            catch (Exception ex)
            {
                TempData["status"] = "NG";
                TempData["title"] = "Gagal merubah password";
                TempData["pesan"] = ex.ToString();
                return Redirect("~/Home/Login");
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost]
        public IActionResult Translate(string lengueage, string urlrtn)
        {
            Response.Cookies.Append(CookieRequestCultureProvider.DefaultCookieName, CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(lengueage)),
            new CookieOptions { Expires = DateTimeOffset.Now.AddDays(15) });
            //return RedirectToAction(nameof(Index));
            //string cookieValueFromReq = Request.Cookies["Key"];
           
            
            if (lengueage=="id")
            {
                
                HttpContext.Session.Remove("en");
                HttpContext.Session.SetString("ID", lengueage.ToString());
            }
            else if(lengueage == "en")
            {

                HttpContext.Session.Remove("ina");
                HttpContext.Session.SetString("en", lengueage.ToString());
            }
            //else if( lengueage =="")
            //{
            //    HttpContext.Session.Remove("en");
            //    HttpContext.Session.SetString("ID", lengueage.ToString());
            //}
            
            return LocalRedirect(urlrtn);
        }

    }
}