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
    public partial class LEDColor : Form
    {
        //Declare global variables
        List<String> ledCArray = new List<String>();
        List<ComboBox> comboBoxes = new List<ComboBox>();

        public LEDColor(int NumLEDs, List<String> LEDCArray)
        {
            InitializeComponent();

            //Set global variables to passed variables
            this.ledCArray = LEDCArray;

            //If list is empty, create new blank text boxes for each led, otherwise load existing array if numLEDs = list length
            if(ledCArray.Count > 0 && ledCArray.Count == NumLEDs)
            {
                //Load existing array into panel
                for(int i = 0; i < ledCArray.Count; i++)
                {
                    createAndAdd2Panel(ledCArray[i], i);
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
            //Create new combobox, add to list, and then add to panel
            Label lbl = new Label();
            lbl.Text = "Color Selection: " + count;
            lbl.Top = 25 * count;

            ComboBox cbox = new ComboBox();
            comboBoxes.Add(cbox);
            cbox.Items.AddRange(new object[] {
                "0 - Red",
                "1 - Green",
                "2 - Blue",
                "3 - White",
                "4 - Yellow",
                "5 - Cyan",
                "6 - Magenta"});
            cbox.SelectedIndex = 0;
            cbox.Top = 25 * count;
            cbox.Left = 100;

            this.panel1.Controls.Add(lbl);
            this.panel1.Controls.Add(cbox);
        }

        private void createAndAdd2Panel(String text, int count)
        {
            //Create new textbox, add to list, and then add to panel
            Label lbl = new Label();
            lbl.Text = "Color Selection: " + count;
            lbl.Top = 25 * count;

            ComboBox cbox = new ComboBox();
            comboBoxes.Add(cbox);
            cbox.Items.AddRange(new object[] {
                "0 - Red",
                "1 - Green",
                "2 - Blue",
                "3 - White",
                "4 - Yellow",
                "5 - Cyan",
                "6 - Magenta"});
            cbox.SelectedItem = text;
            cbox.Top = 25 * count;
            cbox.Left = 100;

            this.panel1.Controls.Add(lbl);
            this.panel1.Controls.Add(cbox);
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            int i = 0, t = 0;

            //Add to led color array then exit screen
            ledCArray.Clear();
            for (i = 0; i < comboBoxes.Count; i++)
            {
                ledCArray.Add(comboBoxes[i].Text.ToString().Trim());
            }
            btnExit_Click(null, null);
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
