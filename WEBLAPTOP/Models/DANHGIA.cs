namespace WEBLAPTOP.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("DANHGIA")]
    public partial class DANHGIA
    {
        [Key]
        public int ID_GH { get; set; }

        [Required(ErrorMessage = "Nội dung đánh giá không được bỏ trống")]
        [StringLength(2000, ErrorMessage = "Nội dung tối đa 2000 ký tự")]
        [RegularExpression(@"^[a-zA-Z0-9À-ỹ\s.,!?-]+$",
        ErrorMessage = "Nội dung chứa ký tự không hợp lệ")]
        public string NoiDung { get; set; }

        [Required(ErrorMessage = "Điểm đánh giá không được bỏ trống")]
        [Range(1, 5, ErrorMessage = "Điểm phải từ 1 đến 5")]
        public int? Diem { get; set; }

        public int? ID_KH { get; set; }

        public int? ID_SP { get; set; }

        public virtual KHACHHANG KHACHHANG { get; set; }

        public virtual SANPHAM SANPHAM { get; set; }
    }
}
