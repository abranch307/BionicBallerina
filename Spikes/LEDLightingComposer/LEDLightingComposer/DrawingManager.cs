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
        private List<LEDStripEffect> ledStrips;

        /*
        */
        public DrawingManager()
        {
            //Initialize drawing objects List
            ledStrips = new List<LEDStripEffect>();
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
            foreach(LEDStripEffect lse in ledStrips)
            {
                lse.drawLEDEffect(g);
            }
        }

        /*
        */
        internal List<LEDStripEffect> LedStrips
        {
            get
            {
                return ledStrips;
            }

            set
            {
                ledStrips = value;
            }
        }
    }
}
