using System.ComponentModel.DataAnnotations;

namespace MyApp.WebMVC.Models
{
    public class FileLinkViewModel
    {
        public Guid Id { get; set; }

        [Display(Name = "File URL")]
        [DataType(DataType.Url)]
        public string Url { get; set; }

        [Display(Name = "File Description (optional)")]
        [StringLength(20, ErrorMessage = "File Description cannot be longer than 20 characters.")]
        public string? FileDescription { get; set; }



        [Required(ErrorMessage = "Price is required.")]
        [DataType(DataType.Currency)]
        [Range(20, double.MaxValue, ErrorMessage = "Price must be greater than 20.")]
        [DisplayFormat(DataFormatString = "{0:F2}", ApplyFormatInEditMode = true)]
        [Display(Name = "Price")]
        public decimal Price { get; set; }

        [Display(Name = "Status")]
        public string? StatusDisplay { get; set; }

        [Display(Name = "Created At")]
        [DataType(DataType.DateTime)]
        public DateTimeOffset CreatedAt { get; set; }

        [Display(Name = "Updated At")]
        [DataType(DataType.DateTime)]
        public DateTimeOffset UpdatedAt { get; set; }

        public string? UniqueCode { get; set; }
        public int SellsCount { get; set; }

        public Guid UserId { get; set; }
    }
}
