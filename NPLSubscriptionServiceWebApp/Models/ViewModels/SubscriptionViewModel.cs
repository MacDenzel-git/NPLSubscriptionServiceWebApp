using NPLDataAccessLayer.DataTransferObjects;
using NPLReusableResourcesPackage.General;

namespace NPLSubscriptionServiceWebApp.Models.ViewModels
{
    public class SubscriptionViewModel
    {
        public OutputHandler OutputHandler { get; set; }
        public SubscriptionDTO Subscription { get; set; }
        public IEnumerable <SubscriptionDTO> Subscriptions { get; set; }
        public IEnumerable<SubscriptionStatusDTO> SubscriptionStatuses { get; set; }
        public IEnumerable<SubscriptionTypeDTO> SubscriptionTypes { get; set; }
        public IEnumerable<PaymentDTO> ClientPaymentRecords { get; set; } //FilterByClientApi
        public IEnumerable<PromotionDTO> Promotions { get; set; }
        public IEnumerable<ClientDTO> Clients { get; set;}
        public IEnumerable<PublicationDTO> Publications { get; set;}
        public IEnumerable<TypeOfDeliveryDTO> TypeOfDeliveries { get; set;}



    }
}
