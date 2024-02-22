namespace shopthoitrang.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Voucher_user
    {
        [Key]
        [StringLength(50)]
        public string id_voucher { get; set; }

        [StringLength(50)]
        public string id_discount { get; set; }

        [StringLength(50)]
        public string id_user { get; set; }

        [StringLength(50)]
        public string status_use { get; set; }

        [StringLength(50)]
        public string date_use { get; set; }

        public virtual Discount_code Discount_code { get; set; }

        public virtual User_info User_info { get; set; }
    }
}
