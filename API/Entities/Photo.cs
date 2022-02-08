using System.ComponentModel.DataAnnotations.Schema;

namespace API.Entities
{
    // when Entity Framework creates this table, it'll call it "Photos"
    [Table("Photos")]
    public class Photo
    {
        public int Id { get; set; }

        public string Url { get; set; }

        public bool IsMain { get; set; }

        public string PublicId { get; set; }

        // add these two property to fully define the relationship between AppUser and Photos for Entity Framework

        public AppUser AppUser { get; set; }

        public int AppUserId { get; set; }
    }
}