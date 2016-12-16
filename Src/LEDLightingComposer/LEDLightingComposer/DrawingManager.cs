/*
	Author: Aaron Branch, Zach Jarmon, Peter Martinez
	Created: 
	Last Modified:
	Class: .cs
	Class Description:
		
*/

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
            Function:

            Parameters:

            Returns:
        */
        public DrawingManager()
        {
            //Initialize drawing objects List
            drawableObjects = new List<DrawableObject>();
        }

        /*
            Function:

            Parameters:

            Returns:
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
            Function:

            Parameters:

            Returns:
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
            Function:

            Parameters:

            Returns:
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
