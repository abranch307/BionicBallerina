using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;

namespace LEDLightingComposer
{
    public class DrawableObject
    {
        private List<LED> leds;
        private String[] ledColorArray, ledPositionArray;
        private String objectType, mcuNameDesc, pinSetupDesc;
        private int pinSetup, top, left, bottom, right, rotation;

        /*
            Function:

            Parameters:

            Returns:
        */
        public DrawableObject(String ObjectType, int PinSetup, int NUMLeds, String[] LEDColorArray, String[] LEDPositionArray, String MCUNameDesc, String PinSetupDesc, int Top, int Left, int Bottom, int Right, int Rotation, bool Add)
        {
            this.objectType = ObjectType;
            this.pinSetup = PinSetup;
            this.ledColorArray = LEDColorArray;
            this.ledPositionArray = LEDPositionArray;
            this.mcuNameDesc = MCUNameDesc;
            this.pinSetupDesc = PinSetupDesc;
            this.top = Top;
            this.left = Left;
            this.bottom = Bottom;
            this.right = Right;
            this.rotation = Rotation;

            leds = new List<LED>();

            if (objectType.Equals("LED"))
            {
                for (int i = 0; i < NUMLeds; i++)
                {
                    if (Add)
                    {
                        try
                        {
                            //Add led to list with specific color
                            leds.Add(new LED(objectType, "", Effects.getColorFromCode( int.Parse(LEDColorArray[i].Trim().Substring(0, 1)), 0), Top, Left, new Rectangle(Left, Top, 20, 20)));
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Error in DrawableObject class: " + ex.Message);
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
                                   //if (Top > Bottom)
                                   //{
                                   //    add = false;
                                   //}

                    }

                }
            }else
            {
                //Add text position to screen on next row
                leds.Add(new LED(objectType, (mcuNameDesc + " ; " + PinSetupDesc), Color.Black, Top, Left, new Rectangle(Left, Top, 20, 20)));
            }
        }

        /*
            Function:

            Parameters:

            Returns:
        */
        public void drawLEDEffect(Graphics g)
        {
            if (objectType.Equals("LED"))
            {
                //Loop through all leds
                for (int i = 0; i < leds.Count; i++)
                {
                    //Draw all leds with the desired effect
                    leds[i].drawObject(g);
                }
            }
            else
            {
                leds[0].drawObject(g);
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
