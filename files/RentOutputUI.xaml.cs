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
    /// Interaktionslogik für RentOutputUI.xaml
    /// </summary>
    public partial class RentOutputUI : UserControl
    {
        private FlatData fd;
        private FlatCosts fc;

        private CostUpdateData cud;

        private CostUpdateData based_on;

        private int rnr;

        internal RentOutputUI(int room_id, FlatData flatData, FlatCosts flatCosts)
        {
            InitializeComponent();

            fd = flatData;
            fc = flatCosts;

            rnr = room_id;

            setup_rent_output();

            calculate_initial_rent();
        }


        internal RentOutputUI(int room_id, CostUpdateData costUpdateData)
        {
            InitializeComponent();

            cud = costUpdateData;

            rnr = room_id;

            setup_rent_output();

            get_annual_billing();

            calculate_rent();

            if (cud.isRentChange)
            {
                if (cud.based_on_AB.Contains("-"))
                {
                    calculate_new_rent();
                }
            }
        }

        private void _room_name()
        {
            if (fd != null)
            {
                LL_room_name.Content = $"{fd.rooms[rnr-1].name} - {fd.rooms[rnr-1].area}{AUX_Units.area}";
            }

            if (cud != null)
            {
                LL_room_name.Content = $"{cud.roomConsumptionValues[rnr - 1].room.name} - {cud.roomConsumptionValues[rnr - 1].room.area}{AUX_Units.area}";
            }
            
        }


        private void calculate_initial_rent()
        {
            AUX_Calculations calculate = new AUX_Calculations(fd, fc);

            double flat_area = fd.flat_space;
            double room_area = fd.rooms[rnr - 1].area;
            double shared_space_portion = calculate.shared_space() / fd.rooms.Count;
            double total_rented_area = room_area + shared_space_portion;

            double cold_rent = total_rented_area / flat_area * fc.cold_rent;
            double extra_cost_advance = total_rented_area / flat_area * fc.extra_costs_advance;


            DIE_room_area.insert_value(room_area, 2);
            DIE_shared_area.insert_value(shared_space_portion, 2);
            DIE_total_area.insert_value(total_rented_area, 2);


            DIE_cold_rent_room.insert_value(room_area / flat_area * fc.cold_rent, true, false);
            DIE_cold_rent_share.insert_value(shared_space_portion / flat_area * fc.cold_rent, true, false);
            DIE_cold_rent.insert_value(cold_rent, true, false);


            DIE_extra_costs_share.Visibility = Visibility.Hidden;
            DIE_extra_costs_heating.Visibility = Visibility.Hidden;
            DIE_extra_costs_advance.insert_value(extra_cost_advance, true, false);


            DIE_monthly_costs.insert_value(cold_rent + extra_cost_advance, true, false);
        }

        private void calculate_new_rent()
        {
            RoomData room = cud.get_FlatData().rooms[rnr - 1];
            BillingPeriodData billingPeriod = new BillingPeriodData(room);

            AUX_Calculations calculate = new AUX_Calculations(cud);

            double flat_area = cud.get_FlatData().flat_space;
            int room_count = cud.get_FlatData().rooms.Count;

            double room_area = cud.get_FlatData().rooms[rnr - 1].area;
            double shared_space_portion = calculate.shared_space() / room_count;
            double total_rented_area = room_area + shared_space_portion;

            foreach (BillingPeriodData data in based_on.roomConsumptionValues)
            {
                if (data.room.id == rnr && data != null)
                {
                    billingPeriod = data;
                }
            }

            // partial cold rent
            double shared_cold_rent = shared_space_portion / flat_area * cud.cold_rent;
            double room_cold_rent = room_area / flat_area * cud.cold_rent;
            double cold_rent = shared_cold_rent + room_cold_rent;

            // shared costs share calculation
            double shared_cost_share_total = based_on.extra_costs_shared / based_on.annual_costs * cud.extra_costs_advance;

            // heating costs share calculation
            double heating_cost_share_total = based_on.extra_costs_heating / based_on.annual_costs * cud.extra_costs_advance;

            // extra cost calculation
            double extra_cost_share = shared_cost_share_total * total_rented_area/ flat_area;

            double heating_units_consumption = billingPeriod.heating_units_usage + based_on.heating_units_shared / based_on.get_FlatData().rooms.Count;

            double extra_cost_heating_share = heating_units_consumption / based_on.heating_units_usage * heating_cost_share_total;

            // extra cost calculation
            double extra_costs = extra_cost_share + extra_cost_heating_share;

            // total rent share calculation
            double total_rent = cold_rent + extra_costs;

            DIE_room_area.insert_value(room_area, 2);
            DIE_shared_area.insert_value(shared_space_portion, 2);
            DIE_total_area.insert_value(total_rented_area, 2);

            DIE_cold_rent_room.insert_value(room_area / flat_area * cud.cold_rent, true, false);
            DIE_cold_rent_share.insert_value(shared_space_portion / flat_area * cud.cold_rent, true, false);
            DIE_cold_rent.insert_value(cold_rent, true, false);

            DIE_extra_costs_share.Visibility = Visibility.Visible;
            DIE_extra_costs_heating.Visibility = Visibility.Visible;

            DIE_extra_costs_share.insert_value(extra_cost_share, true, false);
            DIE_extra_costs_heating.insert_value(extra_cost_heating_share, true, false);
            DIE_extra_costs_advance.insert_value(extra_costs, true, false);

            DIE_monthly_costs.insert_value(total_rent, true, false);
        }

        private void calculate_rent()
        {
            AUX_Calculations calculate = new AUX_Calculations(cud);

            double flat_area = cud.get_FlatData().flat_space;
            double room_area = cud.get_FlatData().rooms[rnr - 1].area;
            double shared_space_portion = calculate.shared_space() / cud.get_FlatData().rooms.Count;
            double total_rented_area = room_area + shared_space_portion;

            double cold_rent = total_rented_area / flat_area * cud.cold_rent;
            double extra_cost_advance = total_rented_area / flat_area * cud.extra_costs_advance;


            DIE_room_area.insert_value(room_area, 2);
            DIE_shared_area.insert_value(shared_space_portion, 2);
            DIE_total_area.insert_value(total_rented_area, 2);


            DIE_cold_rent_room.insert_value(room_area / flat_area * cud.cold_rent, true, false);
            DIE_cold_rent_share.insert_value(shared_space_portion / flat_area * cud.cold_rent, true, false);
            DIE_cold_rent.insert_value(cold_rent, true, false);


            DIE_extra_costs_share.Visibility = Visibility.Hidden;
            DIE_extra_costs_heating.Visibility = Visibility.Hidden;
            DIE_extra_costs_advance.insert_value(extra_cost_advance, true, false);


            DIE_monthly_costs.insert_value(cold_rent + extra_cost_advance, true, false);
        }


        private void get_annual_billing()
        {
            if (cud.isRentChange)
            {
                if (cud.based_on_AB.Length > 1)
                {
                    MainWindow mainWindow = AUX_Units.mainWindow();

                    foreach (CostUpdateData cud_item in mainWindow.getCostUpdates())
                    {
                        string header = $"{cud_item.cause} - {cud_item.cost_update_received:d}";

                        //MessageBox.Show(header + "\n" + cud.based_on_AB);

                        if (cud.based_on_AB == header)
                        {
                            based_on = cud_item;
                        }

                    }
                }

            }
        }

        internal void setup_rent_output()
        {
            DIE_room_area.configurate("room", true, AUX_Units.area);
            DIE_shared_area.configurate("share", true, AUX_Units.area);
            DIE_total_area.configurate("total", true, AUX_Units.area);

            DIE_room_area.set_tx_color(new SolidColorBrush(Colors.LightSlateGray));
            DIE_shared_area.set_tx_color(new SolidColorBrush(Colors.LightSlateGray));
            DIE_total_area.set_tx_color(new SolidColorBrush(Colors.LightSlateGray));

            DIE_cold_rent_room.configurate("room", true, AUX_Units.currency);
            DIE_cold_rent_share.configurate("share", true, AUX_Units.currency);
            DIE_cold_rent.configurate(">", true, AUX_Units.currency);

            DIE_extra_costs_share.configurate("house", true, AUX_Units.currency);
            DIE_extra_costs_heating.configurate("heating", true, AUX_Units.currency);
            DIE_extra_costs_advance.configurate("+", true, AUX_Units.currency);

            DIE_monthly_costs.configurate("=", true, AUX_Units.currency);
            DIE_monthly_costs.Background = new SolidColorBrush(Colors.MediumAquamarine);
            DIE_monthly_costs.Foreground = new SolidColorBrush(Colors.Black);
            DIE_monthly_costs.set_tx_color(new SolidColorBrush(Colors.Black));
            DIE_monthly_costs.set_tx_fontweight(FontWeights.Bold);

            DIE_cold_rent.set_tx_color(new SolidColorBrush(Colors.Black));
            DIE_cold_rent.Background = new SolidColorBrush(Colors.MediumAquamarine);
            DIE_cold_rent.Foreground = new SolidColorBrush(Colors.Black);
            DIE_cold_rent.set_tx_fontweight(FontWeights.Medium);

            DIE_cold_rent_room.set_tx_color(new SolidColorBrush(Colors.DarkGray));
            DIE_cold_rent_room.set_tx_fontweight(FontWeights.Light);

            DIE_cold_rent_share.set_tx_color(new SolidColorBrush(Colors.DarkGray));
            DIE_cold_rent_share.set_tx_fontweight(FontWeights.Light);

            DIE_extra_costs_share.set_tx_color(new SolidColorBrush(Colors.LightSlateGray));
            DIE_extra_costs_share.set_tx_fontweight(FontWeights.Light);

            DIE_extra_costs_heating.set_tx_color(new SolidColorBrush(Colors.LightSlateGray));
            DIE_extra_costs_heating.set_tx_fontweight(FontWeights.Light);

            DIE_extra_costs_advance.set_tx_color(new SolidColorBrush(Colors.Black));
            DIE_extra_costs_advance.Background = new SolidColorBrush(Colors.MediumAquamarine);
            DIE_extra_costs_advance.Foreground = new SolidColorBrush(Colors.Black);
            DIE_extra_costs_advance.set_tx_fontweight(FontWeights.Medium);

            _room_name();
        }
    }
}
