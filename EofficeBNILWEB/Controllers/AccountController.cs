using System.IO;
using EofficeBNILWEB.DataAccess;
using EofficeBNILWEB.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Data.OleDb;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;
using System.Data;
using System.Diagnostics;
using System.Dynamic;

namespace EofficeBNILWEB.Controllers
{
    public class AccountController : Controller
    {
        private readonly IConfiguration _config;
        private readonly IDataAccessProvider _dataAccessProvider;
        private readonly IHostingEnvironment _environment;
        private readonly string urlApi;
        public AccountController(IConfiguration config, IDataAccessProvider dataAccessProvider, IHostingEnvironment Environment)
        {
            _config = config;
            _dataAccessProvider = dataAccessProvider;
            _environment = Environment;
            urlApi = _config.GetValue<string>("AppSettings:apiUrl");
        }
        public IActionResult ForgetPassword(Guid id)
        {
            ViewBag.tokenId = id;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgetPassword(ParamUpdateForgotPassword pr)
        {
            try
            {
                GeneralOutputModel generalOutput = new GeneralOutputModel();

                generalOutput = await _dataAccessProvider.PostForgotPassword("User/UpdateForgotPasswordUser", pr);
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
    }
}
