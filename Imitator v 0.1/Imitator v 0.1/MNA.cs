using System;
using Modbus.Device;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Shapes;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Input;

/*Изображение и управление агрегатом MNA*/

namespace Imitator_v_0._1
{
    public delegate void MNASetHeandler();

    public class MNA : BaseObjectVU
    {
        public ModelMNA ModelMNA { get; set; }
        public MNATimeProcess MNATimeProcess { get; set; }
        
        public bool VVOn1 { get; set; }
        public bool VVOn2 { get; set; }
        public bool VVOff1 { get; set; }
        public bool VVOff2 { get; set; }
        public bool Current { get; set; }

        bool PrevVVOn1 { get; set; }
        bool PrevVVOn2 { get; set; }
        bool PrevVVOff1 { get; set; }
        bool PrevVVOff2 { get; set; }
        bool PrevCurrent { get; set; }

        public MNA(ModbusMaster mbMaster, byte slaveAdress, string name)
        {
            if (MainWindow.FlagSimulation)
                ModelMNA = new ModelMNA();

            MNATimeProcess = new MNATimeProcess();
            this.mbMaster = mbMaster;
            SlaveAdress = slaveAdress;
            Name = name;

            if (MainWindow.FlagSimulation)
                ModelMNA = new ModelMNA();

            PositionObject = new Position();
        }

        public void SetTimeProcessVVOn()
        {
            if (!MainWindow.FlagSimulation)
                mbMaster.WriteSingleRegister(SlaveAdress, 2, MNATimeProcess.TimeProcessVVOn);
            else
            {
                ModelMNA.timeProcessVVOn = MNATimeProcess.TimeProcessVVOn;
            }
        }
        public void SetTimeProcessVVOff()
        {
            if (!MainWindow.FlagSimulation)
                mbMaster.WriteSingleRegister(SlaveAdress, 2, MNATimeProcess.TimeProcessVVOff);
            else
            {
                ModelMNA.timeProcessVVOff = MNATimeProcess.TimeProcessVVOff;
            }
        }
        public void SetTimeProcessCurrentOn()
        {
            if (!MainWindow.FlagSimulation)
                mbMaster.WriteSingleRegister(SlaveAdress, 2, MNATimeProcess.TimeProcessCurrentOn);
            else
            {
                ModelMNA.timeProcessCurrentOn = MNATimeProcess.TimeProcessCurrentOn;
            }
        }
        public void SetTimeProcessCurrentOff()
        {
            if (!MainWindow.FlagSimulation)
                mbMaster.WriteSingleRegister(SlaveAdress, 2, MNATimeProcess.TimeProcessCurrentOff);
            else
            {
                ModelMNA.timeProcessCurrentOff = MNATimeProcess.TimeProcessCurrentOff;
            }
        }

        Image padlockVVOn1;
        Image padlockVVOn2;
        Image padlockVVOff1;
        Image padlockVVOff2;
        Image padlockCurrent;

        bool FlagLockVVOn1 { get; set; }
        bool FlagLockVVOn2 { get; set; }
        bool FlagLockVVOff1 { get; set; }
        bool FlagLockVVOff2 { get; set; }
        bool FlagLockCurrent { get; set; }

