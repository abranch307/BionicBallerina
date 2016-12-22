/*
	Author: Aaron Branch, Zach Jarmon, Peter Martinez
	Created: 
	Last Modified:
	Class: HttpRequestResponse.cs
	Class Description:
		This class handles communication between WiFi enabled devices via HTTPS requests and responses.  In this
        program, this class specifically communicates with the WiFi modules connected to ProTrinket microcontrollers,
        passing commands to the WiFi module which in turn relays to microcontrollers.
*/

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LEDLightingComposer
{
    static class HttpRequestResponse
    {
        //Global variables
        private static String ESP8266DNSPREFIX = "ESP_"; //Allows for later filtering of returned ipaddress/hostname to only show ESP8266 WiFi Modules (Currently don't do b/c user may not have DNS)
        private static ConcurrentBag<String> ips; //Allows for multiple threads to add to same list (thread safe list)
        private static List<String> iparray, endingIPS; //iparray holds all ipaddress that are conjured for pinging on local network while endingIPS holds actually found ipaddresses on network (ipaddresses that returned pings)
        private static int waitTimeMiliSeconds = 7000; //Default value for calling thread in sendStartHTTPSCommand to wait for spawned ping threads to return before killing all threads
        private static short START = 0, PAUSE = 1, STOP = 2, UPDATETIME = 3; //Available commands to send to WiFi module through HTTPS request
        public static bool sendSignalThreads; //Used to verify all selected WiFi modules returned a READY response
        public enum Command { Start, Pause, Stop, UpdateTime}

        /*
            Function: sendHTTPRequest
                Sends an HTTP request to passed ip address with passed uri, parameter, bytes to send and waits for a response

            Parameters: String IPAddress - remote device's/server's ip address, String Uri - uri for ip address local directory needed, 
                String Parameter - parameter for uri, String BytesAsString - data to send to remote computer

            Returns:WebResponse - the response from remote computer including http response variables
        */
        public static WebResponse sendHttpRequest(String IPAddress, String Uri, String Parameter, String BytesAsString)
        {
            //Declare variables
            WebRequest request = null;
            WebResponse response = null;
            Stream dataStream = null;

            try {
                // Create a request for the URL. 
                request = WebRequest.Create(string.Format(
                  "http://{0}{1}?{2}={3}", IPAddress.Trim(), Uri.Trim(), Parameter, BytesAsString.Trim()));

                // If required by the server, set the credentials.
                //request.Credentials = CredentialCache.DefaultCredentials;

                // Set the Method property of the request to POST.
                request.Method = "POST";

                // Set the ContentType property of the WebRequest.
                request.ContentType = "application/octet-stream";

                // Set the ContentLength property of the WebRequest.
                request.ContentLength = BytesAsString.Length;

                // Get the request stream.
                dataStream = request.GetRequestStream();

                // Write the data to the request stream.
                dataStream.Write(Encoding.ASCII.GetBytes(BytesAsString), 0, BytesAsString.Length);

                // Close the Stream object.
                dataStream.Close();

                // Get the response.
                response = request.GetResponse();
            }catch(Exception ex)
            {
                //MessageBox.Show("Error creating HTTP request: " + ex.Message);
                response = null;
            }

            return response;
        }

        /*
            Function: sendHttpRequestNoResponse
                Sends an HTTP request to passed ip address with passed uri, parameter, bytes to send and DOES NOT wait for a response
                (Speeds up synchronization efforts since time is not wasted waiting for a respond to the command sent, allowing music and
                performance on microcontroller to be more likely to start in synchrony)

            Parameters: String IPAddress - remote computer's/server's ip address, String Uri - uri for ip address local directory needed, 
                String Parameter - parameter for uri, String BytesAsString - data to send to remote computer

            Returns: bool - true or false as to the success of the http request sent
        */
        public static bool sendHttpRequestNoResponse(String IPAddress, String Uri, String Parameter, String BytesAsString)
        {
            //Declare variables
            WebRequest request = null;
            WebResponse response = null;
            Stream dataStream = null;
            bool bret = false;

            try
            {
                // Create a request for the URL. 
                request = WebRequest.Create(string.Format(
                  "http://{0}{1}?{2}={3}", IPAddress.Trim(), Uri.Trim(), Parameter, BytesAsString.Trim()));

                // If required by the server, set the credentials.
                //request.Credentials = CredentialCache.DefaultCredentials;

                // Set the Method property of the request to POST.
                request.Method = "POST";

                // Set the ContentType property of the WebRequest.
                request.ContentType = "application/octet-stream";

                // Set the ContentLength property of the WebRequest.
                request.ContentLength = BytesAsString.Length;

                // Get the request stream.
                dataStream = request.GetRequestStream();

                // Write the data to the request stream.
                dataStream.Write(Encoding.ASCII.GetBytes(BytesAsString), 0, BytesAsString.Length);

                // Close the Stream object.
                dataStream.Close();

                // Get the response.
                response = request.GetResponse();
                response.Close();

                bret = true;
            }
            catch (Exception ex)
            {
                //MessageBox.Show("Error creating HTTP request: " + ex.Message);

            }

            return bret;
        }

        /*
            Function: getResponse
                Takes a WebResponse (HttpResponse), pulls response from remote device/server and converts retrieved data
                as a string if status is OK

            Parameters: WebResponse wr - http response matching previously sent http request

            Returns: String - data in http response
        */
        public static String getResponse(WebResponse wr)
        {
            //Declare variables
            String responseFromServer = "";

            //Exit sub if response is null
            if(wr == null)
            {
                return responseFromServer;
            }

            // Display the status.
            responseFromServer = (((HttpWebResponse)wr).StatusDescription);

            try
            {
                if (responseFromServer.Trim().Equals("OK"))
                {
                    // Get the stream containing content returned by the server.
                    Stream rdataStream = wr.GetResponseStream();

                    // Open the stream using a StreamReader for easy access.
                    StreamReader reader = new StreamReader(rdataStream);

                    // Read the content.
                    responseFromServer = reader.ReadToEnd();

                    // Clean up the streams and the response.
                    reader.Close();

                }
                else
                {
                    responseFromServer = "";
                }
            }catch(Exception ex)
            {

            }

            //Close response
            wr.Close();
            return responseFromServer;
        }

        /*
            Function: sendHTTPSCommandToWiFiModules
                Sends commands (Play, Stop, Pause, Update Performance Time) to WiFi Modules as an https request that
                the WiFi module will relay to connected microcontroller via Serial interface

            Parameters: Command Command2Send - command to send to WiFi Module, MCUIPAddresses - list of ip address to send
                command to (should only be ip addresses of valid ESP8266 WiFi modules or equivalent),
                String SendString - if sending UpdateTime this will be current performance time in milliseconds as a string

            Returns: bool - true or false as to the success of sending commands and all devices on other end of ipaddresses responding
        */
        public static bool sendHTTPSCommandToWiFiModules(Command Command2Send, List<String> MCUIPAddresses, String SendString)
        {
            //Declare variables
            Boolean[] esp8266sReady;
            int iLoopCount = 0;
            Boolean allReady = false;
            bool bret = false;

            //Reset sendSignalThreads to false
            sendSignalThreads = false;

            //Verify ip address list is not empty and all values have been entered
            foreach (String ip in MCUIPAddresses)
            {
                if (!ip.Trim().Equals(""))
                {
                    iLoopCount += 1;
                }
            }
            if (iLoopCount == 0 || iLoopCount != MCUIPAddresses.Count)
            {
                //Notify user that there is either nothing in list or not all values in list has values
                //MessageBox.Show("Please validate that your ip addresses were entered correctly then retry sending...");
                return bret;
            }

            //Reset iLoopCount and setup thread array
            SynchronizedCommandSend scs = null;
            Thread[] threads = new Thread[MCUIPAddresses.Count];
            esp8266sReady = new Boolean[MCUIPAddresses.Count];
            iLoopCount = 0;
            Stopwatch stopWatch = new Stopwatch();

            /*Create a thread for each element in the list which will contact ESP8266 module and wait until all modules
            have responded with a ready signal before sending the start, stop, or restart signal*/
            foreach (String ip in MCUIPAddresses)
            {
                switch (Command2Send)
                {
                    case Command.Start:
                        scs = new SynchronizedCommandSend(esp8266sReady, ip, iLoopCount, START, null);
                        break;
                    case Command.Pause:
                        scs = new SynchronizedCommandSend(esp8266sReady, ip, iLoopCount, PAUSE, null);
                        break;
                    case Command.Stop:
                        scs = new SynchronizedCommandSend(esp8266sReady, ip, iLoopCount, STOP, null);
                        break;
                    case Command.UpdateTime:
                        scs = new SynchronizedCommandSend(esp8266sReady, ip, iLoopCount, UPDATETIME, SendString);
                        break;
                }

                threads[iLoopCount] = new Thread(new ThreadStart(scs.SynchronizedSend));
                threads[iLoopCount].Start();
                iLoopCount += 1;
            }

            //Loop until all threads have specified the ESP8266 module is ready
            stopWatch.Start();
            while (true)
            {
                //Exit if elapsed time exceeds wait time
                if (stopWatch.ElapsedMilliseconds >= waitTimeMiliSeconds)
                {
                    //End all threads then exit loop to exit program without synchronizing
                    foreach (Thread th in threads) { th.Abort(); }
                    break;
                }
                else
                {
                    //Set allReady to true until proven otherwise
                    allReady = true;

                    //Loop through all elements in esp8266sReady list and verify if all values are 1, meaning all esp8266 modules are ready
                    foreach (Boolean ready in esp8266sReady)
                    {
                        if (!ready) { allReady = false; break; }
                    }

                    //If all ready, then have all threads send the signal, otherwise continue loop
                    if (allReady)
                    {
                        //Have all threads send signal and exit loop
                        sendSignalThreads = true;

                        //Wait for threads to finish
                        foreach (Thread th in threads)
                        {
                            th.Join();
                        }

                        break;
                    }
                }
            }

            //Reset sendSignalThreads to false
            bret = sendSignalThreads;
            sendSignalThreads = false;

            return bret;
        }

        /*
            Function: getAllIPAddressesOnNetwork
                Gets ip base (assumes a 192.168. network) for device program is running on, pings all devices on the network, and adds 
                ip addresses whose device responded to a list to be returned

            Parameters: None

            Returns: List<String> - list of ip addresses whose device responded to ping (currently is not filtered to only return ESP8266s, but returns all devices.
                To return only the ESP8266 ip addresses, one could check for "ESP_" in dns hostname and only return those, but this only works if there is a dns
                server on network.  Attempts to allow ESP8266 to return its own hostname when pinged using the MDNResponder class failed to work)
        */
        public static List<String> getAllIPAddressesOnNetwork()
        {
            //Declare variables
            List<Thread> threads = new List<Thread>();
            iparray = new List<String>();
            ips = new ConcurrentBag<String>();
            endingIPS = new List<string>();
            String sret = "", ipBase = "", ipt = "";

            //Get IP Base
            ipBase = getLocalBaseIP();

            //Ping all devices on network
            for (int i = 1; i < 255; i++)
            {
                try
                {
                    ipt = ipBase + i.ToString();
                    iparray.Add(ipt);

                    threads.Add(new Thread(new ThreadStart(pingNetworkAddress)));
                    threads.ElementAt(threads.Count - 1).Start();
                }
                catch (Exception ex)
                {

                }
            }

            //Join threads
            foreach (Thread t in threads)
            {
                try
                {
                    t.Join();
                }
                catch (Exception ex)
                {

                }
            }

            for (int i = 0; i < ips.Count; i++)
            {
                endingIPS.Add(ips.ElementAt(i));
            }
            return endingIPS;
        }

        /*
            Function: getLocalBaseIP:
                Returns the base ip address of the connected device (assumes a 192.168. network to filter out other network interfaces
                attached to device (ex. Ethernet and WiFi interfaces - the program wouldn't know which device to send through unless
                specified which could be an added functionality)

            Parameters:  None

            Returns: String - first 3 octets of user's local ip address
        */
        private static String getLocalBaseIP()
        {
            String strHostName = Dns.GetHostName();
            IPHostEntry ipEntry = Dns.GetHostEntry(strHostName);
            IPAddress[] addr = ipEntry.AddressList;
            String[] strArray = { "" };
            String sret = "";

            for (int i = 0; i < addr.Length; i++)
            {
                if (addr[i].AddressFamily.ToString().Equals("InterNetwork") && addr[i].ToString().Contains("192.168."))
                {
                    strArray = addr[i].ToString().Split('.');
                }
            }

            for(int i = 0; i < strArray.Length; i++)
            {
                //Concatenate ip address back
                sret += strArray[i] + ".";

                //Exit if in third octect
                if(i == 2)
                {
                    break;
                }
            }
            return sret;
        }

        /*
            Function: pingNetworkAddress
                Pings the network address at the end of the global iparray.  This method is spawned in a new thread, so
                using the global array removes the need to pass values through the thread intitialization

            Parameters: Nothing

            Returns: Nothing
        */
        private static void pingNetworkAddress()
        {
            //Declare variables
            String ipaddress = "", name = "";
            Ping p = null;
            PingReply r = null;
            
            try
            {
                ipaddress = iparray[(iparray.Count - 1)];

                //p.PingCompleted += new PingCompletedEventHandler(p_PingCompleted);
                //p.SendAsync(ipaddress, 1000, ipaddress);

                //for (int i = 0; i < 2; i++)
                //{
                    p = new Ping();
                    r = p.Send(ipaddress, 3000);
                    p.Dispose();
                //}

                if (r.Status == IPStatus.Success)
                {
                    try
                    {
                        //Try to retrieve host name from ip address
                        IPHostEntry hostEntry = Dns.GetHostEntry(ipaddress);
                        name = hostEntry.HostName;
                    }
                    catch (Exception ex)
                    {

                    }
                    //If this ip address contains the ESP8266 prefix or a blank name (could be no dns avaiable), then add to list
                    //if (name.Trim().ToUpper().Contains(ESP8266DNSPREFIX) || name.Trim().Equals(""))
                    //{
                    ips.Add(name + " ; " + ipaddress);
                    //}
                }
            }
            catch (Exception ex)
            {
                 
            }
        }

        /*
            Function: p_PingCompleted
                Callback function for pings (Currently not used as was not efficient as handling the completed pings myself)

            Parameters: object & PingCompletedEventArgs parameters

            Returns: Nothing
        */
        public static void p_PingCompleted(object sender, PingCompletedEventArgs e)
        {
            //Declare variables
            string ip = (string)e.UserState, name = "";

            //If ping successful, continue
            if (e.Reply != null && e.Reply.Status == IPStatus.Success)
            {
                try
                {
                    //Try to retrieve host name from ip address
                    IPHostEntry hostEntry = Dns.GetHostEntry(ip);
                    name = hostEntry.HostName;
                }
                catch (Exception ex)
                {
                    
                }

                //If this ip address contains the ESP8266 prefix, then add to list
                //if (name.Trim().ToUpper().Contains(ESP8266DNSPREFIX))
                //{
                ips.Add(name + " ; " + ip);
                //}
            }
            else if (e.Reply == null)
            {
                //Do nothing
            }
        }
    }
}
