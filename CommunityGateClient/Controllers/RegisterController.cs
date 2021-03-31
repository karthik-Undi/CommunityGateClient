using CommunityGateClient.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace CommunityGateClient.Controllers
{
    public class RegisterController : Controller
    {
        static readonly log4net.ILog _log4net = log4net.LogManager.GetLogger(typeof(RegisterController));
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult RegisterForResident()
        {
            _log4net.Info("Register For Resident Was Called");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RegisterForResident(RegisterDetailsForResident regDetailsResident)
        {
            if (ModelState.IsValid)
            {
                //string token;
                using (var httpClient = new HttpClient())
                {
                    StringContent content = new StringContent(JsonConvert.SerializeObject(regDetailsResident), Encoding.UTF8, "application/json");

                    using (var response = await httpClient.PostAsync("http://localhost:27414/api/Resident", content))
                    {
                        if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                        {
                            _log4net.Info("Register Was Done With Email " + regDetailsResident.ResidentEmail + " But Registration Didnt Happen !!");
                            ViewBag.message = "Registration Failed";

                        }
                        else
                        {
                            _log4net.Info("Register Was Done With Email " + regDetailsResident.ResidentEmail + " And the Registration Was Successfull !!");

                            ViewBag.message = "Registered Successfully";
                        }
                    }

                }
            }
            return View();
        }
    }
}
