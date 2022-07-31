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
    public class PaymentController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly string apiUrl = "Payment";
        public string BaseUrl
        {
            get
            {
                return _configuration["EndpointUrl"];
            }
        }
        public PaymentController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task<IActionResult> Index(string message = "")
        {
            PaymentViewModel viewModel = new PaymentViewModel();

            try
            {
                viewModel.OutputHandler = new OutputHandler { IsErrorOccured = false };

                //Get Client Type through API end Point
                var requestUrl = $"{BaseUrl}{apiUrl}/GetAllPayments";
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(requestUrl);
                    HttpResponseMessage responseMessage = await client.GetAsync(requestUrl);

                    //check status, if OK, all went well continue to converting from Json to ViewModel Data Transfer Object for rendering to view 
                    if (responseMessage.StatusCode == HttpStatusCode.OK)
                    {
                        string data = await responseMessage.Content.ReadAsStringAsync();

                        //Json to DTO convertion using Newtonsoft.Json
                        viewModel.Payments = JsonConvert.DeserializeObject<IEnumerable<PaymentDTO>>(data);

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
            var viewModel = new PaymentViewModel
            {
                OutputHandler = new OutputHandler { IsErrorOccured = false },
                Clients = await StaticDataHandler.GetClients(BaseUrl),
                PaymentTypes = await StaticDataHandler.GetPaymentTypes(BaseUrl)

            };

            return View(viewModel);
        }


        [HttpPost]
        public async Task<IActionResult> Create(PaymentViewModel paymentViewModel)
        {
            OutputHandler result = new();

            //capture Created Date = the time this item was/is created
            //paymentViewModel.Payment.CreatedDate = DateTime.Now.AddHours(2);
            // paymentViewModel.Payment.CreatedBy = "SYSADMIN"; //add session user's Email

            var requestUrl = $"{BaseUrl}{apiUrl}/Create";
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(requestUrl);
                client.BaseAddress = new Uri(requestUrl);
                HttpResponseMessage responseMessage = await client.PostAsJsonAsync(requestUrl, paymentViewModel.Payment);

                if (responseMessage.StatusCode == HttpStatusCode.OK)
                {
                    var data = await responseMessage.Content.ReadAsStringAsync();
                    result = JsonConvert.DeserializeObject<OutputHandler>(data);
                    return RedirectToAction("Index", "Payment", new { message = result.Message });
                }
                else
                {
                    //an error has occured, prep the UI and send user message
                    var data = await responseMessage.Content.ReadAsStringAsync();
                    result = JsonConvert.DeserializeObject<OutputHandler>(data);
                    paymentViewModel.OutputHandler = result;

                    //populate the dropdown for reload
                    paymentViewModel.Clients = await StaticDataHandler.GetClients(BaseUrl);
                    paymentViewModel.PaymentTypes = await StaticDataHandler.GetPaymentTypes(BaseUrl);
                    return View(paymentViewModel);
                }
            }

        }
        [HttpGet]
        public async Task<IActionResult> Update(int paymentId)
        {
            //Setup Dropdown lists  
            var paymentVm = new PaymentViewModel
            {
                Clients = await StaticDataHandler.GetClients(BaseUrl),
                PaymentTypes = await StaticDataHandler.GetPaymentTypes(BaseUrl),
                OutputHandler = new OutputHandler { IsErrorOccured = false }
            };
            try
            {
                //Get Payment through API end Point
                var requestUrl = $"{BaseUrl}{apiUrl}/GetPayment?PaymentId={paymentId}";
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(requestUrl);
                    HttpResponseMessage responseMessage = await client.GetAsync(requestUrl);

                    //check status, if OK, all went well continue to converting from Json to ViewModel Data Transfer Object for rendering to view 
                    if (responseMessage.StatusCode == HttpStatusCode.OK)
                    {
                        string data = await responseMessage.Content.ReadAsStringAsync();

                        //Json to DTO convertion using Newtonsoft.Json
                        paymentVm.Payment = JsonConvert.DeserializeObject<PaymentDTO>(data);


                    }
                    else if (responseMessage.StatusCode == HttpStatusCode.NotFound)
                    {
                        //if the database doesn't have values, return message to user
                        paymentVm.OutputHandler = new OutputHandler { IsErrorOccured = false, Message = "No records found" };
                        return View(paymentVm);
                    }
                };
            }
            catch (Exception ex)
            {
                //in an event of an exception return General error

                var error = StandardMessages.getExceptionMessage(ex); //variable to avoid initialization/Instance related errors
                paymentVm.OutputHandler = new OutputHandler
                {
                    IsErrorOccured = true,
                    Message = error.Message
                };
            }
            return View(paymentVm);
        }

        [HttpPost]
        public async Task<IActionResult> Update(PaymentViewModel paymentViewModel)
        {
            OutputHandler result = new();

            //capture Modified Date = the time this item was modified/changed
            //paymentViewModel.Payment.ModifiedDate = DateTime.Now.AddHours(2);
            //paymentViewModel.Payment.ModifiedBy = "SYSADMIN"; //add session user's Email

            var requestUrl = $"{BaseUrl}{apiUrl}/Update";

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(requestUrl);
                HttpResponseMessage responseMessage = await client.PutAsJsonAsync(client.BaseAddress, paymentViewModel.Payment);
                if (responseMessage.StatusCode == HttpStatusCode.OK)
                {
                    var data = await responseMessage.Content.ReadAsStringAsync();
                    result = JsonConvert.DeserializeObject<OutputHandler>(data);
                    return RedirectToAction("Index", "Payment", new { message = result.Message });
                }
                else
                {
                    //an error has occured, prep the UI and send user message
                    var data = await responseMessage.Content.ReadAsStringAsync();
                    result = JsonConvert.DeserializeObject<OutputHandler>(data);
                    if (result.Message == null)
                    {
                        paymentViewModel.OutputHandler = new OutputHandler
                        {
                            IsErrorOccured = true,
                            Message = StandardMessages.GetGeneralErrorMessage()
                        };
                    }
                    else
                    {
                        paymentViewModel.OutputHandler = result;
                    }


                    //populate the dropdown for reload
                    paymentViewModel.Clients = await StaticDataHandler.GetClients(BaseUrl);
                    paymentViewModel.PaymentTypes = await StaticDataHandler.GetPaymentTypes(BaseUrl);
                    return View(paymentViewModel);
                }
            }
        }
        public async Task<IActionResult> Delete(int id)
        {
            OutputHandler resultHandler = new();
            var requestUrl = $"{BaseUrl}{apiUrl}/Delete?paymentId={id}";
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
