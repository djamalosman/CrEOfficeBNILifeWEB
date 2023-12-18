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
using static System.Net.WebRequestMethods;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;

namespace EofficeBNILWEB.Controllers
{
	public class SettingUserController : Controller
	{
        private readonly IConfiguration _config;
        private readonly IDataAccessProvider _dataAccessProvider;
        private readonly IHostingEnvironment _environment;
        private readonly string urlApi;
        public SettingUserController(IConfiguration config, IDataAccessProvider dataAccessProvider, IHostingEnvironment Environment)
        {
            _config = config;
            _dataAccessProvider = dataAccessProvider;
            _environment = Environment;
            urlApi = _config.GetValue<string>("AppSettings:apiUrl");
        }
        public async Task<IActionResult> Index()
		{
            List<DataOuputUserPGA> getUserPgaAll = new List<DataOuputUserPGA>();
            GeneralOutputModel generalOutput = new GeneralOutputModel();
            var token = HttpContext.Session.GetString("token");
            generalOutput = await _dataAccessProvider.GetUserPGAasync("User/GetuserPga/", token);
            var jsonApiResponseSerialize = JsonConvert.SerializeObject(generalOutput.Result);
            getUserPgaAll = JsonConvert.DeserializeObject<List<DataOuputUserPGA>>(jsonApiResponseSerialize);

            if (generalOutput.Status == "UA")
            {
                return RedirectToAction("Logout", "Home");
            }
           
            ViewBag.SelectUserPGA = getUserPgaAll;
            return View();
		}


        [HttpPost]
        public async Task<string> GetUserList()
        {
            UserOutputWeb documentOutput = new UserOutputWeb();
            GeneralOutputModel generalOutput = new GeneralOutputModel();
            List<Dictionary<String, String>> _list = new List<Dictionary<string, string>>();
            var token = HttpContext.Session.GetString("token");
            var draw = Request.Form["draw"].FirstOrDefault();
            var sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
            var sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault();
            var searchValue = Request.Form["search[value]"].FirstOrDefault();
            int pageSize = Convert.ToInt32(Request.Form["length"].FirstOrDefault() ?? "0");
            int skip = Convert.ToInt32(Request.Form["start"].FirstOrDefault() ?? "0");

            ParamGetUsertWeb pr = new ParamGetUsertWeb();
            pr.draw = draw;
            pr.sortColumn = sortColumn;
            pr.sortColumnDirection = sortColumnDirection;
            pr.searchValue = searchValue;
            pr.pageSize = pageSize;
            pr.start = skip;

            generalOutput = await _dataAccessProvider.GetDataUserAsync("User/GetDataUser/", token, pr);
            var jsonApiResponseSerialize = JsonConvert.SerializeObject(generalOutput.Result);
            documentOutput = JsonConvert.DeserializeObject<UserOutputWeb>(jsonApiResponseSerialize);

            return JsonConvert.SerializeObject(documentOutput);
        }

