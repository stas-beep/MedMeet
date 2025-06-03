using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System.Text;
using System.Threading.Tasks;

namespace Database.Models
{
    public class User : IdentityUser<int>
    {
        public string FullName { get; set; }
        public int? SpecialtyId { get; set; }
        public Specialty Specialty { get; set; }
        public int? CabinetId { get; set; }
        public Cabinet Cabinet { get; set; }

        public ICollection<Record> RecordAsPatient { get; set; }
        public ICollection<Record> RecordAsDoctor { get; set; }
    }
}
