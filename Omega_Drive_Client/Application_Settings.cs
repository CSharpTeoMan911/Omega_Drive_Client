using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omega_Drive_Client
{
    public class Application_Settings
    {
        public string IP_ADDRESS { get; set; }
        public string PORT_NUMBER { get; set; }
        public string PROTOCOL_INDEX { get; set; }

        public bool ALLOW_SELF_SIGNED_CERTIFICATES { get; set; }
    }
}
