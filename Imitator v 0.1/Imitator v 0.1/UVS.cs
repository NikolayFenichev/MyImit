using System;
using Modbus.Device;
using System.Linq;
using System.Collections;
using System.Windows.Shapes;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Input;

/*Изображение и управления UVS*/

namespace Imitator_v_0._1
{
    public delegate void MessageHeandler(string s, SolidColorBrush color);
    public delegate void UVSSetHeandler();

    public class UVS : BaseObjectVU, IComparable<UVS>
    {
        bool flagLockMagneticStarter;
        bool flagLockPressure;
        bool flagLockVoltage;
        bool flagLockSH;

        bool prevVoltage;
        bool prevMagneticStarter;
        bool prevPressure;
        bool prevSH;

        public bool Voltage { get; set; }
        public bool MagneticStarter { get; set; }
        public bool Pressure { get; set; }
        public bool SH { get; set; }
        public bool ProcessOn { get; set; }
        public bool ProcessOff { get; set; }
        public UVSTimeProcess UVSTimeProcess { get; set; }
        public UVSAdress UVSAdress { get; set; }
        public ModelUVS ModelUVS { get; }
  
        Image padlockPressure;
        Image padlockVoltage;
        Image padlockMagneticStarter;
        Image padlockSH;
        
        MenuItem[] menuItems = new MenuItem[2];
        ContextMenu contextMenu = new ContextMenu();

        public UVS(ModbusMaster mbMaster, byte slaveAdress, string name)
        {
            UVSTimeProcess = new UVSTimeProcess();
            this.mbMaster = mbMaster;
            SlaveAdress = slaveAdress;
            Name = name;

            if (MainWindow.FlagSimulation)
                ModelUVS = new ModelUVS();

            PositionObject = new Position();

            UVSAdress = new UVSAdress();
        }

        public override void RegisterUVSMessageHeandler(MessageHeandler del)
        {
            MessageHeandler += del;
        }
        public override void UnregisterUVSMessageHeandler(MessageHeandler del)
        {
            MessageHeandler -= del;
        }

        public override UIElement Create() // создание UVS
        {
            gridObject = new Grid()
            {
                Height = 173,
                Width = 190,
                ShowGridLines = true,
                Background = Brushes.White,
                Margin = new Thickness(0, 0, 0, 0),
            };

            Ellipse pumpUI = new Ellipse()
            {
                Name = "pumpUI",
                Height = 100,
                Width = 100,
                Fill = Brushes.Yellow,
                Stroke = Brushes.Black,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Margin = new Thickness(43, 8, 0, 0),
            };
            Ellipse magneticStarterUI = new Ellipse()
            {
                Name = "magneticStarterUI",
                Height = 30,
                Width = 30,
                Fill = Brushes.Yellow,
                Stroke = Brushes.Black,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Margin = new Thickness(142, 78, 0, 0)
            };
            Ellipse pressureUI = new Ellipse()
            {
                Name = "pressureUI",
                Height = 30,
                Width = 30,
                Fill = Brushes.Yellow,
                Stroke = Brushes.Black,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Margin = new Thickness(143, 3, 0, 0),
            };
            Ellipse voltageUI = new Ellipse()
            {
                Name = "voltageUI",
                Height = 30,
                Width = 30,
                Fill = Brushes.Red,
                Stroke = Brushes.Black,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Margin = new Thickness(12, 78, 0, 0)
            };
            Ellipse shUI = new Ellipse()
            {
                Name = "shUI",
                Height = 30,
                Width = 30,
                Fill = Brushes.Red,
                Stroke = Brushes.Black,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Margin = new Thickness(79, 113, 0, 0)
            };

            Label labelMagneticStarter = new Label()
            {
                Content = "МП",
                HorizontalAlignment = HorizontalAlignment.Left,
                Margin = new Thickness(139, 79, 0, 0),
                VerticalAlignment = VerticalAlignment.Top,
                FontFamily = new FontFamily("Verdana"),
                FontSize = 14,
                FontWeight = FontWeights.Bold,
                Height = 33,
                Width = 44
            };
            Label labelPressure = new Label()
            {
                Content = "P",
                HorizontalAlignment = HorizontalAlignment.Left,
                Margin = new Thickness(147, 3, 0, 0),
                VerticalAlignment = VerticalAlignment.Top,
                FontFamily = new FontFamily("Verdana"),
                FontSize = 16,
                FontWeight = FontWeights.Bold,
                Height = 33,
                Width = 25
            };
            Label labelVoltage = new Label()
            {
                Content = "U",
                HorizontalAlignment = HorizontalAlignment.Left,
                Margin = new Thickness(15, 78, 0, 0),
                VerticalAlignment = VerticalAlignment.Top,
                FontFamily = new FontFamily("Verdana"),
                FontSize = 16,
                FontWeight = FontWeights.Bold,
                Height = 33,
                Width = 25
            };
            Label labelSH = new Label()
            {
                Content = "СШ",
                HorizontalAlignment = HorizontalAlignment.Left,
                Margin = new Thickness(75, 114, 0, 0),
                VerticalAlignment = VerticalAlignment.Top,
                FontFamily = new FontFamily("Verdana"),
                FontSize = 14,
                FontWeight = FontWeights.Bold,
                Height = 29,
                Width = 41
            };
            labelName = new Label()
            {
                Name = "name",
                Content = "",
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, -60, 0, 0),
                VerticalAlignment = VerticalAlignment.Center,
                FontFamily = new FontFamily("Verdana"),
                FontSize = 14,
                FontWeight = FontWeights.Bold,
                Height = 29,
                Width = 100
            };