        public override UIElement Create() // создаем MNA
        {
            gridObject = new Grid()
            {
                Height = 236,
                Width = 260
            };

            Ellipse pumpUI = new Ellipse()
            {
                Name = "pumpUI",
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Height = 100,
                Width = 100,
                Margin = new Thickness(53, 16, 0, 0),
                Stroke = Brushes.Black
            };

            Border borderVV = new Border()
            {
                Name = "borderVV",
                BorderBrush = Brushes.Black,
                BorderThickness = new Thickness(1),
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Height = 44,
                Width = 85,
                Margin = new Thickness(10, 135, 0, 0)
            };
            Border borderCurrent = new Border()
            {
                Name = "borderCurrent",
                BorderBrush = Brushes.Black,
                BorderThickness = new Thickness(1),
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Height = 44,
                Width = 85,
                Margin = new Thickness(10, 185, 0, 0)
            };

            Grid gridVV = new Grid()
            {
                Name = "gridVV",
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Height = 44,
                Width = 85,
                Margin = new Thickness(-1)
            };
            Grid gridCurrent = new Grid()
            {
                Name = "gridCurrent",
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Height = 28,
                Width = 56,
                Margin = new Thickness(-1)
            };
            Grid gridCHRP = new Grid()
            {
                Name = "gridCHRP",
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Height = 63,
                Width = 56,
                Margin = new Thickness(-1)
            };

            Rectangle rectangleVVOn1 = new Rectangle()
            {
                Name = "rectangleVVOn1",
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Height = 10,
                Width = 23,
                Margin = new Thickness(17, 6, 0, 0),
                Stroke = Brushes.Black
            };
            Rectangle rectangleVVOn2 = new Rectangle()
            {
                Name = "rectangleVVOn2",
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Height = 10,
                Width = 23,
                Margin = new Thickness(44, 6, 0, 0),
                Stroke = Brushes.Black
            };
            Rectangle rectangleVVoff1 = new Rectangle()
            {
                Name = "rectangleVVoff1",
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Height = 10,
                Width = 23,
                Margin = new Thickness(17, 27, 0, 0),
                Stroke = Brushes.Black
            };
            Rectangle rectangleVVoff2 = new Rectangle()
            {
                Name = "rectangleVVoff2",
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Height = 10,
                Width = 23,
                Margin = new Thickness(44, 27, 0, 0),
                Stroke = Brushes.Black
            };
            Rectangle rectangleCurrent = new Rectangle()
            {
                Name = "rectangleCurrent",
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Height = 10,
                Width = 23,
                Margin = new Thickness(17, 6, 0, 0),
                Stroke = Brushes.Black
            };
            Rectangle rectangleRes1 = new Rectangle()
            {
                Name = "rectangleRes1",
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Height = 10,
                Width = 23,
                Margin = new Thickness(30, 3, 0, 0),
                Stroke = Brushes.Black
            };// резерв
            Rectangle rectangleRes2 = new Rectangle()
            {
                Name = "rectangleRes2",
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Height = 10,
                Width = 23,
                Margin = new Thickness(3, 16, 0, 0),
                Stroke = Brushes.Black
            };// резерв
            Rectangle rectangleRes3 = new Rectangle()
            {
                Name = "rectangleRes3",
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Height = 10,
                Width = 23,
                Margin = new Thickness(30, 16, 0, 0),
                Stroke = Brushes.Black
            };// резерв

            TextBlock tbVVon2 = new TextBlock()
            {
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Height = 10,
                Width = 23,
                Margin = new Thickness(44, 5, 0, 0),
                Text = "ВКЛ",
                TextWrapping = TextWrapping.Wrap,
                FontSize = 8,
                Padding = new Thickness(3, 0, 0, 0),
                FontWeight = FontWeights.Bold
            };
            TextBlock tbVVon1 = new TextBlock()
            {
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Height = 10,
                Width = 23,
                Margin = new Thickness(17, 4, 0, 0),
                Text = "ВКЛ",
                TextWrapping = TextWrapping.Wrap,
                FontSize = 8,
                Padding = new Thickness(3, 1, 0, 0),
                FontWeight = FontWeights.Bold
            };
            TextBlock tbVVoff2 = new TextBlock()
            {
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Height = 10,
                Width = 23,
                Margin = new Thickness(44, 25, 0, 0),
                Text = "ОТК",
                TextWrapping = TextWrapping.Wrap,
                FontSize = 8,
                Padding = new Thickness(3, 1, 0, 0),
                FontWeight = FontWeights.Bold
            };
            TextBlock tbVVoff1 = new TextBlock()
            {
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Height = 10,
                Width = 23,
                Margin = new Thickness(17, 25, 0, 0),
                Text = "ОТК",
                TextWrapping = TextWrapping.Wrap,
                FontSize = 8,
                Padding = new Thickness(3, 1, 0, 0),
                FontWeight = FontWeights.Bold
            };
            TextBlock tbCurrent = new TextBlock()
            {
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Height = 10,
                Width = 23,
                Margin = new Thickness(17, 4, 0, 0),
                Text = "ТОК",
                TextWrapping = TextWrapping.Wrap,
                FontSize = 8,
                Padding = new Thickness(3, 1, 0, 0),
                FontWeight = FontWeights.Bold
            };
            TextBlock tbButtonCHRP = new TextBlock()
            {
                TextAlignment = TextAlignment.Center,
                TextWrapping = TextWrapping.Wrap,
                Text = "Стоп с ЧРП",
                Height = 25,
                Width = 44,
                FontSize = 9
            };
            TextBlock tbButtonBRU = new TextBlock()
            {
                TextAlignment = TextAlignment.Center,
                TextWrapping = TextWrapping.Wrap,
                Text = "Стоп с БРУ",
                Height = 25,
                Width = 44,
                FontSize = 9
            };
            TextBlock tbButtonPlace = new TextBlock()
            {
                TextAlignment = TextAlignment.Center,
                TextWrapping = TextWrapping.Wrap,
                Text = "Стоп по Месту",
                Height = 25,
                Width = 44,
                FontSize = 9
            };

            Button buttonTurnOn = new Button()
            {
                Name = "buttonTurnOn",
                Content = "Пуск",
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Height = 29,
                Width = 50,
                Margin = new Thickness(199, 102, 0, 0)
            };
            Button buttonTurnOff = new Button()
            {
                Name = "buttonTurnOff",
                Content = "Стоп",
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Height = 29,
                Width = 50,
                Margin = new Thickness(199, 133, 0, 0)
            };
            Button stopByPlace = new Button()
            {
                Name = "stopByPlace",
                Content = tbButtonPlace,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Height = 29,
                Width = 50,
                Margin = new Thickness(199, 6, 0, 0)
            };
            Button stopByBRU = new Button()
            {
                Name = "stopByBRU",
                Content = tbButtonBRU,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Height = 29,
                Width = 50,
                Margin = new Thickness(199, 36, 0, 0)
            };
            Button stopByCHRP = new Button()
            {
                Name = "stopByCHRP",
                Content = tbButtonCHRP,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Height = 29,
                Width = 50,
                Margin = new Thickness(199, 66, 0, 0),
            };

            padlockVVOn1 = new Image()
            {
                Name = "padlockVVOn1",
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Margin = new Thickness(3, 3, 0, 0),
                Height = 13,
                Width = 14,
                Source = BitmapOpen
            };
            padlockVVOff1 = new Image()
            {
                Name = "padlockVVOn2",
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Margin = new Thickness(3, 25, 0, 0),
                Height = 13,
                Width = 14,
                Source = BitmapOpen
            };
            padlockVVOn2 = new Image()
            {
                Name = "padlockVVOff1",
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Margin = new Thickness(69, 3, 0, 0),
                Height = 13,
                Width = 14,
                Source = BitmapOpen
            };
            padlockVVOff2 = new Image()
            {
                Name = "padlockVVOff2",
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Height = 13,
                Width = 14,
                Margin = new Thickness(69, 25, 0, 0),
                Source = BitmapOpen

            };
            padlockCurrent = new Image()
            {
                Name = "padlockCurrent",
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Height = 13,
                Width = 14,
                Margin = new Thickness(3, 3, 0, 0),
                Source = BitmapOpen

            };

            Label lbCHRP = new Label()
            {
                Content = "ЧРП",
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Margin = new Thickness(10, 3, 0, 0)
            };
            labelName = new Label()
            {
                Name = "name",
                Content = "",
                HorizontalAlignment = HorizontalAlignment.Left,
                Margin = new Thickness(77, 50, 0, 0),
                VerticalAlignment = VerticalAlignment.Top,
                FontFamily = new FontFamily("Verdana"),
                FontSize = 14,
                FontWeight = FontWeights.Bold,
                Height = 29,
                Width = 100
            };

            buttonTurnOn.Click += ButtonTurnOn_Click;
            buttonTurnOff.Click += ButtonTurnOff_Click;
            stopByPlace.Click += StopByPlace_Click;
            stopByBRU.Click += StopByBRU_Click;
            padlockVVOn1.MouseDown += PadlockVVOn1_MouseDown;
            padlockVVOff1.MouseDown += PadlockVVOff1_MouseDown;
            padlockVVOn2.MouseDown += PadlockVVOn2_MouseDown;
            padlockVVOff2.MouseDown += PadlockVVOff2_MouseDown;
            padlockCurrent.MouseDown += PadlockCurrent_MouseDown;

            borderVV.Child = gridVV;
            borderCurrent.Child = gridCurrent;

            gridVV.Children.Add(rectangleVVOn1);
            gridVV.Children.Add(rectangleVVOn2);
            gridVV.Children.Add(rectangleVVoff1);
            gridVV.Children.Add(rectangleVVoff2);
            gridVV.Children.Add(tbVVon1);
            gridVV.Children.Add(tbVVon2);
            gridVV.Children.Add(tbVVoff1);
            gridVV.Children.Add(tbVVoff2);

            gridVV.Children.Add(padlockVVOn1);
            gridVV.Children.Add(padlockVVOn2);
            gridVV.Children.Add(padlockVVOff1);
            gridVV.Children.Add(padlockVVOff2);

            gridCHRP.Children.Add(lbCHRP);

            gridCurrent.Children.Add(rectangleCurrent);
            gridCurrent.Children.Add(tbCurrent);
            gridCurrent.Children.Add(padlockCurrent);

            gridObject.MouseUp += GridMna_MouseUp;

            gridObject.Children.Add(pumpUI);
            gridObject.Children.Add(buttonTurnOn);
            gridObject.Children.Add(buttonTurnOff);
            gridObject.Children.Add(stopByPlace);
            gridObject.Children.Add(stopByBRU);
            gridObject.Children.Add(stopByCHRP);
            gridObject.Children.Add(borderVV);
            gridObject.Children.Add(borderCurrent);
            gridObject.Children.Add(labelName);

            elements.Add(rectangleVVOn1);
            elements.Add(rectangleVVOn2);
            elements.Add(rectangleVVoff1);
            elements.Add(rectangleVVoff2);
            elements.Add(rectangleCurrent);
            elements.Add(pumpUI);

            if (gridObject.Children[gridObject.Children.IndexOf(labelName)] is Label) // выставляем имя
                ((Label)(gridObject.Children[gridObject.Children.IndexOf(labelName)])).Content = Name;

            MessageHeandler($"{DateTime.Now} :Создан объект \"МНА - {Name}\"", Brushes.Black);

            return gridObject;
        }
        public override void TurnOn() // включение
        {
            Cmd += 1024;

            if (!MainWindow.FlagSimulation)
                mbMaster.WriteSingleRegister(SlaveAdress, 1, Cmd);
            else
                ModelMNA.Cmd = Cmd;
            Cmd = 0;
            MessageHeandler($"{DateTime.Now} :\"МНА - {Name}\" Команда пуск по месту", Brushes.Black);
        }
        public override void TurnOff() // отключение
        {
            Cmd += 4096;

            if (!MainWindow.FlagSimulation)
                mbMaster.WriteSingleRegister(SlaveAdress, 1, Cmd);
            else
                ModelMNA.Cmd = Cmd;
            Cmd = 0;
            MessageHeandler($"{DateTime.Now} :\"МНА - {Name}\" Команда стоп по месту", Brushes.Black);
        }
        public override void Survey() // опрос
        {
            BitArray bA = ModBusSurveyMass();

            VVOn1 = bA.Get(0);
            VVOn2 = bA.Get(1);
            VVOff1 = bA.Get(2);
            VVOff2 = bA.Get(3);
            Current = bA.Get(4);

            if (PrevVVOn1 == false && VVOn1 == true)
                MessageHeandler($"{DateTime.Now} :\"МНА - {Name}\" ВВ включен сигнал 1 установлен", Brushes.Green);
            else if (PrevVVOn1 == true && VVOn1 == false)
                MessageHeandler($"{DateTime.Now} :\"МНА - {Name}\" ВВ включен сигнал 1 снят", Brushes.Red);

            if (PrevVVOn2 == false && VVOn2 == true)
                MessageHeandler($"{DateTime.Now} :\"МНА - {Name}\" ВВ включен сигнал 2 установлен", Brushes.Green);
            else if (PrevVVOn2 == true && VVOn2 == false)
                MessageHeandler($"{DateTime.Now} :\"МНА - {Name}\" ВВ включен сигнал 2 снят", Brushes.YellowGreen);

            if (PrevVVOff1 == false && VVOff1 == true)
                MessageHeandler($"{DateTime.Now} :\"МНА - {Name}\" ВВ отключен сигнал 1 установлен", Brushes.Green);
            else if (PrevVVOff1 == true && VVOff1 == false)
                MessageHeandler($"{DateTime.Now} :\"МНА - {Name}\" ВВ отключен сигнал 1 снят", Brushes.YellowGreen);

            if (PrevVVOff2 == false && VVOff2 == true)
                MessageHeandler($"{DateTime.Now} :\"МНА - {Name}\" ВВ отключен сигнал 2 установлен", Brushes.Green);
            else if (PrevVVOff2 == true && VVOff2 == false)
                MessageHeandler($"{DateTime.Now} :\"МНА - {Name}\" ВВ отключен сигнал 2 снят", Brushes.Red);

            if (PrevCurrent == false && Current == true)
                MessageHeandler($"{DateTime.Now} :\"МНА - {Name}\" Ток набран", Brushes.Green);
            else if (PrevCurrent == true && Current == false)
                MessageHeandler($"{DateTime.Now} :\"МНА - {Name}\" Ток сброшен", Brushes.Red);

            PrevVVOn1 = VVOn1;
            PrevVVOn2 = VVOn2;
            PrevVVOff1 = VVOff1;
            PrevVVOff2 = VVOff2;
            PrevCurrent = Current;
        }
        public override BitArray ModBusSurveyMass() // разбиваем state на биты
        {
            if (!MainWindow.FlagSimulation)
                State = mbMaster.ReadHoldingRegisters(SlaveAdress, 0, 1)[0];
            else
                State = ModelMNA.State;

            bStateMass[0] = (byte)State;

            return new BitArray(bStateMass);
        }

