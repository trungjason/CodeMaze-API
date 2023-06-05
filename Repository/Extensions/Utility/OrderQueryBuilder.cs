using Entities.Models;
using System.Reflection;
using System.Text;

namespace Repository.Extensions.Utility
{
    public static class OrderQueryBuilder
    {
        public static string CreateOrderQuery<T>(string orderByQueryString)
        {
            // orderByQueryString will contain value that have format like 
            // [propertyName] [asc]|[desc]|[],[propertyName] [asc]|[desc][]...

            // Split by ',' => We get [propertyName] [asc]|[desc]|[]
            var orderParams = orderByQueryString.Trim().Split(',');

            // Get All property info of T class
            var propertyInfos = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var orderQueryBuilder = new StringBuilder();

            foreach (var param in orderParams)
            {
                if (string.IsNullOrWhiteSpace(param)) continue;

                // Split by ' ' => We get [propertyName] at 0 and [asc]|[desc]|[] at 1
                var propertyFromQueryName = param.Split(" ")[0];

                // Check if object property is exists 
                var objectProperty = propertyInfos.FirstOrDefault(pi =>
                    pi.Name.Equals(propertyFromQueryName, StringComparison.InvariantCultureIgnoreCase));

                if (objectProperty == null) continue;

                var direction = param.EndsWith(" desc") ? "descending" : "ascending";

                orderQueryBuilder.Append($"{objectProperty.Name.ToString()} {direction}, ");
            }

            // OrderQuery = “Name ascending, DateOfBirth descending”
            // System.Linq.Dynamic.Core.OrderBy will translate the abrove OrderQuery to
            // employees.OrderBy(e => e.Name).ThenByDescending(o => o.Age);
            var orderQuery = orderQueryBuilder.ToString().TrimEnd(',', ' ');
            
            return orderQuery;
        }
    }
}
