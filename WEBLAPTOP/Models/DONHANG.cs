namespace WEBLAPTOP.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("DONHANG")]
    public partial class DONHANG
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public DONHANG()
        {
            DONHANG_SANPHAM = new HashSet<DONHANG_SANPHAM>();
        }

        [Key]
        public int ID_DH { get; set; }

        [Column(TypeName = "date")]
        public DateTime? NgayLap { get; set; }

        [StringLength(255)]
        public string GhiChu { get; set; }

        [StringLength(30)]
        public string TrangThai { get; set; }

        public int? ID_KH { get; set; }

        public int? ID_KM { get; set; }

        public virtual KHACHHANG KHACHHANG { get; set; }

        public virtual KHUYENMAI KHUYENMAI { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DONHANG_SANPHAM> DONHANG_SANPHAM { get; set; }
    }
}
