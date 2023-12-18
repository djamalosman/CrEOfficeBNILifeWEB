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
using System.Reflection.Metadata.Ecma335;
using DocumentFormat.OpenXml.Office2016.Excel;

namespace EofficeBNILWEB.Controllers
{
    public class BackDateController : Controller
    {
        private readonly IConfiguration _config;
        private readonly IDataAccessProvider _dataAccessProvider;
        private readonly IHostingEnvironment _environment;
        private readonly string urlApi;
        private readonly IHtmlLocalizer<HomeController> _localiza;
        public FtpSettings _ftpconfig { get; }
        public BackDateController(IConfiguration config, IDataAccessProvider dataAccessProvider, IHostingEnvironment Environment, IHtmlLocalizer<HomeController> localiza, IOptions<FtpSettings> ftpconfig)
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
        public async Task<IActionResult> SaveLetterMemo(ParamInsertMemoBackDateWeb pr, IFormFile inputfile)
        {
            try
            {
                var token = HttpContext.Session.GetString("token");
                GeneralOutputModel generalOutput = new GeneralOutputModel();
                ParamInsertMemoBackdate prApi = new ParamInsertMemoBackdate();
                List<ParamInsertCheckerMemo> prApiChecker = new List<ParamInsertCheckerMemo>();
                List<ParamInsertCheckerMemoPengirim> prApiCheckerPengirim = new List<ParamInsertCheckerMemoPengirim>();

                List<ParamInsertCheckerMemoPenerima> prApiCheckerPenerima = new List<ParamInsertCheckerMemoPenerima>();
                List<ParamInsertCheckerMemoCarbonCopy> prApiCheckerCarbonCopy = new List<ParamInsertCheckerMemoCarbonCopy>();
                List<ParamInsertSendMemogoingRecipient> prApiReceiver = new List<ParamInsertSendMemogoingRecipient>();
                List<ParamInsertDelibrationMemo> prApiDelibration = new List<ParamInsertDelibrationMemo>();
                List<ParamInsertApproverMemo> prApiApprover = new List<ParamInsertApproverMemo>();
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
                                idUnitChecker = idUnitChecker,
                                Is_Approver = 0 ,
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
                                is_approver = 1,
                            });
                        }
                    }

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

                    var checksenderischecker = prApiCheckerPengirim.Where(p => p.idUserChecker == pr.bossUserId).FirstOrDefault();
                    if (checksenderischecker == null)
                    {
                        prApiCheckerPengirim.Add(new ParamInsertCheckerMemoPengirim
                        {
                            idUserChecker = pr.bossUserId,
                            idLevelChecker = pr.bossLevelId,
                            idUnitChecker = pr.bossUnitId
                        });
                    }



                    if (pr.idUserCheckerDelibretion.Length > 0)
                    {
                        for (int a = 0; a < pr.idUserCheckerDelibretion.Length; a++)
                        {
                            Guid idDelibretion = new Guid(pr.idUserCheckerDelibretion.GetValue(a).ToString());
                            int idLevel = Convert.ToInt32(pr.idLevelCheckerDelibretion.GetValue(a).ToString());
                            Guid idUnitDelibretion = new Guid(pr.idUnitCheckerDelibretion.GetValue(a).ToString());
                            prApiDelibration.Add(new ParamInsertDelibrationMemo
                            {
                                idUserDelibration = idDelibretion,
                                idLevelDelibration = idLevel,
                                idUnitDelibration = idUnitDelibretion
                            });
                        }
                    }


                    prApi.idUserChecker = prApiChecker;
                    prApi.idUserCheckerPengirim = prApiCheckerPengirim;
                    prApi.idUserApprover = prApiApprover;
                    prApi.idUserCheckerPenerima = prApiCheckerPenerima;
                    prApi.idUserCheckerCarbonCopy = prApiCheckerCarbonCopy;
                    prApi.idUserDelibration = prApiDelibration;
                    prApi.idUserCheckerlain = prApiPemeriksaLainya;
                    //prApi.senderName = prApiReceiver;
                }

                prApi.idresponsesurat = responseApiAttachment.idLetter == Guid.Empty ? pr.idresponsesurat : responseApiAttachment.idLetter;
                prApi.saveType = pr.saveType;
                prApi.comment = pr.comment;
                prApi.summary = pr.summary;
                //prApi.senderCompanyName = pr.senderCompanyName;

                generalOutput = await _dataAccessProvider.InsertLetterMemoBackdate("Memo/InsertMemoBackDateLetter", token, prApi);
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
                ParamInsertAttachmentMemo prAttachment = new ParamInsertAttachmentMemo();
                int print = pr.saveType;
                if (pr.saveType == 3)
                {
                    pr.saveType = 1;
                }
                if (pr.saveType == 1 || pr.saveType == 2)
                {
                    prApi.outboxType = pr.outboxType;
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
                    //prApi.senderName = pr.senderName;
                    //prApi.senderAddress = pr.senderAddress;
                    prApi.isiSurat = pr.isiSurat;

                    //if (pr.senderName.Length > 0)
                    //{
                    //    for (int i = 0; i < pr.senderName.Length; i++)
                    //    {
                    //        string senderName = pr.senderName.GetValue(i).ToString();
                    //        prApiReceiver.Add(new ParamInsertOutgoingRecipient
                    //        {
                    //            senderName = senderName,
                    //            senderCompanyName = pr.senderCompanyName,
                    //            senderAddress = pr.senderAddress
                    //        });
                    //    }
                    //}

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
                    prApi.idUserChecker = prApiChecker;
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

            return View(letterList);
        }

        public async Task<IActionResult> DraftMemo()
        {
            MemoOutputWeb letterList = new MemoOutputWeb();
            GeneralOutputModel generalOutput = new GeneralOutputModel();
            var token = HttpContext.Session.GetString("token");

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

            return View(letterList);
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

        public async Task<FileResult> PrintLetterMemo(Guid id)
        {
            var token = HttpContext.Session.GetString("token");
            GeneralOutputModel generalOutput = new GeneralOutputModel();
            OutputDetailMemo letterDetail = new OutputDetailMemo();
            ParamGetDetailMemo pr = new ParamGetDetailMemo();
            pr.idLetter = id;

            generalOutput = await _dataAccessProvider.GetDetailMemo("Memo/GetDetailMemoWeb/", token, pr);
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
            Document pdfDoc = new Document(PageSize.A4, 25, 25, 25, 15);
            HTMLWorker htmlparser = new HTMLWorker(pdfDoc);

            using (MemoryStream ms = new MemoryStream())
            {
                PdfWriter pdfWriter = PdfWriter.GetInstance(pdfDoc, ms);
                pdfDoc.Open();
                //Font
                var fontName = "Calibri";
                string fontPath = Path.Combine(_environment.WebRootPath, "fonts/Calibri.ttf");
                //FontFactory.Register(fontPath);

                Font normal = FontFactory.GetFont(fontName, 12, Font.NORMAL, BaseColor.BLACK);
                Font bold = FontFactory.GetFont(fontName, 12, Font.BOLD, BaseColor.BLACK);
                Font underline = FontFactory.GetFont(fontName, 12, Font.UNDERLINE, BaseColor.BLACK);
                Font underlineBold = FontFactory.GetFont(fontName, 12, Font.UNDERLINE, BaseColor.BLACK);
                underlineBold.SetStyle(Font.BOLD);

                PdfPTable table = new PdfPTable(3);
                float[] width = new float[] { 0.3f, 0.1f, 2f };
                table.WidthPercentage = 100;
                table.SetWidths(width);
                table.HorizontalAlignment = Element.ALIGN_RIGHT;
                table.DefaultCell.Border = 0;
                table.SpacingBefore = 20f;
                table.SpacingAfter = 0f;

                PdfPCell cell = new PdfPCell();
                cell.Border = 0;
                Paragraph paragraph = new Paragraph("No", normal);
                paragraph.Alignment = Element.ALIGN_LEFT;
                cell.AddElement(paragraph);
                table.AddCell(cell);

                cell = new PdfPCell();
                cell.Border = 0;
                paragraph = new Paragraph(":", normal);
                paragraph.Alignment = Element.ALIGN_LEFT;
                cell.AddElement(paragraph);
                table.AddCell(cell);

                cell = new PdfPCell();
                cell.Border = 0;
                paragraph = new Paragraph(letterDetail.letter.letterNumber, normal);
                paragraph.Alignment = Element.ALIGN_LEFT;
                cell.AddElement(paragraph);
                table.AddCell(cell);

                cell = new PdfPCell();
                cell.Border = 0;
                paragraph = new Paragraph("Perihal", normal);
                paragraph.Alignment = Element.ALIGN_LEFT;
                cell.AddElement(paragraph);
                table.AddCell(cell);

                cell = new PdfPCell();
                cell.Border = 0;
                paragraph = new Paragraph(":", normal);
                paragraph.Alignment = Element.ALIGN_LEFT;
                cell.AddElement(paragraph);
                table.AddCell(cell);

                cell = new PdfPCell();
                cell.Border = 0;
                paragraph = new Paragraph(letterDetail.letter.about, normal);
                paragraph.Alignment = Element.ALIGN_LEFT;
                cell.AddElement(paragraph);
                table.AddCell(cell);

                //cell = new PdfPCell();
                //cell.Border = 0;
                //paragraph = new Paragraph("Lampiran", normal);
                //paragraph.Alignment = Element.ALIGN_LEFT;
                //cell.AddElement(paragraph);
                //table.AddCell(cell);

                //cell = new PdfPCell();
                //cell.Border = 0;
                //paragraph = new Paragraph(":", normal);
                //paragraph.Alignment = Element.ALIGN_LEFT;
                //cell.AddElement(paragraph);
                //table.AddCell(cell);

                //cell = new PdfPCell();
                //cell.Border = 0;
                //paragraph = new Paragraph("(*apabila diperlukan)", bold);
                //paragraph.Alignment = Element.ALIGN_LEFT;
                //cell.AddElement(paragraph);
                //table.AddCell(cell);

                //Add table to document
                pdfDoc.Add(table);

                table = new PdfPTable(1);
                width = new float[] { 2f };
                table.WidthPercentage = 100;
                table.SetWidths(width);
                table.HorizontalAlignment = Element.ALIGN_LEFT;
                table.DefaultCell.Border = 0;
                table.SpacingBefore = 20f;
                table.SpacingAfter = 0f;

                cell = new PdfPCell();
                table.AddCell(new Phrase("Jakarta, " + Convert.ToDateTime(letterDetail.letter.letterDate).ToString("dd MMMM yyyy"), normal));
                pdfDoc.Add(table);

                table = new PdfPTable(1);
                width = new float[] { 2f };
                table.WidthPercentage = 100;
                table.SetWidths(width);
                table.HorizontalAlignment = Element.ALIGN_LEFT;
                table.DefaultCell.Border = 0;
                table.SpacingBefore = 20f;
                table.SpacingAfter = 0f;

                cell = new PdfPCell();
                table.AddCell(new Phrase("Kepada Yth, ", normal));
                table.AddCell(new Phrase(letterDetail.receiver[0].fullname, bold));
                //table.AddCell(new Phrase(letterDetail.outgoingRecipient[0].recipientAddress, normal));
                pdfDoc.Add(table);

                table = new PdfPTable(1);
                width = new float[] { 2f };
                table.WidthPercentage = 100;
                table.SetWidths(width);
                table.HorizontalAlignment = Element.ALIGN_LEFT;
                table.DefaultCell.Border = 0;
                table.SpacingBefore = 20f;
                table.SpacingAfter = 0f;

                cell = new PdfPCell();
                table.AddCell(new Phrase("Up    " + letterDetail.sender[0].fullname, underlineBold));
                table.AddCell(new Phrase("         " + letterDetail.sender[0].positionName, bold));
                pdfDoc.Add(table);

                table = new PdfPTable(1);
                width = new float[] { 2f };
                table.WidthPercentage = 100;
                table.SetWidths(width);
                table.HorizontalAlignment = Element.ALIGN_LEFT;
                table.DefaultCell.Border = 0;
                table.SpacingBefore = 20f;
                table.SpacingAfter = 0f;

                cell = new PdfPCell();
                table.AddCell(new Phrase("Dengan Hormat, ", normal));
                pdfDoc.Add(table);

                //ADD HTML CONTENT
                htmlparser.Parse(sr);

                table = new PdfPTable(1);
                width = new float[] { 2f };
                table.WidthPercentage = 100;
                table.SetWidths(width);
                table.HorizontalAlignment = Element.ALIGN_LEFT;
                table.DefaultCell.Border = 0;
                table.SpacingBefore = 20f;
                table.SpacingAfter = 0f;

                cell = new PdfPCell();
                table.AddCell(new Phrase("Demikian kami sampaikan, Atas perhatian dan kerjasamanya, kami ucapkan terima kasih.", normal));
                pdfDoc.Add(table);

                table = new PdfPTable(1);
                width = new float[] { 2f };
                table.WidthPercentage = 100;
                table.SetWidths(width);
                table.HorizontalAlignment = Element.ALIGN_LEFT;
                table.DefaultCell.Border = 0;
                table.SpacingBefore = 20f;
                table.SpacingAfter = 0f;

                cell = new PdfPCell();
                table.AddCell(new Phrase("Hormat Kami", normal));
                table.AddCell(new Phrase("PT BNI Life Insurance", normal));
                pdfDoc.Add(table);

                table = new PdfPTable(1);
                width = new float[] { 2f };
                table.WidthPercentage = 100;
                table.SetWidths(width);
                table.HorizontalAlignment = Element.ALIGN_LEFT;
                table.DefaultCell.Border = 0;
                table.DefaultCell.PaddingBottom = 8;
                table.SpacingBefore = 10f;
                table.SpacingAfter = 20f;

                cell = new PdfPCell();
                table.AddCell(new Phrase(" ", normal));
                pdfDoc.Add(table);

                table = new PdfPTable(2);
                width = new float[] { 2f, 2f };
                table.WidthPercentage = 100;
                table.SetWidths(width);
                table.HorizontalAlignment = Element.ALIGN_LEFT;
                table.DefaultCell.Border = 0;
                table.DefaultCell.PaddingBottom = 8;
                table.SpacingBefore = 10f;
                table.SpacingAfter = 20f;

                if (letterDetail.letter.letterNumber != "NO_LETTER")
                {
                    QRCodeModel myQRCode = new QRCodeModel();
                    myQRCode.QRCodeText = letterDetail.letter.letterNumber;
                    using (QRCodeGenerator qrGenerator = new QRCodeGenerator())
                    using (QRCodeData qRCodeData = qrGenerator.CreateQrCode(
                        myQRCode.QRCodeText, QRCodeGenerator.ECCLevel.Q))
                    using (QRCode qRCode = new QRCode(qRCodeData))
                    {
                        Bitmap qrCodeImage = qRCode.GetGraphic(3);
                        byte[] BitmapArray = qrCodeImage.ConvertBitmapToByteArray();
                        string urlImgQrcode = string.Format("data:image/png;base64,{0}", Convert.ToBase64String(BitmapArray));
                        ViewBag.QRCode = urlImgQrcode;
                    }

                    if (letterDetail.letter.signatureType == 1)
                    {
                        string filename = "ttdexample";
                        string filePath = Path.Combine("wwwroot\\uploads\\ttdexample\\" + filename + ".png");
                        string filettd = Path.GetFileName(filePath);
                        string pathttd = $"{this.Request.Scheme}://{this.Request.Host}{this.Request.PathBase}" + "/uploads/ttdexample/" + filettd;

                        cell.Border = 0;
                        Image imagettd = Image.GetInstance(pathttd);
                        imagettd.ScaleToFit(5F, 10F);//Set width and height in float   
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.AddElement(imagettd);
                        table.AddCell(imagettd);
                        //pdfDoc.Add(imagettd);

                        //string filenameqrcode = "barcodeexample";
                        //string filePathqrcode = Path.Combine("wwwroot\\uploads\\ttdexample\\" + filenameqrcode + ".png");
                        //string filettdqrcode = Path.GetFileName(filePathqrcode);
                        //string pathttdqrcode = $"{this.Request.Scheme}://{this.Request.Host}{this.Request.PathBase}" + "/uploads/ttdexample/" + filettdqrcode;
                        //cell.Border = 0;
                        //Image imagettdqrcode = Image.GetInstance(pathttdqrcode);
                        ////image.ScaleToFit(550F, 200F);//Set width and height in float   
                        //cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        //cell.AddElement(imagettdqrcode);
                        //table.AddCell(imagettdqrcode);

                        //sb = new StringBuilder();
                        //sb.Append("<img src=" + sb + ">");
                        //sr = new StringReader(sb.ToString());
                        //htmlparser.Parse(sr);

                        //table.AddCell(imagettdqrcode);

                        //pdfDoc.Add(imagettdqrcode);
                        pdfDoc.Add(table);
                    }
                    else if (letterDetail.letter.signatureType == 2)
                    {
                        Regex regex = new Regex(@"^data:image/(?<mediaType>[^;]+);base64,(?<data>.*)");
                        Match match = regex.Match(ViewBag.QRCode);
                        Image imagettdqrcode = Image.GetInstance(
                            Convert.FromBase64String(match.Groups["data"].Value)
                        );
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.AddElement(imagettdqrcode);
                        table.AddCell(imagettdqrcode);
                        pdfDoc.Add(table);
                    }
                    else if (letterDetail.letter.signatureType == 3)
                    {


                        string filename = "ttdexample";
                        string filePath = Path.Combine("wwwroot\\uploads\\ttdexample\\" + filename + ".png");
                        string filettd = Path.GetFileName(filePath);
                        string pathttd = $"{this.Request.Scheme}://{this.Request.Host}{this.Request.PathBase}" + "/uploads/ttdexample/" + filettd;
                        cell.Border = 0;
                        Image imagettd = Image.GetInstance(pathttd);
                        imagettd.ScaleToFit(5F, 10F);//Set width and height in float   
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.AddElement(imagettd);
                        table.AddCell(imagettd);

                        Regex regex = new Regex(@"^data:image/(?<mediaType>[^;]+);base64,(?<data>.*)");
                        Match match = regex.Match(ViewBag.QRCode);
                        Image imagettdqrcode = Image.GetInstance(
                            Convert.FromBase64String(match.Groups["data"].Value)
                        );
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.AddElement(imagettdqrcode);
                        table.AddCell(imagettdqrcode);
                        pdfDoc.Add(table);
                    }
                }

                table = new PdfPTable(1);
                width = new float[] { 2f };
                table.WidthPercentage = 100;
                table.SetWidths(width);
                table.HorizontalAlignment = Element.ALIGN_LEFT;
                table.DefaultCell.Border = 0;
                table.SpacingBefore = 10f;
                table.SpacingAfter = 20f;

                cell = new PdfPCell();
                table.AddCell(new Phrase(letterDetail.sender[0].fullname, underlineBold));
                table.AddCell(new Phrase(letterDetail.sender[0].positionName, bold));
                pdfDoc.Add(table);


                pdfDoc.Close();

                byte[] bytess = ms.ToArray();
                byte[] bytes = AddPageNumbers(bytess);
                ms.Close();

                return File(ms.ToArray(), "application/pdf", fileName);
            }
        }


        public static byte[] AddPageNumbers(byte[] pdf)
        {
            MemoryStream ms = new MemoryStream();
            // we create a reader for a certain document
            PdfReader reader = new PdfReader(pdf);
            // we retrieve the total number of pages
            int n = reader.NumberOfPages;
            // we retrieve the size of the first page
            Rectangle psize = reader.GetPageSize(1);

            // step 1: creation of a document-object
            Document document = new Document(psize, 50, 50, 50, 50);
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
                cb.BeginText();
                cb.SetFontAndSize(bf, 10);
                cb.ShowTextAligned(PdfContentByte.ALIGN_RIGHT, "Page " + p + " Of " + n, 570, 10, 0);
                cb.EndText();
            }
            // step 5: we close the document
            document.Close();
            return ms.ToArray();
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
                    var titlePopup = "Memo Berhasil Disetujui";
                    var pesanPopup = "Memo Berhasil Disetujui";
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
        public async Task<string> GetNomimalMaxMin(Guid idPengadaan)
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



        #region BackDate

        public async Task<IActionResult> MemoBackDate()
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

        [Route("BackDate/DetailMemoBackdate/{id}/{letterType}")]
        public async Task<IActionResult> DetailMemoBackdate(Guid id, string letterType)
        {
            OutputDetailMemoBackDate letterDetail = new OutputDetailMemoBackDate();
            GeneralOutputModel generalOutput = new GeneralOutputModel();
            List<OutputContentTemplate> output = new List<OutputContentTemplate>();
            List<StringmapMemoOutput> jenisMemo = new List<StringmapMemoOutput>();
            List<MemotypeOuput> tipeMemo = new List<MemotypeOuput>();
            var token = HttpContext.Session.GetString("token");
            ParamGetDetailMemo pr = new ParamGetDetailMemo();
            pr.idLetter = id;
            pr.lettertype=letterType;
            generalOutput = await _dataAccessProvider.GetDetailMemoBackDate("Memo/GetDetailMemoBackDateWeb/", token, pr);
            var jsonApiResponseSerialize = JsonConvert.SerializeObject(generalOutput.Result);
            letterDetail = JsonConvert.DeserializeObject<OutputDetailMemoBackDate>(jsonApiResponseSerialize);

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

    }


  


}
