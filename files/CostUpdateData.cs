using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlatShareCostCalculator
{
    internal class CostUpdateData
    {
        public bool isAnnualBilling { get; set; }
        public bool isRentChange { get; set; }

        public string cause { get; set; }
        public string based_on_AB { get; set; }

        public double initial_cold_rent { get; set; }
        public double initial_extra_costs_advance { get; set; }

        public double cold_rent { get; set; }
        public double extra_costs_advance { get; set; }

        // 
        public double annual_costs { get; set; }
        public double extra_costs_shared { get; set; }
        public double extra_costs_heating { get; set; }

        public double heating_units_usage { get; set; }
        public double heating_units_shared { get; set; }


        
        // room heating units usage and monthly payments
        public ObservableCollection<BillingPeriodData> roomConsumptionValues { get; set; }
        
        public DateTime period_start { get; set; }
        public DateTime period_end { get; set; }
        public DateTime cost_update_received { get; set; }

        private readonly FlatCosts flatCosts;
        private readonly FlatData flatData;


        public CostUpdateData(FlatData data, FlatCosts costs)
        {
            isAnnualBilling = false;
            isRentChange = false;

            based_on_AB = "";
            cause = "";

            flatData = data;
            flatCosts = costs;

            period_start = new DateTime();
            period_end = new DateTime();
            cost_update_received = new DateTime();

            cold_rent = flatCosts.cold_rent;
            extra_costs_advance = flatCosts.extra_costs_advance;

            roomConsumptionValues = new ObservableCollection<BillingPeriodData>();

            foreach (RoomData room in data.rooms)
            {
                roomConsumptionValues.Add(new BillingPeriodData(room));
            }
        }


        public CostUpdateData()
        {
            AUX_XML_Handler aUX_XML_ = new AUX_XML_Handler();
            ObservableCollection<object> data = aUX_XML_.load_data_from_xml();

            isAnnualBilling = false;
            isRentChange = false;

            based_on_AB = "";
            cause = "";

            flatData = (FlatData)data[0];
            flatCosts = (FlatCosts)data[1];

            period_start = new DateTime();
            period_end = new DateTime();
            cost_update_received = new DateTime();

            cold_rent = flatCosts.cold_rent;
            extra_costs_advance = flatCosts.extra_costs_advance;

            roomConsumptionValues = new ObservableCollection<BillingPeriodData>();

            foreach (RoomData room in flatData.rooms)
            {
                roomConsumptionValues.Add(new BillingPeriodData(room));
            }
        }


        public FlatCosts get_FlatCosts()
        {
            return flatCosts;
        }

        public FlatData get_FlatData()
        {
            return flatData;
        }
    }
}
