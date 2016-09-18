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

namespace WinFormESP8266StartSequenceSender
{
    public partial class ESP8266SequenceStarter : Form
    {
        //Declare global variables
        private List<String> mcuIPAddresses;
        private Boolean sendSignalThreads;
        private static short START = 0, STOP = 1, RESTART = 2;

        public ESP8266SequenceStarter()
        {
            InitializeComponent();
            
            //Initialize variables
            mcuIPAddresses = new List<String>();
            sendSignalThreads = false;

            //Set defaults
            this.radStartLEDSeq.Select();
        }

        /*
            Function btnAIPA_Click
            This method will open a new form where the user can enter an ip address for each MCU
        */
        private void btnAIPA_Click(object sender, EventArgs e)
        {
            try {
                //Declare variables
                String numMcus = "";
                int mcus = 0;

                //Verify a number > 0 has been entered for # of MCUs
                if((numMcus = this.txtNumMCUs.Text.ToString().Trim()).Equals(""))
                {
                    //Notify user a number of MCUs must be entered before continuing
                    MessageBox.Show("Please enter a valid number of MCUs before trying to enter IP Addresses...");
                    return;
                }

                //Open a new IP Address form
                mcus = int.Parse(numMcus);

                IPAddressForm ipaf = new IPAddressForm(mcus, mcuIPAddresses);
                ipaf.Owner = this;
                ipaf.Show();
            }catch(Exception ex)
            {
                MessageBox.Show("Error occured while trying to open IP Addresses Form: {0}", ex.Message);
            }
        }

        /*
            Function btnSHTTPR_Click (Send HTTP Request):
            This function will verify ip address list is full and then send the specified signal to all 
            ESP8266 modules synchronously using threads
        */
        private void btnSHTTPR_Click(object sender, EventArgs e)
        {
            //Declare variables
            Boolean[] esp8266sReady;
            int iLoopCount = 0;
            Boolean allReady = false;

            //Reset sendSignalThreads to false
            sendSignalThreads = false;

            //Verify ip address list is not empty and all values have been entered
            foreach(String ip in mcuIPAddresses)
            {
                if (!ip.Trim().Equals(""))
                {
                    iLoopCount += 1;
                }
            }
            if(iLoopCount == 0 || iLoopCount != mcuIPAddresses.Count)
            {
                //Notify user that there is either nothing in list or not all values in list has values
                MessageBox.Show("Please validate that your ip addresses were entered correctly then retry sending...");
                return;
            }

            //Reset iLoopCount and setup thread array
            SynchronizedCommandSend scs;
            Thread[] threads = new Thread[mcuIPAddresses.Count];
            esp8266sReady = new Boolean[mcuIPAddresses.Count];
            iLoopCount = 0;

            /*Create a thread for each element in the list which will contact ESP8266 module and wait until all modules
            have responded with a ready signal before sending the start, stop, or restart signal*/
            foreach (String ip in mcuIPAddresses)
            {
                if (this.radStartLEDSeq.Checked)
                {
                    scs = new SynchronizedCommandSend(this, esp8266sReady, ip, iLoopCount, START);
                }
                else if (this.radRestartLEDSeq.Checked)
                {
                    scs = new SynchronizedCommandSend(this, esp8266sReady, ip, iLoopCount, RESTART);
                }
                else {
                    scs = new SynchronizedCommandSend(this, esp8266sReady, ip, iLoopCount, STOP);
                }
                threads[iLoopCount] = new Thread(new ThreadStart(scs.SynchronizedSend));
                threads[iLoopCount].Start();
                iLoopCount += 1;
            }

            //Reset iLoopCount
            iLoopCount = 0;

            //Loop until all threads have specified the ESP8266 module is ready
            while (true)
            {
                iLoopCount += 1;
                Thread.Sleep(1000);

                //Exit if loop reaches 1000
                if(iLoopCount == 5)
                {
                    //End all threads then exit loop to exit program without synchronizing
                    foreach(Thread th in threads) { th.Abort(); }
                    break;
                }
                else
                {
                    //Set allReady to true until proven otherwise
                    allReady = true;

                    //Loop through all elements in esp8266sReady list and verify if all values are 1, meaning all esp8266 modules are ready
                    foreach(Boolean ready in esp8266sReady)
                    {
                        if (!ready) { allReady = false; break; }
                    }

                    //If all ready, then have all threads send the signal, otherwise continue loop
                    if (allReady)
                    {
                        //Have all threads send signal and exit loop
                        sendSignalThreads = true;

                        //Wait for threads to finish
                        foreach(Thread th in threads)
                        {
                            th.Join();
                        }

                        break;
                    }
                }
            }

            //Reset sendSignalThreads to false
            sendSignalThreads = false;
        }

        /*
            Function btnFindMCUs_Click
            This function will search the connected network for devices with dns names that begin with 
            "ESP_", since all ESP8266 modules will be programmed to adhere to this policy
        */
        private void btnFindMCUs_Click(object sender, EventArgs e)
        {
            //Declare variables

            //Search Network for DNS names that begin with ESP_ abd show user the list of devices found, 
            //including their DNS Name and IP Address
            MessageBox.Show(HttpRequestResponse.getAllESP8266DeviceIPAddresses());

        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        public bool SendSignalThreads
        {
            get
            {
                return sendSignalThreads;
            }

            set
            {
                sendSignalThreads = value;
            }
        }
    }
}
