using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NPLDataAccessLayer.DataTransferObjects;
using NPLReusableResourcesPackage.ErrorHandlingContainer;
using NPLReusableResourcesPackage.General;
using NPLSubscriptionServiceWebApp.Models.General;
using NPLSubscriptionServiceWebApp.Models.ViewModels;
using System.Net;

namespace NPLSubscriptionServiceWebApp.Controllers
{
    public class DistrictController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly string apiUrl = "District";
        public string BaseUrl
        {
            get
            {
                return _configuration["EndpointUrl"];
            }
        }
        public DistrictController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task<IActionResult> Index(string message = "")
        {
            DistrictViewModel viewModel = new DistrictViewModel();

            try
            {
                viewModel.OutputHandler = new OutputHandler { IsErrorOccured = false };
               
                //Get Client Type through API end Point
                var requestUrl = $"{BaseUrl}{apiUrl}/GetAllDistricts";
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(requestUrl);
                    HttpResponseMessage responseMessage = await client.GetAsync(requestUrl);

                    //check status, if OK, all went well continue to converting from Json to ViewModel Data Transfer Object for rendering to view 
                    if (responseMessage.StatusCode == HttpStatusCode.OK)
                    {
                        string data = await responseMessage.Content.ReadAsStringAsync();

                        //Json to DTO convertion using Newtonsoft.Json
                        viewModel.Districts = JsonConvert.DeserializeObject<IEnumerable<DistrictDTO>>(data);

                    }
                    else if (responseMessage.StatusCode == HttpStatusCode.NotFound)
                    {
                        //if the database doesn't have values, return message to user
                        viewModel.OutputHandler = new OutputHandler { IsErrorOccured = false, Message = "No records found" };
                        return View(viewModel);
                    }

                };
            }
            catch (Exception ex)
            {
                //in an event of an exception return General error

                var error = StandardMessages.getExceptionMessage(ex); //variable to avoid initialization/Instance related errors
                viewModel.OutputHandler.Message = error.Message;
                viewModel.OutputHandler.IsErrorOccured = true ;
                 
                return View(viewModel);
            }
            if (!String.IsNullOrEmpty(message))
            {
                viewModel.OutputHandler.Message = message;

            }
            return View(viewModel);

        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            //Populate dropDown List
            var viewModel = new DistrictViewModel
            {
                OutputHandler = new OutputHandler { IsErrorOccured = false },
                Countries = await StaticDataHandler.GetCountries(BaseUrl)   
            };

