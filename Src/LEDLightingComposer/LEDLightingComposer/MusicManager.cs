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
        public AxWindowsMediaPlayer player2;
        public Panel TrackBarPanel;
        public TrackBar trackBar;
        public TextBox timer;
        private Button loadSong, jump2Secs;
        private Label songName;
        public String currentSongFilePath;
        public bool isPlaying, settingUp;

        public MusicManager(LEDLightingComposerCS LLC, AxWindowsMediaPlayer Player2, Button LoadSong, Button Jump2Secs, TextBox Timer, Panel TrackBarPanel, Label SongName)
        {
            //Set class variables to passed
            this.llc = LLC;
            this.player2 = Player2;
            this.loadSong = LoadSong;
            this.jump2Secs = Jump2Secs;
            this.timer = Timer;
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

        public void UpdateLabel()
        {
            try
            {
                timer.Text = Convert.ToInt32(player2.Ctlcontrols.currentPosition).ToString();
                timer.Update();
                this.trackBar.Value = (int)this.player2.Ctlcontrols.currentPosition;
            }
            catch(Exception ex)
            {

            }
        }

        #region Private Methods


        #endregion Private Methods


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


        #region Screen Events

        /*
            Function btnLoadSong_Click:
            This function will load a song a filestream for playing via onscreen buttons
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

                //Jump EffectsManager to current performance time
                EffectsManager.findCurrentSeqFromPerformanceTime((long)jump2Secs);

                //Invalidate LLC screen so redraw can happen
                llc.Invalidate();
            }catch(Exception ex)
            {
                MessageBox.Show("Error seeking to position: " + ex.Message);
            }
        }

        /*
            
        */
        public void WMPlayer_PositionChange(object sender, AxWMPLib._WMPOCXEvents_PositionChangeEvent e)
        {
            this.timer.Text = this.player2.Ctlcontrols.currentPositionString;
            this.timer.Update();
            this.trackBar.Value = (int)this.player2.Ctlcontrols.currentPosition;
        }

        /*
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
                    //this.timer.Text = "0";
                    //this.timer.Update();

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

                            //Send start signal and Synchronize MCUs
                            if(!HttpRequestResponse.sendStartHTTPSCommand("STOP", llc.getSelectedIPAddresses(), null))
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

                            //Send start signal and Synchronize MCUs
                            if(!HttpRequestResponse.sendStartHTTPSCommand("PAUSE", llc.getSelectedIPAddresses(), null))
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

                case 3:    // Playing (Done 2nd at start)
                    //this.timer.Text = "Playing";
                    //this.timer.Update();
                    this.isPlaying = true;

                    //Pause until the setup below has gone through
                    amp.Ctlcontrols.pause();

                    //Update music player time and timer text
                    //updateMusicPlayerAndTimerText();

                    //Change trackbar maximum to total seconds in song
                    this.trackBar.Value = (int)this.player2.Ctlcontrols.currentPosition;
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
                                if (HttpRequestResponse.sendStartHTTPSCommand("UPDATETIME", llc.getSelectedIPAddresses(), convertTimeToSeconds(this.timer.Text)))
                                {
                                    //Send start signal and Synchronize MCUs
                                    if (!HttpRequestResponse.sendStartHTTPSCommand("START", llc.getSelectedIPAddresses(), null))
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

        /*
        */
        public void updateMusicPlayerAndTimerText()
        {
            //Declare variables
            long jump2Secs = 0;
            double currentPosition = 0;
            String currentPositionString = "";

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
            }catch(Exception ex) { }
        }

        /*
           Convert "00:45" and like formats to .75
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
                temp = (Time2Convert + ":00").Split(':');
            }
            else
            {
                temp = Time2Convert.Split(':');
            }

            if(temp.Length > 2)
            {
                //Account for hours
                sRet = Convert.ToUInt32(((float.Parse(temp[0]) * 3600) + (float.Parse(temp[1]) * 60) + float.Parse(temp[2]))).ToString();
            }
            else
            {
                //Only minutes and seconds
                sRet = Convert.ToDouble(((float.Parse(temp[0])*60) + float.Parse(temp[1]))).ToString();
            }

            return sRet;
        }

        #endregion Screen Events
    }
}
