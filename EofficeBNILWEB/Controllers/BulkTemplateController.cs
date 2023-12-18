using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Office2016.Excel;
using EofficeBNILWEB.DataAccess;
using EofficeBNILWEB.Models;
using Microsoft.AspNetCore.Mvc;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc.Localization;
using System.Web;

namespace EofficeBNILWEB.Controllers
{
    public class BulkTemplateController : Controller
    {
        private readonly IConfiguration _config;
        private readonly IDataAccessProvider _dataAccessProvider;
        private readonly IHostingEnvironment _environment;
        private readonly string urlApi;
        private readonly IHtmlLocalizer<HomeController> _localiza;
        public BulkTemplateController(IConfiguration config, IDataAccessProvider dataAccessProvider, IHostingEnvironment Environment, IHtmlLocalizer<HomeController> localiza)
        {
            _config = config;
            _dataAccessProvider = dataAccessProvider;
            _environment = Environment;
            urlApi = _config.GetValue<string>("AppSettings:apiUrl");
            _localiza = localiza;
        }
        // GET: /<controller>/
        public async Task<IActionResult> Index()
        {
            List<OutputContentTemplate> output = new List<OutputContentTemplate>();
            GeneralOutputModel generalOutput = new GeneralOutputModel();
            var token = HttpContext.Session.GetString("token");
            generalOutput = await _dataAccessProvider.GetContentTemplate("Template/GetTemplate/", token);
            var jsonApiResponseSerialize = JsonConvert.SerializeObject(generalOutput.Result);
            output = JsonConvert.DeserializeObject<List<OutputContentTemplate>>(jsonApiResponseSerialize);

            if (generalOutput.Status == "UA")
            {
                return RedirectToAction("Logout", "Home");
            }
            ViewBag.data = output;
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> SaveTamplateBulk(ParamInsertContentTemplateBulk pr)
        {
            try
            {
                
                var token = HttpContext.Session.GetString("token");
                GeneralOutputModel generalOutput = new GeneralOutputModel();
                ParamInsertTemplateBulk prApiInsert = new ParamInsertTemplateBulk();
                List<ListParameter> prApiParam = new List<ListParameter>();
                //if (pr.templateName == null)
                //{
                //    TempData["status"] = "NG";
                //    TempData["title"] = "Nama template tidak boleh kosong";
                //    TempData["pesan"] = generalOutput.Message;
                //    return Redirect("~/Template/Index");
                //}
                //if (pr.templateContent == null)
                //{
                //    TempData["status"] = "NG";
                //    TempData["title"] = "Template tidak boleh kosong";
                //    TempData["pesan"] = generalOutput.Message;
                //    return Redirect("~/Template/Index");
                //}
                prApiInsert.templateContent = pr.templateContent;
                prApiInsert.templateName = pr.templateName;
                if (pr.parameter.Length > 0)
                {
                    for (int i = 0; i < pr.parameter.Length; i++)
                    {
                        string parameter = pr.parameter.GetValue(i).ToString();
                        prApiParam.Add(new ListParameter
                        {
                            parameter = parameter
                        });
                    }
                }
                prApiInsert.parameter = prApiParam;
                generalOutput = await _dataAccessProvider.AddContentTemplateBulk("TemplateBulk/InsertTemplateBulk", token, prApiInsert);
                var jsonApiResponseSerialize = JsonConvert.SerializeObject(generalOutput.Result);

                if (generalOutput.Status == "OK")
                {
                    TempData["status"] = "OK";
                    TempData["title"] = "Berhasil menambahkan template surat";
                    TempData["pesan"] = generalOutput.Message;
                }
                else if (generalOutput.Status == "UA")
                {
                    return RedirectToAction("Logout", "Home");
                }
                else
                {
                    TempData["status"] = "NG";
                    TempData["title"] = "Gagal menambahkan template surat";
                    TempData["pesan"] = generalOutput.Message;
                }


                return Redirect("~/Template/Index");
            }
            catch (Exception ex)
            {
                TempData["status"] = "NG";
                TempData["title"] = "Gagal menambahkan template surat";
                TempData["pesan"] = ex.ToString();

                return Redirect("~/Template/Index");
            }
        }
        [HttpPost]
        public async Task<IActionResult> UpdateTemplate(ParamUpdateContentTemplate pr)
        {
            try
            {

                var token = HttpContext.Session.GetString("token");
                GeneralOutputModel generalOutput = new GeneralOutputModel();
                generalOutput = await _dataAccessProvider.UpdateContentTemplate("Template/UpdateTemplate", token, pr);
                var jsonApiResponseSerialize = JsonConvert.SerializeObject(generalOutput.Result);

                if (generalOutput.Status == "OK")
                {
                    TempData["status"] = "OK";
                    TempData["title"] = "Berhasil merubah template surat";
                    TempData["pesan"] = generalOutput.Message;
                }
                else if (generalOutput.Status == "UA")
                {
                    return RedirectToAction("Logout", "Home");
                }
                else
                {
                    TempData["status"] = "NG";
                    TempData["title"] = "Gagal merubah template surat";
                    TempData["pesan"] = generalOutput.Message;
                }


                return Redirect("~/Template/Index");
            }
            catch (Exception ex)
            {
                TempData["status"] = "NG";
                TempData["title"] = "Gagal merubah template surat";
                TempData["pesan"] = ex.ToString();

                return Redirect("~/Template/Index");
            }
        }
        [HttpPost]
        public async Task<string> DeleteTemplate(ParamUpdateContentTemplate pr)
        {
            var token = HttpContext.Session.GetString("token");
            GeneralOutputModel generalOutput = new GeneralOutputModel();
            pr.isDeleted = 1;
            generalOutput = await _dataAccessProvider.UpdateContentTemplate("Template/UpdateTemplate", token, pr);
            var jsonApiResponseSerialize = JsonConvert.SerializeObject(generalOutput);

            return jsonApiResponseSerialize;
        }

    }
}
