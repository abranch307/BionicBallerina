using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;

namespace LEDLightingComposer
{
    public static class Effects
    {
        //Colors
        public const byte CLEAR = 0;
        public const byte WHITE = 1;
        public const byte RED = 2;
        public const byte GREEN = 3;
        public const byte BLUE = 4;
        public const byte YELLOW = 5;
        public const byte CYAN = 6;
        public const byte MAGENTA = 7;
        public const byte ORANGE = 8;

        //Lighting Effect Types
        public const short FILLER = -1;
        public const byte ALLCLEAR = 0;
        public const byte RAINBOW = 1;
        public const byte LOADCOLOR = 2;
        public const byte BOUNCEBACK = 3;
        public const byte FLOWTHROUGH = 4;

        #region Public Methods

        /*
            Function:

            Parameters:

            Returns:
        */
        public static bool allClear(Strip strip, DrawingManager DrawManager)
        {
            //Declare variables
            DrawableObject dbo = null;
            bool bRet = false;
            int i = 0, currentSequence = strip.CurrentSequence;
            Structs.LightingSequence seq;

            try
            {
                seq = strip.LSeqs[currentSequence];
            }catch(Exception ex)
            {
                try
                {
                    //CurrentSequence possibly past all sequences so try last sequence
                    seq = strip.LSeqs[(currentSequence-1)];
                }
                catch (Exception ex2)
                {
                    return bRet;
                }
            }

            //Find drawable object that matches this strip's pin setup for manipulating LEDs onscreen
            dbo = DrawManager.getDrawableObject("PINSETUP", strip.PinSetup.ToString());

            if(dbo == null)
            {
                return bRet;
            }

            //Set all of Drawable Object's LEDS to clear
            for (i = 0; i < seq.totalPixels; i++)
            {
                dbo.Leds[i].LEDColor = Color.Transparent;
            }

            return bRet;
        }

        /*
            Function:

            Parameters:

            Returns:
        */
        public static bool loadColor(Strip strip, DrawingManager DrawManager)
        {
            //Declare variables
            bool bRet = false;
            DrawableObject dbo = null;
            int i = 0, currentSequence = strip.CurrentSequence;
            Structs.LightingSequence seq;

            try
            {
                seq = strip.LSeqs[currentSequence];
            }
            catch (Exception ex)
            {
                return bRet;
            }

            //Exit if invalid pointers passed
            if (strip == null)
            {
                return bRet;
            }

            //Find drawable object that matches this strip's pin setup for manipulating LEDs onscreen
            dbo = DrawManager.getDrawableObject("PINSETUP", strip.PinSetup.ToString());

            if (dbo == null)
            {
                return bRet;
            }

            //Clear all pixels first
            allClear(strip, DrawManager);

            //Set all of Drawable Object's LEDS to clear
            for (i = 0; i < seq.totalPixels; i++)
            {
                try
                {
                    if ((i + strip.ShiftPixelBy) >= seq.totalPixels)
                    {
                        //Attempt to wrap shifted color to beginning of strip of flowthrough effect
                        if (seq.lightsequence == Effects.FLOWTHROUGH)
                        {
                            dbo.Leds[((i + strip.ShiftPixelBy) % seq.totalPixels)].LEDColor = getColorFromCode(int.Parse(seq.colors[i].Split('-')[0].Trim()), seq.brightness);
                        }else
                        {
                            //Do nothing, leave pixels as is
                        }
                    }
                    else if((i + strip.ShiftPixelBy) < 0)
                    {
                        //Do nothing as we don't want to wrap negative numbers back to end, as this would be from the bounce back effect (all effects move forward)

                    }else
                    {
                        dbo.Leds[(i + strip.ShiftPixelBy)].LEDColor = getColorFromCode(int.Parse(seq.colors[i].Split('-')[0].Trim()), seq.brightness);
                    }
                }catch(Exception ex)
                {
                    
                }
            }

            return bRet;
        }

