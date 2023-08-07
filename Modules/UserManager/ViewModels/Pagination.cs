/*
Copyright © Upendo Ventures, LLC

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and 
associated documentation files (the "Software"), to deal in the Software without restriction, 
including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, 
and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, 
subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial 
portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT 
NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND 
NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES 
OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN 
CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */

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