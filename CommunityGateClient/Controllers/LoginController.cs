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
        static readonly log4net.ILog _log4net = log4net.LogManager.GetLogger(typeof(LoginController));
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Login()
        {
            _log4net.Info("Login Page Was Called !!");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginDetails loginDetails)
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
                        _log4net.Info("Login Was Done With Email " + loginDetails.Username + " But the Credentials Were Wrong !!");
                        ViewBag.message = "Invalid User";

                    }
                    else
                    {
                        _log4net.Info("Login Was Done With Email " + loginDetails.Username + " And the Right Password !!");
                        ViewBag.message = "Success";
                    }
                }

            }
            return View();



        }

    }
}
