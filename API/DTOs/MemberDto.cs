using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.DTOs
{
    // MemberDto to avoid a circle cycle error where AppUser has a Photo class, and vice versa
    public class MemberDto
    {
        public int Id { get; set; }
        // AutoMapper is smart enough to connect it with UserName from AppUser entity
        public string Username { get; set; }  
        // Photo Url will be a little tricky for AutoMapper to work out, so we add configurations to AutoMapperProfiles.cs
        public string PhotoUrl { get; set; }

        public int Age { get; set; }

        public string KnownAs { get; set; }
        public DateTime Created { get; set; } 

        public DateTime LastActive { get; set; }

        public string Gender { get; set; }

        public string Introduction { get; set; }

        public string LookingFor { get; set; }

        public string Interests { get; set; }

        public string City { get; set; }

        public string Country { get; set; }

        // One-to-many // One user can have many photos
        // Entity Framework adds a AppUserId to Photos table because it recognized the relationship
        public ICollection<PhotoDto> Photos { get; set; }
    }
}