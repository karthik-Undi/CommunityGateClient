﻿using CommunityGateClient.Models;
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
    public class ResidentController : Controller
    {
        string BaseurlForDashboardAPI = "http://localhost:52044/";

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
            //AtAGlance
            TempData["UserEmail"] = "resident@gmail.com";
            TempData["quantity1"] = 4;
            TempData["property1"] = "Visitors today";
            TempData["quantity2"] = 2;
            TempData["property2"] = "Unresolved Complaints";
            TempData["property3"] = "Wallet Balance";
            TempData["quantity3"] = 4000;
            TempData["property4"] = "Payment Due";
            TempData["quantity4"] = 5;

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



    }
}