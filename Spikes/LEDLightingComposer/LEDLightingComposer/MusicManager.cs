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
    class MusicManager
    {
        //Declare global variables
        private LEDLightingComposer llc;
        private SoundPlayer player;
        public AxWindowsMediaPlayer player2;
        public TextBox timer;
        private Button loadSong, jump2Secs;
        private Label songName;
        public String currentSongFilePath;
        public bool isPlaying;

        public MusicManager(LEDLightingComposer LLC, AxWindowsMediaPlayer Player2, Button LoadSong, Button Jump2Secs, TextBox Timer, Label SongName)
        {
            //Set class variables to passed
            this.llc = LLC;
            this.player2 = Player2;
            this.loadSong = LoadSong;
            this.jump2Secs = Jump2Secs;
            this.timer = Timer;
            this.songName = SongName;
            this.currentSongFilePath = "";
            this.IsPlaying = false;
            
            //Initialize sound player
            player = new SoundPlayer();
        }

        public void UpdateLabel()
        {
            timer.Text = Convert.ToInt32(player2.Ctlcontrols.currentPosition).ToString();
            timer.Update();
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
            
            try {
                //Jump Windows Media Player object to position in timer textbox (in seconds)
                player2.Ctlcontrols.currentPosition = int.Parse(this.timer.Text.ToString().Trim());
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
        }

        /*
        */
        public void WMPlayer_PlayStateChange(object sender, AxWMPLib._WMPOCXEvents_PlayStateChangeEvent e)
        {
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
                    this.isPlaying = false;
                    break;
                case 2:    // Paused
                    //this.timer.Text = "Paused";
                    //this.timer.Update();
                    this.isPlaying = false;
                    break;

                case 3:    // Playing
                    //this.timer.Text = "Playing";
                    //this.timer.Update();
                    this.isPlaying = true;
                    this.llc.startTicker();
                    break;

                case 4:    // ScanForward
                    //currentStateLabel.Text = "ScanForward";
                    this.isPlaying = false;
                    break;

                case 5:    // ScanReverse
                    //currentStateLabel.Text = "ScanReverse";
                    this.isPlaying = false;
                    break;

                case 6:    // Buffering
                    //currentStateLabel.Text = "Buffering";
                    this.isPlaying = false;
                    break;

                case 7:    // Waiting
                    //currentStateLabel.Text = "Waiting";
                    this.isPlaying = false;
                    break;

                case 8:    // MediaEnded
                    //currentStateLabel.Text = "MediaEnded";
                    this.isPlaying = false;
                    break;

                case 9:    // Transitioning
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
        }

        #endregion Screen Events
    }
}
