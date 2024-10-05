using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fricks.Service.Settings
{
    public class PayOSSetting
    {
        public required string ClientId { get; set; }
        public required string ApiKey { get; set; }
        public required string ChecksumKey { get; set; }
        public required string ReturnUrl { get; set; }
        public required string CancelUrl { get; set;}
    }
}
