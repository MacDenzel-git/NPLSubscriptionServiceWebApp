using NPLDataAccessLayer.DataTransferObjects;
using NPLReusableResourcesPackage.General;

namespace NPLSubscriptionServiceWebApp.Models.ViewModels
{
    public class ClientTypeViewModel
    {
        public ClientTypeDTO ClientType { get; set; }
        public IEnumerable<ClientTypeDTO> ClientTypes { get; set; }
        public OutputHandler OutputHandler { get; set; }
    }
}
