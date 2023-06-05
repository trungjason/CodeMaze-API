using Contacts.Interfaces;
using Entities.LinkModels;
using Entities.Models;
using Microsoft.Net.Http.Headers;
using Shared.DataTransferObjects;

namespace CodeMaze_API.Utility
{
    public class EmployeeLinks : IEmployeeLinks
    {
        // We are going to use LinkGenerator to generate links for our responses 
        // and IDataShaper to shape our data
        private readonly LinkGenerator _linkGenerator;
        private readonly IDataShaper<EmployeeDTO> _dataShaper;

        public EmployeeLinks(LinkGenerator linkGenerator, IDataShaper<EmployeeDTO> dataShaper)
        {
            _linkGenerator = linkGenerator;
            _dataShaper = dataShaper;
        }

        // This method will be call to generate response, and it will determine whether or not should generate links
        public LinkResponse TryGenerateLinks(
            IEnumerable<EmployeeDTO> employeesDto,
            string fields,
            Guid companyId,
            HttpContext httpContext)
        {
            var shapedEmployees = ShapeData(employeesDto, fields);

            if (ShouldGenerateLinks(httpContext))
                return ReturnLinkdedEmployees(employeesDto, fields, companyId, httpContext, shapedEmployees);

            return ReturnShapedEmployees(shapedEmployees);
        }

        private List<Entity> ShapeData(IEnumerable<EmployeeDTO> employeesDto, string fields)
        {
            // Shaped Data with Fields QueryString and select only Entity to return to client
            return _dataShaper
                    .ShapeData(employeesDto, fields)
                    .Select(e => e.Entity)
                    .ToList();
        }

        private bool ShouldGenerateLinks(HttpContext httpContext)
        {
            var mediaType = (MediaTypeHeaderValue)httpContext.Items["AcceptHeaderMediaType"];

            if (mediaType == null) return false;

            return mediaType.SubTypeWithoutSuffix.EndsWith("hateoas", StringComparison.InvariantCultureIgnoreCase);
        }

        private LinkResponse ReturnShapedEmployees(List<Entity> shapedEmployees) 
            => new LinkResponse { ShapedEntities = shapedEmployees };

        private LinkResponse ReturnLinkdedEmployees(
            IEnumerable<EmployeeDTO> employeesDto,
            string fields, 
            Guid companyId, 
            HttpContext httpContext, 
            List<Entity> shapedEmployees
            )
        {
            var employeeDtoList = employeesDto.ToList();

            for (var index = 0; index < employeeDtoList.Count(); index++)
            {
                var employeeLinks = CreateLinksForEmployee(httpContext, companyId, employeeDtoList[index].Id, fields);
                shapedEmployees[index].Add("Links", employeeLinks);
            };

            var employeeCollection = new LinkCollectionWrapper<Entity>(shapedEmployees);

            var linkedEmployees = CreateLinksForEmployees(httpContext, employeeCollection);

            return new LinkResponse { HasLinks = true, LinkedEntities = linkedEmployees };
        }

        private List<Link> CreateLinksForEmployee(
            HttpContext httpContext,
            Guid companyId,
            Guid id,
            string fields = ""
            )
        {
            var links = new List<Link> {
                new Link(
                    _linkGenerator.GetUriByAction(
                        httpContext,
                        "GetEmployeeForCompany",
                        values: new { companyId, id, fields }
                        ),
                    "self",
                    "GET"),

                new Link(
                    _linkGenerator.GetUriByAction(
                        httpContext,
                        "DeleteEmployeeForCompany",
                        values: new { companyId, id }
                        ),
                    "delete_employee",
                    "DELETE"),

                new Link(
                    _linkGenerator.GetUriByAction(
                        httpContext,
                        "UpdateEmployeeForCompany",
                        values: new { companyId, id }
                        ),
                    "update_employee",
                    "PUT"),

                new Link(
                    _linkGenerator.GetUriByAction(
                        httpContext,
                        "PartiallyUpdateEmployeeForCompany",
                        values: new { companyId, id }
                        ),
                    "partially_update_employee",
                    "PATCH")
            };

            return links;
        }


        private LinkCollectionWrapper<Entity> CreateLinksForEmployees(
            HttpContext httpContext, LinkCollectionWrapper<Entity> employeesWrapper)
        {
            employeesWrapper.Links.Add(
                new Link(
                    _linkGenerator.GetUriByAction(
                        httpContext, 
                        "GetEmployeesForCompany", 
                        values: new { }
                    ),
                    "self",
                    "GET"
                )
            );

            return employeesWrapper;
        }
    }
}
