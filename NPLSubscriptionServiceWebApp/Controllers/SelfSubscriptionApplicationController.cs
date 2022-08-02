using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NPLDataAccessLayer.DataTransferObjects;
using NPLReusableResourcesPackage.ErrorHandlingContainer;
using NPLReusableResourcesPackage.General;
using NPLSubscriptionServiceWebApp.Models.General;
using NPLSubscriptionServiceWebApp.Models.ViewModels;
using System.Net;

namespace NPLSelfSubscriptionServiceWebApp.Controllers
{
    public class SelfSubscriptionApplicationController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly string apiUrl = "SelfSubscription";
        public string BaseUrl
        {
            get
            {
                return _configuration["EndpointUrl"];
            }
        }
        public SelfSubscriptionApplicationController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task<IActionResult> Index(string message = "")
        {
            SelfSubscriptionApplicationViewModel viewModel = new SelfSubscriptionApplicationViewModel();

            try
            {
                viewModel.OutputHandler = new OutputHandler { IsErrorOccured = false };

                //Get Client Type through API end Point
                var requestUrl = $"{BaseUrl}{apiUrl}/GetAllSelfSubscriptions";
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(requestUrl);
                    HttpResponseMessage responseMessage = await client.GetAsync(requestUrl);

                    //check status, if OK, all went well continue to converting from Json to ViewModel Data Transfer Object for rendering to view 
                    if (responseMessage.StatusCode == HttpStatusCode.OK)
                    {
                        string data = await responseMessage.Content.ReadAsStringAsync();

                        //Json to DTO convertion using Newtonsoft.Json
                        viewModel.SelfSubscriptionApplications = JsonConvert.DeserializeObject<IEnumerable<SelfSubscriptionApplicationDTO>>(data);

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
                viewModel.OutputHandler.IsErrorOccured = true;

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
            var viewModel = new SelfSubscriptionApplicationViewModel
            {
                OutputHandler = new OutputHandler { IsErrorOccured = false },
                SubscriptionTypes = await StaticDataHandler.GetSubscriptionTypes(BaseUrl),
                Publications = await StaticDataHandler.GetPublications(BaseUrl),
                TypeOfDeliveries = await StaticDataHandler.GetTypesOfDelivery(BaseUrl),
                Regions = await StaticDataHandler.GetRegions(BaseUrl),
                Districts = await StaticDataHandler.GetDistricts(BaseUrl),
                PaymentTypes = await StaticDataHandler.GetPaymentTypes(BaseUrl)


            };

            return View(viewModel);
        }


        [HttpPost]
        public async Task<IActionResult> Create(SelfSubscriptionApplicationViewModel selfSubscriptionApplicationViewModel)
        {
            OutputHandler result = new();

            //capture Created Date = the time this item was/is created
            selfSubscriptionApplicationViewModel.SelfSubscriptionApplication.CreatedDate = DateTime.Now.AddHours(2);
            selfSubscriptionApplicationViewModel.SelfSubscriptionApplication.CreatedBy = "SYSADMIN"; //add session user's Email

            var requestUrl = $"{BaseUrl}{apiUrl}/Create";
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(requestUrl);
                client.BaseAddress = new Uri(requestUrl);
                HttpResponseMessage responseMessage = await client.PostAsJsonAsync(requestUrl, selfSubscriptionApplicationViewModel.SelfSubscriptionApplication);

                if (responseMessage.StatusCode == HttpStatusCode.OK)
                {
                    var data = await responseMessage.Content.ReadAsStringAsync();
                    result = JsonConvert.DeserializeObject<OutputHandler>(data);
                    return RedirectToAction("Index", "SelfSubscription", new { message = result.Message });
                }
                else
                {
                    //an error has occured, prep the UI and send user message
                    var data = await responseMessage.Content.ReadAsStringAsync();
                    result = JsonConvert.DeserializeObject<OutputHandler>(data);
                    selfSubscriptionApplicationViewModel.OutputHandler = result;
                    selfSubscriptionApplicationViewModel.SubscriptionTypes = await StaticDataHandler.GetSubscriptionTypes(BaseUrl);
                    selfSubscriptionApplicationViewModel.Publications = await StaticDataHandler.GetPublications(BaseUrl);
                    selfSubscriptionApplicationViewModel.TypeOfDeliveries = await StaticDataHandler.GetTypesOfDelivery(BaseUrl);
                    selfSubscriptionApplicationViewModel.Regions = await StaticDataHandler.GetRegions(BaseUrl);
                    selfSubscriptionApplicationViewModel.Districts = await StaticDataHandler.GetDistricts(BaseUrl);
                    selfSubscriptionApplicationViewModel.PaymentTypes = await StaticDataHandler.GetPaymentTypes(BaseUrl);

                    //populate the dropdown for reload
                    return View(selfSubscriptionApplicationViewModel);
                }
            }

        }
        [HttpGet]
        public async Task<IActionResult> Update(int subscriptionApplicationId)
        {
            //Setup Dropdown lists  
            var SelfSubscriptionVm = new SelfSubscriptionApplicationViewModel
            {
                  SubscriptionTypes = await StaticDataHandler.GetSubscriptionTypes(BaseUrl),
                Publications = await StaticDataHandler.GetPublications(BaseUrl),
                TypeOfDeliveries = await StaticDataHandler.GetTypesOfDelivery(BaseUrl),
                Regions = await StaticDataHandler.GetRegions(BaseUrl),
                Districts = await StaticDataHandler.GetDistricts(BaseUrl),
                PaymentTypes = await StaticDataHandler.GetPaymentTypes(BaseUrl),
                OutputHandler = new OutputHandler { IsErrorOccured = false }
            };
            try
            {
                //Get SelfSubscription through API end Point
                var requestUrl = $"{BaseUrl}{apiUrl}/GetSelfSubscription?SelfSubscriptionId={subscriptionApplicationId}";
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(requestUrl);
                    HttpResponseMessage responseMessage = await client.GetAsync(requestUrl);

                    //check status, if OK, all went well continue to converting from Json to ViewModel Data Transfer Object for rendering to view 
                    if (responseMessage.StatusCode == HttpStatusCode.OK)
                    {
                        string data = await responseMessage.Content.ReadAsStringAsync();

                        //Json to DTO convertion using Newtonsoft.Json
                        SelfSubscriptionVm.SelfSubscriptionApplication = JsonConvert.DeserializeObject<SelfSubscriptionApplicationDTO>(data);


                    }
                    else if (responseMessage.StatusCode == HttpStatusCode.NotFound)
                    {
                        //if the database doesn't have values, return message to user
                        SelfSubscriptionVm.OutputHandler = new OutputHandler { IsErrorOccured = false, Message = "No records found" };
                        return View(SelfSubscriptionVm);
                    }
                };
            }
            catch (Exception ex)
            {
                //in an event of an exception return General error

                var error = StandardMessages.getExceptionMessage(ex); //variable to avoid initialization/Instance related errors
                SelfSubscriptionVm.OutputHandler = new OutputHandler
                {
                    IsErrorOccured = true,
                    Message = error.Message
                };
            }
            return View(SelfSubscriptionVm);
        }

