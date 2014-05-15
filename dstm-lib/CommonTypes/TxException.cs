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
        public string msg;
        public IRemotePadInt remote;

        public TxException(string msg, IRemotePadInt remote)
        {
            this.msg = msg;
            this.remote = remote;
        }

        public TxException(System.Runtime.Serialization.SerializationInfo info,
        System.Runtime.Serialization.StreamingContext context)
            : base(info, context)
        {
            msg = info.GetString("msg");
            remote = (IRemotePadInt)info.GetValue("remote", typeof(IRemotePadInt));
        }



        public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("msg", msg);
            info.AddValue("remote", remote);
        }
    }
}
