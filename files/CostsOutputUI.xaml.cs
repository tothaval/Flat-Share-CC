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
    /// Interaktionslogik für CostsOutputUI.xaml
    /// </summary>
    public partial class CostsOutputUI : UserControl
    {
        private CostUpdateData cud;

        private int rnr;

        internal CostsOutputUI(int room_id, CostUpdateData data)
        {
            InitializeComponent();

            rnr= room_id;
            cud= data;

            setup_costs_output();
            calculate_cost_update();
        }

        private void _room_name()
        {
            if (cud != null)
            {
                LL_room_name.Content = $"{cud.roomConsumptionValues[rnr - 1].room.name} - {cud.roomConsumptionValues[rnr - 1].room.area}{AUX_Units.area}";
            }

        }

        private void calculate_cost_update()
        {
            // other die configuration required.
            AUX_Calculations calculate = new AUX_Calculations(cud);

            // rented area calculation
            double flat_area = cud.get_FlatData().flat_space;
            int room_count = cud.get_FlatData().rooms.Count;
            double room_area = cud.roomConsumptionValues[rnr-1].room.area;

            double shared_space_portion = calculate.shared_space() / room_count;
            double total_rented_area = room_area + shared_space_portion;

            DIE_room_area.insert_value(room_area, 2);
            DIE_shared_area.insert_value(shared_space_portion, 2);
            DIE_total_area.insert_value(total_rented_area, 2);

            double payments_total = 0;            

            foreach (double item in cud.roomConsumptionValues[rnr - 1].monthly_payments)
            {               
                payments_total += item;
            }


            DIE_payments.insert_value(payments_total, true, false);

            double payments_cr = total_rented_area / flat_area * cud.initial_cold_rent * 12;
            
            DIE_payments_ec.insert_value(payments_total - payments_cr, true, false) ;
            DIE_payments_cr.insert_value(payments_cr, true, false);            
            
            DIE_cold_rent_costs.insert_value(total_rented_area / flat_area * cud.initial_cold_rent * 12, true, false);

            DIE_cold_rent_diff.insert_value((double)DIE_cold_rent_costs.value - (double)DIE_payments_cr.value, true, false);

                        
            double ec_share = cud.extra_costs_shared * total_rented_area / flat_area;
            
            DIE_extra_costs_share.insert_value(ec_share, true, false);


            double heating_units_consumption = cud.roomConsumptionValues[rnr - 1].heating_units_usage + cud.heating_units_shared / room_count;
            double heating_share = heating_units_consumption / cud.heating_units_usage;

            double ec_heating = cud.extra_costs_heating * heating_share;

            DIE_extra_costs_heating.insert_value(ec_heating, true, false);


            DIE_costs_total.insert_value(
                (double)DIE_cold_rent_costs.value 
                + (double)DIE_extra_costs_share.value 
                + (double)DIE_extra_costs_heating.value
                ,true, false
                );


            double salvo = (double)DIE_payments_ec.value - ec_share;
            
            DIE_ecs_diff.insert_value(salvo, true, false);
            
            if (salvo > 0)
            {
                DIE_ecs_diff.insert_value(0.0, true, false);
                
            }
            
            salvo -= ec_heating;
            
            DIE_ech_diff.insert_value(salvo, true, false);


            DIE_balance_total.insert_value(
                (double)DIE_cold_rent_diff.value 
                + (double)DIE_ecs_diff.value 
                + (double)DIE_ech_diff.value, true, false);            
        }

        public double get_DIE_balance_total_value()
        {
            return (double)DIE_balance_total.value;
        }


        internal void setup_costs_output()
        {
            DIE_room_area.configurate("room", true, AUX_Units.area);
            DIE_shared_area.configurate("share", true, AUX_Units.area);
            DIE_total_area.configurate("total", true, AUX_Units.area);

            DIE_payments_cr.configurate("rent", true, AUX_Units.currency);
            DIE_payments_ec.configurate("costs", true, AUX_Units.currency);
            DIE_payments.configurate(">", true, AUX_Units.currency);

            DIE_cold_rent_costs.configurate("rent", true, AUX_Units.currency);
            DIE_extra_costs_share.configurate("house", true, AUX_Units.currency);
            DIE_extra_costs_heating.configurate("heating", true, AUX_Units.currency);

            DIE_balance_total.configurate("=", true, AUX_Units.currency);
            DIE_costs_total.configurate("-", true, AUX_Units.currency);

            DIE_cold_rent_diff.configurate("rent", true, AUX_Units.currency);
            DIE_ecs_diff.configurate("house", true, AUX_Units.currency);
            DIE_ech_diff.configurate("heating", true, AUX_Units.currency);

            DIE_room_area.set_tx_color(new SolidColorBrush(Colors.LightSlateGray));
            DIE_shared_area.set_tx_color(new SolidColorBrush(Colors.LightSlateGray));
            DIE_total_area.set_tx_color(new SolidColorBrush(Colors.LightSlateGray));

            DIE_payments_cr.set_tx_color(new SolidColorBrush(Colors.LightSlateGray));
            DIE_payments_ec.set_tx_color(new SolidColorBrush(Colors.LightSlateGray));

            DIE_payments.set_tx_color(new SolidColorBrush(Colors.LightSlateGray));
            DIE_balance_total.set_tx_color(new SolidColorBrush(Colors.LightSlateGray));
            DIE_costs_total.set_tx_color(new SolidColorBrush(Colors.LightSlateGray));

            DIE_payments.Background = new SolidColorBrush(Colors.MediumAquamarine);
            DIE_payments.Foreground = new SolidColorBrush(Colors.Black);
            DIE_payments.set_tx_color(new SolidColorBrush(Colors.Black));
            DIE_payments.set_tx_fontweight(FontWeights.Bold);

            DIE_balance_total.Background = new SolidColorBrush(Colors.MediumAquamarine);
            DIE_balance_total.Foreground = new SolidColorBrush(Colors.Black);
            DIE_balance_total.set_tx_color(new SolidColorBrush(Colors.Black));
            DIE_balance_total.set_tx_fontweight(FontWeights.Bold);

            DIE_costs_total.Background = new SolidColorBrush(Colors.MediumAquamarine);
            DIE_costs_total.Foreground = new SolidColorBrush(Colors.Black);
            DIE_costs_total.set_tx_color(new SolidColorBrush(Colors.Black));
            DIE_costs_total.set_tx_fontweight(FontWeights.Bold);

            DIE_cold_rent_costs.set_tx_color(new SolidColorBrush(Colors.LightSlateGray));
            DIE_extra_costs_share.set_tx_color(new SolidColorBrush(Colors.LightSlateGray));
            DIE_extra_costs_heating.set_tx_color(new SolidColorBrush(Colors.LightSlateGray));

            DIE_cold_rent_diff.set_tx_color(new SolidColorBrush(Colors.LightSlateGray));
            DIE_ecs_diff.set_tx_color(new SolidColorBrush(Colors.LightSlateGray));
            DIE_ech_diff.set_tx_color(new SolidColorBrush(Colors.LightSlateGray));

            _room_name();
        }
    }
}
