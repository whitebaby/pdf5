using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using pdf5.Models;


namespace pdf5.ViewModels.UnitWorkVMs
{
    public partial class UnitWorkApiBatchVM : BaseBatchVM<UnitWork, UnitWorkApi_BatchEdit>
    {
        public UnitWorkApiBatchVM()
        {
            ListVM = new UnitWorkApiListVM();
            LinkedVM = new UnitWorkApi_BatchEdit();
        }

    }

	/// <summary>
    /// Class to define batch edit fields
    /// </summary>
    public class UnitWorkApi_BatchEdit : BaseVM
    {

        protected override void InitVM()
        {
        }

    }

}
