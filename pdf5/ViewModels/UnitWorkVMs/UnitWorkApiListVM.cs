using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using pdf5.Models;


namespace pdf5.ViewModels.UnitWorkVMs
{
    public partial class UnitWorkApiListVM : BasePagedListVM<UnitWorkApi_View, UnitWorkApiSearcher>
    {

        protected override IEnumerable<IGridColumn<UnitWorkApi_View>> InitGridHeader()
        {
            return new List<GridColumn<UnitWorkApi_View>>{
                this.MakeGridHeader(x => x.UnitWorkName),
                this.MakeGridHeader(x => x.UnitWorkName_view),
                this.MakeGridHeaderAction(width: 200)
            };
        }

        public override IOrderedQueryable<UnitWorkApi_View> GetSearchQuery()
        {
            var query = DC.Set<UnitWork>()
                .Select(x => new UnitWorkApi_View
                {
				    ID = x.ID,
                    UnitWorkName = x.UnitWorkName,
                    UnitWorkName_view = x.Parent.UnitWorkName,
                })
                .OrderBy(x => x.ID);
            return query;
        }

    }

    public class UnitWorkApi_View : UnitWork{
        [Display(Name = "部门名称")]
        public String UnitWorkName_view { get; set; }

    }
}
