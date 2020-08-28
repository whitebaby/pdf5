using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WalkingTec.Mvvm.Core;
using pdf5.Controllers;
using pdf5.ViewModels.ConvertLogVMs;
using pdf5.Models;
using pdf5;

namespace pdf5.Test
{
    [TestClass]
    public class ConvertLogControllerTest
    {
        private ConvertLogController _controller;
        private string _seed;

        public ConvertLogControllerTest()
        {
            _seed = Guid.NewGuid().ToString();
            _controller = MockController.CreateController<ConvertLogController>(_seed, "user");
        }

        [TestMethod]
        public void SearchTest()
        {
            PartialViewResult rv = (PartialViewResult)_controller.Index();
            Assert.IsInstanceOfType(rv.Model, typeof(IBasePagedListVM<TopBasePoco, BaseSearcher>));
            string rv2 = _controller.Search(rv.Model as ConvertLogListVM);
            Assert.IsTrue(rv2.Contains("\"Code\":200"));
        }

        [TestMethod]
        public void CreateTest()
        {
            PartialViewResult rv = (PartialViewResult)_controller.Create();
            Assert.IsInstanceOfType(rv.Model, typeof(ConvertLogVM));

            ConvertLogVM vm = rv.Model as ConvertLogVM;
            ConvertLog v = new ConvertLog();
			
            vm.Entity = v;
            _controller.Create(vm);

            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
                var data = context.Set<ConvertLog>().FirstOrDefault();
				
            }

        }

        [TestMethod]
        public void EditTest()
        {
            ConvertLog v = new ConvertLog();
            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
       			
                context.Set<ConvertLog>().Add(v);
                context.SaveChanges();
            }

            PartialViewResult rv = (PartialViewResult)_controller.Edit(v.ID.ToString());
            Assert.IsInstanceOfType(rv.Model, typeof(ConvertLogVM));

            ConvertLogVM vm = rv.Model as ConvertLogVM;
            v = new ConvertLog();
            v.ID = vm.Entity.ID;
       		
            vm.Entity = v;
            vm.FC = new Dictionary<string, object>();
			
            _controller.Edit(vm);

            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
                var data = context.Set<ConvertLog>().FirstOrDefault();
 				
            }

        }


        [TestMethod]
        public void DeleteTest()
        {
            ConvertLog v = new ConvertLog();
            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
        		
                context.Set<ConvertLog>().Add(v);
                context.SaveChanges();
            }

            PartialViewResult rv = (PartialViewResult)_controller.Delete(v.ID.ToString());
            Assert.IsInstanceOfType(rv.Model, typeof(ConvertLogVM));

            ConvertLogVM vm = rv.Model as ConvertLogVM;
            v = new ConvertLog();
            v.ID = vm.Entity.ID;
            vm.Entity = v;
            _controller.Delete(v.ID.ToString(),null);

            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
                Assert.AreEqual(context.Set<ConvertLog>().Count(), 0);
            }

        }


        [TestMethod]
        public void DetailsTest()
        {
            ConvertLog v = new ConvertLog();
            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
				
                context.Set<ConvertLog>().Add(v);
                context.SaveChanges();
            }
            PartialViewResult rv = (PartialViewResult)_controller.Details(v.ID.ToString());
            Assert.IsInstanceOfType(rv.Model, typeof(IBaseCRUDVM<TopBasePoco>));
            Assert.AreEqual(v.ID, (rv.Model as IBaseCRUDVM<TopBasePoco>).Entity.GetID());
        }

        [TestMethod]
        public void BatchDeleteTest()
        {
            ConvertLog v1 = new ConvertLog();
            ConvertLog v2 = new ConvertLog();
            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
				
                context.Set<ConvertLog>().Add(v1);
                context.Set<ConvertLog>().Add(v2);
                context.SaveChanges();
            }

            PartialViewResult rv = (PartialViewResult)_controller.BatchDelete(new string[] { v1.ID.ToString(), v2.ID.ToString() });
            Assert.IsInstanceOfType(rv.Model, typeof(ConvertLogBatchVM));

            ConvertLogBatchVM vm = rv.Model as ConvertLogBatchVM;
            vm.Ids = new string[] { v1.ID.ToString(), v2.ID.ToString() };
            _controller.DoBatchDelete(vm, null);

            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
                Assert.AreEqual(context.Set<ConvertLog>().Count(), 0);
            }
        }


    }
}
