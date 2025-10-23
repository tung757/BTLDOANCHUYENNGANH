using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WEBLAPTOP.Models;

namespace WEBLAPTOP.ViewModel
{
    public class DonHangView
    {
        public DONHANG DONHANG { get; set; }
        public List<DONHANG_SANPHAM> DONHANG_SANPHAM  { get; set; }
    }
}