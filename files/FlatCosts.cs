using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlatShareCostCalculator
{
    internal class FlatCosts
    {
        public double cold_rent { get; set; }
        public double extra_costs_advance { get; set; }

        public FlatCosts()
        {
            cold_rent= 0;
            extra_costs_advance= 0; 
        }
    }
}
