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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Threading;
using System.Diagnostics;

/*Логическая модель UVS*/

namespace Imitator_v_0._1
{
    public partial class ModelUVS : BaseObjectNU
    {
        bool puskTotal = false;
        bool puskByButton = false;
        bool puskInPlace = false;
        bool puskByMPSA = false;

        bool stopTotal = false;
        bool stopByButton = false;
        bool stopInPlace = false;
        bool stopByMPSA = false;
        bool stopWhenPusk = false;

        bool magneticStarter = false;
        bool prevMagneticStarter = false;
        bool pressure = false;
        bool prevPressure = false;
        bool voltage = false;
        bool prevVoltage = false;
        bool SSH = false;
        bool prevSSH = false;

        bool magneticStarterOnTimer = false;
        bool prevMagneticStarterOnTimer = false;
        bool magneticStarterOnVU = false;
        bool prevMagneticStarterOnVU = false;
        bool voltageOnVU = true;
        bool SSHOnVU = true;

        bool voltageOnTimer = false;
        bool prevVoltageOnTimer = false;

        bool pressureOnTimer = false;
        bool prevPressureOnTimer = false;
        bool pressureOnVU = false;
        bool prevPressureOnVU = false;
        bool prevVoltageOnVU = false;

        bool resetPuskMagneticStarter = false;
        bool resetPuskPressure = false;
        bool resetStopByPlace = false;

        bool flagForPuskMagneticStarter = false;
        bool flagForPuskPressure = false;
        bool flagForStopMagneticStarter = false;
        bool flagForStopPressure = false;
        bool flagForStopByPlace = false;

        public bool FlagLockMagneticStarter { get; set; } = false;
        public bool FlagLockPressure { get; set; } = false;
        public bool FlagLockVoltage { get; set; } = false;
        public bool FlagLockSH { get; set; } = false;
        public bool FlagMagneticStarterVU { get; set; } = false;
        public bool FlagPressureVU { get; set; } = false;

        bool processPusk = false;

        public int TimeProcessMagneticStarter { get; set; } = 0;
        public int TimeProcessOffMagneticStarter { get; set; } = 0;
        public int TimeProcessOnPressure { get; set; } = 0;
        public int TimeProcessOffPressure { get; set; } = 0;
        public int TimeStopInPlace { get; set; } = 0;

        long internalTimeInTimerForPuskMagneticStarter = 0;
        long internalTimeInTimerForStopMagneticStarter = 0;
        long internalTimeInTimerForStopByPlace = 0;
        long internalTimeInTimerForPuskPressure = 0;
        long internalTimeInTimerForStopPressure = 0;

        ushort state = 0;

        public override ushort State { get { return state; } }
        public override ushort Cmd { get; set; } = 0;

        public override void Update() //работа логической модели
        {
            switch (Cmd) // получаем адреса
            {
                case 1:

                case 2:
                    voltageOnVU = true;
                    Cmd = 0;
                    break;
                case 4:
                    magneticStarterOnVU = true;
                    Cmd = 0;
                    break;
                case 8:

                case 16:
                    pressureOnVU = true;
                    Cmd = 0;
                    break;
                case 32:

                case 64:
                    SSHOnVU = true;
                    Cmd = 0;
                    break;
                case 128:

                case 256:
                    puskInPlace = true;
                    processPusk = true;
                    Cmd = 0;
                    break;
                case 512:
                    stopInPlace = true;
                    Cmd = 0;
                    break;
                case 1024:
                    puskByMPSA = true;
                    processPusk = true;
                    Cmd = 0;
                    break;
                case 2048:
                    stopByMPSA = true;
                    Cmd = 0;
                    break;
                case 4096:
                    FlagLockMagneticStarter = !FlagLockMagneticStarter;
                    Cmd = 0;
                    break;
                case 8192:
                    FlagLockPressure = !FlagLockPressure;
                    Cmd = 0;
                    break;
                case 16384:
                    FlagLockVoltage = !FlagLockVoltage;
                    Cmd = 0;
                    break;
                case 32768:
                    FlagLockMagneticStarter = !FlagLockMagneticStarter;
                    Cmd = 0;
                    break;

            }

            puskTotal = puskByButton || puskInPlace || puskByMPSA;
            stopTotal = stopByButton || stopInPlace || stopByMPSA;

            if ((stopTotal || stopWhenPusk) && processPusk)
            {
                stopWhenPusk = true;
                if (magneticStarterOnTimer)
                {
                    resetPuskMagneticStarter = TimerStopMagneticStarter(TimeProcessOffMagneticStarter, stopTotal, puskTotal);
                    resetPuskPressure = true;
                    stopWhenPusk = false;
                    processPusk = false;
                }
                else
                {
                    resetPuskMagneticStarter = true;
                    resetPuskPressure = true;
                    stopWhenPusk = false;
                    processPusk = false;
                }
            }
            else
            {
                resetPuskMagneticStarter = TimerStopMagneticStarter(TimeProcessOffMagneticStarter, stopTotal, puskTotal);
                resetPuskPressure = TimerStopPressure(TimeProcessOffPressure, stopTotal, puskTotal);
                
            }

            prevMagneticStarterOnTimer = magneticStarterOnTimer;
            magneticStarterOnTimer = TimerPuskMagneticStarter(TimeProcessMagneticStarter, puskTotal, resetPuskMagneticStarter);

            prevPressureOnTimer = pressureOnTimer;
            pressureOnTimer = TimerPuskPressure(TimeProcessOnPressure, puskTotal, resetPuskPressure);

            prevVoltageOnTimer = voltageOnTimer;
            voltageOnTimer = !TimerStopByPlace(TimeStopInPlace, stopInPlace);

            ChangeOnVU();
            OutState();

            puskInPlace = false;
            stopInPlace = false;

            prevMagneticStarter = magneticStarter;
            prevMagneticStarterOnVU = magneticStarterOnVU;
            prevPressure = pressure;
            prevPressureOnVU = pressureOnVU;
            prevVoltage = voltage;
            prevVoltageOnVU = voltageOnVU;
            prevSSH = SSH;


        }