        [HttpPost]
        public async Task<IActionResult> Update(SelfSubscriptionApplicationViewModel selfSubscriptionApplicationViewModel)
        {
            OutputHandler result = new();

            //capture Modified Date = the time this item was modified/changed
            selfSubscriptionApplicationViewModel.SelfSubscriptionApplication.ModifiedDate = DateTime.Now.AddHours(2);
            selfSubscriptionApplicationViewModel.SelfSubscriptionApplication.ModifiedBy = "SYSADMIN"; //add session user's Email

            var requestUrl = $"{BaseUrl}{apiUrl}/Update";

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(requestUrl);
                HttpResponseMessage responseMessage = await client.PutAsJsonAsync(client.BaseAddress, selfSubscriptionApplicationViewModel.SelfSubscriptionApplication);
                if (responseMessage.StatusCode == HttpStatusCode.OK)
                {
                    var data = await responseMessage.Content.ReadAsStringAsync();
                    result = JsonConvert.DeserializeObject<OutputHandler>(data);
                    return RedirectToAction("Index", "SelfSubscription", new { message = result.Message });
                }
                else
                {
                    //an error has occured, prep the UI and send user message
                    var data = await responseMessage.Content.ReadAsStringAsync();
                    result = JsonConvert.DeserializeObject<OutputHandler>(data);
                    if (result.Message == null)
                    {
                        selfSubscriptionApplicationViewModel.OutputHandler = new OutputHandler
                        {
                            IsErrorOccured = true,
                            Message = StandardMessages.GetGeneralErrorMessage()
                        };
                    }
                    else
                    {
                        selfSubscriptionApplicationViewModel.OutputHandler = result;
                    }


                    //populate the dropdown for reload
                     selfSubscriptionApplicationViewModel.SubscriptionTypes = await StaticDataHandler.GetSubscriptionTypes(BaseUrl);
                    selfSubscriptionApplicationViewModel.Publications = await StaticDataHandler.GetPublications(BaseUrl);
                    selfSubscriptionApplicationViewModel.TypeOfDeliveries = await StaticDataHandler.GetTypesOfDelivery(BaseUrl);
                     selfSubscriptionApplicationViewModel.Regions = await StaticDataHandler.GetRegions(BaseUrl);
                    selfSubscriptionApplicationViewModel.Districts = await StaticDataHandler.GetDistricts(BaseUrl);

                    selfSubscriptionApplicationViewModel.PaymentTypes = await StaticDataHandler.GetPaymentTypes(BaseUrl);
                    return View(selfSubscriptionApplicationViewModel);
                }
            }
        }
        public async Task<IActionResult> Delete(int id)
        {
            OutputHandler resultHandler = new();
            var requestUrl = $"{BaseUrl}{apiUrl}/Delete?SelfSubscriptionId={id}";
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
