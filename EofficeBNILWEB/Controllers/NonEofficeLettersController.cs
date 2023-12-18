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
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Localization;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Wordprocessing;
using System.Web.WebPages;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Office2016.Excel;

namespace EofficeBNILWEB.Controllers
{
    public class NonEofficeLettersController : Controller
    {
        private readonly IConfiguration _config;
        private readonly IDataAccessProvider _dataAccessProvider;
        private readonly IHostingEnvironment _environment;
        private readonly string urlApi;
        public NonEofficeLettersController(IConfiguration config, IDataAccessProvider dataAccessProvider, IHostingEnvironment Environment)
        {
            _config = config;
            _dataAccessProvider = dataAccessProvider;
            _environment = Environment;
            urlApi = _config.GetValue<string>("AppSettings:apiUrl");
        }

        public IActionResult Index()
        {
           
            return View();
        }
        public IActionResult ReportNonEoffcie()
        {
            return View();
        }

        [HttpPost]
        public async Task<string> SearchOutboxNonEoffice(string? trackingNumber, DateTime? startDate, DateTime? endDate)
        {
            List<ReportNonOutboxLetterOutput> reportOutput = new List<ReportNonOutboxLetterOutput>();
            GeneralOutputModel generalOutput = new GeneralOutputModel();
            List<Dictionary<String, String>> _list = new List<Dictionary<string, string>>();
            var token = HttpContext.Session.GetString("token");

            ParamReportNonOuboxLetter pr = new ParamReportNonOuboxLetter();
            pr.trackingNumber = trackingNumber;
            pr.startDate = startDate;
            pr.endDate = endDate;
            generalOutput = await _dataAccessProvider.GetDataReportNonEoffice("NonEofficeLetters/GetDataNonEofficeLetter", token, pr);
            var jsonApiResponseSerialize = JsonConvert.SerializeObject(generalOutput.Result);
            reportOutput = JsonConvert.DeserializeObject<List<ReportNonOutboxLetterOutput>>(jsonApiResponseSerialize);
            int no = 1;
            foreach (var item in reportOutput)
            {
                Dictionary<String, String> dict = new Dictionary<string, string>();
                dict.Add("no", no.ToString());
                dict.Add("nmrawb", item.nmrawb);
                dict.Add("nmrreferen", item.nmrreferen);
                dict.Add("ReceiptDate", item.ReceiptDate == null ? "" : Convert.ToDateTime(item.ReceiptDate).ToString("dd-MMM-yyyy"));
                dict.Add("expedition_name", item.expedition_name);
                dict.Add("sender_name", item.sender_name);
                dict.Add("npp", item.nip);
                dict.Add("unitname", item.unitname);
                dict.Add("kodeunit", item.kodeunit);
                dict.Add("letter_number", item.letter_number);
                dict.Add("docReceiver", item.docReceiver);
                dict.Add("phonenumber", item.phonenumber);
                dict.Add("address", item.address);
                dict.Add("purposename", item.purposename);
                if(item.statuskirim == 3)
                {
                    dict.Add("DateUntil", Convert.ToDateTime(item.DateUntil).ToString("dd-MMM-yyyy"));
                }
                else
                {
                    dict.Add("DateUntil", "");
                }
                
                dict.Add("statusname",item.statusname);
                dict.Add("cretaeby", item.cretaeby);
                dict.Add("updateby", item.updateby);

                _list.Add(dict);
                no++;
            }

            

            return JsonConvert.SerializeObject(_list);
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
                    worksheet.Cell(curerentRow, 14).Value = item.statuskirim == 3 ?  item.DateUntil.ToString() : null;
                    worksheet.Cell(curerentRow, 15).Value = item.statusname.ToString();

                }
                using (var stream = new MemoryStream())
                {

                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    var data= File(content, "application/vnd.openxmlformats-officedocument.spreadsheethtml.sheet", "NON_E-OFFICE_EKSPEDISI_REGISTRATION.xlsx");
                    return (data);
                    return Redirect("~/GenerateNoDoc/RegisterExpeditionNonEoffcie");
                }

            }
            
        }


        //public async Task<IActionResult> ExportExcel(ParamReportNonOuboxLetter pr)
        //{
        //    using (var workbook = new XLWorkbook())
        //    {

        //        List<ReportNonOutboxLetterOutput> documentOutput = new List<ReportNonOutboxLetterOutput>();
        //        GeneralOutputModel generalOutput = new GeneralOutputModel();
        //        var token = HttpContext.Session.GetString("token");
        //        generalOutput = await _dataAccessProvider.GetDataReportNonEoffice("NonEofficeLetters/GetDataNonEofficeLetter", token, pr);
        //        var jsonApiResponseSerialize = JsonConvert.SerializeObject(generalOutput.Result);
        //        documentOutput = JsonConvert.DeserializeObject<List<ReportNonOutboxLetterOutput>>(jsonApiResponseSerialize);
        //        var worksheet = workbook.Worksheets.Add("data");
        //        var curerentRow = 1;
                
        //        worksheet.Cell(curerentRow, 1).Value = "Nomor Surat";
        //        worksheet.Cell(curerentRow, 2).Value = "No AWB / No Pengiriman";
        //        worksheet.Cell(curerentRow, 3).Value = "Tanggal";
        //        worksheet.Cell(curerentRow, 4).Value = "Nama Ekspedisi";
        //        worksheet.Cell(curerentRow, 5).Value = "Nama Pengirim";
        //        worksheet.Cell(curerentRow, 6).Value = "Division";
        //        worksheet.Cell(curerentRow, 7).Value = "Penerima";
        //        worksheet.Cell(curerentRow, 8).Value = "Nomor Refrensi";
        //        worksheet.Cell(curerentRow, 9).Value = "Alamat";
        //        foreach (var item in documentOutput)
        //        {
        //            curerentRow++;
        //                worksheet.Cell(curerentRow, 1).Value = item.letter_number.ToString();
        //                worksheet.Cell(curerentRow, 2).Value = item.nmrawb.ToString();
        //                worksheet.Cell(curerentRow, 3).Value = item.ReceiptDate == null ? "" : Convert.ToDateTime(item.ReceiptDate).ToString("dd MMM yyyy");
        //                worksheet.Cell(curerentRow, 4).Value = item.expedition_name.ToString();
        //                worksheet.Cell(curerentRow, 5).Value = item.sender_name.ToString();
        //                worksheet.Cell(curerentRow, 6).Value = item.unitname.ToString();
        //                worksheet.Cell(curerentRow, 7).Value = item.docReceiver.ToString();
        //                worksheet.Cell(curerentRow, 8).Value = item.nmrreferen.ToString();
        //                worksheet.Cell(curerentRow, 9).Value = item.address.ToString();

        //        }
        //        using (var stream = new MemoryStream())
        //        {
        //            workbook.SaveAs(stream);
        //            var content = stream.ToArray();
        //            return File(content, "application/vnd.openxmlformats-officedocument.spreadsheethtml.sheet", "ReportNonEoffcie.xlsx");
        //        }

        //    }
        //    return View();
        //}



        public IActionResult KurirReport()
        {
            return View();
        }


        public async Task<string> SearchKurirOutboxNonEoffice(string? trackingNumber, DateTime? startDate, DateTime? endDate)
        {
            List<ReportNonOutboxLetterOutput> reportOutput = new List<ReportNonOutboxLetterOutput>();
            GeneralOutputModel generalOutput = new GeneralOutputModel();
            List<Dictionary<String, String>> _list = new List<Dictionary<string, string>>();
            var token = HttpContext.Session.GetString("token");

            ParamReportNonOuboxLetter pr = new ParamReportNonOuboxLetter();
            pr.trackingNumber = trackingNumber;
            pr.startDate = startDate;
            pr.endDate = endDate;
            generalOutput = await _dataAccessProvider.GetDataReportNonEoffice("NonEofficeLetters/GetDataKurirNonEofficeLetter", token, pr);
            var jsonApiResponseSerialize = JsonConvert.SerializeObject(generalOutput.Result);
            reportOutput = JsonConvert.DeserializeObject<List<ReportNonOutboxLetterOutput>>(jsonApiResponseSerialize);
            int no = 1;
            foreach (var item in reportOutput)
            {
                Dictionary<String, String> dict = new Dictionary<string, string>();
                dict.Add("no", no.ToString());
                dict.Add("nmresi", item.nmresi);
                dict.Add("letter_number", item.letter_number);
                //dict.Add("nmrawb", item.nmrawb);
                dict.Add("deliveryname", item.deliveryname);
                dict.Add("ReceiptDate", item.ReceiptDate == null ? "" : Convert.ToDateTime(item.ReceiptDate).ToString("dd MMM yyyy"));
                dict.Add("expedition_name", item.expedition_name);
                dict.Add("sender_name", item.sender_name);
                dict.Add("unitname", item.unitname);
                dict.Add("docReceiver", item.docReceiver);
                dict.Add("phonenumber", item.phonenumber);
                dict.Add("address", item.address);
                dict.Add("purposename", item.purposename);
                dict.Add("tgluntil", item.tgluntil);
                dict.Add("statusname", item.statusname);
                dict.Add("cretaeby", item.cretaeby);
                dict.Add("updateby", item.updateby);
                _list.Add(dict);
                no++;
            }

            return JsonConvert.SerializeObject(_list);
        }


        public async Task<IActionResult> ExportExcelKurir(ParamReportNonOuboxLetter pr)
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
                worksheet.Cell(curerentRow, 1).Value = "Nomor Resi";
                worksheet.Cell(curerentRow, 2).Value = "Nomor Surat";
                worksheet.Cell(curerentRow, 3).Value = "Tanggal Penerima";
                worksheet.Cell(curerentRow, 4).Value = "Tipe Jenis Antaran";
                worksheet.Cell(curerentRow, 5).Value = "Nama Kurir";
                worksheet.Cell(curerentRow, 6).Value = "Nama Pengirim";
                worksheet.Cell(curerentRow, 7).Value = "Division";
                worksheet.Cell(curerentRow, 8).Value = "Nama Tujuan";
                worksheet.Cell(curerentRow, 9).Value = "Nomor Telepon";
                worksheet.Cell(curerentRow, 10).Value = "Alamat";
                worksheet.Cell(curerentRow, 11).Value = "Nama Penerima";
                worksheet.Cell(curerentRow, 11).Value = "Status";
                worksheet.Cell(curerentRow, 12).Value = "dibuat Oleh";
                worksheet.Cell(curerentRow, 13).Value = "diubah Oleh";
                foreach (var item in documentOutput)
                {
                    curerentRow++;
                    worksheet.Cell(curerentRow, 1).Value = item.nmresi.ToString();
                    worksheet.Cell(curerentRow, 2).Value = item.letter_number.ToString() == null ? "" : item.letter_number.ToString();
                    worksheet.Cell(curerentRow, 3).Value = item.ReceiptDate == null ? "" : Convert.ToDateTime(item.ReceiptDate).ToString("dd MMM yyyy");
                    worksheet.Cell(curerentRow, 4).Value = item.deliveryname.ToString() == null ? "" : item.deliveryname.ToString();
                    worksheet.Cell(curerentRow, 5).Value = item.expedition_name.ToString() == null ? "" : item.expedition_name.ToString();
                    worksheet.Cell(curerentRow, 6).Value = item.sender_name.ToString() == null ? "" : item.sender_name.ToString();
                    worksheet.Cell(curerentRow, 7).Value = item.unitname.ToString() == null ? "" : item.unitname.ToString();
                    worksheet.Cell(curerentRow, 8).Value = item.docReceiver.ToString()== null ? "" : item.docReceiver.ToString();
                    worksheet.Cell(curerentRow, 9).Value = item.phonenumber;
                    worksheet.Cell(curerentRow, 10).Value = item.address.ToString() == null ? "" : item.address.ToString();
                    worksheet.Cell(curerentRow, 11).Value = item.statusname.ToString() == null ? "" : item.statusname.ToString();
                    worksheet.Cell(curerentRow, 12).Value = item.cretaeby.ToString();
                    worksheet.Cell(curerentRow, 13).Value = item.updateby.ToString();
                }
                using (var stream = new MemoryStream())
                {

                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    var data = File(content, "application/vnd.openxmlformats-officedocument.spreadsheethtml.sheet", "NON E-OFFICE COURIER REPORT_.xlsx");
                    return (data);
                    return Redirect("~/GenerateNoDoc/KurirReport");
                }

            }
        }

        #region Detail View Delivery Dokumen Non-Eoffice

        public async Task<IActionResult> ViewDetailsEKspedisi()
        {
            
            return View();
        }


        [HttpPost]
        public async Task<string> GetDetailViewEKspedisiList()
        {
            OutputDetailsView OutputEkspedisiNon = new OutputDetailsView();
            GeneralOutputModel generalOutput = new GeneralOutputModel();
            List<Dictionary<String, String>> _list = new List<Dictionary<string, string>>();
            var token = HttpContext.Session.GetString("token");
            var draw = Request.Form["draw"].FirstOrDefault();
            var sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
            var sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault();
            var searchValue = Request.Form["search[value]"].FirstOrDefault();
            int pageSize = Convert.ToInt32(Request.Form["length"].FirstOrDefault() ?? "0");
            int skip = Convert.ToInt32(Request.Form["start"].FirstOrDefault() ?? "0");

            ParamGetDetailsView pr = new ParamGetDetailsView();
            pr.draw = draw;
            pr.sortColumn = sortColumn;
            pr.sortColumnDirection = sortColumnDirection;
            pr.searchValue = searchValue;
            pr.pageSize = pageSize;
            pr.start = skip;
            generalOutput = await _dataAccessProvider.GetDetailsViewEkspedisi_("NonEofficeLetters/GetDetailsViewEkspedisi/", token, pr);
            var jsonApiResponseSerialize = JsonConvert.SerializeObject(generalOutput.Result);
            OutputEkspedisiNon = JsonConvert.DeserializeObject<OutputDetailsView>(jsonApiResponseSerialize);

            return JsonConvert.SerializeObject(OutputEkspedisiNon);
        }
      
        public async Task<IActionResult> ReportEkspedisiNonEofficeByUser(ParamReportNonOuboxLetter pr)
        {
            using (var workbook = new XLWorkbook())
            {

                List<ReportNonOutboxLetterOutput> documentOutput = new List<ReportNonOutboxLetterOutput>();
                GeneralOutputModel generalOutput = new GeneralOutputModel();
                var token = HttpContext.Session.GetString("token");
                generalOutput = await _dataAccessProvider.GetDataReportNonEoffice("NonEofficeLetters/GetDataNonEofficeLetterByUser", token, pr);
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
                    worksheet.Cell(curerentRow, 11).Value = item.address == null ? "" : item.address.ToString();
                    worksheet.Cell(curerentRow, 12).Value = item.purposename == null ? "" : item.purposename.ToString();
                    worksheet.Cell(curerentRow, 13).Value = item.DateUntil;
                    worksheet.Cell(curerentRow, 14).Value = item.statusname.ToString();
                    //worksheet.Cell(curerentRow, 13).Value = "";
                    //worksheet.Cell(curerentRow, 14).Value = item.statusname.ToString();

                }
                using (var stream = new MemoryStream())
                {

                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    var data = File(content, "application/vnd.openxmlformats-officedocument.spreadsheethtml.sheet", "NON_E-OFFICE_EKSPEDISI_REGISTRATION.xlsx");

                    return (data);


                }

            }

        }

        public async Task<IActionResult> ViewDetailsKurir()
        {

            return View();
        }


        [HttpPost]
        public async Task<string> GetDetailViewKurirList()
        {
            OutputDetailsView OutputEkspedisiNon = new OutputDetailsView();
            GeneralOutputModel generalOutput = new GeneralOutputModel();
            List<Dictionary<String, String>> _list = new List<Dictionary<string, string>>();
            var token = HttpContext.Session.GetString("token");
            var draw = Request.Form["draw"].FirstOrDefault();
            var sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
            var sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault();
            var searchValue = Request.Form["search[value]"].FirstOrDefault();
            int pageSize = Convert.ToInt32(Request.Form["length"].FirstOrDefault() ?? "0");
            int skip = Convert.ToInt32(Request.Form["start"].FirstOrDefault() ?? "0");

            ParamGetDetailsView pr = new ParamGetDetailsView();
            pr.draw = draw;
            pr.sortColumn = sortColumn;
            pr.sortColumnDirection = sortColumnDirection;
            pr.searchValue = searchValue;
            pr.pageSize = pageSize;
            pr.start = skip;
            generalOutput = await _dataAccessProvider.GetDetailsViewKurir_("NonEofficeLetters/GetDetailsViewKurir/", token, pr);
            var jsonApiResponseSerialize = JsonConvert.SerializeObject(generalOutput.Result);
            OutputEkspedisiNon = JsonConvert.DeserializeObject<OutputDetailsView>(jsonApiResponseSerialize);

            return JsonConvert.SerializeObject(OutputEkspedisiNon);
        }



        //LacakSuratKeluarNonEoffice,NonEofficeLetters
        //public IActionResult LacakSuratKeluarNonEoffice()
        //{
        //    return View();
        //}

        //[HttpPost]
        //public async Task<IActionResult> LacakSuratKeluarNonEoffice(ParamReportNonOuboxLetter pr)
        //{
        //    return;
        //}

        
        public async Task<IActionResult> LacakSuratKeluarNonEoffice(ParamReportNonOuboxLetter pr)
        {
            if (pr.trackingNumber !=null)
            {
                List<ReportNonOutboxLetterOutput> reportOutput = new List<ReportNonOutboxLetterOutput>();
                GeneralOutputModel generalOutput = new GeneralOutputModel();
                List<Dictionary<String, String>> _list = new List<Dictionary<string, string>>();
                var token = HttpContext.Session.GetString("token");

                //ParamReportNonOuboxLetter pr = new ParamReportNonOuboxLetter();
                //pr.trackingNumber = trackingNumber;

                generalOutput = await _dataAccessProvider.GetDataReportNonEoffice("NonEofficeLetters/SearchSuratKeluarKurirNonEoffice", token, pr);
                var jsonApiResponseSerialize = JsonConvert.SerializeObject(generalOutput.Result);
                reportOutput = JsonConvert.DeserializeObject<List<ReportNonOutboxLetterOutput>>(jsonApiResponseSerialize);
               
                

                foreach (var item in reportOutput)
                {
                    ViewBag.type=item.delivery_type;
                }
                ViewBag.Data = reportOutput;
            }
            else
            {
                ViewBag.Data=null;
                ViewBag.type=null;
                return View();
            }
            return View();

            //var jsonApiResponseSerialize = JsonConvert.SerializeObject(generalOutput.Result);

            //int no = 1;

            //foreach (var item in reportOutput)
            //{
            //    Dictionary<String, String> dict = new Dictionary<string, string>();
            //    dict.Add("no", no.ToString());
            //    dict.Add("nmrawb", item.nmrawb);
            //    dict.Add("nmrreferen", item.nmrreferen);
            //    dict.Add("ReceiptDate", item.ReceiptDate == null ? "" : Convert.ToDateTime(item.ReceiptDate).ToString("dd-MMM-yyyy"));
            //    dict.Add("expedition_name", item.expedition_name);
            //    dict.Add("sender_name", item.sender_name);
            //    dict.Add("npp", item.nip);
            //    dict.Add("unitname", item.unitname);
            //    dict.Add("kodeunit", item.kodeunit);
            //    dict.Add("letter_number", item.letter_number);
            //    dict.Add("docReceiver", item.docReceiver);
            //    dict.Add("phonenumber", item.phonenumber);
            //    dict.Add("address", item.address);
            //    dict.Add("purposename", item.purposename);
            //    dict.Add("nmresi", item.nmresi);
            //    dict.Add("delivery_type", item.delivery_type.ToString());
            //    if (item.statuskirim == 3)
            //    {
            //        dict.Add("DateUntil", Convert.ToDateTime(item.DateUntil).ToString("dd-MMM-yyyy"));
            //    }
            //    else
            //    {
            //        dict.Add("DateUntil", "");
            //    }

            //    dict.Add("statusname", item.statusname);
            //    dict.Add("cretaeby", item.cretaeby);
            //    dict.Add("updateby", item.updateby);

            //    _list.Add(dict);

            //    no++;
            //}

            //return JsonConvert.SerializeObject(_list);
        }

        #endregion

    }
}
