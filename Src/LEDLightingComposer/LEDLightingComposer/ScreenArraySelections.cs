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
    public partial class ScreenArraySelections : Form
    {
        //Declare global variables
        private String type;
        private List<int> ledPArray = new List<int>();
        private List<String> ledCArray = new List<String>();
        private Dictionary<String, bool> foundIPAddresses = new Dictionary<string, bool>();
        private List<ComboBox> comboBoxes = new List<ComboBox>();
        private List<TextBox> txtBoxes = new List<TextBox>();
        private List<CheckBox> chkBoxes = new List<CheckBox>();
        private List<Label> labels = new List<Label>();

        public ScreenArraySelections(String Type, int NumLEDs, List<int> LEDPArray, List<String> LEDCArray, Dictionary<string, bool> FoundIPAddresses)
        {
            InitializeComponent();

            //Anchor elements
            this.panel1.Anchor = (AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right);
            this.btnSave.Anchor = (AnchorStyles.Bottom | AnchorStyles.Left);
            this.btnExit.Anchor = (AnchorStyles.Bottom | AnchorStyles.Left);

            //Set variables
            this.type = Type;

            if (type.Equals("LED"))
            {
                //Change form title
                this.Text = "LED Position Array";

                //Set global variables to passed variables
                this.ledPArray = LEDPArray;

                //If list is empty, create new blank text boxes for each led, otherwise load existing array if numLEDs = list length
                if (ledPArray.Count > 0 && ledPArray.Count == NumLEDs)
                {
                    //Load existing array into panel
                    for (int i = 0; i < ledPArray.Count; i++)
                    {
                        createAndAdd2Panel(ledPArray[i].ToString(), null, i);
                    }
                }
                else
                {
                    //Load new text boxes into panel
                    for (int i = 0; i < NumLEDs; i++)
                    {
                        createAndAdd2Panel("", null, i);
                    }
                }
            }else if (type.Equals("COLOR"))
            {
                //Change form title
                this.Text = "LED Color Array";

                //Set global variables to passed variables
                this.ledCArray = LEDCArray;

                //If list is empty, create new blank text boxes for each led, otherwise load existing array if numLEDs = list length
                if (ledCArray.Count > 0 && ledCArray.Count == NumLEDs)
                {
                    //Load existing array into panel
                    for (int i = 0; i < ledCArray.Count; i++)
                    {
                        createAndAdd2Panel(ledCArray[i], null, i);
                    }
                }
                else
                {
                    //Load new comboboxes into panel
                    for (int i = 0; i < NumLEDs; i++)
                    {
                        createAndAdd2Panel("", null, i);
                    }
                }
            }else if (type.Equals("IP"))
            {
                //Change form title
                this.Text = "Found IP Addresses";

                //Return if foundIPAddresses are null
                if(foundIPAddresses == null)
                {
                    return;
                }

                //Set global variables to passed variables
                this.foundIPAddresses = FoundIPAddresses;

                //If list is empty, create new blank text boxes for each led, otherwise load existing array if numLEDs = list length
                if (foundIPAddresses.Count > 0)
                {
                    //Load existing array into panel
                    for (int i = 0; i < foundIPAddresses.Count; i++)
                    {
                        createAndAdd2Panel(null, new Dictionary<string, bool>() { {FoundIPAddresses.Keys.ElementAt(i), foundIPAddresses.Values.ElementAt(i) } }, i);
                    }
                }
            }

        }

        private void createAndAdd2Panel(String text, Dictionary<String, bool> dic, int count)
        {
            //Create new textbox, add to list, and then add to panel
            Label lbl = new Label();

            if (type.Equals("LED"))
            {
                //Create Label
                lbl.Text = "LED Index: " + count;
                lbl.Top = 25 * count;

                //Create and setup textbox
                TextBox txtbox = new TextBox();
                txtBoxes.Add(txtbox);

                if (text.Trim().Equals(""))
                {
                    txtbox.Text = count.ToString();
                }
                else
                {
                    txtbox.Text = text;
                }
                txtbox.Top = 25 * count;
                txtbox.Left = 100;
                txtbox.Enabled = false;

                //Add textbox
                this.panel1.Controls.Add(txtbox);
            }
            else if (type.Equals("COLOR"))
            {
                //Create Label
                lbl.Text = "Color Selection: " + count;

                ComboBox cbox = new ComboBox();
                comboBoxes.Add(cbox);
                cbox.Items.AddRange(new object[] {
                "0 - Clear",
                "1 - White",
                "2 - Red",
                "3 - Green",
                "4 - Blue",
                "5 - Yellow",
                "6 - Cyan",
                "7 - Magenta",
                "8 - Orange"});
                if (text.Trim().Equals(""))
                {
                    cbox.SelectedIndex = 0;
                }
                else
                {
                    cbox.SelectedItem = text;
                }
                cbox.Top = 25 * count;
                cbox.Left = 100;

                this.panel1.Controls.Add(cbox);
            }
            else if (type.Equals("IP"))
            {
                //Create Label
                lbl.Text = dic.Keys.ElementAt(0);
                lbl.Width = 250;
                labels.Add(lbl);
                lbl.Top = 25 * count;

                //Create and setup checkbox
                CheckBox chkbox = new CheckBox();
                chkBoxes.Add(chkbox);

                if (dic.Values.ElementAt(0))
                {
                    chkbox.Checked = true;
                }
                else
                {
                    chkbox.Checked = false;
                }
                chkbox.Top = 25 * count;
                chkbox.Left = 300;

                this.panel1.Controls.Add(chkbox);
            }

            //Add label
            this.panel1.Controls.Add(lbl);
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            if (type.Equals("IP"))
            {
                //Clear ip address info and exit
                foundIPAddresses.Clear();

                //Set
                this.DialogResult = DialogResult.Retry;
                this.Close();
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            int i = 0, t = 0;

            if (type.Equals("LED"))
            {
                //Loop through array of text boxes and verify all have a value
                for (i = 0; i < txtBoxes.Count; i++)
                {
                    try
                    {
                        t = Convert.ToInt32(txtBoxes[i].Text.ToString().Trim());
                    }
                    catch (Exception ex)
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
            }
            else if (type.Equals("COLOR"))
            {
                //Add to led color array then exit screen
                ledCArray.Clear();
                for (i = 0; i < comboBoxes.Count; i++)
                {
                    ledCArray.Add(comboBoxes[i].Text.ToString().Trim());
                }
            }
            else if (type.Equals("IP"))
            {
                //Add to found ip addresses array then exit screen
                foundIPAddresses.Clear();
                for (i = 0; i < chkBoxes.Count; i++)
                {
                    foundIPAddresses.Add(labels[i].Text, chkBoxes[i].Checked);
                }
            }
            
            btnExit_Click(null, null);
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
