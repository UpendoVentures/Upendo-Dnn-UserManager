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

using DotNetNuke.Data;
using DotNetNuke.Framework;
using System.Collections.Generic;
using Upendo.Modules.UserManager.Data;
using Upendo.Modules.UserManager.Models;

namespace Upendo.Modules.UserManager.Data
{
    public interface IExampleInfoRepository
    {
        void CreateItem(ExampleInfo t);
        void DeleteItem(int itemId, int moduleId);
        void DeleteItem(ExampleInfo t);
        IEnumerable<ExampleInfo> GetItems(int moduleId);
        ExampleInfo GetItem(int itemId, int moduleId);
        void UpdateItem(ExampleInfo t);
    }

    public class ExampleInfoRepository : ServiceLocator<IExampleInfoRepository, ExampleInfoRepository>, IExampleInfoRepository
    {
        public void CreateItem(ExampleInfo t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<ExampleInfo>();
                rep.Insert(t);
            }
        }

        public void DeleteItem(int itemId, int moduleId)
        {
            var t = GetItem(itemId, moduleId);
            DeleteItem(t);
        }

        public void DeleteItem(ExampleInfo t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<ExampleInfo>();
                rep.Delete(t);
            }
        }

        public IEnumerable<ExampleInfo> GetItems(int moduleId)
        {
            IEnumerable<ExampleInfo> t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<ExampleInfo>();
                t = rep.Get(moduleId);
            }
            return t;
        }

        public ExampleInfo GetItem(int itemId, int moduleId)
        {
            ExampleInfo t;
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<ExampleInfo>();
                t = rep.GetById(itemId, moduleId);
            }
            return t;
        }

        public void UpdateItem(ExampleInfo t)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<ExampleInfo>();
                rep.Update(t);
            }
        }

        protected override System.Func<IExampleInfoRepository> GetFactory()
        {
            return () => new ExampleInfoRepository();
        }
    }
}
