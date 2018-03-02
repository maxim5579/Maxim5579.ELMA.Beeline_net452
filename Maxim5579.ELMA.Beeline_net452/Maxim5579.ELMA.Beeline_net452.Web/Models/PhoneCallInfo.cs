using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EleWise.ELMA.CRM.Models;

namespace Maxim5579.ELMA.Beeline_net452.Web.Models
{
    public class PhoneCallInfo
    {
        public string PhoneString { get; set; }
        public ICollection<IContractor> Contractors { get; set; }
        public ICollection<ILead> Leads { get; set; }
    }
}