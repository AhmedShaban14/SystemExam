using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ExamSystem.Models
{
    public class UserInfo
    {
        [Key]
        public int Id { get; set; }
        public string Email { get; set; }

    }
}