using System;
using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UriWebApi.Services;

namespace UriWebApi.Controllers
{
    [Route("api/uri")]
    [ApiController]
    public class UriController : Controller
    {
        [HttpGet("{Get}")]
        public JsonResult Get(string nome)
        {
            try
            {
                UriService uriService = new UriService();

                object result = uriService.GetRanking(nome);

                return Json(new { data = result });
            }
            catch (Exception ex)
            {
                Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                return Json(new { mensagem = ex.Message });
            }
        }
    }
}