            padlockPressure = new Image()
            {
                Name = "padlockPressure",
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Margin = new Thickness(171, 24, 0, 0),
                Height = 16,
                Width = 16,
                Source = BitmapOpen
            };
            padlockVoltage = new Image()
            {
                Name = "padlockVoltage",
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Margin = new Thickness(41, 100, 0, 0),
                Height = 16,
                Width = 16,
                Source = BitmapOpen
            };
            padlockMagneticStarter = new Image()
            {
                Name = "padlockMagneticStarter",
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Margin = new Thickness(171, 100, 0, 0),
                Height = 16,
                Width = 16,
                Source = BitmapOpen
            };
            padlockSH = new Image()
            {
                Name = "padlockSH",
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Height = 16,
                Width = 16,
                Margin = new Thickness(105, 137, 0, 0),
                Source = BitmapOpen

            };

            Button buttonTurnOn = new Button()
            {
                Content = "Пуск",
                HorizontalAlignment = HorizontalAlignment.Left,
                Margin = new Thickness(15, 117, 0, 0),
                VerticalAlignment = VerticalAlignment.Top,
                Width = 55,
                Height = 22,
            };
            Button buttonTurnOff = new Button()
            {
                Content = "Стоп",
                HorizontalAlignment = HorizontalAlignment.Left,
                Margin = new Thickness(121, 117, 0, 0),
                VerticalAlignment = VerticalAlignment.Top,
                Width = 55,
                Height = 22,
            };

            buttonTurnOn.Click += ButtonTurnOn_Clik;
            buttonTurnOff.Click += ButtonTurnOff_Clik;

            magneticStarterUI.MouseDown += MagneticStarterUI_MouseDown;
            pressureUI.MouseDown += PressureUI_MouseDown;
            voltageUI.MouseDown += VoltageUI_MouseDown;
            shUI.MouseDown += ShUI_MouseDown;
            labelMagneticStarter.MouseDown += LabelMagneticStarter_MouseDown;
            labelPressure.MouseDown += LabelPressure_MouseDown;
            labelVoltage.MouseDown += LabelVoltage_MouseDown;
            labelSH.MouseDown += LabelSH_MouseDown;

            padlockMagneticStarter.MouseDown += PadlockMagneticStarter_MouseDown;
            padlockPressure.MouseDown += PadlockPressure_MouseDown;
            padlockVoltage.MouseDown += PadlockVoltage_MouseDown;
            padlockSH.MouseDown += PadlockSH_MouseDown;

