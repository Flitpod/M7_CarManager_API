using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace M7_CarClient.Model
{
    public class RegisterViewModel
    {
        public string Email { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Password { get; set; }
        public string? PhotoContentType { get; set; }
        public byte[]? PhotoData { get; set; }
    }
}
