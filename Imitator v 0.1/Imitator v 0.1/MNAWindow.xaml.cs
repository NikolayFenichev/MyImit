using Modbus.Device;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Threading;
using System.IO.Ports;
using System.Diagnostics;

/*Окно объектов MNA*/

namespace Imitator_v_0._1
{
    // <summary>
    // Логика взаимодействия для MNAWindow.xaml
    // </summary>
    public partial class MNAWindow : Window
    {
        static int countRow = 0;
        static int countRowForButton = 0;
        static int countColumnForButton = 0;
        static Border borderMNA;
        static Grid gridObject;
        static List<MNA> massMNA = new List<MNA>();
        static public long Timer { get; set; }
        static public MNASetHeandler mnaSetHeandler;
        static public DeleteHeandler deleteHeandler;
        public static Position PositionMNA { get; set; } = new Position();
        bool exitFlag = false;

        Button btnAddGrid = new Button()
        {
            Name = "AddNewUvs",
            Content = "+",
            HorizontalAlignment = HorizontalAlignment.Left,
            Margin = new Thickness(10, 10, 0, 0),
            VerticalAlignment = VerticalAlignment.Top,
            Width = 240,
            Height = 217,
        };

        Stopwatch sw = new Stopwatch();

        public MNAWindow()
        {
            InitializeComponent();
        }

        public static int CountMNA
        {
            get
            {
                return massMNA.Count;
            }
        }
        public static MNA GetMNA(int number)
        {
            return massMNA[number - 1];
        }
        public static void RegisterDelegate(MNASetHeandler del)
        {
            mnaSetHeandler = del;
        }

        void UpdateVU() // обновление картинки
        {
            while (true)
            {
                if (exitFlag) break;
                Dispatcher.Invoke(new Action(delegate ()
                {
                    OutVu();

                }));
                Thread.Sleep(100);
            }
        }
        void UpdateSurvey() // опрос
        {
            while (true)
            {
                if (MainWindow.FlagSimulation)
                {
                    for (int i = 0; i < massMNA.Count; i++)
                    {
                        massMNA[i].Survey();
                    }
                }
                else
                {
                    for (int i = 0; i < massMNA.Count; i++)
                    {
                        massMNA[i].Survey();
                    }
                }
                Thread.Sleep(200);
            }
        }
        void UpdateModelMNA() // обновление логической модели
        {
            sw.Start();
            while (true)
            {
                Timer = sw.ElapsedMilliseconds;

                for (int i = 0; i < massMNA.Count; i++)
                {
                    massMNA[i].ModelMNA.Update();
                }
                Thread.Sleep(200);
            }
        }

