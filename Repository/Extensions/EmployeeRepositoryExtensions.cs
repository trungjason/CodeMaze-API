using Entities.Models;
using Repository.Extensions.Utility;
using System.Linq.Dynamic.Core;
using System.Reflection;
using System.Text;

namespace Repository.Extensions
{
    public static class EmployeeRepositoryExtensions
    {
        public static IQueryable<Employee> FilterEmployeesByAgeRange(
            this IQueryable<Employee> employees, uint MinAge, uint MaxAge)
        {
            return employees.Where(employee => employee.Age >= MinAge
                                && employee.Age <= MaxAge);
        }

        public static IQueryable<Employee> SearchEmployeeByName(this IQueryable<Employee> employees, string? searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return employees;

            var lowerCaseTerm = searchTerm.Trim().ToLower();

            return employees.Where(e => e.Name == null ? false : e.Name.ToLower().Contains(lowerCaseTerm));
        }

        public static IQueryable<Employee> Sort(this IQueryable<Employee> employees, string? orderByQueryString)
        {
            if (string.IsNullOrWhiteSpace(orderByQueryString))
                return employees.OrderBy(employee => employee.Name);

            
            var orderQuery = OrderQueryBuilder.CreateOrderQuery<Employee>(orderByQueryString);

            if (string.IsNullOrWhiteSpace(orderQuery))
                return employees.OrderBy(e => e.Name);

            // OrderQuery = “Name ascending, DateOfBirth descending”
            // System.Linq.Dynamic.Core.OrderBy will translate the abrove OrderQuery to
            // employees.OrderBy(e => e.Name).ThenByDescending(o => o.Age);
            return employees.OrderBy(orderQuery);
        }
    }
}
