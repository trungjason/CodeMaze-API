namespace Shared.DataTransferObjects
{

    // We can add Serializable or set Property to {get; init;} to make record convertable to XML
    //[Serializable]
    public record CompanyDTO
    {
        public Guid Id { get; init; }
        
        public string? Name { get; init; }
        
        public string? FullAddress { get; init; }
    };

    public record CreateCompanyDTO (string Name, string Address, string Country, IEnumerable<CreateEmployeeDTO> Employees);
}
