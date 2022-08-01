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
    public class ClientController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly string apiUrl = "Client";
        public string BaseUrl
        {
            get
            {
                return _configuration["EndpointUrl"];
            }
        }
        public ClientController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task<IActionResult> Index(string message = "")
        {
            ClientViewModel clientVm = new ClientViewModel();

            try
            {
                clientVm.OutputHandler = new OutputHandler { IsErrorOccured = false };
                //var sessionDetails = await StaticDataHandler.GetSessionDetails();
                //Get Client through API end Point
                var requestUrl = $"{BaseUrl}{apiUrl}/GetAllClients";
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(requestUrl);
                    HttpResponseMessage responseMessage = await client.GetAsync(requestUrl);

                    //check status, if OK, all went well continue to converting from Json to ViewModel Data Transfer Object for rendering to view 
                    if (responseMessage.StatusCode == HttpStatusCode.OK)
                    {
                        string data = await responseMessage.Content.ReadAsStringAsync();

                        //Json to DTO convertion using Newtonsoft.Json
                        clientVm.Clients = JsonConvert.DeserializeObject<IEnumerable<ClientDTO>>(data);

                    }
                    else if (responseMessage.StatusCode == HttpStatusCode.NotFound)
                    {
                        //if the database doesn't have values, return message to user
                        clientVm.OutputHandler = new OutputHandler { IsErrorOccured = false, Message = "No records found" };
                        return View(clientVm);
                    }
                   
                };
            }
            catch (Exception ex)
            {
                //in an event of an exception return General error

                var error = StandardMessages.getExceptionMessage(ex); //variable to avoid initialization/Instance related errors
                clientVm.OutputHandler.Message = error.Message;
                clientVm.OutputHandler.IsErrorOccured = true;

                return View(clientVm);
            }
            if (!String.IsNullOrEmpty(message))
            {
                clientVm.OutputHandler.Message = message;
                 
            }
            return View(clientVm);

        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            //Populate dropDown List
            var viewModel = new ClientViewModel
            {
                ClientTypes = await StaticDataHandler.GetClientTypes(BaseUrl),
                Districts = await StaticDataHandler.GetDistricts(BaseUrl),
                Regions = await StaticDataHandler.GetRegions(BaseUrl),
                OutputHandler = new OutputHandler { IsErrorOccured = false }
            };

            return View(viewModel);
        }
        [HttpPost]
        public async Task<IActionResult> Create(ClientViewModel clientViewModel)
        {
            OutputHandler result = new();

            //capture Created Date = the time this item was/is created
            clientViewModel.Client.CreatedDate = DateTime.Now.AddHours(2);
              clientViewModel.Client.CreatedBy = "SYSADMIN"; //add session user's Email
            
            var requestUrl = $"{BaseUrl}{apiUrl}/Create";
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(requestUrl);
                client.BaseAddress = new Uri(requestUrl);
                HttpResponseMessage responseMessage = await client.PostAsJsonAsync(requestUrl, clientViewModel.Client);

                if (responseMessage.StatusCode == HttpStatusCode.OK)
                {
                    var data = await responseMessage.Content.ReadAsStringAsync();
                    result = JsonConvert.DeserializeObject<OutputHandler>(data);
                    return RedirectToAction("Index", "Client", new { message = result.Message });
                }
                else
                {
                    //an error has occured, prep the UI and send user message
                    var data = await responseMessage.Content.ReadAsStringAsync();
                    result = JsonConvert.DeserializeObject<OutputHandler>(data);
                    clientViewModel.OutputHandler = result;

                    //populate the dropdown for reload
                    clientViewModel.ClientTypes = await StaticDataHandler.GetClientTypes(BaseUrl);
                    clientViewModel.Districts = await StaticDataHandler.GetDistricts(BaseUrl);
                    clientViewModel.Regions = await StaticDataHandler.GetRegions(BaseUrl);
                    return View(clientViewModel);
                }
            }
            
        }
        [HttpGet]
        public async Task<IActionResult> Update(int clientId)
        {
            //Setup Dropdown lists  
            var clientVm = new ClientViewModel
            {
                ClientTypes = await StaticDataHandler.GetClientTypes(BaseUrl),
                Districts = await StaticDataHandler.GetDistricts(BaseUrl),
                Regions = await StaticDataHandler.GetRegions(BaseUrl),
                OutputHandler = new OutputHandler { IsErrorOccured = false }
            };
            try
            {
                //Get Client through API end Point
                var requestUrl = $"{BaseUrl}{apiUrl}/GetClient?ClientId={clientId}";
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(requestUrl);
                    HttpResponseMessage responseMessage = await client.GetAsync(requestUrl);

                    //check status, if OK, all went well continue to converting from Json to ViewModel Data Transfer Object for rendering to view 
                    if (responseMessage.StatusCode == HttpStatusCode.OK)
                    {
                        string data = await responseMessage.Content.ReadAsStringAsync();

                        //Json to DTO convertion using Newtonsoft.Json
                        clientVm.Client = JsonConvert.DeserializeObject<ClientDTO>(data);
                        clientVm.Client.OldEmail = clientVm.Client.Email;

                    }
                    else if (responseMessage.StatusCode == HttpStatusCode.NotFound)
                    {
                        //if the database doesn't have values, return message to user
                        clientVm.OutputHandler = new OutputHandler { IsErrorOccured = false, Message = "No records found" };
                        return View(clientVm);
                    }
                };
            }
            catch (Exception ex)
            {
                //in an event of an exception return General error

                var error = StandardMessages.getExceptionMessage(ex); //variable to avoid initialization/Instance related errors
                clientVm.OutputHandler = new OutputHandler
                {
                    IsErrorOccured = true,
                    Message = error.Message
                };
            }
            return View(clientVm);
        }

        [HttpPost]
        public async Task<IActionResult> Update(ClientViewModel clientViewModel)
        {
            OutputHandler result = new();

            //capture Modified Date = the time this item was modified/changed
            clientViewModel.Client.ModifiedDate = DateTime.Now.AddHours(2);
            clientViewModel.Client.ModifiedBy = "SYSADMIN"; //add session user's Email

            var requestUrl = $"{BaseUrl}{apiUrl}/Update";

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(requestUrl);
                HttpResponseMessage responseMessage = await client.PutAsJsonAsync(client.BaseAddress, clientViewModel.Client);
                if (responseMessage.StatusCode == HttpStatusCode.OK)
                {
                    var data = await responseMessage.Content.ReadAsStringAsync();
                    result = JsonConvert.DeserializeObject<OutputHandler>(data);
                    return RedirectToAction("Index", "Client", new { message = result.Message });
                }
                else
                {
                    //an error has occured, prep the UI and send user message
                    var data = await responseMessage.Content.ReadAsStringAsync();
                    result = JsonConvert.DeserializeObject<OutputHandler>(data);
                    if (result.Message == null)
                    {
                        clientViewModel.OutputHandler = new OutputHandler
                        {
                            IsErrorOccured = true,
                            Message = StandardMessages.GetGeneralErrorMessage()
                        };
                    }
                    else
                    {
                        clientViewModel.OutputHandler = result;
                    }


                    //populate the dropdown for reload
                    clientViewModel.ClientTypes = await StaticDataHandler.GetClientTypes(BaseUrl);
                    clientViewModel.Districts = await StaticDataHandler.GetDistricts(BaseUrl);
                    clientViewModel.Regions = await StaticDataHandler.GetRegions(BaseUrl);
                    return View(clientViewModel);
                }
            }
        }
        public async Task<IActionResult> Delete(int id)
        {
            OutputHandler resultHandler = new();
            var requestUrl = $"{BaseUrl}{apiUrl}/Delete?clientId={id}";
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
