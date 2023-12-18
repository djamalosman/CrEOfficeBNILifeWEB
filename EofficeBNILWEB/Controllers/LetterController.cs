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

namespace EofficeBNILWEB.Controllers
{
    public class LetterController : Controller
    {
        private readonly IConfiguration _config;
        private readonly IDataAccessProvider _dataAccessProvider;
        private readonly IHostingEnvironment _environment;
        private readonly string urlApi;
        private readonly IHtmlLocalizer<HomeController> _localiza;
        public FtpSettings _ftpconfig { get; }
        public LetterController(IConfiguration config, IDataAccessProvider dataAccessProvider, IHostingEnvironment Environment, IHtmlLocalizer<HomeController> localiza, IOptions<FtpSettings> ftpconfig)
        {
            _config = config;
            _dataAccessProvider = dataAccessProvider;
            _environment = Environment;
            urlApi = _config.GetValue<string>("AppSettings:apiUrl");
            _localiza = localiza;
            _ftpconfig = ftpconfig.Value;
        }

        public async Task<IActionResult> DetailLetterSM(Guid id)
        {
            OutputDetailLetter letterDetail = new OutputDetailLetter();
            GeneralOutputModel generalOutput = new GeneralOutputModel();
            var token = HttpContext.Session.GetString("token");
            ParamGetDetailLetter pr = new ParamGetDetailLetter();
            pr.idLetter = id;

            generalOutput = await _dataAccessProvider.GetDetailLetter("Letter/GetDetailLetterSM/", token, pr);
            var jsonApiResponseSerialize = JsonConvert.SerializeObject(generalOutput.Result);
            letterDetail = JsonConvert.DeserializeObject<OutputDetailLetter>(jsonApiResponseSerialize);

            return View(letterDetail);
        }

        public FileResult DownloadAttachment(string filename)
        {
            var fileName = filename;
            //Build the File Path.
            string path = Path.Combine(_environment.WebRootPath, "uploads/letter/") + fileName;

            //Read the File data into Byte Array.
            byte[] bytes = System.IO.File.ReadAllBytes(path);

            //Send the File to Download.
            return File(bytes, "application/octet-stream", fileName);
        }
        public async Task<IActionResult> NewOutboxCommissioner()
        {
            GeneralOutputModel generalOutput = new GeneralOutputModel();
            List<OutputContentTemplate> output = new List<OutputContentTemplate>();
            var token = HttpContext.Session.GetString("token");
            //ParamPreviewNoLetter prPreview = new ParamPreviewNoLetter();
            //prPreview.letterType = "SK";
            //generalOutput = await _dataAccessProvider.PreviewNoLetter("Letter/PreviewNoDoc", token, prPreview);
            //var jsonApiResponseSerialize = JsonConvert.SerializeObject(generalOutput.Result);
            string NoLetter = "";
            generalOutput = await _dataAccessProvider.GetContentTemplate("Template/GetTemplate/", token);
            var jsonApiResponseSerialize = JsonConvert.SerializeObject(generalOutput.Result);
            output = JsonConvert.DeserializeObject<List<OutputContentTemplate>>(jsonApiResponseSerialize);
            //jsonApiResponseSerialize = JsonConvert.SerializeObject(output);

            if (generalOutput.Status == "UA")
            {
                TempData["status"] = "NG";
                TempData["pesan"] = "Session habis silahkan login kembali";
                return RedirectToAction("Logout", "Home");
            }

            ViewBag.NoLetter = NoLetter;
            ViewBag.TemplateData = output;
            //ViewBag.JsonTemplateData = jsonApiResponseSerialize.ToString();
            return View();
        }

        public async Task<IActionResult> SuratKeluarBackdate()
        {
            GeneralOutputModel generalOutput = new GeneralOutputModel();
            List<OutputContentTemplate> output = new List<OutputContentTemplate>();
            var token = HttpContext.Session.GetString("token");
            //ParamPreviewNoLetter prPreview = new ParamPreviewNoLetter();
            //prPreview.letterType = "SK";
            //generalOutput = await _dataAccessProvider.PreviewNoLetter("Letter/PreviewNoDoc", token, prPreview);
            //var jsonApiResponseSerialize = JsonConvert.SerializeObject(generalOutput.Result);
            string NoLetter = "";
            generalOutput = await _dataAccessProvider.GetContentTemplate("Template/GetTemplate/", token);
            var jsonApiResponseSerialize = JsonConvert.SerializeObject(generalOutput.Result);
            output = JsonConvert.DeserializeObject<List<OutputContentTemplate>>(jsonApiResponseSerialize);
            //jsonApiResponseSerialize = JsonConvert.SerializeObject(output);

            if (generalOutput.Status == "UA")
            {
                TempData["status"] = "NG";
                TempData["pesan"] = "Session habis silahkan login kembali";
                return RedirectToAction("Logout", "Home");
            }

            ViewBag.NoLetter = NoLetter;
            ViewBag.TemplateData = output;
            //ViewBag.JsonTemplateData = jsonApiResponseSerialize.ToString();
            return View();
        }
        
        [Route("Letter/DetailLetterSK/{id}/{letterType}")]
        public async Task<IActionResult> DetailLetterSK(Guid id,string letterType)
        {
            OutputDetailLetter letterDetail = new OutputDetailLetter();
            GeneralOutputModel generalOutput = new GeneralOutputModel();
            List<OutputContentTemplate> output = new List<OutputContentTemplate>();
            var token = HttpContext.Session.GetString("token");
            ParamGetDetailLetter pr = new ParamGetDetailLetter();
            pr.idLetter = id;
            pr.lettertype=letterType;
            generalOutput = await _dataAccessProvider.GetDetailLetter("Letter/GetDetailLetterSK/", token, pr);
            var jsonApiResponseSerialize = JsonConvert.SerializeObject(generalOutput.Result);
            letterDetail = JsonConvert.DeserializeObject<OutputDetailLetter>(jsonApiResponseSerialize);

            generalOutput = await _dataAccessProvider.GetContentTemplate("Template/GetTemplate/", token);
            jsonApiResponseSerialize = JsonConvert.SerializeObject(generalOutput.Result);
            output = JsonConvert.DeserializeObject<List<OutputContentTemplate>>(jsonApiResponseSerialize);

            ViewBag.TemplateData = output;
            return View(letterDetail);
        }

        public async Task<IActionResult> SKDelivery()
        {
            GeneralOutputModel generalOutput = new GeneralOutputModel();
            var token = HttpContext.Session.GetString("token");
            //ParamPreviewNoLetter prPreview = new ParamPreviewNoLetter();
            //prPreview.letterType = "SK";
            //generalOutput = await _dataAccessProvider.PreviewNoLetter("Letter/PreviewNoDoc", token, prPreview);
            //var jsonApiResponseSerialize = JsonConvert.SerializeObject(generalOutput.Result);
            List<StringmapOutput> shippingTypeList = new List<StringmapOutput>();
            List<StringmapOutput> statusCodeList = new List<StringmapOutput>();
            List<StringmapOutput> deliveryTypeList = new List<StringmapOutput>();
            ParamGetStringmap pr = new ParamGetStringmap();
            pr.objectName = "tr_delivery";
            pr.attributeName = "SHIPPING_TYPE_CODE";

            ParamGetStringmap prStatus = new ParamGetStringmap();
            prStatus.objectName = "tr_delivery";
            prStatus.attributeName = "STATUS_CODE";

            ParamGetStringmap prDelivery = new ParamGetStringmap();
            prDelivery.objectName = "tr_delivery";
            prDelivery.attributeName = "DELIVERY_TYPE_CODE";

            ParamGetSKDelivery prGetSK = new ParamGetSKDelivery();
            prGetSK.type = "All";

            generalOutput = await _dataAccessProvider.GetStringmapAsync("General/GetStringmap/", pr, token);
            var jsonApiResponseSerialize = JsonConvert.SerializeObject(generalOutput.Result);
            shippingTypeList = JsonConvert.DeserializeObject<List<StringmapOutput>>(jsonApiResponseSerialize);

            generalOutput = await _dataAccessProvider.GetStringmapAsync("General/GetStringmap/", prStatus, token);
            jsonApiResponseSerialize = JsonConvert.SerializeObject(generalOutput.Result);
            statusCodeList = JsonConvert.DeserializeObject<List<StringmapOutput>>(jsonApiResponseSerialize);

            generalOutput = await _dataAccessProvider.GetStringmapAsync("General/GetStringmap/", prDelivery, token);
            jsonApiResponseSerialize = JsonConvert.SerializeObject(generalOutput.Result);
            deliveryTypeList = JsonConvert.DeserializeObject<List<StringmapOutput>>(jsonApiResponseSerialize);

            List<DeliveryLetterOutput> LetterList = new List<DeliveryLetterOutput>();
            generalOutput = await _dataAccessProvider.GetSkDelivery("Letter/DeliveryLetterList/", token, prGetSK);
            jsonApiResponseSerialize = JsonConvert.SerializeObject(generalOutput.Result);
            LetterList = JsonConvert.DeserializeObject<List<DeliveryLetterOutput>>(jsonApiResponseSerialize);

            if (generalOutput.Status == "UA")
            {
                TempData["status"] = "NG";
                TempData["pesan"] = "Session habis silahkan login kembali";
                return RedirectToAction("Logout", "Home");
            }

            ViewBag.Output = null;
            ViewBag.ShippingTypeList = shippingTypeList;
            ViewBag.StatusCodeList = statusCodeList;
            ViewBag.DeliveryTypeList = deliveryTypeList;
            ViewBag.LetterList = LetterList;
            ViewBag.UlTabActiveHome = "active";
            ViewBag.DivTabActiveHome = "active show";
            ViewBag.UlTabActiveUpload = "";
            ViewBag.DivTabActiveUpload = "fade";
            if (TempData["OutputApi"] != null)
            {
                var OutputUploadDocument = JsonConvert.DeserializeObject<List<UploadEkspedisiEofficeOutput>>((string)TempData["OutputApi"]);

                ViewBag.Output = OutputUploadDocument;

                ViewBag.UlTabActiveHome = "";
                ViewBag.DivTabActiveHome = "fade";
                ViewBag.UlTabActiveUpload = "active";
                ViewBag.DivTabActiveUpload = "active show";
            }
            return View();
        }

