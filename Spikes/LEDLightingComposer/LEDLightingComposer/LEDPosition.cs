using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LEDLightingComposer
{
    public partial class LEDPosition : Form
    {
        //Declare global variables
        List<int> ledPArray = new List<int>();
        List<TextBox> txtBoxes = new List<TextBox>();

        public LEDPosition(int NumLEDs, List<int> LEDPArray)
        {
            InitializeComponent();

            //Set global variables to passed variables
            this.ledPArray = LEDPArray;

            //If list is empty, create new blank text boxes for each led, otherwise load existing array if numLEDs = list length
            if(ledPArray.Count > 0 && ledPArray.Count == NumLEDs)
            {
                //Load existing array into panel
                for(int i = 0; i < ledPArray.Count; i++)
                {
                    createAndAdd2Panel(ledPArray[i].ToString(), i);
                }
            }
            else
            {
                //Load new text boxes into panel
                for(int i = 0; i < NumLEDs; i++)
                {
                    createAndAdd2Panel(i);
                }
            }

        }

        /*
        */
        private void createAndAdd2Panel(int count)
        {
            //Create new textbox, add to list, and then add to panel
            Label lbl = new Label();
            lbl.Text = "LED Index: " + count;
            lbl.Top = 25 * count;

            TextBox txtbox = new TextBox();
            txtBoxes.Add(txtbox);
            txtbox.Top = 25 * count;
            txtbox.Left = 100;

            this.panel1.Controls.Add(lbl);
            this.panel1.Controls.Add(txtbox);
        }

        private void createAndAdd2Panel(String text, int count)
        {
            //Create new textbox, add to list, and then add to panel
            Label lbl = new Label();
            lbl.Text = "LED Index: " + count;
            lbl.Top = 25 * count;

            TextBox txtbox = new TextBox();
            txtBoxes.Add(txtbox);
            txtbox.Text = text;
            txtbox.Top = 25 * count;
            txtbox.Left = 100;

            this.panel1.Controls.Add(lbl);
            this.panel1.Controls.Add(txtbox);
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            int i = 0, t = 0;

            //Loop through array of text boxes and verify all have a value
            for (i = 0; i < txtBoxes.Count; i++)
            {
                try
                {
                    t = Convert.ToInt32(txtBoxes[i].Text.ToString().Trim());
                }catch(Exception ex)
                {
                    MessageBox.Show("There is at least 1 textbox that doesn't have a value.  You must have valid values in all textboxes before saving...: " + ex.Message);
                    return;
                }
                
            }

            //If all text boxes have values, add to led position array then exit screen
            ledPArray.Clear();
            for (i = 0; i < txtBoxes.Count; i++)
            {
                ledPArray.Add(Convert.ToInt32(txtBoxes[i].Text.ToString().Trim()));
            }
            btnExit_Click(null, null);
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
