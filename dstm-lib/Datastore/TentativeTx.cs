using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datastore
{
    internal class TentativeTx
    {
        private int _txID;
        private string _clientURL;

        private List<ServerObject> writtenObjects;

        internal int TXID { get { return _txID; } }
        internal string CLIENTURL { get { return _clientURL; } }

        internal TentativeTx(int txID, string clientURL)
        {
            _txID = txID;
            _clientURL = clientURL;
            writtenObjects = new List<ServerObject>();
        }

        internal void AddObject(ServerObject obj)
        {
            writtenObjects.Add(obj);
        }
    }
}
