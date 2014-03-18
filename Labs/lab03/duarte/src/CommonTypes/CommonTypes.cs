using System;

namespace ChatApplicationInterfaces
{

    public interface ICS
    {

        void Reg(string nick, string URL);
        void SendMsg(string nick, string msg);

    }

    public interface ICC
    {
        void StoCMsg(string user, string msg);
    }
   
}