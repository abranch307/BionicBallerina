using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LEDLightingComposer
{
    class SynchronizedCommandSend
    {
        //Global variables
        //private ESP8266SequenceStarter ess;
        private Boolean[] esp8266sReady;
        private String ipAddress;
        private int threadNo, waitTimeMiliSeconds = 7000;
        private short command;
        private String sendString = "";
        private static short START = 0, STOP = 1, PAUSE = 2, UPDATETIME = 3;
        private static String readyUri = "/ready", readyParam = "READY", initLEDSeqsUri = "/init_led_seqs", initActionParam = "INITLEDSEQS", initUpdatePerformTimeUri = "/update_performance_time", initUPTActionParam = "UPT";
        
        public SynchronizedCommandSend(Boolean[] ESP8266sReady, String IPAddress, int ThreadNo, short Command, String SendString)
        {
            //Set global to passed
            this.esp8266sReady = ESP8266sReady;
            this.ipAddress = IPAddress;
            this.threadNo = ThreadNo;
            this.command = Command;
            this.sendString = SendString;
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
            String curUri = "", curParameter = "", curCommand = "";
            Stopwatch stopWatch = new Stopwatch();
            int i = 0;
            bool exitLoop = false;

            //Send http request to ESP8266 (repeat for specified wait time until an http response is received.  if wait time is exceeded, then exit thread)
            stopWatch.Start();
            while (!ready)
            {
                //Send http request to ESP8266 and wait for http response from ESP8266 (set ready to true if response received)
                if(!HttpRequestResponse.getResponse(HttpRequestResponse.sendHttpRequest(ipAddress, readyUri, readyParam, "Y")).Equals(""))
                {
                    ready = true;
                }


                //If wait time exceeded, exit thread
                if(stopWatch.ElapsedMilliseconds >= waitTimeMiliSeconds)
                {
                    exitLoop = true;
                    break;
                }
            }

            //Exit method if specified
            if (exitLoop)
            {
                return;
            }

            //Set uri, parameter, and command depending on type
            if (command == START)
            {
                curUri = initLEDSeqsUri;
                curParameter = initActionParam;
                curCommand = command.ToString();
            }
            else if(command == STOP)
            {
                curUri = initLEDSeqsUri;
                curParameter = initActionParam;
                curCommand = command.ToString();
            }
            else if(command == PAUSE)
            {
                curUri = initLEDSeqsUri;
                curParameter = initActionParam;
                curCommand = command.ToString();
            }
            else
            {
                curUri = initUpdatePerformTimeUri;
                curParameter = initUPTActionParam;
                curCommand = sendString;
            }

            //An http response was received from ESP8266, so set thread's allReady element to true
            esp8266sReady[threadNo] = true;

            //Wait up to specified milliseconds for ESP8266s to synchronize and the sendSignalThreads is changed to true
            stopWatch.Reset();
            while(true)
            {
                //Check if ESP8266SequenceStarter's allReady is true
                if (HttpRequestResponse.sendSignalThreads)
                {
                    //Send signal to ESP8266 via http
                    HttpRequestResponse.sendHttpRequestNoResponse(ipAddress, curUri, curParameter, curCommand);

                    //Exit loop
                    break;
                }

                //Exit and set exitLoop to true if elapsed time is greater than wait time
                if(stopWatch.ElapsedMilliseconds >= waitTimeMiliSeconds)
                {
                    exitLoop = true;
                    break;
                }
            }
        }
    }
}
