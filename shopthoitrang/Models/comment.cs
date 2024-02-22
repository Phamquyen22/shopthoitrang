namespace shopthoitrang.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("comment")]
    public partial class comment
    {
        [Key]
        [StringLength(50)]
        public string id_comment { get; set; }

        [StringLength(50)]
        public string id_product { get; set; }

        [Column("comment")]
        public string comment1 { get; set; }

        [StringLength(50)]
        public string id_user { get; set; }

        public string photo { get; set; }

        [StringLength(10)]
        public string date_post { get; set; }

        public virtual product product { get; set; }
    }
}
