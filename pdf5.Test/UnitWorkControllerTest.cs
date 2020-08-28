using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WalkingTec.Mvvm.Core;
using pdf5.Controllers;
using pdf5.ViewModels.UnitWorkVMs;
using pdf5.Models;
using pdf5;

namespace pdf5.Test
{
    [TestClass]
    public class UnitWorkControllerTest
    {
        private UnitWorkController _controller;
        private string _seed;

        public UnitWorkControllerTest()
        {
            _seed = Guid.NewGuid().ToString();
            _controller = MockController.CreateController<UnitWorkController>(_seed, "user");
        }

        [TestMethod]
        public void SearchTest()
        {
            PartialViewResult rv = (PartialViewResult)_controller.Index();
            Assert.IsInstanceOfType(rv.Model, typeof(IBasePagedListVM<TopBasePoco, BaseSearcher>));
            string rv2 = _controller.Search(rv.Model as UnitWorkListVM);
            Assert.IsTrue(rv2.Contains("\"Code\":200"));
        }

        [TestMethod]
        public void CreateTest()
        {
            PartialViewResult rv = (PartialViewResult)_controller.Create();
            Assert.IsInstanceOfType(rv.Model, typeof(UnitWorkVM));

            UnitWorkVM vm = rv.Model as UnitWorkVM;
            UnitWork v = new UnitWork();
			
            vm.Entity = v;
            _controller.Create(vm);

            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
                var data = context.Set<UnitWork>().FirstOrDefault();
				
                Assert.AreEqual(data.CreateBy, "user");
                Assert.IsTrue(DateTime.Now.Subtract(data.CreateTime.Value).Seconds < 10);
            }

        }

        [TestMethod]
        public void EditTest()
        {
            UnitWork v = new UnitWork();
            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
       			
                context.Set<UnitWork>().Add(v);
                context.SaveChanges();
            }

            PartialViewResult rv = (PartialViewResult)_controller.Edit(v.ID.ToString());
            Assert.IsInstanceOfType(rv.Model, typeof(UnitWorkVM));

            UnitWorkVM vm = rv.Model as UnitWorkVM;
            v = new UnitWork();
            v.ID = vm.Entity.ID;
       		
            vm.Entity = v;
            vm.FC = new Dictionary<string, object>();
			
            _controller.Edit(vm);

            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
                var data = context.Set<UnitWork>().FirstOrDefault();
 				
                Assert.AreEqual(data.UpdateBy, "user");
                Assert.IsTrue(DateTime.Now.Subtract(data.UpdateTime.Value).Seconds < 10);
            }

        }


        [TestMethod]
        public void DeleteTest()
        {
            UnitWork v = new UnitWork();
            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
        		
                context.Set<UnitWork>().Add(v);
                context.SaveChanges();
            }

            PartialViewResult rv = (PartialViewResult)_controller.Delete(v.ID.ToString());
            Assert.IsInstanceOfType(rv.Model, typeof(UnitWorkVM));

            UnitWorkVM vm = rv.Model as UnitWorkVM;
            v = new UnitWork();
            v.ID = vm.Entity.ID;
            vm.Entity = v;
            _controller.Delete(v.ID.ToString(),null);

            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
                Assert.AreEqual(context.Set<UnitWork>().Count(), 0);
            }

        }


        [TestMethod]
        public void DetailsTest()
        {
            UnitWork v = new UnitWork();
            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
				
                context.Set<UnitWork>().Add(v);
                context.SaveChanges();
            }
            PartialViewResult rv = (PartialViewResult)_controller.Details(v.ID.ToString());
            Assert.IsInstanceOfType(rv.Model, typeof(IBaseCRUDVM<TopBasePoco>));
            Assert.AreEqual(v.ID, (rv.Model as IBaseCRUDVM<TopBasePoco>).Entity.GetID());
        }

        [TestMethod]
        public void BatchDeleteTest()
        {
            UnitWork v1 = new UnitWork();
            UnitWork v2 = new UnitWork();
            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
				
                context.Set<UnitWork>().Add(v1);
                context.Set<UnitWork>().Add(v2);
                context.SaveChanges();
            }

            PartialViewResult rv = (PartialViewResult)_controller.BatchDelete(new string[] { v1.ID.ToString(), v2.ID.ToString() });
            Assert.IsInstanceOfType(rv.Model, typeof(UnitWorkBatchVM));

            UnitWorkBatchVM vm = rv.Model as UnitWorkBatchVM;
            vm.Ids = new string[] { v1.ID.ToString(), v2.ID.ToString() };
            _controller.DoBatchDelete(vm, null);

            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
                Assert.AreEqual(context.Set<UnitWork>().Count(), 0);
            }
        }

        [TestMethod]
        public void ExportTest()
        {
            PartialViewResult rv = (PartialViewResult)_controller.Index();
            Assert.IsInstanceOfType(rv.Model, typeof(IBasePagedListVM<TopBasePoco, BaseSearcher>));
            IActionResult rv2 = _controller.ExportExcel(rv.Model as UnitWorkListVM);
            Assert.IsTrue((rv2 as FileContentResult).FileContents.Length > 0);
        }


    }
}
