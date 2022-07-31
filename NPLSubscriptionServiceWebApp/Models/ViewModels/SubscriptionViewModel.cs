using NPLDataAccessLayer.DataTransferObjects;
using NPLReusableResourcesPackage.General;

namespace NPLSubscriptionServiceWebApp.Models.ViewModels
{
    public class SubscriptionViewModel
    {
        public OutputHandler OutputHandler { get; set; }
        public SubscriptionDTO Subscription { get; set; }
        public IEnumerable <SubscriptionDTO> Subscriptions { get; set; }
    }
}
