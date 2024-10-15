using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fricks.Service.BusinessModel.BannerModels
{
    public class BannerModel
    {
        public int Id { get; set; }

        public string? Name { get; set; }

        public string? Image { get; set; }

        public int Index { get; set; }
    }
}
