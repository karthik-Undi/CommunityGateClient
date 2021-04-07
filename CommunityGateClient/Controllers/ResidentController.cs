using CommunityGateClient.Models;
using CommunityGateClient.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ServicesAPI.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Providers.Entities;

namespace CommunityGateClient.Controllers
{
    public class ResidentController : Controller
    {
        static readonly log4net.ILog _log4net = log4net.LogManager.GetLogger(typeof(ResidentController));
        static Residents resident=new Residents();
        static int postid=0;
        string BaseurlForDashboardAPI = "http://localhost:52044/";
        string BaseurlForResidentAPI = "http://localhost:27414/";
        string BaseurlForVisitorAPI = "https://localhost:44301/";
        string BaseurlForComplaintAPI = "http://localhost:36224/";
        string baseUrlForServicesAPI = "http://localhost:41093/";
        string baseUrlForFaFAPI = "http://localhost:62521/";




        public IActionResult Index()
        {
            return View();
        }


        public async Task<IActionResult> ResidentDashboard()
        {
            TempData["UserID"] = 104;
            
            int UserID = Convert.ToInt32(TempData.Peek("UserID"));
            _log4net.Info("Home Page was called For Resident with Id " + UserID);
            //Get Resident Details
            try
            {

                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(BaseurlForResidentAPI);
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    HttpResponseMessage Res = await client.GetAsync("/api/Resident/" + UserID);
                    if (Res.IsSuccessStatusCode)
                    {
                        var Response = Res.Content.ReadAsStringAsync().Result;
                        resident = JsonConvert.DeserializeObject<Residents>(Response);
                    }

                }
            }
            catch (Exception)
            {
                ViewBag.Message = "DashBoard API Not Reachable. Please Try Again Later.";
            }



            //AtAGlance
            OneForAll ofa = new OneForAll();
            try
            {

                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(BaseurlForResidentAPI);
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    HttpResponseMessage Res = await client.GetAsync("/api/Resident/AtAGlance/"+ resident.ResidentId);
                    if (Res.IsSuccessStatusCode)
                    {
                        var Response = Res.Content.ReadAsStringAsync().Result;
                        ofa = JsonConvert.DeserializeObject<OneForAll>(Response);
                    }

                }
            }
            catch (Exception)
            {
                ViewBag.Message = "DashBoard API Not Reachable. Please Try Again Later.";
            }



