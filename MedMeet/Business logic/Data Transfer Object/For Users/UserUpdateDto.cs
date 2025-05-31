using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business_logic.Data_Transfer_Object.For_Users
{
    public class UserUpdateDto
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string? Password { get; set; }
        public string Role { get; set; }
        public int? SpecialtyId { get; set; }
        public int? CabinetId { get; set; }
    }
}
