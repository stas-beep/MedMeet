using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business_logic.Auth
{
    public class RegisterUserDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string UserName { get; set; }
    }
}