        public async Task<IActionResult> ViewQrcodeEoffice(ParamGenerateDeliveryNumber pr)
        {
            try
            {
                //pr.type = 3;
                //pr.userCode = "01";
                var token = HttpContext.Session.GetString("token");
                GeneralOutputModel generalOutput = new GeneralOutputModel();
                DeliveryDetailOutput deliveryDetail = new DeliveryDetailOutput();
                ParamGetDetailDelivery prDetailDeliv = new ParamGetDetailDelivery();
                prDetailDeliv.idDelivery = pr.idDeliv;
                generalOutput = await _dataAccessProvider.GetDetailDeliveryEoffice("Letter/GetDetailDeliveryEoffice/", token, prDetailDeliv);
                var jsonApiResponseSerialize = JsonConvert.SerializeObject(generalOutput.Result);
                deliveryDetail = JsonConvert.DeserializeObject<DeliveryDetailOutput>(jsonApiResponseSerialize);

                var Data = deliveryDetail.deliveryNumber;
                if (deliveryDetail.deliveryNumber == null)
                {
                    // getDataNomorDocument
                    generalOutput = await _dataAccessProvider.GenerateQrCodeDeliveryEoffice("Letter/GenerateQrCodeDelivery", token, pr);
                    jsonApiResponseSerialize = JsonConvert.SerializeObject(generalOutput.Result);
                    Data = JsonConvert.DeserializeObject<string>(jsonApiResponseSerialize);
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

                //ViewBag.detailDelivery = deliveryDetail;
                ViewBag.DataQrcode = Data;

                return View(deliveryDetail);
            }
            catch (Exception ex)
            {
                TempData["status"] = "NG";
                TempData["title"] = "Gagal generate QR Code";
                TempData["pesan"] = ex.ToString();
                return Redirect("~/Letter/SKDelivery");
            }
        }

        public async Task<IActionResult> DetailSKDelivery(Guid id)
        {
            try
            {
                var token = HttpContext.Session.GetString("token");
                DeliveryDetailOutput deliveryDetail = new DeliveryDetailOutput();
                GeneralOutputModel generalOutput = new GeneralOutputModel();
                ParamGetDetailDelivery prDetailDeliv = new ParamGetDetailDelivery();
                prDetailDeliv.idDelivery = id;

                List<StringmapOutput> shippingTypeList = new List<StringmapOutput>();
                List<StringmapOutput> statusCodeList = new List<StringmapOutput>();
                List<StringmapOutput> deliveryTypeList = new List<StringmapOutput>();
                ParamGetStringmap pr = new ParamGetStringmap();
                pr.objectName = "tr_delivery";
                pr.attributeName = "SHIPPING_TYPE_CODE";

                ParamGetStringmap prStatus = new ParamGetStringmap();
                prStatus.objectName = "tr_delivery";
                prStatus.attributeName = "STATUS_CODE";

                ParamGetStringmap prDelivery = new ParamGetStringmap();
                prDelivery.objectName = "tr_delivery";
                prDelivery.attributeName = "DELIVERY_TYPE_CODE";

                generalOutput = await _dataAccessProvider.GetStringmapAsync("General/GetStringmap/", pr, token);
                var jsonApiResponseSerialize = JsonConvert.SerializeObject(generalOutput.Result);
                shippingTypeList = JsonConvert.DeserializeObject<List<StringmapOutput>>(jsonApiResponseSerialize);

                generalOutput = await _dataAccessProvider.GetStringmapAsync("General/GetStringmap/", prStatus, token);
                jsonApiResponseSerialize = JsonConvert.SerializeObject(generalOutput.Result);
                statusCodeList = JsonConvert.DeserializeObject<List<StringmapOutput>>(jsonApiResponseSerialize);

                generalOutput = await _dataAccessProvider.GetStringmapAsync("General/GetStringmap/", prDelivery, token);
                jsonApiResponseSerialize = JsonConvert.SerializeObject(generalOutput.Result);
                deliveryTypeList = JsonConvert.DeserializeObject<List<StringmapOutput>>(jsonApiResponseSerialize);

                generalOutput = await _dataAccessProvider.GetDetailDeliveryEoffice("Letter/GetDetailDeliveryEoffice/", token, prDetailDeliv);
                jsonApiResponseSerialize = JsonConvert.SerializeObject(generalOutput.Result);
                deliveryDetail = JsonConvert.DeserializeObject<DeliveryDetailOutput>(jsonApiResponseSerialize);

                ViewBag.DetailDeliveryJSON = jsonApiResponseSerialize;
                ViewBag.ShippingTypeList = shippingTypeList;
                ViewBag.StatusCodeList = statusCodeList;
                ViewBag.DeliveryTypeList = deliveryTypeList;
                ViewBag.UlTabActiveHome = "active";
                ViewBag.DivTabActiveHome = "active show";
                ViewBag.UlTabActiveUpload = "";
                ViewBag.DivTabActiveUpload = "fade";

                return View(deliveryDetail);
            }
            catch (Exception ex)
            {
                TempData["status"] = "NG";
                TempData["title"] = "Gagal generate get detail delivery";
                TempData["pesan"] = ex.ToString();
                return Redirect("~/Letter/SKDelivery");
            }

        }

        public async Task<IActionResult> ExportDeliveryEoffice(ParamGetReportEkspedisiEoffice pr)
        {
            using (var workbook = new XLWorkbook())
            {
                List<DeliveryReportOutput> output = new List<DeliveryReportOutput>();
                GeneralOutputModel generalOutput = new GeneralOutputModel();
                var token = HttpContext.Session.GetString("token");
                //pr.statusElse = 3;
                generalOutput = await _dataAccessProvider.GetReportDeliveryEoffice("Letter/DeliveryLetterReportList/", token, pr);
                var jsonApiResponseSerialize = JsonConvert.SerializeObject(generalOutput.Result);
                output = JsonConvert.DeserializeObject<List<DeliveryReportOutput>>(jsonApiResponseSerialize);

                var fileName = "Export ekspedisi_" + DateTime.Now.ToString("dd_MM_yyyy") + ".xlsx";
                var worksheet = workbook.Worksheets.Add("data");
                var curerentRow = 1;
                var no = 1;
                worksheet.Cell(curerentRow, 1).Value = "No";
                worksheet.Cell(curerentRow, 2).Value = "Nomor Surat";
                worksheet.Cell(curerentRow, 3).Value = "Pengirim";
                worksheet.Cell(curerentRow, 4).Value = "Divisi";
                worksheet.Cell(curerentRow, 5).Value = "Nama Ekspedisi";
                worksheet.Cell(curerentRow, 6).Value = "Nomor Referensi";
                worksheet.Cell(curerentRow, 7).Value = "Nomor AWB/Pengiriman";
                worksheet.Cell(curerentRow, 8).Value = "Nama Tujuan";
                worksheet.Cell(curerentRow, 9).Value = "Nomor HP Tujuan";
                worksheet.Cell(curerentRow, 10).Value = "Alamat Tujuan";
                worksheet.Cell(curerentRow, 11).Value = "Nama Penerima";
                worksheet.Cell(curerentRow, 12).Value = "Tanggal Penerimaan";
                worksheet.Cell(curerentRow, 13).Value = "Jenis Pengiriman";
                worksheet.Cell(curerentRow, 14).Value = "Tipe Pengiriman";
                worksheet.Cell(curerentRow, 15).Value = "Status Pengiriman";
                foreach (var item in output)
                {
                    curerentRow++;
                    worksheet.Cell(curerentRow, 1).Value = no.ToString();
                    worksheet.Cell(curerentRow, 2).Value = item.letterNumber.ToString();
                    worksheet.Cell(curerentRow, 3).Value = item.sender.ToString();
                    worksheet.Cell(curerentRow, 4).Value = item.senderDivision.ToString();
                    worksheet.Cell(curerentRow, 5).Value = item.expedition == null ? "" : item.expedition.ToString();
                    worksheet.Cell(curerentRow, 6).Value = item.referenceNumber == null ? "" : item.referenceNumber.ToString();
                    worksheet.Cell(curerentRow, 7).Value = item.receiptNumber == null ? "" : item.receiptNumber.ToString();
                    worksheet.Cell(curerentRow, 8).Value = item.destination_receiver_name.ToString();
                    worksheet.Cell(curerentRow, 9).Value = item.receiverPhone == null ? "" : item.receiverPhone.ToString();
                    worksheet.Cell(curerentRow, 10).Value = item.receiverAddress.ToString();
                    worksheet.Cell(curerentRow, 11).Value = item.receiver == null ? "" : item.receiver.ToString();
                    worksheet.Cell(curerentRow, 12).Value = item.receiveDate == null ? "" : item.receiveDate.ToString();
                    worksheet.Cell(curerentRow, 13).Value = item.shippingTypeCodeValue.ToString();
                    worksheet.Cell(curerentRow, 14).Value = item.deliveryTypeCodeValue == null ? "" : item.deliveryTypeCodeValue.ToString();
                    worksheet.Cell(curerentRow, 15).Value = item.statusCodeValue.ToString();
                    no++;
                }
                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(content, "application/vnd.openxmlformats-officedocument.spreadsheethtml.sheet", fileName);
                }
            }
            return View();
        }

        public async Task<IActionResult> EofficeDeliveryReport()
        {
            return View();
        }

        public async Task<IActionResult> EofficeUserDeliveryReport(Guid? id)
        {
            GeneralOutputModel generalOutput = new GeneralOutputModel();
            var token = HttpContext.Session.GetString("token");
            ParamGetSKDelivery prGetSK = new ParamGetSKDelivery();
            if (id == null || id == Guid.Empty)
            {
                prGetSK.type = "User";
            }
            else
            {
                prGetSK.type = "Detail";
                prGetSK.idDelivery = id;
            }

            List<DeliveryLetterOutput> LetterList = new List<DeliveryLetterOutput>();
            generalOutput = await _dataAccessProvider.GetSkDelivery("Letter/DeliveryLetterList/", token, prGetSK);
            var jsonApiResponseSerialize = JsonConvert.SerializeObject(generalOutput.Result);
            LetterList = JsonConvert.DeserializeObject<List<DeliveryLetterOutput>>(jsonApiResponseSerialize);

            if (generalOutput.Status == "UA")
            {
                TempData["status"] = "NG";
                TempData["pesan"] = "Session habis silahkan login kembali";
                return RedirectToAction("Logout", "Home");
            }

            ViewBag.LetterList = LetterList;

            return View();
        }


        public IActionResult ReadFtpServer()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ReadFtpServer(IFormFile inputfile)
        {
            //FTP Server URL.
            string ftp = _ftpconfig.ftpServer;
            //FTP Folder name. Leave blank if you want to upload to root folder.
            string ftpFolder = _ftpconfig.ftpFolder;
            byte[] fileBytes = null;

            string fileName = Path.GetFileName(inputfile.FileName);
            using (StreamReader fileStream = new StreamReader(inputfile.OpenReadStream()))
            {
                fileBytes = Encoding.UTF8.GetBytes(fileStream.ReadToEnd());
                fileStream.Close();
            }

            try
            {
                //Create FTP Request.
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(ftp + ftpFolder + fileName);
                request.Method = WebRequestMethods.Ftp.UploadFile;

                //Enter FTP Server credentials.
                request.Credentials = new NetworkCredential(_ftpconfig.ftpUsername, _ftpconfig.ftpPassword);
                request.ContentLength = fileBytes.Length;
                request.UsePassive = true;
                request.UseBinary = true;
                request.KeepAlive = true;
                request.ServicePoint.ConnectionLimit = fileBytes.Length;
                request.EnableSsl = false;

                using (Stream requestStream = request.GetRequestStream())
                {
                    requestStream.Write(fileBytes, 0, fileBytes.Length);
                    requestStream.Close();
                }

                FtpWebResponse response = (FtpWebResponse)request.GetResponse();

                response.Close();
            }
            catch (WebException ex)
            {
                throw new Exception((ex.Response as FtpWebResponse).StatusDescription);
            }

            return View();
        }

