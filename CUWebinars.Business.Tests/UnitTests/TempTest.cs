using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CUWebinars.Business.Models;
using CUWebinars.Business.Notification;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CUWebinars.Business.Tests.UnitTests
{
    [TestClass]
    public class TempTest
    {
        [TestMethod]
        public void Bla()
        {
            TtsTemplateHelper tts = new TtsTemplateHelper();

            var oi = tts.FormatTimeWithDuration(DateTime.Now, USTimeZone.Eastern, true, 2);

            Trace.TraceInformation(oi);

        }
    }
}
