using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyncLight.Models
{
    public class MenuBar
    {
        /// <summary>
        /// 主键
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 图标
        /// </summary>
        public string Icon {  get; set; }

        /// <summary>
        /// 显示名称
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// 视图名称
        /// </summary>
        public Type ViewName { get; set; }

        /// <summary>
        /// 提示
        /// </summary>
        public string Tip { get; set; }
    }
}