            gridObject.ContextMenu = contextMenu;
            MenuItem menuItem1Settings = new MenuItem()
            {
                Name = "settings",
                Header = "Настройки",
            };
            menuItem1Settings.Click += Settings_Click;
            contextMenu.Items.Add(menuItem1Settings);
            MenuItem menuItemDelete = new MenuItem()
            {
                Name = "delete",
                Header = "Удалить",
            };
            menuItemDelete.Click += Delete_Click;
            contextMenu.Items.Add(menuItemDelete);
            MenuItem menuItemRename = new MenuItem()
            {
                Name = "rename",
                Header = "Переименовать",
            };
            menuItemRename.Click += Rename_Click;
            contextMenu.Items.Add(menuItemRename);
            MenuItem menuItemAdress = new MenuItem()
            {
                Name = "adress",
                Header = "Адреса",
            };
            menuItemAdress.Click += Adress_Click;
            contextMenu.Items.Add(menuItemAdress);

            gridObject.MouseUp += GridUvs_MouseUp;

            gridObject.Children.Add(pumpUI);
            gridObject.Children.Add(voltageUI);
            gridObject.Children.Add(magneticStarterUI);
            gridObject.Children.Add(pressureUI);
            gridObject.Children.Add(shUI);
            gridObject.Children.Add(labelVoltage);
            gridObject.Children.Add(labelMagneticStarter);
            gridObject.Children.Add(labelPressure);
            gridObject.Children.Add(labelSH);
            gridObject.Children.Add(labelName);
            gridObject.Children.Add(buttonTurnOn);
            gridObject.Children.Add(buttonTurnOff);
            gridObject.Children.Add(padlockMagneticStarter);
            gridObject.Children.Add(padlockPressure);
            gridObject.Children.Add(padlockVoltage);
            gridObject.Children.Add(padlockSH);

            elements.Add(pumpUI);
            elements.Add(voltageUI);
            elements.Add(magneticStarterUI);
            elements.Add(pressureUI);
            elements.Add(shUI);

            if (gridObject.Children[gridObject.Children.IndexOf(labelName)] is Label) // записываем имя
                ((Label)(gridObject.Children[gridObject.Children.IndexOf(labelName)])).Content = Name;

            MessageHeandler($"{DateTime.Now} :Создан объект \"Вспомсистема - {Name}\"", Brushes.Black);

            return gridObject;
        }       
        public override void TurnOn() // включение
        {
            Cmd += 256;

            if (!MainWindow.FlagSimulation)
                mbMaster.WriteSingleRegister(SlaveAdress, 1, Cmd);
            else
                ModelUVS.Cmd = Cmd;
            Cmd = 0;
            MessageHeandler($"{DateTime.Now} :\"Вспомсистема - {Name}\" Команда пуск по месту", Brushes.Black);
        }
        public override void TurnOff() // отключение
        {
            Cmd += 512;
            //bool[] mass = new bool[] { false, false, false, false, false, false, false, false, false, false, false, false, false, false, true, false };
            if (!MainWindow.FlagSimulation)
                mbMaster.WriteSingleRegister(SlaveAdress, 1, Cmd);
            else
                ModelUVS.Cmd = Cmd;
            Cmd = 0;
            MessageHeandler($"{DateTime.Now} :\"Вспомсистема - {Name}\" Команда стоп по месту", Brushes.Black);
        }
        public override void Survey() // опрос
        {
            BitArray bA = ModBusSurveyMass();

            Voltage = bA.Get(0);
            MagneticStarter = bA.Get(1);
            Pressure = bA.Get(2);
            SH = bA.Get(3);
            ProcessOn = bA.Get(4);
            ProcessOff = bA.Get(5);

            if (prevVoltage == false && Voltage == true)
            {
                MessageHeandler($"{DateTime.Now} :\"Вспомсистема - {Name}\" Напряжение подано", Brushes.Green);
            }
            else if (prevVoltage == true && Voltage == false)
            {
                MessageHeandler($"{DateTime.Now} :\"Вспомсистема - {Name}\" Напряжение снято", Brushes.Red);
            }

            if (prevMagneticStarter == false && MagneticStarter == true)
            {
                MessageHeandler($"{DateTime.Now} :\"Вспомсистема - {Name}\" МП включен", Brushes.Green);
            }
            else if (prevMagneticStarter == true && MagneticStarter == false)
            {
                MessageHeandler($"{DateTime.Now} :\"Вспомсистема - {Name}\" МП отключен", Brushes.Red);
            }

            if (prevPressure == false && Pressure == true)
            {
                MessageHeandler($"{DateTime.Now} :\"Вспомсистема - {Name}\" Давление подано", Brushes.Green);
            }
            else if (prevPressure == true && Pressure == false)
            {
                MessageHeandler($"{DateTime.Now} :\"Вспомсистема - {Name}\" Давление снято", Brushes.Red);
            }

            if (prevSH == false && SH == true)
            {
                MessageHeandler($"{DateTime.Now} :\"Вспомсистема - {Name}\" Напряжение на СШ подано", Brushes.Green);
            }
            else if (prevSH == true && SH == false)
            {
                MessageHeandler($"{DateTime.Now} :\"Вспомсистема - {Name}\" Напряжение на СШ снято", Brushes.Red);
            }

            prevVoltage = Voltage;
            prevMagneticStarter = MagneticStarter;
            prevPressure = Pressure;
            prevSH = SH;
        }
        public override BitArray ModBusSurveyMass() // разбиваем state на биты
        {
            if (!MainWindow.FlagSimulation)
                State = mbMaster.ReadHoldingRegisters(SlaveAdress, 0, 1)[0];
            else
                State = ModelUVS.State;

            bStateMass[0] = (byte)State;

            return new BitArray(bStateMass);
        }

