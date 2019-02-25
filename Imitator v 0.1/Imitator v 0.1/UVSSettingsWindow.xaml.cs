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
using System.Windows.Shapes;

/*Окно настройки UVS*/

namespace Imitator_v_0._1
{
    public partial class UVSSettingsWindow : Window
    {
        public UVSSettingsWindow()
        {
            InitializeComponent();
        }

        private void Cancel_Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void Apply_Button_Click(object sender, RoutedEventArgs e)// записываем таймера UVS через окно настроек
        {
            ushort u;

            UVSTimeProcess uVSTimeProcess = new UVSTimeProcess();

            if (ushort.TryParse(timeProcessMagneticStarter.Text, out u))
                uVSTimeProcess.TimeProcessMagneticStarter = u;
            else
                MessageBox.Show("Не верное значение");

            if (ushort.TryParse(timeProcessMagneticStarter.Text, out u))
                uVSTimeProcess.TimeProcessOffMagneticStarter = u;
            else
                MessageBox.Show("Не верное значение");

            if (ushort.TryParse(timeProcessOnPressure.Text, out u))
                uVSTimeProcess.TimeProcessOnPressure = u;
            else
                MessageBox.Show("Не верное значение");

            if (ushort.TryParse(timeProcessOffPressure.Text, out u))
                uVSTimeProcess.TimeProcessOffPressure = u;
            else
                MessageBox.Show("Не верное значение");

            if (ushort.TryParse(timeStopInPlace.Text, out u))
                uVSTimeProcess.TimeStopInPlace = u;
            else
                MessageBox.Show("Не верное значение");

            UVSWindow uvsWindow = this.Owner as UVSWindow;
            uvsWindow.SetTimeUVS(UVSWindow.PositionUvs, uVSTimeProcess);

            this.Close();
        }
    }
}
