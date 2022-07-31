using NPLDataAccessLayer.DataTransferObjects;
using NPLReusableResourcesPackage.General;

namespace NPLSubscriptionServiceWebApp.Models.ViewModels
{
    public class PaymentTypeViewModel
    {
        public PaymentTypeDTO PaymentType { get; set; }
        public IEnumerable<PaymentTypeDTO> PaymentTypes { get; set; }
        public OutputHandler OutputHandler { get; set; }
    }
}
