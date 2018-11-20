using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GarageTool
{

    class RemDevice :Device
    {
        private string validation;
        public string Validation { get { return validation; } }

        public RemDevice(string val)
        {
            validation = val;
        }

    }
}
