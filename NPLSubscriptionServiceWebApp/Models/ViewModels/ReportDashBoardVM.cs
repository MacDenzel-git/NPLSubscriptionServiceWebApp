using NPLDataAccessLayer.DataTransferObjects;
using NPLReusableResourcesPackage.General;

namespace NPLSubscriptionServiceWebApp.Models.ViewModels
{
    public class ReportDashBoardVM
    {
      public  ReportDashBoardDTO ReportDashBoard { get; set; }
        public OutputHandler OutputHandler { get; set; }    
    }
}
