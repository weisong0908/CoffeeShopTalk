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
        public string RemoteIpAddress { get; set; }
        public string RemotePortNumber { get; set; }
        public string LocalIpAddress { get; set; }
        public string LocalPortNumber { get; set; }
        public string Scheme { get; set; }
        public string Host { get; set; }
        private readonly ILogger<PrivacyModel> _logger;

        public PrivacyModel(ILogger<PrivacyModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
            RemoteIpAddress = HttpContext.Connection.RemoteIpAddress.ToString();
            RemotePortNumber = HttpContext.Connection.RemotePort.ToString();
            LocalIpAddress = HttpContext.Connection.LocalIpAddress.ToString();
            LocalPortNumber = HttpContext.Connection.LocalPort.ToString();
            Scheme = HttpContext.Request.Scheme;
            Host = HttpContext.Request.Host.Value;
        }
    }
}
