using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CommunityGateClient.Models.ViewModels;
using System.Net.Http;
using Newtonsoft.Json;
using System.Text;
using CommunityGateClient.Models;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Http;

namespace CommunityGateClient.Controllers
{
    public class LoginController : Controller
    {
        static readonly log4net.ILog _log4net = log4net.LogManager.GetLogger(typeof(LoginController));
        string baseUrlForResidentApi = "http://localhost:27414/";
        string baseUrlForEmployeeAPI = "http://localhost:62288/";

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Login()
        {
            _log4net.Info("Login Page Was Called !!");
            return View();
        }

        public IActionResult FirstEmployeeLogin(string email, string role)
        {
            var model = new LoginDetails();
            model.Username = email;
            model.LoginType = "Employee";

            _log4net.Info("Login Page Was Called !!");
            return View("Login", model);
        }

        public IActionResult FirstResidentLogin(string email)
        {
            var model = new LoginDetails();
            model.Username = email;
            model.LoginType = "Resident";

            _log4net.Info("Login Page Was Called !!");
            return View("Login", model);
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginDetails loginDetails)
        {

            //string token;
            using (var httpClient = new HttpClient())
            {
                StringContent content = new StringContent(JsonConvert.SerializeObject(loginDetails), Encoding.UTF8, "application/json");
                try
                {
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
                            if(loginDetails.LoginType == "Resident")
                            {
                                Residents resident = new Residents();
                                try
                                {

                                    using (var client = new HttpClient())
                                    {
                                        client.BaseAddress = new Uri(baseUrlForResidentApi);
                                        client.DefaultRequestHeaders.Clear();
                                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                                        HttpResponseMessage Res = await client.GetAsync("api/Resident/GetResidentByMail/" + loginDetails.Username );
                                        if (Res.IsSuccessStatusCode)
                                        {
                                            var Response = Res.Content.ReadAsStringAsync().Result;
                                            resident = JsonConvert.DeserializeObject<Residents>(Response);
                                            HttpContext.Session.SetInt32("UserID", resident.ResidentId);
                                            return RedirectToAction("ResidentDashboard", "Resident");
                                        }

                                    }
                                }
                                catch (Exception)
                                {
                                    ViewBag.Message = "Resident API Not Reachable. Please Try Again Later.";
                                }
                            }
                            if(loginDetails.LoginType == "Employee")
                            {
                                Employees employee = new Employees();
                                try
                                {

                                    using (var client = new HttpClient())
                                    {
                                        client.BaseAddress = new Uri(baseUrlForEmployeeAPI);
                                        client.DefaultRequestHeaders.Clear();
                                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                                        HttpResponseMessage Res = await client.GetAsync("api/Employees/GetEmployeeByMail/" + loginDetails.Username);
                                        if (Res.IsSuccessStatusCode)
                                        {
                                            var Response = Res.Content.ReadAsStringAsync().Result;
                                            employee = JsonConvert.DeserializeObject<Employees>(Response);
                                            if (employee.EmployeeDept == "Security")
                                            {
                                                HttpContext.Session.SetInt32("UserID", employee.EmployeeId);
                                                return RedirectToAction("SecurityDashboard", "Security");
                                            }
                                            else
                                            {
                                                HttpContext.Session.SetInt32("UserID", employee.EmployeeId);
                                                return RedirectToAction("UtilityDashboard", "Utility");
                                            }
                                        }

                                    }
                                }
                                catch (Exception)
                                {
                                    ViewBag.Message = "Employee API Not Reachable. Please Try Again Later.";
                                }

                            }
                            if(loginDetails.LoginType == "Admin")
                            {
                                return RedirectToAction("SecurityDashboard", "Admin");
                            }
                        }
                    }

                }
                catch (Exception)
                {
                    ViewBag.Message = "Login API not Loaded. Please Try Later.";
                }
                return View();
            }





        }
    }
}
