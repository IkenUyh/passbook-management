using System;
using System.Collections.Generic;
using System.Text;

namespace frontend_csharp.Models.Auth
{
    public class ErrorResponse
    {
        public int Status { get; set; }
        public string Error { get; set; }
        public string Message { get; set; } 
    }
}
