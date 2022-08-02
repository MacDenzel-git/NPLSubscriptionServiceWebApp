using NPLDataAccessLayer.DataTransferObjects;
using NPLReusableResourcesPackage.General;

namespace NPLSubscriptionServiceWebApp.Models.ViewModels
{
    public class NewsLetterViewModel
    {
        public OutputHandler OutputHandler { get; set; }
        public NewsLetterDTO NewsLetter { get; set; }
        public IEnumerable<NewsLetterDTO> NewsLetters { get; set; }
        public IEnumerable<PublicationDTO> Publications { get; set; }
     }
}
