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

/*Окно настройки адресов UVS*/

namespace Imitator_v_0._1
{
    public partial class UVSAdressSignal : Window
    {
        UVSAdress uvsAdress;

        public UVSAdressSignal()
        {
            InitializeComponent();
            uvsAdress = new UVSAdress();
        }

        private void Cancel_Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private string[] SetAdress(string adress) // разбиваем адрес на части 
        {
            string[] s;

            return s = adress.Split('.');
        }

        private void TextToAdress() // преобразование текста в адрес
        {
            uvsAdress.MagneticStarterAdress = magneticStarterAdress.Text;
            uvsAdress.PressureAdress = pressureAdress.Text;
            uvsAdress.VoltageAdress = voltageAdress.Text;
            uvsAdress.SHAdress = shAdress.Text;
        }

        private void AdressToText() // преобразование адреса в текст
        {
            magneticStarterAdress.Text = uvsAdress.MagneticStarterAdress;
            pressureAdress.Text = uvsAdress.PressureAdress;
            voltageAdress.Text = uvsAdress.VoltageAdress;
            shAdress.Text = uvsAdress.SHAdress;
        }

        private void Apply_Button_Click(object sender, RoutedEventArgs e) // записываем адреса
        {
            ushort u;

            try 
            {
                /*получаем адреса элементов из текстбоксов*/

                u = ushort.Parse(SetAdress(magneticStarterAdress.Text)[0]);
                if (u >= 0 && u <= 40000)
                    uvsAdress.MagneticStarterRegisterAdress = u;
                else
                    throw new Exception();

                u = ushort.Parse(SetAdress(pressureAdress.Text)[0]);
                if (u >= 0 && u <= 40000)
                    uvsAdress.PressureRegisterAdress = u;
                else
                    throw new Exception();

                u = ushort.Parse(SetAdress(voltageAdress.Text)[0]);
                if (u >= 0 && u <= 40000)
                    uvsAdress.VoltageRegisterAdress = u;
                else
                    throw new Exception();

                u = ushort.Parse(SetAdress(shAdress.Text)[0]);
                if (u >= 0 && u <= 40000)
                    uvsAdress.SHRegisterAdress = u;
                else
                    throw new Exception();

                u = ushort.Parse(SetAdress(magneticStarterAdress.Text)[1]);
                if(u >= 0 && u <= 15)
                    uvsAdress.MagneticStarterBitAdress = u;
                else
                    throw new Exception();

                u = ushort.Parse(SetAdress(pressureAdress.Text)[1]);
                if (u >= 0 && u <= 15)
                    uvsAdress.PressureBitAdress = u;
                else
                    throw new Exception();

                u = ushort.Parse(SetAdress(voltageAdress.Text)[1]);
                if (u >= 0 && u <= 15)
                    uvsAdress.VoltageBitAdress = u;
                else
                    throw new Exception();

                u = ushort.Parse(SetAdress(shAdress.Text)[1]);
                if (u >= 0 && u <= 15)
                    uvsAdress.SHBitAdress = u;
                else
                    throw new Exception();

                UVSWindow uvsWindow = this.Owner as UVSWindow;
                uvsWindow.SetAdressUVS(UVSWindow.PositionUvs, uvsAdress); // запись
                this.Close();
            }
            catch(ArgumentNullException)
            {
                MessageBox.Show("Введены не все значения");
                AdressToText();
            }
            catch(FormatException)
            {
                MessageBox.Show("Введено не корректное значение");
                AdressToText();
            }
            catch(Exception)
            {
                MessageBox.Show("Введено не корректное значение");
                AdressToText();
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            TextToAdress();
        }
    }
}
