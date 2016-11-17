using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinFormESP8266StartSequenceSender
{
    static class HttpRequestResponse
    {
        //Global variables
        private static String ESP8266DNSPREFIX = "ESP_";
        private static List<String> ips, iparray;

        /*
        */
        public static WebResponse sendHttpRequest(String IPAddress, String Command, String Parameter, String BytesAsString)
        {
            //Declare variables
            WebRequest request = null;
            WebResponse response = null;
            Stream dataStream = null;

            try {
                // Create a request for the URL. 
                request = WebRequest.Create(string.Format(
                  "http://{0}{1}?{2}={3}", IPAddress.Trim(), Command.Trim(), Parameter, BytesAsString.Trim()));

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
                MessageBox.Show("Error creating HTTP request: " + ex.Message);
            }

            return response;
        }

        /*
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

            //Close response
            wr.Close();
            return responseFromServer;
        }

        /*
        */
        public static String getAllESP8266DeviceIPAddresses()
        {
            //Declare variables
            String sret = "", ipBase = "", ipt = "";
            ips = new List<String>();
            Boolean cont = false;
            EnterIPAddressBaseForm eipabf = new EnterIPAddressBaseForm();
            Thread[] threads = new Thread[254];
            iparray = new List<String>();

            //Ask user for ipBase
            if (eipabf.ShowDialog() == DialogResult.OK)
            {

                //Set ipBase to base entered by user in form
                ipBase = eipabf.textBox1.Text.ToString().Trim();
                eipabf.Dispose();
            }
            else
            {
                //Dispose of form and exit sub
                eipabf.Dispose();
                sret = "No base entered...";
                return sret;
            }

            //Ping all devices on network
            for (int i = 1; i < 255; i++)
            {
                ipt = ipBase + i.ToString();
                iparray.Add(ipt);
                threads[i - 1] = new Thread(new ThreadStart(pingAllNetworkAddresses));
                threads[i - 1].Start();
                threads[i - 1].Join();
            }

            //Join threads
            for (int i = 1; i < 255; i++)
            {
                threads[i - 1].Join();
            }

            //Concatenate strings together in readable format
            foreach (String ip in ips)
            {
                //Get hostname and add hostname + ip address to string
                sret += ip + "\n";
            }

            return sret;
        }

        /*
        */
        private static void pingAllNetworkAddresses()
        {
            try
            {
                String ipaddress = iparray[(iparray.Count - 1)];
                Ping p = new Ping();
                p.PingCompleted += new PingCompletedEventHandler(p_PingCompleted);
                p.SendAsync(ipaddress, 100, ipaddress);
            }
            catch (Exception ex)
            {

            }
        }

        public static void p_PingCompleted(object sender, PingCompletedEventArgs e)
        {
            //Declare variables
            string ip = (string)e.UserState;

            //If ping successful, continue
            if (e.Reply != null && e.Reply.Status == IPStatus.Success)
            {
                string name;
                try
                {
                    //Try to retrieve host name from ip address
                    IPHostEntry hostEntry = Dns.GetHostEntry(ip);
                    name = hostEntry.HostName;

                    //If this ip address contains the ESP8266 prefix, then add to list
                    if (name.Trim().ToUpper().Contains(ESP8266DNSPREFIX))
                    {
                        ips.Add(name + " - " + ip);
                    }
                }
                catch (Exception ex)
                {
                    
                }
            }
            else if (e.Reply == null)
            {
                //Do nothing
            }
        }
    }
}
