using Nest;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace QLCUNL.Models
{
    public class Label:BaseET
    {
        
        [Keyword]
        [DisplayName("Label ID")]
        public string id_label { get; set; }
        [DisplayName("Tên Label")]
        public string ten_label { get; set; }
        public Label()
        {
            ngay_tao = XMedia.XUtil.TimeInEpoch(DateTime.Now);
            ngay_sua = XMedia.XUtil.TimeInEpoch(DateTime.Now);
        }
    }
}
