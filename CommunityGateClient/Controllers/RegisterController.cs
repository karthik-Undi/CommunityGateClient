using CommunityGateClient.Models;
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

        static readonly log4net.ILog _log4net = log4net.LogManager.GetLogger(typeof(LoginController));


        public IActionResult Index()
        {
            return View();
        }
        public IActionResult RegisterEmployee()
        {

            return View();
        }
        [HttpPost]
        public async Task<IActionResult> RegisterEmployee(EmployeeRegistration employee)
        {
            if (ModelState.IsValid)
            {
                using (var httpClient = new HttpClient())
                {
                    ViewBag.message = "";
                   
                    StringContent content = new StringContent(JsonConvert.SerializeObject(employee), Encoding.UTF8, "application/json");

                    using (var response = await httpClient.PostAsync("http://localhost:62288/api/Employees/", content))
                    {



                        if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                        {
                            ViewBag.message = "Failed";
                        }
                        else
                        {
                            _log4net.Info("Registration Was Done With Email " + employee.EmployeeEmail);
                            return RedirectToAction("FirstEmployeeLogin", "Login",new {email=employee.EmployeeEmail,role=employee.EmployeeDept });
                        }
                    }

                }
            }
            return View();
        }

    }
}
