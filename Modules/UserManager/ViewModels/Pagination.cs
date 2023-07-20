using System.Collections.Generic;

namespace Upendo.Modules.UserManager.ViewModels
{
    public struct Pagination
    {
        public double Take { get; set; }
        public int PageIndex { get; set; }
        public string Filter { get; set; }
        public int? GoToPage { get; set; }
        public int PortalId { get; set; }
        public string Search { get; set; }
        public string OrderBy;
        public string Order;
        public string ServerUrl;
    }
    public struct DataTableResponse<T>
    {
        public double Take { get; set; }
        public int PageIndex { get; set; }
        public int Page { get; set; }
        public int GoToPage { get; set; }
        public double PagesTotal { get; set; }
        public double RecordsTotal { get; set; }
        public int RecordsFiltered { get; set; }
        public string Search;
        public string OrderBy;
        public string Order;
        public List<T> Data;
    }

}