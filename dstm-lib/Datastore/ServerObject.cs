using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datastore
{
    [Serializable]
    //WARNING: Changed the accessiblity of ServerObject to public because of fetch data
    public class ServerObject
    {
        private int _uid;
        private int _value;

        private int _readTS;
        private int _writeTS;

        // not sure if can be removed later on
        private int _committedVersion;

        internal int UID { get { return _uid; } }
        internal int VALUE { get { return _value; } }

        internal int READTS
        {
            get { return _readTS; }
            set { _readTS = value; }
        }

        /**
         * New write timestamps imply that a new ServerObject has to be created.
         * Hence, there's no set property.
         */
        internal int WRITETS { get { return _writeTS; } }

        internal int COMMITEDVER { get { return _committedVersion; } }

        /**
         * ServerObject constructor
         * @param uid the globally unique id for the object in the system
         * 
         * This constructor is only called by createPadInt calls
         **/
        internal ServerObject(int uid)
        {
            _uid = uid;
            // rest of values are, by default, 0
        }

        /**
         * ServerObject constructor
         * @param uid the globally unique id for the object in the system
         * 
         * This constructor is called by write calls
         **/
        internal ServerObject(int uid, int value, int writeTS)
        {
            _uid = uid;
            _value = value;
            _writeTS = writeTS;
            _committedVersion = writeTS; // probably not needed
        }
    }
}
