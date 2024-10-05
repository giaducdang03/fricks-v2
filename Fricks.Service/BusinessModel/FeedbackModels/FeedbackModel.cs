using Fricks.Repository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fricks.Service.BusinessModel.FeedbackModels
{
    public class FeedbackModel : BaseEntity
    {
        public int? UserId { get; set; }

        public int? ProductId { get; set; }

        public string? Image { get; set; }

        public string? Content { get; set; }

        public int? Rate { get; set; }

        public string? UserName { get; set; }

        public string? ProductName { get; set; }
    }
}
