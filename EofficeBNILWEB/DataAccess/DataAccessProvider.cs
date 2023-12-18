using EofficeBNILWEB.Models;
using Microsoft.VisualBasic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.AccessControl;
using System.Text;
using System.Text.Json;
using System.Web;

namespace EofficeBNILWEB.DataAccess
{
    public class DataAccessProvider : IDataAccessProvider
    {
        GeneralOutputModel output = new GeneralOutputModel();
        private readonly IConfiguration _config;
        private readonly string urlApi;
        private readonly string urllApi;
        public DataAccessProvider(IConfiguration config)
        {
            _config = config;
            urlApi = _config.GetValue<string>("AppSettings:apiUrl");
            urllApi = _config.GetValue<string>("AppSettings:apiUrll");
        }
        public async Task<GeneralOutputModel> GetDataMenuAsync(string param, string accessUrl, string token)
        {
            try 
            {
                GeneralOutputModel data = new GeneralOutputModel();
                HttpClient httpClient = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Get, urlApi + accessUrl + param);
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                HttpResponseMessage response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var apiResponse = await response.Content.ReadAsStringAsync();
                    data = JsonConvert.DeserializeObject<GeneralOutputModel>(apiResponse);
                }
                else if(response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    output.Status = "UA";
                    output.Message = "Session Habis";

                    return output;
                }
                return data;
            }catch(Exception ex)
            {
                output.Status = "NG";
                output.Message = ex.ToString();

                return output;
            }
        }

        public async Task<GeneralOutputModel> LoginAsync(LoginParam pr,string accessUrl)
        {
            try
            {
                GeneralOutputModel data = new GeneralOutputModel();
                string jsonString = JsonConvert.SerializeObject(pr);  //convert to JSON
                Dictionary<string, string> values = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonString);  //convert to key/value pairs
                HttpClient httpClient = new HttpClient();
                var response = await httpClient.PostAsync(urlApi + accessUrl, new FormUrlEncodedContent(values));
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var apiResponse = await response.Content.ReadAsStringAsync();
                    data = JsonConvert.DeserializeObject<GeneralOutputModel>(apiResponse);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    output.Status = "UA";
                    output.Message = "Session Habis";

                    return output;
                }
                return data;
            }
            catch (Exception ex)
            {
                output.Status = "NG";
                output.Message = ex.ToString();

                return output;
            }
        }

        public async Task<GeneralOutputModel> GetUnitAsync(string accessUrl, string token)
        {
            try
            {
                GeneralOutputModel data = new GeneralOutputModel();
                HttpClient httpClient = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Get, urlApi + accessUrl);
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                HttpResponseMessage response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var apiResponse = await response.Content.ReadAsStringAsync();
                    data = JsonConvert.DeserializeObject<GeneralOutputModel>(apiResponse);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    output.Status = "UA";
                    output.Message = "Session Habis";

                    return output;
                }
                return data;
            }
            catch (Exception ex)
            {
                output.Status = "NG";
                output.Message = ex.ToString();

                return output;
            }
        }

        public async Task<GeneralOutputModel> GetDocumentAsync(string accessUrl, string token, ParamGetDocumentWeb pr)
        {
            try
            {
                GeneralOutputModel data = new GeneralOutputModel();
                string jsonString = JsonConvert.SerializeObject(pr);  //convert to JSON
                Dictionary<string, string> values = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonString);  //convert to key/value pairs
                HttpClient httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                var response = await httpClient.PostAsync(urlApi + accessUrl, new FormUrlEncodedContent(values));
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var apiResponse = await response.Content.ReadAsStringAsync();
                    data = JsonConvert.DeserializeObject<GeneralOutputModel>(apiResponse);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    output.Status = "UA";
                    output.Message = "Session Habis";

                    return output;
                }
                return data;
            }
            catch (Exception ex)
            {
                output.Status = "NG";
                output.Message = ex.ToString();

                return output;
            }
        }

        public async Task<GeneralOutputModel> GetStringmapAsync(string accessUrl, ParamGetStringmap pr, string token)
        {
            try
            {
                GeneralOutputModel data = new GeneralOutputModel();
                string jsonString = JsonConvert.SerializeObject(pr);  //convert to JSON
                Dictionary<string, string> values = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonString);  //convert to key/value pairs
                HttpClient httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                var response = await httpClient.PostAsync(urlApi + accessUrl, new FormUrlEncodedContent(values));
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var apiResponse = await response.Content.ReadAsStringAsync();
                    data = JsonConvert.DeserializeObject<GeneralOutputModel>(apiResponse);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    output.Status = "UA";
                    output.Message = "Session Habis";

                    return output;
                }
                return data;
            }
            catch (Exception ex)
            {
                output.Status = "NG";
                output.Message = ex.ToString();

                return output;
            }
        }

        public async Task<GeneralOutputModel> PostInsertDocumentAsync(string accessUrl, ParamInsertDocument pr, string token)
        {
            try
            {
                GeneralOutputModel data = new GeneralOutputModel();
                string jsonString = JsonConvert.SerializeObject(pr);  //convert to JSON
                Dictionary<string, string> values = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonString);  //convert to key/value pairs
                HttpClient httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                var response = await httpClient.PostAsync(urlApi + accessUrl, new FormUrlEncodedContent(values));
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var apiResponse = await response.Content.ReadAsStringAsync();
                    data = JsonConvert.DeserializeObject<GeneralOutputModel>(apiResponse);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    output.Status = "UA";
                    output.Message = "Session Habis";

                    return output;
                }
                return data;
            }
            catch (Exception ex)
            {
                output.Status = "NG";
                output.Message = ex.ToString();

                return output;
            }
        }

        public async Task<GeneralOutputModel> GetDocumentByTrackingNumberAsync(string accessUrl, string token, ParamCheckTrackingNumber pr)
        {
            try
            {
                GeneralOutputModel data = new GeneralOutputModel();
                string jsonString = JsonConvert.SerializeObject(pr);  //convert to JSON
                Dictionary<string, string> values = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonString);  //convert to key/value pairs
                HttpClient httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                var response = await httpClient.PostAsync(urlApi + accessUrl, new FormUrlEncodedContent(values));
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var apiResponse = await response.Content.ReadAsStringAsync();
                    data = JsonConvert.DeserializeObject<GeneralOutputModel>(apiResponse);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    output.Status = "UA";
                    output.Message = "Session Habis";

                    return output;
                }
                return data;
            }
            catch (Exception ex)
            {
                output.Status = "NG";
                output.Message = ex.ToString();

                return output;
            }
        }

        public async Task<GeneralOutputModel> GetDetailDocumentAsync(string accessUrl, string token, ParamGetDetailDocument pr)
        {
            try
            {
                GeneralOutputModel data = new GeneralOutputModel();
                string jsonString = JsonConvert.SerializeObject(pr);  //convert to JSON
                Dictionary<string, string> values = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonString);  //convert to key/value pairs
                HttpClient httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                var response = await httpClient.PostAsync(urlApi + accessUrl, new FormUrlEncodedContent(values));
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var apiResponse = await response.Content.ReadAsStringAsync();
                    data = JsonConvert.DeserializeObject<GeneralOutputModel>(apiResponse);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    output.Status = "UA";
                    output.Message = "Session Habis";

                    return output;
                }
                return data;
            }
            catch (Exception ex)
            {
                output.Status = "NG";
                output.Message = ex.ToString();

                return output;
            }
        }

        public async Task<GeneralOutputModel> PutUpdateDocumentAsync(string accessUrl, string token, ParamUpdateDocument pr)
        {
            try
            {
                GeneralOutputModel data = new GeneralOutputModel();

                string jsonStringReceiver = JsonConvert.SerializeObject(pr.receiverDocument);
                string jsonString = JsonConvert.SerializeObject(pr);  //convert to JSON
                var objResponse1 = JsonConvert.DeserializeObject<List<ParamUpdateReceiver>>(jsonStringReceiver);
                var jObject = JObject.Parse(jsonString);
                jObject["receiverDocument"] = JsonConvert.SerializeObject(objResponse1);
                jsonString = JsonConvert.SerializeObject(jObject);  //convert to JSON
                Dictionary<string, string> values = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonString);  //convert to key/value pairs
                HttpClient httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                var response = await httpClient.PostAsync(urlApi + accessUrl, new FormUrlEncodedContent(values));
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    output.Status = "UA";
                    output.Message = "Session Habis";

                    return output;
                }
                else
                {
                    var apiResponse = await response.Content.ReadAsStringAsync();
                    data = JsonConvert.DeserializeObject<GeneralOutputModel>(apiResponse);
                }
                return data;
            }
            catch (Exception ex)
            {
                output.Status = "NG";
                output.Message = ex.ToString();

                return output;
            }
        }
        public async Task<GeneralOutputModel> PutReceiveDocumentAsync(string accessUrl, string token, ParamReceiveDocument pr)
        {
            try
            {
                GeneralOutputModel data = new GeneralOutputModel();
                string jsonString = JsonConvert.SerializeObject(pr);  //convert to JSON
                Dictionary<string, string> values = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonString);  //convert to key/value pairs
                HttpClient httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                var response = await httpClient.PostAsync(urlApi + accessUrl, new FormUrlEncodedContent(values));
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    output.Status = "UA";
                    output.Message = "Session Habis";

                    return output;
                }
                else
                {
                    var apiResponse = await response.Content.ReadAsStringAsync();
                    data = JsonConvert.DeserializeObject<GeneralOutputModel>(apiResponse);
                }
                return data;
            }
            catch (Exception ex)
            {
                output.Status = "NG";
                output.Message = ex.ToString();

                return output;
            }
        }
        public async Task<GeneralOutputModel> PostInsertGenerateNoDoc(string accessUrl, ParamInsertGenerateNoDoc pr, string token)
        {
            try
            {
                GeneralOutputModel data = new GeneralOutputModel();
                string jsonString = JsonConvert.SerializeObject(pr);  //convert to JSON
                Dictionary<string, string> values = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonString);  //convert to key/value pairs
                HttpClient httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                var response = await httpClient.PostAsync(urlApi + accessUrl, new FormUrlEncodedContent(values));
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var apiResponse = await response.Content.ReadAsStringAsync();
                    data = JsonConvert.DeserializeObject<GeneralOutputModel>(apiResponse);
                }
                return data;
            }
            catch (Exception ex)
            {
                output.Status = "NG";
                output.Message = ex.ToString();

                return output;
            }
        }

        public async Task<GeneralOutputModel> PostValidasiBarcode(string accessUrl, string token, DataOuputValidasiBarcode pr)
            {
            try
            {
                GeneralOutputModel data = new GeneralOutputModel();
                string jsonString = JsonConvert.SerializeObject(pr);  //convert to JSON
                Dictionary<string, string> values = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonString);  //convert to key/value pairs
                HttpClient httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                var response = await httpClient.PostAsync(urlApi + accessUrl, new FormUrlEncodedContent(values));
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var apiResponse = await response.Content.ReadAsStringAsync();
                    data = JsonConvert.DeserializeObject<GeneralOutputModel>(apiResponse);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    output.Status = "UA";
                    output.Message = "Session Habis";

                    return output;
                }
                return data;
            }
            catch (Exception ex)
            {
                output.Status = "NG";
                output.Message = ex.ToString();

                return output;
            }
        }

        public async Task<GeneralOutputModel> PostUploadDocumentAsync(string accessUrl, string token, ParamUploadDocumentString pr)
        {
            try
            {
                GeneralOutputModel data = new GeneralOutputModel();

                string jsonStringReceiver = JsonConvert.SerializeObject(pr.jsonDataString);
                string jsonString = JsonConvert.SerializeObject(pr);  //convert to JSON
                var objResponse1 = JsonConvert.DeserializeObject<List<ParamUploadDocument>>(jsonStringReceiver);
                var jObject = JObject.Parse(jsonString);
                jObject["jsonDataString"] = JsonConvert.SerializeObject(objResponse1);
                jsonString = JsonConvert.SerializeObject(jObject);  //convert to JSON
                Dictionary<string, string> values = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonString);  //convert to key/value pairs
                HttpClient httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                var response = await httpClient.PostAsync(urlApi + accessUrl, new FormUrlEncodedContent(values));
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    output.Status = "UA";
                    output.Message = "Session Habis";

                    return output;
                }
                else
                {
                    var apiResponse = await response.Content.ReadAsStringAsync();
                    data = JsonConvert.DeserializeObject<GeneralOutputModel>(apiResponse);
                }
                return data;
            }
            catch (Exception ex)
            {
                output.Status = "NG";
                output.Message = ex.ToString();

                return output;
            }
        }
        public async Task<GeneralOutputModel> GetDataPenerima(string accessUrl, string token, ParamGetPenerima pr)
        {
            try
            {
                GeneralOutputModel data = new GeneralOutputModel();
                string jsonString = JsonConvert.SerializeObject(pr);  //convert to JSON
                Dictionary<string, string> values = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonString);  //convert to key/value pairs
                HttpClient httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                var response = await httpClient.PostAsync(urlApi + accessUrl, new FormUrlEncodedContent(values));
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var apiResponse = await response.Content.ReadAsStringAsync();
                    data = JsonConvert.DeserializeObject<GeneralOutputModel>(apiResponse);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    output.Status = "UA";
                    output.Message = "Session Habis";

                    return output;
                }
                return data;
            }
            catch (Exception ex)
            {
                output.Status = "NG";
                output.Message = ex.ToString();

                return output;
            }
        }

        public async Task<GeneralOutputModel> InsertAttachmentLetter(string accessUrl, string token, ParamInsertAttachment pr)
        {
            try
            {
                GeneralOutputModel data = new GeneralOutputModel();
                string jsonString = JsonConvert.SerializeObject(pr);  //convert to JSON
                Dictionary<string, string> values = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonString);  //convert to key/value pairs
                HttpClient httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                var response = await httpClient.PostAsync(urlApi + accessUrl, new FormUrlEncodedContent(values));
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var apiResponse = await response.Content.ReadAsStringAsync();
                    data = JsonConvert.DeserializeObject<GeneralOutputModel>(apiResponse);
                }
                return data;
            }
            catch (Exception ex)
            {
                output.Status = "NG";
                output.Message = ex.ToString();

                return output;
            }
        }

        public async Task<GeneralOutputModel> InsertLetter(string accessUrl, string token, ParamInsertLetter pr)
        {
            try
            {
                GeneralOutputModel data = new GeneralOutputModel();

                string jsonStringReceiver = JsonConvert.SerializeObject(pr.idUserReceiver);
                string jsonStringCopy = JsonConvert.SerializeObject(pr.idUserCopy);
                string jsonString = JsonConvert.SerializeObject(pr);  //convert to JSON
                var objStringReceiver = JsonConvert.DeserializeObject<List<ParamInsertReceiver>>(jsonStringReceiver);
                var objStringCopy = JsonConvert.DeserializeObject<List<ParamInsertCopy>>(jsonStringCopy);
                var jObject = JObject.Parse(jsonString);
                jObject["idUserReceiver"] = JsonConvert.SerializeObject(objStringReceiver);
                jObject["idUserCopy"] = JsonConvert.SerializeObject(objStringCopy);
                jsonString = JsonConvert.SerializeObject(jObject);  //convert to JSON
                Dictionary<string, string> values = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonString);  //convert to key/value pairs
                HttpClient httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                var response = await httpClient.PostAsync(urlApi + accessUrl, new FormUrlEncodedContent(values));
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    output.Status = "UA";
                    output.Message = "Session Habis";

                    return output;
                }
                else
                {
                    var apiResponse = await response.Content.ReadAsStringAsync();
                    data = JsonConvert.DeserializeObject<GeneralOutputModel>(apiResponse);
                }
                return data;
            }
            catch (Exception ex)
            {
                output.Status = "NG";
                output.Message = ex.ToString();

                return output;
            }
        }

        public async Task<GeneralOutputModel> GetDocumentReport(string accessUrl, string token, ParamGetDetailReportDocument pr)
        {
            try
            {
                GeneralOutputModel data = new GeneralOutputModel();
                string jsonString = JsonConvert.SerializeObject(pr);  //convert to JSON
                Dictionary<string, string> values = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonString);  //convert to key/value pairs
                HttpClient httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                var response = await httpClient.PostAsync(urlApi + accessUrl, new FormUrlEncodedContent(values));
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    output.Status = "UA";
                    output.Message = "Session Habis";

                    return output;
                }
                else
                {
                    var apiResponse = await response.Content.ReadAsStringAsync();
                    data = JsonConvert.DeserializeObject<GeneralOutputModel>(apiResponse);
                }
                return data;
            }
            catch (Exception ex)
            {
                output.Status = "NG";
                output.Message = ex.ToString();

                return output;
            }
        }

        public async Task<GeneralOutputModel> GetDataUserAsync(string accessUrl, string token, ParamGetUsertWeb pr)
        {
            try
            {
                GeneralOutputModel data = new GeneralOutputModel();
                string jsonString = JsonConvert.SerializeObject(pr);  //convert to JSON
                Dictionary<string, string> values = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonString);  //convert to key/value pairs
                HttpClient httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                var response = await httpClient.PostAsync(urlApi + accessUrl, new FormUrlEncodedContent(values));
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var apiResponse = await response.Content.ReadAsStringAsync();
                    data = JsonConvert.DeserializeObject<GeneralOutputModel>(apiResponse);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    output.Status = "UA";
                    output.Message = "Session Habis";

                    return output;
                }
                return data;
            }
            catch (Exception ex)
            {
                output.Status = "NG";
                output.Message = ex.ToString();

                return output;
            }
        }




        public async Task<GeneralOutputModel> GetLetterDraft(string accessUrl, string token, ParamGetLetterWeb pr)
        {
            try
            {
                GeneralOutputModel data = new GeneralOutputModel();
                string jsonString = JsonConvert.SerializeObject(pr);  //convert to JSON
                Dictionary<string, string> values = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonString);  //convert to key/value pairs
                HttpClient httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                var response = await httpClient.PostAsync(urlApi + accessUrl, new FormUrlEncodedContent(values));
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    output.Status = "UA";
                    output.Message = "Session Habis";

                    return output;
                }
                else
                {
                    var apiResponse = await response.Content.ReadAsStringAsync();
                    data = JsonConvert.DeserializeObject<GeneralOutputModel>(apiResponse);
                }
                return data;
            }
            catch (Exception ex)
            {
                output.Status = "NG";
                output.Message = ex.ToString();

                return output;
            }
        }

        public async Task<GeneralOutputModel> GetInboxLetter(string accessUrl, string token, ParamGetLetterWeb pr)
        {
            try
            {
                GeneralOutputModel data = new GeneralOutputModel();
                string jsonString = JsonConvert.SerializeObject(pr);  //convert to JSON
                Dictionary<string, string> values = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonString);  //convert to key/value pairs
                HttpClient httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                var response = await httpClient.PostAsync(urlApi + accessUrl, new FormUrlEncodedContent(values));
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    output.Status = "UA";
                    output.Message = "Session Habis";

                    return output;
                }
                else
                {
                    var apiResponse = await response.Content.ReadAsStringAsync();
                    data = JsonConvert.DeserializeObject<GeneralOutputModel>(apiResponse);
                }
                return data;
            }
            catch (Exception ex)
            {
                output.Status = "NG";
                output.Message = ex.ToString();

                return output;
            }
        }

        public async Task<GeneralOutputModel> DeleteLetter(string accessUrl, string token, ParamDeleteLetter pr)

        {
            try
            {
                GeneralOutputModel data = new GeneralOutputModel();
                string jsonString = JsonConvert.SerializeObject(pr);  //convert to JSON
                Dictionary<string, string> values = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonString);  //convert to key/value pairs
                HttpClient httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                var response = await httpClient.PostAsync(urlApi + accessUrl, new FormUrlEncodedContent(values));

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var apiResponse = await response.Content.ReadAsStringAsync();
                    data = JsonConvert.DeserializeObject<GeneralOutputModel>(apiResponse);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)

                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)

                {
                    output.Status = "UA";
                    output.Message = "Session Habis";

                    return output;
                }

                else
                {
                    var apiResponse = await response.Content.ReadAsStringAsync();
                    data = JsonConvert.DeserializeObject<GeneralOutputModel>(apiResponse);
                }

                return data;
            }
            catch (Exception ex)
            {
                output.Status = "NG";
                output.Message = ex.ToString();

                return output;
            }
        }


        public async Task<GeneralOutputModel> ResetPassword(string accessUrl, string token, ParamUpdatePassword pr)

        {
            try
            {
                GeneralOutputModel data = new GeneralOutputModel();
                string jsonString = JsonConvert.SerializeObject(pr);  //convert to JSON
                Dictionary<string, string> values = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonString);  //convert to key/value pairs
                HttpClient httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                var response = await httpClient.PostAsync(urlApi + accessUrl, new FormUrlEncodedContent(values));

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var apiResponse = await response.Content.ReadAsStringAsync();
                    data = JsonConvert.DeserializeObject<GeneralOutputModel>(apiResponse);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)

                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    output.Status = "UA";
                    output.Message = "Session Habis";

                    return output;
                }

                return data;
            }
            catch (Exception ex)
            {
                output.Status = "NG";
                output.Message = ex.ToString();

                return output;
            }
        }


        public async Task<GeneralOutputModel> GetPositionAsync(string accessUrl, string token)
        {
            try
            {
                GeneralOutputModel data = new GeneralOutputModel();
                HttpClient httpClient = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Get, urlApi + accessUrl);
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                HttpResponseMessage response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    output.Status = "UA";
                    output.Message = "Session Habis";

                    return output;
                }
                else
                {
                    var apiResponse = await response.Content.ReadAsStringAsync();
                    data = JsonConvert.DeserializeObject<GeneralOutputModel>(apiResponse);
                }

                return data;
            }
            catch (Exception ex)
            {
                output.Status = "NG";
                output.Message = ex.ToString();

                return output;
            }
        }
		
		
		public async Task<GeneralOutputModel> GetDetailUserAsync(string accessUrl, string token, ParamGetDetailUser pr)
        {
            try
            {
                GeneralOutputModel data = new GeneralOutputModel();
                string jsonString = JsonConvert.SerializeObject(pr);  //convert to JSON
                Dictionary<string, string> values = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonString);  //convert to key/value pairs
                HttpClient httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                var response = await httpClient.PostAsync(urlApi + accessUrl, new FormUrlEncodedContent(values));
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var apiResponse = await response.Content.ReadAsStringAsync();
                    data = JsonConvert.DeserializeObject<GeneralOutputModel>(apiResponse);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    output.Status = "UA";
                    output.Message = "Session Habis";

                    return output;
                }
                return data;
            }
            catch (Exception ex)
            {
                output.Status = "NG";
                output.Message = ex.ToString();

                return output;
            }
        }
        public async Task<GeneralOutputModel> GetDetailLetter(string accessUrl, string token, ParamGetDetailLetter pr)
        {
            try
            {
                GeneralOutputModel data = new GeneralOutputModel();
                string jsonString = JsonConvert.SerializeObject(pr);  //convert to JSON
                Dictionary<string, string> values = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonString);  //convert to key/value pairs
                HttpClient httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                var response = await httpClient.PostAsync(urlApi + accessUrl, new FormUrlEncodedContent(values));
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    output.Status = "UA";
                    output.Message = "Session Habis";

                    return output;
                }
                else
                {
                    var apiResponse = await response.Content.ReadAsStringAsync();
                    data = JsonConvert.DeserializeObject<GeneralOutputModel>(apiResponse);
                }
                return data;
            }
            catch (Exception ex)
            {
                output.Status = "NG";
                output.Message = ex.ToString();

                return output;
            }
        }

        public async Task<GeneralOutputModel> DeleteAttachment(string accessUrl, string token, ParamDeleteAttachment pr)
        {
            try
            {
                GeneralOutputModel data = new GeneralOutputModel();
                string jsonString = JsonConvert.SerializeObject(pr);  //convert to JSON
                Dictionary<string, string> values = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonString);  //convert to key/value pairs
                HttpClient httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                var response = await httpClient.PostAsync(urlApi + accessUrl, new FormUrlEncodedContent(values));
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    output.Status = "UA";
                    output.Message = "Session Habis";

                    return output;
                }
                else
                {
                    var apiResponse = await response.Content.ReadAsStringAsync();
                    data = JsonConvert.DeserializeObject<GeneralOutputModel>(apiResponse);
                }
                return data;
            }
            catch (Exception ex)
            {
                output.Status = "NG";
                output.Message = ex.ToString();

                return output;
            }
        }

        public async Task<GeneralOutputModel> ReceiveCheckedDoc(string accessUrl, string token, ParamReceiveCheckedDoc pr)
        {
            try
            {
                GeneralOutputModel data = new GeneralOutputModel();
                string jsonString = JsonConvert.SerializeObject(pr);  //convert to JSON
                Dictionary<string, string> values = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonString);  //convert to key/value pairs
                HttpClient httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                var response = await httpClient.PostAsync(urlApi + accessUrl, new FormUrlEncodedContent(values));
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    output.Status = "UA";
                    output.Message = "Session Habis";

                    return output;
                }
                else
                {
                    var apiResponse = await response.Content.ReadAsStringAsync();
                    data = JsonConvert.DeserializeObject<GeneralOutputModel>(apiResponse);
                }
                return data;
            }
            catch (Exception ex)
            {
                output.Status = "NG";
                output.Message = ex.ToString();

                return output;
            }
        }
        public async Task<GeneralOutputModel> PutUpdateUserAsync(string accessUrl, string token, ParamUpdateUser pr)
        {
            try
            {
                GeneralOutputModel data = new GeneralOutputModel();
                string jsonString = JsonConvert.SerializeObject(pr);  //convert to JSON
                Dictionary<string, string> values = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonString);  //convert to key/value pairs
                HttpClient httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                var response = await httpClient.PostAsync(urlApi + accessUrl, new FormUrlEncodedContent(values));
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var apiResponse = await response.Content.ReadAsStringAsync();
                    data = JsonConvert.DeserializeObject<GeneralOutputModel>(apiResponse);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    output.Status = "UA";
                    output.Message = "Session Habis";

                    return output;
                }
                return data;
            }
            catch (Exception ex)
            {
                output.Status = "NG";
                output.Message = ex.ToString();

                return output;
            }
        }
        public async Task<GeneralOutputModel> GetDetailDocumentReceiverAsync(string accessUrl, string token, ParamGetDetailDocumentReceiver pr)
        {
            try
            {
                GeneralOutputModel data = new GeneralOutputModel();
                string jsonString = JsonConvert.SerializeObject(pr);  //convert to JSON
                Dictionary<string, string> values = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonString);  //convert to key/value pairs
                HttpClient httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                var response = await httpClient.PostAsync(urlApi + accessUrl, new FormUrlEncodedContent(values));
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var apiResponse = await response.Content.ReadAsStringAsync();
                    data = JsonConvert.DeserializeObject<GeneralOutputModel>(apiResponse);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    output.Status = "UA";
                    output.Message = "Session Habis";

                    return output;
                }
                return data;
            }
            catch (Exception ex)
            {
                output.Status = "NG";
                output.Message = ex.ToString();

                return output;
            }
        }

        public async Task<GeneralOutputModel> GetUserByUnitAsync(string accessUrl, string token, ParamGetUserByUnit pr)
        {
            try
            {
                GeneralOutputModel data = new GeneralOutputModel();
                string jsonString = JsonConvert.SerializeObject(pr);  //convert to JSON
                Dictionary<string, string> values = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonString);  //convert to key/value pairs
                HttpClient httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                var response = await httpClient.PostAsync(urlApi + accessUrl, new FormUrlEncodedContent(values));

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var apiResponse = await response.Content.ReadAsStringAsync();
                    data = JsonConvert.DeserializeObject<GeneralOutputModel>(apiResponse);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)

                    if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    {
                        output.Status = "UA";
                        output.Message = "Session Habis";

                        return output;
                    }

                return data;
            }
            catch (Exception ex)
            {
                output.Status = "NG";
                output.Message = ex.ToString();

                return output;
            }
        }

        public async Task<GeneralOutputModel> UpdateAdminDivisi(string accessUrl, string token, ParamUpdateAdminDivisi pr)
        {
            try
            {
                GeneralOutputModel data = new GeneralOutputModel();
                string jsonString = JsonConvert.SerializeObject(pr);  //convert to JSON
                Dictionary<string, string> values = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonString);  //convert to key/value pairs
                HttpClient httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                var response = await httpClient.PostAsync(urlApi + accessUrl, new FormUrlEncodedContent(values));

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var apiResponse = await response.Content.ReadAsStringAsync();
                    data = JsonConvert.DeserializeObject<GeneralOutputModel>(apiResponse);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)

                    if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    {
                        output.Status = "UA";
                        output.Message = "Session Habis";

                        return output;
                    }

                return data;
            }
            catch (Exception ex)
            {
                output.Status = "NG";
                output.Message = ex.ToString();

                return output;
            }
        }

        public async Task<GeneralOutputModel> PreviewNoLetter(string accessUrl, string token, ParamPreviewNoLetter pr)
        {
            try
            {
                GeneralOutputModel data = new GeneralOutputModel();
                string jsonString = JsonConvert.SerializeObject(pr);  //convert to JSON
                Dictionary<string, string> values = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonString);  //convert to key/value pairs
                HttpClient httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                var response = await httpClient.PostAsync(urlApi + accessUrl, new FormUrlEncodedContent(values));

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var apiResponse = await response.Content.ReadAsStringAsync();
                    data = JsonConvert.DeserializeObject<GeneralOutputModel>(apiResponse);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)

                    if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    {
                        output.Status = "UA";
                        output.Message = "Session Habis";

                        return output;
                    }

                return data;
            }
            catch (Exception ex)
            {
                output.Status = "NG";
                output.Message = ex.ToString();

                return output;
            }
        }


        public async Task<GeneralOutputModel> GetDocumentSearchOutgongMail(string accessUrl, string token, ParamGetDetailSearchoutgoingDocument pr)
        {
            try
            {
                GeneralOutputModel data = new GeneralOutputModel();
                string jsonString = JsonConvert.SerializeObject(pr);  //convert to JSON
                Dictionary<string, string> values = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonString);  //convert to key/value pairs
                HttpClient httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                var response = await httpClient.PostAsync(urlApi + accessUrl, new FormUrlEncodedContent(values));
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    output.Status = "UA";
                    output.Message = "Session Habis";

                    return output;
                }
                else
                {
                    var apiResponse = await response.Content.ReadAsStringAsync();
                    data = JsonConvert.DeserializeObject<GeneralOutputModel>(apiResponse);
                }
                return data;
            }
            catch (Exception ex)
            {
                output.Status = "NG";
                output.Message = ex.ToString();

                return output;
            }
        }

        public async Task<GeneralOutputModel> MailForgotPassword(string accessUrl, ParamForgotPassword pr)
        {
            try
            {
                GeneralOutputModel data = new GeneralOutputModel();
                string jsonString = JsonConvert.SerializeObject(pr);  //convert to JSON
                Dictionary<string, string> values = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonString);  //convert to key/value pairs
                HttpClient httpClient = new HttpClient();
                //httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                var response = await httpClient.PostAsync(urlApi + accessUrl, new FormUrlEncodedContent(values));

                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    output.Status = "UA";
                    output.Message = "Session Habis";

                    return output;
                }
                else
                {
                    var apiResponse = await response.Content.ReadAsStringAsync();
                    data = JsonConvert.DeserializeObject<GeneralOutputModel>(apiResponse);
                }

                return data;
            }
            catch (Exception ex)
            {
                output.Status = "NG";
                output.Message = ex.ToString();

                return output;
            }
        }

        public async Task<GeneralOutputModel> GetDashboardContent(string accessUrl, string token)
        {
            try
            {
                GeneralOutputModel data = new GeneralOutputModel();
                HttpClient httpClient = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Get, urlApi + accessUrl);
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                HttpResponseMessage response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    output.Status = "UA";
                    output.Message = "Session Habis";

                    return output;
                }
                else
                {
                    var apiResponse = await response.Content.ReadAsStringAsync();
                    data = JsonConvert.DeserializeObject<GeneralOutputModel>(apiResponse);
                }
                return data;
            }
            catch (Exception ex)
            {
                output.Status = "NG";
                output.Message = ex.ToString();

                return output;
            }
        }

        public async Task<GeneralOutputModel> PostInsertDisposition(string accessUrl, string token, ParamInsertLetterDisposition pr)
        {
            try
            {
                GeneralOutputModel data = new GeneralOutputModel();

                string jsonStringReceiver = JsonConvert.SerializeObject(pr.listUser);
                string jsonString = JsonConvert.SerializeObject(pr);  //convert to JSON
                var objResponse1 = JsonConvert.DeserializeObject<List<ListInsertUserDisposition>>(jsonStringReceiver);
                var jObject = JObject.Parse(jsonString);
                jObject["listUser"] = JsonConvert.SerializeObject(objResponse1);
                jsonString = JsonConvert.SerializeObject(jObject);  //convert to JSON
                Dictionary<string, string> values = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonString);  //convert to key/value pairs
                HttpClient httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                var response = await httpClient.PostAsync(urlApi + accessUrl, new FormUrlEncodedContent(values));
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    output.Status = "UA";
                    output.Message = "Session Habis";

                    return output;
                }
                else
                {
                    var apiResponse = await response.Content.ReadAsStringAsync();
                    data = JsonConvert.DeserializeObject<GeneralOutputModel>(apiResponse);
                }
                return data;
            }
            catch (Exception ex)
            {
                output.Status = "NG";
                output.Message = ex.ToString();

                return output;
            }
        }

        public async Task<GeneralOutputModel> PostForgotPassword(string accessUrl, ParamUpdateForgotPassword pr)
        {
            try
            {
                GeneralOutputModel data = new GeneralOutputModel();
                string jsonString = JsonConvert.SerializeObject(pr);  //convert to JSON
                Dictionary<string, string> values = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonString);  //convert to key/value pairs
                HttpClient httpClient = new HttpClient();
                var response = await httpClient.PostAsync(urlApi + accessUrl, new FormUrlEncodedContent(values));
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var apiResponse = await response.Content.ReadAsStringAsync();
                    data = JsonConvert.DeserializeObject<GeneralOutputModel>(apiResponse);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    output.Status = "UA";
                    output.Message = "Session Habis";

                    return output;
                }
                return data;
            }
            catch (Exception ex)
            {
                output.Status = "NG";
                output.Message = ex.ToString();

                return output;
            }
        }


        public async Task<GeneralOutputModel> UpdatePasswordLogin(string accessUrl, ParamUpdatePasswordLogin pr)
        {
            try
            {
                GeneralOutputModel data = new GeneralOutputModel();
                string jsonString = JsonConvert.SerializeObject(pr);  //convert to JSON
                Dictionary<string, string> values = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonString);  //convert to key/value pairs
                HttpClient httpClient = new HttpClient();
                var response = await httpClient.PostAsync(urlApi + accessUrl, new FormUrlEncodedContent(values));
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var apiResponse = await response.Content.ReadAsStringAsync();
                    data = JsonConvert.DeserializeObject<GeneralOutputModel>(apiResponse);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    output.Status = "UA";
                    output.Message = "Session Habis";

                    return output;
                }
                return data;
            }
            catch (Exception ex)
            {
                output.Status = "NG";
                output.Message = ex.ToString();

                return output;
            }
        }


        public async Task<GeneralOutputModel> GetUserPGAasync(string accessUrl, string token)
        {
            try
            {
                GeneralOutputModel data = new GeneralOutputModel();
                HttpClient httpClient = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Get, urlApi + accessUrl);
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                HttpResponseMessage response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var apiResponse = await response.Content.ReadAsStringAsync();
                    data = JsonConvert.DeserializeObject<GeneralOutputModel>(apiResponse);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    output.Status = "UA";
                    output.Message = "Session Habis";

                    return output;
                }
                return data;
            }
            catch (Exception ex)
            {
                output.Status = "NG";
                output.Message = ex.ToString();

                return output;
            }
        }


        public async Task<GeneralOutputModel> GetUserDirekturasync(string accessUrl, string token)
        {
            try
            {
                GeneralOutputModel data = new GeneralOutputModel();
                HttpClient httpClient = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Get, urlApi + accessUrl);
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                HttpResponseMessage response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var apiResponse = await response.Content.ReadAsStringAsync();
                    data = JsonConvert.DeserializeObject<GeneralOutputModel>(apiResponse);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    output.Status = "UA";
                    output.Message = "Session Habis";

                    return output;
                }
                return data;
            }
            catch (Exception ex)
            {
                output.Status = "NG";
                output.Message = ex.ToString();

                return output;
            }
        }


        public async Task<GeneralOutputModel> GetUserSeketarisasync(string accessUrl, string token)
        {
            try
            {
                GeneralOutputModel data = new GeneralOutputModel();
                HttpClient httpClient = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Get, urlApi + accessUrl);
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                HttpResponseMessage response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var apiResponse = await response.Content.ReadAsStringAsync();
                    data = JsonConvert.DeserializeObject<GeneralOutputModel>(apiResponse);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    output.Status = "UA";
                    output.Message = "Session Habis";

                    return output;
                }
                return data;
            }
            catch (Exception ex)
            {
                output.Status = "NG";
                output.Message = ex.ToString();

                return output;
            }
        }


        public async Task<GeneralOutputModel> GetDataUserSekdirAsync(string accessUrl, string token)
        {

            try
            {
                GeneralOutputModel data = new GeneralOutputModel();
                HttpClient httpClient = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Get, urlApi + accessUrl);
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                HttpResponseMessage response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var apiResponse = await response.Content.ReadAsStringAsync();
                    data = JsonConvert.DeserializeObject<GeneralOutputModel>(apiResponse);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    output.Status = "UA";
                    output.Message = "Session Habis";

                    return output;
                }
                return data;
            }
            catch (Exception ex)
            {
                output.Status = "NG";
                output.Message = ex.ToString();

                return output;
            }
            //try
            //{
            //    GeneralOutputModel data = new GeneralOutputModel();
            //    string jsonString = JsonConvert.SerializeObject(pr);  //convert to JSON
            //    Dictionary<string, string> values = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonString);  //convert to key/value pairs
            //    HttpClient httpClient = new HttpClient();
            //    httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
            //    var response = await httpClient.PostAsync(urlApi + accessUrl, new FormUrlEncodedContent(values));
            //    if (response.StatusCode == System.Net.HttpStatusCode.OK)
            //    {
            //        var apiResponse = await response.Content.ReadAsStringAsync();
            //        data = JsonConvert.DeserializeObject<GeneralOutputModel>(apiResponse);
            //    }
            //    else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            //    {
            //        output.Status = "UA";
            //        output.Message = "Session Habis";

            //        return output;
            //    }
            //    return data;
            //}
            //catch (Exception ex)
            //{
            //    output.Status = "NG";
            //    output.Message = ex.ToString();

            //    return output;
            //}
        }


        public async Task<GeneralOutputModel> UpdateDataSettingSeketaris(string accessUrl, string token, ParamUpdateUserSekdirWeb pr)
        {
            try
            {
                GeneralOutputModel data = new GeneralOutputModel();
                string jsonString = JsonConvert.SerializeObject(pr);  //convert to JSON
                Dictionary<string, string> values = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonString);  //convert to key/value pairs
                HttpClient httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                var response = await httpClient.PostAsync(urlApi + accessUrl, new FormUrlEncodedContent(values));

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var apiResponse = await response.Content.ReadAsStringAsync();
                    data = JsonConvert.DeserializeObject<GeneralOutputModel>(apiResponse);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)

                    if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    {
                        output.Status = "UA";
                        output.Message = "Session Habis";

                        return output;
                    }

                return data;
            }
            catch (Exception ex)
            {
                output.Status = "NG";
                output.Message = ex.ToString();

                return output;
            }
        }

        public async Task<GeneralOutputModel> GetUserByDirekturAsync(string accessUrl, string token, ParamUpdateUserSekdirWeb pr)
        {
            try
            {
                GeneralOutputModel data = new GeneralOutputModel();
                string jsonString = JsonConvert.SerializeObject(pr);  //convert to JSON
                Dictionary<string, string> values = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonString);  //convert to key/value pairs
                HttpClient httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                var response = await httpClient.PostAsync(urlApi + accessUrl, new FormUrlEncodedContent(values));

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var apiResponse = await response.Content.ReadAsStringAsync();
                    data = JsonConvert.DeserializeObject<GeneralOutputModel>(apiResponse);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)

                    if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    {
                        output.Status = "UA";
                        output.Message = "Session Habis";

                        return output;
                    }

                return data;
            }
            catch (Exception ex)
            {
                output.Status = "NG";
                output.Message = ex.ToString();

                return output;
            }
        }
   

        public async Task<GeneralOutputModel> PutUpdateUserSekDirAsync(string accessUrl, string token, ParamUpdateUserSekdirWeb pr)
        {
            try
            {
                GeneralOutputModel data = new GeneralOutputModel();
                string jsonString = JsonConvert.SerializeObject(pr);  //convert to JSON
                Dictionary<string, string> values = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonString);  //convert to key/value pairs
                HttpClient httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                var response = await httpClient.PostAsync(urlApi + accessUrl, new FormUrlEncodedContent(values));
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var apiResponse = await response.Content.ReadAsStringAsync();
                    data = JsonConvert.DeserializeObject<GeneralOutputModel>(apiResponse);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    output.Status = "UA";
                    output.Message = "Session Habis";

                    return output;
                }
                return data;
            }
            catch (Exception ex)
            {
                output.Status = "NG";
                output.Message = ex.ToString();

                return output;
            }
        }


        public async Task<GeneralOutputModel> PutDeleteUserSekdirAsync(string accessUrl, string token, ParamGetDetailUserSekdir pr)
        {
            try
            {
                GeneralOutputModel data = new GeneralOutputModel();
                string jsonString = JsonConvert.SerializeObject(pr);  //convert to JSON
                Dictionary<string, string> values = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonString);  //convert to key/value pairs
                HttpClient httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                var response = await httpClient.PostAsync(urlApi + accessUrl, new FormUrlEncodedContent(values));
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var apiResponse = await response.Content.ReadAsStringAsync();
                    data = JsonConvert.DeserializeObject<GeneralOutputModel>(apiResponse);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    output.Status = "UA";
                    output.Message = "Session Habis";

                    return output;
                }
                return data;
            }
            catch (Exception ex)
            {
                output.Status = "NG";
                output.Message = ex.ToString();

                return output;
            }
        }


        public async Task<GeneralOutputModel> GetUserSuperasync(string accessUrl, string token)
        {
            try
            {
                GeneralOutputModel data = new GeneralOutputModel();
                HttpClient httpClient = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Get, urlApi + accessUrl);
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                HttpResponseMessage response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var apiResponse = await response.Content.ReadAsStringAsync();
                    data = JsonConvert.DeserializeObject<GeneralOutputModel>(apiResponse);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    output.Status = "UA";
                    output.Message = "Session Habis";

                    return output;
                }
                return data;
            }
            catch (Exception ex)
            {
                output.Status = "NG";
                output.Message = ex.ToString();

                return output;
            }
        }

        public async Task<GeneralOutputModel> GetDataSuperUserAsync(string accessUrl, string token, ParamGetUsertWeb pr)
        {
            try
            {
                GeneralOutputModel data = new GeneralOutputModel();
                string jsonString = JsonConvert.SerializeObject(pr);  //convert to JSON
                Dictionary<string, string> values = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonString);  //convert to key/value pairs
                HttpClient httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                var response = await httpClient.PostAsync(urlApi + accessUrl, new FormUrlEncodedContent(values));
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var apiResponse = await response.Content.ReadAsStringAsync();
                    data = JsonConvert.DeserializeObject<GeneralOutputModel>(apiResponse);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    output.Status = "UA";
                    output.Message = "Session Habis";

                    return output;
                }
                return data;
            }
            catch (Exception ex)
            {
                output.Status = "NG";
                output.Message = ex.ToString();

                return output;
            }
        }

        public async Task<GeneralOutputModel> PutUpdateSuperUserAsync(string accessUrl, string token, ParamGetDetailUser pr)
        {
            try
            {
                GeneralOutputModel data = new GeneralOutputModel();
                string jsonString = JsonConvert.SerializeObject(pr);  //convert to JSON
                Dictionary<string, string> values = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonString);  //convert to key/value pairs
                HttpClient httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                var response = await httpClient.PostAsync(urlApi + accessUrl, new FormUrlEncodedContent(values));
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var apiResponse = await response.Content.ReadAsStringAsync();
                    data = JsonConvert.DeserializeObject<GeneralOutputModel>(apiResponse);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    output.Status = "UA";
                    output.Message = "Session Habis";

                    return output;
                }
                return data;
            }
            catch (Exception ex)
            {
                output.Status = "NG";
                output.Message = ex.ToString();

                return output;
            }
        }

        public async Task<GeneralOutputModel> PutDeleteSuperUserAsync(string accessUrl, string token, ParamGetDetailUser pr)
        {
            try
            {
                GeneralOutputModel data = new GeneralOutputModel();
                string jsonString = JsonConvert.SerializeObject(pr);  //convert to JSON
                Dictionary<string, string> values = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonString);  //convert to key/value pairs
                HttpClient httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                var response = await httpClient.PostAsync(urlApi + accessUrl, new FormUrlEncodedContent(values));
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var apiResponse = await response.Content.ReadAsStringAsync();
                    data = JsonConvert.DeserializeObject<GeneralOutputModel>(apiResponse);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    output.Status = "UA";
                    output.Message = "Session Habis";

                    return output;
                }
                return data;
            }
            catch (Exception ex)
            {
                output.Status = "NG";
                output.Message = ex.ToString();

                return output;
            }
        }


      

        public async Task<GeneralOutputModel> GetUserAdminHctsync(string accessUrl, string token)
        {
            try
            {
                GeneralOutputModel data = new GeneralOutputModel();
                HttpClient httpClient = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Get, urlApi + accessUrl);
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                HttpResponseMessage response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var apiResponse = await response.Content.ReadAsStringAsync();
                    data = JsonConvert.DeserializeObject<GeneralOutputModel>(apiResponse);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    output.Status = "UA";
                    output.Message = "Session Habis";

                    return output;
                }
                return data;
            }
            catch (Exception ex)
            {
                output.Status = "NG";
                output.Message = ex.ToString();

                return output;
            }
        }

        public async Task<GeneralOutputModel> GetDataAdminHctAsync(string accessUrl, string token, ParamGetUsertWeb pr)
        {
            try
            {
                GeneralOutputModel data = new GeneralOutputModel();
                string jsonString = JsonConvert.SerializeObject(pr);  //convert to JSON
                Dictionary<string, string> values = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonString);  //convert to key/value pairs
                HttpClient httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                var response = await httpClient.PostAsync(urlApi + accessUrl, new FormUrlEncodedContent(values));
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var apiResponse = await response.Content.ReadAsStringAsync();
                    data = JsonConvert.DeserializeObject<GeneralOutputModel>(apiResponse);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    output.Status = "UA";
                    output.Message = "Session Habis";

                    return output;
                }
                return data;
            }
            catch (Exception ex)
            {
                output.Status = "NG";
                output.Message = ex.ToString();

                return output;
            }
        }

        public async Task<GeneralOutputModel> PutUpdateAdminHctAsync(string accessUrl, string token, ParamGetDetailUser pr)
        {
            try
            {
                GeneralOutputModel data = new GeneralOutputModel();
                string jsonString = JsonConvert.SerializeObject(pr);  //convert to JSON
                Dictionary<string, string> values = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonString);  //convert to key/value pairs
                HttpClient httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                var response = await httpClient.PostAsync(urlApi + accessUrl, new FormUrlEncodedContent(values));
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var apiResponse = await response.Content.ReadAsStringAsync();
                    data = JsonConvert.DeserializeObject<GeneralOutputModel>(apiResponse);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    output.Status = "UA";
                    output.Message = "Session Habis";

                    return output;
                }
                return data;
            }
            catch (Exception ex)
            {
                output.Status = "NG";
                output.Message = ex.ToString();

                return output;
            }
        }

        public async Task<GeneralOutputModel> PutDeleteAdminHctAsync(string accessUrl, string token, ParamGetDetailUser pr)
        {
            try
            {
                GeneralOutputModel data = new GeneralOutputModel();
                string jsonString = JsonConvert.SerializeObject(pr);  //convert to JSON
                Dictionary<string, string> values = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonString);  //convert to key/value pairs
                HttpClient httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                var response = await httpClient.PostAsync(urlApi + accessUrl, new FormUrlEncodedContent(values));
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var apiResponse = await response.Content.ReadAsStringAsync();
                    data = JsonConvert.DeserializeObject<GeneralOutputModel>(apiResponse);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    output.Status = "UA";
                    output.Message = "Session Habis";

                    return output;
                }
                return data;
            }
            catch (Exception ex)
            {
                output.Status = "NG";
                output.Message = ex.ToString();

                return output;
            }
        }

       
        public async Task<GeneralOutputModel> GetEmployeesync(string accessUrl, string token)
        {
            try
            {
                GeneralOutputModel data = new GeneralOutputModel();
                HttpClient httpClient = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Get, urlApi + accessUrl);
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                HttpResponseMessage response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var apiResponse = await response.Content.ReadAsStringAsync();
                    data = JsonConvert.DeserializeObject<GeneralOutputModel>(apiResponse);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    output.Status = "UA";
                    output.Message = "Session Habis";

                    return output;
                }
                return data;
            }
            catch (Exception ex)
            {
                output.Status = "NG";
                output.Message = ex.ToString();

                return output;
            }
        }

        public async Task<GeneralOutputModel> GetAllDataposition(string accessUrl, string token)
        {
            try
            {
                GeneralOutputModel data = new GeneralOutputModel();
                HttpClient httpClient = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Get, urlApi + accessUrl);
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                HttpResponseMessage response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var apiResponse = await response.Content.ReadAsStringAsync();
                    data = JsonConvert.DeserializeObject<GeneralOutputModel>(apiResponse);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    output.Status = "UA";
                    output.Message = "Session Habis";

                    return output;
                }
                return data;
            }
            catch (Exception ex)
            {
                output.Status = "NG";
                output.Message = ex.ToString();

                return output;
            }
        }

        public async Task<GeneralOutputModel> SearchEmployeeGetById(string accessUrl, string token, ParamGetDetailUser pr)
        {
            try
            {
                GeneralOutputModel data = new GeneralOutputModel();
                string jsonString = JsonConvert.SerializeObject(pr);  //convert to JSON
                Dictionary<string, string> values = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonString);  //convert to key/value pairs
                HttpClient httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                var response = await httpClient.PostAsync(urlApi + accessUrl, new FormUrlEncodedContent(values));
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var apiResponse = await response.Content.ReadAsStringAsync();
                    data = JsonConvert.DeserializeObject<GeneralOutputModel>(apiResponse);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    output.Status = "UA";
                    output.Message = "Session Habis";

                    return output;
                }
                return data;
            }
            catch (Exception ex)
            {
                output.Status = "NG";
                output.Message = ex.ToString();

                return output;
            }
        }


        public async Task<GeneralOutputModel> PutUpdatePositionEmpAsync(string accessUrl, string token, ParamUpdateUser pr)
        {
            try
            {
                GeneralOutputModel data = new GeneralOutputModel();
                string jsonString = JsonConvert.SerializeObject(pr);  //convert to JSON
                Dictionary<string, string> values = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonString);  //convert to key/value pairs
                HttpClient httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                var response = await httpClient.PostAsync(urlApi + accessUrl, new FormUrlEncodedContent(values));
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var apiResponse = await response.Content.ReadAsStringAsync();
                    data = JsonConvert.DeserializeObject<GeneralOutputModel>(apiResponse);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    output.Status = "UA";
                    output.Message = "Session Habis";

                    return output;
                }
                return data;
            }
            catch (Exception ex)
            {
                output.Status = "NG";
                output.Message = ex.ToString();

                return output;
            }
        }


      
        public async Task<GeneralOutputModel> GetSuratKeluarNon(string accessUrl, string token)
        {
            try
            {
                GeneralOutputModel data = new GeneralOutputModel();
                HttpClient httpClient = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Get, urlApi + accessUrl);
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                HttpResponseMessage response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var apiResponse = await response.Content.ReadAsStringAsync();
                    data = JsonConvert.DeserializeObject<GeneralOutputModel>(apiResponse);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    output.Status = "UA";
                    output.Message = "Session Habis";

                    return output;
                }
                return data;
            }
            catch (Exception ex)
            {
                output.Status = "NG";
                output.Message = ex.ToString();

                return output;
            }
        }

        public async Task<GeneralOutputModel> PostInsertSendNonEofficeAsync(string accessUrl, ParamInsertNonEoffice pr, string token)
        {
            try
            {
                GeneralOutputModel data = new GeneralOutputModel();
                string jsonString = JsonConvert.SerializeObject(pr);  //convert to JSON
                Dictionary<string, string> values = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonString);  //convert to key/value pairs
                HttpClient httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                var response = await httpClient.PostAsync(urlApi + accessUrl, new FormUrlEncodedContent(values));
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var apiResponse = await response.Content.ReadAsStringAsync();
                    data = JsonConvert.DeserializeObject<GeneralOutputModel>(apiResponse);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    output.Status = "UA";
                    output.Message = "Session Habis";

                    return output;
                }
                return data;
            }
            catch (Exception ex)
            {
                output.Status = "NG";
                output.Message = ex.ToString();

                return output;
            }
        }

        public async Task<GeneralOutputModel> PostUpdateNonEofficeAsync(string accessUrl, UpdateNonEoffice pr, string token)
        {
            try
            {
                GeneralOutputModel data = new GeneralOutputModel();
                string jsonString = JsonConvert.SerializeObject(pr);  //convert to JSON
                Dictionary<string, string> values = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonString);  //convert to key/value pairs
                HttpClient httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                var response = await httpClient.PostAsync(urlApi + accessUrl, new FormUrlEncodedContent(values));
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var apiResponse = await response.Content.ReadAsStringAsync();
                    data = JsonConvert.DeserializeObject<GeneralOutputModel>(apiResponse);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    output.Status = "UA";
                    output.Message = "Session Habis";

                    return output;
                }
                return data;
            }
            catch (Exception ex)
            {
                output.Status = "NG";
                output.Message = ex.ToString();

                return output;
            }
        }

        public async Task<GeneralOutputModel> PostUploadLetterNonEoffcieAsync(string accessUrl, string token, ParamUploadLetterNonOfficeString pr)
        {
            try
            {
                GeneralOutputModel data = new GeneralOutputModel();

                string jsonStringReceiver = JsonConvert.SerializeObject(pr.jsonDataString);
                string jsonString = JsonConvert.SerializeObject(pr);  //convert to JSON
                var objResponse1 = JsonConvert.DeserializeObject<List<ParamUploadputletterNonEoffice>>(jsonStringReceiver);
                var jObject = JObject.Parse(jsonString);
                jObject["jsonDataString"] = JsonConvert.SerializeObject(objResponse1);
                jsonString = JsonConvert.SerializeObject(jObject);  //convert to JSON
                Dictionary<string, string> values = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonString);  //convert to key/value pairs
                HttpClient httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                var response = await httpClient.PostAsync(urlApi + accessUrl, new FormUrlEncodedContent(values));
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    output.Status = "UA";
                    output.Message = "Session Habis";

                    return output;
                }
                else
                {
                    var apiResponse = await response.Content.ReadAsStringAsync();
                    data = JsonConvert.DeserializeObject<GeneralOutputModel>(apiResponse);
                }
                return data;
            }
            catch (Exception ex)
            {
                output.Status = "NG";
                output.Message = ex.ToString();

                return output;
            }
        }

        public async Task<GeneralOutputModel> GetStringmapNonEofficeAsync(string accessUrl, ParamGetStringmapNonEoffice pr, string token)
        {
            try
            {
                GeneralOutputModel data = new GeneralOutputModel();
                string jsonString = JsonConvert.SerializeObject(pr);  //convert to JSON
                Dictionary<string, string> values = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonString);  //convert to key/value pairs
                HttpClient httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                var response = await httpClient.PostAsync(urlApi + accessUrl, new FormUrlEncodedContent(values));
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var apiResponse = await response.Content.ReadAsStringAsync();
                    data = JsonConvert.DeserializeObject<GeneralOutputModel>(apiResponse);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    output.Status = "UA";
                    output.Message = "Session Habis";

                    return output;
                }
                return data;
            }
            catch (Exception ex)
            {
                output.Status = "NG";
                output.Message = ex.ToString();

                return output;
            }
        }


        public async Task<GeneralOutputModel> GetDataEksepdisiNonEoffice(string accessUrl, string token, ParamGetEkspedisi pr)
        {
            try
            {
                GeneralOutputModel data = new GeneralOutputModel();
                string jsonString = JsonConvert.SerializeObject(pr);  //convert to JSON
                Dictionary<string, string> values = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonString);  //convert to key/value pairs
                HttpClient httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                var response = await httpClient.PostAsync(urlApi + accessUrl, new FormUrlEncodedContent(values));
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var apiResponse = await response.Content.ReadAsStringAsync();
                    data = JsonConvert.DeserializeObject<GeneralOutputModel>(apiResponse);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    output.Status = "UA";
                    output.Message = "Session Habis";

                    return output;
                }
                return data;
            }
            catch (Exception ex)
            {
                output.Status = "NG";
                output.Message = ex.ToString();

                return output;
            }
        }


        public async Task<GeneralOutputModel> PostInsertGenerateNoDocNonEoffice(string accessUrl, ParamInsertGenerateNoDocNonEoffice pr, string token)
        {
            try
            {
                GeneralOutputModel data = new GeneralOutputModel();
                string jsonString = JsonConvert.SerializeObject(pr);  //convert to JSON
                Dictionary<string, string> values = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonString);  //convert to key/value pairs
                HttpClient httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                var response = await httpClient.PostAsync(urlApi + accessUrl, new FormUrlEncodedContent(values));
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var apiResponse = await response.Content.ReadAsStringAsync();
                    data = JsonConvert.DeserializeObject<GeneralOutputModel>(apiResponse);
                }
                return data;
            }
            catch (Exception ex)
            {
                output.Status = "NG";
                output.Message = ex.ToString();

                return output;
            }
        }

        public async Task<GeneralOutputModel> GeDataByIdNonEoffice(string accessUrl, string token, getByIdNonEoffice pr)
        {
            try
            {
                GeneralOutputModel data = new GeneralOutputModel();
                string jsonString = JsonConvert.SerializeObject(pr);  //convert to JSON
                Dictionary<string, string> values = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonString);  //convert to key/value pairs
                HttpClient httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                var response = await httpClient.PostAsync(urlApi + accessUrl, new FormUrlEncodedContent(values));
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var apiResponse = await response.Content.ReadAsStringAsync();
                    data = JsonConvert.DeserializeObject<GeneralOutputModel>(apiResponse);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    output.Status = "UA";
                    output.Message = "Session Habis";

                    return output;
                }
                return data;
            }
            catch (Exception ex)
            {
                output.Status = "NG";
                output.Message = ex.ToString();

                return output;
            }
        }


        public async Task<GeneralOutputModel> InsertLevelEmpl(string accessUrl, string token, ParamUpdateLevelemp pr)
        {
            try
            {
                GeneralOutputModel data = new GeneralOutputModel();
                string jsonString = JsonConvert.SerializeObject(pr);  //convert to JSON
                Dictionary<string, string> values = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonString);  //convert to key/value pairs
                HttpClient httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                var response = await httpClient.PostAsync(urlApi + accessUrl, new FormUrlEncodedContent(values));

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var apiResponse = await response.Content.ReadAsStringAsync();
                    data = JsonConvert.DeserializeObject<GeneralOutputModel>(apiResponse);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)

                    if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    {
                        output.Status = "UA";
                        output.Message = "Session Habis";

                        return output;
                    }

                return data;
            }
            catch (Exception ex)
            {
                output.Status = "NG";
                output.Message = ex.ToString();

                return output;
            }
        }


        public async Task<GeneralOutputModel> GetUnitHctAsync(string accessUrl, string token)
        {
            try
            {
                GeneralOutputModel data = new GeneralOutputModel();
                HttpClient httpClient = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Get, urlApi + accessUrl);
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                HttpResponseMessage response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var apiResponse = await response.Content.ReadAsStringAsync();
                    data = JsonConvert.DeserializeObject<GeneralOutputModel>(apiResponse);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    output.Status = "UA";
                    output.Message = "Session Habis";

                    return output;
                }
                return data;
            }
            catch (Exception ex)
            {
                output.Status = "NG";
                output.Message = ex.ToString();

                return output;
            }
        }


        public async Task<GeneralOutputModel> UpdateLevelEMp(string accessUrl, string token, ParamUpdateLevelemp pr)
        {
            try
            {
                GeneralOutputModel data = new GeneralOutputModel();
                string jsonString = JsonConvert.SerializeObject(pr);  //convert to JSON
                Dictionary<string, string> values = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonString);  //convert to key/value pairs
                HttpClient httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                var response = await httpClient.PostAsync(urlApi + accessUrl, new FormUrlEncodedContent(values));

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var apiResponse = await response.Content.ReadAsStringAsync();
                    data = JsonConvert.DeserializeObject<GeneralOutputModel>(apiResponse);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)

                    if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    {
                        output.Status = "UA";
                        output.Message = "Session Habis";

                        return output;
                    }

                return data;
            }
            catch (Exception ex)
            {
                output.Status = "NG";
                output.Message = ex.ToString();

                return output;
            }
        }
        public async Task<GeneralOutputModel> GetDataReportNonEoffice(string accessUrl, string token, ParamReportNonOuboxLetter pr)
        {
            try
            {
                GeneralOutputModel data = new GeneralOutputModel();
                string jsonString = JsonConvert.SerializeObject(pr);  //convert to JSON
                Dictionary<string, string> values = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonString);  //convert to key/value pairs
                HttpClient httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                var response = await httpClient.PostAsync(urlApi + accessUrl, new FormUrlEncodedContent(values));
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    output.Status = "UA";
                    output.Message = "Session Habis";

                    return output;
                }
                else
                {
                    var apiResponse = await response.Content.ReadAsStringAsync();
                    data = JsonConvert.DeserializeObject<GeneralOutputModel>(apiResponse);
                }
                return data;
            }
            catch (Exception ex)
            {
                output.Status = "NG";
                output.Message = ex.ToString();

                return output;
            }
        }
        public async Task<GeneralOutputModel> GetContentTemplate(string accessUrl, string token)

        {
            try
            {
                GeneralOutputModel data = new GeneralOutputModel();
                HttpClient httpClient = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Get, urlApi + accessUrl);
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                HttpResponseMessage response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var apiResponse = await response.Content.ReadAsStringAsync();
                    data = JsonConvert.DeserializeObject<GeneralOutputModel>(apiResponse);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)

                {
                    output.Status = "UA";
                    output.Message = "Session Habis";

                    return output;
                }

               

                return data;
            }
            catch (Exception ex)
            {
                output.Status = "NG";
                output.Message = ex.ToString();

                return output;
            }
        }

        public async Task<GeneralOutputModel> AddContentTemplate(string accessUrl, string token, ParamInsertContentTemplate pr)
        {
            try
            {
                GeneralOutputModel data = new GeneralOutputModel();
                string jsonString = JsonConvert.SerializeObject(pr);  //convert to JSON
                Dictionary<string, string> values = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonString);  //convert to key/value pairs
                HttpClient httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                var response = await httpClient.PostAsync(urlApi + accessUrl, new FormUrlEncodedContent(values));

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var apiResponse = await response.Content.ReadAsStringAsync();
                    data = JsonConvert.DeserializeObject<GeneralOutputModel>(apiResponse);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)

                    if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    {
                        output.Status = "UA";
                        output.Message = "Session Habis";

                        return output;
                    }

                return data;
            }
            catch (Exception ex)
            {
                output.Status = "NG";
                output.Message = ex.ToString();

                return output;
            }
        }


        public async Task<GeneralOutputModel> AddContentTemplateBulk(string accessUrl, string token, ParamInsertTemplateBulk pr)
        {
            try
            {
                GeneralOutputModel data = new GeneralOutputModel();

                string jsonStringReceiver = JsonConvert.SerializeObject(pr.parameter);
                string jsonString = JsonConvert.SerializeObject(pr);  //convert to JSON
                var objStringReceiver = JsonConvert.DeserializeObject<List<ListParameter>>(jsonStringReceiver);
                var jObject = JObject.Parse(jsonString);
                jObject["parameter"] = JsonConvert.SerializeObject(objStringReceiver);
                jsonString = JsonConvert.SerializeObject(jObject);  //convert to JSON
                Dictionary<string, string> values = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonString);  //convert to key/value pairs
                HttpClient httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                var response = await httpClient.PostAsync(urlApi + accessUrl, new FormUrlEncodedContent(values));
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    output.Status = "UA";
                    output.Message = "Session Habis";

                    return output;
                }
                else
                {
                    var apiResponse = await response.Content.ReadAsStringAsync();
                    data = JsonConvert.DeserializeObject<GeneralOutputModel>(apiResponse);
                }
                return data;
            }
            catch (Exception ex)
            {
                output.Status = "NG";
                output.Message = ex.ToString();

                return output;
            }
        }

        public async Task<GeneralOutputModel> UpdateContentTemplate(string accessUrl, string token, ParamUpdateContentTemplate pr)
        {
            try
            {
                GeneralOutputModel data = new GeneralOutputModel();
                string jsonString = JsonConvert.SerializeObject(pr);  //convert to JSON
                Dictionary<string, string> values = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonString);  //convert to key/value pairs
                HttpClient httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                var response = await httpClient.PostAsync(urlApi + accessUrl, new FormUrlEncodedContent(values));

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var apiResponse = await response.Content.ReadAsStringAsync();
                    data = JsonConvert.DeserializeObject<GeneralOutputModel>(apiResponse);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)

                    if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    {
                        output.Status = "UA";
                        output.Message = "Session Habis";

                        return output;
                    }
                return data;
            }
            catch (Exception ex)
            {
                output.Status = "NG";
                output.Message = ex.ToString();

                return output;
            }
        }


        public async Task<GeneralOutputModel> GetDataReportOutgoingMailEkspedisinKurir(string accessUrl, string token, ParamReportOutgoingMailEksnKurir pr)
        {
            try
            {
                GeneralOutputModel data = new GeneralOutputModel();
                string jsonString = JsonConvert.SerializeObject(pr);  //convert to JSON
                Dictionary<string, string> values = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonString);  //convert to key/value pairs
                HttpClient httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                var response = await httpClient.PostAsync(urlApi + accessUrl, new FormUrlEncodedContent(values));
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    output.Status = "UA";
                    output.Message = "Session Habis";

                    return output;
                }
                else
                {
                    var apiResponse = await response.Content.ReadAsStringAsync();
                    data = JsonConvert.DeserializeObject<GeneralOutputModel>(apiResponse);
                }
                return data;
            }
            catch (Exception ex)
            {
                output.Status = "NG";
                output.Message = ex.ToString();

                return output;
            }
        }
        public async Task<GeneralOutputModel> InsertLetterOutbox(string accessUrl, string token, ParamInsertLetterOutbox pr)
        {
            try
            {
                GeneralOutputModel data = new GeneralOutputModel();

                string jsonStringChecker = JsonConvert.SerializeObject(pr.idUserChecker);
                string jsonStringReceiver = JsonConvert.SerializeObject(pr.senderName);
                string jsonString = JsonConvert.SerializeObject(pr);  //convert to JSON
                var objStringChecker = JsonConvert.DeserializeObject<List<ParamInsertChecker>>(jsonStringChecker);
                var objStringReceiver = JsonConvert.DeserializeObject<List<ParamInsertOutgoingRecipient>>(jsonStringReceiver);
                var jObject = JObject.Parse(jsonString);
                jObject["idUserChecker"] = JsonConvert.SerializeObject(objStringChecker);
                jObject["senderName"] = JsonConvert.SerializeObject(objStringReceiver);
                jsonString = JsonConvert.SerializeObject(jObject);  //convert to JSON
                Dictionary<string, string> values = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonString);  //convert to key/value pairs
                HttpClient httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                var response = await httpClient.PostAsync(urlApi + accessUrl, new FormUrlEncodedContent(values));
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    output.Status = "UA";
                    output.Message = "Session Habis";

                    return output;
                }
                else
                {
                    var apiResponse = await response.Content.ReadAsStringAsync();
                    data = JsonConvert.DeserializeObject<GeneralOutputModel>(apiResponse);
                }
                return data;
            }
            catch (Exception ex)
            {
                output.Status = "NG";
                output.Message = ex.ToString();

                return output;
            }
        }

        public async Task<GeneralOutputModel> ApprovalLetter(string accessUrl, string token, ParamApprovalLetter pr)
        {
            try
            {
                GeneralOutputModel data = new GeneralOutputModel();
                string jsonString = JsonConvert.SerializeObject(pr);  //convert to JSON
                Dictionary<string, string> values = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonString);  //convert to key/value pairs
                HttpClient httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                var response = await httpClient.PostAsync(urlApi + accessUrl, new FormUrlEncodedContent(values));

                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    output.Status = "UA";
                    output.Message = "Session Habis";

                    return output;
                }
                else
                {
                    var apiResponse = await response.Content.ReadAsStringAsync();
                    data = JsonConvert.DeserializeObject<GeneralOutputModel>(apiResponse);
                }
                return data;
            }
            catch (Exception ex)
            {
                output.Status = "NG";
                output.Message = ex.ToString();

                return output;
            }
        }
        
        
        public async Task<GeneralOutputModel> PutUploadSignatureAsync(string accessUrl, string token, ImageUploadTTD pr)
        {
            try
            {
                GeneralOutputModel data = new GeneralOutputModel();
                string jsonString = JsonConvert.SerializeObject(pr);  //convert to JSON
                Dictionary<string, string> values = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonString);  //convert to key/value pairs
                HttpClient httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                var response = await httpClient.PostAsync(urlApi + accessUrl, new FormUrlEncodedContent(values));
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var apiResponse = await response.Content.ReadAsStringAsync();
                    data = JsonConvert.DeserializeObject<GeneralOutputModel>(apiResponse);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    output.Status = "UA";
                    output.Message = "Session Habis";

                    return output;
                }
                return data;
            }
            catch (Exception ex)
            {
                output.Status = "NG";
                output.Message = ex.ToString();

                return output;
            }
        }

        public async Task<GeneralOutputModel> GetOuputSignatureUser(string accessUrl, string token)
        {
            try
            {
                GeneralOutputModel data = new GeneralOutputModel();
                HttpClient httpClient = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Get, urlApi + accessUrl);
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                HttpResponseMessage response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var apiResponse = await response.Content.ReadAsStringAsync();
                    data = JsonConvert.DeserializeObject<GeneralOutputModel>(apiResponse);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    output.Status = "UA";
                    output.Message = "Session Habis";

                    return output;
                }
                return data;
            }
            catch (Exception ex)
            {
                output.Status = "NG";
                output.Message = ex.ToString();

                return output;
            }
        }


        //public async Task<GeneralOutputModel> PutApprovalUserSigantureAsync(string accessUrl, string token, ParamGetApprovalSignature pr)
        public async Task<GeneralOutputModel> PutApprovalUserSigantureAsync(string accessUrl, string token, ParamJsonStirngSiganture pr)
        {
            try
            {
                GeneralOutputModel data = new GeneralOutputModel();

                string jsonStringReceiver = JsonConvert.SerializeObject(pr.jsonDataString);
                string jsonString = JsonConvert.SerializeObject(pr);  //convert to JSON
                var objResponse1 = JsonConvert.DeserializeObject<List<ParamGetApprovalSignature>>(jsonStringReceiver);
                var jObject = JObject.Parse(jsonString);
                jObject["jsonDataString"] = JsonConvert.SerializeObject(objResponse1);
                jsonString = JsonConvert.SerializeObject(jObject);  //convert to JSON
                Dictionary<string, string> values = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonString);  //convert to key/value pairs
                HttpClient httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                var response = await httpClient.PostAsync(urlApi + accessUrl, new FormUrlEncodedContent(values));
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    output.Status = "UA";
                    output.Message = "Session Habis";

                    return output;
                }
                else
                {
                    var apiResponse = await response.Content.ReadAsStringAsync();
                    data = JsonConvert.DeserializeObject<GeneralOutputModel>(apiResponse);
                }
                return data;
            }
            catch (Exception ex)
            {
                output.Status = "NG";
                output.Message = ex.ToString();

                return output;
            }
        }


        public async Task<GeneralOutputModel> PutRejectUserSigantureAsync(string accessUrl, string token, ParamJsonStirngSiganture pr)
        {
            try
            {
                GeneralOutputModel data = new GeneralOutputModel();

                string jsonStringReceiver = JsonConvert.SerializeObject(pr.jsonDataString);
                string jsonString = JsonConvert.SerializeObject(pr);  //convert to JSON
                var objResponse1 = JsonConvert.DeserializeObject<List<ParamGetApprovalSignature>>(jsonStringReceiver);
                var jObject = JObject.Parse(jsonString);
                jObject["jsonDataString"] = JsonConvert.SerializeObject(objResponse1);
                jsonString = JsonConvert.SerializeObject(jObject);  //convert to JSON
                Dictionary<string, string> values = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonString);  //convert to key/value pairs
                HttpClient httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                var response = await httpClient.PostAsync(urlApi + accessUrl, new FormUrlEncodedContent(values));
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    output.Status = "UA";
                    output.Message = "Session Habis";

                    return output;
                }
                else
                {
                    var apiResponse = await response.Content.ReadAsStringAsync();
                    data = JsonConvert.DeserializeObject<GeneralOutputModel>(apiResponse);
                }
                return data;
            }
            catch (Exception ex)
            {
                output.Status = "NG";
                output.Message = ex.ToString();

                return output;
            }
        }



        #region Kurir Non Eoffice
        public async Task<GeneralOutputModel> GetSuratKeluarKurirNon(string accessUrl, string token)
        {
            try
            {
                GeneralOutputModel data = new GeneralOutputModel();
                HttpClient httpClient = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Get, urlApi + accessUrl);
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                HttpResponseMessage response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var apiResponse = await response.Content.ReadAsStringAsync();
                    data = JsonConvert.DeserializeObject<GeneralOutputModel>(apiResponse);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    output.Status = "UA";
                    output.Message = "Session Habis";

                    return output;
                }
                return data;
            }
            catch (Exception ex)
            {
                output.Status = "NG";
                output.Message = ex.ToString();

                return output;
            }
        }


        public async Task<GeneralOutputModel> PostUpdateKurirNonEofficeAsync(string accessUrl, UpdateKurirNonEoffice pr, string token)
        {
            try
            {
                GeneralOutputModel data = new GeneralOutputModel();
                string jsonString = JsonConvert.SerializeObject(pr);  //convert to JSON
                Dictionary<string, string> values = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonString);  //convert to key/value pairs
                HttpClient httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                var response = await httpClient.PostAsync(urlApi + accessUrl, new FormUrlEncodedContent(values));
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var apiResponse = await response.Content.ReadAsStringAsync();
                    data = JsonConvert.DeserializeObject<GeneralOutputModel>(apiResponse);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    output.Status = "UA";
                    output.Message = "Session Habis";

                    return output;
                }
                return data;
            }
            catch (Exception ex)
            {
                output.Status = "NG";
                output.Message = ex.ToString();

                return output;
            }
        }
        #endregion


        public async Task<GeneralOutputModel> DeleterSignatureUserProfile(string accessUrl, string token, OuputSignature pr)
        {
            try
            {
                GeneralOutputModel data = new GeneralOutputModel();
                string jsonString = JsonConvert.SerializeObject(pr);  //convert to JSON
                Dictionary<string, string> values = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonString);  //convert to key/value pairs
                HttpClient httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                var response = await httpClient.PostAsync(urlApi + accessUrl, new FormUrlEncodedContent(values));
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var apiResponse = await response.Content.ReadAsStringAsync();
                    data = JsonConvert.DeserializeObject<GeneralOutputModel>(apiResponse);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    output.Status = "UA";
                    output.Message = "Session Habis";

                    return output;
                }
                return data;
            }
            catch (Exception ex)
            {
                output.Status = "NG";
                output.Message = ex.ToString();

                return output;
            }
        }


        public async Task<GeneralOutputModel> DetailSignatureUserProfile(string accessUrl, string token, OuputSignature pr)
        {
            try
            {
                GeneralOutputModel data = new GeneralOutputModel();
                string jsonString = JsonConvert.SerializeObject(pr);  //convert to JSON
                Dictionary<string, string> values = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonString);  //convert to key/value pairs
                HttpClient httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                var response = await httpClient.PostAsync(urlApi + accessUrl, new FormUrlEncodedContent(values));
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var apiResponse = await response.Content.ReadAsStringAsync();
                    data = JsonConvert.DeserializeObject<GeneralOutputModel>(apiResponse);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    output.Status = "UA";
                    output.Message = "Session Habis";

                    return output;
                }
                return data;
            }
            catch (Exception ex)
            {
                output.Status = "NG";
                output.Message = ex.ToString();

                return output;
            }
        }

        public async Task<GeneralOutputModel> DetailApprovalSignatureUserProfile(string accessUrl, string token, OuputSignature pr)
        {
            try
            {
                GeneralOutputModel data = new GeneralOutputModel();
                string jsonString = JsonConvert.SerializeObject(pr);  //convert to JSON
                Dictionary<string, string> values = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonString);  //convert to key/value pairs
                HttpClient httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                var response = await httpClient.PostAsync(urlApi + accessUrl, new FormUrlEncodedContent(values));
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var apiResponse = await response.Content.ReadAsStringAsync();
                    data = JsonConvert.DeserializeObject<GeneralOutputModel>(apiResponse);
                }

                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    output.Status = "UA";
                    output.Message = "Session Habis";

                    return output;
                }
                return data;
            }
            catch (Exception ex)
            {
                output.Status = "NG";
                output.Message = ex.ToString();

                return output;
            }
        }


        public async Task<GeneralOutputModel> PostUpdateUploadLetterNonEoffcieAsync(string accessUrl, string token, ParamUploadLetterNonOfficeString pr)
        {
            try
            {
                GeneralOutputModel data = new GeneralOutputModel();

                string jsonStringReceiver = JsonConvert.SerializeObject(pr.jsonDataString);
                string jsonString = JsonConvert.SerializeObject(pr);  //convert to JSON
                var objResponse1 = JsonConvert.DeserializeObject<List<ParamUploadputletterNonEoffice>>(jsonStringReceiver);
                var jObject = JObject.Parse(jsonString);
                jObject["jsonDataString"] = JsonConvert.SerializeObject(objResponse1);
                jsonString = JsonConvert.SerializeObject(jObject);  //convert to JSON
                Dictionary<string, string> values = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonString);  //convert to key/value pairs
                HttpClient httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                var response = await httpClient.PostAsync(urlApi + accessUrl, new FormUrlEncodedContent(values));
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    output.Status = "UA";
                    output.Message = "Session Habis";

                    return output;
                }
                else
                {
                    var apiResponse = await response.Content.ReadAsStringAsync();
                    data = JsonConvert.DeserializeObject<GeneralOutputModel>(apiResponse);
                }
                return data;
            }
            catch (Exception ex)
            {
                output.Status = "NG";
                output.Message = ex.ToString();

                return output;
            }
        }


        public async Task<GeneralOutputModel> GetDataKurirNameNonEoffice(string accessUrl, string token, ParamGetKurir pr)
        {
            try
            {
                GeneralOutputModel data = new GeneralOutputModel();
                string jsonString = JsonConvert.SerializeObject(pr);  //convert to JSON
                Dictionary<string, string> values = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonString);  //convert to key/value pairs
                HttpClient httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                var response = await httpClient.PostAsync(urlApi + accessUrl, new FormUrlEncodedContent(values));
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var apiResponse = await response.Content.ReadAsStringAsync();
                    data = JsonConvert.DeserializeObject<GeneralOutputModel>(apiResponse);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    output.Status = "UA";
                    output.Message = "Session Habis";

                    return output;
                }
                return data;
            }
            catch (Exception ex)
            {
                output.Status = "NG";
                output.Message = ex.ToString();

                return output;
            }
        }

        public async Task<GeneralOutputModel> GetDetailsDataEkspediNonEoffice(string accessUrl, getByIdNonEoffice pr, string token)
        {
            try
            {
                GeneralOutputModel data = new GeneralOutputModel();
                string jsonString = JsonConvert.SerializeObject(pr);  //convert to JSON
                Dictionary<string, string> values = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonString);  //convert to key/value pairs
                HttpClient httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                var response = await httpClient.PostAsync(urlApi + accessUrl, new FormUrlEncodedContent(values));
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var apiResponse = await response.Content.ReadAsStringAsync();
                    data = JsonConvert.DeserializeObject<GeneralOutputModel>(apiResponse);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    output.Status = "UA";
                    output.Message = "Session Habis";

                    return output;
                }
                return data;
            }
            catch (Exception ex)
            {
                output.Status = "NG";
                output.Message = ex.ToString();

                return output;
            }
        }


        public async Task<GeneralOutputModel> GetDetailsDataKurirNonEoffice(string accessUrl, getByIdNonEoffice pr, string token)
        {
            try
            {
                GeneralOutputModel data = new GeneralOutputModel();
                string jsonString = JsonConvert.SerializeObject(pr);  //convert to JSON
                Dictionary<string, string> values = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonString);  //convert to key/value pairs
                HttpClient httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                var response = await httpClient.PostAsync(urlApi + accessUrl, new FormUrlEncodedContent(values));
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var apiResponse = await response.Content.ReadAsStringAsync();
                    data = JsonConvert.DeserializeObject<GeneralOutputModel>(apiResponse);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    output.Status = "UA";
                    output.Message = "Session Habis";

                    return output;
                }
                return data;
            }
            catch (Exception ex)
            {
                output.Status = "NG";
                output.Message = ex.ToString();

                return output;
            }
        }


        public async Task<GeneralOutputModel> DetailLetterNonEofficeNotifikasi(string accessUrl, string token, getByIdNonEoffice pr)
        {
            try
            {
                GeneralOutputModel data = new GeneralOutputModel();
                string jsonString = JsonConvert.SerializeObject(pr);  //convert to JSON
                Dictionary<string, string> values = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonString);  //convert to key/value pairs
                HttpClient httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                var response = await httpClient.PostAsync(urlApi + accessUrl, new FormUrlEncodedContent(values));
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var apiResponse = await response.Content.ReadAsStringAsync();
                    data = JsonConvert.DeserializeObject<GeneralOutputModel>(apiResponse);
                }

                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    output.Status = "UA";
                    output.Message = "Session Habis";

                    return output;
                }
                return data;
            }
            catch (Exception ex)
            {
                output.Status = "NG";
                output.Message = ex.ToString();

                return output;
            }
        }


        public async Task<GeneralOutputModel> GetDetailsViewEkspedisi_(string accessUrl, string token, ParamGetDetailsView pr)
        {
            try
            {
                GeneralOutputModel data = new GeneralOutputModel();
                string jsonString = JsonConvert.SerializeObject(pr);  //convert to JSON
                Dictionary<string, string> values = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonString);  //convert to key/value pairs
                HttpClient httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                var response = await httpClient.PostAsync(urlApi + accessUrl, new FormUrlEncodedContent(values));
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var apiResponse = await response.Content.ReadAsStringAsync();
                    data = JsonConvert.DeserializeObject<GeneralOutputModel>(apiResponse);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    output.Status = "UA";
                    output.Message = "Session Habis";

                    return output;
                }
                return data;
            }
            catch (Exception ex)
            {
                output.Status = "NG";
                output.Message = ex.ToString();

                return output;
            }
        }


        public async Task<GeneralOutputModel> GetDetailsViewKurir_(string accessUrl, string token, ParamGetDetailsView pr)
        {
            try
            {
                GeneralOutputModel data = new GeneralOutputModel();
                string jsonString = JsonConvert.SerializeObject(pr);  //convert to JSON
                Dictionary<string, string> values = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonString);  //convert to key/value pairs
                HttpClient httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                var response = await httpClient.PostAsync(urlApi + accessUrl, new FormUrlEncodedContent(values));
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var apiResponse = await response.Content.ReadAsStringAsync();
                    data = JsonConvert.DeserializeObject<GeneralOutputModel>(apiResponse);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    output.Status = "UA";
                    output.Message = "Session Habis";

                    return output;
                }
                return data;
            }
            catch (Exception ex)
            {
                output.Status = "NG";
                output.Message = ex.ToString();

                return output;
            }
        }


        public async Task<GeneralOutputModel> PutApprovalSigantureOneData(string accessUrl, string token, ParamGetApprovalSignatureOnedata pr)
        {
            try
            {
                GeneralOutputModel data = new GeneralOutputModel();
                string jsonString = JsonConvert.SerializeObject(pr);  //convert to JSON
                Dictionary<string, string> values = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonString);  //convert to key/value pairs
                HttpClient httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                var response = await httpClient.PostAsync(urlApi + accessUrl, new FormUrlEncodedContent(values));

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var apiResponse = await response.Content.ReadAsStringAsync();
                    data = JsonConvert.DeserializeObject<GeneralOutputModel>(apiResponse);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)

                    if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    {
                        output.Status = "UA";
                        output.Message = "Session Habis";

                        return output;
                    }

                return data;
            }
            catch (Exception ex)
            {
                output.Status = "NG";
                output.Message = ex.ToString();

                return output;
            }
        }

        public async Task<GeneralOutputModel> PutRejectSigantureOneData(string accessUrl, string token, ParamGetApprovalSignatureOnedata pr)
        {
            try
            {
                GeneralOutputModel data = new GeneralOutputModel();
                string jsonString = JsonConvert.SerializeObject(pr);  //convert to JSON
                Dictionary<string, string> values = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonString);  //convert to key/value pairs
                HttpClient httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                var response = await httpClient.PostAsync(urlApi + accessUrl, new FormUrlEncodedContent(values));

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var apiResponse = await response.Content.ReadAsStringAsync();
                    data = JsonConvert.DeserializeObject<GeneralOutputModel>(apiResponse);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)

                    if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    {
                        output.Status = "UA";
                        output.Message = "Session Habis";

                        return output;
                    }

                return data;
            }
            catch (Exception ex)
            {
                output.Status = "NG";
                output.Message = ex.ToString();

                return output;
            }
        }

        public async Task<GeneralOutputModel> InsertDelivery(string accessUrl, string token, ParamInsertDelivery pr)
        {
            try
            {
                GeneralOutputModel data = new GeneralOutputModel();

                string jsonStringChecker = JsonConvert.SerializeObject(pr.idLetter);
                string jsonString = JsonConvert.SerializeObject(pr);  //convert to JSON
                var objStringChecker = JsonConvert.DeserializeObject<List<InsertListLetterDelivery>>(jsonStringChecker);
                var jObject = JObject.Parse(jsonString);
                jObject["idLetter"] = JsonConvert.SerializeObject(objStringChecker);
                jsonString = JsonConvert.SerializeObject(jObject);  //convert to JSON
                Dictionary<string, string> values = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonString);  //convert to key/value pairs
                HttpClient httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                var response = await httpClient.PostAsync(urlApi + accessUrl, new FormUrlEncodedContent(values));
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    output.Status = "UA";
                    output.Message = "Session Habis";

                    return output;
                }
                else
                {
                    var apiResponse = await response.Content.ReadAsStringAsync();
                    data = JsonConvert.DeserializeObject<GeneralOutputModel>(apiResponse);
                }
                return data;
            }
            catch (Exception ex)
            {
                output.Status = "NG";
                output.Message = ex.ToString();

                return output;
            }
        }


        public async Task<GeneralOutputModel> GetDropDownUserBod(string accessUrl, string token)
        {
            try
            {
                GeneralOutputModel data = new GeneralOutputModel();
                HttpClient httpClient = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Get, urlApi + accessUrl);
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                HttpResponseMessage response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var apiResponse = await response.Content.ReadAsStringAsync();
                    data = JsonConvert.DeserializeObject<GeneralOutputModel>(apiResponse);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    output.Status = "UA";
                    output.Message = "Session Habis";

                    return output;
                }
                return data;
            }
            catch (Exception ex)
            {
                output.Status = "NG";
                output.Message = ex.ToString();

                return output;
            }
        }

        public async Task<GeneralOutputModel> GetViewDataUserBod(string accessUrl, string token, ParamGetUsertWeb pr)
        {
            try
            {
                GeneralOutputModel data = new GeneralOutputModel();
                string jsonString = JsonConvert.SerializeObject(pr);  //convert to JSON
                Dictionary<string, string> values = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonString);  //convert to key/value pairs
                HttpClient httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                var response = await httpClient.PostAsync(urlApi + accessUrl, new FormUrlEncodedContent(values));
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var apiResponse = await response.Content.ReadAsStringAsync();
                    data = JsonConvert.DeserializeObject<GeneralOutputModel>(apiResponse);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    output.Status = "UA";
                    output.Message = "Session Habis";

                    return output;
                }
                return data;
            }
            catch (Exception ex)
            {
                output.Status = "NG";
                output.Message = ex.ToString();

                return output;
            }
        }

        public async Task<GeneralOutputModel> CreatSettingBodAsync(string accessUrl, string token, ParamGetDetailUser pr)
        {
            try
            {
                GeneralOutputModel data = new GeneralOutputModel();
                string jsonString = JsonConvert.SerializeObject(pr);  //convert to JSON
                Dictionary<string, string> values = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonString);  //convert to key/value pairs
                HttpClient httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                var response = await httpClient.PostAsync(urlApi + accessUrl, new FormUrlEncodedContent(values));
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var apiResponse = await response.Content.ReadAsStringAsync();
                    data = JsonConvert.DeserializeObject<GeneralOutputModel>(apiResponse);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    output.Status = "UA";
                    output.Message = "Session Habis";

                    return output;
                }
                return data;
            }
            catch (Exception ex)
            {
                output.Status = "NG";
                output.Message = ex.ToString();

                return output;
            }
        }


        public async Task<GeneralOutputModel> DeleteDataBodAsync(string accessUrl, string token, ParamGetDetailUser pr)
        {
            try
            {
                GeneralOutputModel data = new GeneralOutputModel();
                string jsonString = JsonConvert.SerializeObject(pr);  //convert to JSON
                Dictionary<string, string> values = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonString);  //convert to key/value pairs
                HttpClient httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                var response = await httpClient.PostAsync(urlApi + accessUrl, new FormUrlEncodedContent(values));
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var apiResponse = await response.Content.ReadAsStringAsync();
                    data = JsonConvert.DeserializeObject<GeneralOutputModel>(apiResponse);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    output.Status = "UA";
                    output.Message = "Session Habis";

                    return output;
                }
                return data;
            }
            catch (Exception ex)
            {
                output.Status = "NG";
                output.Message = ex.ToString();

                return output;
            }
        }

        public async Task<GeneralOutputModel> GenerateQrCodeDeliveryEoffice(string accessUrl, string token, ParamGenerateDeliveryNumber pr)
        {
            try
            {
                GeneralOutputModel data = new GeneralOutputModel();
                string jsonString = JsonConvert.SerializeObject(pr);  //convert to JSON
                Dictionary<string, string> values = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonString);  //convert to key/value pairs
                HttpClient httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                var response = await httpClient.PostAsync(urlApi + accessUrl, new FormUrlEncodedContent(values));
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var apiResponse = await response.Content.ReadAsStringAsync();
                    data = JsonConvert.DeserializeObject<GeneralOutputModel>(apiResponse);
                }
                return data;
            }
            catch (Exception ex)
            {
                output.Status = "NG";
                output.Message = ex.ToString();

                return output;
            }
        }

        public async Task<GeneralOutputModel> GetDetailDeliveryEoffice(string accessUrl, string token, ParamGetDetailDelivery pr)
        {
            try
            {
                GeneralOutputModel data = new GeneralOutputModel();
                string jsonString = JsonConvert.SerializeObject(pr);  //convert to JSON
                Dictionary<string, string> values = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonString);  //convert to key/value pairs
                HttpClient httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                var response = await httpClient.PostAsync(urlApi + accessUrl, new FormUrlEncodedContent(values));
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    output.Status = "UA";
                    output.Message = "Session Habis";

                    return output;
                }
                else
                {
                    var apiResponse = await response.Content.ReadAsStringAsync();
                    data = JsonConvert.DeserializeObject<GeneralOutputModel>(apiResponse);
                }
                return data;
            }
            catch (Exception ex)
            {
                output.Status = "NG";
                output.Message = ex.ToString();

                return output;
            }
        }

        public async Task<GeneralOutputModel> UpdateDeliveryEoffice(string accessUrl, string token, ParamUpdateDelivery pr)
        {
            try
            {
                GeneralOutputModel data = new GeneralOutputModel();
                string jsonStringChecker = JsonConvert.SerializeObject(pr.idLetter);
                string jsonString = JsonConvert.SerializeObject(pr);  //convert to JSON
                var objStringChecker = JsonConvert.DeserializeObject<List<InsertListLetterDelivery>>(jsonStringChecker);
                var jObject = JObject.Parse(jsonString);
                jObject["idLetter"] = JsonConvert.SerializeObject(objStringChecker);
                jsonString = JsonConvert.SerializeObject(jObject);  //convert to JSON
                Dictionary<string, string> values = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonString);  //convert to key/value pairs
                HttpClient httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                var response = await httpClient.PostAsync(urlApi + accessUrl, new FormUrlEncodedContent(values));
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    output.Status = "UA";
                    output.Message = "Session Habis";

                    return output;
                }
                else
                {
                    var apiResponse = await response.Content.ReadAsStringAsync();
                    data = JsonConvert.DeserializeObject<GeneralOutputModel>(apiResponse);
                }
                return data;
            }
            catch (Exception ex)
            {
                output.Status = "NG";
                output.Message = ex.ToString();

                return output;
            }
        }

        public async Task<GeneralOutputModel> GetReportDeliveryEoffice(string accessUrl, string token, ParamGetReportEkspedisiEoffice pr)
        {
            try
            {
                GeneralOutputModel data = new GeneralOutputModel();
                string jsonString = JsonConvert.SerializeObject(pr);  //convert to JSON
                Dictionary<string, string> values = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonString);  //convert to key/value pairs
                HttpClient httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                var response = await httpClient.PostAsync(urlApi + accessUrl, new FormUrlEncodedContent(values));
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    output.Status = "UA";
                    output.Message = "Session Habis";

                    return output;
                }
                else
                {
                    var apiResponse = await response.Content.ReadAsStringAsync();
                    data = JsonConvert.DeserializeObject<GeneralOutputModel>(apiResponse);
                }
                return data;
            }
            catch (Exception ex)
            {
                output.Status = "NG";
                output.Message = ex.ToString();

                return output;
            }
        }

        public async Task<GeneralOutputModel> PostUploadExpeditionEoffice(string accessUrl, string token, ParamUploadEkspedisiEofficeString pr)
        {
            try
            {
                GeneralOutputModel data = new GeneralOutputModel();

                string jsonStringReceiver = JsonConvert.SerializeObject(pr.jsonDataString);
                string jsonString = JsonConvert.SerializeObject(pr);  //convert to JSON
                var objResponse1 = JsonConvert.DeserializeObject<List<ParamUploadEkspedisiEoffice>>(jsonStringReceiver);
                var jObject = JObject.Parse(jsonString);
                jObject["jsonDataString"] = JsonConvert.SerializeObject(objResponse1);
                jsonString = JsonConvert.SerializeObject(jObject);  //convert to JSON
                Dictionary<string, string> values = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonString);  //convert to key/value pairs
                HttpClient httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                var response = await httpClient.PostAsync(urlApi + accessUrl, new FormUrlEncodedContent(values));
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    output.Status = "UA";
                    output.Message = "Session Habis";

                    return output;
                }
                else
                {
                    var apiResponse = await response.Content.ReadAsStringAsync();
                    data = JsonConvert.DeserializeObject<GeneralOutputModel>(apiResponse);
                }
                return data;
            }
            catch (Exception ex)
            {
                output.Status = "NG";
                output.Message = ex.ToString();

                return output;
            }
        }
		public async Task<GeneralOutputModel> GetSkDelivery(string accessUrl, string token, ParamGetSKDelivery pr)

        {
            try
            {
                GeneralOutputModel data = new GeneralOutputModel();
                string jsonString = JsonConvert.SerializeObject(pr);  //convert to JSON
                Dictionary<string, string> values = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonString);  //convert to key/value pairs
                HttpClient httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                var response = await httpClient.PostAsync(urlApi + accessUrl, new FormUrlEncodedContent(values));
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var apiResponse = await response.Content.ReadAsStringAsync();
                    data = JsonConvert.DeserializeObject<GeneralOutputModel>(apiResponse);
                }

                return data;
            }
            catch (Exception ex)
            {
                output.Status = "NG";
                output.Message = ex.ToString();

                return output;
            }
        }

        #region Memo
        public async Task<GeneralOutputModel> InsertAttachmentMemo(string accessUrl, string token, ParamInsertAttachmentMemo pr)
        {
            try
            {
                GeneralOutputModel data = new GeneralOutputModel();
                string jsonString = JsonConvert.SerializeObject(pr);  //convert to JSON
                Dictionary<string, string> values = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonString);  //convert to key/value pairs
                HttpClient httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                var response = await httpClient.PostAsync(urlApi + accessUrl, new FormUrlEncodedContent(values));
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var apiResponse = await response.Content.ReadAsStringAsync();
                    data = JsonConvert.DeserializeObject<GeneralOutputModel>(apiResponse);
                }
                return data;
            }
            catch (Exception ex)
            {
                output.Status = "NG";
                output.Message = ex.ToString();

                return output;
            }
        }

        public async Task<GeneralOutputModel> InsertLetterMemo(string accessUrl, string token, ParamInsertMemo pr)
        {
            try
            {
                GeneralOutputModel data = new GeneralOutputModel();

                string jsonStringChecker = JsonConvert.SerializeObject(pr.idUserChecker);
                string jsonStringPenerimaTo = JsonConvert.SerializeObject(pr.idUserCheckerPenerima);
                string jsonStringCarbonCopyCc = JsonConvert.SerializeObject(pr.idUserCheckerCarbonCopy);
                string jsonStringDelibration = JsonConvert.SerializeObject(pr.idUserDelibration);
                string jsonStringApprover = JsonConvert.SerializeObject(pr.idUserApprover);
                string jsonStringCheckerlain = JsonConvert.SerializeObject(pr.idUserCheckerlain);

                string jsonStringDelibretion = JsonConvert.SerializeObject(pr.idUserCheckerDelibretion);

                string jsonString = JsonConvert.SerializeObject(pr);  //convert to JSON
                var objStringChecker = JsonConvert.DeserializeObject<List<ParamInsertCheckerMemo>>(jsonStringChecker);
                var objStringPenerimaTo = JsonConvert.DeserializeObject<List<ParamInsertCheckerMemoPenerima>>(jsonStringPenerimaTo);
                var objStringCarbonCopyCc = JsonConvert.DeserializeObject<List<ParamInsertCheckerMemoCarbonCopy>>(jsonStringCarbonCopyCc);

                var objStringDelibration = JsonConvert.DeserializeObject<List<ParamInsertDelibrationMemo>>(jsonStringDelibration);
                var objStringApprover = JsonConvert.DeserializeObject<List<ParamInsertApproverMemo>>(jsonStringApprover);
                var objStringCheckerlain = JsonConvert.DeserializeObject<List<ParamInsertCheckerMemoLainya>>(jsonStringCheckerlain);



                var objStringDelibretion = JsonConvert.DeserializeObject<List<ParamInsertCheckerMemoDelibretion>>(jsonStringDelibretion);

                var jObject = JObject.Parse(jsonString);
                jObject["idUserChecker"] = JsonConvert.SerializeObject(objStringChecker);
                jObject["idUserApprover"] = JsonConvert.SerializeObject(objStringApprover);
                jObject["idUserCheckerPenerima"] = JsonConvert.SerializeObject(objStringPenerimaTo);
                jObject["idUserCheckerCarbonCopy"] = JsonConvert.SerializeObject(objStringCarbonCopyCc);

                jObject["idUserDelibration"] = JsonConvert.SerializeObject(objStringDelibration);


                jObject["idUserCheckerDelibretion"] = JsonConvert.SerializeObject(objStringDelibretion);

                jObject["idUserCheckerlain"] = JsonConvert.SerializeObject(objStringCheckerlain);

                jsonString = JsonConvert.SerializeObject(jObject);  //convert to JSON
                Dictionary<string, string> values = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonString);  //convert to key/value pairs
                HttpClient httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                var response = await httpClient.PostAsync(urlApi + accessUrl, new FormUrlEncodedContent(values));
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    output.Status = "UA";
                    output.Message = "Session Habis";

                    return output;
                }
                else
                {
                    var apiResponse = await response.Content.ReadAsStringAsync();
                    data = JsonConvert.DeserializeObject<GeneralOutputModel>(apiResponse);
                }
                return data;
            }
            catch (Exception ex)
            {
                output.Status = "NG";
                output.Message = ex.ToString();

                return output;
            }
        }

        public async Task<GeneralOutputModel> GetMemoDistribusi(string accessUrl, string token, ParamGetMemoWeb pr)
        {
            try
            {
                GeneralOutputModel data = new GeneralOutputModel();
                string jsonString = JsonConvert.SerializeObject(pr);  //convert to JSON
                Dictionary<string, string> values = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonString);  //convert to key/value pairs
                HttpClient httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                var response = await httpClient.PostAsync(urlApi + accessUrl, new FormUrlEncodedContent(values));
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    output.Status = "UA";
                    output.Message = "Session Habis";

                    return output;
                }
                else
                {
                    var apiResponse = await response.Content.ReadAsStringAsync();
                    data = JsonConvert.DeserializeObject<GeneralOutputModel>(apiResponse);
                }
                return data;
            }
            catch (Exception ex)
            {
                output.Status = "NG";
                output.Message = ex.ToString();

                return output;
            }
        }

        public async Task<GeneralOutputModel> GetDetailMemo(string accessUrl, string token, ParamGetDetailMemo pr)
        {
            try
            {
                GeneralOutputModel data = new GeneralOutputModel();
                string jsonString = JsonConvert.SerializeObject(pr);  //convert to JSON
                Dictionary<string, string> values = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonString);  //convert to key/value pairs
                HttpClient httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                var response = await httpClient.PostAsync(urlApi + accessUrl, new FormUrlEncodedContent(values));
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    output.Status = "UA";
                    output.Message = "Session Habis";

                    return output;
                }
                else
                {
                    var apiResponse = await response.Content.ReadAsStringAsync();
                    data = JsonConvert.DeserializeObject<GeneralOutputModel>(apiResponse);
                }
                return data;
            }
            catch (Exception ex)
            {
                output.Status = "NG";
                output.Message = ex.ToString();

                return output;
            }
        }


        public async Task<GeneralOutputModel> ApprovalMemo(string accessUrl, string token, ParamApprovalLetterMemo pr)
        {
            try
            {
                GeneralOutputModel data = new GeneralOutputModel();
                string jsonString = JsonConvert.SerializeObject(pr);  //convert to JSON
                Dictionary<string, string> values = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonString);  //convert to key/value pairs
                HttpClient httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                var response = await httpClient.PostAsync(urlApi + accessUrl, new FormUrlEncodedContent(values));

                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    output.Status = "UA";
                    output.Message = "Session Habis";

                    return output;
                }
                else
                {
                    var apiResponse = await response.Content.ReadAsStringAsync();
                    data = JsonConvert.DeserializeObject<GeneralOutputModel>(apiResponse);
                }
                return data;
            }
            catch (Exception ex)
            {
                output.Status = "NG";
                output.Message = ex.ToString();

                return output;
            }
        }


        public async Task<GeneralOutputModel> DeleteAttachmentMemo(string accessUrl, string token, ParamDeleteAttachmentMemo pr)
        {
            try
            {
                GeneralOutputModel data = new GeneralOutputModel();
                string jsonString = JsonConvert.SerializeObject(pr);  //convert to JSON
                Dictionary<string, string> values = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonString);  //convert to key/value pairs
                HttpClient httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                var response = await httpClient.PostAsync(urlApi + accessUrl, new FormUrlEncodedContent(values));
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    output.Status = "UA";
                    output.Message = "Session Habis";

                    return output;
                }
                else
                {
                    var apiResponse = await response.Content.ReadAsStringAsync();
                    data = JsonConvert.DeserializeObject<GeneralOutputModel>(apiResponse);
                }
                return data;
            }
            catch (Exception ex)
            {
                output.Status = "NG";
                output.Message = ex.ToString();

                return output;
            }
        }
        #endregion

        public async Task<GeneralOutputModel> GetMemotypeByIdAsync(string accessUrl, string token, ParamGetMemoTypeById pr)
        {
            try
            {
                GeneralOutputModel data = new GeneralOutputModel();
                string jsonString = JsonConvert.SerializeObject(pr);  //convert to JSON
                Dictionary<string, string> values = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonString);  //convert to key/value pairs
                HttpClient httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                var response = await httpClient.PostAsync(urlApi + accessUrl, new FormUrlEncodedContent(values));

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var apiResponse = await response.Content.ReadAsStringAsync();
                    data = JsonConvert.DeserializeObject<GeneralOutputModel>(apiResponse);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)

                    if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    {
                        output.Status = "UA";
                        output.Message = "Session Habis";

                        return output;
                    }

                return data;
            }
            catch (Exception ex)
            {
                output.Status = "NG";
                output.Message = ex.ToString();

                return output;
            }
        }

        public async Task<GeneralOutputModel> ApprovalDelebrationMemo(string accessUrl, string token, ParamApprovalLetterMemo pr)
        {
            try
            {
                GeneralOutputModel data = new GeneralOutputModel();
                string jsonString = JsonConvert.SerializeObject(pr);  //convert to JSON
                Dictionary<string, string> values = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonString);  //convert to key/value pairs
                HttpClient httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                var response = await httpClient.PostAsync(urlApi + accessUrl, new FormUrlEncodedContent(values));

                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    output.Status = "UA";
                    output.Message = "Session Habis";

                    return output;
                }
                else
                {
                    var apiResponse = await response.Content.ReadAsStringAsync();
                    data = JsonConvert.DeserializeObject<GeneralOutputModel>(apiResponse);
                }
                return data;
            }
            catch (Exception ex)
            {
                output.Status = "NG";
                output.Message = ex.ToString();

                return output;
            }
        }



        public async Task<GeneralOutputModel> GetDataPengadaan(string accessUrl, string token)
        {
            try
            {
                GeneralOutputModel data = new GeneralOutputModel();
                HttpClient httpClient = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Get, urlApi + accessUrl);
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                HttpResponseMessage response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var apiResponse = await response.Content.ReadAsStringAsync();
                    data = JsonConvert.DeserializeObject<GeneralOutputModel>(apiResponse);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    output.Status = "UA";
                    output.Message = "Session Habis";

                    return output;
                }
                return data;
            }
            catch (Exception ex)
            {
                output.Status = "NG";
                output.Message = ex.ToString();

                return output;
            }
        }


        public async Task<GeneralOutputModel> GetPengadaanByIdAsync(string accessUrl, string token, ParamGetMemoPengadaanById pr)
        {
            try
            {
                GeneralOutputModel data = new GeneralOutputModel();
                string jsonString = JsonConvert.SerializeObject(pr);  //convert to JSON
                Dictionary<string, string> values = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonString);  //convert to key/value pairs
                HttpClient httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                var response = await httpClient.PostAsync(urlApi + accessUrl, new FormUrlEncodedContent(values));

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var apiResponse = await response.Content.ReadAsStringAsync();
                    data = JsonConvert.DeserializeObject<GeneralOutputModel>(apiResponse);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)

                    if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    {
                        output.Status = "UA";
                        output.Message = "Session Habis";

                        return output;
                    }

                return data;
            }
            catch (Exception ex)
            {
                output.Status = "NG";
                output.Message = ex.ToString();

                return output;
            }
        }


        public async Task<GeneralOutputModel> GetDataMinMaxNomialByIdAsync(string accessUrl, string token, ParamCheckMinMaxNomial pr)
        {
            try
            {
                GeneralOutputModel data = new GeneralOutputModel();
                string jsonString = JsonConvert.SerializeObject(pr);  //convert to JSON
                Dictionary<string, string> values = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonString);  //convert to key/value pairs
                HttpClient httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                var response = await httpClient.PostAsync(urlApi + accessUrl, new FormUrlEncodedContent(values));
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var apiResponse = await response.Content.ReadAsStringAsync();
                    data = JsonConvert.DeserializeObject<GeneralOutputModel>(apiResponse);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    output.Status = "UA";
                    output.Message = "Session Habis";

                    return output;
                }
                return data;
            }
            catch (Exception ex)
            {
                output.Status = "NG";
                output.Message = ex.ToString();

                return output;
            }
        }

       


        public async Task<GeneralOutputModel> InsertLetterMemoBackdate(string accessUrl, string token, ParamInsertMemoBackdate pr)
        {
            try
            {
                GeneralOutputModel data = new GeneralOutputModel();

                string jsonStringChecker = JsonConvert.SerializeObject(pr.idUserChecker);
                string jsonStringPenerimaTo = JsonConvert.SerializeObject(pr.idUserCheckerPenerima);
                string jsonStringCarbonCopyCc = JsonConvert.SerializeObject(pr.idUserCheckerCarbonCopy);
                string jsonStringDelibration = JsonConvert.SerializeObject(pr.idUserDelibration);
                string jsonStringApprover = JsonConvert.SerializeObject(pr.idUserApprover);
                string jsonStringCheckerlain = JsonConvert.SerializeObject(pr.idUserCheckerlain);
                string jsonStringCheckerPengirim = JsonConvert.SerializeObject(pr.idUserCheckerPengirim);




                string jsonString = JsonConvert.SerializeObject(pr);  //convert to JSON
                var objStringChecker = JsonConvert.DeserializeObject<List<ParamInsertCheckerMemo>>(jsonStringChecker);
                var objStringCheckerPengirim = JsonConvert.DeserializeObject<List<ParamInsertCheckerMemoPengirim>>(jsonStringCheckerPengirim);
                var objStringPenerimaTo = JsonConvert.DeserializeObject<List<ParamInsertCheckerMemoPenerima>>(jsonStringPenerimaTo);
                var objStringCarbonCopyCc = JsonConvert.DeserializeObject<List<ParamInsertCheckerMemoCarbonCopy>>(jsonStringCarbonCopyCc);
                var objStringDelibration = JsonConvert.DeserializeObject<List<ParamInsertDelibrationMemo>>(jsonStringDelibration);
                var objStringApprover = JsonConvert.DeserializeObject<List<ParamInsertApproverMemo>>(jsonStringApprover);
                var objStringCheckerlain = JsonConvert.DeserializeObject<List<ParamInsertCheckerMemoLainya>>(jsonStringCheckerlain);


                var jObject = JObject.Parse(jsonString);
                jObject["idUserChecker"] = JsonConvert.SerializeObject(objStringChecker);
                jObject["idUserCheckerPengirim"] = JsonConvert.SerializeObject(objStringCheckerPengirim);
                jObject["idUserApprover"] = JsonConvert.SerializeObject(objStringApprover);
                jObject["idUserCheckerPenerima"] = JsonConvert.SerializeObject(objStringPenerimaTo);
                jObject["idUserCheckerCarbonCopy"] = JsonConvert.SerializeObject(objStringCarbonCopyCc);
                jObject["idUserDelibration"] = JsonConvert.SerializeObject(objStringDelibration);
                jObject["idUserCheckerlain"] = JsonConvert.SerializeObject(objStringCheckerlain);

                jsonString = JsonConvert.SerializeObject(jObject);  //convert to JSON
                Dictionary<string, string> values = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonString);  //convert to key/value pairs
                HttpClient httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                var response = await httpClient.PostAsync(urlApi + accessUrl, new FormUrlEncodedContent(values));
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    output.Status = "UA";
                    output.Message = "Session Habis";

                    return output;
                }
                else
                {
                    var apiResponse = await response.Content.ReadAsStringAsync();
                    data = JsonConvert.DeserializeObject<GeneralOutputModel>(apiResponse);
                }
                return data;
            }
            catch (Exception ex)
            {
                output.Status = "NG";
                output.Message = ex.ToString();

                return output;
            }
        }


        public async Task<GeneralOutputModel> GetDetailMemoBackDate(string accessUrl, string token, ParamGetDetailMemo pr)
        {
            try
            {
                GeneralOutputModel data = new GeneralOutputModel();
                string jsonString = JsonConvert.SerializeObject(pr);  //convert to JSON
                Dictionary<string, string> values = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonString);  //convert to key/value pairs
                HttpClient httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                var response = await httpClient.PostAsync(urlApi + accessUrl, new FormUrlEncodedContent(values));
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    output.Status = "UA";
                    output.Message = "Session Habis";
                    return output;
                }
                else
                {
                    var apiResponse = await response.Content.ReadAsStringAsync();
                    data = JsonConvert.DeserializeObject<GeneralOutputModel>(apiResponse);
                }
                return data;
            }
            catch (Exception ex)
            {
                output.Status = "NG";
                output.Message = ex.ToString();

                return output;
            }
        }



        public async Task<GeneralOutputModel> GetUserAdminPengadaansync(string accessUrl, string token)
        {
            try
            {
                GeneralOutputModel data = new GeneralOutputModel();
                HttpClient httpClient = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Get, urlApi + accessUrl);
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                HttpResponseMessage response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var apiResponse = await response.Content.ReadAsStringAsync();
                    data = JsonConvert.DeserializeObject<GeneralOutputModel>(apiResponse);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    output.Status = "UA";
                    output.Message = "Session Habis";

                    return output;
                }
                return data;
            }
            catch (Exception ex)
            {
                output.Status = "NG";
                output.Message = ex.ToString();

                return output;
            }
        }

        public async Task<GeneralOutputModel> GetDataAdminPengadaanAsync(string accessUrl, string token, ParamGetUsertWeb pr)
        {
            try
            {
                GeneralOutputModel data = new GeneralOutputModel();
                string jsonString = JsonConvert.SerializeObject(pr);  //convert to JSON
                Dictionary<string, string> values = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonString);  //convert to key/value pairs
                HttpClient httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                var response = await httpClient.PostAsync(urlApi + accessUrl, new FormUrlEncodedContent(values));
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var apiResponse = await response.Content.ReadAsStringAsync();
                    data = JsonConvert.DeserializeObject<GeneralOutputModel>(apiResponse);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    output.Status = "UA";
                    output.Message = "Session Habis";

                    return output;
                }
                return data;
            }
            catch (Exception ex)
            {
                output.Status = "NG";
                output.Message = ex.ToString();

                return output;
            }
        }

        public async Task<GeneralOutputModel> PutUpdateAdminPengadaanAsync(string accessUrl, string token, ParamGetDetailUser pr)
        {
            try
            {
                GeneralOutputModel data = new GeneralOutputModel();
                string jsonString = JsonConvert.SerializeObject(pr);  //convert to JSON
                Dictionary<string, string> values = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonString);  //convert to key/value pairs
                HttpClient httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                var response = await httpClient.PostAsync(urlApi + accessUrl, new FormUrlEncodedContent(values));
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var apiResponse = await response.Content.ReadAsStringAsync();
                    data = JsonConvert.DeserializeObject<GeneralOutputModel>(apiResponse);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    output.Status = "UA";
                    output.Message = "Session Habis";

                    return output;
                }
                return data;
            }
            catch (Exception ex)
            {
                output.Status = "NG";
                output.Message = ex.ToString();

                return output;
            }
        }

        public async Task<GeneralOutputModel> PutDeleteAdminPengadaanAsync(string accessUrl, string token, ParamGetDetailUser pr)
        {
            try
            {
                GeneralOutputModel data = new GeneralOutputModel();
                string jsonString = JsonConvert.SerializeObject(pr);  //convert to JSON
                Dictionary<string, string> values = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonString);  //convert to key/value pairs
                HttpClient httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                var response = await httpClient.PostAsync(urlApi + accessUrl, new FormUrlEncodedContent(values));
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var apiResponse = await response.Content.ReadAsStringAsync();
                    data = JsonConvert.DeserializeObject<GeneralOutputModel>(apiResponse);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    output.Status = "UA";
                    output.Message = "Session Habis";

                    return output;
                }
                return data;
            }
            catch (Exception ex)
            {
                output.Status = "NG";
                output.Message = ex.ToString();

                return output;
            }
        }



        public async Task<GeneralOutputModel> GetUserSetPengadaansync(string accessUrl, string token)
        {
            try
            {
                GeneralOutputModel data = new GeneralOutputModel();
                HttpClient httpClient = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Get, urlApi + accessUrl);
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                HttpResponseMessage response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var apiResponse = await response.Content.ReadAsStringAsync();
                    data = JsonConvert.DeserializeObject<GeneralOutputModel>(apiResponse);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    output.Status = "UA";
                    output.Message = "Session Habis";

                    return output;
                }
                return data;
            }
            catch (Exception ex)
            {
                output.Status = "NG";
                output.Message = ex.ToString();

                return output;
            }
        }

        public async Task<GeneralOutputModel> GetDataSetPengadaanAsync(string accessUrl, string token, ParamGetUsertWeb pr)
        {
            try
            {
                GeneralOutputModel data = new GeneralOutputModel();
                string jsonString = JsonConvert.SerializeObject(pr);  //convert to JSON
                Dictionary<string, string> values = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonString);  //convert to key/value pairs
                HttpClient httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                var response = await httpClient.PostAsync(urlApi + accessUrl, new FormUrlEncodedContent(values));
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var apiResponse = await response.Content.ReadAsStringAsync();
                    data = JsonConvert.DeserializeObject<GeneralOutputModel>(apiResponse);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    output.Status = "UA";
                    output.Message = "Session Habis";

                    return output;
                }
                return data;
            }
            catch (Exception ex)
            {
                output.Status = "NG";
                output.Message = ex.ToString();

                return output;
            }
        }

        public async Task<GeneralOutputModel> InsertDataPengadaan(string accessUrl, string token, ParamInsertPengadaan pr)
        {
            try
            {
                GeneralOutputModel data = new GeneralOutputModel();
                string jsonString = JsonConvert.SerializeObject(pr);  //convert to JSON
                Dictionary<string, string> values = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonString);  //convert to key/value pairs
                HttpClient httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                var response = await httpClient.PostAsync(urlApi + accessUrl, new FormUrlEncodedContent(values));

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var apiResponse = await response.Content.ReadAsStringAsync();
                    data = JsonConvert.DeserializeObject<GeneralOutputModel>(apiResponse);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)

                    if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    {
                        output.Status = "UA";
                        output.Message = "Session Habis";

                        return output;
                    }

                return data;
            }
            catch (Exception ex)
            {
                output.Status = "NG";
                output.Message = ex.ToString();

                return output;
            }
        }


        public async Task<GeneralOutputModel> GetUserSetDelegasisync(string accessUrl, string token)
        {
            try
            {
                GeneralOutputModel data = new GeneralOutputModel();
                HttpClient httpClient = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Get, urlApi + accessUrl);
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                HttpResponseMessage response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var apiResponse = await response.Content.ReadAsStringAsync();
                    data = JsonConvert.DeserializeObject<GeneralOutputModel>(apiResponse);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    output.Status = "UA";
                    output.Message = "Session Habis";

                    return output;
                }
                return data;
            }
            catch (Exception ex)
            {
                output.Status = "NG";
                output.Message = ex.ToString();

                return output;
            }
        }
        public async Task<GeneralOutputModel> GetDataSetDelegasiAsync(string accessUrl, string token, ParamGetUsertWeb pr)
        {
            try
            {
                GeneralOutputModel data = new GeneralOutputModel();
                string jsonString = JsonConvert.SerializeObject(pr);  //convert to JSON
                Dictionary<string, string> values = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonString);  //convert to key/value pairs
                HttpClient httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                var response = await httpClient.PostAsync(urlApi + accessUrl, new FormUrlEncodedContent(values));
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var apiResponse = await response.Content.ReadAsStringAsync();
                    data = JsonConvert.DeserializeObject<GeneralOutputModel>(apiResponse);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    output.Status = "UA";
                    output.Message = "Session Habis";

                    return output;
                }
                return data;
            }
            catch (Exception ex)
            {
                output.Status = "NG";
                output.Message = ex.ToString();

                return output;
            }
        }


        public async Task<GeneralOutputModel> InsertDataDelegasi(string accessUrl, string token, ParamInsertDelegasi pr)
        {
            try
            {
                GeneralOutputModel data = new GeneralOutputModel();
                string jsonString = JsonConvert.SerializeObject(pr);  //convert to JSON
                Dictionary<string, string> values = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonString);  //convert to key/value pairs
                HttpClient httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                var response = await httpClient.PostAsync(urlApi + accessUrl, new FormUrlEncodedContent(values));

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var apiResponse = await response.Content.ReadAsStringAsync();
                    data = JsonConvert.DeserializeObject<GeneralOutputModel>(apiResponse);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)

                    if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    {
                        output.Status = "UA";
                        output.Message = "Session Habis";

                        return output;
                    }

                return data;
            }
            catch (Exception ex)
            {
                output.Status = "NG";
                output.Message = ex.ToString();

                return output;
            }
        }

        

        public async Task<GeneralOutputModel> GetDataPengadaansync(string accessUrl, string token)
        {
            try
            {
                GeneralOutputModel data = new GeneralOutputModel();
                HttpClient httpClient = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Get, urlApi + accessUrl);
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                HttpResponseMessage response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var apiResponse = await response.Content.ReadAsStringAsync();
                    data = JsonConvert.DeserializeObject<GeneralOutputModel>(apiResponse);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    output.Status = "UA";
                    output.Message = "Session Habis";

                    return output;
                }
                return data;
            }
            catch (Exception ex)
            {
                output.Status = "NG";
                output.Message = ex.ToString();

                return output;
            }
        }
        public async Task<GeneralOutputModel> GetDataPengadaanApprovalsync(string accessUrl, string token)
        {
            try
            {
                GeneralOutputModel data = new GeneralOutputModel();
                HttpClient httpClient = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Get, urlApi + accessUrl);
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                HttpResponseMessage response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var apiResponse = await response.Content.ReadAsStringAsync();
                    data = JsonConvert.DeserializeObject<GeneralOutputModel>(apiResponse);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    output.Status = "UA";
                    output.Message = "Session Habis";

                    return output;
                }
                return data;
            }
            catch (Exception ex)
            {
                output.Status = "NG";
                output.Message = ex.ToString();

                return output;
            }
        }

        public async Task<GeneralOutputModel> PutApprovalPengadaanAsync(string accessUrl, string token, ParamJsonStirngPengadaan pr)
        {
            try
            {
                GeneralOutputModel data = new GeneralOutputModel();

                string jsonStringReceiver = JsonConvert.SerializeObject(pr.jsonDataString);
                string jsonString = JsonConvert.SerializeObject(pr);  //convert to JSON
                var objResponse1 = JsonConvert.DeserializeObject<List<ParamGetApprovalPengadaan>>(jsonStringReceiver);
                var jObject = JObject.Parse(jsonString);
                jObject["jsonDataString"] = JsonConvert.SerializeObject(objResponse1);
                jsonString = JsonConvert.SerializeObject(jObject);  //convert to JSON
                Dictionary<string, string> values = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonString);  //convert to key/value pairs
                HttpClient httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                var response = await httpClient.PostAsync(urlApi + accessUrl, new FormUrlEncodedContent(values));
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    output.Status = "UA";
                    output.Message = "Session Habis";

                    return output;
                }
                else
                {
                    var apiResponse = await response.Content.ReadAsStringAsync();
                    data = JsonConvert.DeserializeObject<GeneralOutputModel>(apiResponse);
                }
                return data;
            }
            catch (Exception ex)
            {
                output.Status = "NG";
                output.Message = ex.ToString();

                return output;
            }
        }

        public async Task<GeneralOutputModel> GetDataPengadaanModalsync(string accessUrl, string token, ParamGetPengadaanModal pr)
        {
            try
            {
                GeneralOutputModel data = new GeneralOutputModel();
                string jsonString = JsonConvert.SerializeObject(pr);  //convert to JSON
                Dictionary<string, string> values = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonString);  //convert to key/value pairs
                HttpClient httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                var response = await httpClient.PostAsync(urlApi + accessUrl, new FormUrlEncodedContent(values));
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var apiResponse = await response.Content.ReadAsStringAsync();
                    data = JsonConvert.DeserializeObject<GeneralOutputModel>(apiResponse);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    output.Status = "UA";
                    output.Message = "Session Habis";

                    return output;
                }
                return data;
            }
            catch (Exception ex)
            {
                output.Status = "NG";
                output.Message = ex.ToString();

                return output;
            }
        }

        public async Task<GeneralOutputModel> GetDataDelegasisync(string accessUrl, string token)
        {
            try
            {
                GeneralOutputModel data = new GeneralOutputModel();
                HttpClient httpClient = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Get, urlApi + accessUrl);
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                HttpResponseMessage response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var apiResponse = await response.Content.ReadAsStringAsync();
                    data = JsonConvert.DeserializeObject<GeneralOutputModel>(apiResponse);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    output.Status = "UA";
                    output.Message = "Session Habis";

                    return output;
                }
                return data;
            }
            catch (Exception ex)
            {
                output.Status = "NG";
                output.Message = ex.ToString();

                return output;
            }
        }

        public async Task<GeneralOutputModel> GetDataDelegasiApprovalsync(string accessUrl, string token)
        {
            try
            {
                GeneralOutputModel data = new GeneralOutputModel();
                HttpClient httpClient = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Get, urlApi + accessUrl);
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                HttpResponseMessage response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var apiResponse = await response.Content.ReadAsStringAsync();
                    data = JsonConvert.DeserializeObject<GeneralOutputModel>(apiResponse);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    output.Status = "UA";
                    output.Message = "Session Habis";

                    return output;
                }
                return data;
            }
            catch (Exception ex)
            {
                output.Status = "NG";
                output.Message = ex.ToString();

                return output;
            }
        }



        public async Task<GeneralOutputModel> PutApprovalDelegasiAsync(string accessUrl, string token, ParamJsonStirngDelegasi pr)
        {
            try
            {
                GeneralOutputModel data = new GeneralOutputModel();

                string jsonStringReceiver = JsonConvert.SerializeObject(pr.jsonDataString);
                string jsonString = JsonConvert.SerializeObject(pr);  //convert to JSON
                var objResponse1 = JsonConvert.DeserializeObject<List<ParamGetApprovalDelegasi>>(jsonStringReceiver);
                var jObject = JObject.Parse(jsonString);
                jObject["jsonDataString"] = JsonConvert.SerializeObject(objResponse1);
                jsonString = JsonConvert.SerializeObject(jObject);  //convert to JSON
                Dictionary<string, string> values = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonString);  //convert to key/value pairs
                HttpClient httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                var response = await httpClient.PostAsync(urlApi + accessUrl, new FormUrlEncodedContent(values));
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    output.Status = "UA";
                    output.Message = "Session Habis";

                    return output;
                }
                else
                {
                    var apiResponse = await response.Content.ReadAsStringAsync();
                    data = JsonConvert.DeserializeObject<GeneralOutputModel>(apiResponse);
                }
                return data;
            }
            catch (Exception ex)
            {
                output.Status = "NG";
                output.Message = ex.ToString();

                return output;
            }
        }


        public async Task<GeneralOutputModel> InsertMemoPengadaan(string accessUrl, string token, ParamInsertMemoPengadaan pr)
        {
            try
            {
                GeneralOutputModel data = new GeneralOutputModel();

                string jsonStringChecker = JsonConvert.SerializeObject(pr.idUserChecker);
                string jsonStringPenerimaTo = JsonConvert.SerializeObject(pr.idUserCheckerPenerima);
                string jsonStringCarbonCopyCc = JsonConvert.SerializeObject(pr.idUserCheckerCarbonCopy);
                string jsonStringApprover = JsonConvert.SerializeObject(pr.idUserApprover);
                string jsonStringDelibretion = JsonConvert.SerializeObject(pr.idUserCheckerDelibretion);
                string jsonStringAppoverPengadaan = JsonConvert.SerializeObject(pr.idUserApprovalPengadaan);
                string jsonStringCheckerlain = JsonConvert.SerializeObject(pr.idUserCheckerlain);
                string jsonStringCheckerPengirim = JsonConvert.SerializeObject(pr.idUserCheckerPengirim);


                string jsonString = JsonConvert.SerializeObject(pr);  //convert to JSON
                var objStringChecker = JsonConvert.DeserializeObject<List<ParamInsertCheckerMemo>>(jsonStringChecker);
                var objStringCheckerPengirim = JsonConvert.DeserializeObject<List<ParamInsertCheckerMemoPengirim>>(jsonStringCheckerPengirim);
                var objStringPenerimaTo = JsonConvert.DeserializeObject<List<ParamInsertCheckerMemoPenerima>>(jsonStringPenerimaTo);
                var objStringCarbonCopyCc = JsonConvert.DeserializeObject<List<ParamInsertCheckerMemoCarbonCopy>>(jsonStringCarbonCopyCc);
                var objStringApprover = JsonConvert.DeserializeObject<List<ParamInsertApproverMemo>>(jsonStringApprover);
                var objStringDelibretion = JsonConvert.DeserializeObject<List<ParamInsertCheckerMemoDelibretion>>(jsonStringDelibretion);
                var objStringAppoverPengadaan = JsonConvert.DeserializeObject<List<ParamInsertApprovalPengadaan>>(jsonStringAppoverPengadaan);
                var objStringCheckerlain = JsonConvert.DeserializeObject<List<ParamInsertCheckerMemoLainya>>(jsonStringCheckerlain);

                var jObject = JObject.Parse(jsonString);
                jObject["idUserChecker"] = JsonConvert.SerializeObject(objStringChecker);
                jObject["idUserCheckerPengirim"] = JsonConvert.SerializeObject(objStringCheckerPengirim);
                jObject["idUserApprover"] = JsonConvert.SerializeObject(objStringApprover);
                jObject["idUserCheckerPenerima"] = JsonConvert.SerializeObject(objStringPenerimaTo);
                jObject["idUserCheckerCarbonCopy"] = JsonConvert.SerializeObject(objStringCarbonCopyCc);
                jObject["idUserApprovalPengadaan"] = JsonConvert.SerializeObject(objStringAppoverPengadaan);
                jObject["idUserCheckerlain"] = JsonConvert.SerializeObject(objStringCheckerlain);


                jObject["idUserCheckerDelibretion"] = JsonConvert.SerializeObject(objStringDelibretion);

                jsonString = JsonConvert.SerializeObject(jObject);  //convert to JSON
                Dictionary<string, string> values = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonString);  //convert to key/value pairs
                HttpClient httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                var response = await httpClient.PostAsync(urlApi + accessUrl, new FormUrlEncodedContent(values));
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    output.Status = "UA";
                    output.Message = "Session Habis";

                    return output;
                }
                else
                {
                    var apiResponse = await response.Content.ReadAsStringAsync();
                    data = JsonConvert.DeserializeObject<GeneralOutputModel>(apiResponse);
                }
                return data;
            }
            catch (Exception ex)
            {
                output.Status = "NG";
                output.Message = ex.ToString();

                return output;
            }
        }


        public async Task<GeneralOutputModel> GetDetailMemoPengadaan(string accessUrl, string token, ParamGetDetailMemo pr)
        {
            try
            {
                GeneralOutputModel data = new GeneralOutputModel();
                string jsonString = JsonConvert.SerializeObject(pr);  //convert to JSON
                Dictionary<string, string> values = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonString);  //convert to key/value pairs
                HttpClient httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                var response = await httpClient.PostAsync(urlApi + accessUrl, new FormUrlEncodedContent(values));
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    output.Status = "UA";
                    output.Message = "Session Habis";

                    return output;
                }
                else
                {
                    var apiResponse = await response.Content.ReadAsStringAsync();
                    data = JsonConvert.DeserializeObject<GeneralOutputModel>(apiResponse);
                }
                return data;
            }
            catch (Exception ex)
            {
                output.Status = "NG";
                output.Message = ex.ToString();

                return output;
            }
        }

        public async Task<GeneralOutputModel> GetLetterDraftMemoSrch(string accessUrl, string token, ParamGetMemoWeb pr)
        {
            try
            {
                GeneralOutputModel data = new GeneralOutputModel();
                string jsonString = JsonConvert.SerializeObject(pr);  //convert to JSON
                Dictionary<string, string> values = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonString);  //convert to key/value pairs
                HttpClient httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                var response = await httpClient.PostAsync(urlApi + accessUrl, new FormUrlEncodedContent(values));
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    output.Status = "UA";
                    output.Message = "Session Habis";

                    return output;
                }
                else
                {
                    var apiResponse = await response.Content.ReadAsStringAsync();
                    data = JsonConvert.DeserializeObject<GeneralOutputModel>(apiResponse);
                }
                return data;
            }
            catch (Exception ex)
            {
                output.Status = "NG";
                output.Message = ex.ToString();

                return output;
            }
        }
        public async Task<GeneralOutputModel> NotifikasiLainnya(string accessUrl, string token, getByIdNotifikasiLainnya pr)
        {
            try
            {
                GeneralOutputModel data = new GeneralOutputModel();
                string jsonString = JsonConvert.SerializeObject(pr); 
                Dictionary<string, string> values = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonString);  //convert to key/value pairs
                HttpClient httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                var response = await httpClient.PostAsync(urlApi + accessUrl, new FormUrlEncodedContent(values));
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var apiResponse = await response.Content.ReadAsStringAsync();
                    data = JsonConvert.DeserializeObject<GeneralOutputModel>(apiResponse);
                }

                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    output.Status = "UA";
                    output.Message = "Session Habis";

                    return output;
                }
                return data;
            }
            catch (Exception ex)
            {
                output.Status = "NG";
                output.Message = ex.ToString();

                return output;
            }
        }


        public async Task<GeneralOutputModel> InsertLetterOutboxBackdate(string accessUrl, string token, ParamInsertLetterOutboxBackdate pr)
                 {
            try
            {
                GeneralOutputModel data = new GeneralOutputModel();

                //string jsonStringChecker = JsonConvert.SerializeObject(pr.idUserChecker);
                //string jsonStringPenerimaTo = JsonConvert.SerializeObject(pr.idUserCheckerPenerima);
                //string jsonStringCarbonCopyCc = JsonConvert.SerializeObject(pr.idUserCheckerCarbonCopy);
                //string jsonStringApprover = JsonConvert.SerializeObject(pr.idUserApprover);
                //string jsonStringDelibretion = JsonConvert.SerializeObject(pr.idUserCheckerDelibretion);
                //string jsonStringAppoverPengadaan = JsonConvert.SerializeObject(pr.idUserApprovalPengadaan);
                //string jsonStringCheckerlain = JsonConvert.SerializeObject(pr.idUserCheckerlain);
                //string jsonStringCheckerPengirim = JsonConvert.SerializeObject(pr.idUserCheckerPengirim);


                //string jsonString = JsonConvert.SerializeObject(pr);  //convert to JSON
                //var objStringChecker = JsonConvert.DeserializeObject<List<ParamInsertCheckerMemo>>(jsonStringChecker);
                //var objStringCheckerPengirim = JsonConvert.DeserializeObject<List<ParamInsertCheckerMemoPengirim>>(jsonStringCheckerPengirim);
                //var objStringPenerimaTo = JsonConvert.DeserializeObject<List<ParamInsertCheckerMemoPenerima>>(jsonStringPenerimaTo);
                //var objStringCarbonCopyCc = JsonConvert.DeserializeObject<List<ParamInsertCheckerMemoCarbonCopy>>(jsonStringCarbonCopyCc);
                //var objStringApprover = JsonConvert.DeserializeObject<List<ParamInsertApproverMemo>>(jsonStringApprover);
                //var objStringDelibretion = JsonConvert.DeserializeObject<List<ParamInsertCheckerMemoDelibretion>>(jsonStringDelibretion);
                //var objStringAppoverPengadaan = JsonConvert.DeserializeObject<List<ParamInsertApprovalPengadaan>>(jsonStringAppoverPengadaan);
                //var objStringCheckerlain = JsonConvert.DeserializeObject<List<ParamInsertCheckerMemoLainya>>(jsonStringCheckerlain);

                //var jObject = JObject.Parse(jsonString);
                //jObject["idUserChecker"] = JsonConvert.SerializeObject(objStringChecker);
                //jObject["idUserCheckerPengirim"] = JsonConvert.SerializeObject(objStringCheckerPengirim);
                //jObject["idUserApprover"] = JsonConvert.SerializeObject(objStringApprover);
                //jObject["idUserCheckerPenerima"] = JsonConvert.SerializeObject(objStringPenerimaTo);
                //jObject["idUserCheckerCarbonCopy"] = JsonConvert.SerializeObject(objStringCarbonCopyCc);
                //jObject["idUserApprovalPengadaan"] = JsonConvert.SerializeObject(objStringAppoverPengadaan);
                //jObject["idUserCheckerlain"] = JsonConvert.SerializeObject(objStringCheckerlain);


                string jsonStringChecker = JsonConvert.SerializeObject(pr.idUserChecker);
                string jsonStringReceiver = JsonConvert.SerializeObject(pr.senderName);
                string jsonStringCheckerPengirim = JsonConvert.SerializeObject(pr.idUserCheckerPengirim);
                string jsonStringApprover = JsonConvert.SerializeObject(pr.idUserApprover);
                string jsonString = JsonConvert.SerializeObject(pr);  //convert to JSON

                var objStringChecker = JsonConvert.DeserializeObject<List<ParamInsertChecker>>(jsonStringChecker);
                var objStringCheckerPengirim = JsonConvert.DeserializeObject<List<ParamInsertCheckerPengirim>>(jsonStringCheckerPengirim);
                var objStringReceiver = JsonConvert.DeserializeObject<List<ParamInsertOutgoingRecipient>>(jsonStringReceiver);
                var objStringApprover = JsonConvert.DeserializeObject<List<ParamInsertApprover>>(jsonStringApprover);
                var jObject = JObject.Parse(jsonString);
                
                jObject["idUserChecker"] = JsonConvert.SerializeObject(objStringChecker);
                jObject["senderName"] = JsonConvert.SerializeObject(objStringReceiver);
                jObject["idUserCheckerPengirim"] = JsonConvert.SerializeObject(objStringCheckerPengirim);
                jObject["idUserApprover"] = JsonConvert.SerializeObject(objStringApprover);
                jsonString = JsonConvert.SerializeObject(jObject);  //convert to JSON
                Dictionary<string, string> values = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonString);  //convert to key/value pairs
                HttpClient httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                var response = await httpClient.PostAsync(urlApi + accessUrl, new FormUrlEncodedContent(values));
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    output.Status = "UA";
                    output.Message = "Session Habis";

                    return output;
                }
                else
                {
                    var apiResponse = await response.Content.ReadAsStringAsync();
                    data = JsonConvert.DeserializeObject<GeneralOutputModel>(apiResponse);
                }
                return data;
            }
            catch (Exception ex)
            {
                output.Status = "NG";
                output.Message = ex.ToString();

                return output;
            }
        }


        public async Task<GeneralOutputModel> UpdateStatusNotifikasi(string accessUrl, string token, ParamUpdateNotif pr)

        {
            try
            {
                GeneralOutputModel data = new GeneralOutputModel();
                string jsonString = JsonConvert.SerializeObject(pr);  //convert to JSON
                Dictionary<string, string> values = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonString);  //convert to key/value pairs
                HttpClient httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                var response = await httpClient.PostAsync(urlApi + accessUrl, new FormUrlEncodedContent(values));

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var apiResponse = await response.Content.ReadAsStringAsync();
                    data = JsonConvert.DeserializeObject<GeneralOutputModel>(apiResponse);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)

                    if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)

                    {
                        output.Status = "UA";
                        output.Message = "Session Habis";

                        return output;
                    }

                    else
                    {
                        var apiResponse = await response.Content.ReadAsStringAsync();
                        data = JsonConvert.DeserializeObject<GeneralOutputModel>(apiResponse);
                    }

                return data;
            }
            catch (Exception ex)
            {
                output.Status = "NG";
                output.Message = ex.ToString();

                return output;
            }
        }



        public async Task<GeneralOutputModel> PushNotifikasi(string accessUrl, string token, ParamPushNotifikasi pr)

        {
            try
            {
                GeneralOutputModel data = new GeneralOutputModel();
                string jsonString = JsonConvert.SerializeObject(pr);  //convert to JSON
                Dictionary<string, string> values = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonString);  //convert to key/value pairs
                HttpClient httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                var response = await httpClient.PostAsync(urllApi + accessUrl, new FormUrlEncodedContent(values));

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var apiResponse = await response.Content.ReadAsStringAsync();
                    data = JsonConvert.DeserializeObject<GeneralOutputModel>(apiResponse);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)

                    if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)

                    {
                        output.Status = "UA";
                        output.Message = "Session Habis";

                        return output;
                    }

                    else
                    {
                        var apiResponse = await response.Content.ReadAsStringAsync();
                        data = JsonConvert.DeserializeObject<GeneralOutputModel>(apiResponse);
                    }

                return data;
            }
            catch (Exception ex)
            {
                output.Status = "NG";
                output.Message = ex.ToString();

                return output;
            }
        }


        public async Task<GeneralOutputModel> GetSeacrhoutGoingEoffice(string accessUrl, string token, ParamGetReportSeacrhoutGoing pr)
        {
            try
            {
                GeneralOutputModel data = new GeneralOutputModel();
                string jsonString = JsonConvert.SerializeObject(pr);  //convert to JSON
                Dictionary<string, string> values = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonString);  //convert to key/value pairs
                HttpClient httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                var response = await httpClient.PostAsync(urlApi + accessUrl, new FormUrlEncodedContent(values));
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    output.Status = "UA";
                    output.Message = "Session Habis";

                    return output;
                }
                else
                {
                    var apiResponse = await response.Content.ReadAsStringAsync();
                    data = JsonConvert.DeserializeObject<GeneralOutputModel>(apiResponse);
                }
                return data;
            }
            catch (Exception ex)
            {
                output.Status = "NG";
                output.Message = ex.ToString();

                return output;
            }
        }



    }
}
