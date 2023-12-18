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
using Newtonsoft.Json.Linq;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.InkML;
using System.Text;
using DocumentFormat.OpenXml.Wordprocessing;
using System.Web.WebPages;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Localization;
using System.Globalization;

namespace EofficeBNILWEB.Controllers
{
    [Authorize]
    public class DocumentController : Controller
    {
        private readonly IConfiguration _config;
        private readonly IDataAccessProvider _dataAccessProvider;
        private readonly IHostingEnvironment _environment;
        private readonly string urlApi;
        private readonly IHtmlLocalizer<HomeController> _localiza;
        public DocumentController(IConfiguration config, IDataAccessProvider dataAccessProvider, IHostingEnvironment Environment, IHtmlLocalizer<HomeController> localiza)
        {
            _config = config;
            _dataAccessProvider = dataAccessProvider;
            _environment = Environment;
            urlApi = _config.GetValue<string>("AppSettings:apiUrl");
            _localiza = localiza;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult DocIn() 
        {
            return View();
        }
        public IActionResult DocOut()
        {
            return View();
        }
        public async Task<IActionResult> Create() 
        {
            List<AdminDivisionOutput> divisionList = new List<AdminDivisionOutput>();
            GeneralOutputModel generalOutput = new GeneralOutputModel();
            var token = HttpContext.Session.GetString("token");
            generalOutput = await _dataAccessProvider.GetUnitAsync("Unit/GetAdminDivision/", token);
            var jsonApiResponseSerialize = JsonConvert.SerializeObject(generalOutput.Result);
            divisionList = JsonConvert.DeserializeObject<List<AdminDivisionOutput>>(jsonApiResponseSerialize);

            if (generalOutput.Status == "UA")
            {
                return RedirectToAction("Logout", "Home");
            }

            List<StringmapOutput> documentTypeList = new List<StringmapOutput>();
            ParamGetStringmap pr = new ParamGetStringmap();
            pr.objectName = "tr_document";
            pr.attributeName = "DOCUMENT_TYPE";

            generalOutput = await _dataAccessProvider.GetStringmapAsync("General/GetStringmap/", pr, token);
            jsonApiResponseSerialize = JsonConvert.SerializeObject(generalOutput.Result);
            documentTypeList = JsonConvert.DeserializeObject<List<StringmapOutput>>(jsonApiResponseSerialize);

            if (generalOutput.Status == "UA")
            {
                return RedirectToAction("Logout", "Home");
            }
            ViewBag.Output = null;
            ViewBag.UlTabActiveHome = "active";
            ViewBag.DivTabActiveHome = "active show";
            ViewBag.UlTabActiveUpload = "";
            ViewBag.DivTabActiveUpload = "fade";

            if (TempData["OutputApi"] != null)
            {
                var OutputUploadDocument = JsonConvert.DeserializeObject<List<UploadDocumentOutput>>((string)TempData["OutputApi"]);

                ViewBag.Output = OutputUploadDocument;

                ViewBag.UlTabActiveHome = "";
                ViewBag.DivTabActiveHome = "fade";
                ViewBag.UlTabActiveUpload = "active";
                ViewBag.DivTabActiveUpload = "active show";
            }
            

            //List<UploadDocumentOutput> OutputUploadDocument = (List<UploadDocumentOutput>)TempData["OutputApi"];

            ViewBag.Divison = divisionList;
            ViewBag.DocumentType = documentTypeList;
            return View();
        }
        public async Task<IActionResult> Detail(Guid id)
        {
            List<AdminDivisionOutput> divisionList = new List<AdminDivisionOutput>();
            DocumentOutput documentOutput = new DocumentOutput();
            GeneralOutputModel generalOutput = new GeneralOutputModel();
            var token = HttpContext.Session.GetString("token");
            generalOutput = await _dataAccessProvider.GetUnitAsync("Unit/GetAdminDivision/", token);
            var jsonApiResponseSerialize = JsonConvert.SerializeObject(generalOutput.Result);
            divisionList = JsonConvert.DeserializeObject<List<AdminDivisionOutput>>(jsonApiResponseSerialize);

            if (generalOutput.Status == "UA")
            {
                return RedirectToAction("Logout", "Home");
            }

            List<StringmapOutput> documentTypeList = new List<StringmapOutput>();
            ParamGetStringmap pr = new ParamGetStringmap();
            pr.objectName = "tr_document";
            pr.attributeName = "DOCUMENT_TYPE";

            generalOutput = await _dataAccessProvider.GetStringmapAsync("General/GetStringmap/", pr, token);
            jsonApiResponseSerialize = JsonConvert.SerializeObject(generalOutput.Result);
            documentTypeList = JsonConvert.DeserializeObject<List<StringmapOutput>>(jsonApiResponseSerialize);

            if (generalOutput.Status == "UA")
            {
                return RedirectToAction("Logout", "Home");
            }

            ParamGetDetailDocument prGetDetail = new ParamGetDetailDocument();
            prGetDetail.idDocument = id;
            generalOutput = await _dataAccessProvider.GetDetailDocumentAsync("Document/GetDetailDocument", token, prGetDetail);
            jsonApiResponseSerialize = JsonConvert.SerializeObject(generalOutput.Result);
            documentOutput = JsonConvert.DeserializeObject<DocumentOutput>(jsonApiResponseSerialize);

            if (generalOutput.Status == "UA")
            {
                return RedirectToAction("Logout", "Home");
            }
            var editableDoc = 0;
            var idUnitLogin = HttpContext.Session.GetString("idUnit");
            if (idUnitLogin == "D0D02AA8-7A27-ED11-89B0-3CF011A1A981" && (documentOutput.statusCode == 1))
            {
                editableDoc = 1;
            }

            ViewBag.Divison = divisionList;
            ViewBag.DocumentType = documentTypeList;
            ViewBag.EditableText = editableDoc == 1 ? "" : "readonly=1 style=background-color:#e6e9ec !important";
            ViewBag.IdDate = editableDoc == 1 ? "kt_datetimepicker_2" : "iddate";
            ViewBag.EditableDropdown = editableDoc == 1 ? "select2stylecss" : "";
            ViewBag.EditableNum = editableDoc;


            return View(documentOutput);
        }
        public async Task<IActionResult> ReceiveDocIn(Guid id)
        {
            List<AdminDivisionOutput> divisionList = new List<AdminDivisionOutput>();
            DocumentOutput documentOutput = new DocumentOutput();
            GeneralOutputModel generalOutput = new GeneralOutputModel();
            var token = HttpContext.Session.GetString("token");
            generalOutput = await _dataAccessProvider.GetUnitAsync("Unit/GetAdminDivision/", token);
            var jsonApiResponseSerialize = JsonConvert.SerializeObject(generalOutput.Result);
            divisionList = JsonConvert.DeserializeObject<List<AdminDivisionOutput>>(jsonApiResponseSerialize);

            if (generalOutput.Status == "UA")
            {
                return RedirectToAction("Logout", "Home");
            }

            List<StringmapOutput> documentTypeList = new List<StringmapOutput>();
            ParamGetStringmap pr = new ParamGetStringmap();
            pr.objectName = "tr_document";
            pr.attributeName = "DOCUMENT_TYPE";

            generalOutput = await _dataAccessProvider.GetStringmapAsync("General/GetStringmap/", pr, token);
            jsonApiResponseSerialize = JsonConvert.SerializeObject(generalOutput.Result);
            documentTypeList = JsonConvert.DeserializeObject<List<StringmapOutput>>(jsonApiResponseSerialize);

            if (generalOutput.Status == "UA")
            {
                return RedirectToAction("Logout", "Home");
            }

            ParamGetDetailDocument prGetDetail = new ParamGetDetailDocument();
            prGetDetail.idDocument = id;
            generalOutput = await _dataAccessProvider.GetDetailDocumentAsync("Document/GetDetailDocument", token, prGetDetail);
            jsonApiResponseSerialize = JsonConvert.SerializeObject(generalOutput.Result);
            documentOutput = JsonConvert.DeserializeObject<DocumentOutput>(jsonApiResponseSerialize);

            if (generalOutput.Status == "UA")
            {
                return RedirectToAction("Logout", "Home");
            }
            var editableDoc = 0;
            var idUnitLogin = HttpContext.Session.GetString("idUnit");
            if (idUnitLogin == "D0D02AA8-7A27-ED11-89B0-3CF011A1A981" && (documentOutput.statusCode == 1 || documentOutput.statusCode == 4))
            {
                editableDoc = 1;
            }

            ViewBag.Divison = divisionList;
            ViewBag.DocumentType = documentTypeList;
            ViewBag.EditableText = editableDoc == 1 ? "" : "readonly=1 style=background-color:#e6e9ec !important";
            ViewBag.EditableTextTotal = documentOutput.qtyTotal == 0 ? "" : "readonly=1 style=background-color:#e6e9ec !important";
            ViewBag.IdDate = editableDoc == 1 ? "kt_datetimepicker_2" : "iddate";
            ViewBag.EditableDropdown = editableDoc == 1 ? "select2stylecss" : "";


            return View(documentOutput);
        }
        public async Task<IActionResult> DocInDistribution(Guid id)
        {
            List<AdminDivisionOutput> divisionList = new List<AdminDivisionOutput>();
            DocumentDistributionOutput documentOutput = new DocumentDistributionOutput();
            GeneralOutputModel generalOutput = new GeneralOutputModel();

            var token = HttpContext.Session.GetString("token");
            ParamGetDetailDocumentReceiver prGetDetail = new ParamGetDetailDocumentReceiver();
            prGetDetail.idDocumentReceiver = id;

            generalOutput = await _dataAccessProvider.GetDetailDocumentReceiverAsync("Document/GetDetailDocumentDistribution", token, prGetDetail);
            var jsonApiResponseSerialize = JsonConvert.SerializeObject(generalOutput.Result);
            documentOutput = JsonConvert.DeserializeObject<DocumentDistributionOutput>(jsonApiResponseSerialize);

            if (generalOutput.Status == "UA")
            {
                TempData["status"] = "NG";
                TempData["pesan"] = "Session habis silahkan login kembali";
                return RedirectToAction("Logout", "Home");
            }
            ParamPreviewNoLetter prPreview = new ParamPreviewNoLetter();
            prPreview.letterType = "SM";
            generalOutput = await _dataAccessProvider.PreviewNoLetter("Letter/PreviewNoDoc", token, prPreview);
            jsonApiResponseSerialize = JsonConvert.SerializeObject(generalOutput.Result);
            string NoLetter = JsonConvert.DeserializeObject<string>(jsonApiResponseSerialize);

            if (generalOutput.Status == "UA")
            {
                TempData["status"] = "NG";
                TempData["pesan"] = "Session habis silahkan login kembali";
                return RedirectToAction("Logout", "Home");
            }

            ViewBag.NoLetter = NoLetter;
            return View(documentOutput);
        }
        public IActionResult DocumentReport()
        {
            return View();
        }
        public FileResult DownloadTemplate()
        {
            var fileName = "TemplateUploadDocument.xlsx";
            //Build the File Path.
            string path = Path.Combine(_environment.WebRootPath, "download/") + fileName;

            //Read the File data into Byte Array.
            byte[] bytes = System.IO.File.ReadAllBytes(path);

            //Send the File to Download.
            return File(bytes, "application/octet-stream", fileName);
        }
        public async Task<IActionResult> NewArchive()
        {
            GeneralOutputModel generalOutput = new GeneralOutputModel();
            var token = HttpContext.Session.GetString("token");
            ParamPreviewNoLetter prPreview = new ParamPreviewNoLetter();
            prPreview.letterType = "SM";
            generalOutput = await _dataAccessProvider.PreviewNoLetter("Letter/PreviewNoDoc", token, prPreview);
            var jsonApiResponseSerialize = JsonConvert.SerializeObject(generalOutput.Result);
            string NoLetter = JsonConvert.DeserializeObject<string>(jsonApiResponseSerialize);

            if (generalOutput.Status == "UA")
            {
                TempData["status"] = "NG";
                TempData["pesan"] = "Session habis silahkan login kembali";
                return RedirectToAction("Logout", "Home");
            }

            ViewBag.NoLetter = NoLetter;
            return View();
        }

        [HttpPost]
        public async Task<string> GetDocumentList()
        {
            DocumentOutputWeb documentOutput = new DocumentOutputWeb();
            GeneralOutputModel generalOutput = new GeneralOutputModel();
            List<Dictionary<String, String>> _list = new List<Dictionary<string, string>>();
            var token = HttpContext.Session.GetString("token");
            var draw = Request.Form["draw"].FirstOrDefault();
            var sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
            var sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault();
            var searchValue = Request.Form["search[value]"].FirstOrDefault();
            int pageSize = Convert.ToInt32(Request.Form["length"].FirstOrDefault() ?? "0");
            int skip = Convert.ToInt32(Request.Form["start"].FirstOrDefault() ?? "0");

            ParamGetDocumentWeb pr = new ParamGetDocumentWeb();
            pr.draw = draw;
            pr.sortColumn = sortColumn;
            pr.sortColumnDirection = sortColumnDirection;
            pr.searchValue = searchValue;
            pr.pageSize = pageSize;
            pr.start = skip;

            generalOutput = await _dataAccessProvider.GetDocumentAsync("Document/GetDocument/", token, pr);
            var jsonApiResponseSerialize = JsonConvert.SerializeObject(generalOutput.Result);
            documentOutput = JsonConvert.DeserializeObject<DocumentOutputWeb>(jsonApiResponseSerialize);

            return JsonConvert.SerializeObject(documentOutput);
        }

        [HttpPost]
        public async Task<string> GetDocumentByTrackingNumber(string trackingNumber)
        {
            DocumentOutputWeb documentOutput = new DocumentOutputWeb();
            GeneralOutputModel generalOutput = new GeneralOutputModel();
            var token = HttpContext.Session.GetString("token");

            ParamCheckTrackingNumber pr = new ParamCheckTrackingNumber();
            pr.trackingNumber = trackingNumber;

            generalOutput = await _dataAccessProvider.GetDocumentByTrackingNumberAsync("Document/GetDocumentByTrackingNumber/", token, pr);
            var jsonApiResponseSerialize = JsonConvert.SerializeObject(generalOutput);
            //generalOutput = JsonConvert.DeserializeObject<GeneralOutputModel>(jsonApiResponseSerialize);

            return jsonApiResponseSerialize;
        }

        [HttpPost]
        public async Task<IActionResult> Create(ParamInsertDocument pr)
        {
            try 
            {
                var token = HttpContext.Session.GetString("token");
                GeneralOutputModel generalOutput = new GeneralOutputModel();
                generalOutput = await _dataAccessProvider.PostInsertDocumentAsync("Document/InsertDocument", pr, token);
                var jsonApiResponseSerialize = JsonConvert.SerializeObject(generalOutput.Result);

                if (generalOutput.Status == "OK")
                {
                    TempData["status"] = "OK";
                    TempData["title"] = "Berhasil menambahkan dokumen masuk";
                    TempData["pesan"] = generalOutput.Message;
                }
                else if (generalOutput.Status == "UA")
                {
                    return RedirectToAction("Logout", "Home");
                }
                else
                {
                    TempData["status"] = "NG";
                    TempData["title"] = "Gagal menambahkan dokumen masuk";
                    TempData["pesan"] = generalOutput.Message;
                }
                

                return Redirect("~/Document/DocIn");
            }
            catch (Exception ex)
            {
                TempData["status"] = "NG";
                TempData["title"] = "Gagal menambahkan dokumen masuk";
                TempData["pesan"] = ex.ToString();
                return Redirect("~/Document/DocIn");
            }
        }

        [HttpPost]
        public async Task<IActionResult> UploadDocument(IFormFile inputfile)
        {
            try
            {
                var token = HttpContext.Session.GetString("token");
                GeneralOutputModel generalOutput = new GeneralOutputModel();
                List<ParamUploadDocument> data = new List<ParamUploadDocument>();
                ParamUploadDocumentString pr = new ParamUploadDocumentString();
                List<UploadDocumentOutput> responseApi = new List<UploadDocumentOutput>();
                ViewBag.Output = null;
                if ((inputfile != null) && (inputfile.Length > 0))
                {
                    //Create a Folder.
                    string path = Path.Combine(_environment.WebRootPath, "uploads");
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }
                    //Save the uploaded Excel file.
                    string fileName = Path.GetFileName(inputfile.FileName);
                    string filePath = Path.Combine(path, fileName);
                    using (FileStream stream = new FileStream(filePath, FileMode.Create))
                    {
                        inputfile.CopyTo(stream);
                    }
                    //Read the connection string for the Excel file.
                    string conString = _config.GetValue<string>("AppSettings:ExcelConString");
                    DataTable dataUpload = new DataTable();
                    conString = string.Format(conString, filePath);

                    using (OleDbConnection connExcel = new OleDbConnection(conString))
                    {
                        using (OleDbCommand cmdExcel = new OleDbCommand())
                        {
                            using (OleDbDataAdapter odaExcel = new OleDbDataAdapter())
                            {
                                cmdExcel.Connection = connExcel;

                                //Get the name of First Sheet.
                                connExcel.Open();
                                DataTable dtExcelSchema;
                                dtExcelSchema = connExcel.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                                string sheetName = dtExcelSchema.Rows[0]["TABLE_NAME"].ToString();
                                connExcel.Close();
                                //string sheetName = "Sheet1$";

                                //Read Data from First Sheet.
                                connExcel.Open();
                                cmdExcel.CommandText = "SELECT * From [" + sheetName + "]";
                                odaExcel.SelectCommand = cmdExcel;
                                odaExcel.Fill(dataUpload);
                                connExcel.Close();
                            }
                        }
                    }
                    foreach (DataRow row in dataUpload.Rows)
                    {
                        string documentType = row["JENIS"].ToString();

                        if (documentType == null || documentType == "")
                        {
                            break;
                        }

                        string trackingNumber = row["NOMOR RESI / AWB"].ToString();
                        string penerimaDoc = row["NAMA PENERIMA"].ToString();
                        string receiverUnit = row["DIVISI PENERIMA"].ToString();
                        string sender = row["PENGIRIM"].ToString();
                        data.Add(new ParamUploadDocument
                        {
                            documentType = documentType,
                            trackingNumber = trackingNumber,
                            receiverUnit = receiverUnit,
                            senderName = sender,
                            docReceiver = penerimaDoc
                        });

                    }
                    pr.jsonDataString = data;
                    generalOutput = await _dataAccessProvider.PostUploadDocumentAsync("Document/UploadDocument", token, pr);
                    var jsonApiResponseSerialize = JsonConvert.SerializeObject(generalOutput.Result);
                    responseApi = JsonConvert.DeserializeObject<List<UploadDocumentOutput>>(jsonApiResponseSerialize);

                    if (generalOutput.Status == "OK")
                    {
                        TempData["status"] = "OK";
                        TempData["title"] = "Info";
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
                    TempData["OutputApi"] = jsonApiResponseSerialize;

                    return Redirect("~/Document/Create");
                }

                TempData["status"] = "NG";
                TempData["pesan"] = "Siliahkan pilih file yang akan di upload";

                return Redirect("~/Document/Create");
            }
            catch (Exception ex)
            {
                TempData["status"] = "NG";
                TempData["title"] = "Gagal menambahkan dokumen masuk";
                TempData["pesan"] = ex.ToString();
                return Redirect("~/Document/DocIn");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Detail(ParamUpdateDocumentWeb pr)
        {
            try
            {
                var token = HttpContext.Session.GetString("token");
                GeneralOutputModel generalOutput = new GeneralOutputModel();
                ParamUpdateDocument prApi = new ParamUpdateDocument();
                List<ParamUpdateReceiver> prApiReceiver = new List<ParamUpdateReceiver>();
                prApi.idDocument = pr.idDocument;
                prApi.documentType = pr.documentType;
                prApi.trackingNumber = pr.trackingNumber;
                prApi.senderName = pr.senderName;
                prApi.docReceiver = pr.docReceiver;
                //prApi.receivedDate = pr.receivedDate;
                prApi.distributionTime = pr.distributionTime;
                prApi.comment = "Dokumen diperbarui";
                if (pr.idDocumentReceiver.Length > 0)
                {
                    for (int a = 0; a < pr.idDocumentReceiver.Length; a++)
                    {
                        Guid idDocumentReceiver = new Guid(pr.idDocumentReceiver.GetValue(a).ToString());
                        Guid idUnit = new Guid(pr.idUnit.GetValue(a).ToString());
                        Guid idUserTu = new Guid(pr.idUserTu.GetValue(a).ToString());
                        string returnNumber = pr.returnNumber.GetValue(a) == null ? null : pr.returnNumber.GetValue(a).ToString();

                        prApiReceiver.Add(new ParamUpdateReceiver
                        {
                            idDocumentReceiver = idDocumentReceiver,
                            idUnit = idUnit,
                            idUserTu = idUserTu,
                            returnNumber = returnNumber
                        });
                    }
                }
                if (pr.idUnitNew != null) { 
                    if (pr.idUnitNew.Length > 0)
                    {
                        for (int a = 0; a < pr.idUnitNew.Length; a++)
                        {
                            Guid idUnit = new Guid(pr.idUnitNew.GetValue(a).ToString());
                            Guid idUserTu = new Guid(pr.idUserTuNew.GetValue(a).ToString());
                            string returnNumber = pr.returnNumberNew.GetValue(a).ToString();

                            //DataOuputValidasiBarcode dvbr = new DataOuputValidasiBarcode();
                            //dvbr.type_brcd = 2;
                            //dvbr.nmr_brcd = returnNumber;
                            //generalOutput = await _dataAccessProvider.PostValidasiBarcode("Document/ValidasiBarcodeMallingRoom", token, dvbr);
                            //var jsonApiResponseSerializeBarcode = JsonConvert.SerializeObject(generalOutput.Result);
                            //if (generalOutput.Status == "UA")
                            //{
                            //    TempData["status"] = "NG";
                            //    TempData["pesan"] = "Session habis silahkan login kembali";
                            //    return RedirectToAction("Logout", "Home");
                            //}
                            //else if(generalOutput.Status == "NG")
                            //{
                            //    TempData["status"] = "NG";
                            //    TempData["title"] = "Nomor return tidak ditemukan";
                            //    TempData["pesan"] = "Nomor return tidak ditemukan";
                            //    return Redirect("~/Document/DocIn");
                            //}

                            prApiReceiver.Add(new ParamUpdateReceiver
                            {
                                idDocumentReceiver = Guid.Empty,
                                idUnit = idUnit,
                                idUserTu = idUserTu,
                                returnNumber = returnNumber
                            });
                        }
                    }
                }
                prApi.receiverDocument = prApiReceiver;

                generalOutput = await _dataAccessProvider.PutUpdateDocumentAsync("Document/UpdateDocument", token, prApi);
                var jsonApiResponseSerialize = JsonConvert.SerializeObject(generalOutput.Result);

                if (generalOutput.Status == "OK")
                {
                    TempData["status"] = "OK";
                    TempData["title"] = "Berhasil merubah dokumen masuk";
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
                    TempData["title"] = "Gagal merubah dokumen masuk";
                    TempData["pesan"] = generalOutput.Message;
                }


                return Redirect("~/Document/DocIn");
            }
            catch (Exception ex)
            {
                TempData["status"] = "NG";
                TempData["title"] = "Gagal merubah dokumen masuk";
                TempData["pesan"] = ex.ToString();
                return Redirect("~/Document/DocIn");
            }
        }

        [HttpPost]
        public async Task<IActionResult> ReceiveDocIn(ParamReceiveDocumentWeb pr)
        {
            var message = "menerima";
            try
            {
                var token = HttpContext.Session.GetString("token");
                GeneralOutputModel generalOutput = new GeneralOutputModel();
                string unitName = "";
                for (var i =0;i< pr.UnitNameNew.Length;i++)
                {
                    if (unitName == "")
                    {
                        unitName = pr.UnitNameNew[i];
                    }
                    else
                    {
                        unitName += ", " + pr.UnitNameNew[i];
                    }
                }
                if (unitName != "")
                {
                    pr.comment = pr.comment + " (Milik " + unitName + ")";
                }

                ParamReceiveDocument prApi = new ParamReceiveDocument();
                prApi.idDocument = pr.idDocument;
                prApi.idDocumentReceiver = pr.idDocumentReceiver;
                prApi.qtyTotal = pr.qtyTotal;
                prApi.receivedDocument = pr.receivedDocument;
                prApi.returnedDocument = pr.returnedDocument;
                prApi.receivedDate = pr.receivedDate;
                prApi.distributionTime = pr.distributionTime;
                prApi.comment = pr.comment;
                prApi.flagReceive = pr.flagReceive;
                prApi.UnitNameNew = unitName;

                generalOutput = await _dataAccessProvider.PutReceiveDocumentAsync("Document/ReceiveDocument", token, prApi);
                var jsonApiResponseSerialize = JsonConvert.SerializeObject(generalOutput.Result);
                if (pr.flagReceive == 3)
                {
                    message = "menerima sebagian";
                }else if (pr.flagReceive == 4)
                {
                    message = "menolak";
                }
                if (generalOutput.Status == "OK")
                {
                    TempData["status"] = "OK";
                    TempData["title"] = "Berhasil "+message+" dokumen masuk";
                    TempData["pesan"] = generalOutput.Message;
                }
                else if (generalOutput.Status == "UA")
                {
                    return RedirectToAction("Logout", "Home");
                }
                else
                {
                    TempData["status"] = "NG";
                    TempData["title"] = "Gagal "+message+" dokumen masuk";
                    TempData["pesan"] = generalOutput.Message;
                }


                return Redirect("~/Document/DocIn");
            }
            catch (Exception ex)
            {
                TempData["status"] = "NG";
                TempData["title"] = "Gagal "+message+" dokumen masuk";
                TempData["pesan"] = ex.ToString();
                return Redirect("~/Document/DocIn");
            }
        }

        [HttpPost]
        public async Task<string> GetUserReceiver(string keyword)
        {
            DocumentOutputWeb documentOutput = new DocumentOutputWeb();
            GeneralOutputModel generalOutput = new GeneralOutputModel();
            List<AdminDivisionOutput> receiverList = new List<AdminDivisionOutput>();
            var token = HttpContext.Session.GetString("token");
            Guid idUnit = new Guid(HttpContext.Session.GetString("idUnit"));
            Guid parentIdPosition = new Guid(HttpContext.Session.GetString("parentIdPosition"));
            Guid directorIdUnit = new Guid(HttpContext.Session.GetString("directorIdUnit"));

            ParamGetPenerima pr = new ParamGetPenerima();
            pr.keyword = keyword;

            generalOutput = await _dataAccessProvider.GetDataPenerima("General/GetDataPenerima/", token, pr);
            var jsonApiResponseSerialize = JsonConvert.SerializeObject(generalOutput.Result);
            receiverList = JsonConvert.DeserializeObject<List<AdminDivisionOutput>>(jsonApiResponseSerialize).Where(p => p.idUnit == idUnit || p.idPosition == parentIdPosition || p.idUnit == directorIdUnit).DistinctBy(p=>p.idUser).ToList();
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
            receiverList = JsonConvert.DeserializeObject<List<AdminDivisionOutput>>(jsonApiResponseSerialize).DistinctBy(p => p.idUser).ToList();
            jsonApiResponseSerialize = JsonConvert.SerializeObject(receiverList);

            return jsonApiResponseSerialize;
        }

        [HttpPost]
        public async Task<string> SearchDocReport(string? trackingNumber,DateTime? startDate,DateTime? endDate)
        {
            List<DocumentOutput> documentOutput = new List<DocumentOutput>();
            GeneralOutputModel generalOutput = new GeneralOutputModel();
            List<Dictionary<String, String>> _list = new List<Dictionary<string, string>>();
            var token = HttpContext.Session.GetString("token");

            ParamGetDetailReportDocument pr = new ParamGetDetailReportDocument();
            pr.trackingNumber = trackingNumber;
            pr.startDate = startDate;
            pr.endDate = endDate;
            generalOutput = await _dataAccessProvider.GetDocumentReport("Document/GetReportDocument", token, pr);
            var jsonApiResponseSerialize = JsonConvert.SerializeObject(generalOutput.Result);
            documentOutput = JsonConvert.DeserializeObject<List<DocumentOutput>>(jsonApiResponseSerialize);
            foreach (var item in documentOutput)
            {
                foreach (var item2 in item.documentReceiver)
                {
                    Dictionary<String, String> dict = new Dictionary<string, string>();
                    dict.Add("trackingNumber", item.trackingNumber);
                    dict.Add("returnNumber", item2.returnNumber == null ? "" : item2.returnNumber);
                    dict.Add("receivedDate", item2.receivedDate == null ? "" : Convert.ToDateTime(item2.receivedDate).ToString("dd MMM yyyy"));
                    dict.Add("senderName", item.senderName);
                    dict.Add("unitName", item2.unitName);
                    dict.Add("tuName", item2.tuName);
                    dict.Add("status", item2.statusCodeValue);

                    _list.Add(dict);
                }
            }

            return JsonConvert.SerializeObject(_list);
        }

        [HttpPost]
        public async Task<string> ReceiveCheckedDoc(string listIdDoc)
        {
            GeneralOutputModel generalOutput = new GeneralOutputModel();
            List<AdminDivisionOutput> receiverList = new List<AdminDivisionOutput>();
            var token = HttpContext.Session.GetString("token");
            Guid idUnit = new Guid(HttpContext.Session.GetString("idUnit"));

            ParamReceiveCheckedDoc pr = new ParamReceiveCheckedDoc();
            pr.listIdDoc = listIdDoc;

            generalOutput = await _dataAccessProvider.ReceiveCheckedDoc("Document/ReceiveCheckedDoc/", token, pr);
            var jsonApiResponseSerialize = JsonConvert.SerializeObject(generalOutput);
            //receiverList = JsonConvert.DeserializeObject<GeneralOutputModel>(jsonApiResponseSerialize);
            //jsonApiResponseSerialize = JsonConvert.SerializeObject(receiverList);

            return jsonApiResponseSerialize;
        }

        public IActionResult SearchOutgoingMail()
        {
            return View();
        }

        [HttpPost]
        public async Task<string> SearchDocOutgoingMail(string? trackingNumber, DateTime? startDate, DateTime? endDate)
        {
            List<DocumentOutput> documentOutput = new List<DocumentOutput>();
            GeneralOutputModel generalOutput = new GeneralOutputModel();
            List<Dictionary<String, String>> _list = new List<Dictionary<string, string>>();
            var token = HttpContext.Session.GetString("token");

            ParamGetDetailSearchoutgoingDocument pr = new ParamGetDetailSearchoutgoingDocument();
            pr.trackingNumber = trackingNumber;
            pr.startDate = startDate;
            pr.endDate = endDate;
            generalOutput = await _dataAccessProvider.GetDocumentSearchOutgongMail("Document/GetSearchReportDocument", token, pr);
            var jsonApiResponseSerialize = JsonConvert.SerializeObject(generalOutput.Result);
            documentOutput = JsonConvert.DeserializeObject<List<DocumentOutput>>(jsonApiResponseSerialize);
            int no = 1;
            foreach (var item in documentOutput)
            {
                foreach (var item2 in item.documentReceiver)
                {

                    Dictionary<String, String> dict = new Dictionary<string, string>();
                    dict.Add("no", no.ToString());
                    dict.Add("trackingNumber", item.trackingNumber);
                    dict.Add("returnNumber", item2.returnNumber == null ? "" : item2.returnNumber);
                    dict.Add("receivedDate", item2.receivedDate == null ? "" : Convert.ToDateTime(item2.receivedDate).ToString("dd MMM yyyy"));
                    dict.Add("senderName", item.senderName);
                    dict.Add("unitName", item2.unitName);
                    dict.Add("tuName", item2.tuName);
                    dict.Add("mofied_by", item2.mofied_by);
                    dict.Add("status", item2.statusCodeValue);

                    _list.Add(dict);
                    no++;
                }
            }
            
            return JsonConvert.SerializeObject(_list);
        }

        
        public async Task<IActionResult> ExportExcel(ParamGetDetailSearchoutgoingDocument pr)
        {
            using (var workbook= new XLWorkbook())
            {

                List<DocumentOutput> documentOutput = new List<DocumentOutput>();
                GeneralOutputModel generalOutput = new GeneralOutputModel();
                var token = HttpContext.Session.GetString("token");
                generalOutput = await _dataAccessProvider.GetDocumentSearchOutgongMail("Document/GetSearchReportDocument", token, pr);
                var jsonApiResponseSerialize = JsonConvert.SerializeObject(generalOutput.Result);
                documentOutput = JsonConvert.DeserializeObject<List<DocumentOutput>>(jsonApiResponseSerialize);
                var worksheet = workbook.Worksheets.Add("data");
                var curerentRow = 1;
                var no = 1;
                worksheet.Cell(curerentRow, 1).Value = "No";
                worksheet.Cell(curerentRow, 2).Value = "Tracking Number";
                worksheet.Cell(curerentRow, 3).Value = "Return Number";
                worksheet.Cell(curerentRow, 4).Value = "Received Date";
                worksheet.Cell(curerentRow, 5).Value = "Sender";
                worksheet.Cell(curerentRow, 6).Value = "Division";
                worksheet.Cell(curerentRow, 7).Value = "Receiver";
                worksheet.Cell(curerentRow, 8).Value = "Create By";
                worksheet.Cell(curerentRow, 9).Value = "Status";
                foreach (var item in documentOutput)
                {
                    foreach (var item2 in item.documentReceiver)
                    {
                        curerentRow++;
                        worksheet.Cell(curerentRow, 1).Value = no.ToString();
                        worksheet.Cell(curerentRow, 2).Value= item.trackingNumber.ToString();
                        worksheet.Cell(curerentRow, 3).Value = item2.returnNumber == null ? "" : item2.returnNumber.ToString();
                        worksheet.Cell(curerentRow, 4).Value = item2.receivedDate == null ? "" : Convert.ToDateTime(item2.receivedDate).ToString("dd MMM yyyy");
                        worksheet.Cell(curerentRow, 5).Value = item.senderName.ToString();
                        worksheet.Cell(curerentRow, 6).Value = item2.unitName.ToString();
                        worksheet.Cell(curerentRow, 7).Value = item2.tuName.ToString();
                        worksheet.Cell(curerentRow, 8).Value = item2.mofied_by.ToString();
                        worksheet.Cell(curerentRow, 9).Value = item2.statusCodeValue.ToString();
                        no++;
                    }
                }
                    using (var stream = new MemoryStream())
                    {
                        workbook.SaveAs(stream);
                        var content = stream.ToArray();
                        return File(content,"application/vnd.openxmlformats-officedocument.spreadsheethtml.sheet","data.xlsx");
                    }
                
            }
            return View();
        }


        [HttpPost]
        public IActionResult Translate(string lengueage, string urlrtn)
        {
            Response.Cookies.Append(CookieRequestCultureProvider.DefaultCookieName, CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(lengueage)),
            new CookieOptions { Expires = DateTimeOffset.Now.AddDays(15) });
            //return RedirectToAction(nameof(IndexIndex));
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


        public IActionResult OutgoingmailreportEksnKurir()
        {
            return View();
        }


        [HttpPost]
        public async Task<string> SearchOutgoingMailReportEksnKurir(string? trackingNumber, DateTime? startDate, DateTime? endDate)
        {
            List<ReportOutgoingMailOutput> reportOutput = new List<ReportOutgoingMailOutput>();
            GeneralOutputModel generalOutput = new GeneralOutputModel();
            List<Dictionary<String, String>> _list = new List<Dictionary<string, string>>();
            var token = HttpContext.Session.GetString("token");
            string a = "2022-01-01";
            //string b = "2022-01-01";
            CultureInfo provider = CultureInfo.InvariantCulture;
            DateTime dt = DateTime.ParseExact(a, "yyyy-MM-dd", provider);
            ParamReportOutgoingMailEksnKurir pr = new ParamReportOutgoingMailEksnKurir();
            pr.trackingNumber = trackingNumber;
            pr.startDate = dt;
            pr.endDate = dt;
            generalOutput = await _dataAccessProvider.GetDataReportOutgoingMailEkspedisinKurir("Document/GetDataOutgoingMailEksnKurir", token, pr);
            var jsonApiResponseSerialize = JsonConvert.SerializeObject(generalOutput.Result);
            reportOutput = JsonConvert.DeserializeObject<List<ReportOutgoingMailOutput>>(jsonApiResponseSerialize);
            int no = 1;
            foreach (var item in reportOutput)
            {
                Dictionary<String, String> dict = new Dictionary<string, string>();
                dict.Add("no", no.ToString());
                dict.Add("letter_number", item.letter_number);
                dict.Add("deliveryname", item.deliveryname);
                dict.Add("ReceiptDate", item.ReceiptDate == null ? "" : Convert.ToDateTime(item.ReceiptDate).ToString("dd MMM yyyy"));
                dict.Add("expedition_name", item.expedition_name);
                dict.Add("sender_name", item.sender_name);
                dict.Add("unitname", item.unitname);
                dict.Add("docReceiver", item.docReceiver);
                dict.Add("nmrreferen", item.nmrreferen);
                dict.Add("address", item.address);
                dict.Add("statusname", item.statusname);
                _list.Add(dict);
                no++;
            }

            return JsonConvert.SerializeObject(_list);
        }


        public async Task<IActionResult> ExportExcelRegisOutgoingmail(ParamReportNonOuboxLetter pr)
        {
            //using (var workbook = new XLWorkbook())
            //{

            //    List<ReportNonOutboxLetterOutput> documentOutput = new List<ReportNonOutboxLetterOutput>();
            //    GeneralOutputModel generalOutput = new GeneralOutputModel();
            //    var token = HttpContext.Session.GetString("token");
            //    generalOutput = await _dataAccessProvider.GetDataReportNonEoffice("NonEofficeLetters/GetDataNonEofficeLetter", token, pr);
            //    var jsonApiResponseSerialize = JsonConvert.SerializeObject(generalOutput.Result);
            //    documentOutput = JsonConvert.DeserializeObject<List<ReportNonOutboxLetterOutput>>(jsonApiResponseSerialize);
            //    var worksheet = workbook.Worksheets.Add("data");
            //    var curerentRow = 1;

            //    worksheet.Cell(curerentRow, 1).Value = "Nomor Surat";
            //    worksheet.Cell(curerentRow, 2).Value = "Tipe Kirim";
            //    worksheet.Cell(curerentRow, 3).Value = "Tanggal";
            //    worksheet.Cell(curerentRow, 4).Value = "Nama Ekspedisi";
            //    worksheet.Cell(curerentRow, 5).Value = "Nama Pengirim";
            //    worksheet.Cell(curerentRow, 6).Value = "Division";
            //    worksheet.Cell(curerentRow, 7).Value = "Penerima";
            //    worksheet.Cell(curerentRow, 8).Value = "Nomor Refrensi";
            //    worksheet.Cell(curerentRow, 9).Value = "Alamat";
            //    worksheet.Cell(curerentRow, 10).Value = "Status";
            //    foreach (var item in documentOutput)
            //    {
            //        curerentRow++;
            //        worksheet.Cell(curerentRow, 1).Value = item.letter_number.ToString();
            //        worksheet.Cell(curerentRow, 2).Value = item.deliveryname.ToString();
            //        worksheet.Cell(curerentRow, 3).Value = item.ReceiptDate == null ? "" : Convert.ToDateTime(item.ReceiptDate).ToString("dd MMM yyyy");
            //        worksheet.Cell(curerentRow, 4).Value = item.expedition_name.ToString();
            //        worksheet.Cell(curerentRow, 5).Value = item.sender_name.ToString();
            //        worksheet.Cell(curerentRow, 6).Value = item.unitname.ToString();
            //        worksheet.Cell(curerentRow, 7).Value = item.docReceiver.ToString();
            //        worksheet.Cell(curerentRow, 8).Value = item.nmrreferen.ToString();
            //        worksheet.Cell(curerentRow, 9).Value = item.address.ToString();
            //        worksheet.Cell(curerentRow, 10).Value = item.statusname.ToString();

            //    }
            //    using (var stream = new MemoryStream())
            //    {
            //        workbook.SaveAs(stream);
            //        var content = stream.ToArray();
            //        return File(content, "application/vnd.openxmlformats-officedocument.spreadsheethtml.sheet", "ReportNonEoffcie.xlsx");
            //    }

            //}
            return Redirect("~/Document/OutgoingmailreportEksnKurir");
        }


        public IActionResult LacakSuratKeluar()
        {
            return View();
        }

        [HttpPost]
        public async Task<string> SearchOutgoingLetter(string trackingNumber)
        {
            

            List<DeliveryReportOutput> output = new List<DeliveryReportOutput>();
            GeneralOutputModel generalOutput = new GeneralOutputModel();
            List<Dictionary<String, String>> _list = new List<Dictionary<string, string>>();
            var token = HttpContext.Session.GetString("token");

            ParamGetReportSeacrhoutGoing pr = new ParamGetReportSeacrhoutGoing();
            pr.statusElse = 0;
            pr.trackingNumber= trackingNumber;
            generalOutput = await _dataAccessProvider.GetSeacrhoutGoingEoffice("Document/GetSearchOutgoingLetterNumber/", token, pr);
            var jsonApiResponseSerialize = JsonConvert.SerializeObject(generalOutput.Result);
            output = JsonConvert.DeserializeObject<List<DeliveryReportOutput>>(jsonApiResponseSerialize);
            int no = 1;
            foreach (var item in output)
            {
                Dictionary<String, String> dict = new Dictionary<string, string>();
                dict.Add("no", no.ToString());
                dict.Add("letterNumber", item.letterNumber);
                dict.Add("sender", item.sender);
                dict.Add("senderDivision", item.senderDivision);
                dict.Add("receiveDate", item.receiveDate == null ? "" : Convert.ToDateTime(item.receiveDate).ToString("dd MMM yyyy"));
                dict.Add("expedition", item.expedition);
                dict.Add("referenceNumber", item.referenceNumber);
                dict.Add("receiptNumber", item.receiptNumber);
                dict.Add("receiver", item.receiver);
                dict.Add("destinationReceiver", item.destination_receiver_name);
                dict.Add("receiverAddress", item.receiverAddress);
                dict.Add("receiverPhone", item.receiverPhone);
                dict.Add("shippingType", item.shippingTypeCodeValue);
                dict.Add("deliveryType", item.deliveryTypeCodeValue);
                dict.Add("status", item.statusCodeValue);

                _list.Add(dict);
                no++;
            }

            return JsonConvert.SerializeObject(_list);
        }
    }
}
