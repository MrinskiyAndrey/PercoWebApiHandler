using System;
using System.Collections.Generic;
using System.Text;

namespace NewEmployeesService.Models
{
    public class Employee
    {
        public string Position { get; set; } = "(не определена)";
        public string Division { get; set; } = "(не определено)";
        public string? TabelNumber { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? MiddleName { get; set; }
    }
}
