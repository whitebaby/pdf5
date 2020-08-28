using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WalkingTec.Mvvm.Core;

namespace pdf5.Models
{
    public class UnitWork : BasePoco, ITreeData<UnitWork>
    {
        /// </summary>
        [Display(Name = "部门名称")]
        public string UnitWorkName { get; set; }

        public List<UnitWork> Children { get; set; }

        [Display(Name = "上级部门")]
        public UnitWork Parent { get; set; }

        [Display(Name = "上级部门")]
        public Guid? ParentId { get; set; }

    }
}
