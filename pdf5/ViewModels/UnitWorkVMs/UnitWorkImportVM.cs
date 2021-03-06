﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using pdf5.Models;


namespace pdf5.ViewModels.UnitWorkVMs
{
    public partial class UnitWorkTemplateVM : BaseTemplateVM
    {
        [Display(Name = "部门名称")]
        public ExcelPropety UnitWorkName_Excel = ExcelPropety.CreateProperty<UnitWork>(x => x.UnitWorkName);
        [Display(Name = "上级部门")]
        public ExcelPropety Parent_Excel = ExcelPropety.CreateProperty<UnitWork>(x => x.ParentId);

	    protected override void InitVM()
        {
            Parent_Excel.DataType = ColumnDataType.ComboBox;
            Parent_Excel.ListItems = DC.Set<UnitWork>().GetSelectListItems(LoginUserInfo?.DataPrivileges, null, y => y.UnitWorkName);
        }

    }

    public class UnitWorkImportVM : BaseImportVM<UnitWorkTemplateVM, UnitWork>
    {

    }

}
