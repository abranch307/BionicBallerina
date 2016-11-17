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
        private MusicManager musicmanager;
        private DatabaseManager databasemanager;
        private DrawingManager composerDrawManager;
        private System.Windows.Forms.Timer screenRefreshTimer;
        private Dictionary<String, bool> foundIPAddresses = new Dictionary<string, bool>();

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
            musicmanager = new MusicManager(this, this.WMPlayer, this.btnLoadSong, this.btnJump2Secs, this.txtTimer, this.pnlTBar, this.lblSongName);
            databasemanager = new DatabaseManager(this,this.lblProjectName, this.btnAdd2Project, this.btnEditRecord, this.btnSend2SDCard, this.btnSend2SDCard, this.btnOpenProject, this.dgvProjectData);
            composerDrawManager = new DrawingManager();
            EffectsManager.Init(databasemanager, composerDrawManager);

            //Initialize screen refresh timer
            screenRefreshTimer = new System.Windows.Forms.Timer();
            screenRefreshTimer.Enabled = true;
            screenRefreshTimer.Interval = 100; /* 100 millisec */
            screenRefreshTimer.Tick += new EventHandler(TimerCallback);
        }
        
        /*
            Function: startTicker
            This function will start a new thread which will increment "Timer - Secs" textbox on main
            "LED Lighting Composer" screen.  This will happen only while media player's "is playing" is true

            Parameters: none

            Returns: nothing
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
        */
        public void mcuIPSetupAndWait()
        {
            //Declare variables
            List<String> ipAddresses = new List<string>();

            //Skip getting new ip addresses if foundIPAddresses is not empty
            if (foundIPAddresses == null || foundIPAddresses.Count <= 0)
            {
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
            }

            try
            {
                //Show screen for user IP address selection
                ScreenArraySelections ledp = new ScreenArraySelections("IP", foundIPAddresses.Count, null, null, foundIPAddresses);
                ledp.Owner = this;
                var diagResult = ledp.ShowDialog();
                
            }
            catch (Exception ex)
            {
                
            }
        }

        #region Private Methods

        /*
            Function: TimerCallback
            Invalidates Windows Form so form can be redrawn (mainly for LED redrawing).  Also
            calls LEDStripEffect's updateLEDEffects function from Drawing Manager if isPlaying 
            is true for MusicManager class.

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

        /*
        */
        private void updateLabelThreadProc()
        {
            while (musicmanager.isPlaying)
            {
                try
                {
                    this.BeginInvoke(new MethodInvoker(UpdateLabel));
                }catch(Exception ex)
                {

                }
                System.Threading.Thread.Sleep(1000);
            }
        }

        private void UpdateLabel()
        {
            musicmanager.UpdateLabel();
        }

        #endregion Private Methods

        private void btnLoadSong_Click(object sender, EventArgs e)
        {
            //Allow the MusicManager class to handle this event
            musicmanager.btnLoadSong_Click(sender, e);
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            //Close program
            this.Close();
        }

        private void btnSend2LocalFile_Click(object sender, EventArgs e)
        {
            //Allow the DatabaseManager class handle this event
            databasemanager.btnSend2LocalFile_Click(sender, e);
        }

        private void btnSend2SDCard_Click(object sender, EventArgs e)
        {
            //Allow the DatabaseManager class to handle this event
            databasemanager.btnSend2SDCard_Click(sender, e);
        }

        private void btnSendViaHTTP_Click(object sender, EventArgs e)
        {
            //Allow the DatabaseManager class to handle this event
            databasemanager.btnSendViaHTTP_Click(sender, e);
        }

        private void btnEditRecord_Click(object sender, EventArgs e)
        {
            //Allow the DatabaseManager class to handle this event
            databasemanager.btnEditRecord_Click(sender, e);
        }

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

        private void btnMCUIPSetup_Click(object sender, EventArgs e)
        {
            //Declare variables
            List<String> ipAddresses = new List<string>();

            //Skip getting new ip addresses if foundIPAddresses is not empty
            if (foundIPAddresses == null || foundIPAddresses.Count <= 0)
            {
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
                }catch(Exception ex)
                {

                }
            }

            try
            {
                //Show screen for user IP address selection
                ScreenArraySelections ledp = new ScreenArraySelections("IP", foundIPAddresses.Count, null, null, foundIPAddresses);
                ledp.Owner = this;

                while (ledp.ShowDialog() == DialogResult.Retry)
                {
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

                    ledp = new ScreenArraySelections("IP", foundIPAddresses.Count, null, null, foundIPAddresses);
                    ledp.Owner = this;
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show("No IP Address were found or another issue occurred: " + ex.Message);
            }
        }

        private void btnJump2Secs_Click(object sender, EventArgs e)
        {
            //Allow the MusicManager class to handle this event
            musicmanager.btnJump2Secs_Click(sender, e);
        }

        private void WMPlayer_PositionChange(object sender, AxWMPLib._WMPOCXEvents_PositionChangeEvent e)
        {
            //Allow the MusicManager class to handle this event
            musicmanager.WMPlayer_PositionChange(sender, e);
        }

        private void WMPlayer_PlayStateChange(object sender, AxWMPLib._WMPOCXEvents_PlayStateChangeEvent e)
        {
            //Allow the MusicManager class to handle this event
            musicmanager.WMPlayer_PlayStateChange(sender, e);
        }

        private void dgvProjectData_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            //Allow the DatabaseManager class to handle this event
            databasemanager.dgvProjectData_CellDoubleClick(sender, e);
        }

        private void btnClearGrid_Click(object sender, EventArgs e)
        {
            //Allow the DatabaseManager class to handle this event
            databasemanager.btnClearGrid_Click(sender, e, composerDrawManager);
        }

        private void btnOpenProject_Click(object sender, EventArgs e)
        {
            //Allow the DatabaseManager class to handle this event
            databasemanager.btnOpenProject_Click(sender, e, this, composerDrawManager);
        }

        private void LEDLightingComposer_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.composerDrawManager = null;
        }

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
