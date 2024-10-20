using System.Reflection;
using System.Web;

namespace Fricks.ViewModels.ResponseModels
{
    public class ResponseModel<T>
    {
        public int HttpCode { get; set; } = 200;
        public string Message { get; set; } = "";
        public T Data { get; set; } = default(T);

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
