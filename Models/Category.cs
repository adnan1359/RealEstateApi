using System.ComponentModel.DataAnnotations;

namespace RealEstateApi.Models
{
    public class Category
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string ImageUrl { get; set; }

        // For establishing one to many relationship with Property Class
        public ICollection<Property> Properties { get; set; }
    }
}
