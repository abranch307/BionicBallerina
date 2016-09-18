using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinFormESP8266StartSequenceSender
{
    class SynchronizedCommandSend
    {
        //Global variables
        private ESP8266SequenceStarter ess;
        private Boolean[] esp8266sReady;
        private String ipAddress;
        private int threadNo;
        short signal;
        private static short START = 0, STOP = 1, RESTART = 2;
        private static String readyUri = "/ready", initLEDSeqsUri = "/init_led_seqs", readyParam = "READY", initActionParam = "INITLEDSEQS";
        
        public SynchronizedCommandSend(ESP8266SequenceStarter ESS, Boolean[] ESP8266sReady, String IPAddress, int ThreadNo, short Signal)
        {
            //Set global to passed
            this.ess = ESS;
            this.esp8266sReady = ESP8266sReady;
            this.ipAddress = IPAddress;
            this.threadNo = ThreadNo;
            this.signal = Signal;
        }

        /*
            Function SynchronizedSend:
            This method will send an http request to ESP8266 using passed ip address and set list element to true (mainly to verify that ESP8266 is
            reachable), then wait until ESP8266SequenceStarter's allReady is true to send signal to ESP8266
        */
        public void SynchronizedSend()
        {
            //Declare variables
            Boolean ready = false;
            int i = 0;

            //Send http request to ESP8266 (repeat 1000 times until an http response is received.  if 1000 times is met, then exit thread)
            while (!ready)
            {
                //Add 1 to i for count
                i += 1;

                //Send http request to ESP8266 and wait for http response from ESP8266 (set ready to true if response received)
                if(!HttpRequestResponse.getResponse(HttpRequestResponse.sendHttpRequest(ipAddress, readyUri, readyParam, "Y")).Equals(""))
                {
                    ready = true;
                }


                //If i = 1000, then exit thread
                if(i > 1000)
                {
                    return;
                }
            }

            //An http response was received from ESP8266, so set thread's allReady element to true
            esp8266sReady[threadNo] = true;

            //Wait 1000 iterations until ESP8266SequenceStarter's allReady is true
            i = 0;
            while(true)
            {
                //Check if ESP8266SequenceStarter's allReady is true
                if (ess.SendSignalThreads)
                {
                    //Send signal to ESP8266 via http
                    HttpRequestResponse.getResponse(HttpRequestResponse.sendHttpRequest(ipAddress, initLEDSeqsUri, initActionParam, signal.ToString()));

                    //Exit loop
                    break;
                }
            }
        }
    }
}
