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
    public delegate void DeleteHeandler(Position position);

    public partial class UVSWindow : Window
    {
        static int countRow = 0;
        static int countRowForButton = 0;
        static int countColumnForButton = 0;
        static Border borderUVS;
        static List<UVS> massUVS = new List<UVS>();
        static Grid gridObject;
        static public long Timer { get; set; }
        static public UVSSetHeandler uVSSetHeandler;
        static public DeleteHeandler deleteHeandler;
        static public Position PositionUvs { get; set; } = new Position();
        bool listSorted = false;
        bool exitFlag = false;

        Button btnAddGrid = new Button()
        {
            Name = "AddNewUvs",
            Content = "+",
            HorizontalAlignment = HorizontalAlignment.Left,
            Margin = new Thickness(10, 10, 0, 0),
            VerticalAlignment = VerticalAlignment.Top,
            Width = 170,
            Height = 153,
        };
        Stopwatch sw = new Stopwatch();

        public UVSWindow()
        {
            InitializeComponent();
        }

        public static int CountUVS
        {
            get
            {
                return massUVS.Count;
            }
        }
        public static UVS GetUVS(int number)
        {
            return massUVS[number - 1];
        }
        public static void RegisterDelegate(UVSSetHeandler del)
        {
            uVSSetHeandler = del;
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
                    for (int i = 0; i < massUVS.Count; i++)
                    {
                        massUVS[i].Survey();
                    }
                }
                else
                {
                    for (int i = 0; i < massUVS.Count; i++)
                    {
                        massUVS[i].Survey();
                    }
                }
                Thread.Sleep(200);
            }
        }
        void UpdateModelUVS() // обновление логической модели
        {
            sw.Start();
            while (true)
            {
                Timer = sw.ElapsedMilliseconds;

                for (int i = 0; i < massUVS.Count; i++)
                {
                    massUVS[i].ModelUVS.Update();
                }
                Thread.Sleep(200);
            }
        }

        
        public static void OutVu() // отрисовка цвета сигналов
        {
            for (byte i = 0; i < massUVS.Count; i++)
            {
                if (massUVS[i].MagneticStarter && massUVS[i].Pressure && !massUVS[i].ProcessOn && !massUVS[i].ProcessOff)
                    massUVS[i].SetColor("pumpUI", Brushes.Lime);
                else if (!massUVS[i].ProcessOn && !massUVS[i].ProcessOff) massUVS[i].SetColor("pumpUI", Brushes.Yellow);

                if (massUVS[i].Voltage)
                {
                    massUVS[i].SetColor("voltageUI", Brushes.Lime);
                }
                else massUVS[i].SetColor("voltageUI", Brushes.Red);

                if (massUVS[i].MagneticStarter)
                {
                    massUVS[i].SetColor("magneticStarterUI", Brushes.Lime);
                }
                else massUVS[i].SetColor("magneticStarterUI", Brushes.Yellow);

                if (massUVS[i].Pressure)
                {
                    massUVS[i].SetColor("pressureUI", Brushes.Lime);
                }
                else massUVS[i].SetColor("pressureUI", Brushes.Yellow);

                if (massUVS[i].SH)
                {
                    massUVS[i].SetColor("shUI", Brushes.Lime);
                }
                else massUVS[i].SetColor("shUI", Brushes.Yellow);
            }
        }
        private void Draw() // отображение на экран
        {
            gridTable.Children.Remove(btnAddGrid);
            massUVS[massUVS.Count - 1].SetName();
            UIElement ui = massUVS[massUVS.Count - 1].Create();
            Grid.SetRow(ui, countRow);
            Grid.SetColumn(ui, (massUVS.Count - 1) - countRow * gridTable.ColumnDefinitions.Count);
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
            if (massUVS.Count % gridTable.ColumnDefinitions.Count == 0)
                countRow++;
        }
        private void Draw(Position positionUvs) // отображение в конкретную позицию
        {
            int startDraw = positionUvs.PositionInMass;
            int endDraw = massUVS.Count;
            int count = massUVS.Count - startDraw;

            if (startDraw == endDraw)
            {
                gridTable.Children.Remove(btnAddGrid);
                AddButton();
            }
            else
            {
                while (startDraw < endDraw)
                {
                    if (countColumnForButton == 3)
                    {
                        countRowForButton++;
                        countColumnForButton = -1;
                    }
                    gridTable.Children.Remove(btnAddGrid);
                    {
                        UIElement ui = massUVS[startDraw].Create();
                        Grid.SetRow(ui, countRow);
                        Grid.SetColumn(ui, startDraw - countRow * gridTable.ColumnDefinitions.Count);
                        gridTable.Children.Add(ui);
                        countColumnForButton++;
                        AddButton();
                    }
                    if (countColumnForButton == 3)
                    {
                        countRowForButton++;
                        countColumnForButton = -1;
                    }
                    count--;
                    startDraw++;
                    if (startDraw % (gridTable.ColumnDefinitions.Count) == 0)
                        countRow++;
                };
            }
        }
        private void Delete(Position positionUvs) // удаление UVS
        {
            int numberDel = massUVS.Count - positionUvs.PositionInMass;
            for (int i = positionUvs.PositionInMass; i < massUVS.Count; i++)
                ((Grid)(gridTable.Children[i])).ContextMenu.Items.Clear();

            gridTable.Children.RemoveRange(positionUvs.PositionInMass, massUVS.Count - (positionUvs.PositionInMass));
            massUVS.RemoveAt(positionUvs.PositionInMass);
            countRow = positionUvs.Row;
            countRowForButton = positionUvs.Row;
            countColumnForButton = positionUvs.Column;
            Draw(positionUvs);
        }
        public void AddObject(List<UVS> massUVS) // добавление объекта UVS
        {
            UVS uvs = new UVS(MainWindow.MbMaster, (byte)(massUVS.Count + 1), UVS.StartName);
            uvs.RegisterUVSMessageHeandler(AddMessage);
            massUVS.Add(uvs);

            massUVS[massUVS.Count - 1].SetTimeProcessMagneticStarter();
            massUVS[massUVS.Count - 1].SetTimeProcessOnPressure();
            massUVS[massUVS.Count - 1].SetTimeProcessOffPressure();
            massUVS[massUVS.Count - 1].SetTimeStopInPlace();

            Draw();
        }
        private void AddButton() // добавление кнопки "Добаввить объект"
        {
            Grid.SetRow(btnAddGrid, countRowForButton);
            Grid.SetColumn(btnAddGrid, countColumnForButton);
            gridTable.Children.Add(btnAddGrid);
        }
        private void AddBorder() // добавление выделяющей рамки
        {
            Console.WriteLine($"в главном гриде {gridTable.Children.Count} элементов до удаления");
            gridTable.Children.Remove(borderUVS);
            Console.WriteLine($"в главном гриде {gridTable.Children.Count} элементов после удаления");
            borderUVS = new Border()
            {
                Name = "BorderUvs",
                Width = gridObject.Width,
                Height = gridObject.Height,
                BorderBrush = Brushes.Black,
                BorderThickness = new Thickness(1)
            };
            Grid.SetRow(borderUVS, PositionUvs.Row);
            Grid.SetColumn(borderUVS, PositionUvs.Column);
            gridTable.Children.Add(borderUVS);
        }
        
        private void AddMessage(string s, SolidColorBrush colorText) // добавление сообщения
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
        public static void SetUvs(Position position, Grid grid) // выбор UVS
        {
            PositionUvs = position;
            gridObject = grid;
        }
        public void SetTimeUVS(Position position, UVSTimeProcess UVSTimeProcess) // установка таймеров
        {
            massUVS[position.PositionInMass].UVSTimeProcess = UVSTimeProcess;

            massUVS[position.PositionInMass].SetTimeProcessMagneticStarter();
            massUVS[position.PositionInMass].SetTimeProcessOnPressure();
            massUVS[position.PositionInMass].SetTimeProcessOffPressure();
            massUVS[position.PositionInMass].SetTimeStopInPlace();
        }
        public void SetAdressUVS(Position position, UVSAdress uvsAdress) // адрес конкретного UVS
        {
            massUVS[position.PositionInMass].UVSAdress = uvsAdress;
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            Thread trVU = new Thread(UpdateVU);
            Thread trSurveyl = new Thread(UpdateSurvey);
            trVU.Start();
            trSurveyl.Start();       
            if (MainWindow.FlagSimulation)
            {
                Thread trModel = new Thread(UpdateModelUVS);
                trModel.Start();
            }
        }
        private void AddGrid_Clik(object sender, RoutedEventArgs e)
        {
            CreateWindow addWindow = new CreateWindow();
            addWindow.Owner = this;

            if (addWindow.ShowDialog() == true)
                AddObject(massUVS);
        }
        private void Window_Loaded_1(object sender, RoutedEventArgs e)
        {
            Grid.SetRow(btnAddGrid, 0);
            Grid.SetColumn(btnAddGrid, 0);
            gridTable.ShowGridLines = true;
            gridTable.Children.Add(btnAddGrid);
            RegisterDelegate(AddBorder);
            deleteHeandler = Delete;

            btnAddGrid.Click += AddGrid_Clik;
        }
        private void Sort_Button_Click(object sender, RoutedEventArgs e) // Сортировка UVS
        {
            for (int i = 0; i < massUVS.Count; i++)
                ((Grid)(gridTable.Children[i])).ContextMenu.Items.Clear();
            gridTable.Children.Clear();

            if (!listSorted)
            {
                massUVS.Sort();
                listSorted = !listSorted;
            }
            else
                massUVS.Reverse();

            countRow = 0;
            countRowForButton = 0;
            countColumnForButton = 0;

            PositionUvs.Column = 0;
            PositionUvs.Row = 0;

            Draw(PositionUvs);
        }
        private void Delete_Button_Click(object sender, RoutedEventArgs e)
        {
            panelMessage.SelectAll();
            panelMessage.Selection.Text = "\0";
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            exitFlag = true;
        }       
    }

}

