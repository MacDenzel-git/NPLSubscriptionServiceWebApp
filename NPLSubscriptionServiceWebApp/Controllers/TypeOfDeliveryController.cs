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
    public class TypeOfDeliveryController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly string apiUrl = "TypeOfDelivery";
        public string BaseUrl
        {
            get
            {
                return _configuration["EndpointUrl"];
            }
        }
        public TypeOfDeliveryController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task<IActionResult> Index(string message = "")
        {
            TypeOfDeliveryViewModel viewModel = new TypeOfDeliveryViewModel();

            try
            {
                viewModel.OutputHandler = new OutputHandler { IsErrorOccured = false };
               
                //Get Client Type through API end Point
                var requestUrl = $"{BaseUrl}{apiUrl}/GetAllTypeOfDeliveries";
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(requestUrl);
                    HttpResponseMessage responseMessage = await client.GetAsync(requestUrl);

                    //check status, if OK, all went well continue to converting from Json to ViewModel Data Transfer Object for rendering to view 
                    if (responseMessage.StatusCode == HttpStatusCode.OK)
                    {
                        string data = await responseMessage.Content.ReadAsStringAsync();

                        //Json to DTO convertion using Newtonsoft.Json
                        viewModel.TypeOfDeliveries = JsonConvert.DeserializeObject<IEnumerable<TypeOfDeliveryDTO>>(data);

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
            var viewModel = new TypeOfDeliveryViewModel
            {
                OutputHandler = new OutputHandler { IsErrorOccured = false }
            };

            return View(viewModel);
        }


        [HttpPost]
        public async Task<IActionResult> Create(TypeOfDeliveryViewModel typeOfDeliveryViewModel)
        {
            OutputHandler result = new();

            //capture Created Date = the time this item was/is created
            typeOfDeliveryViewModel.TypeOfDelivery.CreatedDate = DateTime.Now.AddHours(2);
            typeOfDeliveryViewModel.TypeOfDelivery.CreatedBy = "SYSADMIN"; //add session user's Email

            var requestUrl = $"{BaseUrl}{apiUrl}/Create";
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(requestUrl);
                client.BaseAddress = new Uri(requestUrl);
                HttpResponseMessage responseMessage = await client.PostAsJsonAsync(requestUrl, typeOfDeliveryViewModel.TypeOfDelivery);

                if (responseMessage.StatusCode == HttpStatusCode.OK)
                {
                    var data = await responseMessage.Content.ReadAsStringAsync();
                    result = JsonConvert.DeserializeObject<OutputHandler>(data);
                    return RedirectToAction("Index", "TypeOfDelivery", new { message = result.Message });
                }
                else
                {
                    //an error has occured, prep the UI and send user message
                    var data = await responseMessage.Content.ReadAsStringAsync();
                    result = JsonConvert.DeserializeObject<OutputHandler>(data);
                    typeOfDeliveryViewModel.OutputHandler = result;

                    //populate the dropdown for reload
                     return View(typeOfDeliveryViewModel);
                }
            }

        }
        [HttpGet]
        public async Task<IActionResult> Update(int typeOfDeliveryId)
        {
            //Setup Dropdown lists  
            var clientTypeVm = new TypeOfDeliveryViewModel
            {
             
                OutputHandler = new OutputHandler { IsErrorOccured = false }
            };
            try
            {
                //Get TypeOfDelivery through API end Point
                var requestUrl = $"{BaseUrl}{apiUrl}/GetTypeOfDelivery?TypeOfDeliveryId={typeOfDeliveryId}";
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(requestUrl);
                    HttpResponseMessage responseMessage = await client.GetAsync(requestUrl);

                    //check status, if OK, all went well continue to converting from Json to ViewModel Data Transfer Object for rendering to view 
                    if (responseMessage.StatusCode == HttpStatusCode.OK)
                    {
                        string data = await responseMessage.Content.ReadAsStringAsync();

                        //Json to DTO convertion using Newtonsoft.Json
                        clientTypeVm.TypeOfDelivery = JsonConvert.DeserializeObject<TypeOfDeliveryDTO>(data);
                        

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
        public async Task<IActionResult> Update(TypeOfDeliveryViewModel typeOfDeliveryViewModel)
        {
            OutputHandler result = new();

            //capture Modified Date = the time this item was modified/changed
            //typeOfDeliveryViewModel.TypeOfDelivery.ModifiedDate = DateTime.Now.AddHours(2);
            //typeOfDeliveryViewModel.TypeOfDelivery.ModifiedBy = "SYSADMIN"; //add session user's Email

            var requestUrl = $"{BaseUrl}{apiUrl}/Update";

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(requestUrl);
                HttpResponseMessage responseMessage = await client.PutAsJsonAsync(client.BaseAddress, typeOfDeliveryViewModel.TypeOfDelivery);
                if (responseMessage.StatusCode == HttpStatusCode.OK)
                {
                    var data = await responseMessage.Content.ReadAsStringAsync();
                    result = JsonConvert.DeserializeObject<OutputHandler>(data);
                    return RedirectToAction("Index", "TypeOfDelivery", new { message = result.Message });
                }
                else
                {
                    //an error has occured, prep the UI and send user message
                    var data = await responseMessage.Content.ReadAsStringAsync();
                    result = JsonConvert.DeserializeObject<OutputHandler>(data);
                    if (result.Message == null)
                    {
                        typeOfDeliveryViewModel.OutputHandler = new OutputHandler
                        {
                            IsErrorOccured = true,
                            Message = StandardMessages.GetGeneralErrorMessage()
                        };
                    }
                    else
                    {
                        typeOfDeliveryViewModel.OutputHandler = result;
                    }


                    //populate the dropdown for reload
                   
                    return View(typeOfDeliveryViewModel);
                }
            }
        }
        public async Task<IActionResult> Delete(int id)
        {
            OutputHandler resultHandler = new();
            var requestUrl = $"{BaseUrl}{apiUrl}/Delete?typeOfDeliveryId={id}";
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
