using EofficeBNILWEB.DataAccess;
using EofficeBNILWEB.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json;
using System.Drawing;
using System.Data.OleDb;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;
using System.Data;
using ClosedXML.Excel;
using System.Text;

using System.Drawing.Imaging;
using System.Globalization;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.AspNetCore.Localization;
using QRCoder;
using QRCode = QRCoder.QRCode;
using System.Diagnostics.Metrics;
using DocumentFormat.OpenXml.Presentation;
using DocumentFormat.OpenXml.Wordprocessing;
using DocumentFormat.OpenXml.Spreadsheet;
using System.Xml.Linq;
using System.Web.WebPages;
using System.Configuration;
using System.Drawing.Drawing2D;
using System.Net;
using System.Net.Http;
using iTextSharp.text;
using Org.BouncyCastle.Utilities.Net;
using DocumentFormat.OpenXml.ExtendedProperties;
using System.Text.RegularExpressions;
using ZXing.Common;
using ZXing;
using BarcodeLib;
using System.IO;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using Org.BouncyCastle.Asn1.X509;
using System.Drawing.Text;
using System;
using RawPrint;
using System.Net.Mime;
using iTextSharp.text.pdf;
using Rectangle = iTextSharp.text.Rectangle;
using Document = iTextSharp.text.Document;
using Paragraph = iTextSharp.text.Paragraph;
using DocumentFormat.OpenXml.Bibliography;
using Microsoft.VisualBasic;

namespace EofficeBNILWEB.Controllers
{

