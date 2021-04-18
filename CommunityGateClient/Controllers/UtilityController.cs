using CommunityGateClient.Models;
using CommunityGateClient.Models.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace CommunityGateClient.Controllers
{
    public class UtilityController : Controller
    {

        static readonly log4net.ILog _log4net = log4net.LogManager.GetLogger(typeof(UtilityController));
        static Employees employee = new Employees();
        string baseUrlForEmployeeAPI = "http://localhost:62288/";
        string BaseurlForDashboardAPI = "http://localhost:52044/";
        string baseUrlForFaFAPI = "http://localhost:62521/";
        string BaseurlForVisitorAPI = "https://localhost:44301/";
        static Residents resident = new Residents();
        static int postid = 0;
        string BaseurlForResidentAPI = "http://localhost:27414/";
        string BaseurlForComplaintAPI = "http://localhost:36224/";
        string baseUrlForServicesAPI = "http://localhost:41093/";
        string baseUrlForPaymentAPI = "http://localhost:27340/";
        static Services serv = new Services();


        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> UtilityDashboard()
        {
            if (HttpContext.Session.GetInt32("UserID") != null)
            {
                TempData["UserID"] = HttpContext.Session.GetInt32("UserID");

                int UserID = Convert.ToInt32(TempData.Peek("UserID"));
                _log4net.Info("Home Page was called For Security with Id " + UserID);
                //Get Resident Details
                try
                {

                    using (var client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(baseUrlForEmployeeAPI);
                        client.DefaultRequestHeaders.Clear();
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        HttpResponseMessage Res = await client.GetAsync("api/Employees/" + UserID);
                        if (Res.IsSuccessStatusCode)
                        {
                            var Response = Res.Content.ReadAsStringAsync().Result;
                            employee = JsonConvert.DeserializeObject<Employees>(Response);
                        }

                    }
                }
                catch (Exception)
                {
                    ViewBag.Message = "Employee API Not Reachable. Please Try Again Later.";
                }

                try
                {
                    TempData["UserEmail"] = employee.EmployeeEmail;
                }
                catch (Exception)
                {
                    TempData["UserEmail"] = "API not reachable";
                }

                //NoticeBoard
                try
                {
                    List<DashBoardPosts> dashBoardPostList = new List<DashBoardPosts>();
                    using (var client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(BaseurlForDashboardAPI);

                        client.DefaultRequestHeaders.Clear();
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        HttpResponseMessage Res = await client.GetAsync("api/Dashboard/");
                        var model = new List<DashBoardPosts>();
                        if (Res.IsSuccessStatusCode)
                        {
                            var Response = Res.Content.ReadAsStringAsync().Result;

                            dashBoardPostList = JsonConvert.DeserializeObject<List<DashBoardPosts>>(Response);
                            var tables = new OneForAll
                            {
                                DashboardPosts = dashBoardPostList.OrderByDescending(post => post.DashItemId).ToList()

                            };
                            return View(tables);
                        }

                    }
                }
                catch (Exception)
                {
                    ViewBag.Message = "DashBoard API Not Reachable. Please Try Again Later.";
                }
                return View();
            }
            else
                return RedirectToAction("Login", "Login");
        }



        public async Task<IActionResult> ShowServices()
        {
            List<ServiceDetails> servicedetails = new List<ServiceDetails>();
            int userid = Convert.ToInt32(TempData.Peek("UserID"));
            _log4net.Info("Show Services For Employee ID " + userid + " Was Called !!");
            try
            {

                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(baseUrlForServicesAPI);
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    HttpResponseMessage Res = await client.GetAsync("api/Services/");
                    if (Res.IsSuccessStatusCode)
                    {
                        var Response = Res.Content.ReadAsStringAsync().Result;
                        servicedetails = JsonConvert.DeserializeObject<List<ServiceDetails>>(Response);
                    }

                }
            }
            catch (Exception)
            {
                ViewBag.Message = "Services API Not Reachable. Please Try Again Later.";
            }
            return View(servicedetails.Where(x=>x.ServiceStatus=="Requested" && x.ServiceType=="Utility"));
        }

        public async Task<IActionResult> ShowAllServices()
        {
            List<ServiceDetails> servicedetails = new List<ServiceDetails>();
            int userid = Convert.ToInt32(TempData.Peek("UserID"));
            _log4net.Info("Show Services For Employee ID " + userid + " Was Called !!");
            try
            {

                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(baseUrlForServicesAPI);
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    HttpResponseMessage Res = await client.GetAsync("api/Services/");
                    if (Res.IsSuccessStatusCode)
                    {
                        var Response = Res.Content.ReadAsStringAsync().Result;
                        servicedetails = JsonConvert.DeserializeObject<List<ServiceDetails>>(Response);
                    }

                }
            }
            catch (Exception)
            {
                ViewBag.Message = "Services API Not Reachable. Please Try Again Later.";
            }
            return View(servicedetails.Where(x => x.ServiceType == "Utility"));
        }


        public IActionResult AcceptRequest(int id)
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> AcceptRequest(int id, Services service)
        {
            int userid = Convert.ToInt32(TempData.Peek("UserID"));
            service.ServiceStatus = "Accepted";
            service.EmployeeId = userid;
            service.ServiceId = id;
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(baseUrlForServicesAPI);
                    StringContent content = new StringContent(JsonConvert.SerializeObject(service), Encoding.UTF8, "application/json");

                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    HttpResponseMessage response = await client.PostAsync("/api/Services/EmployeeItem/" + id, content);
                    if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                    {
                        ViewBag.Message = "Failed";
                    }
                    else
                    {
                        _log4net.Info(" employee With ID " + userid + " Accepted request with id  " + id);
                        serv = service;
                        return RedirectToAction("ShowAllServices");
                    }

                }

            }
            catch (Exception)
            {
                ViewBag.Message = "Service API Not Reachable. Please Try Again Later.";
            }

            return RedirectToAction("ShowServices");
        }
        static int resid = 0;
        public IActionResult RequestPayment(int id,int residentid)
        {
            resid = residentid;
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> RequestPayment(int id, Payments payment)
        {
            int userid = Convert.ToInt32(TempData.Peek("UserID"));
            payment.EmployeeId=userid;
            payment.PaymentStatus ="Requested";
            payment.ServiceId = id;
            payment.PaymentFor = "Utility";
            payment.ResidentId = resid;
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(baseUrlForPaymentAPI);
                    StringContent content = new StringContent(JsonConvert.SerializeObject(payment), Encoding.UTF8, "application/json");

                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    HttpResponseMessage response = await client.PostAsync("/api/Payments", content);
                    if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                    {
                        ViewBag.Message = "Failed";
                    }
                    else
                    {
                        _log4net.Info(" employee With ID " + userid + " Accepted request with id  " + id);
                        //return RedirectToAction("ShowAllServices");
                    }

                }

            }
            catch (Exception)
            {
                ViewBag.Message = "Service API Not Reachable. Please Try Again Later.";
            }
            Services temp = serv;
            temp.ServiceStatus = "Requested Payment";
            temp.ServiceId = id;
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(baseUrlForServicesAPI);
                    StringContent content = new StringContent(JsonConvert.SerializeObject(temp), Encoding.UTF8, "application/json");

                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    HttpResponseMessage response = await client.PostAsync("/api/Services/ServiceStatus/" + id, content);
                    if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                    {
                        ViewBag.Message = "Failed";
                    }
                    else
                    {
                        _log4net.Info(" employee With ID " + userid + " Accepted request with id  " + id);
                        return RedirectToAction("ShowAllServices");
                    }

                }

            }
            catch (Exception)
            {
                ViewBag.Message = "Service API Not Reachable. Please Try Again Later.";
            }


            return RedirectToAction("ShowServices");
        }

        public async Task<IActionResult> EmployeeWallet()
        {
            Employees employees = new Employees();
            _log4net.Info("Recharge Wallet and View Payment History Was Called !!");
         
            //TempData["UserID"] = 1011;
            int UserID = Convert.ToInt32(TempData.Peek("UserID"));

            try
            {
                using (var clientForEmployeeDetails = new HttpClient())
                {
                    clientForEmployeeDetails.BaseAddress = new Uri(baseUrlForEmployeeAPI);
                    clientForEmployeeDetails.DefaultRequestHeaders.Clear();
                    clientForEmployeeDetails.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    HttpResponseMessage Res = await clientForEmployeeDetails.GetAsync("api/Employees/" + UserID);
                    if (Res.IsSuccessStatusCode)
                    {
                        var Response = Res.Content.ReadAsStringAsync().Result;
                        employees = JsonConvert.DeserializeObject<Employees>(Response);

                    }

                }
            }
            catch (Exception)
            {
                ViewBag.Message = "Employee API Not Reachable. Please Try Again Later.";
            }


            return View(employees);
        }
        [HttpPost]
        public async Task<IActionResult> EmployeeWallet(Employees e)
        {
            Employees employees = new Employees();
            _log4net.Info("Recharge Wallet and View Payment History Was Called !!");

            //TempData["UserID"] = 1011;
            int UserID = Convert.ToInt32(TempData.Peek("UserID"));

            try
            {
                using (var clientForEmployeeDetails = new HttpClient())
                {
                    clientForEmployeeDetails.BaseAddress = new Uri(baseUrlForEmployeeAPI);
                    clientForEmployeeDetails.DefaultRequestHeaders.Clear();
                    clientForEmployeeDetails.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    HttpResponseMessage Res = await clientForEmployeeDetails.GetAsync("api/Employees/" + UserID);
                    if (Res.IsSuccessStatusCode)
                    {
                        var Response = Res.Content.ReadAsStringAsync().Result;
                        employees = JsonConvert.DeserializeObject<Employees>(Response);

                    }

                }
            }
            catch (Exception)
            {
                ViewBag.Message = "Employee API Not Reachable. Please Try Again Later.";
            }

            employees.EmployeeWallet = employees.EmployeeWallet - e.EmployeeWallet;
            try
            {
                using (var clientToUpdateEmployeeWallet = new HttpClient())
                {
                    clientToUpdateEmployeeWallet.BaseAddress = new Uri(baseUrlForEmployeeAPI);
                    StringContent content = new StringContent(JsonConvert.SerializeObject(employees), Encoding.UTF8, "application/json");
                    clientToUpdateEmployeeWallet.DefaultRequestHeaders.Clear();
                    clientToUpdateEmployeeWallet.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    HttpResponseMessage Res = await clientToUpdateEmployeeWallet.PostAsync("api/Employees/UpdateEmployeeWallet/" + UserID, content);
                    if (Res.StatusCode == System.Net.HttpStatusCode.BadRequest)
                    {
                        ViewBag.Message = "Failed";
                    }
                    else
                    {
                        _log4net.Info("Wallet of Employee With ID " + UserID + " Was Updated !!");

                    }
                }
            }
            catch (Exception)
            {
                ViewBag.Message = "Employee API Not Reachable. Please Try Again Later.";
            }



            return View(employees);
        }
    }
}
