namespace shopthoitrang.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Messages
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int id_chat { get; set; }

        public int? ConversationID { get; set; }

        public int? UserID { get; set; }

        public string message { get; set; }

        public DateTime? Timestamp { get; set; }

        [StringLength(50)]
        public string status { get; set; }

        public virtual Conversations Conversations { get; set; }
    }
}
