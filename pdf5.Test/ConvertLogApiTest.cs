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
    public class ConvertLogApiTest
    {
        private ConvertLogApiController _controller;
        private string _seed;

        public ConvertLogApiTest()
        {
            _seed = Guid.NewGuid().ToString();
            _controller = MockController.CreateApi<ConvertLogApiController>(_seed, "user");
        }

        [TestMethod]
        public void SearchTest()
        {
            string rv = _controller.Search(new ConvertLogApiSearcher());
            Assert.IsTrue(string.IsNullOrEmpty(rv)==false);
        }

        [TestMethod]
        public void CreateTest()
        {
            ConvertLogApiVM vm = _controller.CreateVM<ConvertLogApiVM>();
            ConvertLog v = new ConvertLog();
            
            vm.Entity = v;
            var rv = _controller.Add(vm);
            Assert.IsInstanceOfType(rv, typeof(OkObjectResult));

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

            ConvertLogApiVM vm = _controller.CreateVM<ConvertLogApiVM>();
            var oldID = v.ID;
            v = new ConvertLog();
            v.ID = oldID;
       		
            vm.Entity = v;
            vm.FC = new Dictionary<string, object>();
			
            var rv = _controller.Edit(vm);
            Assert.IsInstanceOfType(rv, typeof(OkObjectResult));

            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
                var data = context.Set<ConvertLog>().FirstOrDefault();
 				
            }

        }

		[TestMethod]
        public void GetTest()
        {
            ConvertLog v = new ConvertLog();
            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
        		
                context.Set<ConvertLog>().Add(v);
                context.SaveChanges();
            }
            var rv = _controller.Get(v.ID.ToString());
            Assert.IsNotNull(rv);
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

            var rv = _controller.BatchDelete(new string[] { v1.ID.ToString(), v2.ID.ToString() });
            Assert.IsInstanceOfType(rv, typeof(OkObjectResult));

            using (var context = new DataContext(_seed, DBTypeEnum.Memory))
            {
                Assert.AreEqual(context.Set<ConvertLog>().Count(), 0);
            }

            rv = _controller.BatchDelete(new string[] {});
            Assert.IsInstanceOfType(rv, typeof(OkResult));

        }


    }
}
