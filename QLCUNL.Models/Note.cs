using Nest;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace QLCUNL.Models
{
    public class Note : BaseET
    {
        public string id { get; set; }
        public LoaiNote loai { get; set; }        
        public string id_obj { get; set; }
        public List<int> thuoc_tinh { get; set; }
        public string noi_dung { get; set; }
        public LoaiDuLieu loai_du_lieu { get; set; }
    }
}
