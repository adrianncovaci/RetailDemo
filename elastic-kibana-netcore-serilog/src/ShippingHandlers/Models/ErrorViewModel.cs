using System;

namespace Elastic.Kibana.Serilog.Models
{
    public class ShippingRequest
    {
        public string OrderId { get; set; }
        public string Provider { get; set;}
    }
}
