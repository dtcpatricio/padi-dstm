﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datastore
{
    public interface IWorkerReplica
    {
        //Sends the updated server objects in transaction to replica
        void update(string worker_url, List<ServerObject> writtenObjects);
    }
}
