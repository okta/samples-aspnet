using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace okta_aspnet_mvc_example.Models
{
    public class ForgotPasswordViewModel
    {
        [Required]
        public string UserName { get; set; }
    }
}
