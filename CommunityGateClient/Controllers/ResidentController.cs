﻿using CommunityGateClient.Models;
using CommunityGateClient.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
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
        static Residents resident=new Residents();

        string BaseurlForDashboardAPI = "http://localhost:52044/";
        string BaseurlForResidentAPI = "http://localhost:27414/";
        string BaseurlForVisitorAPI = "https://localhost:44301/";
        string BaseurlForComplaintAPI = "https://localhost:63429/";




        public IActionResult Index()
        {
            return View();
        }

        public IActionResult dash()
        {
            TempData["UserEmail"] = "resident@gmail.com";
            return View();
        }

        public async Task<IActionResult> ResidentDashboard()
        {
            TempData["UserID"] = 104;
            int UserID = Convert.ToInt32(TempData.Peek("UserID"));
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




            TempData["UserEmail"] = "resident@gmail.com";
            TempData["quantity1"] = ofa.visitors.ToList().Count();
            TempData["property1"] = "Visitors today";
            TempData["quantity2"] = ofa.complaints.ToList().Count();
            TempData["property2"] = "Unresolved Complaints";
            TempData["property3"] = "Wallet Balance";
            TempData["quantity3"] = resident.ResidentWallet;
            TempData["property4"] = "Payment Due";
            TempData["quantity4"] = ofa.payments.ToList().Count();

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
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> AddVisitor(Visitors visitors)
        {
            if (ModelState.IsValid)
            {
                TempData["UserID"] = 104;
                int UserID = Convert.ToInt32(TempData.Peek("UserID"));

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
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> UpdateVisitor(int id, Visitors visitors)
        {
            if (ModelState.IsValid)
            {
                TempData["UserID"] = 104;
                int UserID = Convert.ToInt32(TempData.Peek("UserID"));

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
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> AddNotice(DashBoardPosts dashBoardPost)
        {
            TempData["UserID"] = 104;
            int UserID = Convert.ToInt32(TempData.Peek("UserID"));
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
            return View(dashBoardPost);
        }
        [HttpPost]
        public async Task<IActionResult> UpdateNotice(int id, DashBoardPosts dashBoardPost)
        {
            TempData["UserID"] = 104;
            int UserID = Convert.ToInt32(TempData.Peek("UserID"));
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
            var model = new DashBoardPosts();
            model.DashTitle = title;
            model.DashType = type;
            model.DashIntendedFor = intendedfor;
            model.DashBody = body;

            return RedirectToAction("UpdateNotice", model);
        }
        ///////complaint
        public IActionResult AddComplaint()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> AddComplaint(Complaints complaint)
        {
            TempData["UserID"] = 104;
            int UserID = Convert.ToInt32(TempData.Peek("UserID"));
            complaint.ResidentId = UserID;
            
            if (ModelState.IsValid)
            {
                //send visitor Details
                try
                {

                    using (var client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(BaseurlForComplaintAPI);
                        StringContent content = new StringContent(JsonConvert.SerializeObject(complaint), Encoding.UTF8, "application/json");

                        client.DefaultRequestHeaders.Clear();
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        HttpResponseMessage response = await client.PostAsync("/api/Complaint", content);
                        if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                        {
                            ViewBag.Message = "Failed";
                        }
                        else
                        {
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



    }
}