        private bool[] TenToTwin(int current) // из десятичной в двоичную
        {
            bool[] currentTwin = new bool[16];
            int remainder;

            for (int i = 0; i < 16; i++)
            {
                remainder = current % 2;
                current /= 2;
                if (remainder > 0)
                    currentTwin[i] = true;
                else
                    currentTwin[i] = false;
            }

            return currentTwin;
        }

        public void SetTimeProcessMagneticStarter()
        {
            if (!MainWindow.FlagSimulation)
                mbMaster.WriteSingleRegister(SlaveAdress, 2, UVSTimeProcess.TimeProcessMagneticStarter);
            else
            {
                ModelUVS.TimeProcessMagneticStarter = UVSTimeProcess.TimeProcessMagneticStarter;
                ModelUVS.TimeProcessMagneticStarter = UVSTimeProcess.TimeProcessMagneticStarter;
            }
        }
        public void SetTimeProcessOnPressure()
        {
            if (!MainWindow.FlagSimulation)
                mbMaster.WriteSingleRegister(SlaveAdress, 3, UVSTimeProcess.TimeProcessOnPressure);
            else
                ModelUVS.TimeProcessOnPressure = UVSTimeProcess.TimeProcessOnPressure;
        }
        public void SetTimeProcessOffPressure()
        {
            if (!MainWindow.FlagSimulation)
                mbMaster.WriteSingleRegister(SlaveAdress, 4, UVSTimeProcess.TimeProcessOffPressure);
            else
                ModelUVS.TimeProcessOffPressure = UVSTimeProcess.TimeProcessOffPressure;
        }
        public void SetTimeStopInPlace()
        {
            if (!MainWindow.FlagSimulation)
                mbMaster.WriteSingleRegister(SlaveAdress, 5, UVSTimeProcess.TimeStopInPlace);
            else
                ModelUVS.TimeStopInPlace = UVSTimeProcess.TimeStopInPlace;
        }

        public int CompareTo(UVS u)
        {
            return this.Name.CompareTo(u.Name);
        }

        private void ButtonTurnOn_Clik(object sender, RoutedEventArgs e)
        {
            TurnOn();
        }
        private void ButtonTurnOff_Clik(object sender, RoutedEventArgs e)
        {
            TurnOff();
        }

