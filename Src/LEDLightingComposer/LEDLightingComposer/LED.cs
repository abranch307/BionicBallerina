/*
	Author: Aaron Branch, Zach Jarmon, Peter Martinez
	Created: 
	Last Modified:    
	Class: .cs
	Class Description:
		This class 
*/

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
        private int top, left;
        private Rectangle rect;
        private Color ledColor;
        private String type, text;

        public LED(String Type, String Text, Color Clr, int Top, int Left, Rectangle Rect)
        {
            this.type = Type;
            this.text = Text;
            this.ledColor = Clr;
            this.top = Top;
            this.left = Left;
            this.rect = Rect;
        }

        public void drawObject(Graphics g)
        {
            if (type.Equals("LED"))
            {
                Pen pen = new Pen(Color.Black);
                g.FillRectangle(new SolidBrush(ledColor), rect);

                //Draw leds in a straight line
                g.DrawRectangle(pen, rect);
            }else if(type.Equals("TEXT"))
            {
                Font drawFont = new Font("Arial", 8);
                SolidBrush drawBrush = new SolidBrush(Color.Black);
                Pen drawPen = new Pen(Color.Blue);
                StringFormat drawFormat = new StringFormat();

                try
                {
                    g.DrawString(text, drawFont, drawBrush, left, top, drawFormat);
                    drawFont.Dispose();
                    drawBrush.Dispose();
                    drawFormat.Dispose();
                }
                catch (Exception ex)
                {

                }
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
                ledColor = Effects.updateBrightness(ledColor, Brightness);
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
