using NPLDataAccessLayer.DataTransferObjects;
using NPLReusableResourcesPackage.General;

namespace NPLSubscriptionServiceWebApp.Models.ViewModels
{
    public class RegionViewModel
    {
        public RegionDTO Region { get; set; }
        public IEnumerable<RegionDTO> Regions { get; set; }
        public OutputHandler OutputHandler { get; set; }
    }
}
