using Nest;
using System;
using System.Collections.Generic;
using System.Text;

namespace QLCUNL.Models
{
    public class Menu : BaseET
    {
        [Keyword]
        public string id_menu { get; set; }
        public string ten_menu { get; set; }
        public string url { get; set; }
        public int order { get; set; }
    }
}
