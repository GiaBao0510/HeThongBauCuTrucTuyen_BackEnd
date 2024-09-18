using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace BackEnd.src.core.Models
{
    public class AppUser: IdentityUser
    {
        //Bổ sung sau...
        public string? password { get; set; }
    }
}