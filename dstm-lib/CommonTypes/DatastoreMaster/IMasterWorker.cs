﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonTypes
{
    public interface IMasterWorker
    {
    /*    void setAsReplica(Dictionary<int, string> availableServers);
        
        void setWorker(int id);
        */
        // Sets whos my sucessor and whos sending me updates
        void setReplica(string sucessor, string predecessor);
        void setSucessor(string sucessor);
        void setPredecessor(string predecessor);
        void substituteFailedServer();
        void fetch_data(string predecessor_url);
        void fetch_recover_data(string failed_sucessor);
        void freeze();
        void recover();
        void fail();
        void status();
    }
}
