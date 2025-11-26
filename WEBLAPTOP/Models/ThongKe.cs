using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WEBLAPTOP.Models
{
	public class ThongKe
	{
        public int TongKhachHang { get; set; }
        public int TongSanPham { get; set; }
        public int TongDonHang { get; set; }
        public int SanPhamSapHet { get; set; } 

        public List<DonHangHienThi> DonHangMoiNhat { get; set; }

        public List<KHACHHANG> KhachHangMoi { get; set; }

        public List<decimal> DoanhThu6Thang { get; set; } 
        public List<string> NhanThang { get; set; }
    }
    public class DonHangHienThi
    {
        public int ID_DH { get; set; }
        public string TenKhachHang { get; set; }
        public decimal TongTien { get; set; }
        public string TrangThai { get; set; }
    }
}