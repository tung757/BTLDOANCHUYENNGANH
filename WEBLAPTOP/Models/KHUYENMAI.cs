namespace WEBLAPTOP.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("KHUYENMAI")]
    public partial class KHUYENMAI
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public KHUYENMAI()
        {
            DONHANGs = new HashSet<DONHANG>();
        }

        [Key]
        public int ID_KM { get; set; }

        [Required(ErrorMessage = "Giảm giá không được bỏ trống")]
        public int? GiamGia { get; set; }

        [StringLength(4000)]
        [RegularExpression(@"^[a-zA-Z0-9À-ỹ\s.,!?-]*$",
            ErrorMessage = "Mô tả chứa ký tự không hợp lệ")]
        public string Mota { get; set; }

        public int? TrangThai { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DONHANG> DONHANGs { get; set; }
    }
}
