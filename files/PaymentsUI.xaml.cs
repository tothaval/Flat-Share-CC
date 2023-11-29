using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
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
    /// Interaktionslogik für PaymentsUI.xaml
    /// </summary>
    public partial class PaymentsUI : UserControl
    {
        public List<int> months { get; set; }
        public List<double> payment { get; set; }
        List<StackPanel> payment_lines { get; set; }

        private System.Windows.Threading.DispatcherTimer _timer = new System.Windows.Threading.DispatcherTimer();
        internal BillingPeriodData billingPeriodData { get; set; }

        internal PaymentsUI(BillingPeriodData periodData)
        {
            months = new List<int>();
            payment = new List<double>();

            payment_lines = new List<StackPanel>();

            billingPeriodData = periodData;

            InitializeComponent();

            MainStackPanel.Children.Add(add_payment_line());

            LL_room_name.Content = $"{billingPeriodData.room.name} - {billingPeriodData.room.area}{AUX_Units.area}";

            //if (loading)
            //{
            //    _timer.Tick += _timer_Tick;
            //    _timer.Interval = TimeSpan.FromSeconds(0.28);
            //    _timer.Start();
            //}
        }


        // generate and configure UserControlBars

        private void _timer_Tick(object sender, EventArgs e)
        {
            MainStackPanel.Children.Remove(MainStackPanel.Children[1]);

            //load_payments();

            _timer.Stop();
        }

        private StackPanel add_payment_line()
        {
            StackPanel stackPanel = new StackPanel() { Orientation = Orientation.Horizontal };

            DataInputElement DIE_months = new DataInputElement();
            DataInputElement DIE_payment = new DataInputElement();

            DIE_months.configurate("months", false, "12", "x");
            DIE_payment.configurate("payment", false, "0", AUX_Units.currency);

            int x = 0;
            DIE_months.set_input_value_type(x);
            DIE_payment.set_input_value_type(0.0);
                        
            DIE_months.KeyUp += DIE_months_KeyUp;
            DIE_payment.KeyUp += DIE_payment_KeyUp;

            stackPanel.Children.Add(DIE_months);
            stackPanel.Children.Add(DIE_payment);

            payment_lines.Add(stackPanel);

            return stackPanel;
        }

        internal void load_payments(BillingPeriodData data)
        {
            MainStackPanel.Children.Clear();
            
            List<double> o = new List<double>();
            List<int> u = new List<int>();

            int c = 0;

            foreach (double payment in data.monthly_payments)
            {

                if (!o.Contains(payment))
                {
                    c = 0;

                    o.Add(payment);
                    u.Add(c);
                }

                if (o.Contains(payment))
                {
                    u[o.IndexOf(payment)] += 1;
                }
                c++;
            }

            foreach (double d in o)
            {
                StackPanel stackPanel = new StackPanel() { Orientation = Orientation.Horizontal };

                DataInputElement DIE_months = new DataInputElement();
                DataInputElement DIE_payment = new DataInputElement();

                DIE_months.configurate("months", false, "12", "x");
                DIE_payment.configurate("payment", false, "0", AUX_Units.currency);

                int x = 0;
                DIE_months.set_input_value_type(x);
                DIE_payment.set_input_value_type(0.0);

                DIE_months.KeyUp += DIE_months_KeyUp;
                DIE_payment.KeyUp += DIE_payment_KeyUp;

                DIE_months.insert_value(u[o.IndexOf(d)]);

                DIE_payment.insert_value(d, true, false);

                stackPanel.Children.Add(DIE_months);
                stackPanel.Children.Add(DIE_payment);

                payment_lines.Add(stackPanel);
                MainStackPanel.Children.Add(stackPanel);
            } 
        }

        public void change_room_name(string room_name)
        {
            LL_room_name.Content = room_name;
        }

        public void save_payments()
        {            
            billingPeriodData.monthly_payments.Clear();
            
            foreach (StackPanel payment_line in payment_lines)
            {
                DataInputElement months = (DataInputElement)payment_line.Children[0];
                DataInputElement payment = (DataInputElement)payment_line.Children[1];
            
                for (int i = 0; i < (int)months.value; i++)
                {
                    billingPeriodData.monthly_payments.Add((double)payment.value);
                }
            }
        }

        private void DIE_payment_KeyUp(object sender, KeyEventArgs e)
        {
            DataInputElement payment_die = (DataInputElement)sender;

            if (e.Key == Key.Enter || e.Key == Key.Return)
            {
                payment.Add((double)payment_die.value);
            }
        }

        private void DIE_months_KeyUp(object sender, KeyEventArgs e)
        {
            DataInputElement months_die = (DataInputElement)sender;

            if (e.Key == Key.Enter || e.Key == Key.Return)
            {
                int check = 0;

                months.Add((int)months_die.value);

                foreach (int value in months)
                {
                    check += value;
                }


                if (check < 12)
                {
                    int diff = 12 - check;

                    MainStackPanel.Children.Add(add_payment_line());

                    ((DataInputElement)payment_lines[payment_lines.Count - 1].Children[0]).insert_value(diff);

                    months_die.TX.IsEnabled = false;

                }
                else if (check > 12)
                {
                    MessageBox.Show($"{check}\nmax value = 12");

                    months.Remove((int)months_die.value);
                }
                else
                {
                    months_die.TX.IsEnabled = false;
                }


                StackPanel parent = months_die.Parent as StackPanel;

                ((DataInputElement)parent.Children[1]).TX.Focus();

                e.Handled = true;
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {

        }

    }
}
