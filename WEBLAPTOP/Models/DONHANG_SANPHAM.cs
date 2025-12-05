namespace WEBLAPTOP.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class DONHANG_SANPHAM
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ID_DH { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ID_SP { get; set; }

        [Required(ErrorMessage = "Số lượng không được bỏ trống")]
        [Range(1, 999, ErrorMessage = "Số lượng phải >= 1")]
        public int? SoLuong { get; set; }

        [Required(ErrorMessage = "Đơn giá không được bỏ trống")]
        [Range(1, int.MaxValue, ErrorMessage = "Đơn giá phải > 0")]
        public int? DonGia { get; set; }

        public virtual DONHANG DONHANG { get; set; }

        public virtual SANPHAM SANPHAM { get; set; }
    }
}