        private void MagneticStarterUI_MouseDown(object sender, RoutedEventArgs e)
        {
            if (!flagLockMagneticStarter)
            {

                Cmd += 4;

                if (!MainWindow.FlagSimulation)
                    mbMaster.WriteSingleRegister(SlaveAdress, 1, Cmd);
                else
                    ModelUVS.Cmd = Cmd;

                Cmd = 0;

                if (MagneticStarter)
                    MessageHeandler($"{DateTime.Now} :\"Вспомсистема - {Name}\" Команда отключить МП", Brushes.Black);
                else
                    MessageHeandler($"{DateTime.Now} :\"Вспомсистема - {Name}\" Команда включить МП", Brushes.Black);
            }
            else
                MessageHeandler($"{DateTime.Now} :\"Вспомсистема - {Name}\" Команда управления МП не выполнена. Установлена блокировка", Brushes.Red);
        }
        private void PressureUI_MouseDown(object sender, RoutedEventArgs e)
        {
            if (!flagLockPressure)
            {
                Cmd += 16;

                if (!MainWindow.FlagSimulation)
                    mbMaster.WriteSingleRegister(SlaveAdress, 1, Cmd);
                else
                    ModelUVS.Cmd = Cmd;

                Cmd = 0;

                if (MagneticStarter)
                    MessageHeandler($"{DateTime.Now} :\"Вспомсистема - {Name}\" Команда отключить давление", Brushes.Black);
                else
                    MessageHeandler($"{DateTime.Now} :\"Вспомсистема - {Name}\" Команда включить давление", Brushes.Black);
            }
            else
                MessageHeandler($"{DateTime.Now} :\"Вспомсистема - {Name}\" Команда управления давлением не выполнена. Установлена блокировка", Brushes.Red);
        }
        private void VoltageUI_MouseDown(object sender, RoutedEventArgs e)
        {
            if (!flagLockVoltage)
            {
                Cmd += 2;

                if (!MainWindow.FlagSimulation)
                    mbMaster.WriteSingleRegister(SlaveAdress, 1, Cmd);
                else
                    ModelUVS.Cmd = Cmd;

                Cmd = 0;

                if (Voltage)
                    MessageHeandler($"{DateTime.Now} :\"Вспомсистема - {Name}\" Команда отключить напряжение", Brushes.Black);
                else
                    MessageHeandler($"{DateTime.Now} :\"Вспомсистема - {Name}\" Команда включить напряжение", Brushes.Black);
            }
            else
                MessageHeandler($"{DateTime.Now} :\"Вспомсистема - {Name}\" Команда управления напряжением не выполнена. Установлена блокировка", Brushes.Red);
        }
        private void ShUI_MouseDown(object sender, RoutedEventArgs e)
        {
            if (!flagLockSH)
            {
                Cmd += 128;

                if (!MainWindow.FlagSimulation)
                    mbMaster.WriteSingleRegister(SlaveAdress, 1, Cmd);
                else
                    ModelUVS.Cmd = Cmd;

                Cmd = 0;

                if (SH)
                    MessageHeandler($"{DateTime.Now} :\"Вспомсистема - {Name}\" Команда отключить напряжение на СШ", Brushes.Black);
                else
                    MessageHeandler($"{DateTime.Now} :\"Вспомсистема - {Name}\" Команда включить напряжение на СШ", Brushes.Black);
            }
            else
                MessageHeandler($"{DateTime.Now} :\"Вспомсистема - {Name}\" Команда управления напрядением на СШ не выполнена. Установлена блокировка", Brushes.Red);
        }

