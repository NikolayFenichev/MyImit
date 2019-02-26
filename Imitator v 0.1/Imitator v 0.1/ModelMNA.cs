using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*Логическая модель MNA*/

namespace Imitator_v_0._1
{
    public class ModelMNA : BaseObjectNU
    {
        bool flagForPuskVV1On;
        bool flagForPuskVV2On;
        bool flagForPuskVV1Off;
        bool flagForPuskVV2Off;
        bool flagForPuskCurrent;
        bool flagForStopCurrent;

        bool puskTotal;
        bool puskByImit;
        bool puskByMPSA;

        bool stopTotal;
        bool stopByImit;
        bool stopByBRU;
        bool stopInPlace;
        bool stopByMPSA;

        bool prevVV1OnTimer;
        bool prevVV2OnTimer;
        bool prevVV1OffTimer;
        bool prevVV2OffTimer;
        bool prevCurrentOnTimer;

        bool vV1OnTimer;
        bool vV2OnTimer;
        bool vV1OffTimer;
        bool vV2OffTimer;
        bool currentOnTimer;
        bool currentOffTimer;
        bool currentTimer;

        bool prevVV1On;
        bool prevVV2On;
        bool prevVV1Off;
        bool prevVV2Off;
        bool prevCurrent;

        bool processOn;
        bool processOff;

        int tmp;

        bool vV1On;
        bool vV2On;
        bool vV1Off;
        bool vV2Off;
        bool current;

        bool vV1OnVu;
        bool vV2OnVu;
        bool vV1OffVu;
        bool vV2OffVu;
        bool currentVu;

        bool prevVV1OnVu;
        bool prevVV2OnVu;
        bool prevVV1OffVu;
        bool prevVV2OffVu;
        bool prevCurrentVu;

        bool flagLockVV1On;
        bool flagLockVV2On;
        bool flagLockVV1Off;
        bool flagLockVV2Off;
        bool flagLockCurrent;

        bool resetVV1OnTimer;
        bool resetVV2OnTimer;
        bool resetVV1OffTimer;
        bool resetVV2OffTimer;
        bool resetCurrentOnTimer;

        bool initialization = true;

        ushort state;

        public int timeProcessVVOn;
        public int timeProcessVVOff;
        public int timeProcessCurrentOn;
        public int timeProcessCurrentOff;

        long internalTimeInTimerForPuskVV1On;
        long internalTimeInTimerForPuskVV2On;
        long internalTimeInTimerForPuskVV1Off;
        long internalTimeInTimerForPuskVV2Off;
        long internalTimeInTimerForPuskCurrent;
        long internalTimeInTimerForStopCurrent;

        public override ushort State { get { return state; } }
        public override ushort Cmd { get; set; }

        public override void Update() // обновление модели
        {
            switch (Cmd) // получаем команды
            {
                case 1:
                    vV1OnVu = true;
                    Cmd = 0;
                    break;
                case 2:
                    vV2OnVu = true;
                    Cmd = 0;
                    break;
                case 4:
                    vV1OffVu = true;
                    Cmd = 0;
                    break;
                case 8:
                    vV2OffVu = true;
                    Cmd = 0;
                    break;
                case 16:
                    current = true;
                    Cmd = 0;
                    break;
                case 32:
                    if (flagLockVV1On)
                        flagLockVV1On = false;
                    else
                        flagLockVV1On = true;
                    Cmd = 0;
                    break;
                case 64:
                    if (flagLockVV2On)
                        flagLockVV2On = false;
                    else
                        flagLockVV2On = true;
                    Cmd = 0;
                    break;
                case 128:
                    if (flagLockVV1Off)
                        flagLockVV1Off = false;
                    else
                        flagLockVV1Off = true;
                    Cmd = 0;
                    break;
                case 256:
                    if (flagLockVV2Off)
                        flagLockVV2Off = false;
                    else
                        flagLockVV2Off = true;
                    Cmd = 0;
                    break;
                case 512:
                    if (flagLockCurrent)
                        flagLockCurrent = false;
                    else
                        flagLockCurrent = true;
                    Cmd = 0;
                    break;
                case 1024:
                    puskByImit = true;
                    Cmd = 0;
                    break;
                case 2048:
                    puskByMPSA = true;
                    Cmd = 0;
                    break;
                case 4096:
                    stopByImit = true;
                    Cmd = 0;
                    break;
                case 8192:
                    stopInPlace = true;
                    Cmd = 0;
                    break;
                case 16384:
                    stopByBRU = true;
                    Cmd = 0;
                    break;
                case 32768:
                    stopByMPSA = true;
                    Cmd = 0;
                    break;
            }

            puskTotal = puskByMPSA || puskByImit;
            stopTotal = stopByImit || stopByBRU || stopInPlace || stopByMPSA;

            if (puskTotal)
                processOn = true;
            else if(vV1OnTimer || vV2OnTimer)
                processOn = false;

            if (stopTotal)
                processOff = true;
            else if (vV1OffTimer || vV2OffTimer)
                processOff = false;

            prevVV1OnTimer = vV1OnTimer;
            prevVV2OnTimer = vV2OnTimer;
            prevVV1OffTimer = vV1OffTimer;
            prevVV2OffTimer = vV2OffTimer;
            prevCurrentOnTimer = currentOnTimer;

            resetVV1OnTimer = TimerPuskVV1Off(timeProcessVVOff, stopTotal, puskTotal);
            resetVV2OnTimer = TimerPuskVV2Off(timeProcessVVOff, stopTotal, puskTotal);
            resetCurrentOnTimer = TimerStopCurrent(timeProcessCurrentOff, stopTotal, puskTotal);
            vV1OnTimer = TimerPuskVV1On(timeProcessVVOn, puskTotal, resetVV1OnTimer);
            vV2OnTimer = TimerPuskVV2On(timeProcessVVOn, puskTotal, resetVV2OnTimer);
            currentOnTimer = TimerPuskCurrent(timeProcessCurrentOn, puskTotal, resetCurrentOnTimer);

            vV1OffTimer = !vV1OnTimer;
            vV2OffTimer = !vV2OnTimer;                     

            ChangeOnVU();
            OutState();

            puskByImit = false;
            puskByMPSA = false;
            stopByImit = false;
            stopByBRU = false;
            stopByMPSA = false;
            stopInPlace = false;
            initialization = false;

            prevVV1On = vV1On;
            prevVV2On = vV2On;
            prevVV1Off = vV1Off;
            prevVV2Off = vV2Off;
            prevCurrent = current;
        }

