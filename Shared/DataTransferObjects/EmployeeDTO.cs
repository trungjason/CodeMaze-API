namespace Shared.DataTransferObjects
{
    // We can add Serializable or set Property to {get; init;} to make record convertable to XML
    //[Serializable]
    public record EmployeeDTO
    {
        public Guid Id { get; init; }

        public string? Name  { get; init; }

        public int Age  { get; init; }

        public string? Position { get; init; }
    };

    public record CreateEmployeeDTO(string Name, int Age, string Position);
}
