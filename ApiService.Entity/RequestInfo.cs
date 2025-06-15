using System;
using System.Collections.Generic;
using System.Text;

namespace ApiService.Entity
{
    public class RequestInfo
    {
        public string Command { get; set; }
        public object Data { get; set; }
    }
}
