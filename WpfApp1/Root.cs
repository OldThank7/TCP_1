using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1
{

    public class Rootobject
    {
        public Address address { get; set; }
    }


    public class Address
    {
        public string Temp { get; set; }
        public string Humi { get; set; }
    }

}