        /*
            Function:

            Parameters:

            Returns:
        */
        public static bool bounceBack(Strip strip, DrawingManager DrawManager, int initHead, int initTail)
        {
            //Declare variables
            bool bRet = false;
            DrawableObject dbo = null;
            int i = 0, currentSequence = strip.CurrentSequence;
            Structs.LightingSequence seq;

            try
            {
                seq = strip.LSeqs[currentSequence];
            }
            catch (Exception ex)
            {
                return bRet;
            }

            //Exit if invalid pointers passed
            if (strip == null)
            {
                return bRet;
            }

            try
            {
                //Clear all pixels first
                //Effects.allClear(strip, DrawManager);

                //Initilialize if first time in
                if (strip.Init)
                {
                    strip.Init = false;
                    strip.Bounces = seq.bounces;
                    strip.ShiftPixelBy = 0;
                    strip.Head = initHead;
                    strip.Tail = initTail;

                    //Simply set colors, setup values, and exit
                    loadColor(strip, DrawManager);

                    bRet = true;

                    return bRet;
                }

                if (strip.Bounces > 0)
                {
                    if (strip.Forward)
                    {
                        //Add 1 to shiftPixelsBy
                        strip.ShiftPixelBy++;
                        strip.Head++;
                        strip.Tail++;

                        if (strip.Head >= seq.totalPixels)
                        {
                            strip.Forward = false;
                            strip.Bounces++;

                            strip.ShiftPixelBy--;
                            strip.Head--;
                            strip.Tail--;
                        }

                        //Load color with a shift and clear previous tail pixel
                        loadColor(strip, DrawManager);
                    }
                    else
                    {
                        //Subtract 1 from shiftPixelsBy
                        strip.ShiftPixelBy--;
                        strip.Head--;
                        strip.Tail--;

                        if (strip.Tail < 0)
                        {
                            strip.Forward = true;
                            strip.Bounces++;

                            strip.ShiftPixelBy++;
                            strip.Head++;
                            strip.Tail++;
                        }

                        //Load color with a shift and clear previous head pixel
                        loadColor(strip, DrawManager);
                    }
                }
                //Set return value to true
                bRet = true;
            }catch(Exception ex)
            {
                
            }

            return bRet;
        }

