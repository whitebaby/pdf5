using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WalkingTec.Mvvm.Core;

namespace pdf5.Models
{
    public class ConvertLog :TopBasePoco
    {
        [Display(Name ="源文件名称")]
        public string SourceFileName { get; set; }
        [Display(Name = "目标文件名称")]
        public string DistFileName { get; set; }

        [Display(Name = "转换时间")]
        public string ConvertTime { get; set; }
        [Display(Name = "操作用户")]
        public string UserName { get; set; }
        [Display(Name = "转换状态")]
        public string ConvertStatus { get; set; }
        [Display(Name = "源文件ID")]
        public string SourceFileID { get; set; }
        [Display(Name = "目标文件ID")]
        public string DistFileID { get; set; }
    }
}
