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
    public class RegionController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly string apiUrl = "Region";
        public string BaseUrl
        {
            get
            {
                return _configuration["EndpointUrl"];
            }
        }
        public RegionController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task<IActionResult> Index(string message = "")
        {
            RegionViewModel viewModel = new RegionViewModel();

            try
            {
                viewModel.OutputHandler = new OutputHandler { IsErrorOccured = false };
               
                //Get Client Type through API end Point
                var requestUrl = $"{BaseUrl}{apiUrl}/GetAllRegions";
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(requestUrl);
                    HttpResponseMessage responseMessage = await client.GetAsync(requestUrl);

                    //check status, if OK, all went well continue to converting from Json to ViewModel Data Transfer Object for rendering to view 
                    if (responseMessage.StatusCode == HttpStatusCode.OK)
                    {
                        string data = await responseMessage.Content.ReadAsStringAsync();

                        //Json to DTO convertion using Newtonsoft.Json
                        viewModel.Regions = JsonConvert.DeserializeObject<IEnumerable<RegionDTO>>(data);

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
            var viewModel = new RegionViewModel
            {
                OutputHandler = new OutputHandler { IsErrorOccured = false },
                Countries = await StaticDataHandler.GetCountries(BaseUrl)
            };

            return View(viewModel);
        }


        [HttpPost]
        public async Task<IActionResult> Create(RegionViewModel regionViewModel)
        {
            OutputHandler result = new();

            //capture Created Date = the time this item was/is created
            //regionViewModel.Region.CreatedDate = DateTime.Now.AddHours(2);
           // regionViewModel.Region.CreatedBy = "SYSADMIN"; //add session user's Email

            var requestUrl = $"{BaseUrl}{apiUrl}/Create";
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(requestUrl);
                client.BaseAddress = new Uri(requestUrl);
                HttpResponseMessage responseMessage = await client.PostAsJsonAsync(requestUrl, regionViewModel.Region);

                if (responseMessage.StatusCode == HttpStatusCode.OK)
                {
                    var data = await responseMessage.Content.ReadAsStringAsync();
                    result = JsonConvert.DeserializeObject<OutputHandler>(data);
                    return RedirectToAction("Index", "Region", new { message = result.Message });
                }
                else
                {
                    //an error has occured, prep the UI and send user message
                    var data = await responseMessage.Content.ReadAsStringAsync();
                    result = JsonConvert.DeserializeObject<OutputHandler>(data);
                    regionViewModel.OutputHandler = result;  
                    
                    //populate the dropdown for reload
                    regionViewModel.Countries = await StaticDataHandler.GetCountries(BaseUrl);
                     return View(regionViewModel);
                }
            }

        }
        [HttpGet]
        public async Task<IActionResult> Update(int regionId)
        {
            //Setup Dropdown lists  
            var regionVm = new RegionViewModel
            {
                Countries = await StaticDataHandler.GetCountries(BaseUrl),
            OutputHandler = new OutputHandler { IsErrorOccured = false }
            };
            try
            {
                //Get Region through API end Point
                var requestUrl = $"{BaseUrl}{apiUrl}/GetRegion?RegionId={regionId}";
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(requestUrl);
                    HttpResponseMessage responseMessage = await client.GetAsync(requestUrl);

                    //check status, if OK, all went well continue to converting from Json to ViewModel Data Transfer Object for rendering to view 
                    if (responseMessage.StatusCode == HttpStatusCode.OK)
                    {
                        string data = await responseMessage.Content.ReadAsStringAsync();

                        //Json to DTO convertion using Newtonsoft.Json
                        regionVm.Region = JsonConvert.DeserializeObject<RegionDTO>(data);
                        

                    }
                    else if (responseMessage.StatusCode == HttpStatusCode.NotFound)
                    {
                        //if the database doesn't have values, return message to user
                        regionVm.OutputHandler = new OutputHandler { IsErrorOccured = false, Message = "No records found" };
                        return View(regionVm);
                    }
                };
            }
            catch (Exception ex)
            {
                //in an event of an exception return General error

                var error = StandardMessages.getExceptionMessage(ex); //variable to avoid initialization/Instance related errors
                regionVm.OutputHandler = new OutputHandler
                {
                    IsErrorOccured = true,
                    Message = error.Message
                };
            }
            return View(regionVm);
        }

        [HttpPost]
        public async Task<IActionResult> Update(RegionViewModel regionViewModel)
        {
            OutputHandler result = new();

            //capture Modified Date = the time this item was modified/changed
            regionViewModel.Region.ModifiedDate = DateTime.Now.AddHours(2);
            regionViewModel.Region.ModifiedBy = "SYSADMIN"; //add session user's Email

            var requestUrl = $"{BaseUrl}{apiUrl}/Update";

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(requestUrl);
                HttpResponseMessage responseMessage = await client.PutAsJsonAsync(client.BaseAddress, regionViewModel.Region);
                if (responseMessage.StatusCode == HttpStatusCode.OK)
                {
                    var data = await responseMessage.Content.ReadAsStringAsync();
                    result = JsonConvert.DeserializeObject<OutputHandler>(data);
                    return RedirectToAction("Index", "Region", new { message = result.Message });
                }
                else
                {
                    //an error has occured, prep the UI and send user message
                    var data = await responseMessage.Content.ReadAsStringAsync();
                    result = JsonConvert.DeserializeObject<OutputHandler>(data);
                    if (result.Message == null)
                    {
                        regionViewModel.OutputHandler = new OutputHandler
                        {
                            IsErrorOccured = true,
                            Message = StandardMessages.GetGeneralErrorMessage()
                        };
                    }
                    else
                    {
                        regionViewModel.OutputHandler = result;
                    }


                    //populate the dropdown for reload
                    regionViewModel.Countries = await StaticDataHandler.GetCountries(BaseUrl),
                    return View(regionViewModel);
                }
            }
        }
        public async Task<IActionResult> Delete(int id)
        {
            OutputHandler resultHandler = new();
            var requestUrl = $"{BaseUrl}{apiUrl}/Delete?regionId={id}";
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
