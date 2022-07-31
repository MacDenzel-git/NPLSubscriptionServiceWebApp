using NPLDataAccessLayer.DataTransferObjects;
using NPLReusableResourcesPackage.General;

namespace NPLSubscriptionServiceWebApp.Models.ViewModels
{
    public class PaymentViewModel
    {
        public OutputHandler OutputHandler { get; set; }
        public PaymentDTO Payment { get; set; }
        public IEnumerable<PaymentDTO> Payments { get; set; }
        public IEnumerable<PaymentTypeDTO> PaymentTypes { get; set; }
        public IEnumerable<ClientDTO> Clients { get; set; }
    }
}
