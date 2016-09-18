using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using System.Data;

namespace LEDLightingComposer
{
    public class DatabaseManager
    {
        //Declare global variables
        private Button add2Project, openProject, editRecord, send2MCUViaHTTP, send2SDCard;
        private DataGridView projectGrid;
        private Label projectName;
        MySqlConnection con;
        private String connString = "Server=localhost;Database=test;Uid=root;Pwd=user1234";

        public DatabaseManager(Label ProjectName, Button Add2Project, Button EditRecord, Button Send2MCUViaHTTP, Button Send2SDCard, Button OpenProject, DataGridView ProjectGrid)
        {
            //Set global variables to passed variables
            this.projectName = ProjectName;
            this.add2Project = Add2Project;
            this.editRecord = EditRecord;
            this.send2MCUViaHTTP = Send2MCUViaHTTP;
            this.send2SDCard = Send2SDCard;
            this.openProject = OpenProject;
            this.projectGrid = ProjectGrid;
        }

        /*
        */
        public void loadProjectNames(ComboBox ProjectNames)
        {
            //Declare variables
            MySqlCommand cmd;
            MySqlDataReader rdr;

            //Open db connection
            OpenDBConnection();

            try
            {
                cmd = con.CreateCommand();
                cmd.CommandText = "Select Project_Name, Description from LED_Project";
                rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    ProjectNames.Items.Add(rdr.GetString(0) + " - " + rdr.GetString(1));
                }
                rdr.Close();
            }catch(Exception ex)
            {
                MessageBox.Show("Error loading project names to combobox: " + ex.Message);
            }

            //Close db connection
            CloseDBConnection();
        }

        /*
        */
        public void loadMCUNames(ComboBox MCUNames)
        {
            //Declare variables
            MySqlCommand cmd;
            MySqlDataReader rdr;

            //Open db connection
            OpenDBConnection();

            try
            {
                cmd = con.CreateCommand();
                cmd.CommandText = "Select MCU_Name, Description from MCU";
                rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    MCUNames.Items.Add(rdr.GetString(0) + " - " + rdr.GetString(1));
                }
                rdr.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading project names to combobox: " + ex.Message);
            }

