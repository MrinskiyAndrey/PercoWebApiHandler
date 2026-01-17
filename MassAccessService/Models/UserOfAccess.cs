using System;
using System.Collections.Generic;
using System.Text;

namespace MassAccessService.Models
{
    public class UserOfAccess
    {
        public int? Id { get; set; }
        public string? FIO { get; set; }
        public string? TabNumber { get; set; }
        public string? Department { get; set; }
    }
}
