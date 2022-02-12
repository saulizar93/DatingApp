using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.DTOs
{
    //this is the object we're going to return when user registers
    public class UserDto
    {
        public string Username { get; set; }
        public string Token { get; set; }

        // photoUrl to their main photo
        public string PhotoUrl { get; set; }
        public string KnownAs { get; set; }
        public string Gender { get; set; }
        
    }
}