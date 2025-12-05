using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WEBLAPTOP.Models
{
    public class CartItem
    {
        public int ID_SP { get; set; }
        [StringLength(500, ErrorMessage = "URL ảnh không hợp lệ")]
        public string Images_url { get; set; }

        [Required(ErrorMessage = "Tên sản phẩm không được bỏ trống")]
        [StringLength(200, ErrorMessage = "Tên sản phẩm tối đa 200 ký tự")]
        [RegularExpression(@"^[a-zA-Z0-9À-ỹ\s.,!?-]+$",
            ErrorMessage = "Tên sản phẩm chứa ký tự không hợp lệ")]
        public string TenSP { get; set; }

        [Required(ErrorMessage = "Giá bán không được bỏ trống")]
        [Range(1, int.MaxValue, ErrorMessage = "Giá bán phải lớn hơn 0")]
        public int? GiaBan { get; set; }

        [Required(ErrorMessage = "Số lượng không được bỏ trống")]
        [Range(1, 999, ErrorMessage = "Số lượng phải từ 1 đến 999")]
        public int? SoLuong { get; set; }

        // TongTien có thể để tự tính, không cần nhập
        [Range(1, int.MaxValue)]
        public int? TongTien { get; set; }
    }
}