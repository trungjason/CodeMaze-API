﻿using System.ComponentModel.DataAnnotations;

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


    public abstract record EmployeeForManipulationDto
    {
        [Required(ErrorMessage = "Employee name is a required field.")]
        [MaxLength(30, ErrorMessage = "Maximum length for the Name is 30 characters.")]
        public string? Name { get; init; }

        [Range(18, int.MaxValue, ErrorMessage = "Age is required and it can't be lower than 18")]
        public int Age { get; init; }

        [Required(ErrorMessage = "Position is a required field.")]
        [MaxLength(20, ErrorMessage = "Maximum length for the Position is 20 characters.")]
        public string? Position { get; init; }
    }

    public record CreateEmployeeDTO : EmployeeForManipulationDto;

    public record UpdateEmployeeDTO : EmployeeForManipulationDto;
}
