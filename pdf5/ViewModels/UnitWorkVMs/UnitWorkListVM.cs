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
    public partial class UnitWorkListVM : BasePagedListVM<UnitWork_View, UnitWorkSearcher>
    {
        protected override List<GridAction> InitGridAction()
        {
            return new List<GridAction>
            {
                this.MakeStandardAction("UnitWork", GridActionStandardTypesEnum.Create, Localizer["Create"],"", dialogWidth: 800),
                this.MakeStandardAction("UnitWork", GridActionStandardTypesEnum.Edit, Localizer["Edit"], "", dialogWidth: 800),
                this.MakeStandardAction("UnitWork", GridActionStandardTypesEnum.Delete, Localizer["Delete"], "", dialogWidth: 800),
                this.MakeStandardAction("UnitWork", GridActionStandardTypesEnum.Details, Localizer["Details"], "", dialogWidth: 800),
                this.MakeStandardAction("UnitWork", GridActionStandardTypesEnum.BatchEdit, Localizer["BatchEdit"], "", dialogWidth: 800),
                this.MakeStandardAction("UnitWork", GridActionStandardTypesEnum.BatchDelete, Localizer["BatchDelete"], "", dialogWidth: 800),
                this.MakeStandardAction("UnitWork", GridActionStandardTypesEnum.Import, Localizer["Import"], "", dialogWidth: 800),
                this.MakeStandardAction("UnitWork", GridActionStandardTypesEnum.ExportExcel, Localizer["Export"], ""),
            };
        }


        protected override IEnumerable<IGridColumn<UnitWork_View>> InitGridHeader()
        {
            return new List<GridColumn<UnitWork_View>>{
                this.MakeGridHeader(x => x.UnitWorkName),
                this.MakeGridHeader(x => x.UnitWorkName_view),
                this.MakeGridHeaderAction(width: 200)
            };
        }

        public override IOrderedQueryable<UnitWork_View> GetSearchQuery()
        {
            var query = DC.Set<UnitWork>()
                .Select(x => new UnitWork_View
                {
				    ID = x.ID,
                    UnitWorkName = x.UnitWorkName,
                    UnitWorkName_view = x.Parent.UnitWorkName,
                })
                .OrderBy(x => x.ID);
            return query;
        }

    }

    public class UnitWork_View : UnitWork{
        [Display(Name = "部门名称")]
        public String UnitWorkName_view { get; set; }

    }
}