        private void LabelMagneticStarter_MouseDown(object sender, RoutedEventArgs e)
        {
            if (!flagLockMagneticStarter)
            {
                Cmd += 4;

                if (!MainWindow.FlagSimulation)
                    mbMaster.WriteSingleRegister(SlaveAdress, 1, Cmd);
                else
                    ModelUVS.Cmd = Cmd;

                Cmd = 0;

                if (MagneticStarter)
                    MessageHeandler($"{DateTime.Now} :\"Вспомсистема - {Name}\" Команда отключить МП", Brushes.Black);
                else
                    MessageHeandler($"{DateTime.Now} :\"Вспомсистема - {Name}\" Команда включить МП", Brushes.Black);
            }
            else
                MessageHeandler($"{DateTime.Now} :\"Вспомсистема - {Name}\" Команда управления МП не выполнена. Установлена блокировка", Brushes.Red);
        }
        private void LabelPressure_MouseDown(object sender, RoutedEventArgs e)
        {
            if (!flagLockPressure)
            {
                Cmd += 16;

                if (!MainWindow.FlagSimulation)
                    mbMaster.WriteSingleRegister(SlaveAdress, 1, Cmd);
                else
                    ModelUVS.Cmd = Cmd;

                Cmd = 0;

                if (MagneticStarter)
                    MessageHeandler($"{DateTime.Now} :\"Вспомсистема - {Name}\" Команда отключить давление", Brushes.Black);
                else
                    MessageHeandler($"{DateTime.Now} :\"Вспомсистема - {Name}\" Команда включить давление", Brushes.Black);
            }
            else
                MessageHeandler($"{DateTime.Now} :\"Вспомсистема - {Name}\" Команда управления давлением не выполнена. Установлена блокировка", Brushes.Red);
        }
        private void LabelVoltage_MouseDown(object sender, RoutedEventArgs e)
        {
            if (!flagLockVoltage)
            {
                Cmd += 2;

                if (!MainWindow.FlagSimulation)
                    mbMaster.WriteSingleRegister(SlaveAdress, 1, Cmd);
                else
                    ModelUVS.Cmd = Cmd;

                Cmd = 0;

                if (Voltage)
                    MessageHeandler($"{DateTime.Now} :\"Вспомсистема - {Name}\" Команда отключить напряжение", Brushes.Black);
                else
                    MessageHeandler($"{DateTime.Now} :\"Вспомсистема - {Name}\" Команда включить напряжение", Brushes.Black);
            }
            else
                MessageHeandler($"{DateTime.Now} :\"Вспомсистема - {Name}\" Команда управления напряжением не выполнена. Установлена блокировка", Brushes.Red);
        }
        private void LabelSH_MouseDown(object sender, RoutedEventArgs e)
        {
            if (!flagLockSH)
            {
                Cmd += 64;

                if (!MainWindow.FlagSimulation)
                    mbMaster.WriteSingleRegister(SlaveAdress, 1, Cmd);
                else
                    ModelUVS.Cmd = Cmd;

                Cmd = 0;

                if (SH)
                    MessageHeandler($"{DateTime.Now} :\"Вспомсистема - {Name}\" Команда отключить напряжение на СШ", Brushes.Black);
                else
                    MessageHeandler($"{DateTime.Now} :\"Вспомсистема - {Name}\" Команда включить напряжение на СШ", Brushes.Black);
            }
            else
                MessageHeandler($"{DateTime.Now} :\"Вспомсистема - {Name}\" Команда управления напрядением на СШ не выполнена. Установлена блокировка", Brushes.Red);
        }

