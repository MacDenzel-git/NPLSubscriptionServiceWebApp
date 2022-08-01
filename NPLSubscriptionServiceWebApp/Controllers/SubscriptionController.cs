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
    public class SubscriptionController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly string apiUrl = "Subscription";
        public string BaseUrl
        {
            get
            {
                return _configuration["EndpointUrl"];
            }
        }
        public SubscriptionController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task<IActionResult> Index(string message = "")
        {
            SubscriptionViewModel viewModel = new SubscriptionViewModel();

            try
            {
                viewModel.OutputHandler = new OutputHandler { IsErrorOccured = false };

                //Get Client Type through API end Point
                var requestUrl = $"{BaseUrl}{apiUrl}/GetAllSubscriptions";
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(requestUrl);
                    HttpResponseMessage responseMessage = await client.GetAsync(requestUrl);

                    //check status, if OK, all went well continue to converting from Json to ViewModel Data Transfer Object for rendering to view 
                    if (responseMessage.StatusCode == HttpStatusCode.OK)
                    {
                        string data = await responseMessage.Content.ReadAsStringAsync();

                        //Json to DTO convertion using Newtonsoft.Json
                        viewModel.Subscriptions = JsonConvert.DeserializeObject<IEnumerable<SubscriptionDTO>>(data);

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
            var viewModel = new SubscriptionViewModel
            {
                OutputHandler = new OutputHandler { IsErrorOccured = false },
                Clients = await StaticDataHandler.GetClients(BaseUrl),
                Promotions = await StaticDataHandler.GetPromotions(BaseUrl),
                SubscriptionStatuses = await StaticDataHandler.GetSubscriptionStatuses(BaseUrl),
                SubscriptionTypes = await StaticDataHandler.GetSubscriptionTypes(BaseUrl),
                Publications = await StaticDataHandler.GetPublications(BaseUrl),
                TypeOfDeliveries = await StaticDataHandler.GetTypesOfDelivery(BaseUrl),
                ClientPaymentRecords = await StaticDataHandler.GetPayments(BaseUrl), //TODO filter by user

            };

            return View(viewModel);
        }


        [HttpPost]
        public async Task<IActionResult> Create(SubscriptionViewModel subscriptionViewModel)
        {
            OutputHandler result = new();

            //capture Created Date = the time this item was/is created
            subscriptionViewModel.Subscription.CreatedDate = DateTime.Now.AddHours(2);
            subscriptionViewModel.Subscription.CreatedBy = "SYSADMIN"; //add session user's Email

            var requestUrl = $"{BaseUrl}{apiUrl}/Create";
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(requestUrl);
                client.BaseAddress = new Uri(requestUrl);
                HttpResponseMessage responseMessage = await client.PostAsJsonAsync(requestUrl, subscriptionViewModel.Subscription);

                if (responseMessage.StatusCode == HttpStatusCode.OK)
                {
                    var data = await responseMessage.Content.ReadAsStringAsync();
                    result = JsonConvert.DeserializeObject<OutputHandler>(data);
                    return RedirectToAction("Index", "Subscription", new { message = result.Message });
                }
                else
                {
                    //an error has occured, prep the UI and send user message
                    var data = await responseMessage.Content.ReadAsStringAsync();
                    result = JsonConvert.DeserializeObject<OutputHandler>(data);
                    subscriptionViewModel.OutputHandler = result;
                    subscriptionViewModel.Clients = await StaticDataHandler.GetClients(BaseUrl);
                    subscriptionViewModel.Promotions = await StaticDataHandler.GetPromotions(BaseUrl);
                    subscriptionViewModel.SubscriptionStatuses = await StaticDataHandler.GetSubscriptionStatuses(BaseUrl);
                    subscriptionViewModel.SubscriptionTypes = await StaticDataHandler.GetSubscriptionTypes(BaseUrl);
                    subscriptionViewModel.Publications = await StaticDataHandler.GetPublications(BaseUrl);
                    subscriptionViewModel.TypeOfDeliveries = await StaticDataHandler.GetTypesOfDelivery(BaseUrl);
                    subscriptionViewModel.ClientPaymentRecords = await StaticDataHandler.GetPayments(BaseUrl); //TODO filter by user

                    //populate the dropdown for reload
                     return View(subscriptionViewModel);
                }
            }

        }
        [HttpGet]
        public async Task<IActionResult> Update(int subscriptionId)
        {
            //Setup Dropdown lists  
            var subscriptionVm = new SubscriptionViewModel
            {
                Clients = await StaticDataHandler.GetClients(BaseUrl),
                Promotions = await StaticDataHandler.GetPromotions(BaseUrl),
                SubscriptionStatuses = await StaticDataHandler.GetSubscriptionStatuses(BaseUrl),
                SubscriptionTypes = await StaticDataHandler.GetSubscriptionTypes(BaseUrl),
                Publications = await StaticDataHandler.GetPublications(BaseUrl),
                TypeOfDeliveries = await StaticDataHandler.GetTypesOfDelivery(BaseUrl),
                ClientPaymentRecords = await StaticDataHandler.GetPayments(BaseUrl), //TODO filter by user

                OutputHandler = new OutputHandler { IsErrorOccured = false }
            };
            try
            {
                //Get Subscription through API end Point
                var requestUrl = $"{BaseUrl}{apiUrl}/GetSubscription?SubscriptionId={subscriptionId}";
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(requestUrl);
                    HttpResponseMessage responseMessage = await client.GetAsync(requestUrl);

                    //check status, if OK, all went well continue to converting from Json to ViewModel Data Transfer Object for rendering to view 
                    if (responseMessage.StatusCode == HttpStatusCode.OK)
                    {
                        string data = await responseMessage.Content.ReadAsStringAsync();

                        //Json to DTO convertion using Newtonsoft.Json
                        subscriptionVm.Subscription = JsonConvert.DeserializeObject<SubscriptionDTO>(data);


                    }
                    else if (responseMessage.StatusCode == HttpStatusCode.NotFound)
                    {
                        //if the database doesn't have values, return message to user
                        subscriptionVm.OutputHandler = new OutputHandler { IsErrorOccured = false, Message = "No records found" };
                        return View(subscriptionVm);
                    }
                };
            }
            catch (Exception ex)
            {
                //in an event of an exception return General error

                var error = StandardMessages.getExceptionMessage(ex); //variable to avoid initialization/Instance related errors
                subscriptionVm.OutputHandler = new OutputHandler
                {
                    IsErrorOccured = true,
                    Message = error.Message
                };
            }
            return View(subscriptionVm);
        }

        [HttpPost]
        public async Task<IActionResult> Update(SubscriptionViewModel subscriptionViewModel)
        {
            OutputHandler result = new();

            //capture Modified Date = the time this item was modified/changed
            subscriptionViewModel.Subscription.ModifiedDate = DateTime.Now.AddHours(2);
            subscriptionViewModel.Subscription.ModifiedBy = "SYSADMIN"; //add session user's Email

            var requestUrl = $"{BaseUrl}{apiUrl}/Update";

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(requestUrl);
                HttpResponseMessage responseMessage = await client.PutAsJsonAsync(client.BaseAddress, subscriptionViewModel.Subscription);
                if (responseMessage.StatusCode == HttpStatusCode.OK)
                {
                    var data = await responseMessage.Content.ReadAsStringAsync();
                    result = JsonConvert.DeserializeObject<OutputHandler>(data);
                    return RedirectToAction("Index", "Subscription", new { message = result.Message });
                }
                else
                {
                    //an error has occured, prep the UI and send user message
                    var data = await responseMessage.Content.ReadAsStringAsync();
                    result = JsonConvert.DeserializeObject<OutputHandler>(data);
                    if (result.Message == null)
                    {
                        subscriptionViewModel.OutputHandler = new OutputHandler
                        {
                            IsErrorOccured = true,
                            Message = StandardMessages.GetGeneralErrorMessage()
                        };
                    }
                    else
                    {
                        subscriptionViewModel.OutputHandler = result;
                    }


                    //populate the dropdown for reload
                    subscriptionViewModel.Clients = await StaticDataHandler.GetClients(BaseUrl);
                    subscriptionViewModel.Promotions = await StaticDataHandler.GetPromotions(BaseUrl);
                    subscriptionViewModel.SubscriptionStatuses = await StaticDataHandler.GetSubscriptionStatuses(BaseUrl);
                    subscriptionViewModel.SubscriptionTypes = await StaticDataHandler.GetSubscriptionTypes(BaseUrl);
                    subscriptionViewModel.Publications = await StaticDataHandler.GetPublications(BaseUrl);
                    subscriptionViewModel.TypeOfDeliveries = await StaticDataHandler.GetTypesOfDelivery(BaseUrl);
                    subscriptionViewModel.ClientPaymentRecords = await StaticDataHandler.GetPayments(BaseUrl); //TODO filter by user

                    return View(subscriptionViewModel);
                }
            }
        }
        public async Task<IActionResult> Delete(int id)
        {
            OutputHandler resultHandler = new();
            var requestUrl = $"{BaseUrl}{apiUrl}/Delete?subscriptionId={id}";
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

        [HttpPost]
        public async Task<IActionResult> SelfSubscription(SubscriptionViewModel subscriptionViewModel)
        {
            OutputHandler result = new();

            //capture Created Date = the time this item was/is created
            subscriptionViewModel.Subscription.CreatedDate = DateTime.Now.AddHours(2);
            subscriptionViewModel.Subscription.CreatedBy = "SYSADMIN"; //add session user's Email
            subscriptionViewModel.Subscription.

            var requestUrl = $"{BaseUrl}{apiUrl}/Create";
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(requestUrl);
                client.BaseAddress = new Uri(requestUrl);
                HttpResponseMessage responseMessage = await client.PostAsJsonAsync(requestUrl, subscriptionViewModel.Subscription);

                if (responseMessage.StatusCode == HttpStatusCode.OK)
                {
                    var data = await responseMessage.Content.ReadAsStringAsync();
                    result = JsonConvert.DeserializeObject<OutputHandler>(data);
                    return RedirectToAction("Index", "Subscription", new { message = result.Message });
                }
                else
                {
                    //an error has occured, prep the UI and send user message
                    var data = await responseMessage.Content.ReadAsStringAsync();
                    result = JsonConvert.DeserializeObject<OutputHandler>(data);
                    subscriptionViewModel.OutputHandler = result;
                    subscriptionViewModel.Clients = await StaticDataHandler.GetClients(BaseUrl);
                    subscriptionViewModel.Promotions = await StaticDataHandler.GetPromotions(BaseUrl);
                    subscriptionViewModel.SubscriptionStatuses = await StaticDataHandler.GetSubscriptionStatuses(BaseUrl);
                    subscriptionViewModel.SubscriptionTypes = await StaticDataHandler.GetSubscriptionTypes(BaseUrl);
                    subscriptionViewModel.Publications = await StaticDataHandler.GetPublications(BaseUrl);
                    subscriptionViewModel.TypeOfDeliveries = await StaticDataHandler.GetTypesOfDelivery(BaseUrl);
                    subscriptionViewModel.ClientPaymentRecords = await StaticDataHandler.GetPayments(BaseUrl); //TODO filter by user

                    //populate the dropdown for reload
                    return View(subscriptionViewModel);
                }
            }

        }
    }
}
