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
    public class NewsLetterController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly string apiUrl = "NewsLetter";
        public string BaseUrl
        {
            get
            {
                return _configuration["EndpointUrl"];
            }
        }
        public NewsLetterController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task<IActionResult> Index(string message = "")
        {
            NewsLetterViewModel viewModel = new NewsLetterViewModel();

            try
            {
                viewModel.OutputHandler = new OutputHandler { IsErrorOccured = false };
               
                //Get Client Type through API end Point
                var requestUrl = $"{BaseUrl}{apiUrl}/GetAllNewsLetters";
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(requestUrl);
                    HttpResponseMessage responseMessage = await client.GetAsync(requestUrl);

                    //check status, if OK, all went well continue to converting from Json to ViewModel Data Transfer Object for rendering to view 
                    if (responseMessage.StatusCode == HttpStatusCode.OK)
                    {
                        string data = await responseMessage.Content.ReadAsStringAsync();

                        //Json to DTO convertion using Newtonsoft.Json
                        viewModel.NewsLetters = JsonConvert.DeserializeObject<IEnumerable<NewsLetterDTO>>(data);

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
            var viewModel = new NewsLetterViewModel
            {
                OutputHandler = new OutputHandler { IsErrorOccured = false }, 
                Publications = await StaticDataHandler.GetPublications(BaseUrl)
            };

            return View(viewModel);
        }


        [HttpPost]
        public async Task<IActionResult> Create(NewsLetterViewModel newsLetterViewModel,  IFormFile pdfDoc)
        {
              if (pdfDoc.Length > 0)
                {
                    using (var ms = new MemoryStream())
                    {
                        string extension = Path.GetExtension(pdfDoc.FileName);
                        pdfDoc.CopyTo(ms);
                        var fileBytes = ms.ToArray();
                        // act on the Base64 data
                        newsLetterViewModel.NewsLetter.File = fileBytes;
                       newsLetterViewModel.NewsLetter.FileLocation = pdfDoc.FileName;
                    }
                }
            
            OutputHandler result = new();

            //capture Created Date = the time this item was/is created
            newsLetterViewModel.NewsLetter.CreatedDate = DateTime.Now.AddHours(2);
            newsLetterViewModel.NewsLetter.CreatedBy = "SYSADMIN"; //add session user's Email

            var requestUrl = $"{BaseUrl}{apiUrl}/Create";
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(requestUrl);
                client.BaseAddress = new Uri(requestUrl);
                HttpResponseMessage responseMessage = await client.PostAsJsonAsync(requestUrl, newsLetterViewModel.NewsLetter);

                if (responseMessage.StatusCode == HttpStatusCode.OK)
                {
                    var data = await responseMessage.Content.ReadAsStringAsync();
                    result = JsonConvert.DeserializeObject<OutputHandler>(data);
                    return RedirectToAction("Index", "NewsLetter", new { message = result.Message });
                }
                else
                {
                    //an error has occured, prep the UI and send user message
                    var data = await responseMessage.Content.ReadAsStringAsync();
                    result = JsonConvert.DeserializeObject<OutputHandler>(data);
                    newsLetterViewModel.OutputHandler = result;

                    //populate the dropdown for reload
                    newsLetterViewModel.Publications = await StaticDataHandler.GetPublications(BaseUrl);
                     return View(newsLetterViewModel);
                }
            }

        }
        [HttpGet]
        public async Task<IActionResult> Update(int newsLetterId)
        {
            //Setup Dropdown lists  
            var clientTypeVm = new NewsLetterViewModel
            {
                Publications = await StaticDataHandler.GetPublications(BaseUrl),
                OutputHandler = new OutputHandler { IsErrorOccured = false }
            };
            try
            {
                //Get NewsLetter through API end Point
                var requestUrl = $"{BaseUrl}{apiUrl}/GetNewsLetter?NewsLetterId={newsLetterId}";
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(requestUrl);
                    HttpResponseMessage responseMessage = await client.GetAsync(requestUrl);

                    //check status, if OK, all went well continue to converting from Json to ViewModel Data Transfer Object for rendering to view 
                    if (responseMessage.StatusCode == HttpStatusCode.OK)
                    {
                        string data = await responseMessage.Content.ReadAsStringAsync();

                        //Json to DTO convertion using Newtonsoft.Json
                        clientTypeVm.NewsLetter = JsonConvert.DeserializeObject<NewsLetterDTO>(data);
                        

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
        public async Task<IActionResult> Update(NewsLetterViewModel newsLetterViewModel)
        {
            OutputHandler result = new();

            //capture Modified Date = the time this item was modified/changed
            newsLetterViewModel.NewsLetter.ModifiedDate = DateTime.Now.AddHours(2);
            newsLetterViewModel.NewsLetter.ModifiedBy = "SYSADMIN"; //add session user's Email

            var requestUrl = $"{BaseUrl}{apiUrl}/Update";

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(requestUrl);
                HttpResponseMessage responseMessage = await client.PutAsJsonAsync(client.BaseAddress, newsLetterViewModel.NewsLetter);
                if (responseMessage.StatusCode == HttpStatusCode.OK)
                {
                    var data = await responseMessage.Content.ReadAsStringAsync();
                    result = JsonConvert.DeserializeObject<OutputHandler>(data);
                    return RedirectToAction("Index", "NewsLetter", new { message = result.Message });
                }
                else
                {
                    //an error has occured, prep the UI and send user message
                    var data = await responseMessage.Content.ReadAsStringAsync();
                    result = JsonConvert.DeserializeObject<OutputHandler>(data);
                    if (result.Message == null)
                    {
                        newsLetterViewModel.OutputHandler = new OutputHandler
                        {
                            IsErrorOccured = true,
                            Message = StandardMessages.GetGeneralErrorMessage()
                        };
                    }
                    else
                    {
                        newsLetterViewModel.OutputHandler = result;
                    }


                    //populate the dropdown for reload
                    newsLetterViewModel.Publications = await StaticDataHandler.GetPublications(BaseUrl);
                    return View(newsLetterViewModel);
                }
            }
        }
        public async Task<IActionResult> Delete(int id)
        {
            OutputHandler resultHandler = new();
            var requestUrl = $"{BaseUrl}{apiUrl}/Delete?NewsLetterId={id}";
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
        
        public async Task<IActionResult> SendNewsLetter(int newsLetterId)
        {
            OutputHandler resultHandler = new();
            var requestUrl = $"{BaseUrl}{apiUrl}/SendNewsLetter?NewsLetterId={newsLetterId}";
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(requestUrl);
                HttpResponseMessage responseMessage = await client.GetAsync(requestUrl);

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
