namespace Shared.RequestFeatures
{
    public class EmployeeParameters : RequestParameters
    {
        public EmployeeParameters() => OrderBy = "name";

        public uint MinAge { get; set; }

        public uint MaxAge { get; set; } = Int32.MaxValue;

        public bool IsValidAgeRange => MaxAge > MinAge;

        // Searching QueryString
        public string? SearchTerm { get; set; }
    }
}
