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
        private static String ESP8266DNSPREFIX = "ESP_";
        private static ConcurrentBag<String> ips;
        private static List<String> iparray, endingIPS;
        private static int waitTimeMiliSeconds = 7000;
        private static short START = 0, PAUSE = 1, STOP = 2, UPDATETIME = 3;
        public static bool sendSignalThreads;

        /*
            Function:

            Parameters:

            Returns:
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
            Function:

            Parameters:

            Returns:
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
            Function:

            Parameters:

            Returns:
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
            Function:

            Parameters:

            Returns:
        */
        public static bool sendStartHTTPSCommand(String Type, List<String> mcuIPAddresses, String SendString)
        {
            //Declare variables
            Boolean[] esp8266sReady;
            int iLoopCount = 0;
            Boolean allReady = false;
            bool bret = false;

            //Reset sendSignalThreads to false
            sendSignalThreads = false;

            //Verify ip address list is not empty and all values have been entered
            foreach (String ip in mcuIPAddresses)
            {
                if (!ip.Trim().Equals(""))
                {
                    iLoopCount += 1;
                }
            }
            if (iLoopCount == 0 || iLoopCount != mcuIPAddresses.Count)
            {
                //Notify user that there is either nothing in list or not all values in list has values
                //MessageBox.Show("Please validate that your ip addresses were entered correctly then retry sending...");
                return false;
            }

            //Reset iLoopCount and setup thread array
            SynchronizedCommandSend scs;
            Thread[] threads = new Thread[mcuIPAddresses.Count];
            esp8266sReady = new Boolean[mcuIPAddresses.Count];
            iLoopCount = 0;
            Stopwatch stopWatch = new Stopwatch();

            /*Create a thread for each element in the list which will contact ESP8266 module and wait until all modules
            have responded with a ready signal before sending the start, stop, or restart signal*/
            foreach (String ip in mcuIPAddresses)
            {
                if (Type.Equals("START"))
                {
                    scs = new SynchronizedCommandSend(esp8266sReady, ip, iLoopCount, START, null);
                }
                else if (Type.Equals("PAUSE"))
                {
                    scs = new SynchronizedCommandSend(esp8266sReady, ip, iLoopCount, PAUSE, null);
                }
                else if(Type.Equals("STOP"))
                {
                    scs = new SynchronizedCommandSend(esp8266sReady, ip, iLoopCount, STOP, null);
                }else
                {
                    scs = new SynchronizedCommandSend(esp8266sReady, ip, iLoopCount, UPDATETIME, SendString);
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
            Function:

            Parameters:

            Returns:
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
            Method getLocalBaseIP:

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
            Function:

            Parameters:

            Returns:
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
            Function:

            Parameters:

            Returns:
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
