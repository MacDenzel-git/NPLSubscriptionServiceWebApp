using NPLDataAccessLayer.DataTransferObjects;
using NPLDataAccessLayer.Models;
using NPLReusableResourcesPackage.General;

namespace NPLSubscriptionServiceWebApp.Models.ViewModels
{
    public class PublicationViewModel
    {
        public OutputHandler OutputHandler { get; set; }
        public PublicationDTO Publication { get; set; }
        public IEnumerable<PublicationDTO> Publications { get; set; }
    }
}
