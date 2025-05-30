using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database.Models
{
    public class Record
    {
        public int Id { get; set; }

        public int PatientId { get; set; }
        public User Patient { get; set; }

        public int DoctorId { get; set; }
        public User Doctor { get; set; }

        public DateTime AppointmentDate { get; set; }
        public string Status { get; set; }

        public string Notes { get; set; }

        public ICollection<Prescription> Prescriptions { get; set; }
    }
}
