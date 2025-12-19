namespace WEBLAPTOP.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    using System.Web.Mvc;

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

        [Required(ErrorMessage = "Mã sản phẩm không được để trống")]
        [StringLength(50, ErrorMessage = "Mã sản phẩm tối đa 50 ký tự")]
        public string MaSP { get; set; }

        [Required(ErrorMessage = "Tên sản phẩm không được để trống")]
        [StringLength(255, ErrorMessage = "Tên sản phẩm tối đa 255 ký tự")]
        public string TenSP { get; set; }

        [Required(ErrorMessage = "Giá nhập không được để trống")]
        [Range(0, int.MaxValue, ErrorMessage = "Giá phải >= 0")]
        public int? Gia { get; set; }

        [Required(ErrorMessage = "Giá bán không được để trống")]
        [Range(0, int.MaxValue, ErrorMessage = "Giá bán phải >= 0")]
        public int? GiaBan { get; set; }

        //[StringLength(4000, ErrorMessage = "Mô tả tối đa 4000 ký tự")]
        [AllowHtml] // <-- THÊM DÒNG NÀY TRÊN THUỘC TÍNH MOTA
        public string Mota { get; set; }

        [Required]
        public int? Status_SP { get; set; }

        [Column(TypeName = "date")]
        public DateTime? NgayTao { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Số lượng phải >= 0")]
        public int? SoLuong { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Số lượng bán phải >= 0")]
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
