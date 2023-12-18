using EofficeBNILWEB.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Reflection;
using System.Diagnostics;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using System.Numerics;
using EofficeBNILWEB.DataAccess;
using Microsoft.AspNetCore.Authorization;
using DocumentFormat.OpenXml.InkML;

namespace EofficeBNILWEB.Controllers
{
    public class NotificationViewComponent : ViewComponent
    {
        private readonly IConfiguration _config;
        private readonly IDataAccessProvider _dataAccessProvider;
        private readonly string urlApi;
        public NotificationViewComponent(IConfiguration config, IDataAccessProvider dataAccessProvider)
        {
            _config = config;
            _dataAccessProvider = dataAccessProvider;
            urlApi = _config.GetValue<string>("AppSettings:apiUrl");
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            DashboardOutput dahsboarOutput = new DashboardOutput();
            GeneralOutputModel generalOutput = new GeneralOutputModel();
            var token = HttpContext.Session.GetString("token");
            generalOutput = await _dataAccessProvider.GetDashboardContent("General/GetDashboardContent/", token);
            var jsonApiResponseSerialize = JsonConvert.SerializeObject(generalOutput.Result);
            dahsboarOutput = JsonConvert.DeserializeObject<DashboardOutput>(jsonApiResponseSerialize);

            return await Task.FromResult((IViewComponentResult)View("Notification", dahsboarOutput));
        }


        

    }
}
