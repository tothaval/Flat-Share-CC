using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlatShareCostCalculator
{
    internal class FlatData
    {
        public int flat_id { get; set; }
        public string flat_address { get; set; }
        public double flat_space { get; set; }

        public ObservableCollection<RoomData> rooms { get; set; }

        public FlatData()
        {
            rooms = new ObservableCollection<RoomData> { };
        }
    }
}
