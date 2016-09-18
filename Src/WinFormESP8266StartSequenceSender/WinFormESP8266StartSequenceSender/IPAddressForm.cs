using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace WinFormESP8266StartSequenceSender
{
    public partial class IPAddressForm : Form
    {
        //Global variables
        List<String> mcuIPAddresses;
        List<TextBox> txtBoxes;

        public IPAddressForm(int NumMcus, List<String> MCUIPAddresses)
        {
            //Declare components
            InitializeComponent();

            //Initialize variables
            txtBoxes = new List<TextBox>();

            //Set class variables to passed variables
            this.mcuIPAddresses = MCUIPAddresses;

            //Verify if list has elements and matches number of mcus passed
            if (mcuIPAddresses.Count > 0 && mcuIPAddresses.Count == NumMcus)
            {
                //Do not reset list
            }
            else if(mcuIPAddresses != null)
            {
                //Reset list
                mcuIPAddresses.Clear();
            }

            //Loop for num mcus and programatically add labels and textboxes
            if (LoopNumMCUs(NumMcus) < 1)
            {
                //Close this window since form was not setup correctly
                this.Close();
            }
        }

        /*
            Function LoopNumMCUs:
            This function will loop for passed number of times, creating a label and textbox pair on the form for user to enter ip address.
            If the ip address list has elements, then a string will be passed to the creation of the label and textbox to be entered into
            the textbox initially

            Parameters: int NumMcus - number of label-textbox pairs to create

            Returns: int - number of loop iterations
        */
        private int LoopNumMCUs(int NumMcus)
        {
            //Declare variables
            int i = 0;
            String str = "";

            for(i = 0; i < NumMcus; i++)
            {
                //Change string value for textbox if necessary
                if (mcuIPAddresses.Count > 0)
                {
                    //Set str to list value
                    str = mcuIPAddresses[i].ToString().Trim();
                }
                else
                {
                    //Set str to blank value
                    str = "";
                }

                //Create label and textbox pair and add to form
                if (CreateLabelTextbox(i, str) < 1)
                {
                    //Notify user that there was an error setting up IP Address Form
                    MessageBox.Show("There was an error creating the IP Address Form.  Please try again...");
                    i = -1;
                    break;
                }
            }

            return (i+1);
        }

        /*
            Function CreateLabelTextBox:
            This function will create a label and textbox pair and add to panel on form.  The label-textbox pair position will be
            determined by the passed iteration value

            Parameters: int count - iteration for calling loop which will determine the position on the form, String text - text to put into textbox

            Returns: int - 1 if label and textbox creation is successful, -1 if unsuccessful
        */
        private int CreateLabelTextbox(int count, String text)
        {
            //Declare variables
            int iret = -1;

            //Create and setup label and textbox
            Label lbl = new Label();
            lbl.Text = "MCU IP Address: " + count;
            lbl.Top = 25 * count;

            TextBox txtbox = new TextBox();
            txtBoxes.Add(txtbox);
            txtbox.Top = 25 * count;
            txtbox.Left = 100;
            txtbox.Text = text;

            //Add label and textbox to form panel
            this.panel1.Controls.Add(lbl);
            this.panel1.Controls.Add(txtbox);

            //Set iret to 1 for success
            iret = 1;

            return iret;
        }

        /*
            Function btnSave_Click
            This function will verify valid ip addresses have been entered and save to list array
        */
        private void btnSave_Click(object sender, EventArgs e)
        {
            //Declare variables
            int i = 0;
            String str = "", IPADDRESS_PATTERN = @"^(\d|[1-9]\d|1\d\d|2([0-4]\d|5[0-5]))\.(\d|[1-9]\d|1\d\d|2([0-4]\d|5[0-5]))\.(\d|[1-9]\d|1\d\d|2([0-4]\d|5[0-5]))\.(\d|[1-9]\d|1\d\d|2([0-4]\d|5[0-5]))$";
            Regex rgx = new Regex(IPADDRESS_PATTERN);

            //Loop through array of text boxes and verify all have a value
            for (i = 0; i < txtBoxes.Count; i++)
            {
                try
                {
                    //Get current element's textbox
                    str = txtBoxes[i].Text.ToString().Trim();

                    //Verify the string is in a valid ip address format
                    if (!rgx.IsMatch(str))
                    {
                        //Notify user that the ip address must be in correct form then exit function
                        MessageBox.Show("The ip address " + str + " you entered is not in the correct format.  Please verify and re-enter before trying to save again...");
                        return;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("There is an issue with a value in one of the textboxes.  You must have valid values in all textboxes before saving...: " + ex.Message);
                    return;
                }

            }

            //If all text boxes have values, add to led position array then exit screen
            mcuIPAddresses.Clear();
            for (i = 0; i < txtBoxes.Count; i++)
            {
                mcuIPAddresses.Add(txtBoxes[i].Text.ToString().Trim());
            }
            btnExit_Click(null, null);
        }

        /*
            This function will close the form without saving IP Address Array
        */
        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
