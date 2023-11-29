using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlatShareCostCalculator
{
    internal class BillingPeriodData
    {
        public RoomData room { get; set; }

        public double heating_units_usage { get; set; }

        public List<double> monthly_payments { get; set; }

        public BillingPeriodData(RoomData _room)
        {
            room = _room;

            heating_units_usage = 0;
            monthly_payments = new List<double>();
        }
    }
}
