using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CommunityGateClient.Models.ViewModels;
using System.Net.Http;
using Newtonsoft.Json;
using System.Text;

namespace CommunityGateClient.Controllers
{
    public class LoginController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Login(LoginDetails loginDetails)
        {
            //string token;
            using (var httpClient = new HttpClient())
            {
                ViewBag.message = "";

                StringContent content = new StringContent(JsonConvert.SerializeObject(loginDetails), Encoding.UTF8, "application/json");

                using (var response = await httpClient.PostAsync("https://localhost:44388/api/Login/AuthenicateUser", content))
                {



                    if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    {
                        ViewBag.message = "Invalid User";

                    }
                    else
                    {
                        ViewBag.message = "Success";
                    }
                }

            }
            return View();



        }

    }
}
