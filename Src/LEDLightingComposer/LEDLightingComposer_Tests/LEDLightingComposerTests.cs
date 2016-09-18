using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LEDLightingComposer;
using System.Threading;
using System.Windows.Forms;

namespace LEDLightingComposer_Tests
{
    [TestClass]
    public class LEDLightingComposerTests
    {
        //Set global variables
        private static LEDLightingComposerCS llc;

        [ClassInitialize()]
        public static void ClassInit(TestContext context)
        {
            //Create new LEDLightingComposer object
            llc = new LEDLightingComposerCS();
        }

        [TestMethod]
        [Timeout(TestTimeout.Infinite)]
        public void startTickerThreadIsCreated_Test()
        {
            //Declare variables
            Thread t = null;

            //Arrange ----------------------------
            //This method's arrange is handled within ClassInit method

            //Act ----------------------------

            //Change WM Player state to playing
            llc.MManager.IsPlaying = true;
            t = llc.startTicker();

            //Sleep for 2 seconds, allowing enough time for thread to dispose if it will be
            Thread.Sleep(2000);

            //Assert ----------------------------
            Assert.AreEqual(true, t.IsAlive);

            //Cleanup used local variables
            t.Abort();
            t = null;
        }

        [ClassCleanup()]
        public static void ClassClean()
        {
            llc = null;
        }
    }
}