        public async Task<IActionResult> Update(Guid id)
        {
            
            GeneralOutputModel generalOutput = new GeneralOutputModel();
            var token = HttpContext.Session.GetString("token");
            DataOuputUser userOutput = new DataOuputUser();

            ParamGetDetailUser prGetDetail = new ParamGetDetailUser();
            prGetDetail.Iduser = id;
            generalOutput = await _dataAccessProvider.GetDetailUserAsync("User/GetDetailUser", token, prGetDetail);
            var jsonApiResponseSerializee = JsonConvert.SerializeObject(generalOutput.Result);
            userOutput = JsonConvert.DeserializeObject<DataOuputUser>(jsonApiResponseSerializee);

            if (generalOutput.Status == "UA")
            {
                return RedirectToAction("Logout", "Home");
            }
            //dynamic mymodel = new ExpandoObject();
            //mymodel.PositionOuput = positionList;
            //mymodel.DataOuputUser = userOutput;
            ViewBag.Pga = userOutput.Unit_name;
            ViewBag.DataUser = userOutput.Nip +"-"+ userOutput.Fullname;
            ViewBag.user = userOutput.Iduser;
            ViewBag.grup = userOutput.IDgroup;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Update(ParamUpdateUserWeb pr)
        {
            try
            {
                
                
                GeneralOutputModel generalOutput = new GeneralOutputModel();
                ParamUpdateUser prApi = new ParamUpdateUser();
                var token = HttpContext.Session.GetString("token");
                //List<ParamUpdateReceiver> prApiReceiver = new List<ParamUpdateReceiver>();
                prApi.Iduser = pr.Iduser;
                prApi.IDgroup = pr.IDgroup;
                
                generalOutput = await _dataAccessProvider.PutUpdateUserAsync("User/UpdateSettingUser", token, prApi);
                var jsonApiResponseSerialize = JsonConvert.SerializeObject(generalOutput.Result);

                

                if (generalOutput.Status == "OK")
                {
                    TempData["status"] = "OK";
                    TempData["title"] = "Berhasil";
                    TempData["pesan"] = "Menambah Data Admin Mailing Room";
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
                    TempData["title"] = "Gagal merubah data user";
                    TempData["pesan"] = generalOutput.Message;
                }


                return Redirect("~/SettingUser/index");
            }
            catch (Exception ex)
            {
                TempData["status"] = "NG";
                TempData["title"] = "Gagal merubah dokumen masuk";
                TempData["pesan"] = ex.ToString();
                return Redirect("~/SettingUser/index");
            }
        }


        public async Task<IActionResult> Delete(Guid id)
        {
            
            GeneralOutputModel generalOutput = new GeneralOutputModel();
            var token = HttpContext.Session.GetString("token");
            DataOuputUser userOutput = new DataOuputUser();

            ParamGetDetailUser prGetDetail = new ParamGetDetailUser();
            prGetDetail.Iduser = id;
            generalOutput = await _dataAccessProvider.GetDetailUserAsync("User/GetDetailUser", token, prGetDetail);
            var jsonApiResponseSerializee = JsonConvert.SerializeObject(generalOutput.Result);
            userOutput = JsonConvert.DeserializeObject<DataOuputUser>(jsonApiResponseSerializee);




            if (generalOutput.Status == "UA")
            {
                return RedirectToAction("Logout", "Home");
            }

            string idgroup = "U";
            ParamUpdateUser prApi = new ParamUpdateUser();
            prApi.Iduser = userOutput.Iduser;
            prApi.IDgroup = idgroup;

            generalOutput = await _dataAccessProvider.PutUpdateUserAsync("User/UpdateSettingUser", token, prApi);
            var jsonApiResponseSerialize = JsonConvert.SerializeObject(generalOutput.Result);



            if (generalOutput.Status == "OK")
            {
                TempData["status"] = "OK";
                TempData["title"] = "Berhasil";
                TempData["pesan"] = "Menghapus Data Admin Mailing Room";
            }
            else if (generalOutput.Status == "UA")
            {
                TempData["status"] = "NG";
                TempData["pesan"] = "Session habis silahkan login kembali";
                return RedirectToAction("Logout", "Home");
            }

            return Redirect("~/SettingUser/index");
        }

        public async Task <IActionResult> ProfileUser()
        {
            OuputSignature getSignatue = new OuputSignature();
            GeneralOutputModel generalOutput = new GeneralOutputModel();
            var token = HttpContext.Session.GetString("token");
            generalOutput = await _dataAccessProvider.GetOuputSignatureUser("User/GetOuputSignatureUser/", token);
            var jsonApiResponseSerialize = JsonConvert.SerializeObject(generalOutput.Result);
            getSignatue = JsonConvert.DeserializeObject<OuputSignature>(jsonApiResponseSerialize);

            if (generalOutput.Status == "UA")
            {
                return RedirectToAction("Logout", "Home");
            }
            if (getSignatue !=null)
            {
                ViewBag.nameFIle=getSignatue.NameImage;
                ViewBag.typeFIle=getSignatue.TypeImage;
                ViewBag.idFile=getSignatue.idMg;
                ViewBag.statusFile=getSignatue.status_code;
                ViewBag.viewImage=getSignatue.NameImage+".png";
            }
            else
            {
                ViewBag.nameFIle="";
            }
            

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ProfileUser(IFormFile formFile)
        {
            try
            {

                
                GeneralOutputModel generalOutput = new GeneralOutputModel();
                ImageUploadTTD ttd = new ImageUploadTTD();
                ttd.NameImage = formFile.FileName;
                string imageFormat = formFile.ContentType.Split('/')[1].ToLower();  // Extracts the format (e.g., "png", "jpeg")
                ttd.TypeImage = imageFormat;

                ttd.LenghtImage = Convert.ToInt32(formFile.Length);
                var token = HttpContext.Session.GetString("token");
                //List<ParamUpdateReceiver> prApiReceiver = new List<ParamUpdateReceiver>();
                //prApi.Iduser = pr.Iduser;
                //prApi.IDgroup = pr.IDgroup;

                generalOutput = await _dataAccessProvider.PutUploadSignatureAsync("User/InsertUserSignatureWeb", token, ttd);
                var jsonApiResponseSerialize = JsonConvert.SerializeObject(generalOutput.Result);
                var data = JsonConvert.DeserializeObject<ImageUploadTTD>(jsonApiResponseSerialize);
                if (generalOutput.Status == "OK")
                {
                    var nip = HttpContext.Session.GetString("nip");
                    string FileName = nip+ ".png";
                    // combining GUID to create unique name before saving in wwwroot
                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + FileName;

                    // getting full path inside wwwroot/images
                    var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/imgsignature/", FileName);

                    // copying file
                    formFile.CopyTo(new FileStream(imagePath, FileMode.Create));

                    TempData["status"] = "OK";
                    TempData["title"] = "Berhasil";
                    TempData["pesan"] = "Menambah Tanda Tangan";
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
                    TempData["title"] = "Gagal Menambah Tanda Tangan";
                    TempData["pesan"] = "Data sudah ada,jika ingin menambah data hapus terlebih dahulu data sembulnya";
                }


                return Redirect("~/SettingUser/ProfileUser");
            }
            catch (Exception ex)
            {
                TempData["status"] = "NG";
                TempData["title"] = "Gagal menambah super user";
                TempData["pesan"] = ex.ToString();
                return Redirect("~/SettingUser/ProfileUser");
            }
        }


        public async Task<IActionResult> DeleteSignature(Guid id)
        {

            GeneralOutputModel generalOutput = new GeneralOutputModel();
            var token = HttpContext.Session.GetString("token");
            OuputSignature prApi = new OuputSignature();
            prApi.idMg = id;


            generalOutput = await _dataAccessProvider.DeleterSignatureUserProfile("User/DeleteUserProfileSignatureWeb", token, prApi);
            var jsonApiResponseSerialize = JsonConvert.SerializeObject(generalOutput.Result);
            //var userOutput = JsonConvert.DeserializeObject<DataOuputUser>(jsonApiResponseSerialize);


            if (generalOutput.Status == "OK")
            {
                TempData["status"] = "OK";
                TempData["title"] = "Berhasil";
                TempData["pesan"] = "Menghapus Data Tanda Tangan";
            }

            else if (generalOutput.Status == "UA")
            {
                TempData["status"] = "NG";
                TempData["pesan"] = "Session habis silahkan login kembali";
                return RedirectToAction("Logout", "Home");
            }

            return Redirect("~/SettingUser/ProfileUser");
        }



        public async Task<IActionResult> DetailProfileUser(Guid id)
        {

            GeneralOutputModel generalOutput = new GeneralOutputModel();
            var token = HttpContext.Session.GetString("token");
            OuputSignature prApi = new OuputSignature();
            prApi.idMg = id;


            generalOutput = await _dataAccessProvider.DetailSignatureUserProfile("User/DetailUserProfileSignatureWeb", token, prApi);
            var jsonApiResponseSerialize = JsonConvert.SerializeObject(generalOutput.Result);
            //var userOutput = JsonConvert.DeserializeObject<DataOuputUser>(jsonApiResponseSerialize);



            return Redirect("~/SettingUser/ProfileUser");
        }


        public async Task<IActionResult> DetailApprovalProfileUser(Guid id)
        {
            OutputDetailApprovalSignature DetailApproval = new OutputDetailApprovalSignature();
            GeneralOutputModel generalOutput = new GeneralOutputModel();
            var token = HttpContext.Session.GetString("token");
            OuputSignature prApi = new OuputSignature();
            prApi.idMg = id;


            generalOutput = await _dataAccessProvider.DetailApprovalSignatureUserProfile("User/DetailApprovalUserProfileSignatureWeb", token, prApi);
            var jsonApiResponseSerialize = JsonConvert.SerializeObject(generalOutput.Result);
            DetailApproval = JsonConvert.DeserializeObject<OutputDetailApprovalSignature>(jsonApiResponseSerialize);

            return View(DetailApproval);
        }


       
        public IActionResult UserSetting()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> UserSetting(ParamUpdatePassword pr)
        {
            try
            {
                var token = HttpContext.Session.GetString("token");
                GeneralOutputModel generalOutput = new GeneralOutputModel();


                generalOutput = await _dataAccessProvider.ResetPassword("User/UpdatePassword", token, pr);
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


                return RedirectToAction("UserSetting");
            }
            catch (Exception ex)
            {
                TempData["status"] = "NG";
                TempData["title"] = "Gagal merubah password";
                TempData["pesan"] = ex.ToString();
                //return Redirect("~/Document/DocIn");
                return Redirect("~/SettingUser/UserSetting");
            }
        }

        public IActionResult FirtsTimeLogin()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> FirtsTimeLogin(ParamUpdatePassword pr)
        {
            try
            {
                var token = HttpContext.Session.GetString("token");
                GeneralOutputModel generalOutput = new GeneralOutputModel();


                generalOutput = await _dataAccessProvider.ResetPassword("User/UpdatePassword", token, pr);
                var jsonApiResponseSerialize = JsonConvert.SerializeObject(generalOutput.Result);

                if (generalOutput.Status == "OK")
                {
                    TempData["status"] = "OK";
                    TempData["title"] = "Update password";
                    TempData["pesan"] = generalOutput.Message;
                }
                Response.Cookies.Delete("en");
                Response.Cookies.Delete("ID");
                HttpContext.Session.Clear();
                HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

                return RedirectToAction("Login", "Home");
            }
            catch (Exception ex)
            {
                TempData["status"] = "NG";
                TempData["title"] = "Update password";
                TempData["pesan"] = ex.ToString();
                return Redirect("~/SettingUser/FirtsTimeLogin");
            }
        }
        public async Task<IActionResult> AdminDivisi()
        {
            List<AdminDivisionOutput> divisionList = new List<AdminDivisionOutput>();
            List<AdminDivisionOutput> adminDivisionList = new List<AdminDivisionOutput>();
            GeneralOutputModel generalOutput = new GeneralOutputModel();
            var token = HttpContext.Session.GetString("token");
            generalOutput = await _dataAccessProvider.GetUnitAsync("User/GetAllDivision/", token);
            var jsonApiResponseSerialize = JsonConvert.SerializeObject(generalOutput.Result);
            divisionList = JsonConvert.DeserializeObject<List<AdminDivisionOutput>>(jsonApiResponseSerialize);

            if (generalOutput.Status == "UA")
            {
                return RedirectToAction("Logout", "Home");
            }
            generalOutput = await _dataAccessProvider.GetUnitAsync("User/GetAllUserAdminDivision/", token);
            jsonApiResponseSerialize = JsonConvert.SerializeObject(generalOutput.Result);
            adminDivisionList = JsonConvert.DeserializeObject<List<AdminDivisionOutput>>(jsonApiResponseSerialize);

            if (generalOutput.Status == "UA")
            {
                return RedirectToAction("Logout", "Home");
            }

            ViewBag.DivisionList = divisionList;
            ViewBag.AdminDivisionList = adminDivisionList;

            return View();
        }

        [HttpPost]
        public async Task<string> GetUserDivisi(Guid idUnit)
        {
            DocumentOutputWeb documentOutput = new DocumentOutputWeb();
            GeneralOutputModel generalOutput = new GeneralOutputModel();
            var token = HttpContext.Session.GetString("token");

            ParamGetUserByUnit pr = new ParamGetUserByUnit();
            pr.idUnit = idUnit;

            generalOutput = await _dataAccessProvider.GetUserByUnitAsync("User/GetUserByUnit/", token, pr);
            var jsonApiResponseSerialize = JsonConvert.SerializeObject(generalOutput);
            //generalOutput = JsonConvert.DeserializeObject<GeneralOutputModel>(jsonApiResponseSerialize);

            return jsonApiResponseSerialize;
        }

        [HttpPost]
        public async Task<string> SettingAdmin(Guid idUnit, Guid idUser)
        {
            DocumentOutputWeb documentOutput = new DocumentOutputWeb();
            GeneralOutputModel generalOutput = new GeneralOutputModel();
            var token = HttpContext.Session.GetString("token");

            ParamUpdateAdminDivisi pr = new ParamUpdateAdminDivisi();
            pr.idUnit = idUnit;
            pr.idUser = idUser;

            generalOutput = await _dataAccessProvider.UpdateAdminDivisi("User/UpdateAdminDivisi/", token, pr);
            var jsonApiResponseSerialize = JsonConvert.SerializeObject(generalOutput);
            //generalOutput = JsonConvert.DeserializeObject<GeneralOutputModel>(jsonApiResponseSerialize);

            if (generalOutput.Status == "OK")
            {
                TempData["status"] = "OK";
                TempData["title"] = "Berhasil";
                TempData["pesan"] = "Menambah admin divisi";
            }
            else
            {
                TempData["status"] = "NG";
                TempData["title"] = "Gagal menghapus admin divisi";
                TempData["pesan"] = generalOutput.Message;
            }

            return jsonApiResponseSerialize;
        }

        public async Task<IActionResult> DeleteSetingAdmin(Guid idUser, Guid idUnit)
        {

            DocumentOutputWeb documentOutput = new DocumentOutputWeb();
            GeneralOutputModel generalOutput = new GeneralOutputModel();
            var token = HttpContext.Session.GetString("token");

            ParamUpdateAdminDivisi pr = new ParamUpdateAdminDivisi();
            pr.idUnit = idUnit;
            pr.idUser = idUser;

            generalOutput = await _dataAccessProvider.UpdateAdminDivisi("User/DeleteAdminDivisi/", token, pr);
            //var jsonApiResponseSerialize = JsonConvert.SerializeObject(generalOutput);
            //generalOutput = JsonConvert.DeserializeObject<GeneralOutputModel>(jsonApiResponseSerialize);
            if (generalOutput.Status == "OK")
            {
                TempData["status"] = "OK";
                TempData["title"] = "Berhasil";
                TempData["pesan"] = "Menghapus admin divisi";
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
                TempData["title"] = "Gagal menghapus admin divisi";
                TempData["pesan"] = generalOutput.Message;
            }
            return Redirect("~/SettingUser/AdminDivisi");
        }

        [HttpPost]
        public async Task<IActionResult> MailForgotPassword(ParamForgotPassword pr)
        {
            try
            {
                //var token = HttpContext.Session.GetString("token");
                GeneralOutputModel generalOutput = new GeneralOutputModel();
                pr.token = "953a3220084d73ea9948e3046c3c242d";
                generalOutput = await _dataAccessProvider.MailForgotPassword("User/MailForgotPassword", pr);
                var jsonApiResponseSerialize = JsonConvert.SerializeObject(generalOutput.Result);

                if (generalOutput.Status == "OK")
                {
                    TempData["status"] = "OK";
                    TempData["title"] = "Berhasil Send Email";
                    TempData["pesan"] = "Silahkan cek email";
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
                    TempData["title"] = "Gagal Send Email";
                    TempData["pesan"] = generalOutput.Message;
                }


                return RedirectToAction("Login","Home");
            }
            catch (Exception ex)
            {
                TempData["status"] = "NG";
                TempData["title"] = "Gagal";
                TempData["pesan"] = ex.ToString();
                return RedirectToAction("Login", "Home");
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


        public async Task<IActionResult> SecretarySetting()
        {

            List<DataOuputUserDirektur> getUserDrktAll = new List<DataOuputUserDirektur>();
            List<DataOuputSekdir> getViewAll = new List<DataOuputSekdir>();
            GeneralOutputModel generalOutput = new GeneralOutputModel();
            var token = HttpContext.Session.GetString("token");
            generalOutput = await _dataAccessProvider.GetUserDirekturasync("User/GetuserDirektur/", token);
            var jsonApiResponseSerialize = JsonConvert.SerializeObject(generalOutput.Result);
            getUserDrktAll = JsonConvert.DeserializeObject<List<DataOuputUserDirektur>>(jsonApiResponseSerialize);

            if (generalOutput.Status == "UA")
            {
                return RedirectToAction("Logout", "Home");
            }
            generalOutput = await _dataAccessProvider.GetDataUserSekdirAsync("User/GetDataUserSekdir/", token);
            jsonApiResponseSerialize = JsonConvert.SerializeObject(generalOutput.Result);
            getViewAll = JsonConvert.DeserializeObject<List<DataOuputSekdir>>(jsonApiResponseSerialize);

            if (generalOutput.Status == "UA")
            {
                return RedirectToAction("Logout", "Home");
            }

            ViewBag.SelectUserDirektur = getUserDrktAll;
            ViewBag.SelectAllData = getViewAll;

            return View();
            //List<DataOuputUserDirektur> getUserDrktAll = new List<DataOuputUserDirektur>();
            //List<UserSekdirOutputWeb> getViewAll = new List<UserSekdirOutputWeb>();
            //GeneralOutputModel generalOutput = new GeneralOutputModel();
            //var token = HttpContext.Session.GetString("token");
            //generalOutput = await _dataAccessProvider.GetUserDirekturasync("User/GetuserDirektur/", token);
            //var jsonApiResponseSerialize = JsonConvert.SerializeObject(generalOutput.Result);
            //getUserDrktAll = JsonConvert.DeserializeObject<List<DataOuputUserDirektur>>(jsonApiResponseSerialize);


            //if (generalOutput.Status == "UA")
            //{
            //    return RedirectToAction("Logout", "Home");
            //}
            //generalOutput = await _dataAccessProvider.GetDataUserSekdirAsync("User/GetDataUserSekdir/", token);
            //jsonApiResponseSerializee = JsonConvert.SerializeObject(generalOutput.Result);
            //getViewAll = JsonConvert.DeserializeObject<UserSekdirOutputWeb>(jsonApiResponseSerializee);
            ////generalOutput = await _dataAccessProvider.GetUserSeketarisasync("User/GetuserSeketaris/", token);
            ////jsonApiResponseSerialize = JsonConvert.SerializeObject(generalOutput.Result);
            ////var getUserSkrtAll = JsonConvert.DeserializeObject<List<DataOuputUserSeketaris>>(jsonApiResponseSerialize);

            //if (generalOutput.Status == "UA")
            //{
            //    return RedirectToAction("Logout", "Home");
            //}

            //ViewBag.SelectUserDirektur = getUserDrktAll;
            //ViewBag.SelectAllData = getViewAll;
            ////ViewBag.SelectUserSkrt = getUserSkrtAll;

        }


        [HttpPost]
        public async Task<string> UpdateSettingSeketaris(Guid IdDirektur, Guid iduser)
        {
            DocumentOutputWeb documentOutput = new DocumentOutputWeb();
            GeneralOutputModel generalOutput = new GeneralOutputModel();
            var token = HttpContext.Session.GetString("token");

            ParamUpdateUserSekdirWeb pr = new ParamUpdateUserSekdirWeb();
            pr.IdDirektur = IdDirektur;
            pr.Iduser = iduser;

            generalOutput = await _dataAccessProvider.UpdateDataSettingSeketaris("User/UpdateSettingSeketaris/", token, pr);
            var jsonApiResponseSerialize = JsonConvert.SerializeObject(generalOutput);
            //generalOutput = JsonConvert.DeserializeObject<GeneralOutputModel>(jsonApiResponseSerialize);

            if (generalOutput.Status == "OK")
            {
                TempData["status"] = "OK";
                TempData["title"] = "Berhasil";
                TempData["pesan"] = "Menambah Seketaris";
            }
            else if (generalOutput.Status == "NOTOK")
            {
                TempData["status"] = "NOTOK";
                TempData["pesan"] = "Seketaris sudah di gunakan";
            }
            else
            {
                TempData["status"] = "NG";
                TempData["title"] = "Gagal Menambahkan Seketaris";
                TempData["pesan"] = generalOutput.Message;
            }

            return jsonApiResponseSerialize;
        }


        [HttpPost]
        public async Task<string> GetSeketaris(Guid IdDirektur)
        {
            DocumentOutputWeb documentOutput = new DocumentOutputWeb();
            GeneralOutputModel generalOutput = new GeneralOutputModel();
            var token = HttpContext.Session.GetString("token");

            ParamUpdateUserSekdirWeb pr = new ParamUpdateUserSekdirWeb();
            pr.IdDirektur = IdDirektur;

            generalOutput = await _dataAccessProvider.GetUserByDirekturAsync("User/GetUserBySeketaris/", token, pr);
            var jsonApiResponseSerialize = JsonConvert.SerializeObject(generalOutput);
            //generalOutput = JsonConvert.DeserializeObject<GeneralOutputModel>(jsonApiResponseSerialize);

            return jsonApiResponseSerialize;
        }

     
        [HttpPost]
        public async Task<IActionResult> UpdateSekDir(ParamUpdateUserSekdirWeb pr)
        {
            try
            {


                GeneralOutputModel generalOutput = new GeneralOutputModel();
                ParamUpdateUser prApi = new ParamUpdateUser();
                var token = HttpContext.Session.GetString("token");
                //List<ParamUpdateReceiver> prApiReceiver = new List<ParamUpdateReceiver>();
                //prApi.Iduser = pr.Iduser;
                //prApi.IDgroup = pr.IDgroup;

                generalOutput = await _dataAccessProvider.PutUpdateUserSekDirAsync("User/UpdateSettingSeketaris", token, pr);
                var jsonApiResponseSerialize = JsonConvert.SerializeObject(generalOutput.Result);



                if (generalOutput.Status == "OK")
                {
                    TempData["status"] = "OK";
                    TempData["title"] = "Berhasil";
                    TempData["pesan"] = "Menambah Data Seketaris";
                }
                else if (generalOutput.Status == "UA")
                {
                    TempData["status"] = "NG";
                    TempData["pesan"] = "Session habis silahkan login kembali";
                    return RedirectToAction("Logout", "Home");
                }
                else if (generalOutput.Status == "NOTOK")
                {
                    TempData["status"] = "NG";
                    TempData["title"] = "Gagal";
                    TempData["pesan"] = "Seketaris Sudah Digunakan";
                }
                else
                {
                    TempData["status"] = "NG";
                    TempData["title"] = "Gagal menambah data seketaris";
                    TempData["pesan"] = generalOutput.Message;
                }


                return Redirect("~/SettingUser/SecretarySetting");
            }
            catch (Exception ex)
            {
                TempData["status"] = "NG";
                TempData["title"] = "Gagal merubah dokumen masuk";
                TempData["pesan"] = ex.ToString();
                return Redirect("~/SettingUser/SecretarySetting");
            }
        }
       
        public async Task<IActionResult> DeleteSekDir(Guid id)
        {

            GeneralOutputModel generalOutput = new GeneralOutputModel();
            var token = HttpContext.Session.GetString("token");
            ParamGetDetailUserSekdir prApi = new ParamGetDetailUserSekdir();
            prApi.ID_SETDIRKOM = id;
            

            generalOutput = await _dataAccessProvider.PutDeleteUserSekdirAsync("User/DeleteSettingUserSekdir", token, prApi);
            var jsonApiResponseSerialize = JsonConvert.SerializeObject(generalOutput.Result);
            //var userOutput = JsonConvert.DeserializeObject<DataOuputUser>(jsonApiResponseSerialize);


            if (generalOutput.Status == "OK")
            {
                TempData["status"] = "OK";
                TempData["title"] = "Berhasil";
                TempData["pesan"] = "Menghapus Data Seketaris";
            }
           
            else if (generalOutput.Status == "UA")
            {
                TempData["status"] = "NG";
                TempData["pesan"] = "Session habis silahkan login kembali";
                return RedirectToAction("Logout", "Home");
            }

            return Redirect("~/SettingUser/SecretarySetting");
        }


        public async Task<IActionResult> SuperUser()
        {
            List<DataOuputSuperUser> getUserPgaAll = new List<DataOuputSuperUser>();
            GeneralOutputModel generalOutput = new GeneralOutputModel();
            var token = HttpContext.Session.GetString("token");
            generalOutput = await _dataAccessProvider.GetUserSuperasync("User/GetSuperUser/", token);
            var jsonApiResponseSerialize = JsonConvert.SerializeObject(generalOutput.Result);
            var getSuperUserAll = JsonConvert.DeserializeObject<List<DataOuputSuperUser>>(jsonApiResponseSerialize);

            if (generalOutput.Status == "UA")
            {
                return RedirectToAction("Logout", "Home");
            }
            ViewBag.SelectSuperUser = getSuperUserAll;
            return View();
        }

        [HttpPost]
        public async Task<string> GetSupertUserList()
        {
            SuperUserOutputWeb documentOutput = new SuperUserOutputWeb();
            GeneralOutputModel generalOutput = new GeneralOutputModel();
            List<Dictionary<String, String>> _list = new List<Dictionary<string, string>>();
            var token = HttpContext.Session.GetString("token");
            var draw = Request.Form["draw"].FirstOrDefault();
            var sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
            var sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault();
            var searchValue = Request.Form["search[value]"].FirstOrDefault();
            int pageSize = Convert.ToInt32(Request.Form["length"].FirstOrDefault() ?? "0");
            int skip = Convert.ToInt32(Request.Form["start"].FirstOrDefault() ?? "0");

            ParamGetUsertWeb pr = new ParamGetUsertWeb();
            pr.draw = draw;
            pr.sortColumn = sortColumn;
            pr.sortColumnDirection = sortColumnDirection;
            pr.searchValue = searchValue;
            pr.pageSize = pageSize;
            pr.start = skip;

            generalOutput = await _dataAccessProvider.GetDataSuperUserAsync("User/GetAllDataSuperUser/", token, pr);
            var jsonApiResponseSerialize = JsonConvert.SerializeObject(generalOutput.Result);
            documentOutput = JsonConvert.DeserializeObject<SuperUserOutputWeb>(jsonApiResponseSerialize);

            return JsonConvert.SerializeObject(documentOutput);
        }


        [HttpPost]
        public async Task<IActionResult> UpdateSuperUser(ParamGetDetailUser pr)
        {
            try
            {


                GeneralOutputModel generalOutput = new GeneralOutputModel();
                ParamUpdateUser prApi = new ParamUpdateUser();
                var token = HttpContext.Session.GetString("token");
                //List<ParamUpdateReceiver> prApiReceiver = new List<ParamUpdateReceiver>();
                //prApi.Iduser = pr.Iduser;
                //prApi.IDgroup = pr.IDgroup;

                generalOutput = await _dataAccessProvider.PutUpdateSuperUserAsync("User/UpdateSuperUser", token, pr);
                var jsonApiResponseSerialize = JsonConvert.SerializeObject(generalOutput.Result);



                if (generalOutput.Status == "OK")
                {
                    TempData["status"] = "OK";
                    TempData["title"] = "Berhasil";
                    TempData["pesan"] = "Menambah Data Super User";
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
                    TempData["title"] = "Gagal merubah data super user";
                    TempData["pesan"] = generalOutput.Message;
                }


                return Redirect("~/SettingUser/SuperUser");
            }
            catch (Exception ex)
            {
                TempData["status"] = "NG";
                TempData["title"] = "Gagal menambah super user";
                TempData["pesan"] = ex.ToString();
                return Redirect("~/SettingUser/SuperUser");
            }
        }

        public async Task<IActionResult> DeleteSuperUser(Guid id)
        {

            GeneralOutputModel generalOutput = new GeneralOutputModel();
            var token = HttpContext.Session.GetString("token");
            ParamGetDetailUser prApi = new ParamGetDetailUser();
            prApi.Iduser = id;


            generalOutput = await _dataAccessProvider.PutDeleteSuperUserAsync("User/DeleteSuperUser", token, prApi);
            var jsonApiResponseSerialize = JsonConvert.SerializeObject(generalOutput.Result);
            //var userOutput = JsonConvert.DeserializeObject<DataOuputUser>(jsonApiResponseSerialize);


            if (generalOutput.Status == "OK")
            {
                TempData["status"] = "OK";
                TempData["title"] = "Berhasil";
                TempData["pesan"] = "Menghapus Data Super User";
            }
            else if (generalOutput.Status == "UA")
            {
                TempData["status"] = "NG";
                TempData["pesan"] = "Session habis silahkan login kembali";
                return RedirectToAction("Logout", "Home");
            }

            return Redirect("~/SettingUser/SuperUser");
        }

        public async Task<IActionResult> AdminHct()
        {
            List<DataOuputAdminHCT> getUserPgaAll = new List<DataOuputAdminHCT>();
            GeneralOutputModel generalOutput = new GeneralOutputModel();
            var token = HttpContext.Session.GetString("token");
            generalOutput = await _dataAccessProvider.GetUserAdminHctsync("User/AdminHct/", token);
            var jsonApiResponseSerialize = JsonConvert.SerializeObject(generalOutput.Result);
            var getSuperUserAll = JsonConvert.DeserializeObject<List<DataOuputAdminHCT>>(jsonApiResponseSerialize);

            if (generalOutput.Status == "UA")
            {
                return RedirectToAction("Logout", "Home");
            }
            ViewBag.SelectSuperUser = getSuperUserAll;
            return View();
        }

        [HttpPost]
        public async Task<string> GetAdminHctList()
        {
            AdminHCTOutputWeb documentOutput = new AdminHCTOutputWeb();
            GeneralOutputModel generalOutput = new GeneralOutputModel();
            List<Dictionary<String, String>> _list = new List<Dictionary<string, string>>();
            var token = HttpContext.Session.GetString("token");
            var draw = Request.Form["draw"].FirstOrDefault();
            var sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
            var sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault();
            var searchValue = Request.Form["search[value]"].FirstOrDefault();
            int pageSize = Convert.ToInt32(Request.Form["length"].FirstOrDefault() ?? "0");
            int skip = Convert.ToInt32(Request.Form["start"].FirstOrDefault() ?? "0");

            ParamGetUsertWeb pr = new ParamGetUsertWeb();
            pr.draw = draw;
            pr.sortColumn = sortColumn;
            pr.sortColumnDirection = sortColumnDirection;
            pr.searchValue = searchValue;
            pr.pageSize = pageSize;
            pr.start = skip;

            generalOutput = await _dataAccessProvider.GetDataAdminHctAsync("User/GetAllDataAdminHct/", token, pr);
            var jsonApiResponseSerialize = JsonConvert.SerializeObject(generalOutput.Result);
            documentOutput = JsonConvert.DeserializeObject<AdminHCTOutputWeb>(jsonApiResponseSerialize);

            return JsonConvert.SerializeObject(documentOutput);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateAdminHct(ParamGetDetailUser pr)
        {
            try
            {


                GeneralOutputModel generalOutput = new GeneralOutputModel();
                ParamUpdateUser prApi = new ParamUpdateUser();
                var token = HttpContext.Session.GetString("token");
                //List<ParamUpdateReceiver> prApiReceiver = new List<ParamUpdateReceiver>();
                //prApi.Iduser = pr.Iduser;
                //prApi.IDgroup = pr.IDgroup;

                generalOutput = await _dataAccessProvider.PutUpdateAdminHctAsync("User/UpdateAdminHCT", token, pr);
                var jsonApiResponseSerialize = JsonConvert.SerializeObject(generalOutput.Result);



                if (generalOutput.Status == "OK")
                {
                    TempData["status"] = "OK";
                    TempData["title"] = "Berhasil";
                    TempData["pesan"] = "Menambah Data admin hct";
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
                    TempData["title"] = "Gagal menambah admin hct";
                    TempData["pesan"] = generalOutput.Message;
                }


                return Redirect("~/SettingUser/AdminHct");
            }
            catch (Exception ex)
            {
                TempData["status"] = "NG";
                TempData["title"] = "Gagal menambah admin hct";
                TempData["pesan"] = ex.ToString();
                return Redirect("~/SettingUser/AdminHct");
            }
        }

        public async Task<IActionResult> DeleteAdminHct(Guid id)
        {

            GeneralOutputModel generalOutput = new GeneralOutputModel();
            var token = HttpContext.Session.GetString("token");
            ParamGetDetailUser prApi = new ParamGetDetailUser();
            prApi.Iduser = id;


            generalOutput = await _dataAccessProvider.PutDeleteAdminHctAsync("User/DeleteAdminHct", token, prApi);
            var jsonApiResponseSerialize = JsonConvert.SerializeObject(generalOutput.Result);
            //var userOutput = JsonConvert.DeserializeObject<DataOuputUser>(jsonApiResponseSerialize);


            if (generalOutput.Status == "OK")
            {
                TempData["status"] = "OK";
                TempData["title"] = "Berhasil";
                TempData["pesan"] = "Menghapus Data Admin Hct";
            }
            else if (generalOutput.Status == "UA")
            {
                TempData["status"] = "NG";
                TempData["pesan"] = "Session habis silahkan login kembali";
                return RedirectToAction("Logout", "Home");
            }

            return Redirect("~/SettingUser/AdminHct");
        }

        public async Task<IActionResult> Emplsetposition()
        {
            List<AdminDivisionOutput> divisionList = new List<AdminDivisionOutput>();
            List<AdminDivisionOutput> adminDivisionList = new List<AdminDivisionOutput>();
            GeneralOutputModel generalOutput = new GeneralOutputModel();
            var token = HttpContext.Session.GetString("token");
            generalOutput = await _dataAccessProvider.GetUnitAsync("User/GetAllDivision/", token);
            var jsonApiResponseSerialize = JsonConvert.SerializeObject(generalOutput.Result);
            divisionList = JsonConvert.DeserializeObject<List<AdminDivisionOutput>>(jsonApiResponseSerialize);

            if (generalOutput.Status == "UA")
            {
                return RedirectToAction("Logout", "Home");
            }
            generalOutput = await _dataAccessProvider.GetUnitHctAsync("User/GetAllUserAdminDivisionHct/", token);
            jsonApiResponseSerialize = JsonConvert.SerializeObject(generalOutput.Result);
            adminDivisionList = JsonConvert.DeserializeObject<List<AdminDivisionOutput>>(jsonApiResponseSerialize);

            if (generalOutput.Status == "UA")
            {
                return RedirectToAction("Logout", "Home");
            }

            List<StringmapOutputNonEoffice> LevelList = new List<StringmapOutputNonEoffice>();
            ParamGetStringmapNonEoffice prr = new ParamGetStringmapNonEoffice();
            prr.objectName = "tr_level_employee";
            prr.attributeName = "STATUS_LEVEL";

            generalOutput = await _dataAccessProvider.GetStringmapNonEofficeAsync("General/GetStringmapLevelStgJbtnWeb/", prr, token);
            jsonApiResponseSerialize = JsonConvert.SerializeObject(generalOutput.Result);
            LevelList = JsonConvert.DeserializeObject<List<StringmapOutputNonEoffice>>(jsonApiResponseSerialize);

            if (generalOutput.Status == "UA")
            {
                return RedirectToAction("Logout", "Home");
            }


            List<OuputLevelemp> ViewSdataList = new List<OuputLevelemp>();
            generalOutput = await _dataAccessProvider.GetUnitHctAsync("User/GetAllEmpLevel/", token);
            jsonApiResponseSerialize = JsonConvert.SerializeObject(generalOutput.Result);
            ViewSdataList = JsonConvert.DeserializeObject<List<OuputLevelemp>>(jsonApiResponseSerialize);
            ViewBag.DivisionList = divisionList;
            ViewBag.AdminDivisionList = adminDivisionList;
            ViewBag.LevelEmp = LevelList;
            ViewBag.ViewSdata = ViewSdataList;

            return View();
        }

        [HttpPost]
        public async Task<string> SettingLevelEmployee(Guid idUnit, Guid idUser,int idLevel)
        {
            DocumentOutputWeb documentOutput = new DocumentOutputWeb();
            GeneralOutputModel generalOutput = new GeneralOutputModel();
            var token = HttpContext.Session.GetString("token");

            ParamUpdateLevelemp pr = new ParamUpdateLevelemp();
            pr.idUnit = idUnit;
            pr.idUser = idUser;
            pr.idLevel = idLevel;

            generalOutput = await _dataAccessProvider.InsertLevelEmpl("User/InsertLevelEmpl/", token, pr);
            var jsonApiResponseSerialize = JsonConvert.SerializeObject(generalOutput);
            //generalOutput = JsonConvert.DeserializeObject<GeneralOutputModel>(jsonApiResponseSerialize);

            if (generalOutput.Status == "OK")
            {
                TempData["status"] = "OK";
                TempData["title"] = "Berhasil";
                TempData["pesan"] = "Menambah data";
            }
           
            else
            {
                TempData["status"] = "NG";
                TempData["title"] = "Gagal menambah data";
                TempData["pesan"] = generalOutput.Message;
            }

            return jsonApiResponseSerialize;
        }

        public async Task<IActionResult> DeleteSetLvlEmp(Guid idUser, Guid idemplevel)
        {

            DocumentOutputWeb documentOutput = new DocumentOutputWeb();
            GeneralOutputModel generalOutput = new GeneralOutputModel();
            var token = HttpContext.Session.GetString("token");

            ParamUpdateLevelemp pr = new ParamUpdateLevelemp();
            pr.idemplevel = idemplevel;
            pr.idUser = idUser;
            pr.statuscode = 0;

            generalOutput = await _dataAccessProvider.UpdateLevelEMp("User/DeleteLevelEmpl/", token, pr);
            //var jsonApiResponseSerialize = JsonConvert.SerializeObject(generalOutput);
            //generalOutput = JsonConvert.DeserializeObject<GeneralOutputModel>(jsonApiResponseSerialize);
            if (generalOutput.Status == "OK")
            {
                TempData["status"] = "OK";
                TempData["title"] = "Berhasil";
                TempData["pesan"] = "Menghapus data";
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
                TempData["title"] = "Gagal menghapus admin divisi";
                TempData["pesan"] = generalOutput.Message;
            }
            return Redirect("~/SettingUser/Emplsetposition");
        }

        [HttpPost]
        public async Task<string> GetUserDivisiUsingLevelEmp(Guid idUnit)
        {
            DocumentOutputWeb documentOutput = new DocumentOutputWeb();
            GeneralOutputModel generalOutput = new GeneralOutputModel();
            var token = HttpContext.Session.GetString("token");

            ParamGetUserByUnit pr = new ParamGetUserByUnit();
            pr.idUnit = idUnit;

            generalOutput = await _dataAccessProvider.GetUserByUnitAsync("User/GetUserByUnit/", token, pr);
            var jsonApiResponseSerialize = JsonConvert.SerializeObject(generalOutput);
            //generalOutput = JsonConvert.DeserializeObject<GeneralOutputModel>(jsonApiResponseSerialize);

            return jsonApiResponseSerialize;
        }


        public async Task<IActionResult> ApprovalSignatureUser()
        {
            List<OuputSignature> getUserPgaAll = new List<OuputSignature>();
            GeneralOutputModel generalOutput = new GeneralOutputModel();
            var token = HttpContext.Session.GetString("token");
            generalOutput = await _dataAccessProvider.GetOuputSignatureUser("User/GetOuputApprovalSignatureUser/", token);
            var jsonApiResponseSerialize = JsonConvert.SerializeObject(generalOutput.Result);
            var getUserPermohonanApproval = JsonConvert.DeserializeObject<List<OuputSignature>>(jsonApiResponseSerialize);

            generalOutput = await _dataAccessProvider.GetOuputSignatureUser("User/GetOuputApprovalRejectSignatureUserWeb/", token);
            jsonApiResponseSerialize = JsonConvert.SerializeObject(generalOutput.Result);
            var getApprovalReject = JsonConvert.DeserializeObject<List<OuputSignature>>(jsonApiResponseSerialize);

            if (generalOutput.Status == "UA")
            {
                return RedirectToAction("Logout", "Home");
            }
            ViewBag.UserSignature = getUserPermohonanApproval;
            ViewBag.UserSignatureApprovalReject = getApprovalReject;
            return View();
        }


        public async Task<string> ApprovalSignature(IEnumerable<ParamGetApprovalSignature> items)
        {
            GeneralOutputModel generalOutput = new GeneralOutputModel();
            var token = HttpContext.Session.GetString("token");
            //ParamGetApprovalSignature data = new ParamGetApprovalSignature();
            List<ParamGetApprovalSignature> data = new List<ParamGetApprovalSignature>();
            ParamJsonStirngSiganture pr = new ParamJsonStirngSiganture();
            foreach (var item in items)
            {
                if(item.isChecked == true && item.idMg !=null)
                {
                    var value = item.idMg;
                    data.Add(new ParamGetApprovalSignature
                    {
                        idMg = value,
                    });
                }
            }


            pr.jsonDataString = data;
            //prApi.idMg = item.idMg;
            generalOutput = await _dataAccessProvider.PutApprovalUserSigantureAsync("User/ApprovalSignatureUserWeb", token, pr);
            var jsonApiResponseSerialize = JsonConvert.SerializeObject(generalOutput.Result);

            if (generalOutput.Status == "OK")
            {
                TempData["status"] = "OK";
                TempData["title"] = "Berhasil";
                TempData["pesan"] = "Tanda tangan user disetujui";
            }
            else
            {
                TempData["status"] = "NG";
                TempData["title"] = "Gagal";
                TempData["pesan"] = "Data harus di ceklist";
            }

            return jsonApiResponseSerialize;
            //return Redirect("~/SettingUser/ApprovalSignatureUser");
        }

        public async Task<string> RejectSignature(IEnumerable<ParamGetApprovalSignature> items)
        {
            GeneralOutputModel generalOutput = new GeneralOutputModel();
            var token = HttpContext.Session.GetString("token");
            //ParamGetApprovalSignature data = new ParamGetApprovalSignature();
            List<ParamGetApprovalSignature> data = new List<ParamGetApprovalSignature>();
            ParamJsonStirngSiganture pr = new ParamJsonStirngSiganture();
            foreach (var item in items)
            {
                if (item.isChecked == true)
                {
                    var value = item.idMg;
                    data.Add(new ParamGetApprovalSignature
                    {
                        idMg = value,
                    });
                }

            }
            pr.jsonDataString = data;
            generalOutput = await _dataAccessProvider.PutApprovalUserSigantureAsync("User/RejectSignatureUserWeb", token, pr);
            var jsonApiResponseSerialize = JsonConvert.SerializeObject(generalOutput.Result);

            if (generalOutput.Status == "OK")
            {
                TempData["status"] = "OK";
                TempData["title"] = "Di tolak";
                TempData["pesan"] = "Tanda tangan User Tidak disetujui";
            }
            else
            {
                TempData["status"] = "NG";
                TempData["title"] = "Gagal";
                TempData["pesan"] = "Data harus di ceklist";
            }

            return jsonApiResponseSerialize;
        }

        
        public async Task<IActionResult> ApprovalSignatureOneData(Guid id)
        {

            GeneralOutputModel generalOutput = new GeneralOutputModel();
            var token = HttpContext.Session.GetString("token");
            ParamGetApprovalSignatureOnedata prApi = new ParamGetApprovalSignatureOnedata();
            prApi.idMg = id;


            generalOutput = await _dataAccessProvider.PutApprovalSigantureOneData("User/ApprovalSignatureOneDataWeb", token, prApi);
            var jsonApiResponseSerialize = JsonConvert.SerializeObject(generalOutput.Result);
            //var userOutput = JsonConvert.DeserializeObject<DataOuputUser>(jsonApiResponseSerialize);


            if (generalOutput.Status == "OK")
            {
                TempData["status"] = "OK";
                TempData["title"] = "Berhasil";
                TempData["pesan"] = "Tanda tangan user di setujui";
            }
            else if (generalOutput.Status == "UA")
            {
                TempData["status"] = "NG";
                TempData["pesan"] = "Session habis silahkan login kembali";
                return RedirectToAction("Logout", "Home");
            }

            return Redirect("~/SettingUser/ApprovalSignatureUser");
        }

        public async Task<IActionResult> RejectSignatureOneData(Guid id)
        {

            GeneralOutputModel generalOutput = new GeneralOutputModel();
            var token = HttpContext.Session.GetString("token");
            ParamGetApprovalSignatureOnedata prApi = new ParamGetApprovalSignatureOnedata();
            prApi.idMg = id;


            generalOutput = await _dataAccessProvider.PutRejectSigantureOneData("User/RejectSignatureOneDataWeb", token, prApi);
            var jsonApiResponseSerialize = JsonConvert.SerializeObject(generalOutput.Result);
            //var userOutput = JsonConvert.DeserializeObject<DataOuputUser>(jsonApiResponseSerialize);


            if (generalOutput.Status == "OK")
            {
                TempData["status"] = "OK";
                TempData["title"] = "Tidak disetujui";
                TempData["pesan"] = "Tanda tangan tidak disetujui";
            }
            else if (generalOutput.Status == "UA")
            {
                TempData["status"] = "NG";
                TempData["pesan"] = "Session habis silahkan login kembali";
                return RedirectToAction("Logout", "Home");
            }

            return Redirect("~/SettingUser/ApprovalSignatureUser");
        }
    
    
        public async Task<IActionResult> SettingApproval()
        {
            return View();
        }


        public async Task<IActionResult> SetApprovalBod()
        {
            List<DataOuputSuperUser> getUserPgaAll = new List<DataOuputSuperUser>();
            GeneralOutputModel generalOutput = new GeneralOutputModel();
            var token = HttpContext.Session.GetString("token");
            generalOutput = await _dataAccessProvider.GetDropDownUserBod("User/GetDropdwonSetUserApprovalBod/", token);
            var jsonApiResponseSerialize = JsonConvert.SerializeObject(generalOutput.Result);
            var getSuperUserAll = JsonConvert.DeserializeObject<List<DataOuputSuperUser>>(jsonApiResponseSerialize);

            List<AdminDivisionOutput> divisionList = new List<AdminDivisionOutput>();
            generalOutput = await _dataAccessProvider.GetUnitAsync("User/GetAllDivision/", token);
             jsonApiResponseSerialize = JsonConvert.SerializeObject(generalOutput.Result);
            divisionList = JsonConvert.DeserializeObject<List<AdminDivisionOutput>>(jsonApiResponseSerialize);

            if (generalOutput.Status == "UA")
            {
                return RedirectToAction("Logout", "Home");
            }
            ViewBag.SelectBod = getSuperUserAll;

            ViewBag.SelectDivisi = divisionList;
            return View();
        }

        [HttpPost]
        public async Task<string> GetUserSetApprvlBodList()
        {
            SuperUserOutputWeb documentOutput = new SuperUserOutputWeb();
            GeneralOutputModel generalOutput = new GeneralOutputModel();
            List<Dictionary<String, String>> _list = new List<Dictionary<string, string>>();
            var token = HttpContext.Session.GetString("token");
            var draw = Request.Form["draw"].FirstOrDefault();
            var sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
            var sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault();
            var searchValue = Request.Form["search[value]"].FirstOrDefault();
            int pageSize = Convert.ToInt32(Request.Form["length"].FirstOrDefault() ?? "0");
            int skip = Convert.ToInt32(Request.Form["start"].FirstOrDefault() ?? "0");

            ParamGetUsertWeb pr = new ParamGetUsertWeb();
            pr.draw = draw;
            pr.sortColumn = sortColumn;
            pr.sortColumnDirection = sortColumnDirection;
            pr.searchValue = searchValue;
            pr.pageSize = pageSize;
            pr.start = skip;

            generalOutput = await _dataAccessProvider.GetViewDataUserBod("User/GetAllDataSettingBod/", token, pr);
            var jsonApiResponseSerialize = JsonConvert.SerializeObject(generalOutput.Result);
            documentOutput = JsonConvert.DeserializeObject<SuperUserOutputWeb>(jsonApiResponseSerialize);

            return JsonConvert.SerializeObject(documentOutput);
        }


        public async Task<IActionResult> DeleteSettingBod(Guid id)
        {

            GeneralOutputModel generalOutput = new GeneralOutputModel();
            var token = HttpContext.Session.GetString("token");
            ParamGetDetailUser prApi = new ParamGetDetailUser();
            prApi.IdBod = id;


            generalOutput = await _dataAccessProvider.DeleteDataBodAsync("User/DeleteSettingApprvlBodWeb", token, prApi);
            var jsonApiResponseSerialize = JsonConvert.SerializeObject(generalOutput.Result);
            //var userOutput = JsonConvert.DeserializeObject<DataOuputUser>(jsonApiResponseSerialize);


            if (generalOutput.Status == "OK")
            {
                TempData["status"] = "OK";
                TempData["title"] = "Berhasil";
                TempData["pesan"] = "Menghapus Data Super User";
            }
            else if (generalOutput.Status == "UA")
            {
                TempData["status"] = "NG";
                TempData["pesan"] = "Session habis silahkan login kembali";
                return RedirectToAction("Logout", "Home");
            }

            return Redirect("~/SettingUser/SetApprovalBod");
        }


        [HttpPost]
        public async Task<IActionResult> CreateUserBod(ParamGetDetailUser pr)
        {
            try
            {


                GeneralOutputModel generalOutput = new GeneralOutputModel();
                ParamUpdateUser prApi = new ParamUpdateUser();
                var token = HttpContext.Session.GetString("token");

                generalOutput = await _dataAccessProvider.CreatSettingBodAsync("User/CreateBodWeb", token, pr);
                var jsonApiResponseSerialize = JsonConvert.SerializeObject(generalOutput.Result);
                if (generalOutput.Status == "OK")
                {
                    TempData["status"] = "OK";
                    TempData["title"] = "Berhasil";
                    TempData["pesan"] = "Menambah Data";
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
                    TempData["title"] = "Gagal merubah data super user";
                    TempData["pesan"] = generalOutput.Message;
                }


                return Redirect("~/SettingUser/SetApprovalBod");
            }
            catch (Exception ex)
            {
                TempData["status"] = "NG";
                TempData["title"] = "Gagal menambah super user";
                TempData["pesan"] = ex.ToString();

                return Redirect("~/SettingUser/SetApprovalBod");
            }
        }



        public async Task<IActionResult> AdminPengadaan()
        {
            List<DataOuputAdminPengadaan> getUserPgaAll = new List<DataOuputAdminPengadaan>();
            GeneralOutputModel generalOutput = new GeneralOutputModel();
            var token = HttpContext.Session.GetString("token");
            generalOutput = await _dataAccessProvider.GetUserAdminPengadaansync("User/AdminPengadaan/", token);
            var jsonApiResponseSerialize = JsonConvert.SerializeObject(generalOutput.Result);
            var getSuperUserAll = JsonConvert.DeserializeObject<List<DataOuputAdminPengadaan>>(jsonApiResponseSerialize);

            if (generalOutput.Status == "UA")
            {
                return RedirectToAction("Logout", "Home");
            }
            ViewBag.SelectSuperUser = getSuperUserAll;
            return View();
        }

        [HttpPost]
        public async Task<string> GetAdminPengadaanList()
        {
            AdminPengadaanOutputWeb documentOutput = new AdminPengadaanOutputWeb();
            GeneralOutputModel generalOutput = new GeneralOutputModel();
            List<Dictionary<String, String>> _list = new List<Dictionary<string, string>>();
            var token = HttpContext.Session.GetString("token");
            var draw = Request.Form["draw"].FirstOrDefault();
            var sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
            var sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault();
            var searchValue = Request.Form["search[value]"].FirstOrDefault();
            int pageSize = Convert.ToInt32(Request.Form["length"].FirstOrDefault() ?? "0");
            int skip = Convert.ToInt32(Request.Form["start"].FirstOrDefault() ?? "0");

            ParamGetUsertWeb pr = new ParamGetUsertWeb();
            pr.draw = draw;
            pr.sortColumn = sortColumn;
            pr.sortColumnDirection = sortColumnDirection;
            pr.searchValue = searchValue;
            pr.pageSize = pageSize;
            pr.start = skip;

            generalOutput = await _dataAccessProvider.GetDataAdminPengadaanAsync("User/GetAllDataAdminPengadaan/", token, pr);
            var jsonApiResponseSerialize = JsonConvert.SerializeObject(generalOutput.Result);
            documentOutput = JsonConvert.DeserializeObject<AdminPengadaanOutputWeb>(jsonApiResponseSerialize);

            return JsonConvert.SerializeObject(documentOutput);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateAdminPengadaan(ParamGetDetailUser pr)
        {
            try
            {
                GeneralOutputModel generalOutput = new GeneralOutputModel();
                ParamUpdateUser prApi = new ParamUpdateUser();
                var token = HttpContext.Session.GetString("token");
                //List<ParamUpdateReceiver> prApiReceiver = new List<ParamUpdateReceiver>();
                //prApi.Iduser = pr.Iduser;
                //prApi.IDgroup = pr.IDgroup;

                generalOutput = await _dataAccessProvider.PutUpdateAdminPengadaanAsync("User/UpdateAdminPengadaan", token, pr);
                var jsonApiResponseSerialize = JsonConvert.SerializeObject(generalOutput.Result);



                if (generalOutput.Status == "OK")
                {
                    TempData["status"] = "OK";
                    TempData["title"] = "Berhasil";
                    TempData["pesan"] = "Menambah Data admin hct";
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
                    TempData["title"] = "Gagal menambah admin Pengadaan";
                    TempData["pesan"] = generalOutput.Message;
                }


                return Redirect("~/SettingUser/AdminPengadaan");
            }
            catch (Exception ex)
            {
                TempData["status"] = "NG";
                TempData["title"] = "Gagal menambah admin hct";
                TempData["pesan"] = ex.ToString();
                return Redirect("~/SettingUser/AdminPengadaan");
            }
        }

        public async Task<IActionResult> DeleteAdminPengadaan(Guid id)
        {

            GeneralOutputModel generalOutput = new GeneralOutputModel();
            var token = HttpContext.Session.GetString("token");
            ParamGetDetailUser prApi = new ParamGetDetailUser();
            prApi.Iduser = id;


            generalOutput = await _dataAccessProvider.PutDeleteAdminPengadaanAsync("User/DeleteAdminPengadaan", token, prApi);
            var jsonApiResponseSerialize = JsonConvert.SerializeObject(generalOutput.Result);
            //var userOutput = JsonConvert.DeserializeObject<DataOuputUser>(jsonApiResponseSerialize);


            if (generalOutput.Status == "OK")
            {
                TempData["status"] = "OK";
                TempData["title"] = "Berhasil";
                TempData["pesan"] = "Menghapus Data Admin Hct";
            }
            else if (generalOutput.Status == "UA")
            {
                TempData["status"] = "NG";
                TempData["pesan"] = "Session habis silahkan login kembali";
                return RedirectToAction("Logout", "Home");
            }

            return Redirect("~/SettingUser/AdminPengadaan");
        }


        public async Task<IActionResult> SetPengadaan()
        {
            List<DataOuputSetPengadaan> getUserPgaAll = new List<DataOuputSetPengadaan>();
            GeneralOutputModel generalOutput = new GeneralOutputModel();
            var token = HttpContext.Session.GetString("token");
            generalOutput = await _dataAccessProvider.GetUserSetPengadaansync("User/SetPengadaan/", token);
            var jsonApiResponseSerialize = JsonConvert.SerializeObject(generalOutput.Result);
            var getSuperUserAll = JsonConvert.DeserializeObject<List<DataOuputSetPengadaan>>(jsonApiResponseSerialize);

            if (generalOutput.Status == "UA")
            {
                return RedirectToAction("Logout", "Home");
            }
            ViewBag.SelectSuperUser = getSuperUserAll;
            return View();
        }

        [HttpPost]
        public async Task<string> GetSetPengadaanList()
        {
            SetPengadaanOutputWeb documentOutput = new SetPengadaanOutputWeb();
            GeneralOutputModel generalOutput = new GeneralOutputModel();
            List<Dictionary<String, String>> _list = new List<Dictionary<string, string>>();
            var token = HttpContext.Session.GetString("token");
            var draw = Request.Form["draw"].FirstOrDefault();
            var sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
            var sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault();
            var searchValue = Request.Form["search[value]"].FirstOrDefault();
            int pageSize = Convert.ToInt32(Request.Form["length"].FirstOrDefault() ?? "0");
            int skip = Convert.ToInt32(Request.Form["start"].FirstOrDefault() ?? "0");

            ParamGetUsertWeb pr = new ParamGetUsertWeb();
            pr.draw = draw;
            pr.sortColumn = sortColumn;
            pr.sortColumnDirection = sortColumnDirection;
            pr.searchValue = searchValue;
            pr.pageSize = pageSize;
            pr.start = skip;

            generalOutput = await _dataAccessProvider.GetDataSetPengadaanAsync("User/GetAllDataSetPengadaanWeb/", token, pr);
            var jsonApiResponseSerialize = JsonConvert.SerializeObject(generalOutput.Result);
            documentOutput = JsonConvert.DeserializeObject<SetPengadaanOutputWeb>(jsonApiResponseSerialize);

            return JsonConvert.SerializeObject(documentOutput);
        }


        [HttpPost]
        public async Task<string> InsertPengadaan(Guid Id, string name, string min_nominal, string max_nominal, string approver1, string approver2)
        {
            DocumentOutputWeb documentOutput = new DocumentOutputWeb();
            GeneralOutputModel generalOutput = new GeneralOutputModel();
            var token = HttpContext.Session.GetString("token");

            ParamInsertPengadaan pr = new ParamInsertPengadaan();
            pr.Id = Id;
            pr.name = name;
            pr.min_nominal = min_nominal;
            pr.max_nominal = max_nominal;

            if (approver1 == null)
            {
                pr.approver1 = "00000000-0000-0000-0000-000000000000";

            }
            else
            {
                pr.approver1 = approver1;
            }
            if (approver2== null)
            {
                pr.approver2 = "00000000-0000-0000-0000-000000000000";

            }
            else
            {
                pr.approver2 = approver2;
            }

            generalOutput = await _dataAccessProvider.InsertDataPengadaan("User/InsertDataPengadaan/", token, pr);
            var jsonApiResponseSerialize = JsonConvert.SerializeObject(generalOutput);
            //generalOutput = JsonConvert.DeserializeObject<GeneralOutputModel>(jsonApiResponseSerialize);

            if (generalOutput.Status == "OK")
            {
                TempData["status"] = "OK";
                TempData["title"] = "Berhasil";
                TempData["pesan"] = "Menambah Data Pengadaan";
            }
            else
            {
                TempData["status"] = "NG";
                TempData["title"] = "Gagal Menambahkan Pengadaan";
                TempData["pesan"] = generalOutput.Message;
            }
            //return Redirect("~/SettingUser/SetPengadaan");
            return jsonApiResponseSerialize;
        }


        public async Task<IActionResult> SetDelegasi()
        {
            List<DataOuputSetDelegasi> getUserPgaAll = new List<DataOuputSetDelegasi>();
            GeneralOutputModel generalOutput = new GeneralOutputModel();
            var token = HttpContext.Session.GetString("token");
            generalOutput = await _dataAccessProvider.GetUserSetDelegasisync("User/SetDelegasi/", token);
            var jsonApiResponseSerialize = JsonConvert.SerializeObject(generalOutput.Result);
            var getSuperUserAll = JsonConvert.DeserializeObject<List<DataOuputSetDelegasi>>(jsonApiResponseSerialize);

            if (generalOutput.Status == "UA")
            {
                return RedirectToAction("Logout", "Home");
            }
            ViewBag.SelectSuperUser = getSuperUserAll;
            return View();
        }

        [HttpPost]
        public async Task<string> GetSetDelegasiList()
        {
            DataOuputSetDelegasi documentOutput = new DataOuputSetDelegasi();
            GeneralOutputModel generalOutput = new GeneralOutputModel();
            List<Dictionary<String, String>> _list = new List<Dictionary<string, string>>();
            var token = HttpContext.Session.GetString("token");
            var draw = Request.Form["draw"].FirstOrDefault();
            var sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
            var sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault();
            var searchValue = Request.Form["search[value]"].FirstOrDefault();
            int pageSize = Convert.ToInt32(Request.Form["length"].FirstOrDefault() ?? "0");
            int skip = Convert.ToInt32(Request.Form["start"].FirstOrDefault() ?? "0");

            ParamGetUsertWeb pr = new ParamGetUsertWeb();
            pr.draw = draw;
            pr.sortColumn = sortColumn;
            pr.sortColumnDirection = sortColumnDirection;
            pr.searchValue = searchValue;
            pr.pageSize = pageSize;
            pr.start = skip;

            generalOutput = await _dataAccessProvider.GetDataSetDelegasiAsync("User/GetAllDataSetDelegasiWeb/", token, pr);
            var jsonApiResponseSerialize = JsonConvert.SerializeObject(generalOutput.Result);
            //documentOutput = JsonConvert.DeserializeObject<DataOuputSetDelegasi>(jsonApiResponseSerialize);

            return jsonApiResponseSerialize;
        }

        [HttpPost]
        public async Task<string> InsertDelegasi(Guid Id, Guid id_user_delegasi, DateTime startdate, DateTime enddate)
        {
            DocumentOutputWeb documentOutput = new DocumentOutputWeb();
            GeneralOutputModel generalOutput = new GeneralOutputModel();
            var token = HttpContext.Session.GetString("token");

            ParamInsertDelegasi pr = new ParamInsertDelegasi();
            pr.Id = Id;
            //pr.id_user = id_user;
            pr.id_user_delegasi = id_user_delegasi;
            pr.StartDate = startdate;
            pr.EndDate = enddate;


            generalOutput = await _dataAccessProvider.InsertDataDelegasi("User/InsertDataDelegasi/", token, pr);
            var jsonApiResponseSerialize = JsonConvert.SerializeObject(generalOutput);
            //generalOutput = JsonConvert.DeserializeObject<GeneralOutputModel>(jsonApiResponseSerialize);

            if (generalOutput.Status == "OK")
            {
                TempData["status"] = "OK";
                TempData["title"] = "Berhasil";
                TempData["pesan"] = "Menambah Data Pengadaan";
            }
            else
            {
                TempData["status"] = "NG";
                TempData["title"] = "Gagal Menambahkan Pengadaan";
                TempData["pesan"] = generalOutput.Message;
            }

            return jsonApiResponseSerialize;
        }



        public async Task<IActionResult> ApprovalPengadaan()
        {

            List<DataOuputSetPengadaan> getJenisPengadaan = new List<DataOuputSetPengadaan>();
            List<DataOuputSetPengadaanApproval> getJenisPengadaanApproval = new List<DataOuputSetPengadaanApproval>();

            GeneralOutputModel generalOutput = new GeneralOutputModel();
            var token = HttpContext.Session.GetString("token");
            generalOutput = await _dataAccessProvider.GetDataPengadaansync("User/GetDataPengadaanAllWeb/", token);
            var jsonApiResponseSerialize = JsonConvert.SerializeObject(generalOutput.Result);
            var getJenisPengadaanAll = JsonConvert.DeserializeObject<List<DataOuputSetPengadaan>>(jsonApiResponseSerialize);


            generalOutput = await _dataAccessProvider.GetDataPengadaanApprovalsync("User/GetDataPengadaanApprovalWeb/", token);
            jsonApiResponseSerialize = JsonConvert.SerializeObject(generalOutput.Result);
            var getApproval = JsonConvert.DeserializeObject<List<DataOuputSetPengadaanApproval>>(jsonApiResponseSerialize);

            if (generalOutput.Status == "UA")
            {
                return RedirectToAction("Logout", "Home");
            }
            ViewBag.JenisPengadaan = getJenisPengadaanAll;
            ViewBag.JenisPengadaanApproval = getApproval;

            return View();
        }


        public async Task<string> ApprovalJenisPengadaan(IEnumerable<ParamGetApprovalPengadaan> items)
        {
            GeneralOutputModel generalOutput = new GeneralOutputModel();
            var token = HttpContext.Session.GetString("token");
            //ParamGetApprovalSignature data = new ParamGetApprovalSignature();
            List<ParamGetApprovalPengadaan> data = new List<ParamGetApprovalPengadaan>();
            ParamJsonStirngPengadaan pr = new ParamJsonStirngPengadaan();
            foreach (var item in items)
            {
                if (item.isChecked == true && item.Id != null)
                {
                    var value = item.Id;
                    data.Add(new ParamGetApprovalPengadaan
                    {
                        Id = value,
                    });
                }
            }


            pr.jsonDataString = data;
            //prApi.idMg = item.idMg;
            generalOutput = await _dataAccessProvider.PutApprovalPengadaanAsync("User/ApprovalJenisPengadaanWeb", token, pr);
            var jsonApiResponseSerialize = JsonConvert.SerializeObject(generalOutput.Result);

            if (generalOutput.Status == "OK")
            {
                TempData["status"] = "OK";
                TempData["title"] = "Berhasil";
                TempData["pesan"] = "Tanda tangan user disetujui";
            }
            else
            {
                TempData["status"] = "NG";
                TempData["title"] = "Gagal";
                TempData["pesan"] = "Data harus di ceklist";
            }

            return jsonApiResponseSerialize;
            //return Redirect("~/SettingUser/ApprovalSignatureUser");
        }

        //[HttpPost]
        //public async Task<IActionResult> GetDataPengadaanView(Guid id)
        //{

        //    List<DataOuputSetPengadaan> getJenisPengadaan = new List<DataOuputSetPengadaan>();
        //    GeneralOutputModel generalOutput = new GeneralOutputModel();
        //    var token = HttpContext.Session.GetString("token");
        //    ParamGetPengadaanModal prApi = new ParamGetPengadaanModal();
        //    prApi.Id = id;

        //    generalOutput = await _dataAccessProvider.GetDataPengadaanModalsync("User/GetDataPengadaanViewWeb/", token, prApi);
        //    var jsonApiResponseSerialize = JsonConvert.SerializeObject(generalOutput.Result);
        //    //var getJenisPengadaanAll = JsonConvert.DeserializeObject<List<DataOuputSetPengadaan>>(jsonApiResponseSerialize);

        //    return jsonApiResponseSerialize;


        //}


        [HttpPost]
        public async Task<string> GetDataPengadaanView(Guid id)
        {
            DocumentOutputWeb documentOutput = new DocumentOutputWeb();
            GeneralOutputModel generalOutput = new GeneralOutputModel();
            var token = HttpContext.Session.GetString("token");

            ParamGetPengadaanModal prApi = new ParamGetPengadaanModal();
            prApi.Id = id;

            generalOutput = await _dataAccessProvider.GetDataPengadaanModalsync("User/GetDataPengadaanViewWeb/", token, prApi);
            var jsonApiResponseSerialize = JsonConvert.SerializeObject(generalOutput);
            //generalOutput = JsonConvert.DeserializeObject<GeneralOutputModel>(jsonApiResponseSerialize);

            return jsonApiResponseSerialize;
        }


        public async Task<IActionResult> ApprovalDelegasi()
        {
      
            List<DataOuputSetDelegasiApproval> getDelegasi = new List<DataOuputSetDelegasiApproval>();
            //List<DataOuputSetDelegasiApproval> getDelegasiApproval = new List<DataOuputSetDelegasiApproval>();

            GeneralOutputModel generalOutput = new GeneralOutputModel();
            var token = HttpContext.Session.GetString("token");
            generalOutput = await _dataAccessProvider.GetDataDelegasisync("User/GetDataDelegasiWeb/", token);
            var jsonApiResponseSerialize = JsonConvert.SerializeObject(generalOutput.Result);
            var getDelegasiNew = JsonConvert.DeserializeObject<List<DataOuputSetDelegasiApproval>>(jsonApiResponseSerialize);


            generalOutput = await _dataAccessProvider.GetDataDelegasiApprovalsync("User/GetDataDelegasiApprovalWeb/", token);
            jsonApiResponseSerialize = JsonConvert.SerializeObject(generalOutput.Result);
            var getDelegasiApprovalReject = JsonConvert.DeserializeObject<List<DataOuputSetDelegasiApproval>>(jsonApiResponseSerialize);

            if (generalOutput.Status == "UA")
            {
                return RedirectToAction("Logout", "Home");
            }
            ViewBag.DelegasiNew = getDelegasiNew;
            ViewBag.DelegasiApprovalReject = getDelegasiApprovalReject;

            return View();
        }


        public async Task<string> ApprovalDelegasiUser(IEnumerable<ParamGetApprovalDelegasi> items)
        {
            GeneralOutputModel generalOutput = new GeneralOutputModel();
            var token = HttpContext.Session.GetString("token");
            //ParamGetApprovalSignature data = new ParamGetApprovalSignature();
            List<ParamGetApprovalDelegasi> data = new List<ParamGetApprovalDelegasi>();
            ParamJsonStirngDelegasi pr = new ParamJsonStirngDelegasi();
            foreach (var item in items)
            {
                if (item.isChecked == true && item.Id != null)
                {
                    var value = item.Id;
                    data.Add(new ParamGetApprovalDelegasi
                    {
                        Id = value,
                    });
                }
            }


            pr.jsonDataString = data;
            //prApi.idMg = item.idMg;
            generalOutput = await _dataAccessProvider.PutApprovalDelegasiAsync("User/ApprovalDelegasiUserWeb", token, pr);
            var jsonApiResponseSerialize = JsonConvert.SerializeObject(generalOutput.Result);

            if (generalOutput.Status == "OK")
            {
                TempData["status"] = "OK";
                TempData["title"] = "Berhasil";
                TempData["pesan"] = "Pengajuan Delegasi Disetujui";
            }
            else
            {
                TempData["status"] = "NG";
                TempData["title"] = "Gagal";
                TempData["pesan"] = "Data harus di ceklist";
            }

            return jsonApiResponseSerialize;
            //return Redirect("~/SettingUser/ApprovalSignatureUser");
        }


        public async Task<IActionResult> Resetpassword()
        {
            List<AdminDivisionOutput> divisionList = new List<AdminDivisionOutput>();
            List<AdminDivisionOutput> adminDivisionList = new List<AdminDivisionOutput>();
            GeneralOutputModel generalOutput = new GeneralOutputModel();
            var token = HttpContext.Session.GetString("token");
            generalOutput = await _dataAccessProvider.GetUnitAsync("User/GetAllDivision/", token);
            var jsonApiResponseSerialize = JsonConvert.SerializeObject(generalOutput.Result);
            divisionList = JsonConvert.DeserializeObject<List<AdminDivisionOutput>>(jsonApiResponseSerialize);

            if (generalOutput.Status == "UA")
            {
                return RedirectToAction("Logout", "Home");
            }
           

            if (generalOutput.Status == "UA")
            {
                return RedirectToAction("Logout", "Home");
            }

            ViewBag.DivisionList = divisionList;

            return View();
        }


        [HttpPost]
        public async Task<string> SettingResetPswd(Guid idUnit, Guid idUser)
        {
            DocumentOutputWeb documentOutput = new DocumentOutputWeb();
            GeneralOutputModel generalOutput = new GeneralOutputModel();
            var token = HttpContext.Session.GetString("token");

            ParamUpdateAdminDivisi pr = new ParamUpdateAdminDivisi();
            pr.idUnit = idUnit;
            pr.idUser = idUser;

            generalOutput = await _dataAccessProvider.UpdateAdminDivisi("User/UpdateResetPasswordWeb/", token, pr);
            var jsonApiResponseSerialize = JsonConvert.SerializeObject(generalOutput);
            //generalOutput = JsonConvert.DeserializeObject<GeneralOutputModel>(jsonApiResponseSerialize);

            if (generalOutput.Status == "OK")
            {
                TempData["status"] = "OK";
                TempData["title"] = "Berhasil";
                TempData["pesan"] = "Reset Password";
            }
            else
            {
                TempData["status"] = "NG";
                TempData["title"] = "Gagal Reset Password";
                TempData["pesan"] = generalOutput.Message;
            }

            return jsonApiResponseSerialize;
        }


        

    }
}
