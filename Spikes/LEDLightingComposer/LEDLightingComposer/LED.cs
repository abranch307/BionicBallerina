using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LEDLightingComposer
{
    class LED
    {
        private String stripName;
        private int top, left;
        private Rectangle rect;

        public LED(String StripName, int Top, int Left, Rectangle Rect)
        {
            this.stripName = StripName;
            this.top = Top;
            this.left = Left;
            this.rect = Rect;
        }

        public void drawObject(Graphics g, Brush drawBrush)
        {
            Pen pen = new Pen(Color.Black);
            g.DrawRectangle(pen, rect);
            g.FillRectangle(drawBrush, rect);
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
    }
}