            //Close db connection
            CloseDBConnection();
        }

        /*
        */
        public void loadLightingEffects(ComboBox LightingEffects)
        {
            //Declare variables
            MySqlCommand cmd;
            MySqlDataReader rdr;

            //Open db connection
            OpenDBConnection();

            try
            {
                cmd = con.CreateCommand();
                cmd.CommandText = "Select Lighting_Effect, Description from Lighting_Effects";
                rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    LightingEffects.Items.Add(rdr.GetString(0) + " - " + rdr.GetString(1));
                }
                rdr.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading project names to combobox: " + ex.Message);
            }

            //Close db connection
            CloseDBConnection();
        }

        /*
        */
        public void loadAllProjects2SearchGrid(DataGridView dgv)
        {
            //Declare variables
            MySqlCommand cmd = null;
            MySqlDataAdapter adap = null;
            DataSet ds = null;

            try
            {
                //Open db connection
                OpenDBConnection();

                cmd = con.CreateCommand();
                cmd.CommandText = "Select * from LED_Project";
                adap = new MySqlDataAdapter(cmd);
                ds = new DataSet();
                adap.Fill(ds);
                dgv.DataSource = ds.Tables[0].DefaultView;

                //Close db connection
                CloseDBConnection();
            }catch(Exception ex)
            {
                MessageBox.Show("Error loading all projects to search browser...: " + ex.Message);
            }
        }

        /*
        */
        public int loadProjects2ProjectGrid(String ProjectName, DataGridView dgv)
        {
            //Declare variables
            MySqlCommand cmd = null;
            MySqlDataAdapter adap = null;
            DataSet ds = null;
            int iret = -1;

            try
            {
                //Open db connection
                OpenDBConnection();

                cmd = con.CreateCommand();
                cmd.CommandText = "Select * from LED_Effect where Project_Name = @PName";
                cmd.Parameters.AddWithValue("@PName", ProjectName);
                adap = new MySqlDataAdapter(cmd);
                ds = new DataSet();
                adap.Fill(ds);
                dgv.DataSource = ds.Tables[0].DefaultView;

                iret = 1;

                //Close db connection
                CloseDBConnection();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading " + ProjectName +  " project to Project Grid...: " + ex.Message);
            }

            return iret;
        }

        /*
        */
        public int loadLEDStripEffectsIntoDrawingManager(DataGridView dgv, DrawingManager dmanager, int bottom, int right)
        {
            int iret = 0, tp = 0;

            //Add all led strips from grid into drawing manager
            foreach (DataGridViewRow row in dgv.Rows)
            {
                iret += 1;

                //If the Drawing Manager's led strip is empty, then start top - left at 20-20, otherwise start at last led strips' last led top + 30
                if(dmanager.LedStrips.Count < 1)
                {
                    //Start at top 20, left 20
                    dmanager.LedStrips.Add(new LEDStripEffect(row.Cells[2].Value.ToString().Trim(),int.Parse(row.Cells[4].Value.ToString().Trim()), row.Cells[9].Value.ToString().Trim().Split(';'),int.Parse(row.Cells[5].Value.ToString().Trim()), int.Parse(row.Cells[6].Value.ToString().Trim()),int.Parse(row.Cells[7].Value.ToString().Trim()), 20, 20, bottom, right));
                }
                else
                {
                    //Start at top == last strips' led top + 30, left 20
                    tp = dmanager.LedStrips[(dmanager.LedStrips.Count - 1)].Leds[(dmanager.LedStrips[(dmanager.LedStrips.Count - 1)].Leds.Count - 1)].Top + 30;
                    dmanager.LedStrips.Add(new LEDStripEffect(row.Cells[2].Value.ToString().Trim(), int.Parse(row.Cells[4].Value.ToString().Trim()), row.Cells[9].Value.ToString().Trim().Split(';'), int.Parse(row.Cells[5].Value.ToString().Trim()), int.Parse(row.Cells[6].Value.ToString().Trim()), int.Parse(row.Cells[7].Value.ToString().Trim()), tp, 20, bottom, right));
                }
            }
            return iret;
        }

        /*
        */
        public bool verifyNameExistsInDatabase(String Table, String Value1, String Value2, String Value3, String Value4)
        {
            //Declare variables
            MySqlCommand cmd;
            MySqlDataReader rdr;
            bool bret = false;

            try
            {
                //Open db connection
                OpenDBConnection();
                cmd = con.CreateCommand();

                //Conjure query string depending on passed table name
                switch (Table)
                {
                    case "LED_PROJECT":
                        cmd.CommandText = "Select Project_Name from LED_Project where Project_Name = @Name";
                        cmd.Parameters.AddWithValue("@Name", Value1);
                        break;
                    case "LIGHTING_EFFECTS":
                        cmd.CommandText = "Select Lighting_Effect from Lighting_Effects where Lighting_Effect = @Name";
                        cmd.Parameters.AddWithValue("@Name", Value1);
                        break;
                    case "MCU":
                        cmd.CommandText = "Select MCU_Name from MCU where MCU_Name = @Name";
                        cmd.Parameters.AddWithValue("@Name", Value1);
                        break;
                    case "MCU_PINS":
                        cmd.CommandText = "Select Pin_Setup from MCU_Pins where Project_Name = @PName and MCU_Name = @CName and Data_Pin = @DPin and Clock_Pin = @CPin";
                        cmd.Parameters.AddWithValue("@PName", Value1);
                        cmd.Parameters.AddWithValue("@CName", Value2);
                        cmd.Parameters.AddWithValue("@DPin", int.Parse(Value3));
                        cmd.Parameters.AddWithValue("@CPin", int.Parse(Value4));
                        break;
                    default:
                        return false;
                }

                rdr = cmd.ExecuteReader();
                if (rdr.Read())
                {
                    bret = true;
                }
                rdr.Close();

                //Close db connection
                CloseDBConnection();
            }
            catch(Exception ex)
            {
                MessageBox.Show("Error verifying name exists in table " + Table + ": " + ex.Message);

            }

            return bret;
        }

        /*
        */
        public int insertRecordIntoDB(String Table, String Value1, String Value2, String Value3, String Value4)
        {
            //Declare variables
            MySqlCommand cmd;
            int iret = 0;

            try
            {
                //Open db connection
                OpenDBConnection();
                cmd = con.CreateCommand();

                //Conjure query string depending on passed table name
                switch (Table)
                {
                    case "LED_PROJECT":
                        cmd.CommandText = "Insert into LED_Project Values(@Name, @Desc)";
                        cmd.Parameters.AddWithValue("@Name", Value1);
                        cmd.Parameters.AddWithValue("@Desc", Value2);
                        break;
                    case "LIGHTING_EFFECTS":
                        cmd.CommandText = "Insert into Lighting_Effects Values(@Name, @Desc)";
                        cmd.Parameters.AddWithValue("@Name", Value1);
                        cmd.Parameters.AddWithValue("@Desc", Value2);
                        break;
                    case "MCU":
                        cmd.CommandText = "Insert into MCU Values(@Name, @Desc)";
                        cmd.Parameters.AddWithValue("@Name", Value1);
                        cmd.Parameters.AddWithValue("@Desc", Value2);
                        break;
                    default:
                        return 0;
                }

                iret = cmd.ExecuteNonQuery();

                //Close db connection
                CloseDBConnection();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error verifying name exists in table " + Table + ": " + ex.Message);

            }

            return iret;
        }

        /*
        */
        public int insertRecordIntoDBReturnIncr(String Table, String Value1, String Value2, String Value3, String Value4, String Value5, String Value6, String Value7, String Value8, String Value9)
        {
            //Declare variables
            MySqlCommand cmd;
            int iret = -1;

            try
            {
                //Open db connection
                OpenDBConnection();
                cmd = con.CreateCommand();

                //Conjure query string depending on passed table name
                switch (Table)
                {
                    case "LED_EFFECT":
                        cmd.CommandText = "Insert into LED_Effect (Project_Name, MCU_Name, Pin_Setup, Num_LEDs, Lighting_Effect, Effect_Start, Effect_Duration, LED_Position_Array, LED_Color_Array) Values(@PName, @CName, @PSetup, @NumLEDs, @LEffect, @EffectStart, @EffectDuration, @LEDPArray, @LEDCArray)";
                        cmd.Parameters.AddWithValue("@PName", Value1);
                        cmd.Parameters.AddWithValue("@CName", Value2);
                        cmd.Parameters.AddWithValue("@PSetup", int.Parse(Value3));
                        cmd.Parameters.AddWithValue("@NumLEDs", int.Parse(Value4));
                        cmd.Parameters.AddWithValue("@LEffect", int.Parse(Value5));
                        cmd.Parameters.AddWithValue("@EffectStart", int.Parse(Value6));
                        cmd.Parameters.AddWithValue("@EffectDuration", int.Parse(Value7));
                        cmd.Parameters.AddWithValue("@LEDPArray", Value8);
                        cmd.Parameters.AddWithValue("@LEDCArray", Value9);
                        break;
                    case "MCU_PINS":
                        cmd.CommandText = "Insert into MCU_Pins (Description, Project_Name, MCU_Name, Data_Pin, Clock_Pin) Values(@Desc, @PName, @MName, @DPin, @CPin)";
                        cmd.Parameters.AddWithValue("@Desc", Value1);
                        cmd.Parameters.AddWithValue("@PName", Value2);
                        cmd.Parameters.AddWithValue("@MName", Value3);
                        cmd.Parameters.AddWithValue("@DPin", int.Parse(Value4));
                        cmd.Parameters.AddWithValue("@CPin", int.Parse(Value5));
                        break;
                    default:
                        return -1;
                }

                iret = cmd.ExecuteNonQuery();

                //Close db connection
                CloseDBConnection();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error verifying name exists in table " + Table + ": " + ex.Message);

            }

            return iret;
        }

        /*
        */
        public int getPinSetupValue(String ProjectName, String MCUName, String[] PinSetup)
        {
            //Declare variables
            MySqlCommand cmd;
            MySqlDataReader rdr;
            int iret = -1;

            try
            {
                //Open db connection
                OpenDBConnection();
                cmd = con.CreateCommand();

                //Conjure query string
                cmd.CommandText = "Select Pin_Setup from MCU_Pins where Project_Name = @PName and MCU_Name = @CName and Data_Pin = @DPin and Clock_Pin = @CPin";
                cmd.Parameters.AddWithValue("@PName", ProjectName);
                cmd.Parameters.AddWithValue("@CName", MCUName);
                cmd.Parameters.AddWithValue("@DPin", int.Parse(PinSetup[0].Trim()));
                cmd.Parameters.AddWithValue("@CPin", int.Parse(PinSetup[1].Trim()));

                rdr = cmd.ExecuteReader();
                if (rdr.Read())
                {
                    iret = rdr.GetInt32(0);
                }
                rdr.Close();

                //Close db connection
                CloseDBConnection();
            }
            catch(Exception ex)
            {
                MessageBox.Show("Error getting true Pin Setup value...: " + ex.Message);
            }

            return iret;
        }

        #region Private Methods

        /*
        */
        private bool OpenDBConnection()
        {
            bool bret = false;

            try {
                //Connect to database
                con = new MySqlConnection(connString);
                con.Open();
                bret = true;
            }catch(Exception ex)
            {
                MessageBox.Show("Error connecting to database: " + ex.Message);
            }

            return bret;
        }

        /*
        */
        private bool CloseDBConnection()
        {
            bool bret = false;

            try
            {
                //Disconnect from database
                con.Close();
                bret = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error disconnecting from database: " + ex.Message);
            }

            return bret;
        }

        #endregion Private Methods


        #region Screen Events

        /*
        */
        public void dgvProjectData_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            
        }

        /*
        */
        public void btnEditRecord_Click(object sender, EventArgs e)
        {

        }

        /*
        */
        public void btnAdd2Project_Click(object sender, EventArgs e, LEDLightingComposer LLC, int TimerVal, String CurrentSongPath)
        {
            //Open project form with initial values for timer and song file path
            Project pj = new Project(this, TimerVal, CurrentSongPath);
            pj.Owner = LLC;
            pj.Show();
        }

        /*
        */
        public void btnOpenProject_Click(object sender, EventArgs e, LEDLightingComposer LLC, DrawingManager DManager)
        {
            //Open Project Name form where user can select project to load to grid...
            SearchBrowser sb = new SearchBrowser(this.projectGrid, this, LLC, DManager);
            sb.Owner = LLC;
            sb.Show();
        }

        /*
        */
        public void btnSendViaHTTP_Click(object sender, EventArgs e)
        {

        }

        /*
        */
        public void btnSend2SDCard_Click(object sender, EventArgs e)
        {

        }

        /*
        */
        public void btnClearGrid_Click(object sender, EventArgs e, DrawingManager dmanager)
        {
            //Clear grid
            this.projectGrid.DataSource = null;

            //Clear Drawing Manager's LED Strip Effect
            dmanager.LedStrips.Clear();
        }

        #endregion Screen Events
    }
}
