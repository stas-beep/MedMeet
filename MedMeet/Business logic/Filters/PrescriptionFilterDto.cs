using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business_logic.Filters
{
    public class PrescriptionFilterDto
    {
        public int? RecordId { get; set; }
        public string? Medication { get; set; }
        public string? Dosage { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
