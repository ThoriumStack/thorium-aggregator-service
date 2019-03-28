﻿using Microsoft.AspNetCore.Identity;

namespace Thorium.Aggregator.Models
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        public bool IsAdmin { get; set; }
        public string DataEventRecordsRole { get; set; }
        public string SecuredFilesRole { get; set; }
        public bool IsInternal { get; set; }
    }
}