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

        [StringLength(2000)]
        public string NoiDung { get; set; }

        public int? Diem { get; set; }

        public int? ID_KH { get; set; }

        public int? ID_SP { get; set; }

        public virtual KHACHHANG KHACHHANG { get; set; }

        public virtual SANPHAM SANPHAM { get; set; }
    }
}
