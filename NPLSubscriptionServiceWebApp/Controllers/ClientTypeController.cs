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
    public class PaymentTypeController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly string apiUrl = "PaymentType";
        public string BaseUrl
        {
            get
            {
                return _configuration["EndpointUrl"];
            }
        }
        public PaymentTypeController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task<IActionResult> Index(string message = "")
        {
            ClientTypeViewModel clientTypeVm = new ClientTypeViewModel();

            try
            {
                clientTypeVm.OutputHandler = new OutputHandler { IsErrorOccured = false };
               
                //Get Payment Type through API end Point
                var requestUrl = $"{BaseUrl}{apiUrl}/GetAllPaymentTypes";
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(requestUrl);
                    HttpResponseMessage responseMessage = await client.GetAsync(requestUrl);

                    //check status, if OK, all went well continue to converting from Json to ViewModel Data Transfer Object for rendering to view 
                    if (responseMessage.StatusCode == HttpStatusCode.OK)
                    {
                        string data = await responseMessage.Content.ReadAsStringAsync();

                        //Json to DTO convertion using Newtonsoft.Json
                        clientTypeVm.ClientTypes = JsonConvert.DeserializeObject<IEnumerable<PaymentTypeDTO>>(data);

                    }
                    else if (responseMessage.StatusCode == HttpStatusCode.NotFound)
                    {
                        //if the database doesn't have values, return message to user
                        clientTypeVm.OutputHandler = new OutputHandler { IsErrorOccured = false, Message = "No records found" };
                        return View(clientTypeVm);
                    }

                };
            }
            catch (Exception ex)
            {
                //in an event of an exception return General error

                var error = StandardMessages.getExceptionMessage(ex); //variable to avoid initialization/Instance related errors
                clientTypeVm.OutputHandler.Message = error.Message;
                return View(clientTypeVm);
            }
            if (!String.IsNullOrEmpty(message))
            {
                clientTypeVm.OutputHandler.Message = message;

            }
            return View(clientTypeVm);

        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            //Populate dropDown List
            var viewModel = new ClientTypeViewModel
            {
                OutputHandler = new OutputHandler { IsErrorOccured = false }
            };

            return View(viewModel);
        }


        [HttpPost]
        public async Task<IActionResult> Create(ClientTypeViewModel clientTypeViewModel)
        {
            OutputHandler result = new();

            //capture Created Date = the time this item was/is created
            //clientTypeViewModel.ClientType.CreatedDate = DateTime.Now.AddHours(2);
           // clientTypeViewModel.ClientType.CreatedBy = "SYSADMIN"; //add session user's Email

            var requestUrl = $"{BaseUrl}{apiUrl}/Create";
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(requestUrl);
                client.BaseAddress = new Uri(requestUrl);
                HttpResponseMessage responseMessage = await client.PostAsJsonAsync(requestUrl, clientTypeViewModel.ClientType);

                if (responseMessage.StatusCode == HttpStatusCode.OK)
                {
                    var data = await responseMessage.Content.ReadAsStringAsync();
                    result = JsonConvert.DeserializeObject<OutputHandler>(data);
                    return RedirectToAction("Index", "ClientType", new { message = result.Message });
                }
                else
                {
                    //an error has occured, prep the UI and send user message
                    var data = await responseMessage.Content.ReadAsStringAsync();
                    result = JsonConvert.DeserializeObject<OutputHandler>(data);
                    clientTypeViewModel.OutputHandler = result;

                    //populate the dropdown for reload
                     return View(clientTypeViewModel);
                }
            }

        }
        [HttpGet]
        public async Task<IActionResult> Update(int clientTypeId)
        {
            //Setup Dropdown lists  
            var clientTypeVm = new ClientTypeViewModel
            {
             
                OutputHandler = new OutputHandler { IsErrorOccured = false }
            };
            try
            {
                //Get ClientType through API end Point
                var requestUrl = $"{BaseUrl}{apiUrl}/GetClientType?ClientTypeId={clientTypeId}";
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(requestUrl);
                    HttpResponseMessage responseMessage = await client.GetAsync(requestUrl);

                    //check status, if OK, all went well continue to converting from Json to ViewModel Data Transfer Object for rendering to view 
                    if (responseMessage.StatusCode == HttpStatusCode.OK)
                    {
                        string data = await responseMessage.Content.ReadAsStringAsync();

                        //Json to DTO convertion using Newtonsoft.Json
                        clientTypeVm.ClientType = JsonConvert.DeserializeObject<ClientTypeDTO>(data);
                        

                    }
                    else if (responseMessage.StatusCode == HttpStatusCode.NotFound)
                    {
                        //if the database doesn't have values, return message to user
                        clientTypeVm.OutputHandler = new OutputHandler { IsErrorOccured = false, Message = "No records found" };
                        return View(clientTypeVm);
                    }
                };
            }
            catch (Exception ex)
            {
                //in an event of an exception return General error

                var error = StandardMessages.getExceptionMessage(ex); //variable to avoid initialization/Instance related errors
                clientTypeVm.OutputHandler = new OutputHandler
                {
                    IsErrorOccured = true,
                    Message = error.Message
                };
            }
            return View(clientTypeVm);
        }

        [HttpPost]
        public async Task<IActionResult> Update(ClientTypeViewModel clientTypeViewModel)
        {
            OutputHandler result = new();

            //capture Modified Date = the time this item was modified/changed
            //clientTypeViewModel.ClientType.ModifiedDate = DateTime.Now.AddHours(2);
            //clientTypeViewModel.ClientType.ModifiedBy = "SYSADMIN"; //add session user's Email

            var requestUrl = $"{BaseUrl}{apiUrl}/Update";

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(requestUrl);
                HttpResponseMessage responseMessage = await client.PutAsJsonAsync(client.BaseAddress, clientTypeViewModel.ClientType);
                if (responseMessage.StatusCode == HttpStatusCode.OK)
                {
                    var data = await responseMessage.Content.ReadAsStringAsync();
                    result = JsonConvert.DeserializeObject<OutputHandler>(data);
                    return RedirectToAction("Index", "ClientType", new { message = result.Message });
                }
                else
                {
                    //an error has occured, prep the UI and send user message
                    var data = await responseMessage.Content.ReadAsStringAsync();
                    result = JsonConvert.DeserializeObject<OutputHandler>(data);
                    if (result.Message == null)
                    {
                        clientTypeViewModel.OutputHandler = new OutputHandler
                        {
                            IsErrorOccured = true,
                            Message = StandardMessages.GetGeneralErrorMessage()
                        };
                    }
                    else
                    {
                        clientTypeViewModel.OutputHandler = result;
                    }


                    //populate the dropdown for reload
                   
                    return View(clientTypeViewModel);
                }
            }
        }
        public async Task<IActionResult> Delete(int id)
        {
            OutputHandler resultHandler = new();
            var requestUrl = $"{BaseUrl}{apiUrl}/Delete?clientTypeId={id}";
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
