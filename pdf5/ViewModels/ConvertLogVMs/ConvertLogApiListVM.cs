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
    public partial class ConvertLogApiListVM : BasePagedListVM<ConvertLogApi_View, ConvertLogApiSearcher>
    {

        protected override IEnumerable<IGridColumn<ConvertLogApi_View>> InitGridHeader()
        {
            return new List<GridColumn<ConvertLogApi_View>>{
                this.MakeGridHeader(x => x.SourceFileName),
                this.MakeGridHeader(x => x.DistFileName),
                this.MakeGridHeader(x => x.ConvertTime),
                this.MakeGridHeader(x => x.UserName),
                this.MakeGridHeader(x => x.ConvertStatus),
                this.MakeGridHeaderAction(width: 200)
            };
        }

        public override IOrderedQueryable<ConvertLogApi_View> GetSearchQuery()
        {
            var query = DC.Set<ConvertLog>()
                .CheckContain(Searcher.SourceFileName, x=>x.SourceFileName)
                .CheckContain(Searcher.DistFileName, x=>x.DistFileName)
                .CheckContain(Searcher.ConvertTime, x=>x.ConvertTime)
                .CheckContain(Searcher.UserName, x=>x.UserName)
                .CheckContain(Searcher.ConvertStatus, x=>x.ConvertStatus)
                .Select(x => new ConvertLogApi_View
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

    public class ConvertLogApi_View : ConvertLog{

    }
}
