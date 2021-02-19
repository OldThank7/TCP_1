using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace WpfApp1
{
    class getAndset
    {
        private static double humi;
        private static double temp;

        private static bool Fan = false;
        private static bool Light_bulb = false;

        private static double box_Max_Humi;
        private static double box_Mix_Temp;

        public static double Humi { get => humi; set => humi = value; }
        public static double Temp { get => temp; set => temp = value; }
        public static bool Light_bulb1 { get => Light_bulb; set => Light_bulb = value; }
        public static bool Fan1 { get => Fan; set => Fan = value; }
        public static double Box_Max_Humi { get => box_Max_Humi; set => box_Max_Humi = value; }
        public static double Box_Mix_Temp { get => box_Mix_Temp; set => box_Mix_Temp = value; }
    }
}
