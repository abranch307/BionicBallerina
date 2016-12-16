/*
	Author: Aaron Branch, Zach Jarmon, Peter Martinez
	Created: 
	Last Modified:
	Class: MusicManager.cs
	Class Description:
		This class handles the Windows Media Player (AxWindowsMediaPlayer) component on screen and
        events surrounding the component including song loading, playing state changes, and synchronize
        selected WiFi modules when state changes to Play, Stop, and Pause if synchronization option is
        selected

*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Media;
using AxWMPLib;
using System.Threading;

namespace LEDLightingComposer
{
    public class MusicManager
    {
        //Declare global variables
        private LEDLightingComposerCS llc;
        private SoundPlayer player;
        private Button loadSong, jump2Secs;
        private Label songName;
        private WaitDialog waitDialogBox;
        public AxWindowsMediaPlayer player2;
        public Panel TrackBarPanel;
        public TrackBar trackBar;
        public CheckBox chkPlayerDelayTime;
        public TextBox timer, playerDelayTime;
        public String currentSongFilePath;
        public bool isPlaying, settingUp;

        /*
            Default Constructor
                Sets up global variables including passed screen elements and initialize
                Windows Media Player component

            Parameters: Classes, Media Player object, and screen elements
        */
        public MusicManager(LEDLightingComposerCS LLC, AxWindowsMediaPlayer Player2, Button LoadSong, Button Jump2Secs, TextBox Timer, Panel TrackBarPanel, Label SongName, CheckBox ChkPlayerDelayTime, TextBox PlayerDelayTime)
        {
            //Set class variables to passed
            this.llc = LLC;
            this.player2 = Player2;
            this.loadSong = LoadSong;
            this.jump2Secs = Jump2Secs;
            this.timer = Timer;
            this.chkPlayerDelayTime = ChkPlayerDelayTime;
            this.playerDelayTime = PlayerDelayTime;
            this.TrackBarPanel = TrackBarPanel;
            this.songName = SongName;
            this.currentSongFilePath = "";
            this.IsPlaying = false;
            this.settingUp = false;

            //Find trackBar txt in TrackBarPanel
            foreach(Control trkBar in TrackBarPanel.Controls)
            {
                if(trkBar is TrackBar)
                {
                    this.trackBar = (TrackBar)trkBar;
                    this.trackBar.Minimum = 0;
                    break;
                }
            }

            //Initialize sound player
            player = new SoundPlayer();
        }

        #region Public Methods

        /*
            Function: updateLabel
                Updates timer textbox onscreen to match windows media player's current position/time
            
            Parameters: Nothing

            Returns: int - value of media player's current position/time in seconds
        */
        public int updateLabel()
        {
            int iret = 0;

            try
            {
                timer.Text = Convert.ToInt32(player2.Ctlcontrols.currentPosition).ToString();
                timer.Update();
                iret = (this.trackBar.Value = (int)this.player2.Ctlcontrols.currentPosition);
            }
            catch(Exception ex)
            {

            }

            return iret;
        }

        /*
            Function: updateMusicPlayerAndTimerText
                Updates timer textbox time to match Windows Media Player's time and updates EffectsManager
                perforamnce which in turn updates led strips current lighting effect sequences

            Parameters: Nothing

            Returns: bool - true or false depending on if method runs and does not encounter an
            Exception
        */
        public bool updateMusicPlayerAndTimerText()
        {
            //Declare variables
            long jump2Secs = 0;
            double currentPosition = 0;
            String currentPositionString = "";
            bool bret = false;

            try
            {
                //Update timer text
                //currentPositionString = this.player2.Ctlcontrols.currentPosition.ToString();
                this.timer.Text = this.player2.Ctlcontrols.currentPositionString;
                //this.timer.Text = convertTimeToSeconds(currentPositionString);
                this.timer.Update();

                //Update player position
                currentPosition = double.Parse(convertTimeToSeconds(this.timer.Text.ToString().Trim()));
                //player2.Ctlcontrols.currentPosition = currentPosition;

                //Convert seconds to milliseconds for performance time
                jump2Secs = (long)Math.Floor(currentPosition * 1000);

                //Jump EffectsManager to current performance time
                EffectsManager.findCurrentSeqFromPerformanceTime(jump2Secs);

                bret = true;
            }
            catch (Exception ex) { }

            return bret;
        }

        /*
            Function: convertTimeToSeconds
                Time from Windows Media Player usually comes in this format "00:45".  This method
                formats the WMP format to show the time as seconds like the format .75

            Parameters: String Time2Convert - string representation of time format that needs
                to be converted

            Returns: String - time converted into seconds as a float number
        */
        public String convertTimeToSeconds(String Time2Convert)
        {
            //Declare variables
            String sRet = "";
            String[] temp = { "" };

            //Split ??:?? into hour, min, second
            if (Time2Convert.Trim().Equals(""))
            {
                temp = ("00:00").Split(':');
            }
            else if (!Time2Convert.Contains(":"))
            {
                sRet = Time2Convert;
                return sRet;
            }
            else
            {
                temp = Time2Convert.Split(':');
            }

            if (temp.Length > 2)
            {
                //Account for hours
                sRet = Convert.ToUInt32(((float.Parse(temp[0]) * 3600) + (float.Parse(temp[1]) * 60) + float.Parse(temp[2]))).ToString();
            }
            else
            {
                //Only minutes and seconds
                sRet = Convert.ToDouble(((float.Parse(temp[0]) * 60) + float.Parse(temp[1]))).ToString();
            }

            return sRet;
        }

        #endregion Public Methods


        #region Private Methods


        #endregion Private Methods


        #region Screen Events

        /*
            Function btnLoadSong_Click:
                This function allows a user to choose a song from Windows Explorer
                and loads the selected song into the Windows Media Player component.
                The song plays right away...
            
            Parameters: object & eventargs

            Returns: Nothing
        */
        public void btnLoadSong_Click(object sender, EventArgs e)
        {
            // Create an instance of the open file dialog box.
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            // Set filter options and filter index.
            openFileDialog1.Filter = "MP3 (.mp3)|*.mp3|WAV (.wav)|*.wav|All Files (*.*)|*.*";
            openFileDialog1.FilterIndex = 1;

            openFileDialog1.Multiselect = false;

            // Call the ShowDialog method to show the dialog box and process input if the user clicked OK.
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                // Open the selected file for playing
                try
                {
                    //Play media with Windows Media Player
                    currentSongFilePath = openFileDialog1.FileName;
                    this.player2.URL = currentSongFilePath;

                    //Change songname
                    this.songName.Text = (currentSongFilePath.Split('\\'))[(currentSongFilePath.Split('\\')).Length-1];
                    this.songName.Update();

                    //Setup performance
                    this.timer.Text = "0";
                    this.timer.Update();

                    btnJump2Secs_Click(null, null);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
                }
            }
        }

        /*
            Function: btnJump2Secs_Click
                After the user enters a time (seconds) into the timer textbox and
                presses the Jump 2 Secs button, this method will be fired and update
                the Windows Media Player's current position to match user's entered
                time.  The EffectsManager class is also called to update led strip's
                performance to match updated time.

            Parameters: object & eventargs

            Returns: Nothing
        */
        public void btnJump2Secs_Click(object sender, EventArgs e)
        {
            //Declare variables
            double jump2Secs = 0;

            try {
                try
                {
                    //Jump Windows Media Player object to position in timer textbox (in seconds)
                    jump2Secs = double.Parse(this.timer.Text.ToString().Trim());
                }catch(Exception ex)
                {
                    jump2Secs = double.Parse(convertTimeToSeconds(this.timer.Text.ToString().Trim()));
                }

                this.player2.Ctlcontrols.currentPosition = jump2Secs;

                try
                {
                    //Set trackBar
                    this.trackBar.Value = (int)this.player2.Ctlcontrols.currentPosition;
                }catch(Exception ex)
                {
                    //Set trackBar
                    this.trackBar.Value = 0;
                }

                //Convert seconds to milliseconds for performance time
                jump2Secs = (long)(Math.Floor(jump2Secs * 1000));

                //Invoke wait dialog
                waitDialogBox = new WaitDialog();
                waitDialogBox.Show();

                //Disable jump2secs button
                this.jump2Secs.Enabled = false;

                //Jump EffectsManager to current performance time
                EffectsManager.findCurrentSeqFromPerformanceTime((long)jump2Secs);

                //Close wait dialog
                waitDialogBox.Close();
                waitDialogBox = null;

                //Enable jump2secs button
                this.jump2Secs.Enabled = true;

                //Invalidate LLC screen so redraw can happen
                llc.Invalidate();
            }catch(Exception ex)
            {
                MessageBox.Show("Error seeking to position: " + ex.Message);
            }
        }

        /*
            Function: chkPlayerDelayTime_CheckedChanged
                Enables or disables the playerDelayTime textbox depending on if the
                checkbox was just checked or unchecked

            Parameters: object, eventargs

            Returns: Nothing
        */
        public void chkPlayerDelayTime_CheckedChanged(object sender, EventArgs e)
        {
            //Disable or enable playerDelayTime textbox depending on checkbox value
            if (this.chkPlayerDelayTime.Checked)
            {
                this.playerDelayTime.Enabled = true;
            }
            else
            {
                this.playerDelayTime.Enabled = false;
            }
        }

        /*
            Function: WMPlayer_PositionChange
                Updates the timer textbox to match Windows Media Player's currentPosition time
                as the player's currentPosition time changes

            Parameters: object & AxWMPLib._WMPOCXEvents_PositionChangeEvent

            Returns: Nothing
        */
        public void WMPlayer_PositionChange(object sender, AxWMPLib._WMPOCXEvents_PositionChangeEvent e)
        {
            this.timer.Text = this.player2.Ctlcontrols.currentPositionString;
            this.timer.Update();
            this.trackBar.Value = (int)this.player2.Ctlcontrols.currentPosition;
        }

        /*
            Function: WMPlayer_PlayStateChange
                Method fires when the WMP changes its play state mainly between Play, Pause, and
                Stop. This method will update EffectManager's peformance time, allow user to select
                WiFi module ipaddresses to synchronize music with, and send https 
                requests to synchronize WiFi modules/ProTrinket performance with playing music (Starts,
                Stops, or Pauses) performance running on ProTrinket & LED Strips

            Parameters: object & AxWMPLib._WMPOCXEvents_PlayStateChangeEvent

            Returns: Nothing
        */
        public void WMPlayer_PlayStateChange(object sender, AxWMPLib._WMPOCXEvents_PlayStateChangeEvent e)
        {
            //Exit if settingUp is true (prevents infinite loop when pausing and playing during MCU sync after play is pressed)
            if (settingUp)
            {
                return;
            }

            //Declare variables
            AxWindowsMediaPlayer amp = (sender as AxWindowsMediaPlayer);

            //Set setting up to true so music thread will not run until setup in case statement below finishes
            settingUp = true;

            // Test the current state of the player and display a message for each state.
            switch (e.newState)
            {
                case 0:    // Undefined
                    //this.timer.Text = "Undefined";
                    //this.timer.Update();
                    this.isPlaying = false;
                    break;
                case 1:    // Stopped
                    //this.timer.Text = "Stopped";
                    //this.timer.Update();

                    //Reset time onscreen
                    this.timer.Text = "0";
                    this.timer.Update();

                    //Update music player time and timer text
                    //updateMusicPlayerAndTimerText();

                    this.isPlaying = false;

                    //Stop performance on mcus if Synchronize MCUs is checked
                    if (llc.getSyncMCUsChecked())
                    {
                        //Disable windows media player controls
                        amp.Ctlenabled = false;

                        try
                        {
                            //Verify if user needs to setup found IP addresses first
                            if (!llc.getSkipIPSetupChecked())
                            {
                                //Enable windows media player controls
                                amp.Ctlenabled = true;
                                settingUp = false;
                                return;
                            }
                            else if (llc.getSelectedIPAddresses() == null || llc.getSelectedIPAddresses().Count <= 0)
                            {
                                //Enable windows media player controls
                                amp.Ctlenabled = true;
                                settingUp = false;
                                return;
                            }

                            //Send stop signal and Synchronize MCUs
                            if(!HttpRequestResponse.sendHTTPSCommandToWiFiModules(HttpRequestResponse.Command.Stop, llc.getSelectedIPAddresses(), null))
                            {
                                MessageBox.Show("Stopping performance on microcontrollers failed...");
                            }
                        }
                        catch (Exception ex)
                        {

                        }

                        //Enable windows media player controls
                        amp.Ctlenabled = true;
                    }

                    try
                    {
                        if (EffectsManager.StripsArray.Count > 0)
                        {
                            //Reset EffectsManager performance time
                            EffectsManager.resetPerformanceTime();
                        }
                    }catch(Exception ex)
                    {

                    }

                    break;
                case 2:    // Paused
                    //this.timer.Text = "Paused";
                    //this.timer.Update();
                    this.isPlaying = false;

                    //Pause performance on mcus if Synchronize MCUs is checked
                    if (llc.getSyncMCUsChecked())
                    {
                        //Disable windows media player controls
                        amp.Ctlenabled = false;

                        try
                        {
                            //Verify if user needs to setup found IP addresses first
                            if (!llc.getSkipIPSetupChecked())
                            {
                                //Enable windows media player controls
                                amp.Ctlenabled = true;
                                settingUp = false;
                                return;
                            }
                            else if (llc.getSelectedIPAddresses() == null || llc.getSelectedIPAddresses().Count <= 0)
                            {
                                //Enable windows media player controls
                                amp.Ctlenabled = true;
                                settingUp = false;
                                return;
                            }

                            //Send pause signal and Synchronize MCUs
                            if(!HttpRequestResponse.sendHTTPSCommandToWiFiModules(HttpRequestResponse.Command.Pause, llc.getSelectedIPAddresses(), null))
                            {
                                MessageBox.Show("Pausing performance on microcontrollers failed...");
                            }
                        }
                        catch (Exception ex)
                        {

                        }

                        //Enable windows media player controls
                        amp.Ctlenabled = true;
                    }

                    //Set continueFromCurrentTime to true so when play is pressed, performance will resume from there
                    EffectsManager.continueFromCurrentTime = true;

                    break;

                case 3:// Playing (Done 2nd at start)
                    //this.timer.Text = "Playing";
                    //this.timer.Update();
                    float trackerTime = 0;
                    int delayTime = 0;
                    this.isPlaying = true;

                    //Pause until the setup below has gone through
                    amp.Ctlcontrols.pause();

                    //Update music player time and timer text
                    //updateMusicPlayerAndTimerText();

                    //Change trackbar maximum to total seconds in song
                    this.trackBar.Value = (int)this.player2.Ctlcontrols.currentPosition;
                    trackerTime = this.trackBar.Value;
                    this.trackBar.Maximum = Convert.ToInt32(Decimal.Parse(this.player2.currentMedia.duration.ToString()));

                    //Start performance on mcus if Synchronize MCUs is checked
                    if (llc.getSyncMCUsChecked())
                    {
                        //Disable windows media player controls
                        amp.Ctlenabled = false;

                        try
                        {
                            //Verify if user needs to setup found IP addresses first
                            if (!llc.getSkipIPSetupChecked())
                            {
                                llc.mcuIPSetupAndWait();
                            }
                            else if(llc.getSelectedIPAddresses() == null || llc.getSelectedIPAddresses().Count <= 0)
                            {
                                llc.mcuIPSetupAndWait();
                            }

                            //Send performance time to update in mcus
                            if (llc.getSelectedIPAddresses() != null && llc.getSelectedIPAddresses().Count > 0)
                            {
                                if (HttpRequestResponse.sendHTTPSCommandToWiFiModules(HttpRequestResponse.Command.UpdateTime, llc.getSelectedIPAddresses(), Convert.ToString(Decimal.ToInt32(Decimal.Parse(convertTimeToSeconds(this.timer.Text)) * 1000))))
                                {
                                    //Delay for a few seconds
                                    System.Threading.Thread.Sleep(1000);
                                    //Send start signal and Synchronize MCUs
                                    if (!HttpRequestResponse.sendHTTPSCommandToWiFiModules(HttpRequestResponse.Command.Start, llc.getSelectedIPAddresses(), null))
                                    {
                                        MessageBox.Show("Starting performance on microcontrollers failed...");
                                    }
                                }
                                else
                                {
                                    MessageBox.Show("Updating performance times on microcontrollers failed...");
                                }
                            }
                        }catch(Exception ex)
                        {

                        }

                        //Enable windows media player controls
                        amp.Ctlenabled = true;
                    }

                    //Delay play time if specified
                    if (this.chkPlayerDelayTime.Checked)
                    {
                        //Get delay time from playerDelayTime textbox
                        try
                        {
                            delayTime = Decimal.ToInt32(Decimal.Parse(this.playerDelayTime.Text.ToString().Trim()) * 1000);
                        }catch(Exception ex)
                        {
                            delayTime = 0;
                        }

                        //Wait delayed amount of time
                        System.Threading.Thread.Sleep(delayTime);
                        this.player2.Ctlcontrols.currentPosition = trackerTime;
                    }

                    //Update brightness for all current strips
                    EffectsManager.updateStripsBrightness();

                    //Start ticker
                    this.llc.startTicker();

                    //Continue playing song
                    amp.Ctlcontrols.play();

                    break;

                case 4:    // ScanForward
                    //currentStateLabel.Text = "ScanForward";
                    this.isPlaying = true;

                    //Update music player time and timer text
                    updateMusicPlayerAndTimerText();

                    break;

                case 5:    // ScanReverse
                    //currentStateLabel.Text = "ScanReverse";
                    this.isPlaying = true;

                    //Update music player time and timer text
                    updateMusicPlayerAndTimerText();

                    break;

                case 6:    // Buffering
                    //currentStateLabel.Text = "Buffering";
                    this.isPlaying = false;
                    break;

                case 7:    // Waiting
                    //currentStateLabel.Text = "Waiting";
                    this.isPlaying = false;
                    break;

                case 8:    // MediaEnded (at end of song)
                    //currentStateLabel.Text = "MediaEnded";
                    this.isPlaying = false;
                    break;

                case 9:    // Transitioning (Done 1st at start...)
                    //currentStateLabel.Text = "Transitioning";
                    this.isPlaying = false;
                    break;

                case 10:   // Ready
                    //currentStateLabel.Text = "Ready";
                    this.isPlaying = false;
                    break;

                case 11:   // Reconnecting
                    //currentStateLabel.Text = "Reconnecting";
                    this.isPlaying = false;
                    break;

                case 12:   // Last
                    //currentStateLabel.Text = "Last";
                    this.isPlaying = false;
                    break;

                default:
                    //currentStateLabel.Text = ("Unknown State: " + e.newState.ToString());
                    this.isPlaying = false;
                    break;
            }

            settingUp = false;
        }
        

        #endregion Screen Events


        #region Getters & Setters

        public bool IsPlaying
        {
            get
            {
                return isPlaying;
            }

            set
            {
                isPlaying = value;
            }
        }

        public string CurrentSongFilePath
        {
            get
            {
                return currentSongFilePath;
            }

            set
            {
                currentSongFilePath = value;
            }
        }

        #endregion Getters & Setters
    }
}