        /*
            Function:

            Parameters:

            Returns:
        */
        public static bool flowThrough(Strip strip, DrawingManager DrawManager)
        {
            //Declare variables
            bool bRet = false;
            DrawableObject dbo = null;
            int i = 0, currentSequence = strip.CurrentSequence;
            Structs.LightingSequence seq;

            try
            {
                seq = strip.LSeqs[currentSequence];
            }
            catch (Exception ex)
            {
                return bRet;
            }

            //Exit if invalid pointers passed
            if (strip == null)
            {
                return bRet;
            }

            //Declare variables
            int elem = 0;

            //Add 1 to i if i is -1 and reset ps
            if (strip.I < 0)
            {
                //Add 1 to i making i = 0
                strip.I++;

                //Reset shiftpixelsby
                strip.ShiftPixelBy = 0;

                if (strip.IsRainbow)
                {
                    //Reset p values
                    strip.P0 = 0;
                    strip.P1 = 1;
                    strip.P2 = 2;
                    strip.P3 = 3;
                    strip.P4 = 4;
                    strip.P5 = 5;
                }
                else
                {
                    //Set virual pixel elements to default element indexes
                    for (elem = 0; elem < seq.totalPixels; elem++)
                    {
                        //Add 1 to pixel's shift value
                        strip.VirtualPixelArray[elem] = elem - 1;
                    }
                }
            }

            if (strip.IsRainbow)
            {
                //Verify if p0 is greater than numPixels, and if so add 1 to shiftPixelsBy
                if (strip.P0 >= seq.totalPixels)
                {
                    //Add 1 to shiftPixelsBy and reset j
                    strip.I++;

                    //Reset p values
                    strip.P0 = 0; strip.P1 = 1; strip.P2 = 2; strip.P3 = 3; strip.P4 = 4; strip.P5 = 5;
                }

                if (strip.I < seq.iterations)
                {
                    //Find drawable object that matches this strip's pin setup for manipulating LEDs onscreen
                    dbo = DrawManager.getDrawableObject("PINSETUP", strip.PinSetup.ToString());

                    if (dbo == null)
                    {
                        return bRet;
                    }

                    //Set all of Drawable Object's LEDS to clear
                    //allClear(strip, DrawManager);

                    //Set specific pixels to rainbow colors
                    try { if (strip.P0 >= seq.totalPixels) { strip.P0++; } else { dbo.Leds[strip.P0++].LEDColor = Effects.getColorFromCode(CLEAR, seq.brightness); } } catch (Exception ex) { }
                    try { if (strip.P1 >= seq.totalPixels) { strip.P1++; } else { dbo.Leds[strip.P1++].LEDColor = Effects.getColorFromCode(RED, seq.brightness); } } catch (Exception ex) { }
                    try { if (strip.P2 >= seq.totalPixels) { strip.P2++; } else { dbo.Leds[strip.P2++].LEDColor = Effects.getColorFromCode(ORANGE, seq.brightness); } } catch (Exception ex) { }
                    try { if (strip.P3 >= seq.totalPixels) { strip.P3++; } else { dbo.Leds[strip.P3++].LEDColor = Effects.getColorFromCode(YELLOW, seq.brightness); } } catch (Exception ex) { }
                    try { if (strip.P4 >= seq.totalPixels) { strip.P4++; } else { dbo.Leds[strip.P4++].LEDColor = Effects.getColorFromCode(GREEN, seq.brightness); } } catch (Exception ex) { }
                    try { if (strip.P5 >= seq.totalPixels) { strip.P5++; } else { dbo.Leds[strip.P5++].LEDColor = Effects.getColorFromCode(BLUE, seq.brightness); } } catch (Exception ex) { }

                    if (strip.P0 >= seq.totalPixels)
                    {
                        strip.P0 = 0;
                    }
                    if (strip.P1 >= seq.totalPixels)
                    {
                        strip.P1 = 0;
                    }
                    if (strip.P2 >= seq.totalPixels)
                    {
                        strip.P2 = 0;
                    }
                    if (strip.P3 >= seq.totalPixels)
                    {
                        strip.P3 = 0;
                    }
                    if (strip.P4 >= seq.totalPixels)
                    {
                        strip.P4 = 0;
                    }
                    if (strip.P5 >= seq.totalPixels)
                    {
                        strip.P5 = 0;
                    }
                }
            }
            else
            {
                //Verify if first element is greater than total pixels, and if so add 1 to shiftPixelsBy
                if (strip.VirtualPixelArray[0] >= seq.totalPixels)
                {
                    //Add 1 to shiftPixelsBy and reset j
                    strip.I++;
                }

                if (strip.I <= seq.iterations)
                {
                    //Find drawable object that matches this strip's pin setup for manipulating LEDs onscreen
                    dbo = DrawManager.getDrawableObject("PINSETUP", strip.PinSetup.ToString());

                    if (dbo == null)
                    {
                        return bRet;
                    }

                    for (elem = 0; elem < seq.totalPixels; elem++)
                    {
                        //Add 1 to pixel's shift value
                        strip.VirtualPixelArray[elem] = strip.VirtualPixelArray[elem] + 1;

                        //Verify pixel shift is not over end of led strip
                        if (strip.VirtualPixelArray[elem] >= seq.totalPixels)
                        {
                            //Change virtual index to 0
                            strip.VirtualPixelArray[elem] = 0;
                        }

                        //Set pixel color
                        try { dbo.Leds[strip.VirtualPixelArray[elem]].LEDColor = Effects.getColorFromCode(int.Parse(seq.colors[elem].Split('-')[0].Trim()), seq.brightness); } catch (Exception ex) { }
                    }
                }
                else
                {
                    allClear(strip, DrawManager);
                }
            }

            return bRet;
        }

