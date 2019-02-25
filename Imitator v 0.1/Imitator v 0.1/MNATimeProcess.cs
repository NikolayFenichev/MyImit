using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*Таймера MNA*/

namespace Imitator_v_0._1
{
    public class MNATimeProcess
    {
        public ushort TimeProcessVVOn { get; set; }
        public ushort TimeProcessVVOff { get; set; }
        public ushort TimeProcessCurrentOn { get; set; }
        public ushort TimeProcessCurrentOff { get; set; }

        public MNATimeProcess()
        {
            TimeProcessVVOn = 1500;
            TimeProcessVVOff = 1500;
            TimeProcessCurrentOn = 3000;
            TimeProcessCurrentOff = 3000;
        }
    }
}
