using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonTypes;

namespace Padi_dstm
{
    [Serializable]
    public class PadInt : IPadInt
    {
        private int _uid;
        private int _value;
        private string url; // URL of worker server. Not sure if needed

        // placeholder constructor for testing
        public PadInt(int val)
        {
            
            _value = val;
        }

        public string URL { get { return url; } }
        public int UID { get { return _uid; } }


        public int Read()
        {
            _value = PadiDstm.Transaction.Read(this);
            return _value; // placeholder
        }

        public void Write(int val)
        {
            PadiDstm.Transaction.Write(this, val);
            return; // placeholder
        }
    }
}
