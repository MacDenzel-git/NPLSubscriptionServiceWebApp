using NPLDataAccessLayer.DataTransferObjects;
using NPLReusableResourcesPackage.General;

namespace NPLSubscriptionServiceWebApp.Models.ViewModels
{
    public class SubsctiptionTypeViewModel
    {
        public OutputHandler OutputHandler { get; set; }
        public SubscriptionTypeDTO SubscriptionType { get; set; }
        public IEnumerator<SubscriptionTypeDTO> SubscriptionTypes { get; set; }

    }
}
