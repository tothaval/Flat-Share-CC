using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FlatShareCostCalculator
{
    internal static class AUX_Units
    {
        public const string area = "m²";
        public const string currency = "€";
        public const string heating = "units";

        public static MainWindow mainWindow()
        {
            MainWindow window = (MainWindow)Application.Current.MainWindow;

            return window;
        }
    }
}
