using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Modeling.Diagrams;
using System.ComponentModel;

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

        /*
        */
        public bool updateBrightness(int Brightness)
        {
            //Declare variables
            bool bret = false;

            try
            {
                float correctionFactor = (((float)255 - (float)Brightness) / (float)255);
                float red = (255 - ledColor.R) * correctionFactor + ledColor.R;
                float green = (255 - ledColor.G) * correctionFactor + ledColor.G;
                float blue = (255 - ledColor.B) * correctionFactor + ledColor.B;
                ledColor = Color.FromArgb(ledColor.A, (int)red, (int)green, (int)blue);
            }catch(Exception ex)
            {

            }

            return bret;
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
