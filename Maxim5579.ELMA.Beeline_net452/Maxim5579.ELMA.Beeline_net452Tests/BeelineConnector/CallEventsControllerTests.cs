using Microsoft.VisualStudio.TestTools.UnitTesting;
using Maxim5579.ELMA.Beeline_net452;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maxim5579.ELMA.Beeline_net452.Tests
{
    [TestClass()]
    public class CallEventsControllerTests
    {
        [TestMethod()]
        public void CallEventsControllerTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void FindRelationsByCallIDTest()
        {
            var tmp = new CallEventsController();
            string callID = "123";
            tmp.FindRelationsByCallID(callID);
            Assert.Fail();
        }
    }
}