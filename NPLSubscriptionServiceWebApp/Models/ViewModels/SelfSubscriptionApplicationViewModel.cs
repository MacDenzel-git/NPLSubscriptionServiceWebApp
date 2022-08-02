using NPLDataAccessLayer.DataTransferObjects;
using NPLReusableResourcesPackage.General;

namespace NPLSubscriptionServiceWebApp.Models.ViewModels
{
    public class SelfSubscriptionApplicationViewModel
    {
        public SelfSubscriptionApplicationDTO SelfSubscriptionApplication { get; set; }
        public IEnumerable<SelfSubscriptionApplicationDTO> SelfSubscriptionApplications { get; set; }
        public OutputHandler OutputHandler { get; set; }
        public IEnumerable<SubscriptionTypeDTO> SubscriptionTypes { get; set; }
        public IEnumerable<PaymentTypeDTO> PaymentTypes { get; set; }
         public IEnumerable<PublicationDTO> Publications { get; set; }
        public IEnumerable<TypeOfDeliveryDTO> TypeOfDeliveries { get; set; }
        public IEnumerable<RegionDTO> Regions { get;  set; }
         public IEnumerable<DistrictDTO> Districts { get;  set; }
         public IEnumerable<ClientTypeDTO> ClientTypes { get;  set; }
    }
}
