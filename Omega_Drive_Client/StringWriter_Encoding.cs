using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omega_Drive_Client
{
    internal class StringWriter_Encoding:StringWriter
    {
        public override Encoding Encoding => Encoding.UTF8;
    }
}
