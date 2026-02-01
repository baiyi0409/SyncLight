using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyncLight.Models
{
    public class ScreenItem
    {
        public int Index { get; set; }

        public string DeviceName { get; set; } = string.Empty;

        public string DisplayName {  get; set; } = string.Empty;

        public Rectangle Bounds { get; set; }

        public Rectangle WorkingArea { get; set; }

        public bool IsPrimary { get; set; }

        public string Resolution { get; set; } = string.Empty;
    }
}
