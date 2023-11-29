using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FlatShareCostCalculator
{
    /// <summary>
    /// Interaktionslogik für AnnualBillingUI.xaml
    /// </summary>
    public partial class AnnualBillingUI : UserControl
    {
        private CostUpdateData costUpdateData;

        public AnnualBillingUI()
        {
            costUpdateData = new CostUpdateData(AUX_Units.mainWindow().getFlatData(), AUX_Units.mainWindow().getFlatCosts());

            InitializeComponent();

            setup();
        }

        internal CostUpdateData extract_data()
        {

            DIE_reception_date.parse_value();
            costUpdateData.cost_update_received = (DateTime)DIE_reception_date.value;

            DIE_period_start.parse_value();
            costUpdateData.period_start = (DateTime)DIE_period_start.value;

            DIE_period_end.parse_value();
            costUpdateData.period_end = (DateTime)DIE_period_end.value;

            DIE_initial_cold_rent.parse_value();
            costUpdateData.initial_cold_rent = (double)DIE_initial_cold_rent.value;

            DIE_initial_extra_costs_advance.parse_value();
            costUpdateData.initial_extra_costs_advance = (double)DIE_initial_extra_costs_advance.value;

            DIE_annual_costs.parse_value();
            costUpdateData.annual_costs = (double)DIE_annual_costs.value;

            DIE_extra_costs_shared.parse_value();
            costUpdateData.extra_costs_shared = (double)DIE_extra_costs_shared.value;

            DIE_extra_costs_heating.parse_value();
            costUpdateData.extra_costs_heating = (double)DIE_extra_costs_heating.value;

            DIE_heating_units_usage.parse_value();
            costUpdateData.heating_units_usage = (double)DIE_heating_units_usage.value;

            DIE_heating_units_shared.parse_value();
            costUpdateData.heating_units_shared = (double)DIE_heating_units_shared.value;


            foreach (DataInputElement die in SP_room_consumption.Children)
            {
                foreach (BillingPeriodData room in costUpdateData.roomConsumptionValues)
                {
                    if (die.id == room.room.id)
                    {
                        die.parse_value();
                        room.heating_units_usage = (double)die.value;
                    }
                }
            }

            return costUpdateData;
        }

        private void setup_DIEs()
        {
            // date value DIEs
            DIE_reception_date.configurate("change date", true, $"{DateTime.Now:d}", "");
            DIE_period_start.configurate("period start", true, $"{DateTime.Now:d}", "");
            DIE_period_end.configurate("period end", true, $"{DateTime.Now:d}", "");

            // double value DIEs
            DIE_initial_cold_rent.configurate("cold rent", false, "0", AUX_Units.currency);
            DIE_initial_extra_costs_advance.configurate("extra costs advance", false, "0", AUX_Units.currency);
            DIE_annual_costs.configurate("annual costs", false, "0", AUX_Units.currency);
            DIE_extra_costs_shared.configurate("extra costs shared", false, "0", AUX_Units.currency);
            DIE_extra_costs_heating.configurate("extra costs heating", false, "0", AUX_Units.currency);

            DIE_heating_units_usage.configurate("heating units consumption", false, "0", AUX_Units.heating);
            DIE_heating_units_shared.configurate("shared consumption", false, "0", AUX_Units.heating);

            // date value DIEs
            DIE_reception_date.set_input_value_type(new DateTime());
            DIE_reception_date.insert_value(new DateTime());
            DIE_period_start.set_input_value_type(new DateTime());
            DIE_period_end.set_input_value_type(new DateTime());

            // double value DIEs
            DIE_initial_cold_rent.set_input_value_type(0.0);
            DIE_initial_extra_costs_advance.set_input_value_type(0.0);
            DIE_annual_costs.set_input_value_type(0.0);
            DIE_extra_costs_shared.set_input_value_type(0.0);
            DIE_extra_costs_heating.set_input_value_type(0.0);

            DIE_heating_units_usage.set_input_value_type(0.0);
            DIE_heating_units_shared.set_input_value_type(0.0);

            // date value DIEs
            DIE_reception_date.KeyDown += DIE_reception_date_KeyDown;
            DIE_period_start.KeyDown += DIE_period_start_KeyDown;
            DIE_period_end.KeyDown += DIE_period_end_KeyDown;

            // double value DIEs
            DIE_initial_cold_rent.KeyDown += DIE_initial_cold_rent_KeyDown;
            DIE_initial_extra_costs_advance.KeyDown += DIE_initial_extra_costs_advance_KeyDown;
            DIE_annual_costs.KeyDown += DIE_annual_costs_KeyDown;
            DIE_extra_costs_shared.KeyDown += DIE_extra_costs_shared_KeyDown;
            DIE_extra_costs_heating.KeyDown += DIE_extra_costs_heating_KeyDown;

            DIE_heating_units_usage.KeyDown += DIE_heating_units_usage_KeyDown;
            DIE_heating_units_shared.KeyDown += DIE_heating_units_shared_KeyDown;
        }


        private void setup_room_DIEs()
        {
            SP_room_consumption.Children.Clear();

            FlatData flatData = AUX_Units.mainWindow().getFlatData();

            foreach (RoomData room in flatData.rooms)
            {
                DataInputElement DIE_room = new DataInputElement();

                DIE_room.id = room.id;
                DIE_room.configurate($"{room.name} - {room.area}{AUX_Units.area}", false, AUX_Units.heating);
                DIE_room.set_input_value_type(0.0);

                DIE_room.KeyDown += DIE_room_KeyDown;

                SP_room_consumption.Children.Add(DIE_room);
            }
        }

        private void shared_heating_units_update()
        {
            double shared = costUpdateData.heating_units_usage;

            foreach (BillingPeriodData room in costUpdateData.roomConsumptionValues)
            {
                shared -= room.heating_units_usage;
            }

            costUpdateData.heating_units_shared = shared;

            DIE_heating_units_shared.insert_value(shared, 4);
        }



        internal void setup()
        {
            setup_DIEs();
            setup_room_DIEs();
        }

        internal void update(CostUpdateData _costUpdateData)
        {
            costUpdateData = _costUpdateData;
            
            // date value DIEs
            DIE_reception_date.insert_value(costUpdateData.cost_update_received, false, false);
            DIE_period_start.insert_value(costUpdateData.period_start, false, false);
            DIE_period_end.insert_value(costUpdateData.period_end, false, false);

            // double value DIEs
            DIE_initial_cold_rent.insert_value(costUpdateData.initial_cold_rent, true, false);
            DIE_initial_extra_costs_advance.insert_value(costUpdateData.initial_extra_costs_advance, true, false);
            DIE_annual_costs.insert_value(costUpdateData.annual_costs, true, false);
            DIE_extra_costs_shared.insert_value(costUpdateData.extra_costs_shared, true, false);
            DIE_extra_costs_heating.insert_value(costUpdateData.extra_costs_heating, true, false);

            DIE_heating_units_usage.insert_value(costUpdateData.heating_units_usage, 4);
            DIE_heating_units_shared.insert_value(costUpdateData.heating_units_shared, 4);

            foreach (DataInputElement die in SP_room_consumption.Children)
            {
                foreach (BillingPeriodData room in costUpdateData.roomConsumptionValues)
                {
                    if (die.id == room.room.id)
                    {
                        die.insert_value(room.heating_units_usage, 4);
                    }
                }
            }
        }

        private void DIE_heating_units_shared_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter || e.Key == Key.Return)
            {
                costUpdateData.heating_units_shared = (double)DIE_heating_units_shared.value;
            }
        }

        private void DIE_heating_units_usage_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter || e.Key == Key.Return)
            {
                costUpdateData.heating_units_usage = (double)DIE_heating_units_usage.value;
            }
        }

        private void DIE_extra_costs_heating_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter || e.Key == Key.Return)
            {
                costUpdateData.extra_costs_heating = (double)DIE_extra_costs_heating.value;
            }
        }

        private void DIE_extra_costs_shared_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter || e.Key == Key.Return)
            {
                costUpdateData.extra_costs_shared = (double)DIE_extra_costs_shared.value;
            }
        }

        private void DIE_annual_costs_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter || e.Key == Key.Return)
            {
                costUpdateData.annual_costs = (double)DIE_annual_costs.value;
            }
        }

        private void DIE_period_end_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter || e.Key == Key.Return)
            {
                costUpdateData.period_end = (DateTime)DIE_period_end.value;
            }
        }

        private void DIE_period_start_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter || e.Key == Key.Return)
            {
                costUpdateData.period_start = (DateTime)DIE_period_start.value;
            }
        }

        private void DIE_reception_date_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter || e.Key == Key.Return)
            {
                costUpdateData.cost_update_received = (DateTime)DIE_reception_date.value;
            }
        }

        private void DIE_initial_extra_costs_advance_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter || e.Key == Key.Return)
            {
                costUpdateData.initial_extra_costs_advance = (double)DIE_initial_extra_costs_advance.value;
            }
        }

        private void DIE_initial_cold_rent_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter || e.Key == Key.Return)
            {
                costUpdateData.initial_cold_rent = (double)DIE_initial_cold_rent.value;
            }
        }

        private void DIE_room_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter || e.Key == Key.Return)
            {
                DataInputElement room_die = (DataInputElement)sender;

                costUpdateData.roomConsumptionValues[room_die.id - 1].heating_units_usage = (double)room_die.value;

                shared_heating_units_update();
            }
        }
    }
}
