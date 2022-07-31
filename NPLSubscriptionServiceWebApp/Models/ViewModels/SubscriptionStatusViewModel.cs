using NPLDataAccessLayer.DataTransferObjects;
using NPLReusableResourcesPackage.General;

namespace NPLSubscriptionServiceWebApp.Models.ViewModels
{
    public class SubscriptionStatusViewModel
    {
        public OutputHandler OutputHandler { get; set; }
        public SubscriptionStatusDTO SubscriptionStatus { get; set; }
        public IEnumerable<SubscriptionStatusDTO> SubscriptionStatuses { get; set; }
    }
}
