using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Okta.Auth.Sdk;

namespace okta_aspnet_mvc_example.Models
{
    public class SelectFactorViewModel
    {
        public List<FactorViewModel> Factors { get; set; }

        public string FactorType { get; set; }
    }
}
