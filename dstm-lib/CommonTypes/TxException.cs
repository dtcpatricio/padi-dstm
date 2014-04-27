using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonTypes
{
    public class TxException : ApplicationException
    {
        private string _msg;

        public string getMessage { get { return _msg; } }

        public TxException(string msg)
        {
            _msg = msg;
        }

    }
}
