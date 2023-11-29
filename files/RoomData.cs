using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlatShareCostCalculator
{
    internal class RoomData
    {
        public int id { get; set; }
        public string name { get; set; }

        public double area { get; set; }

        public RoomData()
        {
            id = 0;
            name = string.Empty;
            area = 0;
        }
    }
}
