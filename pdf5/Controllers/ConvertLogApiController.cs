﻿using Microsoft.AspNetCore.Mvc;
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
using System.Threading;
using Microsoft.AspNetCore.Authorization;
using NPOI.XWPF.UserModel;

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
        public async Task<ActionResult> Converting(Guid id,String disFileFormat)
        {
            if (id == Guid.Empty)
            {
                return BadRequest(WalkingTec.Mvvm.Core.Program._localizer["FileNotFound"]);
            }
            var vm = CreateVM<FileAttachmentVM>(id);
            String sourceFileName = vm.Entity.FileName;
            String sourchFileLocation = @"C:" + vm.Entity.Path;
            String distFileName =null;
            switch (disFileFormat)
            {
                case "doc":
                    distFileName = sourceFileName.Replace("pdf", "doc");
                    break;
                case "xls":
                    distFileName = sourceFileName.Replace("pdf", "xls");
                    break;
                case "ppt":
                    distFileName = sourceFileName.Replace("pdf", "ppt");
                    break;
                default:
                    break;
            }
            String distFullPath = @"C:\result\" + distFileName;
            Guid SourceFileID = id;
            Guid DistFileID = Guid.NewGuid();
            //添加convertLog日志
            await Task.Run(() => {
                ConvertLog convertLog = new ConvertLog
                {
                    SourceFileName = sourceFileName,
                    DistFileName = distFullPath,
                    ConvertTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:ffff"),
                    UserName = "xingmiao",
                    ConvertStatus = "start",
                    SourceFileID = SourceFileID.ToString(),
                    DistFileID = DistFileID.ToString()
                };
                DC.AddEntity(convertLog);

                int x = DC.SaveChanges();
                if (x > 0)
                {
                    Debug.WriteLine("添加converLog日志成功");
                    
                }
            });
            
            if (sourchFileLocation.Length > 5)
            {
                Thread t = new Thread(() =>
                {
                    //转换文档，然后更新状态
                    Task.Run(() =>
                    {
                        GetLicense();
                        if (disFileFormat == "doc")
                        {
                            Aspose.Pdf.Document pdfDocument = new Aspose.Pdf.Document(sourchFileLocation);
                            DocSaveOptions saveOptions = new DocSaveOptions();
                            saveOptions.CustomProgressHandler = new UnifiedSaveOptions.ConversionProgressEventHandler(ShowProgressOnConsole);
                            saveOptions.Format = DocSaveOptions.DocFormat.Doc;
                            pdfDocument.Save(distFullPath, saveOptions);
                        }
                         else if (disFileFormat == "xls")
                        {
                            Aspose.Pdf.Document pdfDocument = new Aspose.Pdf.Document(sourchFileLocation);
                            ExcelSaveOptions saveOptions = new ExcelSaveOptions();
                            saveOptions.Format = ExcelSaveOptions.ExcelFormat.XMLSpreadSheet2003;
                            pdfDocument.Save(distFullPath, saveOptions);
                        }
                        else if (disFileFormat == "ppt")
                        {
                            Aspose.Pdf.Document pdfDocument = new Aspose.Pdf.Document(sourchFileLocation);
                            PptxSaveOptions saveOptions = new PptxSaveOptions();
                            pdfDocument.Save(distFullPath, saveOptions);
                        }



            }).ContinueWith((t) =>
                    {
                        Debug.WriteLine(t);
                        ConvertLog updataConverLog = DC.Set<ConvertLog>().FirstOrDefault(x => x.SourceFileID == SourceFileID.ToString());
                        updataConverLog.ConvertStatus = "success";
                        DC.UpdateEntity(updataConverLog);

                        int x = DC.SaveChanges();
                        if (x > 0)
                        {
                            Debug.WriteLine("修改成功");
                        }


                    });
                })
                {
                    IsBackground = false//因为线程IsBackground = false，不是后台线程，所以主线程即使完成了，子线程也会继续完成
                };
                t.Start();
                //插入转换后附件文档信息，但不上传
                await Task.Run(() =>
                {

                    var sm = ConfigInfo.FileUploadOptions.SaveFileMode;
                    var vm2 = CreateVM<FileAttachmentVM>();
                    vm2.Entity.ID = DistFileID;
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


                return Ok(new { sourceFileName, sourchFileLocation, distFileName , distFullPath , SourceFileID , DistFileID = DistFileID.ToString()});

            }
            else
            {
                return Ok("错误");
            }
        }

        [HttpGet("downloadFile/{id}")]
        [AllowAnonymous]
        [ActionDescription("下载文件")]
        public IActionResult DownloadFile(Guid id)
        {
            if (id == Guid.Empty)
            {
                return BadRequest("没有找到文件");
            }
            var vm = CreateVM<FileAttachmentVM>(id);
            var data = FileHelper.GetFileByteForDownLoadByVM(vm, ConfigInfo);
            if (data == null)
            {
                data = new byte[0];
            }
            var ext = vm.Entity.FileExt.ToLower();
            var contenttype = "application/octet-stream";
            if (ext == "pdf")
            {
                contenttype = "application/pdf";
            }
            if (ext == "png" || ext == "bmp" || ext == "gif" || ext == "tif" || ext == "jpg" || ext == "jpeg")
            {
                contenttype = $"image/{ext}";
            }
            return File(data, contenttype, vm.Entity.FileName ?? (Guid.NewGuid().ToString() + ext));
        }
        [ActionDescription("License")]
        [HttpPost("License")]
        public void GetLicense()
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
        [HttpPost("ConvertStatus")]
        [ActionDescription("ConvertStatus")]
        public ConvertLog ConvertStatus(Guid id)
        {
            ConvertLog queryConverLog = DC.Set<ConvertLog>().FirstOrDefault(x => x.SourceFileID == id.ToString());
            return queryConverLog;
        }

    }
}
