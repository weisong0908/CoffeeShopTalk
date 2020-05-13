using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace CoffeeShopTalk.Pages
{
    public class PrivacyModel : PageModel
    {
        public string IpAddress { get; set; }
        public string PortNumber { get; set; }
        public string Scheme { get; set; }
        public string Host { get; set; }
        private readonly ILogger<PrivacyModel> _logger;

        public PrivacyModel(ILogger<PrivacyModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
            IpAddress = HttpContext.Connection.RemoteIpAddress.ToString();
            PortNumber = HttpContext.Connection.RemotePort.ToString();
            Scheme = HttpContext.Request.Scheme;
            Host = HttpContext.Request.Host.Value;
        }
    }
}
