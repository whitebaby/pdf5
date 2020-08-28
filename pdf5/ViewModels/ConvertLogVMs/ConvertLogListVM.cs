using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WalkingTec.Mvvm.Core;
using WalkingTec.Mvvm.Core.Extensions;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using pdf5.Models;


namespace pdf5.ViewModels.ConvertLogVMs
{
    public partial class ConvertLogListVM : BasePagedListVM<ConvertLog_View, ConvertLogSearcher>
    {
        protected override List<GridAction> InitGridAction()
        {
            return new List<GridAction>
            {
                this.MakeStandardAction("ConvertLog", GridActionStandardTypesEnum.Create, Localizer["Create"],"", dialogWidth: 800),
                this.MakeStandardAction("ConvertLog", GridActionStandardTypesEnum.Edit, Localizer["Edit"], "", dialogWidth: 800),
                this.MakeStandardAction("ConvertLog", GridActionStandardTypesEnum.Delete, Localizer["Delete"], "", dialogWidth: 800),
                this.MakeStandardAction("ConvertLog", GridActionStandardTypesEnum.Details, Localizer["Details"], "", dialogWidth: 800),
                this.MakeStandardAction("ConvertLog", GridActionStandardTypesEnum.BatchEdit, Localizer["BatchEdit"], "", dialogWidth: 800),
                this.MakeStandardAction("ConvertLog", GridActionStandardTypesEnum.BatchDelete, Localizer["BatchDelete"], "", dialogWidth: 800),
                this.MakeStandardAction("ConvertLog", GridActionStandardTypesEnum.Import, Localizer["Import"], "", dialogWidth: 800),
                this.MakeStandardAction("ConvertLog", GridActionStandardTypesEnum.ExportExcel, Localizer["Export"], ""),
            };
        }


        protected override IEnumerable<IGridColumn<ConvertLog_View>> InitGridHeader()
        {
            return new List<GridColumn<ConvertLog_View>>{
                this.MakeGridHeader(x => x.SourceFileName),
                this.MakeGridHeader(x => x.DistFileName),
                this.MakeGridHeader(x => x.ConvertTime),
                this.MakeGridHeader(x => x.UserName),
                this.MakeGridHeader(x => x.ConvertStatus),
                this.MakeGridHeaderAction(width: 200)
            };
        }

        public override IOrderedQueryable<ConvertLog_View> GetSearchQuery()
        {
            var query = DC.Set<ConvertLog>()
                .CheckContain(Searcher.SourceFileName, x=>x.SourceFileName)
                .CheckContain(Searcher.DistFileName, x=>x.DistFileName)
                .CheckContain(Searcher.ConvertTime, x=>x.ConvertTime)
                .CheckContain(Searcher.UserName, x=>x.UserName)
                .CheckContain(Searcher.ConvertStatus, x=>x.ConvertStatus)
                .Select(x => new ConvertLog_View
                {
				    ID = x.ID,
                    SourceFileName = x.SourceFileName,
                    DistFileName = x.DistFileName,
                    ConvertTime = x.ConvertTime,
                    UserName = x.UserName,
                    ConvertStatus = x.ConvertStatus,
                })
                .OrderBy(x => x.ID);
            return query;
        }

    }

    public class ConvertLog_View : ConvertLog{

    }
}