        [HttpPost]
        public async Task<string> SaveAttachment(string param, IFormFile file)
        {
            try
            {
                var token = HttpContext.Session.GetString("token");
                GeneralOutputModel generalOutput = new GeneralOutputModel();
                ParamInsertAttachment pr = new ParamInsertAttachment();
                OutputInsertAttachment responseApi = new OutputInsertAttachment();
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

                generalOutput = await _dataAccessProvider.InsertAttachmentLetter("Letter/InsertAttachment", token, pr);
                var jsonApiResponseSerialize = JsonConvert.SerializeObject(generalOutput.Result);
                responseApi = JsonConvert.DeserializeObject<OutputInsertAttachment>(jsonApiResponseSerialize);
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
        public async Task<IActionResult> SaveLetter(ParamInsertLetterWeb pr, IFormFile inputfile)
        {
            try
            {
                var token = HttpContext.Session.GetString("token");
                GeneralOutputModel generalOutput = new GeneralOutputModel();
                ParamInsertLetter prApi = new ParamInsertLetter();
                List<ParamInsertReceiver> prApiReceiver = new List<ParamInsertReceiver>();
                List<ParamInsertCopy> prApiCopy = new List<ParamInsertCopy>();
                ParamInsertAttachment prAttachment = new ParamInsertAttachment();
                OutputInsertAttachment responseApiAttachment = new OutputInsertAttachment();
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

                    generalOutput = await _dataAccessProvider.InsertAttachmentLetter("Letter/InsertAttachment", token, prAttachment);
                    var jsonApiResponseSerializes = JsonConvert.SerializeObject(generalOutput.Result);
                    responseApiAttachment = JsonConvert.DeserializeObject<OutputInsertAttachment>(jsonApiResponseSerializes);
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

                prApi.documentType = pr.documentType;
                prApi.saveType = pr.saveType;
                prApi.letterTypeCode = pr.letterTypeCode;
                prApi.idresponsesurat = responseApiAttachment.idLetter == Guid.Empty ? pr.idresponsesurat : responseApiAttachment.idLetter;
                prApi.idDocument = pr.idDocument;
                prApi.trackingNumber = pr.trackingNumber;
                prApi.letterDate = pr.letterDate;
                prApi.about = pr.about;
                prApi.attachmentCount = pr.attachmentCount;
                prApi.priority = pr.priority;
                prApi.senderName = pr.senderName;
                prApi.senderAddress = pr.senderAddress;
                prApi.senderLetterDate = pr.senderLetterDate;
                prApi.senderLetterNumber = pr.senderLetterNumber;
                prApi.isiSurat = HttpUtility.HtmlEncode(pr.isiSurat);

                if (pr.idUserReceiver.Length > 0)
                {
                    for (int a = 0; a < pr.idUserReceiver.Length; a++)
                    {
                        Guid idDocumentReceiver = new Guid(pr.idUserReceiver.GetValue(a).ToString());
                        prApiReceiver.Add(new ParamInsertReceiver
                        {
                            idUserReceiver = idDocumentReceiver
                        });
                    }
                }
                if (pr.idUserCopy.Length > 0)
                {
                    for (int a = 0; a < pr.idUserCopy.Length; a++)
                    {
                        Guid idDocumentCopy = new Guid(pr.idUserCopy.GetValue(a).ToString());
                        prApiCopy.Add(new ParamInsertCopy
                        {
                            idUserCopy = idDocumentCopy
                        });
                    }
                }
                prApi.idUserReceiver = prApiReceiver;
                prApi.idUserCopy = prApiCopy;

                generalOutput = await _dataAccessProvider.InsertLetter("Letter/InsertLetter", token, prApi);
                var jsonApiResponseSerialize = JsonConvert.SerializeObject(generalOutput.Result);
                var titlePopup = "Surat berhasil disimpan";
                var pesanPopup = "Surat berhasil disimpan";
                if (pr.saveType == 4)
                {
                    titlePopup = "Surat berhasil disimpan";
                    pesanPopup = "Surat berhasil didistribusikan";
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

                if (pr.sumLetterByDoc + 1 < pr.receivedDocument)
                {
                    return RedirectToAction("DocInDistribution", "Document", new { id = pr.idDocument });
                }
                else
                {
                    return Redirect("~/Outbox/Index");
                }

            }
            catch (Exception ex)
            {
                TempData["status"] = "NG";
                TempData["title"] = "Gagal membuat surat masuk";
                TempData["pesan"] = ex.ToString();
                return Redirect("~/Document/DocIn");
            }
        }

        [HttpPost]
        public async Task<string> DeleteAttachment(Guid idFile)
        {
            var jsonApiResponseSerialize = "";
            GeneralOutputModel generalOutput = new GeneralOutputModel();
            try
            {
                var token = HttpContext.Session.GetString("token");
                ParamDeleteAttachment prApi = new ParamDeleteAttachment();
                prApi.idAttachment = idFile;

                generalOutput = await _dataAccessProvider.DeleteAttachment("Letter/DeleteAttachment", token, prApi);
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
        public async Task<IActionResult> PostLetterDisposition(ParamInsertUserDisposition pr)
        {
            try
            {
                var token = HttpContext.Session.GetString("token");
                GeneralOutputModel generalOutput = new GeneralOutputModel();
                ParamInsertLetterDisposition prApi = new ParamInsertLetterDisposition();
                List<ListInsertUserDisposition> prApiReceiver = new List<ListInsertUserDisposition>();

                prApi.idLetter = pr.idLetter;


                if (pr.idUser.Length > 0)
                {
                    for (int a = 0; a < pr.idUser.Length; a++)
                    {
                        Guid idUserDispo = new Guid(pr.idUser.GetValue(a).ToString());
                        Guid idPositionDispo = new Guid(pr.idPosition.GetValue(a).ToString());
                        string notesDispo = pr.notes.GetValue(a).ToString();
                        prApiReceiver.Add(new ListInsertUserDisposition
                        {
                            idLetter = pr.idLetter,
                            idUser = idUserDispo,
                            idPosition = idPositionDispo,
                            notes = notesDispo
                        });
                    }
                }
                prApi.listUser = prApiReceiver;

                generalOutput = await _dataAccessProvider.PostInsertDisposition("Letter/InsertDisposition", token, prApi);
                var jsonApiResponseSerialize = JsonConvert.SerializeObject(generalOutput.Result);
                var titlePopup = "Berhasil Disposisi";
                var pesanPopup = "Berhasil Disposisi";
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
            catch (Exception ex)
            {
                TempData["status"] = "NG";
                TempData["title"] = "Gagal melakukan disposisi";
                TempData["pesan"] = ex.ToString();
                return Redirect("~/Inbox/Index");
            }
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
            DocumentOutputWeb documentOutput = new DocumentOutputWeb();
            GeneralOutputModel generalOutput = new GeneralOutputModel();
            List<AdminDivisionOutput> receiverList = new List<AdminDivisionOutput>();
            var token = HttpContext.Session.GetString("token");
            Guid idUnit = new Guid(HttpContext.Session.GetString("idUnit"));
            Guid parentIdPosition = new Guid();
            Guid guidEmpty = Guid.Empty;
            if (HttpContext.Session.GetString("parentIdPosition") != guidEmpty.ToString().ToUpper())
            {
                parentIdPosition = new Guid(HttpContext.Session.GetString("parentIdPosition"));
            }
            Guid directorIdUnit = new Guid(HttpContext.Session.GetString("directorIdUnit"));

            ParamGetPenerima pr = new ParamGetPenerima();
            pr.keyword = keyword;

            generalOutput = await _dataAccessProvider.GetDataPenerima("General/GetDataPenerima/", token, pr);
            var jsonApiResponseSerialize = JsonConvert.SerializeObject(generalOutput.Result);
            receiverList = JsonConvert.DeserializeObject<List<AdminDivisionOutput>>(jsonApiResponseSerialize).Where(p => p.idLevel != 0 && (p.idUnit == idUnit || p.idUnit == directorIdUnit || p.idPosition == parentIdPosition)).ToList();
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
        public async Task<IActionResult> SaveOutboxLetter(ParamInsertLetterOutboxWeb pr, IFormFile inputfile)
        {
            try
            {
                var token = HttpContext.Session.GetString("token");
                GeneralOutputModel generalOutput = new GeneralOutputModel();
                ParamInsertLetterOutbox prApi = new ParamInsertLetterOutbox();
                List<ParamInsertChecker> prApiChecker = new List<ParamInsertChecker>();
                List<ParamInsertOutgoingRecipient> prApiReceiver = new List<ParamInsertOutgoingRecipient>();
                ParamInsertAttachment prAttachment = new ParamInsertAttachment();
                OutputInsertAttachment responseApiAttachment = new OutputInsertAttachment();
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

                        generalOutput = await _dataAccessProvider.InsertAttachmentLetter("Letter/InsertAttachment", token, prAttachment);
                        var jsonApiResponseSerializes = JsonConvert.SerializeObject(generalOutput.Result);
                        responseApiAttachment = JsonConvert.DeserializeObject<OutputInsertAttachment>(jsonApiResponseSerializes);
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
                    prApi.letterTypeCode = pr.letterTypeCode;
                    prApi.letterDate = pr.letterDate;
                    prApi.about = pr.about;
                    prApi.resultType = pr.resultType;
                    prApi.signatureType = pr.signatureType;
                    prApi.bossPositionId = pr.bossPositionId;
                    prApi.bossUserId = pr.bossUserId;
                    prApi.bossUnitId = pr.bossUnitId;
                    prApi.bossLevelId = pr.bossLevelId;
                    prApi.bossPositionName = pr.bossPositionName;
                    prApi.attachmentCount = pr.attachmentCount;
                    prApi.priority = pr.priority;
                    //prApi.senderName = pr.senderName;
                    if (pr.senderName.Length > 0)
                    {
                        for (int i = 0; i < pr.senderName.Length; i++)
                        {
                            string senderName = pr.senderName.GetValue(i).ToString();
                            prApiReceiver.Add(new ParamInsertOutgoingRecipient
                            {
                                senderName = senderName,
                                senderCompanyName = pr.senderCompanyName,
                                senderAddress = pr.senderAddress,
                                senderPhoneNumber = pr.senderPhoneNumber
                            });
                        }
                    }
                    prApi.senderAddress = pr.senderAddress;
                    prApi.senderPhoneNumber = pr.senderPhoneNumber;
                    prApi.isiSurat = pr.isiSurat;

                    if (pr.idUserChecker.Length > 0)
                    {
                        for (int a = 0; a < pr.idUserChecker.Length; a++)
                        {
                            Guid idChecker = new Guid(pr.idUserChecker.GetValue(a).ToString());
                            int idLevel = Convert.ToInt32(pr.idLevelChecker.GetValue(a).ToString());
                            Guid idUnitChecker = new Guid(pr.idUnitChecker.GetValue(a).ToString());
                            prApiChecker.Add(new ParamInsertChecker
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
                        prApiChecker.Add(new ParamInsertChecker
                        {
                            idUserChecker = pr.bossUserId,
                            idLevelChecker = pr.bossLevelId,
                            idUnitChecker = pr.bossUnitId
                        });
                    }
                    prApi.idUserChecker = prApiChecker;
                    prApi.senderName = prApiReceiver;
                }

                prApi.idresponsesurat = responseApiAttachment.idLetter == Guid.Empty ? pr.idresponsesurat : responseApiAttachment.idLetter;
                prApi.saveType = pr.saveType;
                prApi.comment = pr.comment;
                prApi.senderCompanyName = pr.senderCompanyName;

                generalOutput = await _dataAccessProvider.InsertLetterOutbox("Letter/InsertLetterOutbox", token, prApi);
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
                        await PrintLetter(idLetter);
                    }
                }
                if (pr.saveType != 1)
                {
                    return Redirect("~/Outbox/Index");
                }
                else
                {
                    return Redirect("~/Outbox/Draft");
                }


            }
            catch (Exception ex)
            {
                TempData["status"] = "NG";
                TempData["title"] = "Gagal membuat surat keluar";
                TempData["pesan"] = ex.ToString();
                return Redirect("~/Letter/NewOutboxCommissioner");
            }
        }

        [HttpPost]
        public async Task<string> SavePrevOutboxLetter(ParamInsertLetterOutboxWeb pr, IFormFile inputfile)
        {
            OutputPreviewLetter resultPreview = new OutputPreviewLetter();
            try
            {
                var token = HttpContext.Session.GetString("token");
                GeneralOutputModel generalOutput = new GeneralOutputModel();
                ParamInsertLetterOutbox prApi = new ParamInsertLetterOutbox();
                List<ParamInsertChecker> prApiChecker = new List<ParamInsertChecker>();
                List<ParamInsertOutgoingRecipient> prApiReceiver = new List<ParamInsertOutgoingRecipient>();
                OutputInsertAttachment responseApiAttachment = new OutputInsertAttachment();
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
                    prApi.senderAddress = pr.senderAddress;
                    prApi.isiSurat = pr.isiSurat;

                    if (pr.senderName.Length > 0)
                    {
                        for (int i = 0; i < pr.senderName.Length; i++)
                        {
                            string senderName = pr.senderName.GetValue(i).ToString();
                            prApiReceiver.Add(new ParamInsertOutgoingRecipient
                            {
                                senderName = senderName,
                                senderCompanyName = pr.senderCompanyName,
                                senderAddress = pr.senderAddress,
                                senderPhoneNumber = pr.senderPhoneNumber
                            });
                        }
                    }

                    if (pr.idUserChecker.Length > 0)
                    {
                        for (int a = 0; a < pr.idUserChecker.Length; a++)
                        {
                            Guid idChecker = new Guid(pr.idUserChecker.GetValue(a).ToString());
                            int idLevel = Convert.ToInt32(pr.idLevelChecker.GetValue(a).ToString());
                            Guid idUnitChecker = new Guid(pr.idUnitChecker.GetValue(a).ToString());
                            prApiChecker.Add(new ParamInsertChecker
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
                        prApiChecker.Add(new ParamInsertChecker
                        {
                            idUserChecker = pr.bossUserId,
                            idLevelChecker = pr.bossLevelId,
                            idUnitChecker = pr.bossUnitId
                        });
                    }
                    prApi.idUserChecker = prApiChecker;
                    prApi.senderName = prApiReceiver;
                }

                prApi.idresponsesurat = pr.idresponsesurat;
                prApi.saveType = pr.saveType;
                prApi.comment = pr.comment;
                prApi.senderCompanyName = pr.senderCompanyName;
                prApi.senderPhoneNumber = pr.senderPhoneNumber;

                generalOutput = await _dataAccessProvider.InsertLetterOutbox("Letter/InsertLetterOutbox", token, prApi);
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

        [HttpPost]
        public async Task<IActionResult> ApprovalLetter(ParamApprovalLetter pr, IFormFile inputfile)
        {
            try
            {
                var token = HttpContext.Session.GetString("token");
                GeneralOutputModel generalOutput = new GeneralOutputModel();
                generalOutput = await _dataAccessProvider.ApprovalLetter("Letter/ApprovalLetter", token, pr);
                var jsonApiResponseSerialize = JsonConvert.SerializeObject(generalOutput.Result);
                var titlePopup = "Surat berhasil disetujui";
                var pesanPopup = "Surat berhasil disetujui";
                if (pr.saveType == 5)
                {
                    titlePopup = "Surat berhasil ditolak";
                    pesanPopup = "Surat berhasil ditolak";
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
            catch (Exception ex)
            {
                TempData["status"] = "NG";
                TempData["title"] = "Gagal melakukan aksi pada surat keluar";
                TempData["pesan"] = ex.ToString();
                return Redirect("~/Inbox/Index");
            }
        }

        public async Task<FileResult> PrintLetter(Guid id)
        {
            var token = HttpContext.Session.GetString("token");
            GeneralOutputModel generalOutput = new GeneralOutputModel();
            OutputDetailLetter letterDetail = new OutputDetailLetter();
            ParamGetDetailLetter prLetter = new ParamGetDetailLetter();
            prLetter.idLetter = id;

            generalOutput = await _dataAccessProvider.GetDetailLetter("Letter/GetDetailLetterSK/", token, prLetter);
            var jsonApiResponseSerialize = JsonConvert.SerializeObject(generalOutput.Result);
            letterDetail = JsonConvert.DeserializeObject<OutputDetailLetter>(jsonApiResponseSerialize);

            var letternumber = letterDetail.letter.letterNumber;
            letternumber = letternumber.Replace(".", "_");
            var fileName = letternumber + ".pdf";
            if (letterDetail.letter.statusCode != 4)
            {
                fileName = "Draft_" + letterDetail.letter.about + ".pdf";
            }
            StringBuilder sb = new StringBuilder();
            sb.Append("<div style='padding-left:2px;'>" + letterDetail.letterContent.letterContent + "</div>");
            StringReader sr = new StringReader(sb.ToString());
            //Document pdfDoc = new Document(PageSize.A4, 25, 25, 25, 15);
            Document pdfDoc = new Document(PageSize.A4, 56, 56, 113, 97);
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
                Font underlineBold = FontFactory.GetFont(fontName, 12, Font.BOLD | Font.UNDERLINE, BaseColor.BLACK);

                PdfPTable table = new PdfPTable(3);
                float[] width = new float[] { 0.3f, 0.1f, 2f };
                table.WidthPercentage = 100;
                table.SetWidths(width);
                table.HorizontalAlignment = Element.ALIGN_RIGHT;
                table.DefaultCell.Border = 0;
                table.SpacingBefore = 0f;
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

                cell = new PdfPCell();
                cell.Border = 0;
                paragraph = new Paragraph("Lampiran", normal);
                paragraph.Alignment = Element.ALIGN_LEFT;
                cell.AddElement(paragraph);
                table.AddCell(cell);

                cell = new PdfPCell();
                cell.Border = 0;
                paragraph = new Paragraph(":", normal);
                paragraph.Alignment = Element.ALIGN_LEFT;
                cell.AddElement(paragraph);
                table.AddCell(cell);

                var lampiran = letterDetail.letter.attachmentDesc == null ? "-" : letterDetail.letter.attachmentDesc;
                cell = new PdfPCell();
                cell.Border = 0;
                paragraph = new Paragraph(lampiran, bold);
                paragraph.Alignment = Element.ALIGN_LEFT;
                cell.AddElement(paragraph);
                table.AddCell(cell);

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
                table.AddCell(new Phrase(letterDetail.outgoingRecipient[0].recipientCompany, bold));
                table.AddCell(new Phrase(letterDetail.outgoingRecipient[0].recipientAddress, normal));
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
                int no = 1;
                foreach (var item in letterDetail.outgoingRecipient)
                {
                    if (no == 1)
                    {
                        if (letterDetail.outgoingRecipient.Count() > 1)
                        {
                            table.AddCell(new Phrase("Up    " + no + ". " + item.recipientName, bold));
                        }
                        else
                        {
                            table.AddCell(new Phrase("Up    " + item.recipientName, bold));
                        }

                    }
                    else
                    {
                        table.AddCell(new Phrase("         " + no + ". " + item.recipientName, bold));
                    }
                    no++;
                }
                pdfDoc.Add(table);

                //table = new PdfPTable(1);
                //width = new float[] { 2f };
                //table.WidthPercentage = 100;
                //table.SetWidths(width);
                //table.HorizontalAlignment = Element.ALIGN_LEFT;
                //table.DefaultCell.Border = 0;
                //table.SpacingBefore = 20f;
                //table.SpacingAfter = 0f;

                //cell = new PdfPCell();
                //table.AddCell(new Phrase("Dengan hormat, ", normal));
                //pdfDoc.Add(table);

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

                //cell = new PdfPCell();
                //table.AddCell(new Phrase("Demikian kami sampaikan, Atas perhatian dan kerjasamanya, kami ucapkan terima kasih.", normal));
                //pdfDoc.Add(table);

                //table = new PdfPTable(1);
                //width = new float[] { 2f };
                //table.WidthPercentage = 100;
                //table.SetWidths(width);
                //table.HorizontalAlignment = Element.ALIGN_LEFT;
                //table.DefaultCell.Border = 0;
                //table.SpacingBefore = 20f;
                //table.SpacingAfter = 0f;

                //cell = new PdfPCell();
                //table.AddCell(new Phrase("Hormat kami", normal));
                //table.AddCell(new Phrase("PT BNI Life Insurance", normal));
                //pdfDoc.Add(table);

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


                string filename = letterDetail.sender[0].nip;

                if (letterDetail.letter.letterNumber != "NO_LETTER")
                {
                    QRCodeModel myQRCode = new QRCodeModel();
                    myQRCode.QRCodeText = "NIP : " + letterDetail.sender[0].nip + "\nNAMA : " + letterDetail.sender[0].fullname + "\nJabatan : " + letterDetail.sender[0].positionName;
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
                        table = new PdfPTable(1);
                        width = new float[] { 1f };
                        table.WidthPercentage = 20;
                        table.SetWidths(width);
                        table.HorizontalAlignment = Element.ALIGN_LEFT;
                        table.DefaultCell.Border = 0;
                        table.DefaultCell.PaddingBottom = 8;

                        string filePath = Path.Combine("wwwroot\\uploads\\imgsignature\\" + filename + ".png");
                        string filettd = Path.GetFileName(filePath);
                        string pathttd = $"{this.Request.Scheme}://{this.Request.Host}{this.Request.PathBase}" + "/uploads/imgsignature/" + filettd;

                        cell.Border = 0;
                        //Image imagettd = Image.GetInstance(pathttd);
                        Image imagettd = GetSignatureImage(filename);
                        //Image imagettd = GetSignatureImage(item.nip);
                        //cell.AddElement(imagettd);
                        if (imagettd != null)
                        {
                            //imagettd.ScaleToFit(2F, 2F);//Set width and height in float   
                            //cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                            cell.AddElement(imagettd);
                            //table.AddCell(imagettd);
                        }
                        else
                        {
                            var img = "";
                            paragraph = new Paragraph(img, normal);
                            cell.AddElement(paragraph);
                        }
                        
                        pdfDoc.Add(table);
                    }
                    else if (letterDetail.letter.signatureType == 2)
                    {
                        table = new PdfPTable(1);
                        width = new float[] { 1f };
                        table.WidthPercentage = 20;
                        table.SetWidths(width);
                        table.HorizontalAlignment = Element.ALIGN_LEFT;
                        table.DefaultCell.Border = 0;
                        table.DefaultCell.PaddingBottom = 8;

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
                        table = new PdfPTable(2);
                        width = new float[] { 1f, 1f };
                        table.WidthPercentage = 20;
                        table.SetWidths(width);
                        table.HorizontalAlignment = Element.ALIGN_LEFT;
                        table.DefaultCell.Border = 0;
                        table.DefaultCell.PaddingBottom = 8;

                        string filePath = Path.Combine("wwwroot\\uploads\\imgsignature\\" + filename + ".png");
                        string filettd = Path.GetFileName(filePath);
                        string pathttd = $"{this.Request.Scheme}://{this.Request.Host}{this.Request.PathBase}" + "/uploads/imgsignature/" + filettd;
                        //cell.Border = 0;
                        Image imagettd = Image.GetInstance(pathttd);
                        imagettd.ScaleToFit(2F, 2F);//Set width and height in float   
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

                cell = new PdfPCell();
                cell.Border = 0;
                paragraph = new Paragraph(letterDetail.sender[0].fullname, underlineBold);
                //table.AddCell(new Phrase(letterDetail.sender[0].fullname, underlineBold));
                cell.AddElement(paragraph);
                table.AddCell(cell);
                table.AddCell(new Phrase(letterDetail.sender[0].positionName, bold));
                pdfDoc.Add(table);

                pdfDoc.Close();

                var dateLetterFormat = Convert.ToDateTime(letterDetail.letter.letterDate).ToString("dd MMMM yyyy");
                var letterNumber = letterDetail.letter.letterNumber;
                byte[] bytess = ms.ToArray();
                byte[] bytes = AddPageNumbers(bytess, dateLetterFormat, letterNumber, "Surat Keluar");
                ms.Close();

                return File(bytes.ToArray(), "application/pdf", fileName);
            }
        }

        [HttpPost]
        public async Task<IActionResult> InsertDelivery(ParamInsertDeliveryWeb pr)
        {
            try
            {
                var token = HttpContext.Session.GetString("token");
                GeneralOutputModel generalOutput = new GeneralOutputModel();
                ParamInsertDelivery prApi = new ParamInsertDelivery();
                List<InsertListLetterDelivery> prApiLetter = new List<InsertListLetterDelivery>();
                List<Guid> LisId = pr.idLetter.Split(',').Select(Guid.Parse).ToList();
                for (int i = 0; i < LisId.Count(); i++)
                {
                    prApiLetter.Add(new InsertListLetterDelivery
                    {
                        idLetter = LisId[i]
                    });
                }
                prApi.idLetter = prApiLetter;
                prApi.shippingType = pr.shippingType;
                prApi.receiveDate = pr.receiveDate;
                prApi.expedition = pr.expedition;
                prApi.referenceNumber = pr.referenceNumber;
                prApi.receiptNumber = pr.receiptNumber;
                prApi.address = pr.address;
                prApi.deliveryType = pr.deliveryType;
                prApi.status = pr.status == null ? 1 : pr.status;
                prApi.receiverName = pr.receiverName;
                prApi.destination_receiver_name = pr.destination_receiver_name;
                prApi.receiverPhone = pr.receiverPhone;
                prApi.receiveDate = pr.receiveDate;

                generalOutput = await _dataAccessProvider.InsertDelivery("Letter/InsertDelivery", token, prApi);
                var jsonApiResponseSerialize = JsonConvert.SerializeObject(generalOutput.Result);

                if (generalOutput.Status == "OK")
                {
                    TempData["status"] = "OK";
                    TempData["title"] = "Data berhasil ditambahkan";
                    TempData["pesan"] = "Data berhasil ditambahkan";
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

                return Redirect("~/Letter/SKDelivery");

            }
            catch (Exception ex)
            {
                TempData["status"] = "NG";
                TempData["title"] = "Gagal menambahkan data";
                TempData["pesan"] = ex.ToString();
                return Redirect("~/Letter/SKDelivery");
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

        [HttpPost]
        public async Task<string> LetterNotifSecretary(Guid idLetter)
        {
            OutputDetailLetter letterDetail = new OutputDetailLetter();
            GeneralOutputModel generalOutput = new GeneralOutputModel();
            var token = HttpContext.Session.GetString("token");
            ParamGetDetailLetter pr = new ParamGetDetailLetter();
            pr.idLetter = idLetter;

            generalOutput = await _dataAccessProvider.GetDetailLetter("Letter/LetterNotifSecretary/", token, pr);
            var jsonApiResponseSerialize = JsonConvert.SerializeObject(generalOutput);

            return jsonApiResponseSerialize;
        }

        [HttpPost]
        public async Task<IActionResult> UpdateSKDelivery(ParamUpdateDeliveryWeb pr)
        {
            try
            {
                var token = HttpContext.Session.GetString("token");
                GeneralOutputModel generalOutput = new GeneralOutputModel();
                ParamUpdateDelivery prApi = new ParamUpdateDelivery();
                List<InsertListLetterDelivery> prApiLetter = new List<InsertListLetterDelivery>();
                List<Guid> LisId = pr.idLetter.Split(',').Select(Guid.Parse).ToList();
                for (int i = 0; i < LisId.Count(); i++)
                {
                    prApiLetter.Add(new InsertListLetterDelivery
                    {
                        idLetter = LisId[i]
                    });
                }
                prApi.saveType = 1;
                prApi.idDelivery = pr.idDelivery;
                prApi.idLetter = prApiLetter;
                prApi.shippingType = pr.shippingType;
                prApi.receiveDate = pr.receiveDate;
                prApi.expedition = pr.expedition;
                prApi.referenceNumber = pr.referenceNumber;
                prApi.receiptNumber = pr.receiptNumber;
                prApi.address = pr.address;
                prApi.deliveryType = pr.deliveryType;
                prApi.status = pr.status == null ? 1 : pr.status;
                prApi.receiverName = pr.receiverName;
                prApi.destination_receiver_name = pr.destination_receiver_name;
                prApi.receiverPhone = pr.receiverPhone;

                generalOutput = await _dataAccessProvider.UpdateDeliveryEoffice("Letter/UpdateDeliveryEoffice", token, prApi);
                var jsonApiResponseSerialize = JsonConvert.SerializeObject(generalOutput.Result);

                if (generalOutput.Status == "OK")
                {
                    TempData["status"] = "OK";
                    TempData["title"] = "Data berhasil dirubah";
                    TempData["pesan"] = "Data berhasil dirubah";
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

                return Redirect("~/Letter/SKDelivery");

            }
            catch (Exception ex)
            {
                TempData["status"] = "NG";
                TempData["title"] = "Gagal merubah data";
                TempData["pesan"] = ex.ToString();
                return Redirect("~/Letter/SKDelivery");
            }
        }

        [HttpPost]
        public async Task<IActionResult> UploadEkspedisiEoffice(IFormFile inputfile)
        {
            try
            {
                var token = HttpContext.Session.GetString("token");
                GeneralOutputModel generalOutput = new GeneralOutputModel();
                List<ParamUploadEkspedisiEoffice> data = new List<ParamUploadEkspedisiEoffice>();
                ParamUploadEkspedisiEofficeString pr = new ParamUploadEkspedisiEofficeString();
                List<UploadEkspedisiEofficeOutput> responseApi = new List<UploadEkspedisiEofficeOutput>();
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
                        string letterNumber = row["Nomor Surat"].ToString();
                        string sender = row["Pengirim"].ToString();
                        string senderDivision = row["Divisi"].ToString();
                        string expedition = row["Nama Ekspedisi"].ToString();
                        string referenceNumber = row["Nomor Referensi"] == null ? "" : row["Nomor Referensi"].ToString();
                        string receiptNumber = row["Nomor AWB/Pengiriman"] == null ? "" : row["Nomor AWB/Pengiriman"].ToString();
                        string destinatin_receiver = row["Nama Tujuan"] == null ? "" : row["Nama Tujuan"].ToString();
                        string receiver = row["Nama Penerima"].ToString();
                        string receiverAddress = row["Alamat Tujuan"].ToString();
                        string receiveDate = row["Tanggal Penerimaan"] == null ? null : row["Tanggal Penerimaan"].ToString();
                        string shippingType = row["Jenis Pengiriman"].ToString();
                        string deliveryType = row["Tipe Pengiriman"] == null ? "" : row["Tipe Pengiriman"].ToString();
                        string status = row["Status Pengiriman"].ToString();
                        data.Add(new ParamUploadEkspedisiEoffice
                        {
                            letterNumber = letterNumber,
                            sender = sender,
                            senderDivision = senderDivision,
                            expedition = expedition,
                            referenceNumber = referenceNumber,
                            receiptNumber = receiptNumber,
                            receiver = receiver,
                            destination_receiver_name = destinatin_receiver,
                            receiverAddress = receiverAddress,
                            receiveDate = receiveDate == null ? null : DateTime.Parse(receiveDate),
                            shippingTypeCodeValue = shippingType,
                            deliveryTypeCodeValue = deliveryType,
                            statusCodeValue = status
                        });
                    }
                    pr.jsonDataString = data;
                    generalOutput = await _dataAccessProvider.PostUploadExpeditionEoffice("Letter/UploadExpeditionEoffice", token, pr);
                    var jsonApiResponseSerialize = JsonConvert.SerializeObject(generalOutput.Result);
                    responseApi = JsonConvert.DeserializeObject<List<UploadEkspedisiEofficeOutput>>(jsonApiResponseSerialize);

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
                        TempData["title"] = "Gagal merubah pengiriman surat";
                        TempData["pesan"] = generalOutput.Message;
                    }
                    TempData["OutputApi"] = jsonApiResponseSerialize;

                    return Redirect("~/Letter/SKDelivery");
                }

                TempData["status"] = "NG";
                TempData["pesan"] = "Siliahkan pilih file yang akan di upload";

                return Redirect("~/Letter/SKDelivery");
            }
            catch (Exception ex)
            {
                TempData["status"] = "NG";
                TempData["title"] = "Gagal merubah pengiriman surat";
                TempData["pesan"] = ex.ToString();
                return Redirect("~/Letter/SKDelivery");
            }
        }


        [HttpPost]
        public async Task<string> SearchDeliveryEoffice(DateTime? startDate, DateTime? endDate)
        {
            List<DeliveryReportOutput> output = new List<DeliveryReportOutput>();
            GeneralOutputModel generalOutput = new GeneralOutputModel();
            List<Dictionary<String, String>> _list = new List<Dictionary<string, string>>();
            var token = HttpContext.Session.GetString("token");

            ParamGetReportEkspedisiEoffice pr = new ParamGetReportEkspedisiEoffice();
            pr.statusElse = 0;
            pr.startDate = startDate;
            pr.endDate = endDate;
            generalOutput = await _dataAccessProvider.GetReportDeliveryEoffice("Letter/DeliveryLetterReportList/", token, pr);
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

        [HttpPost]
        public async Task<string> SearchLetter(ParamGetLetterWeb pr)
        {
            LetterOutputWeb letterList = new LetterOutputWeb();
            GeneralOutputModel generalOutput = new GeneralOutputModel();
            var token = HttpContext.Session.GetString("token");
            int draw = Convert.ToInt32(pr.draw);
            pr.pageSize = 20;
            pr.start = (draw - 1) * pr.pageSize;
            pr.draw = pr.start == 0 ? "1" : Convert.ToString(pr.start + 1);
            var urlApi = "Letter/GetInbox/";
            if (pr.searchType == 2)
            {
                urlApi = "Letter/GetOutbox";
            }
            else if (pr.searchType == 3)
            {
                urlApi = "Letter/GetDraftLetter";
            }
            else if (pr.searchType == 4)
            {
                urlApi = "Letter/GetDraftLetter";
            }
            else if (pr.searchType == 5)
            {
                urlApi = "Letter/GetDraftLetter";
            }

            generalOutput = await _dataAccessProvider.GetLetterDraft(urlApi, token, pr);
            var jsonApiResponseSerialize = JsonConvert.SerializeObject(generalOutput.Result);
            letterList = JsonConvert.DeserializeObject<LetterOutputWeb>(jsonApiResponseSerialize);
            jsonApiResponseSerialize = JsonConvert.SerializeObject(letterList);

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
            sb.Append("<div style='padding-left:2px;'>" + letterDetail.letterContent.letterContent + "</div>");
            StringReader sr = new StringReader(sb.ToString());
            //Document pdfDoc = new Document(PageSize.A4, 25, 25, 25, 15);
            Document pdfDoc = new Document(PageSize.A4, 56, 56, 113, 97);
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

                cell = new PdfPCell();
                cell.BorderWidthLeft = 1f;
                cell.BorderWidthRight = 1f;
                cell.BorderWidthTop = 1f;
                cell.BorderWidthBottom = 1f;
                cell.MinimumHeight = 30f;
                paragraph = new Paragraph("Nomor Registrasi Memo/Registration Number:" + letterDetail.letter.letterDeliberationNumber, normalDeliberation);
                paragraph.Alignment = Element.ALIGN_LEFT;
                cell.AddElement(paragraph);
                table.AddCell(cell);

                cell = new PdfPCell();
                cell.BorderWidthLeft = 0f;
                cell.BorderWidthRight = 1f;
                cell.BorderWidthTop = 1f;
                cell.BorderWidthBottom = 1f;
                cell.MinimumHeight = 30f;
                paragraph = new Paragraph("Tanggal Registrasi Memo/Date of Memo Registration : " + Convert.ToDateTime(letterDetail.letter.letterDate).ToString("dd MMMM yyyy"), normalDeliberation);
                paragraph.Alignment = Element.ALIGN_LEFT;
                cell.AddElement(paragraph);
                table.AddCell(cell);

                cell = new PdfPCell();
                cell.BorderWidthLeft = 1f;
                cell.BorderWidthRight = 1f;
                cell.BorderWidthTop = 0f;
                cell.BorderWidthBottom = 1f;
                cell.MinimumHeight = 100f;
                paragraph = new Paragraph("Nomor Memo : " + letterDetail.letter.letterNumber, normalDeliberation);
                paragraph.Alignment = Element.ALIGN_LEFT;
                cell.AddElement(paragraph);
                table.AddCell(cell);

                cell = new PdfPCell();
                cell.BorderWidthLeft = 0f;
                cell.BorderWidthRight = 1f;
                cell.BorderWidthTop = 0f;
                cell.BorderWidthBottom = 1f;
                cell.MinimumHeight = 100f;
                paragraph = new Paragraph("(Ringkasan Memo/Summary of the proposal) \n", normalDeliberation);
                paragraph.Alignment = Element.ALIGN_LEFT;
                cell.AddElement(paragraph);
                table.AddCell(cell);

                cell = new PdfPCell();
                cell.BorderWidthLeft = 1f;
                cell.BorderWidthRight = 1f;
                cell.BorderWidthTop = 0f;
                cell.BorderWidthBottom = 1f;
                cell.MinimumHeight = 100f;
                paragraph = new Paragraph("Deliberation Column:", normalDeliberation);
                paragraph.Alignment = Element.ALIGN_LEFT;
                cell.AddElement(paragraph);
                int no = 1;
                foreach (var item in letterDetail.delibration)
                {
                    if (letterDetail.receiver.Count() > 1)
                    {
                        paragraph = new Paragraph(no + ". " + item.fullname + " - " + item.positionName, normalDeliberation);
                        paragraph.Alignment = Element.ALIGN_LEFT;
                        cell.AddElement(paragraph);
                    }
                    else
                    {
                        paragraph = new Paragraph(item.fullname + " - " + item.positionName, normalDeliberation);
                        paragraph.Alignment = Element.ALIGN_LEFT;
                        cell.AddElement(paragraph);
                    }
                    no++;
                }
                table.AddCell(cell);

                cell = new PdfPCell();
                cell.BorderWidthLeft = 0f;
                cell.BorderWidthRight = 1f;
                cell.BorderWidthTop = 0f;
                cell.BorderWidthBottom = 1f;
                cell.MinimumHeight = 100f;
                paragraph = new Paragraph("catatan/notes:", underlineDeliberation);
                paragraph.Alignment = Element.ALIGN_LEFT;
                cell.AddElement(paragraph);
                no = 1;
                foreach (var item in letterDetail.delibration)
                {
                    if (letterDetail.receiver.Count() > 1)
                    {
                        paragraph = new Paragraph(no + ". " + item.commentdlbrt, normalDeliberation);
                        paragraph.Alignment = Element.ALIGN_LEFT;
                        cell.AddElement(paragraph);
                    }
                    else
                    {
                        paragraph = new Paragraph(item.commentdlbrt, normalDeliberation);
                        paragraph.Alignment = Element.ALIGN_LEFT;
                        cell.AddElement(paragraph);
                    }
                    no++;
                }
                table.AddCell(cell);

                pdfDoc.Add(table);

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
                    table = new PdfPTable(3);
                    width = new float[] { 1f, 2f, 1f };
                    table.WidthPercentage = 100;
                    table.SetWidths(width);
                    table.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.DefaultCell.Border = 0;
                    table.SpacingBefore = 0f;
                    table.SpacingAfter = 0f;

                    cell = new PdfPCell();
                    cell.Rowspan = 2;
                    cell.BorderWidthLeft = 1f;
                    cell.BorderWidthRight = 1f;
                    cell.BorderWidthTop = 0f;
                    cell.BorderWidthBottom = 1f;
                    cell.MinimumHeight = 120f;
                    paragraph = new Paragraph("Disetujui \n", normalDeliberation);
                    paragraph.Alignment = Element.ALIGN_LEFT;
                    cell.AddElement(paragraph);
                    paragraph = new Paragraph(item.positionName, normalDeliberation);
                    paragraph.Alignment = Element.ALIGN_LEFT;
                    cell.AddElement(paragraph);
                    table.AddCell(cell);

                    cell = new PdfPCell();
                    cell.Rowspan = 2;
                    cell.BorderWidthLeft = 0f;
                    cell.BorderWidthRight = 1f;
                    cell.BorderWidthTop = 0f;
                    cell.BorderWidthBottom = 1f;
                    cell.MinimumHeight = 120f;
                    paragraph = new Paragraph("catatan/notes : \n", underlineDeliberation);
                    paragraph.Alignment = Element.ALIGN_LEFT;
                    cell.AddElement(paragraph);
                    paragraph = new Paragraph(comment, normalDeliberation);
                    paragraph.Alignment = Element.ALIGN_LEFT;
                    cell.AddElement(paragraph);
                    table.AddCell(cell);

                    cell = new PdfPCell();
                    cell.BorderWidthLeft = 0f;
                    cell.BorderWidthRight = 1f;
                    cell.BorderWidthTop = 0f;
                    cell.BorderWidthBottom = 1f;
                    cell.MinimumHeight = 40f;
                    paragraph = new Paragraph("Tanggal : ", underlineDeliberation);
                    paragraph.Alignment = Element.ALIGN_LEFT;
                    cell.AddElement(paragraph);
                    paragraph = new Paragraph(approveDate, normalDeliberation);
                    paragraph.Alignment = Element.ALIGN_LEFT;
                    cell.AddElement(paragraph);
                    table.AddCell(cell);

                    cell = new PdfPCell();
                    cell.BorderWidthLeft = 0f;
                    cell.BorderWidthRight = 1f;
                    cell.BorderWidthTop = 0f;
                    cell.BorderWidthBottom = 1f;
                    paragraph = new Paragraph("Tanda Tangan : \n", underlineDeliberation);
                    paragraph.Alignment = Element.ALIGN_LEFT;
                    cell.AddElement(paragraph);
                    if (letterDetail.letter.letterNumber != "NO_LETTER")
                    {
                        if (letterDetail.letter.signatureType == 1)
                        {
                            Image imagettd = GetSignatureImage(item.nip);
                            cell.AddElement(imagettd);
                        }
                        else if (letterDetail.letter.signatureType == 2)
                        {
                            Image QRCodeSignatureApprover = GetBarcodeSignature("NIP : " + item.nip + "\nNAMA : " + item.fullname + "\nJabatan : " + item.positionName);
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
                            //innertable = new PdfPTable(2);
                            //float[] widthinner = new float[] { 0.3f, 0.3f };
                            //innertable.WidthPercentage = 100;
                            //innertable.SetWidths(widthinner);
                            //innertable.HorizontalAlignment = Element.ALIGN_LEFT;
                            //innertable.DefaultCell.Border = 1;
                            //innertable.DefaultCell.PaddingBottom = 8;

                            //innercell = new PdfPCell();
                            //innercell.BorderWidthLeft = 1f;
                            //innercell.BorderWidthRight = 1f;
                            //innercell.BorderWidthTop = 1f;
                            //innercell.BorderWidthBottom = 1f;

                            Image imagettd = GetSignatureImage(item.nip);
                            cell.AddElement(imagettd);
                            Image QRCodeSignatureApprover = GetBarcodeSignature("NIP : " + item.nip + "\nNAMA : " + item.fullname + "\nJabatan : " + item.positionName);
                            cell.AddElement(QRCodeSignatureApprover);
                        }
                    }

                    paragraph = new Paragraph(item.fullname, underlineDeliberationBold);
                    paragraph.Alignment = Element.ALIGN_LEFT;
                    cell.AddElement(paragraph);
                    table.AddCell(cell);


                    pdfDoc.Add(table);
                }



                //add new page
                pdfDoc.NewPage();

                table = new PdfPTable(1);
                width = new float[] { 2f };
                table.WidthPercentage = 100;
                table.SetWidths(width);
                table.HorizontalAlignment = Element.ALIGN_LEFT;
                table.DefaultCell.Border = 0;
                table.SpacingBefore = 00f;
                table.SpacingAfter = 0f;
                if (letterDetail.letter.idmemoType.ToString().ToUpper() == "726A2598-2290-ED11-80D1-B7157F320A73") //memo direksi
                {
                    table.AddCell(new Phrase("MEMO", bold));
                    pdfDoc.Add(table);

                    table = new PdfPTable(3);
                    width = new float[] { 0.3f, 0.1f, 2f };
                    table.WidthPercentage = 100;
                    table.SetWidths(width);
                    table.HorizontalAlignment = Element.ALIGN_RIGHT;
                    table.DefaultCell.Border = 0;
                    table.SpacingBefore = 0f;
                    table.SpacingAfter = 0f;

                    cell = new PdfPCell();
                    cell.Border = 0;
                    paragraph = new Paragraph("Nomor", normal);
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
                    paragraph = new Paragraph("Tanggal", normal);
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
                    paragraph = new Paragraph(Convert.ToDateTime(letterDetail.letter.letterDate).ToString("dd MMMM yyyy"), normal);
                    paragraph.Alignment = Element.ALIGN_LEFT;
                    cell.AddElement(paragraph);
                    table.AddCell(cell);

                    cell = new PdfPCell();
                    cell.Border = 0;
                    paragraph = new Paragraph("Kepada", normal);
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
                    no = 1;
                    foreach (var item in letterDetail.receiver)
                    {
                        if (letterDetail.receiver.Count() > 1)
                        {
                            paragraph = new Paragraph(no + ". " + item.fullname + " - " + item.positionName, normal);
                            paragraph.Alignment = Element.ALIGN_LEFT;
                            cell.AddElement(paragraph);
                        }
                        else
                        {
                            paragraph = new Paragraph(item.fullname + " - " + item.positionName, normal);
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

                    cell = new PdfPCell();
                    cell.Border = 0;
                    paragraph = new Paragraph(":", normal);
                    paragraph.Alignment = Element.ALIGN_LEFT;
                    cell.AddElement(paragraph);
                    table.AddCell(cell);

                    cell = new PdfPCell();
                    cell.Border = 0;
                    no = 1;
                    foreach (var item in letterDetail.copy)
                    {
                        if (letterDetail.copy.Count() > 1)
                        {
                            paragraph = new Paragraph(no + ". " + item.fullname + " - " + item.positionName, normal);
                            paragraph.Alignment = Element.ALIGN_LEFT;
                            cell.AddElement(paragraph);
                        }
                        else
                        {
                            paragraph = new Paragraph(item.fullname + " - " + item.positionName, normal);
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

                    cell = new PdfPCell();
                    cell.Border = 0;
                    paragraph = new Paragraph(":", normal);
                    paragraph.Alignment = Element.ALIGN_LEFT;
                    cell.AddElement(paragraph);
                    table.AddCell(cell);

                    cell = new PdfPCell();
                    cell.Border = 0;
                    paragraph = new Paragraph(letterDetail.sender[0].fullname + " - " + letterDetail.sender[0].positionName, normal);
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

                    cell = new PdfPCell();
                    cell.Border = 0;
                    paragraph = new Paragraph("Lampiran", normal);
                    paragraph.Alignment = Element.ALIGN_LEFT;
                    cell.AddElement(paragraph);
                    table.AddCell(cell);

                    cell = new PdfPCell();
                    cell.Border = 0;
                    paragraph = new Paragraph(":", normal);
                    paragraph.Alignment = Element.ALIGN_LEFT;
                    cell.AddElement(paragraph);
                    table.AddCell(cell);

                    var lampiran = letterDetail.letter.attachmentDesc == null ? "-" : letterDetail.letter.attachmentDesc;
                    cell = new PdfPCell();
                    cell.Border = 0;
                    paragraph = new Paragraph(lampiran, bold);
                    paragraph.Alignment = Element.ALIGN_LEFT;
                    cell.AddElement(paragraph);
                    table.AddCell(cell);

                    //Add table to document
                    pdfDoc.Add(table);
                }
                else if (letterDetail.letter.idmemoType.ToString().ToUpper() == "D8D86FAE-2290-ED11-80D1-B7157F320A73") //Surat Keputusan Direksi
                {
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
                }

                Paragraph line = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(2F, 100.0F, colorBlack, Element.ALIGN_CENTER, 1)));
                line.SpacingBefore = 0f;
                line.SetLeading(2F, 0.5F);
                pdfDoc.Add(line);
                Paragraph lines = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(2F, 100.0F, colorBlack, Element.ALIGN_CENTER, 1)));
                lines.SpacingBefore = 0f;
                line.SetLeading(0.5F, 0.5F);
                pdfDoc.Add(line);

                //ADD HTML CONTENT
                htmlparser.Parse(sr);


                if (letterDetail.letter.idmemoType.ToString().ToUpper() == "D8D86FAE-2290-ED11-80D1-B7157F320A73") //Surat Keputusan Direksi
                {
                    table = new PdfPTable(3);
                    width = new float[] { 0.5f, 0.1f, 2f };
                    table.WidthPercentage = 100;
                    table.SetWidths(width);
                    table.HorizontalAlignment = Element.ALIGN_RIGHT;
                    table.DefaultCell.Border = 0;
                    table.SpacingBefore = 0f;
                    table.SpacingAfter = 0f;

                    cell = new PdfPCell();
                    cell.Border = 0;
                    paragraph = new Paragraph("Ditetapkan Di", normal);
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
                    paragraph = new Paragraph("Jakarta", normal);
                    paragraph.Alignment = Element.ALIGN_LEFT;
                    cell.AddElement(paragraph);
                    table.AddCell(cell);

                    cell = new PdfPCell();
                    cell.Border = 0;
                    paragraph = new Paragraph("Pada Tanggal", normal);
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
                    paragraph = new Paragraph(Convert.ToDateTime(letterDetail.letter.letterDate).ToString("dd MMMM yyyy"), normal);
                    paragraph.Alignment = Element.ALIGN_LEFT;
                    cell.AddElement(paragraph);
                    table.AddCell(cell);
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
                    table.AddCell(new Phrase("Direksi PT BNI Life Insurance", normal));
                    pdfDoc.Add(table);
                }



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


                string filename = letterDetail.sender[0].nip;

                if (letterDetail.letter.idmemoType.ToString().ToUpper() != "D8D86FAE-2290-ED11-80D1-B7157F320A73")//surat keputusan direksi
                {
                    if (letterDetail.letter.letterNumber != "NO_LETTER")
                    {
                        Image imagettdqrcode = GetBarcodeSignature("NIP : " + letterDetail.sender[0].nip + "\nNAMA : " + letterDetail.sender[0].fullname + "\nJabatan : " + letterDetail.sender[0].positionName);

                        if (letterDetail.letter.signatureType == 1)
                        {
                            table = new PdfPTable(1);
                            width = new float[] { 1f };
                            table.WidthPercentage = 20;
                            table.SetWidths(width);
                            table.HorizontalAlignment = Element.ALIGN_LEFT;
                            table.DefaultCell.Border = 0;
                            table.DefaultCell.PaddingBottom = 8;

                            cell.Border = 0;
                            Image imagettd = GetSignatureImage(filename);
                            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                            cell.AddElement(imagettd);
                            table.AddCell(imagettd);
                            pdfDoc.Add(table);
                        }
                        else if (letterDetail.letter.signatureType == 2)
                        {
                            table = new PdfPTable(1);
                            width = new float[] { 1f };
                            table.WidthPercentage = 20;
                            table.SetWidths(width);
                            table.HorizontalAlignment = Element.ALIGN_LEFT;
                            table.DefaultCell.Border = 0;
                            table.DefaultCell.PaddingBottom = 8;


                            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                            cell.AddElement(imagettdqrcode);
                            table.AddCell(imagettdqrcode);
                            pdfDoc.Add(table);
                        }
                        else if (letterDetail.letter.signatureType == 3)
                        {
                            table = new PdfPTable(2);
                            width = new float[] { 1f, 1f };
                            table.WidthPercentage = 20;
                            table.SetWidths(width);
                            table.HorizontalAlignment = Element.ALIGN_LEFT;
                            table.DefaultCell.Border = 0;
                            table.DefaultCell.PaddingBottom = 8;

                            //Image imagettd = GetSignatureImage(filename);
                            //cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                            //cell.AddElement(imagettd);
                            //table.AddCell(imagettd);

                            var img = "";
                            paragraph = new Paragraph(img, normal);
                            cell.AddElement(paragraph);

                            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                            cell.AddElement(imagettdqrcode);
                            table.AddCell(imagettdqrcode);
                            pdfDoc.Add(table);
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
                    }

                    table = new PdfPTable(1);
                    width = new float[] { 2f };
                    table.WidthPercentage = 100;
                    table.SetWidths(width);
                    table.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.DefaultCell.Border = 0;

                    cell = new PdfPCell();
                    cell.Border = 0;
                    paragraph = new Paragraph(letterDetail.sender[0].fullname, underlineBold);
                    cell.AddElement(paragraph);
                    table.AddCell(cell);
                    table.AddCell(new Phrase(letterDetail.sender[0].positionName, bold));
                    pdfDoc.Add(table);
                }
                else
                {
                    var dirChecker = letterDetail.checker.Where(p=>p.idLevelChecker == 2).ToList();
                    table = new PdfPTable(dirChecker.Count());
                    width = new float[dirChecker.Count()];
                    for (int i =0;i<dirChecker.Count();i++)
                    {
                        width[i] = 1f;
                    }
                    table.WidthPercentage = 100;
                    table.SetWidths(width);
                    table.HorizontalAlignment = Element.ALIGN_LEFT;
                    table.DefaultCell.Border = 0;

                    foreach (var item in dirChecker)
                    {
                        cell = new PdfPCell();
                        cell.Border = 0;
                        if (letterDetail.letter.letterNumber != "NO_LETTER")
                        {
                            if (letterDetail.letter.signatureType == 1)
                            {
                                Image imagettd = GetSignatureImage(item.nip);
                                if (imagettd != null)
                                {
                                    cell.AddElement(imagettd);
                                }
                                else
                                {
                                    var img = "";
                                    paragraph = new Paragraph(img, normal);
                                    cell.AddElement(paragraph);
                                }
                                //cell.AddElement(imagettd);
                            }
                            else if (letterDetail.letter.signatureType == 2)
                            {
                                Image QRCodeSignatureApprover = GetBarcodeSignature("NIP : " + item.nip + "\nNAMA : " + item.fullname + "\nJabatan : " + item.positionName);
                                cell.AddElement(QRCodeSignatureApprover);
                            }
                            else
                            {
                                Image imagettd = GetSignatureImage(item.nip);
                                cell.AddElement(imagettd);
                                Image QRCodeSignatureApprover = GetBarcodeSignature("NIP : " + item.nip + "\nNAMA : " + item.fullname + "\nJabatan : " + item.positionName);
                                cell.AddElement(QRCodeSignatureApprover);
                            }
                        }

                        paragraph = new Paragraph(item.fullname, underlineBold);
                        cell.AddElement(paragraph);
                        paragraph = new Paragraph(item.positionName, bold);
                        cell.AddElement(paragraph);

                        table.AddCell(cell);
                    }
                    pdfDoc.Add(table);
                }

                pdfDoc.Close();

                var dateLetterFormat = Convert.ToDateTime(letterDetail.letter.letterDate).ToString("dd MMMM yyyy");
                var letterNumber = letterDetail.letter.letterNumber;
                byte[] bytess = ms.ToArray();
                byte[] bytes = AddPageNumbers(bytess, dateLetterFormat, letterNumber, "Memo");
                ms.Close();

                return File(bytes.ToArray(), "application/pdf", fileName);
            }


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
                Bitmap qrCodeImage = qRCode.GetGraphic(3);
                byte[] BitmapArray = qrCodeImage.ConvertBitmapToByteArray();
                string urlImgQrcode = string.Format("data:image/png;base64,{0}", Convert.ToBase64String(BitmapArray));
                QRCode = urlImgQrcode;
            }

            Regex regex = new Regex(@"^data:image/(?<mediaType>[^;]+);base64,(?<data>.*)");
            Match match = regex.Match(QRCode);
            Image imagettdqrcode = Image.GetInstance(
                Convert.FromBase64String(match.Groups["data"].Value)
            );
            imagettdqrcode.ScaleAbsolute(120f, 155.25f);
            return imagettdqrcode;
        }
        
        //public Image GetSignatureImage(string filename)
        //{
        //    string filePath = Path.Combine("wwwroot\\uploads\\imgsignature\\" + filename + ".png");
        //    string filettd = Path.GetFileName(filePath);
        //    string pathttd = $"{this.Request.Scheme}://{this.Request.Host}{this.Request.PathBase}" + "/uploads/imgsignature/" + filettd;
        //    //cell.Border = 0;
        //    Image imagettd = Image.GetInstance(pathttd);
        //    //imagettd.ScaleToFit(2F, 2F);//Set width and height in float  

        //    return imagettd;
        //}

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

        [HttpPost]
        public async Task<IActionResult> SaveOutboxLetterBackdate(ParamInsertLetterOutboxBackdateWeb pr, IFormFile inputfile)
        {
            try
            {
                var token = HttpContext.Session.GetString("token");
                GeneralOutputModel generalOutput = new GeneralOutputModel();
                ParamInsertLetterOutboxBackdate prApi = new ParamInsertLetterOutboxBackdate();
                List<ParamInsertChecker> prApiChecker = new List<ParamInsertChecker>();
                List<ParamInsertOutgoingRecipient> prApiReceiver = new List<ParamInsertOutgoingRecipient>();
                List<ParamInsertApprover> prApiApprover = new List<ParamInsertApprover>();
                ParamInsertAttachment prAttachment = new ParamInsertAttachment();
                OutputInsertAttachment responseApiAttachment = new OutputInsertAttachment();
                //List<ParamInsertCheckerPengirim> prApiCheckerPengirim = new List<ParamInsertCheckerPengirim>();
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

                        generalOutput = await _dataAccessProvider.InsertAttachmentLetter("Letter/InsertAttachment", token, prAttachment);
                        var jsonApiResponseSerializes = JsonConvert.SerializeObject(generalOutput.Result);
                        responseApiAttachment = JsonConvert.DeserializeObject<OutputInsertAttachment>(jsonApiResponseSerializes);
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
                    prApi.letterTypeCode = pr.letterTypeCode;
                    prApi.letterDate = pr.letterDate;
                    prApi.about = pr.about;
                    prApi.resultType = pr.resultType;
                    prApi.signatureType = pr.signatureType;
                    prApi.bossPositionId = pr.bossPositionId;
                    prApi.bossUserId = pr.bossUserId;
                    prApi.bossUnitId = pr.bossUnitId;
                    prApi.bossLevelId = pr.bossLevelId;
                    prApi.bossPositionName = pr.bossPositionName;
                    prApi.attachmentCount = pr.attachmentCount;
                    prApi.priority = pr.priority;
                    //prApi.senderName = pr.senderName;
                    if (pr.senderName.Length > 0)
                    {
                        for (int i = 0; i < pr.senderName.Length; i++)
                        {
                            string senderName = pr.senderName.GetValue(i).ToString();
                            prApiReceiver.Add(new ParamInsertOutgoingRecipient
                            {
                                senderName = senderName,
                                senderCompanyName = pr.senderCompanyName,
                                senderAddress = pr.senderAddress,
                                senderPhoneNumber = pr.senderPhoneNumber
                            });
                        }
                    }
                    prApi.senderAddress = pr.senderAddress;
                    prApi.senderPhoneNumber = pr.senderPhoneNumber;
                    prApi.isiSurat = pr.isiSurat;

                    if (pr.idUserChecker.Length > 0)
                    {
                        for (int a = 0; a < pr.idUserChecker.Length; a++)
                        {
                            Guid idChecker = new Guid(pr.idUserChecker.GetValue(a).ToString());
                            int idLevel = Convert.ToInt32(pr.idLevelChecker.GetValue(a).ToString());
                            Guid idUnitChecker = new Guid(pr.idUnitChecker.GetValue(a).ToString());
                            prApiChecker.Add(new ParamInsertChecker
                            {
                                idUserChecker = idChecker,
                                idLevelChecker = idLevel,
                                idUnitChecker = idUnitChecker
                            });
                        }
                    }
                    //var checksenderischecker = prApiChecker.Where(p => p.idUserChecker == pr.bossUserId).FirstOrDefault();
                    //if (checksenderischecker == null)
                    //{
                    //    prApiChecker.Add(new ParamInsertChecker
                    //    {
                    //        idUserChecker = pr.bossUserId,
                    //        idLevelChecker = pr.bossLevelId,
                    //        idUnitChecker = pr.bossUnitId
                    //    });
                    //}
                    prApi.idUserChecker = prApiChecker;
                    prApi.senderName = prApiReceiver;
                    var checksenderischecker = prApiChecker.Where(p => p.idUserChecker == pr.bossUserId).FirstOrDefault();
                    if (checksenderischecker == null)
                    {
                        prApiChecker.Add(new ParamInsertChecker
                        {
                            idUserChecker = pr.bossUserId,
                            idLevelChecker = pr.bossLevelId,
                            idUnitChecker = pr.bossUnitId
                        });
                    }

                    if (pr.idUserApprover.Length > 0)
                    {
                        for (int a = 0; a < pr.idUserApprover.Length; a++)
                        {
                            Guid idApprover = new Guid(pr.idUserApprover.GetValue(a).ToString());
                            int idLevel = Convert.ToInt32(pr.idLevelApprover.GetValue(a).ToString());
                            Guid idUnitApprover = new Guid(pr.idUnitApprover.GetValue(a).ToString());
                            prApiApprover.Add(new ParamInsertApprover
                            {
                                idUserApprover = idApprover,
                                idLevelApprover = idLevel,
                                idUnitApprover = idUnitApprover,
                                is_approver = 1,
                            });
                        }
                    }


                    //prApi.idUserCheckerPengirim = prApiCheckerPengirim;
                     prApi.idUserApprover = prApiApprover;
                }


               
                prApi.idresponsesurat = responseApiAttachment.idLetter == Guid.Empty ? pr.idresponsesurat : responseApiAttachment.idLetter;
                prApi.saveType = pr.saveType;
                prApi.comment = pr.comment;
                prApi.senderCompanyName = pr.senderCompanyName;

                generalOutput = await _dataAccessProvider.InsertLetterOutboxBackdate("Letter/InsertLetterOutboxBackdate", token, prApi);
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
                        await PrintLetter(idLetter);
                    }
                }
                if (pr.saveType != 1)
                {
                    return Redirect("~/Outbox/Index");
                }
                else
                {
                    return Redirect("~/Outbox/Draft");
                }


            }
            catch (Exception ex)
            {
                TempData["status"] = "NG";
                TempData["title"] = "Gagal membuat surat keluar";
                TempData["pesan"] = ex.ToString();
                return Redirect("~/Letter/NewOutboxCommissioner");
            }
        }

        [Route("Letter/DetailLetterSKBackdate/{id}/{letterType}")]
        public async Task<IActionResult> DetailLetterSKBackdate(Guid id,string lettertype)
        {
            OutputDetailLetterBackdate letterDetail = new OutputDetailLetterBackdate();
            GeneralOutputModel generalOutput = new GeneralOutputModel();
            List<OutputContentTemplate> output = new List<OutputContentTemplate>();
            var token = HttpContext.Session.GetString("token");
            ParamGetDetailLetter pr = new ParamGetDetailLetter();
            pr.idLetter = id;
            pr.lettertype=lettertype;

            generalOutput = await _dataAccessProvider.GetDetailLetter("Letter/GetDetailLetterSKBackdate/", token, pr);
            var jsonApiResponseSerialize = JsonConvert.SerializeObject(generalOutput.Result);
            letterDetail = JsonConvert.DeserializeObject<OutputDetailLetterBackdate>(jsonApiResponseSerialize);

            generalOutput = await _dataAccessProvider.GetContentTemplate("Template/GetTemplate/", token);
            jsonApiResponseSerialize = JsonConvert.SerializeObject(generalOutput.Result);
            output = JsonConvert.DeserializeObject<List<OutputContentTemplate>>(jsonApiResponseSerialize);

            ViewBag.TemplateData = output;
            return View(letterDetail);
        }



    }
}