        bool TimerPuskMagneticStarter(int typeTimer, bool start, bool reset)
        {
            if (start && !reset && !flagForPuskMagneticStarter)
            {
                internalTimeInTimerForPuskMagneticStarter = UVSWindow.Timer;
                flagForPuskMagneticStarter = true;
            }

            if (reset)
            {
                flagForPuskMagneticStarter = false;
                internalTimeInTimerForPuskMagneticStarter = 0;
                return false;
            }
            else if ((UVSWindow.Timer >= internalTimeInTimerForPuskMagneticStarter + typeTimer) && internalTimeInTimerForPuskMagneticStarter != 0)
            {                
                return true;
            }
            else
                return false;
        }
        bool TimerPuskPressure(int typeTimer, bool start, bool reset)
        {
            if (start && !reset && !flagForPuskPressure)
            {
                internalTimeInTimerForPuskPressure = UVSWindow.Timer;
                flagForPuskPressure = true;
            }

            if (reset)
            {
                flagForPuskPressure = false;
                internalTimeInTimerForPuskPressure = 0;
                return false;
            }
            else if ((UVSWindow.Timer >= internalTimeInTimerForPuskPressure + typeTimer) && internalTimeInTimerForPuskPressure != 0)
            {
                return true;
            }
            else
                return false;
        }
        bool TimerStopMagneticStarter(int typeTimer, bool start, bool reset)
        {
            if (start && !reset && !flagForStopMagneticStarter)
            {
                internalTimeInTimerForStopMagneticStarter = UVSWindow.Timer;
                flagForStopMagneticStarter = true;
            }

            if (reset)
            {
                flagForStopMagneticStarter = false;
                internalTimeInTimerForStopMagneticStarter = 0;
                return false;
            }
            else if ((UVSWindow.Timer >= internalTimeInTimerForStopMagneticStarter + typeTimer) && internalTimeInTimerForStopMagneticStarter != 0)
            {              
                return true;
            }
            else
                return false;
        }
        bool TimerStopPressure(int typeTimer, bool start, bool reset)
        {
            if (start && !reset && !flagForStopPressure)
            {
                internalTimeInTimerForStopPressure = UVSWindow.Timer;
                flagForStopPressure = true;
            }

            if (reset)
            {
                flagForStopPressure = false;
                internalTimeInTimerForStopPressure = 0;
                return false;
            }
            else if ((UVSWindow.Timer >= internalTimeInTimerForStopPressure + typeTimer) && internalTimeInTimerForStopPressure != 0)
            {                
                return true;
            }
            else
                return false;
        }
        bool TimerStopByPlace(int typeTimer, bool start)
        {
            if (start)
            {
                internalTimeInTimerForStopByPlace = UVSWindow.Timer;
                flagForStopByPlace = true;
            }

            if (resetStopByPlace)
            {
                flagForStopByPlace = false;
                internalTimeInTimerForStopByPlace = 0;
                return false;
            }
            else if ((UVSWindow.Timer >= internalTimeInTimerForStopByPlace + typeTimer) && internalTimeInTimerForStopByPlace != 0)
            {
                flagForStopByPlace = false;
                return false;
            }
            else if (flagForStopByPlace)
                return true;
            else
                return false;
        }

        protected override void ChangeOnVU() // собираем изменения сигналов
        {
            if (((magneticStarterOnTimer != prevMagneticStarterOnTimer) || (prevMagneticStarterOnVU != magneticStarterOnVU) ||stopInPlace ) && !FlagLockMagneticStarter)
            {
                if (magneticStarterOnVU)
                    magneticStarter = !magneticStarter;
                else if (stopInPlace)
                    magneticStarter = false;
                else if (magneticStarterOnTimer)
                    magneticStarter = true;
                else if (!magneticStarterOnTimer)
                    magneticStarter = false;

                magneticStarterOnVU = false;

            }

            if (((pressureOnTimer != prevPressureOnTimer) || (prevPressureOnVU != pressureOnVU)) && !FlagLockPressure)
            {
                if (pressureOnVU)
                    pressure = !pressure;
                else if (pressureOnTimer)
                    pressure = true;
                else if (!pressureOnTimer)
                    pressure = false;

                if (pressure)
                    processPusk = false;

                pressureOnVU = false;
            }

            if (SSHOnVU)
            {
                SSH = !SSH;

            }

            if (((voltageOnTimer != prevVoltageOnTimer) || (prevVoltageOnVU != voltageOnVU) || (prevSSH != SSH)) && !FlagLockVoltage)
            {
                if (prevSSH != SSH)
                    voltage = SSH;
                else
                    voltage = !voltage;
            }
            voltageOnVU = false;
            SSHOnVU = false;

        } 
        public override void OutState() // запись состояний в state
        {            
            SetState(prevMagneticStarter, magneticStarter, 2);
            SetState(prevPressure, pressure, 4);
            SetState(prevVoltage, voltage, 1);
            SetState(prevSSH, SSH, 8);
        }

        protected override void SetState(bool b1, bool b2, int i)
        {
            if ((b1 != b2) && b2)
                state = (ushort)(State | i);
            else if (b1 != b2)
                state = (ushort)(State ^ i);
        }
    }
}