        bool TimerPuskVV1On(int typeTimer, bool start, bool reset) //таймер включения ВВ 1
        {
            if (start && !reset && !flagForPuskVV1On)
            {
                internalTimeInTimerForPuskVV1On = MNAWindow.Timer;
                flagForPuskVV1On = true;
            }

            if (reset)
            {
                flagForPuskVV1On = false;
                internalTimeInTimerForPuskVV1On = 0;
                return false;
            }
            else if ((MNAWindow.Timer >= internalTimeInTimerForPuskVV1On + typeTimer) && internalTimeInTimerForPuskVV1On != 0)
            {
                return true;
            }
            else
                return false;
        }
        bool TimerPuskVV2On(int typeTimer, bool start, bool reset) //таймер включения ВВ 2
        {
            if (start && !reset && !flagForPuskVV2On)
            {
                internalTimeInTimerForPuskVV2On = MNAWindow.Timer;
                flagForPuskVV2On = true;
            }

            if (reset)
            {
                flagForPuskVV2On = false;
                internalTimeInTimerForPuskVV2On = 0;
                return false;
            }
            else if ((MNAWindow.Timer >= internalTimeInTimerForPuskVV2On + typeTimer) && internalTimeInTimerForPuskVV2On != 0)
            {
                return true;
            }
            else
                return false;
        }
        bool TimerPuskVV1Off(int typeTimer, bool start, bool reset) //таймер отключения ВВ 1
        {
            if (start && !reset && !flagForPuskVV1Off)
            {
                internalTimeInTimerForPuskVV1Off = MNAWindow.Timer;
                flagForPuskVV1Off = true;
            }

            if (reset)
            {
                flagForPuskVV1Off = false;
                internalTimeInTimerForPuskVV1Off = 0;
                return false;
            }
            else if ((MNAWindow.Timer >= internalTimeInTimerForPuskVV1Off + typeTimer) && internalTimeInTimerForPuskVV1Off != 0)
            {
                return true;
            }
            else
                return false;
        }
        bool TimerPuskVV2Off(int typeTimer, bool start, bool reset) //таймер отключения ВВ 2
        {
            if (start && !reset && !flagForPuskVV2Off)
            {
                internalTimeInTimerForPuskVV2Off = MNAWindow.Timer;
                flagForPuskVV2Off = true;
            }

            if (reset)
            {
                flagForPuskVV2Off = false;
                internalTimeInTimerForPuskVV2Off = 0;
                return false;
            }
            else if ((MNAWindow.Timer >= internalTimeInTimerForPuskVV2Off + typeTimer) && internalTimeInTimerForPuskVV2Off != 0)
            {
                return true;
            }
            else
                return false;
        }
        bool TimerPuskCurrent(int typeTimer, bool start, bool reset) // таймер набора тока
        {
            if (start && !reset && !flagForPuskCurrent)
            {
                internalTimeInTimerForPuskCurrent = MNAWindow.Timer;
                flagForPuskCurrent = true;
            }

            if (reset)
            {
                flagForPuskCurrent = false;
                internalTimeInTimerForPuskCurrent = 0;
                return false;
            }
            else if ((MNAWindow.Timer >= internalTimeInTimerForPuskCurrent + typeTimer) && internalTimeInTimerForPuskCurrent != 0)
            {
                return true;
            }
            else
                return false;
        }
        bool TimerStopCurrent(int typeTimer, bool start, bool reset) // таймер спада тока
        {
            if (start && !reset && !flagForStopCurrent)
            {
                internalTimeInTimerForStopCurrent = MNAWindow.Timer;
                flagForStopCurrent = true;
            }

            if (reset)
            {
                flagForStopCurrent = false;
                internalTimeInTimerForStopCurrent = 0;
                return false;
            }
            else if ((MNAWindow.Timer >= internalTimeInTimerForStopCurrent + typeTimer) && internalTimeInTimerForStopCurrent != 0)
            {
                return true;
            }
            else
                return false;
        }

