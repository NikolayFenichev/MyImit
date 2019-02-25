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
using Modbus.Device;
using System.IO.Ports;
using System.Net.Sockets;

/*Главное окно выбора UVS или MNA*/

namespace Imitator_v_0._1
{
    public partial class MainWindow : Window
    {
        public static UVSWindow uvsWindow;
        public static MNAWindow mnaWindow;
        static public ModbusMaster MbMaster { get; private set; }
        static public ModbusMaster MbTcpMaster { get; private set; }
        
        SerialPort sp = new SerialPort("COM2")
        {
            BaudRate = 9600,
            DataBits = 8,
            Parity = Parity.None,
            StopBits = StopBits.One,
            ReadTimeout = 500,
            WriteTimeout = 500

        };
        public static bool FlagSimulation { get; private set; } = false;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void VspomSistems_Click(object sender, RoutedEventArgs e)
        {
            uvsWindow = new UVSWindow()
            {
                Owner = this
            };
            if(Imitation.IsChecked == true) // работаем с имитационной моделью
            {
                FlagSimulation = true;
                try
                {
                    uvsWindow.Show();
                }
                catch
                {
                    new ErrorWindow().Show();
                    Imitation.IsChecked = false;
                }
            }
            else // работаем с железом
            {
                FlagSimulation = false;
                MbMaster = ModbusSerialMaster.CreateRtu(sp);
                try
                {
                    sp.Open();
                    uvsWindow.Show();
                }
                catch (System.IO.IOException)
                {
                    Console.WriteLine("Порт не существует");
                }
                catch (Exception)
                {

                }
            }           
        }

        private void PumpingUnit_Click(object sender, RoutedEventArgs e)
        {
            mnaWindow = new MNAWindow()
            {
                Owner = this
            };
            if (Imitation.IsChecked == true)
            {
                FlagSimulation = true;
                try
                {
                    MbMaster = ModbusIpMaster.CreateIp(new TcpClient("127.0.0.1", 502));
                    mnaWindow.Show();
                }
                catch
                {
                    new ErrorWindow().Show();
                    Imitation.IsChecked = false;
                }
            }
            else
            {
                FlagSimulation = false;
                MbMaster = ModbusSerialMaster.CreateRtu(sp);
                try
                {
                    sp.Open();
                    mnaWindow.Show();
                }
                catch (System.IO.IOException)
                {
                    Console.WriteLine("Порт не существует");
                }
            }
            
        }
    }
}
