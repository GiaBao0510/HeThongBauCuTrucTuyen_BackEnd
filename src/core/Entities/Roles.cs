using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using BackEnd.core.Interfaces;

namespace BackEnd.core.Entities
{
    public class Roles : IRole
    {
        [Key]
        [DisplayName("Role definition")]
        public int RoleID { set; get; }
        [Required]
        [DisplayName("Role name")]
        [StringLength(30)]
        public string TenVaiTro { set; get; }
    }
}