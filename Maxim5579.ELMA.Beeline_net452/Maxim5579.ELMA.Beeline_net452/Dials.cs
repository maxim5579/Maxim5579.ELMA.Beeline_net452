using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Maxim5579.ELMA.Beeline_net452.BeelineAPI;

namespace Maxim5579.ELMA.Beeline_net452
{
    public class Dials:List<Dial>
    {
        public Dial findBySubscriptionID(string SID)
        {
            return this.Find(x => x.SubscriptionID == SID);
        }

        public int FindIndexBySubscriptionID(string SID)
        {
            return this.IndexOf(findBySubscriptionID(SID));
        }
    }
}