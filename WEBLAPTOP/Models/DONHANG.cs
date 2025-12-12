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
        //[RegularExpression(@"^[a-zA-Z0-9À-ỹ\s.,!?-]*$",
        //    ErrorMessage = "Ghi chú chứa ký tự không hợp lệ")]
        public string GhiChu { get; set; }

        [StringLength(30)]
        //[RegularExpression(@"^[a-zA-Z0-9À-ỹ\s.,!?-]*$",
        //    ErrorMessage = "Trạng thái chứa ký tự không hợp lệ")]
        public string TrangThai { get; set; }

        public int? ID_KH { get; set; }

        public int? ID_KM { get; set; }

        //[Required(ErrorMessage = "Tên người nhận không được bỏ trống")]
        //[StringLength(255, ErrorMessage = "Tên tối đa 255 ký tự")]
        //[RegularExpression(@"^[a-zA-ZÀ-ỹ\s]+$",
        //    ErrorMessage = "Tên chỉ được phép chứa chữ cái")]
        public string Ten { get; set; }

        //[Required(ErrorMessage = "Địa chỉ giao hàng không được bỏ trống")]
        [StringLength(4000)]
        public string DiaChiGiaoHang { get; set; }

        //[Required(ErrorMessage = "Số điện thoại không được bỏ trống")]
        //[StringLength(10)]
        //[RegularExpression(@"^0[0-9]{9}$", ErrorMessage = "Số điện thoại phải gồm 10 số và bắt đầu bằng 0")]
        public string SDT { get; set; }

        [StringLength(255)]
        public string PhuongthucTT { get; set; }

        [StringLength(1000)]
        public string PhuongThucNhanHang { get; set; }

        public virtual KHACHHANG KHACHHANG { get; set; }

        public virtual KHUYENMAI KHUYENMAI { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DONHANG_SANPHAM> DONHANG_SANPHAM { get; set; }
    }
}
