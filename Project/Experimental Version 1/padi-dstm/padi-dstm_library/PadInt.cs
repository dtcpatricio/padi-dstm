using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace padi_dstm_library
{
    public class PadInt : MarshalByRefObject
    {
        private int value = 69;
        private int uid;

        // url of worker server where padint is located
        private string url = "";

        public PadInt(int uid, string url)
        {
            this.uid = uid;
            this.url = url;
        }

        public int getValue()
        {
            return this.value;
        }

        public void setValue(int value)
        {
            this.value = value;
        }

        public int getUid()
        {
            return this.uid;
        }

        public string getUrl()
        {
            return this.url;
        }

        // PadInt methods - Read and Write
        public int Read()
        {
            return getValue();
        }
    }
}
