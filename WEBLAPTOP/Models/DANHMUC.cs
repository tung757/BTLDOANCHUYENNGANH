namespace WEBLAPTOP.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("DANHMUC")]
    public partial class DANHMUC
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public DANHMUC()
        {
            SANPHAMs = new HashSet<SANPHAM>();
        }

        [Key]
        public int ID_DM { get; set; }

        [Required(ErrorMessage = "Tên danh mục không được bỏ trống")]
        [StringLength(255, ErrorMessage = "Tên danh mục tối đa 255 ký tự")]
        [RegularExpression(@"^[a-zA-Z0-9À-ỹ\s.,!?-]+$",
        ErrorMessage = "Tên danh mục chứa ký tự không hợp lệ")]
        public string TenDM { get; set; }


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SANPHAM> SANPHAMs { get; set; }
    }
}
