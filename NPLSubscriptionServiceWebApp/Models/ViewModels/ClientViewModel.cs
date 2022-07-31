using NPLDataAccessLayer.DataTransferObjects;
using NPLReusableResourcesPackage.General;

namespace NPLSubscriptionServiceWebApp.Models.ViewModels
{
    public class ClientViewModel
    {
        public ClientDTO Client { get; set; }
        public IEnumerable<ClientDTO> Clients { get; set; }
        public IEnumerable<ClientTypeDTO> ClientTypes { get; set; }
        public IEnumerable<RegionDTO> Regions { get; set; }
        public IEnumerable<DistrictDTO> Districts { get; set; }
        public  OutputHandler OutputHandler { get; set; }
    }
}
