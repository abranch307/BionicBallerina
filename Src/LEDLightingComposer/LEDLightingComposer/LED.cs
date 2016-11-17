using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LEDLightingComposer
{
    public class LED
    {
        private String stripName;
        private int top, left;
        private Rectangle rect;
        private Color ledColor;

        public LED(Color Clr, int Top, int Left, Rectangle Rect)
        {
            this.ledColor = Clr;
            this.top = Top;
            this.left = Left;
            this.rect = Rect;
        }

        public void drawObject(String Type, Graphics g)
        {
            Pen pen = new Pen(Color.Black);
            g.FillRectangle(new SolidBrush(ledColor), rect);

            if (Type.Equals("LINE"))
            {
                //Draw leds in a straight line
                g.DrawRectangle(pen, rect);
            }
        }

        public int Top
        {
            get
            {
                return top;
            }

            set
            {
                top = value;
            }
        }

        public Color LEDColor
        {
            get
            {
                return ledColor;
            }
            set
            {
                ledColor = value;
            }
        }
    }
}
