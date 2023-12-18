using EofficeBNILWEB.DataAccess;
using EofficeBNILWEB.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Reflection.Metadata.Ecma335;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Localization;
namespace EofficeBNILWEB.Controllers
{
    public class OutboxController : Controller
    {
        private readonly IConfiguration _config;
        private readonly IDataAccessProvider _dataAccessProvider;
        private readonly string urlApi;
        public OutboxController(IConfiguration config, IDataAccessProvider dataAccessProvider)
        {
            _config = config;
            _dataAccessProvider = dataAccessProvider;
            urlApi = _config.GetValue<string>("AppSettings:apiUrl");
        }

        public async Task<IActionResult> Index()
        {
            LetterOutputWeb letterList = new LetterOutputWeb();
            GeneralOutputModel generalOutput = new GeneralOutputModel();
            var token = HttpContext.Session.GetString("token");

            List<StringmapOutput> LetterTypeList = new List<StringmapOutput>();
            ParamGetStringmap prLetterType = new ParamGetStringmap();
            prLetterType.objectName = "tm_letter";
            prLetterType.attributeName = "LETTER_TYPE_CODE";

            generalOutput = await _dataAccessProvider.GetStringmapAsync("General/GetStringMapSearchOutboxWM/", prLetterType, token);
            var jsonApiResponseSerialize = JsonConvert.SerializeObject(generalOutput.Result);
            LetterTypeList = JsonConvert.DeserializeObject<List<StringmapOutput>>(jsonApiResponseSerialize);

            ParamGetLetterWeb pr = new ParamGetLetterWeb();
            pr.draw = "1";
            pr.sortColumn = "";
            pr.sortColumnDirection = "";
            pr.searchValue = "";
            pr.pageSize = 20;
            pr.start = 0;

            generalOutput = await _dataAccessProvider.GetLetterDraft("Letter/GetOutbox/", token, pr);
            jsonApiResponseSerialize = JsonConvert.SerializeObject(generalOutput.Result);
            letterList = JsonConvert.DeserializeObject<LetterOutputWeb>(jsonApiResponseSerialize);

            ViewBag.LetterTypeList = LetterTypeList;
            return View(letterList);
        }

        public IActionResult Send()
        {
            return View();
        }

        public IActionResult SuratSppd()
        {
            return View();
        }

        public async Task<IActionResult> Draft()
        {
            LetterOutputWeb letterList = new LetterOutputWeb();
            GeneralOutputModel generalOutput = new GeneralOutputModel();
            var token = HttpContext.Session.GetString("token");

            List<StringmapOutput> LetterTypeList = new List<StringmapOutput>();
            ParamGetStringmap prLetterType = new ParamGetStringmap();
            prLetterType.objectName = "tm_letter";
            prLetterType.attributeName = "LETTER_TYPE_CODE";

            generalOutput = await _dataAccessProvider.GetStringmapAsync("General/GetStringmap/", prLetterType, token);
            var jsonApiResponseSerialize = JsonConvert.SerializeObject(generalOutput.Result);
            LetterTypeList = JsonConvert.DeserializeObject<List<StringmapOutput>>(jsonApiResponseSerialize);

            ParamGetLetterWeb pr = new ParamGetLetterWeb();
            pr.draw = "1";
            pr.sortColumn = "";
            pr.sortColumnDirection = "";
            pr.searchValue = "";
            pr.pageSize = 20;
            pr.start = 0;

            generalOutput = await _dataAccessProvider.GetLetterDraft("Letter/GetDraftLetter/", token, pr);
            jsonApiResponseSerialize = JsonConvert.SerializeObject(generalOutput.Result);
            letterList = JsonConvert.DeserializeObject<LetterOutputWeb>(jsonApiResponseSerialize);
            
            ViewBag.LetterTypeList = LetterTypeList;
            return View(letterList);
        }

        public IActionResult EditDraft()
        {
            return View();
        }

        public IActionResult Batal()
        {
            return View();
        }
        public async Task<IActionResult> Delete(string param)
        {
            ParamDeleteLetter pr = new ParamDeleteLetter();
            pr.idLetter = new Guid(param);
            GeneralOutputModel generalOutput = new GeneralOutputModel();
            var token = HttpContext.Session.GetString("token");

            generalOutput = await _dataAccessProvider.DeleteLetter("Letter/DeleteLetter/", token, pr);
            var jsonApiResponseSerialize = JsonConvert.SerializeObject(generalOutput);
            generalOutput = JsonConvert.DeserializeObject<GeneralOutputModel>(jsonApiResponseSerialize);

            if (generalOutput.Status == "OK")
            {
                TempData["status"] = "OK";
                TempData["title"] = "Berhasil menghapus surat";
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
                TempData["title"] = "Gagal menambahkan dokumen masuk";
                TempData["pesan"] = generalOutput.Message;
            }

            return RedirectToAction("Draft");
        }


        [HttpPost]
        public IActionResult Translate(string lengueage, string urlrtn)
        {
            Response.Cookies.Append(CookieRequestCultureProvider.DefaultCookieName, CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(lengueage)),
            new CookieOptions { Expires = DateTimeOffset.Now.AddDays(15) });
            //return RedirectToAction(nameof(Index));
            //string cookieValueFromReq = Request.Cookies["Key"];
            HttpContext.Session.Remove("ina");
            HttpContext.Session.Remove("en");
            if (lengueage == "id")
            {

                HttpContext.Session.SetString("ina", lengueage.ToString());
            }
            else if (lengueage == "en")
            {

                HttpContext.Session.SetString("en", lengueage.ToString());
            }

            return LocalRedirect(urlrtn);
        }



    }
}