namespace WEBLAPTOP.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("KHACHHANG")]
    public partial class KHACHHANG
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public KHACHHANG()
        {
            DANHGIAs = new HashSet<DANHGIA>();
            DONHANGs = new HashSet<DONHANG>();
            GIOHANGs = new HashSet<GIOHANG>();
        }

        [Key]
        public int ID_KH { get; set; }

        [Required(ErrorMessage = "Tên khách hàng không được bỏ trống")]
        [StringLength(50)]
        [RegularExpression(@"^[a-zA-ZÀ-ỹ\s]+$",
        ErrorMessage = "Tên chỉ được chứa chữ cái")]
        public string TenKH { get; set; }

        [Required(ErrorMessage = "Địa chỉ không được bỏ trống")]
        [StringLength(255)]
        public string DiaChi { get; set; }

        [Required(ErrorMessage = "Số điện thoại không được bỏ trống")]
        [StringLength(10)]
        [RegularExpression(@"^0[0-9]{9}$",
            ErrorMessage = "Số điện thoại phải gồm 10 số và bắt đầu bằng 0")]
        public string SDT { get; set; }

        [StringLength(5)]
        [RegularExpression(@"^(Nam|Nữ|Khác)?$",
            ErrorMessage = "Giới tính không hợp lệ")]
        public string GioTinh { get; set; }

        [Column(TypeName = "date")]
        public DateTime? NgaySinh { get; set; }

        [Required(ErrorMessage = "Email không được bỏ trống")]
        [StringLength(50)]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Tài khoản không được bỏ trống")]
        [StringLength(20)]
        [RegularExpression(@"^[a-zA-Z0-9]+$",
            ErrorMessage = "Tài khoản chỉ chứa chữ và số")]
        public string TK { get; set; }

 
        [StringLength(20)]
        [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d]{6,20}$",
            ErrorMessage = "Mật khẩu phải từ 6–20 ký tự và có ít nhất 1 chữ + 1 số")]
        public string MK { get; set; }

        public int? PhanQuyen { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DANHGIA> DANHGIAs { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DONHANG> DONHANGs { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<GIOHANG> GIOHANGs { get; set; }
    }
}
