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
using Microsoft.AspNetCore.Mvc.Localization;

namespace EofficeBNILWEB.Controllers
{
    [Authorize]
    public class MenuViewComponent : ViewComponent
    {
        private readonly IConfiguration _config;
        private readonly IDataAccessProvider _dataAccessProvider;
        private readonly string urlApi;
        public MenuViewComponent(IConfiguration config, IDataAccessProvider dataAccessProvider)
        {
            _config = config;
            _dataAccessProvider = dataAccessProvider;
            urlApi = _config.GetValue<string>("AppSettings:apiUrl");
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            List<MenuOutput> menuList = new List<MenuOutput>();
            GeneralOutputModel generalOutput = new GeneralOutputModel();
            var token = HttpContext.Session.GetString("token");
            var idGroup = HttpContext.Session.GetString("idGroup");
            generalOutput = await _dataAccessProvider.GetDataMenuAsync(idGroup, "Menu/",token);
            var jsonApiResponseSerialize = JsonConvert.SerializeObject(generalOutput.Result);
            menuList = JsonConvert.DeserializeObject<List<MenuOutput>>(jsonApiResponseSerialize);
           
            return await Task.FromResult((IViewComponentResult)View("Menu", menuList));
        }
    }
}
