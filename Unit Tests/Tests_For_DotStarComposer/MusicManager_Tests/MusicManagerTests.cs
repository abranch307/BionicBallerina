using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LEDLightingComposer;
using System.Threading;
using System.Windows.Forms;

namespace MusicManager_Tests
{
    [TestClass]
    public class MusicManagerTests
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
            Tests that updateLabel function updates timer textbox to match
            Windows Media Player current position correctly
        */
        [TestMethod]
        public void updateLabel_Test()
        {
            //Declare variables
            int curPositionTime = 60, labelTime = -1;

            //Arrange ----------------------------

            //The LEDLightingComposerCS class is handled in ClassInit method

            //Act ----------------------------

            //Change Window Media Player's current position time to 60 seconds
            llc.MManager.player2.Ctlcontrols.currentPosition = curPositionTime;

            //Update label time
            llc.MManager.updateLabel();

            try
            {
                //Set label time from text
                labelTime = int.Parse(llc.MManager.timer.Text.ToString().Trim());
            }catch(Exception ex)
            {

            }

            //Assert ----------------------------
            Assert.AreEqual(labelTime, llc.MManager.player2.Ctlcontrols.currentPosition);
        }



        [ClassCleanup()]
        public static void ClassClean()
        {
            llc.Dispose();
        }
    }
}
