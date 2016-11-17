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
            //String drawString = "HELLO! The bottom is " + bottom + " and the right is " + right;
            //Font drawFont = new Font("Arial", 16);
            //SolidBrush drawBrush = new SolidBrush(Color.Black);
            //Pen drawPen = new Pen(Color.Blue);
            //float x = 10.0F, y = 10.0F;
            //StringFormat drawFormat = new StringFormat();

            //try
            //{
            //    g.DrawRectangle(drawPen, new Rectangle(10, 10, (right - 20), (bottom - 20)));
            //    g.DrawString(drawString, drawFont, drawBrush, x, y, drawFormat);
            //    drawFont.Dispose();
            //    drawBrush.Dispose();
            //    drawFormat.Dispose();
            //}
            //catch (Exception ex)
            //{

            //}

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