        public override void OutState() //запись состояний в state
        {            
            SetState(prevVV1On, vV1On, 1);
            SetState(prevVV2On, vV2On, 2);
            SetState(prevVV1Off, vV1Off, 4);
            SetState(prevVV2Off, vV2Off, 8);           
            SetState(prevCurrent, current, 16);
        }

        protected override void SetState(bool b1, bool b2, int i)
        {
            if ((b1 != b2) && b2)
                state = (ushort)(State | i);
            else if (b1 != b2)
                state = (ushort)(State ^ i);
        }

        protected override void ChangeOnVU() // Собираем изменения сигналов
        {
            if ((((vV1OnTimer != prevVV1OnTimer) || (prevVV1OnVu != vV1OnVu) || stopInPlace || stopByBRU) && !flagLockVV1On) || initialization)
            {
                if (vV1OnVu)
                    vV1On = !vV1On;
                else if (stopInPlace)
                    vV1On = false;
                else if (stopByBRU)
                    vV1On = false;
                else if (vV1OnTimer)
                {
                    vV1On = true;
                }
                else if (!vV1OnTimer)
                    vV1On = false;

                vV1OnVu = false;
            }
            
            if (((vV2OnTimer != prevVV2OnTimer) || (prevVV2OnVu != vV2OnVu) || stopInPlace || stopByBRU) && !flagLockVV2On)
            {
                if (vV2OnVu)
                    vV2On = !vV2On;
                else if (stopInPlace)
                    vV2On = false;
                else if (stopByBRU)
                    vV2On = false;
                else if (vV2OnTimer)
                    vV2On = true;
                else if (!vV2OnTimer)
                    vV2On = false;

                vV2OnVu = false;
            }

            if (((vV1OffTimer != prevVV1OffTimer) || (prevVV1OffVu != vV1OffVu) || stopInPlace || stopByBRU) && !flagLockVV1Off)
            {
                if (vV1OffVu)
                    vV1Off = !vV1Off;
                else if (stopInPlace)
                    vV1Off = true;
                else if (stopByBRU)
                    vV1Off = true;
                else if (vV1OffTimer)
                    vV1Off = true;
                else if (!vV1OffTimer)
                    vV1Off = false;

                vV1OffVu = false;
            }

            if (((vV2OffTimer != prevVV2OffTimer) || (prevVV2OffVu != vV2OffVu) || stopInPlace || stopByBRU) && !flagLockVV2Off)
            {
                if (vV2OffVu)
                    vV2Off = !vV2Off;
                else if (stopInPlace)
                    vV2Off = true;
                else if (stopByBRU)
                    vV2Off = true;
                else if (vV2OffTimer)
                    vV2Off = true;
                else if (!vV2OffTimer)
                    vV2Off = false;

                vV2OffVu = false;
            }
            
            if (((currentOnTimer != prevCurrentOnTimer) || (prevCurrentVu != currentVu)) && !flagLockCurrent)
            {
                if (currentVu)
                    current = !current;
                else if (currentOnTimer)
                    current = true;
                else if (!currentTimer)
                    current = false;

                currentVu = false;
            }
        }

        //protected void SetElement(bool vV,bool timer, bool prevTimer, bool vU, bool prevVu, bool stopInPlace, bool stopByBRU, bool flagLock)
        //{
        //    if (((timer != prevTimer) || (prevVu != vU) || stopInPlace || stopByBRU) && !flagLock)
        //    {
        //        if (vU)
        //            vV = !vV;
        //        else if (stopInPlace)
        //            vV = false;
        //        else if (stopByBRU)
        //            vV = false;
        //        else if (timer)
        //            vV = true;
        //        else if (!timer)
        //            vV = false;
        //    }
        //}
    }
}
