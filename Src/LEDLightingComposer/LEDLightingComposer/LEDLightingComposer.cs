/*
	Author: Aaron Branch, Zach Jarmon, Peter Martinez
	Created: 
	Last Modified:
    Program Description:
        This program allows a user to take pre-configured lighting effects, compose them for multiple led strips
        across multiple microcontrollers, save composed effects as a project, export composed effects to a txt file 
        which can be compiled into microcontrollers later.  This program also allows the user to select multiple
        ip addresses to mutliple WiFi modules and send commands to synchronize performances with music running
        through this program's Windows Media Player component
	Class: LEDLightingComposer.cs
	Class Description:
		This class initializes the components for the main LEDLighting Composer windows form, as well as all used
        classes in the program.  Different screen events are passed to specific classes to handle if relevant to
        that class, otherwise the screen even is handled by this class.
*/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LEDLightingComposer
{
    public partial class LEDLightingComposerCS : Form
    {
        //Declare global variables
        private MusicManager musicmanager; //Class that manages the AXWindowsMediaPlayer object and synchronizing WiFi modules with music
        private DatabaseManager databasemanager; //Class that manages saving, updating, and deleting projects in the MariaDB database
        private DrawingManager composerDrawManager; //Class that manages the drawing of objects onto the panel on screen
        private System.Windows.Forms.Timer screenRefreshTimer; //Controls the updating of drawing elements and timer label on screen through its own thread
        private Dictionary<String, bool> foundIPAddresses = new Dictionary<string, bool>(); //Holds found ip addresses from pings found through HTTPSRequestResponse class methods
        private WaitDialog waitDialogBox; //Global wait dialog box used throughout this class

        /*
            Default constructor
                Initializes helper classes, anchors screen components, and sets screen defaults
        */
        public LEDLightingComposerCS()
        {
            InitializeComponent();

            //Anchor components
            this.lblAudioControls.Anchor = (AnchorStyles.Top | AnchorStyles.Right);
            this.chkSynchronizeMCUs.Anchor = (AnchorStyles.Top | AnchorStyles.Right);
            this.chkSkipIPSetup.Anchor = (AnchorStyles.Top | AnchorStyles.Right);
            this.btnMCUIPSetup.Anchor = (AnchorStyles.Top | AnchorStyles.Right);
            this.lblSongName.Anchor = (AnchorStyles.Top | AnchorStyles.Right);
            this.WMPlayer.Anchor = (AnchorStyles.Top | AnchorStyles.Right);
            this.lblTimer.Anchor = (AnchorStyles.Top | AnchorStyles.Right);
            this.txtTimer.Anchor = (AnchorStyles.Top | AnchorStyles.Right);
            this.btnJump2Secs.Anchor = (AnchorStyles.Top | AnchorStyles.Right);
            this.chkPlayerDelayTime.Anchor = (AnchorStyles.Top | AnchorStyles.Right);
            this.txtPlayerDelayTime.Anchor = (AnchorStyles.Top | AnchorStyles.Right);
            this.btnLoadSong.Anchor = (AnchorStyles.Top | AnchorStyles.Right);
            this.pnlDraw.Anchor = (AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right);

            this.lblProjectDatabase.Anchor = (AnchorStyles.Bottom| AnchorStyles.Left);
            this.lblProjectName.Anchor = (AnchorStyles.Bottom | AnchorStyles.Left);
            this.btnOpenProject.Anchor = (AnchorStyles.Bottom| AnchorStyles.Left);
            this.btnAdd2Project.Anchor = (AnchorStyles.Bottom| AnchorStyles.Left);
            this.btnEditRecord.Anchor = (AnchorStyles.Bottom | AnchorStyles.Left);
            this.btnClearGrid.Anchor = (AnchorStyles.Bottom | AnchorStyles.Left);
            this.pnlTBar.Anchor = (AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right);
            this.dgvProjectData.Anchor = (AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right);
            this.btnSend2LocalFile.Anchor = (AnchorStyles.Bottom | AnchorStyles.Left);
            this.btnSendViaHTTP.Anchor = (AnchorStyles.Bottom | AnchorStyles.Left);
            this.btnSend2SDCard.Anchor = (AnchorStyles.Bottom | AnchorStyles.Left);
            this.btnExit.Anchor = (AnchorStyles.Bottom| AnchorStyles.Left);

            //Initialize classes to handle certain form functions
            musicmanager = new MusicManager(this, this.WMPlayer, this.btnLoadSong, this.btnJump2Secs, this.txtTimer, this.pnlTBar, this.lblSongName, this.chkPlayerDelayTime, this.txtPlayerDelayTime);
            databasemanager = new DatabaseManager(this,this.lblProjectName, this.btnAdd2Project, this.btnEditRecord, this.btnSend2SDCard, this.btnSend2SDCard, this.btnOpenProject, this.dgvProjectData);
            composerDrawManager = new DrawingManager();
            EffectsManager.Init(databasemanager, composerDrawManager);

            //Initialize screen refresh timer
            screenRefreshTimer = new System.Windows.Forms.Timer();
            screenRefreshTimer.Enabled = true;
            screenRefreshTimer.Interval = 100; /* 100 millisec */
            screenRefreshTimer.Tick += new EventHandler(TimerCallback);
        }

        #region Public Methods

        /*
            Function: startTicker
                This function will start a new thread which will increment "Timer - Secs" textbox on main
                "LED Lighting Composer" screen by calling the updateLabelThreadProc method.  This will happen 
                only while media player's "isPlaying" variable is true which also lends to closing the thread 
                when the "isPlaying" variable is set to false

            Parameters: none

            Returns: Thread - a started thread that runs the updateLabelThreadProc method
        */
        public Thread startTicker()
        {
            //Update strips to current performance time
            musicmanager.updateMusicPlayerAndTimerText();

            //Start a ticker in new thread which will update timer textbox every second when media player is playing
            Thread t = new Thread(new ThreadStart(updateLabelThreadProc));
            t.Start();
            return t;
        }

        /*
            Function: mcuIPSetupAndWait
                Pings all ip addresses on computer's/device's and displays them, along with
                their hostname if available, in a new screen for user to select WiFi modules
                with which to synchronize Music from Windows Media Player (AxWindowsMediaPlayer)
            
            Parameters: None

            Returns: List<String> - ip address returned from pinging network or ip addresses that were remembered
                from user's previous selection (If user has selected ip addresses, no ping will be done so user
                doesn't have to re-select their ip addresses.  User has to refresh to get new ip addresses)
        */
        public List<String> mcuIPSetupAndWait()
        {
            //Declare variables
            List<String> ipAddresses = new List<string>();

            //Skip getting new ip addresses if foundIPAddresses is not empty
            if (foundIPAddresses == null || foundIPAddresses.Count <= 0)
            {
                //Invoke wait dialog
                waitDialogBox = new WaitDialog();
                waitDialogBox.Show();

                //Create new dictionary
                foundIPAddresses = new Dictionary<string, bool>();

                //Get IP Addresses on network
                ipAddresses = HttpRequestResponse.getAllIPAddressesOnNetwork();

                try
                {
                    //Add to dictionary
                    foreach (String str in ipAddresses)
                    {
                        foundIPAddresses.Add(str, false);
                    }
                }
                catch (Exception ex)
                {

                }

                //Close wait dialog
                waitDialogBox.Close();
                waitDialogBox = null;
            }

            try
            {
                //Show screen for user IP address selection
                ScreenArraySelections ledp = new ScreenArraySelections("IP", foundIPAddresses.Count, null, null, foundIPAddresses);
                ledp.Owner = this;

                while (ledp.ShowDialog() == DialogResult.Retry)
                {
                    //Invoke wait dialog
                    waitDialogBox = new WaitDialog();
                    waitDialogBox.Show();

                    //Create new dictionary
                    foundIPAddresses = new Dictionary<string, bool>();

                    //Get IP Addresses on network
                    ipAddresses = HttpRequestResponse.getAllIPAddressesOnNetwork();

                    //Add to dictionary
                    foreach (String str in ipAddresses)
                    {
                        try
                        {
                            //If this ip address contains the ESP8266 prefix or a blank name (could be no dns avaiable), then add to list
                            //if (name.Trim().ToUpper().Contains(ESP8266DNSPREFIX) || name.Trim().Equals(""))
                            //{
                            foundIPAddresses.Add(str, false);
                            //}
                        }
                        catch (Exception ex)
                        {

                        }
                    }

                    ledp = new ScreenArraySelections("IP", foundIPAddresses.Count, null, null, foundIPAddresses);
                    ledp.Owner = this;

                    //Close wait dialog
                    waitDialogBox.Close();
                    waitDialogBox = null;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("No IP Address were found or another issue occurred: " + ex.Message);
            }

            return ipAddresses;
        }

        #endregion Public Methods

        #region Private Methods       

        /*
            Function: updateLabelThreadProc
                Spawns a new thread that in turn calls the updateLabel function and sleeps
                for 1 second
            Parameters: None

            Returns: Nothing
        */
        private void updateLabelThreadProc()
        {
            while (musicmanager.isPlaying)
            {
                try
                {
                    this.BeginInvoke(new MethodInvoker(updateLabel));
                }
                catch (Exception ex)
                {

                }
                System.Threading.Thread.Sleep(1000);
            }
        }

        /*
            Function: updateLabel
                Calls MusicManager class's update label to update timer textbox to match
                Windows Media Player track time
            
            Parameter: None

            Returns: Nothing
        */
        private void updateLabel()
        {
            musicmanager.updateLabel();
        }

        /*
            Function: TimerCallback
                Invalidates Windows Form so form can be redrawn (mainly for LED redrawing).  Also
                calls LEDStripEffect's updateLEDEffects function from Drawing Manager if isPlaying 
                is true for MusicManager class (meaning Windows Media Player is playing).

            Parameters: object & eventargs

            Returns: nothing
        */
        private void TimerCallback(object sender, EventArgs e)
        {
            //Invalidate panel here?

            //Call LEDStripEffect's updateLEDEffects function from DrawingManager if musicmanager's isPlaying is true
            if (musicmanager.isPlaying && !musicmanager.settingUp)
            {
                //Invalidate Windows Form so screen can be redrawn
                this.Invalidate();

                //Update Effects Manager and its strips performance time
                //EffectsManager.updatePerformance(EffectsManager.getElapsedTime(DateTime.Now.Millisecond));
                EffectsManager.updatePerformance(EffectsManager.getElapsedTime());
            }
            return;
        }

        #endregion Private Methods

        #region Screen Events

        /*
            Function: btnMCUIPSetup_Click
                Calls mcuIPSetupAndWait method

            Parameters: object & eventargs

            Returns: Nothing
        */
        private void btnMCUIPSetup_Click(object sender, EventArgs e)
        {
            //Call mcuIPSetupAndWait method
            mcuIPSetupAndWait();
        }

        #region DatabaseManager Class Handles

        /*
            Function: btnSend2LocalFile
                Handled in DatabaseManager class
            
            Parameters: object and eventargs

            Returns: Nothing
        */
        private void btnSend2LocalFile_Click(object sender, EventArgs e)
        {
            //Allow the DatabaseManager class handle this event
            databasemanager.btnSend2LocalFile_Click(sender, e);
        }

        /*
            Function: btnSend2SDCard_Click
                Handled in DatabaseManager class
            
            Parameters: object and eventargs

            Returns: Nothing
        */
        private void btnSend2SDCard_Click(object sender, EventArgs e)
        {
            //Allow the DatabaseManager class to handle this event
            databasemanager.btnSend2SDCard_Click(sender, e);
        }

        /*
            Function: btnSendViaHTTP_Click
                Handled in DatabaseManager class
            
            Parameters: object and eventargs

            Returns: Nothing
        */
        private void btnSendViaHTTP_Click(object sender, EventArgs e)
        {
            //Allow the DatabaseManager class to handle this event
            databasemanager.btnSendViaHTTP_Click(sender, e);
        }

        /*
            Function: btnEditRecord_Click
                Handled in DatabaseManager class
            
            Parameters: object and eventargs

            Returns: Nothing
        */
        private void btnEditRecord_Click(object sender, EventArgs e)
        {
            //Allow the DatabaseManager class to handle this event
            databasemanager.btnEditRecord_Click(sender, e);
        }

        /*
            Function: btnAdd2Project_Click
                Handled in DatabaseManager class
            
            Parameters: object and eventargs

            Returns: Nothing
        */
        private void btnAdd2Project_Click(object sender, EventArgs e)
        {
            //Allow the DatabaseManager class to handle this event
            int i = 0;
            if (!musicmanager.timer.Text.ToString().Trim().Equals(""))
            {
                try
                {
                    i = int.Parse(musicmanager.timer.Text.ToString().Trim());
                }catch(Exception ex)
                {
                    try
                    {
                        i = int.Parse(musicmanager.timer.Text.ToString().Trim().Split(':')[1]);
                    }
                    catch(Exception ex2)
                    {

                    }
                }
            }
            databasemanager.btnAdd2Project_Click(sender, e, this, i, musicmanager.CurrentSongFilePath);
        }

        /*
            Function: dgvProjectData_CellDoubleClick
                Handled in DatabaseManager class
            
            Parameters: object and eventargs

            Returns: Nothing
        */
        private void dgvProjectData_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            //Allow the DatabaseManager class to handle this event
            databasemanager.dgvProjectData_CellDoubleClick(sender, e);
        }

        /*
            Function: btnClearGrid_Click
                Handled in DatabaseManager class
            
            Parameters: object and eventargs

            Returns: Nothing
        */
        private void btnClearGrid_Click(object sender, EventArgs e)
        {
            //Allow the DatabaseManager class to handle this event
            databasemanager.btnClearGrid_Click(sender, e, composerDrawManager);

            //Invalid screen to clear drawing in panel
            this.Invalidate();
        }

        /*
            Function: btnOpenProject_Click
                Handled in DatabaseManager class
            
            Parameters: object and eventargs

            Returns: Nothing
        */
        private void btnOpenProject_Click(object sender, EventArgs e)
        {
            //Allow the DatabaseManager class to handle this event
            databasemanager.btnOpenProject_Click(sender, e, this, composerDrawManager);
        }

        #endregion DatabaseManager Class Handles


        #region MusicManager Class Handles

        /*
            Function: btnLoadSong_Click
                Handled in MusicManager class
            
            Parameters: object and eventargs

            Returns: Nothing
        */
        private void btnLoadSong_Click(object sender, EventArgs e)
        {
            //Allow the MusicManager class to handle this event
            musicmanager.btnLoadSong_Click(sender, e);
        }

        /*
            Function: btnJump2Secs_Click
                Handled in MusicManager class
            
            Parameters: object and eventargs

            Returns: Nothing
        */
        private void btnJump2Secs_Click(object sender, EventArgs e)
        {
            //Allow the MusicManager class to handle this event
            musicmanager.btnJump2Secs_Click(sender, e);
        }

        /*
            Function: chkPlayerDelayTime_CheckedChanged
                Handled in MusicManager class
            
            Parameters: object and eventargs

            Returns: Nothing
        */
        private void chkPlayerDelayTime_CheckedChanged(object sender, EventArgs e)
        {
            //Allow the MusicManager class to handle this event
            musicmanager.chkPlayerDelayTime_CheckedChanged(sender, e);
        }

        /*
            Function: WMPlayer_PositionChange
                Handled in MusicManager class
            
            Parameters: object and eventargs

            Returns: Nothing
        */
        private void WMPlayer_PositionChange(object sender, AxWMPLib._WMPOCXEvents_PositionChangeEvent e)
        {
            //Allow the MusicManager class to handle this event
            musicmanager.WMPlayer_PositionChange(sender, e);
        }

        /*
            Function: WMPlayer_PlayStateChange
                Handled in MusicManager class
            
            Parameters: object and eventargs

            Returns: Nothing
        */
        private void WMPlayer_PlayStateChange(object sender, AxWMPLib._WMPOCXEvents_PlayStateChangeEvent e)
        {
            //Allow the MusicManager class to handle this event
            musicmanager.WMPlayer_PlayStateChange(sender, e);
        }

        #endregion MusicManager Class Handles

        /*
            Function: OnPaint
                Calls form's native paint function along with DrawingManager's "draw" function to
                draw special components on top of base screen and within designated panel on screen

            Parameters: painteventargs

            Returns: Nothing
        */
        protected override void OnPaint(PaintEventArgs e)
        {
            //Draw the form's native elements
            base.OnPaint(e);

            //Draw items from DrawingManager
            try {
                //composerDrawManager.draw(e.Graphics, getDrawingBottom(), getDrawingRight());
                composerDrawManager.draw(this.pnlDraw.CreateGraphics(), getDrawingBottom(), getDrawingRight());
            }
            catch(Exception ex)
            {

            }
        }

        /*
            Function: LEDLightingComposerCS_Resize
                Redraw elements on screen to match new panel size if music is not playing

            Parameters: object & eventargs

            Returns: Nothing
        */
        private void LEDLightingComposerCS_Resize(object sender, EventArgs e)
        {
            //Redraw LEDs on screen if necessary
            if (!musicmanager.isPlaying)
            {
                this.Invalidate();
                databasemanager.updateProjectsInProjectGrid(this.lblProjectName.Text.ToString().Trim());
            }
        }

        /*
            Function: btnExit_Click
                Exit program
        */
        private void btnExit_Click(object sender, EventArgs e)
        {
            //Close program
            this.Close();
        }

        #endregion Screen Events

        #region Getters & Setters

        public int getDrawingBottom()
        {
            return this.btnClearGrid.Top;
        }

        public int getDrawingRight()
        {
            return this.btnJump2Secs.Left;
        }

        public bool getSyncMCUsChecked()
        {
            bool bret = false;

            bret = this.chkSynchronizeMCUs.Checked;

            return bret;
        }

        public bool getSkipIPSetupChecked()
        {
            bool bret = false;

            bret = this.chkSkipIPSetup.Checked;

            return bret;
        }

        public List<String> getSelectedIPAddresses()
        {
            List<String> sret = new List<string>();

            if(foundIPAddresses == null)
            {
                return null;
            }

            for(int i = 0; i < foundIPAddresses.Count; i++)
            {
                if (foundIPAddresses.ElementAt(i).Value)
                {
                    sret.Add(foundIPAddresses.ElementAt(i).Key.ToString().Split(';')[1].Trim());
                }
            }

            return sret;
        }

        public MusicManager MManager
        {
            get { return this.musicmanager; }
        }

        public DatabaseManager DataManager
        {
            get { return this.databasemanager; }
        }

        public DrawingManager DrawManager
        {
            get { return this.composerDrawManager; }
        }

        public Dictionary<string, bool> FoundIPAddresses
        {
            get
            {
                return foundIPAddresses;
            }

            set
            {
                foundIPAddresses = value;
            }
        }

        #endregion Getters & Setters
    }
}
