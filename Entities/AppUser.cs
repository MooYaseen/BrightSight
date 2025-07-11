﻿using Microsoft.AspNetCore.Identity;

namespace Graduation.Entities
{
    public class AppUser: IdentityUser<int>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string KnownAs { get; set; }
        public DateTime Created { get; set; } = DateTime.Now;
        public DateTime LastActive { get; set; } = DateTime.Now;
        public string Gender { get; set; }
        // public List<Photo> Photos { get; set; } = new();
        public string City { get; set; }
        public string Country { get; set; }
        public string Role { get; set; }
        public ICollection<AppUserRole> UserRoles { get; set; }

        public List<Message> MessagesSent { get; set; }
        public List<Message> MessagesReceived { get; set; }

        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public string DisplayName { get; set; }


        public ICollection<Photo> Photos { get; set; } = new List<Photo>();
    }
}