            return View(viewModel);
        }


        [HttpPost]
        public async Task<IActionResult> Create(DistrictViewModel districtViewModel)
        {
            OutputHandler result = new();

            //capture Created Date = the time this item was/is created
            districtViewModel.District.DateCreated = DateTime.Now.AddHours(2);
            districtViewModel.District.CreatedBy = "SYSADMIN"; //add session user's Email

            var requestUrl = $"{BaseUrl}{apiUrl}/Create";
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(requestUrl);
                client.BaseAddress = new Uri(requestUrl);
                HttpResponseMessage responseMessage = await client.PostAsJsonAsync(requestUrl, districtViewModel.District);

                if (responseMessage.StatusCode == HttpStatusCode.OK)
                {
                    var data = await responseMessage.Content.ReadAsStringAsync();
                    result = JsonConvert.DeserializeObject<OutputHandler>(data);
                    return RedirectToAction("Index", "District", new { message = result.Message });
                }
                else
                {
                    //an error has occured, prep the UI and send user message
                    var data = await responseMessage.Content.ReadAsStringAsync();
                    result = JsonConvert.DeserializeObject<OutputHandler>(data);
                    districtViewModel.OutputHandler = result;

                    //populate the dropdown for reload
                    districtViewModel.Countries = await StaticDataHandler.GetCountries(BaseUrl);

                     return View(districtViewModel);
                }
            }

        }
        [HttpGet]
        public async Task<IActionResult> Update(int districtId)
        {
            //Setup Dropdown lists  
            var districtVm = new DistrictViewModel
            {
                Countries = await StaticDataHandler.GetCountries(BaseUrl),
                OutputHandler = new OutputHandler { IsErrorOccured = false }
            };
            try
            {
                //Get District through API end Point
                var requestUrl = $"{BaseUrl}{apiUrl}/GetDistrict?DistrictId={districtId}";
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(requestUrl);
                    HttpResponseMessage responseMessage = await client.GetAsync(requestUrl);

                    //check status, if OK, all went well continue to converting from Json to ViewModel Data Transfer Object for rendering to view 
                    if (responseMessage.StatusCode == HttpStatusCode.OK)
                    {
                        string data = await responseMessage.Content.ReadAsStringAsync();

                        //Json to DTO convertion using Newtonsoft.Json
                        districtVm.District = JsonConvert.DeserializeObject<DistrictDTO>(data);
                        

                    }
                    else if (responseMessage.StatusCode == HttpStatusCode.NotFound)
                    {
                        //if the database doesn't have values, return message to user
                        districtVm.OutputHandler = new OutputHandler { IsErrorOccured = false, Message = "No records found" };
                        return View(districtVm);
                    }
                };
            }
            catch (Exception ex)
            {
                //in an event of an exception return General error

                var error = StandardMessages.getExceptionMessage(ex); //variable to avoid initialization/Instance related errors
                districtVm.OutputHandler = new OutputHandler
                {
                    IsErrorOccured = true,
                    Message = error.Message
                };
            }

            districtVm.Countries = await StaticDataHandler.GetCountries(BaseUrl);
            return View(districtVm);
        }

        [HttpPost]
        public async Task<IActionResult> Update(DistrictViewModel districtViewModel)
        {
            OutputHandler result = new();

            //capture Modified Date = the time this item was modified/changed
            //districtViewModel.District.ModifiedDate = DateTime.Now.AddHours(2);
            //districtViewModel.District.ModifiedBy = "SYSADMIN"; //add session user's Email

            var requestUrl = $"{BaseUrl}{apiUrl}/Update";

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(requestUrl);
                HttpResponseMessage responseMessage = await client.PutAsJsonAsync(client.BaseAddress, districtViewModel.District);
                if (responseMessage.StatusCode == HttpStatusCode.OK)
                {
                    var data = await responseMessage.Content.ReadAsStringAsync();
                    result = JsonConvert.DeserializeObject<OutputHandler>(data);
                    return RedirectToAction("Index", "District", new { message = result.Message });
                }
                else
                {
                    //an error has occured, prep the UI and send user message
                    var data = await responseMessage.Content.ReadAsStringAsync();
                    result = JsonConvert.DeserializeObject<OutputHandler>(data);
                    if (result.Message == null)
                    {
                        districtViewModel.OutputHandler = new OutputHandler
                        {
                            IsErrorOccured = true,
                            Message = StandardMessages.GetGeneralErrorMessage()
                        };
                    }
                    else
                    {
                        districtViewModel.OutputHandler = result;
                    }


                    //populate the dropdown for reload
                   
                    return View(districtViewModel);
                }
            }
        }
        public async Task<IActionResult> Delete(int id)
        {
            OutputHandler resultHandler = new();
            var requestUrl = $"{BaseUrl}{apiUrl}/Delete?districtId={id}";
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(requestUrl);
                HttpResponseMessage responseMessage = await client.DeleteAsync(requestUrl);

                if (responseMessage.StatusCode == HttpStatusCode.OK)
                {
                    string data = await responseMessage.Content.ReadAsStringAsync();
                    resultHandler = JsonConvert.DeserializeObject<OutputHandler>(data);
                    return RedirectToAction("Index");
                }
                else if (responseMessage.StatusCode == HttpStatusCode.BadRequest)
                {
                    string data = await responseMessage.Content.ReadAsStringAsync();
                    resultHandler = JsonConvert.DeserializeObject<OutputHandler>(data);
                    return RedirectToAction("Index", new { message = resultHandler.Message });
                }
            }
            return RedirectToAction("Index");
        }
    }
}
