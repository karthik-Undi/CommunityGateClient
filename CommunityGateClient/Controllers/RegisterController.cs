using CommunityGateClient.Models;
using CommunityGateClient.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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
    public class RegisterController : Controller
    {

        static readonly log4net.ILog _log4net = log4net.LogManager.GetLogger(typeof(RegisterController));
        string BaseurlForHouseAPI = "http://localhost:26408/";


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
                    try
                    {

                        StringContent content = new StringContent(JsonConvert.SerializeObject(employee), Encoding.UTF8, "application/json");

                        using (var response = await httpClient.PostAsync("http://localhost:62288/api/Employees/", content))
                        {



                            if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                            {
                                ViewBag.Message = "Failed";
                            }
                            else
                            {
                                _log4net.Info("Registration Was Done With Email " + employee.EmployeeEmail);
                                return RedirectToAction("FirstEmployeeLogin", "Login", new { email = employee.EmployeeEmail, role = employee.EmployeeDept });
                            }
                        }
                    }
                    catch(Exception)
                    {
                        ViewBag.Message = "Employee API Not Reachable. Please Try Again Later.";
                    }

                }
            }
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> RegisterForResident()
        {
            _log4net.Info("Register For Resident Was Called");
            try
            {
                List<HouseList> freeHouseInfo = new List<HouseList>();
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(BaseurlForHouseAPI);

                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    HttpResponseMessage Res = await client.GetAsync("api/House");

                    if (Res.IsSuccessStatusCode)
                    {
                        var Response = Res.Content.ReadAsStringAsync().Result;

                        freeHouseInfo = JsonConvert.DeserializeObject<List<HouseList>>(Response);
                        ViewBag.freeHouseDropDownList = new SelectList(freeHouseInfo, "HouseId", "HouseId");
                    }
                    else
                    {
                        ViewBag.freeHouseDropDownList = null;
                    }

                }

            }
            catch (Exception)
            {
                ViewBag.Message = "House API Not Reachable. Please Try Again Later.";
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RegisterForResident(RegisterDetailsForResident regDetailsResident)
        {
            try
            {
                List<HouseList> freeHouseInfo = new List<HouseList>();
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(BaseurlForHouseAPI);

                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    HttpResponseMessage Res = await client.GetAsync("api/House");

                    if (Res.IsSuccessStatusCode)
                    {
                        var Response = Res.Content.ReadAsStringAsync().Result;

                        freeHouseInfo = JsonConvert.DeserializeObject<List<HouseList>>(Response);
                        ViewBag.freeHouseDropDownList = new SelectList(freeHouseInfo, "HouseId", "HouseId");
                    }
                    else
                    {
                        ViewBag.freeHouseDropDownList = null;
                    }
                }
            }
            catch (Exception)
            {
                ViewBag.Message = "House API Not Reachable. Please Try Again Later.";
            }
            if (ModelState.IsValid)
            {
                try
                {
                    using (var httpClient = new HttpClient())
                    {
                        StringContent content = new StringContent(JsonConvert.SerializeObject(regDetailsResident), Encoding.UTF8, "application/json");

                        using (var response = await httpClient.PostAsync("http://localhost:27414/api/Resident", content))
                        {
                            if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                            {
                                _log4net.Info("Registration Was Done With Email " + regDetailsResident.ResidentEmail + " But Registration Wasn't Successfull  !!");
                                ViewBag.message = "Registration Failed";

                            }
                            else
                            {
                                _log4net.Info("Registration Was Done With Email " + regDetailsResident.ResidentEmail + " And the Registration Was Successfull !!");
                                return RedirectToAction("FirstResidentLogin", "Login", new { email = regDetailsResident.ResidentEmail});
                            }
                        }

                    }
                }
                catch (Exception)
                {
                    ViewBag.Message = "Resident API Not Reachable. Please Try Again Later.";
                }
            }


            return View();
        }

    }
}
