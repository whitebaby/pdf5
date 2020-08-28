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
    public partial class ConvertLogApiSearcher : BaseSearcher
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

        protected override void InitVM()
        {
        }

    }
}
