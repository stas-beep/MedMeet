using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database.Models
{
    public class User
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }

        public int? SpecialtyId { get; set; }
        public Specialty Specialty { get; set; }
        public int? CabinetId { get; set; }
        public Cabinet Cabinet { get; set; }

        public ICollection<Record> RecordAsPatient { get; set; }
        public ICollection<Record> RecordAsDoctor { get; set; }
    }
}
