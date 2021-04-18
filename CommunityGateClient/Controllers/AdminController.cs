using CommunityGateClient.Models;
using CommunityGateClient.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace CommunityGateClient.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        static readonly log4net.ILog _log4net = log4net.LogManager.GetLogger(typeof(AdminController));
        static Employees employee = new Employees();
        string baseUrlForEmployeeAPI = "http://localhost:62288/";
        string BaseurlForDashboardAPI = "http://localhost:52044/";
        string baseUrlForFaFAPI = "http://localhost:62521/";
        string BaseurlForVisitorAPI = "https://localhost:44301/";

        public async Task<IActionResult> SecurityDashboard()
        {
            TempData["UserID"] = 1002;

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

        public async Task<IActionResult> ViewAllFaF()
        {

            List<FafDetails> familyAndFriends = new List<FafDetails>();
            _log4net.Info("Show all Family and Friends Was Called !!");
            try
            {

                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(baseUrlForFaFAPI);
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    HttpResponseMessage Res = await client.GetAsync("api/Faf/");
                    if (Res.IsSuccessStatusCode)
                    {
                        var Response = Res.Content.ReadAsStringAsync().Result;
                        familyAndFriends = JsonConvert.DeserializeObject<List<FafDetails>>(Response);
                    }

                }
            }
            catch (Exception)
            {
                ViewBag.Message = "Family and Friends API Not Reachable. Please Try Again Later.";
            }
            return View(familyAndFriends);
        }

        public async Task<IActionResult> ViewAllVisitors()
        {

            List<VisitorDetails> visitors = new List<VisitorDetails>();
            _log4net.Info("Show all Visitors Was Called !!");
            try
            {

                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(BaseurlForVisitorAPI);
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    HttpResponseMessage Res = await client.GetAsync("api/Visitor/");
                    if (Res.IsSuccessStatusCode)
                    {
                        var Response = Res.Content.ReadAsStringAsync().Result;
                        visitors = JsonConvert.DeserializeObject<List<VisitorDetails>>(Response);
                    }

                }
            }
            catch (Exception)
            {
                ViewBag.Message = "Visitors API Not Reachable. Please Try Again Later.";
            }
            return View(visitors);
        }
        public async Task<IActionResult> ViewAllEmployees()
        {

            List<Employees> employees = new List<Employees>();
            _log4net.Info("Show all Employees Was Called !!");
            try
            {

                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(baseUrlForEmployeeAPI);
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    HttpResponseMessage Res = await client.GetAsync("api/Employees/");
                    if (Res.IsSuccessStatusCode)
                    {
                        var Response = Res.Content.ReadAsStringAsync().Result;
                        employees = JsonConvert.DeserializeObject<List<Employees>>(Response);
                    }

                }
            }
            catch (Exception)
            {
                ViewBag.Message = "Employees API Not Reachable. Please Try Again Later.";
            }
            return View(employees.Where(emp => emp.EmployeeId != 1014));
        }

        public async Task<IActionResult> ShowNonApprovedEmployees()
        {

            List<Employees> employees = new List<Employees>();
            _log4net.Info("Show all Employees Was Called !!");
            try
            {

                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(baseUrlForEmployeeAPI);
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    HttpResponseMessage Res = await client.GetAsync("api/Employees/");
                    if (Res.IsSuccessStatusCode)
                    {
                        var Response = Res.Content.ReadAsStringAsync().Result;
                        employees = JsonConvert.DeserializeObject<List<Employees>>(Response);
                    }

                }
            }
            catch (Exception)
            {
                ViewBag.Message = "Employees API Not Reachable. Please Try Again Later.";
            }
            return View(employees.Where(emp => emp.IsApproved=="Not Approved"));
        }



    }
}
