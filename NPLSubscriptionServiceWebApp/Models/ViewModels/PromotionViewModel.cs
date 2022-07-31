using NPLDataAccessLayer.DataTransferObjects;
using NPLReusableResourcesPackage.General;

namespace NPLSubscriptionServiceWebApp.Models.ViewModels
{
    public class PromotionViewModel
    {
        public OutputHandler OutputHandler { get; set; }
        public PromotionDTO Promotion { get; set; }
        public IEnumerable<PromotionDTO> Promotions { get; set; }

    }
}
