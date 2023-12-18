using EofficeBNILWEB.DataAccess;
using EofficeBNILWEB.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.AspNetCore.Localization;
namespace EofficeBNILWEB.Controllers
{
    public class InboxController : Controller
    {
        private readonly IConfiguration _config;
        private readonly IDataAccessProvider _dataAccessProvider;
        private readonly string urlApi;
        private readonly IHtmlLocalizer<HomeController> _localiza;
        public InboxController(IConfiguration config, IDataAccessProvider dataAccessProvider, IHtmlLocalizer<HomeController> localiza)
        {
            _config = config;
            _dataAccessProvider = dataAccessProvider;
            urlApi = _config.GetValue<string>("AppSettings:apiUrl");
            _localiza = localiza;
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

            generalOutput = await _dataAccessProvider.GetStringmapAsync("General/GetStringMapSearchInboxWM/", prLetterType, token);
            var jsonApiResponseSerialize = JsonConvert.SerializeObject(generalOutput.Result);
            LetterTypeList = JsonConvert.DeserializeObject<List<StringmapOutput>>(jsonApiResponseSerialize);

            ParamGetLetterWeb pr = new ParamGetLetterWeb();
            pr.draw = "1";
            pr.sortColumn = "";
            pr.sortColumnDirection = "";
            pr.searchValue = "";
            pr.pageSize = 20;
            pr.start = 0;

            generalOutput = await _dataAccessProvider.GetLetterDraft("Letter/GetInbox/", token, pr);
            jsonApiResponseSerialize = JsonConvert.SerializeObject(generalOutput.Result);
            letterList = JsonConvert.DeserializeObject<LetterOutputWeb>(jsonApiResponseSerialize);

            ViewBag.LetterTypeList = LetterTypeList;

            return View(letterList);
        }
        public IActionResult Draft()
        {
            return View();
        }

        public IActionResult Suratmasuk()
        {
            return View();
        }

        public IActionResult Suratsppd()
        {
            return View();
        }

        public IActionResult Suratperintah()
        {
            return View();
        }

        public IActionResult Suratundangan()
        {
            return View();
        }   
         
        public IActionResult Arsip()
        {
            return View();
        }

        public IActionResult DetailArsip()
        {
            return View();
        }

         public IActionResult SuratMasukSeketaris()
        {
            return View();
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
