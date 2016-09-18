using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace ESP8266StructSenderProg
{
    public partial class StructSenderFrm : Form
    {
        //Setup structs
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct PinSetup
        {
            public Int16 numpixels;
            public byte datapin;
            public byte clockpin;
            public byte dotstarType;

            public byte[] ToBytes()
            {
                Byte[] bytes = new Byte[Marshal.SizeOf(typeof(PinSetup))];
                GCHandle pinStructure = GCHandle.Alloc(this, GCHandleType.Pinned);
                try
                {
                    Marshal.Copy(pinStructure.AddrOfPinnedObject(), bytes, 0, bytes.Length);
                    return bytes;
                }
                finally
                {
                    pinStructure.Free();
                }
            }
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct ChangePixelColor
        {
            public Int16 pixelIndex;
            public Int32 color;

            public byte[] ToBytes()
            {
                Byte[] bytes = new Byte[Marshal.SizeOf(typeof(ChangePixelColor))];
                GCHandle pinStructure = GCHandle.Alloc(this, GCHandleType.Pinned);
                try
                {
                    Marshal.Copy(pinStructure.AddrOfPinnedObject(), bytes, 0, bytes.Length);
                    return bytes;
                }
                finally
                {
                    pinStructure.Free();
                }
            }
        }

        public StructSenderFrm()
        {
            InitializeComponent();

            //Setup defaults
            this.cboxColor.SelectedIndex = 0;
        }

        //Send PinSetup struct(s) to ESP8266 via HTTP request
        private void sendStruct2ESP8266(List<PinSetup> psa)
        {
            try {
                // Create POST data and convert it to a byte array.
                byte[] byteArray = null;
                foreach (PinSetup ps in psa)
                {
                    if (byteArray == null)
                    {
                        byteArray = ps.ToBytes();
                    }
                    else
                    {
                        byteArray.Concat(ps.ToBytes());
                    }
                }

                // Create a request for the URL. 
                WebRequest request = WebRequest.Create(string.Format(
                  "http://{0}{1}?PINSETUPARG={2}", this.txtIPAddress.Text.ToString().Trim(), "/add_struct", Encoding.UTF8.GetString(byteArray)));
                // If required by the server, set the credentials.
                //request.Credentials = CredentialCache.DefaultCredentials;
                // Set the Method property of the request to POST.
                request.Method = "POST";
                // Set the ContentType property of the WebRequest.
                request.ContentType = "application/octet-stream";
                // Set the ContentLength property of the WebRequest.
                request.ContentLength = byteArray.Length;
                // Get the request stream.
                Stream dataStream = request.GetRequestStream();
                // Write the data to the request stream.
                dataStream.Write(byteArray, 0, byteArray.Length);
                // Close the Stream object.
                dataStream.Close();
                // Get the response.
                WebResponse response = request.GetResponse();
                // Display the status.
                Console.WriteLine(((HttpWebResponse)response).StatusDescription);
                // Get the stream containing content returned by the server.
                Stream rdataStream = response.GetResponseStream();
                // Open the stream using a StreamReader for easy access.
                StreamReader reader = new StreamReader(rdataStream);
                // Read the content.
                string responseFromServer = reader.ReadToEnd();
                // Display the content.
                MessageBox.Show(responseFromServer);
                // Clean up the streams and the response.
                reader.Close();
                response.Close();
            }catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //Send ChangePixelColor struct(s) to ESP8266 via HTTP request
        private void sendStruct2ESP8266(List<ChangePixelColor> cpca)
        {
            try
            {
                // Create POST data and convert it to a byte array.
                byte[] byteArray = { };
                foreach (ChangePixelColor cpc in cpca)
                {
                    if (byteArray == null)
                    {
                        byteArray = cpc.ToBytes();
                    }
                    else
                    {
                        byteArray.Concat(cpc.ToBytes());
                    }
                }

                // Create a request for the URL. 
                WebRequest request = WebRequest.Create(string.Format(
                  "http://{0}{1}?CHANGEPIXELCOLORARG={2}", this.txtIPAddress.Text.ToString().Trim(), "/add_struct", Encoding.UTF8.GetString(byteArray)));

                // If required by the server, set the credentials.
                //request.Credentials = CredentialCache.DefaultCredentials;

                // Set the Method property of the request to POST.
                request.Method = "POST";
                // Set the ContentType property of the WebRequest.
                request.ContentType = "application/octet-stream";
                // Set the ContentLength property of the WebRequest.
                request.ContentLength = byteArray.Length;
                // Get the request stream.
                Stream dataStream = request.GetRequestStream();
                // Write the data to the request stream.
                dataStream.Write(byteArray, 0, byteArray.Length);
                // Close the Stream object.
                dataStream.Close();
                // Get the response.
                WebResponse response = request.GetResponse();
                // Display the status.
                Console.WriteLine(((HttpWebResponse)response).StatusDescription);
                // Get the stream containing content returned by the server.
                Stream rdataStream = response.GetResponseStream();
                // Open the stream using a StreamReader for easy access.
                StreamReader reader = new StreamReader(rdataStream);
                // Read the content.
                string responseFromServer = reader.ReadToEnd();
                // Display the content.
                MessageBox.Show(responseFromServer);
                // Clean up the streams and the response.
                reader.Close();
                response.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnPinStruct_Click(object sender, EventArgs e)
        {
            List<PinSetup> psa = new List<PinSetup>();
            PinSetup ps;

            try {
                //Set values on screen into struct
                if (this.txtNumStructs.Text.ToString().Trim().Equals(""))
                {
                    this.txtNumStructs.Text = "1";
                    this.txtNumStructs.Update();
                }

                for (int i = 0; i < int.Parse(this.txtNumStructs.Text.ToString().Trim()); i++)
                {
                    //Create a new struct
                    ps = new PinSetup();

                    if (this.txtNumPixels.Text.ToString().Trim().Equals(""))
                    {
                        this.txtNumPixels.Text = "10";
                        this.txtNumPixels.Update();
                    }

                    if (this.txtDataPin.Text.ToString().Trim().Equals(""))
                    {
                        this.txtDataPin.Text = "4";
                        this.txtDataPin.Update();
                    }

                    if (this.txtClockPin.Text.ToString().Trim().Equals(""))
                    {
                        this.txtClockPin.Text = "5";
                        this.txtClockPin.Update();
                    }

                    //Set struct data
                    ps.numpixels = short.Parse(this.txtNumPixels.Text.ToString().Trim());
                    ps.datapin = byte.Parse(this.txtDataPin.Text.ToString().Trim());
                    ps.clockpin = byte.Parse(this.txtClockPin.Text.ToString().Trim()); ;
                    ps.dotstarType = 0;

                    //Add struct to array
                    psa.Add(ps);
                }

                //Send structs via http request to ESP8266
                sendStruct2ESP8266(psa);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnChangeColorStruct_Click(object sender, EventArgs e)
        {
            List<ChangePixelColor> cpca = new List<ChangePixelColor>();
            ChangePixelColor cpc;
            String[] sColor = { "" };

            try
            {
                //Set values on screen into struct
                if (this.txtNumStructs.Text.ToString().Trim().Equals(""))
                {
                    this.txtNumStructs.Text = "1";
                    this.txtNumStructs.Update();
                }

                for (int i = 0; i < int.Parse(this.txtNumStructs.Text.ToString().Trim()); i++)
                {
                    //Create a new struct
                    cpc = new ChangePixelColor();

                    if (this.txtPixelIndex.Text.ToString().Trim().Equals(""))
                    {
                        this.txtPixelIndex.Text = "0";
                        this.txtPixelIndex.Update();
                    }

                    //Split combo box by - and get color
                    sColor = this.cboxColor.SelectedItem.ToString().Trim().Split('-');

                    //Set struct data
                    cpc.pixelIndex = short.Parse(this.txtPixelIndex.Text.ToString().Trim());
                    cpc.color = Convert.ToInt32(sColor[1].Trim(), 16);

                    //Add struct to array
                    cpca.Add(cpc);
                }

                //Send structs via http request to ESP8266
                sendStruct2ESP8266(cpca);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
