/*
	Author: Aaron Branch, Zach Jarmon, Peter Martinez
	Created: 
	Last Modified:    
	Class: .cs
	Class Description:
		This class 
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;

namespace LEDLightingComposer
{
    public static class EffectsManager
    {
        //Declare global variables
        private static DatabaseManager dbmanager;
        private static DrawingManager drawManager;
        private static List<Strip> stripsArray = new List<Strip>();
        private static Stopwatch stopWatch;
        private static long performanceElapsedTime, prevTime;
        public static bool continueFromCurrentTime;

        public static void Init(DatabaseManager DBManager, DrawingManager DrawManager)
        {
            stopWatch = new Stopwatch();
            stopWatch.Start();
            dbmanager = DBManager;
            drawManager = DrawManager;
            performanceElapsedTime = 0;
            prevTime = -1;
            continueFromCurrentTime = false;
        }

        #region Public Methods

        /*
            Function:

            Parameters:

            Returns:
        */
        public static bool updatePerformance(long elapsedTime)
        {
            //Declare variables
            bool bRet = true;

            //Add passed elapsed time to performance time
            performanceElapsedTime += elapsedTime;

            //Update all strips with elapsed time
            foreach (Strip s in stripsArray)
            {
                try
                {
                    //Only set bRet if Strip update returns false (this way, we'll know if at least one update returned false)
                    if (!s.update(performanceElapsedTime))
                    {
                        bRet = false;
                    }
                }
                catch (Exception ex)
                {
                    bRet = false;
                }
            }

            return bRet;
        }

        /*
            Function:

            Parameters:

            Returns:
        */
        public static bool replaceStrips(DataGridView dgv)
        {
            //Declare variables
            bool bRet = true;
            String pinSetup = "", listPinSetups = "", projectName = "", stripName = "";
            List<Structs.LightingSequence> temp = new List<Structs.LightingSequence>();

            //Clear stripsArray list
            stripsArray.Clear();

            foreach(DataGridViewRow row in dgv.Rows)
            {
                //Get values from row
                projectName = row.Cells["PROJECT_NAME"].Value.ToString().Trim();
                stripName = row.Cells["MCU_NAME"].Value.ToString().Trim();
                pinSetup = row.Cells["PIN_SETUP"].Value.ToString().Trim();

                //Add a new strip if 
                if (!listPinSetups.Contains(pinSetup))
                {
                    try
                    {
                        //Setup variables
                        temp = new List<Structs.LightingSequence>();

                        //Add this pin setup to list
                        listPinSetups += pinSetup + ", ";

                        //Create structs of lighting effects for this strip
                        temp = createAllLSeqsFromGrid("PINSETUP", pinSetup, dgv);

                        stripsArray.Add(new Strip(stripName, int.Parse(pinSetup), temp, drawManager));
                    }catch(Exception ex)
                    {
                        MessageBox.Show("Error in EffectsManager Class: " + ex.Message);
                    }
                }
            }

            return bRet;
        }

        /*
            Function:

            Parameters:

            Returns:
        */
        public static long getElapsedTime()
        {
            //Declare variables
            long elapsedTime = 0;
            long currentTime = stopWatch.ElapsedMilliseconds;

            //Turn on stop watch if necessary
            if (!stopWatch.IsRunning)
            {
                stopWatch.Start();
            }

            //Set prevTime to currentTime if not currently set or continueFromCurrentTime is true
            if (prevTime == -1)
            {
                prevTime = currentTime;
            }
            if (continueFromCurrentTime)
            {
                prevTime = currentTime;
                continueFromCurrentTime = false;
            }

            //Get elapsed time then set previous time
            elapsedTime = currentTime - prevTime;
            prevTime = currentTime;

            return elapsedTime;
        }

        /*
            Function:

            Parameters:

            Returns:
        */
        public static long resetPerformanceTime()
        {
            //Reset global variables
            performanceElapsedTime = 0;
            prevTime = -1;
            stopWatch.Reset();
            stopWatch.Stop();

            //Reset strips
            foreach(Strip strip in stripsArray)
            {
                strip.resetPerformance();
            }

            return performanceElapsedTime;
        }

        /*
            Function:

            Parameters:

            Returns:
        */
        public static bool findCurrentSeqFromPerformanceTime(long performanceTime)
        {
            //Declare variables
            bool bRet = true;

            //Reset global variables
            performanceElapsedTime = performanceTime;
            //prevTime = performanceTime;

            //Reset strips' global variables
            foreach (Strip s in stripsArray)
            {
                //Only set bRet if Strip update returns false (this way, we'll know if at least one update returned false)
                if (!s.findCurrentSeqFromPerformanceTime(performanceTime))
                {
                    bRet = false;
                }
            }

            return bRet;
        }

        /*
            Function:

            Parameters:

            Returns:
        */
        public static bool updateStripsBrightness()
        {
            bool bRet = true;

            foreach (Strip s in stripsArray)
            {
                try
                {
                    //Update brightness of strip
                    if (!Effects.updateBrightness(s, drawManager))
                    {
                        bRet = false;
                        
                    }
                }
                catch (Exception ex)
                {
                    bRet = false;
                }
            }

            return bRet;
        }

        /*
            Function:

            Parameters:

            Returns:
        */
        public static Structs.LightingSequence createAddLSeqStruct(List<Structs.LightingSequence> LSeqs, int LightSequence, int NumPixels, String[] PixelPositions, String[] Colors, float DelayTime, float Duration, int Bounces, int Iterations, int Brightness, int IncrBrightness, float BrightnessDelayTime)
        {
            //Declare variables
            Structs.LightingSequence temp;

            //Invoke default constraints on values for certain lighting effects
            switch (LightSequence)
            {
                case Effects.FILLER:
                    DelayTime = Duration;
                    break;
                case Effects.ALLCLEAR:
                    //Delay time should equal duration time so no unnecessary updating is done
                    DelayTime = Duration;
                    break;
                case Effects.RAINBOW:
                    //Default iterations if necessary
                    if(Iterations == 0)
                    {
                        Iterations = 1;
                    }
                    break;
                case Effects.LOADCOLOR:
                    //Delay time should equal duration time so no unnecessary updating is done
                    DelayTime = Duration;
                    break;
                case Effects.BOUNCEBACK:
                    //Default iterations if necessary
                    if (Iterations == 0)
                    {
                        Iterations = 1;
                    }

                    //Default bounces if necessary
                    if (Bounces == 0)
                    {
                        Bounces = 2;
                    }
                    break;
                case Effects.FLOWTHROUGH:
                    //Default iterations if necessary
                    if (Iterations == 0)
                    {
                        Iterations = 1;
                    }
                    break;
            }

            temp = new Structs.LightingSequence {
                lightsequence = LightSequence,
                totalPixels = (ushort)PixelPositions.Length,
                effectedPixels = PixelPositions,
                colors = Colors,
                delayTime = (UInt32)Math.Floor(DelayTime*1000),
                duration = (ushort)(Duration*1000),
                bounces = (ushort)Bounces,
                iterations = (ushort)Iterations,
                brightness = (short)Brightness,
                incrBrightness = (short)IncrBrightness,
                brightnessDelayTime = (int)(BrightnessDelayTime * 1000)};

            if (LSeqs != null)
            {
                LSeqs.Add(temp);
            }

            return temp;
        }

        /*
            Function:

            Parameters:

            Returns:
        */
        public static List<Structs.LightingSequence> createAllLSeqsFromGrid(String Type, String Value1, DataGridView dgv)
        {
            //Declare variables
            List<Structs.LightingSequence> lSeqRet = new List<Structs.LightingSequence>();
            bool proceed = false;

            //Create a list of lighting sequences for all elements or matching pin setup in row depending on passed Type parameter
            foreach(DataGridViewRow row in dgv.Rows)
            {
                if (Type.Equals("ALL"))
                {
                    proceed = true;
                }else
                {
                    if(int.Parse(row.Cells["PIN_SETUP"].Value.ToString().Trim()) == int.Parse(Value1))
                    {
                        proceed = true;
                    }else
                    {
                        proceed = false;
                    }
                }


                if (proceed)
                {
                    lSeqRet.Add(createAddLSeqStruct(null, int.Parse(row.Cells["LIGHTING_EFFECT"].Value.ToString().Trim()),
                        int.Parse(row.Cells["NUM_LEDS"].Value.ToString().Trim()),
                        row.Cells["LED_POSITION_ARRAY"].Value.ToString().Trim().Split(','),
                        row.Cells["LED_COLOR_ARRAY"].Value.ToString().Trim().Split(','),
                        float.Parse(row.Cells["DELAY_TIME"].Value.ToString().Trim()),
                        float.Parse(row.Cells["EFFECT_DURATION"].Value.ToString().Trim()),
                        int.Parse(row.Cells["BOUNCES"].Value.ToString().Trim()),
                        int.Parse(row.Cells["ITERATIONS"].Value.ToString().Trim()),
                        int.Parse(row.Cells["BRIGHTNESS"].Value.ToString().Trim()),
                        int.Parse(row.Cells["INCR_BRIGHTNESS"].Value.ToString().Trim()),
                        float.Parse(row.Cells["BRIGHTNESS_DELAYTIME"].Value.ToString().Trim())));
                }
            }

            return lSeqRet;
        }

        /*
            Function:

            Parameters:

            Returns:
        */
        public static Stopwatch StopWatch
        {
            get
            {
                return stopWatch;
            }

            set
            {
                stopWatch = value;
            }
        }

        #endregion Public Methods


        #region Getters & Setters

        public static List<Strip> StripsArray
        {
            get
            {
                return stripsArray;
            }
        }

        #endregion Getters & Setters

    }
}
