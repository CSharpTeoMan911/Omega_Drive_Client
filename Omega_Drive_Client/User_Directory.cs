using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omega_Drive_Client
{
    internal class User_Directory
    {
        public string directory_name;
        public List<Tuple<string, byte[]>> files_list = new List<Tuple<string, byte[]>>();
        public List<User_Directory> sub_directories = new List<User_Directory>();
    }
}
