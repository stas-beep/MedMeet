using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business_logic.Filters
{
    public class UserFilterDto
    {
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public int? SpecialtyId { get; set; }
        public int? CabinetId { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
