using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LEDLightingComposer
{
    public class DrawingManager
    {
        //Declare global variables
        private List<DrawableObject> drawableObjects;

        /*
        */
        public DrawingManager()
        {
            //Initialize drawing objects List
            drawableObjects = new List<DrawableObject>();
        }

        /*
        */
        public void draw(Graphics g, int bottom, int right)
        {
            //Draw all strip effects
            foreach(DrawableObject dbo in drawableObjects)
            {
                dbo.drawLEDEffect(g);
            }

            //Draw line for track bar

        }

        /*
        */
        public DrawableObject getDrawableObject(String Type, String Value1)
        {
            //Declare variables
            DrawableObject doRet = null;

            try
            {
                //Loop through drawable objects and find object according to parameters
                if (Type.Equals("PINSETUP"))
                {
                    foreach (DrawableObject d in drawableObjects)
                    {
                        if (d.PinSetup == int.Parse(Value1))
                        {
                            //Set return 
                            doRet = d;
                            break;
                        }
                    }
                }
            }catch(Exception ex)
            {
                //Simply allow returning of null object
            }

            return doRet;
        }

        /*
        */
        internal List<DrawableObject> DrawableObjects
        {
            get
            {
                return drawableObjects;
            }

            set
            {
                drawableObjects = value;
            }
        }
    }
}
