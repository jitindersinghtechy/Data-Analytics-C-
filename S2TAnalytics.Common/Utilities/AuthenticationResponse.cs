using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S2TAnalytics.Common.Utilities
{
    public class AuthenticationResponse
    {
        public string Message { get; set; }
        public bool Success { get; set; }
        public Object Data { get; set; }

    }
}
