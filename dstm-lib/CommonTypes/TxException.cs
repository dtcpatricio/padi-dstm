using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonTypes
{
    [Serializable]
    public class TxException : ApplicationException
    {
        private string _msg;

        public string GETMESSAGE { get { return _msg; } }

        public TxException(string msg)
        {
            _msg = msg;
        }

        public TxException(System.Runtime.Serialization.SerializationInfo info,
        System.Runtime.Serialization.StreamingContext context)
            : base(info, context)
        {
            _msg = info.GetString("_msg");
        }

        public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("_msg", _msg);
        }
    }
}
