using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace apbd_projekt.Shared
{
    public class Stock
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        public string Ticker { get; set; }
        public string CompanyName { get; set; }
        public string Description { get; set; }
        public string Industry { get; set; }
        public string Country { get; set; }
        public string LogoURL { get; set; }

        public ICollection<StockDay> StockDays { get; set; }

    }
}
