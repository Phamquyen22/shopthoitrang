namespace shopthoitrang.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class best_pro
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int id_best_pro { get; set; }

        public int? id_pro { get; set; }

        [StringLength(50)]
        public string date_end { get; set; }

        public virtual product product { get; set; }
    }
}
