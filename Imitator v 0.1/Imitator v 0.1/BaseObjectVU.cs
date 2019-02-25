using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows;
using Modbus.Device;
using System.Collections;
using System.Windows.Shapes;
using System.Windows.Media;

/*Базовый класс объекта верхнего уровня*/

namespace Imitator_v_0._1
{
    public abstract class BaseObjectVU : IComparable<BaseObjectVU>
    {
        protected ModbusMaster mbMaster; //подключение
        protected Grid gridObject; //Grid объекта
        protected Label labelName;
        protected byte SlaveAdress { get; set; }
        protected MessageHeandler MessageHeandler;

        protected byte[] bStateMass = new byte[1];
        protected List<Shape> elements = new List<Shape>(); //список элементов объекта

        protected ushort State { get; set; } // состояние
        public ushort Cmd { get; set; } // команды
        public string Name { get; set; }// имя объекта
        public static string StartName { get; set; } // стартовое имя для имени объекта
        public Position PositionObject { get; set; } // позиция объекта
        protected BitmapImage BitmapClose { get; set; } = new BitmapImage(new Uri("Images/Close.PNG", UriKind.Relative));
        protected BitmapImage BitmapOpen { get; set; } = new BitmapImage(new Uri("Images/Open.PNG", UriKind.Relative));

        public abstract void TurnOn(); // включить
        public abstract void TurnOff(); // отключить
        public abstract void Survey(); // опрос
        public abstract UIElement Create(); // создание объекта
        public abstract BitArray ModBusSurveyMass();
        public abstract void RegisterUVSMessageHeandler(MessageHeandler del);
        public abstract void UnregisterUVSMessageHeandler(MessageHeandler del);

        public virtual void SetColor(string name, Brush brushes)
        {
            foreach (object o in elements)
            {
                if (o is Shape && ((Shape)o).Name == name)
                    ((Shape)o).Fill = brushes;
            }
        }
        public virtual void SetName()
        {
            Name = StartName;
        }
        public static void SetName(string name)
        {
            StartName = name;
        }

        public int CompareTo(BaseObjectVU b)
        {
            return this.Name.CompareTo(b.Name);
        }
    }
}
