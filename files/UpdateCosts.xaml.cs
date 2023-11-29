using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
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
    /// Interaktionslogik für UpdateCosts.xaml
    /// </summary>
    public partial class UpdateCosts : UserControl
    {
        internal CostUpdateData costUpdateData;

        public string cause { get; set; }
        public DateTime date { get; set; }

        public string header { get; set; }

        private System.Windows.Threading.DispatcherTimer _timer = new System.Windows.Threading.DispatcherTimer();


        public UpdateCosts(string _cause, DateTime _date)
        {

            InitializeComponent();

            MainWindow mainWindow = AUX_Units.mainWindow();

            costUpdateData = new CostUpdateData(mainWindow.getFlatData(), mainWindow.getFlatCosts());
            mainWindow.addCostUpdate(costUpdateData);

            setup_edit_tab();

            setup_period_tab();

            set_cause_and_date(_cause, _date);

            setup_new_rent_tab();
        }

        internal UpdateCosts(CostUpdateData _costUpdateData)
        {

            InitializeComponent();


            costUpdateData = _costUpdateData;

            setup_edit_tab();
            setup_payments_tab();
            setup_period_tab();
            setup_new_rent_tab();

            _timer.Tick += _timer_Tick; ;
            _timer.Interval = TimeSpan.FromSeconds(0.28);
            _timer.Start();

        }

        private void _timer_Tick(object? sender, EventArgs e)
        {
            load();

            _timer.Stop();
        }

        private void calculate_costs()
        {
            int counter = 0;
            double difference = 0.0;

            foreach (RoomData room in costUpdateData.get_FlatData().rooms)
            {
                CostUpdateOutputUI outputUI = new CostUpdateOutputUI(room.id, costUpdateData);

                if (counter % 2 == 0)
                {
                    outputUI.Background = new SolidColorBrush(Colors.AntiqueWhite);
                    outputUI.Foreground = new SolidColorBrush(Colors.SlateGray);
                }
                else
                {
                    outputUI.Background = new SolidColorBrush(Colors.SlateGray);
                    outputUI.Foreground = new SolidColorBrush(Colors.AntiqueWhite);
                }                

                if (costUpdateData.isAnnualBilling)
                {
                    SP_cost_split_result.Children.Add(outputUI);

                    double total_balance_value = ((CostsOutputUI)outputUI.MainBorder.Child).get_DIE_balance_total_value();

                    if (total_balance_value < 0)
                    {
                        difference += total_balance_value;
                    }                    
                }
                else
                {
                    SP_new_costs_result.Children.Add(outputUI);
                }                               



                counter++;
            }

            if (costUpdateData.isAnnualBilling)
            {
                DataInputElement die_combined_balance = new DataInputElement();
                die_combined_balance.configurate("balance total", true, AUX_Units.currency);
                die_combined_balance.insert_value(difference, true, false);

                die_combined_balance.HorizontalAlignment = HorizontalAlignment.Right;

                SP_cost_split_result.Children.Add(die_combined_balance);


            }

        }

        public string header_string()
        {
            return header = $"{cause}\n{date:d}";
        }

        public void load()
        {
            date = costUpdateData.cost_update_received;
            cause = costUpdateData.cause;

            DIE_cause.insert_value(costUpdateData.cause);
            DIE_date.insert_value(costUpdateData.cost_update_received, false, false);

            if (costUpdateData.isRentChange)
            {
                DIE_new_cold_rent.insert_value(costUpdateData.cold_rent, true, false);

                DIE_new_extra_costs_advance.insert_value(costUpdateData.extra_costs_advance, true, false);

                CB_raise.IsChecked = costUpdateData.isRentChange;

                if (costUpdateData.based_on_AB.Contains("-"))
                {
                    CB_SelectAB.IsChecked = true;

                    foreach (ComboBoxItem item in CoB_SelectAB.Items)
                    {
                        if (item.Content.ToString() == costUpdateData.based_on_AB)
                        {
                            CoB_SelectAB.SelectedItem = item;
                        }
                    }
                }

                show_new_rent_costs();
            }

            if (costUpdateData.isAnnualBilling)
            {
                DIE_period_start.insert_value(costUpdateData.period_start, false, false);
                DIE_period_end.insert_value(costUpdateData.period_end, false, false);

                AnnualBillingUI_object.update(costUpdateData);

                CB_annual_billing.IsChecked = costUpdateData.isAnnualBilling;


                foreach (PaymentsUI paymentsUI in SP_payments.Children)
                {
                    foreach (BillingPeriodData item in costUpdateData.roomConsumptionValues)
                    {
                        if (item.room.id == paymentsUI.billingPeriodData.room.id)
                        {
                            paymentsUI.billingPeriodData = item;

                            //Thread.Sleep(4);

                            paymentsUI.load_payments(item);
                        }
                    }
                }

                show_annual_billing_costs();
            }

            set_header();
        }


        private void reset_costUpdateData()
        {
            costUpdateData = new CostUpdateData(
                AUX_Units.mainWindow().getFlatData(),
                AUX_Units.mainWindow().getFlatCosts()
                );

            SP_new_costs_result.Children.Clear();

            DIE_new_cold_rent.insert_value(0, true, false);
            DIE_new_extra_costs_advance.insert_value(0, true, false);
        }

        public void save_costUpdate()
        {
            if (CB_annual_billing.IsChecked != null)
            {
                costUpdateData.isAnnualBilling = (bool)CB_annual_billing.IsChecked;
            }

            if (CB_raise.IsChecked != null)
            {
                costUpdateData.isRentChange = (bool)CB_raise.IsChecked;
            }

            if (costUpdateData.isAnnualBilling)
            {
                costUpdateData = AnnualBillingUI_object.extract_data();
            }

            DIE_cause.parse_value();
            costUpdateData.cause = (string)DIE_cause.value;
            DIE_date.parse_value();
            costUpdateData.cost_update_received = (DateTime)DIE_date.value;

            DIE_new_cold_rent.parse_value();
            costUpdateData.cold_rent = (double)DIE_new_cold_rent.value;
            DIE_new_extra_costs_advance.parse_value();
            costUpdateData.extra_costs_advance = (double)DIE_new_extra_costs_advance.value;

            (new AUX_XML_Handler()).save_cost_update_to_xml(costUpdateData);
        }

        public void set_cause_and_date(string _cause, DateTime _date)
        {
            cause = _cause;
            date = _date;

            set_header();
        }

        private void set_header()
        {
            //UpdateCostsUI costsUI = AUX_Units.mainWindow().UpdateCostsUI_object;

            //int index = costsUI.MainTabControl.SelectedIndex;

            //TabItem item = (TabItem)costsUI.MainTabControl.Items[index];

            if (this.Parent as TabItem != null)
            {
                TabItem item = this.Parent as TabItem;

                item.Header = header_string();

                item.ToolTip = $"period start {costUpdateData.period_start:d}\nperiod end {costUpdateData.period_end:d}";
            }
        }



        public void setup_edit_tab()
        {
            DIE_cause.configurate("change cause", false, "cost update", "");
            DIE_cause.set_input_value_type("");
            DIE_cause.focus_and_select();
            DIE_cause.KeyDown += DIE_cause_KeyDown;

            DIE_date.configurate("change date", false, $"{DateTime.Now:d}", "");
            DIE_date.set_input_value_type(new DateTime());
            DIE_date.KeyDown += DIE_date_KeyDown;
        }

        public void setup_new_rent_tab()
        {
            DIE_new_cold_rent.configurate("cold rent", false, AUX_Units.currency);
            DIE_new_cold_rent.set_input_value_type(0.0);
            DIE_new_cold_rent.focus_and_select();
            DIE_new_cold_rent.KeyDown += DIE_new_cold_rent_KeyDown;

            DIE_new_extra_costs_advance.configurate("costs advance", false, AUX_Units.currency);
            DIE_new_extra_costs_advance.set_input_value_type(0.0);
            DIE_new_extra_costs_advance.focus_and_select();
            DIE_new_extra_costs_advance.KeyDown += DIE_new_extra_costs_advance_KeyDown;
        }


        public void setup_payments_tab()
        {
            int counter = 0;

            SP_payments.Children.Clear();

            foreach (RoomData room in costUpdateData.get_FlatData().rooms)
            {
                BillingPeriodData billingPeriodData = new BillingPeriodData(room);
                PaymentsUI paymentsUI = new PaymentsUI(billingPeriodData);

                if (counter % 2 == 0)
                {
                    paymentsUI.Background = new SolidColorBrush(Colors.AntiqueWhite);
                    paymentsUI.Foreground = new SolidColorBrush(Colors.SlateGray);
                    paymentsUI.LL_room_name.Foreground = new SolidColorBrush(Colors.Black);
                }
                else
                {
                    paymentsUI.Background = new SolidColorBrush(Colors.SlateGray);
                    paymentsUI.Foreground = new SolidColorBrush(Colors.AntiqueWhite);
                    paymentsUI.LL_room_name.Foreground = new SolidColorBrush(Colors.AntiqueWhite);
                }

                SP_payments.Children.Add(paymentsUI);

                counter++;
            }
        }

        public void setup_period_tab()
        {
            DIE_period_start.configurate("begin", false, $"{DateTime.Now:d}", "");
            DIE_period_start.set_input_value_type(new DateTime());
            DIE_period_start.KeyDown += DIE_period_start_KeyDown; ;

            DIE_period_end.configurate("end", false, $"{DateTime.Now:d}", "");
            DIE_period_end.set_input_value_type(new DateTime());
            DIE_period_end.KeyDown += DIE_period_end_KeyDown; ;
        }


        private void show_annual_billing_costs()
        {
            ((TabItem)MainTabControl.Items[MainTabControl.Items.Count - 3]).IsSelected = true;

            SP_cost_split_result.Children.Clear();

            calculate_costs();
        }


        private void show_new_rent_costs()
        {
            ((TabItem)MainTabControl.Items[MainTabControl.Items.Count - 1]).IsSelected = true;
                        
            SP_new_costs_result.Children.Clear();

            calculate_costs();
        }

        private void save_new_rent_data()
        {
            if (CB_annual_billing != null)
            {
                if (CB_annual_billing.IsChecked != null)
                {
                    costUpdateData.isAnnualBilling = CB_annual_billing.IsChecked.Value;
                }
            }

            if (CB_raise != null)
            {
                if (CB_raise.IsChecked != null)
                {
                    costUpdateData.isRentChange = CB_raise.IsChecked.Value;
                }
            }

            costUpdateData.cause = (string)DIE_cause.value;
            costUpdateData.cost_update_received = (DateTime)DIE_date.value;

            costUpdateData.cold_rent = (double)DIE_new_cold_rent.value;
            costUpdateData.extra_costs_advance = (double)DIE_new_extra_costs_advance.value;
        }


        private void BTN_save_new_rent_data_Click(object sender, RoutedEventArgs e)
        {
            save_new_rent_data();
            show_new_rent_costs();
        }

        private void BTN_save_payments_Click(object sender, RoutedEventArgs e)
        {
            costUpdateData.roomConsumptionValues.Clear();

            foreach (PaymentsUI payments in SP_payments.Children)
            {
                payments.save_payments();
                costUpdateData.roomConsumptionValues.Add(payments.billingPeriodData);
            }
        }

        private void CB_payment_period_Checked(object sender, RoutedEventArgs e)
        {
            DIE_period_start.Visibility = Visibility.Visible;
            DIE_period_end.Visibility = Visibility.Visible;
        }

        private void CB_payment_period_Unchecked(object sender, RoutedEventArgs e)
        {
            DIE_period_start.Visibility = Visibility.Collapsed;
            DIE_period_end.Visibility = Visibility.Collapsed;
        }

        private void CB_annual_billing_Checked(object sender, RoutedEventArgs e)
        {
            CB_raise.IsEnabled = false;

            CB_payment_period.IsChecked = true;
            CB_payment_period.IsEnabled = false;

            cause = "annual billing";
            DIE_cause.insert_value(cause);

            costUpdateData.isAnnualBilling = true;
            costUpdateData.cause = cause;
            costUpdateData.cost_update_received = date;

            set_header();

            foreach (TabItem item in MainTabControl.Items)
            {
                if (item.Header.ToString() == "annual costs")
                {
                    item.Visibility = Visibility.Visible;
                }
                if (item.Header.ToString() == "payments")
                {
                    item.Visibility = Visibility.Visible;
                }

                if (item.Header.ToString() == "cost split")
                {
                    item.Visibility = Visibility.Visible;
                }
            }
        }

        private void CB_annual_billing_Unchecked(object sender, RoutedEventArgs e)
        {
            CB_raise.IsEnabled = true;

            CB_payment_period.IsChecked = false;
            CB_payment_period.IsEnabled = false;

            cause = "cost update";
            DIE_cause.insert_value(cause);

            reset_costUpdateData();

            costUpdateData.isAnnualBilling = false;
            costUpdateData.cause = cause;
            costUpdateData.cost_update_received = date;

            set_header();

            foreach (TabItem item in MainTabControl.Items)
            {
                if (item.Header.ToString() == "annual costs")
                {
                    item.Visibility = Visibility.Collapsed;
                }
                if (item.Header.ToString() == "payments")
                {
                    item.Visibility = Visibility.Collapsed;
                }

                if (item.Header.ToString() == "cost split")
                {
                    item.Visibility = Visibility.Collapsed;
                }
            }
        }

        private void CB_raise_Checked(object sender, RoutedEventArgs e)
        {
            CB_annual_billing.IsEnabled = false;

            cause = "rent change";
            DIE_cause.insert_value(cause);

            costUpdateData.isRentChange = true;
            costUpdateData.cause = cause;
            costUpdateData.cost_update_received = date;

            set_header();

            foreach (TabItem item in MainTabControl.Items)
            {
                if (item.Header.ToString() == "new rent")
                {
                    item.Visibility = Visibility.Visible;
                }

                if (item.Header.ToString() == "new costs")
                {
                    item.Visibility = Visibility.Visible;
                }
            }

            foreach (CostUpdateData item in AUX_Units.mainWindow().getCostUpdates())
            {
                ComboBoxItem comboBoxItem = new ComboBoxItem();

                if (item.isAnnualBilling)
                {
                    comboBoxItem.Content = $"{item.cause} - {item.cost_update_received:d}";
                    comboBoxItem.Name = $"cob_{CoB_SelectAB.Items.Count - 1}";

                    comboBoxItem.ToolTip = comboBoxItem.Name;

                    CoB_SelectAB.Items.Add(comboBoxItem);
                }
            }


            save_new_rent_data();

        }

        private void CB_raise_Unchecked(object sender, RoutedEventArgs e)
        {
            CB_annual_billing.IsEnabled = true;

            cause = "cost update";
            DIE_cause.insert_value(cause);

            reset_costUpdateData();

            costUpdateData.isRentChange = false;
            costUpdateData.cause = cause;
            costUpdateData.cost_update_received = date;

            set_header();

            foreach (TabItem item in MainTabControl.Items)
            {
                if (item.Header.ToString() == "new rent")
                {
                    item.Visibility = Visibility.Collapsed;
                }

                if (item.Header.ToString() == "new costs")
                {
                    item.Visibility = Visibility.Collapsed;

                }
            }

            CoB_SelectAB.Items.Clear();

            ComboBoxItem comboBoxItem = new ComboBoxItem()
            {
                Content = "select"
            };
            CoB_SelectAB.Items.Add(comboBoxItem);

            save_new_rent_data();
        }

        private void DIE_period_end_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter || e.Key == Key.Return)
            {
                costUpdateData.period_end = ((DateTime)DIE_period_end.value).Date;

                DIE_period_start.ToolTip = $"{costUpdateData.period_start} < - > {costUpdateData.period_end}";
                DIE_period_end.ToolTip = $"{costUpdateData.period_start} < - > {costUpdateData.period_end}";

                set_header();
            }
        }

        private void DIE_period_start_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter || e.Key == Key.Return)
            {
                costUpdateData.period_start = ((DateTime)DIE_period_start.value).Date;

                DIE_period_start.ToolTip = $"{costUpdateData.period_start:d} < - > {costUpdateData.period_end:d}";
                DIE_period_end.ToolTip = $"{costUpdateData.period_start:d} < - > {costUpdateData.period_end:d}";

                set_header();
            }
        }

        private void DIE_cause_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter || e.Key == Key.Return)
            {
                cause = (string)DIE_cause.value;
                costUpdateData.cause = cause;

                set_header();

                save_new_rent_data();
            }
        }

        private void DIE_date_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter || e.Key == Key.Return)
            {
                date = ((DateTime)DIE_date.value).Date;
                costUpdateData.cost_update_received = date;

                set_header();

                save_new_rent_data();
            }
        }
        private void DIE_new_cold_rent_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter || e.Key == Key.Return)
            {
                costUpdateData.cold_rent = (double)DIE_new_cold_rent.value;

                DIE_new_cold_rent.ToolTip = costUpdateData.cold_rent.ToString("C");
                DIE_new_extra_costs_advance.focus_and_select();

                save_new_rent_data();
            }

        }

        private void DIE_new_extra_costs_advance_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter || e.Key == Key.Return)
            {
                costUpdateData.extra_costs_advance = (double)DIE_new_extra_costs_advance.value;
                DIE_new_extra_costs_advance.ToolTip = costUpdateData.extra_costs_advance.ToString("C");
                BTN_save_new_rent_data.Focus();

                save_new_rent_data();
            }
        }


        private void TabItem_GotFocus(object sender, RoutedEventArgs e)
        {
            AnnualBillingUI_object.update(costUpdateData);
        }

        private void CoB_SelectAB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (costUpdateData != null && CB_SelectAB != null)
            {
                ComboBoxItem boxItem = (ComboBoxItem)CoB_SelectAB.SelectedItem;

                costUpdateData.based_on_AB = boxItem.Content.ToString();

                save_new_rent_data();
            }
        }
    }
}
