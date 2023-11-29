using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlatShareCostCalculator
{
    internal class AUX_Calculations
    {
        FlatData flatData;
        FlatCosts flatCosts;

        CostUpdateData costUpdateData;

        public AUX_Calculations(CostUpdateData costUpdate)
        {
            this.costUpdateData = costUpdate;

            this.flatData = this.costUpdateData.get_FlatData();
            this.flatCosts = this.costUpdateData.get_FlatCosts();
        }


        public AUX_Calculations(FlatData _flatData, FlatCosts _flatCosts)
        {
            this.flatData = _flatData;
            this.flatCosts = _flatCosts;

            costUpdateData = new CostUpdateData(flatData, flatCosts);
        }



        public double shared_space()
        {
            double shared_space = flatData.flat_space;

            foreach (RoomData room in flatData.rooms)
            {
                shared_space -= room.area;
            }

            return shared_space;
        }


    }
}