        public static void OutVu() // отрисовка цвета сигналов
        {
            for (byte i = 0; i < massMNA.Count; i++)
            {
                if (massMNA[i].VVOn1 && massMNA[i].VVOn2 && massMNA[i].Current && !massMNA[i].VVOff1 && !massMNA[i].VVOff2)
                {
                    massMNA[i].SetColor("pumpUI", Brushes.Lime);
                }
                else massMNA[i].SetColor("pumpUI", Brushes.Yellow);

                if (massMNA[i].VVOn1)
                {
                    massMNA[i].SetColor("rectangleVVOn1", Brushes.Lime);
                }
                else massMNA[i].SetColor("rectangleVVOn1", Brushes.Yellow);

                if (massMNA[i].VVOn2)
                {
                    massMNA[i].SetColor("rectangleVVOn2", Brushes.Lime);
                }
                else massMNA[i].SetColor("rectangleVVOn2", Brushes.Yellow);

                if (massMNA[i].VVOff1)
                {
                    massMNA[i].SetColor("rectangleVVoff1", Brushes.Lime);
                }
                else massMNA[i].SetColor("rectangleVVoff1", Brushes.Yellow);

                if (massMNA[i].VVOff2)
                {
                    massMNA[i].SetColor("rectangleVVoff2", Brushes.Lime);
                }
                else massMNA[i].SetColor("rectangleVVoff2", Brushes.Yellow);

                if (massMNA[i].Current)
                {
                    massMNA[i].SetColor("rectangleCurrent", Brushes.Lime);
                }
                else massMNA[i].SetColor("rectangleCurrent", Brushes.Yellow);
            }
        }
        private void Draw() // отображение на экран
        {
            gridTable.Children.Remove(btnAddGrid);

            UIElement ui = massMNA[massMNA.Count - 1].Create();
            massMNA[massMNA.Count - 1].SetName();
            Grid.SetRow(ui, countRow);
            Grid.SetColumn(ui, (massMNA.Count - 1) - countRow * gridTable.ColumnDefinitions.Count);
            gridTable.Children.Add(ui);

            if (countColumnForButton == 3)
            {
                gridTable.RowDefinitions.Insert(gridTable.RowDefinitions.Count, new RowDefinition() { Height = new GridLength(180) });
                gridTable.Height += 180;
                countRowForButton++;
                countColumnForButton = -1;
            }
            countColumnForButton++;

            AddButton();

            if (massMNA.Count % gridTable.ColumnDefinitions.Count == 0)
                countRow++;
        }
        private void Draw(Position positionUvs) // добавление в конкретную позицию
        {
            int startDraw = positionUvs.PositionInMass;
            int endDraw = massMNA.Count;

            if (startDraw == endDraw)
            {
                gridTable.Children.Remove(btnAddGrid);

                AddButton();
            }
            else
            {
                while (startDraw < endDraw)
                {
                    gridTable.Children.Remove(btnAddGrid);

                    if (countColumnForButton >= 3)
                    {
                        countRowForButton++;
                        countColumnForButton = -1;
                    }
                    {
                        UIElement ui = massMNA[startDraw].Create();
                        massMNA[startDraw].SetName();
                        Grid.SetRow(ui, countRow);
                        Grid.SetColumn(ui, (startDraw) - countRow * gridTable.ColumnDefinitions.Count);
                        gridTable.Children.Add(ui);
                        countColumnForButton++;
                        AddButton();
                    }
                    startDraw++;
                };
            }
        }
        private void Delete(Position positionUvs) // удаление MNA
        {
            for (int i = positionUvs.PositionInMass; i < massMNA.Count; i++)
                ((Grid)(gridTable.Children[i])).ContextMenu.Items.Clear();

            gridTable.Children.RemoveRange(positionUvs.PositionInMass, massMNA.Count - (positionUvs.PositionInMass));
            massMNA.RemoveAt(positionUvs.PositionInMass);
            countRow = positionUvs.Row;
            countRowForButton = positionUvs.Row;
            countColumnForButton = positionUvs.Column;
            Draw(positionUvs);
        }
        void AddObject(List<MNA> massMNA) // добавление объекта MNA
        {
            MNA mna = new MNA(MainWindow.MbMaster, (byte)(massMNA.Count + 1), MNA.StartName);
            mna.RegisterUVSMessageHeandler(AddMessage);
            massMNA.Add(mna);

            massMNA[massMNA.Count - 1].SetTimeProcessVVOn();
            massMNA[massMNA.Count - 1].SetTimeProcessVVOff();
            massMNA[massMNA.Count - 1].SetTimeProcessCurrentOn();
            massMNA[massMNA.Count - 1].SetTimeProcessCurrentOff();

            Draw();
        }
        private void AddButton() // добавление кнопки "Добаввить объект"
        {
            Grid.SetRow(btnAddGrid, countRowForButton);
            Grid.SetColumn(btnAddGrid, countColumnForButton);
            gridTable.Children.Add(btnAddGrid);
        }
        public void AddBorder() // добавление выделяющей рамки
        {
            Console.WriteLine($"в главном гриде {gridTable.Children.Count} элементов до удаления");
            gridTable.Children.Remove(borderMNA);
            Console.WriteLine($"в главном гриде {gridTable.Children.Count} элементов после удаления");
            borderMNA = new Border()
            {
                Name = "BorderUvs",
                Width = gridObject.Width,
                Height = gridObject.Height,
                BorderBrush = Brushes.Black,
                BorderThickness = new Thickness(2)
            };
            Grid.SetRow(borderMNA, PositionMNA.Row);
            Grid.SetColumn(borderMNA, PositionMNA.Column);
            gridTable.Children.Add(borderMNA);
        }
        void AddMessage(string s, SolidColorBrush colorText) //добавление сообщения
        {
            panelMessage.Dispatcher.Invoke(new Action(() =>
            {
                ((Paragraph)panelMessage.Document.Blocks.FirstBlock).LineHeight = 1;
                TextRange range = new TextRange(panelMessage.Document.ContentEnd, panelMessage.Document.ContentEnd);
                range.Text = s + "\n";
                range.ApplyPropertyValue(TextElement.ForegroundProperty, colorText);
                panelMessage.ScrollToEnd();
            }));
        }

        public static void SetMNA(Position positionUvs, Grid grid) // выбор MNA
        {
            PositionMNA = positionUvs;
            gridObject = grid;
        }
        public void SetTimeMNA(Position position, MNATimeProcess mnaTimeProcess) // установка таймеров MNA
        {
            massMNA[position.PositionInMass].MNATimeProcess = mnaTimeProcess;

            massMNA[position.PositionInMass].SetTimeProcessVVOn();
            massMNA[position.PositionInMass].SetTimeProcessVVOff();
            massMNA[position.PositionInMass].SetTimeProcessCurrentOn();
            massMNA[position.PositionInMass].SetTimeProcessCurrentOff();
        }

        private void AddGrid_Clik(object sender, RoutedEventArgs e)
        {

            CreateWindow addWindow = new CreateWindow();
            addWindow.Owner = this;

            if (addWindow.ShowDialog() == true)
                AddObject(massMNA);
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Grid.SetRow(btnAddGrid, 0);
            Grid.SetColumn(btnAddGrid, 0);
            gridTable.ShowGridLines = true;
            gridTable.Children.Add(btnAddGrid);
            RegisterDelegate(AddBorder);
            deleteHeandler = Delete;

            btnAddGrid.Click += AddGrid_Clik;
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            exitFlag = true;
        }


        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            Thread trVU = new Thread(UpdateVU);
            Thread trSurveyl = new Thread(UpdateSurvey);
            trVU.Start();
            trSurveyl.Start();      
            if (MainWindow.FlagSimulation)
            {
                Thread trModel = new Thread(UpdateModelMNA);
                trModel.Start();
            }
        }
    }
}