            try
            {
                TempData["UserEmail"] = "resident@gmail.com";
                TempData["quantity1"] = ofa.visitors.ToList().Count();
                TempData["property1"] = "Visitors today";
                TempData["quantity2"] = ofa.complaints.ToList().Count();
                TempData["property2"] = "Unresolved Complaints";
                TempData["property3"] = "Wallet Balance";
                TempData["quantity3"] = resident.ResidentWallet;
                TempData["property4"] = "Payment Due";
                TempData["quantity4"] = ofa.payments.ToList().Count();
            }
            catch(Exception)
            {
                TempData["UserEmail"] = "resident@gmail.com";
                TempData["quantity1"] = "API not reachable";
                TempData["property1"] = "Visitors today";
                TempData["quantity2"] = "API not reachable";
                TempData["property2"] = "Unresolved Complaints";
                TempData["property3"] = "Wallet Balance";
                TempData["quantity3"] = "API not reachable";
                TempData["property4"] = "Payment Due";
                TempData["quantity4"] = "API not reachable";
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

        public IActionResult AddVisitor()
        {
            _log4net.Info("Add Visitor Was Called !!");
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> AddVisitor(Visitors visitors)
        {
            if (ModelState.IsValid)
            {
                TempData["UserID"] = 104;
                int UserID = Convert.ToInt32(TempData.Peek("UserID"));
                _log4net.Info("Visitor For Resident With ID " +UserID+ " Was Added!!");
                //send visitor Details
                visitors.ResidentId = UserID;
                try
                {

                    using (var client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(BaseurlForVisitorAPI);
                        StringContent content = new StringContent(JsonConvert.SerializeObject(visitors), Encoding.UTF8, "application/json");

                        client.DefaultRequestHeaders.Clear();
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        HttpResponseMessage response = await client.PostAsync("/api/Visitor", content);
                        if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                        {
                            ViewBag.Message = "Failed";
                        }
                        else
                        {
                            _log4net.Info("Visitor with name " + visitors.VisitorName + " Was Added !!");
                            return RedirectToAction("ResidentDashboard");
                        }

                    }
                }
                catch (Exception)
                {
                    ViewBag.Message = "DashBoard API Not Reachable. Please Try Again Later.";
                }
            }
            return View();
        }

        public async Task<IActionResult> ShowVisitors()
        {
            
            List<Visitors> visitors = new List<Visitors>();
           int residentId = Convert.ToInt32(TempData.Peek("UserID"));
            _log4net.Info("Show Visitors For Resident With ID "+residentId+ " Was Called !!");
            try
            {

                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(BaseurlForVisitorAPI);
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    HttpResponseMessage Res = await client.GetAsync("/api/Visitor/" +residentId);
                    if (Res.IsSuccessStatusCode)
                    {
                        var Response = Res.Content.ReadAsStringAsync().Result;
                        visitors = JsonConvert.DeserializeObject<List<Visitors>>(Response);
                    }

                }
            }
            catch (Exception)
            {
                ViewBag.Message = "DashBoard API Not Reachable. Please Try Again Later.";
            }
            return View(visitors);
        }

        public IActionResult UpdateVisitor()
        {
            _log4net.Info("Update Visitor Was Called !!");
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> UpdateVisitor(int id, Visitors visitors)
        {
            if (ModelState.IsValid)
            {
                TempData["UserID"] = 104;
                int UserID = Convert.ToInt32(TempData.Peek("UserID"));
                _log4net.Info("Update Visitor For Resident With ID "+UserID+" Was Called !!");
                //send visitor Details
                visitors.ResidentId = UserID;
                try
                {

                    using (var client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(BaseurlForVisitorAPI);
                        StringContent content = new StringContent(JsonConvert.SerializeObject(visitors), Encoding.UTF8, "application/json");
                     
                        client.DefaultRequestHeaders.Clear();
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        HttpResponseMessage response = await client.PostAsync("/api/Visitor/"+id, content);
                        if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                        {
                            ViewBag.Message = "Failed";
                        }
                        else
                        {
                            _log4net.Info("Visitor with Id " + id + " Was Updated !!");
                            return RedirectToAction("ShowVisitors");
                        }

                    }
                }
                catch (Exception)
                {
                    ViewBag.Message = "DashBoard API Not Reachable. Please Try Again Later.";
                }
            }
            return View();
        }
        public IActionResult DeleteVisitor()
        {
            _log4net.Info("Delete Visitor Was Called !!");
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> DeleteVisitor(int id)
        {
            try
            {

                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(BaseurlForVisitorAPI);

                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    HttpResponseMessage response = await client.DeleteAsync("/api/Visitor/RemoveVisitor/"+ id);
                    if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                    {
                        ViewBag.Message = "Failed";
                    }
                    else
                    {
                        _log4net.Info("Visitor With ID " + id + " Was Deleted !!");
                        return RedirectToAction("ShowVisitors");
                    }

                }
            }
            catch (Exception)
            {
                ViewBag.Message = "DashBoard API Not Reachable. Please Try Again Later.";
            }
            return View();
        }
        ///////////////////////////////////////////////////////////////////////////
        public IActionResult AddNotice()
        {
            _log4net.Info("Add Notice Was Called !!");
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> AddNotice(DashBoardPosts dashBoardPost)
        {
            TempData["UserID"] = 104;
            int UserID = Convert.ToInt32(TempData.Peek("UserID"));
            _log4net.Info("Add Notice For Resident With Id " + UserID + " Was Called !!");
            dashBoardPost.ResidentId = UserID;
            dashBoardPost.DashTime = DateTime.Now;
            dashBoardPost.ResidentName = resident.ResidentName;
            if (ModelState.IsValid)
            {
                //send visitor Details
                dashBoardPost.ResidentId = UserID;
                try
                {

                    using (var client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(BaseurlForDashboardAPI);
                        StringContent content = new StringContent(JsonConvert.SerializeObject(dashBoardPost), Encoding.UTF8, "application/json");

                        client.DefaultRequestHeaders.Clear();
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        HttpResponseMessage response = await client.PostAsync("/api/Dashboard", content);
                        if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                        {
                            ViewBag.Message = "Failed";
                        }
                        else
                        {
                            _log4net.Info("Dashboard With Title " + dashBoardPost.DashTitle + " Was Added !!");
                            return RedirectToAction("ResidentDashboard");
                        }

                    }
                }
                catch (Exception)
                {
                    ViewBag.Message = "DashBoard API Not Reachable. Please Try Again Later.";
                }
            }
            return View();
        }
        /////////////////////////////////////////

        public async Task<IActionResult> ShowPosts()
        {
            try
            {
                List<DashBoardPosts> dashBoardPostList = new List<DashBoardPosts>();
                int UserID = Convert.ToInt32(TempData.Peek("UserID"));
                _log4net.Info("Show Posts for Resident With ID " + UserID + " Was Called !!");
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(BaseurlForDashboardAPI);

                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    HttpResponseMessage Res = await client.GetAsync("api/Dashboard/GetPostsByResidentId/" +UserID);
                    var model = new List<DashBoardPosts>();
                    if (Res.IsSuccessStatusCode)
                    {
                        var Response = Res.Content.ReadAsStringAsync().Result;

                        dashBoardPostList = JsonConvert.DeserializeObject<List<DashBoardPosts>>(Response);
                        var postlist = new List<DashBoardPosts>();
                        postlist = dashBoardPostList.OrderByDescending(post => post.DashItemId).ToList();
                        return View(postlist);
                    }

                }
            }
            catch (Exception)
            {
                ViewBag.Message = "DashBoard API Not Reachable. Please Try Again Later.";
            }
            return View();
        }

        public IActionResult UpdateNotice(DashBoardPosts dashBoardPost)
        {
            _log4net.Info("Update Notice Was Called !!");
            return View(dashBoardPost);
        }
        [HttpPost]
        public async Task<IActionResult> UpdateNotice(int id, DashBoardPosts dashBoardPost)
        {
            id = postid;
            TempData["UserID"] = 104;
            int UserID = Convert.ToInt32(TempData.Peek("UserID"));
            _log4net.Info("Update Notice For Resident With ID " + UserID + " Was Called !!");
            dashBoardPost.ResidentId = UserID;
            dashBoardPost.DashTime = DateTime.Now;
            dashBoardPost.DashIntendedFor = dashBoardPost.DashIntendedFor + " (Edited)";
            if (ModelState.IsValid)
            {
                //send visitor Details
                try
                {

                    using (var client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(BaseurlForDashboardAPI);
                        StringContent content = new StringContent(JsonConvert.SerializeObject(dashBoardPost), Encoding.UTF8, "application/json");

                        client.DefaultRequestHeaders.Clear();
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        HttpResponseMessage response = await client.PostAsync("/api/Dashboard/" + id, content);
                        if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                        {
                            ViewBag.Message = "Failed";
                        }
                        else
                        {
                            _log4net.Info("Dahboard Post With ID " + id + " Was Updated !!");
                            return RedirectToAction("ResidentDashboard");
                        }

                    }
                }
                catch (Exception)
                {
                    ViewBag.Message = "DashBoard API Not Reachable. Please Try Again Later.";
                }
            }
            return View(dashBoardPost);
        }

        ////////////
        public IActionResult DeleteNotice()
        {
            _log4net.Info("Delete Notice Was Called !!");
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> DeleteNotice(int id)
        {
            try
            {

                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(BaseurlForDashboardAPI);

                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    HttpResponseMessage response = await client.DeleteAsync("/api/Dashboard/delete/" + id);
                    if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                    {
                        ViewBag.Message = "Failed";
                    }
                    else
                    {
                        _log4net.Info("Dashboard Post With Id " + id + " Was Deleted !!");
                        return RedirectToAction("ShowPosts");
                    }

                }
            }
            catch (Exception)
            {
                ViewBag.Message = "DashBoard API Not Reachable. Please Try Again Later.";
            }
            return View();
        }
        public IActionResult UpdatePostAutofill(int id,string title, string type,string intendedfor,string body)
        {
            postid = id;
            var model = new DashBoardPosts();
            model.DashItemId = id;
            model.DashTitle = title;
            model.DashType = type;
            model.DashIntendedFor = intendedfor;
            model.DashBody = body;

            return RedirectToAction("UpdateNotice", model);
        }
        ///////complaint
        public IActionResult AddComplaint()
        {
            _log4net.Info("Add Complaint Was Called !!");
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> AddComplaint(Complaints complaint)
        {
            TempData["UserID"] = 104;
            int UserID = Convert.ToInt32(TempData.Peek("UserID"));
            _log4net.Info("Add Complaint for Resident With ID " + UserID + " Was Called !!");
            complaint.ResidentId = UserID;
            
            if (ModelState.IsValid)
            {
                try
                {

                    using (var client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(BaseurlForComplaintAPI);
                        StringContent content = new StringContent(JsonConvert.SerializeObject(complaint), Encoding.UTF8, "application/json");

                        client.DefaultRequestHeaders.Clear();
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        HttpResponseMessage response = await client.PostAsync("/api/Complaints", content);
                        if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                        {
                            ViewBag.Message = "Failed";
                        }
                        else
                        {
                            _log4net.Info("Complaint Reagrding " + complaint.ComplaintRegarding + " Was Registered !!");
                            return RedirectToAction("ShowComplaints");
                        }

                    }
                }
                catch (Exception)
                {
                    ViewBag.Message = "Complaint API Not Reachable. Please Try Again Later.";
                }
            }
            return View();
        }
        public async Task<IActionResult> ShowComplaints()
        {
            List<Complaints> complaints = new List<Complaints>();
            int residentId = Convert.ToInt32(TempData.Peek("UserID"));
            _log4net.Info("Show Complaints For Resident ID " + residentId + " Was Called !!");
            try
            {

                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(BaseurlForComplaintAPI);
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    HttpResponseMessage Res = await client.GetAsync("api/Complaints/GetComplaintByResidentId/" + residentId);
                    if (Res.IsSuccessStatusCode)
                    {
                        var Response = Res.Content.ReadAsStringAsync().Result;
                        complaints = JsonConvert.DeserializeObject<List<Complaints>>(Response);
                    }

                }
            }
            catch (Exception e)
            {
                ViewBag.Message = "Complaints API Not Reachable. Please Try Again Later.";
            }
            return View(complaints);
        }

        public IActionResult AddService()
        {
            _log4net.Info("Add Service Was Called !!");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddService(Services services)
        {
            TempData["UserID"] = 104;
            int UserID = Convert.ToInt32(TempData.Peek("UserID"));
            _log4net.Info("Add Service for Resident With ID " + UserID + " Was Called !!");
            services.ResidentId = UserID;

            if (ModelState.IsValid)
            {
                try
                {

                    using (var client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(baseUrlForServicesAPI);
                        StringContent content = new StringContent(JsonConvert.SerializeObject(services), Encoding.UTF8, "application/json");

                        client.DefaultRequestHeaders.Clear();
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        HttpResponseMessage response = await client.PostAsync("/api/Services", content);
                        if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                        {
                            ViewBag.Message = "Failed";
                        }
                        else
                        {
                            _log4net.Info("Service Type " + services.ServiceType + " Was Registered !!");
                            return RedirectToAction("ShowServices");
                        }

                    }
                }
                catch (Exception)
                {
                    ViewBag.Message = "Service API Not Reachable. Please Try Again Later.";
                }
            }
            return View();
        }

        public async Task<IActionResult> ShowServices()
        {
            List<ServiceDetails> servicedetails = new List<ServiceDetails>();
            int residentId = Convert.ToInt32(TempData.Peek("UserID"));
            _log4net.Info("Show Services For Resident ID " + residentId + " Was Called !!");
            try
            {

                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(baseUrlForServicesAPI);
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    HttpResponseMessage Res = await client.GetAsync("api/Services/" + residentId);
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
            return View(servicedetails);
        }

        //////////////////////////FaF
        public IActionResult AddFaF()
        {
            _log4net.Info("Add FoF Was Called !!");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddFaF(FriendsAndFamily faf)
        {
            TempData["UserID"] = 104;
            int UserID = Convert.ToInt32(TempData.Peek("UserID"));
            _log4net.Info("Add friends and family for Resident With ID " + UserID + " Was Called !!");
            faf.ResidentId = UserID;

            if (ModelState.IsValid)
            {
                try
                {

                    using (var client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(baseUrlForFaFAPI);
                        StringContent content = new StringContent(JsonConvert.SerializeObject(faf), Encoding.UTF8, "application/json");

                        client.DefaultRequestHeaders.Clear();
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        HttpResponseMessage response = await client.PostAsync("/api/FaF", content);
                        if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                        {
                            ViewBag.Message = "Failed";
                        }
                        else
                        {
                            _log4net.Info("friend/family  " + faf.FaFname + " Was Registered !!");
                            return RedirectToAction("ShowFaF");
                        }

                    }
                }
                catch (Exception)
                {
                    ViewBag.Message = "FriendsandFamilyAPI Not Reachable. Please Try Again Later.";
                }
            }
            return View();
        }
        public async Task<IActionResult> ShowFaF()
        {
            List<FriendsAndFamily> friendsAndFamilies = new List<FriendsAndFamily>();
            int residentId = Convert.ToInt32(TempData.Peek("UserID"));
            _log4net.Info("friends and family list For Resident ID " + residentId + " Was Called !!");
            try
            {

                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(baseUrlForFaFAPI);
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    HttpResponseMessage Res = await client.GetAsync("api/FaF/" + residentId);
                    if (Res.IsSuccessStatusCode)
                    {
                        var Response = Res.Content.ReadAsStringAsync().Result;
                        friendsAndFamilies = JsonConvert.DeserializeObject<List<FriendsAndFamily>>(Response);
                    }

                }
            }
            catch (Exception)
            {
                ViewBag.Message = "Services API Not Reachable. Please Try Again Later.";
            }
            return View(friendsAndFamilies);
        }
        ////////////////////Wallet
        public IActionResult RechargeWallet()
        {
            _log4net.Info("Add FoF Was Called !!");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RechargeWallet(Residents res)
        {
            TempData["UserID"] = 104;
            int UserID = Convert.ToInt32(TempData.Peek("UserID"));
            _log4net.Info("Add friends and family for Resident With ID " + UserID + " Was Called !!");
            resident.ResidentWallet = resident.ResidentWallet +res.ResidentWallet;
            if (ModelState.IsValid)
            {
                try
                {

                    using (var client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(BaseurlForResidentAPI);
                        StringContent content = new StringContent(JsonConvert.SerializeObject(resident), Encoding.UTF8, "application/json");

                        client.DefaultRequestHeaders.Clear();
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        HttpResponseMessage response = await client.PostAsync("/api/Resident/" + resident.ResidentId, content);
                        if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                        {
                            ViewBag.Message = "Failed";
                        }
                        else
                        {
                            var data = response.Content.ReadAsStringAsync().Result;
                            Residents tempres = JsonConvert.DeserializeObject<Residents>(data);
                            _log4net.Info(" new wallet balance  " + tempres.ResidentWallet + " Was updated !!");
                            return RedirectToAction("ResidentDashboard");
                        }

                    }
                }
                catch (Exception)
                {
                    ViewBag.Message = "FriendsandFamilyAPI Not Reachable. Please Try Again Later.";
                }
            }
            return View();
        }
    }
}
