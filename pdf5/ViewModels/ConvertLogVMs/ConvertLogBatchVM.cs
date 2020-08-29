using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using pdf5.Models;


namespace pdf5.ViewModels.ConvertLogVMs
{
    public partial class ConvertLogBatchVM : BaseBatchVM<ConvertLog, ConvertLog_BatchEdit>
    {
        public ConvertLogBatchVM()
        {
            ListVM = new ConvertLogListVM();
            LinkedVM = new ConvertLog_BatchEdit();
        }

    }

	/// <summary>
    /// Class to define batch edit fields
    /// </summary>
    public class ConvertLog_BatchEdit : BaseVM
    {
        [Display(Name = "源文件名称")]
        public String SourceFileName { get; set; }
        [Display(Name = "目标文件名称")]
        public String DistFileName { get; set; }
        [Display(Name = "转换时间")]
        public String ConvertTime { get; set; }
        [Display(Name = "操作用户")]
        public String UserName { get; set; }
        [Display(Name = "转换状态")]
        public String ConvertStatus { get; set; }
        [Display(Name = "源文件ID")]
        public String SourceFileID { get; set; }
        [Display(Name = "目标文件ID")]
        public String DistFileID { get; set; }

        protected override void InitVM()
        {
        }

    }

}
