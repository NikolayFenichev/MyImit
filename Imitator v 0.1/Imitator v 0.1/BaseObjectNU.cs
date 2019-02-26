using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*Базовый класс объекта нижнего уровня*/

namespace Imitator_v_0._1
{
    abstract public class BaseObjectNU
    {       
        abstract public void Update(); // обновление поведения объекта
        abstract public void OutState(); // формирование состояний в state
        abstract protected void ChangeOnVU(); // собираем изменения сигналов
        abstract protected void SetState(bool b1, bool b2, int i); // запись в state

        abstract public ushort State { get; } // свойство состояний
        abstract public ushort Cmd { get; set; } // свойство команд
    }
}
