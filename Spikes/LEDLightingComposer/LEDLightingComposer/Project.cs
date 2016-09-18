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
    public partial class Project : Form
    {
        //Declare global variables
        private DatabaseManager dbmanager;
        private List<int> ledPArray = new List<int>();
        private List<String> ledCArray = new List<string>();
        
        public Project()
        {
            InitializeComponent();

            //Setup screen values
            setupScreenValues();
        }

        public Project(DatabaseManager DBManager, int TimerVal, String CurrentSongPath)
        {
            //Initialize Components
            InitializeComponent();

            //Set global variables to passed variables
            this.dbmanager = DBManager;

            //Load Project Names from LED_Project table
            dbmanager.loadProjectNames(this.cBoxProjectName);

            //Load MCU Names from MCU table
            dbmanager.loadMCUNames(this.cBoxMCUName);

            //Load Lighting Effects from Lighting_Effects table
            dbmanager.loadLightingEffects(this.cBoxLEffect);

            //Setup screen values
            setupScreenValues();

            //Set screen values to default
            this.txtEffectStartTime.Text = TimerVal.ToString();
            this.txtEffectStartTime.Update();
            this.txtSongPath.Text = CurrentSongPath;
            this.txtSongPath.Update();
        }

        #region Verification Methods

        /*
        */
        private bool veriProjectName()
        {
            //Declare variables
            bool bret = false;
            String[] pName = { "" };

            try {
                //Verify a project name has been entered
                if ((pName[0] = this.cBoxProjectName.Text.ToString().Trim().ToUpper()).Equals(""))
                {
                    //Notify user a Project name is required then return false
                    MessageBox.Show("Please enter a Project Name before trying to save...");
                    return bret;
                }

                //Verify if project exists, and if not create
                pName = pName[0].Split('-');
                if (!dbmanager.verifyNameExistsInDatabase("LED_PROJECT", pName[0].Trim(), "", "", ""))
                {
                    if(dbmanager.insertRecordIntoDB("LED_PROJECT", pName[0].Trim(), pName[1].Trim(), "", "") < 1)
                    {
                        bret = false;
                    }
                    else
                    {
                        bret = true;
                    }
                }
                else
                {
                    bret = true;
                }

                //Update textbox string
                this.cBoxProjectName.Text = pName[0].Trim() + " - " + pName[1].Trim();
                this.cBoxProjectName.Update();
            }catch(Exception ex)
            {
                MessageBox.Show("Error verifying Project Name... : " + ex.Message);
            }

            return bret;
        }
        
        /*
        */
        private bool veriMCUName()
        {
            //Declare variables
            String[] mcuName = { "" };
            bool bret = false;

            try {
                if ((mcuName[0] = this.cBoxMCUName.Text.ToString().Trim().ToUpper()).Equals(""))
                {
                    //Notify user a mcu name is required
                    MessageBox.Show("Please enter/select a MCU Name before trying to save...");
                    return bret;
                }
                else
                {
                    //Create MCU Name in database if necessary
                    mcuName = mcuName[0].Split('-');
                    if (!dbmanager.verifyNameExistsInDatabase("MCU", mcuName[0].Trim(), "", "", ""))
                    {
                        if(dbmanager.insertRecordIntoDB("MCU", mcuName[0].Trim(), mcuName[1].Trim(), "", "") < 1)
                        {
                            bret = false;
                        }
                        else
                        {
                            bret = true;
                        }
                    }
                    else
                    {
                        bret = true;
                    }

                    //Update textbox string
                    this.cBoxMCUName.Text = mcuName[0].Trim() + " - " + mcuName[1].Trim();
                    this.cBoxMCUName.Update();
                }
            }catch(Exception ex)
            {
                MessageBox.Show("Error while verifying MCU Name...: " + ex.Message);
            }

            return bret;
        }
        
        /*
        */
        private bool veriPinSetup()
        {
            //Declare variables
            String[] pinSetup = { "" };
            String projectName = (this.cBoxProjectName.Text.ToString().Trim().ToUpper().Split('-'))[0].Trim(),
                mcuName = (this.cBoxMCUName.Text.ToString().Trim().ToUpper().Split('-'))[0].Trim();
            bool bret = false;

            try
            {
                if ((pinSetup[0] = this.cBoxPinSetup.Text.ToString().Trim().ToUpper()).Equals(""))
                {
                    //Notify user a pin setup is required
                    MessageBox.Show("Please enter/select a Pin Setup before trying to save...");
                    return bret;
                }
                else
                {
                    //Create MCU Name in database
                    pinSetup = pinSetup[0].Split(';');
                    if (!dbmanager.verifyNameExistsInDatabase("MCU_PINS", projectName, mcuName, pinSetup[0].Trim(), pinSetup[1].Trim()))
                    {
                        if(dbmanager.insertRecordIntoDBReturnIncr("MCU_PINS", pinSetup[2].Trim(), projectName, mcuName, pinSetup[0].Trim(), pinSetup[1].Trim(), "", "", null, null) < 1)
                        {
                            //Set bret to false since insert was not successful
                            bret = false;
                        }
                        else
                        {
                            //Set bret to true since insert was successful
                            bret = true;
                        }
                    }
                    else
                    {
                        //Set bret to true since record exists in table
                        bret = true;
                    }

                    //Update textbox string
                    this.cBoxPinSetup.Text = pinSetup[0].Trim() + ";" + pinSetup[1].Trim() + ";" + pinSetup[2].Trim();
                    this.cBoxPinSetup.Update();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error while verifying Pin Setup...: " + ex.Message);
            }

            return bret;
        }

        /*
        */
        private bool veriLEDPositionArray()
        {
            //Declare variables
            bool bret = false;

            try {
                if (ledPArray.Count == (int.Parse(this.txtNumLEDs.Text.ToString().Trim()))) { bret = true; }
            }catch(Exception ex)
            {
                MessageBox.Show("There was an error verifying LED Position Array count match...: " + ex.Message);
            }

            return bret;
        }

        /*
        */
        private bool veriLEDColorArray()
        {
            //Declare variables
            bool bret = false;

            try
            {
                if (ledCArray.Count == (int.Parse(this.txtNumLEDs.Text.ToString().Trim()))) { bret = true; }
            }
            catch (Exception ex)
            {
                MessageBox.Show("There was an error verifying LED Position Array count match...: " + ex.Message);
            }

            return bret;
        }

        /*
        */
        private bool veriLightingEffect()
        {
            //Declare variables
            String[] lEffect = { "" };
            bool bret = false;

            try
            {
                if ((lEffect[0] = this.cBoxLEffect.Text.ToString().Trim().ToUpper()).Equals(""))
                {
                    //Notify user a mcu name is required
                    MessageBox.Show("Please enter/select an Lighting Effect before trying to save...");
                    return bret;
                }
                else
                {
                    //Create MCU Name in database if necessary
                    lEffect = lEffect[0].Split('-');
                    if (!dbmanager.verifyNameExistsInDatabase("LIGHTING_EFFECTS", lEffect[0].Trim(), "", "", ""))
                    {
                        if(dbmanager.insertRecordIntoDB("LIGHTING_EFFECTS", lEffect[0].Trim(), lEffect[1].Trim(), "", "") < 1)
                        {
                            bret = false;
                        }
                        else
                        {
                            bret = true;
                        }
                    }
                    else
                    {
                        bret = true;
                    }

                    //Update textbox string
                    this.cBoxLEffect.Text = lEffect[0].Trim() + " - " + lEffect[1].Trim();
                    this.cBoxLEffect.Update();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error while verifying MCU Name...: " + ex.Message);
            }

            return bret;
        }

        /*
        */
        private int veriIntTextbox(TextBox txt)
        {
            //Declare variables
            int iret = -1;

            try
            {
                iret = int.Parse(txt.Text.ToString().Trim());
            }catch(Exception ex)
            {
                MessageBox.Show("Error verifying int in textbox...: " + ex.Message);
            }

            return iret;
        }

        #endregion Verification Methods

        /*
        */
        private void btnLEDPArray_Click(object sender, EventArgs e)
        {
            int leds = 0;

            if (this.txtNumLEDs.Text.ToString().Trim().Equals(""))
            {
                //Notify user they must enter # of leds first
                MessageBox.Show("Please enter number of leds before clicking this button...");
                return;
            }
            else
            {
                try
                {
                    leds = Convert.ToInt32(this.txtNumLEDs.Text.ToString().Trim());
                }catch(Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    return;
                }
            }

            LEDPosition ledp = new LEDPosition(leds, ledPArray);
            ledp.Owner = this;
            ledp.Show();
        }

        /*
        */
        private void btnLEDCArray_Click(object sender, EventArgs e)
        {
            int leds = 0;

            if (this.txtNumLEDs.Text.ToString().Trim().Equals(""))
            {
                //Notify user they must enter # of leds first
                MessageBox.Show("Please enter number of leds before clicking this button...");
                return;
            }
            else
            {
                try
                {
                    leds = Convert.ToInt32(this.txtNumLEDs.Text.ToString().Trim());
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    return;
                }
            }

            LEDColor ledc = new LEDColor(leds, ledCArray);
            ledc.Owner = this;
            ledc.Show();
        }

        /*
        */
        private void btnSave2Project_Click(object sender, EventArgs e)
        {
            //Declare variables
            String projectName = "", mcuName = "", ledPArray = "", ledCArray = "";
            String[] pinSetup = { "" };
            int numLEDs = 0, lightingEffect, effectNum = 0, effectStart = 0, effectDuration = 0, truePinSetupVal = 0;

            //Verify Project Name has been entered (if yes, verify if Project Name exists in database and if not, create in database)
            if (!veriProjectName()) { return; }

            //Verify MCU Name has been entered (if yes, verify if MCU Name exists in database and if not, create in database)
            if (!veriMCUName()) { return; }

            //Verify Pin Setup has been entered (if yes, verify if Pin Setup exists in database and if not, create in database)
            if (!veriPinSetup()) { return; }

            //Verify # of leds entered
            if(veriIntTextbox(this.txtNumLEDs) < 1) {
                MessageBox.Show("Please enter number of LEDs > 0 before pressing the save button...");
                return;
            }

            //Verify LED Postion Array has a count equal to # of leds
            if (!veriLEDPositionArray()) { return; }

            //Verify LED Color Array has a count equal to # of leds
            if (!veriLEDColorArray()) { return; }

            //Verify Lighting effect has been entered and in correct format (0 - ????) (if yes, verify if Lighting Effect exists in database and if not, create in database)
            if (!veriLightingEffect()) { return; }

            //Verify Effect_Start has been entered
            if (veriIntTextbox(this.txtEffectStartTime) < 0) {
                MessageBox.Show("Your entered effect start time must be at least 0...");
                return; }

            //Verify Effect_Duration has been entered
            if (veriIntTextbox(this.txtEffectDuration) < 1) {
                MessageBox.Show("Your entered effect duration must be greater than 0...");
                return;
            }

            //Create LED_Effect
            projectName = getProjectName();
            mcuName = getMCUName();
            pinSetup = getPinSetup();
            numLEDs = getNumLEDs();
            lightingEffect = getLightingEffect();
            effectStart = getEffectStart();
            effectDuration = getEffectDuration();
            ledPArray = getLEDPArray();
            ledCArray = getLEDCArray();
            if((truePinSetupVal = dbmanager.getPinSetupValue(projectName, mcuName, pinSetup)) < 0)
            {
                //Notify user that pinsetup was not created successfully
                MessageBox.Show("Your Pin Setup was not created successfully. Please try to save again...");
                return;
            }
            if((effectNum = dbmanager.insertRecordIntoDBReturnIncr("LED_EFFECT", projectName, mcuName, truePinSetupVal.ToString(), numLEDs.ToString(), lightingEffect.ToString(), effectStart.ToString(), effectDuration.ToString(), ledPArray, ledCArray)) < 0)
            {
                //Notify user effect was not created successfully
                MessageBox.Show("The effect was not created successfully.  Please try again...");
                return;
            }

            //Notify user that record was added to project successfully
            MessageBox.Show("The LED Effect record has been added to the database successfully...");

            //Reset screen values
            //setupScreenValues();
        }

        /*
        */
        private String getProjectName()
        {
            return (this.cBoxProjectName.Text.ToString().Trim().Split('-'))[0].Trim();
        }

        /*
        */
        private String getMCUName()
        {
            return this.cBoxMCUName.Text.ToString().Trim().ToUpper().Split('-')[0].Trim();
        }

        /*
        */
        private String[] getPinSetup()
        {
            return this.cBoxPinSetup.Text.ToString().Trim().Split(';');
        }
        
        /*
        */
        private int getNumLEDs()
        {
            return int.Parse(this.txtNumLEDs.Text.Trim());
        }

        /*
        */
        private int getLightingEffect()
        {
            return int.Parse(this.cBoxLEffect.Text.ToString().Trim().Split('-')[0].Trim());
        }
        
        /*
        */
        private int getEffectStart()
        {
            return int.Parse(this.txtEffectStartTime.Text.Trim());
        }
        
        /*
        */
        private int getEffectDuration()
        {
            return int.Parse(this.txtEffectDuration.Text.ToString());
        }

        /*
        */
        private String getLEDPArray()
        {
            String sret = "";

            for(int i = 0; i < ledPArray.Count; i++)
            {
                if(i == (ledPArray.Count - 1))
                {
                    sret += ledPArray[i].ToString().Trim();
                }
                else
                {
                    sret += ledPArray[i].ToString().Trim() + ";";
                }
            }

            return sret;
        }

        /*
        */
        private String getLEDCArray()
        {
            String sret = "";

            for (int i = 0; i < ledCArray.Count; i++)
            {
                if (i == (ledCArray.Count - 1))
                {
                    sret += ledCArray[i].ToString().Trim();
                }
                else
                {
                    sret += ledCArray[i].ToString().Trim() + ";";
                }
            }

            return sret;
        }

        /*
        */
        private void setupScreenValues()
        {
            //Reset cbox values
            this.cBoxProjectName.SelectedIndex = -1;
            this.cBoxProjectName.Text = "Project Name - Description";
            this.cBoxProjectName.Update();
            this.cBoxMCUName.SelectedIndex = -1;
            this.cBoxMCUName.Text = "MCU Name - Description";
            this.cBoxMCUName.Update();
            this.cBoxPinSetup.SelectedIndex = -1;
            this.cBoxPinSetup.Text = "Pin#1;Pin#2;Description";
            this.cBoxPinSetup.Update();
            this.cBoxLEffect.SelectedIndex = -1;
            this.cBoxLEffect.Text = "Effect# - Description";
            this.cBoxLEffect.Update();

            //Reset textbox values
            this.txtNumLEDs.Text = "2";
            this.txtNumLEDs.Update();
            this.txtEffectStartTime.Text = "0";
            this.txtEffectStartTime.Update();
            this.txtEffectDuration.Text = "0";
            this.txtEffectDuration.Update();
            this.txtSongPath.Text = "";
            this.txtSongPath.Update();

            //Reset arrays
            ledPArray.Clear();
            ledCArray.Clear();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            //Close program
            this.Close();
        }

        private void cBoxProjectName_DropDownClosed(object sender, EventArgs e)
        {

        }

        private void cBoxMCUName_DropDownClosed(object sender, EventArgs e)
        {

        }

        private void cBoxPinSetup_DropDownClosed(object sender, EventArgs e)
        {

        }

        private void cBoxLEffect_DropDownClosed(object sender, EventArgs e)
        {

        }
    }
}
