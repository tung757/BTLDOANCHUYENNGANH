namespace WEBLAPTOP.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("SANPHAM")]
    public partial class SANPHAM
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public SANPHAM()
        {
            DANHGIAs = new HashSet<DANHGIA>();
            DONHANG_SANPHAM = new HashSet<DONHANG_SANPHAM>();
            GIOHANG_SANPHAM = new HashSet<GIOHANG_SANPHAM>();
        }

        [Key]
        public int ID_SP { get; set; }

        [StringLength(20)]
        public string MaSP { get; set; }

        [StringLength(255)]
        public string TenSP { get; set; }

        public int? Gia { get; set; }

        public int? GiaBan { get; set; }

        public string Mota { get; set; }

        public int? Status_SP { get; set; }

        [Column(TypeName = "date")]
        public DateTime? NgayTao { get; set; }

        public int? SoLuong { get; set; }

        public int? SoLuongBan { get; set; }

        [StringLength(255)]
        public string Images_url { get; set; }

        public int? ID_DM { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DANHGIA> DANHGIAs { get; set; }

        public virtual DANHMUC DANHMUC { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DONHANG_SANPHAM> DONHANG_SANPHAM { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<GIOHANG_SANPHAM> GIOHANG_SANPHAM { get; set; }
    }
}
