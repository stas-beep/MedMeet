using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business_logic.Data_Transfer_Object.For_Prescription
{
    public class PrescriptionReadDto
    {
        public int Id { get; set; }
        public int RecordId { get; set; }
        public string Medication { get; set; }
        public string Dosage { get; set; }
        public string Instructions { get; set; }
    }
}
