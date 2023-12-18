using EofficeBNILWEB.DataAccess;
using EofficeBNILWEB.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Data.OleDb;
using System.Data;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;
using System.Web;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Localization;
using System.Net;
using System.Text;
using Microsoft.Extensions.Options;
using iTextSharp.text;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Office2010.Excel;
using QRCoder;
using System.Drawing;
using Font = iTextSharp.text.Font;
using Image = iTextSharp.text.Image;
using Rectangle = iTextSharp.text.Rectangle;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Drawing;
using Paragraph = iTextSharp.text.Paragraph;
using Path = System.IO.Path;
using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Wordprocessing;
using Document = iTextSharp.text.Document;
using PageSize = iTextSharp.text.PageSize;
using iTextSharp.text.pdf.draw;
using DocumentFormat.OpenXml.EMMA;
using System;
using System.Globalization;
using DocumentFormat.OpenXml.Office2010.PowerPoint;
using DocumentFormat.OpenXml.Spreadsheet;
using static Org.BouncyCastle.Bcpg.Attr.ImageAttrib;
using static System.Net.Mime.MediaTypeNames;


namespace EofficeBNILWEB.Controllers
{
    public class MemoController : Controller
    {
        private readonly IConfiguration _config;
        private readonly IDataAccessProvider _dataAccessProvider;
        private readonly IHostingEnvironment _environment;
        private readonly string urlApi;
        private readonly IHtmlLocalizer<HomeController> _localiza;
        public FtpSettings _ftpconfig { get; }
        public MemoController(IConfiguration config, IDataAccessProvider dataAccessProvider, IHostingEnvironment Environment, IHtmlLocalizer<HomeController> localiza, IOptions<FtpSettings> ftpconfig)
        {
            _config = config;
            _dataAccessProvider = dataAccessProvider;
            _environment = Environment;
            urlApi = _config.GetValue<string>("AppSettings:apiUrl");
            _localiza = localiza;
            _ftpconfig = ftpconfig.Value;
        }
        public IActionResult Index()
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

        public async Task<IActionResult> NewMemo()
        {
            GeneralOutputModel generalOutput = new GeneralOutputModel();
            List<OutputContentTemplate> output = new List<OutputContentTemplate>();
            List<StringmapMemoOutput> jenisMemo = new List<StringmapMemoOutput>();
            var token = HttpContext.Session.GetString("token");
            string NoLetter = "";
            generalOutput = await _dataAccessProvider.GetContentTemplate("Template/GetTemplate/", token);
            var jsonApiResponseSerialize = JsonConvert.SerializeObject(generalOutput.Result);
            output = JsonConvert.DeserializeObject<List<OutputContentTemplate>>(jsonApiResponseSerialize);
            //jsonApiResponseSerialize = JsonConvert.SerializeObject(output);

            ParamGetStringmap pr = new ParamGetStringmap();
            pr.objectName = "tm_letter_Memo";
            pr.attributeName = "OUTBOX_TYPE_CODE";
            generalOutput = await _dataAccessProvider.GetStringmapAsync("Memo/GetStringmapMemoWeb/", pr, token);
            jsonApiResponseSerialize = JsonConvert.SerializeObject(generalOutput.Result);
            jenisMemo = JsonConvert.DeserializeObject<List<StringmapMemoOutput>>(jsonApiResponseSerialize);

           
            if (generalOutput.Status == "UA")
            {
                TempData["status"] = "NG";
                TempData["pesan"] = "Session habis silahkan login kembali";
                return RedirectToAction("Logout", "Home");
            }

            ViewBag.NoLetter = NoLetter;
            ViewBag.TemplateData = output;
            ViewBag.JenisMemo = jenisMemo;
            //ViewBag.JsonTemplateData = jsonApiResponseSerialize.ToString();
            return View();
        }

        [HttpPost]
        public async Task<string> GetUserReceiver(string keyword)
        {
            DocumenMemotOutput documentMemoOutput = new DocumenMemotOutput();
            GeneralOutputModel generalOutput = new GeneralOutputModel();
            List<AdminDivisionOutput> receiverList = new List<AdminDivisionOutput>();
            var token = HttpContext.Session.GetString("token");
            Guid idUnit = new Guid(HttpContext.Session.GetString("idUnit"));
            //Guid parentIdPosition = new Guid(HttpContext.Session.GetString("parentIdPosition"));
            Guid directorIdUnit = new Guid(HttpContext.Session.GetString("directorIdUnit"));

            ParamGetPenerima pr = new ParamGetPenerima();
            pr.keyword = keyword;

            generalOutput = await _dataAccessProvider.GetDataPenerima("General/GetDataPenerima/", token, pr);
            var jsonApiResponseSerialize = JsonConvert.SerializeObject(generalOutput.Result);
            receiverList = JsonConvert.DeserializeObject<List<AdminDivisionOutput>>(jsonApiResponseSerialize).Where(p => p.idLevel != 0 && (p.idUnit == idUnit || p.idUnit == directorIdUnit)).ToList();
            jsonApiResponseSerialize = JsonConvert.SerializeObject(receiverList);

            return jsonApiResponseSerialize;
        }
        

        [HttpPost]
        public async Task<string> GetUserCopy(string keyword)
        {
            DocumentOutputWeb documentOutput = new DocumentOutputWeb();
            GeneralOutputModel generalOutput = new GeneralOutputModel();
            List<AdminDivisionOutput> receiverList = new List<AdminDivisionOutput>();
            var token = HttpContext.Session.GetString("token");
            Guid idUnit = new Guid(HttpContext.Session.GetString("idUnit"));

            ParamGetPenerima pr = new ParamGetPenerima();
            pr.keyword = keyword;

            generalOutput = await _dataAccessProvider.GetDataPenerima("General/GetDataPenerima/", token, pr);
            var jsonApiResponseSerialize = JsonConvert.SerializeObject(generalOutput.Result);
            receiverList = JsonConvert.DeserializeObject<List<AdminDivisionOutput>>(jsonApiResponseSerialize).Where(p => p.idLevel != 0).ToList();
            jsonApiResponseSerialize = JsonConvert.SerializeObject(receiverList);

            return jsonApiResponseSerialize;
        }

        [HttpPost]
        public async Task<IActionResult> SaveLetterMemo(ParamInsertMemoWeb pr, IFormFile inputfile)
        {
            try
            {
                var token = HttpContext.Session.GetString("token");
                GeneralOutputModel generalOutput = new GeneralOutputModel();
                ParamInsertMemo prApi = new ParamInsertMemo();
                List<ParamInsertCheckerMemo> prApiChecker = new List<ParamInsertCheckerMemo>();
                List<ParamInsertCheckerMemoPenerima> prApiCheckerPenerima = new List<ParamInsertCheckerMemoPenerima>();
                List<ParamInsertCheckerMemoCarbonCopy> prApiCheckerCarbonCopy = new List<ParamInsertCheckerMemoCarbonCopy>();
                List<ParamInsertCheckerMemoDelibretion> prApiDelibretion = new List<ParamInsertCheckerMemoDelibretion>();
                List<ParamInsertSendMemogoingRecipient> prApiReceiver = new List<ParamInsertSendMemogoingRecipient>();
                List<ParamInsertCheckerMemoLainya> prApiPemeriksaLainya = new List<ParamInsertCheckerMemoLainya>();
                ParamInsertAttachmentMemo prAttachment = new ParamInsertAttachmentMemo();
                OutputInsertAttachmentMemo responseApiAttachment = new OutputInsertAttachmentMemo();
                int print = pr.saveType;
                if (pr.saveType == 3)
                {
                    pr.saveType = 1;
                }
                if (pr.saveType == 1 || pr.saveType == 2)
                {
                    if (inputfile != null)
                    {
                        string path = Path.Combine(_environment.WebRootPath, "uploads/letter");
                        if (!Directory.Exists(path))
                        {
                            Directory.CreateDirectory(path);
                        }
                        string fileName = Path.GetFileName(inputfile.FileName);
                        string filePath = Path.Combine(path, fileName);
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await inputfile.CopyToAsync(stream);
                        }
                        prAttachment.idLetter = pr.idresponsesurat;
                        prAttachment.filename = inputfile.FileName;
                        prAttachment.isDocLetter = 1;

                        generalOutput = await _dataAccessProvider.InsertAttachmentMemo("Memo/InsertAttachmentMemo", token, prAttachment);
                        var jsonApiResponseSerializes = JsonConvert.SerializeObject(generalOutput.Result);
                        responseApiAttachment = JsonConvert.DeserializeObject<OutputInsertAttachmentMemo>(jsonApiResponseSerializes);
                        if (generalOutput.Status == "NG")
                        {
                            TempData["status"] = "NG";
                            TempData["title"] = generalOutput.Message;
                            TempData["pesan"] = generalOutput.Message;
                        }
                        else if (generalOutput.Status == "UA")
                        {
                            TempData["status"] = "NG";
                            TempData["pesan"] = "Session habis silahkan login kembali";
                            return RedirectToAction("Logout", "Home");
                        }
                    }

                    prApi.outboxType = pr.outboxType;
                    prApi.idMemoType = pr.idMemoType;
                    prApi.letterTypeCode = pr.letterTypeCode;
                    prApi.letterDate = pr.letterDate;
                    prApi.about = pr.about;
                    prApi.resultType = pr.resultType;
                    prApi.signatureType = pr.signatureType;
                    prApi.bossPositionId = pr.bossPositionId;
                    prApi.bossUserId = pr.bossUserId;
                    prApi.bossUnitId = pr.bossUnitId;
                    prApi.bossLevelId = pr.bossLevelId;
                    prApi.attachmentCount = pr.attachmentCount;
                    prApi.priority = pr.priority;
                    
                    prApi.isiSurat = pr.isiSurat;


                    

                    #region Memo CC (CarbonCopy)
                    if (pr.idUserCheckerCarbonCopy.Length > 0)
                    {
                        for (int a = 0; a < pr.idUserCheckerCarbonCopy.Length; a++)
                        {
                            Guid idCheckerCarbonCopy= new Guid(pr.idUserCheckerCarbonCopy.GetValue(a).ToString());
                            int idLevelCarbonCopy = Convert.ToInt32(pr.idLevelCheckerCarbonCopy.GetValue(a).ToString());
                            Guid idUnitCheckerCarbonCopy = new Guid(pr.idUnitCheckerCarbonCopy.GetValue(a).ToString());
                            prApiCheckerCarbonCopy.Add(new ParamInsertCheckerMemoCarbonCopy
                            {
                                idUserCheckerCarbonCopy = idCheckerCarbonCopy,
                                idLevelCheckerCarbonCopy = idLevelCarbonCopy,
                                idUnitCheckerCarbonCopy = idUnitCheckerCarbonCopy
                            });
                        }
                    }

                    #endregion


                    #region Delibretion
                        if (pr.idUserCheckerDelibretion.Length > 0)
                        {
                            for (int a = 0; a < pr.idUserCheckerDelibretion.Length; a++)
                            {
                                Guid idCheckerDelibretion = new Guid(pr.idUserCheckerDelibretion.GetValue(a).ToString());
                                int idLevelDelibretion = Convert.ToInt32(pr.idLevelCheckerDelibretion.GetValue(a).ToString());
                                Guid idUnitCheckerDelibretion = new Guid(pr.idUnitCheckerDelibretion.GetValue(a).ToString());
                                prApiDelibretion.Add(new ParamInsertCheckerMemoDelibretion
                                {
                                    idUserCheckerDelibretion = idCheckerDelibretion,
                                    idLevelCheckerDelibretion = idLevelDelibretion,
                                    idUnitCheckerDelibretion = idUnitCheckerDelibretion
                                });
                            }
                        }
                    #endregion


                    if (pr.idUserChecker.Length > 0)
                    {
                        for (int a = 0; a < pr.idUserChecker.Length; a++)
                        {
                            Guid idChecker = new Guid(pr.idUserChecker.GetValue(a).ToString());
                            int idLevel = Convert.ToInt32(pr.idLevelChecker.GetValue(a).ToString());
                            Guid idUnitChecker = new Guid(pr.idUnitChecker.GetValue(a).ToString());
                            prApiChecker.Add(new ParamInsertCheckerMemo
                            {
                                idUserChecker = idChecker,
                                idLevelChecker = idLevel,
                                idUnitChecker = idUnitChecker
                            });
                        }
                    }


                    // divisi lainya

                    if (pr.idUserCheckerlain.Length > 0)
                    {
                        for (int a = 0; a < pr.idUserCheckerlain.Length; a++)
                        {
                            Guid idChecker = new Guid(pr.idUserCheckerlain.GetValue(a).ToString());
                            int idLevel = Convert.ToInt32(pr.idLevelCheckerlain.GetValue(a).ToString());
                            Guid idUnitChecker = new Guid(pr.idUnitCheckerlain.GetValue(a).ToString());
                            prApiPemeriksaLainya.Add(new ParamInsertCheckerMemoLainya
                            {
                                idUserCheckerlain = idChecker,
                                idLevelCheckerlain = idLevel,
                                idUnitCheckerlain = idUnitChecker
                            });
                        }
                    }

                    #region Memo To (Penerima)
                    if (pr.idUserCheckerPenerima.Length > 0)
                    {
                        for (int a = 0; a < pr.idUserCheckerPenerima.Length; a++)
                        {
                            Guid idCheckerPenerima = new Guid(pr.idUserCheckerPenerima.GetValue(a).ToString());
                            int idLevelPenerima = Convert.ToInt32(pr.idLevelCheckerPenerima.GetValue(a).ToString());
                            Guid idUnitCheckerPenerima = new Guid(pr.idUnitCheckerPenerima.GetValue(a).ToString());
                            prApiCheckerPenerima.Add(new ParamInsertCheckerMemoPenerima
                            {
                                idUserCheckerPenerima = idCheckerPenerima,
                                idLevelCheckerPenerima = idLevelPenerima,
                                idUnitCheckerPenerima = idUnitCheckerPenerima
                            });
                        }
                    }

                    #endregion

                    var checksenderischecker = prApiChecker.Where(p => p.idUserChecker == pr.bossUserId).FirstOrDefault();
                    if (checksenderischecker == null)
                    {
                        prApiChecker.Add(new ParamInsertCheckerMemo
                        {
                            idUserChecker = pr.bossUserId,
                            idLevelChecker = pr.bossLevelId,
                            idUnitChecker = pr.bossUnitId
                        });
                    }




                    prApi.idUserChecker = prApiChecker;
                    prApi.idUserCheckerPenerima = prApiCheckerPenerima;
                    prApi.idUserCheckerCarbonCopy = prApiCheckerCarbonCopy;
                    prApi.idUserCheckerDelibretion = prApiDelibretion;
                    prApi.idUserCheckerlain = prApiPemeriksaLainya;
                    //prApi.senderName = prApiReceiver;
                }

                prApi.idresponsesurat = responseApiAttachment.idLetter == Guid.Empty ? pr.idresponsesurat : responseApiAttachment.idLetter;
                prApi.saveType = pr.saveType;
                prApi.comment = pr.comment;
                prApi.summary = pr.summary;
                //prApi.senderCompanyName = pr.senderCompanyName;

                generalOutput = await _dataAccessProvider.InsertLetterMemo("Memo/InsertMemoLetter", token, prApi);
                var jsonApiResponseSerialize = JsonConvert.SerializeObject(generalOutput.Result);
                var titlePopup = "Memo berhasil disimpan";
                var pesanPopup = "Memo berhasil disimpan";
                if (pr.saveType == 4)
                {
                    titlePopup = "Memo berhasil dikirim";
                    pesanPopup = "Memo berhasil dikirim";
                }
                if (pr.saveType == 2)
                {
                    titlePopup = "Memo berhasil dikirim";
                    pesanPopup = "Memo berhasil dikirim";
                }
                if (pr.saveType == 5)
                {
                    titlePopup = "Memo berhasil ditolak";
                    pesanPopup = "Memo berhasil ditolak";
                }
                if (generalOutput.Status == "OK")
                {
                    TempData["status"] = "OK";
                    TempData["title"] = titlePopup;
                    TempData["pesan"] = pesanPopup;
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
                    TempData["title"] = generalOutput.Message;
                    TempData["pesan"] = generalOutput.Message;
                }
                if (print == 3)
                {
                    if (generalOutput.Status == "OK")
                    {
                        Guid idLetter = new Guid(generalOutput.Result.ToString());
                        //await PrintLetter(idLetter);
                    }
                }
                if (pr.saveType != 1)
                {
                    return Redirect("~/Memo/DistributionMemo");
                }
                else
                {
                    return Redirect("~/Memo/DraftMemo");
                }


            }
            catch (Exception ex)
            {
                TempData["status"] = "NG";
                TempData["title"] = "Gagal membuat surat keluar";
                TempData["pesan"] = ex.ToString();
                return Redirect("~/Memo/NewMemo");
            }
        }


        [HttpPost]
        public async Task<string> SaveAttachment(string param, IFormFile file)
        {
            try
            {
                var token = HttpContext.Session.GetString("token");
                GeneralOutputModel generalOutput = new GeneralOutputModel();
                ParamInsertAttachmentMemo pr = new ParamInsertAttachmentMemo();
                OutputInsertAttachmentMemo responseApi = new OutputInsertAttachmentMemo();
                string[] splitParam = param.Split("~");
                string dateNow = DateTime.Now.ToString("ddMMYYYYHHmmss");
                //foreach (var file in files)
                //{
                string path = Path.Combine(_environment.WebRootPath, "uploads/letter");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                //Save the uploaded Excel file.
                string fileName = Path.GetFileName(file.FileName);
                string filePath = Path.Combine(path, fileName);
                //using (FileStream stream = new FileStream(filePath, FileMode.Create))
                //{
                //    file.CopyTo(stream);
                //}

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                pr.idLetter = splitParam[1] == "0" ? Guid.Empty : new Guid(splitParam[1]);
                pr.filename = file.FileName;
                pr.isDocLetter = 0;

                generalOutput = await _dataAccessProvider.InsertAttachmentMemo("Memo/InsertAttachmentMemoWeb", token, pr);
                var jsonApiResponseSerialize = JsonConvert.SerializeObject(generalOutput.Result);
                responseApi = JsonConvert.DeserializeObject<OutputInsertAttachmentMemo>(jsonApiResponseSerialize);
                jsonApiResponseSerialize = JsonConvert.SerializeObject(responseApi);
                return jsonApiResponseSerialize;
                //}
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }



        [HttpPost]
        public async Task<string> SavePrevMemoLetter(ParamInsertMemoWeb pr, IFormFile inputfile)
        {
            OutputPreviewMemo resultPreview = new OutputPreviewMemo();
            try
            {
                var token = HttpContext.Session.GetString("token");
                GeneralOutputModel generalOutput = new GeneralOutputModel();
                ParamInsertMemo prApi = new ParamInsertMemo();
                List<ParamInsertCheckerMemo> prApiChecker = new List<ParamInsertCheckerMemo>();
                List<ParamInsertCheckerMemoPenerima> prApiCheckerPenerima = new List<ParamInsertCheckerMemoPenerima>();
                List<ParamInsertCheckerMemoCarbonCopy> prApiCheckerCarbonCopy = new List<ParamInsertCheckerMemoCarbonCopy>();
                List<ParamInsertSendMemogoingRecipient> prApiReceiver = new List<ParamInsertSendMemogoingRecipient>();
                List<ParamInsertCheckerMemoDelibretion> prApiDelibretion = new List<ParamInsertCheckerMemoDelibretion>();
                List<ParamInsertCheckerMemoLainya> prApiPemeriksaLainya = new List<ParamInsertCheckerMemoLainya>();
                ParamInsertAttachmentMemo prAttachment = new ParamInsertAttachmentMemo();
                int print = pr.saveType;
                if (pr.saveType == 3)
                {
                    pr.saveType = 1;
                }
                if (pr.saveType == 1 || pr.saveType == 2)
                {
                    prApi.outboxType = pr.outboxType;
                    prApi.idMemoType = pr.idMemoType;
                    prApi.letterTypeCode = pr.letterTypeCode;
                    prApi.letterDate = pr.letterDate;
                    prApi.about = pr.about;
                    prApi.resultType = pr.resultType;
                    prApi.signatureType = pr.signatureType;
                    prApi.bossPositionId = pr.bossPositionId;
                    prApi.bossUserId = pr.bossUserId;
                    prApi.bossUnitId = pr.bossUnitId;
                    prApi.bossLevelId = pr.bossLevelId;
                    prApi.attachmentCount = pr.attachmentCount;
                    prApi.priority = pr.priority;
                    
                    prApi.isiSurat = pr.isiSurat;

                    

                    #region Memo To (Penerima)
                    if (pr.idUserCheckerPenerima.Length > 0)
                    {
                        for (int a = 0; a < pr.idUserCheckerPenerima.Length; a++)
                        {
                            Guid idCheckerPenerima = new Guid(pr.idUserCheckerPenerima.GetValue(a).ToString());
                            int idLevelPenerima = Convert.ToInt32(pr.idLevelCheckerPenerima.GetValue(a).ToString());
                            Guid idUnitCheckerPenerima = new Guid(pr.idUnitCheckerPenerima.GetValue(a).ToString());
                            prApiCheckerPenerima.Add(new ParamInsertCheckerMemoPenerima
                            {
                                idUserCheckerPenerima = idCheckerPenerima,
                                idLevelCheckerPenerima = idLevelPenerima,
                                idUnitCheckerPenerima = idUnitCheckerPenerima
                            });
                        }
                    }

                    #endregion

                    #region Memo CC (CarbonCopy)
                    if (pr.idUserCheckerCarbonCopy.Length > 0)
                    {
                        for (int a = 0; a < pr.idUserCheckerCarbonCopy.Length; a++)
                        {
                            Guid idCheckerCarbonCopy = new Guid(pr.idUserCheckerCarbonCopy.GetValue(a).ToString());
                            int idLevelCarbonCopy = Convert.ToInt32(pr.idLevelCheckerCarbonCopy.GetValue(a).ToString());
                            Guid idUnitCheckerCarbonCopy = new Guid(pr.idUnitCheckerCarbonCopy.GetValue(a).ToString());
                            prApiCheckerCarbonCopy.Add(new ParamInsertCheckerMemoCarbonCopy
                            {
                                idUserCheckerCarbonCopy = idCheckerCarbonCopy,
                                idLevelCheckerCarbonCopy = idLevelCarbonCopy,
                                idUnitCheckerCarbonCopy = idUnitCheckerCarbonCopy
                            });
                        }
                    }

                    #endregion

                    if (pr.idUserChecker.Length > 0)
                    {
                        for (int a = 0; a < pr.idUserChecker.Length; a++)
                        {
                            Guid idChecker = new Guid(pr.idUserChecker.GetValue(a).ToString());
                            int idLevel = Convert.ToInt32(pr.idLevelChecker.GetValue(a).ToString());
                            Guid idUnitChecker = new Guid(pr.idUnitChecker.GetValue(a).ToString());
                            prApiChecker.Add(new ParamInsertCheckerMemo
                            {
                                idUserChecker = idChecker,
                                idLevelChecker = idLevel,
                                idUnitChecker = idUnitChecker
                            });
                        }
                    }
                    var checksenderischecker = prApiChecker.Where(p => p.idUserChecker == pr.bossUserId).FirstOrDefault();
                    if (checksenderischecker == null)
                    {
                        prApiChecker.Add(new ParamInsertCheckerMemo
                        {
                            idUserChecker = pr.bossUserId,
                            idLevelChecker = pr.bossLevelId,
                            idUnitChecker = pr.bossUnitId
                        });
                    }

                    // divisi lainya

                    if (pr.idUserCheckerlain.Length > 0)
                    {
                        for (int a = 0; a < pr.idUserCheckerlain.Length; a++)
                        {
                            Guid idChecker = new Guid(pr.idUserCheckerlain.GetValue(a).ToString());
                            int idLevel = Convert.ToInt32(pr.idLevelCheckerlain.GetValue(a).ToString());
                            Guid idUnitChecker = new Guid(pr.idUnitCheckerlain.GetValue(a).ToString());
                            prApiPemeriksaLainya.Add(new ParamInsertCheckerMemoLainya
                            {
                                idUserCheckerlain = idChecker,
                                idLevelCheckerlain = idLevel,
                                idUnitCheckerlain = idUnitChecker
                            });
                        }
                    }

                    prApi.idUserChecker = prApiChecker;
                    prApi.idUserCheckerPenerima = prApiCheckerPenerima;
                    prApi.idUserCheckerCarbonCopy = prApiCheckerCarbonCopy;
                    prApi.idUserCheckerDelibretion = prApiDelibretion;
                    prApi.idUserCheckerlain = prApiPemeriksaLainya;
                    //prApi.senderName = prApiReceiver;
                }

                prApi.idresponsesurat = pr.idresponsesurat;
                prApi.saveType = pr.saveType;
                prApi.comment = pr.comment;

                generalOutput = await _dataAccessProvider.InsertLetterMemo("Memo/InsertMemoLetter", token, prApi);
                var jsonApiResponseSerialize = JsonConvert.SerializeObject(generalOutput.Result);

                var pesanPopup = "Surat berhasil dipreview";

                resultPreview.Status = generalOutput.Status;
                resultPreview.Result = generalOutput.Result.ToString();
                resultPreview.Message = pesanPopup;
                var json = JsonConvert.SerializeObject(resultPreview);

                return json;
            }
            catch (Exception ex)
            {
                resultPreview.Status = "NG";
                resultPreview.Result = pr.idresponsesurat.ToString();
                resultPreview.Message = "Gagal preview surat keluar";

                var json = JsonConvert.SerializeObject(resultPreview);

                return json;
            }
        }


        public async Task<IActionResult> DistributionMemo()
        {
            MemoOutputWeb letterList = new MemoOutputWeb();
            GeneralOutputModel generalOutput = new GeneralOutputModel();
            var token = HttpContext.Session.GetString("token");
            List<StringmapMemoOutput> jenisMemo = new List<StringmapMemoOutput>();
            List<StringmapOutput> LetterTypeList = new List<StringmapOutput>();
            ParamGetMemoWeb pr = new ParamGetMemoWeb();
            pr.draw = "1";
            pr.sortColumn = "";
            pr.sortColumnDirection = "";
            pr.searchValue = "";
            pr.pageSize = 20;
            pr.start = 0;

            generalOutput = await _dataAccessProvider.GetMemoDistribusi("Memo/GetDistribusiMemoWeb/", token, pr);
            var jsonApiResponseSerialize = JsonConvert.SerializeObject(generalOutput.Result);
            letterList = JsonConvert.DeserializeObject<MemoOutputWeb>(jsonApiResponseSerialize);

            ParamGetStringmap prr = new ParamGetStringmap();
            prr.objectName = "tm_letter";
            prr.attributeName = "LETTER_TYPE_CODE";
            generalOutput = await _dataAccessProvider.GetStringmapAsync("General/GetStringMapMemoSrchMemoWM/", prr, token);
            jsonApiResponseSerialize = JsonConvert.SerializeObject(generalOutput.Result);
            LetterTypeList = JsonConvert.DeserializeObject<List<StringmapOutput>>(jsonApiResponseSerialize);

            prr.objectName = "tm_letter_Memo";
            prr.attributeName = "OUTBOX_TYPE_CODE";
            generalOutput = await _dataAccessProvider.GetStringmapAsync("Memo/GetStringmapMemoWeb/", prr, token);
            jsonApiResponseSerialize = JsonConvert.SerializeObject(generalOutput.Result);
            jenisMemo = JsonConvert.DeserializeObject<List<StringmapMemoOutput>>(jsonApiResponseSerialize);

            ViewBag.LetterTypeList = LetterTypeList;
            ViewBag.JenisMemo = jenisMemo;
            return View(letterList);
        }

        public async Task<IActionResult> DraftMemo()
        {
            MemoOutputWeb letterList = new MemoOutputWeb();
            GeneralOutputModel generalOutput = new GeneralOutputModel();
            var token = HttpContext.Session.GetString("token");
            List<StringmapOutput> LetterTypeList = new List<StringmapOutput>();
            ParamGetMemoWeb pr = new ParamGetMemoWeb();
            pr.draw = "1";
            pr.sortColumn = "";
            pr.sortColumnDirection = "";
            pr.searchValue = "";
            pr.pageSize = 20;
            pr.start = 0;

            generalOutput = await _dataAccessProvider.GetMemoDistribusi("Memo/GetDraftMemoWeb/", token, pr);
            var jsonApiResponseSerialize = JsonConvert.SerializeObject(generalOutput.Result);
            letterList = JsonConvert.DeserializeObject<MemoOutputWeb>(jsonApiResponseSerialize);

            ParamGetStringmap prr = new ParamGetStringmap();
            prr.objectName = "tm_letter";
            prr.attributeName = "LETTER_TYPE_CODE";
            generalOutput = await _dataAccessProvider.GetStringmapAsync("General/GetStringMapMemoSrchMemoWM/", prr, token);
            jsonApiResponseSerialize = JsonConvert.SerializeObject(generalOutput.Result);
            LetterTypeList = JsonConvert.DeserializeObject<List<StringmapOutput>>(jsonApiResponseSerialize);

            ViewBag.LetterTypeList = LetterTypeList;
            return View(letterList);
        }


        [Route("Memo/DetailMemo/{id}/{letterType}")]
        public async Task<IActionResult> DetailMemo(Guid id,string letterType)
        {
            OutputDetailMemo letterDetail = new OutputDetailMemo();
            GeneralOutputModel generalOutput = new GeneralOutputModel();
            List<OutputContentTemplate> output = new List<OutputContentTemplate>();
            List<StringmapMemoOutput> jenisMemo = new List<StringmapMemoOutput>();
            List<MemotypeOuput> tipeMemo = new List<MemotypeOuput>();
            var token = HttpContext.Session.GetString("token");
            ParamGetDetailMemo pr = new ParamGetDetailMemo();
            pr.idLetter = id;
            pr.lettertype=letterType;
            generalOutput = await _dataAccessProvider.GetDetailMemo("Memo/GetDetailMemoWeb/", token, pr);
            var jsonApiResponseSerialize = JsonConvert.SerializeObject(generalOutput.Result);
            letterDetail = JsonConvert.DeserializeObject<OutputDetailMemo>(jsonApiResponseSerialize);

            generalOutput = await _dataAccessProvider.GetContentTemplate("Template/GetTemplate/", token);
            jsonApiResponseSerialize = JsonConvert.SerializeObject(generalOutput.Result);
            output = JsonConvert.DeserializeObject<List<OutputContentTemplate>>(jsonApiResponseSerialize);

            ParamGetStringmap prr = new ParamGetStringmap();
            prr.objectName = "tm_letter_Memo";
            prr.attributeName = "OUTBOX_TYPE_CODE";
            generalOutput = await _dataAccessProvider.GetStringmapAsync("Memo/GetStringmapMemoWeb/", prr, token);
            jsonApiResponseSerialize = JsonConvert.SerializeObject(generalOutput.Result);
            jenisMemo = JsonConvert.DeserializeObject<List<StringmapMemoOutput>>(jsonApiResponseSerialize);

            ParamGetMemoTypeById prrr = new ParamGetMemoTypeById();
            prrr.stingMapVal = letterDetail.letter.outboxType;
            generalOutput = await _dataAccessProvider.GetMemotypeByIdAsync("Memo/GetMemotypeByIdWeb/", token, prrr);
            var jsonApiResponseSerializeee = JsonConvert.SerializeObject(generalOutput.Result);
            tipeMemo = JsonConvert.DeserializeObject<List<MemotypeOuput>>(jsonApiResponseSerializeee);


            ViewBag.TemplateData = output;
            ViewBag.JenisMemo = jenisMemo;
            ViewBag.TipeMemo = tipeMemo;
            return View(letterDetail);
        }


        [HttpPost]
        public async Task<string> DeleteAttachmentMemo(Guid idFile)
        {
            var jsonApiResponseSerialize = "";
            GeneralOutputModel generalOutput = new GeneralOutputModel();
            try
            {
                var token = HttpContext.Session.GetString("token");
                ParamDeleteAttachmentMemo prApi = new ParamDeleteAttachmentMemo();
                prApi.idAttachment = idFile;

                generalOutput = await _dataAccessProvider.DeleteAttachmentMemo("Memo/DeleteAttachmentMemoWeb", token, prApi);
                jsonApiResponseSerialize = JsonConvert.SerializeObject(generalOutput);


                return jsonApiResponseSerialize;
            }
            catch (Exception ex)
            {
                generalOutput.Status = "NG";
                generalOutput.Message = ex.ToString();

                jsonApiResponseSerialize = JsonConvert.SerializeObject(generalOutput);

                return jsonApiResponseSerialize;
            }
        }


        [HttpPost]
        public async Task<string> LetterNotifSecretary(Guid idLetter)
        {
            OutputDetailMemo letterDetail = new OutputDetailMemo();
            GeneralOutputModel generalOutput = new GeneralOutputModel();
            var token = HttpContext.Session.GetString("token");
            ParamGetDetailMemo pr = new ParamGetDetailMemo();
            pr.idLetter = idLetter;

            generalOutput = await _dataAccessProvider.GetDetailMemo("Memo/MemoLetterNotifSecretaryWeb/", token, pr);
            var jsonApiResponseSerialize = JsonConvert.SerializeObject(generalOutput);

            return jsonApiResponseSerialize;
        }





        [HttpPost]
        public async Task<IActionResult> ApprovalLetterMemo(ParamApprovalLetterMemo pr, IFormFile inputfile)
        {
            try
            {
                var token = HttpContext.Session.GetString("token");
                GeneralOutputModel generalOutput = new GeneralOutputModel();

                if (pr.idUserDelibration.ToString() != "")
                {
                    if (pr.commentdlbrt != "")
                    {
                        generalOutput = await _dataAccessProvider.ApprovalDelebrationMemo("Memo/ApprovalDelebrationMemoWeb", token, pr);
                        var jsonApiResponseSerialize = JsonConvert.SerializeObject(generalOutput.Result);
                        var titlePopup = "berhasil";
                        var pesanPopup = "berhasil";
                        if (generalOutput.Status == "OK")
                        {
                            TempData["status"] = "OK";
                            TempData["title"] = titlePopup;
                            TempData["pesan"] = pesanPopup;
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
                            TempData["title"] = generalOutput.Message;
                            TempData["pesan"] = generalOutput.Message;
                        }


                    }
                    else
                    {
                        TempData["status"] = "NG";
                        TempData["title"] = generalOutput.Message;
                        TempData["pesan"] = generalOutput.Message;
                    }
                }
                else
                {
                    generalOutput = await _dataAccessProvider.ApprovalMemo("Memo/ApprovalMemoWeb", token, pr);
                    var jsonApiResponseSerialize = JsonConvert.SerializeObject(generalOutput.Result);

                    

                    var titlePopup = "Memo berhasil Disetujui";
                    var pesanPopup = "Memo berhasil Disetujui";
                    if (pr.saveType == 5)
                    {
                        titlePopup = "Memo berhasil ditolak";
                        pesanPopup = "Memo berhasil ditolak";
                    }
                    if (generalOutput.Status == "OK")
                    {
                        ParamPushNotifikasi prn = new ParamPushNotifikasi();
                        prn.idletter = pr.idresponsesurat;
                        generalOutput = await _dataAccessProvider.PushNotifikasi("General/PushNotifikasiToMobilePublic/", token, prn);

                        TempData["status"] = "OK";
                        TempData["title"] = titlePopup;
                        TempData["pesan"] = pesanPopup;
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
                        TempData["title"] = generalOutput.Message;
                        TempData["pesan"] = generalOutput.Message;
                    }

                    return Redirect("~/Inbox/Index");
                }



                return Redirect("~/Inbox/Index");

            }
            catch (Exception ex)
            {
                TempData["status"] = "NG";
                TempData["title"] = "Gagal melakukan aksi pada Memo";
                TempData["pesan"] = ex.ToString();
                return Redirect("~/Inbox/Index");
            }
        }


        #region Pengadaan
        public async Task<IActionResult> MemoPengadaan()
        {
            GeneralOutputModel generalOutput = new GeneralOutputModel();
            List<OutputContentTemplate> output = new List<OutputContentTemplate>();
            List<StringmapMemoOutput> jenisMemo = new List<StringmapMemoOutput>();
            List<PengadaanMemoOutputModel> pengadaanMemoList = new List<PengadaanMemoOutputModel>();
            var token = HttpContext.Session.GetString("token");
           
            string NoLetter = "";
            generalOutput = await _dataAccessProvider.GetContentTemplate("Template/GetTemplate/", token);
            var jsonApiResponseSerialize = JsonConvert.SerializeObject(generalOutput.Result);
            output = JsonConvert.DeserializeObject<List<OutputContentTemplate>>(jsonApiResponseSerialize);

            generalOutput = await _dataAccessProvider.GetDataPengadaan("Memo/GetAllPengadaanWeb/", token);
            var jsonApiResponseSerializee = JsonConvert.SerializeObject(generalOutput.Result);
            pengadaanMemoList = JsonConvert.DeserializeObject<List<PengadaanMemoOutputModel>>(jsonApiResponseSerializee);


            ParamGetStringmap pr = new ParamGetStringmap();
            pr.objectName = "tm_letter_Memo";
            pr.attributeName = "OUTBOX_TYPE_CODE";
            generalOutput = await _dataAccessProvider.GetStringmapAsync("Memo/GetStringmapMemoWeb/", pr, token);
            jsonApiResponseSerialize = JsonConvert.SerializeObject(generalOutput.Result);
            jenisMemo = JsonConvert.DeserializeObject<List<StringmapMemoOutput>>(jsonApiResponseSerialize);

            if (generalOutput.Status == "UA")
            {
                TempData["status"] = "NG";
                TempData["pesan"] = "Session habis silahkan login kembali";
                return RedirectToAction("Logout", "Home");
            }

            ViewBag.NoLetter = NoLetter;
            ViewBag.TemplateData = output;
            ViewBag.Pengadaan = pengadaanMemoList;
            ViewBag.JenisMemo = jenisMemo;
            return View();
        }

        [HttpPost]
        public async Task<string> GetMinMaxNominalById(string MinMaxNomnal, Guid idMinMax)
        {
            DocumentOutputWeb documentOutput = new DocumentOutputWeb();
            GeneralOutputModel generalOutput = new GeneralOutputModel();
            var token = HttpContext.Session.GetString("token");

            ParamCheckMinMaxNomial pr = new ParamCheckMinMaxNomial();
            pr.minMaxNomnal = MinMaxNomnal;
            pr.IdPengadaan = idMinMax;
            generalOutput = await _dataAccessProvider.GetDataMinMaxNomialByIdAsync("Memo/GetDataMinMaxNomialByIdWeb/", token, pr);
            var jsonApiResponseSerialize = JsonConvert.SerializeObject(generalOutput);

            return jsonApiResponseSerialize;
        }

        [HttpPost]
        public async Task<string> GetTipePengadaan(Guid idPengadaan)
        {
            
            GeneralOutputModel generalOutput = new GeneralOutputModel();
            var token = HttpContext.Session.GetString("token");

            ParamGetMemoPengadaanById pr = new ParamGetMemoPengadaanById();
            pr.idPengadaan = idPengadaan;

            generalOutput = await _dataAccessProvider.GetPengadaanByIdAsync("Memo/GetPengadaanByIdWeb/", token, pr);
            var jsonApiResponseSerialize = JsonConvert.SerializeObject(generalOutput);
            //generalOutput = JsonConvert.DeserializeObject<GeneralOutputModel>(jsonApiResponseSerialize);

            return jsonApiResponseSerialize;
        }

        #endregion


        [HttpPost]
        public async Task<string> GetTypeMemo(int outboxType)
        {

            GeneralOutputModel generalOutput = new GeneralOutputModel();
            var token = HttpContext.Session.GetString("token");

            ParamGetMemoTypeById pr = new ParamGetMemoTypeById();
            pr.stingMapVal = outboxType;

            generalOutput = await _dataAccessProvider.GetMemotypeByIdAsync("Memo/GetMemotypeByIdWeb/", token, pr);
            var jsonApiResponseSerialize = JsonConvert.SerializeObject(generalOutput);
            //generalOutput = JsonConvert.DeserializeObject<GeneralOutputModel>(jsonApiResponseSerialize);

            return jsonApiResponseSerialize;
        }

        [HttpPost]
        public async Task<string> GetApprovalPengadaan(Guid idPengadaan)
        {

            GeneralOutputModel generalOutput = new GeneralOutputModel();
            var token = HttpContext.Session.GetString("token");

            ParamGetMemoPengadaanById pr = new ParamGetMemoPengadaanById();
            pr.idPengadaan = idPengadaan;

            generalOutput = await _dataAccessProvider.GetPengadaanByIdAsync("Memo/GetApprovalPengadaandWeb/", token, pr);
            var jsonApiResponseSerialize = JsonConvert.SerializeObject(generalOutput);
            //generalOutput = JsonConvert.DeserializeObject<GeneralOutputModel>(jsonApiResponseSerialize);

            return jsonApiResponseSerialize;
        }



        [HttpPost]
        public async Task<IActionResult> SaveMemoPengadaan(ParamInsertMemoWeb pr, IFormFile inputfile)
        {
            try
            {

                var token = HttpContext.Session.GetString("token");
                GeneralOutputModel generalOutput = new GeneralOutputModel();
                ParamInsertMemoPengadaan prApi = new ParamInsertMemoPengadaan();
                List<ParamInsertCheckerMemo> prApiChecker = new List<ParamInsertCheckerMemo>();
                List<ParamInsertCheckerMemoPengirim> prApiCheckerPengirim = new List<ParamInsertCheckerMemoPengirim>();
                List<ParamInsertCheckerMemoPenerima> prApiCheckerPenerima = new List<ParamInsertCheckerMemoPenerima>();
                List<ParamInsertCheckerMemoCarbonCopy> prApiCheckerCarbonCopy = new List<ParamInsertCheckerMemoCarbonCopy>();
                List<ParamInsertCheckerMemoDelibretion> prApiDelibretion = new List<ParamInsertCheckerMemoDelibretion>();
                List<ParamInsertApprovalPengadaan> prApiApprovalPengadaan = new List<ParamInsertApprovalPengadaan>();
                List<ParamInsertApproverMemo> prApiApprover = new List<ParamInsertApproverMemo>();
                List<ParamInsertSendMemogoingRecipient> prApiReceiver = new List<ParamInsertSendMemogoingRecipient>();
                ParamInsertAttachmentMemo prAttachment = new ParamInsertAttachmentMemo();
                OutputInsertAttachmentMemo responseApiAttachment = new OutputInsertAttachmentMemo();
                List<ParamInsertCheckerMemoLainya> prApiPemeriksaLainya = new List<ParamInsertCheckerMemoLainya>();
                int print = pr.saveType;
                if (pr.saveType == 3)
                {
                    pr.saveType = 1;
                }
                if (pr.saveType == 1 || pr.saveType == 2)
                {
                    if (inputfile != null)
                    {
                        string path = Path.Combine(_environment.WebRootPath, "uploads/letter");
                        if (!Directory.Exists(path))
                        {
                            Directory.CreateDirectory(path);
                        }
                        string fileName = Path.GetFileName(inputfile.FileName);
                        string filePath = Path.Combine(path, fileName);
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await inputfile.CopyToAsync(stream);
                        }
                        prAttachment.idLetter = pr.idresponsesurat;
                        prAttachment.filename = inputfile.FileName;
                        prAttachment.isDocLetter = 1;

                        generalOutput = await _dataAccessProvider.InsertAttachmentMemo("Memo/InsertAttachmentMemo", token, prAttachment);
                        var jsonApiResponseSerializes = JsonConvert.SerializeObject(generalOutput.Result);
                        responseApiAttachment = JsonConvert.DeserializeObject<OutputInsertAttachmentMemo>(jsonApiResponseSerializes);
                        if (generalOutput.Status == "NG")
                        {
                            TempData["status"] = "NG";
                            TempData["title"] = generalOutput.Message;
                            TempData["pesan"] = generalOutput.Message;
                        }
                        else if (generalOutput.Status == "UA")
                        {
                            TempData["status"] = "NG";
                            TempData["pesan"] = "Session habis silahkan login kembali";
                            return RedirectToAction("Logout", "Home");
                        }
                    }
                    //string idpd = pr.idMinMax.ToString() == "" : "";
                    var idpd = pr.idMinMax.ToString() == "" ? pr.idMinMax2.ToString() : pr.idMinMax.ToString();
                    prApi.outboxType = pr.outboxType;
                    prApi.idMemoType = pr.idMemoType;
                    prApi.letterTypeCode = pr.letterTypeCode;
                    prApi.letterDate = pr.letterDate;
                    prApi.about = pr.about;
                    prApi.resultType = pr.resultType;
                    prApi.signatureType = pr.signatureType;
                    prApi.bossPositionId = pr.bossPositionId;
                    prApi.bossUserId = pr.bossUserId;
                    prApi.bossUnitId = pr.bossUnitId;
                    prApi.bossLevelId = pr.bossLevelId;
                    prApi.attachmentCount = pr.attachmentCount;
                    prApi.priority = pr.priority;

                    prApi.isiSurat = pr.isiSurat;
                    prApi.idMixMax = pr.idMinMax;
                    prApi.MinMaxNomnal = pr.MinMaxNomnal;

                    #region Memo To (Penerima)
                    if (pr.idUserCheckerPenerima.Length > 0)
                    {
                        for (int a = 0; a < pr.idUserCheckerPenerima.Length; a++)
                        {
                            Guid idCheckerPenerima = new Guid(pr.idUserCheckerPenerima.GetValue(a).ToString());
                            int idLevelPenerima = Convert.ToInt32(pr.idLevelCheckerPenerima.GetValue(a).ToString());
                            Guid idUnitCheckerPenerima = new Guid(pr.idUnitCheckerPenerima.GetValue(a).ToString());
                            prApiCheckerPenerima.Add(new ParamInsertCheckerMemoPenerima
                            {
                                idUserCheckerPenerima = idCheckerPenerima,
                                idLevelCheckerPenerima = idLevelPenerima,
                                idUnitCheckerPenerima = idUnitCheckerPenerima
                            });
                        }
                    }

                    #endregion

                    #region Memo CC (CarbonCopy)
                    if (pr.idUserCheckerCarbonCopy.Length > 0)
                    {
                        for (int a = 0; a < pr.idUserCheckerCarbonCopy.Length; a++)
                        {
                            Guid idCheckerCarbonCopy = new Guid(pr.idUserCheckerCarbonCopy.GetValue(a).ToString());
                            int idLevelCarbonCopy = Convert.ToInt32(pr.idLevelCheckerCarbonCopy.GetValue(a).ToString());
                            Guid idUnitCheckerCarbonCopy = new Guid(pr.idUnitCheckerCarbonCopy.GetValue(a).ToString());
                            prApiCheckerCarbonCopy.Add(new ParamInsertCheckerMemoCarbonCopy
                            {
                                idUserCheckerCarbonCopy = idCheckerCarbonCopy,
                                idLevelCheckerCarbonCopy = idLevelCarbonCopy,
                                idUnitCheckerCarbonCopy = idUnitCheckerCarbonCopy
                            });
                        }
                    }

                    #endregion




                    #region Delibretion
                    if (pr.idUserCheckerDelibretion.Length > 0)
                    {
                        for (int a = 0; a < pr.idUserCheckerDelibretion.Length; a++)
                        {
                            Guid idCheckerDelibretion = new Guid(pr.idUserCheckerDelibretion.GetValue(a).ToString());
                            int idLevelDelibretion = Convert.ToInt32(pr.idLevelCheckerDelibretion.GetValue(a).ToString());
                            Guid idUnitCheckerDelibretion = new Guid(pr.idUnitCheckerDelibretion.GetValue(a).ToString());
                            prApiDelibretion.Add(new ParamInsertCheckerMemoDelibretion
                            {
                                idUserCheckerDelibretion = idCheckerDelibretion,
                                idLevelCheckerDelibretion = idLevelDelibretion,
                                idUnitCheckerDelibretion = idUnitCheckerDelibretion
                            });
                        }
                    }
                    #endregion

                    #region User Approval Pengadaaan

                    if (pr.idUserApprovalPengadaan.Length > 0)
                    {
                        for (int a = 0; a < pr.idUserApprovalPengadaan.Length; a++)
                        {
                            Guid idUserApprovalPengadaan = new Guid(pr.idUserApprovalPengadaan.GetValue(a).ToString());
                            Guid idUserPositionApprovalPengadaan = new Guid(pr.idUserApprovalPositionPengadaan.GetValue(a).ToString());
                            Guid idUserUnitApprovalPengadaan = new Guid(pr.idUserApprovalunitPengadaan.GetValue(a).ToString());
                            prApiApprovalPengadaan.Add(new ParamInsertApprovalPengadaan
                            {
                                idUserApprovalPengadaan = idUserApprovalPengadaan,
                                idUserApprovalPositionPengadaan = idUserPositionApprovalPengadaan,
                                idUserApprovalunitPengadaan = idUserUnitApprovalPengadaan,
                                is_approver = 1,
                            });
                        }
                    }

                    if (pr.idUserApprover.Length > 0)
                    {
                        for (int a = 0; a < pr.idUserApprover.Length; a++)
                        {
                            Guid idApprover = new Guid(pr.idUserApprover.GetValue(a).ToString());
                            int idLevel = Convert.ToInt32(pr.idLevelApprover.GetValue(a).ToString());
                            Guid idUnitApprover = new Guid(pr.idUnitApprover.GetValue(a).ToString());
                            prApiApprover.Add(new ParamInsertApproverMemo
                            {
                                idUserApprover = idApprover,
                                idLevelApprover = idLevel,
                                idUnitApprover = idUnitApprover,
                                is_approver = 2,
                            });
                        }
                    }
                    #endregion

                    if (pr.idUserChecker.Length > 0)
                    {
                        for (int a = 0; a < pr.idUserChecker.Length; a++)
                        {
                            Guid idChecker = new Guid(pr.idUserChecker.GetValue(a).ToString());
                            int idLevel = Convert.ToInt32(pr.idLevelChecker.GetValue(a).ToString());
                            Guid idUnitChecker = new Guid(pr.idUnitChecker.GetValue(a).ToString());
                            prApiChecker.Add(new ParamInsertCheckerMemo
                            {
                                idUserChecker = idChecker,
                                idLevelChecker = idLevel,
                                idUnitChecker = idUnitChecker
                            });
                        }
                    }

                    var checksenderischecker = prApiChecker.Where(p => p.idUserChecker == pr.bossUserId).FirstOrDefault();
                    if (checksenderischecker == null)
                    {
                        prApiChecker.Add(new ParamInsertCheckerMemo
                        {
                            idUserChecker = pr.bossUserId,
                            idLevelChecker = pr.bossLevelId,
                            idUnitChecker = pr.bossUnitId
                        });
                    }

                    // divisi lainya

                    if (pr.idUserCheckerlain.Length > 0)
                    {
                        for (int a = 0; a < pr.idUserCheckerlain.Length; a++)
                        {
                            Guid idChecker = new Guid(pr.idUserCheckerlain.GetValue(a).ToString());
                            int idLevel = Convert.ToInt32(pr.idLevelCheckerlain.GetValue(a).ToString());
                            Guid idUnitChecker = new Guid(pr.idUnitCheckerlain.GetValue(a).ToString());
                            prApiPemeriksaLainya.Add(new ParamInsertCheckerMemoLainya
                            {
                                idUserCheckerlain = idChecker,
                                idLevelCheckerlain = idLevel,
                                idUnitCheckerlain = idUnitChecker
                            });
                        }
                    }

                    prApi.idUserChecker = prApiChecker;
                    prApi.idUserCheckerPengirim = prApiCheckerPengirim;
                    prApi.idUserCheckerPenerima = prApiCheckerPenerima;
                    prApi.idUserCheckerCarbonCopy = prApiCheckerCarbonCopy;
                    prApi.idUserCheckerDelibretion = prApiDelibretion;
                    prApi.idUserApprovalPengadaan = prApiApprovalPengadaan;
                    prApi.idUserApprover = prApiApprover;
                    prApi.idUserCheckerlain = prApiPemeriksaLainya;

                    //prApi.senderName = prApiReceiver;
                }

                prApi.idresponsesurat = responseApiAttachment.idLetter == Guid.Empty ? pr.idresponsesurat : responseApiAttachment.idLetter;
                prApi.saveType = pr.saveType;
                prApi.comment = pr.comment;
                prApi.summary = pr.summary;
                //prApi.senderCompanyName = pr.senderCompanyName;

                generalOutput = await _dataAccessProvider.InsertMemoPengadaan("Memo/InsertMemoPengadaanWeb", token, prApi);
                var jsonApiResponseSerialize = JsonConvert.SerializeObject(generalOutput.Result);
                var titlePopup = "Memo berhasil disimpan";
                var pesanPopup = "Memo berhasil disimpan";
                if (pr.saveType == 4)
                {
                    titlePopup = "Memo berhasil dikirim";
                    pesanPopup = "Memo berhasil dikirim";
                }
                if (pr.saveType == 2)
                {
                    titlePopup = "Memo berhasil dikirim";
                    pesanPopup = "Memo berhasil dikirim";
                }
                if (pr.saveType == 5)
                {
                    titlePopup = "Memo berhasil ditolak";
                    pesanPopup = "Memo berhasil ditolak";
                }
                if (generalOutput.Status == "OK")
                {
                    TempData["status"] = "OK";
                    TempData["title"] = titlePopup;
                    TempData["pesan"] = pesanPopup;
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
                    TempData["title"] = generalOutput.Message;
                    TempData["pesan"] = generalOutput.Message;
                }
                if (print == 3)
                {
                    if (generalOutput.Status == "OK")
                    {
                        Guid idLetter = new Guid(generalOutput.Result.ToString());
                        //await PrintLetter(idLetter);
                    }
                }
                if (pr.saveType != 1)
                {
                    return Redirect("~/Memo/DistributionMemo");
                }
                else
                {
                    return Redirect("~/Memo/DraftMemo");
                }


            }
            catch (Exception ex)
            {
                TempData["status"] = "NG";
                TempData["title"] = " Memo pengadaan gagal dikirim";
                TempData["pesan"] = ex.ToString();
                return Redirect("~/Memo/NewMemo");
            }
        }

        [Route("Memo/DetailMemoPengadaan/{id}/{letterType}")]
        public async Task<IActionResult> DetailMemoPengadaan(Guid id,string typeletter)
        {
            OutputDetailMemoPengadaan letterDetail = new OutputDetailMemoPengadaan();
            GeneralOutputModel generalOutput = new GeneralOutputModel();
            List<OutputContentTemplate> output = new List<OutputContentTemplate>();
            List<StringmapMemoOutput> jenisMemo = new List<StringmapMemoOutput>();
            List<MemotypeOuput> tipeMemo = new List<MemotypeOuput>();
            List<PengadaanMemoOutputModel> pengadaanMemoList = new List<PengadaanMemoOutputModel>();
            var token = HttpContext.Session.GetString("token");
            ParamGetDetailMemo pr = new ParamGetDetailMemo();
            pr.idLetter = id;
            pr.lettertype=typeletter;
            generalOutput = await _dataAccessProvider.GetDetailMemoPengadaan("Memo/GetDetailMemoPengadaanWeb/", token, pr);
            var jsonApiResponseSerialize = JsonConvert.SerializeObject(generalOutput.Result);
            letterDetail = JsonConvert.DeserializeObject<OutputDetailMemoPengadaan>(jsonApiResponseSerialize);

            generalOutput = await _dataAccessProvider.GetContentTemplate("Template/GetTemplate/", token);
            jsonApiResponseSerialize = JsonConvert.SerializeObject(generalOutput.Result);
            output = JsonConvert.DeserializeObject<List<OutputContentTemplate>>(jsonApiResponseSerialize);

            ParamGetStringmap prr = new ParamGetStringmap();
            prr.objectName = "tm_letter_Memo";
            prr.attributeName = "OUTBOX_TYPE_CODE";
            generalOutput = await _dataAccessProvider.GetStringmapAsync("Memo/GetStringmapMemoWeb/", prr, token);
            jsonApiResponseSerialize = JsonConvert.SerializeObject(generalOutput.Result);
            jenisMemo = JsonConvert.DeserializeObject<List<StringmapMemoOutput>>(jsonApiResponseSerialize);

            ParamGetMemoTypeById prrr = new ParamGetMemoTypeById();
            prrr.stingMapVal = letterDetail.letter.outboxType;
            generalOutput = await _dataAccessProvider.GetMemotypeByIdAsync("Memo/GetMemotypeByIdWeb/", token, prrr);
            var jsonApiResponseSerializeee = JsonConvert.SerializeObject(generalOutput.Result);
            tipeMemo = JsonConvert.DeserializeObject<List<MemotypeOuput>>(jsonApiResponseSerializeee);


            generalOutput = await _dataAccessProvider.GetDataPengadaan("Memo/GetAllPengadaanWeb/", token);
            var jsonApiResponseSerializee = JsonConvert.SerializeObject(generalOutput.Result);
            pengadaanMemoList = JsonConvert.DeserializeObject<List<PengadaanMemoOutputModel>>(jsonApiResponseSerializee);

            var nominal= String.Format(CultureInfo.CreateSpecificCulture("id-id"), "{0:N}", letterDetail.Nominal.nominal);
            ViewBag.TemplateData = output;
            ViewBag.JenisMemo = jenisMemo;
            ViewBag.TipeMemo = tipeMemo;
            ViewBag.Pengadaan = pengadaanMemoList;
            ViewBag.Nominall = nominal;
            return View(letterDetail);
        }


        [HttpPost]
        public async Task<IActionResult> UpdateMemoPengadaan(ParamInsertMemoWeb pr, IFormFile inputfile)
        {
            try
            {
                var token = HttpContext.Session.GetString("token");
                GeneralOutputModel generalOutput = new GeneralOutputModel();
                ParamInsertMemoPengadaan prApi = new ParamInsertMemoPengadaan();
                List<ParamInsertCheckerMemo> prApiChecker = new List<ParamInsertCheckerMemo>();
                List<ParamInsertCheckerMemoPengirim> prApiCheckerPengirim = new List<ParamInsertCheckerMemoPengirim>();
                List<ParamInsertCheckerMemoPenerima> prApiCheckerPenerima = new List<ParamInsertCheckerMemoPenerima>();
                List<ParamInsertCheckerMemoCarbonCopy> prApiCheckerCarbonCopy = new List<ParamInsertCheckerMemoCarbonCopy>();
                List<ParamInsertCheckerMemoDelibretion> prApiDelibretion = new List<ParamInsertCheckerMemoDelibretion>();
                List<ParamInsertApprovalPengadaan> prApiApprovalPengadaan = new List<ParamInsertApprovalPengadaan>();
                List<ParamInsertApproverMemo> prApiApprover = new List<ParamInsertApproverMemo>();
                List<ParamInsertSendMemogoingRecipient> prApiReceiver = new List<ParamInsertSendMemogoingRecipient>();
                ParamInsertAttachmentMemo prAttachment = new ParamInsertAttachmentMemo();
                OutputInsertAttachmentMemo responseApiAttachment = new OutputInsertAttachmentMemo();
                List<ParamInsertCheckerMemoLainya> prApiPemeriksaLainya = new List<ParamInsertCheckerMemoLainya>();
                int print = pr.saveType;
                if (pr.saveType == 3)
                {
                    pr.saveType = 1;
                }
                if (pr.saveType == 1 || pr.saveType == 2)
                {
                    if (inputfile != null)
                    {
                        string path = Path.Combine(_environment.WebRootPath, "uploads/letter");
                        if (!Directory.Exists(path))
                        {
                            Directory.CreateDirectory(path);
                        }
                        string fileName = Path.GetFileName(inputfile.FileName);
                        string filePath = Path.Combine(path, fileName);
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await inputfile.CopyToAsync(stream);
                        }
                        prAttachment.idLetter = pr.idresponsesurat;
                        prAttachment.filename = inputfile.FileName;
                        prAttachment.isDocLetter = 1;

                        generalOutput = await _dataAccessProvider.InsertAttachmentMemo("Memo/InsertAttachmentMemo", token, prAttachment);
                        var jsonApiResponseSerializes = JsonConvert.SerializeObject(generalOutput.Result);
                        responseApiAttachment = JsonConvert.DeserializeObject<OutputInsertAttachmentMemo>(jsonApiResponseSerializes);
                        if (generalOutput.Status == "NG")
                        {
                            TempData["status"] = "NG";
                            TempData["title"] = generalOutput.Message;
                            TempData["pesan"] = generalOutput.Message;
                        }
                        else if (generalOutput.Status == "UA")
                        {
                            TempData["status"] = "NG";
                            TempData["pesan"] = "Session habis silahkan login kembali";
                            return RedirectToAction("Logout", "Home");
                        }
                    }
                    //string idpd = pr.idMinMax.ToString() == "" : "";
                    var idpd = pr.idMinMax.ToString() == "00000000-0000-0000-0000-000000000000" ? pr.idMinMax2.ToString() : pr.idMinMax.ToString();
                    Guid PdIdMinMax = new Guid(idpd);
                    prApi.outboxType = pr.outboxType;
                    prApi.idMemoType = pr.idMemoType;
                    prApi.letterTypeCode = pr.letterTypeCode;
                    prApi.letterDate = pr.letterDate;
                    prApi.about = pr.about;
                    prApi.resultType = pr.resultType;
                    prApi.signatureType = pr.signatureType;
                    prApi.bossPositionId = pr.bossPositionId;
                    prApi.bossUserId = pr.bossUserId;
                    prApi.bossUnitId = pr.bossUnitId;
                    prApi.bossLevelId = pr.bossLevelId;
                    prApi.attachmentCount = pr.attachmentCount;
                    prApi.priority = pr.priority;

                    prApi.isiSurat = pr.isiSurat;
                    prApi.idMixMax = PdIdMinMax;
                    prApi.MinMaxNomnal = pr.MinMaxNomnal;

                    #region Memo To (Penerima)
                    if (pr.idUserCheckerPenerima.Length > 0)
                    {
                        for (int a = 0; a < pr.idUserCheckerPenerima.Length; a++)
                        {
                            Guid idCheckerPenerima = new Guid(pr.idUserCheckerPenerima.GetValue(a).ToString());
                            int idLevelPenerima = Convert.ToInt32(pr.idLevelCheckerPenerima.GetValue(a).ToString());
                            Guid idUnitCheckerPenerima = new Guid(pr.idUnitCheckerPenerima.GetValue(a).ToString());
                            prApiCheckerPenerima.Add(new ParamInsertCheckerMemoPenerima
                            {
                                idUserCheckerPenerima = idCheckerPenerima,
                                idLevelCheckerPenerima = idLevelPenerima,
                                idUnitCheckerPenerima = idUnitCheckerPenerima
                            });
                        }
                    }

                    #endregion

                    #region Memo CC (CarbonCopy)
                    if (pr.idUserCheckerCarbonCopy.Length > 0)
                    {
                        for (int a = 0; a < pr.idUserCheckerCarbonCopy.Length; a++)
                        {
                            Guid idCheckerCarbonCopy = new Guid(pr.idUserCheckerCarbonCopy.GetValue(a).ToString());
                            int idLevelCarbonCopy = Convert.ToInt32(pr.idLevelCheckerCarbonCopy.GetValue(a).ToString());
                            Guid idUnitCheckerCarbonCopy = new Guid(pr.idUnitCheckerCarbonCopy.GetValue(a).ToString());
                            prApiCheckerCarbonCopy.Add(new ParamInsertCheckerMemoCarbonCopy
                            {
                                idUserCheckerCarbonCopy = idCheckerCarbonCopy,
                                idLevelCheckerCarbonCopy = idLevelCarbonCopy,
                                idUnitCheckerCarbonCopy = idUnitCheckerCarbonCopy
                            });
                        }
                    }

                    #endregion


                    #region Delibretion
                    if (pr.idUserCheckerDelibretion.Length > 0)
                    {
                        for (int a = 0; a < pr.idUserCheckerDelibretion.Length; a++)
                        {
                            Guid idCheckerDelibretion = new Guid(pr.idUserCheckerDelibretion.GetValue(a).ToString());
                            int idLevelDelibretion = Convert.ToInt32(pr.idLevelCheckerDelibretion.GetValue(a).ToString());
                            Guid idUnitCheckerDelibretion = new Guid(pr.idUnitCheckerDelibretion.GetValue(a).ToString());
                            prApiDelibretion.Add(new ParamInsertCheckerMemoDelibretion
                            {
                                idUserCheckerDelibretion = idCheckerDelibretion,
                                idLevelCheckerDelibretion = idLevelDelibretion,
                                idUnitCheckerDelibretion = idUnitCheckerDelibretion
                            });
                        }
                    }
                    #endregion

                    #region User Approval Pengadaaan

                    if (pr.idUserApprovalPengadaan.Length > 0)
                    {
                        for (int a = 0; a < pr.idUserApprovalPengadaan.Length; a++)
                        {
                            Guid idUserApprovalPengadaan = new Guid(pr.idUserApprovalPengadaan.GetValue(a).ToString());
                            Guid idUserPositionApprovalPengadaan = new Guid(pr.idUserApprovalPositionPengadaan.GetValue(a).ToString());
                            Guid idUserUnitApprovalPengadaan = new Guid(pr.idUserApprovalunitPengadaan.GetValue(a).ToString());
                            prApiApprovalPengadaan.Add(new ParamInsertApprovalPengadaan
                            {
                                idUserApprovalPengadaan = idUserApprovalPengadaan,
                                idUserApprovalPositionPengadaan = idUserPositionApprovalPengadaan,
                                idUserApprovalunitPengadaan = idUserUnitApprovalPengadaan,
                                is_approver = 1,
                            });
                        }
                    }

                    if (pr.idUserApprover.Length > 0)
                    {
                        for (int a = 0; a < pr.idUserApprover.Length; a++)
                        {
                            Guid idApprover = new Guid(pr.idUserApprover.GetValue(a).ToString());
                            int idLevel = Convert.ToInt32(pr.idLevelApprover.GetValue(a).ToString());
                            Guid idUnitApprover = new Guid(pr.idUnitApprover.GetValue(a).ToString());
                            prApiApprover.Add(new ParamInsertApproverMemo
                            {
                                idUserApprover = idApprover,
                                idLevelApprover = idLevel,
                                idUnitApprover = idUnitApprover,
                                is_approver = 2,
                            });
                        }
                    }
                    #endregion

                    if (pr.idUserChecker.Length > 0)
                    {
                        for (int a = 0; a < pr.idUserChecker.Length; a++)
                        {
                            Guid idChecker = new Guid(pr.idUserChecker.GetValue(a).ToString());
                            int idLevel = Convert.ToInt32(pr.idLevelChecker.GetValue(a).ToString());
                            Guid idUnitChecker = new Guid(pr.idUnitChecker.GetValue(a).ToString());
                            prApiChecker.Add(new ParamInsertCheckerMemo
                            {
                                idUserChecker = idChecker,
                                idLevelChecker = idLevel,
                                idUnitChecker = idUnitChecker
                            });
                        }
                    }
                    var checksenderischecker = prApiChecker.Where(p => p.idUserChecker == pr.bossUserId).FirstOrDefault();
                    if (checksenderischecker == null)
                    {
                        prApiChecker.Add(new ParamInsertCheckerMemo
                        {
                            idUserChecker = pr.bossUserId,
                            idLevelChecker = pr.bossLevelId,
                            idUnitChecker = pr.bossUnitId
                        });
                    }

                    // divisi lainya

                    if (pr.idUserCheckerlain.Length > 0)
                    {
                        for (int a = 0; a < pr.idUserCheckerlain.Length; a++)
                        {
                            Guid idChecker = new Guid(pr.idUserCheckerlain.GetValue(a).ToString());
                            int idLevel = Convert.ToInt32(pr.idLevelCheckerlain.GetValue(a).ToString());
                            Guid idUnitChecker = new Guid(pr.idUnitCheckerlain.GetValue(a).ToString());
                            prApiPemeriksaLainya.Add(new ParamInsertCheckerMemoLainya
                            {
                                idUserCheckerlain = idChecker,
                                idLevelCheckerlain = idLevel,
                                idUnitCheckerlain = idUnitChecker
                            });
                        }
                    }
                    prApi.idUserChecker = prApiChecker;
                    prApi.idUserCheckerPengirim = prApiCheckerPengirim;
                    prApi.idUserCheckerPenerima = prApiCheckerPenerima;
                    prApi.idUserCheckerCarbonCopy = prApiCheckerCarbonCopy;
                    prApi.idUserCheckerDelibretion = prApiDelibretion;
                    prApi.idUserApprovalPengadaan = prApiApprovalPengadaan;
                    prApi.idUserApprover = prApiApprover;
                    prApi.idUserCheckerlain = prApiPemeriksaLainya;
                    //prApi.senderName = prApiReceiver;
                }

                prApi.idresponsesurat = responseApiAttachment.idLetter == Guid.Empty ? pr.idresponsesurat : responseApiAttachment.idLetter;
                prApi.saveType = pr.saveType;
                prApi.comment = pr.comment;
                prApi.summary = pr.summary;
                //prApi.senderCompanyName = pr.senderCompanyName;

                generalOutput = await _dataAccessProvider.InsertMemoPengadaan("Memo/InsertMemoPengadaanWeb", token, prApi);
                var jsonApiResponseSerialize = JsonConvert.SerializeObject(generalOutput.Result);
                var titlePopup = "Surat berhasil disimpan";
                var pesanPopup = "Surat berhasil disimpan";
                if (pr.saveType == 4)
                {
                    titlePopup = "Surat berhasil didistribusikan";
                    pesanPopup = "Surat berhasil didistribusikan";
                }
                if (pr.saveType == 2)
                {
                    titlePopup = "Surat berhasil dikirim";
                    pesanPopup = "Surat berhasil dikirim";
                }
                if (pr.saveType == 5)
                {
                    titlePopup = "Surat berhasil ditolak";
                    pesanPopup = "Surat berhasil ditolak";
                }
                if (generalOutput.Status == "OK")
                {
                    TempData["status"] = "OK";
                    TempData["title"] = titlePopup;
                    TempData["pesan"] = pesanPopup;
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
                    TempData["title"] = generalOutput.Message;
                    TempData["pesan"] = generalOutput.Message;
                }
                if (print == 3)
                {
                    if (generalOutput.Status == "OK")
                    {
                        Guid idLetter = new Guid(generalOutput.Result.ToString());
                        //await PrintLetter(idLetter);
                    }
                }
                if (pr.saveType != 1)
                {
                    return Redirect("~/Memo/DistributionMemo");
                }
                else
                {
                    return Redirect("~/Memo/DraftMemo");
                }


            }
            catch (Exception ex)
            {
                TempData["status"] = "NG";
                TempData["title"] = "Gagal membuat surat keluar";
                TempData["pesan"] = ex.ToString();
                return Redirect("~/Memo/NewMemo");
            }
        }

        public async Task<IActionResult> ApprovalMemoPengadaan(ParamApprovalLetterMemo pr, IFormFile inputfile)
        {
            try
            {
                var token = HttpContext.Session.GetString("token");
                GeneralOutputModel generalOutput = new GeneralOutputModel();

                if (pr.idUserDelibration.ToString() != "")
                {
                    if (pr.commentdlbrt != "" && pr.comment !="PengadaanDelebrasiBNI")
                    {

                        generalOutput = await _dataAccessProvider.ApprovalDelebrationMemo("Memo/ApprovalDelebrationMemoWeb", token, pr);
                        generalOutput = await _dataAccessProvider.ApprovalMemo("Memo/ApprovalMemoPengadaanWeb", token, pr);
                        var jsonApiResponseSerialize = JsonConvert.SerializeObject(generalOutput.Result);
                        var titlePopup = "Memo berhasil Disetuji";
                        var pesanPopup = "Memo berhasil Disetuji";
                        if (pr.saveType == 5)
                        {
                            titlePopup = "Memo berhasil ditolak";
                            pesanPopup = "Memo berhasil ditolak";
                        }
                        if (generalOutput.Status == "OK")
                        {
                            TempData["status"] = "OK";
                            TempData["title"] = titlePopup;
                            TempData["pesan"] = pesanPopup;
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
                            TempData["title"] = generalOutput.Message;
                            TempData["pesan"] = generalOutput.Message;
                        }


                    }
                    else if (pr.commentdlbrt != "" && pr.comment =="PengadaanDelebrasiBNI")
                    {
                        generalOutput = await _dataAccessProvider.ApprovalDelebrationMemo("Memo/ApprovalDelebrationMemoWeb", token, pr);
                        var jsonApiResponseSerialize = JsonConvert.SerializeObject(generalOutput.Result);
                        var titlePopup = "berhasil";
                        var pesanPopup = "berhasil";
                        if (generalOutput.Status == "OK")
                        {
                            TempData["status"] = "OK";
                            TempData["title"] = titlePopup;
                            TempData["pesan"] = pesanPopup;
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
                            TempData["title"] = generalOutput.Message;
                            TempData["pesan"] = generalOutput.Message;
                        }
                    }
                    else
                    {
                        TempData["status"] = "NG";
                        TempData["title"] = generalOutput.Message;
                        TempData["pesan"] = generalOutput.Message;
                    }
                }
                else
                {
                    generalOutput = await _dataAccessProvider.ApprovalMemo("Memo/ApprovalMemoPengadaanWeb", token, pr);
                    var jsonApiResponseSerialize = JsonConvert.SerializeObject(generalOutput.Result);
                    var titlePopup = "Memo berhasil Diseutuji";
                    var pesanPopup = "Memo berhasil Diseutuji";
                    if (pr.saveType == 5)
                    {
                        titlePopup = "Memo berhasil ditolak";
                        pesanPopup = "Memo berhasil ditolak";
                    }
                    if (generalOutput.Status == "OK")
                    {
                        TempData["status"] = "OK";
                        TempData["title"] = titlePopup;
                        TempData["pesan"] = pesanPopup;
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
                        TempData["title"] = generalOutput.Message;
                        TempData["pesan"] = generalOutput.Message;
                    }
                    return Redirect("~/Inbox/Index");
                }



                return Redirect("~/Inbox/Index");

            }
            catch (Exception ex)
            {
                TempData["status"] = "NG";
                TempData["title"] = "Gagal melakukan aksi pada surat keluar";
                TempData["pesan"] = ex.ToString();
                return Redirect("~/Inbox/Index");
            }
        }

        [HttpPost]
        public async Task<string> SavePrevMemoPengadaan(ParamInsertMemoWeb pr, IFormFile inputfile)
        {
            OutputPreviewMemo resultPreview = new OutputPreviewMemo();
            try
            {
                var token = HttpContext.Session.GetString("token");
                GeneralOutputModel generalOutput = new GeneralOutputModel();
                ParamInsertMemoPengadaan prApi = new ParamInsertMemoPengadaan();
                List<ParamInsertCheckerMemo> prApiChecker = new List<ParamInsertCheckerMemo>();
                List<ParamInsertCheckerMemoPenerima> prApiCheckerPenerima = new List<ParamInsertCheckerMemoPenerima>();
                List<ParamInsertCheckerMemoCarbonCopy> prApiCheckerCarbonCopy = new List<ParamInsertCheckerMemoCarbonCopy>();
                List<ParamInsertCheckerMemoDelibretion> prApiDelibretion = new List<ParamInsertCheckerMemoDelibretion>();
                List<ParamInsertApprovalPengadaan> prApiApprovalPengadaan = new List<ParamInsertApprovalPengadaan>();
                List<ParamInsertApproverMemo> prApiApprover = new List<ParamInsertApproverMemo>();
                List<ParamInsertSendMemogoingRecipient> prApiReceiver = new List<ParamInsertSendMemogoingRecipient>();
                ParamInsertAttachmentMemo prAttachment = new ParamInsertAttachmentMemo();
                OutputInsertAttachmentMemo responseApiAttachment = new OutputInsertAttachmentMemo();
                int print = pr.saveType;
                if (pr.saveType == 3)
                {
                    pr.saveType = 1;
                }
                if (pr.saveType == 1 || pr.saveType == 2)
                {
                    var idpd = pr.idMinMax.ToString() == "" ? pr.idMinMax2.ToString() : pr.idMinMax.ToString();
                    prApi.outboxType = pr.outboxType;
                    prApi.idMemoType = pr.idMemoType;
                    prApi.letterTypeCode = pr.letterTypeCode;
                    prApi.letterDate = pr.letterDate;
                    prApi.about = pr.about;
                    prApi.resultType = pr.resultType;
                    prApi.signatureType = pr.signatureType;
                    prApi.bossPositionId = pr.bossPositionId;
                    prApi.bossUserId = pr.bossUserId;
                    prApi.bossUnitId = pr.bossUnitId;
                    prApi.bossLevelId = pr.bossLevelId;
                    prApi.attachmentCount = pr.attachmentCount;
                    prApi.priority = pr.priority;

                    prApi.isiSurat = pr.isiSurat;
                    prApi.idMixMax = pr.idMinMax;
                    prApi.MinMaxNomnal = pr.MinMaxNomnal;


                    #region Memo To (Penerima)
                    if (pr.idUserCheckerPenerima.Length > 0)
                    {
                        for (int a = 0; a < pr.idUserCheckerPenerima.Length; a++)
                        {
                            Guid idCheckerPenerima = new Guid(pr.idUserCheckerPenerima.GetValue(a).ToString());
                            int idLevelPenerima = Convert.ToInt32(pr.idLevelCheckerPenerima.GetValue(a).ToString());
                            Guid idUnitCheckerPenerima = new Guid(pr.idUnitCheckerPenerima.GetValue(a).ToString());
                            prApiCheckerPenerima.Add(new ParamInsertCheckerMemoPenerima
                            {
                                idUserCheckerPenerima = idCheckerPenerima,
                                idLevelCheckerPenerima = idLevelPenerima,
                                idUnitCheckerPenerima = idUnitCheckerPenerima
                            });
                        }
                    }

                    #endregion

                    #region Memo CC (CarbonCopy)
                    if (pr.idUserCheckerCarbonCopy.Length > 0)
                    {
                        for (int a = 0; a < pr.idUserCheckerCarbonCopy.Length; a++)
                        {
                            Guid idCheckerCarbonCopy = new Guid(pr.idUserCheckerCarbonCopy.GetValue(a).ToString());
                            int idLevelCarbonCopy = Convert.ToInt32(pr.idLevelCheckerCarbonCopy.GetValue(a).ToString());
                            Guid idUnitCheckerCarbonCopy = new Guid(pr.idUnitCheckerCarbonCopy.GetValue(a).ToString());
                            prApiCheckerCarbonCopy.Add(new ParamInsertCheckerMemoCarbonCopy
                            {
                                idUserCheckerCarbonCopy = idCheckerCarbonCopy,
                                idLevelCheckerCarbonCopy = idLevelCarbonCopy,
                                idUnitCheckerCarbonCopy = idUnitCheckerCarbonCopy
                            });
                        }
                    }

                    #endregion


                    #region Delibretion
                    if (pr.idUserCheckerDelibretion.Length > 0)
                    {
                        for (int a = 0; a < pr.idUserCheckerDelibretion.Length; a++)
                        {
                            Guid idCheckerDelibretion = new Guid(pr.idUserCheckerDelibretion.GetValue(a).ToString());
                            int idLevelDelibretion = Convert.ToInt32(pr.idLevelCheckerDelibretion.GetValue(a).ToString());
                            Guid idUnitCheckerDelibretion = new Guid(pr.idUnitCheckerDelibretion.GetValue(a).ToString());
                            prApiDelibretion.Add(new ParamInsertCheckerMemoDelibretion
                            {
                                idUserCheckerDelibretion = idCheckerDelibretion,
                                idLevelCheckerDelibretion = idLevelDelibretion,
                                idUnitCheckerDelibretion = idUnitCheckerDelibretion
                            });
                        }
                    }
                    #endregion

                    #region User Approval Pengadaaan

                    if (pr.idUserApprovalPengadaan.Length > 0)
                    {
                        for (int a = 0; a < pr.idUserApprovalPengadaan.Length; a++)
                        {
                            Guid idUserApprovalPengadaan = new Guid(pr.idUserApprovalPengadaan.GetValue(a).ToString());
                            Guid idUserPositionApprovalPengadaan = new Guid(pr.idUserApprovalPositionPengadaan.GetValue(a).ToString());
                            Guid idUserUnitApprovalPengadaan = new Guid(pr.idUserApprovalunitPengadaan.GetValue(a).ToString());
                            prApiApprovalPengadaan.Add(new ParamInsertApprovalPengadaan
                            {
                                idUserApprovalPengadaan = idUserApprovalPengadaan,
                                idUserApprovalPositionPengadaan = idUserPositionApprovalPengadaan,
                                idUserApprovalunitPengadaan = idUserUnitApprovalPengadaan,
                                is_approver = 1,
                            });
                        }
                    }

                    if (pr.idUserApprover.Length > 0)
                    {
                        for (int a = 0; a < pr.idUserApprover.Length; a++)
                        {
                            Guid idApprover = new Guid(pr.idUserApprover.GetValue(a).ToString());
                            int idLevel = Convert.ToInt32(pr.idLevelApprover.GetValue(a).ToString());
                            Guid idUnitApprover = new Guid(pr.idUnitApprover.GetValue(a).ToString());
                            prApiApprover.Add(new ParamInsertApproverMemo
                            {
                                idUserApprover = idApprover,
                                idLevelApprover = idLevel,
                                idUnitApprover = idUnitApprover,
                                is_approver = 2,
                            });
                        }
                    }
                    #endregion

                    if (pr.idUserChecker.Length > 0)
                    {
                        for (int a = 0; a < pr.idUserChecker.Length; a++)
                        {
                            Guid idChecker = new Guid(pr.idUserChecker.GetValue(a).ToString());
                            int idLevel = Convert.ToInt32(pr.idLevelChecker.GetValue(a).ToString());
                            Guid idUnitChecker = new Guid(pr.idUnitChecker.GetValue(a).ToString());
                            prApiChecker.Add(new ParamInsertCheckerMemo
                            {
                                idUserChecker = idChecker,
                                idLevelChecker = idLevel,
                                idUnitChecker = idUnitChecker
                            });
                        }
                    }
                    var checksenderischecker = prApiChecker.Where(p => p.idUserChecker == pr.bossUserId).FirstOrDefault();
                    if (checksenderischecker == null)
                    {
                        prApiChecker.Add(new ParamInsertCheckerMemo
                        {
                            idUserChecker = pr.bossUserId,
                            idLevelChecker = pr.bossLevelId,
                            idUnitChecker = pr.bossUnitId
                        });
                    }
                    prApi.idUserChecker = prApiChecker;
                    prApi.idUserCheckerPenerima = prApiCheckerPenerima;
                    prApi.idUserCheckerCarbonCopy = prApiCheckerCarbonCopy;
                    prApi.idUserCheckerDelibretion = prApiDelibretion;
                    prApi.idUserApprovalPengadaan = prApiApprovalPengadaan;
                    prApi.idUserApprover = prApiApprover;
                }

                prApi.idresponsesurat = pr.idresponsesurat;
                prApi.idresponsesurat = responseApiAttachment.idLetter == Guid.Empty ? pr.idresponsesurat : responseApiAttachment.idLetter;
                prApi.saveType = pr.saveType;
                prApi.comment = pr.comment;
                prApi.summary = pr.summary;
                //prApi.senderCompanyName = pr.senderCompanyName;

                generalOutput = await _dataAccessProvider.InsertMemoPengadaan("Memo/InsertMemoPengadaanWeb", token, prApi);
                var jsonApiResponseSerialize = JsonConvert.SerializeObject(generalOutput.Result);


                var pesanPopup = "Surat berhasil dipreview";

                resultPreview.Status = generalOutput.Status;
                resultPreview.Result = generalOutput.Result.ToString();
                resultPreview.Message = pesanPopup;
                var json = JsonConvert.SerializeObject(resultPreview);

                return json;
            }
            catch (Exception ex)
            {
                resultPreview.Status = "NG";
                resultPreview.Result = pr.idresponsesurat.ToString();
                resultPreview.Message = "Gagal preview Memo Pengadaan";

                var json = JsonConvert.SerializeObject(resultPreview);

                return json;
            }
        }

        [HttpPost]
        public async Task<string> UpdatePrevMemoPengadaan(ParamInsertMemoWeb pr, IFormFile inputfile)
        {
            OutputPreviewMemo resultPreview = new OutputPreviewMemo();
            try
            {

                var token = HttpContext.Session.GetString("token");
                GeneralOutputModel generalOutput = new GeneralOutputModel();
                ParamInsertMemoPengadaan prApi = new ParamInsertMemoPengadaan();
                List<ParamInsertCheckerMemo> prApiChecker = new List<ParamInsertCheckerMemo>();
                List<ParamInsertCheckerMemoPenerima> prApiCheckerPenerima = new List<ParamInsertCheckerMemoPenerima>();
                List<ParamInsertCheckerMemoCarbonCopy> prApiCheckerCarbonCopy = new List<ParamInsertCheckerMemoCarbonCopy>();
                List<ParamInsertCheckerMemoDelibretion> prApiDelibretion = new List<ParamInsertCheckerMemoDelibretion>();
                List<ParamInsertApprovalPengadaan> prApiApprovalPengadaan = new List<ParamInsertApprovalPengadaan>();
                List<ParamInsertApproverMemo> prApiApprover = new List<ParamInsertApproverMemo>();
                List<ParamInsertSendMemogoingRecipient> prApiReceiver = new List<ParamInsertSendMemogoingRecipient>();
                ParamInsertAttachmentMemo prAttachment = new ParamInsertAttachmentMemo();
                OutputInsertAttachmentMemo responseApiAttachment = new OutputInsertAttachmentMemo();
                int print = pr.saveType;
                if (pr.saveType == 3)
                {
                    pr.saveType = 1;
                }
                if (pr.saveType == 1 || pr.saveType == 2)
                {
                    var idpd = pr.idMinMax.ToString() == "00000000-0000-0000-0000-000000000000" ? pr.idMinMax2.ToString() : pr.idMinMax.ToString();
                    Guid PdIdMinMax = new Guid(idpd);
                    prApi.outboxType = pr.outboxType;
                    prApi.idMemoType = pr.idMemoType;
                    prApi.letterTypeCode = pr.letterTypeCode;
                    prApi.letterDate = pr.letterDate;
                    prApi.about = pr.about;
                    prApi.resultType = pr.resultType;
                    prApi.signatureType = pr.signatureType;
                    prApi.bossPositionId = pr.bossPositionId;
                    prApi.bossUserId = pr.bossUserId;
                    prApi.bossUnitId = pr.bossUnitId;
                    prApi.bossLevelId = pr.bossLevelId;
                    prApi.attachmentCount = pr.attachmentCount;
                    prApi.priority = pr.priority;

                    prApi.isiSurat = pr.isiSurat;
                    prApi.idMixMax = PdIdMinMax;
                    prApi.MinMaxNomnal = pr.MinMaxNomnal;

                    #region Memo To (Penerima)
                    if (pr.idUserCheckerPenerima.Length > 0)
                    {
                        for (int a = 0; a < pr.idUserCheckerPenerima.Length; a++)
                        {
                            Guid idCheckerPenerima = new Guid(pr.idUserCheckerPenerima.GetValue(a).ToString());
                            int idLevelPenerima = Convert.ToInt32(pr.idLevelCheckerPenerima.GetValue(a).ToString());
                            Guid idUnitCheckerPenerima = new Guid(pr.idUnitCheckerPenerima.GetValue(a).ToString());
                            prApiCheckerPenerima.Add(new ParamInsertCheckerMemoPenerima
                            {
                                idUserCheckerPenerima = idCheckerPenerima,
                                idLevelCheckerPenerima = idLevelPenerima,
                                idUnitCheckerPenerima = idUnitCheckerPenerima
                            });
                        }
                    }

                    #endregion

                    #region Memo CC (CarbonCopy)
                    if (pr.idUserCheckerCarbonCopy.Length > 0)
                    {
                        for (int a = 0; a < pr.idUserCheckerCarbonCopy.Length; a++)
                        {
                            Guid idCheckerCarbonCopy = new Guid(pr.idUserCheckerCarbonCopy.GetValue(a).ToString());
                            int idLevelCarbonCopy = Convert.ToInt32(pr.idLevelCheckerCarbonCopy.GetValue(a).ToString());
                            Guid idUnitCheckerCarbonCopy = new Guid(pr.idUnitCheckerCarbonCopy.GetValue(a).ToString());
                            prApiCheckerCarbonCopy.Add(new ParamInsertCheckerMemoCarbonCopy
                            {
                                idUserCheckerCarbonCopy = idCheckerCarbonCopy,
                                idLevelCheckerCarbonCopy = idLevelCarbonCopy,
                                idUnitCheckerCarbonCopy = idUnitCheckerCarbonCopy
                            });
                        }
                    }

                    #endregion


                    #region Delibretion
                    if (pr.idUserCheckerDelibretion.Length > 0)
                    {
                        for (int a = 0; a < pr.idUserCheckerDelibretion.Length; a++)
                        {
                            Guid idCheckerDelibretion = new Guid(pr.idUserCheckerDelibretion.GetValue(a).ToString());
                            int idLevelDelibretion = Convert.ToInt32(pr.idLevelCheckerDelibretion.GetValue(a).ToString());
                            Guid idUnitCheckerDelibretion = new Guid(pr.idUnitCheckerDelibretion.GetValue(a).ToString());
                            prApiDelibretion.Add(new ParamInsertCheckerMemoDelibretion
                            {
                                idUserCheckerDelibretion = idCheckerDelibretion,
                                idLevelCheckerDelibretion = idLevelDelibretion,
                                idUnitCheckerDelibretion = idUnitCheckerDelibretion
                            });
                        }
                    }
                    #endregion

                    #region User Approval Pengadaaan

                    if (pr.idUserApprovalPengadaan.Length > 0)
                    {
                        for (int a = 0; a < pr.idUserApprovalPengadaan.Length; a++)
                        {
                            Guid idUserApprovalPengadaan = new Guid(pr.idUserApprovalPengadaan.GetValue(a).ToString());
                            Guid idUserPositionApprovalPengadaan = new Guid(pr.idUserApprovalPositionPengadaan.GetValue(a).ToString());
                            Guid idUserUnitApprovalPengadaan = new Guid(pr.idUserApprovalunitPengadaan.GetValue(a).ToString());
                            prApiApprovalPengadaan.Add(new ParamInsertApprovalPengadaan
                            {
                                idUserApprovalPengadaan = idUserApprovalPengadaan,
                                idUserApprovalPositionPengadaan = idUserPositionApprovalPengadaan,
                                idUserApprovalunitPengadaan = idUserUnitApprovalPengadaan,
                                is_approver = 1,
                            });
                        }
                    }

                    if (pr.idUserApprover.Length > 0)
                    {
                        for (int a = 0; a < pr.idUserApprover.Length; a++)
                        {
                            Guid idApprover = new Guid(pr.idUserApprover.GetValue(a).ToString());
                            int idLevel = Convert.ToInt32(pr.idLevelApprover.GetValue(a).ToString());
                            Guid idUnitApprover = new Guid(pr.idUnitApprover.GetValue(a).ToString());
                            prApiApprover.Add(new ParamInsertApproverMemo
                            {
                                idUserApprover = idApprover,
                                idLevelApprover = idLevel,
                                idUnitApprover = idUnitApprover,
                                is_approver = 2,
                            });
                        }
                    }
                    #endregion

                    if (pr.idUserChecker.Length > 0)
                    {
                        for (int a = 0; a < pr.idUserChecker.Length; a++)
                        {
                            Guid idChecker = new Guid(pr.idUserChecker.GetValue(a).ToString());
                            int idLevel = Convert.ToInt32(pr.idLevelChecker.GetValue(a).ToString());
                            Guid idUnitChecker = new Guid(pr.idUnitChecker.GetValue(a).ToString());
                            prApiChecker.Add(new ParamInsertCheckerMemo
                            {
                                idUserChecker = idChecker,
                                idLevelChecker = idLevel,
                                idUnitChecker = idUnitChecker
                            });
                        }
                    }
                    var checksenderischecker = prApiChecker.Where(p => p.idUserChecker == pr.bossUserId).FirstOrDefault();
                    if (checksenderischecker == null)
                    {
                        prApiChecker.Add(new ParamInsertCheckerMemo
                        {
                            idUserChecker = pr.bossUserId,
                            idLevelChecker = pr.bossLevelId,
                            idUnitChecker = pr.bossUnitId
                        });
                    }
                    prApi.idUserChecker = prApiChecker;
                    prApi.idUserCheckerPenerima = prApiCheckerPenerima;
                    prApi.idUserCheckerCarbonCopy = prApiCheckerCarbonCopy;
                    prApi.idUserCheckerDelibretion = prApiDelibretion;
                    prApi.idUserApprovalPengadaan = prApiApprovalPengadaan;
                    prApi.idUserApprover = prApiApprover;
                    //prApi.senderName = prApiReceiver;
                }

                prApi.idresponsesurat = pr.idresponsesurat;
                prApi.idresponsesurat = responseApiAttachment.idLetter == Guid.Empty ? pr.idresponsesurat : responseApiAttachment.idLetter;
                prApi.saveType = pr.saveType;
                prApi.comment = pr.comment;
                prApi.summary = pr.summary;
                //prApi.senderCompanyName = pr.senderCompanyName;

                generalOutput = await _dataAccessProvider.InsertMemoPengadaan("Memo/InsertMemoPengadaanWeb", token, prApi);
                var jsonApiResponseSerialize = JsonConvert.SerializeObject(generalOutput.Result);


                var pesanPopup = "Surat berhasil dipreview";

                resultPreview.Status = generalOutput.Status;
                resultPreview.Result = generalOutput.Result.ToString();
                resultPreview.Message = pesanPopup;
                var json = JsonConvert.SerializeObject(resultPreview);

                return json;
            }
            catch (Exception ex)
            {
                resultPreview.Status = "NG";
                resultPreview.Result = pr.idresponsesurat.ToString();
                resultPreview.Message = "Gagal preview Memo Pengadaan";

                var json = JsonConvert.SerializeObject(resultPreview);

                return json;
            }
        }

        public async Task<FileResult> PrintLetterMemo(Guid id)
        {
            var token = HttpContext.Session.GetString("token");
            GeneralOutputModel generalOutput = new GeneralOutputModel();
            OutputDetailMemo letterDetail = new OutputDetailMemo();
            ParamGetDetailMemo pr = new ParamGetDetailMemo();
            pr.idLetter = id;



            generalOutput = await _dataAccessProvider.GetDetailMemo("Memo/GetDetailMemoWebPriviewWeb/", token, pr);
            var jsonApiResponseSerialize = JsonConvert.SerializeObject(generalOutput.Result);
            letterDetail = JsonConvert.DeserializeObject<OutputDetailMemo>(jsonApiResponseSerialize);

            var letternumber = letterDetail.letter.letterNumber;
            letternumber = letternumber.Replace(".", "_");
            var fileName = letternumber + ".pdf";
            if (letterDetail.letter.statusCode != 4)
            {
                fileName = "Draft_" + letterDetail.letter.about + ".pdf";
            }
            
            //Surat Keputusan Direksi (Done)
            if (letterDetail.letter.idmemoType.ToString().ToUpper() == "D8D86FAE-2290-ED11-80D1-B7157F320A73")
            {
                return PrivSuratKeputusanDireksi(letternumber, letterDetail, fileName);
                   
            }

            //Surat Edaran Direksi (Done (Tapi Masi Ikuti Memo Direksi))
            else if (letterDetail.letter.idmemoType.ToString().ToUpper() == "836EAFD1-2290-ED11-80D1-B7157F320A73")
            {
                return PrivSuratEdaranDireksi(letternumber, letterDetail, fileName);
            }

            // Surat Pernyataan Direksi (Done (Tapi Masi Ikuti Template Memo Direksi))
            else if (letterDetail.letter.idmemoType.ToString().ToUpper() == "889019DC-2290-ED11-80D1-B7157F320A73")
            {
                return PrivSuratPernyataanDireksi(letternumber, letterDetail, fileName);
            }

            // Surat Keterangan Direksi (Onprocess)
            else if (letterDetail.letter.idmemoType.ToString().ToUpper() == "899019DC-2290-ED11-80D1-B7157F320A73")
            {
                return PrivSuratKeteranganDireksi(letternumber, letterDetail, fileName);
            }

            // Surat Kuasa Operasional
            else if (letterDetail.letter.idmemoType.ToString().ToUpper() == "ED1622E3-2290-ED11-80D1-B7157F320A73")
            {
                return PrivSuratKuasaOperasional(letternumber, letterDetail, fileName);
            }

            // Surat kuasa dewan komisaris
            else if (letterDetail.letter.idmemoType.ToString().ToUpper() == "C555BE1B-2390-ED11-80D1-B7157F320A73")
            {
                return PrivSuratKuasaDewanKom(letternumber, letterDetail, fileName);
            }
            
            // Surat pernyataan dewan komisaris
            else if (letterDetail.letter.idmemoType.ToString().ToUpper() == "C655BE1B-2390-ED11-80D1-B7157F320A73")
            {
                return PrivSuratPernyataanDewanKom(letternumber, letterDetail, fileName);
            }

            //Surat keputusan dewan komisaris (Done)
            else if (letterDetail.letter.idmemoType.ToString().ToUpper() == "19A76160-2390-ED11-80D1-B7157F320A73")
            {
                return PrivSuratKeputusanDewanKom(letternumber, letterDetail, fileName);
            }

            //Surat Kuasa
            else if (letterDetail.letter.idmemoType.ToString().ToUpper() == "92F6F889-2390-ED11-80D1-B7157F320A73")
            {
                return PrivSuratKuasa(letternumber, letterDetail, fileName);
            }

            //Surat Pernyataan
            else if (letterDetail.letter.idmemoType.ToString().ToUpper() == "6FE7AAA4-2390-ED11-80D1-B7157F320A73")
            {
                return PrivSuratPernyataan(letternumber, letterDetail, fileName);
            }

            // Memo Divisi
            else if (letterDetail.letter.idmemoType.ToString().ToUpper() == "70E7AAA4-2390-ED11-80D1-B7157F320A73")
            {
                return PrivSuratMemoDivisi(letternumber, letterDetail, fileName);
            }

            // Memo Departement sma dengan memo divisi
            else if (letterDetail.letter.idmemoType.ToString().ToUpper() == "242852B4-2390-ED11-80D1-B7157F320A73")
            {
                return PrivSuratMemoDepartement(letternumber, letterDetail, fileName);
            }

            // Surat Keterangan
            else if (letterDetail.letter.idmemoType.ToString().ToUpper() == "7BAE59CB-2390-ED11-80D1-B7157F320A73")
            {
                return PrivSuratKeterangan(letternumber, letterDetail, fileName);
            }

            

            StringBuilder sb = new StringBuilder();
                sb.Append("<div style='padding-left:2px;'>" + letterDetail.letterContent.letterContent + "</div>");
                StringReader sr = new StringReader(sb.ToString());
                //Document pdfDoc = new Document(PageSize.A4, 25, 25, 25, 15);
                //Document pdfDoc = new Document(PageSize.A4, 56.7f, 56.7f, 113.4f, 99.225f);
                Document pdfDoc = new Document(PageSize.A4, 56.7f, 56.7f, 90.4f, 99.225f);
                HTMLWorker htmlparser = new HTMLWorker(pdfDoc);


            using (MemoryStream ms = new MemoryStream())
            {
                //PdfWriter pdfWriter = PdfWriter.GetInstance(pdfDoc, ms);
                //pdfWriter.PageEvent = new PDFHeaderEvent();
                //pdfWriter.PageEvent = new PDFHeaderEvent2();
                //pdfDoc.Open();
                PdfWriter pdfWriter = PdfWriter.GetInstance(pdfDoc, ms);
                pdfDoc.Open();
                //Font
                int no;
                var fontName = "Calibri";
                string fontPath = Path.Combine(_environment.WebRootPath, "fonts/Calibri.ttf");
                //FontFactory.Register(fontPath);

                Font normal = FontFactory.GetFont(fontName, 12, Font.NORMAL, BaseColor.BLACK);
                Font bold = FontFactory.GetFont(fontName, 12, Font.BOLD, BaseColor.BLACK);
                Font boldMemo = FontFactory.GetFont(fontName, 14, Font.BOLD, BaseColor.BLACK);
                Font underline = FontFactory.GetFont(fontName, 12, Font.UNDERLINE, BaseColor.BLACK);
                Font underlineBold = FontFactory.GetFont(fontName, 12, Font.BOLD | Font.UNDERLINE, BaseColor.BLACK);

                Font normalDeliberation = FontFactory.GetFont(fontName, 10, Font.NORMAL, BaseColor.BLACK);
                Font underlineDeliberation = FontFactory.GetFont(fontName, 10, Font.UNDERLINE, BaseColor.BLACK);
                Font underlineDeliberationBold = FontFactory.GetFont(fontName, 10, Font.BOLD | Font.UNDERLINE, BaseColor.BLACK);
                // Colors
                BaseColor colorBlack = new BaseColor(System.Drawing.ColorTranslator.FromHtml("#000000"));
                BaseColor colorWhite = new BaseColor(System.Drawing.ColorTranslator.FromHtml("#FFFFFF"));



                PdfPTable table = new PdfPTable(2);
                float[] width = new float[] { 0.667f, 2f };
                table.WidthPercentage = 100;
                table.SetWidths(width);
                table.DefaultCell.Border = 0;
                table.SpacingBefore = 0f;
                table.SpacingAfter = 0f;

                PdfPCell cell = new PdfPCell();
                Paragraph paragraph = new Paragraph();


                #region header

                //cell = new PdfPCell();
                //cell.Border = Rectangle.NO_BORDER;
                //cell.HorizontalAlignment= Element.ALIGN_LEFT;
                //cell.Phrase = new Phrase("", bold);
                //table.AddCell(cell);

                //Image logo = GetLogoImage("logobni");
                //logo.ScalePercent(55);
                //cell = new PdfPCell(logo);
                //cell.Border = Rectangle.NO_BORDER;
                //cell.HorizontalAlignment= Element.ALIGN_RIGHT;
                //table.AddCell(cell);
                //pdfDoc.Add(table);

                //table = new PdfPTable(1);
                //width = new float[] { 2f };
                //table.WidthPercentage = 100;
                //table.SetWidths(width);
                //table.HorizontalAlignment = Element.ALIGN_LEFT;
                //table.DefaultCell.Border = 0;
                //table.SpacingBefore = 00f;
                //table.SpacingAfter = 0f;


                //table.AddCell(new Phrase("MEMO", boldMemo));
                //pdfDoc.Add(table);

                table = new PdfPTable(2);
                width = new float[] { 0.2f, 1f };
                table.WidthPercentage = 100;
                table.SetWidths(width);
                table.HorizontalAlignment = Element.ALIGN_RIGHT;
                table.DefaultCell.Border = 0;
                table.SpacingBefore = 0f;
                table.SpacingAfter = 0f;

                cell.Border = 0;
                paragraph = new Paragraph("Nomor", normal);
                paragraph.Alignment = Element.ALIGN_LEFT;
                cell.AddElement(paragraph);
                table.AddCell(cell);

                //cell = new PdfPCell();
                //cell.Border = 0;
                //paragraph = new Paragraph(":", normal);
                //paragraph.Alignment = Element.ALIGN_RIGHT;
                //cell.AddElement(paragraph);
                //table.AddCell(cell);

                cell = new PdfPCell();
                cell.Border = 0;
                paragraph = new Paragraph(":"+" "+letterDetail.letter.letterNumber, normal);
                paragraph.Alignment = Element.ALIGN_LEFT;
                cell.AddElement(paragraph);
                table.AddCell(cell);

                cell = new PdfPCell();
                cell.Border = 0;
                paragraph = new Paragraph("Tanggal", normal);
                paragraph.Alignment = Element.ALIGN_LEFT;
                cell.AddElement(paragraph);
                table.AddCell(cell);

                //cell = new PdfPCell();
                //cell.Border = 0;
                //paragraph = new Paragraph(":", normal);
                //paragraph.Alignment = Element.ALIGN_RIGHT;
                //cell.AddElement(paragraph);
                //table.AddCell(cell);

                cell = new PdfPCell();
                cell.Border = 0;
                paragraph = new Paragraph(":"+" "+Convert.ToDateTime(letterDetail.letter.letterDate).ToString("dd MMMM yyyy"), normal);
                paragraph.Alignment = Element.ALIGN_LEFT;
                cell.AddElement(paragraph);
                table.AddCell(cell);

                cell = new PdfPCell();
                cell.Border = 0;
                paragraph = new Paragraph("Kepada", normal);
                paragraph.Alignment = Element.ALIGN_LEFT;
                cell.AddElement(paragraph);
                table.AddCell(cell);

                //cell = new PdfPCell();
                //cell.Border = 0;
                //paragraph = new Paragraph(":", normal);
                //paragraph.Alignment = Element.ALIGN_RIGHT;
                //cell.AddElement(paragraph);
                //table.AddCell(cell);

                cell = new PdfPCell();
                cell.Border = 0;
                no = 1;
                foreach (var item in letterDetail.receiver)
                {
                    if (letterDetail.receiver.Count() > 1)
                    {
                        paragraph = new Paragraph(":"+" "+no + ". " + item.fullname + " - " + item.positionName, normal);
                        paragraph.Alignment = Element.ALIGN_LEFT;
                        cell.AddElement(paragraph);
                    }
                    else
                    {
                        paragraph = new Paragraph(":"+" "+item.fullname + " - " + item.positionName, normal);
                        paragraph.Alignment = Element.ALIGN_LEFT;
                        cell.AddElement(paragraph);
                    }
                    no++;
                }
                table.AddCell(cell);

                cell = new PdfPCell();
                cell.Border = 0;
                paragraph = new Paragraph("Tembusan", normal);
                paragraph.Alignment = Element.ALIGN_LEFT;
                cell.AddElement(paragraph);
                table.AddCell(cell);

                //cell = new PdfPCell();
                //cell.Border = 0;
                //paragraph = new Paragraph(":", normal);
                //paragraph.Alignment = Element.ALIGN_RIGHT;
                //cell.AddElement(paragraph);
                //table.AddCell(cell);

                cell = new PdfPCell();
                cell.Border = 0;
                no = 1;
                foreach (var item in letterDetail.copy)
                {
                    if (letterDetail.copy.Count() > 1)
                    {
                        paragraph = new Paragraph(":"+" "+no + ". " + item.fullname + " - " + item.positionName, normal);
                        paragraph.Alignment = Element.ALIGN_LEFT;
                        cell.AddElement(paragraph);
                    }
                    else
                    {
                        paragraph = new Paragraph(":"+" "+item.fullname + " - " + item.positionName, normal);
                        paragraph.Alignment = Element.ALIGN_LEFT;
                        cell.AddElement(paragraph);
                    }
                    no++;
                }
                if (letterDetail.copy.Count() == 0)
                {
                    paragraph = new Paragraph("-", normal);
                    paragraph.Alignment = Element.ALIGN_LEFT;
                    cell.AddElement(paragraph);
                }
                table.AddCell(cell);

                cell = new PdfPCell();
                cell.Border = 0;
                paragraph = new Paragraph("Dari", normal);
                paragraph.Alignment = Element.ALIGN_LEFT;
                cell.AddElement(paragraph);
                table.AddCell(cell);

                //cell = new PdfPCell();
                //cell.Border = 0;
                //paragraph = new Paragraph(":", normal);
                //paragraph.Alignment = Element.ALIGN_RIGHT;
                //cell.AddElement(paragraph);
                //table.AddCell(cell);

                cell = new PdfPCell();
                cell.Border = 0;
                paragraph = new Paragraph(":"+" "+letterDetail.sender[0].fullname + " - " + letterDetail.sender[0].positionName, normal);
                paragraph.Alignment = Element.ALIGN_LEFT;
                cell.AddElement(paragraph);
                table.AddCell(cell);

                cell = new PdfPCell();
                cell.Border = 0;
                paragraph = new Paragraph("Perihal", normal);
                paragraph.Alignment = Element.ALIGN_LEFT;
                cell.AddElement(paragraph);
                table.AddCell(cell);

                //cell = new PdfPCell();
                //cell.Border = 0;
                //paragraph = new Paragraph(":", normal);
                //paragraph.Alignment = Element.ALIGN_RIGHT;
                //cell.AddElement(paragraph);
                //table.AddCell(cell);

                cell = new PdfPCell();
                cell.Border = 0;
                paragraph = new Paragraph(":"+" "+letterDetail.letter.about, normal);
                paragraph.Alignment = Element.ALIGN_LEFT;
                cell.AddElement(paragraph);
                table.AddCell(cell);

                cell = new PdfPCell();
                cell.Border = 0;
                paragraph = new Paragraph("Lampiran", normal);
                paragraph.Alignment = Element.ALIGN_LEFT;
                cell.AddElement(paragraph);
                table.AddCell(cell);

                //cell = new PdfPCell();
                //cell.Border = 0;
                //paragraph = new Paragraph(":", normal);
                //paragraph.Alignment = Element.ALIGN_RIGHT;
                //cell.AddElement(paragraph);
                //table.AddCell(cell);

                var lampiran = letterDetail.letter.attachmentDesc == null ? "-" : letterDetail.letter.attachmentDesc;
                cell = new PdfPCell();
                cell.Border = 0;
                paragraph = new Paragraph(":"+" "+lampiran, bold);
                paragraph.Alignment = Element.ALIGN_LEFT;
                cell.AddElement(paragraph);
                table.AddCell(cell);

                //Add table to document
                pdfDoc.Add(table);


                Paragraph line = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(2F, 100.0F, colorBlack, Element.ALIGN_CENTER, 1)));
                line.SpacingBefore = 0f;
                line.SetLeading(2F, 0.5F);
                pdfDoc.Add(line);
                Paragraph lines = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(2F, 100.0F, colorBlack, Element.ALIGN_CENTER, 1)));
                lines.SpacingBefore = 0f;
                line.SetLeading(0.5F, 0.5F);
                pdfDoc.Add(line);

                Paragraph liness = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(2F, 100.0F, colorWhite, Element.ALIGN_CENTER, 1)));
                liness.SpacingBefore = 0f;
                liness.SetLeading(0.5F, 0.5F);
                pdfDoc.Add(liness);

                //Paragraph linesss = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(2F, 100.0F, colorWhite, Element.ALIGN_CENTER, 1)));
                //linesss.SpacingBefore = 0f;
                //linesss.SetLeading(0.5F, 0.5F);
                //pdfDoc.Add(linesss);


                table = new PdfPTable(1);
                width = new float[] { 2f };
                table.WidthPercentage = 100;
                table.SetWidths(width);
                table.HorizontalAlignment = Element.ALIGN_LEFT;
                table.DefaultCell.Border = 0;
                table.SpacingBefore = 00f;
                table.SpacingAfter = 0f;


                //table.AddCell(new Phrase("Dengan hormat", normal));
                //pdfDoc.Add(table);

                Paragraph linessss = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(2F, 100.0F, colorWhite, Element.ALIGN_CENTER, 1)));
                linessss.SpacingBefore = 0f;
                linessss.SetLeading(0.5F, 0.5F);
                pdfDoc.Add(linessss);

                Paragraph lin = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(2F, 100.0F, colorWhite, Element.ALIGN_CENTER, 1)));
                lin.SpacingBefore = 0f;
                lin.SetLeading(0.5F, 0.5F);
                pdfDoc.Add(lin);

                #endregion

                //ADD HTML CONTENT
                htmlparser.Parse(sr);


                #region Tanda Tangan

                Paragraph a = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(2F, 100.0F, colorWhite, Element.ALIGN_CENTER, 1)));
                a.SpacingBefore = 0f;
                a.SetLeading(0.5F, 0.5F);
                pdfDoc.Add(a);



                cell = new PdfPCell();
                table.AddCell(new Phrase("", normal));

                cell = new PdfPCell();
                table.AddCell(new Phrase(" ", normal));
                cell = new PdfPCell();
                table.AddCell(new Phrase(" ", normal));
                cell = new PdfPCell();
                table.AddCell(new Phrase("Hormat kami, \n" + "PT BNI Life Insurance", normal));
                //table.AddCell(new Phrase("Best Regards", normal));
                pdfDoc.Add(table);

                

                string filename = letterDetail.sender[0].nip;


                // Tanda tangan
                var dirChecker = letterDetail.sender.OrderBy(p => p.idUserSender).ToList();
                //var dirChecker = letterDetail.checker.Where(p => p.idLevelChecker == 2 || p.idLevelChecker == 1).ToList();

                table = new PdfPTable(dirChecker.Count());
                width = new float[dirChecker.Count()];
                for (int i = 0; i < dirChecker.Count(); i++)
                {
                    width[i] = 1f;
                }
                table.WidthPercentage = 100;
                table.SetWidths(width);
                table.HorizontalAlignment = Element.ALIGN_LEFT;
                table.DefaultCell.Border = 0;
                var no1 = 0;


                //cell = new PdfPCell();
                //table.AddCell(new Phrase("Direksi PT BNI Life Insurance", normal));
                foreach (var item in dirChecker)
                {
                    cell = new PdfPCell();
                    cell.Border = 0;




                    if (letterDetail.letter.letterNumber != "NO_LETTER")
                    {
                        if (letterDetail.letter.signatureType == 1)
                        {

                            Image imagettd = GetSignatureImage(item.nip);
                            //cell.AddElement(imagettd);
                            if (imagettd != null)
                            {
                                cell.AddElement(imagettd);
                            }
                            else
                            {
                                var img = "File tanda tangan tidak ditemukan";
                                paragraph = new Paragraph(img, normal);
                                cell.AddElement(paragraph);
                            }

                        }
                        else if (letterDetail.letter.signatureType == 2)
                        {
                            Image QRCodeSignatureApprover = GetBarcodeSignature("NIP : " + item.nip + "\nNAMA : " + item.fullname + "\nJabatan : " + item.positionName);
                            QRCodeSignatureApprover.ScaleAbsoluteHeight(10f);
                            cell.AddElement(QRCodeSignatureApprover);
                        }
                        else if (letterDetail.letter.signatureType == 3)
                        {

                            var img = "";
                            paragraph = new Paragraph(img, normal);
                            cell.AddElement(paragraph);
                        }
                        else
                        {

                            Image imagettd = GetSignatureImage(item.nip);
                            cell.AddElement(imagettd);
                            Image QRCodeSignatureApprover = GetBarcodeSignature("NIP : " + item.nip + "\nNAMA : " + item.fullname + "\nJabatan : " + item.positionName);
                            cell.AddElement(QRCodeSignatureApprover);
                            //if (imagettd != null)
                            //{
                            //    cell.AddElement(imagettd);
                            //    Image QRCodeSignatureApprover = GetBarcodeSignature("NIP : " + item.nip + "\nNAMA : " + item.fullname + "\nJabatan : " + item.positionName);
                            //    cell.AddElement(QRCodeSignatureApprover);
                            //}
                            //else
                            //{
                            //    var img = "File tanda tangan tidak di temukan";
                            //    paragraph = new Paragraph(img, normal);
                            //    cell.AddElement(paragraph);
                            //    Image QRCodeSignatureApprover = GetBarcodeSignature("NIP : " + item.nip + "\nNAMA : " + item.fullname + "\nJabatan : " + item.positionName);
                            //    cell.AddElement(QRCodeSignatureApprover);
                            //}

                        }
                        no1++;
                    }

                    paragraph = new Paragraph(item.fullname, underlineBold);
                    cell.AddElement(paragraph);
                    paragraph = new Paragraph(item.positionName, bold);
                    cell.AddElement(paragraph);

                    table.AddCell(cell);
                }
                // end tanda tangan
                pdfDoc.Add(table);

                #endregion

                pdfDoc.NewPage();

                #region Lampiran

                PdfPTable table1 = new PdfPTable(2);
                float[] width1 = new float[] { 0.667f, 2f };
                table1.WidthPercentage = 100;
                table1.SetWidths(width1);
                table1.HorizontalAlignment = Element.ALIGN_LEFT;
                table1.DefaultCell.Border = 0;
                table1.SpacingBefore = 0f;
                table1.SpacingAfter = 0f;

                PdfPCell cell1 = new PdfPCell();
                Paragraph paragraph1 = new Paragraph();

                cell1 = new PdfPCell();
                cell1.BorderWidthLeft = 1f;
                cell1.BorderWidthRight = 1f;
                cell1.BorderWidthTop = 1f;
                cell1.BorderWidthBottom = 1f;
                cell1.MinimumHeight = 30f;
                paragraph1 = new Paragraph("Nomor Registrasi Memo/Registration Number:" + letterDetail.letter.letterDeliberationNumber, normalDeliberation);
                paragraph1.Alignment = Element.ALIGN_LEFT;
                cell1.AddElement(paragraph1);
                table1.AddCell(cell1);

                cell1 = new PdfPCell();
                cell1.BorderWidthLeft = 0f;
                cell1.BorderWidthRight = 1f;
                cell1.BorderWidthTop = 1f;
                cell1.BorderWidthBottom = 1f;
                cell1.MinimumHeight = 30f;
                paragraph1 = new Paragraph("Tanggal Registrasi Memo/Date of Memo Registration : " + Convert.ToDateTime(letterDetail.letter.letterDate).ToString("dd MMMM yyyy"), normalDeliberation);
                paragraph1.Alignment = Element.ALIGN_LEFT;
                cell1.AddElement(paragraph1);
                table1.AddCell(cell1);

                cell1 = new PdfPCell();
                cell1.BorderWidthLeft = 1f;
                cell1.BorderWidthRight = 1f;
                cell1.BorderWidthTop = 0f;
                cell1.BorderWidthBottom = 1f;
                cell1.MinimumHeight = 100f;
                paragraph1 = new Paragraph("Nomor Memo : " + letterDetail.letter.letterNumber, normalDeliberation);
                paragraph1.Alignment = Element.ALIGN_LEFT;
                cell1.AddElement(paragraph1);
                table1.AddCell(cell1);

                cell1 = new PdfPCell();
                cell1.BorderWidthLeft = 0f;
                cell1.BorderWidthRight = 1f;
                cell1.BorderWidthTop = 0f;
                cell1.BorderWidthBottom = 1f;
                cell1.MinimumHeight = 100f;
                paragraph1 = new Paragraph("(Ringkasan Memo/Summary of the proposal) \n" + letterDetail.letterContent.summary, normalDeliberation);
                paragraph1.Alignment = Element.ALIGN_LEFT;
                cell1.AddElement(paragraph1);
                table1.AddCell(cell1);

                cell1 = new PdfPCell();
                cell1.BorderWidthLeft = 1f;
                cell1.BorderWidthRight = 1f;
                cell1.BorderWidthTop = 0f;
                cell1.BorderWidthBottom = 1f;
                cell1.MinimumHeight = 100f;
                paragraph1 = new Paragraph("Deliberation:", normalDeliberation);
                paragraph1.Alignment = Element.ALIGN_LEFT;
                cell1.AddElement(paragraph1);
                no = 1;
                foreach (var item in letterDetail.delibration)
                {
                    if (letterDetail.receiver.Count() > 1)
                    {
                        paragraph1 = new Paragraph(no + ". " + item.fullname + " - " + item.positionName, normalDeliberation);
                        paragraph1.Alignment = Element.ALIGN_LEFT;
                        cell1.AddElement(paragraph1);
                    }
                    else
                    {
                        paragraph1 = new Paragraph(item.fullname + " - " + item.positionName, normalDeliberation);
                        paragraph1.Alignment = Element.ALIGN_LEFT;
                        cell1.AddElement(paragraph1);
                    }
                    no++;
                }
                table1.AddCell(cell1);

                cell1 = new PdfPCell();
                cell1.BorderWidthLeft = 0f;
                cell1.BorderWidthRight = 1f;
                cell1.BorderWidthTop = 0f;
                cell1.BorderWidthBottom = 1f;
                cell1.MinimumHeight = 100f;
                paragraph1 = new Paragraph("Catatan / Notes : \n", underlineDeliberation);
                paragraph1.Alignment = Element.ALIGN_LEFT;
                cell1.AddElement(paragraph1);
                no = 1;
                foreach (var item in letterDetail.delibration)
                {
                    if (letterDetail.receiver.Count() > 1)
                    {
                        paragraph1 = new Paragraph(no + ". " + item.commentdlbrt, normalDeliberation);
                        paragraph1.Alignment = Element.ALIGN_LEFT;
                        cell1.AddElement(paragraph1);
                    }
                    else
                    {
                        paragraph1 = new Paragraph(item.commentdlbrt, normalDeliberation);
                        paragraph1.Alignment = Element.ALIGN_LEFT;
                        cell1.AddElement(paragraph1);
                    }
                    no++;
                }
                table1.AddCell(cell1);

                pdfDoc.Add(table1);

                //var checkerList = letterDetail.checker.Where(p => p.ordernumber != 1).OrderBy(p => p.idLevelChecker).ToList();
                var checkerList = letterDetail.checker.OrderBy(p => p.idLevelChecker).ToList();
                foreach (var item in checkerList)
                {
                    var getLogComment = letterDetail.log.Where(p => p.idUserLog == item.idUserChecker ||  p.description.Contains("pemeriksa ke")).OrderByDescending(p => p.createdOn).FirstOrDefault();
                    var comment = "";
                    string approveDate = "";
                    if (getLogComment != null)
                    {
                        comment = getLogComment.comment;
                        approveDate = Convert.ToDateTime(getLogComment.createdOn).ToString("dd MMMM yyyy");
                    }
                    table1 = new PdfPTable(3);
                    width = new float[] { 1f, 2f, 1f };
                    table1.WidthPercentage = 100;
                    table1.SetWidths(width);
                    table1.HorizontalAlignment = Element.ALIGN_LEFT;
                    table1.DefaultCell.Border = 0;
                    table1.SpacingBefore = 0f;
                    table1.SpacingAfter = 0f;

                    cell1 = new PdfPCell();
                    cell1.Rowspan = 2;
                    cell1.BorderWidthLeft = 1f;
                    cell1.BorderWidthRight = 1f;
                    cell1.BorderWidthTop = 0f;
                    cell1.BorderWidthBottom = 1f;
                    cell1.MinimumHeight = 120f;
                    if (letterDetail.letter.statusCode == 2)
                    {
                        paragraph1 = new Paragraph("Belum diputuskan / Undecided \n", normalDeliberation);
                    }
                    else if (letterDetail.letter.statusCode == 5)
                    {
                        paragraph1 = new Paragraph("Ditolak / Reject \n", normalDeliberation);
                    }
                    else
                    {
                        if (item.idLevelChecker == 2 ||item.idLevelChecker == 1)
                        {
                            paragraph1 = new Paragraph("Disetujui / Approved \n", normalDeliberation);
                        }
                        else
                        {
                            paragraph1 = new Paragraph("Mengetahui / Understand \n", normalDeliberation);

                        }
                    }

                    paragraph1.Alignment = Element.ALIGN_LEFT;
                    cell1.AddElement(paragraph1);
                    paragraph1 = new Paragraph(item.positionName, normalDeliberation);
                    paragraph1.Alignment = Element.ALIGN_LEFT;
                    cell1.AddElement(paragraph1);
                    table1.AddCell(cell1);

                    cell1 = new PdfPCell();
                    cell1.Rowspan = 2;
                    cell1.BorderWidthLeft = 0f;
                    cell1.BorderWidthRight = 1f;
                    cell1.BorderWidthTop = 0f;
                    cell1.BorderWidthBottom = 1f;
                    cell1.MinimumHeight = 120f;
                    paragraph1 = new Paragraph("Catatan / Notes : \n", underlineDeliberation);
                    paragraph1.Alignment = Element.ALIGN_LEFT;
                    cell1.AddElement(paragraph1);
                    paragraph1 = new Paragraph(comment, normalDeliberation);
                    paragraph1.Alignment = Element.ALIGN_LEFT;
                    cell1.AddElement(paragraph1);
                    table1.AddCell(cell1);

                    cell1 = new PdfPCell();
                    cell1.BorderWidthLeft = 0f;
                    cell1.BorderWidthRight = 1f;
                    cell1.BorderWidthTop = 0f;
                    cell1.BorderWidthBottom = 1f;
                    cell1.MinimumHeight = 40f;
                    paragraph1 = new Paragraph("Tanggal : ", underlineDeliberation);
                    paragraph1.Alignment = Element.ALIGN_LEFT;
                    //cell1.AddElement(paragraph);
                    paragraph1 = new Paragraph(approveDate, normalDeliberation);
                    paragraph1.Alignment = Element.ALIGN_LEFT;
                    cell1.AddElement(paragraph1);
                    table1.AddCell(cell1);

                    cell1 = new PdfPCell();
                    cell1.BorderWidthLeft = 0f;
                    cell1.BorderWidthRight = 1f;
                    cell1.BorderWidthTop = 0f;
                    cell1.BorderWidthBottom = 1f;
                    paragraph1 = new Paragraph("Tanda Tangan : \n", underlineDeliberation);
                    paragraph1.Alignment = Element.ALIGN_LEFT;
                    cell1.AddElement(paragraph1);
                    if (letterDetail.letter.letterNumber != "NO_LETTER")
                    {
                        if (letterDetail.letter.signatureType == 1)
                        {
                            Image imagettd = GetSignatureImage(item.nip);
                            //cell.AddElement(imagettd);
                            if (imagettd != null)
                            {
                                cell1.AddElement(imagettd);
                            }
                            else
                            {
                                var img = "File tanda tangan tidak ditemukan";
                                paragraph1 = new Paragraph(img, normal);
                                cell1.AddElement(paragraph1);
                            }
                        }
                        else if (letterDetail.letter.signatureType == 2)
                        {

                            Image QRCodeSignatureApprover = GetBarcodeSignature("NIP : " + item.nip + "\nNAMA : " + item.fullname + "\nJabatan : " + item.positionName);
                            cell1.AddElement(QRCodeSignatureApprover);
                        }
                        else if (letterDetail.letter.signatureType == 3)
                        {

                            var img = "";
                            paragraph1 = new Paragraph(img, normal);
                            cell1.AddElement(paragraph1);
                        }
                        else
                        {
                            //innertable1 = new PdfPtable1(2);
                            //float[] widthinner = new float[] { 0.3f, 0.3f };
                            //innertable1.WidthPercentage = 100;
                            //innertable1.SetWidths(widthinner);
                            //innertable1.HorizontalAlignment = Element.ALIGN_LEFT;
                            //innertable1.DefaultCell.Border = 1;
                            //innertable1.DefaultCell.PaddingBottom = 8;

                            //innercell = new PdfPCell();
                            //innercell.BorderWidthLeft = 1f;
                            //innercell.BorderWidthRight = 1f;
                            //innercell.BorderWidthTop = 1f;
                            //innercell.BorderWidthBottom = 1f;




                            Image imagettd = GetSignatureImage(item.nip);
                            cell1.AddElement(imagettd);

                            Image QRCodeSignatureApprover = GetBarcodeSignature("NIP : " + item.nip + "\nNAMA : " + item.fullname + "\nJabatan : " + item.positionName);
                            cell1.AddElement(QRCodeSignatureApprover);
                            //if (imagettd !=null)
                            //{
                            //    cell.AddElement(imagettd);

                            //    Image QRCodeSignatureApprover = GetBarcodeSignature("NIP : " + item.nip + "\nNAMA : " + item.fullname + "\nJabatan : " + item.positionName);
                            //    cell.AddElement(QRCodeSignatureApprover);
                            //}
                            //else
                            //{
                            //    var img = "File tanda tangan tidak di temukan";
                            //    paragraph = new Paragraph(img, normal);

                            //    cell.AddElement(paragraph);
                            //    Image QRCodeSignatureApprover = GetBarcodeSignature("NIP : " + item.nip + "\nNAMA : " + item.fullname + "\nJabatan : " + item.positionName);
                            //    cell.AddElement(QRCodeSignatureApprover);
                            //}

                        }
                    }

                    paragraph1 = new Paragraph(item.fullname, bold);
                    paragraph1.Alignment = Element.ALIGN_CENTER;
                    cell1.AddElement(paragraph1);
                    table1.AddCell(cell1);


                    pdfDoc.Add(table1);
                }


                #endregion




                

                pdfDoc.Close();

                var dateLetterFormat = Convert.ToDateTime(letterDetail.letter.letterDate).ToString("dd MMMM yyyy");
                var letterNumber = letterDetail.letter.letterNumber;
                byte[] bytess = ms.ToArray();
                byte[] bytes = AddPageNumbers(bytess, dateLetterFormat, letterNumber, "Memo");
                ms.Close();

                return File(bytes.ToArray(), "application/pdf", fileName);
            }

        }

        public async Task<FileResult> PrintMemoPengadaan(Guid id)
        {
            var token = HttpContext.Session.GetString("token");
            GeneralOutputModel generalOutput = new GeneralOutputModel();
            OutputDetailMemo letterDetail = new OutputDetailMemo();
            ParamGetDetailMemo pr = new ParamGetDetailMemo();
            pr.idLetter = id;

            generalOutput = await _dataAccessProvider.GetDetailMemoPengadaan("Memo/GetDetailsMemoPengadaanWebPrivieWeb/", token, pr);
            var jsonApiResponseSerialize = JsonConvert.SerializeObject(generalOutput.Result);
            letterDetail = JsonConvert.DeserializeObject<OutputDetailMemo>(jsonApiResponseSerialize);

            var letternumber = letterDetail.letter.letterNumber;
            letternumber = letternumber.Replace(".", "_");
            var fileName = letternumber + ".pdf";
            if (letterDetail.letter.statusCode != 4)
            {
                fileName = "Draft_" + letterDetail.letter.about + ".pdf";
            }
            StringBuilder sb = new StringBuilder();
            sb.Append(letterDetail.letterContent.letterContent);
            StringReader sr = new StringReader(sb.ToString());
            //Document pdfDoc = new Document(PageSize.A4, 56.7f, 56.7f, 113.4f, 99.225f);
            Document pdfDoc = new Document(PageSize.A4, 56.7f, 56.7f, 66.5f, 99.225f);
            HTMLWorker htmlparser = new HTMLWorker(pdfDoc);

                using (MemoryStream ms = new MemoryStream())
                {
                    PdfWriter pdfWriter = PdfWriter.GetInstance(pdfDoc, ms);
                    pdfDoc.Open();
                    //Font
                    int no;
                    var fontName = "Calibri";
                    string fontPath = Path.Combine(_environment.WebRootPath, "fonts/Calibri.ttf");
                    //FontFactory.Register(fontPath);

                    Font normal = FontFactory.GetFont(fontName, 12, Font.NORMAL, BaseColor.BLACK);
                    Font bold = FontFactory.GetFont(fontName, 10, Font.BOLD, BaseColor.BLACK);
                    Font underline = FontFactory.GetFont(fontName, 12, Font.UNDERLINE, BaseColor.BLACK);
                    Font underlineBold = FontFactory.GetFont(fontName, 12, Font.BOLD | Font.UNDERLINE, BaseColor.BLACK);

                    Font normalDeliberation = FontFactory.GetFont(fontName, 10, Font.NORMAL, BaseColor.BLACK);
                    Font underlineDeliberation = FontFactory.GetFont(fontName, 10, Font.UNDERLINE, BaseColor.BLACK);
                    Font underlineDeliberationBold = FontFactory.GetFont(fontName, 10, Font.BOLD | Font.UNDERLINE, BaseColor.BLACK);
                    // Colors
                    BaseColor colorBlack = new BaseColor(System.Drawing.ColorTranslator.FromHtml("#000000"));
                    BaseColor colorWhite = new BaseColor(System.Drawing.ColorTranslator.FromHtml("#FFFFFF"));

                    PdfPTable table = new PdfPTable(2);
                    float[] width = new float[] { 0.667f, 2f };
                    table.WidthPercentage = 100;
                    table.SetWidths(width);
                    table.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.DefaultCell.Border = 0;
                    table.SpacingBefore = 0f;
                    table.SpacingAfter = 0f;

                    PdfPCell cell = new PdfPCell();
                    Paragraph paragraph = new Paragraph();

                    #region header
                

                    //table = new PdfPTable(1);
                    //width = new float[] { 2f };
                    //table.WidthPercentage = 100;
                    //table.SetWidths(width);
                    //table.HorizontalAlignment = Element.ALIGN_LEFT;
                    //table.DefaultCell.Border = 0;
                    //table.SpacingBefore = 00f;
                    //table.SpacingAfter = 0f;


                    //table.AddCell(new Phrase("MEMO", bold));
                    //pdfDoc.Add(table);

                    table = new PdfPTable(2);
                    width = new float[] { 0.2f, 1f };
                    table.WidthPercentage = 100;
                    table.SetWidths(width);
                    table.HorizontalAlignment = Element.ALIGN_RIGHT;
                    table.DefaultCell.Border = 0;
                    table.SpacingBefore = 0f;
                    table.SpacingAfter = 0f;

                    cell.Border = 0;
                    paragraph = new Paragraph("Nomor", normal);
                    paragraph.Alignment = Element.ALIGN_LEFT;
                    cell.AddElement(paragraph);
                    table.AddCell(cell);

                    //cell = new PdfPCell();
                    //cell.Border = 0;
                    //paragraph = new Paragraph(":", normal);
                    //paragraph.Alignment = Element.ALIGN_RIGHT;
                    //cell.AddElement(paragraph);
                    //table.AddCell(cell);

                    cell = new PdfPCell();
                    cell.Border = 0;
                    paragraph = new Paragraph(":"+" "+letterDetail.letter.letterNumber, normal);
                    paragraph.Alignment = Element.ALIGN_LEFT;
                    cell.AddElement(paragraph);
                    table.AddCell(cell);

                    cell = new PdfPCell();
                    cell.Border = 0;
                    paragraph = new Paragraph("Tanggal", normal);
                    paragraph.Alignment = Element.ALIGN_LEFT;
                    cell.AddElement(paragraph);
                    table.AddCell(cell);

                    //cell = new PdfPCell();
                    //cell.Border = 0;
                    //paragraph = new Paragraph(":", normal);
                    //paragraph.Alignment = Element.ALIGN_RIGHT;
                    //cell.AddElement(paragraph);
                    //table.AddCell(cell);

                    cell = new PdfPCell();
                    cell.Border = 0;
                    paragraph = new Paragraph(":"+" "+Convert.ToDateTime(letterDetail.letter.letterDate).ToString("dd MMMM yyyy"), normal);
                    paragraph.Alignment = Element.ALIGN_LEFT;
                    cell.AddElement(paragraph);
                    table.AddCell(cell);

                    cell = new PdfPCell();
                    cell.Border = 0;
                    paragraph = new Paragraph("Kepada", normal);
                    paragraph.Alignment = Element.ALIGN_LEFT;
                    cell.AddElement(paragraph);
                    table.AddCell(cell);

                    //cell = new PdfPCell();
                    //cell.Border = 0;
                    //paragraph = new Paragraph(":", normal);
                    //paragraph.Alignment = Element.ALIGN_RIGHT;
                    //cell.AddElement(paragraph);
                    //table.AddCell(cell);

                    cell = new PdfPCell();
                    cell.Border = 0;
                    no = 1;
                    foreach (var item in letterDetail.receiver)
                    {
                        if (letterDetail.receiver.Count() > 1)
                        {
                            paragraph = new Paragraph(":"+" "+no + ". " + item.fullname + " - " + item.positionName, normal);
                            paragraph.Alignment = Element.ALIGN_LEFT;
                            cell.AddElement(paragraph);
                        }
                        else
                        {
                            paragraph = new Paragraph(":"+" "+item.fullname + " - " + item.positionName, normal);
                            paragraph.Alignment = Element.ALIGN_LEFT;
                            cell.AddElement(paragraph);
                        }
                        no++;
                    }
                    table.AddCell(cell);

                    cell = new PdfPCell();
                    cell.Border = 0;
                    paragraph = new Paragraph("Tembusan", normal);
                    paragraph.Alignment = Element.ALIGN_LEFT;
                    cell.AddElement(paragraph);
                    table.AddCell(cell);

                    //cell = new PdfPCell();
                    //cell.Border = 0;
                    //paragraph = new Paragraph(":", normal);
                    //paragraph.Alignment = Element.ALIGN_RIGHT;
                    //cell.AddElement(paragraph);
                    //table.AddCell(cell);

                    cell = new PdfPCell();
                    cell.Border = 0;
                    no = 1;
                    foreach (var item in letterDetail.copy)
                    {
                        if (letterDetail.copy.Count() > 1)
                        {
                            paragraph = new Paragraph(":"+" "+no + ". " + item.fullname + " - " + item.positionName, normal);
                            paragraph.Alignment = Element.ALIGN_LEFT;
                            cell.AddElement(paragraph);
                        }
                        else
                        {
                            paragraph = new Paragraph(":"+" "+item.fullname + " - " + item.positionName, normal);
                            paragraph.Alignment = Element.ALIGN_LEFT;
                            cell.AddElement(paragraph);
                        }
                        no++;
                    }
                    if (letterDetail.copy.Count() == 0)
                    {
                        paragraph = new Paragraph("-", normal);
                        paragraph.Alignment = Element.ALIGN_LEFT;
                        cell.AddElement(paragraph);
                    }
                    table.AddCell(cell);

                    cell = new PdfPCell();
                    cell.Border = 0;
                    paragraph = new Paragraph("Dari", normal);
                    paragraph.Alignment = Element.ALIGN_LEFT;
                    cell.AddElement(paragraph);
                    table.AddCell(cell);

                    //cell = new PdfPCell();
                    //cell.Border = 0;
                    //paragraph = new Paragraph(":", normal);
                    //paragraph.Alignment = Element.ALIGN_RIGHT;
                    //cell.AddElement(paragraph);
                    //table.AddCell(cell);

                    cell = new PdfPCell();
                    cell.Border = 0;
                    paragraph = new Paragraph(":"+" "+letterDetail.sender[0].fullname + " - " + letterDetail.sender[0].positionName, normal);
                    paragraph.Alignment = Element.ALIGN_LEFT;
                    cell.AddElement(paragraph);
                    table.AddCell(cell);

                    cell = new PdfPCell();
                    cell.Border = 0;
                    paragraph = new Paragraph("Perihal", normal);
                    paragraph.Alignment = Element.ALIGN_LEFT;
                    cell.AddElement(paragraph);
                    table.AddCell(cell);

                    //cell = new PdfPCell();
                    //cell.Border = 0;
                    //paragraph = new Paragraph(":", normal);
                    //paragraph.Alignment = Element.ALIGN_RIGHT;
                    //cell.AddElement(paragraph);
                    //table.AddCell(cell);

                    cell = new PdfPCell();
                    cell.Border = 0;
                    paragraph = new Paragraph(":"+" "+letterDetail.letter.about, normal);
                    paragraph.Alignment = Element.ALIGN_LEFT;
                    cell.AddElement(paragraph);
                    table.AddCell(cell);

                    cell = new PdfPCell();
                    cell.Border = 0;
                    paragraph = new Paragraph("Lampiran", normal);
                    paragraph.Alignment = Element.ALIGN_LEFT;
                    cell.AddElement(paragraph);
                    table.AddCell(cell);

                    //cell = new PdfPCell();
                    //cell.Border = 0;
                    //paragraph = new Paragraph(":", normal);
                    //paragraph.Alignment = Element.ALIGN_RIGHT;
                    //cell.AddElement(paragraph);
                    //table.AddCell(cell);

                    var lampiran = letterDetail.letter.attachmentDesc == null ? "-" : letterDetail.letter.attachmentDesc;
                    cell = new PdfPCell();
                    cell.Border = 0;
                    paragraph = new Paragraph(":"+" "+lampiran, bold);
                    paragraph.Alignment = Element.ALIGN_LEFT;
                    cell.AddElement(paragraph);
                    table.AddCell(cell);

                    //Add table to document
                    pdfDoc.Add(table);
               

                    Paragraph line = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(2F, 100.0F, colorBlack, Element.ALIGN_CENTER, 1)));
                    line.SpacingBefore = 0f;
                    line.SetLeading(2F, 0.5F);
                    pdfDoc.Add(line);

                    Paragraph lines = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(2F, 100.0F, colorBlack, Element.ALIGN_CENTER, 1)));
                    lines.SpacingBefore = 0f;
                    line.SetLeading(0.5F, 0.5F);
                    pdfDoc.Add(line);

                    Paragraph liness = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(2F, 100.0F, colorWhite, Element.ALIGN_CENTER, 1)));
                    liness.SpacingBefore = 0f;
                    liness.SetLeading(0.5F, 0.5F);
                    pdfDoc.Add(liness);

                    //Paragraph linesss = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(2F, 100.0F, colorWhite, Element.ALIGN_CENTER, 1)));
                    //linesss.SpacingBefore = 0f;
                    //linesss.SetLeading(0.5F, 0.5F);
                    //pdfDoc.Add(linesss);


                    table = new PdfPTable(1);
                    width = new float[] { 2f };
                    table.WidthPercentage = 100;
                    table.SetWidths(width);
                    table.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.DefaultCell.Border = 0;
                    table.SpacingBefore = 00f;
                    table.SpacingAfter = 0f;


                    //table.AddCell(new Phrase("Dengan hormat,", normal));
                    //pdfDoc.Add(table);

                    Paragraph linessss = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(2F, 100.0F, colorWhite, Element.ALIGN_CENTER, 1)));
                    linessss.SpacingBefore = 0f;
                    linessss.SetLeading(0.5F, 0.5F);
                    pdfDoc.Add(linessss);

                    Paragraph lin = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(2F, 100.0F, colorWhite, Element.ALIGN_CENTER, 1)));
                    lin.SpacingBefore = 0f;
                    lin.SetLeading(0.5F, 0.5F);
                    pdfDoc.Add(lin);

                    #endregion

                    //ADD HTML CONTENT
                    htmlparser.Parse(sr);

                    #region Tanda Tangan

                    Paragraph a = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(2F, 100.0F, colorWhite, Element.ALIGN_CENTER, 1)));
                    a.SpacingBefore = 0f;
                    a.SetLeading(0.5F, 0.5F);
                    pdfDoc.Add(a);



                    cell = new PdfPCell();
                    table.AddCell(new Phrase("", normal));

                    cell = new PdfPCell();
                    table.AddCell(new Phrase(" ", normal));
                    cell = new PdfPCell();
                    table.AddCell(new Phrase(" ", normal));
                    cell = new PdfPCell();
                    table.AddCell(new Phrase("Hormat kami, \n" + "PT BNI Life Insurance", normal));
                    //table.AddCell(new Phrase("Best Regards", normal));
                    pdfDoc.Add(table);



                    string filename = letterDetail.sender[0].nip;


                    // Tanda tangan
                    var dirChecker = letterDetail.sender.OrderBy(p => p.idUserSender).ToList();
                    //var dirChecker = letterDetail.checker.Where(p => p.idLevelChecker == 2 || p.idLevelChecker == 1).ToList();

                    table = new PdfPTable(dirChecker.Count());
                    width = new float[dirChecker.Count()];
                    for (int i = 0; i < dirChecker.Count(); i++)
                    {
                        width[i] = 1f;
                    }
                    table.WidthPercentage = 100;
                    table.SetWidths(width);
                    table.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.DefaultCell.Border = 0;
                    var no1 = 0;


                    //cell = new PdfPCell();
                    //table.AddCell(new Phrase("Direksi PT BNI Life Insurance", normal));
                    foreach (var item in dirChecker)
                    {
                        cell = new PdfPCell();
                        cell.Border = 0;




                        if (letterDetail.letter.letterNumber != "NO_LETTER")
                        {
                            if (letterDetail.letter.signatureType == 1)
                            {

                                Image imagettd = GetSignatureImage(item.nip);
                                //cell.AddElement(imagettd);
                                if (imagettd != null)
                                {
                                    cell.AddElement(imagettd);
                                }
                                else
                                {
                                    var img = "File tanda tangan tidak ditemukan";
                                    paragraph = new Paragraph(img, normal);
                                    cell.AddElement(paragraph);
                                }

                            }
                            else if (letterDetail.letter.signatureType == 2)
                            {


                                Image QRCodeSignatureApprover = GetBarcodeSignature("NIP : " + item.nip + "\nNAMA : " + item.fullname + "\nJabatan : " + item.positionName);
                                QRCodeSignatureApprover.ScaleAbsoluteHeight(10f);
                                cell.AddElement(QRCodeSignatureApprover);
                            }
                            else if (letterDetail.letter.signatureType == 3)
                            {
                                var img = "";
                                paragraph = new Paragraph(img, normal);
                                cell.AddElement(paragraph);
                                Paragraph linessia = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(2F, 100.0F, colorWhite, Element.ALIGN_CENTER, 1)));
                                linessia.SpacingBefore = 0f;
                                linessia.SetLeading(0.5F, 0.5F);
                                pdfDoc.Add(linessia);

                                Paragraph linessiaa = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(2F, 100.0F, colorWhite, Element.ALIGN_CENTER, 1)));
                                linessiaa.SpacingBefore = 0f;
                                linessiaa.SetLeading(0.5F, 0.5F);
                                pdfDoc.Add(linessiaa);

                                Paragraph linessiaaa = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(2F, 100.0F, colorWhite, Element.ALIGN_CENTER, 1)));
                                linessiaaa.SpacingBefore = 0f;
                                linessiaaa.SetLeading(0.5F, 0.5F);
                                pdfDoc.Add(linessiaaa);
                            }
                            else
                            {

                                Image imagettd = GetSignatureImage(item.nip);
                                cell.AddElement(imagettd);
                                Image QRCodeSignatureApprover = GetBarcodeSignature("NIP : " + item.nip + "\nNAMA : " + item.fullname + "\nJabatan : " + item.positionName);
                                cell.AddElement(QRCodeSignatureApprover);
                                //if (imagettd != null)
                                //{
                                //    cell.AddElement(imagettd);
                                //    Image QRCodeSignatureApprover = GetBarcodeSignature("NIP : " + item.nip + "\nNAMA : " + item.fullname + "\nJabatan : " + item.positionName);
                                //    cell.AddElement(QRCodeSignatureApprover);
                                //}
                                //else
                                //{
                                //    var img = "File tanda tangan tidak di temukan";
                                //    paragraph = new Paragraph(img, normal);
                                //    cell.AddElement(paragraph);
                                //    Image QRCodeSignatureApprover = GetBarcodeSignature("NIP : " + item.nip + "\nNAMA : " + item.fullname + "\nJabatan : " + item.positionName);
                                //    cell.AddElement(QRCodeSignatureApprover);
                                //}

                            }
                            no1++;
                        }

                        paragraph = new Paragraph(item.fullname, underlineBold);
                        cell.AddElement(paragraph);
                        paragraph = new Paragraph(item.positionName, bold);
                        cell.AddElement(paragraph);

                        table.AddCell(cell);
                    }
                    // end tanda tangan
                    pdfDoc.Add(table);

                    #endregion

                    pdfDoc.NewPage();

                    #region Lampiran

                    PdfPTable table1 = new PdfPTable(2);
                    float[] width1 = new float[] { 0.667f, 2f };
                    table1.WidthPercentage = 100;
                    table1.SetWidths(width1);
                    table1.HorizontalAlignment = Element.ALIGN_LEFT;
                    table1.DefaultCell.Border = 0;
                    table1.SpacingBefore = 0f;
                    table1.SpacingAfter = 0f;

                    PdfPCell cell1 = new PdfPCell();
                    Paragraph paragraph1 = new Paragraph();

                    cell1 = new PdfPCell();
                    cell1.BorderWidthLeft = 1f;
                    cell1.BorderWidthRight = 1f;
                    cell1.BorderWidthTop = 1f;
                    cell1.BorderWidthBottom = 1f;
                    cell1.MinimumHeight = 30f;
                    paragraph1 = new Paragraph("Nomor Registrasi Memo/Registration Number:" + letterDetail.letter.letterDeliberationNumber, normalDeliberation);
                    paragraph1.Alignment = Element.ALIGN_LEFT;
                    cell1.AddElement(paragraph1);
                    table1.AddCell(cell1);

                    cell1 = new PdfPCell();
                    cell1.BorderWidthLeft = 0f;
                    cell1.BorderWidthRight = 1f;
                    cell1.BorderWidthTop = 1f;
                    cell1.BorderWidthBottom = 1f;
                    cell1.MinimumHeight = 30f;
                    paragraph1 = new Paragraph("Tanggal Registrasi Memo/Date of Memo Registration : " + Convert.ToDateTime(letterDetail.letter.letterDate).ToString("dd MMMM yyyy"), normalDeliberation);
                    paragraph1.Alignment = Element.ALIGN_LEFT;
                    cell1.AddElement(paragraph1);
                    table1.AddCell(cell1);

                    cell1 = new PdfPCell();
                    cell1.BorderWidthLeft = 1f;
                    cell1.BorderWidthRight = 1f;
                    cell1.BorderWidthTop = 0f;
                    cell1.BorderWidthBottom = 1f;
                    cell1.MinimumHeight = 100f;
                    paragraph1 = new Paragraph("Nomor Memo : " + letterDetail.letter.letterNumber, normalDeliberation);
                    paragraph1.Alignment = Element.ALIGN_LEFT;
                    cell1.AddElement(paragraph1);
                    table1.AddCell(cell1);

                    cell1 = new PdfPCell();
                    cell1.BorderWidthLeft = 0f;
                    cell1.BorderWidthRight = 1f;
                    cell1.BorderWidthTop = 0f;
                    cell1.BorderWidthBottom = 1f;
                    cell1.MinimumHeight = 100f;
                    paragraph1 = new Paragraph("(Ringkasan Memo/Summary of the proposal) \n" + letterDetail.letterContent.summary, normalDeliberation);
                    paragraph1.Alignment = Element.ALIGN_LEFT;
                    cell1.AddElement(paragraph1);
                    table1.AddCell(cell1);
                    pdfDoc.Add(table1);
                //var delebrationList = letterDetail.delibration.OrderBy(p => p.idDeliberation).ToList();


                    //cell1 = new PdfPCell();
                    //cell1.BorderWidthLeft = 1f;
                    //cell1.BorderWidthRight = 1f;
                    //cell1.BorderWidthTop = 0f;
                    //cell1.BorderWidthBottom = 1f;
                    //cell1.MinimumHeight = 100f;
                    //paragraph1 = new Paragraph("Deliberation Column:", normalDeliberation);
                    //paragraph1.Alignment = Element.ALIGN_LEFT;
                    //cell1.AddElement(paragraph1);
                    //no = 1;
                    //foreach (var item in letterDetail.delibration)
                    //{
                    //    if (letterDetail.receiver.Count() > 1)
                    //    {
                    //        paragraph1 = new Paragraph(no + ". " + item.fullname + " - " + item.positionName, normalDeliberation);
                    //        paragraph1.Alignment = Element.ALIGN_LEFT;
                    //        cell1.AddElement(paragraph1);
                    //    }
                    //    else
                    //    {
                    //        paragraph1 = new Paragraph(item.fullname + " - " + item.positionName, normalDeliberation);
                    //        paragraph1.Alignment = Element.ALIGN_LEFT;
                    //        cell1.AddElement(paragraph1);
                    //    }
                    //    no++;
                    //}
                    //table1.AddCell(cell1);

                    //cell1 = new PdfPCell();
                    //cell1.BorderWidthLeft = 0f;
                    //cell1.BorderWidthRight = 1f;
                    //cell1.BorderWidthTop = 0f;
                    //cell1.BorderWidthBottom = 1f;
                    //cell1.MinimumHeight = 100f;
                    //paragraph1 = new Paragraph("catatan / notes : \n", underlineDeliberation);
                    //paragraph1.Alignment = Element.ALIGN_LEFT;
                    //cell1.AddElement(paragraph1);
                    //no = 1;
                    //foreach (var item in letterDetail.delibration)
                    //{
                    //    if (letterDetail.receiver.Count() > 1)
                    //    {
                    //        paragraph1 = new Paragraph(no + ". " + item.commentdlbrt, normalDeliberation);
                    //        paragraph1.Alignment = Element.ALIGN_LEFT;
                    //        cell1.AddElement(paragraph1);
                    //    }
                    //    else
                    //    {
                    //        paragraph1 = new Paragraph(item.commentdlbrt, normalDeliberation);
                    //        paragraph1.Alignment = Element.ALIGN_LEFT;
                    //        cell1.AddElement(paragraph1);
                    //    }
                    //    no++;
                    //}
                    //table1.AddCell(cell1);

                    //pdfDoc.Add(table1);

                #region delebrasi
                var delebrationList = letterDetail.delibration.OrderBy(p => p.idDeliberation).ToList();

                            foreach (var item in delebrationList)
                            {
                                //var getLogComment = letterDetail.log.Where(p => p.idUserLog == item.idUser && p.description.Contains("pemeriksa ke")).OrderByDescending(p => p.createdOn).FirstOrDefault();
                                var comment = "";
                               
                                if (delebrationList.Count > 0)
                                {
                                    comment = item.commentdlbrt;
                                    
                                }
                                table1 = new PdfPTable(2);
                                width = new float[] { 1f, 1f };
                                table1.WidthPercentage = 100;
                                table1.SetWidths(width);
                                table1.HorizontalAlignment = Element.ALIGN_LEFT;
                                table1.DefaultCell.Border = 0;
                                table1.SpacingBefore = 0f;
                                table1.SpacingAfter = 0f;

                                cell1 = new PdfPCell();
                                //cell1.Rowspan = 2;
                                cell1.BorderWidthLeft = 1f;
                                cell1.BorderWidthRight = 1f;
                                cell1.BorderWidthTop = 0f;
                                cell1.BorderWidthBottom = 1f;
                                cell1.MinimumHeight = 100f;
                                //if (letterDetail.letter.statusCode == 2)
                                //{
                                //    paragraph1 = new Paragraph("Belum diputuskan / Undecided \n", normalDeliberation);
                                //}
                                //else if (letterDetail.letter.statusCode == 5)
                                //{
                                //    paragraph1 = new Paragraph("Ditolak / Reject \n", normalDeliberation);
                                //}
                                //else
                                //{
                                //    if (item.idLevelChecker == 2 ||item.idLevelChecker == 1)
                                //    {
                                //        paragraph1 = new Paragraph("Disetujui / Approved \n", normalDeliberation);
                                //    }
                                //    else
                                //    {
                                //        paragraph1 = new Paragraph("Mengetahui / Understand \n", normalDeliberation);

                                //    }
                                //}
                                //paragraph1 = new Paragraph("", normalDeliberation);
                                paragraph1 = new Paragraph("Deliberation:\n", normalDeliberation);
                                paragraph1.Alignment = Element.ALIGN_LEFT;
                                cell1.AddElement(paragraph1);
                                paragraph1 = new Paragraph( item.fullname +"\n"+ item.positionName, normalDeliberation);
                                paragraph1.Alignment = Element.ALIGN_LEFT;
                                cell1.AddElement(paragraph1);
                                table1.AddCell(cell1);

                                cell1 = new PdfPCell();
                                //cell1.Rowspan = 2;
                                cell1.BorderWidthLeft = 0f;
                                cell1.BorderWidthRight = 1f;
                                cell1.BorderWidthTop = 0f;
                                cell1.BorderWidthBottom = 1f;
                                cell1.MinimumHeight = 40f;
                                paragraph1 = new Paragraph("Catatan / Notes : \n", underlineDeliberation);
                                paragraph1.Alignment = Element.ALIGN_LEFT;
                                cell1.AddElement(paragraph1);
                                paragraph1 = new Paragraph(comment, normalDeliberation);
                                paragraph1.Alignment = Element.ALIGN_LEFT;


                    //cell1 = new PdfPCell();
                    //cell1.BorderWidthLeft = 0f;
                    //cell1.BorderWidthRight = 1f;
                    //cell1.BorderWidthTop = 0f;
                    //cell1.BorderWidthBottom = 1f;
                    //cell1.MinimumHeight = 100f;
                    //paragraph1 = new Paragraph("catatan / notes : \n", underlineDeliberation);
                    //paragraph1.Alignment = Element.ALIGN_LEFT;
                    //cell1.AddElement(paragraph1);
                    //no = 1;
                    //foreach (var item in letterDetail.delibration)
                    //{
                    //    if (letterDetail.receiver.Count() > 1)
                    //    {
                    //        paragraph1 = new Paragraph(no + ". " + item.commentdlbrt, normalDeliberation);
                    //        paragraph1.Alignment = Element.ALIGN_LEFT;
                    //        cell1.AddElement(paragraph1);
                    //    }
                    //    else
                    //    {
                    //        paragraph1 = new Paragraph(item.commentdlbrt, normalDeliberation);
                    //        paragraph1.Alignment = Element.ALIGN_LEFT;
                    //        cell1.AddElement(paragraph1);
                    //    }
                    //    no++;
                    //}
                    //table1.AddCell(cell1);

                    //pdfDoc.Add(table1);


                    //cell1.AddElement(paragraph1);
                    //table1.AddCell(cell1);

                    //cell1 = new PdfPCell();
                    //cell1.BorderWidthLeft = 0f;
                    //cell1.BorderWidthRight = 1f;
                    //cell1.BorderWidthTop = 0f;
                    //cell1.BorderWidthBottom = 1f;
                    //cell1.MinimumHeight = 40f;
                    //paragraph1 = new Paragraph("Tanggal : ", underlineDeliberation);
                    //paragraph1.Alignment = Element.ALIGN_LEFT;
                    ////cell1.AddElement(paragraph);
                    //paragraph1 = new Paragraph("", normalDeliberation);
                    //paragraph1.Alignment = Element.ALIGN_LEFT;
                    //cell1.AddElement(paragraph1);
                    //table1.AddCell(cell1);

                    //cell1 = new PdfPCell();
                    //cell1.BorderWidthLeft = 0f;
                    //cell1.BorderWidthRight = 1f;
                    //cell1.BorderWidthTop = 0f;
                    //cell1.BorderWidthBottom = 1f;
                    //paragraph1 = new Paragraph("Tanda Tangan : \n", underlineDeliberation);
                    //paragraph1.Alignment = Element.ALIGN_LEFT;
                    //cell1.AddElement(paragraph1);


                    //paragraph1 = new Paragraph(item.fullname, bold);
                   paragraph1.Alignment = Element.ALIGN_LEFT;
                    cell1.AddElement(paragraph1);
                                table1.AddCell(cell1);


                    pdfDoc.Add(table1);
                            }
                #endregion

                //var checkerList = letterDetail.checker.Where(p => p.ordernumber != 1).OrderBy(p => p.idLevelChecker).ToList();
                var checkerList = letterDetail.checker.OrderBy(p => p.idLevelChecker).ToList();
                    foreach (var item in checkerList)
                    {
                        var getLogComment = letterDetail.log.Where(p => p.idUserLog == item.idUserChecker && p.description.Contains("pemeriksa ke")).OrderByDescending(p => p.createdOn).FirstOrDefault();
                        var comment = "";
                        string approveDate = "";
                        if (getLogComment != null)
                        {
                            comment = getLogComment.comment;
                            approveDate = Convert.ToDateTime(getLogComment.createdOn).ToString("dd MMMM yyyy");
                        }
                        table1 = new PdfPTable(3);
                        width = new float[] { 1f, 2f, 1f };
                        table1.WidthPercentage = 100;
                        table1.SetWidths(width);
                        table1.HorizontalAlignment = Element.ALIGN_LEFT;
                        table1.DefaultCell.Border = 0;
                        table1.SpacingBefore = 0f;
                        table1.SpacingAfter = 0f;

                        cell1 = new PdfPCell();
                        cell1.Rowspan = 2;
                        cell1.BorderWidthLeft = 1f;
                        cell1.BorderWidthRight = 1f;
                        cell1.BorderWidthTop = 0f;
                        cell1.BorderWidthBottom = 1f;
                        cell1.MinimumHeight = 120f;
                        if (letterDetail.letter.statusCode == 2)
                        {
                            paragraph1 = new Paragraph("Belum diputuskan / Undecided \n", normalDeliberation);
                        }
                        else if (letterDetail.letter.statusCode == 5)
                        {
                            paragraph1 = new Paragraph("Ditolak / Reject \n", normalDeliberation);
                        }
                        else
                        {
                            if (item.idLevelChecker == 2 ||item.idLevelChecker == 1)
                            {
                                paragraph1 = new Paragraph("Disetujui / Approved \n", normalDeliberation);
                            }
                            else
                            {
                                paragraph1 = new Paragraph("Mengetahui / Understand \n", normalDeliberation);

                            }
                        }

                        paragraph1.Alignment = Element.ALIGN_LEFT;
                        cell1.AddElement(paragraph1);
                        paragraph1 = new Paragraph(item.positionName, normalDeliberation);
                        paragraph1.Alignment = Element.ALIGN_LEFT;
                        cell1.AddElement(paragraph1);
                        table1.AddCell(cell1);

                        cell1 = new PdfPCell();
                        cell1.Rowspan = 2;
                        cell1.BorderWidthLeft = 0f;
                        cell1.BorderWidthRight = 1f;
                        cell1.BorderWidthTop = 0f;
                        cell1.BorderWidthBottom = 1f;
                        cell1.MinimumHeight = 120f;
                        paragraph1 = new Paragraph("Catatan / Notes : \n", underlineDeliberation);
                        paragraph1.Alignment = Element.ALIGN_LEFT;
                        cell1.AddElement(paragraph1);
                        paragraph1 = new Paragraph(comment, normalDeliberation);
                        paragraph1.Alignment = Element.ALIGN_LEFT;
                        cell1.AddElement(paragraph1);
                        table1.AddCell(cell1);

                        cell1 = new PdfPCell();
                        cell1.BorderWidthLeft = 0f;
                        cell1.BorderWidthRight = 1f;
                        cell1.BorderWidthTop = 0f;
                        cell1.BorderWidthBottom = 1f;
                        cell1.MinimumHeight = 40f;
                        paragraph1 = new Paragraph("Tanggal : ", underlineDeliberation);
                        paragraph1.Alignment = Element.ALIGN_LEFT;
                        //cell1.AddElement(paragraph);
                        paragraph1 = new Paragraph(approveDate, normalDeliberation);
                        paragraph1.Alignment = Element.ALIGN_LEFT;
                        cell1.AddElement(paragraph1);
                        table1.AddCell(cell1);

                        cell1 = new PdfPCell();
                        cell1.BorderWidthLeft = 0f;
                        cell1.BorderWidthRight = 1f;
                        cell1.BorderWidthTop = 0f;
                        cell1.BorderWidthBottom = 1f;
                        paragraph1 = new Paragraph("Tanda Tangan : \n", underlineDeliberation);
                        paragraph1.Alignment = Element.ALIGN_LEFT;
                        cell1.AddElement(paragraph1);
                        if (letterDetail.letter.letterNumber != "NO_LETTER")
                        {
                            if (letterDetail.letter.signatureType == 1)
                            {
                                Image imagettd = GetSignatureImage(item.nip);
                                //cell.AddElement(imagettd);
                                if (imagettd != null)
                                {
                                    cell1.AddElement(imagettd);
                                }
                                else
                                {
                                    var img = "File tanda tangan tidak ditemukan";
                                    paragraph1 = new Paragraph(img, normal);
                                    cell1.AddElement(paragraph1);
                                }
                            }
                            else if (letterDetail.letter.signatureType == 2)
                            {

                                Image QRCodeSignatureApprover = GetBarcodeSignature("NIP : " + item.nip + "\nNAMA : " + item.fullname + "\nJabatan : " + item.positionName);
                                cell1.AddElement(QRCodeSignatureApprover);
                            }
                            else
                            {
                                //innertable1 = new PdfPtable1(2);
                                //float[] widthinner = new float[] { 0.3f, 0.3f };
                                //innertable1.WidthPercentage = 100;
                                //innertable1.SetWidths(widthinner);
                                //innertable1.HorizontalAlignment = Element.ALIGN_LEFT;
                                //innertable1.DefaultCell.Border = 1;
                                //innertable1.DefaultCell.PaddingBottom = 8;

                                //innercell = new PdfPCell();
                                //innercell.BorderWidthLeft = 1f;
                                //innercell.BorderWidthRight = 1f;
                                //innercell.BorderWidthTop = 1f;
                                //innercell.BorderWidthBottom = 1f;




                                Image imagettd = GetSignatureImage(item.nip);
                                cell1.AddElement(imagettd);

                                Image QRCodeSignatureApprover = GetBarcodeSignature("NIP : " + item.nip + "\nNAMA : " + item.fullname + "\nJabatan : " + item.positionName);
                                cell1.AddElement(QRCodeSignatureApprover);
                                //if (imagettd !=null)
                                //{
                                //    cell.AddElement(imagettd);

                                //    Image QRCodeSignatureApprover = GetBarcodeSignature("NIP : " + item.nip + "\nNAMA : " + item.fullname + "\nJabatan : " + item.positionName);
                                //    cell.AddElement(QRCodeSignatureApprover);
                                //}
                                //else
                                //{
                                //    var img = "File tanda tangan tidak di temukan";
                                //    paragraph = new Paragraph(img, normal);

                                //    cell.AddElement(paragraph);
                                //    Image QRCodeSignatureApprover = GetBarcodeSignature("NIP : " + item.nip + "\nNAMA : " + item.fullname + "\nJabatan : " + item.positionName);
                                //    cell.AddElement(QRCodeSignatureApprover);
                                //}

                            }
                        }

                        paragraph1 = new Paragraph(item.fullname, bold);
                        paragraph1.Alignment = Element.ALIGN_CENTER;
                        cell1.AddElement(paragraph1);
                        table1.AddCell(cell1);


                        pdfDoc.Add(table1);
                    }


                    #endregion


                    pdfDoc.Close();

                    var dateLetterFormat = Convert.ToDateTime(letterDetail.letter.letterDate).ToString("dd MMMM yyyy");
                    var letterNumber = letterDetail.letter.letterNumber;
                    byte[] bytess = ms.ToArray();
                    byte[] bytes = AddPageNumbers(bytess, dateLetterFormat, letterNumber, "Memo");
                    ms.Close();

                    return File(bytes.ToArray(), "application/pdf", fileName);
                }
        }



        #region Print Direksi
            //Done
            private FileContentResult PrivSuratKeputusanDireksi(string letternumber, OutputDetailMemo letterDetail, string fileName)
            {
                #region Surat Keputusan Direksi

                StringBuilder sb = new StringBuilder();
                sb.Append("<div style='padding-left:2px;'>" + letterDetail.letterContent.letterContent + "</div>");
                StringReader sr = new StringReader(sb.ToString());
                //Document pdfDoc = new Document(PageSize.A4, 25, 25, 25, 15);
                //Document pdfDoc = new Document(PageSize.A4, 56, 56, 113, 97);
                Document pdfDoc = new Document(PageSize.A4, 56.7f, 56.7f, 66.5f, 99.225f);
                HTMLWorker htmlparser = new HTMLWorker(pdfDoc);

            
                using (MemoryStream ms = new MemoryStream())
                {
                    PdfWriter pdfWriter = PdfWriter.GetInstance(pdfDoc, ms);
                    pdfDoc.Open();
                    //Font
                    int no;
                    var fontName = "Calibri";
                    string fontPath = Path.Combine(_environment.WebRootPath, "fonts/Calibri.ttf");
                    //FontFactory.Register(fontPath);

                    Font normal = FontFactory.GetFont(fontName, 12, Font.NORMAL, BaseColor.BLACK);
                    Font bold = FontFactory.GetFont(fontName, 12, Font.BOLD, BaseColor.BLACK);
                    Font underline = FontFactory.GetFont(fontName, 12, Font.UNDERLINE, BaseColor.BLACK);
                    Font underlineBold = FontFactory.GetFont(fontName, 12, Font.BOLD | Font.UNDERLINE, BaseColor.BLACK);

                    Font normalDeliberation = FontFactory.GetFont(fontName, 10, Font.NORMAL, BaseColor.BLACK);
                    Font underlineDeliberation = FontFactory.GetFont(fontName, 10, Font.UNDERLINE, BaseColor.BLACK);
                    Font underlineDeliberationBold = FontFactory.GetFont(fontName, 10, Font.BOLD | Font.UNDERLINE, BaseColor.BLACK);
                    // Colors
                    BaseColor colorBlack = new BaseColor(System.Drawing.ColorTranslator.FromHtml("#000000"));
                    BaseColor colorWhite = new BaseColor(System.Drawing.ColorTranslator.FromHtml("#FFFFFF"));

                        PdfPTable table = new PdfPTable(2);
                        float[] width = new float[] { 0.667f, 2f };
                        table.WidthPercentage = 100;
                        table.SetWidths(width);
                        table.HorizontalAlignment = Element.ALIGN_LEFT;
                        table.DefaultCell.Border = 0;
                        table.SpacingBefore = 0f;
                        table.SpacingAfter = 0f;

                        PdfPCell cell = new PdfPCell();
                        Paragraph paragraph = new Paragraph();

                        #region header
                                table = new PdfPTable(1);
                                width = new float[] { 2f };
                                table.WidthPercentage = 100;
                                table.SetWidths(width);
                                table.HorizontalAlignment = Element.ALIGN_LEFT;
                                table.DefaultCell.Border = 0;
                                table.SpacingBefore = 00f;
                                table.SpacingAfter = 0f;

                                cell = new PdfPCell();
                                cell.Border = 0;
                                paragraph = new Paragraph(new Phrase("KEPUTUSAN DIREKSI PT BNI LIFE INSURANCE", bold));
                                paragraph.Alignment = Element.ALIGN_CENTER;
                                cell.AddElement(paragraph);
                                table.AddCell(cell);
                                pdfDoc.Add(table);

                                table = new PdfPTable(1);
                                width = new float[] { 10f };
                                table.WidthPercentage = 100;
                                table.SetWidths(width);
                                table.HorizontalAlignment = Element.ALIGN_CENTER;
                                table.DefaultCell.Border = 0;
                                table.SpacingBefore = 00f;
                                table.SpacingAfter = 0f;

                                cell = new PdfPCell();
                                cell.Border = 0;
                                paragraph = new Paragraph(new Phrase("NOMOR : " + letterDetail.letter.letterNumber, bold));
                                paragraph.Alignment = Element.ALIGN_CENTER;
                                cell.AddElement(paragraph);
                                table.AddCell(cell);
                                pdfDoc.Add(table);


                                table = new PdfPTable(1);
                                width = new float[] { 10f };
                                table.WidthPercentage = 100;
                                table.SetWidths(width);
                                table.HorizontalAlignment = Element.ALIGN_CENTER;
                                table.DefaultCell.Border = 0;
                                table.SpacingBefore = 00f;
                                table.SpacingAfter = 0f;

                                cell = new PdfPCell();
                                cell.Border = 0;
                                paragraph = new Paragraph(new Phrase("TANGGAL " + Convert.ToDateTime(letterDetail.letter.letterDate).ToString("dd MMMM yyyy"), bold));
                                paragraph.Alignment = Element.ALIGN_CENTER;
                                cell.AddElement(paragraph);
                                table.AddCell(cell);
                                pdfDoc.Add(table);

                                table = new PdfPTable(1);
                                width = new float[] { 10f };
                                table.WidthPercentage = 100;
                                table.SetWidths(width);
                                table.HorizontalAlignment = Element.ALIGN_CENTER;
                                table.DefaultCell.Border = 0;
                                table.SpacingBefore = 0f;
                                table.SpacingAfter = 0f;

                                cell = new PdfPCell();
                                cell.Border = 0;
                                paragraph = new Paragraph(new Phrase("TENTANG", bold));
                                paragraph.Alignment = Element.ALIGN_CENTER;
                                cell.AddElement(paragraph);
                                table.AddCell(cell);
                                pdfDoc.Add(table);

                                table = new PdfPTable(1);
                                width = new float[] { 10f };
                                table.WidthPercentage = 100;
                                table.SetWidths(width);
                                table.HorizontalAlignment = Element.ALIGN_CENTER;
                                table.DefaultCell.Border = 0;
                                table.SpacingBefore = 00f;
                                table.SpacingAfter = 0f;

                                cell = new PdfPCell();
                                cell.Border = 0;
                                paragraph = new Paragraph(new Phrase(letterDetail.letter.about, underlineBold));
                                paragraph.Alignment = Element.ALIGN_CENTER;
                                cell.AddElement(paragraph);
                                table.AddCell(cell);
                                pdfDoc.Add(table);

                                    table = new PdfPTable(1);
                                    width = new float[] { 2f };
                                    table.WidthPercentage = 100;
                                    table.SetWidths(width);
                                    table.HorizontalAlignment = Element.ALIGN_LEFT;
                                    table.DefaultCell.Border = 0;
                                    table.SpacingBefore = 00f;
                                    table.SpacingAfter = 0f;

                                Paragraph line = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(2F, 100.0F, colorBlack, Element.ALIGN_CENTER, 1)));
                                line.SpacingBefore = 0f;
                                line.SetLeading(2F, 0.5F);
                                pdfDoc.Add(line);
                                Paragraph lines = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(2F, 100.0F, colorBlack, Element.ALIGN_CENTER, 1)));
                                lines.SpacingBefore = 0f;
                                line.SetLeading(0.5F, 0.5F);
                                pdfDoc.Add(line);

                                #endregion
                        
                        
                        //ADD HTML CONTENT
                        htmlparser.Parse(sr);

                        #region Tanda Tangan

                        //Paragraph a = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(2F, 100.0F, colorWhite, Element.ALIGN_CENTER, 1)));
                        //a.SpacingBefore = 0f;
                        //a.SetLeading(0.5F, 0.5F);
                        //pdfDoc.Add(a);



                        cell = new PdfPCell();
                        table.AddCell(new Phrase("", normal));

                        cell = new PdfPCell();
                        table.AddCell(new Phrase(" ", normal));
                        cell = new PdfPCell();
                        table.AddCell(new Phrase(" ", normal));
                        cell = new PdfPCell();
                        table.AddCell(new Phrase("Hormat kami, \n" + "PT BNI Life Insurance", normal));
                        //table.AddCell(new Phrase("Best Regards", normal));
                        pdfDoc.Add(table);



                        string filename = letterDetail.sender[0].nip;


                        // Tanda tangan
                        //var dirChecker = letterDetail.checker.OrderBy(p => p.idLevelChecker).ToList();
                         var dirChecker = letterDetail.checker.Where(p => p.idLevelChecker == 2).OrderBy(p => p.idLevelChecker).ToList();

                        table = new PdfPTable(dirChecker.Count());
                        width = new float[dirChecker.Count()];
                        for (int i = 0; i < dirChecker.Count(); i++)
                        {
                            width[i] = 1f;
                        }
                        table.WidthPercentage = 100;
                        table.SetWidths(width);
                        table.HorizontalAlignment = Element.ALIGN_LEFT;
                        table.DefaultCell.Border = 0;
                        var no1 = 0;


                        //cell = new PdfPCell();
                        //table.AddCell(new Phrase("Direksi PT BNI Life Insurance", normal));
                        foreach (var item in dirChecker)
                        {
                            cell = new PdfPCell();
                            cell.Border = 0;




                            if (letterDetail.letter.letterNumber != "NO_LETTER")
                            {
                                if (letterDetail.letter.signatureType == 1)
                                {

                                    Image imagettd = GetSignatureImage(item.nip);
                                    //cell.AddElement(imagettd);
                                    if (imagettd != null)
                                    {
                                        cell.AddElement(imagettd);
                                    }
                                    else
                                    {
                                        var img = "File tanda tangan tidak ditemukan";
                                        paragraph = new Paragraph(img, normal);
                                        cell.AddElement(paragraph);
                                    }

                                }
                                else if (letterDetail.letter.signatureType == 2)
                                {


                                    Image QRCodeSignatureApprover = GetBarcodeSignature("NIP : " + item.nip + "\nNAMA : " + item.fullname + "\nJabatan : " + item.positionName);
                                    QRCodeSignatureApprover.ScaleAbsoluteHeight(10f);
                                    cell.AddElement(QRCodeSignatureApprover);
                                }
                                else if (letterDetail.letter.signatureType == 3)
                                {
                                    var img = "";
                                    paragraph = new Paragraph(img, normal);
                                    cell.AddElement(paragraph);
                                    Paragraph linessia1 = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(2F, 100.0F, colorWhite, Element.ALIGN_CENTER, 1)));
                                    linessia1.SpacingBefore = 0f;
                                    linessia1.SetLeading(0.5F, 0.5F);
                                    pdfDoc.Add(linessia1);

                                    Paragraph linessiaa = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(2F, 100.0F, colorWhite, Element.ALIGN_CENTER, 1)));
                                    linessiaa.SpacingBefore = 0f;
                                    linessiaa.SetLeading(0.5F, 0.5F);
                                    pdfDoc.Add(linessiaa);

                                    Paragraph linessiaaa = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(2F, 100.0F, colorWhite, Element.ALIGN_CENTER, 1)));
                                    linessiaaa.SpacingBefore = 0f;
                                    linessiaaa.SetLeading(0.5F, 0.5F);
                                    pdfDoc.Add(linessiaaa);
                                }
                                else
                                {

                                    Image imagettd = GetSignatureImage(item.nip);
                                    cell.AddElement(imagettd);
                                    Image QRCodeSignatureApprover = GetBarcodeSignature("NIP : " + item.nip + "\nNAMA : " + item.fullname + "\nJabatan : " + item.positionName);
                                    cell.AddElement(QRCodeSignatureApprover);
                                    //if (imagettd != null)
                                    //{
                                    //    cell.AddElement(imagettd);
                                    //    Image QRCodeSignatureApprover = GetBarcodeSignature("NIP : " + item.nip + "\nNAMA : " + item.fullname + "\nJabatan : " + item.positionName);
                                    //    cell.AddElement(QRCodeSignatureApprover);
                                    //}
                                    //else
                                    //{
                                    //    var img = "File tanda tangan tidak di temukan";
                                    //    paragraph = new Paragraph(img, normal);
                                    //    cell.AddElement(paragraph);
                                    //    Image QRCodeSignatureApprover = GetBarcodeSignature("NIP : " + item.nip + "\nNAMA : " + item.fullname + "\nJabatan : " + item.positionName);
                                    //    cell.AddElement(QRCodeSignatureApprover);
                                    //}

                                }
                                no1++;
                            }

                            paragraph = new Paragraph(item.fullname, underlineBold);
                            cell.AddElement(paragraph);
                            paragraph = new Paragraph(item.positionName, bold);
                            cell.AddElement(paragraph);

                            table.AddCell(cell);
                        }
                        // end tanda tangan
                        pdfDoc.Add(table);

                        #endregion

                        pdfDoc.NewPage();

                        #region Lampiran

                        PdfPTable table1 = new PdfPTable(2);
                        float[] width1 = new float[] { 0.667f, 2f };
                        table1.WidthPercentage = 100;
                        table1.SetWidths(width1);
                        table1.HorizontalAlignment = Element.ALIGN_LEFT;
                        table1.DefaultCell.Border = 0;
                        table1.SpacingBefore = 0f;
                        table1.SpacingAfter = 0f;

                        PdfPCell cell1 = new PdfPCell();
                        Paragraph paragraph1 = new Paragraph();

                        cell1 = new PdfPCell();
                        cell1.BorderWidthLeft = 1f;
                        cell1.BorderWidthRight = 1f;
                        cell1.BorderWidthTop = 1f;
                        cell1.BorderWidthBottom = 1f;
                        cell1.MinimumHeight = 30f;
                        paragraph1 = new Paragraph("Nomor Registrasi Memo/Registration Number:" + letterDetail.letter.letterDeliberationNumber, normalDeliberation);
                        paragraph1.Alignment = Element.ALIGN_LEFT;
                        cell1.AddElement(paragraph1);
                        table1.AddCell(cell1);

                        cell1 = new PdfPCell();
                        cell1.BorderWidthLeft = 0f;
                        cell1.BorderWidthRight = 1f;
                        cell1.BorderWidthTop = 1f;
                        cell1.BorderWidthBottom = 1f;
                        cell1.MinimumHeight = 30f;
                        paragraph1 = new Paragraph("Tanggal Registrasi Memo/Date of Memo Registration : " + Convert.ToDateTime(letterDetail.letter.letterDate).ToString("dd MMMM yyyy"), normalDeliberation);
                        paragraph1.Alignment = Element.ALIGN_LEFT;
                        cell1.AddElement(paragraph1);
                        table1.AddCell(cell1);

                        cell1 = new PdfPCell();
                        cell1.BorderWidthLeft = 1f;
                        cell1.BorderWidthRight = 1f;
                        cell1.BorderWidthTop = 0f;
                        cell1.BorderWidthBottom = 1f;
                        cell1.MinimumHeight = 100f;
                        paragraph1 = new Paragraph("Nomor Memo : " + letterDetail.letter.letterNumber, normalDeliberation);
                        paragraph1.Alignment = Element.ALIGN_LEFT;
                        cell1.AddElement(paragraph1);
                        table1.AddCell(cell1);

                        cell1 = new PdfPCell();
                        cell1.BorderWidthLeft = 0f;
                        cell1.BorderWidthRight = 1f;
                        cell1.BorderWidthTop = 0f;
                        cell1.BorderWidthBottom = 1f;
                        cell1.MinimumHeight = 100f;
                        paragraph1 = new Paragraph("(Ringkasan Memo/Summary of the proposal) \n" + letterDetail.letterContent.summary, normalDeliberation);
                        paragraph1.Alignment = Element.ALIGN_LEFT;
                        cell1.AddElement(paragraph1);
                        table1.AddCell(cell1);

                        cell1 = new PdfPCell();
                        cell1.BorderWidthLeft = 1f;
                        cell1.BorderWidthRight = 1f;
                        cell1.BorderWidthTop = 0f;
                        cell1.BorderWidthBottom = 1f;
                        cell1.MinimumHeight = 100f;
                        paragraph1 = new Paragraph("Deliberation:", normalDeliberation);
                        paragraph1.Alignment = Element.ALIGN_LEFT;
                        cell1.AddElement(paragraph1);
                        no = 1;
                        foreach (var item in letterDetail.delibration)
                        {
                            if (letterDetail.receiver.Count() > 1)
                            {
                                paragraph1 = new Paragraph(no + ". " + item.fullname + " - " + item.positionName, normalDeliberation);
                                paragraph1.Alignment = Element.ALIGN_LEFT;
                                cell1.AddElement(paragraph1);
                            }
                            else
                            {
                                paragraph1 = new Paragraph(item.fullname + " - " + item.positionName, normalDeliberation);
                                paragraph1.Alignment = Element.ALIGN_LEFT;
                                cell1.AddElement(paragraph1);
                            }
                            no++;
                        }
                        table1.AddCell(cell1);

                        cell1 = new PdfPCell();
                        cell1.BorderWidthLeft = 0f;
                        cell1.BorderWidthRight = 1f;
                        cell1.BorderWidthTop = 0f;
                        cell1.BorderWidthBottom = 1f;
                        cell1.MinimumHeight = 100f;
                        paragraph1 = new Paragraph("Catatan / Notes : \n", underlineDeliberation);
                        paragraph1.Alignment = Element.ALIGN_LEFT;
                        cell1.AddElement(paragraph1);
                        no = 1;
                        foreach (var item in letterDetail.delibration)
                        {
                            if (letterDetail.receiver.Count() > 1)
                            {
                                paragraph1 = new Paragraph(no + ". " + item.commentdlbrt, normalDeliberation);
                                paragraph1.Alignment = Element.ALIGN_LEFT;
                                cell1.AddElement(paragraph1);
                            }
                            else
                            {
                                paragraph1 = new Paragraph(item.commentdlbrt, normalDeliberation);
                                paragraph1.Alignment = Element.ALIGN_LEFT;
                                cell1.AddElement(paragraph1);
                            }
                            no++;
                        }
                        table1.AddCell(cell1);

                        pdfDoc.Add(table1);

                        //var checkerList = letterDetail.checker.Where(p => p.ordernumber != 1).OrderBy(p => p.idLevelChecker).ToList();
                        var checkerList = letterDetail.checker.OrderBy(p => p.idLevelChecker).ToList();
                        foreach (var item in checkerList)
                        {
                            var getLogComment = letterDetail.log.Where(p => p.idUserLog == item.idUserChecker && p.description.Contains("pemeriksa ke")).OrderByDescending(p => p.createdOn).FirstOrDefault();
                            var comment = "";
                            string approveDate = "";
                            if (getLogComment != null)
                            {
                                comment = getLogComment.comment;
                                approveDate = Convert.ToDateTime(getLogComment.createdOn).ToString("dd MMMM yyyy");
                            }
                            table1 = new PdfPTable(3);
                            width = new float[] { 1f, 2f, 1f };
                            table1.WidthPercentage = 100;
                            table1.SetWidths(width);
                            table1.HorizontalAlignment = Element.ALIGN_LEFT;
                            table1.DefaultCell.Border = 0;
                            table1.SpacingBefore = 0f;
                            table1.SpacingAfter = 0f;

                            cell1 = new PdfPCell();
                            cell1.Rowspan = 2;
                            cell1.BorderWidthLeft = 1f;
                            cell1.BorderWidthRight = 1f;
                            cell1.BorderWidthTop = 0f;
                            cell1.BorderWidthBottom = 1f;
                            cell1.MinimumHeight = 120f;
                            if (letterDetail.letter.statusCode == 2)
                            {
                                paragraph1 = new Paragraph("Belum diputuskan / Undecided \n", normalDeliberation);
                            }
                            else if (letterDetail.letter.statusCode == 5)
                            {
                                paragraph1 = new Paragraph("Ditolak / Reject \n", normalDeliberation);
                            }
                            else
                            {
                                if (item.idLevelChecker == 2 ||item.idLevelChecker == 1)
                                {
                                    paragraph1 = new Paragraph("Disetujui / Approved \n", normalDeliberation);
                                }
                                else
                                {
                                    paragraph1 = new Paragraph("Mengetahui / Understand \n", normalDeliberation);

                                }
                            }

                            paragraph1.Alignment = Element.ALIGN_LEFT;
                            cell1.AddElement(paragraph1);
                            paragraph1 = new Paragraph(item.positionName, normalDeliberation);
                            paragraph1.Alignment = Element.ALIGN_LEFT;
                            cell1.AddElement(paragraph1);
                            table1.AddCell(cell1);

                            cell1 = new PdfPCell();
                            cell1.Rowspan = 2;
                            cell1.BorderWidthLeft = 0f;
                            cell1.BorderWidthRight = 1f;
                            cell1.BorderWidthTop = 0f;
                            cell1.BorderWidthBottom = 1f;
                            cell1.MinimumHeight = 120f;
                            paragraph1 = new Paragraph("Catatan / Notes : \n", underlineDeliberation);
                            paragraph1.Alignment = Element.ALIGN_LEFT;
                            cell1.AddElement(paragraph1);
                            paragraph1 = new Paragraph(comment, normalDeliberation);
                            paragraph1.Alignment = Element.ALIGN_LEFT;
                            cell1.AddElement(paragraph1);
                            table1.AddCell(cell1);

                            cell1 = new PdfPCell();
                            cell1.BorderWidthLeft = 0f;
                            cell1.BorderWidthRight = 1f;
                            cell1.BorderWidthTop = 0f;
                            cell1.BorderWidthBottom = 1f;
                            cell1.MinimumHeight = 40f;
                            paragraph1 = new Paragraph("Tanggal : ", underlineDeliberation);
                            paragraph1.Alignment = Element.ALIGN_LEFT;
                            //cell1.AddElement(paragraph);
                            paragraph1 = new Paragraph(approveDate, normalDeliberation);
                            paragraph1.Alignment = Element.ALIGN_LEFT;
                            cell1.AddElement(paragraph1);
                            table1.AddCell(cell1);

                            cell1 = new PdfPCell();
                            cell1.BorderWidthLeft = 0f;
                            cell1.BorderWidthRight = 1f;
                            cell1.BorderWidthTop = 0f;
                            cell1.BorderWidthBottom = 1f;
                            paragraph1 = new Paragraph("Tanda Tangan : \n", underlineDeliberation);
                            paragraph1.Alignment = Element.ALIGN_LEFT;
                            cell1.AddElement(paragraph1);
                            if (letterDetail.letter.letterNumber != "NO_LETTER")
                            {
                                if (letterDetail.letter.signatureType == 1)
                                {
                                    Image imagettd = GetSignatureImage(item.nip);
                                    //cell.AddElement(imagettd);
                                    if (imagettd != null)
                                    {
                                        
                                        cell1.AddElement(imagettd);
                                    }
                                    else
                                    {
                                        var img = "File tanda tangan tidak ditemukan";
                                        paragraph1 = new Paragraph(img, normal);
                                        cell1.AddElement(paragraph1);
                                    }

                                    //string[] signatures = new string[] { item.idUserChecker.ToString() };
                                    //int count = 1;
                                    //foreach (var signature in signatures)
                                    //{

                                    //    Image imagettdd = GetSignatureImage(item.nip);
                                    //    //var pixelData = barcodeWriter.Write(signature);
                                    //    //Image img = Image.GetInstance(pixelData.Pixels, pixelData.Width, pixelData.Height);
                                    //    pdfDoc.Add(imagettdd);
                                    //    if (count == 1)
                                    //    {
                                    //        pdfDoc.Add(new Paragraph("\n"));
                                    //    }
                                    //    count++;
                                    //}
                                }
                                else if (letterDetail.letter.signatureType == 2)
                                {

                                    Image QRCodeSignatureApprover = GetBarcodeSignature("NIP : " + item.nip + "\nNAMA : " + item.fullname + "\nJabatan : " + item.positionName);
                                    cell1.AddElement(QRCodeSignatureApprover);
                                }
                                else if (letterDetail.letter.signatureType == 3)
                                {
                                    var img = "";
                                    paragraph = new Paragraph(img, normal);
                                    cell.AddElement(paragraph);
                                    Paragraph linessia = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(2F, 100.0F, colorWhite, Element.ALIGN_CENTER, 1)));
                                    linessia.SpacingBefore = 0f;
                                    linessia.SetLeading(0.5F, 0.5F);
                                    pdfDoc.Add(linessia);

                                    Paragraph linessiaa = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(2F, 100.0F, colorWhite, Element.ALIGN_CENTER, 1)));
                                    linessiaa.SpacingBefore = 0f;
                                    linessiaa.SetLeading(0.5F, 0.5F);
                                    pdfDoc.Add(linessiaa);

                                    Paragraph linessiaaa = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(2F, 100.0F, colorWhite, Element.ALIGN_CENTER, 1)));
                                    linessiaaa.SpacingBefore = 0f;
                                    linessiaaa.SetLeading(0.5F, 0.5F);
                                    pdfDoc.Add(linessiaaa);
                                }
                                else
                                {
                                    //innertable1 = new PdfPtable1(2);
                                    //float[] widthinner = new float[] { 0.3f, 0.3f };
                                    //innertable1.WidthPercentage = 100;
                                    //innertable1.SetWidths(widthinner);
                                    //innertable1.HorizontalAlignment = Element.ALIGN_LEFT;
                                    //innertable1.DefaultCell.Border = 1;
                                    //innertable1.DefaultCell.PaddingBottom = 8;

                                    //innercell = new PdfPCell();
                                    //innercell.BorderWidthLeft = 1f;
                                    //innercell.BorderWidthRight = 1f;
                                    //innercell.BorderWidthTop = 1f;
                                    //innercell.BorderWidthBottom = 1f;




                                    Image imagettd = GetSignatureImage(item.nip);
                                    cell1.AddElement(imagettd);

                                    Image QRCodeSignatureApprover = GetBarcodeSignature("NIP : " + item.nip + "\nNAMA : " + item.fullname + "\nJabatan : " + item.positionName);
                                    cell1.AddElement(QRCodeSignatureApprover);
                                    //if (imagettd !=null)
                                    //{
                                    //    cell.AddElement(imagettd);

                                    //    Image QRCodeSignatureApprover = GetBarcodeSignature("NIP : " + item.nip + "\nNAMA : " + item.fullname + "\nJabatan : " + item.positionName);
                                    //    cell.AddElement(QRCodeSignatureApprover);
                                    //}
                                    //else
                                    //{
                                    //    var img = "File tanda tangan tidak di temukan";
                                    //    paragraph = new Paragraph(img, normal);

                                    //    cell.AddElement(paragraph);
                                    //    Image QRCodeSignatureApprover = GetBarcodeSignature("NIP : " + item.nip + "\nNAMA : " + item.fullname + "\nJabatan : " + item.positionName);
                                    //    cell.AddElement(QRCodeSignatureApprover);
                                    //}

                                }
                            }

                            paragraph1 = new Paragraph(item.fullname, bold);
                            paragraph1.Alignment = Element.ALIGN_CENTER;
                            cell1.AddElement(paragraph1);
                            table1.AddCell(cell1);


                            pdfDoc.Add(table1);
                        }


                        #endregion

                        pdfDoc.Close();

                        var dateLetterFormat = Convert.ToDateTime(letterDetail.letter.letterDate).ToString("dd MMMM yyyy");
                        var letterNumber = letterDetail.letter.letterNumber;
                        byte[] bytess = ms.ToArray();
                        byte[] bytes = AddPageNumbers(bytess, dateLetterFormat, letterNumber, "Memo");
                        ms.Close();

                        return File(bytes.ToArray(), "application/pdf", fileName);
                }

                #endregion
            }

            // Done (Masi Ikuti Template Memo Direksi)
            private FileContentResult PrivSuratEdaranDireksi(string letternumber, OutputDetailMemo letterDetail, string fileName)
            {
                #region Surat Edaran Direksi

                StringBuilder sb = new StringBuilder();
                sb.Append("<div style='padding-left:2px;'>" + letterDetail.letterContent.letterContent + "</div>");
                StringReader sr = new StringReader(sb.ToString());
                //Document pdfDoc = new Document(PageSize.A4, 25, 25, 25, 15);
                //Document pdfDoc = new Document(PageSize.A4, 56, 56, 113, 97);
                Document pdfDoc = new Document(PageSize.A4, 56.7f, 56.7f, 66.5f, 99.225f);
                HTMLWorker htmlparser = new HTMLWorker(pdfDoc);


                    using (MemoryStream ms = new MemoryStream())
                    {
                        PdfWriter pdfWriter = PdfWriter.GetInstance(pdfDoc, ms);
                        pdfDoc.Open();
                        //Font
                        int no;
                        var fontName = "Calibri";
                        string fontPath = Path.Combine(_environment.WebRootPath, "fonts/Calibri.ttf");
                        //FontFactory.Register(fontPath);

                        Font normal = FontFactory.GetFont(fontName, 12, Font.NORMAL, BaseColor.BLACK);
                        Font bold = FontFactory.GetFont(fontName, 12, Font.BOLD, BaseColor.BLACK);
                        Font underline = FontFactory.GetFont(fontName, 12, Font.UNDERLINE, BaseColor.BLACK);
                        Font underlineBold = FontFactory.GetFont(fontName, 12, Font.BOLD | Font.UNDERLINE, BaseColor.BLACK);

                        Font normalDeliberation = FontFactory.GetFont(fontName, 10, Font.NORMAL, BaseColor.BLACK);
                        Font underlineDeliberation = FontFactory.GetFont(fontName, 10, Font.UNDERLINE, BaseColor.BLACK);
                        Font underlineDeliberationBold = FontFactory.GetFont(fontName, 10, Font.BOLD | Font.UNDERLINE, BaseColor.BLACK);
                        // Colors
                        BaseColor colorBlack = new BaseColor(System.Drawing.ColorTranslator.FromHtml("#000000"));
                        BaseColor colorWhite = new BaseColor(System.Drawing.ColorTranslator.FromHtml("#FFFFFF"));

                

                        PdfPTable table = new PdfPTable(2);
                        float[] width = new float[] { 0.667f, 2f };
                        table.WidthPercentage = 100;
                        table.SetWidths(width);
                        table.HorizontalAlignment = Element.ALIGN_LEFT;
                        table.DefaultCell.Border = 0;
                        table.SpacingBefore = 0f;
                        table.SpacingAfter = 0f;

                        PdfPCell cell = new PdfPCell();
                        Paragraph paragraph = new Paragraph();


                        #region header
                        //add new page

                        //table = new PdfPTable(1);
                        //width = new float[] { 2f };
                        //table.WidthPercentage = 100;
                        //table.SetWidths(width);
                        //table.HorizontalAlignment = Element.ALIGN_LEFT;
                        //table.DefaultCell.Border = 0;
                        //table.SpacingBefore = 00f;
                        //table.SpacingAfter = 0f;


                        //table.AddCell(new Phrase("MEMO", bold));
                        //pdfDoc.Add(table);

                            table = new PdfPTable(2);
                            width = new float[] { 0.2f, 1f };
                            table.WidthPercentage = 100;
                            table.SetWidths(width);
                            table.HorizontalAlignment = Element.ALIGN_RIGHT;
                            table.DefaultCell.Border = 0;
                            table.SpacingBefore = 0f;
                            table.SpacingAfter = 0f;

                            cell.Border = 0;
                            paragraph = new Paragraph("Nomor", normal);
                            paragraph.Alignment = Element.ALIGN_LEFT;
                            cell.AddElement(paragraph);
                            table.AddCell(cell);

                            //cell = new PdfPCell();
                            //cell.Border = 0;
                            //paragraph = new Paragraph(":", normal);
                            //paragraph.Alignment = Element.ALIGN_RIGHT;
                            //cell.AddElement(paragraph);
                            //table.AddCell(cell);

                            cell = new PdfPCell();
                            cell.Border = 0;
                            paragraph = new Paragraph(":"+" "+letterDetail.letter.letterNumber, normal);
                            paragraph.Alignment = Element.ALIGN_LEFT;
                            cell.AddElement(paragraph);
                            table.AddCell(cell);

                            cell = new PdfPCell();
                            cell.Border = 0;
                            paragraph = new Paragraph("Tanggal", normal);
                            paragraph.Alignment = Element.ALIGN_LEFT;
                            cell.AddElement(paragraph);
                            table.AddCell(cell);

                            //cell = new PdfPCell();
                            //cell.Border = 0;
                            //paragraph = new Paragraph(":", normal);
                            //paragraph.Alignment = Element.ALIGN_RIGHT;
                            //cell.AddElement(paragraph);
                            //table.AddCell(cell);

                            cell = new PdfPCell();
                            cell.Border = 0;
                            paragraph = new Paragraph(":"+" "+Convert.ToDateTime(letterDetail.letter.letterDate).ToString("dd MMMM yyyy"), normal);
                            paragraph.Alignment = Element.ALIGN_LEFT;
                            cell.AddElement(paragraph);
                            table.AddCell(cell);

                            cell = new PdfPCell();
                            cell.Border = 0;
                            paragraph = new Paragraph("Kepada", normal);
                            paragraph.Alignment = Element.ALIGN_LEFT;
                            cell.AddElement(paragraph);
                            table.AddCell(cell);

                            //cell = new PdfPCell();
                            //cell.Border = 0;
                            //paragraph = new Paragraph(":", normal);
                            //paragraph.Alignment = Element.ALIGN_RIGHT;
                            //cell.AddElement(paragraph);
                            //table.AddCell(cell);

                            cell = new PdfPCell();
                            cell.Border = 0;
                            no = 1;
                            foreach (var item in letterDetail.receiver)
                            {
                                if (letterDetail.receiver.Count() > 1)
                                {
                                    paragraph = new Paragraph(":"+" "+no + ". " + item.fullname + " - " + item.positionName, normal);
                                    paragraph.Alignment = Element.ALIGN_LEFT;
                                    cell.AddElement(paragraph);
                                }
                                else
                                {
                                    paragraph = new Paragraph(":"+" "+item.fullname + " - " + item.positionName, normal);
                                    paragraph.Alignment = Element.ALIGN_LEFT;
                                    cell.AddElement(paragraph);
                                }
                                no++;
                            }
                            table.AddCell(cell);

                            cell = new PdfPCell();
                            cell.Border = 0;
                            paragraph = new Paragraph("Tembusan", normal);
                            paragraph.Alignment = Element.ALIGN_LEFT;
                            cell.AddElement(paragraph);
                            table.AddCell(cell);

                            //cell = new PdfPCell();
                            //cell.Border = 0;
                            //paragraph = new Paragraph(":", normal);
                            //paragraph.Alignment = Element.ALIGN_RIGHT;
                            //cell.AddElement(paragraph);
                            //table.AddCell(cell);

                            cell = new PdfPCell();
                            cell.Border = 0;
                            no = 1;
                            foreach (var item in letterDetail.copy)
                            {
                                if (letterDetail.copy.Count() > 1)
                                {
                                    paragraph = new Paragraph(":"+" "+no + ". " + item.fullname + " - " + item.positionName, normal);
                                    paragraph.Alignment = Element.ALIGN_LEFT;
                                    cell.AddElement(paragraph);
                                }
                                else
                                {
                                    paragraph = new Paragraph(":"+" "+item.fullname + " - " + item.positionName, normal);
                                    paragraph.Alignment = Element.ALIGN_LEFT;
                                    cell.AddElement(paragraph);
                                }
                                no++;
                            }
                            if (letterDetail.copy.Count() == 0)
                            {
                                paragraph = new Paragraph("-", normal);
                                paragraph.Alignment = Element.ALIGN_LEFT;
                                cell.AddElement(paragraph);
                            }
                            table.AddCell(cell);

                            cell = new PdfPCell();
                            cell.Border = 0;
                            paragraph = new Paragraph("Dari", normal);
                            paragraph.Alignment = Element.ALIGN_LEFT;
                            cell.AddElement(paragraph);
                            table.AddCell(cell);

                            //cell = new PdfPCell();
                            //cell.Border = 0;
                            //paragraph = new Paragraph(":", normal);
                            //paragraph.Alignment = Element.ALIGN_RIGHT;
                            //cell.AddElement(paragraph);
                            //table.AddCell(cell);

                            cell = new PdfPCell();
                            cell.Border = 0;
                            paragraph = new Paragraph(":"+" "+letterDetail.sender[0].fullname + " - " + letterDetail.sender[0].positionName, normal);
                            paragraph.Alignment = Element.ALIGN_LEFT;
                            cell.AddElement(paragraph);
                            table.AddCell(cell);

                            cell = new PdfPCell();
                            cell.Border = 0;
                            paragraph = new Paragraph("Perihal", normal);
                            paragraph.Alignment = Element.ALIGN_LEFT;
                            cell.AddElement(paragraph);
                            table.AddCell(cell);

                            //cell = new PdfPCell();
                            //cell.Border = 0;
                            //paragraph = new Paragraph(":", normal);
                            //paragraph.Alignment = Element.ALIGN_RIGHT;
                            //cell.AddElement(paragraph);
                            //table.AddCell(cell);

                            cell = new PdfPCell();
                            cell.Border = 0;
                            paragraph = new Paragraph(":"+" "+letterDetail.letter.about, normal);
                            paragraph.Alignment = Element.ALIGN_LEFT;
                            cell.AddElement(paragraph);
                            table.AddCell(cell);

                            cell = new PdfPCell();
                            cell.Border = 0;
                            paragraph = new Paragraph("Lampiran", normal);
                            paragraph.Alignment = Element.ALIGN_LEFT;
                            cell.AddElement(paragraph);
                            table.AddCell(cell);

                            //cell = new PdfPCell();
                            //cell.Border = 0;
                            //paragraph = new Paragraph(":", normal);
                            //paragraph.Alignment = Element.ALIGN_RIGHT;
                            //cell.AddElement(paragraph);
                            //table.AddCell(cell);

                            var lampiran = letterDetail.letter.attachmentDesc == null ? "-" : letterDetail.letter.attachmentDesc;
                            cell = new PdfPCell();
                            cell.Border = 0;
                            paragraph = new Paragraph(":"+" "+lampiran, bold);
                            paragraph.Alignment = Element.ALIGN_LEFT;
                            cell.AddElement(paragraph);
                            table.AddCell(cell);

                            //Add table to document
                            pdfDoc.Add(table);
              

                        Paragraph line = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(2F, 100.0F, colorBlack, Element.ALIGN_CENTER, 1)));
                        line.SpacingBefore = 0f;
                        line.SetLeading(2F, 0.5F);
                        pdfDoc.Add(line);
                        Paragraph lines = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(2F, 100.0F, colorBlack, Element.ALIGN_CENTER, 1)));
                        lines.SpacingBefore = 0f;
                        line.SetLeading(0.5F, 0.5F);
                        pdfDoc.Add(line);

                        Paragraph liness = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(2F, 100.0F, colorWhite, Element.ALIGN_CENTER, 1)));
                        liness.SpacingBefore = 0f;
                        liness.SetLeading(0.5F, 0.5F);
                        pdfDoc.Add(liness);

                        //Paragraph linesss = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(2F, 100.0F, colorWhite, Element.ALIGN_CENTER, 1)));
                        //linesss.SpacingBefore = 0f;
                        //linesss.SetLeading(0.5F, 0.5F);
                        //pdfDoc.Add(linesss);


                        table = new PdfPTable(1);
                        width = new float[] { 2f };
                        table.WidthPercentage = 100;
                        table.SetWidths(width);
                        table.HorizontalAlignment = Element.ALIGN_LEFT;
                        table.DefaultCell.Border = 0;
                        table.SpacingBefore = 00f;
                        table.SpacingAfter = 0f;


                        //table.AddCell(new Phrase("Dengan hormat", normal));
                        //pdfDoc.Add(table);

                        Paragraph linessss = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(2F, 100.0F, colorWhite, Element.ALIGN_CENTER, 1)));
                        linessss.SpacingBefore = 0f;
                        linessss.SetLeading(0.5F, 0.5F);
                        pdfDoc.Add(linessss);

                        Paragraph lin = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(2F, 100.0F, colorWhite, Element.ALIGN_CENTER, 1)));
                        lin.SpacingBefore = 0f;
                        lin.SetLeading(0.5F, 0.5F);
                        pdfDoc.Add(lin);

                        #endregion

                        //ADD HTML CONTENT
                        htmlparser.Parse(sr);


                        #region Tanda Tangan

                        Paragraph a = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(2F, 100.0F, colorWhite, Element.ALIGN_CENTER, 1)));
                        a.SpacingBefore = 0f;
                        a.SetLeading(0.5F, 0.5F);
                        pdfDoc.Add(a);



                        cell = new PdfPCell();
                        table.AddCell(new Phrase("", normal));

                        cell = new PdfPCell();
                        table.AddCell(new Phrase(" ", normal));
                        cell = new PdfPCell();
                        table.AddCell(new Phrase(" ", normal));
                        cell = new PdfPCell();
                        table.AddCell(new Phrase("Hormat kami, \n" + "PT BNI Life Insurance", normal));
                        //table.AddCell(new Phrase("Best Regards", normal));
                        pdfDoc.Add(table);



                        string filename = letterDetail.sender[0].nip;


                        // Tanda tangan
                        var dirChecker = letterDetail.sender.OrderBy(p => p.idUserSender).ToList();
                        //var dirChecker = letterDetail.checker.Where(p => p.idLevelChecker == 2).ToList();


                        table = new PdfPTable(dirChecker.Count());
                        width = new float[dirChecker.Count()];
                        for (int i = 0; i < dirChecker.Count(); i++)
                        {
                            width[i] = 1f;
                        }
                        table.WidthPercentage = 100;
                        table.SetWidths(width);
                        table.HorizontalAlignment = Element.ALIGN_LEFT;
                        table.DefaultCell.Border = 0;
                        var no1 = 0;


                        //cell = new PdfPCell();
                        //table.AddCell(new Phrase("Direksi PT BNI Life Insurance", normal));
                        foreach (var item in dirChecker)
                        {
                            cell = new PdfPCell();
                            cell.Border = 0;

                            if (letterDetail.letter.letterNumber != "NO_LETTER")
                            {
                                if (letterDetail.letter.signatureType == 1)
                                {

                                    Image imagettd = GetSignatureImage(item.nip);
                                    //cell.AddElement(imagettd);
                                    if (imagettd != null)
                                    {
                                        cell.AddElement(imagettd);
                                    }
                                    else
                                    {
                                        var img = "File tanda tangan tidak ditemukan";
                                        paragraph = new Paragraph(img, normal);
                                        cell.AddElement(paragraph);
                                    }

                                }
                                else if (letterDetail.letter.signatureType == 2)
                                {


                                    Image QRCodeSignatureApprover = GetBarcodeSignature("NIP : " + item.nip + "\nNAMA : " + item.fullname + "\nJabatan : " + item.positionName);
                                    QRCodeSignatureApprover.ScaleAbsoluteHeight(10f);
                                    cell.AddElement(QRCodeSignatureApprover);
                                }
                                else if (letterDetail.letter.signatureType == 3)
                                {
                                    var img = "";
                                    paragraph = new Paragraph(img, normal);
                                    cell.AddElement(paragraph);
                                    Paragraph linessia = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(2F, 100.0F, colorWhite, Element.ALIGN_CENTER, 1)));
                                    liness.SpacingBefore = 0f;
                                    liness.SetLeading(0.5F, 0.5F);
                                    pdfDoc.Add(linessia);

                                    Paragraph linessiaa = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(2F, 100.0F, colorWhite, Element.ALIGN_CENTER, 1)));
                                    linessiaa.SpacingBefore = 0f;
                                    linessiaa.SetLeading(0.5F, 0.5F);
                                    pdfDoc.Add(linessiaa);

                                    Paragraph linessiaaa = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(2F, 100.0F, colorWhite, Element.ALIGN_CENTER, 1)));
                                    linessiaaa.SpacingBefore = 0f;
                                    linessiaaa.SetLeading(0.5F, 0.5F);
                                    pdfDoc.Add(linessiaaa);
                                }
                                else
                                {

                                    Image imagettd = GetSignatureImage(item.nip);
                                    cell.AddElement(imagettd);
                                    Image QRCodeSignatureApprover = GetBarcodeSignature("NIP : " + item.nip + "\nNAMA : " + item.fullname + "\nJabatan : " + item.positionName);
                                    cell.AddElement(QRCodeSignatureApprover);
                                    //if (imagettd != null)
                                    //{
                                    //    cell.AddElement(imagettd);
                                    //    Image QRCodeSignatureApprover = GetBarcodeSignature("NIP : " + item.nip + "\nNAMA : " + item.fullname + "\nJabatan : " + item.positionName);
                                    //    cell.AddElement(QRCodeSignatureApprover);
                                    //}
                                    //else
                                    //{
                                    //    var img = "File tanda tangan tidak di temukan";
                                    //    paragraph = new Paragraph(img, normal);
                                    //    cell.AddElement(paragraph);
                                    //    Image QRCodeSignatureApprover = GetBarcodeSignature("NIP : " + item.nip + "\nNAMA : " + item.fullname + "\nJabatan : " + item.positionName);
                                    //    cell.AddElement(QRCodeSignatureApprover);
                                    //}

                                }
                                no1++;
                            }

                            paragraph = new Paragraph(item.fullname, underlineBold);
                            cell.AddElement(paragraph);
                            paragraph = new Paragraph(item.positionName, bold);
                            cell.AddElement(paragraph);

                            table.AddCell(cell);
                        }
                        // end tanda tangan
                        pdfDoc.Add(table);

                        #endregion

                        pdfDoc.NewPage();

                        #region Lampiran

                        PdfPTable table1 = new PdfPTable(2);
                        float[] width1 = new float[] { 0.667f, 2f };
                        table1.WidthPercentage = 100;
                        table1.SetWidths(width1);
                        table1.HorizontalAlignment = Element.ALIGN_LEFT;
                        table1.DefaultCell.Border = 0;
                        table1.SpacingBefore = 0f;
                        table1.SpacingAfter = 0f;

                        PdfPCell cell1 = new PdfPCell();
                        Paragraph paragraph1 = new Paragraph();

                        cell1 = new PdfPCell();
                        cell1.BorderWidthLeft = 1f;
                        cell1.BorderWidthRight = 1f;
                        cell1.BorderWidthTop = 1f;
                        cell1.BorderWidthBottom = 1f;
                        cell1.MinimumHeight = 30f;
                        paragraph1 = new Paragraph("Nomor Registrasi Memo/Registration Number:" + letterDetail.letter.letterDeliberationNumber, normalDeliberation);
                        paragraph1.Alignment = Element.ALIGN_LEFT;
                        cell1.AddElement(paragraph1);
                        table1.AddCell(cell1);

                        cell1 = new PdfPCell();
                        cell1.BorderWidthLeft = 0f;
                        cell1.BorderWidthRight = 1f;
                        cell1.BorderWidthTop = 1f;
                        cell1.BorderWidthBottom = 1f;
                        cell1.MinimumHeight = 30f;
                        paragraph1 = new Paragraph("Tanggal Registrasi Memo/Date of Memo Registration : " + Convert.ToDateTime(letterDetail.letter.letterDate).ToString("dd MMMM yyyy"), normalDeliberation);
                        paragraph1.Alignment = Element.ALIGN_LEFT;
                        cell1.AddElement(paragraph1);
                        table1.AddCell(cell1);

                        cell1 = new PdfPCell();
                        cell1.BorderWidthLeft = 1f;
                        cell1.BorderWidthRight = 1f;
                        cell1.BorderWidthTop = 0f;
                        cell1.BorderWidthBottom = 1f;
                        cell1.MinimumHeight = 100f;
                        paragraph1 = new Paragraph("Nomor Memo : " + letterDetail.letter.letterNumber, normalDeliberation);
                        paragraph1.Alignment = Element.ALIGN_LEFT;
                        cell1.AddElement(paragraph1);
                        table1.AddCell(cell1);

                        cell1 = new PdfPCell();
                        cell1.BorderWidthLeft = 0f;
                        cell1.BorderWidthRight = 1f;
                        cell1.BorderWidthTop = 0f;
                        cell1.BorderWidthBottom = 1f;
                        cell1.MinimumHeight = 100f;
                        paragraph1 = new Paragraph("(Ringkasan Memo/Summary of the proposal) \n" + letterDetail.letterContent.summary, normalDeliberation);
                        paragraph1.Alignment = Element.ALIGN_LEFT;
                        cell1.AddElement(paragraph1);
                        table1.AddCell(cell1);

                        cell1 = new PdfPCell();
                        cell1.BorderWidthLeft = 1f;
                        cell1.BorderWidthRight = 1f;
                        cell1.BorderWidthTop = 0f;
                        cell1.BorderWidthBottom = 1f;
                        cell1.MinimumHeight = 100f;
                        paragraph1 = new Paragraph("Deliberation:", normalDeliberation);
                        paragraph1.Alignment = Element.ALIGN_LEFT;
                        cell1.AddElement(paragraph1);
                        no = 1;
                        foreach (var item in letterDetail.delibration)
                        {
                            if (letterDetail.receiver.Count() > 1)
                            {
                                paragraph1 = new Paragraph(no + ". " + item.fullname + " - " + item.positionName, normalDeliberation);
                                paragraph1.Alignment = Element.ALIGN_LEFT;
                                cell1.AddElement(paragraph1);
                            }
                            else
                            {
                                paragraph1 = new Paragraph(item.fullname + " - " + item.positionName, normalDeliberation);
                                paragraph1.Alignment = Element.ALIGN_LEFT;
                                cell1.AddElement(paragraph1);
                            }
                            no++;
                        }
                        table1.AddCell(cell1);

                        cell1 = new PdfPCell();
                        cell1.BorderWidthLeft = 0f;
                        cell1.BorderWidthRight = 1f;
                        cell1.BorderWidthTop = 0f;
                        cell1.BorderWidthBottom = 1f;
                        cell1.MinimumHeight = 100f;
                        paragraph1 = new Paragraph("Catatan / Notes : \n", underlineDeliberation);
                        paragraph1.Alignment = Element.ALIGN_LEFT;
                        cell1.AddElement(paragraph1);
                        no = 1;
                        foreach (var item in letterDetail.delibration)
                        {
                            if (letterDetail.receiver.Count() > 1)
                            {
                                paragraph1 = new Paragraph(no + ". " + item.commentdlbrt, normalDeliberation);
                                paragraph1.Alignment = Element.ALIGN_LEFT;
                                cell1.AddElement(paragraph1);
                            }
                            else
                            {
                                paragraph1 = new Paragraph(item.commentdlbrt, normalDeliberation);
                                paragraph1.Alignment = Element.ALIGN_LEFT;
                                cell1.AddElement(paragraph1);
                            }
                            no++;
                        }
                        table1.AddCell(cell1);

                        pdfDoc.Add(table1);

                        //var checkerList = letterDetail.checker.Where(p => p.ordernumber != 1).OrderBy(p => p.idLevelChecker).ToList();
                        var checkerList = letterDetail.checker.OrderBy(p => p.idLevelChecker).ToList();
                        foreach (var item in checkerList)
                        {
                            var getLogComment = letterDetail.log.Where(p => p.idUserLog == item.idUserChecker && p.description.Contains("pemeriksa ke")).OrderByDescending(p => p.createdOn).FirstOrDefault();
                            var comment = "";
                            string approveDate = "";
                            if (getLogComment != null)
                            {
                                comment = getLogComment.comment;
                                approveDate = Convert.ToDateTime(getLogComment.createdOn).ToString("dd MMMM yyyy");
                            }
                            table1 = new PdfPTable(3);
                            width = new float[] { 1f, 2f, 1f };
                            table1.WidthPercentage = 100;
                            table1.SetWidths(width);
                            table1.HorizontalAlignment = Element.ALIGN_LEFT;
                            table1.DefaultCell.Border = 0;
                            table1.SpacingBefore = 0f;
                            table1.SpacingAfter = 0f;

                            cell1 = new PdfPCell();
                            cell1.Rowspan = 2;
                            cell1.BorderWidthLeft = 1f;
                            cell1.BorderWidthRight = 1f;
                            cell1.BorderWidthTop = 0f;
                            cell1.BorderWidthBottom = 1f;
                            cell1.MinimumHeight = 120f;
                            if (letterDetail.letter.statusCode == 2)
                                {
                                    paragraph1 = new Paragraph("Belum diputuskan / Undecided \n", normalDeliberation);
                                }
                                else if (letterDetail.letter.statusCode == 5)
                                {
                                    paragraph1 = new Paragraph("Ditolak / Reject \n", normalDeliberation);
                                }
                                else
                                {
                                   if(item.idLevelChecker == 2 ||item.idLevelChecker == 1)
                                   { 
                                       paragraph1 = new Paragraph("Disetujui / Approved \n", normalDeliberation);
                                   }
                                   else
                                   {
                                       paragraph1 = new Paragraph("Mengetahui / Understand \n", normalDeliberation);
                                       
                                   }
                                }

                            paragraph1.Alignment = Element.ALIGN_LEFT;
                            cell1.AddElement(paragraph1);
                            paragraph1 = new Paragraph(item.positionName, normalDeliberation);
                            paragraph1.Alignment = Element.ALIGN_LEFT;
                            cell1.AddElement(paragraph1);
                            table1.AddCell(cell1);

                            cell1 = new PdfPCell();
                            cell1.Rowspan = 2;
                            cell1.BorderWidthLeft = 0f;
                            cell1.BorderWidthRight = 1f;
                            cell1.BorderWidthTop = 0f;
                            cell1.BorderWidthBottom = 1f;
                            cell1.MinimumHeight = 120f;
                            paragraph1 = new Paragraph("Catatan / Notes : \n", underlineDeliberation);
                            paragraph1.Alignment = Element.ALIGN_LEFT;
                            cell1.AddElement(paragraph1);
                            paragraph1 = new Paragraph(comment, normalDeliberation);
                            paragraph1.Alignment = Element.ALIGN_LEFT;
                            cell1.AddElement(paragraph1);
                            table1.AddCell(cell1);

                            cell1 = new PdfPCell();
                            cell1.BorderWidthLeft = 0f;
                            cell1.BorderWidthRight = 1f;
                            cell1.BorderWidthTop = 0f;
                            cell1.BorderWidthBottom = 1f;
                            cell1.MinimumHeight = 40f;
                            paragraph1 = new Paragraph("Tanggal : ", underlineDeliberation);
                            paragraph1.Alignment = Element.ALIGN_LEFT;
                            //cell1.AddElement(paragraph);
                            paragraph1 = new Paragraph(approveDate, normalDeliberation);
                            paragraph1.Alignment = Element.ALIGN_LEFT;
                            cell1.AddElement(paragraph1);
                            table1.AddCell(cell1);

                            cell1 = new PdfPCell();
                            cell1.BorderWidthLeft = 0f;
                            cell1.BorderWidthRight = 1f;
                            cell1.BorderWidthTop = 0f;
                            cell1.BorderWidthBottom = 1f;
                            paragraph1 = new Paragraph("Tanda Tangan : \n", underlineDeliberation);
                            paragraph1.Alignment = Element.ALIGN_LEFT;
                            cell1.AddElement(paragraph1);
                            if (letterDetail.letter.letterNumber != "NO_LETTER")
                            {
                                if (letterDetail.letter.signatureType == 1)
                                {
                                    Image imagettd = GetSignatureImage(item.nip);
                                    //cell.AddElement(imagettd);
                                    if (imagettd != null)
                                    {
                                        cell1.AddElement(imagettd);
                                    }
                                    else
                                    {
                                        var img = "File tanda tangan tidak ditemukan";
                                        paragraph1 = new Paragraph(img, normal);
                                        cell1.AddElement(paragraph1);
                                    }
                                }
                                else if (letterDetail.letter.signatureType == 2)
                                {

                                    Image QRCodeSignatureApprover = GetBarcodeSignature("NIP : " + item.nip + "\nNAMA : " + item.fullname + "\nJabatan : " + item.positionName);
                                    cell1.AddElement(QRCodeSignatureApprover);
                                }
                                else if (letterDetail.letter.signatureType == 3)
                                {
                                    var img = "";
                                    paragraph = new Paragraph(img, normal);
                                    cell.AddElement(paragraph);
                                    Paragraph linessia = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(2F, 100.0F, colorWhite, Element.ALIGN_CENTER, 1)));
                                    liness.SpacingBefore = 0f;
                                    liness.SetLeading(0.5F, 0.5F);
                                    pdfDoc.Add(linessia);

                                    Paragraph linessiaa = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(2F, 100.0F, colorWhite, Element.ALIGN_CENTER, 1)));
                                    linessiaa.SpacingBefore = 0f;
                                    linessiaa.SetLeading(0.5F, 0.5F);
                                    pdfDoc.Add(linessiaa);

                                    Paragraph linessiaaa = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(2F, 100.0F, colorWhite, Element.ALIGN_CENTER, 1)));
                                    linessiaaa.SpacingBefore = 0f;
                                    linessiaaa.SetLeading(0.5F, 0.5F);
                                    pdfDoc.Add(linessiaaa);
                                }
                                else
                                {
                                    //innertable1 = new PdfPtable1(2);
                                    //float[] widthinner = new float[] { 0.3f, 0.3f };
                                    //innertable1.WidthPercentage = 100;
                                    //innertable1.SetWidths(widthinner);
                                    //innertable1.HorizontalAlignment = Element.ALIGN_LEFT;
                                    //innertable1.DefaultCell.Border = 1;
                                    //innertable1.DefaultCell.PaddingBottom = 8;

                                    //innercell = new PdfPCell();
                                    //innercell.BorderWidthLeft = 1f;
                                    //innercell.BorderWidthRight = 1f;
                                    //innercell.BorderWidthTop = 1f;
                                    //innercell.BorderWidthBottom = 1f;




                                    Image imagettd = GetSignatureImage(item.nip);
                                    cell1.AddElement(imagettd);

                                    Image QRCodeSignatureApprover = GetBarcodeSignature("NIP : " + item.nip + "\nNAMA : " + item.fullname + "\nJabatan : " + item.positionName);
                                    cell1.AddElement(QRCodeSignatureApprover);
                                    //if (imagettd !=null)
                                    //{
                                    //    cell.AddElement(imagettd);

                                    //    Image QRCodeSignatureApprover = GetBarcodeSignature("NIP : " + item.nip + "\nNAMA : " + item.fullname + "\nJabatan : " + item.positionName);
                                    //    cell.AddElement(QRCodeSignatureApprover);
                                    //}
                                    //else
                                    //{
                                    //    var img = "File tanda tangan tidak di temukan";
                                    //    paragraph = new Paragraph(img, normal);

                                    //    cell.AddElement(paragraph);
                                    //    Image QRCodeSignatureApprover = GetBarcodeSignature("NIP : " + item.nip + "\nNAMA : " + item.fullname + "\nJabatan : " + item.positionName);
                                    //    cell.AddElement(QRCodeSignatureApprover);
                                    //}

                                }
                            }

                            paragraph1 = new Paragraph(item.fullname, bold);
                            paragraph1.Alignment = Element.ALIGN_CENTER;
                            cell1.AddElement(paragraph1);
                            table1.AddCell(cell1);


                            pdfDoc.Add(table1);
                        }


                        #endregion
                        
                        pdfDoc.Close();

                        var dateLetterFormat = Convert.ToDateTime(letterDetail.letter.letterDate).ToString("dd MMMM yyyy");
                        var letterNumber = letterDetail.letter.letterNumber;
                        byte[] bytess = ms.ToArray();
                        byte[] bytes = AddPageNumbers(bytess, dateLetterFormat, letterNumber, "Memo");
                        ms.Close();

                        return File(bytes.ToArray(), "application/pdf", fileName);
                    }

                #endregion
             }

            // Done (Masi Ikuti Template Memo Direksi)
            private FileContentResult PrivSuratPernyataanDireksi(string letternumber, OutputDetailMemo letterDetail, string fileName)
            {
                #region Surat Pernyataan Direksi
                    StringBuilder sb = new StringBuilder();
                    sb.Append("<div style='padding-left:2px;'>" + letterDetail.letterContent.letterContent + "</div>");
                    StringReader sr = new StringReader(sb.ToString());
                    //Document pdfDoc = new Document(PageSize.A4, 25, 25, 25, 15);
                    //Document pdfDoc = new Document(PageSize.A4, 56, 56, 113, 97);
                    Document pdfDoc = new Document(PageSize.A4, 56.7f, 56.7f, 66.5f, 99.225f);
                    HTMLWorker htmlparser = new HTMLWorker(pdfDoc);


                    using (MemoryStream ms = new MemoryStream())
                    {
                        PdfWriter pdfWriter = PdfWriter.GetInstance(pdfDoc, ms);
                        pdfDoc.Open();
                        //Font
                        int no;
                        var fontName = "Calibri";
                        string fontPath = Path.Combine(_environment.WebRootPath, "fonts/Calibri.ttf");
                        //FontFactory.Register(fontPath);

                        Font normal = FontFactory.GetFont(fontName, 12, Font.NORMAL, BaseColor.BLACK);
                        Font bold = FontFactory.GetFont(fontName, 12, Font.BOLD, BaseColor.BLACK);
                        Font underline = FontFactory.GetFont(fontName, 12, Font.UNDERLINE, BaseColor.BLACK);
                        Font underlineBold = FontFactory.GetFont(fontName, 12, Font.BOLD | Font.UNDERLINE, BaseColor.BLACK);

                        Font normalDeliberation = FontFactory.GetFont(fontName, 10, Font.NORMAL, BaseColor.BLACK);
                        Font underlineDeliberation = FontFactory.GetFont(fontName, 10, Font.UNDERLINE, BaseColor.BLACK);
                        Font underlineDeliberationBold = FontFactory.GetFont(fontName, 10, Font.BOLD | Font.UNDERLINE, BaseColor.BLACK);
                        // Colors
                        BaseColor colorBlack = new BaseColor(System.Drawing.ColorTranslator.FromHtml("#000000"));
                        BaseColor colorWhite = new BaseColor(System.Drawing.ColorTranslator.FromHtml("#FFFFFF"));



                        PdfPTable table = new PdfPTable(2);
                        float[] width = new float[] { 0.667f, 2f };
                        table.WidthPercentage = 100;
                        table.SetWidths(width);
                        table.HorizontalAlignment = Element.ALIGN_LEFT;
                        table.DefaultCell.Border = 0;
                        table.SpacingBefore = 0f;
                        table.SpacingAfter = 0f;

                        PdfPCell cell = new PdfPCell();
                        Paragraph paragraph = new Paragraph();


                        #region header
                        //add new page

                        //table = new PdfPTable(1);
                        //width = new float[] { 2f };
                        //table.WidthPercentage = 100;
                        //table.SetWidths(width);
                        //table.HorizontalAlignment = Element.ALIGN_LEFT;
                        //table.DefaultCell.Border = 0;
                        //table.SpacingBefore = 00f;
                        //table.SpacingAfter = 0f;


                        //table.AddCell(new Phrase("MEMO", bold));
                        //pdfDoc.Add(table);

                            table = new PdfPTable(2);
                            width = new float[] { 0.2f, 1f };
                            table.WidthPercentage = 100;
                            table.SetWidths(width);
                            table.HorizontalAlignment = Element.ALIGN_RIGHT;
                            table.DefaultCell.Border = 0;
                            table.SpacingBefore = 0f;
                            table.SpacingAfter = 0f;

                            cell.Border = 0;
                            paragraph = new Paragraph("Nomor", normal);
                            paragraph.Alignment = Element.ALIGN_LEFT;
                            cell.AddElement(paragraph);
                            table.AddCell(cell);

                            //cell = new PdfPCell();
                            //cell.Border = 0;
                            //paragraph = new Paragraph(":", normal);
                            //paragraph.Alignment = Element.ALIGN_RIGHT;
                            //cell.AddElement(paragraph);
                            //table.AddCell(cell);

                            cell = new PdfPCell();
                            cell.Border = 0;
                            paragraph = new Paragraph(":"+" "+letterDetail.letter.letterNumber, normal);
                            paragraph.Alignment = Element.ALIGN_LEFT;
                            cell.AddElement(paragraph);
                            table.AddCell(cell);

                            cell = new PdfPCell();
                            cell.Border = 0;
                            paragraph = new Paragraph("Tanggal", normal);
                            paragraph.Alignment = Element.ALIGN_LEFT;
                            cell.AddElement(paragraph);
                            table.AddCell(cell);

                            //cell = new PdfPCell();
                            //cell.Border = 0;
                            //paragraph = new Paragraph(":", normal);
                            //paragraph.Alignment = Element.ALIGN_RIGHT;
                            //cell.AddElement(paragraph);
                            //table.AddCell(cell);

                            cell = new PdfPCell();
                            cell.Border = 0;
                            paragraph = new Paragraph(":"+" "+Convert.ToDateTime(letterDetail.letter.letterDate).ToString("dd MMMM yyyy"), normal);
                            paragraph.Alignment = Element.ALIGN_LEFT;
                            cell.AddElement(paragraph);
                            table.AddCell(cell);

                            cell = new PdfPCell();
                            cell.Border = 0;
                            paragraph = new Paragraph("Kepada", normal);
                            paragraph.Alignment = Element.ALIGN_LEFT;
                            cell.AddElement(paragraph);
                            table.AddCell(cell);

                            //cell = new PdfPCell();
                            //cell.Border = 0;
                            //paragraph = new Paragraph(":", normal);
                            //paragraph.Alignment = Element.ALIGN_RIGHT;
                            //cell.AddElement(paragraph);
                            //table.AddCell(cell);

                            cell = new PdfPCell();
                            cell.Border = 0;
                            no = 1;
                            foreach (var item in letterDetail.receiver)
                            {
                                if (letterDetail.receiver.Count() > 1)
                                {
                                    paragraph = new Paragraph(":"+" "+no + ". " + item.fullname + " - " + item.positionName, normal);
                                    paragraph.Alignment = Element.ALIGN_LEFT;
                                    cell.AddElement(paragraph);
                                }
                                else
                                {
                                    paragraph = new Paragraph(":"+" "+item.fullname + " - " + item.positionName, normal);
                                    paragraph.Alignment = Element.ALIGN_LEFT;
                                    cell.AddElement(paragraph);
                                }
                                no++;
                            }
                            table.AddCell(cell);

                            cell = new PdfPCell();
                            cell.Border = 0;
                            paragraph = new Paragraph("Tembusan", normal);
                            paragraph.Alignment = Element.ALIGN_LEFT;
                            cell.AddElement(paragraph);
                            table.AddCell(cell);

                            //cell = new PdfPCell();
                            //cell.Border = 0;
                            //paragraph = new Paragraph(":", normal);
                            //paragraph.Alignment = Element.ALIGN_RIGHT;
                            //cell.AddElement(paragraph);
                            //table.AddCell(cell);

                            cell = new PdfPCell();
                            cell.Border = 0;
                            no = 1;
                            foreach (var item in letterDetail.copy)
                            {
                                if (letterDetail.copy.Count() > 1)
                                {
                                    paragraph = new Paragraph(":"+" "+no + ". " + item.fullname + " - " + item.positionName, normal);
                                    paragraph.Alignment = Element.ALIGN_LEFT;
                                    cell.AddElement(paragraph);
                                }
                                else
                                {
                                    paragraph = new Paragraph(":"+" "+item.fullname + " - " + item.positionName, normal);
                                    paragraph.Alignment = Element.ALIGN_LEFT;
                                    cell.AddElement(paragraph);
                                }
                                no++;
                            }
                            if (letterDetail.copy.Count() == 0)
                            {
                                paragraph = new Paragraph("-", normal);
                                paragraph.Alignment = Element.ALIGN_LEFT;
                                cell.AddElement(paragraph);
                            }
                            table.AddCell(cell);

                            cell = new PdfPCell();
                            cell.Border = 0;
                            paragraph = new Paragraph("Dari", normal);
                            paragraph.Alignment = Element.ALIGN_LEFT;
                            cell.AddElement(paragraph);
                            table.AddCell(cell);

                            //cell = new PdfPCell();
                            //cell.Border = 0;
                            //paragraph = new Paragraph(":", normal);
                            //paragraph.Alignment = Element.ALIGN_RIGHT;
                            //cell.AddElement(paragraph);
                            //table.AddCell(cell);

                            cell = new PdfPCell();
                            cell.Border = 0;
                            paragraph = new Paragraph(":"+" "+letterDetail.sender[0].fullname + " - " + letterDetail.sender[0].positionName, normal);
                            paragraph.Alignment = Element.ALIGN_LEFT;
                            cell.AddElement(paragraph);
                            table.AddCell(cell);

                            cell = new PdfPCell();
                            cell.Border = 0;
                            paragraph = new Paragraph("Perihal", normal);
                            paragraph.Alignment = Element.ALIGN_LEFT;
                            cell.AddElement(paragraph);
                            table.AddCell(cell);

                            //cell = new PdfPCell();
                            //cell.Border = 0;
                            //paragraph = new Paragraph(":", normal);
                            //paragraph.Alignment = Element.ALIGN_RIGHT;
                            //cell.AddElement(paragraph);
                            //table.AddCell(cell);

                            cell = new PdfPCell();
                            cell.Border = 0;
                            paragraph = new Paragraph(":"+" "+letterDetail.letter.about, normal);
                            paragraph.Alignment = Element.ALIGN_LEFT;
                            cell.AddElement(paragraph);
                            table.AddCell(cell);

                            cell = new PdfPCell();
                            cell.Border = 0;
                            paragraph = new Paragraph("Lampiran", normal);
                            paragraph.Alignment = Element.ALIGN_LEFT;
                            cell.AddElement(paragraph);
                            table.AddCell(cell);

                            //cell = new PdfPCell();
                            //cell.Border = 0;
                            //paragraph = new Paragraph(":", normal);
                            //paragraph.Alignment = Element.ALIGN_RIGHT;
                            //cell.AddElement(paragraph);
                            //table.AddCell(cell);

                            var lampiran = letterDetail.letter.attachmentDesc == null ? "-" : letterDetail.letter.attachmentDesc;
                            cell = new PdfPCell();
                            cell.Border = 0;
                            paragraph = new Paragraph(":"+" "+lampiran, bold);
                            paragraph.Alignment = Element.ALIGN_LEFT;
                            cell.AddElement(paragraph);
                            table.AddCell(cell);

                            //Add table to document
                            pdfDoc.Add(table);


                        Paragraph line = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(2F, 100.0F, colorBlack, Element.ALIGN_CENTER, 1)));
                        line.SpacingBefore = 0f;
                        line.SetLeading(2F, 0.5F);
                        pdfDoc.Add(line);
                        Paragraph lines = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(2F, 100.0F, colorBlack, Element.ALIGN_CENTER, 1)));
                        lines.SpacingBefore = 0f;
                        line.SetLeading(0.5F, 0.5F);
                        pdfDoc.Add(line);

                        Paragraph liness = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(2F, 100.0F, colorWhite, Element.ALIGN_CENTER, 1)));
                        liness.SpacingBefore = 0f;
                        liness.SetLeading(0.5F, 0.5F);
                        pdfDoc.Add(liness);

                        //Paragraph linesss = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(2F, 100.0F, colorWhite, Element.ALIGN_CENTER, 1)));
                        //linesss.SpacingBefore = 0f;
                        //linesss.SetLeading(0.5F, 0.5F);
                        //pdfDoc.Add(linesss);


                        table = new PdfPTable(1);
                        width = new float[] { 2f };
                        table.WidthPercentage = 100;
                        table.SetWidths(width);
                        table.HorizontalAlignment = Element.ALIGN_LEFT;
                        table.DefaultCell.Border = 0;
                        table.SpacingBefore = 00f;
                        table.SpacingAfter = 0f;


                        //table.AddCell(new Phrase("Dengan hormat", normal));
                        //pdfDoc.Add(table);

                        Paragraph linessss = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(2F, 100.0F, colorWhite, Element.ALIGN_CENTER, 1)));
                        linessss.SpacingBefore = 0f;
                        linessss.SetLeading(0.5F, 0.5F);
                        pdfDoc.Add(linessss);

                        Paragraph lin = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(2F, 100.0F, colorWhite, Element.ALIGN_CENTER, 1)));
                        lin.SpacingBefore = 0f;
                        lin.SetLeading(0.5F, 0.5F);
                        pdfDoc.Add(lin);

                        #endregion

                        //ADD HTML CONTENT
                        htmlparser.Parse(sr);


                        #region Tanda Tangan

                        Paragraph a = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(2F, 100.0F, colorWhite, Element.ALIGN_CENTER, 1)));
                        a.SpacingBefore = 0f;
                        a.SetLeading(0.5F, 0.5F);
                        pdfDoc.Add(a);



                        cell = new PdfPCell();
                        table.AddCell(new Phrase("", normal));

                        cell = new PdfPCell();
                        table.AddCell(new Phrase(" ", normal));
                        cell = new PdfPCell();
                        table.AddCell(new Phrase(" ", normal));
                        cell = new PdfPCell();
                        table.AddCell(new Phrase("Hormat kami, \n" + "PT BNI Life Insurance", normal));
                        //table.AddCell(new Phrase("Best Regards", normal));
                        pdfDoc.Add(table);



                        string filename = letterDetail.sender[0].nip;


                        // Tanda tangan
                        var dirChecker = letterDetail.sender.OrderBy(p => p.idUserSender).ToList();
                        //var dirChecker = letterDetail.checker.Where(p => p.idLevelChecker == 2).ToList();

                        table = new PdfPTable(dirChecker.Count());
                        width = new float[dirChecker.Count()];
                        for (int i = 0; i < dirChecker.Count(); i++)
                        {
                            width[i] = 1f;
                        }
                        table.WidthPercentage = 100;
                        table.SetWidths(width);
                        table.HorizontalAlignment = Element.ALIGN_LEFT;
                        table.DefaultCell.Border = 0;
                        var no1 = 0;


                        //cell = new PdfPCell();
                        //table.AddCell(new Phrase("Direksi PT BNI Life Insurance", normal));
                        foreach (var item in dirChecker)
                        {
                            cell = new PdfPCell();
                            cell.Border = 0;




                            if (letterDetail.letter.letterNumber != "NO_LETTER")
                            {
                                if (letterDetail.letter.signatureType == 1)
                                {

                                    Image imagettd = GetSignatureImage(item.nip);
                                    //cell.AddElement(imagettd);
                                    if (imagettd != null)
                                    {
                                        cell.AddElement(imagettd);
                                    }
                                    else
                                    {
                                        var img = "File tanda tangan tidak ditemukan";
                                        paragraph = new Paragraph(img, normal);
                                        cell.AddElement(paragraph);
                                    }

                                }
                                else if (letterDetail.letter.signatureType == 2)
                                {


                                    Image QRCodeSignatureApprover = GetBarcodeSignature("NIP : " + item.nip + "\nNAMA : " + item.fullname + "\nJabatan : " + item.positionName);
                                    QRCodeSignatureApprover.ScaleAbsoluteHeight(10f);
                                    cell.AddElement(QRCodeSignatureApprover);
                                }
                                else if (letterDetail.letter.signatureType == 3)
                                {
                                    var img = "";
                                    paragraph = new Paragraph(img, normal);
                                    cell.AddElement(paragraph);
                                    Paragraph linessia = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(2F, 100.0F, colorWhite, Element.ALIGN_CENTER, 1)));
                                    linessia.SpacingBefore = 0f;
                                    linessia.SetLeading(0.5F, 0.5F);
                                    pdfDoc.Add(linessia);

                                    Paragraph linessiaa = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(2F, 100.0F, colorWhite, Element.ALIGN_CENTER, 1)));
                                    linessiaa.SpacingBefore = 0f;
                                    linessiaa.SetLeading(0.5F, 0.5F);
                                    pdfDoc.Add(linessiaa);

                                    Paragraph linessiaaa = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(2F, 100.0F, colorWhite, Element.ALIGN_CENTER, 1)));
                                    linessiaaa.SpacingBefore = 0f;
                                    linessiaaa.SetLeading(0.5F, 0.5F);
                                    pdfDoc.Add(linessiaaa);
                                }
                                else
                                {

                                    Image imagettd = GetSignatureImage(item.nip);
                                    cell.AddElement(imagettd);
                                    Image QRCodeSignatureApprover = GetBarcodeSignature("NIP : " + item.nip + "\nNAMA : " + item.fullname + "\nJabatan : " + item.positionName);
                                    cell.AddElement(QRCodeSignatureApprover);
                                    //if (imagettd != null)
                                    //{
                                    //    cell.AddElement(imagettd);
                                    //    Image QRCodeSignatureApprover = GetBarcodeSignature("NIP : " + item.nip + "\nNAMA : " + item.fullname + "\nJabatan : " + item.positionName);
                                    //    cell.AddElement(QRCodeSignatureApprover);
                                    //}
                                    //else
                                    //{
                                    //    var img = "File tanda tangan tidak di temukan";
                                    //    paragraph = new Paragraph(img, normal);
                                    //    cell.AddElement(paragraph);
                                    //    Image QRCodeSignatureApprover = GetBarcodeSignature("NIP : " + item.nip + "\nNAMA : " + item.fullname + "\nJabatan : " + item.positionName);
                                    //    cell.AddElement(QRCodeSignatureApprover);
                                    //}

                                }
                                no1++;
                            }

                            paragraph = new Paragraph(item.fullname, underlineBold);
                            cell.AddElement(paragraph);
                            paragraph = new Paragraph(item.positionName, bold);
                            cell.AddElement(paragraph);

                            table.AddCell(cell);
                        }
                        // end tanda tangan
                        pdfDoc.Add(table);

                        #endregion

                        pdfDoc.NewPage();

                        #region Lampiran

                        PdfPTable table1 = new PdfPTable(2);
                        float[] width1 = new float[] { 0.667f, 2f };
                        table1.WidthPercentage = 100;
                        table1.SetWidths(width1);
                        table1.HorizontalAlignment = Element.ALIGN_LEFT;
                        table1.DefaultCell.Border = 0;
                        table1.SpacingBefore = 0f;
                        table1.SpacingAfter = 0f;

                        PdfPCell cell1 = new PdfPCell();
                        Paragraph paragraph1 = new Paragraph();

                        cell1 = new PdfPCell();
                        cell1.BorderWidthLeft = 1f;
                        cell1.BorderWidthRight = 1f;
                        cell1.BorderWidthTop = 1f;
                        cell1.BorderWidthBottom = 1f;
                        cell1.MinimumHeight = 30f;
                        paragraph1 = new Paragraph("Nomor Registrasi Memo/Registration Number:" + letterDetail.letter.letterDeliberationNumber, normalDeliberation);
                        paragraph1.Alignment = Element.ALIGN_LEFT;
                        cell1.AddElement(paragraph1);
                        table1.AddCell(cell1);

                        cell1 = new PdfPCell();
                        cell1.BorderWidthLeft = 0f;
                        cell1.BorderWidthRight = 1f;
                        cell1.BorderWidthTop = 1f;
                        cell1.BorderWidthBottom = 1f;
                        cell1.MinimumHeight = 30f;
                        paragraph1 = new Paragraph("Tanggal Registrasi Memo/Date of Memo Registration : " + Convert.ToDateTime(letterDetail.letter.letterDate).ToString("dd MMMM yyyy"), normalDeliberation);
                        paragraph1.Alignment = Element.ALIGN_LEFT;
                        cell1.AddElement(paragraph1);
                        table1.AddCell(cell1);

                        cell1 = new PdfPCell();
                        cell1.BorderWidthLeft = 1f;
                        cell1.BorderWidthRight = 1f;
                        cell1.BorderWidthTop = 0f;
                        cell1.BorderWidthBottom = 1f;
                        cell1.MinimumHeight = 100f;
                        paragraph1 = new Paragraph("Nomor Memo : " + letterDetail.letter.letterNumber, normalDeliberation);
                        paragraph1.Alignment = Element.ALIGN_LEFT;
                        cell1.AddElement(paragraph1);
                        table1.AddCell(cell1);

                        cell1 = new PdfPCell();
                        cell1.BorderWidthLeft = 0f;
                        cell1.BorderWidthRight = 1f;
                        cell1.BorderWidthTop = 0f;
                        cell1.BorderWidthBottom = 1f;
                        cell1.MinimumHeight = 100f;
                        paragraph1 = new Paragraph("(Ringkasan Memo/Summary of the proposal) \n" + letterDetail.letterContent.summary, normalDeliberation);
                        paragraph1.Alignment = Element.ALIGN_LEFT;
                        cell1.AddElement(paragraph1);
                        table1.AddCell(cell1);

                        cell1 = new PdfPCell();
                        cell1.BorderWidthLeft = 1f;
                        cell1.BorderWidthRight = 1f;
                        cell1.BorderWidthTop = 0f;
                        cell1.BorderWidthBottom = 1f;
                        cell1.MinimumHeight = 100f;
                        paragraph1 = new Paragraph("Deliberation:", normalDeliberation);
                        paragraph1.Alignment = Element.ALIGN_LEFT;
                        cell1.AddElement(paragraph1);
                        no = 1;
                        foreach (var item in letterDetail.delibration)
                        {
                            if (letterDetail.receiver.Count() > 1)
                            {
                                paragraph1 = new Paragraph(no + ". " + item.fullname + " - " + item.positionName, normalDeliberation);
                                paragraph1.Alignment = Element.ALIGN_LEFT;
                                cell1.AddElement(paragraph1);
                            }
                            else
                            {
                                paragraph1 = new Paragraph(item.fullname + " - " + item.positionName, normalDeliberation);
                                paragraph1.Alignment = Element.ALIGN_LEFT;
                                cell1.AddElement(paragraph1);
                            }
                            no++;
                        }
                        table1.AddCell(cell1);

                        cell1 = new PdfPCell();
                        cell1.BorderWidthLeft = 0f;
                        cell1.BorderWidthRight = 1f;
                        cell1.BorderWidthTop = 0f;
                        cell1.BorderWidthBottom = 1f;
                        cell1.MinimumHeight = 100f;
                        paragraph1 = new Paragraph("Catatan / Notes : \n", underlineDeliberation);
                        paragraph1.Alignment = Element.ALIGN_LEFT;
                        cell1.AddElement(paragraph1);
                        no = 1;
                        foreach (var item in letterDetail.delibration)
                        {
                            if (letterDetail.receiver.Count() > 1)
                            {
                                paragraph1 = new Paragraph(no + ". " + item.commentdlbrt, normalDeliberation);
                                paragraph1.Alignment = Element.ALIGN_LEFT;
                                cell1.AddElement(paragraph1);
                            }
                            else
                            {
                                paragraph1 = new Paragraph(item.commentdlbrt, normalDeliberation);
                                paragraph1.Alignment = Element.ALIGN_LEFT;
                                cell1.AddElement(paragraph1);
                            }
                            no++;
                        }
                        table1.AddCell(cell1);

                        pdfDoc.Add(table1);

                        //var checkerList = letterDetail.checker.Where(p => p.ordernumber != 1).OrderBy(p => p.idLevelChecker).ToList();
                        var checkerList = letterDetail.checker.OrderBy(p => p.idLevelChecker).ToList();
                        foreach (var item in checkerList)
                        {
                            var getLogComment = letterDetail.log.Where(p => p.idUserLog == item.idUserChecker && p.description.Contains("pemeriksa ke")).OrderByDescending(p => p.createdOn).FirstOrDefault();
                            var comment = "";
                            string approveDate = "";
                            if (getLogComment != null)
                            {
                                comment = getLogComment.comment;
                                approveDate = Convert.ToDateTime(getLogComment.createdOn).ToString("dd MMMM yyyy");
                            }
                            table1 = new PdfPTable(3);
                            width = new float[] { 1f, 2f, 1f };
                            table1.WidthPercentage = 100;
                            table1.SetWidths(width);
                            table1.HorizontalAlignment = Element.ALIGN_LEFT;
                            table1.DefaultCell.Border = 0;
                            table1.SpacingBefore = 0f;
                            table1.SpacingAfter = 0f;

                            cell1 = new PdfPCell();
                            cell1.Rowspan = 2;
                            cell1.BorderWidthLeft = 1f;
                            cell1.BorderWidthRight = 1f;
                            cell1.BorderWidthTop = 0f;
                            cell1.BorderWidthBottom = 1f;
                            cell1.MinimumHeight = 120f;
                            if (letterDetail.letter.statusCode == 2)
                                {
                                    paragraph1 = new Paragraph("Belum diputuskan / Undecided \n", normalDeliberation);
                                }
                                else if (letterDetail.letter.statusCode == 5)
                                {
                                    paragraph1 = new Paragraph("Ditolak / Reject \n", normalDeliberation);
                                }
                                else
                                {
                                   if(item.idLevelChecker == 2 ||item.idLevelChecker == 1)
                                   { 
                                       paragraph1 = new Paragraph("Disetujui / Approved \n", normalDeliberation);
                                   }
                                   else
                                   {
                                       paragraph1 = new Paragraph("Mengetahui / Understand \n", normalDeliberation);
                                       
                                   }
                                }

                            paragraph1.Alignment = Element.ALIGN_LEFT;
                            cell1.AddElement(paragraph1);
                            paragraph1 = new Paragraph(item.positionName, normalDeliberation);
                            paragraph1.Alignment = Element.ALIGN_LEFT;
                            cell1.AddElement(paragraph1);
                            table1.AddCell(cell1);

                            cell1 = new PdfPCell();
                            cell1.Rowspan = 2;
                            cell1.BorderWidthLeft = 0f;
                            cell1.BorderWidthRight = 1f;
                            cell1.BorderWidthTop = 0f;
                            cell1.BorderWidthBottom = 1f;
                            cell1.MinimumHeight = 120f;
                            paragraph1 = new Paragraph("Catatan / Notes : \n", underlineDeliberation);
                            paragraph1.Alignment = Element.ALIGN_LEFT;
                            cell1.AddElement(paragraph1);
                            paragraph1 = new Paragraph(comment, normalDeliberation);
                            paragraph1.Alignment = Element.ALIGN_LEFT;
                            cell1.AddElement(paragraph1);
                            table1.AddCell(cell1);

                            cell1 = new PdfPCell();
                            cell1.BorderWidthLeft = 0f;
                            cell1.BorderWidthRight = 1f;
                            cell1.BorderWidthTop = 0f;
                            cell1.BorderWidthBottom = 1f;
                            cell1.MinimumHeight = 40f;
                            paragraph1 = new Paragraph("Tanggal : ", underlineDeliberation);
                            paragraph1.Alignment = Element.ALIGN_LEFT;
                            //cell1.AddElement(paragraph);
                            paragraph1 = new Paragraph(approveDate, normalDeliberation);
                            paragraph1.Alignment = Element.ALIGN_LEFT;
                            cell1.AddElement(paragraph1);
                            table1.AddCell(cell1);

                            cell1 = new PdfPCell();
                            cell1.BorderWidthLeft = 0f;
                            cell1.BorderWidthRight = 1f;
                            cell1.BorderWidthTop = 0f;
                            cell1.BorderWidthBottom = 1f;
                            paragraph1 = new Paragraph("Tanda Tangan : \n", underlineDeliberation);
                            paragraph1.Alignment = Element.ALIGN_LEFT;
                            cell1.AddElement(paragraph1);
                            if (letterDetail.letter.letterNumber != "NO_LETTER")
                            {
                                if (letterDetail.letter.signatureType == 1)
                                {
                                    Image imagettd = GetSignatureImage(item.nip);
                                    //cell.AddElement(imagettd);
                                    if (imagettd != null)
                                    {
                                        cell1.AddElement(imagettd);
                                    }
                                    else
                                    {
                                        var img = "File tanda tangan tidak ditemukan";
                                        paragraph1 = new Paragraph(img, normal);
                                        cell1.AddElement(paragraph1);
                                    }
                                }
                                else if (letterDetail.letter.signatureType == 2)
                                {

                                    Image QRCodeSignatureApprover = GetBarcodeSignature("NIP : " + item.nip + "\nNAMA : " + item.fullname + "\nJabatan : " + item.positionName);
                                    cell1.AddElement(QRCodeSignatureApprover);
                                }
                                else if (letterDetail.letter.signatureType == 3)
                                {
                                    var img = "";
                                    paragraph = new Paragraph(img, normal);
                                    cell.AddElement(paragraph);
                                    Paragraph linessia = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(2F, 100.0F, colorWhite, Element.ALIGN_CENTER, 1)));
                                    linessia.SpacingBefore = 0f;
                                    linessia.SetLeading(0.5F, 0.5F);
                                    pdfDoc.Add(linessia);

                                    Paragraph linessiaa = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(2F, 100.0F, colorWhite, Element.ALIGN_CENTER, 1)));
                                    linessiaa.SpacingBefore = 0f;
                                    linessiaa.SetLeading(0.5F, 0.5F);
                                    pdfDoc.Add(linessiaa);

                                    Paragraph linessiaaa = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(2F, 100.0F, colorWhite, Element.ALIGN_CENTER, 1)));
                                    linessiaaa.SpacingBefore = 0f;
                                    linessiaaa.SetLeading(0.5F, 0.5F);
                                    pdfDoc.Add(linessiaaa);
                                }
                                else
                                {
                                    //innertable1 = new PdfPtable1(2);
                                    //float[] widthinner = new float[] { 0.3f, 0.3f };
                                    //innertable1.WidthPercentage = 100;
                                    //innertable1.SetWidths(widthinner);
                                    //innertable1.HorizontalAlignment = Element.ALIGN_LEFT;
                                    //innertable1.DefaultCell.Border = 1;
                                    //innertable1.DefaultCell.PaddingBottom = 8;

                                    //innercell = new PdfPCell();
                                    //innercell.BorderWidthLeft = 1f;
                                    //innercell.BorderWidthRight = 1f;
                                    //innercell.BorderWidthTop = 1f;
                                    //innercell.BorderWidthBottom = 1f;




                                    Image imagettd = GetSignatureImage(item.nip);
                                    cell1.AddElement(imagettd);

                                    Image QRCodeSignatureApprover = GetBarcodeSignature("NIP : " + item.nip + "\nNAMA : " + item.fullname + "\nJabatan : " + item.positionName);
                                    cell1.AddElement(QRCodeSignatureApprover);
                                    //if (imagettd !=null)
                                    //{
                                    //    cell.AddElement(imagettd);

                                    //    Image QRCodeSignatureApprover = GetBarcodeSignature("NIP : " + item.nip + "\nNAMA : " + item.fullname + "\nJabatan : " + item.positionName);
                                    //    cell.AddElement(QRCodeSignatureApprover);
                                    //}
                                    //else
                                    //{
                                    //    var img = "File tanda tangan tidak di temukan";
                                    //    paragraph = new Paragraph(img, normal);

                                    //    cell.AddElement(paragraph);
                                    //    Image QRCodeSignatureApprover = GetBarcodeSignature("NIP : " + item.nip + "\nNAMA : " + item.fullname + "\nJabatan : " + item.positionName);
                                    //    cell.AddElement(QRCodeSignatureApprover);
                                    //}

                                }
                            }

                            paragraph1 = new Paragraph(item.fullname, bold);
                            paragraph1.Alignment = Element.ALIGN_CENTER;
                            cell1.AddElement(paragraph1);
                            table1.AddCell(cell1);


                            pdfDoc.Add(table1);
                        }


                        #endregion

                        pdfDoc.Close();

                        var dateLetterFormat = Convert.ToDateTime(letterDetail.letter.letterDate).ToString("dd MMMM yyyy");
                        var letterNumber = letterDetail.letter.letterNumber;
                        byte[] bytess = ms.ToArray();
                        byte[] bytes = AddPageNumbers(bytess, dateLetterFormat, letterNumber, "Memo");
                        ms.Close();

                        return File(bytes.ToArray(), "application/pdf", fileName);
                    }

                #endregion
            }

            // done 
            private FileContentResult PrivSuratKeteranganDireksi(string letternumber, OutputDetailMemo letterDetail, string fileName)
            {
                #region Surat Keterangan Direksi

                StringBuilder sb = new StringBuilder();
                sb.Append("<div style='padding-left:2px;'>" + letterDetail.letterContent.letterContent + "</div>");
                StringReader sr = new StringReader(sb.ToString());
                //Document pdfDoc = new Document(PageSize.A4, 25, 25, 25, 15);
                //Document pdfDoc = new Document(PageSize.A4, 56, 56, 113, 97);
                Document pdfDoc = new Document(PageSize.A4, 56.7f, 56.7f, 66.5f, 99.225f);
                HTMLWorker htmlparser = new HTMLWorker(pdfDoc);


                using (MemoryStream ms = new MemoryStream())
                {
                    PdfWriter pdfWriter = PdfWriter.GetInstance(pdfDoc, ms);
                    pdfDoc.Open();
                    //Font
                    int no;
                    var fontName = "Calibri";
                    string fontPath = Path.Combine(_environment.WebRootPath, "fonts/Calibri.ttf");
                    //FontFactory.Register(fontPath);

                    Font normal = FontFactory.GetFont(fontName, 12, Font.NORMAL, BaseColor.BLACK);
                    Font bold = FontFactory.GetFont(fontName, 12, Font.BOLD, BaseColor.BLACK);
                    Font underline = FontFactory.GetFont(fontName, 12, Font.UNDERLINE, BaseColor.BLACK);
                    Font underlineBold = FontFactory.GetFont(fontName, 12, Font.BOLD | Font.UNDERLINE, BaseColor.BLACK);

                    Font normalDeliberation = FontFactory.GetFont(fontName, 10, Font.NORMAL, BaseColor.BLACK);
                    Font underlineDeliberation = FontFactory.GetFont(fontName, 10, Font.UNDERLINE, BaseColor.BLACK);
                    Font underlineDeliberationBold = FontFactory.GetFont(fontName, 10, Font.BOLD | Font.UNDERLINE, BaseColor.BLACK);
                    // Colors
                    BaseColor colorBlack = new BaseColor(System.Drawing.ColorTranslator.FromHtml("#000000"));
                    BaseColor colorWhite = new BaseColor(System.Drawing.ColorTranslator.FromHtml("#FFFFFF"));

                    PdfPTable table = new PdfPTable(2);
                    float[] width = new float[] { 0.667f, 2f };
                    table.WidthPercentage = 100;
                    table.SetWidths(width);
                    table.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.DefaultCell.Border = 0;
                    table.SpacingBefore = 0f;
                    table.SpacingAfter = 0f;

                    PdfPCell cell = new PdfPCell();
                    Paragraph paragraph = new Paragraph();


                    #region header

                    table = new PdfPTable(1);
                    width = new float[] { 2f };
                    table.WidthPercentage = 100;
                    table.SetWidths(width);
                    table.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.DefaultCell.Border = 0;
                    table.SpacingBefore = 00f;
                    table.SpacingAfter = 0f;

                    cell = new PdfPCell();
                    cell.Border = 0;
                    paragraph = new Paragraph(new Phrase("Surat Keterangan", bold));
                    paragraph.Alignment = Element.ALIGN_CENTER;
                    cell.AddElement(paragraph);
                    table.AddCell(cell);
                    pdfDoc.Add(table);

                    table = new PdfPTable(1);
                    width = new float[] { 10f };
                    table.WidthPercentage = 100;
                    table.SetWidths(width);
                    table.HorizontalAlignment = Element.ALIGN_CENTER;
                    table.DefaultCell.Border = 0;
                    table.SpacingBefore = 00f;
                    table.SpacingAfter = 0f;

                    cell = new PdfPCell();
                    cell.Border = 0;
                    paragraph = new Paragraph(new Phrase("NOMOR : " + letterDetail.letter.letterNumber, normal));
                    paragraph.Alignment = Element.ALIGN_CENTER;
                    cell.AddElement(paragraph);
                    table.AddCell(cell);
                    pdfDoc.Add(table);


                    Paragraph line = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(2F, 100.0F, colorBlack, Element.ALIGN_CENTER, 1)));
                    line.SpacingBefore = 0f;
                    line.SetLeading(2F, 0.5F);
                    pdfDoc.Add(line);
                    Paragraph lines = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(2F, 100.0F, colorBlack, Element.ALIGN_CENTER, 1)));
                    lines.SpacingBefore = 0f;
                    line.SetLeading(0.5F, 0.5F);
                    pdfDoc.Add(line);

                    Paragraph liness = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(2F, 100.0F, colorWhite, Element.ALIGN_CENTER, 1)));
                    liness.SpacingBefore = 0f;
                    liness.SetLeading(0.5F, 0.5F);
                    pdfDoc.Add(liness);

                    //Paragraph a = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(2F, 100.0F, colorWhite, Element.ALIGN_CENTER, 1)));
                    //a.SpacingBefore = 0f;
                    //a.SetLeading(0.5F, 0.5F);
                    //pdfDoc.Add(a);

                    //Paragraph aaw = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(2F, 100.0F, colorWhite, Element.ALIGN_CENTER, 1)));
                    //aaw.SpacingBefore = 0f;
                    //aaw.SetLeading(0.5F, 0.5F);
                    //pdfDoc.Add(aaw);
                    
                    #endregion



                    //ADD HTML CONTENT
                    htmlparser.Parse(sr);

                    #region Tanda Tangan

                    Paragraph ac = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(2F, 100.0F, colorWhite, Element.ALIGN_CENTER, 1)));
                    ac.SpacingBefore = 0f;
                    ac.SetLeading(0.5F, 0.5F);
                    pdfDoc.Add(ac);



                    cell = new PdfPCell();
                    table.AddCell(new Phrase("", normal));

                    cell = new PdfPCell();
                    table.AddCell(new Phrase(" ", normal));
                    cell = new PdfPCell();
                    table.AddCell(new Phrase(" ", normal));
                    cell = new PdfPCell();
                    table.AddCell(new Phrase("Hormat kami, \n" + "PT BNI Life Insurance", normal));
                    table.AddCell(new Phrase("Best Regards", normal));
                    pdfDoc.Add(table);



                    string filename = letterDetail.sender[0].nip;


                    // Tanda tangan
                    var dirChecker = letterDetail.sender.OrderBy(p => p.idUserSender).ToList();
                    //var dirChecker = letterDetail.checker.Where(p => p.idLevelChecker == 2 ).ToList();

                    table = new PdfPTable(dirChecker.Count());
                    width = new float[dirChecker.Count()];
                    for (int i = 0; i < dirChecker.Count(); i++)
                    {
                        width[i] = 1f;
                    }
                    table.WidthPercentage = 100;
                    table.SetWidths(width);
                    table.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.DefaultCell.Border = 0;
                    var no1 = 0;


                    //cell = new PdfPCell();
                    //table.AddCell(new Phrase("Direksi PT BNI Life Insurance", normal));
                    foreach (var item in dirChecker)
                    {
                        cell = new PdfPCell();
                        cell.Border = 0;




                        if (letterDetail.letter.letterNumber != "NO_LETTER")
                        {
                            if (letterDetail.letter.signatureType == 1)
                            {

                                Image imagettd = GetSignatureImage(item.nip);
                                //cell.AddElement(imagettd);
                                if (imagettd != null)
                                {
                                    cell.AddElement(imagettd);
                                }
                                else
                                {
                                    var img = "File tanda tangan tidak ditemukan";
                                    paragraph = new Paragraph(img, normal);
                                    cell.AddElement(paragraph);
                                }

                            }
                            else if (letterDetail.letter.signatureType == 2)
                            {


                                Image QRCodeSignatureApprover = GetBarcodeSignature("NIP : " + item.nip + "\nNAMA : " + item.fullname + "\nJabatan : " + item.positionName);
                                QRCodeSignatureApprover.ScaleAbsoluteHeight(10f);
                                cell.AddElement(QRCodeSignatureApprover);
                            }
                            else if (letterDetail.letter.signatureType == 3)
                            {
                                var img = "";
                                paragraph = new Paragraph(img, normal);
                                cell.AddElement(paragraph);
                                Paragraph linessia = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(2F, 100.0F, colorWhite, Element.ALIGN_CENTER, 1)));
                                linessia.SpacingBefore = 0f;
                                linessia.SetLeading(0.5F, 0.5F);
                                pdfDoc.Add(linessia);

                                Paragraph linessiaa = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(2F, 100.0F, colorWhite, Element.ALIGN_CENTER, 1)));
                                linessiaa.SpacingBefore = 0f;
                                linessiaa.SetLeading(0.5F, 0.5F);
                                pdfDoc.Add(linessiaa);

                                Paragraph linessiaaa = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(2F, 100.0F, colorWhite, Element.ALIGN_CENTER, 1)));
                                linessiaaa.SpacingBefore = 0f;
                                linessiaaa.SetLeading(0.5F, 0.5F);
                                pdfDoc.Add(linessiaaa);
                            }
                            else
                            {

                                Image imagettd = GetSignatureImage(item.nip);
                                cell.AddElement(imagettd);
                                Image QRCodeSignatureApprover = GetBarcodeSignature("NIP : " + item.nip + "\nNAMA : " + item.fullname + "\nJabatan : " + item.positionName);
                                cell.AddElement(QRCodeSignatureApprover);
                                //if (imagettd != null)
                                //{
                                //    cell.AddElement(imagettd);
                                //    Image QRCodeSignatureApprover = GetBarcodeSignature("NIP : " + item.nip + "\nNAMA : " + item.fullname + "\nJabatan : " + item.positionName);
                                //    cell.AddElement(QRCodeSignatureApprover);
                                //}
                                //else
                                //{
                                //    var img = "File tanda tangan tidak di temukan";
                                //    paragraph = new Paragraph(img, normal);
                                //    cell.AddElement(paragraph);
                                //    Image QRCodeSignatureApprover = GetBarcodeSignature("NIP : " + item.nip + "\nNAMA : " + item.fullname + "\nJabatan : " + item.positionName);
                                //    cell.AddElement(QRCodeSignatureApprover);
                                //}

                            }
                            no1++;
                        }

                        paragraph = new Paragraph(item.fullname, underlineBold);
                        cell.AddElement(paragraph);
                        paragraph = new Paragraph(item.positionName, bold);
                        cell.AddElement(paragraph);

                        table.AddCell(cell);
                    }
                    // end tanda tangan
                    pdfDoc.Add(table);

                    #endregion

                    pdfDoc.NewPage();

                    #region Lampiran

                    PdfPTable table1 = new PdfPTable(2);
                    float[] width1 = new float[] { 0.667f, 2f };
                    table1.WidthPercentage = 100;
                    table1.SetWidths(width1);
                    table1.HorizontalAlignment = Element.ALIGN_LEFT;
                    table1.DefaultCell.Border = 0;
                    table1.SpacingBefore = 0f;
                    table1.SpacingAfter = 0f;

                    PdfPCell cell1 = new PdfPCell();
                    Paragraph paragraph1 = new Paragraph();

                    cell1 = new PdfPCell();
                    cell1.BorderWidthLeft = 1f;
                    cell1.BorderWidthRight = 1f;
                    cell1.BorderWidthTop = 1f;
                    cell1.BorderWidthBottom = 1f;
                    cell1.MinimumHeight = 30f;
                    paragraph1 = new Paragraph("Nomor Registrasi Memo/Registration Number:" + letterDetail.letter.letterDeliberationNumber, normalDeliberation);
                    paragraph1.Alignment = Element.ALIGN_LEFT;
                    cell1.AddElement(paragraph1);
                    table1.AddCell(cell1);

                    cell1 = new PdfPCell();
                    cell1.BorderWidthLeft = 0f;
                    cell1.BorderWidthRight = 1f;
                    cell1.BorderWidthTop = 1f;
                    cell1.BorderWidthBottom = 1f;
                    cell1.MinimumHeight = 30f;
                    paragraph1 = new Paragraph("Tanggal Registrasi Memo/Date of Memo Registration : " + Convert.ToDateTime(letterDetail.letter.letterDate).ToString("dd MMMM yyyy"), normalDeliberation);
                    paragraph1.Alignment = Element.ALIGN_LEFT;
                    cell1.AddElement(paragraph1);
                    table1.AddCell(cell1);

                    cell1 = new PdfPCell();
                    cell1.BorderWidthLeft = 1f;
                    cell1.BorderWidthRight = 1f;
                    cell1.BorderWidthTop = 0f;
                    cell1.BorderWidthBottom = 1f;
                    cell1.MinimumHeight = 100f;
                    paragraph1 = new Paragraph("Nomor Memo : " + letterDetail.letter.letterNumber, normalDeliberation);
                    paragraph1.Alignment = Element.ALIGN_LEFT;
                    cell1.AddElement(paragraph1);
                    table1.AddCell(cell1);

                    cell1 = new PdfPCell();
                    cell1.BorderWidthLeft = 0f;
                    cell1.BorderWidthRight = 1f;
                    cell1.BorderWidthTop = 0f;
                    cell1.BorderWidthBottom = 1f;
                    cell1.MinimumHeight = 100f;
                    paragraph1 = new Paragraph("(Ringkasan Memo/Summary of the proposal) \n" + letterDetail.letterContent.summary, normalDeliberation);
                    paragraph1.Alignment = Element.ALIGN_LEFT;
                    cell1.AddElement(paragraph1);
                    table1.AddCell(cell1);

                    cell1 = new PdfPCell();
                    cell1.BorderWidthLeft = 1f;
                    cell1.BorderWidthRight = 1f;
                    cell1.BorderWidthTop = 0f;
                    cell1.BorderWidthBottom = 1f;
                    cell1.MinimumHeight = 100f;
                    paragraph1 = new Paragraph("Deliberation:", normalDeliberation);
                    paragraph1.Alignment = Element.ALIGN_LEFT;
                    cell1.AddElement(paragraph1);
                    no = 1;
                    foreach (var item in letterDetail.delibration)
                    {
                        if (letterDetail.receiver.Count() > 1)
                        {
                            paragraph1 = new Paragraph(no + ". " + item.fullname + " - " + item.positionName, normalDeliberation);
                            paragraph1.Alignment = Element.ALIGN_LEFT;
                            cell1.AddElement(paragraph1);
                        }
                        else
                        {
                            paragraph1 = new Paragraph(item.fullname + " - " + item.positionName, normalDeliberation);
                            paragraph1.Alignment = Element.ALIGN_LEFT;
                            cell1.AddElement(paragraph1);
                        }
                        no++;
                    }
                    table1.AddCell(cell1);

                    cell1 = new PdfPCell();
                    cell1.BorderWidthLeft = 0f;
                    cell1.BorderWidthRight = 1f;
                    cell1.BorderWidthTop = 0f;
                    cell1.BorderWidthBottom = 1f;
                    cell1.MinimumHeight = 100f;
                    paragraph1 = new Paragraph("Catatan / Notes : \n", underlineDeliberation);
                    paragraph1.Alignment = Element.ALIGN_LEFT;
                    cell1.AddElement(paragraph1);
                    no = 1;
                    foreach (var item in letterDetail.delibration)
                    {
                        if (letterDetail.receiver.Count() > 1)
                        {
                            paragraph1 = new Paragraph(no + ". " + item.commentdlbrt, normalDeliberation);
                            paragraph1.Alignment = Element.ALIGN_LEFT;
                            cell1.AddElement(paragraph1);
                        }
                        else
                        {
                            paragraph1 = new Paragraph(item.commentdlbrt, normalDeliberation);
                            paragraph1.Alignment = Element.ALIGN_LEFT;
                            cell1.AddElement(paragraph1);
                        }
                        no++;
                    }
                    table1.AddCell(cell1);

                    pdfDoc.Add(table1);

                    //var checkerList = letterDetail.checker.Where(p => p.ordernumber != 1).OrderBy(p => p.idLevelChecker).ToList();
                    var checkerList = letterDetail.checker.OrderBy(p => p.idLevelChecker).ToList();
                    foreach (var item in checkerList)
                    {
                        var getLogComment = letterDetail.log.Where(p => p.idUserLog == item.idUserChecker && p.description.Contains("pemeriksa ke")).OrderByDescending(p => p.createdOn).FirstOrDefault();
                        var comment = "";
                        string approveDate = "";
                        if (getLogComment != null)
                        {
                            comment = getLogComment.comment;
                            approveDate = Convert.ToDateTime(getLogComment.createdOn).ToString("dd MMMM yyyy");
                        }
                        table1 = new PdfPTable(3);
                        width = new float[] { 1f, 2f, 1f };
                        table1.WidthPercentage = 100;
                        table1.SetWidths(width);
                        table1.HorizontalAlignment = Element.ALIGN_LEFT;
                        table1.DefaultCell.Border = 0;
                        table1.SpacingBefore = 0f;
                        table1.SpacingAfter = 0f;

                        cell1 = new PdfPCell();
                        cell1.Rowspan = 2;
                        cell1.BorderWidthLeft = 1f;
                        cell1.BorderWidthRight = 1f;
                        cell1.BorderWidthTop = 0f;
                        cell1.BorderWidthBottom = 1f;
                        cell1.MinimumHeight = 120f;
                        if (letterDetail.letter.statusCode == 2)
                        {
                            paragraph1 = new Paragraph("Belum diputuskan / Undecided \n", normalDeliberation);
                        }
                        else if (letterDetail.letter.statusCode == 5)
                        {
                            paragraph1 = new Paragraph("Ditolak / Reject \n", normalDeliberation);
                        }
                        else
                        {
                            if (item.idLevelChecker == 2 ||item.idLevelChecker == 1)
                            {
                                paragraph1 = new Paragraph("Disetujui / Approved \n", normalDeliberation);
                            }
                            else
                            {
                                paragraph1 = new Paragraph("Mengetahui / Understand \n", normalDeliberation);

                            }
                        }

                        paragraph1.Alignment = Element.ALIGN_LEFT;
                        cell1.AddElement(paragraph1);
                        paragraph1 = new Paragraph(item.positionName, normalDeliberation);
                        paragraph1.Alignment = Element.ALIGN_LEFT;
                        cell1.AddElement(paragraph1);
                        table1.AddCell(cell1);

                        cell1 = new PdfPCell();
                        cell1.Rowspan = 2;
                        cell1.BorderWidthLeft = 0f;
                        cell1.BorderWidthRight = 1f;
                        cell1.BorderWidthTop = 0f;
                        cell1.BorderWidthBottom = 1f;
                        cell1.MinimumHeight = 120f;
                        paragraph1 = new Paragraph("Catatan / Notes : \n", underlineDeliberation);
                        paragraph1.Alignment = Element.ALIGN_LEFT;
                        cell1.AddElement(paragraph1);
                        paragraph1 = new Paragraph(comment, normalDeliberation);
                        paragraph1.Alignment = Element.ALIGN_LEFT;
                        cell1.AddElement(paragraph1);
                        table1.AddCell(cell1);

                        cell1 = new PdfPCell();
                        cell1.BorderWidthLeft = 0f;
                        cell1.BorderWidthRight = 1f;
                        cell1.BorderWidthTop = 0f;
                        cell1.BorderWidthBottom = 1f;
                        cell1.MinimumHeight = 40f;
                        paragraph1 = new Paragraph("Tanggal : ", underlineDeliberation);
                        paragraph1.Alignment = Element.ALIGN_LEFT;
                        //cell1.AddElement(paragraph);
                        paragraph1 = new Paragraph(approveDate, normalDeliberation);
                        paragraph1.Alignment = Element.ALIGN_LEFT;
                        cell1.AddElement(paragraph1);
                        table1.AddCell(cell1);

                        cell1 = new PdfPCell();
                        cell1.BorderWidthLeft = 0f;
                        cell1.BorderWidthRight = 1f;
                        cell1.BorderWidthTop = 0f;
                        cell1.BorderWidthBottom = 1f;
                        paragraph1 = new Paragraph("Tanda Tangan : \n", underlineDeliberation);
                        paragraph1.Alignment = Element.ALIGN_LEFT;
                        cell1.AddElement(paragraph1);
                        if (letterDetail.letter.letterNumber != "NO_LETTER")
                        {
                            if (letterDetail.letter.signatureType == 1)
                            {
                                Image imagettd = GetSignatureImage(item.nip);
                                //cell.AddElement(imagettd);
                                if (imagettd != null)
                                {
                                    cell1.AddElement(imagettd);
                                }
                                else
                                {
                                    var img = "File tanda tangan tidak ditemukan";
                                    paragraph1 = new Paragraph(img, normal);
                                    cell1.AddElement(paragraph1);
                                }
                            }
                            else if (letterDetail.letter.signatureType == 2)
                            {

                                Image QRCodeSignatureApprover = GetBarcodeSignature("NIP : " + item.nip + "\nNAMA : " + item.fullname + "\nJabatan : " + item.positionName);
                                cell1.AddElement(QRCodeSignatureApprover);
                            }
                            else if (letterDetail.letter.signatureType == 3)
                            {
                                var img = "";
                                paragraph = new Paragraph(img, normal);
                                cell.AddElement(paragraph);
                                Paragraph linessia = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(2F, 100.0F, colorWhite, Element.ALIGN_CENTER, 1)));
                                linessia.SpacingBefore = 0f;
                                linessia.SetLeading(0.5F, 0.5F);
                                pdfDoc.Add(linessia);

                                Paragraph linessiaa = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(2F, 100.0F, colorWhite, Element.ALIGN_CENTER, 1)));
                                linessiaa.SpacingBefore = 0f;
                                linessiaa.SetLeading(0.5F, 0.5F);
                                pdfDoc.Add(linessiaa);

                                Paragraph linessiaaa = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(2F, 100.0F, colorWhite, Element.ALIGN_CENTER, 1)));
                                linessiaaa.SpacingBefore = 0f;
                                linessiaaa.SetLeading(0.5F, 0.5F);
                                pdfDoc.Add(linessiaaa);
                            }
                            else
                            {
                                //innertable1 = new PdfPtable1(2);
                                //float[] widthinner = new float[] { 0.3f, 0.3f };
                                //innertable1.WidthPercentage = 100;
                                //innertable1.SetWidths(widthinner);
                                //innertable1.HorizontalAlignment = Element.ALIGN_LEFT;
                                //innertable1.DefaultCell.Border = 1;
                                //innertable1.DefaultCell.PaddingBottom = 8;

                                //innercell = new PdfPCell();
                                //innercell.BorderWidthLeft = 1f;
                                //innercell.BorderWidthRight = 1f;
                                //innercell.BorderWidthTop = 1f;
                                //innercell.BorderWidthBottom = 1f;




                                Image imagettd = GetSignatureImage(item.nip);
                                cell1.AddElement(imagettd);

                                Image QRCodeSignatureApprover = GetBarcodeSignature("NIP : " + item.nip + "\nNAMA : " + item.fullname + "\nJabatan : " + item.positionName);
                                cell1.AddElement(QRCodeSignatureApprover);
                                //if (imagettd !=null)
                                //{
                                //    cell.AddElement(imagettd);

                                //    Image QRCodeSignatureApprover = GetBarcodeSignature("NIP : " + item.nip + "\nNAMA : " + item.fullname + "\nJabatan : " + item.positionName);
                                //    cell.AddElement(QRCodeSignatureApprover);
                                //}
                                //else
                                //{
                                //    var img = "File tanda tangan tidak di temukan";
                                //    paragraph = new Paragraph(img, normal);

                                //    cell.AddElement(paragraph);
                                //    Image QRCodeSignatureApprover = GetBarcodeSignature("NIP : " + item.nip + "\nNAMA : " + item.fullname + "\nJabatan : " + item.positionName);
                                //    cell.AddElement(QRCodeSignatureApprover);
                                //}

                            }
                        }

                        paragraph1 = new Paragraph(item.fullname, bold);
                        paragraph1.Alignment = Element.ALIGN_CENTER;
                        cell1.AddElement(paragraph1);
                        table1.AddCell(cell1);


                        pdfDoc.Add(table1);
                    }


                    #endregion

                    pdfDoc.Close();

                    var dateLetterFormat = Convert.ToDateTime(letterDetail.letter.letterDate).ToString("dd MMMM yyyy");
                    var letterNumber = letterDetail.letter.letterNumber;
                    byte[] bytess = ms.ToArray();
                    byte[] bytes = AddPageNumbers(bytess, dateLetterFormat, letterNumber, "Memo");
                    ms.Close();

                    return File(bytes.ToArray(), "application/pdf", fileName);

                }
                
                #endregion
            }

            // Done (Masi Ikuti Template Memo Direksi)
            private FileContentResult PrivSuratKuasaOperasional(string letternumber, OutputDetailMemo letterDetail, string fileName)
            {
                #region Surat Kuasa Operasional
                    StringBuilder sb = new StringBuilder();
                    sb.Append("<div style='padding-left:2px;'>" + letterDetail.letterContent.letterContent + "</div>");
                    StringReader sr = new StringReader(sb.ToString());
                    //Document pdfDoc = new Document(PageSize.A4, 25, 25, 25, 15);
                    //Document pdfDoc = new Document(PageSize.A4, 56, 56, 113, 97);
                    Document pdfDoc = new Document(PageSize.A4, 56.7f, 56.7f, 66.5f, 99.225f);
                    HTMLWorker htmlparser = new HTMLWorker(pdfDoc);


                    using (MemoryStream ms = new MemoryStream())
                    {
                            PdfWriter pdfWriter = PdfWriter.GetInstance(pdfDoc, ms);
                            pdfDoc.Open();
                            //Font
                            int no;
                            var fontName = "Calibri";
                            string fontPath = Path.Combine(_environment.WebRootPath, "fonts/Calibri.ttf");
                            //FontFactory.Register(fontPath);

                            Font normal = FontFactory.GetFont(fontName, 12, Font.NORMAL, BaseColor.BLACK);
                            Font bold = FontFactory.GetFont(fontName, 12, Font.BOLD, BaseColor.BLACK);
                            Font underline = FontFactory.GetFont(fontName, 12, Font.UNDERLINE, BaseColor.BLACK);
                            Font underlineBold = FontFactory.GetFont(fontName, 12, Font.BOLD | Font.UNDERLINE, BaseColor.BLACK);

                            Font normalDeliberation = FontFactory.GetFont(fontName, 10, Font.NORMAL, BaseColor.BLACK);
                            Font underlineDeliberation = FontFactory.GetFont(fontName, 10, Font.UNDERLINE, BaseColor.BLACK);
                            Font underlineDeliberationBold = FontFactory.GetFont(fontName, 10, Font.BOLD | Font.UNDERLINE, BaseColor.BLACK);
                            // Colors
                            BaseColor colorBlack = new BaseColor(System.Drawing.ColorTranslator.FromHtml("#000000"));
                            BaseColor colorWhite = new BaseColor(System.Drawing.ColorTranslator.FromHtml("#FFFFFF"));



                            PdfPTable table = new PdfPTable(2);
                            float[] width = new float[] { 0.667f, 2f };
                            table.WidthPercentage = 100;
                            table.SetWidths(width);
                            table.HorizontalAlignment = Element.ALIGN_LEFT;
                            table.DefaultCell.Border = 0;
                            table.SpacingBefore = 0f;
                            table.SpacingAfter = 0f;

                            PdfPCell cell = new PdfPCell();
                            Paragraph paragraph = new Paragraph();


                            #region header
                            //add new page

                            //table = new PdfPTable(1);
                            //width = new float[] { 2f };
                            //table.WidthPercentage = 100;
                            //table.SetWidths(width);
                            //table.HorizontalAlignment = Element.ALIGN_LEFT;
                            //table.DefaultCell.Border = 0;
                            //table.SpacingBefore = 00f;
                            //table.SpacingAfter = 0f;


                            //table.AddCell(new Phrase("MEMO", bold));
                            //pdfDoc.Add(table);

                            table = new PdfPTable(2);
                            width = new float[] { 0.2f, 1f };
                            table.WidthPercentage = 100;
                            table.SetWidths(width);
                            table.HorizontalAlignment = Element.ALIGN_RIGHT;
                            table.DefaultCell.Border = 0;
                            table.SpacingBefore = 0f;
                            table.SpacingAfter = 0f;

                            cell.Border = 0;
                            paragraph = new Paragraph("Nomor", normal);
                            paragraph.Alignment = Element.ALIGN_LEFT;
                            cell.AddElement(paragraph);
                            table.AddCell(cell);

                            //cell = new PdfPCell();
                            //cell.Border = 0;
                            //paragraph = new Paragraph(":", normal);
                            //paragraph.Alignment = Element.ALIGN_RIGHT;
                            //cell.AddElement(paragraph);
                            //table.AddCell(cell);

                            cell = new PdfPCell();
                            cell.Border = 0;
                            paragraph = new Paragraph(":"+" "+letterDetail.letter.letterNumber, normal);
                            paragraph.Alignment = Element.ALIGN_LEFT;
                            cell.AddElement(paragraph);
                            table.AddCell(cell);

                            cell = new PdfPCell();
                            cell.Border = 0;
                            paragraph = new Paragraph("Tanggal", normal);
                            paragraph.Alignment = Element.ALIGN_LEFT;
                            cell.AddElement(paragraph);
                            table.AddCell(cell);

                            //cell = new PdfPCell();
                            //cell.Border = 0;
                            //paragraph = new Paragraph(":", normal);
                            //paragraph.Alignment = Element.ALIGN_RIGHT;
                            //cell.AddElement(paragraph);
                            //table.AddCell(cell);

                            cell = new PdfPCell();
                            cell.Border = 0;
                            paragraph = new Paragraph(":"+" "+Convert.ToDateTime(letterDetail.letter.letterDate).ToString("dd MMMM yyyy"), normal);
                            paragraph.Alignment = Element.ALIGN_LEFT;
                            cell.AddElement(paragraph);
                            table.AddCell(cell);

                            cell = new PdfPCell();
                            cell.Border = 0;
                            paragraph = new Paragraph("Kepada", normal);
                            paragraph.Alignment = Element.ALIGN_LEFT;
                            cell.AddElement(paragraph);
                            table.AddCell(cell);

                            //cell = new PdfPCell();
                            //cell.Border = 0;
                            //paragraph = new Paragraph(":", normal);
                            //paragraph.Alignment = Element.ALIGN_RIGHT;
                            //cell.AddElement(paragraph);
                            //table.AddCell(cell);

                            cell = new PdfPCell();
                            cell.Border = 0;
                            no = 1;
                            foreach (var item in letterDetail.receiver)
                            {
                                if (letterDetail.receiver.Count() > 1)
                                {
                                    paragraph = new Paragraph(":"+" "+no + ". " + item.fullname + " - " + item.positionName, normal);
                                    paragraph.Alignment = Element.ALIGN_LEFT;
                                    cell.AddElement(paragraph);
                                }
                                else
                                {
                                    paragraph = new Paragraph(":"+" "+item.fullname + " - " + item.positionName, normal);
                                    paragraph.Alignment = Element.ALIGN_LEFT;
                                    cell.AddElement(paragraph);
                                }
                                no++;
                            }
                            table.AddCell(cell);

                            cell = new PdfPCell();
                            cell.Border = 0;
                            paragraph = new Paragraph("Tembusan", normal);
                            paragraph.Alignment = Element.ALIGN_LEFT;
                            cell.AddElement(paragraph);
                            table.AddCell(cell);

                            //cell = new PdfPCell();
                            //cell.Border = 0;
                            //paragraph = new Paragraph(":", normal);
                            //paragraph.Alignment = Element.ALIGN_RIGHT;
                            //cell.AddElement(paragraph);
                            //table.AddCell(cell);

                            cell = new PdfPCell();
                            cell.Border = 0;
                            no = 1;
                            foreach (var item in letterDetail.copy)
                            {
                                if (letterDetail.copy.Count() > 1)
                                {
                                    paragraph = new Paragraph(":"+" "+no + ". " + item.fullname + " - " + item.positionName, normal);
                                    paragraph.Alignment = Element.ALIGN_LEFT;
                                    cell.AddElement(paragraph);
                                }
                                else
                                {
                                    paragraph = new Paragraph(":"+" "+item.fullname + " - " + item.positionName, normal);
                                    paragraph.Alignment = Element.ALIGN_LEFT;
                                    cell.AddElement(paragraph);
                                }
                                no++;
                            }
                            if (letterDetail.copy.Count() == 0)
                            {
                                paragraph = new Paragraph("-", normal);
                                paragraph.Alignment = Element.ALIGN_LEFT;
                                cell.AddElement(paragraph);
                            }
                            table.AddCell(cell);

                            cell = new PdfPCell();
                            cell.Border = 0;
                            paragraph = new Paragraph("Dari", normal);
                            paragraph.Alignment = Element.ALIGN_LEFT;
                            cell.AddElement(paragraph);
                            table.AddCell(cell);

                            //cell = new PdfPCell();
                            //cell.Border = 0;
                            //paragraph = new Paragraph(":", normal);
                            //paragraph.Alignment = Element.ALIGN_RIGHT;
                            //cell.AddElement(paragraph);
                            //table.AddCell(cell);

                            cell = new PdfPCell();
                            cell.Border = 0;
                            paragraph = new Paragraph(":"+" "+letterDetail.sender[0].fullname + " - " + letterDetail.sender[0].positionName, normal);
                            paragraph.Alignment = Element.ALIGN_LEFT;
                            cell.AddElement(paragraph);
                            table.AddCell(cell);

                            cell = new PdfPCell();
                            cell.Border = 0;
                            paragraph = new Paragraph("Perihal", normal);
                            paragraph.Alignment = Element.ALIGN_LEFT;
                            cell.AddElement(paragraph);
                            table.AddCell(cell);

                            //cell = new PdfPCell();
                            //cell.Border = 0;
                            //paragraph = new Paragraph(":", normal);
                            //paragraph.Alignment = Element.ALIGN_RIGHT;
                            //cell.AddElement(paragraph);
                            //table.AddCell(cell);

                            cell = new PdfPCell();
                            cell.Border = 0;
                            paragraph = new Paragraph(":"+" "+letterDetail.letter.about, normal);
                            paragraph.Alignment = Element.ALIGN_LEFT;
                            cell.AddElement(paragraph);
                            table.AddCell(cell);

                            cell = new PdfPCell();
                            cell.Border = 0;
                            paragraph = new Paragraph("Lampiran", normal);
                            paragraph.Alignment = Element.ALIGN_LEFT;
                            cell.AddElement(paragraph);
                            table.AddCell(cell);

                            //cell = new PdfPCell();
                            //cell.Border = 0;
                            //paragraph = new Paragraph(":", normal);
                            //paragraph.Alignment = Element.ALIGN_RIGHT;
                            //cell.AddElement(paragraph);
                            //table.AddCell(cell);

                            var lampiran = letterDetail.letter.attachmentDesc == null ? "-" : letterDetail.letter.attachmentDesc;
                            cell = new PdfPCell();
                            cell.Border = 0;
                            paragraph = new Paragraph(":"+" "+lampiran, bold);
                            paragraph.Alignment = Element.ALIGN_LEFT;
                            cell.AddElement(paragraph);
                            table.AddCell(cell);

                            //Add table to document
                            pdfDoc.Add(table);


                            Paragraph line = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(2F, 100.0F, colorBlack, Element.ALIGN_CENTER, 1)));
                            line.SpacingBefore = 0f;
                            line.SetLeading(2F, 0.5F);
                            pdfDoc.Add(line);
                            Paragraph lines = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(2F, 100.0F, colorBlack, Element.ALIGN_CENTER, 1)));
                            lines.SpacingBefore = 0f;
                            line.SetLeading(0.5F, 0.5F);
                            pdfDoc.Add(line);

                            Paragraph liness = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(2F, 100.0F, colorWhite, Element.ALIGN_CENTER, 1)));
                            liness.SpacingBefore = 0f;
                            liness.SetLeading(0.5F, 0.5F);
                            pdfDoc.Add(liness);

                            //Paragraph linesss = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(2F, 100.0F, colorWhite, Element.ALIGN_CENTER, 1)));
                            //linesss.SpacingBefore = 0f;
                            //linesss.SetLeading(0.5F, 0.5F);
                            //pdfDoc.Add(linesss);


                            table = new PdfPTable(1);
                            width = new float[] { 2f };
                            table.WidthPercentage = 100;
                            table.SetWidths(width);
                            table.HorizontalAlignment = Element.ALIGN_LEFT;
                            table.DefaultCell.Border = 0;
                            table.SpacingBefore = 00f;
                            table.SpacingAfter = 0f;


                            //table.AddCell(new Phrase("Dengan hormat", normal));
                            //pdfDoc.Add(table);

                            Paragraph linessss = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(2F, 100.0F, colorWhite, Element.ALIGN_CENTER, 1)));
                            linessss.SpacingBefore = 0f;
                            linessss.SetLeading(0.5F, 0.5F);
                            pdfDoc.Add(linessss);

                            Paragraph lin = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(2F, 100.0F, colorWhite, Element.ALIGN_CENTER, 1)));
                            lin.SpacingBefore = 0f;
                            lin.SetLeading(0.5F, 0.5F);
                            pdfDoc.Add(lin);

                            #endregion

                            //ADD HTML CONTENT
                            htmlparser.Parse(sr);

                            
                            #region Tanda Tangan

                            Paragraph a = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(2F, 100.0F, colorWhite, Element.ALIGN_CENTER, 1)));
                            a.SpacingBefore = 0f;
                            a.SetLeading(0.5F, 0.5F);
                            pdfDoc.Add(a);



                            cell = new PdfPCell();
                            table.AddCell(new Phrase("", normal));

                            cell = new PdfPCell();
                            table.AddCell(new Phrase(" ", normal));
                            cell = new PdfPCell();
                            table.AddCell(new Phrase(" ", normal));
                            cell = new PdfPCell();
                            table.AddCell(new Phrase("Hormat kami, \n" + "PT BNI Life Insurance", normal));
                            //table.AddCell(new Phrase("Best Regards", normal));
                            pdfDoc.Add(table);



                            string filename = letterDetail.sender[0].nip;


                            // Tanda tangan
                            //var dirChecker = letterDetail.checker.OrderBy(p => p.idLevelChecker).ToList();
                            var dirChecker = letterDetail.checker.Where(p => p.idLevelChecker == 2 ).ToList();

                            table = new PdfPTable(dirChecker.Count());
                            width = new float[dirChecker.Count()];
                            for (int i = 0; i < dirChecker.Count(); i++)
                            {
                                width[i] = 1f;
                            }
                            table.WidthPercentage = 100;
                            table.SetWidths(width);
                            table.HorizontalAlignment = Element.ALIGN_LEFT;
                            table.DefaultCell.Border = 0;
                            var no1 = 0;


                            //cell = new PdfPCell();
                            //table.AddCell(new Phrase("Direksi PT BNI Life Insurance", normal));
                            foreach (var item in dirChecker)
                            {
                                cell = new PdfPCell();
                                cell.Border = 0;




                                if (letterDetail.letter.letterNumber != "NO_LETTER")
                                {
                                    if (letterDetail.letter.signatureType == 1)
                                    {

                                        Image imagettd = GetSignatureImage(item.nip);
                                        //cell.AddElement(imagettd);
                                        if (imagettd != null)
                                        {
                                            cell.AddElement(imagettd);
                                        }
                                        else
                                        {
                                            var img = "File tanda tangan tidak ditemukan";
                                            paragraph = new Paragraph(img, normal);
                                            cell.AddElement(paragraph);
                                        }

                                    }
                                    else if (letterDetail.letter.signatureType == 2)
                                    {


                                        Image QRCodeSignatureApprover = GetBarcodeSignature("NIP : " + item.nip + "\nNAMA : " + item.fullname + "\nJabatan : " + item.positionName);
                                        QRCodeSignatureApprover.ScaleAbsoluteHeight(10f);
                                        cell.AddElement(QRCodeSignatureApprover);
                                    }
                                    else if (letterDetail.letter.signatureType == 3)
                                    {
                                        var img = "";
                                        paragraph = new Paragraph(img, normal);
                                        cell.AddElement(paragraph);
                                        Paragraph linessia = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(2F, 100.0F, colorWhite, Element.ALIGN_CENTER, 1)));
                                        linessia.SpacingBefore = 0f;
                                        linessia.SetLeading(0.5F, 0.5F);
                                        pdfDoc.Add(linessia);

                                        Paragraph linessiaa = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(2F, 100.0F, colorWhite, Element.ALIGN_CENTER, 1)));
                                        linessiaa.SpacingBefore = 0f;
                                        linessiaa.SetLeading(0.5F, 0.5F);
                                        pdfDoc.Add(linessiaa);

                                        Paragraph linessiaaa = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(2F, 100.0F, colorWhite, Element.ALIGN_CENTER, 1)));
                                        linessiaaa.SpacingBefore = 0f;
                                        linessiaaa.SetLeading(0.5F, 0.5F);
                                        pdfDoc.Add(linessiaaa);
                                    }
                                    else
                                    {

                                        Image imagettd = GetSignatureImage(item.nip);
                                        cell.AddElement(imagettd);
                                        Image QRCodeSignatureApprover = GetBarcodeSignature("NIP : " + item.nip + "\nNAMA : " + item.fullname + "\nJabatan : " + item.positionName);
                                        cell.AddElement(QRCodeSignatureApprover);
                                        //if (imagettd != null)
                                        //{
                                        //    cell.AddElement(imagettd);
                                        //    Image QRCodeSignatureApprover = GetBarcodeSignature("NIP : " + item.nip + "\nNAMA : " + item.fullname + "\nJabatan : " + item.positionName);
                                        //    cell.AddElement(QRCodeSignatureApprover);
                                        //}
                                        //else
                                        //{
                                        //    var img = "File tanda tangan tidak di temukan";
                                        //    paragraph = new Paragraph(img, normal);
                                        //    cell.AddElement(paragraph);
                                        //    Image QRCodeSignatureApprover = GetBarcodeSignature("NIP : " + item.nip + "\nNAMA : " + item.fullname + "\nJabatan : " + item.positionName);
                                        //    cell.AddElement(QRCodeSignatureApprover);
                                        //}

                                    }
                                    no1++;
                                }

                                paragraph = new Paragraph(item.fullname, underlineBold);
                                cell.AddElement(paragraph);
                                paragraph = new Paragraph(item.positionName, bold);
                                cell.AddElement(paragraph);

                                table.AddCell(cell);
                            }
                            // end tanda tangan
                            pdfDoc.Add(table);

                            #endregion

                            pdfDoc.NewPage();

                            #region Lampiran

                            PdfPTable table1 = new PdfPTable(2);
                            float[] width1 = new float[] { 0.667f, 2f };
                            table1.WidthPercentage = 100;
                            table1.SetWidths(width1);
                            table1.HorizontalAlignment = Element.ALIGN_LEFT;
                            table1.DefaultCell.Border = 0;
                            table1.SpacingBefore = 0f;
                            table1.SpacingAfter = 0f;

                            PdfPCell cell1 = new PdfPCell();
                            Paragraph paragraph1 = new Paragraph();

                            cell1 = new PdfPCell();
                            cell1.BorderWidthLeft = 1f;
                            cell1.BorderWidthRight = 1f;
                            cell1.BorderWidthTop = 1f;
                            cell1.BorderWidthBottom = 1f;
                            cell1.MinimumHeight = 30f;
                            paragraph1 = new Paragraph("Nomor Registrasi Memo/Registration Number:" + letterDetail.letter.letterDeliberationNumber, normalDeliberation);
                            paragraph1.Alignment = Element.ALIGN_LEFT;
                            cell1.AddElement(paragraph1);
                            table1.AddCell(cell1);

                            cell1 = new PdfPCell();
                            cell1.BorderWidthLeft = 0f;
                            cell1.BorderWidthRight = 1f;
                            cell1.BorderWidthTop = 1f;
                            cell1.BorderWidthBottom = 1f;
                            cell1.MinimumHeight = 30f;
                            paragraph1 = new Paragraph("Tanggal Registrasi Memo/Date of Memo Registration : " + Convert.ToDateTime(letterDetail.letter.letterDate).ToString("dd MMMM yyyy"), normalDeliberation);
                            paragraph1.Alignment = Element.ALIGN_LEFT;
                            cell1.AddElement(paragraph1);
                            table1.AddCell(cell1);

                            cell1 = new PdfPCell();
                            cell1.BorderWidthLeft = 1f;
                            cell1.BorderWidthRight = 1f;
                            cell1.BorderWidthTop = 0f;
                            cell1.BorderWidthBottom = 1f;
                            cell1.MinimumHeight = 100f;
                            paragraph1 = new Paragraph("Nomor Memo : " + letterDetail.letter.letterNumber, normalDeliberation);
                            paragraph1.Alignment = Element.ALIGN_LEFT;
                            cell1.AddElement(paragraph1);
                            table1.AddCell(cell1);

                            cell1 = new PdfPCell();
                            cell1.BorderWidthLeft = 0f;
                            cell1.BorderWidthRight = 1f;
                            cell1.BorderWidthTop = 0f;
                            cell1.BorderWidthBottom = 1f;
                            cell1.MinimumHeight = 100f;
                            paragraph1 = new Paragraph("(Ringkasan Memo/Summary of the proposal) \n" + letterDetail.letterContent.summary, normalDeliberation);
                            paragraph1.Alignment = Element.ALIGN_LEFT;
                            cell1.AddElement(paragraph1);
                            table1.AddCell(cell1);

                            cell1 = new PdfPCell();
                            cell1.BorderWidthLeft = 1f;
                            cell1.BorderWidthRight = 1f;
                            cell1.BorderWidthTop = 0f;
                            cell1.BorderWidthBottom = 1f;
                            cell1.MinimumHeight = 100f;
                            paragraph1 = new Paragraph("Deliberation:", normalDeliberation);
                            paragraph1.Alignment = Element.ALIGN_LEFT;
                            cell1.AddElement(paragraph1);
                            no = 1;
                            foreach (var item in letterDetail.delibration)
                            {
                                if (letterDetail.receiver.Count() > 1)
                                {
                                    paragraph1 = new Paragraph(no + ". " + item.fullname + " - " + item.positionName, normalDeliberation);
                                    paragraph1.Alignment = Element.ALIGN_LEFT;
                                    cell1.AddElement(paragraph1);
                                }
                                else
                                {
                                    paragraph1 = new Paragraph(item.fullname + " - " + item.positionName, normalDeliberation);
                                    paragraph1.Alignment = Element.ALIGN_LEFT;
                                    cell1.AddElement(paragraph1);
                                }
                                no++;
                            }
                            table1.AddCell(cell1);

                            cell1 = new PdfPCell();
                            cell1.BorderWidthLeft = 0f;
                            cell1.BorderWidthRight = 1f;
                            cell1.BorderWidthTop = 0f;
                            cell1.BorderWidthBottom = 1f;
                            cell1.MinimumHeight = 100f;
                            paragraph1 = new Paragraph("Catatan / Notes : \n", underlineDeliberation);
                            paragraph1.Alignment = Element.ALIGN_LEFT;
                            cell1.AddElement(paragraph1);
                            no = 1;
                            foreach (var item in letterDetail.delibration)
                            {
                                if (letterDetail.receiver.Count() > 1)
                                {
                                    paragraph1 = new Paragraph(no + ". " + item.commentdlbrt, normalDeliberation);
                                    paragraph1.Alignment = Element.ALIGN_LEFT;
                                    cell1.AddElement(paragraph1);
                                }
                                else
                                {
                                    paragraph1 = new Paragraph(item.commentdlbrt, normalDeliberation);
                                    paragraph1.Alignment = Element.ALIGN_LEFT;
                                    cell1.AddElement(paragraph1);
                                }
                                no++;
                            }
                            table1.AddCell(cell1);

                            pdfDoc.Add(table1);

                            //var checkerList = letterDetail.checker.Where(p => p.ordernumber != 1).OrderBy(p => p.idLevelChecker).ToList();
                            var checkerList = letterDetail.checker.OrderBy(p => p.idLevelChecker).ToList();
                            foreach (var item in checkerList)
                            {
                                var getLogComment = letterDetail.log.Where(p => p.idUserLog == item.idUserChecker && p.description.Contains("pemeriksa ke")).OrderByDescending(p => p.createdOn).FirstOrDefault();
                                var comment = "";
                                string approveDate = "";
                                if (getLogComment != null)
                                {
                                    comment = getLogComment.comment;
                                    approveDate = Convert.ToDateTime(getLogComment.createdOn).ToString("dd MMMM yyyy");
                                }
                                table1 = new PdfPTable(3);
                                width = new float[] { 1f, 2f, 1f };
                                table1.WidthPercentage = 100;
                                table1.SetWidths(width);
                                table1.HorizontalAlignment = Element.ALIGN_LEFT;
                                table1.DefaultCell.Border = 0;
                                table1.SpacingBefore = 0f;
                                table1.SpacingAfter = 0f;

                                cell1 = new PdfPCell();
                                cell1.Rowspan = 2;
                                cell1.BorderWidthLeft = 1f;
                                cell1.BorderWidthRight = 1f;
                                cell1.BorderWidthTop = 0f;
                                cell1.BorderWidthBottom = 1f;
                                cell1.MinimumHeight = 120f;
                                if (letterDetail.letter.statusCode == 2)
                                {
                                    paragraph1 = new Paragraph("Belum diputuskan / Undecided \n", normalDeliberation);
                                }
                                else if (letterDetail.letter.statusCode == 5)
                                {
                                    paragraph1 = new Paragraph("Ditolak / Reject \n", normalDeliberation);
                                }
                                else
                                {
                                   if(item.idLevelChecker == 2 ||item.idLevelChecker == 1)
                                   { 
                                       paragraph1 = new Paragraph("Disetujui / Approved \n", normalDeliberation);
                                   }
                                   else
                                   {
                                       paragraph1 = new Paragraph("Mengetahui / Understand \n", normalDeliberation);
                                       
                                   }
                                }

                                paragraph1.Alignment = Element.ALIGN_LEFT;
                                cell1.AddElement(paragraph1);
                                paragraph1 = new Paragraph(item.positionName, normalDeliberation);
                                paragraph1.Alignment = Element.ALIGN_LEFT;
                                cell1.AddElement(paragraph1);
                                table1.AddCell(cell1);

                                cell1 = new PdfPCell();
                                cell1.Rowspan = 2;
                                cell1.BorderWidthLeft = 0f;
                                cell1.BorderWidthRight = 1f;
                                cell1.BorderWidthTop = 0f;
                                cell1.BorderWidthBottom = 1f;
                                cell1.MinimumHeight = 120f;
                                paragraph1 = new Paragraph("Catatan / Notes : \n", underlineDeliberation);
                                paragraph1.Alignment = Element.ALIGN_LEFT;
                                cell1.AddElement(paragraph1);
                                paragraph1 = new Paragraph(comment, normalDeliberation);
                                paragraph1.Alignment = Element.ALIGN_LEFT;
                                cell1.AddElement(paragraph1);
                                table1.AddCell(cell1);

                                cell1 = new PdfPCell();
                                cell1.BorderWidthLeft = 0f;
                                cell1.BorderWidthRight = 1f;
                                cell1.BorderWidthTop = 0f;
                                cell1.BorderWidthBottom = 1f;
                                cell1.MinimumHeight = 40f;
                                paragraph1 = new Paragraph("Tanggal : ", underlineDeliberation);
                                paragraph1.Alignment = Element.ALIGN_LEFT;
                                //cell1.AddElement(paragraph);
                                paragraph1 = new Paragraph(approveDate, normalDeliberation);
                                paragraph1.Alignment = Element.ALIGN_LEFT;
                                cell1.AddElement(paragraph1);
                                table1.AddCell(cell1);

                                cell1 = new PdfPCell();
                                cell1.BorderWidthLeft = 0f;
                                cell1.BorderWidthRight = 1f;
                                cell1.BorderWidthTop = 0f;
                                cell1.BorderWidthBottom = 1f;
                                paragraph1 = new Paragraph("Tanda Tangan : \n", underlineDeliberation);
                                paragraph1.Alignment = Element.ALIGN_LEFT;
                                cell1.AddElement(paragraph1);
                                if (letterDetail.letter.letterNumber != "NO_LETTER")
                                {
                                    if (letterDetail.letter.signatureType == 1)
                                    {
                                        Image imagettd = GetSignatureImage(item.nip);
                                        //cell.AddElement(imagettd);
                                        if (imagettd != null)
                                        {
                                            cell1.AddElement(imagettd);
                                        }
                                        else
                                        {
                                            var img = "File tanda tangan tidak ditemukan";
                                            paragraph1 = new Paragraph(img, normal);
                                            cell1.AddElement(paragraph1);
                                        }
                                    }
                                    else if (letterDetail.letter.signatureType == 2)
                                    {

                                        Image QRCodeSignatureApprover = GetBarcodeSignature("NIP : " + item.nip + "\nNAMA : " + item.fullname + "\nJabatan : " + item.positionName);
                                        cell1.AddElement(QRCodeSignatureApprover);
                                    }
                                    else if (letterDetail.letter.signatureType == 3)
                                    {
                                        var img = "";
                                        paragraph = new Paragraph(img, normal);
                                        cell.AddElement(paragraph);
                                        Paragraph linessia = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(2F, 100.0F, colorWhite, Element.ALIGN_CENTER, 1)));
                                        linessia.SpacingBefore = 0f;
                                        linessia.SetLeading(0.5F, 0.5F);
                                        pdfDoc.Add(linessia);

                                        Paragraph linessiaa = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(2F, 100.0F, colorWhite, Element.ALIGN_CENTER, 1)));
                                        linessiaa.SpacingBefore = 0f;
                                        linessiaa.SetLeading(0.5F, 0.5F);
                                        pdfDoc.Add(linessiaa);

                                        Paragraph linessiaaa = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(2F, 100.0F, colorWhite, Element.ALIGN_CENTER, 1)));
                                        linessiaaa.SpacingBefore = 0f;
                                        linessiaaa.SetLeading(0.5F, 0.5F);
                                        pdfDoc.Add(linessiaaa);
                                    }
                                    else
                                    {
                                        //innertable1 = new PdfPtable1(2);
                                        //float[] widthinner = new float[] { 0.3f, 0.3f };
                                        //innertable1.WidthPercentage = 100;
                                        //innertable1.SetWidths(widthinner);
                                        //innertable1.HorizontalAlignment = Element.ALIGN_LEFT;
                                        //innertable1.DefaultCell.Border = 1;
                                        //innertable1.DefaultCell.PaddingBottom = 8;

                                        //innercell = new PdfPCell();
                                        //innercell.BorderWidthLeft = 1f;
                                        //innercell.BorderWidthRight = 1f;
                                        //innercell.BorderWidthTop = 1f;
                                        //innercell.BorderWidthBottom = 1f;




                                        Image imagettd = GetSignatureImage(item.nip);
                                        cell1.AddElement(imagettd);

                                        Image QRCodeSignatureApprover = GetBarcodeSignature("NIP : " + item.nip + "\nNAMA : " + item.fullname + "\nJabatan : " + item.positionName);
                                        cell1.AddElement(QRCodeSignatureApprover);
                                        //if (imagettd !=null)
                                        //{
                                        //    cell.AddElement(imagettd);

                                        //    Image QRCodeSignatureApprover = GetBarcodeSignature("NIP : " + item.nip + "\nNAMA : " + item.fullname + "\nJabatan : " + item.positionName);
                                        //    cell.AddElement(QRCodeSignatureApprover);
                                        //}
                                        //else
                                        //{
                                        //    var img = "File tanda tangan tidak di temukan";
                                        //    paragraph = new Paragraph(img, normal);

                                        //    cell.AddElement(paragraph);
                                        //    Image QRCodeSignatureApprover = GetBarcodeSignature("NIP : " + item.nip + "\nNAMA : " + item.fullname + "\nJabatan : " + item.positionName);
                                        //    cell.AddElement(QRCodeSignatureApprover);
                                        //}

                                    }
                                }

                                paragraph1 = new Paragraph(item.fullname, bold);
                                paragraph1.Alignment = Element.ALIGN_CENTER;
                                cell1.AddElement(paragraph1);
                                table1.AddCell(cell1);


                                pdfDoc.Add(table1);
                            }


                            #endregion

                            pdfDoc.Close();

                            var dateLetterFormat = Convert.ToDateTime(letterDetail.letter.letterDate).ToString("dd MMMM yyyy");
                            var letterNumber = letterDetail.letter.letterNumber;
                            byte[] bytess = ms.ToArray();
                            byte[] bytes = AddPageNumbers(bytess, dateLetterFormat, letterNumber, "Memo");
                            ms.Close();

                            return File(bytes.ToArray(), "application/pdf", fileName);
                    }

                #endregion
            }

        #endregion

        #region Print Dewan Komisaris

            //Done(Masi Ikuti Template Memo Direksi)
            private FileContentResult PrivSuratKuasaDewanKom(string letternumber, OutputDetailMemo letterDetail, string fileName)
            {

                #region Surat Kuasa Operasional

                    StringBuilder sb = new StringBuilder();
                    sb.Append("<div style='padding-left:2px;'>" + letterDetail.letterContent.letterContent + "</div>");
                    StringReader sr = new StringReader(sb.ToString());
                    //Document pdfDoc = new Document(PageSize.A4, 25, 25, 25, 15);
                    //Document pdfDoc = new Document(PageSize.A4, 56, 56, 113, 97);
                    Document pdfDoc = new Document(PageSize.A4, 56.7f, 56.7f, 66.5f, 99.225f);
                    HTMLWorker htmlparser = new HTMLWorker(pdfDoc);

                    using (MemoryStream ms = new MemoryStream())
                    {
                        PdfWriter pdfWriter = PdfWriter.GetInstance(pdfDoc, ms);
                        pdfDoc.Open();
                        //Font
                        int no;
                        var fontName = "Calibri";
                        string fontPath = Path.Combine(_environment.WebRootPath, "fonts/Calibri.ttf");
                        //FontFactory.Register(fontPath);

                        Font normal = FontFactory.GetFont(fontName, 12, Font.NORMAL, BaseColor.BLACK);
                        Font bold = FontFactory.GetFont(fontName, 12, Font.BOLD, BaseColor.BLACK);
                        Font underline = FontFactory.GetFont(fontName, 12, Font.UNDERLINE, BaseColor.BLACK);
                        Font underlineBold = FontFactory.GetFont(fontName, 12, Font.BOLD | Font.UNDERLINE, BaseColor.BLACK);

                        Font normalDeliberation = FontFactory.GetFont(fontName, 10, Font.NORMAL, BaseColor.BLACK);
                        Font underlineDeliberation = FontFactory.GetFont(fontName, 10, Font.UNDERLINE, BaseColor.BLACK);
                        Font underlineDeliberationBold = FontFactory.GetFont(fontName, 10, Font.BOLD | Font.UNDERLINE, BaseColor.BLACK);
                        // Colors
                        BaseColor colorBlack = new BaseColor(System.Drawing.ColorTranslator.FromHtml("#000000"));
                        BaseColor colorWhite = new BaseColor(System.Drawing.ColorTranslator.FromHtml("#FFFFFF"));



                        PdfPTable table = new PdfPTable(2);
                        float[] width = new float[] { 0.667f, 2f };
                        table.WidthPercentage = 100;
                        table.SetWidths(width);
                        table.HorizontalAlignment = Element.ALIGN_LEFT;
                        table.DefaultCell.Border = 0;
                        table.SpacingBefore = 0f;
                        table.SpacingAfter = 0f;

                        PdfPCell cell = new PdfPCell();
                        Paragraph paragraph = new Paragraph();


                        #region header
                        //add new page

                        //table = new PdfPTable(1);
                        //width = new float[] { 2f };
                        //table.WidthPercentage = 100;
                        //table.SetWidths(width);
                        //table.HorizontalAlignment = Element.ALIGN_LEFT;
                        //table.DefaultCell.Border = 0;
                        //table.SpacingBefore = 00f;
                        //table.SpacingAfter = 0f;


                        //table.AddCell(new Phrase("MEMO", bold));
                        //pdfDoc.Add(table);

                        table = new PdfPTable(2);
                        width = new float[] { 0.2f, 1f };
                        table.WidthPercentage = 100;
                        table.SetWidths(width);
                        table.HorizontalAlignment = Element.ALIGN_RIGHT;
                        table.DefaultCell.Border = 0;
                        table.SpacingBefore = 0f;
                        table.SpacingAfter = 0f;

                        cell.Border = 0;
                        paragraph = new Paragraph("Nomor", normal);
                        paragraph.Alignment = Element.ALIGN_LEFT;
                        cell.AddElement(paragraph);
                        table.AddCell(cell);

                        //cell = new PdfPCell();
                        //cell.Border = 0;
                        //paragraph = new Paragraph(":", normal);
                        //paragraph.Alignment = Element.ALIGN_RIGHT;
                        //cell.AddElement(paragraph);
                        //table.AddCell(cell);

                        cell = new PdfPCell();
                        cell.Border = 0;
                        paragraph = new Paragraph(":"+" "+letterDetail.letter.letterNumber, normal);
                        paragraph.Alignment = Element.ALIGN_LEFT;
                        cell.AddElement(paragraph);
                        table.AddCell(cell);

                        cell = new PdfPCell();
                        cell.Border = 0;
                        paragraph = new Paragraph("Tanggal", normal);
                        paragraph.Alignment = Element.ALIGN_LEFT;
                        cell.AddElement(paragraph);
                        table.AddCell(cell);

                        //cell = new PdfPCell();
                        //cell.Border = 0;
                        //paragraph = new Paragraph(":", normal);
                        //paragraph.Alignment = Element.ALIGN_RIGHT;
                        //cell.AddElement(paragraph);
                        //table.AddCell(cell);

                        cell = new PdfPCell();
                        cell.Border = 0;
                        paragraph = new Paragraph(":"+" "+Convert.ToDateTime(letterDetail.letter.letterDate).ToString("dd MMMM yyyy"), normal);
                        paragraph.Alignment = Element.ALIGN_LEFT;
                        cell.AddElement(paragraph);
                        table.AddCell(cell);

                        cell = new PdfPCell();
                        cell.Border = 0;
                        paragraph = new Paragraph("Kepada", normal);
                        paragraph.Alignment = Element.ALIGN_LEFT;
                        cell.AddElement(paragraph);
                        table.AddCell(cell);

                        //cell = new PdfPCell();
                        //cell.Border = 0;
                        //paragraph = new Paragraph(":", normal);
                        //paragraph.Alignment = Element.ALIGN_RIGHT;
                        //cell.AddElement(paragraph);
                        //table.AddCell(cell);

                        cell = new PdfPCell();
                        cell.Border = 0;
                        no = 1;
                        foreach (var item in letterDetail.receiver)
                        {
                            if (letterDetail.receiver.Count() > 1)
                            {
                                paragraph = new Paragraph(":"+" "+no + ". " + item.fullname + " - " + item.positionName, normal);
                                paragraph.Alignment = Element.ALIGN_LEFT;
                                cell.AddElement(paragraph);
                            }
                            else
                            {
                                paragraph = new Paragraph(":"+" "+item.fullname + " - " + item.positionName, normal);
                                paragraph.Alignment = Element.ALIGN_LEFT;
                                cell.AddElement(paragraph);
                            }
                            no++;
                        }
                        table.AddCell(cell);

                        cell = new PdfPCell();
                        cell.Border = 0;
                        paragraph = new Paragraph("Tembusan", normal);
                        paragraph.Alignment = Element.ALIGN_LEFT;
                        cell.AddElement(paragraph);
                        table.AddCell(cell);

                        //cell = new PdfPCell();
                        //cell.Border = 0;
                        //paragraph = new Paragraph(":", normal);
                        //paragraph.Alignment = Element.ALIGN_RIGHT;
                        //cell.AddElement(paragraph);
                        //table.AddCell(cell);

                        cell = new PdfPCell();
                        cell.Border = 0;
                        no = 1;
                        foreach (var item in letterDetail.copy)
                        {
                            if (letterDetail.copy.Count() > 1)
                            {
                                paragraph = new Paragraph(":"+" "+no + ". " + item.fullname + " - " + item.positionName, normal);
                                paragraph.Alignment = Element.ALIGN_LEFT;
                                cell.AddElement(paragraph);
                            }
                            else
                            {
                                paragraph = new Paragraph(":"+" "+item.fullname + " - " + item.positionName, normal);
                                paragraph.Alignment = Element.ALIGN_LEFT;
                                cell.AddElement(paragraph);
                            }
                            no++;
                        }
                        if (letterDetail.copy.Count() == 0)
                        {
                            paragraph = new Paragraph("-", normal);
                            paragraph.Alignment = Element.ALIGN_LEFT;
                            cell.AddElement(paragraph);
                        }
                        table.AddCell(cell);

                        cell = new PdfPCell();
                        cell.Border = 0;
                        paragraph = new Paragraph("Dari", normal);
                        paragraph.Alignment = Element.ALIGN_LEFT;
                        cell.AddElement(paragraph);
                        table.AddCell(cell);

                        //cell = new PdfPCell();
                        //cell.Border = 0;
                        //paragraph = new Paragraph(":", normal);
                        //paragraph.Alignment = Element.ALIGN_RIGHT;
                        //cell.AddElement(paragraph);
                        //table.AddCell(cell);

                        cell = new PdfPCell();
                        cell.Border = 0;
                        paragraph = new Paragraph(":"+" "+letterDetail.sender[0].fullname + " - " + letterDetail.sender[0].positionName, normal);
                        paragraph.Alignment = Element.ALIGN_LEFT;
                        cell.AddElement(paragraph);
                        table.AddCell(cell);

                        cell = new PdfPCell();
                        cell.Border = 0;
                        paragraph = new Paragraph("Perihal", normal);
                        paragraph.Alignment = Element.ALIGN_LEFT;
                        cell.AddElement(paragraph);
                        table.AddCell(cell);

                        //cell = new PdfPCell();
                        //cell.Border = 0;
                        //paragraph = new Paragraph(":", normal);
                        //paragraph.Alignment = Element.ALIGN_RIGHT;
                        //cell.AddElement(paragraph);
                        //table.AddCell(cell);

                        cell = new PdfPCell();
                        cell.Border = 0;
                        paragraph = new Paragraph(":"+" "+letterDetail.letter.about, normal);
                        paragraph.Alignment = Element.ALIGN_LEFT;
                        cell.AddElement(paragraph);
                        table.AddCell(cell);

                        cell = new PdfPCell();
                        cell.Border = 0;
                        paragraph = new Paragraph("Lampiran", normal);
                        paragraph.Alignment = Element.ALIGN_LEFT;
                        cell.AddElement(paragraph);
                        table.AddCell(cell);

                        //cell = new PdfPCell();
                        //cell.Border = 0;
                        //paragraph = new Paragraph(":", normal);
                        //paragraph.Alignment = Element.ALIGN_RIGHT;
                        //cell.AddElement(paragraph);
                        //table.AddCell(cell);

                        var lampiran = letterDetail.letter.attachmentDesc == null ? "-" : letterDetail.letter.attachmentDesc;
                        cell = new PdfPCell();
                        cell.Border = 0;
                        paragraph = new Paragraph(":"+" "+lampiran, bold);
                        paragraph.Alignment = Element.ALIGN_LEFT;
                        cell.AddElement(paragraph);
                        table.AddCell(cell);

                        //Add table to document
                        pdfDoc.Add(table);


                        Paragraph line = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(2F, 100.0F, colorBlack, Element.ALIGN_CENTER, 1)));
                        line.SpacingBefore = 0f;
                        line.SetLeading(2F, 0.5F);
                        pdfDoc.Add(line);
                        Paragraph lines = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(2F, 100.0F, colorBlack, Element.ALIGN_CENTER, 1)));
                        lines.SpacingBefore = 0f;
                        line.SetLeading(0.5F, 0.5F);
                        pdfDoc.Add(line);

                        Paragraph liness = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(2F, 100.0F, colorWhite, Element.ALIGN_CENTER, 1)));
                        liness.SpacingBefore = 0f;
                        liness.SetLeading(0.5F, 0.5F);
                        pdfDoc.Add(liness);

                        //Paragraph linesss = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(2F, 100.0F, colorWhite, Element.ALIGN_CENTER, 1)));
                        //linesss.SpacingBefore = 0f;
                        //linesss.SetLeading(0.5F, 0.5F);
                        //pdfDoc.Add(linesss);


                        table = new PdfPTable(1);
                        width = new float[] { 2f };
                        table.WidthPercentage = 100;
                        table.SetWidths(width);
                        table.HorizontalAlignment = Element.ALIGN_LEFT;
                        table.DefaultCell.Border = 0;
                        table.SpacingBefore = 00f;
                        table.SpacingAfter = 0f;


                        //table.AddCell(new Phrase("Dengan hormat", normal));
                        //pdfDoc.Add(table);

                        Paragraph linessss = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(2F, 100.0F, colorWhite, Element.ALIGN_CENTER, 1)));
                        linessss.SpacingBefore = 0f;
                        linessss.SetLeading(0.5F, 0.5F);
                        pdfDoc.Add(linessss);

                        Paragraph lin = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(2F, 100.0F, colorWhite, Element.ALIGN_CENTER, 1)));
                        lin.SpacingBefore = 0f;
                        lin.SetLeading(0.5F, 0.5F);
                        pdfDoc.Add(lin);

                        #endregion

                        //ADD HTML CONTENT
                        htmlparser.Parse(sr);


                        #region Tanda Tangan

                        Paragraph a = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(2F, 100.0F, colorWhite, Element.ALIGN_CENTER, 1)));
                        a.SpacingBefore = 0f;
                        a.SetLeading(0.5F, 0.5F);
                        pdfDoc.Add(a);



                        cell = new PdfPCell();
                        table.AddCell(new Phrase("", normal));

                        cell = new PdfPCell();
                        table.AddCell(new Phrase(" ", normal));
                        cell = new PdfPCell();
                        table.AddCell(new Phrase(" ", normal));
                        cell = new PdfPCell();
                        table.AddCell(new Phrase("Hormat kami, \n" + "PT BNI Life Insurance", normal));
                        //table.AddCell(new Phrase("Best Regards", normal));
                        pdfDoc.Add(table);



                        string filename = letterDetail.sender[0].nip;


                        // Tanda tangan
                         var dirChecker = letterDetail.sender.OrderBy(p => p.idUserSender).ToList();
                         //var dirChecker = letterDetail.checker.Where(p =>  p.idLevelChecker == 1).ToList();

                        table = new PdfPTable(dirChecker.Count());
                        width = new float[dirChecker.Count()];
                        for (int i = 0; i < dirChecker.Count(); i++)
                        {
                            width[i] = 1f;
                        }
                        table.WidthPercentage = 100;
                        table.SetWidths(width);
                        table.HorizontalAlignment = Element.ALIGN_LEFT;
                        table.DefaultCell.Border = 0;
                        var no1 = 0;


                        //cell = new PdfPCell();
                        //table.AddCell(new Phrase("Direksi PT BNI Life Insurance", normal));
                        foreach (var item in dirChecker)
                        {
                            cell = new PdfPCell();
                            cell.Border = 0;




                            if (letterDetail.letter.letterNumber != "NO_LETTER")
                            {
                                if (letterDetail.letter.signatureType == 1)
                                {

                                    Image imagettd = GetSignatureImage(item.nip);
                                    //cell.AddElement(imagettd);
                                    if (imagettd != null)
                                    {
                                        cell.AddElement(imagettd);
                                    }
                                    else
                                    {
                                        var img = "File tanda tangan tidak ditemukan";
                                        paragraph = new Paragraph(img, normal);
                                        cell.AddElement(paragraph);
                                    }

                                }
                                else if (letterDetail.letter.signatureType == 2)
                                {


                                    Image QRCodeSignatureApprover = GetBarcodeSignature("NIP : " + item.nip + "\nNAMA : " + item.fullname + "\nJabatan : " + item.positionName);
                                    QRCodeSignatureApprover.ScaleAbsoluteHeight(10f);
                                    cell.AddElement(QRCodeSignatureApprover);
                                }
                                else if (letterDetail.letter.signatureType == 3)
                                {
                                    var img = "";
                                    paragraph = new Paragraph(img, normal);
                                    cell.AddElement(paragraph);
                                    Paragraph linessia = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(2F, 100.0F, colorWhite, Element.ALIGN_CENTER, 1)));
                                    linessia.SpacingBefore = 0f;
                                    linessia.SetLeading(0.5F, 0.5F);
                                    pdfDoc.Add(linessia);

                                    Paragraph linessiaa = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(2F, 100.0F, colorWhite, Element.ALIGN_CENTER, 1)));
                                    linessiaa.SpacingBefore = 0f;
                                    linessiaa.SetLeading(0.5F, 0.5F);
                                    pdfDoc.Add(linessiaa);

                                    Paragraph linessiaaa = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(2F, 100.0F, colorWhite, Element.ALIGN_CENTER, 1)));
                                    linessiaaa.SpacingBefore = 0f;
                                    linessiaaa.SetLeading(0.5F, 0.5F);
                                    pdfDoc.Add(linessiaaa);
                                }
                                else
                                {

                                    Image imagettd = GetSignatureImage(item.nip);
                                    cell.AddElement(imagettd);
                                    Image QRCodeSignatureApprover = GetBarcodeSignature("NIP : " + item.nip + "\nNAMA : " + item.fullname + "\nJabatan : " + item.positionName);
                                    cell.AddElement(QRCodeSignatureApprover);
                                    //if (imagettd != null)
                                    //{
                                    //    cell.AddElement(imagettd);
                                    //    Image QRCodeSignatureApprover = GetBarcodeSignature("NIP : " + item.nip + "\nNAMA : " + item.fullname + "\nJabatan : " + item.positionName);
                                    //    cell.AddElement(QRCodeSignatureApprover);
                                    //}
                                    //else
                                    //{
                                    //    var img = "File tanda tangan tidak di temukan";
                                    //    paragraph = new Paragraph(img, normal);
                                    //    cell.AddElement(paragraph);
                                    //    Image QRCodeSignatureApprover = GetBarcodeSignature("NIP : " + item.nip + "\nNAMA : " + item.fullname + "\nJabatan : " + item.positionName);
                                    //    cell.AddElement(QRCodeSignatureApprover);
                                    //}

                                }
                                no1++;
                            }

                            paragraph = new Paragraph(item.fullname, underlineBold);
                            cell.AddElement(paragraph);
                            paragraph = new Paragraph(item.positionName, bold);
                            cell.AddElement(paragraph);

                            table.AddCell(cell);
                        }
                        // end tanda tangan
                        pdfDoc.Add(table);

                        #endregion

                        pdfDoc.NewPage();

                        #region Lampiran

                        PdfPTable table1 = new PdfPTable(2);
                        float[] width1 = new float[] { 0.667f, 2f };
                        table1.WidthPercentage = 100;
                        table1.SetWidths(width1);
                        table1.HorizontalAlignment = Element.ALIGN_LEFT;
                        table1.DefaultCell.Border = 0;
                        table1.SpacingBefore = 0f;
                        table1.SpacingAfter = 0f;

                        PdfPCell cell1 = new PdfPCell();
                        Paragraph paragraph1 = new Paragraph();

                        cell1 = new PdfPCell();
                        cell1.BorderWidthLeft = 1f;
                        cell1.BorderWidthRight = 1f;
                        cell1.BorderWidthTop = 1f;
                        cell1.BorderWidthBottom = 1f;
                        cell1.MinimumHeight = 30f;
                        paragraph1 = new Paragraph("Nomor Registrasi Memo/Registration Number:" + letterDetail.letter.letterDeliberationNumber, normalDeliberation);
                        paragraph1.Alignment = Element.ALIGN_LEFT;
                        cell1.AddElement(paragraph1);
                        table1.AddCell(cell1);

                        cell1 = new PdfPCell();
                        cell1.BorderWidthLeft = 0f;
                        cell1.BorderWidthRight = 1f;
                        cell1.BorderWidthTop = 1f;
                        cell1.BorderWidthBottom = 1f;
                        cell1.MinimumHeight = 30f;
                        paragraph1 = new Paragraph("Tanggal Registrasi Memo/Date of Memo Registration : " + Convert.ToDateTime(letterDetail.letter.letterDate).ToString("dd MMMM yyyy"), normalDeliberation);
                        paragraph1.Alignment = Element.ALIGN_LEFT;
                        cell1.AddElement(paragraph1);
                        table1.AddCell(cell1);

                        cell1 = new PdfPCell();
                        cell1.BorderWidthLeft = 1f;
                        cell1.BorderWidthRight = 1f;
                        cell1.BorderWidthTop = 0f;
                        cell1.BorderWidthBottom = 1f;
                        cell1.MinimumHeight = 100f;
                        paragraph1 = new Paragraph("Nomor Memo : " + letterDetail.letter.letterNumber, normalDeliberation);
                        paragraph1.Alignment = Element.ALIGN_LEFT;
                        cell1.AddElement(paragraph1);
                        table1.AddCell(cell1);

                        cell1 = new PdfPCell();
                        cell1.BorderWidthLeft = 0f;
                        cell1.BorderWidthRight = 1f;
                        cell1.BorderWidthTop = 0f;
                        cell1.BorderWidthBottom = 1f;
                        cell1.MinimumHeight = 100f;
                        paragraph1 = new Paragraph("(Ringkasan Memo/Summary of the proposal) \n" + letterDetail.letterContent.summary, normalDeliberation);
                        paragraph1.Alignment = Element.ALIGN_LEFT;
                        cell1.AddElement(paragraph1);
                        table1.AddCell(cell1);

                        cell1 = new PdfPCell();
                        cell1.BorderWidthLeft = 1f;
                        cell1.BorderWidthRight = 1f;
                        cell1.BorderWidthTop = 0f;
                        cell1.BorderWidthBottom = 1f;
                        cell1.MinimumHeight = 100f;
                        paragraph1 = new Paragraph("Deliberation:", normalDeliberation);
                        paragraph1.Alignment = Element.ALIGN_LEFT;
                        cell1.AddElement(paragraph1);
                        no = 1;
                        foreach (var item in letterDetail.delibration)
                        {
                            if (letterDetail.receiver.Count() > 1)
                            {
                                paragraph1 = new Paragraph(no + ". " + item.fullname + " - " + item.positionName, normalDeliberation);
                                paragraph1.Alignment = Element.ALIGN_LEFT;
                                cell1.AddElement(paragraph1);
                            }
                            else
                            {
                                paragraph1 = new Paragraph(item.fullname + " - " + item.positionName, normalDeliberation);
                                paragraph1.Alignment = Element.ALIGN_LEFT;
                                cell1.AddElement(paragraph1);
                            }
                            no++;
                        }
                        table1.AddCell(cell1);

                        cell1 = new PdfPCell();
                        cell1.BorderWidthLeft = 0f;
                        cell1.BorderWidthRight = 1f;
                        cell1.BorderWidthTop = 0f;
                        cell1.BorderWidthBottom = 1f;
                        cell1.MinimumHeight = 100f;
                        paragraph1 = new Paragraph("Catatan / Notes : \n", underlineDeliberation);
                        paragraph1.Alignment = Element.ALIGN_LEFT;
                        cell1.AddElement(paragraph1);
                        no = 1;
                        foreach (var item in letterDetail.delibration)
                        {
                            if (letterDetail.receiver.Count() > 1)
                            {
                                paragraph1 = new Paragraph(no + ". " + item.commentdlbrt, normalDeliberation);
                                paragraph1.Alignment = Element.ALIGN_LEFT;
                                cell1.AddElement(paragraph1);
                            }
                            else
                            {
                                paragraph1 = new Paragraph(item.commentdlbrt, normalDeliberation);
                                paragraph1.Alignment = Element.ALIGN_LEFT;
                                cell1.AddElement(paragraph1);
                            }
                            no++;
                        }
                        table1.AddCell(cell1);

                        pdfDoc.Add(table1);

                        //var checkerList = letterDetail.checker.Where(p => p.ordernumber != 1).OrderBy(p => p.idLevelChecker).ToList();
                        var checkerList = letterDetail.checker.OrderBy(p => p.idLevelChecker).ToList();
                        foreach (var item in checkerList)
                        {
                            var getLogComment = letterDetail.log.Where(p => p.idUserLog == item.idUserChecker && p.description.Contains("pemeriksa ke")).OrderByDescending(p => p.createdOn).FirstOrDefault();
                            var comment = "";
                            string approveDate = "";
                            if (getLogComment != null)
                            {
                                comment = getLogComment.comment;
                                approveDate = Convert.ToDateTime(getLogComment.createdOn).ToString("dd MMMM yyyy");
                            }
                            table1 = new PdfPTable(3);
                            width = new float[] { 1f, 2f, 1f };
                            table1.WidthPercentage = 100;
                            table1.SetWidths(width);
                            table1.HorizontalAlignment = Element.ALIGN_LEFT;
                            table1.DefaultCell.Border = 0;
                            table1.SpacingBefore = 0f;
                            table1.SpacingAfter = 0f;

                            cell1 = new PdfPCell();
                            cell1.Rowspan = 2;
                            cell1.BorderWidthLeft = 1f;
                            cell1.BorderWidthRight = 1f;
                            cell1.BorderWidthTop = 0f;
                            cell1.BorderWidthBottom = 1f;
                            cell1.MinimumHeight = 120f;
                            if (letterDetail.letter.statusCode == 2)
                                {
                                    paragraph1 = new Paragraph("Belum diputuskan / Undecided \n", normalDeliberation);
                                }
                                else if (letterDetail.letter.statusCode == 5)
                                {
                                    paragraph1 = new Paragraph("Ditolak / Reject \n", normalDeliberation);
                                }
                                else
                                {
                                   if(item.idLevelChecker == 2 ||item.idLevelChecker == 1)
                                   { 
                                       paragraph1 = new Paragraph("Disetujui / Approved \n", normalDeliberation);
                                   }
                                   else
                                   {
                                       paragraph1 = new Paragraph("Mengetahui / Understand \n", normalDeliberation);
                                       
                                   }
                                }

                            paragraph1.Alignment = Element.ALIGN_LEFT;
                            cell1.AddElement(paragraph1);
                            paragraph1 = new Paragraph(item.positionName, normalDeliberation);
                            paragraph1.Alignment = Element.ALIGN_LEFT;
                            cell1.AddElement(paragraph1);
                            table1.AddCell(cell1);

                            cell1 = new PdfPCell();
                            cell1.Rowspan = 2;
                            cell1.BorderWidthLeft = 0f;
                            cell1.BorderWidthRight = 1f;
                            cell1.BorderWidthTop = 0f;
                            cell1.BorderWidthBottom = 1f;
                            cell1.MinimumHeight = 120f;
                            paragraph1 = new Paragraph("Catatan / Notes : \n", underlineDeliberation);
                            paragraph1.Alignment = Element.ALIGN_LEFT;
                            cell1.AddElement(paragraph1);
                            paragraph1 = new Paragraph(comment, normalDeliberation);
                            paragraph1.Alignment = Element.ALIGN_LEFT;
                            cell1.AddElement(paragraph1);
                            table1.AddCell(cell1);

                            cell1 = new PdfPCell();
                            cell1.BorderWidthLeft = 0f;
                            cell1.BorderWidthRight = 1f;
                            cell1.BorderWidthTop = 0f;
                            cell1.BorderWidthBottom = 1f;
                            cell1.MinimumHeight = 40f;
                            paragraph1 = new Paragraph("Tanggal : ", underlineDeliberation);
                            paragraph1.Alignment = Element.ALIGN_LEFT;
                            //cell1.AddElement(paragraph);
                            paragraph1 = new Paragraph(approveDate, normalDeliberation);
                            paragraph1.Alignment = Element.ALIGN_LEFT;
                            cell1.AddElement(paragraph1);
                            table1.AddCell(cell1);

                            cell1 = new PdfPCell();
                            cell1.BorderWidthLeft = 0f;
                            cell1.BorderWidthRight = 1f;
                            cell1.BorderWidthTop = 0f;
                            cell1.BorderWidthBottom = 1f;
                            paragraph1 = new Paragraph("Tanda Tangan : \n", underlineDeliberation);
                            paragraph1.Alignment = Element.ALIGN_LEFT;
                            cell1.AddElement(paragraph1);
                            if (letterDetail.letter.letterNumber != "NO_LETTER")
                            {
                                if (letterDetail.letter.signatureType == 1)
                                {
                                    Image imagettd = GetSignatureImage(item.nip);
                                    //cell.AddElement(imagettd);
                                    if (imagettd != null)
                                    {
                                        cell1.AddElement(imagettd);
                                    }
                                    else
                                    {
                                        var img = "File tanda tangan tidak ditemukan";
                                        paragraph1 = new Paragraph(img, normal);
                                        cell1.AddElement(paragraph1);
                                    }
                                }
                                else if (letterDetail.letter.signatureType == 2)
                                {

                                    Image QRCodeSignatureApprover = GetBarcodeSignature("NIP : " + item.nip + "\nNAMA : " + item.fullname + "\nJabatan : " + item.positionName);
                                    cell1.AddElement(QRCodeSignatureApprover);
                                }
                                else if (letterDetail.letter.signatureType == 3)
                                {
                                    var img = "";
                                    paragraph = new Paragraph(img, normal);
                                    cell.AddElement(paragraph);
                                    Paragraph linessia = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(2F, 100.0F, colorWhite, Element.ALIGN_CENTER, 1)));
                                    linessia.SpacingBefore = 0f;
                                    linessia.SetLeading(0.5F, 0.5F);
                                    pdfDoc.Add(linessia);

                                    Paragraph linessiaa = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(2F, 100.0F, colorWhite, Element.ALIGN_CENTER, 1)));
                                    linessiaa.SpacingBefore = 0f;
                                    linessiaa.SetLeading(0.5F, 0.5F);
                                    pdfDoc.Add(linessiaa);

                                    Paragraph linessiaaa = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(2F, 100.0F, colorWhite, Element.ALIGN_CENTER, 1)));
                                    linessiaaa.SpacingBefore = 0f;
                                    linessiaaa.SetLeading(0.5F, 0.5F);
                                    pdfDoc.Add(linessiaaa);
                                }
                                else
                                {
                                    //innertable1 = new PdfPtable1(2);
                                    //float[] widthinner = new float[] { 0.3f, 0.3f };
                                    //innertable1.WidthPercentage = 100;
                                    //innertable1.SetWidths(widthinner);
                                    //innertable1.HorizontalAlignment = Element.ALIGN_LEFT;
                                    //innertable1.DefaultCell.Border = 1;
                                    //innertable1.DefaultCell.PaddingBottom = 8;

                                    //innercell = new PdfPCell();
                                    //innercell.BorderWidthLeft = 1f;
                                    //innercell.BorderWidthRight = 1f;
                                    //innercell.BorderWidthTop = 1f;
                                    //innercell.BorderWidthBottom = 1f;




                                    Image imagettd = GetSignatureImage(item.nip);
                                    cell1.AddElement(imagettd);

                                    Image QRCodeSignatureApprover = GetBarcodeSignature("NIP : " + item.nip + "\nNAMA : " + item.fullname + "\nJabatan : " + item.positionName);
                                    cell1.AddElement(QRCodeSignatureApprover);
                                    //if (imagettd !=null)
                                    //{
                                    //    cell.AddElement(imagettd);

                                    //    Image QRCodeSignatureApprover = GetBarcodeSignature("NIP : " + item.nip + "\nNAMA : " + item.fullname + "\nJabatan : " + item.positionName);
                                    //    cell.AddElement(QRCodeSignatureApprover);
                                    //}
                                    //else
                                    //{
                                    //    var img = "File tanda tangan tidak di temukan";
                                    //    paragraph = new Paragraph(img, normal);

                                    //    cell.AddElement(paragraph);
                                    //    Image QRCodeSignatureApprover = GetBarcodeSignature("NIP : " + item.nip + "\nNAMA : " + item.fullname + "\nJabatan : " + item.positionName);
                                    //    cell.AddElement(QRCodeSignatureApprover);
                                    //}

                                }
                            }

                            paragraph1 = new Paragraph(item.fullname, bold);
                            paragraph1.Alignment = Element.ALIGN_CENTER;
                            cell1.AddElement(paragraph1);
                            table1.AddCell(cell1);


                            pdfDoc.Add(table1);
                        }


                        #endregion

                        pdfDoc.Close();

                        var dateLetterFormat = Convert.ToDateTime(letterDetail.letter.letterDate).ToString("dd MMMM yyyy");
                        var letterNumber = letterDetail.letter.letterNumber;
                        byte[] bytess = ms.ToArray();
                        byte[] bytes = AddPageNumbers(bytess, dateLetterFormat, letterNumber, "Memo");
                        ms.Close();

                        return File(bytes.ToArray(), "application/pdf", fileName);
                }

                #endregion

            }

            //Done(Masi Ikuti Template Memo Direksi)
            private FileContentResult PrivSuratPernyataanDewanKom(string letternumber, OutputDetailMemo letterDetail, string fileName)
            {

               #region Surat Kuasa Operasional

               StringBuilder sb = new StringBuilder();
               sb.Append("<div style='padding-left:2px;'>" + letterDetail.letterContent.letterContent + "</div>");
               StringReader sr = new StringReader(sb.ToString());
               //Document pdfDoc = new Document(PageSize.A4, 25, 25, 25, 15);
               //Document pdfDoc = new Document(PageSize.A4, 56, 56, 113, 97);
               Document pdfDoc = new Document(PageSize.A4, 56.7f, 56.7f, 66.5f, 99.225f);
               HTMLWorker htmlparser = new HTMLWorker(pdfDoc);


                    using (MemoryStream ms = new MemoryStream())
                    {
                        PdfWriter pdfWriter = PdfWriter.GetInstance(pdfDoc, ms);
                        pdfDoc.Open();
                        //Font
                        int no;
                        var fontName = "Calibri";
                        string fontPath = Path.Combine(_environment.WebRootPath, "fonts/Calibri.ttf");
                        //FontFactory.Register(fontPath);

                        Font normal = FontFactory.GetFont(fontName, 12, Font.NORMAL, BaseColor.BLACK);
                        Font bold = FontFactory.GetFont(fontName, 12, Font.BOLD, BaseColor.BLACK);
                        Font underline = FontFactory.GetFont(fontName, 12, Font.UNDERLINE, BaseColor.BLACK);
                        Font underlineBold = FontFactory.GetFont(fontName, 12, Font.BOLD | Font.UNDERLINE, BaseColor.BLACK);

                        Font normalDeliberation = FontFactory.GetFont(fontName, 10, Font.NORMAL, BaseColor.BLACK);
                        Font underlineDeliberation = FontFactory.GetFont(fontName, 10, Font.UNDERLINE, BaseColor.BLACK);
                        Font underlineDeliberationBold = FontFactory.GetFont(fontName, 10, Font.BOLD | Font.UNDERLINE, BaseColor.BLACK);
                        // Colors
                        BaseColor colorBlack = new BaseColor(System.Drawing.ColorTranslator.FromHtml("#000000"));
                        BaseColor colorWhite = new BaseColor(System.Drawing.ColorTranslator.FromHtml("#FFFFFF"));



                        PdfPTable table = new PdfPTable(2);
                        float[] width = new float[] { 0.667f, 2f };
                        table.WidthPercentage = 100;
                        table.SetWidths(width);
                        table.HorizontalAlignment = Element.ALIGN_LEFT;
                        table.DefaultCell.Border = 0;
                        table.SpacingBefore = 0f;
                        table.SpacingAfter = 0f;

                        PdfPCell cell = new PdfPCell();
                        Paragraph paragraph = new Paragraph();


                        #region header
                        //add new page

                        //table = new PdfPTable(1);
                        //width = new float[] { 2f };
                        //table.WidthPercentage = 100;
                        //table.SetWidths(width);
                        //table.HorizontalAlignment = Element.ALIGN_LEFT;
                        //table.DefaultCell.Border = 0;
                        //table.SpacingBefore = 00f;
                        //table.SpacingAfter = 0f;


                        //table.AddCell(new Phrase("MEMO", bold));
                        //pdfDoc.Add(table);

                        table = new PdfPTable(2);
                        width = new float[] { 0.2f, 1f };
                        table.WidthPercentage = 100;
                        table.SetWidths(width);
                        table.HorizontalAlignment = Element.ALIGN_RIGHT;
                        table.DefaultCell.Border = 0;
                        table.SpacingBefore = 0f;
                        table.SpacingAfter = 0f;

                        cell.Border = 0;
                        paragraph = new Paragraph("Nomor", normal);
                        paragraph.Alignment = Element.ALIGN_LEFT;
                        cell.AddElement(paragraph);
                        table.AddCell(cell);

                        //cell = new PdfPCell();
                        //cell.Border = 0;
                        //paragraph = new Paragraph(":", normal);
                        //paragraph.Alignment = Element.ALIGN_RIGHT;
                        //cell.AddElement(paragraph);
                        //table.AddCell(cell);

                        cell = new PdfPCell();
                        cell.Border = 0;
                        paragraph = new Paragraph(":"+" "+letterDetail.letter.letterNumber, normal);
                        paragraph.Alignment = Element.ALIGN_LEFT;
                        cell.AddElement(paragraph);
                        table.AddCell(cell);

                        cell = new PdfPCell();
                        cell.Border = 0;
                        paragraph = new Paragraph("Tanggal", normal);
                        paragraph.Alignment = Element.ALIGN_LEFT;
                        cell.AddElement(paragraph);
                        table.AddCell(cell);

                        //cell = new PdfPCell();
                        //cell.Border = 0;
                        //paragraph = new Paragraph(":", normal);
                        //paragraph.Alignment = Element.ALIGN_RIGHT;
                        //cell.AddElement(paragraph);
                        //table.AddCell(cell);

                        cell = new PdfPCell();
                        cell.Border = 0;
                        paragraph = new Paragraph(":"+" "+Convert.ToDateTime(letterDetail.letter.letterDate).ToString("dd MMMM yyyy"), normal);
                        paragraph.Alignment = Element.ALIGN_LEFT;
                        cell.AddElement(paragraph);
                        table.AddCell(cell);

                        cell = new PdfPCell();
                        cell.Border = 0;
                        paragraph = new Paragraph("Kepada", normal);
                        paragraph.Alignment = Element.ALIGN_LEFT;
                        cell.AddElement(paragraph);
                        table.AddCell(cell);

                        //cell = new PdfPCell();
                        //cell.Border = 0;
                        //paragraph = new Paragraph(":", normal);
                        //paragraph.Alignment = Element.ALIGN_RIGHT;
                        //cell.AddElement(paragraph);
                        //table.AddCell(cell);

                        cell = new PdfPCell();
                        cell.Border = 0;
                        no = 1;
                        foreach (var item in letterDetail.receiver)
                        {
                            if (letterDetail.receiver.Count() > 1)
                            {
                                paragraph = new Paragraph(":"+" "+no + ". " + item.fullname + " - " + item.positionName, normal);
                                paragraph.Alignment = Element.ALIGN_LEFT;
                                cell.AddElement(paragraph);
                            }
                            else
                            {
                                paragraph = new Paragraph(":"+" "+item.fullname + " - " + item.positionName, normal);
                                paragraph.Alignment = Element.ALIGN_LEFT;
                                cell.AddElement(paragraph);
                            }
                            no++;
                        }
                        table.AddCell(cell);

                        cell = new PdfPCell();
                        cell.Border = 0;
                        paragraph = new Paragraph("Tembusan", normal);
                        paragraph.Alignment = Element.ALIGN_LEFT;
                        cell.AddElement(paragraph);
                        table.AddCell(cell);

                        //cell = new PdfPCell();
                        //cell.Border = 0;
                        //paragraph = new Paragraph(":", normal);
                        //paragraph.Alignment = Element.ALIGN_RIGHT;
                        //cell.AddElement(paragraph);
                        //table.AddCell(cell);

                        cell = new PdfPCell();
                        cell.Border = 0;
                        no = 1;
                        foreach (var item in letterDetail.copy)
                        {
                            if (letterDetail.copy.Count() > 1)
                            {
                                paragraph = new Paragraph(":"+" "+no + ". " + item.fullname + " - " + item.positionName, normal);
                                paragraph.Alignment = Element.ALIGN_LEFT;
                                cell.AddElement(paragraph);
                            }
                            else
                            {
                                paragraph = new Paragraph(":"+" "+item.fullname + " - " + item.positionName, normal);
                                paragraph.Alignment = Element.ALIGN_LEFT;
                                cell.AddElement(paragraph);
                            }
                            no++;
                        }
                        if (letterDetail.copy.Count() == 0)
                        {
                            paragraph = new Paragraph("-", normal);
                            paragraph.Alignment = Element.ALIGN_LEFT;
                            cell.AddElement(paragraph);
                        }
                        table.AddCell(cell);

                        cell = new PdfPCell();
                        cell.Border = 0;
                        paragraph = new Paragraph("Dari", normal);
                        paragraph.Alignment = Element.ALIGN_LEFT;
                        cell.AddElement(paragraph);
                        table.AddCell(cell);

                        //cell = new PdfPCell();
                        //cell.Border = 0;
                        //paragraph = new Paragraph(":", normal);
                        //paragraph.Alignment = Element.ALIGN_RIGHT;
                        //cell.AddElement(paragraph);
                        //table.AddCell(cell);

                        cell = new PdfPCell();
                        cell.Border = 0;
                        paragraph = new Paragraph(":"+" "+letterDetail.sender[0].fullname + " - " + letterDetail.sender[0].positionName, normal);
                        paragraph.Alignment = Element.ALIGN_LEFT;
                        cell.AddElement(paragraph);
                        table.AddCell(cell);

                        cell = new PdfPCell();
                        cell.Border = 0;
                        paragraph = new Paragraph("Perihal", normal);
                        paragraph.Alignment = Element.ALIGN_LEFT;
                        cell.AddElement(paragraph);
                        table.AddCell(cell);

                        //cell = new PdfPCell();
                        //cell.Border = 0;
                        //paragraph = new Paragraph(":", normal);
                        //paragraph.Alignment = Element.ALIGN_RIGHT;
                        //cell.AddElement(paragraph);
                        //table.AddCell(cell);

                        cell = new PdfPCell();
                        cell.Border = 0;
                        paragraph = new Paragraph(":"+" "+letterDetail.letter.about, normal);
                        paragraph.Alignment = Element.ALIGN_LEFT;
                        cell.AddElement(paragraph);
                        table.AddCell(cell);

                        cell = new PdfPCell();
                        cell.Border = 0;
                        paragraph = new Paragraph("Lampiran", normal);
                        paragraph.Alignment = Element.ALIGN_LEFT;
                        cell.AddElement(paragraph);
                        table.AddCell(cell);

                        //cell = new PdfPCell();
                        //cell.Border = 0;
                        //paragraph = new Paragraph(":", normal);
                        //paragraph.Alignment = Element.ALIGN_RIGHT;
                        //cell.AddElement(paragraph);
                        //table.AddCell(cell);

                        var lampiran = letterDetail.letter.attachmentDesc == null ? "-" : letterDetail.letter.attachmentDesc;
                        cell = new PdfPCell();
                        cell.Border = 0;
                        paragraph = new Paragraph(":"+" "+lampiran, bold);
                        paragraph.Alignment = Element.ALIGN_LEFT;
                        cell.AddElement(paragraph);
                        table.AddCell(cell);

                        //Add table to document
                        pdfDoc.Add(table);

                        Paragraph linessss = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(2F, 100.0F, colorWhite, Element.ALIGN_CENTER, 1)));
                        linessss.SpacingBefore = 0f;
                        linessss.SetLeading(0.5F, 0.5F);
                        pdfDoc.Add(linessss);

                        //Paragraph lin = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(2F, 100.0F, colorWhite, Element.ALIGN_CENTER, 1)));
                        //lin.SpacingBefore = 0f;
                        //lin.SetLeading(0.5F, 0.5F);
                        //pdfDoc.Add(lin);

                        #endregion

                        //ADD HTML CONTENT
                        htmlparser.Parse(sr);


                        #region Tanda Tangan

                        Paragraph a = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(2F, 100.0F, colorWhite, Element.ALIGN_CENTER, 1)));
                        a.SpacingBefore = 0f;
                        a.SetLeading(0.5F, 0.5F);
                        pdfDoc.Add(a);



                        cell = new PdfPCell();
                        table.AddCell(new Phrase("", normal));

                        cell = new PdfPCell();
                        table.AddCell(new Phrase(" ", normal));
                        cell = new PdfPCell();
                        table.AddCell(new Phrase(" ", normal));
                        cell = new PdfPCell();
                        table.AddCell(new Phrase("Hormat kami, \n" + "PT BNI Life Insurance", normal));
                        //table.AddCell(new Phrase("Best Regards", normal));
                        pdfDoc.Add(table);



                        string filename = letterDetail.sender[0].nip;


                        // Tanda tangan
                        var dirChecker = letterDetail.sender.OrderBy(p => p.idUserSender).ToList();
                        //var dirChecker = letterDetail.checker.Where(p => p.idLevelChecker == 1).ToList();

                        table = new PdfPTable(dirChecker.Count());
                        width = new float[dirChecker.Count()];
                        for (int i = 0; i < dirChecker.Count(); i++)
                        {
                            width[i] = 1f;
                        }
                        table.WidthPercentage = 100;
                        table.SetWidths(width);
                        table.HorizontalAlignment = Element.ALIGN_LEFT;
                        table.DefaultCell.Border = 0;
                        var no1 = 0;


                        //cell = new PdfPCell();
                        //table.AddCell(new Phrase("Direksi PT BNI Life Insurance", normal));
                        foreach (var item in dirChecker)
                        {
                            cell = new PdfPCell();
                            cell.Border = 0;




                            if (letterDetail.letter.letterNumber != "NO_LETTER")
                            {
                                if (letterDetail.letter.signatureType == 1)
                                {

                                    Image imagettd = GetSignatureImage(item.nip);
                                    //cell.AddElement(imagettd);
                                    if (imagettd != null)
                                    {
                                        cell.AddElement(imagettd);
                                    }
                                    else
                                    {
                                        var img = "File tanda tangan tidak ditemukan";
                                        paragraph = new Paragraph(img, normal);
                                        cell.AddElement(paragraph);
                                    }

                                }
                                else if (letterDetail.letter.signatureType == 2)
                                {


                                    Image QRCodeSignatureApprover = GetBarcodeSignature("NIP : " + item.nip + "\nNAMA : " + item.fullname + "\nJabatan : " + item.positionName);
                                    QRCodeSignatureApprover.ScaleAbsoluteHeight(10f);
                                    cell.AddElement(QRCodeSignatureApprover);
                                }
                                else if (letterDetail.letter.signatureType == 3)
                                {
                                    var img = "";
                                    paragraph = new Paragraph(img, normal);
                                    cell.AddElement(paragraph);
                                    Paragraph linessia = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(2F, 100.0F, colorWhite, Element.ALIGN_CENTER, 1)));
                                    linessia.SpacingBefore = 0f;
                                    linessia.SetLeading(0.5F, 0.5F);
                                    pdfDoc.Add(linessia);

                                    Paragraph linessiaa = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(2F, 100.0F, colorWhite, Element.ALIGN_CENTER, 1)));
                                    linessiaa.SpacingBefore = 0f;
                                    linessiaa.SetLeading(0.5F, 0.5F);
                                    pdfDoc.Add(linessiaa);

                                    Paragraph linessiaaa = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(2F, 100.0F, colorWhite, Element.ALIGN_CENTER, 1)));
                                    linessiaaa.SpacingBefore = 0f;
                                    linessiaaa.SetLeading(0.5F, 0.5F);
                                    pdfDoc.Add(linessiaaa);
                                }
                                else
                                {

                                    Image imagettd = GetSignatureImage(item.nip);
                                    cell.AddElement(imagettd);
                                    Image QRCodeSignatureApprover = GetBarcodeSignature("NIP : " + item.nip + "\nNAMA : " + item.fullname + "\nJabatan : " + item.positionName);
                                    cell.AddElement(QRCodeSignatureApprover);
                                    //if (imagettd != null)
                                    //{
                                    //    cell.AddElement(imagettd);
                                    //    Image QRCodeSignatureApprover = GetBarcodeSignature("NIP : " + item.nip + "\nNAMA : " + item.fullname + "\nJabatan : " + item.positionName);
                                    //    cell.AddElement(QRCodeSignatureApprover);
                                    //}
                                    //else
                                    //{
                                    //    var img = "File tanda tangan tidak di temukan";
                                    //    paragraph = new Paragraph(img, normal);
                                    //    cell.AddElement(paragraph);
                                    //    Image QRCodeSignatureApprover = GetBarcodeSignature("NIP : " + item.nip + "\nNAMA : " + item.fullname + "\nJabatan : " + item.positionName);
                                    //    cell.AddElement(QRCodeSignatureApprover);
                                    //}

                                }
                                no1++;
                            }

                            paragraph = new Paragraph(item.fullname, underlineBold);
                            cell.AddElement(paragraph);
                            paragraph = new Paragraph(item.positionName, bold);
                            cell.AddElement(paragraph);

                            table.AddCell(cell);
                        }
                        // end tanda tangan
                        pdfDoc.Add(table);

                        #endregion

                        pdfDoc.NewPage();

                        #region Lampiran

                        PdfPTable table1 = new PdfPTable(2);
                        float[] width1 = new float[] { 0.667f, 2f };
                        table1.WidthPercentage = 100;
                        table1.SetWidths(width1);
                        table1.HorizontalAlignment = Element.ALIGN_LEFT;
                        table1.DefaultCell.Border = 0;
                        table1.SpacingBefore = 0f;
                        table1.SpacingAfter = 0f;

                        PdfPCell cell1 = new PdfPCell();
                        Paragraph paragraph1 = new Paragraph();

                        cell1 = new PdfPCell();
                        cell1.BorderWidthLeft = 1f;
                        cell1.BorderWidthRight = 1f;
                        cell1.BorderWidthTop = 1f;
                        cell1.BorderWidthBottom = 1f;
                        cell1.MinimumHeight = 30f;
                        paragraph1 = new Paragraph("Nomor Registrasi Memo/Registration Number:" + letterDetail.letter.letterDeliberationNumber, normalDeliberation);
                        paragraph1.Alignment = Element.ALIGN_LEFT;
                        cell1.AddElement(paragraph1);
                        table1.AddCell(cell1);

                        cell1 = new PdfPCell();
                        cell1.BorderWidthLeft = 0f;
                        cell1.BorderWidthRight = 1f;
                        cell1.BorderWidthTop = 1f;
                        cell1.BorderWidthBottom = 1f;
                        cell1.MinimumHeight = 30f;
                        paragraph1 = new Paragraph("Tanggal Registrasi Memo/Date of Memo Registration : " + Convert.ToDateTime(letterDetail.letter.letterDate).ToString("dd MMMM yyyy"), normalDeliberation);
                        paragraph1.Alignment = Element.ALIGN_LEFT;
                        cell1.AddElement(paragraph1);
                        table1.AddCell(cell1);

                        cell1 = new PdfPCell();
                        cell1.BorderWidthLeft = 1f;
                        cell1.BorderWidthRight = 1f;
                        cell1.BorderWidthTop = 0f;
                        cell1.BorderWidthBottom = 1f;
                        cell1.MinimumHeight = 100f;
                        paragraph1 = new Paragraph("Nomor Memo : " + letterDetail.letter.letterNumber, normalDeliberation);
                        paragraph1.Alignment = Element.ALIGN_LEFT;
                        cell1.AddElement(paragraph1);
                        table1.AddCell(cell1);

                        cell1 = new PdfPCell();
                        cell1.BorderWidthLeft = 0f;
                        cell1.BorderWidthRight = 1f;
                        cell1.BorderWidthTop = 0f;
                        cell1.BorderWidthBottom = 1f;
                        cell1.MinimumHeight = 100f;
                        paragraph1 = new Paragraph("(Ringkasan Memo/Summary of the proposal) \n" + letterDetail.letterContent.summary, normalDeliberation);
                        paragraph1.Alignment = Element.ALIGN_LEFT;
                        cell1.AddElement(paragraph1);
                        table1.AddCell(cell1);

                        cell1 = new PdfPCell();
                        cell1.BorderWidthLeft = 1f;
                        cell1.BorderWidthRight = 1f;
                        cell1.BorderWidthTop = 0f;
                        cell1.BorderWidthBottom = 1f;
                        cell1.MinimumHeight = 100f;
                        paragraph1 = new Paragraph("Deliberation:", normalDeliberation);
                        paragraph1.Alignment = Element.ALIGN_LEFT;
                        cell1.AddElement(paragraph1);
                        no = 1;
                        foreach (var item in letterDetail.delibration)
                        {
                            if (letterDetail.receiver.Count() > 1)
                            {
                                paragraph1 = new Paragraph(no + ". " + item.fullname + " - " + item.positionName, normalDeliberation);
                                paragraph1.Alignment = Element.ALIGN_LEFT;
                                cell1.AddElement(paragraph1);
                            }
                            else
                            {
                                paragraph1 = new Paragraph(item.fullname + " - " + item.positionName, normalDeliberation);
                                paragraph1.Alignment = Element.ALIGN_LEFT;
                                cell1.AddElement(paragraph1);
                            }
                            no++;
                        }
                        table1.AddCell(cell1);

                        cell1 = new PdfPCell();
                        cell1.BorderWidthLeft = 0f;
                        cell1.BorderWidthRight = 1f;
                        cell1.BorderWidthTop = 0f;
                        cell1.BorderWidthBottom = 1f;
                        cell1.MinimumHeight = 100f;
                        paragraph1 = new Paragraph("Catatan / Notes : \n", underlineDeliberation);
                        paragraph1.Alignment = Element.ALIGN_LEFT;
                        cell1.AddElement(paragraph1);
                        no = 1;
                        foreach (var item in letterDetail.delibration)
                        {
                            if (letterDetail.receiver.Count() > 1)
                            {
                                paragraph1 = new Paragraph(no + ". " + item.commentdlbrt, normalDeliberation);
                                paragraph1.Alignment = Element.ALIGN_LEFT;
                                cell1.AddElement(paragraph1);
                            }
                            else
                            {
                                paragraph1 = new Paragraph(item.commentdlbrt, normalDeliberation);
                                paragraph1.Alignment = Element.ALIGN_LEFT;
                                cell1.AddElement(paragraph1);
                            }
                            no++;
                        }
                        table1.AddCell(cell1);

                        pdfDoc.Add(table1);

                        //var checkerList = letterDetail.checker.Where(p => p.ordernumber != 1).OrderBy(p => p.idLevelChecker).ToList();
                        var checkerList = letterDetail.checker.OrderBy(p => p.idLevelChecker).ToList();
                        foreach (var item in checkerList)
                        {
                            var getLogComment = letterDetail.log.Where(p => p.idUserLog == item.idUserChecker && p.description.Contains("pemeriksa ke")).OrderByDescending(p => p.createdOn).FirstOrDefault();
                            var comment = "";
                            string approveDate = "";
                            if (getLogComment != null)
                            {
                                comment = getLogComment.comment;
                                approveDate = Convert.ToDateTime(getLogComment.createdOn).ToString("dd MMMM yyyy");
                            }
                            table1 = new PdfPTable(3);
                            width = new float[] { 1f, 2f, 1f };
                            table1.WidthPercentage = 100;
                            table1.SetWidths(width);
                            table1.HorizontalAlignment = Element.ALIGN_LEFT;
                            table1.DefaultCell.Border = 0;
                            table1.SpacingBefore = 0f;
                            table1.SpacingAfter = 0f;

                            cell1 = new PdfPCell();
                            cell1.Rowspan = 2;
                            cell1.BorderWidthLeft = 1f;
                            cell1.BorderWidthRight = 1f;
                            cell1.BorderWidthTop = 0f;
                            cell1.BorderWidthBottom = 1f;
                            cell1.MinimumHeight = 120f;
                            if (letterDetail.letter.statusCode == 2)
                                {
                                    paragraph1 = new Paragraph("Belum diputuskan / Undecided \n", normalDeliberation);
                                }
                                else if (letterDetail.letter.statusCode == 5)
                                {
                                    paragraph1 = new Paragraph("Ditolak / Reject \n", normalDeliberation);
                                }
                                else
                                {
                                   if(item.idLevelChecker == 2 ||item.idLevelChecker == 1)
                                   { 
                                       paragraph1 = new Paragraph("Disetujui / Approved \n", normalDeliberation);
                                   }
                                   else
                                   {
                                       paragraph1 = new Paragraph("Mengetahui / Understand \n", normalDeliberation);
                                       
                                   }
                                }

                            paragraph1.Alignment = Element.ALIGN_LEFT;
                            cell1.AddElement(paragraph1);
                            paragraph1 = new Paragraph(item.positionName, normalDeliberation);
                            paragraph1.Alignment = Element.ALIGN_LEFT;
                            cell1.AddElement(paragraph1);
                            table1.AddCell(cell1);

                            cell1 = new PdfPCell();
                            cell1.Rowspan = 2;
                            cell1.BorderWidthLeft = 0f;
                            cell1.BorderWidthRight = 1f;
                            cell1.BorderWidthTop = 0f;
                            cell1.BorderWidthBottom = 1f;
                            cell1.MinimumHeight = 120f;
                            paragraph1 = new Paragraph("Catatan / Notes : \n", underlineDeliberation);
                            paragraph1.Alignment = Element.ALIGN_LEFT;
                            cell1.AddElement(paragraph1);
                            paragraph1 = new Paragraph(comment, normalDeliberation);
                            paragraph1.Alignment = Element.ALIGN_LEFT;
                            cell1.AddElement(paragraph1);
                            table1.AddCell(cell1);

                            cell1 = new PdfPCell();
                            cell1.BorderWidthLeft = 0f;
                            cell1.BorderWidthRight = 1f;
                            cell1.BorderWidthTop = 0f;
                            cell1.BorderWidthBottom = 1f;
                            cell1.MinimumHeight = 40f;
                            paragraph1 = new Paragraph("Tanggal : ", underlineDeliberation);
                            paragraph1.Alignment = Element.ALIGN_LEFT;
                            //cell1.AddElement(paragraph);
                            paragraph1 = new Paragraph(approveDate, normalDeliberation);
                            paragraph1.Alignment = Element.ALIGN_LEFT;
                            cell1.AddElement(paragraph1);
                            table1.AddCell(cell1);

                            cell1 = new PdfPCell();
                            cell1.BorderWidthLeft = 0f;
                            cell1.BorderWidthRight = 1f;
                            cell1.BorderWidthTop = 0f;
                            cell1.BorderWidthBottom = 1f;
                            paragraph1 = new Paragraph("Tanda Tangan : \n", underlineDeliberation);
                            paragraph1.Alignment = Element.ALIGN_LEFT;
                            cell1.AddElement(paragraph1);
                            if (letterDetail.letter.letterNumber != "NO_LETTER")
                            {
                                if (letterDetail.letter.signatureType == 1)
                                {
                                    Image imagettd = GetSignatureImage(item.nip);
                                    //cell.AddElement(imagettd);
                                    if (imagettd != null)
                                    {
                                        cell1.AddElement(imagettd);
                                    }
                                    else
                                    {
                                        var img = "File tanda tangan tidak ditemukan";
                                        paragraph1 = new Paragraph(img, normal);
                                        cell1.AddElement(paragraph1);
                                    }
                                }
                                else if (letterDetail.letter.signatureType == 2)
                                {

                                    Image QRCodeSignatureApprover = GetBarcodeSignature("NIP : " + item.nip + "\nNAMA : " + item.fullname + "\nJabatan : " + item.positionName);
                                    cell1.AddElement(QRCodeSignatureApprover);
                                }
                                else if (letterDetail.letter.signatureType == 3)
                                {
                                    var img = "";
                                    paragraph = new Paragraph(img, normal);
                                    cell.AddElement(paragraph);
                                    Paragraph linessia = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(2F, 100.0F, colorWhite, Element.ALIGN_CENTER, 1)));
                                    linessia.SpacingBefore = 0f;
                                    linessia.SetLeading(0.5F, 0.5F);
                                    pdfDoc.Add(linessia);

                                    Paragraph linessiaa = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(2F, 100.0F, colorWhite, Element.ALIGN_CENTER, 1)));
                                    linessiaa.SpacingBefore = 0f;
                                    linessiaa.SetLeading(0.5F, 0.5F);
                                    pdfDoc.Add(linessiaa);

                                    Paragraph linessiaaa = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(2F, 100.0F, colorWhite, Element.ALIGN_CENTER, 1)));
                                    linessiaaa.SpacingBefore = 0f;
                                    linessiaaa.SetLeading(0.5F, 0.5F);
                                    pdfDoc.Add(linessiaaa);
                                }
                                else
                                {
                                    //innertable1 = new PdfPtable1(2);
                                    //float[] widthinner = new float[] { 0.3f, 0.3f };
                                    //innertable1.WidthPercentage = 100;
                                    //innertable1.SetWidths(widthinner);
                                    //innertable1.HorizontalAlignment = Element.ALIGN_LEFT;
                                    //innertable1.DefaultCell.Border = 1;
                                    //innertable1.DefaultCell.PaddingBottom = 8;

                                    //innercell = new PdfPCell();
                                    //innercell.BorderWidthLeft = 1f;
                                    //innercell.BorderWidthRight = 1f;
                                    //innercell.BorderWidthTop = 1f;
                                    //innercell.BorderWidthBottom = 1f;




                                    Image imagettd = GetSignatureImage(item.nip);
                                    cell1.AddElement(imagettd);

                                    Image QRCodeSignatureApprover = GetBarcodeSignature("NIP : " + item.nip + "\nNAMA : " + item.fullname + "\nJabatan : " + item.positionName);
                                    cell1.AddElement(QRCodeSignatureApprover);
                                    //if (imagettd !=null)
                                    //{
                                    //    cell.AddElement(imagettd);

                                    //    Image QRCodeSignatureApprover = GetBarcodeSignature("NIP : " + item.nip + "\nNAMA : " + item.fullname + "\nJabatan : " + item.positionName);
                                    //    cell.AddElement(QRCodeSignatureApprover);
                                    //}
                                    //else
                                    //{
                                    //    var img = "File tanda tangan tidak di temukan";
                                    //    paragraph = new Paragraph(img, normal);

                                    //    cell.AddElement(paragraph);
                                    //    Image QRCodeSignatureApprover = GetBarcodeSignature("NIP : " + item.nip + "\nNAMA : " + item.fullname + "\nJabatan : " + item.positionName);
                                    //    cell.AddElement(QRCodeSignatureApprover);
                                    //}

                                }
                            }

                            paragraph1 = new Paragraph(item.fullname, bold);
                            paragraph1.Alignment = Element.ALIGN_CENTER;
                            cell1.AddElement(paragraph1);
                            table1.AddCell(cell1);


                            pdfDoc.Add(table1);
                        }


                        #endregion

                        pdfDoc.Close();

                        var dateLetterFormat = Convert.ToDateTime(letterDetail.letter.letterDate).ToString("dd MMMM yyyy");
                        var letterNumber = letterDetail.letter.letterNumber;
                        byte[] bytess = ms.ToArray();
                        byte[] bytes = AddPageNumbers(bytess, dateLetterFormat, letterNumber, "Memo");
                        ms.Close();

                        return File(bytes.ToArray(), "application/pdf", fileName);
                    }

               #endregion

            }

            //done
            private FileContentResult PrivSuratKeputusanDewanKom (string letternumber, OutputDetailMemo letterDetail, string fileName)
            {

                    #region Surat Keputusan Dewan Komisaris

                    StringBuilder sb = new StringBuilder();
                    sb.Append("<div style='padding-left:2px;'>" + letterDetail.letterContent.letterContent + "</div>");
                    StringReader sr = new StringReader(sb.ToString());
                    //Document pdfDoc = new Document(PageSize.A4, 25, 25, 25, 15);
                    //Document pdfDoc = new Document(PageSize.A4, 56, 56, 113, 97);
                    Document pdfDoc = new Document(PageSize.A4, 56.7f, 56.7f, 66.5f, 99.225f);
                    HTMLWorker htmlparser = new HTMLWorker(pdfDoc);


                    using (MemoryStream ms = new MemoryStream())
                    {
                        PdfWriter pdfWriter = PdfWriter.GetInstance(pdfDoc, ms);
                        pdfDoc.Open();
                        //Font
                        int no;
                        var fontName = "Calibri";
                        string fontPath = Path.Combine(_environment.WebRootPath, "fonts/Calibri.ttf");
                        //FontFactory.Register(fontPath);

                        Font normal = FontFactory.GetFont(fontName, 12, Font.NORMAL, BaseColor.BLACK);
                        Font bold = FontFactory.GetFont(fontName, 12, Font.BOLD, BaseColor.BLACK);
                        Font underline = FontFactory.GetFont(fontName, 12, Font.UNDERLINE, BaseColor.BLACK);
                        Font underlineBold = FontFactory.GetFont(fontName, 12, Font.BOLD | Font.UNDERLINE, BaseColor.BLACK);

                        Font normalDeliberation = FontFactory.GetFont(fontName, 10, Font.NORMAL, BaseColor.BLACK);
                        Font underlineDeliberation = FontFactory.GetFont(fontName, 10, Font.UNDERLINE, BaseColor.BLACK);
                        Font underlineDeliberationBold = FontFactory.GetFont(fontName, 10, Font.BOLD | Font.UNDERLINE, BaseColor.BLACK);
                        // Colors
                        BaseColor colorBlack = new BaseColor(System.Drawing.ColorTranslator.FromHtml("#000000"));
                        BaseColor colorWhite = new BaseColor(System.Drawing.ColorTranslator.FromHtml("#FFFFFF"));

                        PdfPTable table = new PdfPTable(2);
                        float[] width = new float[] { 0.667f, 2f };
                        table.WidthPercentage = 100;
                        table.SetWidths(width);
                        table.HorizontalAlignment = Element.ALIGN_LEFT;
                        table.DefaultCell.Border = 0;
                        table.SpacingBefore = 0f;
                        table.SpacingAfter = 0f;

                        PdfPCell cell = new PdfPCell();
                        Paragraph paragraph = new Paragraph();


                            #region header

                            table = new PdfPTable(1);
                            width = new float[] { 2f };
                            table.WidthPercentage = 100;
                            table.SetWidths(width);
                            table.HorizontalAlignment = Element.ALIGN_LEFT;
                            table.DefaultCell.Border = 0;
                            table.SpacingBefore = 00f;
                            table.SpacingAfter = 0f;

                            cell = new PdfPCell();
                            cell.Border = 0;
                            paragraph = new Paragraph(new Phrase("KEPUTUSAN DEWAN KOMISARIS PT BNI LIFE INSURANCE", bold));
                            paragraph.Alignment = Element.ALIGN_CENTER;
                            cell.AddElement(paragraph);
                            table.AddCell(cell);
                            pdfDoc.Add(table);

                            table = new PdfPTable(1);
                            width = new float[] { 10f };
                            table.WidthPercentage = 100;
                            table.SetWidths(width);
                            table.HorizontalAlignment = Element.ALIGN_CENTER;
                            table.DefaultCell.Border = 0;
                            table.SpacingBefore = 00f;
                            table.SpacingAfter = 0f;

                            cell = new PdfPCell();
                            cell.Border = 0;
                            paragraph = new Paragraph(new Phrase("NOMOR : " + letterDetail.letter.letterNumber, bold));
                            paragraph.Alignment = Element.ALIGN_CENTER;
                            cell.AddElement(paragraph);
                            table.AddCell(cell);
                            pdfDoc.Add(table);


                            table = new PdfPTable(1);
                            width = new float[] { 10f };
                            table.WidthPercentage = 100;
                            table.SetWidths(width);
                            table.HorizontalAlignment = Element.ALIGN_CENTER;
                            table.DefaultCell.Border = 0;
                            table.SpacingBefore = 00f;
                            table.SpacingAfter = 0f;

                            cell = new PdfPCell();
                            cell.Border = 0;
                            paragraph = new Paragraph(new Phrase("TANGGAL " + Convert.ToDateTime(letterDetail.letter.letterDate).ToString("dd MMMM yyyy"), bold));
                            paragraph.Alignment = Element.ALIGN_CENTER;
                            cell.AddElement(paragraph);
                            table.AddCell(cell);
                            pdfDoc.Add(table);

                            table = new PdfPTable(1);
                            width = new float[] { 10f };
                            table.WidthPercentage = 100;
                            table.SetWidths(width);
                            table.HorizontalAlignment = Element.ALIGN_CENTER;
                            table.DefaultCell.Border = 0;
                            table.SpacingBefore = 0f;
                            table.SpacingAfter = 0f;

                            cell = new PdfPCell();
                            cell.Border = 0;
                            paragraph = new Paragraph(new Phrase("TENTANG", bold));
                            paragraph.Alignment = Element.ALIGN_CENTER;
                            cell.AddElement(paragraph);
                            table.AddCell(cell);
                            pdfDoc.Add(table);

                            table = new PdfPTable(1);
                            width = new float[] { 10f };
                            table.WidthPercentage = 100;
                            table.SetWidths(width);
                            table.HorizontalAlignment = Element.ALIGN_CENTER;
                            table.DefaultCell.Border = 0;
                            table.SpacingBefore = 00f;
                            table.SpacingAfter = 0f;

                            cell = new PdfPCell();
                            cell.Border = 0;
                            paragraph = new Paragraph(new Phrase(letterDetail.letter.about, underlineBold));
                            paragraph.Alignment = Element.ALIGN_CENTER;
                            cell.AddElement(paragraph);
                            table.AddCell(cell);
                            pdfDoc.Add(table);


                            Paragraph line = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(2F, 100.0F, colorBlack, Element.ALIGN_CENTER, 1)));
                            line.SpacingBefore = 0f;
                            line.SetLeading(2F, 0.5F);
                            pdfDoc.Add(line);
                            Paragraph lines = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(2F, 100.0F, colorBlack, Element.ALIGN_CENTER, 1)));
                            lines.SpacingBefore = 0f;
                            line.SetLeading(0.5F, 0.5F);
                            pdfDoc.Add(line);

                            #endregion

                            //ADD HTML CONTENT
                            htmlparser.Parse(sr);


                            #region Tanda Tangan

                            Paragraph a = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(2F, 100.0F, colorWhite, Element.ALIGN_CENTER, 1)));
                            a.SpacingBefore = 0f;
                            a.SetLeading(0.5F, 0.5F);
                            pdfDoc.Add(a);



                            cell = new PdfPCell();
                            table.AddCell(new Phrase("", normal));

                            cell = new PdfPCell();
                            table.AddCell(new Phrase(" ", normal));
                            cell = new PdfPCell();
                            table.AddCell(new Phrase(" ", normal));
                            cell = new PdfPCell();
                            table.AddCell(new Phrase("Hormat kami, \n" + "PT BNI Life Insurance", normal));
                            //table.AddCell(new Phrase("Best Regards", normal));
                            pdfDoc.Add(table);



                            string filename = letterDetail.sender[0].nip;


                            // Tanda tangan
                            //var dirChecker = letterDetail.checker.OrderBy(p => p.idLevelChecker).ToList();
                            var dirChecker = letterDetail.checker.Where(p => p.idLevelChecker == 1).OrderBy(p => p.idLevelChecker).ToList();

                            table = new PdfPTable(dirChecker.Count());
                            width = new float[dirChecker.Count()];
                            for (int i = 0; i < dirChecker.Count(); i++)
                            {
                                width[i] = 1f;
                            }
                            table.WidthPercentage = 100;
                            table.SetWidths(width);
                            table.HorizontalAlignment = Element.ALIGN_LEFT;
                            table.DefaultCell.Border = 0;
                            var no1 = 0;


                            //cell = new PdfPCell();
                            //table.AddCell(new Phrase("Direksi PT BNI Life Insurance", normal));
                            foreach (var item in dirChecker)
                            {
                                cell = new PdfPCell();
                                cell.Border = 0;




                                if (letterDetail.letter.letterNumber != "NO_LETTER")
                                {
                                    if (letterDetail.letter.signatureType == 1)
                                    {

                                        Image imagettd = GetSignatureImage(item.nip);
                                        //cell.AddElement(imagettd);
                                        if (imagettd != null)
                                        {
                                            cell.AddElement(imagettd);
                                        }
                                        else
                                        {
                                            var img = "File tanda tangan tidak ditemukan";
                                            paragraph = new Paragraph(img, normal);
                                            cell.AddElement(paragraph);
                                        }

                                    }
                                    else if (letterDetail.letter.signatureType == 2)
                                    {


                                        Image QRCodeSignatureApprover = GetBarcodeSignature("NIP : " + item.nip + "\nNAMA : " + item.fullname + "\nJabatan : " + item.positionName);
                                        QRCodeSignatureApprover.ScaleAbsoluteHeight(10f);
                                        cell.AddElement(QRCodeSignatureApprover);
                                    }
                                    else if (letterDetail.letter.signatureType == 3)
                                    {
                                        var img = "";
                                        paragraph = new Paragraph(img, normal);
                                        cell.AddElement(paragraph);
                                        Paragraph linessia = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(2F, 100.0F, colorWhite, Element.ALIGN_CENTER, 1)));
                                        linessia.SpacingBefore = 0f;
                                        linessia.SetLeading(0.5F, 0.5F);
                                        pdfDoc.Add(linessia);

                                        Paragraph linessiaa = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(2F, 100.0F, colorWhite, Element.ALIGN_CENTER, 1)));
                                        linessiaa.SpacingBefore = 0f;
                                        linessiaa.SetLeading(0.5F, 0.5F);
                                        pdfDoc.Add(linessiaa);

                                        Paragraph linessiaaa = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(2F, 100.0F, colorWhite, Element.ALIGN_CENTER, 1)));
                                        linessiaaa.SpacingBefore = 0f;
                                        linessiaaa.SetLeading(0.5F, 0.5F);
                                        pdfDoc.Add(linessiaaa);
                                    }
                                    else
                                    {

                                        Image imagettd = GetSignatureImage(item.nip);
                                        cell.AddElement(imagettd);
                                        Image QRCodeSignatureApprover = GetBarcodeSignature("NIP : " + item.nip + "\nNAMA : " + item.fullname + "\nJabatan : " + item.positionName);
                                        cell.AddElement(QRCodeSignatureApprover);
                                        //if (imagettd != null)
                                        //{
                                        //    cell.AddElement(imagettd);
                                        //    Image QRCodeSignatureApprover = GetBarcodeSignature("NIP : " + item.nip + "\nNAMA : " + item.fullname + "\nJabatan : " + item.positionName);
                                        //    cell.AddElement(QRCodeSignatureApprover);
                                        //}
                                        //else
                                        //{
                                        //    var img = "File tanda tangan tidak di temukan";
                                        //    paragraph = new Paragraph(img, normal);
                                        //    cell.AddElement(paragraph);
                                        //    Image QRCodeSignatureApprover = GetBarcodeSignature("NIP : " + item.nip + "\nNAMA : " + item.fullname + "\nJabatan : " + item.positionName);
                                        //    cell.AddElement(QRCodeSignatureApprover);
                                        //}

                                    }
                                    no1++;
                                }

                                paragraph = new Paragraph(item.fullname, underlineBold);
                                cell.AddElement(paragraph);
                                paragraph = new Paragraph(item.positionName, bold);
                                cell.AddElement(paragraph);

                                table.AddCell(cell);
                            }
                                // end tanda tangan
                                pdfDoc.Add(table);

                                #endregion

                            pdfDoc.NewPage();

                            #region Lampiran

                            PdfPTable table1 = new PdfPTable(2);
                            float[] width1 = new float[] { 0.667f, 2f };
                            table1.WidthPercentage = 100;
                            table1.SetWidths(width1);
                            table1.HorizontalAlignment = Element.ALIGN_LEFT;
                            table1.DefaultCell.Border = 0;
                            table1.SpacingBefore = 0f;
                            table1.SpacingAfter = 0f;

                            PdfPCell cell1 = new PdfPCell();
                            Paragraph paragraph1 = new Paragraph();

                            cell1 = new PdfPCell();
                            cell1.BorderWidthLeft = 1f;
                            cell1.BorderWidthRight = 1f;
                            cell1.BorderWidthTop = 1f;
                            cell1.BorderWidthBottom = 1f;
                            cell1.MinimumHeight = 30f;
                            paragraph1 = new Paragraph("Nomor Registrasi Memo/Registration Number:" + letterDetail.letter.letterDeliberationNumber, normalDeliberation);
                            paragraph1.Alignment = Element.ALIGN_LEFT;
                            cell1.AddElement(paragraph1);
                            table1.AddCell(cell1);

                            cell1 = new PdfPCell();
                            cell1.BorderWidthLeft = 0f;
                            cell1.BorderWidthRight = 1f;
                            cell1.BorderWidthTop = 1f;
                            cell1.BorderWidthBottom = 1f;
                            cell1.MinimumHeight = 30f;
                            paragraph1 = new Paragraph("Tanggal Registrasi Memo/Date of Memo Registration : " + Convert.ToDateTime(letterDetail.letter.letterDate).ToString("dd MMMM yyyy"), normalDeliberation);
                            paragraph1.Alignment = Element.ALIGN_LEFT;
                            cell1.AddElement(paragraph1);
                            table1.AddCell(cell1);

                            cell1 = new PdfPCell();
                            cell1.BorderWidthLeft = 1f;
                            cell1.BorderWidthRight = 1f;
                            cell1.BorderWidthTop = 0f;
                            cell1.BorderWidthBottom = 1f;
                            cell1.MinimumHeight = 100f;
                            paragraph1 = new Paragraph("Nomor Memo : " + letterDetail.letter.letterNumber, normalDeliberation);
                            paragraph1.Alignment = Element.ALIGN_LEFT;
                            cell1.AddElement(paragraph1);
                            table1.AddCell(cell1);

                            cell1 = new PdfPCell();
                            cell1.BorderWidthLeft = 0f;
                            cell1.BorderWidthRight = 1f;
                            cell1.BorderWidthTop = 0f;
                            cell1.BorderWidthBottom = 1f;
                            cell1.MinimumHeight = 100f;
                            paragraph1 = new Paragraph("(Ringkasan Memo/Summary of the proposal) \n" + letterDetail.letterContent.summary, normalDeliberation);
                            paragraph1.Alignment = Element.ALIGN_LEFT;
                            cell1.AddElement(paragraph1);
                            table1.AddCell(cell1);

                            cell1 = new PdfPCell();
                            cell1.BorderWidthLeft = 1f;
                            cell1.BorderWidthRight = 1f;
                            cell1.BorderWidthTop = 0f;
                            cell1.BorderWidthBottom = 1f;
                            cell1.MinimumHeight = 100f;
                            paragraph1 = new Paragraph("Deliberation:", normalDeliberation);
                            paragraph1.Alignment = Element.ALIGN_LEFT;
                            cell1.AddElement(paragraph1);
                            no = 1;
                            foreach (var item in letterDetail.delibration)
                            {
                                if (letterDetail.receiver.Count() > 1)
                                {
                                    paragraph1 = new Paragraph(no + ". " + item.fullname + " - " + item.positionName, normalDeliberation);
                                    paragraph1.Alignment = Element.ALIGN_LEFT;
                                    cell1.AddElement(paragraph1);
                                }
                                else
                                {
                                    paragraph1 = new Paragraph(item.fullname + " - " + item.positionName, normalDeliberation);
                                    paragraph1.Alignment = Element.ALIGN_LEFT;
                                    cell1.AddElement(paragraph1);
                                }
                                no++;
                            }
                            table1.AddCell(cell1);

                            cell1 = new PdfPCell();
                            cell1.BorderWidthLeft = 0f;
                            cell1.BorderWidthRight = 1f;
                            cell1.BorderWidthTop = 0f;
                            cell1.BorderWidthBottom = 1f;
                            cell1.MinimumHeight = 100f;
                            paragraph1 = new Paragraph("Catatan / Notes : \n", underlineDeliberation);
                            paragraph1.Alignment = Element.ALIGN_LEFT;
                            cell1.AddElement(paragraph1);
                            no = 1;
                            foreach (var item in letterDetail.delibration)
                            {
                                if (letterDetail.receiver.Count() > 1)
                                {
                                    paragraph1 = new Paragraph(no + ". " + item.commentdlbrt, normalDeliberation);
                                    paragraph1.Alignment = Element.ALIGN_LEFT;
                                    cell1.AddElement(paragraph1);
                                }
                                else
                                {
                                    paragraph1 = new Paragraph(item.commentdlbrt, normalDeliberation);
                                    paragraph1.Alignment = Element.ALIGN_LEFT;
                                    cell1.AddElement(paragraph1);
                                }
                                no++;
                            }
                            table1.AddCell(cell1);

                            pdfDoc.Add(table1);

                            //var checkerList = letterDetail.checker.Where(p => p.ordernumber != 1).OrderBy(p => p.idLevelChecker).ToList();
                            var checkerList = letterDetail.checker.OrderBy(p => p.idLevelChecker).ToList();
                            foreach (var item in checkerList)
                            {
                                var getLogComment = letterDetail.log.Where(p => p.idUserLog == item.idUserChecker && p.description.Contains("pemeriksa ke")).OrderByDescending(p => p.createdOn).FirstOrDefault();
                                var comment = "";
                                string approveDate = "";
                                if (getLogComment != null)
                                {
                                    comment = getLogComment.comment;
                                    approveDate = Convert.ToDateTime(getLogComment.createdOn).ToString("dd MMMM yyyy");
                                }
                                table1 = new PdfPTable(3);
                                width = new float[] { 1f, 2f, 1f };
                                table1.WidthPercentage = 100;
                                table1.SetWidths(width);
                                table1.HorizontalAlignment = Element.ALIGN_LEFT;
                                table1.DefaultCell.Border = 0;
                                table1.SpacingBefore = 0f;
                                table1.SpacingAfter = 0f;

                                cell1 = new PdfPCell();
                                cell1.Rowspan = 2;
                                cell1.BorderWidthLeft = 1f;
                                cell1.BorderWidthRight = 1f;
                                cell1.BorderWidthTop = 0f;
                                cell1.BorderWidthBottom = 1f;
                                cell1.MinimumHeight = 120f;
                                if (letterDetail.letter.statusCode == 2)
                                {
                                    paragraph1 = new Paragraph("Belum diputuskan / Undecided \n", normalDeliberation);
                                }
                                else if (letterDetail.letter.statusCode == 5)
                                {
                                    paragraph1 = new Paragraph("Ditolak / Reject \n", normalDeliberation);
                                }
                                else
                                {
                                   if(item.idLevelChecker == 2 ||item.idLevelChecker == 1)
                                   { 
                                       paragraph1 = new Paragraph("Disetujui / Approved \n", normalDeliberation);
                                   }
                                   else
                                   {
                                       paragraph1 = new Paragraph("Mengetahui / Understand \n", normalDeliberation);
                                       
                                   }
                                }

                                paragraph1.Alignment = Element.ALIGN_LEFT;
                                cell1.AddElement(paragraph1);
                                paragraph1 = new Paragraph(item.positionName, normalDeliberation);
                                paragraph1.Alignment = Element.ALIGN_LEFT;
                                cell1.AddElement(paragraph1);
                                table1.AddCell(cell1);

                                cell1 = new PdfPCell();
                                cell1.Rowspan = 2;
                                cell1.BorderWidthLeft = 0f;
                                cell1.BorderWidthRight = 1f;
                                cell1.BorderWidthTop = 0f;
                                cell1.BorderWidthBottom = 1f;
                                cell1.MinimumHeight = 120f;
                                paragraph1 = new Paragraph("Catatan / Notes : \n", underlineDeliberation);
                                paragraph1.Alignment = Element.ALIGN_LEFT;
                                cell1.AddElement(paragraph1);
                                paragraph1 = new Paragraph(comment, normalDeliberation);
                                paragraph1.Alignment = Element.ALIGN_LEFT;
                                cell1.AddElement(paragraph1);
                                table1.AddCell(cell1);

                                cell1 = new PdfPCell();
                                cell1.BorderWidthLeft = 0f;
                                cell1.BorderWidthRight = 1f;
                                cell1.BorderWidthTop = 0f;
                                cell1.BorderWidthBottom = 1f;
                                cell1.MinimumHeight = 40f;
                                paragraph1 = new Paragraph("Tanggal : ", underlineDeliberation);
                                paragraph1.Alignment = Element.ALIGN_LEFT;
                                //cell1.AddElement(paragraph);
                                paragraph1 = new Paragraph(approveDate, normalDeliberation);
                                paragraph1.Alignment = Element.ALIGN_LEFT;
                                cell1.AddElement(paragraph1);
                                table1.AddCell(cell1);

                                cell1 = new PdfPCell();
                                cell1.BorderWidthLeft = 0f;
                                cell1.BorderWidthRight = 1f;
                                cell1.BorderWidthTop = 0f;
                                cell1.BorderWidthBottom = 1f;
                                paragraph1 = new Paragraph("Tanda Tangan : \n", underlineDeliberation);
                                paragraph1.Alignment = Element.ALIGN_LEFT;
                                cell1.AddElement(paragraph1);
                                if (letterDetail.letter.letterNumber != "NO_LETTER")
                                {
                                    if (letterDetail.letter.signatureType == 1)
                                    {
                                        Image imagettd = GetSignatureImage(item.nip);
                                        //cell.AddElement(imagettd);
                                        if (imagettd != null)
                                        {
                                            cell1.AddElement(imagettd);
                                        }
                                        else
                                        {
                                            var img = "File tanda tangan tidak ditemukan";
                                            paragraph1 = new Paragraph(img, normal);
                                            cell1.AddElement(paragraph1);
                                        }
                                    }
                                    else if (letterDetail.letter.signatureType == 2)
                                    {

                                        Image QRCodeSignatureApprover = GetBarcodeSignature("NIP : " + item.nip + "\nNAMA : " + item.fullname + "\nJabatan : " + item.positionName);
                                        cell1.AddElement(QRCodeSignatureApprover);
                                    }
                                    else if (letterDetail.letter.signatureType == 3)
                                    {
                                        var img = "";
                                        paragraph = new Paragraph(img, normal);
                                        cell.AddElement(paragraph);
                                        Paragraph linessia = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(2F, 100.0F, colorWhite, Element.ALIGN_CENTER, 1)));
                                        linessia.SpacingBefore = 0f;
                                        linessia.SetLeading(0.5F, 0.5F);
                                        pdfDoc.Add(linessia);

                                        Paragraph linessiaa = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(2F, 100.0F, colorWhite, Element.ALIGN_CENTER, 1)));
                                        linessiaa.SpacingBefore = 0f;
                                        linessiaa.SetLeading(0.5F, 0.5F);
                                        pdfDoc.Add(linessiaa);

                                        Paragraph linessiaaa = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(2F, 100.0F, colorWhite, Element.ALIGN_CENTER, 1)));
                                        linessiaaa.SpacingBefore = 0f;
                                        linessiaaa.SetLeading(0.5F, 0.5F);
                                        pdfDoc.Add(linessiaaa);
                                    }
                                    else
                                    {
                                        //innertable1 = new PdfPtable1(2);
                                        //float[] widthinner = new float[] { 0.3f, 0.3f };
                                        //innertable1.WidthPercentage = 100;
                                        //innertable1.SetWidths(widthinner);
                                        //innertable1.HorizontalAlignment = Element.ALIGN_LEFT;
                                        //innertable1.DefaultCell.Border = 1;
                                        //innertable1.DefaultCell.PaddingBottom = 8;

                                        //innercell = new PdfPCell();
                                        //innercell.BorderWidthLeft = 1f;
                                        //innercell.BorderWidthRight = 1f;
                                        //innercell.BorderWidthTop = 1f;
                                        //innercell.BorderWidthBottom = 1f;




                                        Image imagettd = GetSignatureImage(item.nip);
                                        cell1.AddElement(imagettd);

                                        Image QRCodeSignatureApprover = GetBarcodeSignature("NIP : " + item.nip + "\nNAMA : " + item.fullname + "\nJabatan : " + item.positionName);
                                        cell1.AddElement(QRCodeSignatureApprover);
                                        //if (imagettd !=null)
                                        //{
                                        //    cell.AddElement(imagettd);

                                        //    Image QRCodeSignatureApprover = GetBarcodeSignature("NIP : " + item.nip + "\nNAMA : " + item.fullname + "\nJabatan : " + item.positionName);
                                        //    cell.AddElement(QRCodeSignatureApprover);
                                        //}
                                        //else
                                        //{
                                        //    var img = "File tanda tangan tidak di temukan";
                                        //    paragraph = new Paragraph(img, normal);

                                        //    cell.AddElement(paragraph);
                                        //    Image QRCodeSignatureApprover = GetBarcodeSignature("NIP : " + item.nip + "\nNAMA : " + item.fullname + "\nJabatan : " + item.positionName);
                                        //    cell.AddElement(QRCodeSignatureApprover);
                                        //}

                                    }
                                }

                                paragraph1 = new Paragraph(item.fullname, bold);
                                paragraph1.Alignment = Element.ALIGN_CENTER;
                                cell1.AddElement(paragraph1);
                                table1.AddCell(cell1);


                                pdfDoc.Add(table1);
                            }


                #endregion

                            pdfDoc.Close();

                            var dateLetterFormat = Convert.ToDateTime(letterDetail.letter.letterDate).ToString("dd MMMM yyyy");
                            var letterNumber = letterDetail.letter.letterNumber;
                            byte[] bytess = ms.ToArray();
                            byte[] bytes = AddPageNumbers(bytess, dateLetterFormat, letterNumber, "Memo");
                            ms.Close();

                            return File(bytes.ToArray(), "application/pdf", fileName);
            }

                    #endregion

            }

        #endregion

        #region Print Divisi

            //Done(Masi Ikuti Template Memo Direksi)
            private FileContentResult PrivSuratKuasa(string letternumber, OutputDetailMemo letterDetail, string fileName)
            {
                #region Surat Kuasa Divisi

                    StringBuilder sb = new StringBuilder();
                    sb.Append("<div style='padding-left:2px;'>" + letterDetail.letterContent.letterContent + "</div>");
                    StringReader sr = new StringReader(sb.ToString());
                    //Document pdfDoc = new Document(PageSize.A4, 25, 25, 25, 15);
                    //Document pdfDoc = new Document(PageSize.A4, 56, 56, 113, 97);
                    Document pdfDoc = new Document(PageSize.A4, 56.7f, 56.7f, 66.5f, 99.225f);
                    HTMLWorker htmlparser = new HTMLWorker(pdfDoc);


                    using (MemoryStream ms = new MemoryStream())
                    {
                        PdfWriter pdfWriter = PdfWriter.GetInstance(pdfDoc, ms);
                        pdfDoc.Open();
                        //Font
                        int no;
                        var fontName = "Calibri";
                        string fontPath = Path.Combine(_environment.WebRootPath, "fonts/Calibri.ttf");
                        //FontFactory.Register(fontPath);

                        Font normal = FontFactory.GetFont(fontName, 12, Font.NORMAL, BaseColor.BLACK);
                        Font bold = FontFactory.GetFont(fontName, 12, Font.BOLD, BaseColor.BLACK);
                        Font underline = FontFactory.GetFont(fontName, 12, Font.UNDERLINE, BaseColor.BLACK);
                        Font underlineBold = FontFactory.GetFont(fontName, 12, Font.BOLD | Font.UNDERLINE, BaseColor.BLACK);

                        Font normalDeliberation = FontFactory.GetFont(fontName, 10, Font.NORMAL, BaseColor.BLACK);
                        Font underlineDeliberation = FontFactory.GetFont(fontName, 10, Font.UNDERLINE, BaseColor.BLACK);
                        Font underlineDeliberationBold = FontFactory.GetFont(fontName, 10, Font.BOLD | Font.UNDERLINE, BaseColor.BLACK);
                        // Colors
                        BaseColor colorBlack = new BaseColor(System.Drawing.ColorTranslator.FromHtml("#000000"));
                        BaseColor colorWhite = new BaseColor(System.Drawing.ColorTranslator.FromHtml("#FFFFFF"));



                        PdfPTable table = new PdfPTable(2);
                        float[] width = new float[] { 0.667f, 2f };
                        table.WidthPercentage = 100;
                        table.SetWidths(width);
                        table.HorizontalAlignment = Element.ALIGN_LEFT;
                        table.DefaultCell.Border = 0;
                        table.SpacingBefore = 0f;
                        table.SpacingAfter = 0f;

                        PdfPCell cell = new PdfPCell();
                        Paragraph paragraph = new Paragraph();


                        #region header
                        //add new page

                        //table = new PdfPTable(1);
                        //width = new float[] { 2f };
                        //table.WidthPercentage = 100;
                        //table.SetWidths(width);
                        //table.HorizontalAlignment = Element.ALIGN_LEFT;
                        //table.DefaultCell.Border = 0;
                        //table.SpacingBefore = 00f;
                        //table.SpacingAfter = 0f;


                        //table.AddCell(new Phrase("MEMO", bold));
                        //pdfDoc.Add(table);

                        table = new PdfPTable(2);
                        width = new float[] { 0.2f, 1f };
                        table.WidthPercentage = 100;
                        table.SetWidths(width);
                        table.HorizontalAlignment = Element.ALIGN_RIGHT;
                        table.DefaultCell.Border = 0;
                        table.SpacingBefore = 0f;
                        table.SpacingAfter = 0f;

                        cell.Border = 0;
                        paragraph = new Paragraph("Nomor", normal);
                        paragraph.Alignment = Element.ALIGN_LEFT;
                        cell.AddElement(paragraph);
                        table.AddCell(cell);

                        //cell = new PdfPCell();
                        //cell.Border = 0;
                        //paragraph = new Paragraph(":", normal);
                        //paragraph.Alignment = Element.ALIGN_RIGHT;
                        //cell.AddElement(paragraph);
                        //table.AddCell(cell);

                        cell = new PdfPCell();
                        cell.Border = 0;
                        paragraph = new Paragraph(":"+" "+letterDetail.letter.letterNumber, normal);
                        paragraph.Alignment = Element.ALIGN_LEFT;
                        cell.AddElement(paragraph);
                        table.AddCell(cell);

                        cell = new PdfPCell();
                        cell.Border = 0;
                        paragraph = new Paragraph("Tanggal", normal);
                        paragraph.Alignment = Element.ALIGN_LEFT;
                        cell.AddElement(paragraph);
                        table.AddCell(cell);

                        //cell = new PdfPCell();
                        //cell.Border = 0;
                        //paragraph = new Paragraph(":", normal);
                        //paragraph.Alignment = Element.ALIGN_RIGHT;
                        //cell.AddElement(paragraph);
                        //table.AddCell(cell);

                        cell = new PdfPCell();
                        cell.Border = 0;
                        paragraph = new Paragraph(":"+" "+Convert.ToDateTime(letterDetail.letter.letterDate).ToString("dd MMMM yyyy"), normal);
                        paragraph.Alignment = Element.ALIGN_LEFT;
                        cell.AddElement(paragraph);
                        table.AddCell(cell);

                        cell = new PdfPCell();
                        cell.Border = 0;
                        paragraph = new Paragraph("Kepada", normal);
                        paragraph.Alignment = Element.ALIGN_LEFT;
                        cell.AddElement(paragraph);
                        table.AddCell(cell);

                        //cell = new PdfPCell();
                        //cell.Border = 0;
                        //paragraph = new Paragraph(":", normal);
                        //paragraph.Alignment = Element.ALIGN_RIGHT;
                        //cell.AddElement(paragraph);
                        //table.AddCell(cell);

                        cell = new PdfPCell();
                        cell.Border = 0;
                        no = 1;
                        foreach (var item in letterDetail.receiver)
                        {
                            if (letterDetail.receiver.Count() > 1)
                            {
                                paragraph = new Paragraph(":"+" "+no + ". " + item.fullname + " - " + item.positionName, normal);
                                paragraph.Alignment = Element.ALIGN_LEFT;
                                cell.AddElement(paragraph);
                            }
                            else
                            {
                                paragraph = new Paragraph(":"+" "+item.fullname + " - " + item.positionName, normal);
                                paragraph.Alignment = Element.ALIGN_LEFT;
                                cell.AddElement(paragraph);
                            }
                            no++;
                        }
                        table.AddCell(cell);

                        cell = new PdfPCell();
                        cell.Border = 0;
                        paragraph = new Paragraph("Tembusan", normal);
                        paragraph.Alignment = Element.ALIGN_LEFT;
                        cell.AddElement(paragraph);
                        table.AddCell(cell);

                        //cell = new PdfPCell();
                        //cell.Border = 0;
                        //paragraph = new Paragraph(":", normal);
                        //paragraph.Alignment = Element.ALIGN_RIGHT;
                        //cell.AddElement(paragraph);
                        //table.AddCell(cell);

                        cell = new PdfPCell();
                        cell.Border = 0;
                        no = 1;
                        foreach (var item in letterDetail.copy)
                        {
                            if (letterDetail.copy.Count() > 1)
                            {
                                paragraph = new Paragraph(":"+" "+no + ". " + item.fullname + " - " + item.positionName, normal);
                                paragraph.Alignment = Element.ALIGN_LEFT;
                                cell.AddElement(paragraph);
                            }
                            else
                            {
                                paragraph = new Paragraph(":"+" "+item.fullname + " - " + item.positionName, normal);
                                paragraph.Alignment = Element.ALIGN_LEFT;
                                cell.AddElement(paragraph);
                            }
                            no++;
                        }
                        if (letterDetail.copy.Count() == 0)
                        {
                            paragraph = new Paragraph("-", normal);
                            paragraph.Alignment = Element.ALIGN_LEFT;
                            cell.AddElement(paragraph);
                        }
                        table.AddCell(cell);

                        cell = new PdfPCell();
                        cell.Border = 0;
                        paragraph = new Paragraph("Dari", normal);
                        paragraph.Alignment = Element.ALIGN_LEFT;
                        cell.AddElement(paragraph);
                        table.AddCell(cell);

                        //cell = new PdfPCell();
                        //cell.Border = 0;
                        //paragraph = new Paragraph(":", normal);
                        //paragraph.Alignment = Element.ALIGN_RIGHT;
                        //cell.AddElement(paragraph);
                        //table.AddCell(cell);

                        cell = new PdfPCell();
                        cell.Border = 0;
                        paragraph = new Paragraph(":"+" "+letterDetail.sender[0].fullname + " - " + letterDetail.sender[0].positionName, normal);
                        paragraph.Alignment = Element.ALIGN_LEFT;
                        cell.AddElement(paragraph);
                        table.AddCell(cell);

                        cell = new PdfPCell();
                        cell.Border = 0;
                        paragraph = new Paragraph("Perihal", normal);
                        paragraph.Alignment = Element.ALIGN_LEFT;
                        cell.AddElement(paragraph);
                        table.AddCell(cell);

                        //cell = new PdfPCell();
                        //cell.Border = 0;
                        //paragraph = new Paragraph(":", normal);
                        //paragraph.Alignment = Element.ALIGN_RIGHT;
                        //cell.AddElement(paragraph);
                        //table.AddCell(cell);

                        cell = new PdfPCell();
                        cell.Border = 0;
                        paragraph = new Paragraph(":"+" "+letterDetail.letter.about, normal);
                        paragraph.Alignment = Element.ALIGN_LEFT;
                        cell.AddElement(paragraph);
                        table.AddCell(cell);

                        cell = new PdfPCell();
                        cell.Border = 0;
                        paragraph = new Paragraph("Lampiran", normal);
                        paragraph.Alignment = Element.ALIGN_LEFT;
                        cell.AddElement(paragraph);
                        table.AddCell(cell);

                        //cell = new PdfPCell();
                        //cell.Border = 0;
                        //paragraph = new Paragraph(":", normal);
                        //paragraph.Alignment = Element.ALIGN_RIGHT;
                        //cell.AddElement(paragraph);
                        //table.AddCell(cell);

                        var lampiran = letterDetail.letter.attachmentDesc == null ? "-" : letterDetail.letter.attachmentDesc;
                        cell = new PdfPCell();
                        cell.Border = 0;
                        paragraph = new Paragraph(":"+" "+lampiran, bold);
                        paragraph.Alignment = Element.ALIGN_LEFT;
                        cell.AddElement(paragraph);
                        table.AddCell(cell);

                        //Add table to document
                        pdfDoc.Add(table);


                        Paragraph line = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(2F, 100.0F, colorBlack, Element.ALIGN_CENTER, 1)));
                        line.SpacingBefore = 0f;
                        line.SetLeading(2F, 0.5F);
                        pdfDoc.Add(line);
                        Paragraph lines = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(2F, 100.0F, colorBlack, Element.ALIGN_CENTER, 1)));
                        lines.SpacingBefore = 0f;
                        line.SetLeading(0.5F, 0.5F);
                        pdfDoc.Add(line);

                        Paragraph liness = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(2F, 100.0F, colorWhite, Element.ALIGN_CENTER, 1)));
                        liness.SpacingBefore = 0f;
                        liness.SetLeading(0.5F, 0.5F);
                        pdfDoc.Add(liness);

                        //Paragraph linesss = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(2F, 100.0F, colorWhite, Element.ALIGN_CENTER, 1)));
                        //linesss.SpacingBefore = 0f;
                        //linesss.SetLeading(0.5F, 0.5F);
                        //pdfDoc.Add(linesss);


                        table = new PdfPTable(1);
                        width = new float[] { 2f };
                        table.WidthPercentage = 100;
                        table.SetWidths(width);
                        table.HorizontalAlignment = Element.ALIGN_LEFT;
                        table.DefaultCell.Border = 0;
                        table.SpacingBefore = 00f;
                        table.SpacingAfter = 0f;


                        //table.AddCell(new Phrase("Dengan hormat", normal));
                        //pdfDoc.Add(table);

                        Paragraph linessss = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(2F, 100.0F, colorWhite, Element.ALIGN_CENTER, 1)));
                        linessss.SpacingBefore = 0f;
                        linessss.SetLeading(0.5F, 0.5F);
                        pdfDoc.Add(linessss);

                        Paragraph lin = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(2F, 100.0F, colorWhite, Element.ALIGN_CENTER, 1)));
                        lin.SpacingBefore = 0f;
                        lin.SetLeading(0.5F, 0.5F);
                        pdfDoc.Add(lin);

                        #endregion

                        //ADD HTML CONTENT
                        htmlparser.Parse(sr);


                        #region Tanda Tangan

                        Paragraph a = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(2F, 100.0F, colorWhite, Element.ALIGN_CENTER, 1)));
                        a.SpacingBefore = 0f;
                        a.SetLeading(0.5F, 0.5F);
                        pdfDoc.Add(a);



                        cell = new PdfPCell();
                        table.AddCell(new Phrase("", normal));

                        cell = new PdfPCell();
                        table.AddCell(new Phrase(" ", normal));
                        cell = new PdfPCell();
                        table.AddCell(new Phrase(" ", normal));
                        cell = new PdfPCell();
                        table.AddCell(new Phrase("Hormat kami, \n" + "PT BNI Life Insurance", normal));
                        //table.AddCell(new Phrase("Best Regards", normal));
                        pdfDoc.Add(table);



                        string filename = letterDetail.sender[0].nip;


                        // Tanda tangan
                        var dirChecker = letterDetail.sender.OrderBy(p => p.idUserSender).ToList();
                        //var dirChecker = letterDetail.checker.Where(p => p.idLevelChecker == 2 || p.idLevelChecker == 1).ToList();

                        

                        table = new PdfPTable(dirChecker.Count());
                        width = new float[dirChecker.Count()];
                        for (int i = 0; i < dirChecker.Count(); i++)
                        {
                            width[i] = 1f;
                        }
                        table.WidthPercentage = 100;
                        table.SetWidths(width);
                        table.HorizontalAlignment = Element.ALIGN_LEFT;
                        table.DefaultCell.Border = 0;
                        var no1 = 0;


                        //cell = new PdfPCell();
                        //table.AddCell(new Phrase("Direksi PT BNI Life Insurance", normal));
                        foreach (var item in dirChecker)
                        {
                            cell = new PdfPCell();
                            cell.Border = 0;




                            if (letterDetail.letter.letterNumber != "NO_LETTER")
                            {
                                if (letterDetail.letter.signatureType == 1)
                                {

                                    Image imagettd = GetSignatureImage(item.nip);
                                    //cell.AddElement(imagettd);
                                    if (imagettd != null)
                                    {
                                        cell.AddElement(imagettd);
                                    }
                                    else
                                    {
                                        var img = "File tanda tangan tidak ditemukan";
                                        paragraph = new Paragraph(img, normal);
                                        cell.AddElement(paragraph);
                                    }

                                }
                                else if (letterDetail.letter.signatureType == 2)
                                {


                                    Image QRCodeSignatureApprover = GetBarcodeSignature("NIP : " + item.nip + "\nNAMA : " + item.fullname + "\nJabatan : " + item.positionName);
                                    QRCodeSignatureApprover.ScaleAbsoluteHeight(10f);
                                    cell.AddElement(QRCodeSignatureApprover);
                                }
                                else if (letterDetail.letter.signatureType == 3)
                                {
                                    var img = "";
                                    paragraph = new Paragraph(img, normal);
                                    cell.AddElement(paragraph);
                                    Paragraph linessia = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(2F, 100.0F, colorWhite, Element.ALIGN_CENTER, 1)));
                                    linessia.SpacingBefore = 0f;
                                    linessia.SetLeading(0.5F, 0.5F);
                                    pdfDoc.Add(linessia);

                                    Paragraph linessiaa = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(2F, 100.0F, colorWhite, Element.ALIGN_CENTER, 1)));
                                    linessiaa.SpacingBefore = 0f;
                                    linessiaa.SetLeading(0.5F, 0.5F);
                                    pdfDoc.Add(linessiaa);

                                    Paragraph linessiaaa = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(2F, 100.0F, colorWhite, Element.ALIGN_CENTER, 1)));
                                    linessiaaa.SpacingBefore = 0f;
                                    linessiaaa.SetLeading(0.5F, 0.5F);
                                    pdfDoc.Add(linessiaaa);
                                }
                                else
                                {

                                    Image imagettd = GetSignatureImage(item.nip);
                                    cell.AddElement(imagettd);
                                    Image QRCodeSignatureApprover = GetBarcodeSignature("NIP : " + item.nip + "\nNAMA : " + item.fullname + "\nJabatan : " + item.positionName);
                                    cell.AddElement(QRCodeSignatureApprover);
                                    //if (imagettd != null)
                                    //{
                                    //    cell.AddElement(imagettd);
                                    //    Image QRCodeSignatureApprover = GetBarcodeSignature("NIP : " + item.nip + "\nNAMA : " + item.fullname + "\nJabatan : " + item.positionName);
                                    //    cell.AddElement(QRCodeSignatureApprover);
                                    //}
                                    //else
                                    //{
                                    //    var img = "File tanda tangan tidak di temukan";
                                    //    paragraph = new Paragraph(img, normal);
                                    //    cell.AddElement(paragraph);
                                    //    Image QRCodeSignatureApprover = GetBarcodeSignature("NIP : " + item.nip + "\nNAMA : " + item.fullname + "\nJabatan : " + item.positionName);
                                    //    cell.AddElement(QRCodeSignatureApprover);
                                    //}

                                }
                                no1++;
                            }

                            paragraph = new Paragraph(item.fullname, underlineBold);
                            cell.AddElement(paragraph);
                            paragraph = new Paragraph(item.positionName, bold);
                            cell.AddElement(paragraph);

                            table.AddCell(cell);
                        }
                        // end tanda tangan
                        pdfDoc.Add(table);

                        #endregion

                        pdfDoc.NewPage();

                        #region Lampiran

                        PdfPTable table1 = new PdfPTable(2);
                        float[] width1 = new float[] { 0.667f, 2f };
                        table1.WidthPercentage = 100;
                        table1.SetWidths(width1);
                        table1.HorizontalAlignment = Element.ALIGN_LEFT;
                        table1.DefaultCell.Border = 0;
                        table1.SpacingBefore = 0f;
                        table1.SpacingAfter = 0f;

                        PdfPCell cell1 = new PdfPCell();
                        Paragraph paragraph1 = new Paragraph();

                        cell1 = new PdfPCell();
                        cell1.BorderWidthLeft = 1f;
                        cell1.BorderWidthRight = 1f;
                        cell1.BorderWidthTop = 1f;
                        cell1.BorderWidthBottom = 1f;
                        cell1.MinimumHeight = 30f;
                        paragraph1 = new Paragraph("Nomor Registrasi Memo/Registration Number:" + letterDetail.letter.letterDeliberationNumber, normalDeliberation);
                        paragraph1.Alignment = Element.ALIGN_LEFT;
                        cell1.AddElement(paragraph1);
                        table1.AddCell(cell1);

                        cell1 = new PdfPCell();
                        cell1.BorderWidthLeft = 0f;
                        cell1.BorderWidthRight = 1f;
                        cell1.BorderWidthTop = 1f;
                        cell1.BorderWidthBottom = 1f;
                        cell1.MinimumHeight = 30f;
                        paragraph1 = new Paragraph("Tanggal Registrasi Memo/Date of Memo Registration : " + Convert.ToDateTime(letterDetail.letter.letterDate).ToString("dd MMMM yyyy"), normalDeliberation);
                        paragraph1.Alignment = Element.ALIGN_LEFT;
                        cell1.AddElement(paragraph1);
                        table1.AddCell(cell1);

                        cell1 = new PdfPCell();
                        cell1.BorderWidthLeft = 1f;
                        cell1.BorderWidthRight = 1f;
                        cell1.BorderWidthTop = 0f;
                        cell1.BorderWidthBottom = 1f;
                        cell1.MinimumHeight = 100f;
                        paragraph1 = new Paragraph("Nomor Memo : " + letterDetail.letter.letterNumber, normalDeliberation);
                        paragraph1.Alignment = Element.ALIGN_LEFT;
                        cell1.AddElement(paragraph1);
                        table1.AddCell(cell1);

                        cell1 = new PdfPCell();
                        cell1.BorderWidthLeft = 0f;
                        cell1.BorderWidthRight = 1f;
                        cell1.BorderWidthTop = 0f;
                        cell1.BorderWidthBottom = 1f;
                        cell1.MinimumHeight = 100f;
                        paragraph1 = new Paragraph("(Ringkasan Memo/Summary of the proposal) \n" + letterDetail.letterContent.summary, normalDeliberation);
                        paragraph1.Alignment = Element.ALIGN_LEFT;
                        cell1.AddElement(paragraph1);
                        table1.AddCell(cell1);

                        cell1 = new PdfPCell();
                        cell1.BorderWidthLeft = 1f;
                        cell1.BorderWidthRight = 1f;
                        cell1.BorderWidthTop = 0f;
                        cell1.BorderWidthBottom = 1f;
                        cell1.MinimumHeight = 100f;
                        paragraph1 = new Paragraph("Deliberation:", normalDeliberation);
                        paragraph1.Alignment = Element.ALIGN_LEFT;
                        cell1.AddElement(paragraph1);
                        no = 1;
                        foreach (var item in letterDetail.delibration)
                        {
                            if (letterDetail.receiver.Count() > 1)
                            {
                                paragraph1 = new Paragraph(no + ". " + item.fullname + " - " + item.positionName, normalDeliberation);
                                paragraph1.Alignment = Element.ALIGN_LEFT;
                                cell1.AddElement(paragraph1);
                            }
                            else
                            {
                                paragraph1 = new Paragraph(item.fullname + " - " + item.positionName, normalDeliberation);
                                paragraph1.Alignment = Element.ALIGN_LEFT;
                                cell1.AddElement(paragraph1);
                            }
                            no++;
                        }
                        table1.AddCell(cell1);

                        cell1 = new PdfPCell();
                        cell1.BorderWidthLeft = 0f;
                        cell1.BorderWidthRight = 1f;
                        cell1.BorderWidthTop = 0f;
                        cell1.BorderWidthBottom = 1f;
                        cell1.MinimumHeight = 100f;
                        paragraph1 = new Paragraph("Catatan / Notes : \n", underlineDeliberation);
                        paragraph1.Alignment = Element.ALIGN_LEFT;
                        cell1.AddElement(paragraph1);
                        no = 1;
                        foreach (var item in letterDetail.delibration)
                        {
                            if (letterDetail.receiver.Count() > 1)
                            {
                                paragraph1 = new Paragraph(no + ". " + item.commentdlbrt, normalDeliberation);
                                paragraph1.Alignment = Element.ALIGN_LEFT;
                                cell1.AddElement(paragraph1);
                            }
                            else
                            {
                                paragraph1 = new Paragraph(item.commentdlbrt, normalDeliberation);
                                paragraph1.Alignment = Element.ALIGN_LEFT;
                                cell1.AddElement(paragraph1);
                            }
                            no++;
                        }
                        table1.AddCell(cell1);

                        pdfDoc.Add(table1);

                        //var checkerList = letterDetail.checker.Where(p => p.ordernumber != 1).OrderBy(p => p.idLevelChecker).ToList();
                        var checkerList = letterDetail.checker.OrderBy(p => p.idLevelChecker).ToList();
                        foreach (var item in checkerList)
                        {
                            var getLogComment = letterDetail.log.Where(p => p.idUserLog == item.idUserChecker && p.description.Contains("pemeriksa ke")).OrderByDescending(p => p.createdOn).FirstOrDefault();
                            var comment = "";
                            string approveDate = "";
                            if (getLogComment != null)
                            {
                                comment = getLogComment.comment;
                                approveDate = Convert.ToDateTime(getLogComment.createdOn).ToString("dd MMMM yyyy");
                            }
                            table1 = new PdfPTable(3);
                            width = new float[] { 1f, 2f, 1f };
                            table1.WidthPercentage = 100;
                            table1.SetWidths(width);
                            table1.HorizontalAlignment = Element.ALIGN_LEFT;
                            table1.DefaultCell.Border = 0;
                            table1.SpacingBefore = 0f;
                            table1.SpacingAfter = 0f;

                            cell1 = new PdfPCell();
                            cell1.Rowspan = 2;
                            cell1.BorderWidthLeft = 1f;
                            cell1.BorderWidthRight = 1f;
                            cell1.BorderWidthTop = 0f;
                            cell1.BorderWidthBottom = 1f;
                            cell1.MinimumHeight = 120f;
                            if (letterDetail.letter.statusCode == 2)
                                {
                                    paragraph1 = new Paragraph("Belum diputuskan / Undecided \n", normalDeliberation);
                                }
                                else if (letterDetail.letter.statusCode == 5)
                                {
                                    paragraph1 = new Paragraph("Ditolak / Reject \n", normalDeliberation);
                                }
                                else
                                {
                                   if(item.idLevelChecker == 2 ||item.idLevelChecker == 1)
                                   { 
                                       paragraph1 = new Paragraph("Disetujui / Approved \n", normalDeliberation);
                                   }
                                   else
                                   {
                                       paragraph1 = new Paragraph("Mengetahui / Understand \n", normalDeliberation);
                                       
                                   }
                                }

                            paragraph1.Alignment = Element.ALIGN_LEFT;
                            cell1.AddElement(paragraph1);
                            paragraph1 = new Paragraph(item.positionName, normalDeliberation);
                            paragraph1.Alignment = Element.ALIGN_LEFT;
                            cell1.AddElement(paragraph1);
                            table1.AddCell(cell1);

                            cell1 = new PdfPCell();
                            cell1.Rowspan = 2;
                            cell1.BorderWidthLeft = 0f;
                            cell1.BorderWidthRight = 1f;
                            cell1.BorderWidthTop = 0f;
                            cell1.BorderWidthBottom = 1f;
                            cell1.MinimumHeight = 120f;
                            paragraph1 = new Paragraph("Catatan / Notes : \n", underlineDeliberation);
                            paragraph1.Alignment = Element.ALIGN_LEFT;
                            cell1.AddElement(paragraph1);
                            paragraph1 = new Paragraph(comment, normalDeliberation);
                            paragraph1.Alignment = Element.ALIGN_LEFT;
                            cell1.AddElement(paragraph1);
                            table1.AddCell(cell1);

                            cell1 = new PdfPCell();
                            cell1.BorderWidthLeft = 0f;
                            cell1.BorderWidthRight = 1f;
                            cell1.BorderWidthTop = 0f;
                            cell1.BorderWidthBottom = 1f;
                            cell1.MinimumHeight = 40f;
                            paragraph1 = new Paragraph("Tanggal : ", underlineDeliberation);
                            paragraph1.Alignment = Element.ALIGN_LEFT;
                            //cell1.AddElement(paragraph);
                            paragraph1 = new Paragraph(approveDate, normalDeliberation);
                            paragraph1.Alignment = Element.ALIGN_LEFT;
                            cell1.AddElement(paragraph1);
                            table1.AddCell(cell1);

                            cell1 = new PdfPCell();
                            cell1.BorderWidthLeft = 0f;
                            cell1.BorderWidthRight = 1f;
                            cell1.BorderWidthTop = 0f;
                            cell1.BorderWidthBottom = 1f;
                            paragraph1 = new Paragraph("Tanda Tangan : \n", underlineDeliberation);
                            paragraph1.Alignment = Element.ALIGN_LEFT;
                            cell1.AddElement(paragraph1);
                            if (letterDetail.letter.letterNumber != "NO_LETTER")
                            {
                                if (letterDetail.letter.signatureType == 1)
                                {
                                    Image imagettd = GetSignatureImage(item.nip);
                                    //cell.AddElement(imagettd);
                                    if (imagettd != null)
                                    {
                                        cell1.AddElement(imagettd);
                                    }
                                    else
                                    {
                                        var img = "File tanda tangan tidak ditemukan";
                                        paragraph1 = new Paragraph(img, normal);
                                        cell1.AddElement(paragraph1);
                                    }
                                }
                                else if (letterDetail.letter.signatureType == 2)
                                {

                                    Image QRCodeSignatureApprover = GetBarcodeSignature("NIP : " + item.nip + "\nNAMA : " + item.fullname + "\nJabatan : " + item.positionName);
                                    cell1.AddElement(QRCodeSignatureApprover);
                                }
                                else if (letterDetail.letter.signatureType == 3)
                                {
                                    var img = "";
                                    paragraph = new Paragraph(img, normal);
                                    cell.AddElement(paragraph);
                                    Paragraph linessia = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(2F, 100.0F, colorWhite, Element.ALIGN_CENTER, 1)));
                                    linessia.SpacingBefore = 0f;
                                    linessia.SetLeading(0.5F, 0.5F);
                                    pdfDoc.Add(linessia);

                                    Paragraph linessiaa = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(2F, 100.0F, colorWhite, Element.ALIGN_CENTER, 1)));
                                    linessiaa.SpacingBefore = 0f;
                                    linessiaa.SetLeading(0.5F, 0.5F);
                                    pdfDoc.Add(linessiaa);

                                    Paragraph linessiaaa = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(2F, 100.0F, colorWhite, Element.ALIGN_CENTER, 1)));
                                    linessiaaa.SpacingBefore = 0f;
                                    linessiaaa.SetLeading(0.5F, 0.5F);
                                    pdfDoc.Add(linessiaaa);
                                }
                                else
                                {
                                    //innertable1 = new PdfPtable1(2);
                                    //float[] widthinner = new float[] { 0.3f, 0.3f };
                                    //innertable1.WidthPercentage = 100;
                                    //innertable1.SetWidths(widthinner);
                                    //innertable1.HorizontalAlignment = Element.ALIGN_LEFT;
                                    //innertable1.DefaultCell.Border = 1;
                                    //innertable1.DefaultCell.PaddingBottom = 8;

                                    //innercell = new PdfPCell();
                                    //innercell.BorderWidthLeft = 1f;
                                    //innercell.BorderWidthRight = 1f;
                                    //innercell.BorderWidthTop = 1f;
                                    //innercell.BorderWidthBottom = 1f;




                                    Image imagettd = GetSignatureImage(item.nip);
                                    cell1.AddElement(imagettd);

                                    Image QRCodeSignatureApprover = GetBarcodeSignature("NIP : " + item.nip + "\nNAMA : " + item.fullname + "\nJabatan : " + item.positionName);
                                    cell1.AddElement(QRCodeSignatureApprover);
                                    //if (imagettd !=null)
                                    //{
                                    //    cell.AddElement(imagettd);

                                    //    Image QRCodeSignatureApprover = GetBarcodeSignature("NIP : " + item.nip + "\nNAMA : " + item.fullname + "\nJabatan : " + item.positionName);
                                    //    cell.AddElement(QRCodeSignatureApprover);
                                    //}
                                    //else
                                    //{
                                    //    var img = "File tanda tangan tidak di temukan";
                                    //    paragraph = new Paragraph(img, normal);

                                    //    cell.AddElement(paragraph);
                                    //    Image QRCodeSignatureApprover = GetBarcodeSignature("NIP : " + item.nip + "\nNAMA : " + item.fullname + "\nJabatan : " + item.positionName);
                                    //    cell.AddElement(QRCodeSignatureApprover);
                                    //}

                                }
                            }

                            paragraph1 = new Paragraph(item.fullname, bold);
                            paragraph1.Alignment = Element.ALIGN_CENTER;
                            cell1.AddElement(paragraph1);
                            table1.AddCell(cell1);


                            pdfDoc.Add(table1);
                        }


                        #endregion

                        pdfDoc.Close();

                        var dateLetterFormat = Convert.ToDateTime(letterDetail.letter.letterDate).ToString("dd MMMM yyyy");
                        var letterNumber = letterDetail.letter.letterNumber;
                        byte[] bytess = ms.ToArray();
                        byte[] bytes = AddPageNumbers(bytess, dateLetterFormat, letterNumber, "Memo");
                        ms.Close();

                        return File(bytes.ToArray(), "application/pdf", fileName);
                    }

                #endregion
            }

            //Done(Masi Ikuti Template Memo Direksi)
            private FileContentResult PrivSuratPernyataan(string letternumber, OutputDetailMemo letterDetail, string fileName)
            {
                #region Surat Pernyataan Divisi

                StringBuilder sb = new StringBuilder();
                sb.Append("<div style='padding-left:2px;'>" + letterDetail.letterContent.letterContent + "</div>");
                StringReader sr = new StringReader(sb.ToString());
                //Document pdfDoc = new Document(PageSize.A4, 25, 25, 25, 15);
                //Document pdfDoc = new Document(PageSize.A4, 56, 56, 113, 97);
                Document pdfDoc = new Document(PageSize.A4, 56.7f, 56.7f, 66.5f, 99.225f);
                HTMLWorker htmlparser = new HTMLWorker(pdfDoc);


                using (MemoryStream ms = new MemoryStream())
                {
                    PdfWriter pdfWriter = PdfWriter.GetInstance(pdfDoc, ms);
                    pdfDoc.Open();
                    //Font
                    int no;
                    var fontName = "Calibri";
                    string fontPath = Path.Combine(_environment.WebRootPath, "fonts/Calibri.ttf");
                    //FontFactory.Register(fontPath);

                    Font normal = FontFactory.GetFont(fontName, 12, Font.NORMAL, BaseColor.BLACK);
                    Font bold = FontFactory.GetFont(fontName, 12, Font.BOLD, BaseColor.BLACK);
                    Font underline = FontFactory.GetFont(fontName, 12, Font.UNDERLINE, BaseColor.BLACK);
                    Font underlineBold = FontFactory.GetFont(fontName, 12, Font.BOLD | Font.UNDERLINE, BaseColor.BLACK);

                    Font normalDeliberation = FontFactory.GetFont(fontName, 10, Font.NORMAL, BaseColor.BLACK);
                    Font underlineDeliberation = FontFactory.GetFont(fontName, 10, Font.UNDERLINE, BaseColor.BLACK);
                    Font underlineDeliberationBold = FontFactory.GetFont(fontName, 10, Font.BOLD | Font.UNDERLINE, BaseColor.BLACK);
                    // Colors
                    BaseColor colorBlack = new BaseColor(System.Drawing.ColorTranslator.FromHtml("#000000"));
                    BaseColor colorWhite = new BaseColor(System.Drawing.ColorTranslator.FromHtml("#FFFFFF"));



                    PdfPTable table = new PdfPTable(2);
                    float[] width = new float[] { 0.667f, 2f };
                    table.WidthPercentage = 100;
                    table.SetWidths(width);
                    table.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.DefaultCell.Border = 0;
                    table.SpacingBefore = 0f;
                    table.SpacingAfter = 0f;

                    PdfPCell cell = new PdfPCell();
                    Paragraph paragraph = new Paragraph();


                    #region header
                    //add new page

                    //table = new PdfPTable(1);
                    //width = new float[] { 2f };
                    //table.WidthPercentage = 100;
                    //table.SetWidths(width);
                    //table.HorizontalAlignment = Element.ALIGN_LEFT;
                    //table.DefaultCell.Border = 0;
                    //table.SpacingBefore = 00f;
                    //table.SpacingAfter = 0f;


                    //table.AddCell(new Phrase("MEMO", bold));
                    //pdfDoc.Add(table);

                    table = new PdfPTable(2);
                    width = new float[] { 0.2f, 1f };
                    table.WidthPercentage = 100;
                    table.SetWidths(width);
                    table.HorizontalAlignment = Element.ALIGN_RIGHT;
                    table.DefaultCell.Border = 0;
                    table.SpacingBefore = 0f;
                    table.SpacingAfter = 0f;

                    cell.Border = 0;
                    paragraph = new Paragraph("Nomor", normal);
                    paragraph.Alignment = Element.ALIGN_LEFT;
                    cell.AddElement(paragraph);
                    table.AddCell(cell);

                    //cell = new PdfPCell();
                    //cell.Border = 0;
                    //paragraph = new Paragraph(":", normal);
                    //paragraph.Alignment = Element.ALIGN_RIGHT;
                    //cell.AddElement(paragraph);
                    //table.AddCell(cell);

                    cell = new PdfPCell();
                    cell.Border = 0;
                    paragraph = new Paragraph(":"+" "+letterDetail.letter.letterNumber, normal);
                    paragraph.Alignment = Element.ALIGN_LEFT;
                    cell.AddElement(paragraph);
                    table.AddCell(cell);

                    cell = new PdfPCell();
                    cell.Border = 0;
                    paragraph = new Paragraph("Tanggal", normal);
                    paragraph.Alignment = Element.ALIGN_LEFT;
                    cell.AddElement(paragraph);
                    table.AddCell(cell);

                    //cell = new PdfPCell();
                    //cell.Border = 0;
                    //paragraph = new Paragraph(":", normal);
                    //paragraph.Alignment = Element.ALIGN_RIGHT;
                    //cell.AddElement(paragraph);
                    //table.AddCell(cell);

                    cell = new PdfPCell();
                    cell.Border = 0;
                    paragraph = new Paragraph(":"+" "+Convert.ToDateTime(letterDetail.letter.letterDate).ToString("dd MMMM yyyy"), normal);
                    paragraph.Alignment = Element.ALIGN_LEFT;
                    cell.AddElement(paragraph);
                    table.AddCell(cell);

                    cell = new PdfPCell();
                    cell.Border = 0;
                    paragraph = new Paragraph("Kepada", normal);
                    paragraph.Alignment = Element.ALIGN_LEFT;
                    cell.AddElement(paragraph);
                    table.AddCell(cell);

                    //cell = new PdfPCell();
                    //cell.Border = 0;
                    //paragraph = new Paragraph(":", normal);
                    //paragraph.Alignment = Element.ALIGN_RIGHT;
                    //cell.AddElement(paragraph);
                    //table.AddCell(cell);

                    cell = new PdfPCell();
                    cell.Border = 0;
                    no = 1;
                    foreach (var item in letterDetail.receiver)
                    {
                        if (letterDetail.receiver.Count() > 1)
                        {
                            paragraph = new Paragraph(":"+" "+no + ". " + item.fullname + " - " + item.positionName, normal);
                            paragraph.Alignment = Element.ALIGN_LEFT;
                            cell.AddElement(paragraph);
                        }
                        else
                        {
                            paragraph = new Paragraph(":"+" "+item.fullname + " - " + item.positionName, normal);
                            paragraph.Alignment = Element.ALIGN_LEFT;
                            cell.AddElement(paragraph);
                        }
                        no++;
                    }
                    table.AddCell(cell);

                    cell = new PdfPCell();
                    cell.Border = 0;
                    paragraph = new Paragraph("Tembusan", normal);
                    paragraph.Alignment = Element.ALIGN_LEFT;
                    cell.AddElement(paragraph);
                    table.AddCell(cell);

                    //cell = new PdfPCell();
                    //cell.Border = 0;
                    //paragraph = new Paragraph(":", normal);
                    //paragraph.Alignment = Element.ALIGN_RIGHT;
                    //cell.AddElement(paragraph);
                    //table.AddCell(cell);

                    cell = new PdfPCell();
                    cell.Border = 0;
                    no = 1;
                    foreach (var item in letterDetail.copy)
                    {
                        if (letterDetail.copy.Count() > 1)
                        {
                            paragraph = new Paragraph(":"+" "+no + ". " + item.fullname + " - " + item.positionName, normal);
                            paragraph.Alignment = Element.ALIGN_LEFT;
                            cell.AddElement(paragraph);
                        }
                        else
                        {
                            paragraph = new Paragraph(":"+" "+item.fullname + " - " + item.positionName, normal);
                            paragraph.Alignment = Element.ALIGN_LEFT;
                            cell.AddElement(paragraph);
                        }
                        no++;
                    }
                    if (letterDetail.copy.Count() == 0)
                    {
                        paragraph = new Paragraph("-", normal);
                        paragraph.Alignment = Element.ALIGN_LEFT;
                        cell.AddElement(paragraph);
                    }
                    table.AddCell(cell);

                    cell = new PdfPCell();
                    cell.Border = 0;
                    paragraph = new Paragraph("Dari", normal);
                    paragraph.Alignment = Element.ALIGN_LEFT;
                    cell.AddElement(paragraph);
                    table.AddCell(cell);

                    //cell = new PdfPCell();
                    //cell.Border = 0;
                    //paragraph = new Paragraph(":", normal);
                    //paragraph.Alignment = Element.ALIGN_RIGHT;
                    //cell.AddElement(paragraph);
                    //table.AddCell(cell);

                    cell = new PdfPCell();
                    cell.Border = 0;
                    paragraph = new Paragraph(":"+" "+letterDetail.sender[0].fullname + " - " + letterDetail.sender[0].positionName, normal);
                    paragraph.Alignment = Element.ALIGN_LEFT;
                    cell.AddElement(paragraph);
                    table.AddCell(cell);

                    cell = new PdfPCell();
                    cell.Border = 0;
                    paragraph = new Paragraph("Perihal", normal);
                    paragraph.Alignment = Element.ALIGN_LEFT;
                    cell.AddElement(paragraph);
                    table.AddCell(cell);

                    //cell = new PdfPCell();
                    //cell.Border = 0;
                    //paragraph = new Paragraph(":", normal);
                    //paragraph.Alignment = Element.ALIGN_RIGHT;
                    //cell.AddElement(paragraph);
                    //table.AddCell(cell);

                    cell = new PdfPCell();
                    cell.Border = 0;
                    paragraph = new Paragraph(":"+" "+letterDetail.letter.about, normal);
                    paragraph.Alignment = Element.ALIGN_LEFT;
                    cell.AddElement(paragraph);
                    table.AddCell(cell);

                    cell = new PdfPCell();
                    cell.Border = 0;
                    paragraph = new Paragraph("Lampiran", normal);
                    paragraph.Alignment = Element.ALIGN_LEFT;
                    cell.AddElement(paragraph);
                    table.AddCell(cell);

                    //cell = new PdfPCell();
                    //cell.Border = 0;
                    //paragraph = new Paragraph(":", normal);
                    //paragraph.Alignment = Element.ALIGN_RIGHT;
                    //cell.AddElement(paragraph);
                    //table.AddCell(cell);

                    var lampiran = letterDetail.letter.attachmentDesc == null ? "-" : letterDetail.letter.attachmentDesc;
                    cell = new PdfPCell();
                    cell.Border = 0;
                    paragraph = new Paragraph(":"+" "+lampiran, bold);
                    paragraph.Alignment = Element.ALIGN_LEFT;
                    cell.AddElement(paragraph);
                    table.AddCell(cell);

                    //Add table to document
                    pdfDoc.Add(table);


                    Paragraph line = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(2F, 100.0F, colorBlack, Element.ALIGN_CENTER, 1)));
                    line.SpacingBefore = 0f;
                    line.SetLeading(2F, 0.5F);
                    pdfDoc.Add(line);
                    Paragraph lines = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(2F, 100.0F, colorBlack, Element.ALIGN_CENTER, 1)));
                    lines.SpacingBefore = 0f;
                    line.SetLeading(0.5F, 0.5F);
                    pdfDoc.Add(line);

                    Paragraph liness = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(2F, 100.0F, colorWhite, Element.ALIGN_CENTER, 1)));
                    liness.SpacingBefore = 0f;
                    liness.SetLeading(0.5F, 0.5F);
                    pdfDoc.Add(liness);

                    //Paragraph linesss = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(2F, 100.0F, colorWhite, Element.ALIGN_CENTER, 1)));
                    //linesss.SpacingBefore = 0f;
                    //linesss.SetLeading(0.5F, 0.5F);
                    //pdfDoc.Add(linesss);


                    table = new PdfPTable(1);
                    width = new float[] { 2f };
                    table.WidthPercentage = 100;
                    table.SetWidths(width);
                    table.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.DefaultCell.Border = 0;
                    table.SpacingBefore = 00f;
                    table.SpacingAfter = 0f;


                    //table.AddCell(new Phrase("Dengan hormat", normal));
                    //pdfDoc.Add(table);

                    Paragraph linessss = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(2F, 100.0F, colorWhite, Element.ALIGN_CENTER, 1)));
                    linessss.SpacingBefore = 0f;
                    linessss.SetLeading(0.5F, 0.5F);
                    pdfDoc.Add(linessss);

                    Paragraph lin = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(2F, 100.0F, colorWhite, Element.ALIGN_CENTER, 1)));
                    lin.SpacingBefore = 0f;
                    lin.SetLeading(0.5F, 0.5F);
                    pdfDoc.Add(lin);

                    #endregion

                    //ADD HTML CONTENT
                    htmlparser.Parse(sr);


                    #region Tanda Tangan

                    Paragraph a = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(2F, 100.0F, colorWhite, Element.ALIGN_CENTER, 1)));
                    a.SpacingBefore = 0f;
                    a.SetLeading(0.5F, 0.5F);
                    pdfDoc.Add(a);



                    cell = new PdfPCell();
                    table.AddCell(new Phrase("", normal));

                    cell = new PdfPCell();
                    table.AddCell(new Phrase(" ", normal));
                    cell = new PdfPCell();
                    table.AddCell(new Phrase(" ", normal));
                    cell = new PdfPCell();
                    table.AddCell(new Phrase("Hormat kami, \n" + "PT BNI Life Insurance", normal));
                    //table.AddCell(new Phrase("Best Regards", normal));
                    pdfDoc.Add(table);



                    string filename = letterDetail.sender[0].nip;


                    // Tanda tangan
                    var dirChecker = letterDetail.sender.OrderBy(p => p.idUserSender).ToList();
                    //var dirChecker = letterDetail.checker.Where(p => p.idLevelChecker == 2 || p.idLevelChecker == 1).ToList();
                   
                    table = new PdfPTable(dirChecker.Count());
                    width = new float[dirChecker.Count()];
                    for (int i = 0; i < dirChecker.Count(); i++)
                    {
                        width[i] = 1f;
                    }
                    table.WidthPercentage = 100;
                    table.SetWidths(width);
                    table.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.DefaultCell.Border = 0;
                    var no1 = 0;


                    //cell = new PdfPCell();
                    //table.AddCell(new Phrase("Direksi PT BNI Life Insurance", normal));
                    foreach (var item in dirChecker)
                    {
                        cell = new PdfPCell();
                        cell.Border = 0;




                        if (letterDetail.letter.letterNumber != "NO_LETTER")
                        {
                            if (letterDetail.letter.signatureType == 1)
                            {

                                Image imagettd = GetSignatureImage(item.nip);
                                //cell.AddElement(imagettd);
                                if (imagettd != null)
                                {
                                    cell.AddElement(imagettd);
                                }
                                else
                                {
                                    var img = "File tanda tangan tidak ditemukan";
                                    paragraph = new Paragraph(img, normal);
                                    cell.AddElement(paragraph);
                                }

                            }
                            else if (letterDetail.letter.signatureType == 2)
                            {


                                Image QRCodeSignatureApprover = GetBarcodeSignature("NIP : " + item.nip + "\nNAMA : " + item.fullname + "\nJabatan : " + item.positionName);
                                QRCodeSignatureApprover.ScaleAbsoluteHeight(10f);
                                cell.AddElement(QRCodeSignatureApprover);
                            }
                            else if (letterDetail.letter.signatureType == 3)
                            {
                                var img = "";
                                paragraph = new Paragraph(img, normal);
                                cell.AddElement(paragraph);
                                Paragraph linessia = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(2F, 100.0F, colorWhite, Element.ALIGN_CENTER, 1)));
                                linessia.SpacingBefore = 0f;
                                linessia.SetLeading(0.5F, 0.5F);
                                pdfDoc.Add(linessia);

                                Paragraph linessiaa = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(2F, 100.0F, colorWhite, Element.ALIGN_CENTER, 1)));
                                linessiaa.SpacingBefore = 0f;
                                linessiaa.SetLeading(0.5F, 0.5F);
                                pdfDoc.Add(linessiaa);

                                Paragraph linessiaaa = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(2F, 100.0F, colorWhite, Element.ALIGN_CENTER, 1)));
                                linessiaaa.SpacingBefore = 0f;
                                linessiaaa.SetLeading(0.5F, 0.5F);
                                pdfDoc.Add(linessiaaa);
                            }
                            else
                            {

                                Image imagettd = GetSignatureImage(item.nip);
                                cell.AddElement(imagettd);
                                Image QRCodeSignatureApprover = GetBarcodeSignature("NIP : " + item.nip + "\nNAMA : " + item.fullname + "\nJabatan : " + item.positionName);
                                cell.AddElement(QRCodeSignatureApprover);
                                //if (imagettd != null)
                                //{
                                //    cell.AddElement(imagettd);
                                //    Image QRCodeSignatureApprover = GetBarcodeSignature("NIP : " + item.nip + "\nNAMA : " + item.fullname + "\nJabatan : " + item.positionName);
                                //    cell.AddElement(QRCodeSignatureApprover);
                                //}
                                //else
                                //{
                                //    var img = "File tanda tangan tidak di temukan";
                                //    paragraph = new Paragraph(img, normal);
                                //    cell.AddElement(paragraph);
                                //    Image QRCodeSignatureApprover = GetBarcodeSignature("NIP : " + item.nip + "\nNAMA : " + item.fullname + "\nJabatan : " + item.positionName);
                                //    cell.AddElement(QRCodeSignatureApprover);
                                //}

                            }
                            no1++;
                        }

                        paragraph = new Paragraph(item.fullname, underlineBold);
                        cell.AddElement(paragraph);
                        paragraph = new Paragraph(item.positionName, bold);
                        cell.AddElement(paragraph);

                        table.AddCell(cell);
                    }
                    // end tanda tangan
                    pdfDoc.Add(table);

                    #endregion

                    pdfDoc.NewPage();

                    #region Lampiran

                    PdfPTable table1 = new PdfPTable(2);
                    float[] width1 = new float[] { 0.667f, 2f };
                    table1.WidthPercentage = 100;
                    table1.SetWidths(width1);
                    table1.HorizontalAlignment = Element.ALIGN_LEFT;
                    table1.DefaultCell.Border = 0;
                    table1.SpacingBefore = 0f;
                    table1.SpacingAfter = 0f;

                    PdfPCell cell1 = new PdfPCell();
                    Paragraph paragraph1 = new Paragraph();

                    cell1 = new PdfPCell();
                    cell1.BorderWidthLeft = 1f;
                    cell1.BorderWidthRight = 1f;
                    cell1.BorderWidthTop = 1f;
                    cell1.BorderWidthBottom = 1f;
                    cell1.MinimumHeight = 30f;
                    paragraph1 = new Paragraph("Nomor Registrasi Memo/Registration Number:" + letterDetail.letter.letterDeliberationNumber, normalDeliberation);
                    paragraph1.Alignment = Element.ALIGN_LEFT;
                    cell1.AddElement(paragraph1);
                    table1.AddCell(cell1);

                    cell1 = new PdfPCell();
                    cell1.BorderWidthLeft = 0f;
                    cell1.BorderWidthRight = 1f;
                    cell1.BorderWidthTop = 1f;
                    cell1.BorderWidthBottom = 1f;
                    cell1.MinimumHeight = 30f;
                    paragraph1 = new Paragraph("Tanggal Registrasi Memo/Date of Memo Registration : " + Convert.ToDateTime(letterDetail.letter.letterDate).ToString("dd MMMM yyyy"), normalDeliberation);
                    paragraph1.Alignment = Element.ALIGN_LEFT;
                    cell1.AddElement(paragraph1);
                    table1.AddCell(cell1);

                    cell1 = new PdfPCell();
                    cell1.BorderWidthLeft = 1f;
                    cell1.BorderWidthRight = 1f;
                    cell1.BorderWidthTop = 0f;
                    cell1.BorderWidthBottom = 1f;
                    cell1.MinimumHeight = 100f;
                    paragraph1 = new Paragraph("Nomor Memo : " + letterDetail.letter.letterNumber, normalDeliberation);
                    paragraph1.Alignment = Element.ALIGN_LEFT;
                    cell1.AddElement(paragraph1);
                    table1.AddCell(cell1);

                    cell1 = new PdfPCell();
                    cell1.BorderWidthLeft = 0f;
                    cell1.BorderWidthRight = 1f;
                    cell1.BorderWidthTop = 0f;
                    cell1.BorderWidthBottom = 1f;
                    cell1.MinimumHeight = 100f;
                    paragraph1 = new Paragraph("(Ringkasan Memo/Summary of the proposal) \n" + letterDetail.letterContent.summary, normalDeliberation);
                    paragraph1.Alignment = Element.ALIGN_LEFT;
                    cell1.AddElement(paragraph1);
                    table1.AddCell(cell1);

                    cell1 = new PdfPCell();
                    cell1.BorderWidthLeft = 1f;
                    cell1.BorderWidthRight = 1f;
                    cell1.BorderWidthTop = 0f;
                    cell1.BorderWidthBottom = 1f;
                    cell1.MinimumHeight = 100f;
                    paragraph1 = new Paragraph("Deliberation:", normalDeliberation);
                    paragraph1.Alignment = Element.ALIGN_LEFT;
                    cell1.AddElement(paragraph1);
                    no = 1;
                    foreach (var item in letterDetail.delibration)
                    {
                        if (letterDetail.receiver.Count() > 1)
                        {
                            paragraph1 = new Paragraph(no + ". " + item.fullname + " - " + item.positionName, normalDeliberation);
                            paragraph1.Alignment = Element.ALIGN_LEFT;
                            cell1.AddElement(paragraph1);
                        }
                        else
                        {
                            paragraph1 = new Paragraph(item.fullname + " - " + item.positionName, normalDeliberation);
                            paragraph1.Alignment = Element.ALIGN_LEFT;
                            cell1.AddElement(paragraph1);
                        }
                        no++;
                    }
                    table1.AddCell(cell1);

                    cell1 = new PdfPCell();
                    cell1.BorderWidthLeft = 0f;
                    cell1.BorderWidthRight = 1f;
                    cell1.BorderWidthTop = 0f;
                    cell1.BorderWidthBottom = 1f;
                    cell1.MinimumHeight = 100f;
                    paragraph1 = new Paragraph("Catatan / Notes : \n", underlineDeliberation);
                    paragraph1.Alignment = Element.ALIGN_LEFT;
                    cell1.AddElement(paragraph1);
                    no = 1;
                    foreach (var item in letterDetail.delibration)
                    {
                        if (letterDetail.receiver.Count() > 1)
                        {
                            paragraph1 = new Paragraph(no + ". " + item.commentdlbrt, normalDeliberation);
                            paragraph1.Alignment = Element.ALIGN_LEFT;
                            cell1.AddElement(paragraph1);
                        }
                        else
                        {
                            paragraph1 = new Paragraph(item.commentdlbrt, normalDeliberation);
                            paragraph1.Alignment = Element.ALIGN_LEFT;
                            cell1.AddElement(paragraph1);
                        }
                        no++;
                    }
                    table1.AddCell(cell1);

                    pdfDoc.Add(table1);

                    //var checkerList = letterDetail.checker.Where(p => p.ordernumber != 1).OrderBy(p => p.idLevelChecker).ToList();
                    var checkerList = letterDetail.checker.OrderBy(p => p.idLevelChecker).ToList();
                    foreach (var item in checkerList)
                    {
                        var getLogComment = letterDetail.log.Where(p => p.idUserLog == item.idUserChecker && p.description.Contains("pemeriksa ke")).OrderByDescending(p => p.createdOn).FirstOrDefault();
                        var comment = "";
                        string approveDate = "";
                        if (getLogComment != null)
                        {
                            comment = getLogComment.comment;
                            approveDate = Convert.ToDateTime(getLogComment.createdOn).ToString("dd MMMM yyyy");
                        }
                        table1 = new PdfPTable(3);
                        width = new float[] { 1f, 2f, 1f };
                        table1.WidthPercentage = 100;
                        table1.SetWidths(width);
                        table1.HorizontalAlignment = Element.ALIGN_LEFT;
                        table1.DefaultCell.Border = 0;
                        table1.SpacingBefore = 0f;
                        table1.SpacingAfter = 0f;

                        cell1 = new PdfPCell();
                        cell1.Rowspan = 2;
                        cell1.BorderWidthLeft = 1f;
                        cell1.BorderWidthRight = 1f;
                        cell1.BorderWidthTop = 0f;
                        cell1.BorderWidthBottom = 1f;
                        cell1.MinimumHeight = 120f;
                        if (letterDetail.letter.statusCode == 2)
                                {
                                    paragraph1 = new Paragraph("Belum diputuskan / Undecided \n", normalDeliberation);
                                }
                                else if (letterDetail.letter.statusCode == 5)
                                {
                                    paragraph1 = new Paragraph("Ditolak / Reject \n", normalDeliberation);
                                }
                                else
                                {
                                   if(item.idLevelChecker == 2 ||item.idLevelChecker == 1)
                                   { 
                                       paragraph1 = new Paragraph("Disetujui / Approved \n", normalDeliberation);
                                   }
                                   else
                                   {
                                       paragraph1 = new Paragraph("Mengetahui / Understand \n", normalDeliberation);
                                       
                                   }
                                }

                        paragraph1.Alignment = Element.ALIGN_LEFT;
                        cell1.AddElement(paragraph1);
                        paragraph1 = new Paragraph(item.positionName, normalDeliberation);
                        paragraph1.Alignment = Element.ALIGN_LEFT;
                        cell1.AddElement(paragraph1);
                        table1.AddCell(cell1);

                        cell1 = new PdfPCell();
                        cell1.Rowspan = 2;
                        cell1.BorderWidthLeft = 0f;
                        cell1.BorderWidthRight = 1f;
                        cell1.BorderWidthTop = 0f;
                        cell1.BorderWidthBottom = 1f;
                        cell1.MinimumHeight = 120f;
                        paragraph1 = new Paragraph("Catatan / Notes : \n", underlineDeliberation);
                        paragraph1.Alignment = Element.ALIGN_LEFT;
                        cell1.AddElement(paragraph1);
                        paragraph1 = new Paragraph(comment, normalDeliberation);
                        paragraph1.Alignment = Element.ALIGN_LEFT;
                        cell1.AddElement(paragraph1);
                        table1.AddCell(cell1);

                        cell1 = new PdfPCell();
                        cell1.BorderWidthLeft = 0f;
                        cell1.BorderWidthRight = 1f;
                        cell1.BorderWidthTop = 0f;
                        cell1.BorderWidthBottom = 1f;
                        cell1.MinimumHeight = 40f;
                        paragraph1 = new Paragraph("Tanggal : ", underlineDeliberation);
                        paragraph1.Alignment = Element.ALIGN_LEFT;
                        //cell1.AddElement(paragraph);
                        paragraph1 = new Paragraph(approveDate, normalDeliberation);
                        paragraph1.Alignment = Element.ALIGN_LEFT;
                        cell1.AddElement(paragraph1);
                        table1.AddCell(cell1);

                        cell1 = new PdfPCell();
                        cell1.BorderWidthLeft = 0f;
                        cell1.BorderWidthRight = 1f;
                        cell1.BorderWidthTop = 0f;
                        cell1.BorderWidthBottom = 1f;
                        paragraph1 = new Paragraph("Tanda Tangan : \n", underlineDeliberation);
                        paragraph1.Alignment = Element.ALIGN_LEFT;
                        cell1.AddElement(paragraph1);
                        if (letterDetail.letter.letterNumber != "NO_LETTER")
                        {
                            if (letterDetail.letter.signatureType == 1)
                            {
                                Image imagettd = GetSignatureImage(item.nip);
                                //cell.AddElement(imagettd);
                                if (imagettd != null)
                                {
                                    cell1.AddElement(imagettd);
                                }
                                else
                                {
                                    var img = "File tanda tangan tidak ditemukan";
                                    paragraph1 = new Paragraph(img, normal);
                                    cell1.AddElement(paragraph1);
                                }
                            }
                            else if (letterDetail.letter.signatureType == 2)
                            {

                                Image QRCodeSignatureApprover = GetBarcodeSignature("NIP : " + item.nip + "\nNAMA : " + item.fullname + "\nJabatan : " + item.positionName);
                                cell1.AddElement(QRCodeSignatureApprover);
                            }
                            else if (letterDetail.letter.signatureType == 3)
                            {
                                var img = "";
                                paragraph = new Paragraph(img, normal);
                                cell.AddElement(paragraph);
                                Paragraph linessia = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(2F, 100.0F, colorWhite, Element.ALIGN_CENTER, 1)));
                                linessia.SpacingBefore = 0f;
                                linessia.SetLeading(0.5F, 0.5F);
                                pdfDoc.Add(linessia);

                                Paragraph linessiaa = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(2F, 100.0F, colorWhite, Element.ALIGN_CENTER, 1)));
                                linessiaa.SpacingBefore = 0f;
                                linessiaa.SetLeading(0.5F, 0.5F);
                                pdfDoc.Add(linessiaa);

                                Paragraph linessiaaa = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(2F, 100.0F, colorWhite, Element.ALIGN_CENTER, 1)));
                                linessiaaa.SpacingBefore = 0f;
                                linessiaaa.SetLeading(0.5F, 0.5F);
                                pdfDoc.Add(linessiaaa);
                            }
                            else
                            {
                                //innertable1 = new PdfPtable1(2);
                                //float[] widthinner = new float[] { 0.3f, 0.3f };
                                //innertable1.WidthPercentage = 100;
                                //innertable1.SetWidths(widthinner);
                                //innertable1.HorizontalAlignment = Element.ALIGN_LEFT;
                                //innertable1.DefaultCell.Border = 1;
                                //innertable1.DefaultCell.PaddingBottom = 8;

                                //innercell = new PdfPCell();
                                //innercell.BorderWidthLeft = 1f;
                                //innercell.BorderWidthRight = 1f;
                                //innercell.BorderWidthTop = 1f;
                                //innercell.BorderWidthBottom = 1f;




                                Image imagettd = GetSignatureImage(item.nip);
                                cell1.AddElement(imagettd);

                                Image QRCodeSignatureApprover = GetBarcodeSignature("NIP : " + item.nip + "\nNAMA : " + item.fullname + "\nJabatan : " + item.positionName);
                                cell1.AddElement(QRCodeSignatureApprover);
                                //if (imagettd !=null)
                                //{
                                //    cell.AddElement(imagettd);

                                //    Image QRCodeSignatureApprover = GetBarcodeSignature("NIP : " + item.nip + "\nNAMA : " + item.fullname + "\nJabatan : " + item.positionName);
                                //    cell.AddElement(QRCodeSignatureApprover);
                                //}
                                //else
                                //{
                                //    var img = "File tanda tangan tidak di temukan";
                                //    paragraph = new Paragraph(img, normal);

                                //    cell.AddElement(paragraph);
                                //    Image QRCodeSignatureApprover = GetBarcodeSignature("NIP : " + item.nip + "\nNAMA : " + item.fullname + "\nJabatan : " + item.positionName);
                                //    cell.AddElement(QRCodeSignatureApprover);
                                //}

                            }
                        }

                        paragraph1 = new Paragraph(item.fullname, bold);
                        paragraph1.Alignment = Element.ALIGN_CENTER;
                        cell1.AddElement(paragraph1);
                        table1.AddCell(cell1);


                        pdfDoc.Add(table1);
                    }


                    #endregion

                    pdfDoc.Close();

                    var dateLetterFormat = Convert.ToDateTime(letterDetail.letter.letterDate).ToString("dd MMMM yyyy");
                    var letterNumber = letterDetail.letter.letterNumber;
                    byte[] bytess = ms.ToArray();
                    byte[] bytes = AddPageNumbers(bytess, dateLetterFormat, letterNumber, "Memo");
                    ms.Close();

                    return File(bytes.ToArray(), "application/pdf", fileName);
                }


                #endregion
            }

            // Done
            private FileContentResult PrivSuratMemoDivisi(string letternumber, OutputDetailMemo letterDetail, string fileName)
            {
                #region Surat Memo Divisi

                    StringBuilder sb = new StringBuilder();
                    sb.Append("<div style='padding-left:2px;'>" + letterDetail.letterContent.letterContent + "</div>");
                    StringReader sr = new StringReader(sb.ToString());
                    //Document pdfDoc = new Document(PageSize.A4, 25, 25, 25, 15);
                    //Document pdfDoc = new Document(PageSize.A4, 56.7f, 56.7f, 113.4f, 99.225f);
                    Document pdfDoc = new Document(PageSize.A4, 56.7f, 56.7f, 113.4f, 99.225f);
                    
                    HTMLWorker htmlparser = new HTMLWorker(pdfDoc);


                    using (MemoryStream ms = new MemoryStream())
                    {
                        PdfWriter pdfWriter = PdfWriter.GetInstance(pdfDoc, ms);
                        //pdfWriter.PageEvent = new PDFHeaderEvent();
                        //pdfWriter.PageEvent = new PDFHeaderEvent2();
                        pdfDoc.Open();
                        //Font
                        int no;
                        var fontName = "Calibri";
                        string fontPath = Path.Combine(_environment.WebRootPath, "fonts/Calibri.ttf");
                        //FontFactory.Register(fontPath);

                        Font normal = FontFactory.GetFont(fontName, 12, Font.NORMAL, BaseColor.BLACK);
                        Font bold = FontFactory.GetFont(fontName, 12, Font.BOLD, BaseColor.BLACK);
                        Font underline = FontFactory.GetFont(fontName, 12, Font.UNDERLINE, BaseColor.BLACK);
                        Font underlineBold = FontFactory.GetFont(fontName, 12, Font.BOLD | Font.UNDERLINE, BaseColor.BLACK);
                        Font boldMemo = FontFactory.GetFont(fontName, 14, Font.BOLD, BaseColor.BLACK);

                        Font normalDeliberation = FontFactory.GetFont(fontName, 10, Font.NORMAL, BaseColor.BLACK);
                        Font underlineDeliberation = FontFactory.GetFont(fontName, 10, Font.UNDERLINE, BaseColor.BLACK);
                        Font underlineDeliberationBold = FontFactory.GetFont(fontName, 10, Font.BOLD | Font.UNDERLINE, BaseColor.BLACK);
                        // Colors
                        BaseColor colorBlack = new BaseColor(System.Drawing.ColorTranslator.FromHtml("#000000"));
                        BaseColor colorWhite = new BaseColor(System.Drawing.ColorTranslator.FromHtml("#FFFFFF"));
                        
                

                        PdfPTable table = new PdfPTable(2);
                        float[] width = new float[] { 0.667f, 2f };
                        table.WidthPercentage = 100;
                        table.SetWidths(width);
                        table.DefaultCell.Border = 0;
                        table.SpacingBefore = 0f;
                        table.SpacingAfter = 0f;

                        PdfPCell cell = new PdfPCell();
                        Paragraph paragraph = new Paragraph();


                       #region header

                       //cell = new PdfPCell();
                       //cell.Border = Rectangle.NO_BORDER;
                       //cell.HorizontalAlignment= Element.ALIGN_LEFT;
                       //cell.Phrase = new Phrase("", bold);
                       //table.AddCell(cell);

                       //Image logo = GetLogoImage("logobni");
                       //logo.ScalePercent(55);
                       //cell = new PdfPCell(logo);
                       //cell.Border = Rectangle.NO_BORDER;
                       //cell.HorizontalAlignment= Element.ALIGN_RIGHT;
                       //table.AddCell(cell);
                       //pdfDoc.Add(table);

                       //table = new PdfPTable(1);
                       //width = new float[] { 2f };
                       //table.WidthPercentage = 120;
                       //table.SetWidths(width);
                       //table.HorizontalAlignment = Element.ALIGN_LEFT;
                       //table.DefaultCell.Border = 0;
                       //table.SpacingBefore = 00f;
                       //table.SpacingAfter = 0f;


                       //table.AddCell(new Phrase("MEMO", boldMemo));
                       //pdfDoc.Add(table);
                       
                       table = new PdfPTable(2);
                       width = new float[] { 0.2f, 1f };
                       table.WidthPercentage = 100;
                       table.SetWidths(width);
                       table.HorizontalAlignment = Element.ALIGN_RIGHT;
                       table.DefaultCell.Border = 0;
                       table.SpacingBefore = 0f;
                       table.SpacingAfter = 0f;

                       cell.Border = 0;
                       paragraph = new Paragraph("Nomor", normal);
                       paragraph.Alignment = Element.ALIGN_LEFT;
                       cell.AddElement(paragraph);
                       table.AddCell(cell);

                       //cell = new PdfPCell();
                       //cell.Border = 0;
                       //paragraph = new Paragraph(":", normal);
                       //paragraph.Alignment = Element.ALIGN_RIGHT;
                       //cell.AddElement(paragraph);
                       //table.AddCell(cell);

                       cell = new PdfPCell();
                       cell.Border = 0;
                       paragraph = new Paragraph(":"+" "+letterDetail.letter.letterNumber, normal);
                       paragraph.Alignment = Element.ALIGN_LEFT;
                       cell.AddElement(paragraph);
                       table.AddCell(cell);

                       cell = new PdfPCell();
                       cell.Border = 0;
                       paragraph = new Paragraph("Tanggal", normal);
                       paragraph.Alignment = Element.ALIGN_LEFT;
                       cell.AddElement(paragraph);
                       table.AddCell(cell);

                       //cell = new PdfPCell();
                       //cell.Border = 0;
                       //paragraph = new Paragraph(":", normal);
                       //paragraph.Alignment = Element.ALIGN_RIGHT;
                       //cell.AddElement(paragraph);
                       //table.AddCell(cell);

                       cell = new PdfPCell();
                       cell.Border = 0;
                       paragraph = new Paragraph(":"+" "+Convert.ToDateTime(letterDetail.letter.letterDate).ToString("dd MMMM yyyy"), normal);
                       paragraph.Alignment = Element.ALIGN_LEFT;
                       cell.AddElement(paragraph);
                       table.AddCell(cell);

                       cell = new PdfPCell();
                       cell.Border = 0;
                       paragraph = new Paragraph("Kepada", normal);
                       paragraph.Alignment = Element.ALIGN_LEFT;
                       cell.AddElement(paragraph);
                       table.AddCell(cell);

                       //cell = new PdfPCell();
                       //cell.Border = 0;
                       //paragraph = new Paragraph(":", normal);
                       //paragraph.Alignment = Element.ALIGN_RIGHT;
                       //cell.AddElement(paragraph);
                       //table.AddCell(cell);

                       cell = new PdfPCell();
                       cell.Border = 0;
                       no = 1;
                       foreach (var item in letterDetail.receiver)
                       {
                           if (letterDetail.receiver.Count() > 1)
                           {
                               paragraph = new Paragraph(":"+" "+no + ". " + item.fullname + " - " + item.positionName, normal);
                               paragraph.Alignment = Element.ALIGN_LEFT;
                               cell.AddElement(paragraph);
                           }
                           else
                           {
                               paragraph = new Paragraph(":"+" "+item.fullname + " - " + item.positionName, normal);
                               paragraph.Alignment = Element.ALIGN_LEFT;
                               cell.AddElement(paragraph);
                           }
                           no++;
                       }
                       table.AddCell(cell);

                       cell = new PdfPCell();
                       cell.Border = 0;
                       paragraph = new Paragraph("Tembusan", normal);
                       paragraph.Alignment = Element.ALIGN_LEFT;
                       cell.AddElement(paragraph);
                       table.AddCell(cell);

                       //cell = new PdfPCell();
                       //cell.Border = 0;
                       //paragraph = new Paragraph(":", normal);
                       //paragraph.Alignment = Element.ALIGN_RIGHT;
                       //cell.AddElement(paragraph);
                       //table.AddCell(cell);

                       cell = new PdfPCell();
                       cell.Border = 0;
                       no = 1;
                       foreach (var item in letterDetail.copy)
                       {
                           if (letterDetail.copy.Count() > 1)
                           {
                               paragraph = new Paragraph(":"+" "+no + ". " + item.fullname + " - " + item.positionName, normal);
                               paragraph.Alignment = Element.ALIGN_LEFT;
                               cell.AddElement(paragraph);
                           }
                           else
                           {
                               paragraph = new Paragraph(":"+" "+item.fullname + " - " + item.positionName, normal);
                               paragraph.Alignment = Element.ALIGN_LEFT;
                               cell.AddElement(paragraph);
                           }
                           no++;
                       }
                       if (letterDetail.copy.Count() == 0)
                       {
                           paragraph = new Paragraph("-", normal);
                           paragraph.Alignment = Element.ALIGN_LEFT;
                           cell.AddElement(paragraph);
                       }
                       table.AddCell(cell);

                       cell = new PdfPCell();
                       cell.Border = 0;
                       paragraph = new Paragraph("Dari", normal);
                       paragraph.Alignment = Element.ALIGN_LEFT;
                       cell.AddElement(paragraph);
                       table.AddCell(cell);

                       //cell = new PdfPCell();
                       //cell.Border = 0;
                       //paragraph = new Paragraph(":", normal);
                       //paragraph.Alignment = Element.ALIGN_RIGHT;
                       //cell.AddElement(paragraph);
                       //table.AddCell(cell);

                       cell = new PdfPCell();
                       cell.Border = 0;
                       paragraph = new Paragraph(":"+" "+letterDetail.sender[0].fullname + " - " + letterDetail.sender[0].positionName, normal);
                       paragraph.Alignment = Element.ALIGN_LEFT;
                       cell.AddElement(paragraph);
                       table.AddCell(cell);

                       cell = new PdfPCell();
                       cell.Border = 0;
                       paragraph = new Paragraph("Perihal", normal);
                       paragraph.Alignment = Element.ALIGN_LEFT;
                       cell.AddElement(paragraph);
                       table.AddCell(cell);

                       //cell = new PdfPCell();
                       //cell.Border = 0;
                       //paragraph = new Paragraph(":", normal);
                       //paragraph.Alignment = Element.ALIGN_RIGHT;
                       //cell.AddElement(paragraph);
                       //table.AddCell(cell);

                       cell = new PdfPCell();
                       cell.Border = 0;
                       paragraph = new Paragraph(":"+" "+letterDetail.letter.about, normal);
                       paragraph.Alignment = Element.ALIGN_LEFT;
                       cell.AddElement(paragraph);
                       table.AddCell(cell);

                       cell = new PdfPCell();
                       cell.Border = 0;
                       paragraph = new Paragraph("Lampiran", normal);
                       paragraph.Alignment = Element.ALIGN_LEFT;
                       cell.AddElement(paragraph);
                       table.AddCell(cell);

                       //cell = new PdfPCell();
                       //cell.Border = 0;
                       //paragraph = new Paragraph(":", normal);
                       //paragraph.Alignment = Element.ALIGN_RIGHT;
                       //cell.AddElement(paragraph);
                       //table.AddCell(cell);

                       var lampiran = letterDetail.letter.attachmentDesc == null ? "-" : letterDetail.letter.attachmentDesc;
                       cell = new PdfPCell();
                       cell.Border = 0;
                       paragraph = new Paragraph(":"+" "+lampiran, bold);
                       paragraph.Alignment = Element.ALIGN_LEFT;
                       cell.AddElement(paragraph);
                       table.AddCell(cell);

                       //Add table to document
                       pdfDoc.Add(table);

                       

                       Paragraph line = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(2F, 100.0F, colorBlack, Element.ALIGN_CENTER, 1)));
                       line.SpacingBefore = 0f;
                       line.SetLeading(2F, 0.5F);
                       pdfDoc.Add(line);
                       Paragraph lines = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(2F, 100.0F, colorBlack, Element.ALIGN_CENTER, 1)));
                       lines.SpacingBefore = 0f;
                       line.SetLeading(0.5F, 0.5F);
                       pdfDoc.Add(line);

                       Paragraph liness = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(2F, 100.0F, colorWhite, Element.ALIGN_CENTER, 1)));
                       liness.SpacingBefore = 0f;
                       liness.SetLeading(0.5F, 0.5F);
                       pdfDoc.Add(liness);

                       //Paragraph linesss = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(2F, 100.0F, colorWhite, Element.ALIGN_CENTER, 1)));
                       //linesss.SpacingBefore = 0f;
                       //linesss.SetLeading(0.5F, 0.5F);
                       //pdfDoc.Add(linesss);

                       //Paragraph linessss = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(2F, 100.0F, colorWhite, Element.ALIGN_CENTER, 1)));
                       //linessss.SpacingBefore = 0f;
                       //linessss.SetLeading(0.5F, 0.5F);
                       //pdfDoc.Add(linesss);

                       table = new PdfPTable(1);
                       width = new float[] { 2f };
                       table.WidthPercentage = 100;
                       table.SetWidths(width);
                       table.HorizontalAlignment = Element.ALIGN_LEFT;
                       table.DefaultCell.Border = 0;
                       table.SpacingBefore = 00f;
                       table.SpacingAfter = 0f;


                       table.AddCell(new Phrase("", normal));
                       pdfDoc.Add(table);


                       Paragraph lin = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(2F, 100.0F, colorWhite, Element.ALIGN_CENTER, 1)));
                       lin.SpacingBefore = 0f;
                       lin.SetLeading(0.5F, 0.5F);
                       pdfDoc.Add(lin);
                       #endregion
                       
                       //ADD HTML CONTENT
                       htmlparser.Parse(sr);



                       #region Tanda Tangan

                       Paragraph a = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(2F, 100.0F, colorWhite, Element.ALIGN_CENTER, 1)));
                       a.SpacingBefore = 0f;
                       a.SetLeading(0.5F, 0.5F);
                       pdfDoc.Add(a);



                       cell = new PdfPCell();
                       table.AddCell(new Phrase("", normal));

                       cell = new PdfPCell();
                       table.AddCell(new Phrase(" ", normal));
                       cell = new PdfPCell();
                       table.AddCell(new Phrase(" ", normal));
                       cell = new PdfPCell();
                       table.AddCell(new Phrase("Hormat kami, \n" + "PT BNI Life Insurance", normal));
                       //table.AddCell(new Phrase("Best Regards", normal));
                       pdfDoc.Add(table);



                       string filename = letterDetail.sender[0].nip;


                       // Tanda tangan
                       var dirChecker = letterDetail.sender.OrderBy(p => p.idUserSender).ToList();
                
                       //senderList
                       //var dirChecker = letterDetail.checker.OrderBy(p => p.ordernumber == 1).ToList();

                       table = new PdfPTable(dirChecker.Count());
                       width = new float[dirChecker.Count()];
                       for (int i = 0; i < dirChecker.Count(); i++)
                       {
                           width[i] = 1f;
                       }
                       table.WidthPercentage = 100;
                       table.SetWidths(width);
                       table.HorizontalAlignment = Element.ALIGN_LEFT;
                       table.DefaultCell.Border = 0;
                       var no1 = 0;


                       //cell = new PdfPCell();
                       //table.AddCell(new Phrase("Direksi PT BNI Life Insurance", normal));
                       foreach (var item in dirChecker)
                       {
                           cell = new PdfPCell();
                           cell.Border = 0;




                           if (letterDetail.letter.letterNumber != "NO_LETTER")
                           {
                               if (letterDetail.letter.signatureType == 1)
                               {

                                   Image imagettd = GetSignatureImage(item.nip);
                                   //cell.AddElement(imagettd);
                                   if (imagettd != null)
                                   {
                                        //imagettd.ScaleToFit(90f,90f);
                                       //cell.HorizontalAlignment = Element.ALIGN_CENTER;
                                       cell.AddElement(imagettd);
                                   }
                                   else
                                   {
                                       var img = "File tanda tangan tidak ditemukan";
                                       paragraph = new Paragraph(img, normal);
                                       cell.AddElement(paragraph);
                                   }

                               }
                               else if (letterDetail.letter.signatureType == 2)
                               {


                                   Image QRCodeSignatureApprover = GetBarcodeSignature("NIP : " + item.nip + "\nNAMA : " + item.fullname + "\nJabatan : " + item.positionName);
                                   QRCodeSignatureApprover.ScaleAbsoluteHeight(10f);
                                   cell.AddElement(QRCodeSignatureApprover);

                               }
                                else if (letterDetail.letter.signatureType == 3)
                                {
                                    var img = "";
                                    paragraph = new Paragraph(img, normal);
                                    cell.AddElement(paragraph);
                                    Paragraph linessia = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(2F, 100.0F, colorWhite, Element.ALIGN_CENTER, 1)));
                                    linessia.SpacingBefore = 0f;
                                    linessia.SetLeading(0.5F, 0.5F);
                                    pdfDoc.Add(linessia);

                                    Paragraph linessiaa = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(2F, 100.0F, colorWhite, Element.ALIGN_CENTER, 1)));
                                    linessiaa.SpacingBefore = 0f;
                                    linessiaa.SetLeading(0.5F, 0.5F);
                                    pdfDoc.Add(linessiaa);

                                    Paragraph linessiaaa = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(2F, 100.0F, colorWhite, Element.ALIGN_CENTER, 1)));
                                    linessiaaa.SpacingBefore = 0f;
                                    linessiaaa.SetLeading(0.5F, 0.5F);
                                    pdfDoc.Add(linessiaaa);
                                }
                                else
                               {

                                   Image imagettd = GetSignatureImage(item.nip);
                                   cell.AddElement(imagettd);
                                   Image QRCodeSignatureApprover = GetBarcodeSignature("NIP : " + item.nip + "\nNAMA : " + item.fullname + "\nJabatan : " + item.positionName);
                                   cell.AddElement(QRCodeSignatureApprover);
                                   //if (imagettd != null)
                                   //{
                                   //    cell.AddElement(imagettd);
                                   //    Image QRCodeSignatureApprover = GetBarcodeSignature("NIP : " + item.nip + "\nNAMA : " + item.fullname + "\nJabatan : " + item.positionName);
                                   //    cell.AddElement(QRCodeSignatureApprover);
                                   //}
                                   //else
                                   //{
                                   //    var img = "File tanda tangan tidak di temukan";
                                   //    paragraph = new Paragraph(img, normal);
                                   //    cell.AddElement(paragraph);
                                   //    Image QRCodeSignatureApprover = GetBarcodeSignature("NIP : " + item.nip + "\nNAMA : " + item.fullname + "\nJabatan : " + item.positionName);
                                   //    cell.AddElement(QRCodeSignatureApprover);
                                   //}

                               }
                               no1++;
                           }

                           paragraph = new Paragraph(item.fullname, underlineBold);
                           cell.AddElement(paragraph);
                           paragraph = new Paragraph(item.positionName, bold);
                           cell.AddElement(paragraph);

                           table.AddCell(cell);
                       }
                       // end tanda tangan
                       pdfDoc.Add(table);

                       #endregion

                       pdfDoc.NewPage();

                       #region Lampiran

                       PdfPTable table1 = new PdfPTable(2);
                       float[] width1 = new float[] { 0.667f, 2f };
                       table1.WidthPercentage = 100;
                       table1.SetWidths(width1);
                       table1.HorizontalAlignment = Element.ALIGN_LEFT;
                       table1.DefaultCell.Border = 0;
                       table1.SpacingBefore = 0f;
                       table1.SpacingAfter = 0f;

                       PdfPCell cell1 = new PdfPCell();
                       Paragraph paragraph1 = new Paragraph();

                       cell1 = new PdfPCell();
                       cell1.BorderWidthLeft = 1f;
                       cell1.BorderWidthRight = 1f;
                       cell1.BorderWidthTop = 1f;
                       cell1.BorderWidthBottom = 1f;
                       cell1.MinimumHeight = 30f;
                       paragraph1 = new Paragraph("Nomor Registrasi Memo/Registration Number:" + letterDetail.letter.letterDeliberationNumber, normalDeliberation);
                       paragraph1.Alignment = Element.ALIGN_LEFT;
                       cell1.AddElement(paragraph1);
                       table1.AddCell(cell1);

                       cell1 = new PdfPCell();
                       cell1.BorderWidthLeft = 0f;
                       cell1.BorderWidthRight = 1f;
                       cell1.BorderWidthTop = 1f;
                       cell1.BorderWidthBottom = 1f;
                       cell1.MinimumHeight = 30f;
                       paragraph1 = new Paragraph("Tanggal Registrasi Memo/Date of Memo Registration : " + Convert.ToDateTime(letterDetail.letter.letterDate).ToString("dd MMMM yyyy"), normalDeliberation);
                       paragraph1.Alignment = Element.ALIGN_LEFT;
                       cell1.AddElement(paragraph1);
                       table1.AddCell(cell1);

                       cell1 = new PdfPCell();
                       cell1.BorderWidthLeft = 1f;
                       cell1.BorderWidthRight = 1f;
                       cell1.BorderWidthTop = 0f;
                       cell1.BorderWidthBottom = 1f;
                       cell1.MinimumHeight = 100f;
                       paragraph1 = new Paragraph("Nomor Memo : " + letterDetail.letter.letterNumber, normalDeliberation);
                       paragraph1.Alignment = Element.ALIGN_LEFT;
                       cell1.AddElement(paragraph1);
                       table1.AddCell(cell1);

                       cell1 = new PdfPCell();
                       cell1.BorderWidthLeft = 0f;
                       cell1.BorderWidthRight = 1f;
                       cell1.BorderWidthTop = 0f;
                       cell1.BorderWidthBottom = 1f;
                       cell1.MinimumHeight = 100f;
                       paragraph1 = new Paragraph("(Ringkasan Memo/Summary of the proposal) \n" + letterDetail.letterContent.summary, normalDeliberation);
                       paragraph1.Alignment = Element.ALIGN_LEFT;
                       cell1.AddElement(paragraph1);
                       table1.AddCell(cell1);

                       cell1 = new PdfPCell();
                       cell1.BorderWidthLeft = 1f;
                       cell1.BorderWidthRight = 1f;
                       cell1.BorderWidthTop = 0f;
                       cell1.BorderWidthBottom = 1f;
                       cell1.MinimumHeight = 100f;
                       paragraph1 = new Paragraph("Deliberation:", normalDeliberation);
                       paragraph1.Alignment = Element.ALIGN_LEFT;
                       cell1.AddElement(paragraph1);
                       no = 1;
                       foreach (var item in letterDetail.delibration)
                       {
                           if (letterDetail.receiver.Count() > 1)
                           {
                               paragraph1 = new Paragraph(no + ". " + item.fullname + " - " + item.positionName, normalDeliberation);
                               paragraph1.Alignment = Element.ALIGN_LEFT;
                               cell1.AddElement(paragraph1);
                           }
                           else
                           {
                               paragraph1 = new Paragraph(item.fullname + " - " + item.positionName, normalDeliberation);
                               paragraph1.Alignment = Element.ALIGN_LEFT;
                               cell1.AddElement(paragraph1);
                           }
                           no++;
                       }
                       table1.AddCell(cell1);

                       cell1 = new PdfPCell();
                       cell1.BorderWidthLeft = 0f;
                       cell1.BorderWidthRight = 1f;
                       cell1.BorderWidthTop = 0f;
                       cell1.BorderWidthBottom = 1f;
                       cell1.MinimumHeight = 100f;
                       paragraph1 = new Paragraph("Catatan / Notes : \n", underlineDeliberation);
                       paragraph1.Alignment = Element.ALIGN_LEFT;
                       cell1.AddElement(paragraph1);
                       no = 1;
                       foreach (var item in letterDetail.delibration)
                       {
                           if (letterDetail.receiver.Count() > 1)
                           {
                               paragraph1 = new Paragraph(no + ". " + item.commentdlbrt, normalDeliberation);
                               paragraph1.Alignment = Element.ALIGN_LEFT;
                               cell1.AddElement(paragraph1);
                           }
                           else
                           {
                               paragraph1 = new Paragraph(item.commentdlbrt, normalDeliberation);
                               paragraph1.Alignment = Element.ALIGN_LEFT;
                               cell1.AddElement(paragraph1);
                           }
                           no++;
                       }
                       table1.AddCell(cell1);

                       pdfDoc.Add(table1);

                       //var checkerList = letterDetail.checker.Where(p => p.ordernumber != 1).OrderBy(p => p.idLevelChecker).ToList();
                       var checkerList = letterDetail.checker.OrderBy(p => p.idLevelChecker).ToList();
                       foreach (var item in checkerList)
                       {
                           var getLogComment = letterDetail.log.Where(p => p.idUserLog == item.idUserChecker && p.description.Contains("pemeriksa ke")).OrderByDescending(p => p.createdOn).FirstOrDefault();
                           var comment = "";
                           string approveDate = "";
                           if (getLogComment != null)
                           {
                               comment = getLogComment.comment;
                               approveDate = Convert.ToDateTime(getLogComment.createdOn).ToString("dd MMMM yyyy");
                           }
                           table1 = new PdfPTable(3);
                           width = new float[] { 1f, 2f, 1f };
                           table1.WidthPercentage = 100;
                           table1.SetWidths(width);
                           table1.HorizontalAlignment = Element.ALIGN_LEFT;
                           table1.DefaultCell.Border = 0;
                           table1.SpacingBefore = 0f;
                           table1.SpacingAfter = 0f;

                           cell1 = new PdfPCell();
                           cell1.Rowspan = 2;
                           cell1.BorderWidthLeft = 1f;
                           cell1.BorderWidthRight = 1f;
                           cell1.BorderWidthTop = 0f;
                           cell1.BorderWidthBottom = 1f;
                           cell1.MinimumHeight = 120f;
                           if (letterDetail.letter.statusCode == 2)
                                {
                                    paragraph1 = new Paragraph("Belum diputuskan / Undecided \n", normalDeliberation);
                                }
                                else if (letterDetail.letter.statusCode == 5)
                                {
                                    paragraph1 = new Paragraph("Ditolak / Reject \n", normalDeliberation);
                                }
                                else
                                {
                                   if(item.idLevelChecker == 2 ||item.idLevelChecker == 1)
                                   { 
                                       paragraph1 = new Paragraph("Disetujui / Approved \n", normalDeliberation);
                                   }
                                   else
                                   {
                                       paragraph1 = new Paragraph("Mengetahui / Understand \n", normalDeliberation);
                                       
                                   }
                                }

                           paragraph1.Alignment = Element.ALIGN_LEFT;
                           cell1.AddElement(paragraph1);
                           paragraph1 = new Paragraph(item.positionName, normalDeliberation);
                           paragraph1.Alignment = Element.ALIGN_LEFT;
                           cell1.AddElement(paragraph1);
                           table1.AddCell(cell1);

                           cell1 = new PdfPCell();
                           cell1.Rowspan = 2;
                           cell1.BorderWidthLeft = 0f;
                           cell1.BorderWidthRight = 1f;
                           cell1.BorderWidthTop = 0f;
                           cell1.BorderWidthBottom = 1f;
                           cell1.MinimumHeight = 120f;
                           paragraph1 = new Paragraph("Catatan / Notes : \n", underlineDeliberation);
                           paragraph1.Alignment = Element.ALIGN_LEFT;
                           cell1.AddElement(paragraph1);
                           paragraph1 = new Paragraph(comment, normalDeliberation);
                           paragraph1.Alignment = Element.ALIGN_LEFT;
                           cell1.AddElement(paragraph1);
                           table1.AddCell(cell1);

                           cell1 = new PdfPCell();
                           cell1.BorderWidthLeft = 0f;
                           cell1.BorderWidthRight = 1f;
                           cell1.BorderWidthTop = 0f;
                           cell1.BorderWidthBottom = 1f;
                           cell1.MinimumHeight = 40f;
                           paragraph1 = new Paragraph("Tanggal : ", underlineDeliberation);
                           paragraph1.Alignment = Element.ALIGN_LEFT;
                           //cell1.AddElement(paragraph1);
                           paragraph1 = new Paragraph(approveDate, normalDeliberation);
                           paragraph1.Alignment = Element.ALIGN_LEFT;
                           cell1.AddElement(paragraph1);
                           table1.AddCell(cell1);

                           cell1 = new PdfPCell();
                           cell1.BorderWidthLeft = 0f;
                           cell1.BorderWidthRight = 1f;
                           cell1.BorderWidthTop = 0f;
                           cell1.BorderWidthBottom = 1f;
                           paragraph1 = new Paragraph("Tanda Tangan : \n", underlineDeliberation);
                           paragraph1.Alignment = Element.ALIGN_LEFT;
                           cell1.AddElement(paragraph1);
                            
                           if (letterDetail.letter.letterNumber != "NO_LETTER")
                           {
                                
                               if (letterDetail.letter.signatureType == 1)
                               {
                                   Image imagettd = GetSignatureImage(item.nip);
                                   //cell.AddElement(imagettd);
                                   if (imagettd != null)
                                   {
                                       //cell1.HorizontalAlignment = Element.ALIGN_CENTER;
                                       cell1.AddElement(imagettd);
                                   }
                                   else
                                   {
                                       var img = "File tanda tangan tidak ditemukan";
                                       paragraph1 = new Paragraph(img, normal);
                                       cell1.AddElement(paragraph1);
                                   }
                               }
                               else if (letterDetail.letter.signatureType == 2)
                               {

                                   Image QRCodeSignatureApprover = GetBarcodeSignature("NIP : " + item.nip + "\nNAMA : " + item.fullname + "\nJabatan : " + item.positionName);
                                   cell1.AddElement(QRCodeSignatureApprover);
                               }
                                else if (letterDetail.letter.signatureType == 3)
                                {
                                    var img = "";
                                    paragraph = new Paragraph(img, normal);
                                    cell.AddElement(paragraph);
                                    Paragraph linessia = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(2F, 100.0F, colorWhite, Element.ALIGN_CENTER, 1)));
                                    linessia.SpacingBefore = 0f;
                                    linessia.SetLeading(0.5F, 0.5F);
                                    pdfDoc.Add(linessia);

                                    Paragraph linessiaa = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(2F, 100.0F, colorWhite, Element.ALIGN_CENTER, 1)));
                                    linessiaa.SpacingBefore = 0f;
                                    linessiaa.SetLeading(0.5F, 0.5F);
                                    pdfDoc.Add(linessiaa);

                                    Paragraph linessiaaa = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(2F, 100.0F, colorWhite, Element.ALIGN_CENTER, 1)));
                                    linessiaaa.SpacingBefore = 0f;
                                    linessiaaa.SetLeading(0.5F, 0.5F);
                                    pdfDoc.Add(linessiaaa);
                                }
                               else
                               {





                                   Image imagettd = GetSignatureImage(item.nip);
                                   cell1.AddElement(imagettd);

                                   Image QRCodeSignatureApprover = GetBarcodeSignature("NIP : " + item.nip + "\nNAMA : " + item.fullname + "\nJabatan : " + item.positionName);
                                    
                                   cell1.AddElement(QRCodeSignatureApprover);


                               }
                           }

                                    paragraph1 = new Paragraph(item.fullname, bold);
                                    paragraph1.Alignment = Element.ALIGN_CENTER;
                                    cell1.AddElement(paragraph1);
                                    table1.AddCell(cell1);
                                    

                                    pdfDoc.Add(table1);
                        }


                       #endregion

                       pdfDoc.Close();

                       var dateLetterFormat = Convert.ToDateTime(letterDetail.letter.letterDate).ToString("dd MMMM yyyy");
                       var letterNumber = letterDetail.letter.letterNumber;
                       byte[] bytess = ms.ToArray();
                       byte[] bytes = AddPageNumbers(bytess, dateLetterFormat, letterNumber, "Memo");
                       
                       ms.Close();

                       return File(bytes.ToArray(), "application/pdf", fileName);
                    }

                #endregion
            }

            //Done
            private FileContentResult PrivSuratMemoDepartement(string letternumber, OutputDetailMemo letterDetail, string fileName)
            {
                #region Surat Memo Departement

                    StringBuilder sb = new StringBuilder();
                    sb.Append("<div style='padding-left:2px;'>" + letterDetail.letterContent.letterContent + "</div>");
                    StringReader sr = new StringReader(sb.ToString());
                    //Document pdfDoc = new Document(PageSize.A4, 25, 25, 25, 15);
                    //Document pdfDoc = new Document(PageSize.A4, 56, 56, 113, 97);
                    //Document pdfDoc = new Document(PageSize.A4, 56.7f, 56.7f, 113.4f, 99.225f);
                    Document pdfDoc = new Document(PageSize.A4, 56.7f, 56.7f, 90.4f, 99.225f);
                    HTMLWorker htmlparser = new HTMLWorker(pdfDoc);


                    using (MemoryStream ms = new MemoryStream())
                    {
                        PdfWriter pdfWriter = PdfWriter.GetInstance(pdfDoc, ms);
                        //pdfWriter.PageEvent = new PDFHeaderEvent();
                        //pdfWriter.PageEvent = new PDFHeaderEvent2();
                        pdfDoc.Open();
                        //Font
                        int no;
                        var fontName = "Calibri";
                        string fontPath = Path.Combine(_environment.WebRootPath, "fonts/Calibri.ttf");
                        //FontFactory.Register(fontPath);

                        Font normal = FontFactory.GetFont(fontName, 12, Font.NORMAL, BaseColor.BLACK);
                        Font bold = FontFactory.GetFont(fontName, 12, Font.BOLD, BaseColor.BLACK);
                        Font underline = FontFactory.GetFont(fontName, 12, Font.UNDERLINE, BaseColor.BLACK);
                        Font underlineBold = FontFactory.GetFont(fontName, 12, Font.BOLD | Font.UNDERLINE, BaseColor.BLACK);
                        Font boldMemo = FontFactory.GetFont(fontName, 14, Font.BOLD, BaseColor.BLACK);

                        Font normalDeliberation = FontFactory.GetFont(fontName, 10, Font.NORMAL, BaseColor.BLACK);
                        Font underlineDeliberation = FontFactory.GetFont(fontName, 10, Font.UNDERLINE, BaseColor.BLACK);
                        Font underlineDeliberationBold = FontFactory.GetFont(fontName, 10, Font.BOLD | Font.UNDERLINE, BaseColor.BLACK);
                        // Colors
                        BaseColor colorBlack = new BaseColor(System.Drawing.ColorTranslator.FromHtml("#000000"));
                        BaseColor colorWhite = new BaseColor(System.Drawing.ColorTranslator.FromHtml("#FFFFFF"));

                        PdfPTable table = new PdfPTable(2);
                        float[] width = new float[] { 0.667f, 2f };
                        table.WidthPercentage = 100;
                        table.SetWidths(width);
                        table.DefaultCell.Border = 0;
                        table.SpacingBefore = 0f;
                        table.SpacingAfter = 0f;

                        PdfPCell cell = new PdfPCell();
                        Paragraph paragraph = new Paragraph();


                                #region header
                
                                //cell = new PdfPCell();
                                //cell.Border = Rectangle.NO_BORDER;
                                //cell.HorizontalAlignment= Element.ALIGN_LEFT;
                                //cell.Phrase = new Phrase("", bold);
                                //table.AddCell(cell);

                                //Image logo = GetLogoImage("logobni");
                                //logo.ScalePercent(55);
                                //cell = new PdfPCell(logo);
                                //cell.Border = Rectangle.NO_BORDER;
                                //cell.HorizontalAlignment= Element.ALIGN_RIGHT;
                                //table.AddCell(cell);
                                //pdfDoc.Add(table);

                                //table = new PdfPTable(1);
                                //width = new float[] { 2f };
                                //table.WidthPercentage = 100;
                                //table.SetWidths(width);
                                //table.HorizontalAlignment = Element.ALIGN_LEFT;
                                //table.DefaultCell.Border = 0;
                                //table.SpacingBefore = 00f;
                                //table.SpacingAfter = 0f;


                                //table.AddCell(new Phrase("MEMO", boldMemo));
                                //pdfDoc.Add(table);

                                table = new PdfPTable(2);
                                width = new float[] { 0.2f, 1f };
                                table.WidthPercentage = 100;
                                table.SetWidths(width);
                                table.HorizontalAlignment = Element.ALIGN_RIGHT;
                                table.DefaultCell.Border = 0;
                                table.SpacingBefore = 0f;
                                table.SpacingAfter = 0f;

                                cell.Border = 0;
                                paragraph = new Paragraph("Nomor", normal);
                                paragraph.Alignment = Element.ALIGN_LEFT;
                                cell.AddElement(paragraph);
                                table.AddCell(cell);

                                //cell = new PdfPCell();
                                //cell.Border = 0;
                                //paragraph = new Paragraph(":", normal);
                                //paragraph.Alignment = Element.ALIGN_RIGHT;
                                //cell.AddElement(paragraph);
                                //table.AddCell(cell);

                                cell = new PdfPCell();
                                cell.Border = 0;
                                paragraph = new Paragraph(":"+" "+letterDetail.letter.letterNumber, normal);
                                paragraph.Alignment = Element.ALIGN_LEFT;
                                cell.AddElement(paragraph);
                                table.AddCell(cell);

                                cell = new PdfPCell();
                                cell.Border = 0;
                                paragraph = new Paragraph("Tanggal", normal);
                                paragraph.Alignment = Element.ALIGN_LEFT;
                                cell.AddElement(paragraph);
                                table.AddCell(cell);

                                //cell = new PdfPCell();
                                //cell.Border = 0;
                                //paragraph = new Paragraph(":", normal);
                                //paragraph.Alignment = Element.ALIGN_RIGHT;
                                //cell.AddElement(paragraph);
                                //table.AddCell(cell);

                                cell = new PdfPCell();
                                cell.Border = 0;
                                paragraph = new Paragraph(":"+" "+Convert.ToDateTime(letterDetail.letter.letterDate).ToString("dd MMMM yyyy"), normal);
                                paragraph.Alignment = Element.ALIGN_LEFT;
                                cell.AddElement(paragraph);
                                table.AddCell(cell);

                                cell = new PdfPCell();
                                cell.Border = 0;
                                paragraph = new Paragraph("Kepada", normal);
                                paragraph.Alignment = Element.ALIGN_LEFT;
                                cell.AddElement(paragraph);
                                table.AddCell(cell);

                                //cell = new PdfPCell();
                                //cell.Border = 0;
                                //paragraph = new Paragraph(":", normal);
                                //paragraph.Alignment = Element.ALIGN_RIGHT;
                                //cell.AddElement(paragraph);
                                //table.AddCell(cell);

                                cell = new PdfPCell();
                                cell.Border = 0;
                                no = 1;
                                foreach (var item in letterDetail.receiver)
                                {
                                    if (letterDetail.receiver.Count() > 1)
                                    {
                                        paragraph = new Paragraph(":"+" "+no + ". " + item.fullname + " - " + item.positionName, normal);
                                        paragraph.Alignment = Element.ALIGN_LEFT;
                                        cell.AddElement(paragraph);
                                    }
                                    else
                                    {
                                        paragraph = new Paragraph(":"+" "+item.fullname + " - " + item.positionName, normal);
                                        paragraph.Alignment = Element.ALIGN_LEFT;
                                        cell.AddElement(paragraph);
                                    }
                                    no++;
                                }
                                table.AddCell(cell);

                                cell = new PdfPCell();
                                cell.Border = 0;
                                paragraph = new Paragraph("Tembusan", normal);
                                paragraph.Alignment = Element.ALIGN_LEFT;
                                cell.AddElement(paragraph);
                                table.AddCell(cell);

                                //cell = new PdfPCell();
                                //cell.Border = 0;
                                //paragraph = new Paragraph(":", normal);
                                //paragraph.Alignment = Element.ALIGN_RIGHT;
                                //cell.AddElement(paragraph);
                                //table.AddCell(cell);

                                cell = new PdfPCell();
                                cell.Border = 0;
                                no = 1;
                                foreach (var item in letterDetail.copy)
                                {
                                    if (letterDetail.copy.Count() > 1)
                                    {
                                        paragraph = new Paragraph(":"+" "+no + ". " + item.fullname + " - " + item.positionName, normal);
                                        paragraph.Alignment = Element.ALIGN_LEFT;
                                        cell.AddElement(paragraph);
                                    }
                                    else
                                    {
                                        paragraph = new Paragraph(":"+" "+item.fullname + " - " + item.positionName, normal);
                                        paragraph.Alignment = Element.ALIGN_LEFT;
                                        cell.AddElement(paragraph);
                                    }
                                    no++;
                                }
                                if (letterDetail.copy.Count() == 0)
                                {
                                    paragraph = new Paragraph("-", normal);
                                    paragraph.Alignment = Element.ALIGN_LEFT;
                                    cell.AddElement(paragraph);
                                }
                                table.AddCell(cell);

                                cell = new PdfPCell();
                                cell.Border = 0;
                                paragraph = new Paragraph("Dari", normal);
                                paragraph.Alignment = Element.ALIGN_LEFT;
                                cell.AddElement(paragraph);
                                table.AddCell(cell);

                                //cell = new PdfPCell();
                                //cell.Border = 0;
                                //paragraph = new Paragraph(":", normal);
                                //paragraph.Alignment = Element.ALIGN_RIGHT;
                                //cell.AddElement(paragraph);
                                //table.AddCell(cell);

                                cell = new PdfPCell();
                                cell.Border = 0;
                                paragraph = new Paragraph(":"+" "+letterDetail.sender[0].fullname + " - " + letterDetail.sender[0].positionName, normal);
                                paragraph.Alignment = Element.ALIGN_LEFT;
                                cell.AddElement(paragraph);
                                table.AddCell(cell);

                                cell = new PdfPCell();
                                cell.Border = 0;
                                paragraph = new Paragraph("Perihal", normal);
                                paragraph.Alignment = Element.ALIGN_LEFT;
                                cell.AddElement(paragraph);
                                table.AddCell(cell);

                                //cell = new PdfPCell();
                                //cell.Border = 0;
                                //paragraph = new Paragraph(":", normal);
                                //paragraph.Alignment = Element.ALIGN_RIGHT;
                                //cell.AddElement(paragraph);
                                //table.AddCell(cell);

                                cell = new PdfPCell();
                                cell.Border = 0;
                                paragraph = new Paragraph(":"+" "+letterDetail.letter.about, normal);
                                paragraph.Alignment = Element.ALIGN_LEFT;
                                cell.AddElement(paragraph);
                                table.AddCell(cell);

                                cell = new PdfPCell();
                                cell.Border = 0;
                                paragraph = new Paragraph("Lampiran", normal);
                                paragraph.Alignment = Element.ALIGN_LEFT;
                                cell.AddElement(paragraph);
                                table.AddCell(cell);

                                //cell = new PdfPCell();
                                //cell.Border = 0;
                                //paragraph = new Paragraph(":", normal);
                                //paragraph.Alignment = Element.ALIGN_RIGHT;
                                //cell.AddElement(paragraph);
                                //table.AddCell(cell);

                                var lampiran = letterDetail.letter.attachmentDesc == null ? "-" : letterDetail.letter.attachmentDesc;
                                cell = new PdfPCell();
                                cell.Border = 0;
                                paragraph = new Paragraph(":"+" "+lampiran, bold);
                                paragraph.Alignment = Element.ALIGN_LEFT;
                                cell.AddElement(paragraph);
                                table.AddCell(cell);

                                //Add table to document
                                pdfDoc.Add(table);

                        

                                Paragraph line = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(2F, 100.0F, colorBlack, Element.ALIGN_CENTER, 1)));
                                line.SpacingBefore = 0f;
                                line.SetLeading(2F, 0.5F);
                                pdfDoc.Add(line);
                                Paragraph lines = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(2F, 100.0F, colorBlack, Element.ALIGN_CENTER, 1)));
                                lines.SpacingBefore = 0f;
                                line.SetLeading(0.5F, 0.5F);
                                pdfDoc.Add(line);

                                Paragraph liness = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(2F, 100.0F, colorWhite, Element.ALIGN_CENTER, 1)));
                                liness.SpacingBefore = 0f;
                                liness.SetLeading(0.5F, 0.5F);
                                pdfDoc.Add(liness);

                                //Paragraph linesss = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(2F, 100.0F, colorWhite, Element.ALIGN_CENTER, 1)));
                                //linesss.SpacingBefore = 0f;
                                //linesss.SetLeading(0.5F, 0.5F);
                                //pdfDoc.Add(linesss);

                                //Paragraph linessss = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(2F, 100.0F, colorWhite, Element.ALIGN_CENTER, 1)));
                                //linessss.SpacingBefore = 0f;
                                //linessss.SetLeading(0.5F, 0.5F);
                                //pdfDoc.Add(linesss);

                                table = new PdfPTable(1);
                                width = new float[] { 2f };
                                table.WidthPercentage = 100;
                                table.SetWidths(width);
                                table.HorizontalAlignment = Element.ALIGN_LEFT;
                                table.DefaultCell.Border = 0;
                                table.SpacingBefore = 00f;
                                table.SpacingAfter = 0f;


                                table.AddCell(new Phrase("Dengan hormat", normal));
                                pdfDoc.Add(table);


                                Paragraph lin = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(2F, 100.0F, colorWhite, Element.ALIGN_CENTER, 1)));
                                lin.SpacingBefore = 0f;
                                lin.SetLeading(0.5F, 0.5F);
                                pdfDoc.Add(lin);

                                #endregion

                            //ADD HTML CONTENT
                            htmlparser.Parse(sr);

                            #region Tanda Tangan

                            Paragraph a = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(2F, 100.0F, colorWhite, Element.ALIGN_CENTER, 1)));
                            a.SpacingBefore = 0f;
                            a.SetLeading(0.5F, 0.5F);
                            pdfDoc.Add(a);



                            cell = new PdfPCell();
                            table.AddCell(new Phrase("", normal));

                            cell = new PdfPCell();
                            table.AddCell(new Phrase(" ", normal));
                            cell = new PdfPCell();
                            table.AddCell(new Phrase(" ", normal));
                            cell = new PdfPCell();
                            table.AddCell(new Phrase("Hormat kami, \n" + "PT BNI Life Insurance", normal));
                            //table.AddCell(new Phrase("Best Regards", normal));
                            pdfDoc.Add(table);



                            string filename = letterDetail.sender[0].nip;


                            // Tanda tangan
                            var dirChecker = letterDetail.sender.OrderBy(p => p.idUserSender).ToList();
                            //var dirChecker = letterDetail.checker.Where(p => p.idLevelChecker == 2 || p.idLevelChecker == 1).ToList();
                            //var dirChecker = letterDetail.checker.Where(p => p.ordernumber == 1).ToList();

                            table = new PdfPTable(dirChecker.Count());
                            width = new float[dirChecker.Count()];
                            for (int i = 0; i < dirChecker.Count(); i++)
                            {
                                width[i] = 1f;
                            }
                            table.WidthPercentage = 100;
                            table.SetWidths(width);
                            table.HorizontalAlignment = Element.ALIGN_LEFT;
                            table.DefaultCell.Border = 0;
                            var no1 = 0;


                            //cell = new PdfPCell();
                            //table.AddCell(new Phrase("Direksi PT BNI Life Insurance", normal));
                            foreach (var item in dirChecker)
                            {
                                cell = new PdfPCell();
                                cell.Border = 0;




                                if (letterDetail.letter.letterNumber != "NO_LETTER")
                                {
                                    if (letterDetail.letter.signatureType == 1)
                                    {

                                        Image imagettd = GetSignatureImage(item.nip);
                                        //cell.AddElement(imagettd);
                                        if (imagettd != null)
                                        {
                                            cell.AddElement(imagettd);
                                        }
                                        else
                                        {
                                            var img = "File tanda tangan tidak ditemukan";
                                            paragraph = new Paragraph(img, normal);
                                            cell.AddElement(paragraph);
                                        }

                                    }
                                    else if (letterDetail.letter.signatureType == 2)
                                    {


                                        Image QRCodeSignatureApprover = GetBarcodeSignature("NIP : " + item.nip + "\nNAMA : " + item.fullname + "\nJabatan : " + item.positionName);
                                        QRCodeSignatureApprover.ScaleAbsoluteHeight(10f);
                                        cell.AddElement(QRCodeSignatureApprover);
                                    }
                                    else if (letterDetail.letter.signatureType == 3)
                                    {
                                        var img = "";
                                        paragraph = new Paragraph(img, normal);
                                        cell.AddElement(paragraph);
                                        Paragraph linessia = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(2F, 100.0F, colorWhite, Element.ALIGN_CENTER, 1)));
                                        linessia.SpacingBefore = 0f;
                                        linessia.SetLeading(0.5F, 0.5F);
                                        pdfDoc.Add(linessia);

                                        Paragraph linessiaa = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(2F, 100.0F, colorWhite, Element.ALIGN_CENTER, 1)));
                                        linessiaa.SpacingBefore = 0f;
                                        linessiaa.SetLeading(0.5F, 0.5F);
                                        pdfDoc.Add(linessiaa);

                                        Paragraph linessiaaa = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(2F, 100.0F, colorWhite, Element.ALIGN_CENTER, 1)));
                                        linessiaaa.SpacingBefore = 0f;
                                        linessiaaa.SetLeading(0.5F, 0.5F);
                                        pdfDoc.Add(linessiaaa);
                                    }
                                    else
                                    {

                                        Image imagettd = GetSignatureImage(item.nip);
                                        cell.AddElement(imagettd);
                                        Image QRCodeSignatureApprover = GetBarcodeSignature("NIP : " + item.nip + "\nNAMA : " + item.fullname + "\nJabatan : " + item.positionName);
                                        cell.AddElement(QRCodeSignatureApprover);
                                        //if (imagettd != null)
                                        //{
                                        //    cell.AddElement(imagettd);
                                        //    Image QRCodeSignatureApprover = GetBarcodeSignature("NIP : " + item.nip + "\nNAMA : " + item.fullname + "\nJabatan : " + item.positionName);
                                        //    cell.AddElement(QRCodeSignatureApprover);
                                        //}
                                        //else
                                        //{
                                        //    var img = "File tanda tangan tidak di temukan";
                                        //    paragraph = new Paragraph(img, normal);
                                        //    cell.AddElement(paragraph);
                                        //    Image QRCodeSignatureApprover = GetBarcodeSignature("NIP : " + item.nip + "\nNAMA : " + item.fullname + "\nJabatan : " + item.positionName);
                                        //    cell.AddElement(QRCodeSignatureApprover);
                                        //}

                                    }
                                    no1++;
                                }

                                paragraph = new Paragraph(item.fullname, underlineBold);
                                cell.AddElement(paragraph);
                                paragraph = new Paragraph(item.positionName, bold);
                                cell.AddElement(paragraph);

                                table.AddCell(cell);
                            }
                            // end tanda tangan
                            pdfDoc.Add(table);

                            #endregion

                            pdfDoc.NewPage();

                            #region Lampiran

                            PdfPTable table1 = new PdfPTable(2);
                            float[] width1 = new float[] { 0.667f, 2f };
                            table1.WidthPercentage = 100;
                            table1.SetWidths(width1);
                            table1.HorizontalAlignment = Element.ALIGN_LEFT;
                            table1.DefaultCell.Border = 0;
                            table1.SpacingBefore = 0f;
                            table1.SpacingAfter = 0f;

                            PdfPCell cell1 = new PdfPCell();
                            Paragraph paragraph1 = new Paragraph();

                            cell1 = new PdfPCell();
                            cell1.BorderWidthLeft = 1f;
                            cell1.BorderWidthRight = 1f;
                            cell1.BorderWidthTop = 1f;
                            cell1.BorderWidthBottom = 1f;
                            cell1.MinimumHeight = 30f;
                            paragraph1 = new Paragraph("Nomor Registrasi Memo/Registration Number:" + letterDetail.letter.letterDeliberationNumber, normalDeliberation);
                            paragraph1.Alignment = Element.ALIGN_LEFT;
                            cell1.AddElement(paragraph1);
                            table1.AddCell(cell1);

                            cell1 = new PdfPCell();
                            cell1.BorderWidthLeft = 0f;
                            cell1.BorderWidthRight = 1f;
                            cell1.BorderWidthTop = 1f;
                            cell1.BorderWidthBottom = 1f;
                            cell1.MinimumHeight = 30f;
                            paragraph1 = new Paragraph("Tanggal Registrasi Memo/Date of Memo Registration : " + Convert.ToDateTime(letterDetail.letter.letterDate).ToString("dd MMMM yyyy"), normalDeliberation);
                            paragraph1.Alignment = Element.ALIGN_LEFT;
                            cell1.AddElement(paragraph1);
                            table1.AddCell(cell1);

                            cell1 = new PdfPCell();
                            cell1.BorderWidthLeft = 1f;
                            cell1.BorderWidthRight = 1f;
                            cell1.BorderWidthTop = 0f;
                            cell1.BorderWidthBottom = 1f;
                            cell1.MinimumHeight = 100f;
                            paragraph1 = new Paragraph("Nomor Memo : " + letterDetail.letter.letterNumber, normalDeliberation);
                            paragraph1.Alignment = Element.ALIGN_LEFT;
                            cell1.AddElement(paragraph1);
                            table1.AddCell(cell1);

                            cell1 = new PdfPCell();
                            cell1.BorderWidthLeft = 0f;
                            cell1.BorderWidthRight = 1f;
                            cell1.BorderWidthTop = 0f;
                            cell1.BorderWidthBottom = 1f;
                            cell1.MinimumHeight = 100f;
                            paragraph1 = new Paragraph("(Ringkasan Memo/Summary of the proposal) \n" + letterDetail.letterContent.summary, normalDeliberation);
                            paragraph1.Alignment = Element.ALIGN_LEFT;
                            cell1.AddElement(paragraph1);
                            table1.AddCell(cell1);

                            cell1 = new PdfPCell();
                            cell1.BorderWidthLeft = 1f;
                            cell1.BorderWidthRight = 1f;
                            cell1.BorderWidthTop = 0f;
                            cell1.BorderWidthBottom = 1f;
                            cell1.MinimumHeight = 100f;
                            paragraph1 = new Paragraph("Deliberation:", normalDeliberation);
                            paragraph1.Alignment = Element.ALIGN_LEFT;
                            cell1.AddElement(paragraph1);
                            no = 1;
                            foreach (var item in letterDetail.delibration)
                            {
                                if (letterDetail.receiver.Count() > 1)
                                {
                                    paragraph1 = new Paragraph(no + ". " + item.fullname + " - " + item.positionName, normalDeliberation);
                                    paragraph1.Alignment = Element.ALIGN_LEFT;
                                    cell1.AddElement(paragraph1);
                                }
                                else
                                {
                                    paragraph1 = new Paragraph(item.fullname + " - " + item.positionName, normalDeliberation);
                                    paragraph1.Alignment = Element.ALIGN_LEFT;
                                    cell1.AddElement(paragraph1);
                                }
                                no++;
                            }
                            table1.AddCell(cell1);

                            cell1 = new PdfPCell();
                            cell1.BorderWidthLeft = 0f;
                            cell1.BorderWidthRight = 1f;
                            cell1.BorderWidthTop = 0f;
                            cell1.BorderWidthBottom = 1f;
                            cell1.MinimumHeight = 100f;
                            paragraph1 = new Paragraph("Catatan / Notes : \n", underlineDeliberation);
                            paragraph1.Alignment = Element.ALIGN_LEFT;
                            cell1.AddElement(paragraph1);
                            no = 1;
                            foreach (var item in letterDetail.delibration)
                            {
                                if (letterDetail.receiver.Count() > 1)
                                {
                                    paragraph1 = new Paragraph(no + ". " + item.commentdlbrt, normalDeliberation);
                                    paragraph1.Alignment = Element.ALIGN_LEFT;
                                    cell1.AddElement(paragraph1);
                                }
                                else
                                {
                                    paragraph1 = new Paragraph(item.commentdlbrt, normalDeliberation);
                                    paragraph1.Alignment = Element.ALIGN_LEFT;
                                    cell1.AddElement(paragraph1);
                                }
                                no++;
                            }
                            table1.AddCell(cell1);

                            pdfDoc.Add(table1);

                            //var checkerList = letterDetail.checker.Where(p => p.ordernumber != 1).OrderBy(p => p.idLevelChecker).ToList();
                            var checkerList = letterDetail.checker.OrderBy(p => p.idLevelChecker).ToList();
                            foreach (var item in checkerList)
                            {
                                var getLogComment = letterDetail.log.Where(p => p.idUserLog == item.idUserChecker && p.description.Contains("pemeriksa ke")).OrderByDescending(p => p.createdOn).FirstOrDefault();
                                var comment = "";
                                string approveDate = "";
                                if (getLogComment != null)
                                {
                                    comment = getLogComment.comment;
                                    approveDate = Convert.ToDateTime(getLogComment.createdOn).ToString("dd MMMM yyyy");
                                }
                                table1 = new PdfPTable(3);
                                width = new float[] { 1f, 2f, 1f };
                                table1.WidthPercentage = 100;
                                table1.SetWidths(width);
                                table1.HorizontalAlignment = Element.ALIGN_LEFT;
                                table1.DefaultCell.Border = 0;
                                table1.SpacingBefore = 0f;
                                table1.SpacingAfter = 0f;

                                cell1 = new PdfPCell();
                                cell1.Rowspan = 2;
                                cell1.BorderWidthLeft = 1f;
                                cell1.BorderWidthRight = 1f;
                                cell1.BorderWidthTop = 0f;
                                cell1.BorderWidthBottom = 1f;
                                cell1.MinimumHeight = 120f;
                                if (letterDetail.letter.statusCode == 2)
                                {
                                    paragraph1 = new Paragraph("Belum diputuskan / Undecided \n", normalDeliberation);
                                }
                                else if (letterDetail.letter.statusCode == 5)
                                {
                                    paragraph1 = new Paragraph("Ditolak / Reject \n", normalDeliberation);
                                }
                                else
                                {
                                   if(item.idLevelChecker == 2 ||item.idLevelChecker == 1)
                                   { 
                                       paragraph1 = new Paragraph("Disetujui / Approved \n", normalDeliberation);
                                   }
                                   else
                                   {
                                       paragraph1 = new Paragraph("Mengetahui / Understand \n", normalDeliberation);
                                       
                                   }
                                }

                                paragraph1.Alignment = Element.ALIGN_LEFT;
                                cell1.AddElement(paragraph1);
                                paragraph1 = new Paragraph(item.positionName, normalDeliberation);
                                paragraph1.Alignment = Element.ALIGN_LEFT;
                                cell1.AddElement(paragraph1);
                                table1.AddCell(cell1);

                                cell1 = new PdfPCell();
                                cell1.Rowspan = 2;
                                cell1.BorderWidthLeft = 0f;
                                cell1.BorderWidthRight = 1f;
                                cell1.BorderWidthTop = 0f;
                                cell1.BorderWidthBottom = 1f;
                                cell1.MinimumHeight = 120f;
                                paragraph1 = new Paragraph("Catatan / Notes : \n", underlineDeliberation);
                                paragraph1.Alignment = Element.ALIGN_LEFT;
                                cell1.AddElement(paragraph1);
                                paragraph1 = new Paragraph(comment, normalDeliberation);
                                paragraph1.Alignment = Element.ALIGN_LEFT;
                                cell1.AddElement(paragraph1);
                                table1.AddCell(cell1);

                                cell1 = new PdfPCell();
                                cell1.BorderWidthLeft = 0f;
                                cell1.BorderWidthRight = 1f;
                                cell1.BorderWidthTop = 0f;
                                cell1.BorderWidthBottom = 1f;
                                cell1.MinimumHeight = 40f;
                                paragraph1 = new Paragraph("Tanggal : ", underlineDeliberation);
                                paragraph1.Alignment = Element.ALIGN_LEFT;
                                //cell1.AddElement(paragraph);
                                paragraph1 = new Paragraph(approveDate, normalDeliberation);
                                paragraph1.Alignment = Element.ALIGN_LEFT;
                                cell1.AddElement(paragraph1);
                                table1.AddCell(cell1);

                                cell1 = new PdfPCell();
                                cell1.BorderWidthLeft = 0f;
                                cell1.BorderWidthRight = 1f;
                                cell1.BorderWidthTop = 0f;
                                cell1.BorderWidthBottom = 1f;
                                paragraph1 = new Paragraph("Tanda Tangan : \n", underlineDeliberation);
                                paragraph1.Alignment = Element.ALIGN_LEFT;
                                cell1.AddElement(paragraph1);
                                if (letterDetail.letter.letterNumber != "NO_LETTER")
                                {
                                    if (letterDetail.letter.signatureType == 1)
                                    {
                                        Image imagettd = GetSignatureImage(item.nip);
                                        //cell.AddElement(imagettd);
                                        if (imagettd != null)
                                        {
                                            cell1.AddElement(imagettd);
                                        }
                                        else
                                        {
                                            var img = "File tanda tangan tidak ditemukan";
                                            paragraph1 = new Paragraph(img, normal);
                                            cell1.AddElement(paragraph1);
                                        }
                                    }
                                    else if (letterDetail.letter.signatureType == 2)
                                    {

                                        Image QRCodeSignatureApprover = GetBarcodeSignature("NIP : " + item.nip + "\nNAMA : " + item.fullname + "\nJabatan : " + item.positionName);
                                        cell1.AddElement(QRCodeSignatureApprover);
                                    }
                                    else if (letterDetail.letter.signatureType == 3)
                                    {
                                        var img = "";
                                        paragraph = new Paragraph(img, normal);
                                        cell.AddElement(paragraph);
                                        Paragraph linessia = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(2F, 100.0F, colorWhite, Element.ALIGN_CENTER, 1)));
                                        linessia.SpacingBefore = 0f;
                                        linessia.SetLeading(0.5F, 0.5F);
                                        pdfDoc.Add(linessia);

                                        Paragraph linessiaa = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(2F, 100.0F, colorWhite, Element.ALIGN_CENTER, 1)));
                                        linessiaa.SpacingBefore = 0f;
                                        linessiaa.SetLeading(0.5F, 0.5F);
                                        pdfDoc.Add(linessiaa);

                                        Paragraph linessiaaa = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(2F, 100.0F, colorWhite, Element.ALIGN_CENTER, 1)));
                                        linessiaaa.SpacingBefore = 0f;
                                        linessiaaa.SetLeading(0.5F, 0.5F);
                                        pdfDoc.Add(linessiaaa);
                                    }
                                    else
                                    {
                                        //innertable1 = new PdfPtable1(2);
                                        //float[] widthinner = new float[] { 0.3f, 0.3f };
                                        //innertable1.WidthPercentage = 100;
                                        //innertable1.SetWidths(widthinner);
                                        //innertable1.HorizontalAlignment = Element.ALIGN_LEFT;
                                        //innertable1.DefaultCell.Border = 1;
                                        //innertable1.DefaultCell.PaddingBottom = 8;

                                        //innercell = new PdfPCell();
                                        //innercell.BorderWidthLeft = 1f;
                                        //innercell.BorderWidthRight = 1f;
                                        //innercell.BorderWidthTop = 1f;
                                        //innercell.BorderWidthBottom = 1f;




                                        Image imagettd = GetSignatureImage(item.nip);
                                        cell1.AddElement(imagettd);

                                        Image QRCodeSignatureApprover = GetBarcodeSignature("NIP : " + item.nip + "\nNAMA : " + item.fullname + "\nJabatan : " + item.positionName);
                                        cell1.AddElement(QRCodeSignatureApprover);
                                        //if (imagettd !=null)
                                        //{
                                        //    cell.AddElement(imagettd);

                                        //    Image QRCodeSignatureApprover = GetBarcodeSignature("NIP : " + item.nip + "\nNAMA : " + item.fullname + "\nJabatan : " + item.positionName);
                                        //    cell.AddElement(QRCodeSignatureApprover);
                                        //}
                                        //else
                                        //{
                                        //    var img = "File tanda tangan tidak di temukan";
                                        //    paragraph = new Paragraph(img, normal);

                                        //    cell.AddElement(paragraph);
                                        //    Image QRCodeSignatureApprover = GetBarcodeSignature("NIP : " + item.nip + "\nNAMA : " + item.fullname + "\nJabatan : " + item.positionName);
                                        //    cell.AddElement(QRCodeSignatureApprover);
                                        //}

                                    }
                                }

                                paragraph1 = new Paragraph(item.fullname, bold);
                                paragraph1.Alignment = Element.ALIGN_CENTER;
                                cell1.AddElement(paragraph1);
                                table1.AddCell(cell1);


                                pdfDoc.Add(table1);
                            }


                            #endregion


                            pdfDoc.Close();

                        var dateLetterFormat = Convert.ToDateTime(letterDetail.letter.letterDate).ToString("dd MMMM yyyy");
                        var letterNumber = letterDetail.letter.letterNumber;
                        byte[] bytess = ms.ToArray();
                        byte[] bytes = AddPageNumbers(bytess, dateLetterFormat, letterNumber, "Memo");
                        ms.Close();

                        return File(bytes.ToArray(), "application/pdf", fileName);
                    }

                #endregion
            }

            //Done(Masi Ikuti Template Memo Direksi)
            private FileContentResult PrivSuratKeterangan(string letternumber, OutputDetailMemo letterDetail, string fileName)
            {
                #region Surat Keterangan Divisi

                    StringBuilder sb = new StringBuilder();
                    sb.Append("<div style='padding-left:2px;'>" + letterDetail.letterContent.letterContent + "</div>");
                    StringReader sr = new StringReader(sb.ToString());
                    //Document pdfDoc = new Document(PageSize.A4, 25, 25, 25, 15);
                    //Document pdfDoc = new Document(PageSize.A4, 56, 56, 113, 97);
                    Document pdfDoc = new Document(PageSize.A4, 56.7f, 56.7f, 66.5f, 99.225f);
                    HTMLWorker htmlparser = new HTMLWorker(pdfDoc);


                    using (MemoryStream ms = new MemoryStream())
                    {
                        PdfWriter pdfWriter = PdfWriter.GetInstance(pdfDoc, ms);
                        pdfDoc.Open();
                        //Font
                        int no;
                        var fontName = "Calibri";
                        string fontPath = Path.Combine(_environment.WebRootPath, "fonts/Calibri.ttf");
                        //FontFactory.Register(fontPath);

                        Font normal = FontFactory.GetFont(fontName, 12, Font.NORMAL, BaseColor.BLACK);
                        Font bold = FontFactory.GetFont(fontName, 12, Font.BOLD, BaseColor.BLACK);
                        Font underline = FontFactory.GetFont(fontName, 12, Font.UNDERLINE, BaseColor.BLACK);
                        Font underlineBold = FontFactory.GetFont(fontName, 12, Font.BOLD | Font.UNDERLINE, BaseColor.BLACK);

                        Font normalDeliberation = FontFactory.GetFont(fontName, 10, Font.NORMAL, BaseColor.BLACK);
                        Font underlineDeliberation = FontFactory.GetFont(fontName, 10, Font.UNDERLINE, BaseColor.BLACK);
                        Font underlineDeliberationBold = FontFactory.GetFont(fontName, 10, Font.BOLD | Font.UNDERLINE, BaseColor.BLACK);
                        // Colors
                        BaseColor colorBlack = new BaseColor(System.Drawing.ColorTranslator.FromHtml("#000000"));
                        BaseColor colorWhite = new BaseColor(System.Drawing.ColorTranslator.FromHtml("#FFFFFF"));



                        PdfPTable table = new PdfPTable(2);
                        float[] width = new float[] { 0.667f, 2f };
                        table.WidthPercentage = 100;
                        table.SetWidths(width);
                        table.HorizontalAlignment = Element.ALIGN_LEFT;
                        table.DefaultCell.Border = 0;
                        table.SpacingBefore = 0f;
                        table.SpacingAfter = 0f;

                        PdfPCell cell = new PdfPCell();
                        Paragraph paragraph = new Paragraph();


                        #region header
                        //add new page

                        //table = new PdfPTable(1);
                        //width = new float[] { 2f };
                        //table.WidthPercentage = 100;
                        //table.SetWidths(width);
                        //table.HorizontalAlignment = Element.ALIGN_LEFT;
                        //table.DefaultCell.Border = 0;
                        //table.SpacingBefore = 00f;
                        //table.SpacingAfter = 0f;


                        //table.AddCell(new Phrase("MEMO", bold));
                        //pdfDoc.Add(table);

                        table = new PdfPTable(2);
                                width = new float[] { 0.2f, 1f };
                                table.WidthPercentage = 100;
                                table.SetWidths(width);
                                table.HorizontalAlignment = Element.ALIGN_RIGHT;
                                table.DefaultCell.Border = 0;
                                table.SpacingBefore = 0f;
                                table.SpacingAfter = 0f;

                                cell.Border = 0;
                                paragraph = new Paragraph("Nomor", normal);
                                paragraph.Alignment = Element.ALIGN_LEFT;
                                cell.AddElement(paragraph);
                                table.AddCell(cell);

                                //cell = new PdfPCell();
                                //cell.Border = 0;
                                //paragraph = new Paragraph(":", normal);
                                //paragraph.Alignment = Element.ALIGN_RIGHT;
                                //cell.AddElement(paragraph);
                                //table.AddCell(cell);

                                cell = new PdfPCell();
                                cell.Border = 0;
                                paragraph = new Paragraph(":"+" "+letterDetail.letter.letterNumber, normal);
                                paragraph.Alignment = Element.ALIGN_LEFT;
                                cell.AddElement(paragraph);
                                table.AddCell(cell);

                                cell = new PdfPCell();
                                cell.Border = 0;
                                paragraph = new Paragraph("Tanggal", normal);
                                paragraph.Alignment = Element.ALIGN_LEFT;
                                cell.AddElement(paragraph);
                                table.AddCell(cell);

                                //cell = new PdfPCell();
                                //cell.Border = 0;
                                //paragraph = new Paragraph(":", normal);
                                //paragraph.Alignment = Element.ALIGN_RIGHT;
                                //cell.AddElement(paragraph);
                                //table.AddCell(cell);

                                cell = new PdfPCell();
                                cell.Border = 0;
                                paragraph = new Paragraph(":"+" "+Convert.ToDateTime(letterDetail.letter.letterDate).ToString("dd MMMM yyyy"), normal);
                                paragraph.Alignment = Element.ALIGN_LEFT;
                                cell.AddElement(paragraph);
                                table.AddCell(cell);

                                cell = new PdfPCell();
                                cell.Border = 0;
                                paragraph = new Paragraph("Kepada", normal);
                                paragraph.Alignment = Element.ALIGN_LEFT;
                                cell.AddElement(paragraph);
                                table.AddCell(cell);

                                //cell = new PdfPCell();
                                //cell.Border = 0;
                                //paragraph = new Paragraph(":", normal);
                                //paragraph.Alignment = Element.ALIGN_RIGHT;
                                //cell.AddElement(paragraph);
                                //table.AddCell(cell);

                                cell = new PdfPCell();
                                cell.Border = 0;
                                no = 1;
                                foreach (var item in letterDetail.receiver)
                                {
                                    if (letterDetail.receiver.Count() > 1)
                                    {
                                        paragraph = new Paragraph(":"+" "+no + ". " + item.fullname + " - " + item.positionName, normal);
                                        paragraph.Alignment = Element.ALIGN_LEFT;
                                        cell.AddElement(paragraph);
                                    }
                                    else
                                    {
                                        paragraph = new Paragraph(":"+" "+item.fullname + " - " + item.positionName, normal);
                                        paragraph.Alignment = Element.ALIGN_LEFT;
                                        cell.AddElement(paragraph);
                                    }
                                    no++;
                                }
                                table.AddCell(cell);

                                cell = new PdfPCell();
                                cell.Border = 0;
                                paragraph = new Paragraph("Tembusan", normal);
                                paragraph.Alignment = Element.ALIGN_LEFT;
                                cell.AddElement(paragraph);
                                table.AddCell(cell);

                                //cell = new PdfPCell();
                                //cell.Border = 0;
                                //paragraph = new Paragraph(":", normal);
                                //paragraph.Alignment = Element.ALIGN_RIGHT;
                                //cell.AddElement(paragraph);
                                //table.AddCell(cell);

                                cell = new PdfPCell();
                                cell.Border = 0;
                                no = 1;
                                foreach (var item in letterDetail.copy)
                                {
                                    if (letterDetail.copy.Count() > 1)
                                    {
                                        paragraph = new Paragraph(":"+" "+no + ". " + item.fullname + " - " + item.positionName, normal);
                                        paragraph.Alignment = Element.ALIGN_LEFT;
                                        cell.AddElement(paragraph);
                                    }
                                    else
                                    {
                                        paragraph = new Paragraph(":"+" "+item.fullname + " - " + item.positionName, normal);
                                        paragraph.Alignment = Element.ALIGN_LEFT;
                                        cell.AddElement(paragraph);
                                    }
                                    no++;
                                }
                                if (letterDetail.copy.Count() == 0)
                                {
                                    paragraph = new Paragraph("-", normal);
                                    paragraph.Alignment = Element.ALIGN_LEFT;
                                    cell.AddElement(paragraph);
                                }
                                table.AddCell(cell);

                                cell = new PdfPCell();
                                cell.Border = 0;
                                paragraph = new Paragraph("Dari", normal);
                                paragraph.Alignment = Element.ALIGN_LEFT;
                                cell.AddElement(paragraph);
                                table.AddCell(cell);

                                //cell = new PdfPCell();
                                //cell.Border = 0;
                                //paragraph = new Paragraph(":", normal);
                                //paragraph.Alignment = Element.ALIGN_RIGHT;
                                //cell.AddElement(paragraph);
                                //table.AddCell(cell);

                                cell = new PdfPCell();
                                cell.Border = 0;
                                paragraph = new Paragraph(":"+" "+letterDetail.sender[0].fullname + " - " + letterDetail.sender[0].positionName, normal);
                                paragraph.Alignment = Element.ALIGN_LEFT;
                                cell.AddElement(paragraph);
                                table.AddCell(cell);

                                cell = new PdfPCell();
                                cell.Border = 0;
                                paragraph = new Paragraph("Perihal", normal);
                                paragraph.Alignment = Element.ALIGN_LEFT;
                                cell.AddElement(paragraph);
                                table.AddCell(cell);

                                //cell = new PdfPCell();
                                //cell.Border = 0;
                                //paragraph = new Paragraph(":", normal);
                                //paragraph.Alignment = Element.ALIGN_RIGHT;
                                //cell.AddElement(paragraph);
                                //table.AddCell(cell);

                                cell = new PdfPCell();
                                cell.Border = 0;
                                paragraph = new Paragraph(":"+" "+letterDetail.letter.about, normal);
                                paragraph.Alignment = Element.ALIGN_LEFT;
                                cell.AddElement(paragraph);
                                table.AddCell(cell);

                                cell = new PdfPCell();
                                cell.Border = 0;
                                paragraph = new Paragraph("Lampiran", normal);
                                paragraph.Alignment = Element.ALIGN_LEFT;
                                cell.AddElement(paragraph);
                                table.AddCell(cell);

                                //cell = new PdfPCell();
                                //cell.Border = 0;
                                //paragraph = new Paragraph(":", normal);
                                //paragraph.Alignment = Element.ALIGN_RIGHT;
                                //cell.AddElement(paragraph);
                                //table.AddCell(cell);

                                var lampiran = letterDetail.letter.attachmentDesc == null ? "-" : letterDetail.letter.attachmentDesc;
                                cell = new PdfPCell();
                                cell.Border = 0;
                                paragraph = new Paragraph(":"+" "+lampiran, bold);
                                paragraph.Alignment = Element.ALIGN_LEFT;
                                cell.AddElement(paragraph);
                                table.AddCell(cell);

                                //Add table to document
                                pdfDoc.Add(table);


                        Paragraph line = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(2F, 100.0F, colorBlack, Element.ALIGN_CENTER, 1)));
                        line.SpacingBefore = 0f;
                        line.SetLeading(2F, 0.5F);
                        pdfDoc.Add(line);
                        Paragraph lines = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(2F, 100.0F, colorBlack, Element.ALIGN_CENTER, 1)));
                        lines.SpacingBefore = 0f;
                        line.SetLeading(0.5F, 0.5F);
                        pdfDoc.Add(line);

                        Paragraph liness = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(2F, 100.0F, colorWhite, Element.ALIGN_CENTER, 1)));
                        liness.SpacingBefore = 0f;
                        liness.SetLeading(0.5F, 0.5F);
                        pdfDoc.Add(liness);

                        //Paragraph linesss = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(2F, 100.0F, colorWhite, Element.ALIGN_CENTER, 1)));
                        //linesss.SpacingBefore = 0f;
                        //linesss.SetLeading(0.5F, 0.5F);
                        //pdfDoc.Add(linesss);


                        table = new PdfPTable(1);
                        width = new float[] { 2f };
                        table.WidthPercentage = 100;
                        table.SetWidths(width);
                        table.HorizontalAlignment = Element.ALIGN_LEFT;
                        table.DefaultCell.Border = 0;
                        table.SpacingBefore = 00f;
                        table.SpacingAfter = 0f;


                        //table.AddCell(new Phrase("Dengan hormat", normal));
                        //pdfDoc.Add(table);

                        Paragraph linessss = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(2F, 100.0F, colorWhite, Element.ALIGN_CENTER, 1)));
                        linessss.SpacingBefore = 0f;
                        linessss.SetLeading(0.5F, 0.5F);
                        pdfDoc.Add(linessss);

                        Paragraph lin = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(2F, 100.0F, colorWhite, Element.ALIGN_CENTER, 1)));
                        lin.SpacingBefore = 0f;
                        lin.SetLeading(0.5F, 0.5F);
                        pdfDoc.Add(lin);

                        #endregion

                        //ADD HTML CONTENT
                        htmlparser.Parse(sr);


                        #region Tanda Tangan

                        Paragraph a = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(2F, 100.0F, colorWhite, Element.ALIGN_CENTER, 1)));
                        a.SpacingBefore = 0f;
                        a.SetLeading(0.5F, 0.5F);
                        pdfDoc.Add(a);



                        cell = new PdfPCell();
                        table.AddCell(new Phrase("", normal));

                        cell = new PdfPCell();
                        table.AddCell(new Phrase(" ", normal));
                        cell = new PdfPCell();
                        table.AddCell(new Phrase(" ", normal));
                        cell = new PdfPCell();
                        table.AddCell(new Phrase("Hormat kami, \n" + "PT BNI Life Insurance", normal));
                        //table.AddCell(new Phrase("Best Regards", normal));
                        pdfDoc.Add(table);



                        string filename = letterDetail.sender[0].nip;


                        // Tanda tangan
                        var dirChecker = letterDetail.sender.OrderBy(p => p.idUserSender).ToList();
                        //var dirChecker = letterDetail.checker.Where(p => p.idLevelChecker == 2 || p.idLevelChecker == 1).ToList();
                        //var dirChecker = letterDetail.checker.Where(p => p.ordernumber == 1).ToList();

                        table = new PdfPTable(dirChecker.Count());
                        width = new float[dirChecker.Count()];
                        for (int i = 0; i < dirChecker.Count(); i++)
                        {
                            width[i] = 1f;
                        }
                        table.WidthPercentage = 100;
                        table.SetWidths(width);
                        table.HorizontalAlignment = Element.ALIGN_LEFT;
                        table.DefaultCell.Border = 0;
                        var no1 = 0;


                        //cell = new PdfPCell();
                        //table.AddCell(new Phrase("Direksi PT BNI Life Insurance", normal));
                        foreach (var item in dirChecker)
                        {
                            cell = new PdfPCell();
                            cell.Border = 0;




                            if (letterDetail.letter.letterNumber != "NO_LETTER")
                            {
                                if (letterDetail.letter.signatureType == 1)
                                {

                                    Image imagettd = GetSignatureImage(item.nip);
                                    //cell.AddElement(imagettd);
                                    if (imagettd != null)
                                    {
                                        cell.AddElement(imagettd);
                                    }
                                    else
                                    {
                                        var img = "File tanda tangan tidak ditemukan";
                                        paragraph = new Paragraph(img, normal);
                                        cell.AddElement(paragraph);
                                    }

                                }
                                else if (letterDetail.letter.signatureType == 2)
                                {


                                    Image QRCodeSignatureApprover = GetBarcodeSignature("NIP : " + item.nip + "\nNAMA : " + item.fullname + "\nJabatan : " + item.positionName);
                                    QRCodeSignatureApprover.ScaleAbsoluteHeight(10f);
                                    cell.AddElement(QRCodeSignatureApprover);
                                }
                                else if (letterDetail.letter.signatureType == 3)
                                {
                                    var img = "";
                                    paragraph = new Paragraph(img, normal);
                                    cell.AddElement(paragraph);
                                    Paragraph linessia = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(2F, 100.0F, colorWhite, Element.ALIGN_CENTER, 1)));
                                    linessia.SpacingBefore = 0f;
                                    linessia.SetLeading(0.5F, 0.5F);
                                    pdfDoc.Add(linessia);

                                    Paragraph linessiaa = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(2F, 100.0F, colorWhite, Element.ALIGN_CENTER, 1)));
                                    linessiaa.SpacingBefore = 0f;
                                    linessiaa.SetLeading(0.5F, 0.5F);
                                    pdfDoc.Add(linessiaa);

                                    Paragraph linessiaaa = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(2F, 100.0F, colorWhite, Element.ALIGN_CENTER, 1)));
                                    linessiaaa.SpacingBefore = 0f;
                                    linessiaaa.SetLeading(0.5F, 0.5F);
                                    pdfDoc.Add(linessiaaa);
                                }
                                else
                                {

                                    Image imagettd = GetSignatureImage(item.nip);
                                    cell.AddElement(imagettd);
                                    Image QRCodeSignatureApprover = GetBarcodeSignature("NIP : " + item.nip + "\nNAMA : " + item.fullname + "\nJabatan : " + item.positionName);
                                    cell.AddElement(QRCodeSignatureApprover);
                                    //if (imagettd != null)
                                    //{
                                    //    cell.AddElement(imagettd);
                                    //    Image QRCodeSignatureApprover = GetBarcodeSignature("NIP : " + item.nip + "\nNAMA : " + item.fullname + "\nJabatan : " + item.positionName);
                                    //    cell.AddElement(QRCodeSignatureApprover);
                                    //}
                                    //else
                                    //{
                                    //    var img = "File tanda tangan tidak di temukan";
                                    //    paragraph = new Paragraph(img, normal);
                                    //    cell.AddElement(paragraph);
                                    //    Image QRCodeSignatureApprover = GetBarcodeSignature("NIP : " + item.nip + "\nNAMA : " + item.fullname + "\nJabatan : " + item.positionName);
                                    //    cell.AddElement(QRCodeSignatureApprover);
                                    //}

                                }
                                no1++;
                            }

                            paragraph = new Paragraph(item.fullname, underlineBold);
                            cell.AddElement(paragraph);
                            paragraph = new Paragraph(item.positionName, bold);
                            cell.AddElement(paragraph);

                            table.AddCell(cell);
                        }
                        // end tanda tangan
                        pdfDoc.Add(table);

                        #endregion

                        pdfDoc.NewPage();

                        #region Lampiran

                        PdfPTable table1 = new PdfPTable(2);
                        float[] width1 = new float[] { 0.667f, 2f };
                        table1.WidthPercentage = 100;
                        table1.SetWidths(width1);
                        table1.HorizontalAlignment = Element.ALIGN_LEFT;
                        table1.DefaultCell.Border = 0;
                        table1.SpacingBefore = 0f;
                        table1.SpacingAfter = 0f;

                        PdfPCell cell1 = new PdfPCell();
                        Paragraph paragraph1 = new Paragraph();

                        cell1 = new PdfPCell();
                        cell1.BorderWidthLeft = 1f;
                        cell1.BorderWidthRight = 1f;
                        cell1.BorderWidthTop = 1f;
                        cell1.BorderWidthBottom = 1f;
                        cell1.MinimumHeight = 30f;
                        paragraph1 = new Paragraph("Nomor Registrasi Memo/Registration Number:" + letterDetail.letter.letterDeliberationNumber, normalDeliberation);
                        paragraph1.Alignment = Element.ALIGN_LEFT;
                        cell1.AddElement(paragraph1);
                        table1.AddCell(cell1);

                        cell1 = new PdfPCell();
                        cell1.BorderWidthLeft = 0f;
                        cell1.BorderWidthRight = 1f;
                        cell1.BorderWidthTop = 1f;
                        cell1.BorderWidthBottom = 1f;
                        cell1.MinimumHeight = 30f;
                        paragraph1 = new Paragraph("Tanggal Registrasi Memo/Date of Memo Registration : " + Convert.ToDateTime(letterDetail.letter.letterDate).ToString("dd MMMM yyyy"), normalDeliberation);
                        paragraph1.Alignment = Element.ALIGN_LEFT;
                        cell1.AddElement(paragraph1);
                        table1.AddCell(cell1);

                        cell1 = new PdfPCell();
                        cell1.BorderWidthLeft = 1f;
                        cell1.BorderWidthRight = 1f;
                        cell1.BorderWidthTop = 0f;
                        cell1.BorderWidthBottom = 1f;
                        cell1.MinimumHeight = 100f;
                        paragraph1 = new Paragraph("Nomor Memo : " + letterDetail.letter.letterNumber, normalDeliberation);
                        paragraph1.Alignment = Element.ALIGN_LEFT;
                        cell1.AddElement(paragraph1);
                        table1.AddCell(cell1);

                        cell1 = new PdfPCell();
                        cell1.BorderWidthLeft = 0f;
                        cell1.BorderWidthRight = 1f;
                        cell1.BorderWidthTop = 0f;
                        cell1.BorderWidthBottom = 1f;
                        cell1.MinimumHeight = 100f;
                        paragraph1 = new Paragraph("(Ringkasan Memo/Summary of the proposal) \n" + letterDetail.letterContent.summary, normalDeliberation);
                        paragraph1.Alignment = Element.ALIGN_LEFT;
                        cell1.AddElement(paragraph1);
                        table1.AddCell(cell1);

                        cell1 = new PdfPCell();
                        cell1.BorderWidthLeft = 1f;
                        cell1.BorderWidthRight = 1f;
                        cell1.BorderWidthTop = 0f;
                        cell1.BorderWidthBottom = 1f;
                        cell1.MinimumHeight = 100f;
                        paragraph1 = new Paragraph("Deliberation:", normalDeliberation);
                        paragraph1.Alignment = Element.ALIGN_LEFT;
                        cell1.AddElement(paragraph1);
                        no = 1;
                        foreach (var item in letterDetail.delibration)
                        {
                            if (letterDetail.receiver.Count() > 1)
                            {
                                paragraph1 = new Paragraph(no + ". " + item.fullname + " - " + item.positionName, normalDeliberation);
                                paragraph1.Alignment = Element.ALIGN_LEFT;
                                cell1.AddElement(paragraph1);
                            }
                            else
                            {
                                paragraph1 = new Paragraph(item.fullname + " - " + item.positionName, normalDeliberation);
                                paragraph1.Alignment = Element.ALIGN_LEFT;
                                cell1.AddElement(paragraph1);
                            }
                            no++;
                        }
                        table1.AddCell(cell1);

                        cell1 = new PdfPCell();
                        cell1.BorderWidthLeft = 0f;
                        cell1.BorderWidthRight = 1f;
                        cell1.BorderWidthTop = 0f;
                        cell1.BorderWidthBottom = 1f;
                        cell1.MinimumHeight = 100f;
                        paragraph1 = new Paragraph("Catatan / Notes : \n", underlineDeliberation);
                        paragraph1.Alignment = Element.ALIGN_LEFT;
                        cell1.AddElement(paragraph1);
                        no = 1;
                        foreach (var item in letterDetail.delibration)
                        {
                            if (letterDetail.receiver.Count() > 1)
                            {
                                paragraph1 = new Paragraph(no + ". " + item.commentdlbrt, normalDeliberation);
                                paragraph1.Alignment = Element.ALIGN_LEFT;
                                cell1.AddElement(paragraph1);
                            }
                            else
                            {
                                paragraph1 = new Paragraph(item.commentdlbrt, normalDeliberation);
                                paragraph1.Alignment = Element.ALIGN_LEFT;
                                cell1.AddElement(paragraph1);
                            }
                            no++;
                        }
                        table1.AddCell(cell1);

                        pdfDoc.Add(table1);

                        //var checkerList = letterDetail.checker.Where(p => p.ordernumber != 1).OrderBy(p => p.idLevelChecker).ToList();
                        var checkerList = letterDetail.checker.OrderBy(p => p.idLevelChecker).ToList();
                        foreach (var item in checkerList)
                        {
                            var getLogComment = letterDetail.log.Where(p => p.idUserLog == item.idUserChecker && p.description.Contains("pemeriksa ke")).OrderByDescending(p => p.createdOn).FirstOrDefault();
                            var comment = "";
                            string approveDate = "";
                            if (getLogComment != null)
                            {
                                comment = getLogComment.comment;
                                approveDate = Convert.ToDateTime(getLogComment.createdOn).ToString("dd MMMM yyyy");
                            }
                            table1 = new PdfPTable(3);
                            width = new float[] { 1f, 2f, 1f };
                            table1.WidthPercentage = 100;
                            table1.SetWidths(width);
                            table1.HorizontalAlignment = Element.ALIGN_LEFT;
                            table1.DefaultCell.Border = 0;
                            table1.SpacingBefore = 0f;
                            table1.SpacingAfter = 0f;

                            cell1 = new PdfPCell();
                            cell1.Rowspan = 2;
                            cell1.BorderWidthLeft = 1f;
                            cell1.BorderWidthRight = 1f;
                            cell1.BorderWidthTop = 0f;
                            cell1.BorderWidthBottom = 1f;
                            cell1.MinimumHeight = 120f;
                            if (letterDetail.letter.statusCode == 2)
                                {
                                    paragraph1 = new Paragraph("Belum diputuskan / Undecided \n", normalDeliberation);
                                }
                                else if (letterDetail.letter.statusCode == 5)
                                {
                                    paragraph1 = new Paragraph("Ditolak / Reject \n", normalDeliberation);
                                }
                                else
                                {
                                   if(item.idLevelChecker == 2 ||item.idLevelChecker == 1)
                                   { 
                                       paragraph1 = new Paragraph("Disetujui / Approved \n", normalDeliberation);
                                   }
                                   else
                                   {
                                       paragraph1 = new Paragraph("Mengetahui / Understand \n", normalDeliberation);
                                       
                                   }
                                }

                            paragraph1.Alignment = Element.ALIGN_LEFT;
                            cell1.AddElement(paragraph1);
                            paragraph1 = new Paragraph(item.positionName, normalDeliberation);
                            paragraph1.Alignment = Element.ALIGN_LEFT;
                            cell1.AddElement(paragraph1);
                            table1.AddCell(cell1);

                            cell1 = new PdfPCell();
                            cell1.Rowspan = 2;
                            cell1.BorderWidthLeft = 0f;
                            cell1.BorderWidthRight = 1f;
                            cell1.BorderWidthTop = 0f;
                            cell1.BorderWidthBottom = 1f;
                            cell1.MinimumHeight = 120f;
                            paragraph1 = new Paragraph("Catatan / Notes : \n", underlineDeliberation);
                            paragraph1.Alignment = Element.ALIGN_LEFT;
                            cell1.AddElement(paragraph1);
                            paragraph1 = new Paragraph(comment, normalDeliberation);
                            paragraph1.Alignment = Element.ALIGN_LEFT;
                            cell1.AddElement(paragraph1);
                            table1.AddCell(cell1);

                            cell1 = new PdfPCell();
                            cell1.BorderWidthLeft = 0f;
                            cell1.BorderWidthRight = 1f;
                            cell1.BorderWidthTop = 0f;
                            cell1.BorderWidthBottom = 1f;
                            cell1.MinimumHeight = 40f;
                            paragraph1 = new Paragraph("Tanggal : ", underlineDeliberation);
                            paragraph1.Alignment = Element.ALIGN_LEFT;
                            //cell1.AddElement(paragraph);
                            paragraph1 = new Paragraph(approveDate, normalDeliberation);
                            paragraph1.Alignment = Element.ALIGN_LEFT;
                            cell1.AddElement(paragraph1);
                            table1.AddCell(cell1);

                            cell1 = new PdfPCell();
                            cell1.BorderWidthLeft = 0f;
                            cell1.BorderWidthRight = 1f;
                            cell1.BorderWidthTop = 0f;
                            cell1.BorderWidthBottom = 1f;
                            paragraph1 = new Paragraph("Tanda Tangan : \n", underlineDeliberation);
                            paragraph1.Alignment = Element.ALIGN_LEFT;
                            cell1.AddElement(paragraph1);
                            if (letterDetail.letter.letterNumber != "NO_LETTER")
                            {
                                if (letterDetail.letter.signatureType == 1)
                                {
                                    Image imagettd = GetSignatureImage(item.nip);
                                    //cell.AddElement(imagettd);
                                    if (imagettd != null)
                                    {
                                        cell1.AddElement(imagettd);
                                    }
                                    else
                                    {
                                        var img = "File tanda tangan tidak ditemukan";
                                        paragraph1 = new Paragraph(img, normal);
                                        cell1.AddElement(paragraph1);
                                    }
                                }
                                else if (letterDetail.letter.signatureType == 2)
                                {

                                    Image QRCodeSignatureApprover = GetBarcodeSignature("NIP : " + item.nip + "\nNAMA : " + item.fullname + "\nJabatan : " + item.positionName);
                                    cell1.AddElement(QRCodeSignatureApprover);
                                }
                                else if (letterDetail.letter.signatureType == 3)
                                {
                                    var img = "";
                                    paragraph = new Paragraph(img, normal);
                                    cell.AddElement(paragraph);
                                    Paragraph linessia = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(2F, 100.0F, colorWhite, Element.ALIGN_CENTER, 1)));
                                    linessia.SpacingBefore = 0f;
                                    linessia.SetLeading(0.5F, 0.5F);
                                    pdfDoc.Add(linessia);

                                    Paragraph linessiaa = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(2F, 100.0F, colorWhite, Element.ALIGN_CENTER, 1)));
                                    linessiaa.SpacingBefore = 0f;
                                    linessiaa.SetLeading(0.5F, 0.5F);
                                    pdfDoc.Add(linessiaa);

                                    Paragraph linessiaaa = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(2F, 100.0F, colorWhite, Element.ALIGN_CENTER, 1)));
                                    linessiaaa.SpacingBefore = 0f;
                                    linessiaaa.SetLeading(0.5F, 0.5F);
                                    pdfDoc.Add(linessiaaa);
                                }
                                else
                                {
                                    //innertable1 = new PdfPtable1(2);
                                    //float[] widthinner = new float[] { 0.3f, 0.3f };
                                    //innertable1.WidthPercentage = 100;
                                    //innertable1.SetWidths(widthinner);
                                    //innertable1.HorizontalAlignment = Element.ALIGN_LEFT;
                                    //innertable1.DefaultCell.Border = 1;
                                    //innertable1.DefaultCell.PaddingBottom = 8;

                                    //innercell = new PdfPCell();
                                    //innercell.BorderWidthLeft = 1f;
                                    //innercell.BorderWidthRight = 1f;
                                    //innercell.BorderWidthTop = 1f;
                                    //innercell.BorderWidthBottom = 1f;




                                    Image imagettd = GetSignatureImage(item.nip);
                                    cell1.AddElement(imagettd);

                                    Image QRCodeSignatureApprover = GetBarcodeSignature("NIP : " + item.nip + "\nNAMA : " + item.fullname + "\nJabatan : " + item.positionName);
                                    cell1.AddElement(QRCodeSignatureApprover);
                                    //if (imagettd !=null)
                                    //{
                                    //    cell.AddElement(imagettd);

                                    //    Image QRCodeSignatureApprover = GetBarcodeSignature("NIP : " + item.nip + "\nNAMA : " + item.fullname + "\nJabatan : " + item.positionName);
                                    //    cell.AddElement(QRCodeSignatureApprover);
                                    //}
                                    //else
                                    //{
                                    //    var img = "File tanda tangan tidak di temukan";
                                    //    paragraph = new Paragraph(img, normal);

                                    //    cell.AddElement(paragraph);
                                    //    Image QRCodeSignatureApprover = GetBarcodeSignature("NIP : " + item.nip + "\nNAMA : " + item.fullname + "\nJabatan : " + item.positionName);
                                    //    cell.AddElement(QRCodeSignatureApprover);
                                    //}

                                }
                            }

                            paragraph1 = new Paragraph(item.fullname, bold);
                            paragraph1.Alignment = Element.ALIGN_CENTER;
                            cell1.AddElement(paragraph1);
                            table1.AddCell(cell1);


                            pdfDoc.Add(table1);
                        }


                        #endregion

                        pdfDoc.Close();

                        var dateLetterFormat = Convert.ToDateTime(letterDetail.letter.letterDate).ToString("dd MMMM yyyy");
                        var letterNumber = letterDetail.letter.letterNumber;
                        byte[] bytess = ms.ToArray();
                        byte[] bytes = AddPageNumbers(bytess, dateLetterFormat, letterNumber, "Memo");
                        ms.Close();

                        return File(bytes.ToArray(), "application/pdf", fileName);
                    }

                    #endregion
            }

        #endregion

        // Kelas untuk membuat header
        public class PDFHeaderEvent : PdfPageEventHelper
        {
            public override void OnStartPage(PdfWriter writer, Document document)
            {
                var fontName = "Calibri";
                Font boldMemo = FontFactory.GetFont(fontName, 16, Font.BOLD, BaseColor.BLACK);
                // Buat tabel dengan dua kolom
                PdfPTable table = new PdfPTable(1);
                table.TotalWidth = document.PageSize.Width - document.LeftMargin - document.RightMargin;
                table.DefaultCell.Border = Rectangle.NO_BORDER;
                table.SetWidths(new float[] {table.TotalWidth - 60f });


                // Tambahkan logo ke kolom pertama
                using (var client = new WebClient())
                {
                    //byte[] imageBytes = client.DownloadData("http://10.22.13.34/EOFFICEBNILWEB/download/logo/logobni.png");
                    //byte[] imageBytes = client.DownloadData("http://localhost:5094/EOFFICEBNILWEB/download/logo/logobni.png");
                    byte[] imageBytes = client.DownloadData("http://172.20.20.156/EOFFICEBNILWEB/download/logo/logobni.png");
                    using (var stream = new MemoryStream(imageBytes))
                    {
                        Image logo = Image.GetInstance(stream);
                        logo.ScaleToFit(90f, 90f);
                        PdfPCell logoCell = new PdfPCell(logo);
                        logoCell.Border = 0;
                        logoCell.HorizontalAlignment = Element.ALIGN_RIGHT;
                        table.AddCell(logoCell);
                    }
                }

                //Tambahkan nama ke kolom kedua
                PdfPCell nameCell = new PdfPCell(new Phrase("MEMO",boldMemo));
                nameCell.Border = 0;
                nameCell.HorizontalAlignment = Element.ALIGN_LEFT;
                table.AddCell(nameCell);

                // Tempatkan tabel di atas dokumen
                table.WriteSelectedRows(0, -1, document.LeftMargin, document.PageSize.Height - 8,writer.DirectContent);


                
            }
        }

        public class PDFHeaderEvent2: PdfPageEventHelper
        {
            public override void OnStartPage(PdfWriter writer, Document document)
            {
                // Buat tabel dengan dua kolom
                PdfPTable table = new PdfPTable(1);
                table.TotalWidth = document.PageSize.Width - document.LeftMargin - document.RightMargin;
                table.DefaultCell.Border = Rectangle.NO_BORDER;
                table.SetWidths(new float[] { table.TotalWidth - 60f });


                // Buat tabel dengan dua kolom
                PdfPTable table1 = new PdfPTable(1);
                table1.TotalWidth = document.PageSize.Width - document.LeftMargin - document.RightMargin;
                table1.DefaultCell.Border = Rectangle.NO_BORDER;
                table1.SetWidths(new float[] { table.TotalWidth - 60f });




                //Tambahkan nama ke kolom kedua
                PdfPCell nameCell1 = new PdfPCell(new Phrase("Nama Anda"));
                nameCell1.Border = 1;
                nameCell1.HorizontalAlignment = Element.ALIGN_LEFT;
                table1.AddCell(nameCell1);

                // Tempatkan tabel di atas dokumen
                table1.WriteSelectedRows(0, -2, document.LeftMargin, document.PageSize.Height - 7,writer.DirectContent);



            }
        }

        public static byte[] AddPageNumbers(byte[] pdf, string dateLetterFormat, string letterNumber, string letterType)
        {
            MemoryStream ms = new MemoryStream();
            // we create a reader for a certain document
            PdfReader reader = new PdfReader(pdf);
            // we retrieve the total number of pages
            int n = reader.NumberOfPages;
            // we retrieve the size of the first page
            Rectangle psize = reader.GetPageSize(1);

            // step 1: creation of a document-object
            Document document = new Document(psize, 50, 50, 50, 150);
            // step 2: we create a writer that listens to the document
            PdfWriter writer = PdfWriter.GetInstance(document, ms);
            // step 3: we open the document

            document.Open();
            // step 4: we add content
            PdfContentByte cb = writer.DirectContent;

            int p = 0;
            //Console.WriteLine("There are " + n + " pages in the document.");
            for (int page = 1; page <= reader.NumberOfPages; page++)
            {
                document.NewPage();
                p++;

                PdfImportedPage importedPage = writer.GetImportedPage(reader, page);
                cb.AddTemplate(importedPage, 0, 0);

                BaseFont bf = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);

                cb.MoveTo(420, 10);
                cb.LineTo(420, 30);
                cb.Stroke();
                cb.MoveTo(420, 10);
                cb.LineTo(580, 10);
                cb.Stroke();
                cb.MoveTo(580, 30);
                cb.LineTo(420, 30);
                cb.Stroke();
                cb.MoveTo(500, 10);
                cb.LineTo(500, 30);
                cb.Stroke();
                cb.MoveTo(580, 10);
                cb.LineTo(580, 30);
                cb.Stroke();

                cb.BeginText();
                cb.SetFontAndSize(bf, 10);
                cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, letterType + " No. " + letterNumber + " tanggal " + dateLetterFormat, 10, 10, 0);
                cb.ShowTextAligned(PdfContentByte.ALIGN_RIGHT, "Page " + p + " of " + n, 490, 15, 0);

                cb.EndText();
            }
            // step 5: we close the document
            document.Close();
            return ms.ToArray();
        }

        public Image GetBarcodeSignature(string text)
        {
            string QRCode = "";
            QRCodeModel myQRCode = new QRCodeModel();
            myQRCode.QRCodeText = text;
            using (QRCodeGenerator qrGenerator = new QRCodeGenerator())
            using (QRCodeData qRCodeData = qrGenerator.CreateQrCode(
                myQRCode.QRCodeText, QRCodeGenerator.ECCLevel.Q))
            using (QRCode qRCode = new QRCode(qRCodeData))
            {
                Bitmap qrCodeImage = qRCode.GetGraphic(1);
                byte[] BitmapArray = qrCodeImage.ConvertBitmapToByteArray();
                string urlImgQrcode = string.Format("data:image/png;base64,{0}", Convert.ToBase64String(BitmapArray));
                QRCode = urlImgQrcode;
            }

            Regex regex = new Regex(@"^data:image/(?<mediaType>[^;]+);base64,(?<data>.*)");
            Match match = regex.Match(QRCode);
            Image imagettdqrcode = Image.GetInstance(
                Convert.FromBase64String(match.Groups["data"].Value)
            );

            return imagettdqrcode;
        }
        public Image GetSignatureImage(string filename)
        {

            string filePath = Path.Combine("wwwroot\\uploads\\imgsignature\\" + filename + ".png");
            string filettd = Path.GetFileName(filePath);

            string pathttd = $"{this.Request.Scheme}://{this.Request.Host}{this.Request.PathBase}" + "/uploads/imgsignature/" + filettd;
            //cell.Border = 0;

            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(pathttd);
            request.Method = "HEAD";

            bool exists;
            try
            {
                request.GetResponse();
                exists = true;

            }
            catch
            {
                return null;
            }



            Image imagettd = Image.GetInstance(pathttd);
            float newWidth = 120;
            float newHeight = 150;

            // Atur lebar dan tinggi gambar
            imagettd.ScaleToFit(newWidth, newHeight);

            // Atau Anda bisa menentukan skala persentase
            imagettd.ScalePercent(35); // skala 50%
            return imagettd;
        }

        public Image GetLogoImage(string filename)
        {

            string filePath = Path.Combine("wwwroot\\download\\logo\\" + filename + ".png");
            string filettd = Path.GetFileName(filePath);
            string pathttd = $"{this.Request.Scheme}://{this.Request.Host}{this.Request.PathBase}" + "/download/logo/" + filettd;
            //cell.Border = 0;

            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(pathttd);
            request.Method = "HEAD";

            bool exists;
            try
            {
                request.GetResponse();
                exists = true;

            }
            catch
            {
                return null;
            }


            Image imagettd = Image.GetInstance(pathttd);
            return imagettd;
        }

        [HttpPost]
        public async Task<string> GetDataPemeriksaDiv(string keyword)
        {
            DocumentOutputWeb documentOutput = new DocumentOutputWeb();
            GeneralOutputModel generalOutput = new GeneralOutputModel();
            List<AdminDivisionOutput> receiverList = new List<AdminDivisionOutput>();
            var token = HttpContext.Session.GetString("token");
            Guid idUnit = new Guid(HttpContext.Session.GetString("idUnit"));

            ParamGetPenerima pr = new ParamGetPenerima();
            pr.keyword = keyword;

            generalOutput = await _dataAccessProvider.GetDataPenerima("General/GetDataPemeriksaDivWeb/", token, pr);
            var jsonApiResponseSerialize = JsonConvert.SerializeObject(generalOutput.Result);
            receiverList = JsonConvert.DeserializeObject<List<AdminDivisionOutput>>(jsonApiResponseSerialize).Where(p => p.idLevel != 0).ToList();
            jsonApiResponseSerialize = JsonConvert.SerializeObject(receiverList);

            return jsonApiResponseSerialize;
        }


        [HttpPost]
        public async Task<string> GetDataPemeriksaDivLainya(string keyword)
        {
            DocumentOutputWeb documentOutput = new DocumentOutputWeb();
            GeneralOutputModel generalOutput = new GeneralOutputModel();
            List<AdminDivisionOutput> receiverList = new List<AdminDivisionOutput>();
            var token = HttpContext.Session.GetString("token");
            Guid idUnit = new Guid(HttpContext.Session.GetString("idUnit"));

            ParamGetPenerima pr = new ParamGetPenerima();
            pr.keyword = keyword;

            generalOutput = await _dataAccessProvider.GetDataPenerima("General/GetDataPemeriksaDivLainyaWeb/", token, pr);
            var jsonApiResponseSerialize = JsonConvert.SerializeObject(generalOutput.Result);
            receiverList = JsonConvert.DeserializeObject<List<AdminDivisionOutput>>(jsonApiResponseSerialize).Where(p => p.idLevel != 0).ToList();
            jsonApiResponseSerialize = JsonConvert.SerializeObject(receiverList);

            return jsonApiResponseSerialize;
        }


        [HttpPost]
        public async Task<string> SearchMemo(ParamGetMemoWeb pr)
        {
            
            MemoOutputWeb letterList = new MemoOutputWeb();
            GeneralOutputModel generalOutput = new GeneralOutputModel();
            var token = HttpContext.Session.GetString("token");
            int drawww = Convert.ToInt32(pr.draw);
            int draww= drawww > 0 ? drawww : 1;
            int draw = draww;
            pr.pageSize = 20;   
            pr.start = (draw - 1) * pr.pageSize;
            pr.draw = pr.start == 0 ? "1" : Convert.ToString(pr.start + 1);
          var  urlApi = "Memo/GetDistribusiMemoWeb";
            //var urlApi = "Letter/GetInbox/";
            //if (pr.searchType == 2)
            //{
            //    urlApi = "Memo/GetDistribusiMemoWeb";
            //}
            //else if (pr.searchType == 3)
            //{
            //    urlApi = "Memo/GetDraftLetter";
            //}
            //else if (pr.searchType == 4)
            //{
            //    urlApi = "Memo/GetDraftLetter";
            //}
            //else if (pr.searchType == 5)
            //{
            //    urlApi = "Memo/GetDraftLetter";
            //}

            generalOutput = await _dataAccessProvider.GetLetterDraftMemoSrch(urlApi, token, pr);
            var jsonApiResponseSerialize = JsonConvert.SerializeObject(generalOutput.Result);
            letterList = JsonConvert.DeserializeObject<MemoOutputWeb>(jsonApiResponseSerialize);
            jsonApiResponseSerialize = JsonConvert.SerializeObject(letterList);

            return jsonApiResponseSerialize;
        }


        public async Task<IActionResult> DeleteMemo(string param)
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
                TempData["title"] = "Berhasil menghapus Memo";
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
            return Redirect("~/Memo/DraftMemo");
        }


        [HttpPost]
        public async Task<string> SearchDraftMemo(ParamGetMemoWeb pr)
        {
            MemoOutputWeb letterList = new MemoOutputWeb();
            GeneralOutputModel generalOutput = new GeneralOutputModel();
            var token = HttpContext.Session.GetString("token");
            int draw = Convert.ToInt32(pr.draw);
            pr.pageSize = 20;
            pr.start = (draw - 1) * pr.pageSize;
            pr.draw = pr.start == 0 ? "1" : Convert.ToString(pr.start + 1);
            var urlApi = "Memo/GetDraftMemoWeb";
            //var urlApi = "Letter/GetInbox/";
            //if (pr.searchType == 2)
            //{
            //    urlApi = "Memo/GetDistribusiMemoWeb";
            //}
            //else if (pr.searchType == 3)
            //{
            //    urlApi = "Memo/GetDraftLetter";
            //}
            //else if (pr.searchType == 4)
            //{
            //    urlApi = "Memo/GetDraftLetter";
            //}
            //else if (pr.searchType == 5)
            //{
            //    urlApi = "Memo/GetDraftLetter";
            //}

            generalOutput = await _dataAccessProvider.GetLetterDraftMemoSrch(urlApi, token, pr);
            var jsonApiResponseSerialize = JsonConvert.SerializeObject(generalOutput.Result);
            letterList = JsonConvert.DeserializeObject<MemoOutputWeb>(jsonApiResponseSerialize);
            jsonApiResponseSerialize = JsonConvert.SerializeObject(letterList);

            return jsonApiResponseSerialize;
        }


        [HttpPost]
        public async Task<string> UpdateStatusNotifikasi(string? idStatusNotif)
        {
            ParamUpdateNotif pr = new ParamUpdateNotif();
            pr.idNotif = new Guid(idStatusNotif);
            GeneralOutputModel generalOutput = new GeneralOutputModel();
            var token = HttpContext.Session.GetString("token");

            generalOutput = await _dataAccessProvider.UpdateStatusNotifikasi("General/UpdateStatusNotifikasi/", token, pr);
            var jsonApiResponseSerialize = JsonConvert.SerializeObject(generalOutput);
            generalOutput = JsonConvert.DeserializeObject<GeneralOutputModel>(jsonApiResponseSerialize);
            return JsonConvert.SerializeObject("ok");
        }
    
    
    
    }
}
