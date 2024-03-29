﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Datastore
{
    class Participant : MarshalByRefObject, IParticipant
    {
        public void canCommit(int txID, string coordURL)
        {
            Datastore.canCommit(txID, coordURL);
        }

        public void doCommit(int txID, string coordURL)
        {
            Datastore.doCommit(txID, coordURL);
        }

        public void doAbort(int txID, string coordURL)
        {
            Datastore.doAbort(txID, coordURL);
        }

        public override object InitializeLifetimeService()
        {
            return null;
        }
    }
}
