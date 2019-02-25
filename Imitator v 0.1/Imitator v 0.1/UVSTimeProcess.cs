
/*Таймера UVS*/

namespace Imitator_v_0._1
{
    public class UVSTimeProcess
    {
        public ushort TimeProcessMagneticStarter { get; set; }
        public ushort TimeProcessOffMagneticStarter { get; set; }
        public ushort TimeProcessOnPressure { get; set; }
        public ushort TimeProcessOffPressure { get; set; } 
        public ushort TimeStopInPlace { get; set; }

        public UVSTimeProcess()
        {
            TimeProcessMagneticStarter = 2000;
            TimeProcessOffMagneticStarter = 2000;
            TimeProcessOnPressure = 5000;
            TimeProcessOffPressure = 5000;
            TimeStopInPlace = 1500;
        }
    }
}