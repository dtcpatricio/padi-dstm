using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datastore
{
    class PadIntStorage
    {
        // padInts maps uids with the values of the padInt objects
        IDictionary<int, int> padInts;

        public PadIntStorage()
        {
            padInts = new Dictionary<int, int>();
        }

        public void addPadInt(int uid, int value) 
        {
            padInts.Add(uid, value);
        }

        public int getValue(int uid) 
        {
            return padInts[uid];
        }
    }
}
