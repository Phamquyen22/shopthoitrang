namespace shopthoitrang.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Account")]
    public partial class Account
    {
        [Key]
        [StringLength(50)]
        public string taikhoan { get; set; }

        [Required]
        [StringLength(50)]
        public string matkhau { get; set; }

        [StringLength(50)]
        public string acc_lock { get; set; }

        [Required]
        [StringLength(50)]
        public string id_user { get; set; }

        [StringLength(50)]
        public string role { get; set; }

        public virtual User_info User_info { get; set; }
    }
}
