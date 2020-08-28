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
    public class UnitWorkApiTest
    {
        private UnitWorkApiController _controller;
        private string _seed;

        public UnitWorkApiTest()
        {
            _seed = Guid.NewGuid().ToString();
            _controller = MockController.CreateApi<UnitWorkApiController>(_seed, "user");
        }

        [TestMethod]
        public void SearchTest()
        {
            ContentResult rv = _controller.Search(new UnitWorkApiSearcher()) as ContentResult;
            Assert.IsTrue(string.IsNullOrEmpty(rv.Content)==false);
        }

        [TestMethod]
        public void CreateTest()
        {
            UnitWorkApiVM vm = _controller.CreateVM<UnitWorkApiVM>();
            UnitWork v = new UnitWork();
            
            vm.Entity = v;
            var rv = _controller.Add(vm);
            Assert.IsInstanceOfType(rv, typeof(OkObjectResult));

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

            UnitWorkApiVM vm = _controller.CreateVM<UnitWorkApiVM>();
            var oldID = v.ID;
            v = new UnitWork();
            v.ID = oldID;
       		
            vm.Entity = v;
            vm.FC = new Dictionary<string, object>();
			
            var rv = _controller.Edit(vm);
            Assert.IsInstanceOfType(rv, typeof(OkObjectResult));

            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
                var data = context.Set<UnitWork>().FirstOrDefault();
 				
                Assert.AreEqual(data.UpdateBy, "user");
                Assert.IsTrue(DateTime.Now.Subtract(data.UpdateTime.Value).Seconds < 10);
            }

        }

		[TestMethod]
        public void GetTest()
        {
            UnitWork v = new UnitWork();
            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
        		
                context.Set<UnitWork>().Add(v);
                context.SaveChanges();
            }
            var rv = _controller.Get(v.ID.ToString());
            Assert.IsNotNull(rv);
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

            var rv = _controller.BatchDelete(new string[] { v1.ID.ToString(), v2.ID.ToString() });
            Assert.IsInstanceOfType(rv, typeof(OkObjectResult));

            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
                Assert.AreEqual(context.Set<UnitWork>().Count(), 0);
            }

            rv = _controller.BatchDelete(new string[] {});
            Assert.IsInstanceOfType(rv, typeof(OkResult));

        }


    }
}
