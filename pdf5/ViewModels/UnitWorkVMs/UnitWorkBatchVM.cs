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
    public partial class UnitWorkBatchVM : BaseBatchVM<UnitWork, UnitWork_BatchEdit>
    {
        public UnitWorkBatchVM()
        {
            ListVM = new UnitWorkListVM();
            LinkedVM = new UnitWork_BatchEdit();
        }

    }

	/// <summary>
    /// Class to define batch edit fields
    /// </summary>
    public class UnitWork_BatchEdit : BaseVM
    {

        protected override void InitVM()
        {
        }

    }

}
