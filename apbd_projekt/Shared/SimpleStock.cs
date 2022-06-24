using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace apbd_projekt.Shared
{
    public class SimpleStock
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        public string Ticker { get; set; }
        public string Name { get; set; }
        public string PrimaryExchange { get; set; }
    }
}