        /*
            Function:

            Parameters:

            Returns:
        */
        public static bool updateBrightness(Strip strip, DrawingManager DrawManager)
        {
            //Declare variables
            bool bRet = false;
            DrawableObject dbo = null;
            int i = 0, currentSequence = strip.CurrentSequence;
            Structs.LightingSequence seq;

            try
            {
                seq = strip.LSeqs[currentSequence];
            }
            catch (Exception ex)
            {
                return bRet;
            }

            //Exit if invalid pointers passed
            if (strip == null)
            {
                return bRet;
            }

            //Find drawable object that matches this strip's pin setup for manipulating LEDs onscreen
            dbo = DrawManager.getDrawableObject("PINSETUP", strip.PinSetup.ToString());

            if (dbo == null)
            {
                return bRet;
            }

            //Update brightness if necessary
            if (strip.IncrementBrightness)
            {
                //Add incr to brightness
                seq.brightness += seq.incrBrightness;

                if (seq.brightness > 255)
                {
                    seq.brightness = 255;
                    seq.incrBrightness *= -1;
                }
                else if (seq.brightness < 0)
                {
                    seq.brightness = 0;
                    seq.incrBrightness *= -1;
                }

                //Set brightness for all of Drawable Object's LEDS to specified brightness
                for (i = 0; i < seq.totalPixels; i++)
                {
                    try
                    {
                        if ((i + strip.ShiftPixelBy) < seq.totalPixels && (i + strip.ShiftPixelBy) >= 0)
                        {
                            dbo.Leds[(i + strip.ShiftPixelBy)].updateBrightness(seq.brightness);
                        }
                    }
                    catch (Exception ex)
                    {

                    }
                }
            }
            else
            {
                //Set brightness for all of Drawable Object's LEDS to specified brightness
                for (i = 0; i < seq.totalPixels; i++)
                {
                    try
                    {
                        if ((i + strip.ShiftPixelBy) < seq.totalPixels && (i + strip.ShiftPixelBy) >= 0)
                        {
                            dbo.Leds[(i + strip.ShiftPixelBy)].updateBrightness(seq.brightness);
                        }
                    }
                    catch (Exception ex)
                    {

                    }
                }
            }

            //Change original struct to match updated temp struct
            strip.LSeqs[currentSequence] = seq;

            bRet = true;

            return bRet;
        }

        /*
            Function:

            Parameters:

            Returns:
        */
        public static Color getColorFromCode(int ColorCode, int Brightness)
        {
            Color clr = Color.Transparent;

            switch (ColorCode)
            {
                case CLEAR:
                    clr = Color.Transparent;
                    break;
                case WHITE:
                    clr = Color.White;
                    break;
                case RED:
                    clr = Color.Red;
                    break;
                case GREEN:
                    clr = Color.Green;
                    break;
                case BLUE:
                    clr = Color.Blue;
                    break;
                case YELLOW:
                    clr = Color.Yellow;
                    break;
                case CYAN:
                    clr = Color.Cyan;
                    break;
                case MAGENTA:
                    clr = Color.Magenta;
                    break;
                case ORANGE:
                    clr = Color.Orange;
                    break;
            }

            clr = updateBrightness(clr, Brightness);

            return clr;
        }

        /*
            Function:

            Parameters:

            Returns:
        */
        public static String getEffectFromCode(int EffectCode)
        {
            String sret = "";

            switch (EffectCode)
            {
                case 0:
                    sret = "ALLCLEAR";
                    break;
                case 1:
                    sret = "RAINBOW";
                    break;
                case 2:
                    sret = "LOADCOLOR";
                    break;
                case 3:
                    sret = "BOUNCEBACK";
                    break;
                case 4:
                    sret = "FLOWTHROUGH";
                    break;
                default:
                    sret = "ALLCLEAR";
                    break;
            }

            return sret;
        }

        /*
            Function:

            Parameters:

            Returns:
        */
        public static Color updateBrightness(Color CLR, int Brightness)
        {
            Color clr = CLR;

            try
            {
                float correctionFactor = ((float)Brightness / (float)255);
                int red = ((int)(clr.R * correctionFactor) > 255) ? 255 : (int)(clr.R * correctionFactor);
                int green = ((int)(clr.G * correctionFactor) > 255) ? 255 : (int)(clr.G * correctionFactor);
                int blue = ((int)(clr.B * correctionFactor) > 255) ? 255 : (int)(clr.B * correctionFactor);
                clr = Color.FromArgb(clr.A, red, green, blue);
            }
            catch(Exception ex)
            {

            }

            return clr;
        }


        #endregion Public Methods
    }
}
