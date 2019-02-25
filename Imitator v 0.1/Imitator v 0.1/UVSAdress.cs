using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*Адрес UVS*/

namespace Imitator_v_0._1
{
    public class UVSAdress
    {
        public ushort MagneticStarterRegisterAdress { get; set; }
        public ushort PressureRegisterAdress { get; set; }
        public ushort VoltageRegisterAdress { get; set; }
        public ushort SHRegisterAdress { get; set; }

        public ushort MagneticStarterBitAdress { get; set; }
        public ushort PressureBitAdress { get; set; }
        public ushort VoltageBitAdress { get; set; }
        public ushort SHBitAdress { get; set; }

        /*Текстовое представление разбиваем на адрес памяти и бит*/

        public string MagneticStarterAdress
        {
            get
            {
                return String.Format("{0}.{1}", MagneticStarterRegisterAdress, MagneticStarterBitAdress);
            }

            set
            {
                string[] s = value.Split('.');
                MagneticStarterRegisterAdress = ushort.Parse(s[0]);
                MagneticStarterBitAdress = ushort.Parse(s[1]);
            }
        }
        public string PressureAdress
        {
            get
            {
                return String.Format("{0}.{1}", PressureRegisterAdress, PressureBitAdress);
            }

            set
            {
                string[] s = value.Split('.');
                PressureRegisterAdress = ushort.Parse(s[0]);
                PressureBitAdress = ushort.Parse(s[1]);
            }
        }
        public string VoltageAdress
        {
            get
            {
                return String.Format("{0}.{1}", VoltageRegisterAdress, VoltageBitAdress);
            }

            set
            {
                string[] s = value.Split('.');
                VoltageRegisterAdress = ushort.Parse(s[0]);
                VoltageBitAdress = ushort.Parse(s[1]);
            }
        }
        public string SHAdress
        {
            get
            {
                return String.Format("{0}.{1}", SHRegisterAdress, SHBitAdress);
            }

            set
            {
                string[] s = value.Split('.');
                SHRegisterAdress = ushort.Parse(s[0]);
                SHBitAdress = ushort.Parse(s[1]);
            }
        }
    }
}
