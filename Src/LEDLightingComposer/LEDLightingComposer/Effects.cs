using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

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

        public static bool rainbow(Strip strip, DrawingManager DrawManager)
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

            //Add 1 to I for next iteration and reset ps
            if (strip.I < 0)
            {
                strip.I++;

                //Reset p values
                strip.P0 = 0; strip.P1 = 1; strip.P2 = 2; strip.P3 = 3; strip.P4 = 4; strip.P5 = 5;
            }

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
                allClear(strip, DrawManager);

                //Set specific pixels to rainbow colors
                try { dbo.Leds[strip.P0++].LEDColor = Effects.getColorFromCode(CLEAR); } catch (Exception ex) { }
                try { dbo.Leds[strip.P1++].LEDColor = Effects.getColorFromCode(RED); }catch(Exception ex) { }
                try { dbo.Leds[strip.P2++].LEDColor = Effects.getColorFromCode(ORANGE); }catch(Exception ex) { }
                try { dbo.Leds[strip.P3++].LEDColor = Effects.getColorFromCode(YELLOW); }catch(Exception ex) { }
                try { dbo.Leds[strip.P4++].LEDColor = Effects.getColorFromCode(GREEN); }catch(Exception ex) { }
                try { dbo.Leds[strip.P5++].LEDColor = Effects.getColorFromCode(BLUE); }catch(Exception ex) { }

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

            return bRet;
        }

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
                    dbo.Leds[(i + strip.ShiftPixelBy)].LEDColor = getColorFromCode(int.Parse(seq.colors[i].Split('-')[0].Trim()));
                }catch(Exception ex)
                {

                }
            }

            return bRet;
        }

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
                    //Simply set colors, setup values, and exit
                    loadColor(strip, DrawManager);

                    strip.Init = false;
                    strip.Bounces = seq.bounces;
                    strip.ShiftPixelBy = 0;
                    strip.Head = initHead;
                    strip.Tail = initTail;

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
            int elem = 0, colorValue = 0;

            //Add 1 to i if i is -1 and reset ps
            if (strip.I < 0)
            {
                //Add 1 to i making i = 0
                strip.I++;

                if (strip.IsRainbow)
                {
                    //Reset p values
                    strip.P0 = -1;
                    strip.P1 = 0;
                    strip.P2 = 1;
                    strip.P3 = 2;
                    strip.P4 = 3;
                    strip.P5 = 4;
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
                    try { dbo.Leds[strip.P0++].LEDColor = Effects.getColorFromCode(CLEAR); } catch (Exception ex) { }
                    try { dbo.Leds[strip.P1++].LEDColor = Effects.getColorFromCode(RED); } catch (Exception ex) { }
                    try { dbo.Leds[strip.P2++].LEDColor = Effects.getColorFromCode(ORANGE); } catch (Exception ex) { }
                    try { dbo.Leds[strip.P3++].LEDColor = Effects.getColorFromCode(YELLOW); } catch (Exception ex) { }
                    try { dbo.Leds[strip.P4++].LEDColor = Effects.getColorFromCode(GREEN); } catch (Exception ex) { }
                    try { dbo.Leds[strip.P5++].LEDColor = Effects.getColorFromCode(BLUE); } catch (Exception ex) { }

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
                    strip.I++;;
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
                        try { dbo.Leds[strip.VirtualPixelArray[elem]].LEDColor = Effects.getColorFromCode(int.Parse(seq.colors[elem].Split('-')[0].Trim())); } catch (Exception ex) { }
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
        */
        public static Color getColorFromCode(int ColorCode)
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

            return clr;
        }

        /*
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
    }
}