        public override void RegisterUVSMessageHeandler(MessageHeandler del)
        {
            MessageHeandler += del;
        }
        public override void UnregisterUVSMessageHeandler(MessageHeandler del)
        {
            MessageHeandler -= del;
        }

        private void ButtonTurnOn_Click(object sender, RoutedEventArgs e)
        {
            TurnOn();
        }
        private void ButtonTurnOff_Click(object sender, RoutedEventArgs e)
        {
            TurnOff();
        }
        private void StopByPlace_Click(object sender, RoutedEventArgs e)
        {
            Cmd += 8192;

            if (!MainWindow.FlagSimulation)
                mbMaster.WriteSingleRegister(SlaveAdress, 1, Cmd);
            else
                ModelMNA.Cmd = Cmd;
            Cmd = 0;
            MessageHeandler($"{DateTime.Now} :\"МНА - {Name}\" Команда пуск по месту", Brushes.Black);
        }
        private void StopByBRU_Click(object sender, RoutedEventArgs e)
        {
            Cmd += 16384;

            if (!MainWindow.FlagSimulation)
                mbMaster.WriteSingleRegister(SlaveAdress, 1, Cmd);
            else
                ModelMNA.Cmd = Cmd;
            Cmd = 0;
            MessageHeandler($"{DateTime.Now} :\"МНА - {Name}\" Команда пуск по месту", Brushes.Black);
        }

