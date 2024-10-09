using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Fricks.Service.BusinessModel.PaymentModels
{
    public class PayOSResponseModel
    {
        public string code { get; set; } = string.Empty;    
        public string id { get; set; } = string.Empty;
        public bool cancel { get; set; }
        public string status { get; set; } = string.Empty;
        public string orderCode { get; set; } = string.Empty;
        public string ToUrlParameters()
        {
            var properties = GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);
            var queryString = new List<string>();

            foreach (var property in properties)
            {
                var value = property.GetValue(this);
                if (value != null)
                {
                    queryString.Add($"{property.Name}={HttpUtility.UrlEncode(value.ToString())}");
                }
            }

            return string.Join("&", queryString);
        }
    }
}
