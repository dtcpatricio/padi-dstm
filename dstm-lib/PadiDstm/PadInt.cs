using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonTypes;

namespace PADI_DSTM
{

    public class PadInt : IPadInt
    {
        private int _uid;
        private int _value;
        private string _url; // URL of worker server.


        public PadInt(int uid, string url)
        {
            _uid = uid;
            _url = url;
        }

        public string URL { get { return _url; } }
        public int UID { get { return _uid; } }


        public int Read()
        {
            _value = PadiDstm.Transaction.Read(this);
            return _value;
        }

        public void Write(int val)
        {
            PadiDstm.Transaction.Write(this, val);
            return;
        }
    }
}
