using NPLDataAccessLayer.DataTransferObjects;
using NPLReusableResourcesPackage.General;

namespace NPLSubscriptionServiceWebApp.Models.ViewModels
{
    public class TypeOfDeliveryViewModel
    {
        public OutputHandler OutputHandler { get; set; }
        public TypeOfDeliveryDTO TypeOfDelivery { get; set; }
        public IEnumerable<TypeOfDeliveryDTO> TypeOfDeliveries { get; set; }

    }
}