        private void PadlockMagneticStarter_MouseDown(object sender, RoutedEventArgs e)
        {
            if (padlockMagneticStarter.Source == BitmapClose)
            {
                padlockMagneticStarter.Source = BitmapOpen;
                flagLockMagneticStarter = false;
                MessageHeandler($"{DateTime.Now} :\"Вспомсистема - {Name}\" Команда отключить блокировку МП", Brushes.Black);               
            }
            else
            {
                padlockMagneticStarter.Source = BitmapClose;
                flagLockMagneticStarter = true;
                MessageHeandler($"{DateTime.Now} :\"Вспомсистема - {Name}\" Команда включить блокировку МП", Brushes.Black);
            }

            Cmd += 4096;

            if (!MainWindow.FlagSimulation)
                mbMaster.WriteSingleRegister(SlaveAdress, 1, Cmd);
            else
                ModelUVS.Cmd = Cmd;

            Cmd = 0;
        }
        private void PadlockPressure_MouseDown(object sender, RoutedEventArgs e)
        {
            if (padlockPressure.Source == BitmapClose)
            {
                padlockPressure.Source = BitmapOpen;
                flagLockPressure = false;
                MessageHeandler($"{DateTime.Now} :\"Вспомсистема - {Name}\" Команда отключить блокировку давления", Brushes.Black);
            }
            else
            {
                padlockPressure.Source = BitmapClose;
                flagLockPressure = true;
                MessageHeandler($"{DateTime.Now} :\"Вспомсистема - {Name}\" Команда включить блокировку давления", Brushes.Black);
            }

            Cmd += 8192;

            if (!MainWindow.FlagSimulation)
                mbMaster.WriteSingleRegister(SlaveAdress, 1, Cmd);
            else
                ModelUVS.Cmd = Cmd;

            Cmd = 0;
        }
        private void PadlockVoltage_MouseDown(object sender, RoutedEventArgs e)
        {
            if (padlockVoltage.Source == BitmapClose)
            {
                padlockVoltage.Source = BitmapOpen;
                flagLockVoltage = false;
                MessageHeandler($"{DateTime.Now} :\"Вспомсистема - {Name}\" Команда отключить блокировку напряжения", Brushes.Black);
            }
            else
            {
                padlockVoltage.Source = BitmapClose;
                flagLockVoltage = true;
                MessageHeandler($"{DateTime.Now} :\"Вспомсистема - {Name}\" Команда включить блокировку напряжения", Brushes.Black);
            }

            Cmd += 16384;

            if (!MainWindow.FlagSimulation)
                mbMaster.WriteSingleRegister(SlaveAdress, 1, Cmd);
            else
                ModelUVS.Cmd = Cmd;

            Cmd = 0;
        }
        private void PadlockSH_MouseDown(object sender, RoutedEventArgs e)
        {
            if (padlockSH.Source == BitmapClose)
            {
                padlockSH.Source = BitmapOpen;
                flagLockSH = false;
                MessageHeandler($"{DateTime.Now} :\"Вспомсистема - {Name}\" Команда отключить блокировку напряжения на СШ", Brushes.Black);
            }
            else
            {
                padlockSH.Source = BitmapClose;
                flagLockSH = true;
                MessageHeandler($"{DateTime.Now} :\"Вспомсистема - {Name}\" Команда включить блокировку напряжения на СШ", Brushes.Black);
            }

            Cmd += 32768;

            if (!MainWindow.FlagSimulation)
                mbMaster.WriteSingleRegister(SlaveAdress, 1, Cmd);
            else
                ModelUVS.Cmd = Cmd;

            Cmd = 0;
        }

        private void GridUvs_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Grid gridEvent = sender as Grid;
            PositionObject.Row = (int)gridEvent.GetValue(Grid.RowProperty);
            PositionObject.Column = (int)gridEvent.GetValue(Grid.ColumnProperty);

            UVSWindow.SetUvs(PositionObject, gridObject);
            UVSWindow.uVSSetHeandler();
        }

        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            UVSSettingsWindow uVSSettingsWindow = new UVSSettingsWindow()
            {
                Owner = MainWindow.uvsWindow
            };

            uVSSettingsWindow.timeProcessMagneticStarter.Text = this.UVSTimeProcess.TimeProcessMagneticStarter.ToString();
            uVSSettingsWindow.timeProcessOnPressure.Text = this.UVSTimeProcess.TimeProcessOnPressure.ToString();
            uVSSettingsWindow.timeProcessOffPressure.Text = this.UVSTimeProcess.TimeProcessOffPressure.ToString();
            uVSSettingsWindow.timeStopInPlace.Text = this.UVSTimeProcess.TimeStopInPlace.ToString();

            uVSSettingsWindow.ShowDialog();
        }
        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            UVSWindow.deleteHeandler(this.PositionObject);
        }
        private void Rename_Click(object sender, RoutedEventArgs e)
        {
            CreateWindow createWindow = new CreateWindow();
            createWindow.ShowDialog();
            Name = StartName;
            labelName.Content = StartName;
        }
        private void Adress_Click(object sender, RoutedEventArgs e)
        {
            UVSAdressSignal uvsAdressSignal = new UVSAdressSignal()
            {
                Owner = MainWindow.uvsWindow
            };

            uvsAdressSignal.magneticStarterAdress.Text = this.UVSAdress.MagneticStarterAdress.ToString();
            uvsAdressSignal.pressureAdress.Text = this.UVSAdress.PressureAdress.ToString();
            uvsAdressSignal.voltageAdress.Text = this.UVSAdress.VoltageAdress.ToString();
            uvsAdressSignal.shAdress.Text = this.UVSAdress.SHAdress.ToString();

            uvsAdressSignal.ShowDialog();
        }
    }
}
