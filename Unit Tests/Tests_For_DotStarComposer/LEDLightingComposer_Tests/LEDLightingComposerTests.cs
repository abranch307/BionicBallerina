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

        /*
            Tests that startTicker function spawns thread successfully
        */
        [TestMethod]
        [Timeout(TestTimeout.Infinite)]
        public void startTickerThreadIsCreated_Test()
        {
            //Declare variables
            Thread t = null;

            //Arrange ----------------------------

            //The LEDLightingComposerCS class is handled in ClassInit method

            //Change WM Player state to playing
            llc.MManager.IsPlaying = true;

            //Act ----------------------------

            //Start ticker (which spawns a new thread to handle updates to label timer textbox and screen drawing)
            t = llc.startTicker();

            //Sleep for 2 seconds, allowing enough time for thread to dispose itself if it fails to spawn
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
            llc.Dispose();
        }
    }
}
