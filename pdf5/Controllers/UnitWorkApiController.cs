using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using WalkingTec.Mvvm.Mvc;
using WalkingTec.Mvvm.Core.Auth.Attribute;
using pdf5.ViewModels.UnitWorkVMs;
using pdf5.Models;

namespace pdf5.Controllers
{

    [AuthorizeJwtWithCookie]
    [ActionDescription("工作组测试")]
    [ApiController]
    [Route("api/UnitWork")]
    public partial class UnitWorkApiController : BaseApiController
    {
        [ActionDescription("Search")]
        [HttpPost("Search")]
        public IActionResult Search(UnitWorkApiSearcher searcher)
        {
            if (ModelState.IsValid)
            {
                var vm = CreateVM<UnitWorkApiListVM>();
                vm.Searcher = searcher;
                return Content(vm.GetJson());
            }
            else
            {
                return BadRequest(ModelState.GetErrorJson());
            }
        }

        [ActionDescription("Get")]
        [HttpGet("{id}")]
        public UnitWorkApiVM Get(string id)
        {
            var vm = CreateVM<UnitWorkApiVM>(id);
            return vm;
        }

        [ActionDescription("Create")]
        [HttpPost("Add")]
        public IActionResult Add(UnitWorkApiVM vm)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.GetErrorJson());
            }
            else
            {
                vm.DoAdd();
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState.GetErrorJson());
                }
                else
                {
                    return Ok(vm.Entity);
                }
            }

        }

        [ActionDescription("Edit")]
        [HttpPut("Edit")]
        public IActionResult Edit(UnitWorkApiVM vm)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState.GetErrorJson());
            }
            else
            {
                vm.DoEdit(false);
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState.GetErrorJson());
                }
                else
                {
                    return Ok(vm.Entity);
                }
            }
        }

        [HttpPost("BatchDelete")]
        [ActionDescription("Delete")]
        public IActionResult BatchDelete(string[] ids)
        {
            var vm = CreateVM<UnitWorkApiBatchVM>();
            if (ids != null && ids.Count() > 0)
            {
                vm.Ids = ids;
            }
            else
            {
                return Ok();
            }
            if (!ModelState.IsValid || !vm.DoBatchDelete())
            {
                return BadRequest(ModelState.GetErrorJson());
            }
            else
            {
                return Ok(ids.Count());
            }
        }


        [ActionDescription("Export")]
        [HttpPost("ExportExcel")]
        public IActionResult ExportExcel(UnitWorkApiSearcher searcher)
        {
            var vm = CreateVM<UnitWorkApiListVM>();
            vm.Searcher = searcher;
            vm.SearcherMode = ListVMSearchModeEnum.Export;
            return vm.GetExportData();
        }

        [ActionDescription("CheckExport")]
        [HttpPost("ExportExcelByIds")]
        public IActionResult ExportExcelByIds(string[] ids)
        {
            var vm = CreateVM<UnitWorkApiListVM>();
            if (ids != null && ids.Count() > 0)
            {
                vm.Ids = new List<string>(ids);
                vm.SearcherMode = ListVMSearchModeEnum.CheckExport;
            }
            return vm.GetExportData();
        }

        [ActionDescription("DownloadTemplate")]
        [HttpGet("GetExcelTemplate")]
        public IActionResult GetExcelTemplate()
        {
            var vm = CreateVM<UnitWorkApiImportVM>();
            var qs = new Dictionary<string, string>();
            foreach (var item in Request.Query.Keys)
            {
                qs.Add(item, Request.Query[item]);
            }
            vm.SetParms(qs);
            var data = vm.GenerateTemplate(out string fileName);
            return File(data, "application/vnd.ms-excel", fileName);
        }

        [ActionDescription("Import")]
        [HttpPost("Import")]
        public ActionResult Import(UnitWorkApiImportVM vm)
        {

            if (vm.ErrorListVM.EntityList.Count > 0 || !vm.BatchSaveData())
            {
                return BadRequest(vm.GetErrorJson());
            }
            else
            {
                return Ok(vm.EntityList.Count);
            }
        }


        [HttpGet("GetUnitWorks")]
        public ActionResult GetUnitWorks()
        {
            return Ok(DC.Set<UnitWork>().GetSelectListItems(LoginUserInfo?.DataPrivileges, null, x => x.UnitWorkName));
        }


        [HttpPost("AddItem")]
        public IActionResult AddItem(string name, string id)
        {
            UnitWork unit = new UnitWork
            {
                ParentId = new Guid(id),
                ID = new Guid(),
                UnitWorkName = name
            };

            DC.AddEntity(unit);
            int x = DC.SaveChanges();
            if (x > 0)
            {
                return Content("添加成功");
            }
            return Content("添加失败");

        }
    }
}