    public static class BitmapExtension
    {
        public static byte[] ConvertBitmapToByteArray(this Bitmap bitmap)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                bitmap.Save(ms, ImageFormat.Png);
                return ms.ToArray();
            }
        }
    }
    public class GenerateNoDocController : Controller
    {
        private readonly IConfiguration _config;
        private readonly IDataAccessProvider _dataAccessProvider;
        private readonly string urlApi;
        private readonly IHtmlLocalizer<HomeController> _localiza;
        private readonly IHostingEnvironment _environment;

        public GenerateNoDocController(IConfiguration config, IDataAccessProvider dataAccessProvider, IHostingEnvironment Environment, IHtmlLocalizer<HomeController> localiza)
        {
            _config = config;
            _dataAccessProvider = dataAccessProvider;
            urlApi = _config.GetValue<string>("AppSettings:apiUrl");
            _localiza = localiza;
            _environment = Environment;
        }
        public async Task<IActionResult> Index()
        {

            List<AdminDivisionOutput> divisionList = new List<AdminDivisionOutput>();
            GeneralOutputModel generalOutput = new GeneralOutputModel();
            var token = HttpContext.Session.GetString("token");

            List<StringmapOutput> documentTypeList = new List<StringmapOutput>();
            ParamGetStringmap pr = new ParamGetStringmap();
            pr.objectName = "tr_document_number";
            pr.attributeName = "NUMBER_TYPE";

            generalOutput = await _dataAccessProvider.GetStringmapAsync("General/GetStringmap/", pr, token);
            var jsonApiResponseSerialize = JsonConvert.SerializeObject(generalOutput.Result);
            documentTypeList = JsonConvert.DeserializeObject<List<StringmapOutput>>(jsonApiResponseSerialize);
            ViewBag.DocumentType = documentTypeList;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(ParamInsertGenerateNoDoc pr)
        {
            try
            {
                //var resulttp = pr.type == 1 ? "ND" : "RT";

                if (pr.reprintbarcode != null)
                {

                    var resulttp = pr.type == 1 ? "ND" : "RT";
                    string nd = pr.reprintbarcode;
                    string result1 = nd.Substring(0, 2);
                    if (result1 == resulttp)
                    {

                        var tokenn = HttpContext.Session.GetString("token");
                        GeneralOutputModel generalOutputt = new GeneralOutputModel();
                        DataOuputValidasiBarcode dvbr = new DataOuputValidasiBarcode();
                        dvbr.type_brcd = pr.type;
                        dvbr.nmr_brcd = pr.reprintbarcode.ToString();
                        generalOutputt = await _dataAccessProvider.PostValidasiBarcode("Document/ValidasiBarcodeMallingRoom", tokenn, dvbr);
                        var jsonApiResponseSerializee = JsonConvert.SerializeObject(generalOutputt.Result);
                        var status = generalOutputt.Status;
                        //var Dataa = JsonConvert.DeserializeObject<string>(jsonApiResponseSerializee);

                        if (status == "OK")
                        {
                            CultureInfo culturee = new CultureInfo("en-IN");
                            var nowtgl = DateTime.Now;
                            var dta = nowtgl.ToString("dd-MMMM-yyyy HH:ss:mm", culturee);
                            ViewBag.barcode = pr.reprintbarcode;
                            ViewBag.tipe = pr.type;
                            ViewBag.tipeDate = dta;
                            TempData["status"] = "OK";
                            TempData["title"] = "Berhasil Re-print nomor resi";
                            return View();
                        }
                        else
                        {
                            TempData["status"] = "NG";
                            TempData["title"] = "Gagal Re-print";
                            return Redirect("~/GenerateNoDoc/Index");

                        }

                    }
                    else
                    {
                        TempData["status"] = "NG";
                        TempData["title"] = "Tipe Pada No Resi Tidak Sesuai ";
                        return Redirect("~/GenerateNoDoc/Index");
                    }
                }


                var token = HttpContext.Session.GetString("token");
                GeneralOutputModel generalOutput = new GeneralOutputModel();
                generalOutput = await _dataAccessProvider.PostInsertGenerateNoDoc("Document/BarcodeMallingRoom", pr, token);
                var jsonApiResponseSerialize = JsonConvert.SerializeObject(generalOutput.Result);
                var Data = JsonConvert.DeserializeObject<string>(jsonApiResponseSerialize);
                //var result = pr.reprintbarcode != null ? pr.reprintbarcode :  Data;
                CultureInfo culture = new CultureInfo("en-IN");
                var now = DateTime.Now;
                var dt = now.ToString("dd-MMMM-yyyy HH:ss:mm", culture);
                ViewBag.barcode = Data;
                ViewBag.tipe = pr.type;
                ViewBag.tipeDate = dt;

                return View();
            }
            catch (Exception ex)
            {
                TempData["status"] = "NG";
                TempData["title"] = "Gagal menambahkan dokumen masuk";
                TempData["pesan"] = ex.ToString();
                return Redirect("~/GenerateNoDoc/Index");
            }
        }


        public IActionResult Viewbarcode(ParamInsertGenerateNoDoc pr)
        {
            CultureInfo culture = new CultureInfo("en-IN");
            var now = DateTime.Now;
            var dt = now.ToString("dd-MMMM-yyyy HH:ss:mm", culture);
            ViewBag.tanggal = dt;
            ViewBag.tipe = pr.type;
            ViewBag.barcode = pr.barcode;
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

        #region non-office  ekspedisi
        public async Task<IActionResult> RegisterExpeditionNonEoffcie()
        {
            List<OutputletterNonEoffice> AlldataList = new List<OutputletterNonEoffice>();
            List<AdminDivisionOutput> divisionList = new List<AdminDivisionOutput>();
            GeneralOutputModel generalOutput = new GeneralOutputModel();
            var token = HttpContext.Session.GetString("token");

            generalOutput = await _dataAccessProvider.GetSuratKeluarNon("Document/GetDataSuratKeluarNon/", token);
            var jsonApiResponseSerializee = JsonConvert.SerializeObject(generalOutput.Result);
            AlldataList = JsonConvert.DeserializeObject<List<OutputletterNonEoffice>>(jsonApiResponseSerializee);

            generalOutput = await _dataAccessProvider.GetUnitAsync("Document/GetAdminDivisionNonEoffice/", token);
            var jsonApiResponseSerialize = JsonConvert.SerializeObject(generalOutput.Result);
            divisionList = JsonConvert.DeserializeObject<List<AdminDivisionOutput>>(jsonApiResponseSerialize);

            if (generalOutput.Status == "UA")
            {
                return RedirectToAction("Logout", "Home");
            }

            List<StringmapOutput> deliveryTypeList = new List<StringmapOutput>();
            ParamGetStringmap pr = new ParamGetStringmap();
            pr.objectName = "tr_letter_noneoffice";
            pr.attributeName = "DELIVERY_TYPE";

            generalOutput = await _dataAccessProvider.GetStringmapAsync("General/GetStringmap/", pr, token);
            jsonApiResponseSerialize = JsonConvert.SerializeObject(generalOutput.Result);
            deliveryTypeList = JsonConvert.DeserializeObject<List<StringmapOutput>>(jsonApiResponseSerialize);


            List<StringmapOutputNonEoffice> StatusSendList = new List<StringmapOutputNonEoffice>();
            ParamGetStringmapNonEoffice prr = new ParamGetStringmapNonEoffice();
            prr.objectName = "tr_letter_noneoffice";
            prr.attributeName = "STATUS_SEND";

            generalOutput = await _dataAccessProvider.GetStringmapNonEofficeAsync("General/GetStringmap/", prr, token);
            jsonApiResponseSerialize = JsonConvert.SerializeObject(generalOutput.Result);
            StatusSendList = JsonConvert.DeserializeObject<List<StringmapOutputNonEoffice>>(jsonApiResponseSerialize);

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
                var OutputUploadDocument = JsonConvert.DeserializeObject<List<UploadputletterNonEoffice>>((string)TempData["OutputApi"]);

             
                ViewBag.Output = OutputUploadDocument;

                ViewBag.UlTabActiveHome = "";
                ViewBag.DivTabActiveHome = "fade";
                ViewBag.UlTabActiveUpload = "active";
                ViewBag.DivTabActiveUpload = "active show";
            } else if(TempData["UpdateOutputApi"] != null)
            {
             
                var OutputUpdateUploadDocument = JsonConvert.DeserializeObject<List<UpdtaeUploadputletterNonEoffice>>((string)TempData["UpdateOutputApi"]);

                ViewBag.OutputUpdate = OutputUpdateUploadDocument;

                //ViewBag.UlTabActiveHome = "";
                //ViewBag.DivTabActiveHome = "fade";
                //ViewBag.UlTabActiveUpload = "active";
                //ViewBag.DivTabActiveUpload = "active show";
            }


            OutputletterNonEoffice vald = new OutputletterNonEoffice();
             vald.updtae = 1;
             vald.qrcode = 2;
            ViewBag.Divison = divisionList;
            ViewBag.DeliveryType = deliveryTypeList;
            ViewBag.StatusSend = StatusSendList;
            ViewBag.AllNonLetter = AlldataList;
            ViewBag.UpdateView = vald.updtae;
            ViewBag.QrcodeView = vald.qrcode;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RegisterExpeditionNonEoffcie(ParamInsertNonEoffice pr)
        {
            try
            {
                //int update = 1;
                //int qrcode = 2;
                var token = HttpContext.Session.GetString("token");
                GeneralOutputModel generalOutput = new GeneralOutputModel();
                
                generalOutput = await _dataAccessProvider.PostInsertSendNonEofficeAsync("Document/ParamInsertNonEoffice", pr, token);
                        var jsonApiResponseSerialize = JsonConvert.SerializeObject(generalOutput.Result);
                        var result = pr.delivery_type == 1 ? "Ekspedisi" : "Kurir";
                        
                        if (generalOutput.Status == "OK")
                        {
                            TempData["status"] = "OK";
                            TempData["title"] = "Berhasil menambahkan surat keluar  non e-office";
                            TempData["pesan"] = generalOutput.Message;
                        }
                        else if (generalOutput.Status == "UA")
                        {
                            return RedirectToAction("Logout", "Home");
                        }
                        else
                        {
                            TempData["status"] = "NG";
                            TempData["title"] = "Gagal menambahkan Suarat Keluar  non e-office";
                            TempData["pesan"] = generalOutput.Message;
                        }
                

                return Redirect("~/GenerateNoDoc/RegisterExpeditionNonEoffcie");
            }
            catch (Exception ex)
            {
                TempData["status"] = "NG";
                TempData["title"] = "Gagal menambahkan dokumen masuk";
                TempData["pesan"] = ex.ToString();
                return Redirect("~/GenerateNoDoc/RegisterExpeditionNonEoffcie");
            }
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
            receiverList = JsonConvert.DeserializeObject<List<AdminDivisionOutput>>(jsonApiResponseSerialize);
            jsonApiResponseSerialize = JsonConvert.SerializeObject(receiverList);

            return jsonApiResponseSerialize;
        }

        public async Task<IActionResult> ExportExcelEkspedisiNonEoffice(ParamReportNonOuboxLetter pr)
        {
            
            using (var workbook = new XLWorkbook())
            {

                List<ReportNonOutboxLetterOutput> documentOutput = new List<ReportNonOutboxLetterOutput>();
                GeneralOutputModel generalOutput = new GeneralOutputModel();
                var token = HttpContext.Session.GetString("token");
                generalOutput = await _dataAccessProvider.GetDataReportNonEoffice("NonEofficeLetters/GetDataNonEofficeLetter", token, pr);
                var jsonApiResponseSerializee = JsonConvert.SerializeObject(generalOutput.Result);
                documentOutput = JsonConvert.DeserializeObject<List<ReportNonOutboxLetterOutput>>(jsonApiResponseSerializee);
                var worksheet = workbook.Worksheets.Add("data");
                var curerentRow = 1;
                worksheet.Cell(curerentRow, 1).Value = "No AWB/No Pengiriman";
                worksheet.Cell(curerentRow, 2).Value = "Nomor Refrensi";
                worksheet.Cell(curerentRow, 3).Value = "Tanggal Pengiriman";
                worksheet.Cell(curerentRow, 4).Value = "Nama Ekspedisi";
                worksheet.Cell(curerentRow, 5).Value = "Nama Pengirim";
                worksheet.Cell(curerentRow, 6).Value = "NPP";
                worksheet.Cell(curerentRow, 7).Value = "Nama Divisi";
                worksheet.Cell(curerentRow, 8).Value = "Kode Divisi";
                worksheet.Cell(curerentRow, 9).Value = "Nomor Surat";
                worksheet.Cell(curerentRow, 10).Value = "Nama Tujuan";
                worksheet.Cell(curerentRow, 11).Value = "Nomor Telepon";
                worksheet.Cell(curerentRow, 12).Value = "Alamat";
                worksheet.Cell(curerentRow, 13).Value = "Nama Penerima";
                worksheet.Cell(curerentRow, 14).Value = "Tanggal Penerima";
                worksheet.Cell(curerentRow, 15).Value = "Status Pengiriman";
                foreach (var item in documentOutput)
                {
                    curerentRow++;

                    worksheet.Cell(curerentRow, 1).Value = item.nmrawb == null ? "" : item.nmrawb;
                    worksheet.Cell(curerentRow, 2).Value = item.nmrreferen == null ? "" : item.nmrreferen;
                    worksheet.Cell(curerentRow, 3).Value = item.ReceiptDate == null ? "" : item.ReceiptDate;
                    worksheet.Cell(curerentRow, 4).Value = item.expedition_name.ToString();
                    worksheet.Cell(curerentRow, 5).Value = item.sender_name.ToString();
                    worksheet.Cell(curerentRow, 6).Value = "'" + item.nip.ToString();
                    worksheet.Cell(curerentRow, 7).Value = item.unitname.ToString();
                    worksheet.Cell(curerentRow, 8).Value = item.kodeunit.ToString();
                    worksheet.Cell(curerentRow, 9).Value = item.letter_number.ToString();
                    worksheet.Cell(curerentRow, 10).Value = item.docReceiver.ToString();
                    worksheet.Cell(curerentRow, 11).Value = item.phonenumber == null ? "" : item.phonenumber.ToString();
                    worksheet.Cell(curerentRow, 12).Value = item.address.ToString();
                    worksheet.Cell(curerentRow, 13).Value = item.purposename == null ? "" : item.purposename.ToString();
                    worksheet.Cell(curerentRow, 14).Value = item.DateUntil == null ? "" : Convert.ToDateTime(item.DateUntil).ToString();
                    worksheet.Cell(curerentRow, 15).Value = item.statusname.ToString();

                }
                using (var stream = new MemoryStream())
                {

                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    var data = File(content, "application/vnd.openxmlformats-officedocument.spreadsheethtml.sheet", "NON_E-OFFICE_EKSPEDISI_REGISTRATION.xlsx");
                    return (data);
                    return Redirect("~/GenerateNoDoc/RegisterExpeditionNonEoffcie");
                }

            }


        }

        
        public async Task<IActionResult> GetDataExportExcelEkspedisiNonEoffice(ParamReportNonOuboxLetter pr)
        {
            using (var workbook = new XLWorkbook())
            {

                List<ReportNonOutboxLetterOutput> documentOutput = new List<ReportNonOutboxLetterOutput>();
                GeneralOutputModel generalOutput = new GeneralOutputModel();
                var token = HttpContext.Session.GetString("token");
                generalOutput = await _dataAccessProvider.GetDataReportNonEoffice("NonEofficeLetters/GetDataExpotUsingUpdateNonEofficeEkspedisi", token, pr);
                var jsonApiResponseSerializee = JsonConvert.SerializeObject(generalOutput.Result);
                documentOutput = JsonConvert.DeserializeObject<List<ReportNonOutboxLetterOutput>>(jsonApiResponseSerializee);
                var worksheet = workbook.Worksheets.Add("data");
                var curerentRow = 1;
                worksheet.Cell(curerentRow, 1).Value = "No AWB/No Pengiriman";
                worksheet.Cell(curerentRow, 2).Value = "Nomor Refrensi";
                worksheet.Cell(curerentRow, 3).Value = "Nama Ekspedisi";
                worksheet.Cell(curerentRow, 4).Value = "Nama Pengirim";
                worksheet.Cell(curerentRow, 5).Value = "NPP";
                worksheet.Cell(curerentRow, 6).Value = "Nama Divisi";
                worksheet.Cell(curerentRow, 7).Value = "Kode Divisi";
                worksheet.Cell(curerentRow, 8).Value = "Nomor Surat";
                worksheet.Cell(curerentRow, 9).Value = "Nama Tujuan";
                worksheet.Cell(curerentRow, 10).Value = "Nomor Telepon";
                worksheet.Cell(curerentRow, 11).Value = "Alamat";
                worksheet.Cell(curerentRow, 12).Value = "Nama Penerima";
                worksheet.Cell(curerentRow, 13).Value = "Tanggal Penerima";
                worksheet.Cell(curerentRow, 14).Value = "Status Pengiriman";
                
                //worksheet.Cell(curerentRow, 14).Value = "Status Pengiriman";
                foreach (var item in documentOutput)
                {
                    curerentRow++;

                    worksheet.Cell(curerentRow, 1).Value = item.nmrawb == null ? "" : item.nmrawb;
                    worksheet.Cell(curerentRow, 2).Value = item.nmrreferen == null ? "" : item.nmrreferen;
                    worksheet.Cell(curerentRow, 3).Value = item.expedition_name.ToString();
                    worksheet.Cell(curerentRow, 4).Value = item.sender_name.ToString();
                    worksheet.Cell(curerentRow, 5).Value = "'" + item.nip.ToString();
                    worksheet.Cell(curerentRow, 6).Value = item.unitname.ToString();
                    worksheet.Cell(curerentRow, 7).Value = item.kodeunit.ToString();
                    worksheet.Cell(curerentRow, 8).Value = item.letter_number.ToString();
                    worksheet.Cell(curerentRow, 9).Value = item.docReceiver.ToString();
                    worksheet.Cell(curerentRow, 10).Value = item.phonenumber == null ? "" : item.phonenumber.ToString();
                    worksheet.Cell(curerentRow, 11).Value = item.address == null? "" :item.address.ToString();
                    worksheet.Cell(curerentRow, 12).Value = item.purposename == null ? "" : item.purposename.ToString();
                    worksheet.Cell(curerentRow, 13).Value = item.statuskirim == 1 ? "" : item.statuskirim == 2 ? "" : item.statuskirim == 3 ? item.tgluntil.ToString() : "" ;
                    worksheet.Cell(curerentRow, 14).Value = item.statusname.ToString();
                    //worksheet.Cell(curerentRow, 13).Value = "";
                    //worksheet.Cell(curerentRow, 14).Value = item.statusname.ToString();

                }
                using (var stream = new MemoryStream())
                {

                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    var data = File(content, "application/vnd.openxmlformats-officedocument.spreadsheethtml.sheet", "UPDATE_NON_E-OFFICE_EKSPEDISI_REGISTRATION.xlsx");
                   
                    return (data);

                    
                }

            }

        }

        [HttpPost]
        public async Task<IActionResult> UpdateLetterNonEoffice(UpdateNonEoffice pr)
        {
            try
            {
                //int update = 1;
                //int qrcode = 2;
                var token = HttpContext.Session.GetString("token");
                GeneralOutputModel generalOutput = new GeneralOutputModel();

                generalOutput = await _dataAccessProvider.PostUpdateNonEofficeAsync("Document/ParamUpdateNonEoffice", pr, token);
                var jsonApiResponseSerialize = JsonConvert.SerializeObject(generalOutput.Result);
               

                if (generalOutput.Status == "OK")
                {
                    TempData["status"] = "OK";
                    TempData["title"] = "Berhasil mengubah surat keluar non e-office";
                    TempData["pesan"] = generalOutput.Message;
                }
                else if (generalOutput.Status == "UA")
                {
                    return RedirectToAction("Logout", "Home");
                }
                else
                {
                    TempData["status"] = "NG";
                    TempData["title"] = "Gagal mengubah status keluar  non e-office";
                    TempData["pesan"] = generalOutput.Message;
                }


                return Redirect("~/GenerateNoDoc/RegisterExpeditionNonEoffcie");
            }
            catch (Exception ex)
            {
                TempData["status"] = "NG";
                TempData["title"] = "Gagal menambahkan dokumen masuk";
                TempData["pesan"] = ex.ToString();
                return Redirect("~/GenerateNoDoc/RegisterExpeditionNonEoffcie");
            }
        }

        public FileResult DownloadTemplateNonEoffice()
        {
            var fileName = "TemplateUploadEkspedisiNonEoffice.xlsx";
            //Build the File Path.
            string path = Path.Combine(_environment.WebRootPath, "download/") + fileName;

            //Read the File data into Byte Array.
            byte[] bytes = System.IO.File.ReadAllBytes(path);

            //Send the File to Download.
            return File(bytes, "application/octet-stream", fileName);
        }
        
        
        public async Task<IActionResult> UploadDocumentNonEoffice(IFormFile inputfile)
        {
            try
            {
                var token = HttpContext.Session.GetString("token");
                GeneralOutputModel generalOutput = new GeneralOutputModel();
                List<ParamUploadputletterNonEoffice> data = new List<ParamUploadputletterNonEoffice>();
                ParamUploadLetterNonOfficeString pr = new ParamUploadLetterNonOfficeString();
                List<UploadputletterNonEoffice> responseApi = new List<UploadputletterNonEoffice>();
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
                    int a = 1;
                    foreach (DataRow row in dataUpload.Rows)
                    {
                        var no = a;
                        string nmrawb = row["No AWB/No Pengiriman"].ToString();

                        string nmrreferen = row["Nomor Referensi"].ToString();

                        string expedition_name = row["Nama Ekspedisi"].ToString();
                        if (expedition_name == "" || expedition_name == null)
                        {
                            break;
                        }

                        string sender_name = row["Nama Pengirim"].ToString();
                        if (sender_name == "" || sender_name == null)
                        {
                            break;
                        }

                        string nip = row["NPP"].ToString();
                        if (nip == null || nip == "")
                        {
                            break;
                        }
                        var nn = Convert.ToString(nip);
                        string npp = nip.Replace("'", "");

                        string unitname = row["Kode Divisi"].ToString();
                        if (unitname == "" || unitname == null)
                        {
                            break;
                        }

                        string letter_number = row["Nomor Surat"].ToString();
                        if (letter_number == null || letter_number == "")
                        {
                            break;
                        }

                        string docReceiver = row["Nama Tujuan"].ToString();
                        if (docReceiver == "" || docReceiver == null)
                        {
                            break;
                        }

                        string phonenumber = row["Nomor Telepon"].ToString();

                        string address = row["Alamat"].ToString();
                        if (address == "" || address == null)
                        {
                            break;
                        }

                        //string purposename = row["Nama Penerima"].ToString();

                        //string status = row["Status Pengiriman"].ToString();
                        //if (status == "" || status == null)
                        //{
                        //    break;
                        //}
                        data.Add(new ParamUploadputletterNonEoffice
                        {
                            letter_number = letter_number,
                            nmrawb = nmrawb,
                            expedition_name = expedition_name,
                            sender_name = sender_name,
                            nip = nn,
                            unitname = unitname,
                            docReceiver = docReceiver,
                            phonenumber = phonenumber,
                            nmrreferen = nmrreferen,
                            address = address,
                            //purposename = purposename,
                            //statuskirim = status

                        });
                        a++;
                    }
                    pr.jsonDataString = data;
                    if (pr.jsonDataString.Count == 0)
                    {
                        TempData["status"] = "NG";
                        TempData["title"] = "Gagal Upload";
                        TempData["pesan"] = generalOutput.Message;

                        return Redirect("~/GenerateNoDoc/RegisterExpeditionNonEoffcie");
                    }
                    generalOutput = await _dataAccessProvider.PostUploadLetterNonEoffcieAsync("Document/InsertEkspedisiUsingUpload", token, pr);
                    var jsonApiResponseSerialize = JsonConvert.SerializeObject(generalOutput.Result);
                    responseApi = JsonConvert.DeserializeObject<List<UploadputletterNonEoffice>>(jsonApiResponseSerialize);

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
                        TempData["title"] = "Gagal menambahkan data ekspedisi";
                        TempData["pesan"] = generalOutput.Message;
                    }
                    TempData["OutputApi"] = jsonApiResponseSerialize;
                    return Redirect("~/GenerateNoDoc/RegisterExpeditionNonEoffcie");
                    //pr.jsonDataString = data;
                    //if (pr.jsonDataString.Count != dataUpload.Rows.Count)
                    //{
                    //    TempData["status"] = "NG";
                    //    TempData["title"] = "Gagal menambah data ekspedisi,silahkan di periksa kembali format kolom atau kolom dalam dokumen bernilai kosong";
                    //    TempData["pesan"] = generalOutput.Message;
                    //    TempData["OutputApi"] = null;

                    //    return Redirect("~/GenerateNoDoc/RegisterExpeditionNonEoffcie");
                    //}
                    //else
                    //{
                    //    pr.jsonDataString = data;
                    //    generalOutput = await _dataAccessProvider.PostUploadLetterNonEoffcieAsync("Document/InsertEkspedisiUsingUpload", token, pr);
                    //    var jsonApiResponseSerialize = JsonConvert.SerializeObject(generalOutput.Result);
                    //    responseApi = JsonConvert.DeserializeObject<List<UploadputletterNonEoffice>>(jsonApiResponseSerialize);

                    //    if (generalOutput.Status == "OK")
                    //    {
                    //        TempData["status"] = "OK";
                    //        TempData["title"] = "Info";
                    //        TempData["pesan"] = generalOutput.Message;
                    //    }
                    //    else if (generalOutput.Status == "UA")
                    //    {
                    //        TempData["status"] = "NG";
                    //        TempData["pesan"] = "Session habis silahkan login kembali";
                    //        return RedirectToAction("Logout", "Home");
                    //    }
                    //    else
                    //    {
                    //        TempData["status"] = "NG";
                    //        TempData["title"] = "Gagal menambahkan dokumen masuk";
                    //        TempData["pesan"] = generalOutput.Message;
                    //    }
                    //    TempData["OutputApi"] = jsonApiResponseSerialize;
                    //    return Redirect("~/GenerateNoDoc/RegisterExpeditionNonEoffcie");
                    //}

                    //return Redirect("~/GenerateNoDoc/RegisterExpeditionNonEoffcie");
                }

                TempData["status"] = "NG";
                TempData["pesan"] = "Siliahkan pilih file yang akan di upload";

                return Redirect("~/GenerateNoDoc/RegisterExpeditionNonEoffcie");
            }
            catch (Exception ex)
            {
                TempData["status"] = "NG";
                TempData["title"] = "Gagal";
                TempData["pesan"] ="Gagal upload Dokumen";
                return Redirect("~/GenerateNoDoc/RegisterExpeditionNonEoffcie");
            }
        }


        public FileResult DownloadTemplateUpdateNonEoffice()
        {
            var fileName = "TemplateUpdateEkspedisiNonEoffice.xlsx";
            //Build the File Path.
            string path = Path.Combine(_environment.WebRootPath, "download/") + fileName;

            //Read the File data into Byte Array.
            byte[] bytes = System.IO.File.ReadAllBytes(path);

            //Send the File to Download.
            return File(bytes, "application/octet-stream", fileName);
        }

        public async Task<IActionResult> UpdateUploadDocumentNonEoffice(IFormFile inputfile)
        {
            try
            {
                var token = HttpContext.Session.GetString("token");
                GeneralOutputModel generalOutput = new GeneralOutputModel();
                List<ParamUploadputletterNonEoffice> data = new List<ParamUploadputletterNonEoffice>();
                ParamUploadLetterNonOfficeString pr = new ParamUploadLetterNonOfficeString();
                List<UpdtaeUploadputletterNonEoffice> responseApi = new List<UpdtaeUploadputletterNonEoffice>();
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
                    int i = 1;
                    foreach (DataRow row in dataUpload.Rows)
                    {
                       
                        int no = i;

                        string nmrawb = row["No AWB/No Pengiriman"].ToString();

                        string nmrreferen = row["Nomor Refrensi"].ToString();

                        string expedition_name = row["Nama Ekspedisi"].ToString();
                        if (expedition_name == "" || expedition_name == null)
                        {
                            break;
                        }

                        string sender_name = row["Nama Pengirim"].ToString();
                        if (sender_name == "" || sender_name == null)
                        {
                            break;
                        }

                        string nip = row["NPP"].ToString();
                        if (nip == null || nip == "")
                        {
                            break;
                        }
                        var nn = Convert.ToString(nip);
                        string npp = nip.Replace("'", "");

                        string unitname = row["Nama Divisi"].ToString();
                        if (unitname == "" || unitname == null)
                        {
                            break;
                        }
                        string kodeunit = row["Kode Divisi"].ToString();
                        if (kodeunit == "" || kodeunit == null)
                        {
                            break;
                        }

                        string letter_number = row["Nomor Surat"].ToString();
                        if (letter_number == null || letter_number == "")
                        {
                            break;
                        }

                        string docReceiver = row["Nama Tujuan"].ToString();
                        if (docReceiver == "" || docReceiver == null)
                        {
                            break;
                        }

                        string phonenumber = row["Nomor Telepon"].ToString();

                        string address = row["Alamat"].ToString();
                        if (address == "" || address == null)
                        {
                            break;
                        }

                        string purposename = row["Nama Penerima"].ToString();

                        string DateUntil = row["Tanggal Penerima"].ToString();

                        
                        string status = row["Status Pengiriman"].ToString();
                        if (status == "" || status == null)
                        {
                            break;
                        }
                        DateTime d;
                        data.Add(new ParamUploadputletterNonEoffice
                        {
                            
                            letter_number = letter_number,
                            nmrawb = nmrawb,expedition_name = expedition_name,
                            sender_name = sender_name,
                            nip = nn,
                            unitname = unitname,
                            kodeunit=kodeunit,
                            docReceiver = docReceiver,
                            phonenumber = phonenumber,
                            nmrreferen = nmrreferen,
                            address = address,
                            purposename = purposename,
                            tgluntil = DateUntil,
                            
                            statuskirim = status
                        });
                        i++;
                    }
                    pr.jsonDataString = data;
                    generalOutput = await _dataAccessProvider.PostUpdateUploadLetterNonEoffcieAsync("Document/UpdateEkspedisiNonUsingUploadWeb", token, pr);
                    var jsonApiResponseSerialize = JsonConvert.SerializeObject(generalOutput.Result);
                    responseApi = JsonConvert.DeserializeObject<List<UpdtaeUploadputletterNonEoffice>>(jsonApiResponseSerialize);
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
                        TempData["title"] = "Gagal Mengubah data ekspedisi";
                        TempData["pesan"] = generalOutput.Message;
                    }
                    TempData["UpdateOutputApi"] = jsonApiResponseSerialize;

                    
                    return Redirect("~/GenerateNoDoc/RegisterExpeditionNonEoffcie");
                }

                TempData["status"] = "NG";
                TempData["pesan"] = "Siliahkan pilih file yang akan di upload";

                return Redirect("~/GenerateNoDoc/RegisterExpeditionNonEoffcie");
            }
            catch (Exception ex)
            {
                TempData["status"] = "NG";
                TempData["title"] = "Gagal menambahkan dokumen masuk";
                TempData["pesan"] = ex.ToString();
                return Redirect("~/GenerateNoDoc/RegisterExpeditionNonEoffcie");
            }
        }

        public async Task<string> GetEkspediCopy(string keyword)
        {
            DocumentOutputWeb documentOutput = new DocumentOutputWeb();
            GeneralOutputModel generalOutput = new GeneralOutputModel();
            List<OutputletterNonEoffice> receiverList = new List<OutputletterNonEoffice>();
            var token = HttpContext.Session.GetString("token");

            ParamGetEkspedisi pr = new ParamGetEkspedisi();
            pr.keyword = keyword;

            generalOutput = await _dataAccessProvider.GetDataEksepdisiNonEoffice("Document/GetDataEkspedisiNonEoffice/", token, pr);
            var jsonApiResponseSerialize = JsonConvert.SerializeObject(generalOutput.Result);
            receiverList = JsonConvert.DeserializeObject<List<OutputletterNonEoffice>>(jsonApiResponseSerialize);
            jsonApiResponseSerialize = JsonConvert.SerializeObject(receiverList);

            return jsonApiResponseSerialize;
        }

        [HttpPost]
        public async Task<string> GetDetailEkspedisiNon(string ids)
        {
            DocumentOutputWeb documentOutput = new DocumentOutputWeb();
            GeneralOutputModel generalOutput = new GeneralOutputModel();
            OutputletterNonEoffice DetailData = new OutputletterNonEoffice();
            var token = HttpContext.Session.GetString("token");

            getByIdNonEoffice pr = new getByIdNonEoffice();
            pr.letter_number = ids;

             generalOutput = await _dataAccessProvider.GetDetailsDataEkspediNonEoffice("Document/GetDetailsEkspedisiNonEofficeWeb/", pr, token);
             var jsonApiResponseSerialize = JsonConvert.SerializeObject(generalOutput.Result);
             DetailData = JsonConvert.DeserializeObject<OutputletterNonEoffice>(jsonApiResponseSerialize);
            //var DetailData = JsonConvert.SerializeObject(receiverList);
             var letterNumber =DetailData.letter_number;
             var numbrRfrence = DetailData.nmrreferen;
             var NoAWB = DetailData.nmrawb;
            var data = "" +
                "<div class='form-group'>" +
                    "<label for='inputpassword3' class='col-sm-5 col-form-label'>&nbsp; Nomor Surat</label>" +
                    "<input type='text'  class='form-control' value='"+ letterNumber + "' name='letter_number'  readonly>" +
                "</div>" +
                "<div class='form-group'>" +
                    "<label for='inputpassword3' class='col-sm-5 col-form-label'>&nbsp; Nomor Referensi</label>" +
                    "<input type='text' class='form-control' id='nmrreferen' value='"+ numbrRfrence + "' name='nmrreferen'>" +
                "</div>" +
                "<div class='form-group'>" +
                    "<label for='inputpassword3' class='col-sm-5 col-form-label'>&nbsp; No AWB / No Pengiriman</label>" +
                    "<input type='text' class='form-control' id='nmrawb' value='"+NoAWB+"' name='nmrawb'>" +
                "</div>" +
                "<div class='modal-footer border-top-0 d-flex justify-content-center'>" +
                    "<div class='btn-group'>" +
                    "<button type='submit' class='btn btn-primary'>Submit</button>" +
                    "</div>" +
                "</div>";
            return JsonConvert.SerializeObject(data);
            //return jsonApiResponseSerialize;
        }

        #endregion

        #region kurir non eoffice

        public async Task<IActionResult> RegisterKurirNonEoffcie()
        {
            List<OutputletterKurirNonEoffice> AlldataList = new List<OutputletterKurirNonEoffice>();
            List<AdminDivisionOutput> divisionList = new List<AdminDivisionOutput>();
            GeneralOutputModel generalOutput = new GeneralOutputModel();
            var token = HttpContext.Session.GetString("token");

            generalOutput = await _dataAccessProvider.GetSuratKeluarKurirNon("Document/GetDataSuratKeluarKurirNonWeb/", token);
            var jsonApiResponseSerializee = JsonConvert.SerializeObject(generalOutput.Result);
            AlldataList = JsonConvert.DeserializeObject<List<OutputletterKurirNonEoffice>>(jsonApiResponseSerializee);

            generalOutput = await _dataAccessProvider.GetUnitAsync("Document/GetAdminDivisionNonEoffice/", token);
            var jsonApiResponseSerialize = JsonConvert.SerializeObject(generalOutput.Result);
            divisionList = JsonConvert.DeserializeObject<List<AdminDivisionOutput>>(jsonApiResponseSerialize);

            if (generalOutput.Status == "UA")
            {
                return RedirectToAction("Logout", "Home");
            }

            List<StringmapOutput> deliveryTypeList = new List<StringmapOutput>();
            ParamGetStringmap pr = new ParamGetStringmap();
            pr.objectName = "tr_letter_noneoffice";
            pr.attributeName = "DELIVERY_TYPE";

            generalOutput = await _dataAccessProvider.GetStringmapAsync("General/GetStringmap/", pr, token);
            jsonApiResponseSerialize = JsonConvert.SerializeObject(generalOutput.Result);
            deliveryTypeList = JsonConvert.DeserializeObject<List<StringmapOutput>>(jsonApiResponseSerialize);


            List<StringmapOutputNonEoffice> StatusSendList = new List<StringmapOutputNonEoffice>();
            ParamGetStringmapNonEoffice prr = new ParamGetStringmapNonEoffice();
            prr.objectName = "tr_letter_noneoffice";
            prr.attributeName = "STATUS_SEND";

            generalOutput = await _dataAccessProvider.GetStringmapNonEofficeAsync("General/GetStringmap/", prr, token);
            jsonApiResponseSerialize = JsonConvert.SerializeObject(generalOutput.Result);
            StatusSendList = JsonConvert.DeserializeObject<List<StringmapOutputNonEoffice>>(jsonApiResponseSerialize);

            if (generalOutput.Status == "UA")
            {
                return RedirectToAction("Logout", "Home");
            }
            //ViewBag.Output = null;
            //ViewBag.UlTabActiveHome = "active";
            //ViewBag.DivTabActiveHome = "active show";
            //ViewBag.UlTabActiveUpload = "";
            //ViewBag.DivTabActiveUpload = "fade";


           

            //if (TempData["OutputApi"] != null)
            //{
            //    var OutputUploadDocument = JsonConvert.DeserializeObject<List<UploadputletterNonEoffice>>((string)TempData["OutputApi"]);

            //    ViewBag.Output = OutputUploadDocument;

            //    ViewBag.UlTabActiveHome = "";
            //    ViewBag.DivTabActiveHome = "fade";
            //    ViewBag.UlTabActiveUpload = "active";
            //    ViewBag.DivTabActiveUpload = "active show";
            //}


            //List<UploadDocumentOutput> OutputUploadDocument = (List<UploadDocumentOutput>)TempData["OutputApi"];
            OutputletterNonEoffice vald = new OutputletterNonEoffice();
            vald.updtae = 1;
            vald.qrcode = 2;
            ViewBag.Divison = divisionList;
            ViewBag.DeliveryType = deliveryTypeList;
            ViewBag.StatusSend = StatusSendList;
            ViewBag.AllNonLetter = AlldataList;
            ViewBag.UpdateView = vald.updtae;
            ViewBag.QrcodeView = vald.qrcode;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RegisterKurirNonEoffcie(ParamInsertNonEoffice pr)
        {
            try
            {
                //int update = 1;
                //int qrcode = 2;
                var token = HttpContext.Session.GetString("token");
                GeneralOutputModel generalOutput = new GeneralOutputModel();

                generalOutput = await _dataAccessProvider.PostInsertSendNonEofficeAsync("Document/ParamInsertNonEoffice", pr, token);
                var jsonApiResponseSerialize = JsonConvert.SerializeObject(generalOutput.Result);
                var result = pr.delivery_type == 1 ? "Ekspedisi" : "Kurir";

                if (generalOutput.Status == "OK")
                {
                    TempData["status"] = "OK";
                    TempData["title"] = "Berhasil menambahkan surat keluar  non e-office";
                    TempData["pesan"] = generalOutput.Message;
                }
                else if (generalOutput.Status == "UA")
                {
                    return RedirectToAction("Logout", "Home");
                }
                else
                {
                    TempData["status"] = "NG";
                    TempData["title"] = "Gagal menambahkan Suarat Keluar  non e-office";
                    TempData["pesan"] = generalOutput.Message;
                }


                return Redirect("~/GenerateNoDoc/RegisterKurirNonEoffcie");
            }
            catch (Exception ex)
            {
                TempData["status"] = "NG";
                TempData["title"] = "Gagal menambahkan dokumen masuk";
                TempData["pesan"] = ex.ToString();
                return Redirect("~/GenerateNoDoc/RegisterKurirNonEoffcie");
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateKurirLetterNonEoffice(UpdateKurirNonEoffice pr)
        {
            try
            {
                //int update = 1;
                //int qrcode = 2;
                    var token = HttpContext.Session.GetString("token");
                GeneralOutputModel generalOutput = new GeneralOutputModel();

                generalOutput = await _dataAccessProvider.PostUpdateKurirNonEofficeAsync("Document/ParamUpdateKurirNonEofficeWeb", pr, token);
                var jsonApiResponseSerialize = JsonConvert.SerializeObject(generalOutput.Result);


                if (generalOutput.Status == "OK")
                {
                    TempData["status"] = "OK";
                    TempData["title"] = "Berhasil mengubah surat keluar non e-office";
                    TempData["pesan"] = generalOutput.Message;
                }
                else if (generalOutput.Status == "UA")
                {
                    return RedirectToAction("Logout", "Home");
                }
                else
                {
                    TempData["status"] = "NG";
                    TempData["title"] = "Gagal mengubah status keluar  non e-office";
                    TempData["pesan"] = generalOutput.Message;
                }


                return Redirect("~/GenerateNoDoc/RegisterKurirNonEoffcie");
            }
            catch (Exception ex)
            {
                TempData["status"] = "NG";
                TempData["title"] = "Gagal menambahkan dokumen masuk";
                TempData["pesan"] = ex.ToString();
                return Redirect("~/GenerateNoDoc/RegisterKurirNonEoffcie");
            }
        }


        public async Task<IActionResult> ViewQrcodeNonEoffice(ParamInsertGenerateNoDocNonEoffice pr)
        {
            try
            {
                //pr.type = 3;
                pr.userCode = "01";
                var token = HttpContext.Session.GetString("token");
                GeneralOutputModel generalOutput = new GeneralOutputModel();
                getByIdNonEoffice gedatabyID = new getByIdNonEoffice();
                gedatabyID.letter_number = pr.letter_number;
                gedatabyID.idmailingnoneoffice = pr.idmailingnoneoffice;
                generalOutput = await _dataAccessProvider.GeDataByIdNonEoffice("Document/getByIdNonEoffice", token, gedatabyID);
                var jsonApiResponseSerializee = JsonConvert.SerializeObject(generalOutput.Result);
                var DataByID = JsonConvert.DeserializeObject<OutputletterNonEoffice>(jsonApiResponseSerializee);

                var Data = DataByID.qrcodenumber;
                if(DataByID.qrcodenumber == null)
                {
                    int result = DataByID.type_report == 1 ? 3 : 4;
                    pr.type = result;
                    // getDataNomorDocument
                    generalOutput = await _dataAccessProvider.PostInsertGenerateNoDocNonEoffice("Document/QrCodeMallingRoomNonEoffice", pr, token);
                    jsonApiResponseSerializee = JsonConvert.SerializeObject(generalOutput.Result);
                    Data = JsonConvert.DeserializeObject<string>(jsonApiResponseSerializee);
                }
                





                QRCodeModel myQRCode = new QRCodeModel();
                myQRCode.QRCodeText = Data;
                using (QRCodeGenerator qrGenerator = new QRCodeGenerator())
                using (QRCodeData qRCodeData = qrGenerator.CreateQrCode(
                    myQRCode.QRCodeText, QRCodeGenerator.ECCLevel.Q))
                using (QRCode qRCode = new QRCode(qRCodeData))
                {
                    Bitmap qrCodeImage = qRCode.GetGraphic(3);
                    byte[] BitmapArray = qrCodeImage.ConvertBitmapToByteArray();
                    string url = string.Format("data:image/png;base64,{0}", Convert.ToBase64String(BitmapArray));
                    ViewBag.QRCode = url;
                }
                CultureInfo culture = new CultureInfo("en-IN");
                ViewBag.docReceiver = DataByID.docReceiver;
                ViewBag.address = DataByID.address;
                ViewBag.sender_name = DataByID.sender_name;
                ViewBag.unitname = DataByID.unitname;
                ViewBag.ReceiptDate = DataByID.ReceiptDate.ToString("dd-MMMM-yyyy",culture);
                ViewBag.DataQrcode = Data;
                ViewBag.PhoneNumber = DataByID.phonenumber;
                if (generalOutput.Status == "OK")
                {
                    TempData["status"] = "OK";
                    TempData["title"] = "Berhasil Generate Resi";
                    TempData["pesan"] = generalOutput.Message;
                }
                return RedirectToAction("RegisterKurirNonEoffcie", "GenerateNoDoc");
                //return View();
            }
            catch (Exception ex)
            {
                TempData["status"] = "NG";
                TempData["title"] = "Gagal menambahkan dokumen masuk";
                TempData["pesan"] = ex.ToString();
                return Redirect("~/GenerateNoDoc/RegisterKurirNonEoffcie");
            }
        }

        public async Task<IActionResult> ExportExcelKurirNonEoffice(ParamReportNonOuboxLetter pr)
        {
            using (var workbook = new XLWorkbook())
            {

                List<ReportNonOutboxLetterOutput> documentOutput = new List<ReportNonOutboxLetterOutput>();
                GeneralOutputModel generalOutput = new GeneralOutputModel();
                var token = HttpContext.Session.GetString("token");
                generalOutput = await _dataAccessProvider.GetDataReportNonEoffice("NonEofficeLetters/GetDataKurirNonEofficeLetter", token, pr);
                var jsonApiResponseSerializee = JsonConvert.SerializeObject(generalOutput.Result);
                documentOutput = JsonConvert.DeserializeObject<List<ReportNonOutboxLetterOutput>>(jsonApiResponseSerializee);
                var worksheet = workbook.Worksheets.Add("data");
                var curerentRow = 1;
                worksheet.Cell(curerentRow, 1).Value = "Nomor Surat";
                worksheet.Cell(curerentRow, 2).Value = "Tipe Jenis Antaran";
                worksheet.Cell(curerentRow, 3).Value = "Tanggal";
                worksheet.Cell(curerentRow, 4).Value = "Nama Kurir";
                worksheet.Cell(curerentRow, 5).Value = "Nama Pengirim";
                worksheet.Cell(curerentRow, 6).Value = "Division";
                worksheet.Cell(curerentRow, 7).Value = "Nama Tujuan";
                worksheet.Cell(curerentRow, 8).Value = "Alamat Tujuan";
                worksheet.Cell(curerentRow, 9).Value = "Status";
                foreach (var item in documentOutput)
                {
                    curerentRow++;
                    worksheet.Cell(curerentRow, 1).Value = item.letter_number.ToString();
                    worksheet.Cell(curerentRow, 2).Value = item.deliveryname.ToString();
                    worksheet.Cell(curerentRow, 3).Value = item.ReceiptDate == null ? "" : Convert.ToDateTime(item.ReceiptDate).ToString("dd MMM yyyy");
                    worksheet.Cell(curerentRow, 4).Value = item.expedition_name.ToString();
                    worksheet.Cell(curerentRow, 5).Value = item.sender_name.ToString();
                    worksheet.Cell(curerentRow, 6).Value = item.unitname.ToString();
                    worksheet.Cell(curerentRow, 7).Value = item.docReceiver.ToString();
                    worksheet.Cell(curerentRow, 8).Value = item.address.ToString();
                    worksheet.Cell(curerentRow, 9).Value = item.statusname.ToString();

                }
                using (var stream = new MemoryStream())
                {

                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    var data = File(content, "application/vnd.openxmlformats-officedocument.spreadsheethtml.sheet", "NON_E-OFFICE_KURIR_REGISTRATION.xlsx");
                    return (data);
                    return Redirect("~/GenerateNoDoc/RegisterKurirNonEoffcie");
                }

            }

        }

        public async Task<string> GetKurirCopy(string keyword)
        {
            DocumentOutputWeb documentOutput = new DocumentOutputWeb();
            GeneralOutputModel generalOutput = new GeneralOutputModel();
            List<OutputletterNonEoffice> receiverList = new List<OutputletterNonEoffice>();
            var token = HttpContext.Session.GetString("token");

            ParamGetKurir pr = new ParamGetKurir();
            pr.keyword = keyword;

            generalOutput = await _dataAccessProvider.GetDataKurirNameNonEoffice("Document/GetDataKurirNameNonEofficeWeb/", token, pr);
            var jsonApiResponseSerialize = JsonConvert.SerializeObject(generalOutput.Result);
            receiverList = JsonConvert.DeserializeObject<List<OutputletterNonEoffice>>(jsonApiResponseSerialize);
            jsonApiResponseSerialize = JsonConvert.SerializeObject(receiverList);

            return jsonApiResponseSerialize;
        }

        [HttpPost]
        public async Task<string> GetDetailKurirNon(string ids)
        {
            DocumentOutputWeb documentOutput = new DocumentOutputWeb();
            GeneralOutputModel generalOutput = new GeneralOutputModel();
            OutputletterNonEoffice DetailData = new OutputletterNonEoffice();
            var token = HttpContext.Session.GetString("token");

            getByIdNonEoffice pr = new getByIdNonEoffice();
            pr.letter_number = ids;

            generalOutput = await _dataAccessProvider.GetDetailsDataKurirNonEoffice("Document/GetDetailsKurirNonEofficeWeb/", pr, token);
            var jsonApiResponseSerialize = JsonConvert.SerializeObject(generalOutput.Result);
            DetailData = JsonConvert.DeserializeObject<OutputletterNonEoffice>(jsonApiResponseSerialize);
            //var DetailData = JsonConvert.SerializeObject(receiverList);
            var nomrSuart = DetailData.letter_number;
            var NamaPenerima = DetailData.docReceiver;
            var TanggalTerima = DetailData.ReceiptDate.ToString("yyyy-MM-dd");

            var data = "" +
                "<div class='form-group'>" +
                    "<label for='inputpassword3'>&nbsp; Nomor Surat</label>" +
                    "<input type = 'text'  class='form-control' value='"+ nomrSuart + "' name='letter_number' id='letter_number' readonly>" +
                "</div>" +
                "<div class='form-group'>" +
                    "<label for='password1'>&nbsp;Nama Penerima</label>" +
                    "<input type = 'text' class='form-control' value='"+ NamaPenerima +"' id='docReceiver' name='docReceiver' required>" +
                "</div>" +
                "<div class='form-group'>" +
                    "<label for='password1'>&nbsp;Tanggal Penerima</label>" +
                    "<input type = 'text' class='form-control tgl' value='"+ TanggalTerima + "' required id = 'm_datepicker_1' name='startDate' onchange='myChangeFunction2(this)'>" +
                    "<input type='text' hidden class='form-control' id='datestart' name='startDate'>" +
                "</div>";

            return JsonConvert.SerializeObject(data);
            //return jsonApiResponseSerialize;
        }

        #endregion

        public async Task<IActionResult> DetailNotifikasiNonEoffice(Guid id)
        {
            OutputDetailDataNotifNon DetailApproval = new OutputDetailDataNotifNon();
            GeneralOutputModel generalOutput = new GeneralOutputModel();
            var token = HttpContext.Session.GetString("token");
            getByIdNonEoffice prApi = new getByIdNonEoffice();
            prApi.idmailingnoneoffice = id;

            generalOutput = await _dataAccessProvider.DetailLetterNonEofficeNotifikasi("Document/DetailLetterNonEofficeNotifikasiWeb", token, prApi);
            var jsonApiResponseSerialize = JsonConvert.SerializeObject(generalOutput.Result);
            DetailApproval = JsonConvert.DeserializeObject<OutputDetailDataNotifNon>(jsonApiResponseSerialize);

            return View(DetailApproval);
        }


        public async Task<IActionResult> DetailQrcode(Guid id)
        {
            try
            {

                var token = HttpContext.Session.GetString("token");
                GeneralOutputModel generalOutput = new GeneralOutputModel();
                getByIdNonEoffice gedatabyID = new getByIdNonEoffice();
                gedatabyID.idmailingnoneoffice = id;
                generalOutput = await _dataAccessProvider.GeDataByIdNonEoffice("Document/getByIdNonEoffice", token, gedatabyID);
                var jsonApiResponseSerializee = JsonConvert.SerializeObject(generalOutput.Result);
                var DataByID = JsonConvert.DeserializeObject<OutputletterNonEoffice>(jsonApiResponseSerializee);



                var Data = DataByID.qrcodenumber;
                CultureInfo culture = new CultureInfo("en-IN");
                string filename = "5x10cm";
                string filePath = Path.Combine("wwwroot\\qrcode_barcode\\" + filename + ".JPG");
                string filettd = Path.GetFileName(filePath);
                string pathttd = $"{this.Request.Scheme}://{this.Request.Host}{this.Request.PathBase}" + "/qrcode_barcode/" + filettd;

                string logoPath = "wwwroot\\qrcode_barcode\\logobnil.JPG";
                Bitmap logoImage = new Bitmap(logoPath);

                // Atur posisi dan ukuran logo
                int logoX = 1035;
                int logoY = 5;
                int logoWidth = 150;
                int logoHeight = 68;
                Bitmap resizedLogoImage = new Bitmap(logoImage, new Size(logoWidth, logoHeight));


                QRCodeModel myQRCode = new QRCodeModel();
                myQRCode.QRCodeText = Data;

                using (QRCodeGenerator qrGenerator = new QRCodeGenerator())
                using (QRCodeData qRCodeData = qrGenerator.CreateQrCode(myQRCode.QRCodeText, QRCodeGenerator.ECCLevel.Q))
                using (QRCode qRCode = new QRCode(qRCodeData))
                {
                    Bitmap qrCodeImage = qRCode.GetGraphic(4);

                    using (HttpClient httpClient = new HttpClient())
                    {
                        HttpResponseMessage response = await httpClient.GetAsync(pathttd);

                        if (response.IsSuccessStatusCode)
                        {
                            Stream imageStream = await response.Content.ReadAsStreamAsync();
                            //int dpi = 195; // Resolusi dalam DPI (dots per inch)

                            using (Bitmap templateImage = new Bitmap(imageStream))
                            {
                                int alamatBniLife1 = 60;
                                int alamatBniLife2 = 60;
                                int alamatBniLife3 = 60;
                                int alamatBniLife4 = 60;
                                int alamatBniLife5 = 60;
                                int BniLife7 = 63;





                                int xOffset = 15;
                                int qrWidth = 410;

                                int maxWidth = 500; // Ganti dengan lebar maksimum yang Anda inginkan dalam piksel
                                int maxWidthAlamat = 500; // Ganti dengan lebar maksimum yang Anda inginkan dalam piksel
                                int maxWidthNamaTujuan = 500; // Ganti dengan lebar maksimum yang Anda inginkan dalam piksel
                                int maxWidthNamaPengirim = 375; // Ganti dengan lebar maksimum yang Anda inginkan dalam piksel
                                                                //templateImage.SetResolution(dpi, dpi);
                                float targetDpi1 = 1850;  // Resolusi yang diinginkan untuk textalamatBniLife1
                                float targetDpi2 = 100;  // Resolusi yang diinginkan untuk textalamatBniLife2

                                using (Graphics graphics = Graphics.FromImage(templateImage))
                                {
                                    //float scale1 = targetDpi1 / graphics.DpiX;
                                    //float scale2 = targetDpi2 / graphics.DpiX;
                                    // Menggeser gambar QR code ke kanan
                                    int qrXWidth = 250;
                                    int qrYHeight = 210;
                                    int qrX = 960;
                                    int qrY = 70;
                                    // Menggambar gambar QR code di sisi kanan gambar template
                                    graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                                    graphics.DrawImage(qrCodeImage, new System.Drawing.Rectangle(qrX, qrY, qrXWidth, qrYHeight));



                                    graphics.DrawImage(resizedLogoImage, new System.Drawing.Rectangle(logoX, logoY, logoWidth, logoHeight));


                                    string textalamatBniLife1 = "PT BNI Life Insurance";
                                    string textalamatBniLife2 = "Centennial Tower Lt.9";
                                    string textalamatBniLife3 = "Jalan Jendral Gatot Subroto";
                                    string textalamatBniLife4 = "Phone: 021 -2953 9999";
                                    string textalamatBniLife5 = "Fax: 021 -2963 8998";
                                    string textalamatBniLife6 = "Kav. 24-25, Jakarta 12930";

                                    string textNamaTujuan = "Penerima:";
                                    string textNamaTelepon = "Telp:";
                                    string textNamaAlamatTujuan = "Alamat:";

                                    string textNamaPengirim = "Nama Pengirim:";
                                    string textNamaDivisi = "Divisi:";

                                    string textttdPenerima = "Nama & tanda tangan penerima";



                                    string textToNamatujuan = DataByID.docReceiver;
                                    string textToTelepon = DataByID.phonenumber;
                                    //string textToInsertAlamat = DataByID.address.ToLower();
                                    string textToInsertAlamat = DataByID.address;
                                    string textToNamapengirim = DataByID.sender_name;
                                    string textToDivisi = DataByID.unitname;
                                    string textToReceiptDate = DataByID.ReceiptDate.ToString("dd-MMMM-yyyy", culture);
                                    string textToNomor = DataByID.qrcodenumber;


                                    // Load the private font collection to ensure smooth rendering
                                    PrivateFontCollection privateFonts = new PrivateFontCollection();
                                    privateFonts.AddFontFile("wwwroot\\fonts\\Arial.ttf"); // Replace with the actual path to your font file

                                    // Define font sizes and styles
                                    float fontSizeAlamatBniLife = 8f; // Font size for alamatBniLife
                                    float fontSizeStandard = 8f; // Standard font size
                                    FontStyle fontStyleBold = FontStyle.Bold;


                                    string[] words = textToInsertAlamat.Split(' ');
                                    string[] wordsNamaTujuan = textToNamatujuan.Split(' ');
                                    string[] wordsNamaPengirim = textToNamapengirim.Split(' ');
                                    string[] wordsNamaDivisi = textToDivisi.Split(' ');
                                    //System.Drawing.Font textFontAlamatBniLife = new System.Drawing.Font("Arial", 32, System.Drawing.FontStyle.Bold, GraphicsUnit.World);
                                    //System.Drawing.Font textFont = new System.Drawing.Font("Arial", 8, System.Drawing.FontStyle.Bold);
                                    //System.Drawing.Font textFontAlamat = new System.Drawing.Font("Arial", 20, System.Drawing.FontStyle.Bold, GraphicsUnit.World);
                                    System.Drawing.Font textFontAlamatBniLife = new System.Drawing.Font(privateFonts.Families[0], fontSizeAlamatBniLife, fontStyleBold, GraphicsUnit.Point);
                                    System.Drawing.Font textFont = new System.Drawing.Font(privateFonts.Families[0], fontSizeStandard, fontStyleBold, GraphicsUnit.Point);
                                    System.Drawing.Font textFontAlamat = new System.Drawing.Font(privateFonts.Families[0], fontSizeAlamatBniLife, fontStyleBold, GraphicsUnit.Point);
                                    System.Drawing.Font textFontttdpenerima = new System.Drawing.Font(privateFonts.Families[0], 7f, fontStyleBold, GraphicsUnit.Point);
                                    System.Drawing.Font textFontQrcodeNumber = new System.Drawing.Font(privateFonts.Families[0], fontSizeStandard, fontStyleBold, GraphicsUnit.Point);
                                    //System.Drawing.Font textFontQrcodeNumber = new System.Drawing.Font("Arial", 8, System.Drawing.FontStyle.Bold);

                                    SolidBrush textBrush = new SolidBrush(System.Drawing.Color.Black);
                                    SizeF textSizeAlamatBniLife1 = graphics.MeasureString(textalamatBniLife1, textFontAlamatBniLife);
                                    SizeF textSizeAlamatBniLife2 = graphics.MeasureString(textalamatBniLife2, textFontAlamatBniLife);
                                    SizeF textSizeAlamatBniLife3 = graphics.MeasureString(textalamatBniLife3, textFontAlamatBniLife);
                                    SizeF textSizeAlamatBniLife4 = graphics.MeasureString(textalamatBniLife4, textFontAlamatBniLife);
                                    SizeF textSizeAlamatBniLife5 = graphics.MeasureString(textalamatBniLife5, textFontAlamatBniLife);
                                    SizeF textSizeAlamatBniLife6 = graphics.MeasureString(textalamatBniLife6, textFontAlamatBniLife);




                                    SizeF textSizeBniLife7 = graphics.MeasureString(textNamaTujuan, textFontAlamatBniLife);
                                    SizeF textSizeBniLife8 = graphics.MeasureString(textNamaTelepon, textFontAlamatBniLife);
                                    SizeF textSizeBniLife9 = graphics.MeasureString(textNamaAlamatTujuan, textFontAlamatBniLife);
                                    SizeF textSizeBniLife10 = graphics.MeasureString(textNamaPengirim, textFontAlamatBniLife);
                                    SizeF textSizeBniLife11 = graphics.MeasureString(textNamaDivisi, textFontAlamatBniLife);


                                    SizeF textSizeBniNamatujuan = graphics.MeasureString(textToNamatujuan, textFont);
                                    SizeF textSizeBniTelp = graphics.MeasureString(textToTelepon, textFont);

                                    int alamatBniLife1X = alamatBniLife1;
                                    float alamatBniLife1Y = 4;

                                    int alamatBniLife2X = alamatBniLife2;
                                    float alamatBniLife2Y = 37;
                                    //alamatBniLife1Y + textSizeAlamatBniLife1.Height;  // 2 piksel jarak antar teks

                                    int alamatBniLife3X = alamatBniLife3;
                                    float alamatBniLife3Y = 68;
                                    //alamatBniLife2Y + textSizeAlamatBniLife3.Height;

                                    int alamatBniLife6X = alamatBniLife3;
                                    float alamatBniLife6Y = 97;
                                    //alamatBniLife3Y + textSizeAlamatBniLife6.Height;


                                    int alamatBniLife4X = alamatBniLife4;
                                    float alamatBniLife4Y = 128;
                                    //alamatBniLife6Y + textSizeAlamatBniLife3.Height;

                                    int alamatBniLife5X = alamatBniLife5;
                                    float alamatBniLife5Y = 155;
                                    //alamatBniLife4Y + textSizeAlamatBniLife4.Height + 10;


                                    int alamatBniLife7X = BniLife7;
                                    float alamatBniLife7Y = 170;
                                    int alamatBniLife8X = BniLife7;
                                    float alamatBniLife8Y = 235;
                                    int alamatBniLife9X = BniLife7;
                                    float alamatBniLife9Y = 270;

                                    int alamatBniLife10X = BniLife7;
                                    float alamatBniLife10Y = 458;

                                    int alamatBniLife11X = BniLife7;
                                    float alamatBniLife11Y = 520;


                                    float namatujuanX = 210f;
                                    float namatujuanY = 170f;

                                    float telepontujuanX = 145f;
                                    float telepontujuanY = 240f;
                                    float teleponWidth = 220;
                                    float teleponHeight = 40;

                                    //int alamattujuanX = alamattujuan;
                                    int alamattujuanY = 271;

                                    //int namapengirimX = 250;
                                    float namapengirimY = 458f;

                                    //int divisiX = 125;
                                    float divisiY = 520f;

                                    float ttdpenerimaX = 737f;
                                    float ttdpenerimaY = 520f;

                                    int receiptdateX = 875;
                                    int receiptdateY = 310;

                                    float receiptdateWidth = 500;
                                    float receiptdateHeight = 40;

                                    int nomorX = 775;
                                    int nomorY = 268;
                                    float nomoreWidth = 500;
                                    float nomorXHeight = 40;


                                    #region nama tujuan
                                    StringBuilder linesNamaTujuans = new StringBuilder();
                                    List<string> lineNamaTujuans = new List<string>();
                                    int lineHeightNamaTujuan = 20; // Mendapatkan tinggi teks
                                    int nn = 0;
                                    foreach (string word in wordsNamaTujuan)
                                    {
                                        string cleanWord = word.Trim();

                                        // Membuat baris uji dengan kata tambahan
                                        string testLine = linesNamaTujuans.ToString() + " " + cleanWord;

                                        // Menghitung lebar teks dalam piksel
                                        SizeF size = graphics.MeasureString(testLine, textFont);
                                        double ifsize = 203.5586;
                                        if (size.Width <= maxWidthNamaTujuan)
                                        {
                                            // Jika lebar masih sesuai dengan maxWidth, tambahkan kata ke baris
                                            linesNamaTujuans.Append(" " + word);
                                            if (size.Width == ifsize)
                                            {
                                                linesNamaTujuans.Clear().Append(word);
                                            }
                                            nn++;
                                        }
                                        else
                                        {
                                            // Jika lebar melebihi maxWidth, gambar baris teks sebelumnya
                                            graphics.DrawString(linesNamaTujuans.ToString(), textFont, textBrush, new PointF(215, namatujuanY));

                                            // Pindah ke baris berikutnya
                                            namatujuanY += (int)textFont.GetHeight();
                                            namatujuanY += lineHeightNamaTujuan;

                                            // Mulai baris baru dengan kata ini
                                            linesNamaTujuans.Clear().Append(word);

                                        }
                                    }

                                    // Tambahkan baris terakhir
                                    lineNamaTujuans.Add(linesNamaTujuans.ToString());

                                    // Gambar setiap baris
                                    float ynamaTujuan = namatujuanY;
                                    // Mengukur lebar teks dari baris pertama
                                    float textWidthFirstLineTj = graphics.MeasureString(lineNamaTujuans[0], textFont).Width;
                                    int someIntTj = (int)textWidthFirstLineTj;
                                    if (nn == 1)
                                    {
                                        graphics.DrawString(lineNamaTujuans[0], textFont, textBrush, new PointF(215, ynamaTujuan));
                                    }
                                    
                                    else if (someIntTj <= 297)
                                    {
                                        if (someIntTj == 206)
                                        {
                                            graphics.DrawString(lineNamaTujuans[0], textFont, textBrush, new PointF(61, ynamaTujuan));
                                        }
                                        else if (someIntTj == 236)
                                        {
                                            graphics.DrawString(lineNamaTujuans[0], textFont, textBrush, new PointF(215, ynamaTujuan));
                                        }
                                        else if (someIntTj == 247)
                                        {
                                            graphics.DrawString(lineNamaTujuans[0], textFont, textBrush, new PointF(215, ynamaTujuan));
                                        }
                                        else if (someIntTj <= 115)
                                        {

                                            graphics.DrawString(lineNamaTujuans[0], textFont, textBrush, new PointF(61, ynamaTujuan));
                                        }
                                        else if (someIntTj == 187 || someIntTj == 188 || someIntTj == 210)
                                        {

                                            graphics.DrawString(lineNamaTujuans[0], textFont, textBrush, new PointF(61, ynamaTujuan));
                                        }
                                        else
                                        {
                                            graphics.DrawString(lineNamaTujuans[0], textFont, textBrush, new PointF(61, ynamaTujuan));
                                        }
                                        
                                    }
                                    else if (someIntTj == 304)
                                    {
                                        graphics.DrawString(lineNamaTujuans[0], textFont, textBrush, new PointF(215, ynamaTujuan));
                                    }
                                    else if (someIntTj == 329)
                                    {
                                        graphics.DrawString(lineNamaTujuans[0], textFont, textBrush, new PointF(61, ynamaTujuan));
                                    }
                                    else if (someIntTj == 335)
                                    {
                                        graphics.DrawString(lineNamaTujuans[0], textFont, textBrush, new PointF(215, ynamaTujuan));

                                    }
                                    else if (someIntTj == 348)
                                    {
                                        graphics.DrawString(lineNamaTujuans[0], textFont, textBrush, new PointF(215, ynamaTujuan));

                                    }
                                    else if (someIntTj == 361)
                                    {
                                        graphics.DrawString(lineNamaTujuans[0], textFont, textBrush, new PointF(215, ynamaTujuan));

                                    }
                                    else if (someIntTj == 397)
                                    {
                                        graphics.DrawString(lineNamaTujuans[0], textFont, textBrush, new PointF(215, ynamaTujuan));
                                    }
                                    else if(someIntTj == 454)
                                    {
                                        graphics.DrawString(lineNamaTujuans[0], textFont, textBrush, new PointF(215, ynamaTujuan));
                                    }
                                    else if (someIntTj == 461)
                                    {
                                        graphics.DrawString(lineNamaTujuans[0], textFont, textBrush, new PointF(215, ynamaTujuan));
                                    }
                                    
                                    else if (someIntTj == 471)
                                    {
                                        graphics.DrawString(lineNamaTujuans[0], textFont, textBrush, new PointF(215, ynamaTujuan));
                                    }

                                    else if (someIntTj == 478)
                                    {
                                        graphics.DrawString(lineNamaTujuans[0], textFont, textBrush, new PointF(215, ynamaTujuan));
                                    }
                                    else if (someIntTj == 482)
                                    {
                                        graphics.DrawString(lineNamaTujuans[0], textFont, textBrush, new PointF(215, ynamaTujuan));
                                    }
                                    else if (someIntTj == 497)
                                    {
                                        graphics.DrawString(lineNamaTujuans[0], textFont, textBrush, new PointF(215, ynamaTujuan));
                                    }
                                    else if (someIntTj == 445)
                                    {
                                        graphics.DrawString(lineNamaTujuans[0], textFont, textBrush, new PointF(215, ynamaTujuan));
                                    }
                                    else
                                    {
                                        graphics.DrawString(lineNamaTujuans[0], textFont, textBrush, new PointF(215, ynamaTujuan));

                                    }


                                    // Menggambar teks dari baris pertama


                                    // Iterasi melalui setiap baris teks mulai dari baris kedua
                                    for (int i = 1; i < lineNamaTujuans.Count; i++)
                                    {
                                        // Mengukur lebar teks dari baris saat ini
                                        float textWidth = graphics.MeasureString(lineNamaTujuans[i], textFont).Width;

                                        // Menghitung posisi X untuk memastikan sejajar dengan alamatBniLife7X
                                        float adjustedX = 200  + (textWidthFirstLineTj - textWidth);

                                        // Menggambar teks dengan posisi yang diinginkan
                                        graphics.DrawString(lineNamaTujuans[i], textFont, textBrush, new PointF(adjustedX, ynamaTujuan));

                                        // Tambahkan jarak antar baris
                                        ynamaTujuan += lineHeightNamaTujuan;
                                    }


                                    #endregion



                                    #region Almaat tujuan

                                    StringBuilder line = new StringBuilder();
                                    List<string> lineList = new List<string>();
                                    int lineHeight = 20; // Mendapatkan tinggi teks
                                    int ii = 0;
                                    foreach (string word in words)
                                    {
                                        string cleanWord = word.Trim();

                                        // Membuat baris uji dengan kata tambahan
                                        string testLine = line.ToString() + " " + cleanWord;

                                        // Menghitung lebar teks dalam piksel
                                        SizeF size = graphics.MeasureString(testLine, textFontAlamat);

                                        if (size.Width <= maxWidthAlamat)
                                        {
                                            // Jika lebar masih sesuai dengan maxWidth, tambahkan kata ke baris
                                            line.Append(" " + word);
                                        }
                                        else
                                        {
                                            if (ii == 0)
                                            {
                                                graphics.DrawString(line.ToString(), textFontAlamat, textBrush, new PointF(180, alamattujuanY));
                                            }
                                            else
                                            {
                                                graphics.DrawString(line.ToString(), textFontAlamat, textBrush, new PointF(62, alamattujuanY));
                                            }
                                            // Jika lebar melebihi maxWidth, gambar baris teks sebelumnya


                                            // Pindah ke baris berikutnya
                                            alamattujuanY += (int)textFont.GetHeight();
                                            alamattujuanY += lineHeight;
                                            // Mulai baris baru dengan kata ini
                                            line.Clear().Append(word);
                                            ii++;
                                        }
                                    }

                                    // Tambahkan baris terakhir
                                    lineList.Add(line.ToString());

                                    // Gambar setiap baris
                                    float y = alamattujuanY;

                                    float textWidthFirstLineAlamat = graphics.MeasureString(lineList[0], textFontAlamat).Width;
                                    int someIntAlt = (int)textWidthFirstLineAlamat;

                                    if (someIntAlt <= 240)
                                    {
                                        if (someIntAlt == 50)
                                        {
                                            graphics.DrawString(lineList[0], textFontAlamat, textBrush, new PointF(62, y));
                                        }

                                        else if(someIntAlt == 150)
                                        {
                                            graphics.DrawString(lineList[0], textFontAlamat, textBrush, new PointF(62, y));
                                        }
                                        else if (someIntAlt == 234)
                                        {
                                            graphics.DrawString(lineList[0], textFontAlamat, textBrush, new PointF(62, y));
                                        }
                                        else if(someIntAlt == 236)
                                        {
                                            graphics.DrawString(lineList[0], textFontAlamat, textBrush, new PointF(62, y));
                                        }
                                        else if (someIntAlt == 132)
                                        {
                                            graphics.DrawString(lineList[0], textFontAlamat, textBrush, new PointF(62, y));
                                        }
                                        else if (someIntAlt == 109)
                                        {
                                            graphics.DrawString(lineList[0], textFontAlamat, textBrush, new PointF(62, y));
                                        }
                                        else if (someIntAlt == 138)
                                        {
                                            graphics.DrawString(lineList[0], textFontAlamat, textBrush, new PointF(62, y));
                                        }
                                        else if (someIntAlt == 102)
                                        {
                                            graphics.DrawString(lineList[0], textFontAlamat, textBrush, new PointF(62, y));
                                        }
                                        else if (someIntAlt == 106)
                                        {
                                            graphics.DrawString(lineList[0], textFontAlamat, textBrush, new PointF(62, y));
                                        }
                                        else if (someIntAlt == 187)
                                        {
                                            graphics.DrawString(lineList[0], textFontAlamat, textBrush, new PointF(62, y));
                                        }
                                        else if (someIntAlt == 180)
                                        {
                                            graphics.DrawString(lineList[0], textFontAlamat, textBrush, new PointF(62, y));
                                        }
                                        else
                                        {
                                            //graphics.DrawString(lineList[0], textFont, textBrush, new PointF(180, y));
                                            graphics.DrawString(lineList[0], textFontAlamat, textBrush, new PointF(62, y));
                                        }
                                        

                                    }
                                    //326,252,483(180)
                                    else if (someIntAlt == 252)
                                    {
                                        graphics.DrawString(lineList[0], textFontAlamat, textBrush, new PointF(62, y));
                                    }
                                    else if (someIntAlt == 326)
                                    {
                                        graphics.DrawString(lineList[0], textFontAlamat, textBrush, new PointF(62, y));
                                    }
                                    else if (someIntAlt == 363)
                                    {
                                        graphics.DrawString(lineList[0], textFontAlamat, textBrush, new PointF(62, y));
                                    }
                                    else if (someIntAlt == 374)
                                    {
                                        graphics.DrawString(lineList[0], textFontAlamat, textBrush, new PointF(62, y));
                                    }
                                    else if (someIntAlt == 526)
                                    {
                                        graphics.DrawString(lineList[0], textFontAlamat, textBrush, new PointF(62, y));
                                    }
                                    else if(someIntAlt == 461)
                                    {
                                        graphics.DrawString(lineList[0], textFontAlamat, textBrush, new PointF(180, y));
                                    }
                                    else if (someIntAlt == 389)
                                    {
                                        graphics.DrawString(lineList[0], textFontAlamat, textBrush, new PointF(62, y));
                                    }
                                    else if (someIntAlt == 373)
                                    {
                                        graphics.DrawString(lineList[0], textFontAlamat, textBrush, new PointF(62, y));
                                    }
                                    else if (someIntAlt == 393)
                                    {
                                        graphics.DrawString(lineList[0], textFontAlamat, textBrush, new PointF(180, y));
                                    }
                                    else if (someIntAlt == 395)
                                    {
                                        graphics.DrawString(lineList[0], textFontAlamat, textBrush, new PointF(180, y));
                                    }
                                    else if (someIntAlt == 482)
                                    {
                                        graphics.DrawString(lineList[0], textFontAlamat, textBrush, new PointF(62, y));
                                    }
                                    else if (someIntAlt == 483)
                                    {
                                        graphics.DrawString(lineList[0], textFontAlamat, textBrush, new PointF(180, y));
                                    }

                                    else if (someIntAlt == 492)
                                    {
                                        graphics.DrawString(lineList[0], textFontAlamat, textBrush, new PointF(62, y));
                                    }
                                    else
                                    {
                                        graphics.DrawString(lineList[0], textFontAlamat, textBrush, new PointF(62, y));
                                    }
                                    // Menggambar teks dari baris pertama
                                    //graphics.DrawString(lineList[0], textFont, textBrush, new PointF(35, y));

                                    // Iterasi melalui setiap baris teks mulai dari baris kedua
                                    for (int i = 1; i < lineList.Count; i++)
                                    {
                                        // Mengukur lebar teks dari baris saat ini
                                        float textWidth = graphics.MeasureString(lineList[i], textFontAlamat).Width;

                                        // Menghitung posisi X untuk memastikan sejajar dengan alamatBniLife7X
                                        float adjustedX = alamatBniLife7X + (textWidthFirstLineAlamat - textWidth);

                                        // Menggambar teks dengan posisi yang diinginkan
                                        graphics.DrawString(lineList[i], textFontAlamat, textBrush, new PointF(adjustedX, y));

                                        // Tambahkan jarak antar baris
                                        y += lineHeight;
                                    }

                                    #endregion


                                    #region Nama Pengirim


                                    StringBuilder linespengirim = new StringBuilder();
                                    List<string> listpengirim = new List<string>();
                                    int lineHeightpengirim = 20; // Mendapatkan tinggi teks
                                    int np = 0;
                                    foreach (string word in wordsNamaPengirim)
                                    {
                                        string cleanWord = word.Trim();

                                        // Membuat baris uji dengan kata tambahan
                                        string testLine = linespengirim.ToString() + " " + cleanWord;

                                        // Menghitung lebar teks dalam piksel
                                        SizeF size = graphics.MeasureString(testLine, textFont);

                                        if (size.Width <= maxWidthNamaTujuan)
                                        {
                                            // Jika lebar masih sesuai dengan maxWidth, tambahkan kata ke baris
                                            linespengirim.Append(" " + word);
                                            np++;
                                        }
                                        else
                                        {

                                            // Jika lebar melebihi maxWidth, gambar baris teks sebelumnya
                                            graphics.DrawString(linespengirim.ToString(), textFont, textBrush, new PointF(305, namapengirimY));

                                            // Pindah ke baris berikutnya
                                            namapengirimY += (int)textFont.GetHeight();
                                            namapengirimY += lineHeightNamaTujuan;

                                            // Mulai baris baru dengan kata ini
                                            linespengirim.Clear().Append(word);
                                            np++;
                                        }
                                    }

                                    // Tambahkan baris terakhir
                                    listpengirim.Add(linespengirim.ToString());

                                    // Gambar setiap baris
                                    float yp = namapengirimY;
                                    // Mengukur lebar teks dari baris pertama
                                    float textWidthFirstLinenp = graphics.MeasureString(listpengirim[0], textFont).Width;
                                    int someInt = (int)textWidthFirstLinenp;
                                    int thresholdValue = 300 - 11;
                                    int thresholdValueTw = 400 - 100;

                                    if (np == 1 || np == 2)
                                    {
                                        graphics.DrawString(listpengirim[0], textFont, textBrush, new PointF(305, yp));
                                    }
                                    else if (np == 3 || np == 4)
                                    {
                                        if (someInt != thresholdValue)
                                        {
                                            if (someInt > thresholdValueTw)
                                            {
                                                if (someInt == 316)
                                                {
                                                    graphics.DrawString(listpengirim[0], textFont, textBrush, new PointF(61, yp));
                                                }
                                                else
                                                {
                                                    graphics.DrawString(listpengirim[0], textFont, textBrush, new PointF(305, yp));
                                                }
                                            }
                                            
                                            else
                                            {
                                                graphics.DrawString(listpengirim[0], textFont, textBrush, new PointF(61, yp));
                                            }

                                        }
                                        else
                                        {
                                            graphics.DrawString(listpengirim[0], textFont, textBrush, new PointF(305, yp));
                                        }
                                    }
                                    else
                                    {
                                        graphics.DrawString(listpengirim[0], textFont, textBrush, new PointF(61, yp));
                                    }
                                    // Menggambar teks dari baris pertama


                                    // Iterasi melalui setiap baris teks mulai dari baris kedua
                                    for (int i = 1; i < listpengirim.Count; i++)
                                    {
                                        // Mengukur lebar teks dari baris saat ini
                                        float textWidth = graphics.MeasureString(listpengirim[i], textFont).Width;

                                        // Menghitung posisi X untuk memastikan sejajar dengan alamatBniLife7X
                                        float adjustedX = 200  + (textWidthFirstLinenp - textWidth);

                                        // Menggambar teks dengan posisi yang diinginkan
                                        graphics.DrawString(listpengirim[i], textFont, textBrush, new PointF(adjustedX, yp));

                                        // Tambahkan jarak antar baris
                                        yp += lineHeightpengirim;
                                    }



                                    #endregion



                                    #region Divisii
                                    StringBuilder linesDivsi = new StringBuilder();
                                    List<string> listDivisi = new List<string>();
                                    int lineHeightdivisi = 20; // Mendapatkan tinggi teks
                                    int npd = 0;
                                    foreach (string word in wordsNamaDivisi)
                                    {
                                        string cleanWord = word.Trim();

                                        // Membuat baris uji dengan kata tambahan
                                        string testLine = linesDivsi.ToString() + " " + cleanWord;

                                        // Menghitung lebar teks dalam piksel
                                        SizeF size = graphics.MeasureString(testLine, textFont);

                                        if (size.Width <= maxWidthNamaTujuan)
                                        {
                                            // Jika lebar masih sesuai dengan maxWidth, tambahkan kata ke baris
                                            linesDivsi.Append(" " + word);
                                            npd++;
                                        }
                                        else
                                        {
                                            // Jika lebar melebihi maxWidth, gambar baris teks sebelumnya
                                            graphics.DrawString(linesDivsi.ToString(), textFont, textBrush, new PointF(151, divisiY));

                                            // Pindah ke baris berikutnya
                                            divisiY += (int)textFont.GetHeight();
                                            divisiY += lineHeightdivisi;

                                            // Mulai baris baru dengan kata ini
                                            linesDivsi.Clear().Append(word);
                                            npd++;
                                        }
                                    }

                                    // Tambahkan baris terakhir
                                    listDivisi.Add(linesDivsi.ToString());

                                    // Gambar setiap baris
                                    float yd = divisiY;
                                    // Mengukur lebar teks dari baris pertama
                                    float textWidthFirstLinendiv = graphics.MeasureString(listDivisi[0], textFont).Width;
                                    int someIntdiv = (int)textWidthFirstLinendiv;
                                    int thresholdValuediv = 300 - 103;
                                    int thresholdValueTwdiv = 400 - 100;
                                    if (npd == 1 || npd == 2)
                                    {
                                        graphics.DrawString(listDivisi[0], textFont, textBrush, new PointF(151, yd));
                                    }
                                    else if (npd == 3 || npd == 4)
                                    {
                                        if (thresholdValuediv >= someIntdiv)
                                        {
                                            graphics.DrawString(listDivisi[0], textFont, textBrush, new PointF(60, yd));
                                        }
                                        else if(someIntdiv == 199)
                                        {
                                            graphics.DrawString(listDivisi[0], textFont, textBrush, new PointF(60, yd));
                                        }
                                        else
                                        {
                                            graphics.DrawString(listDivisi[0], textFont, textBrush, new PointF(151, yd));
                                        }
                                    }
                                    else
                                    {
                                        graphics.DrawString(listDivisi[0], textFont, textBrush, new PointF(60, yd));
                                    }
                                    // Menggambar teks dari baris pertama


                                    // Iterasi melalui setiap baris teks mulai dari baris kedua
                                    for (int i = 1; i < listDivisi.Count; i++)
                                    {
                                        // Mengukur lebar teks dari baris saat ini
                                        float textWidth = graphics.MeasureString(listDivisi[i], textFont).Width;

                                        // Menghitung posisi X untuk memastikan sejajar dengan alamatBniLife7X
                                        float adjustedX = 200  + (textWidthFirstLinendiv - textWidth);

                                        // Menggambar teks dengan posisi yang diinginkan
                                        graphics.DrawString(listDivisi[i], textFont, textBrush, new PointF(adjustedX, yd));

                                        // Tambahkan jarak antar baris
                                        yd += lineHeightdivisi;
                                    }



                                    #endregion




                                    StringFormat stringFormat = new StringFormat();
                                    stringFormat.FormatFlags = StringFormatFlags.NoWrap;

                                    graphics.DrawString(textalamatBniLife1, textFontAlamatBniLife, textBrush, new PointF(alamatBniLife1X, alamatBniLife1Y));
                                    graphics.DrawString(textalamatBniLife2, textFontAlamatBniLife, textBrush, new PointF(alamatBniLife2X, alamatBniLife2Y));
                                    graphics.DrawString(textalamatBniLife6, textFontAlamatBniLife, textBrush, new PointF(alamatBniLife6X, alamatBniLife6Y));
                                    graphics.DrawString(textalamatBniLife3, textFontAlamatBniLife, textBrush, new PointF(alamatBniLife3X, alamatBniLife3Y));
                                    //graphics.DrawString(textalamatBniLife4, textFontAlamatBniLife, textBrush, new PointF(alamatBniLife4X, alamatBniLife4Y));
                                    //graphics.DrawString(textalamatBniLife5, textFontAlamatBniLife, textBrush, new PointF(alamatBniLife5X, alamatBniLife5Y));


                                    graphics.DrawString(textNamaTujuan, textFontAlamatBniLife, textBrush, new PointF(alamatBniLife7X, alamatBniLife7Y));
                                    graphics.DrawString(textNamaTelepon, textFontAlamatBniLife, textBrush, new PointF(alamatBniLife8X, alamatBniLife8Y));
                                    graphics.DrawString(textNamaAlamatTujuan, textFontAlamatBniLife, textBrush, new PointF(alamatBniLife9X, alamatBniLife9Y));

                                    graphics.DrawString(textNamaPengirim, textFontAlamatBniLife, textBrush, new PointF(alamatBniLife10X, alamatBniLife10Y));
                                    graphics.DrawString(textNamaDivisi, textFontAlamatBniLife, textBrush, new PointF(alamatBniLife11X, alamatBniLife11Y));


                                    graphics.DrawString(textToTelepon, textFontAlamat, textBrush, new RectangleF(telepontujuanX, telepontujuanY, teleponWidth, teleponHeight), stringFormat);

                                    graphics.DrawString(textttdPenerima, textFontttdpenerima, textBrush, new RectangleF(ttdpenerimaX, ttdpenerimaY, ttdpenerimaX, ttdpenerimaY), stringFormat);

                                    graphics.DrawString(textToReceiptDate, textFontQrcodeNumber, textBrush, new RectangleF(receiptdateX, receiptdateY, receiptdateWidth, receiptdateHeight), stringFormat);

                                    graphics.DrawString(textToNomor, textFontQrcodeNumber, textBrush, new RectangleF(nomorX, nomorY, nomoreWidth, nomorXHeight), stringFormat);
                                    //graphics.DrawString(textToInsert, textFont, textBrush, new RectangleF(namatujuanX, namatujuanY, namatujuanX, namatujuanY), stringFormat);





                                    // Tampilkan gambar yang telah digabungkan
                                    byte[] templateImageBytes;
                                    using (MemoryStream stream = new MemoryStream())
                                    {
                                        templateImage.Save(stream, ImageFormat.Jpeg);
                                        //templateImageBytes = stream.ToArray();
                                        //templateImage.Save(stream, ImageFormat.Jpeg);

                                        // Set tipe konten respons
                                        Response.ContentType = "image/jpeg";

                                        // Mengatur nama file dan memaksa respons untuk mengunduh
                                        Response.Headers.Add("Content-Disposition", "attachment; filename="+DataByID.qrcodenumber+".JPG");

                                        // Mengirimkan gambar sebagai respons
                                        stream.Position = 0;
                                        await stream.CopyToAsync(Response.Body);
                                    }


                                }
                            }
                        }
                    }
                }

                return new EmptyResult();
                //return View();
            }
            catch (Exception ex)
            {
                TempData["status"] = "NG";
                TempData["title"] = "Gagal menambahkan dokumen masuk";
                TempData["pesan"] = ex.ToString();
                return Redirect("~/GenerateNoDoc/RegisterKurirNonEoffcie");
            }
        }


        public async Task<IActionResult> DetailQrcodePdf(string id)
        {
            try
            {
                var token = HttpContext.Session.GetString("token");
                GeneralOutputModel generalOutput = new GeneralOutputModel();
                getByIdNonEoffice gedatabyID = new getByIdNonEoffice();
                gedatabyID.letter_number = id;
                generalOutput = await _dataAccessProvider.GeDataByIdNonEoffice("Document/getByIdNonEoffice", token, gedatabyID);
                var jsonApiResponseSerializee = JsonConvert.SerializeObject(generalOutput.Result);
                var DataByID = JsonConvert.DeserializeObject<OutputletterNonEoffice>(jsonApiResponseSerializee);
                var Data = DataByID.qrcodenumber;
                // Create a new document with custom page size
                Document doc = new Document(new Rectangle(100f, 50f), 0, 0, 0, 0);
                MemoryStream ms = new MemoryStream();
                PdfWriter writer = PdfWriter.GetInstance(doc, ms);

                // Open the document for writing
                doc.Open();

                QRCodeModel myQRCode = new QRCodeModel();
                myQRCode.QRCodeText = Data;

                using (QRCodeGenerator qrGenerator = new QRCodeGenerator())
                using (QRCodeData qRCodeData = qrGenerator.CreateQrCode(myQRCode.QRCodeText, QRCodeGenerator.ECCLevel.Q))
                using (QRCode qRCode = new QRCode(qRCodeData))
                {
                    Bitmap qrCodeImage = qRCode.GetGraphic(3);
                    iTextSharp.text.Image img = iTextSharp.text.Image.GetInstance(qrCodeImage, System.Drawing.Imaging.ImageFormat.Png);
                    img.SetAbsolutePosition(10, 30); // Geser ke posisi tertentu
                    img.ScaleAbsolute(50, 50); // Set the image size in the PDF
                    doc.Add(img);
                }

                // Add address information with dynamic line breaks

                //AddAddressInformation(doc, "PT BNI Life Insurance",60, 30);
                //AddAddressInformation(doc, "Centennial Tower Lt.9");
                //AddAddressInformation(doc, "Jalan Jendral Gatot Subroto");
                //AddAddressInformation(doc, "Phone: 021 -2953 9999");
                //AddAddressInformation(doc, "Fax: 021 -2963 8998");
                //AddAddressInformation(doc, "Kav. 24-25,Jakarta 12930");
                string textNamaTujuan = "Nama Tujuan:";
                string textNamaTelepon = "Telp:";
                string textNamaAlamatTujuan = "Alamat:";

                string textNamaPengirim = "Nama Pengirim:";
                string textNamaDivisi = "Divisi:";

                string textttdPenerima = "Nama & tanda tangan penerima";
                // Add other dynamic information
                //AddDynamicInformation(doc, $"{textNamaTujuan} {DataByID.docReceiver}", 60, 30); // Sesuaikan posisi yang diinginkan
                //AddDynamicInformation(doc, $"{textNamaTelepon} {DataByID.phonenumber}", 60, 20);
                AddAddressInformation(doc, $"{textNamaAlamatTujuan} {DataByID.address}", writer, 60, 20);
               // AddDynamicInformation(doc, $"{textNamaPengirim} {DataByID.sender_name}", 60, 10);
                //AddDynamicInformation(doc, $"{textNamaDivisi} {DataByID.unitname}", 60, 0);

                // Close the document
                doc.Close();

                // Save the PDF to a file
                string pdfFilePath = $"path/to/save/{id}.pdf";
                System.IO.File.WriteAllBytes(pdfFilePath, ms.ToArray());

                // Provide the file for download
                return File(ms.ToArray(), "application/pdf", $"{id}.pdf");
            }
            catch (Exception ex)
            {
                TempData["status"] = "NG";
                TempData["title"] = "Gagal menambahkan dokumen masuk";
                TempData["pesan"] = ex.ToString();
                return Redirect("~/GenerateNoDoc/RegisterKurirNonEoffcie");
            }
        }
        private void AddAddressInformation(Document doc, string addressLine, PdfWriter writer, float xPosition = 0, float yPosition = 0)
        {
            var lines = SplitText(addressLine, 50);

            foreach (var line in lines)
            {
                PdfContentByte canvas = writer.DirectContent; // assuming 'writer' is the PdfWriter instance
                BaseFont bf = BaseFont.CreateFont();
                canvas.BeginText();
                canvas.SetFontAndSize(bf, 12); // Set the font and size as needed
                canvas.SetTextMatrix(xPosition, yPosition);
                canvas.ShowText(line);
                canvas.EndText();
                yPosition -= 10; // Adjust as needed for line spacing
            }
        }

        //private void AddDynamicInformation(Document doc, string information, float xPosition, float yPosition)
        //{
        //    var lines = SplitText(information, 50);

        //    foreach (var line in lines)
        //    {
        //        PdfContentByte canvas = writer.DirectContent; // assuming 'writer' is the PdfWriter instance
        //        BaseFont bf = BaseFont.CreateFont();
        //        canvas.BeginText();
        //        canvas.SetFontAndSize(bf, 12); // Set the font and size as needed
        //        canvas.SetTextMatrix(xPosition, yPosition);
        //        canvas.ShowText(line);
        //        canvas.EndText();

        //        yPosition -= 10; // Adjust as needed for line spacing
        //    }
        //}


        private IEnumerable<string> SplitText(string text, int maxLength)
        {
            for (int i = 0; i < text.Length; i += maxLength)
            {
                yield return text.Substring(i, Math.Min(maxLength, text.Length - i));
            }
        }

        public IActionResult Detailbarcode(string barcode, string type)
		{
			//CultureInfo culture = new CultureInfo("en-IN");
			var now = DateTime.Now;
			//var tanggal = now.ToString("dd-MMMM-yyyy HH:mm:ss");
            string tanggal = now.ToString("dd MMM yyyy HH:mm:ss");

            // Membuat barcode menggunakan ZXing.NET
            BarcodeLib.Barcode barcodee = new BarcodeLib.Barcode();
			System.Drawing.Image barcodeImg = barcodee.Encode(BarcodeLib.TYPE.CODE128, barcode, System.Drawing.Color.Black, System.Drawing.Color.White, 625, 40);

			string logoPath = "wwwroot\\qrcode_barcode\\logobnil.JPG";
			Bitmap logoImage = new Bitmap(logoPath);

			// Atur posisi dan ukuran logo
			int logoX = 395;
			int logoY = 16;
			int logoWidth = 130;
			int logoHeight = 55;
			Bitmap resizedLogoImage = new Bitmap(logoImage, new Size(logoWidth, logoHeight));

			// Membuat gambar hasil dengan menyisipkan barcode, teks, dan logo ke dalam template
			//Bitmap resultImage = new Bitmap(800, 400); // Ukuran gambar hasil
			Bitmap resultImage = new Bitmap(547, 252);
			using (Graphics graphics = Graphics.FromImage(resultImage))
			{
				graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
				graphics.Clear(System.Drawing.Color.White); // Atur latar belakang gambar

                //graphics.DrawImage(barcodeImg, new Point(-1, 138)); // Gambar barcode
               
                    
                if (type =="1")
                {
                    graphics.DrawImage(barcodeImg, new System.Drawing.Rectangle(-95, 138, 750, 40)); // Gambar barcode
                }
                else
                {
                    graphics.DrawImage(barcodeImg, new System.Drawing.Rectangle(-95, 138, 750, 40)); // Gambar barcode
                }

                //graphics.DrawImage(barcodeImg, new System.Drawing.Rectangle(-95, 138, 750, 40)); // Gambar barcode
                graphics.DrawImage(resizedLogoImage, new System.Drawing.Rectangle(logoX, logoY, logoWidth, logoHeight)); // Gambar logo

				// Variabel untuk teks-teks yang akan ditampilkan
				string texttanggal = tanggal;
				string textbarode = barcode;
				string textreceived = "RECEIVED";
				string textmailingroom = "Mailing Room";
				string textnoawbresi = "No.AWB/RESI  :";
				string textalamatBniLife1 = "PT.BNI LIFE INSURANCE";
				string textalamatBniLife2 = "Centennial Tower Lt.9 Jakarta";

				var dtType = type == "1" ? "NON-EKSPEDISI" : "RETURN";
				string texttype = dtType;

				// Menerapkan format dan posisi teks
				using (PrivateFontCollection privateFonts = new PrivateFontCollection())
				{
					privateFonts.AddFontFile("wwwroot\\fonts\\arial.ttf"); // Ganti dengan path ke font yang sesuai

					float Fontreceived = 27f;
					float Fontmailingroom = 16f;
					float Fontnoawbres = 16f;
					float FontType = 16f;
					float Fontbarode = 15f;
					float FontTanggal = 12f;
					float FontAlamat1 = 14f;
					float FontAlamat2 = 14f;

					FontStyle fontStyleBold = FontStyle.Bold;

					using (System.Drawing.Font textFontreceived = new System.Drawing.Font(privateFonts.Families.First(), Fontreceived, FontStyle.Bold, GraphicsUnit.Point))
					using (System.Drawing.Font textFontmailingroom = new System.Drawing.Font(privateFonts.Families.First(), Fontmailingroom, FontStyle.Bold, GraphicsUnit.Point))
					using (System.Drawing.Font textFontnoawbres = new System.Drawing.Font(privateFonts.Families.First(), Fontnoawbres, FontStyle.Bold, GraphicsUnit.Point))
					using (System.Drawing.Font textFontType = new System.Drawing.Font(privateFonts.Families.First(), FontType, FontStyle.Bold, GraphicsUnit.Point))
					using (System.Drawing.Font textFontbarode = new System.Drawing.Font(privateFonts.Families.First(), Fontbarode, FontStyle.Bold, GraphicsUnit.Point))
					using (System.Drawing.Font textFontTanggal = new System.Drawing.Font(privateFonts.Families.First(), FontTanggal, FontStyle.Bold, GraphicsUnit.Point))
					using (System.Drawing.Font textFontAlamat1 = new System.Drawing.Font(privateFonts.Families.First(), FontAlamat1, FontStyle.Bold, GraphicsUnit.Point))
					using (System.Drawing.Font textFontAlamat2 = new System.Drawing.Font(privateFonts.Families.First(), FontAlamat2, FontStyle.Bold, GraphicsUnit.Point))
					
					using (SolidBrush textBrush = new SolidBrush(System.Drawing.Color.Black))
					{
						

						graphics.DrawString(textreceived, textFontreceived, textBrush, new Point(20, 12));
						graphics.DrawString(textmailingroom, textFontmailingroom, textBrush, new Point(20, 60));
						graphics.DrawString(textnoawbresi, textFontnoawbres, textBrush, new Point(20, 95));

						int textTypeX = 285;
						int textTypeY = 90;
						int textTypeWidth = (int)graphics.MeasureString(texttype, textFontType).Width + 130; // Tambahkan beberapa padding
						int textTypeHeight = (int)graphics.MeasureString(texttype, textFontType).Height;
                        if (type =="1")
                        {
                            graphics.FillRectangle(textBrush, 200, textTypeY, textTypeWidth, textTypeHeight);
                        }
                        else
                        {
                            int textTypeWidthR = (int)graphics.MeasureString(texttype, textFontType).Width + 215; // Tambahkan beberapa padding
                            int textTypeHeightR = (int)graphics.MeasureString(texttype, textFontType).Height;
                            graphics.FillRectangle(textBrush, 200, textTypeY, textTypeWidthR, textTypeHeightR);
                        }
                        
						SolidBrush textBrushWhite = new SolidBrush(System.Drawing.Color.White);
                        if (type =="1")
                        {
                            graphics.DrawString(texttype, textFontType, textBrushWhite, new Point(textTypeX, textTypeY));
                        }
                        else
                        {
                            graphics.DrawString(texttype, textFontType, textBrushWhite, new Point(300, 90));
                        }
                        
						//graphics.DrawString(texttype, textFontType, textBrush, new Point(260, 117));

						graphics.DrawString(textbarode, textFontbarode, textBrush, new Point(255, 115));
                        graphics.DrawString(texttanggal, textFontTanggal, textBrush, new Point(360, 200));
                        graphics.DrawString(textalamatBniLife1, textFontAlamat1, textBrush, new Point(20, 190));
                        graphics.DrawString(textalamatBniLife2, textFontAlamat2, textBrush, new Point(20, 220));
                    }

				}
			}
			using (MemoryStream memoryStream = new MemoryStream())
			{
				resultImage.Save(memoryStream, ImageFormat.Png);

				// Mengubah MemoryStream menjadi array byte
				byte[] imageBytes = memoryStream.ToArray();

				// Memberikan nama file yang sesuai
				string fileName = barcode+".png";

				// Mengatur header respons untuk pengunduhan
				Response.Headers.Add("Content-Disposition", new ContentDisposition
				{
					FileName = fileName,
					Inline = false // Mengatur ke false agar tampil sebagai pengunduhan
				}.ToString());

				// Mengirim gambar sebagai respons HTTP
				return File(imageBytes, "image/png");
			}
		}

		//public async Task<IActionResult> Detailbarcode(string barcode, string type)
		//{
		//    CultureInfo culture = new CultureInfo("en-IN");
		//    var now = DateTime.Now;
		//    var tanggal = now.ToString("dd-MMMM-yyyy HH:ss:mm", culture);
		//    string filename = "35x76cm";
		//    string filePath = Path.Combine("wwwroot\\qrcode_barcode\\" + filename + ".JPG");
		//    string filettd = Path.GetFileName(filePath);
		//    string pathttd = $"{this.Request.Scheme}://{this.Request.Host}{this.Request.PathBase}" + "/qrcode_barcode/" + filettd;

		//    string logoPath = "wwwroot\\qrcode_barcode\\logobnil.JPG";
		//    Bitmap logoImage = new Bitmap(logoPath);

		//    // Atur posisi dan ukuran logo
		//    int logoX = 650;
		//    int logoY = 23;
		//    int logoWidth = 170;
		//    int logoHeight = 73;
		//    Bitmap resizedLogoImage = new Bitmap(logoImage, new Size(logoWidth, logoHeight));
		//    // Membuat barcode menggunakan ZXing.NET
		//    BarcodeLib.Barcode barcodee = new BarcodeLib.Barcode();

		//    System.Drawing.Image barcodeImg = barcodee.Encode(BarcodeLib.TYPE.CODE128, barcode, System.Drawing.Color.Black, System.Drawing.Color.White, 415, 20);
		//    System.Drawing.Image templateImg = System.Drawing.Image.FromFile(filePath);

		//    // Membuat gambar hasil dengan menyisipkan barcode dan teks ke dalam template
		//    using (Graphics graphics = Graphics.FromImage(templateImg))
		//    {
		//        //int barcodeWidth = 1830; // Lebar barcode
		//        int barcodeX = 300; // Ubah sesuai dengan posisi X yang Anda inginkan
		//        int barcodeY = 225; // Ganti dengan posisi Y barcode sesuai kebutuhan
		//        //int barcodeWidth = 900;
		//        //int barcodeHeight = 60;
		//        // Menggambar gambar barcode di sisi kanan gambar template
		//        graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
		//        graphics.DrawImage(barcodeImg, new System.Drawing.PointF(barcodeX, barcodeY));
		//        graphics.DrawImage(resizedLogoImage, new System.Drawing.Rectangle(logoX, logoY, logoWidth, logoHeight));
		//        //graphics.DrawImage(barcodeImg, new Point(barcodeX, barcodeY));

		//        // Variabel untuk teks-teks yang akan ditampilkan
		//        string texttanggal = tanggal;
		//        string textbarode = barcode;
		//        string textreceived = "RECEIVED";
		//        string textmailingroom = "Mailing Room";
		//        string textnoawbresi = "No.AWB/RESI :";
		//        string textalamatBniLife1 = "PT.BNI LIFE INSURANCE";
		//        string textalamatBniLife2 = "Centennial Tower Lt.9 Jakarta";

		//        var dtType = type =="1" ? "NON-EKSPEDISI" : "RETURN";
		//        string texttype = dtType;

		//        int receivedX = 25;
		//        int receivedY = 4;

		//        int mailingroomX = 25;
		//        int mailingroomY = 107;

		//        int noawbresX = 25;
		//        int noawbresY = 150;



		//        int typeX = 411;
		//        int typeY = 142;

		//        int barodeX = 375;
		//        int barodeY = 185;

		//        int tglWidth = 560;
		//        int tglHeight = 60;
		//        int tanggalX = 500;
		//        int tanggalY = 300;

		//        int alamat1X = 25;
		//        int alamat1Y = 300;

		//        int alamat2X = 25;
		//        int alamat2Y = 330;

		//        PrivateFontCollection privateFonts = new PrivateFontCollection();
		//        privateFonts.AddFontFile("wwwroot\\fonts\\arial_bold.ttf"); // Replace with the actual path to your font file
		//                                                                    //privateFonts.AddFontFile("wwwroot\\fonts\\arial.ttf"); // Replace with the actual path to your font file

		//        float Fontreceived = 14f; // Font size for alamatBniLife
		//        float Fontmailingroom = 10f; // Font size for alamatBniLife
		//        float Fontnoawbres = 10f; // Font size for alamatBniLife
		//        float FontType = 10f; // Font size for alamatBniLife
		//        float Fontbarode = 8f; // Standard font size
		//        float FontTanggal = 5f; // Standard font size
		//        float FontAlamat1 = 5f; // Standard font size
		//        float FontAlamat2 = 5f; // Standard font size
		//        FontStyle fontStyleBold = FontStyle.Bold;


		//        System.Drawing.Font textFontreceived = new System.Drawing.Font(privateFonts.Families[0], Fontreceived, GraphicsUnit.Point);
		//        System.Drawing.Font textFontmailingroom = new System.Drawing.Font(privateFonts.Families[0], Fontmailingroom, fontStyleBold, GraphicsUnit.Point);
		//        System.Drawing.Font textFontnoawbres = new System.Drawing.Font(privateFonts.Families[0], Fontnoawbres, fontStyleBold, GraphicsUnit.Point);

		//        System.Drawing.Font textFontType = new System.Drawing.Font(privateFonts.Families[0], FontType, fontStyleBold, GraphicsUnit.Point);
		//        System.Drawing.Font textFontbarode = new System.Drawing.Font(privateFonts.Families[0], Fontbarode, fontStyleBold, GraphicsUnit.Point);
		//        System.Drawing.Font textFontTanggal = new System.Drawing.Font(privateFonts.Families[0], FontTanggal, fontStyleBold, GraphicsUnit.Point);

		//        System.Drawing.Font textFontAlamat1 = new System.Drawing.Font(privateFonts.Families[0], FontAlamat1, fontStyleBold, GraphicsUnit.Point);
		//        System.Drawing.Font textFontAlamat2 = new System.Drawing.Font(privateFonts.Families[0], FontAlamat2, fontStyleBold, GraphicsUnit.Point);


		//        SolidBrush textBrush = new SolidBrush(System.Drawing.Color.Black);
		//        SolidBrush textBrushWhite = new SolidBrush(System.Drawing.Color.Black);
		//        // Menggambar teks pada posisi yang telah ditentukan
		//        graphics.DrawString(textreceived, textFontreceived, textBrush, new PointF(receivedX, receivedY));
		//        graphics.DrawString(textmailingroom, textFontmailingroom, textBrush, new PointF(mailingroomX, mailingroomY));
		//        graphics.DrawString(textnoawbresi, textFontnoawbres, textBrush, new PointF(noawbresX, noawbresY));

		//        graphics.DrawString(texttype, textFontType, textBrushWhite, new PointF(typeX, typeY));

		//        graphics.DrawString(textbarode, textFontbarode, textBrush, new PointF(barodeX, barodeY));
		//        graphics.DrawString(texttanggal, textFontTanggal, textBrush, new PointF(tanggalX, tanggalY));
		//        graphics.DrawString(textalamatBniLife1, textFontAlamat1, textBrush, new PointF(alamat1X, alamat1Y));
		//        graphics.DrawString(textalamatBniLife2, textFontAlamat2, textBrush, new PointF(alamat2X, alamat2Y));





		//        // Simpan hasilnya ke MemoryStream
		//        using (MemoryStream stream = new MemoryStream())
		//        {
		//            templateImg.Save(stream, System.Drawing.Imaging.ImageFormat.Jpeg);


		//            Response.ContentType = "image/jpeg";

		//            // Mengatur nama file dan memaksa respons untuk mengunduh
		//            Response.Headers.Add("Content-Disposition", "attachment; filename="+barcode+".jpg");

		//            // Mengirimkan gambar sebagai respons
		//            stream.Position = 0;
		//            await stream.CopyToAsync(Response.Body);

		//        }
		//    }
		//    return new EmptyResult();
		//}



		public async Task<IActionResult> PrintLabelTwice(string barcode, string type)
		{
			try
			{
				CultureInfo culture = new CultureInfo("en-IN");
				var now = DateTime.Now;
				var tanggal = now.ToString("dd-MMMM-yyyy HH:ss:mm", culture);
				string filename = "35x76cm";
				string filePath = Path.Combine("wwwroot\\qrcode_barcode\\" + filename + ".JPG");

				// Membuat barcode menggunakan ZXing.NET
				BarcodeLib.Barcode barcodee = new BarcodeLib.Barcode();
				System.Drawing.Image barcodeImg = barcodee.Encode(BarcodeLib.TYPE.CODE128, barcode, System.Drawing.Color.Black, System.Drawing.Color.White, 615, 50);

				// Memuat gambar template
				System.Drawing.Image templateImg = System.Drawing.Image.FromFile(filePath);

				// Membuat gambar hasil dengan menyisipkan barcode dan teks ke dalam template
				using (MemoryStream stream = new MemoryStream())
				{
					// ... Kode sebelumnya untuk menggambar gambar barcode dan elemen desain lainnya ...

					// Simpan teks yang akan dimasukkan ke dalam label
					string textreceived = "RECEIVED";
					string textmailingroom = "Mailing Room";
					string textnoawbresi = "No.AWB/RESI :";
					string textalamatBniLife1 = "PT.BNI LIFE INSURANCE";
					string textalamatBniLife2 = "Centennial Tower Lt.9 Jakarta";

					var dtType = type == "1" ? "NON-EKSPEDISI" : "RETURN";
					string texttype = dtType;

					int receivedX = 20;
					int receivedY = 4;

					int mailingroomX = 20;
					int mailingroomY = 107;

					int noawbresX = 20;
					int noawbresY = 150;

					int typeX = 413;
					int typeY = 142;

					int barodeX = 350;
					int barodeY = 175;

					int tglWidth = 560;
					int tglHeight = 60;
					int tanggalX = 500;
					int tanggalY = 300;

					int alamat1X = 20;
					int alamat1Y = 300;

					int alamat2X = 20;
					int alamat2Y = 330;

					PrivateFontCollection privateFonts = new PrivateFontCollection();
					privateFonts.AddFontFile("wwwroot\\fonts\\arial_bold.ttf"); // Ganti dengan path yang sesuai
																				//privateFonts.AddFontFile("wwwroot\\fonts\\arial.ttf"); // Ganti dengan path yang sesuai

					float Fontreceived = 14f; // Ukuran font untuk teks "RECEIVED"
					float Fontmailingroom = 9f; // Ukuran font untuk teks "Mailing Room"
					float Fontnoawbres = 9f; // Ukuran font untuk teks "No.AWB/RESI :"
					float FontType = 9f; // Ukuran font untuk teks "Type"
					float Fontbarode = 9f; // Ukuran font untuk teks barcode
					float FontTanggal = 6f; // Ukuran font untuk teks tanggal
					float FontAlamat1 = 6f; // Ukuran font untuk teks alamat 1
					float FontAlamat2 = 5f; // Ukuran font untuk teks alamat 2
					FontStyle fontStyleBold = FontStyle.Bold;

					// Menggambar teks pada gambar label pertama
					using (Graphics graphics = Graphics.FromImage(templateImg))
					{
						SolidBrush textBrush = new SolidBrush(System.Drawing.Color.Black);

						graphics.DrawString(textreceived, new System.Drawing.Font(privateFonts.Families[0], Fontreceived, FontStyle.Bold, GraphicsUnit.Point), textBrush, new PointF(receivedX, receivedY));
						graphics.DrawString(textmailingroom, new System.Drawing.Font(privateFonts.Families[0], Fontmailingroom, FontStyle.Bold, GraphicsUnit.Point), textBrush, new PointF(mailingroomX, mailingroomY));
						graphics.DrawString(textnoawbresi, new System.Drawing.Font(privateFonts.Families[0], Fontnoawbres, FontStyle.Bold, GraphicsUnit.Point), textBrush, new PointF(noawbresX, noawbresY));
						graphics.DrawString(texttype, new System.Drawing.Font(privateFonts.Families[0], FontType, FontStyle.Bold, GraphicsUnit.Point), textBrush, new PointF(typeX, typeY));
						graphics.DrawString(barcode, new System.Drawing.Font(privateFonts.Families[0], Fontbarode, FontStyle.Bold, GraphicsUnit.Point), textBrush, new PointF(barodeX, barodeY));
						graphics.DrawString(tanggal, new System.Drawing.Font(privateFonts.Families[0], FontTanggal, FontStyle.Bold, GraphicsUnit.Point), textBrush, new RectangleF(tanggalX, tanggalY, tglWidth, tglHeight));
						graphics.DrawString(textalamatBniLife1, new System.Drawing.Font(privateFonts.Families[0], FontAlamat1, FontStyle.Bold, GraphicsUnit.Point), textBrush, new PointF(alamat1X, alamat1Y));
						graphics.DrawString(textalamatBniLife2, new System.Drawing.Font(privateFonts.Families[0], FontAlamat2, FontStyle.Bold, GraphicsUnit.Point), textBrush, new PointF(alamat2X, alamat2Y));
					}

					// Simpan hasil gambar label pertama ke dalam MemoryStream
					templateImg.Save(stream, System.Drawing.Imaging.ImageFormat.Jpeg);

					// ... Kode selanjutnya untuk menyimpan atau mencetak label pertama ...

					// Simpan hasil gambar label kedua ke dalam MemoryStream
					stream.Position = 0;
					templateImg.Save(stream, System.Drawing.Imaging.ImageFormat.Jpeg);

					// ... Kode selanjutnya untuk menyimpan atau mencetak label kedua ...

					// Simpan hasilnya ke MemoryStream
					templateImg.Save(stream, System.Drawing.Imaging.ImageFormat.Jpeg);

					// Mengatur nama file untuk menyimpan sementara gambar yang akan dicetak
					string tempFilePath = "path_to_temp_image.jpg"; // Ganti dengan path yang sesuai
					System.IO.File.WriteAllBytes(tempFilePath, stream.ToArray());

					// Inisialisasi koneksi ke printer termal dengan RawPrint
					var printer = new Printer();
					string printerName = "Nama_Printer_Anda";

					// Cetak label pertama
					printer.PrintRawFile(printerName, tempFilePath, true); // Ganti dengan nama printer yang sesuai

					// Cetak label kedua
					printer.PrintRawFile(printerName, tempFilePath, true); // Ganti dengan nama printer yang sesuai

					// Setelah mencetak, Anda dapat menghapus gambar sementara jika diperlukan
					System.IO.File.Delete(tempFilePath);

					// Return respons sukses
					return Ok("Label berhasil dicetak dua kali.");
				}

		        
		    }
			catch (Exception ex)
			{
				// Tangani kesalahan yang mungkin terjadi selama pencetakan
				return StatusCode(500, "Terjadi kesalahan saat mencetak label: " + ex.Message);
			}
		}




	//public IActionResult Detailbarcode(ParamInsertGenerateNoDoc pr)
	//{
	//    //CultureInfo culture = new CultureInfo("en-IN");
	//    //var now = DateTime.Now;
	//    //var dt = now.ToString("dd-MMMM-yyyy HH:ss:mm", culture);
	//    //ViewBag.tanggal = dt;
	//    //ViewBag.tipe = pr.type;
	//    //ViewBag.barcode = pr.barcode;
	//    // Membuat barcode menggunakan ZXing.NET
	//    //Barcode barcode = new Barcode();
	//    //System.Drawing.Image img = barcode.Encode(TYPE.CODE93, pr.barcode, System.Drawing.Color.Black, System.Drawing.Color.Black,250,100);
	//    //var data = ConvertImageToBytes(img);
	//    //return File(data, "image/jpeg"); 

	//    CultureInfo culture = new CultureInfo("en-IN");
	//    var now = DateTime.Now;
	//    var dt = now.ToString("dd-MMMM-yyyy HH:ss:mm", culture);

	//    // Membuat barcode menggunakan ZXing.NET
	//    Barcode barcode = new Barcode();
	//    System.Drawing.Image barcodeImg = barcode.Encode(TYPE.CODE93, pr.barcode, System.Drawing.Color.Black, System.Drawing.Color.White, 250, 100);
	//    string filename = "barcode";
	//    string filePath = Path.Combine("wwwroot\\qrcode_barcode\\" + filename + ".JPG");
	//    // Memuat gambar template
	//    string templateImagePath = "wwwroot/template.jpg"; // Ganti dengan lokasi dan nama file template Anda
	//    System.Drawing.Image templateImg = System.Drawing.Image.FromFile(filePath);

	//    // Membuat gambar hasil dengan menyisipkan barcode ke dalam template
	//    using (Graphics graphics = Graphics.FromImage(templateImg))
	//    {
	//        int barcodeX = 100; // Ganti dengan posisi X barcode sesuai kebutuhan
	//        int barcodeY = 200; // Ganti dengan posisi Y barcode sesuai kebutuhan

	//        graphics.DrawImage(barcodeImg, new Point(barcodeX, barcodeY));

	//        // Simpan hasilnya ke MemoryStream
	//        using (MemoryStream stream = new MemoryStream())
	//        {
	//            templateImg.Save(stream, System.Drawing.Imaging.ImageFormat.Jpeg);
	//            var data = stream.ToArray();

	//            return File(data, "image/jpeg");
	//        }
	//    }
	//}

	//private byte[] ConvertImageToBytes(System.Drawing.Image img)
	//{
	//    using(MemoryStream stream = new MemoryStream())
	//    {
	//        img.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
	//        return stream.ToArray();
	//    }
	//}
	public async Task<IActionResult> ReadNotifikasiLainnya(Guid id)
        {
            OutputNotifLainyya DetailApproval = new OutputNotifLainyya();
            GeneralOutputModel generalOutput = new GeneralOutputModel();
            var token = HttpContext.Session.GetString("token");
            getByIdNotifikasiLainnya prApi = new getByIdNotifikasiLainnya();
            prApi.ID_NOTIFIKASI = id;

            generalOutput = await _dataAccessProvider.NotifikasiLainnya("Document/DetailLetterNonEofficeNotifikasiWeb", token, prApi);
            var jsonApiResponseSerialize = JsonConvert.SerializeObject(generalOutput.Result);
            DetailApproval = JsonConvert.DeserializeObject<OutputNotifLainyya>(jsonApiResponseSerialize);

            return View();
        }

    }
}
