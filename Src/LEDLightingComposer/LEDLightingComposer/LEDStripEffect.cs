using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace LEDLightingComposer
{
    public class LEDStripEffect
    {
        //Declare global variables
        private String stripName;
        private String[] ledColorArray;
        private List<LED> leds;
        private int lightingEffect, effectStart, effectDuration, pinSetup;

        /*
            Lighting effects are:
            0 - All Clear
            1 - Rainbow
            2 - LoadColor
            3 - Bounce Back
            4 - Flowthrough

            Colors are:
            0 - Red
            1 - Green
            2 - Blue
            3 - White
            4 - Yellow
            5 - Cyan
            6 - Magenta
        */

        public LEDStripEffect(String StripName, int NUMLeds, String[] LEDColorArray, int LightingEffect, int EffectStart, int EffectDuration, int PinSetup, int Top, int Left, int Bottom, int Right, bool Add)
        {
            this.stripName = StripName;
            this.ledColorArray = LEDColorArray;
            this.lightingEffect = LightingEffect;
            this.effectStart = EffectStart;
            this.effectDuration = EffectDuration;
            this.pinSetup = PinSetup;
            bool add = Add; //allows loop to not add later if leds cannot fit on screen...

            leds = new List<LED>();
            for (int i = 0; i < NUMLeds; i++)
            {
                if (add)
                {
                    //Add led to list with specific color
                    switch (LEDColorArray[i].Trim().Substring(0,1))
                    {
                        case "0":
                            leds.Add(new LED(stripName, Color.Red, Top, Left, new Rectangle(Left, Top, 20, 20)));
                            break;
                        case "1":
                            leds.Add(new LED(stripName, Color.Green, Top, Left, new Rectangle(Left, Top, 20, 20)));
                            break;
                        case "2":
                            leds.Add(new LED(stripName, Color.Blue, Top, Left, new Rectangle(Left, Top, 20, 20)));
                            break;
                        case "3":
                            leds.Add(new LED(stripName, Color.White, Top, Left, new Rectangle(Left, Top, 20, 20)));
                            break;
                        case "4":
                            leds.Add(new LED(stripName, Color.Yellow, Top, Left, new Rectangle(Left, Top, 20, 20)));
                            break;
                        case "5":
                            leds.Add(new LED(stripName, Color.Cyan, Top, Left, new Rectangle(Left, Top, 20, 20)));
                            break;
                        case "6":
                            leds.Add(new LED(stripName, Color.Magenta, Top, Left, new Rectangle(Left, Top, 20, 20)));
                            break;
                    }
                }

                //Add 20 to left for next LED position
                Left += 20;

                //Verify this will not move LED position into WMPLayer object
                if ((Left + 20) > Right)
                {
                    //Move LED position to next row
                    Left = 0;
                    Top += 30; /*Should i verify bottom before changing this top?*/
                    if(Top > Bottom)
                    {
                        add = false;
                    }

                }

            }
        }

        /*
            Function: updateLEDEffects

        */
        public void updateLEDEffects(int currentTime)
        {
            //Configure animation for effect
            switch (lightingEffect)
            {
                case 0:
                    break;
                case 1:
                    break;
                case 2:
                    break;
                case 3:
                    break;
                case 4:
                    break;
            }
        }

        public void drawLEDEffect(Graphics g)
        {
            //Loop through all leds
            for (int i = 0; i < leds.Count; i++)
            {
                //Draw all leds with the desired effect
                leds[i].drawObject(g);

                //switch (ledColorArray[i].Trim().Substring(0,1))
                //{
                //    case "0":
                //        leds[i].drawObject(g);
                //        break;
                //    case "1":
                //        leds[i].drawObject(g);
                //        break;
                //    case "2":
                //        leds[i].drawObject(g);
                //        break;
                //    case "3":
                //        leds[i].drawObject(g);
                //        break;
                //    case "4":
                //        leds[i].drawObject(g);
                //        break;
                //    case "5":
                //        leds[i].drawObject(g);
                //        break;
                //    case "6":
                //        leds[i].drawObject(g);
                //        break;
                //}
            }
        }

        #region Getters & Setters

        /*
        */
        internal List<LED> Leds
        {
            get
            {
                return leds;
            }

            set
            {
                leds = value;
            }
        }

        /*
        */
        public int PinSetup
        {
            get { return this.pinSetup; }
        }

        #endregion Getters & Setters
    }
}
