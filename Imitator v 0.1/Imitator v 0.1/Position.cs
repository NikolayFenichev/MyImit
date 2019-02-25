
/*Позиция объекта*/

namespace Imitator_v_0._1
{
    public class Position
    {
        public int Row { get; set; }
        public int Column { get; set; }

        public int PositionInMass
        {
            get { return (4 * Row + Column); }       
        }

        public void Clear()
        {
            Row = 0;
            Column = 0;
        }
    }
}