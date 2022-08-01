using NPLDataAccessLayer.DataTransferObjects;
using NPLReusableResourcesPackage.General;

namespace NPLSubscriptionServiceWebApp.Models.ViewModels
{
    public class DistrictViewModel
    {
        public OutputHandler OutputHandler { get; set; }
        public DistrictDTO  District { get; set; }
        public IEnumerable<DistrictDTO>  Districts { get; set; }
        public IEnumerable<CountryDTO>  Countries { get; set; }
    }
}