        private void PadlockVVOn1_MouseDown(object sender, RoutedEventArgs e)
        {
            if (padlockVVOn1.Source == BitmapClose)
            {
                padlockVVOn1.Source = BitmapOpen;
                FlagLockVVOn1 = false;
                MessageHeandler($"{DateTime.Now} :\"Вспомсистема - {Name}\" Команда отключить блокировку ВВ включен сигнал 1", Brushes.Black);
            }
            else
            {
                padlockVVOn1.Source = BitmapClose;
                FlagLockVVOn1 = true;
                MessageHeandler($"{DateTime.Now} :\"Вспомсистема - {Name}\" Команда включить блокировку ВВ включен сигнал 1", Brushes.Black);
            }

            Cmd += 32;

            if (!MainWindow.FlagSimulation)
                mbMaster.WriteSingleRegister(SlaveAdress, 1, Cmd);
            else
                ModelMNA.Cmd = Cmd;

            Cmd = 0;
        }
        private void PadlockVVOn2_MouseDown(object sender, RoutedEventArgs e)
        {
            if (padlockVVOn2.Source == BitmapClose)
            {
                padlockVVOn2.Source = BitmapOpen;
                FlagLockVVOn2 = false;
                MessageHeandler($"{DateTime.Now} :\"Вспомсистема - {Name}\" Команда отключить блокировку ВВ включен сигнал 2", Brushes.Black);
            }
            else
            {
                padlockVVOn2.Source = BitmapClose;
                FlagLockVVOn2 = true;
                MessageHeandler($"{DateTime.Now} :\"Вспомсистема - {Name}\" Команда включить блокировку ВВ включен сигнал 2", Brushes.Black);
            }

            Cmd += 64;

            if (!MainWindow.FlagSimulation)
                mbMaster.WriteSingleRegister(SlaveAdress, 1, Cmd);
            else
                ModelMNA.Cmd = Cmd;

            Cmd = 0;
        }
        private void PadlockVVOff1_MouseDown(object sender, RoutedEventArgs e)
        {
            if (padlockVVOff1.Source == BitmapClose)
            {
                padlockVVOff1.Source = BitmapOpen;
                FlagLockVVOff1 = false;
                MessageHeandler($"{DateTime.Now} :\"Вспомсистема - {Name}\" Команда отключить блокировку ВВ отключен сигнал 1", Brushes.Black);
            }
            else
            {
                padlockVVOff1.Source = BitmapClose;
                FlagLockVVOff1 = true;
                MessageHeandler($"{DateTime.Now} :\"Вспомсистема - {Name}\" Команда включить блокировку ВВ отключен сигнал 1", Brushes.Black);
            }

            Cmd += 128;

            if (!MainWindow.FlagSimulation)
                mbMaster.WriteSingleRegister(SlaveAdress, 1, Cmd);
            else
                ModelMNA.Cmd = Cmd;

            Cmd = 0;
        }
        private void PadlockVVOff2_MouseDown(object sender, RoutedEventArgs e)
        {
            if (padlockVVOff2.Source == BitmapClose)
            {
                padlockVVOff2.Source = BitmapOpen;
                FlagLockVVOff2 = false;
                MessageHeandler($"{DateTime.Now} :\"Вспомсистема - {Name}\" Команда отключить блокировку ВВ отключен сигнал 2", Brushes.Black);
            }
            else
            {
                padlockVVOff2.Source = BitmapClose;
                FlagLockVVOff2 = true;
                MessageHeandler($"{DateTime.Now} :\"Вспомсистема - {Name}\" Команда включить блокировку ВВ отключен сигнал 2", Brushes.Black);
            }

            Cmd += 256;

            if (!MainWindow.FlagSimulation)
                mbMaster.WriteSingleRegister(SlaveAdress, 1, Cmd);
            else
                ModelMNA.Cmd = Cmd;

            Cmd = 0;
        }
        private void PadlockCurrent_MouseDown(object sender, RoutedEventArgs e)
        {
            if (padlockCurrent.Source == BitmapClose)
            {
                padlockCurrent.Source = BitmapOpen;
                FlagLockCurrent = false;
                MessageHeandler($"{DateTime.Now} :\"Вспомсистема - {Name}\" Команда отключить блокировку тока", Brushes.Black);
            }
            else
            {
                padlockCurrent.Source = BitmapClose;
                FlagLockCurrent = true;
                MessageHeandler($"{DateTime.Now} :\"Вспомсистема - {Name}\" Команда включить блокировку тока", Brushes.Black);
            }

            Cmd += 512;

            if (!MainWindow.FlagSimulation)
                mbMaster.WriteSingleRegister(SlaveAdress, 1, Cmd);
            else
                ModelMNA.Cmd = Cmd;

            Cmd = 0;
        }

        private void GridMna_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Grid gridEvent = sender as Grid;
            PositionObject.Row = (int)gridEvent.GetValue(Grid.RowProperty);
            PositionObject.Column = (int)gridEvent.GetValue(Grid.ColumnProperty);

            MNAWindow.SetMNA(PositionObject, gridObject);
            MNAWindow.mnaSetHeandler();
        }
    }
}
