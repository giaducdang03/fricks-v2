using System.ComponentModel.DataAnnotations;

namespace Fricks.ViewModels.RequestModels
{
    public class DateRequestModel
    {
        [Required]
        public int Year { get; set; } = 0;

        [Required]
        public int Month { get; set; } = 0;
    }
}
