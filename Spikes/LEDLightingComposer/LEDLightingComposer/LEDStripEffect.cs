using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace LEDLightingComposer
{
    class LEDStripEffect
    {
        //Declare global variables
        private String stripName;
        private String[] ledColorArray;
        private List<LED> leds;
        private int lightingEffect, effectStart, effectDuration;

        public LEDStripEffect(String StripName, int NUMLeds, String[] LEDColorArray, int LightingEffect, int EffectStart, int EffectDuration, int Top, int Left, int Bottom, int Right)
        {
            this.stripName = StripName;
            this.ledColorArray = LEDColorArray;
            this.lightingEffect = LightingEffect;
            this.effectStart = EffectStart;
            this.effectDuration = EffectDuration;

            bool add = true;

            leds = new List<LED>();
            for (int i = 0; i < NUMLeds; i++)
            {
                if (add)
                {
                    //Add led to list
                    leds.Add(new LED(stripName, Top, Left, new Rectangle(Left, Top, 20, 20)));
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

        public void drawLEDEffect(Graphics g)
        {
            //Loop through all leds
            for(int i = 0; i < leds.Count; i++)
            {
                //Configure animation for effect
                switch (lightingEffect)
                {
                    case 0:
                        break;
                    case 1:
                        break;
                }

                //Draw all leds with the desired effect
                switch (ledColorArray[i].Trim().Substring(0,1))
                {
                    case "0":
                        leds[i].drawObject(g, new SolidBrush(Color.Red));
                        break;
                    case "1":
                        leds[i].drawObject(g, new SolidBrush(Color.Green));
                        break;
                    case "2":
                        leds[i].drawObject(g, new SolidBrush(Color.Blue));
                        break;
                    case "3":
                        leds[i].drawObject(g, new SolidBrush(Color.White));
                        break;
                }
            }
        }

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
    }
}
