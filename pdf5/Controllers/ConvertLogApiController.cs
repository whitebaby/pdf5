using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using WalkingTec.Mvvm.Mvc;
using WalkingTec.Mvvm.Core.Auth.Attribute;
using pdf5.ViewModels.ConvertLogVMs;
using pdf5.Models;
using System.Threading.Tasks;
using Aspose.Pdf;
using System.Diagnostics;
using System.IO;
using static Aspose.Pdf.UnifiedSaveOptions;

namespace pdf5.Controllers
{
    
    [AuthorizeJwtWithCookie]
    [ActionDescription("转换日志")]
    [ApiController]
    [Route("api/ConvertLog")]
	public partial class ConvertLogApiController : BaseApiController
    {
        [ActionDescription("Search")]
        [HttpPost("Search")]
		public IActionResult Search(ConvertLogApiSearcher searcher)
        {
            if (ModelState.IsValid)
            {
                var vm = CreateVM<ConvertLogApiListVM>();
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
        public ConvertLogApiVM Get(string id)
        {
            var vm = CreateVM<ConvertLogApiVM>(id);
            return vm;
        }

        [ActionDescription("Create")]
        [HttpPost("Add")]
        public IActionResult Add(ConvertLogApiVM vm)
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
        public IActionResult Edit(ConvertLogApiVM vm)
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
            var vm = CreateVM<ConvertLogApiBatchVM>();
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
        public IActionResult ExportExcel(ConvertLogApiSearcher searcher)
        {
            var vm = CreateVM<ConvertLogApiListVM>();
            vm.Searcher = searcher;
            vm.SearcherMode = ListVMSearchModeEnum.Export;
            return vm.GetExportData();
        }

        [ActionDescription("CheckExport")]
        [HttpPost("ExportExcelByIds")]
        public IActionResult ExportExcelByIds(string[] ids)
        {
            var vm = CreateVM<ConvertLogApiListVM>();
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
            var vm = CreateVM<ConvertLogApiImportVM>();
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
        public ActionResult Import(ConvertLogApiImportVM vm)
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

        [ActionDescription("Converting")]
        [HttpPost("Converting")]
        public async Task<ActionResult> Converting(Guid id)
        {
            if (id == Guid.Empty)
            {
                return BadRequest(WalkingTec.Mvvm.Core.Program._localizer["FileNotFound"]);
            }
            var vm = CreateVM<FileAttachmentVM>(id);
            String sourceFileName = vm.Entity.FileName;
            String sourchFileLocation = @"D:" + vm.Entity.Path;
            String distFileName = sourceFileName.Replace("pdf", "docx");
            String distFullPath = @"D:/result/" + distFileName;
            if (sourchFileLocation.Length > 5)
            {
                await Task.Run(() =>
                {
                    getLicense();
                    Document pdfDocument = new Document(sourchFileLocation);
                    DocSaveOptions saveOptions = new DocSaveOptions();
                    saveOptions.CustomProgressHandler = new UnifiedSaveOptions.ConversionProgressEventHandler(ShowProgressOnConsole);
                    saveOptions.Format = DocSaveOptions.DocFormat.DocX;
                    pdfDocument.Save(distFullPath, saveOptions);
                    ConvertLog convertLog = new ConvertLog
                    {
                        SourceFileName = sourceFileName,
                        DistFileName = distFullPath,
                        ConvertTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:ffff"),
                        UserName = "xingmiao",
                        ConvertStatus = "start"
                    };
                    DC.AddEntity(convertLog);

                    int x = DC.SaveChanges();
                    if (x > 0)
                    {
                        Debug.WriteLine("添加成功");
                    }

                }).ContinueWith((t) =>
                {
                    Debug.WriteLine(t);
                    ConvertLog updataConverLog = DC.Set<ConvertLog>().FirstOrDefault(x => x.SourceFileName == sourceFileName);
                    updataConverLog.ConvertStatus = "success";
                    DC.UpdateEntity(updataConverLog);

                    int x = DC.SaveChanges();
                    if (x > 0)
                    {
                        Debug.WriteLine("修改成功");
                    }

                    var sm = ConfigInfo.FileUploadOptions.SaveFileMode;
                    var vm2 = CreateVM<FileAttachmentVM>();
                    vm2.Entity.ID = Guid.NewGuid();
                    vm2.Entity.FileName = distFileName;
                    vm2.Entity.UploadTime = DateTime.Now;
                    vm2.Entity.SaveFileMode = sm;
                    vm2.Entity.Length = 1113434;
                    vm2.Entity.Path = distFullPath;
                    vm2.Entity.FileExt = "docx";
                    vm2.Entity.IsTemprory = false;
                    vm2.DoAdd();
                    return Ok(new { Id = vm2.Entity.ID.ToString(), Name = vm2.Entity.FileName });
                });
                //var t2 = Task.Run(() =>
                //{


                //    pdfDocument.Save(distFullPath, saveOptions);
                //    ConvertLog convertLog = new ConvertLog
                //    {
                //        SourceFileName = sourceFileName,
                //        DistFileName = distFullPath,
                //        ConvertTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:ffff"),
                //        userName = "xingmiao"
                //    };
                //    DC.AddEntity(convertLog);
                //    var sm = ConfigInfo.FileUploadOptions.SaveFileMode;
                //    var vm2 = CreateVM<FileAttachmentVM>();
                //    vm2.Entity.ID = Guid.NewGuid();
                //    vm2.Entity.FileName = distFileName;
                //    vm2.Entity.UploadTime = DateTime.Now;
                //    vm2.Entity.SaveFileMode = sm;
                //    vm2.Entity.Length = 1113434;
                //    vm2.Entity.Path = distFullPath;
                //    vm2.Entity.FileExt = "docx";
                //    vm2.Entity.IsTemprory = false;
                //    vm2.DoAdd();
                //    return Ok(new { Id = vm2.Entity.ID.ToString(), Name = vm2.Entity.FileName });

                //});
                //Task.WaitAll(t1, t2);
                //t1,t2,t3 完成后输出下面的消息
                //Console.WriteLine("t1,t2,t3 Is Complete");
                return Ok("t1,t2,t3 Is Complete");

            }
            else
            {
                return Ok("错误");
            }
        }
        [ActionDescription("License")]
        [HttpPost("License")]
        public void getLicense()
        {
            string LData = "PD94bWwgdmVyc2lvbj0iMS4wIiBlbmNvZGluZz0idXRmLTgiPz4NCjxMaWNlbnNlPg0KICAgIDxEYXRhPg0KICAgICAgICA8TGljZW5zZWRUbz5pckRldmVsb3BlcnMuY29tPC9MaWNlbnNlZFRvPg0KICAgICAgICA8RW1haWxUbz5pbmZvQGlyRGV2ZWxvcGVycy5jb208L0VtYWlsVG8+DQogICAgICAgIDxMaWNlbnNlVHlwZT5EZXZlbG9wZXIgT0VNPC9MaWNlbnNlVHlwZT4NCiAgICAgICAgPExpY2Vuc2VOb3RlPkxpbWl0ZWQgdG8gMTAwMCBkZXZlbG9wZXIsIHVubGltaXRlZCBwaHlzaWNhbCBsb2NhdGlvbnM8L0xpY2Vuc2VOb3RlPg0KICAgICAgICA8T3JkZXJJRD43ODQzMzY0Nzc4NTwvT3JkZXJJRD4NCiAgICAgICAgPFVzZXJJRD4xMTk0NDkyNDM3OTwvVXNlcklEPg0KICAgICAgICA8T0VNPlRoaXMgaXMgYSByZWRpc3RyaWJ1dGFibGUgbGljZW5zZTwvT0VNPg0KICAgICAgICA8UHJvZHVjdHM+DQogICAgICAgICAgICA8UHJvZHVjdD5Bc3Bvc2UuVG90YWwgUHJvZHVjdCBGYW1pbHk8L1Byb2R1Y3Q+DQogICAgICAgIDwvUHJvZHVjdHM+DQogICAgICAgIDxFZGl0aW9uVHlwZT5FbnRlcnByaXNlPC9FZGl0aW9uVHlwZT4NCiAgICAgICAgPFNlcmlhbE51bWJlcj57RjJCOTcwNDUtMUIyOS00QjNGLUJENTMtNjAxRUZGQTE1QUE5fTwvU2VyaWFsTnVtYmVyPg0KICAgICAgICA8U3Vic2NyaXB0aW9uRXhwaXJ5PjIwOTkxMjMxPC9TdWJzY3JpcHRpb25FeHBpcnk+DQogICAgICAgIDxMaWNlbnNlVmVyc2lvbj4zLjA8L0xpY2Vuc2VWZXJzaW9uPg0KICAgIDwvRGF0YT4NCiAgICA8U2lnbmF0dXJlPlFYTndiM05sTGxSdmRHRnNMb1B5YjJSMVkzUWdSbUZ0YVd4NTwvU2lnbmF0dXJlPg0KPC9MaWNlbnNlPg==";

            Stream stream = new MemoryStream(Convert.FromBase64String(LData));

            stream.Seek(0, SeekOrigin.Begin);
            new Aspose.Pdf.License().SetLicense(stream);
        }
        [ActionDescription("Status")]
        [HttpPost("Status")]
        public void ShowProgressOnConsole(DocSaveOptions.ProgressEventHandlerInfo eventInfo)
        {
            Console.WriteLine("进入到showprogress");
            switch (eventInfo.EventType)
            {
                case ProgressEventType.TotalProgress:
                    Debug.WriteLine(string.Format("{0}  - Conversion progress : {1}% .", DateTime.Now.ToLongTimeString(), eventInfo.Value.ToString()));
                    if (eventInfo.Value == 100)
                    {

                    }
                    break;

                case ProgressEventType.SourcePageAnalized:
                    Debug.WriteLine(string.Format("{0}  - Source page {1} of {2} analyzed.", DateTime.Now.ToLongTimeString(), eventInfo.Value.ToString(), eventInfo.MaxValue.ToString()));
                    break;
                case ProgressEventType.ResultPageCreated:
                    Debug.WriteLine(string.Format("{0}  - Result page's {1} of {2} layout created.", DateTime.Now.ToLongTimeString(), eventInfo.Value.ToString(), eventInfo.MaxValue.ToString()));
                    break;
                case ProgressEventType.ResultPageSaved:
                    Debug.WriteLine(string.Format("{0}  - Result page {1} of {2} exported.", DateTime.Now.ToLongTimeString(), eventInfo.Value.ToString(), eventInfo.MaxValue.ToString()));
                    //插入
                    break;

                default:
                    break;
            }
        }


    }
}
