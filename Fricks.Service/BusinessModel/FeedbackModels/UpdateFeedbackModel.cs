using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fricks.Service.BusinessModel.FeedbackModels
{
    public class UpdateFeedbackModel
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public int ProductId { get; set; }

        public string? Image { get; set; }

        public string? Content { get; set; }

        [Required]
        [RegularExpression("^[1-5]$", ErrorMessage = "Điểm đánh giá phải trong khoảng từ 1 đến 5")]
        public int Rate { get; set; }
    }
}
