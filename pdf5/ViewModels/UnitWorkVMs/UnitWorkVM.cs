﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using pdf5.Models;


namespace pdf5.ViewModels.UnitWorkVMs
{
    public partial class UnitWorkVM : BaseCRUDVM<UnitWork>
    {
        public List<ComboSelectListItem> AllParents { get; set; }

        public UnitWorkVM()
        {
            SetInclude(x => x.Parent);
        }

        protected override void InitVM()
        {
            AllParents = DC.Set<UnitWork>().GetSelectListItems(LoginUserInfo?.DataPrivileges, null, y => y.UnitWorkName);
        }

        public override void DoAdd()
        {           
            base.DoAdd();
        }

        public override void DoEdit(bool updateAllFields = false)
        {
            base.DoEdit(updateAllFields);
        }

        public override void DoDelete()
        {
            base.DoDelete();
        }
    }
}
