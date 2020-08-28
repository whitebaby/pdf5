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
    public partial class ConvertLogTemplateVM : BaseTemplateVM
    {
        [Display(Name = "源文件名称")]
        public ExcelPropety SourceFileName_Excel = ExcelPropety.CreateProperty<ConvertLog>(x => x.SourceFileName);
        [Display(Name = "目标文件名称")]
        public ExcelPropety DistFileName_Excel = ExcelPropety.CreateProperty<ConvertLog>(x => x.DistFileName);
        [Display(Name = "转换时间")]
        public ExcelPropety ConvertTime_Excel = ExcelPropety.CreateProperty<ConvertLog>(x => x.ConvertTime);
        [Display(Name = "操作用户")]
        public ExcelPropety UserName_Excel = ExcelPropety.CreateProperty<ConvertLog>(x => x.UserName);
        [Display(Name = "转换状态")]
        public ExcelPropety ConvertStatus_Excel = ExcelPropety.CreateProperty<ConvertLog>(x => x.ConvertStatus);

	    protected override void InitVM()
        {
        }

    }

    public class ConvertLogImportVM : BaseImportVM<ConvertLogTemplateVM, ConvertLog>
    {

    }

}
