using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using System.Data;
using System.IO;

namespace LEDLightingComposer
{
    public class DatabaseManager
    {
        //Declare global variables
        private LEDLightingComposerCS llc;
        private Button add2Project, openProject, editRecord, send2MCUViaHTTP, send2SDCard;
        private DataGridView projectGrid;
        private Label projectName;
        private readonly String MCU_PINS = "MCU_PINS", MCU_PINS_OPTION1 = "PROJECTMCU", LED_PROJECT = "LED_PROJECT",
            LIGHTING_EFFECTS = "LIGHTING_EFFECTS", MCU = "MCU";
        MySqlConnection con, con2, con3;
        private String connString = "Server=localhost;Database=test;Uid=root;Pwd=user1234";

        public DatabaseManager(LEDLightingComposerCS LLC, Label ProjectName, Button Add2Project, Button EditRecord, Button Send2MCUViaHTTP, Button Send2SDCard, Button OpenProject, DataGridView ProjectGrid)
        {
            //Set global variables to passed variables
            this.llc = LLC;
            this.projectName = ProjectName;
            this.add2Project = Add2Project;
            this.editRecord = EditRecord;
            this.send2MCUViaHTTP = Send2MCUViaHTTP;
            this.send2SDCard = Send2SDCard;
            this.openProject = OpenProject;
            this.projectGrid = ProjectGrid;
        }

        /*
           Function loadCBoxByType:

        */
        public void loadCBoxByType(String Type, ComboBox CBox, String Value1, String Value2)
        {
            //Declare variables
            MySqlCommand cmd;
            MySqlDataReader rdr;

            //Open db connection
            OpenDBConnection();

            try
            {
                cmd = con.CreateCommand();

                switch (Type)
                {
                    case "PROJECTNAMES":
                        cmd.CommandText = "Select Project_Name, Description from LED_Project";
                        break;
                    case "MCUNAMES":
                        cmd.CommandText = "Select MCU_Name, Description from MCU";
                        break;
                    case "MCUPINS":
                        cmd.CommandText = "Select Data_Pin, Clock_Pin, Description from MCU_Pins where Project_Name = @PName and MCU_Name = @MName";
                        cmd.Parameters.AddWithValue("@PName", Value1);
                        cmd.Parameters.AddWithValue("@MName", Value2);
                        break;
                    case "LIGHTINGEFFECTS":
                        cmd.CommandText = "Select Lighting_Effect, Description from Lighting_Effects";
                        break;
                }
                
                rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    if (Type.Equals("MCUPINS"))
                    {
                        CBox.Items.Add(rdr.GetString(0) + ";" + rdr.GetString(1) + ";" + rdr.GetString(2));
                    }
                    else
                    {
                        CBox.Items.Add(rdr.GetString(0) + " - " + rdr.GetString(1));
                    }
                }
                rdr.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading " + Type + " to combobox: " + ex.Message);
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
                cmd.CommandText = "Select LE.EFFECT_NUM, LE.PROJECT_NAME, LP.Description as PROJECT_DESC, LE.MCU_NAME, M.Description as MCU_DESC, LE.PIN_SETUP, MP.DATA_PIN, MP.CLOCK_PIN, " +
                    "LE.NUM_LEDS, LE.LIGHTING_EFFECT, LES.DESCRIPTION, LE.EFFECT_START, LE.EFFECT_DURATION, (LE.Effect_Start + LE.Effect_Duration) as ENDOFEFFECT, LE.LED_POSITION_ARRAY, LE.LED_COLOR_ARRAY " +
                    "from Led_Effect LE, MCU_Pins MP, Lighting_Effects LES, MCU M, LED_Project LP where MP.PIN_SETUP = LE.PIN_SETUP and LES.Lighting_Effect = LE.Lighting_Effect "+
                    "and M.MCU_NAME = LE.MCU_NAME and LP.Project_Name = LE.Project_Name and LE.Project_Name = @PName order by LE.Pin_Setup, LE.Effect_Start";
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
        public bool updateProjectsInProjectGrid(String ProjectName)
        {
            //Declare variables
            bool bRet = false;

            try
            {
                //Update datagridview
                loadProjects2ProjectGrid(ProjectName, projectGrid);

                //Clear drawing manager's led strip array
                llc.DrawManager.LedStrips.Clear();

                //Load led strips and effects into drawing manager
                loadLEDStripEffectsIntoDrawingManager(projectGrid, llc.DrawManager, llc.getDrawingBottom(), llc.getDrawingRight());

                bRet = true;
            }catch(Exception ex)
            {

            }

            return bRet;
        }

        /*
        */
        public int loadLEDStripEffectsIntoDrawingManager(DataGridView dgv, DrawingManager dmanager, int bottom, int right)
        {
            int pinSetup, iret = 0, tp = 0;
            bool skip = false;

            //Add all led strips from grid into drawing manager
            foreach (DataGridViewRow row in dgv.Rows)
            {
                iret += 1;

                //Set pin setup
                pinSetup = int.Parse(row.Cells["PIN_SETUP"].Value.ToString().Trim());

                //If the Drawing Manager's led strip is empty, then start top - left at 20-20, otherwise start at last led strips' last led top + 30
                if (dmanager.LedStrips.Count < 1)
                {
                    //Start at top 20, left 20
                    tp = 20;
                }
                else
                {
                    //Start at top == last strips' led top + 30, left 20
                    tp = dmanager.LedStrips[(dmanager.LedStrips.Count - 1)].Leds[(dmanager.LedStrips[(dmanager.LedStrips.Count - 1)].Leds.Count - 1)].Top + 30;
                }

                //Add LEDs to strip if strip not already added
                skip = false;
                foreach(LEDStripEffect lse in dmanager.LedStrips)
                {
                    if(lse.PinSetup == pinSetup)
                    {
                        skip = true;
                        break;
                    }
                }

                if (!skip)
                {
                    dmanager.LedStrips.Add(new LEDStripEffect(row.Cells["MCU_NAME"].Value.ToString().Trim(), int.Parse(row.Cells["NUM_LEDS"].Value.ToString().Trim()), row.Cells["LED_COLOR_ARRAY"].Value.ToString().Trim().Split(';'), int.Parse(row.Cells["LIGHTING_EFFECT"].Value.ToString().Trim()), int.Parse(row.Cells["EFFECT_START"].Value.ToString().Trim()), int.Parse(row.Cells["EFFECT_DURATION"].Value.ToString().Trim()), pinSetup, tp, 20, bottom, right, true));
                }
            }
            return iret;
        }

        /*
        */
        public bool verifyNameExistsInDatabase(String Table, String Option1, String Value1, String Value2, String Value3, String Value4)
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
                        if (Option1.Equals(MCU_PINS_OPTION1))
                        {
                            cmd.CommandText = "Select Pin_Setup from MCU_Pins where Project_Name = @PName and MCU_Name = @CName";
                            cmd.Parameters.AddWithValue("@PName", Value1);
                            cmd.Parameters.AddWithValue("@CName", Value2);
                        }
                        else
                        {
                            cmd.CommandText = "Select Pin_Setup from MCU_Pins where Project_Name = @PName and MCU_Name = @CName and Data_Pin = @DPin and Clock_Pin = @CPin";
                            cmd.Parameters.AddWithValue("@PName", Value1);
                            cmd.Parameters.AddWithValue("@CName", Value2);
                            cmd.Parameters.AddWithValue("@DPin", int.Parse(Value3));
                            cmd.Parameters.AddWithValue("@CPin", int.Parse(Value4));
                        }
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
        public bool verifyOverlappingLightingEffects(String ProjectName, int TruePinSetupVal, int EffectStart, int EffectDuration)
        {
            //Declare variables
            MySqlCommand cmd;
            MySqlDataReader rdr;
            bool bret = false;
            int endOfEffect = EffectStart + EffectDuration, iret = 0;

            try
            {
                //Open db connection
                OpenDBConnection();
                cmd = con.CreateCommand();

                //Conjure query string depending on passed table name
                cmd.CommandText = "Select Effect_Start, (Effect_Start + Effect_Duration) as EndOfEffect from LED_Effect where Project_Name = @Name and Pin_Setup = @PSetup "+
                    "where Effect_Start between @BegEffect and @EndEffect or (Effect_Start + Effect_Duration) between @BegEffect and @EndEffect";
                cmd.Parameters.AddWithValue("@Name", ProjectName);
                cmd.Parameters.AddWithValue("@PSetup", TruePinSetupVal);
                cmd.Parameters.AddWithValue("@BegEffect", EffectStart);
                cmd.Parameters.AddWithValue("@EndEffect", endOfEffect);
                
                rdr = cmd.ExecuteReader();
                if (rdr.Read())
                {
                    iret = rdr.GetInt32(0);
                    bret = true;
                }
                rdr.Close();

                //Close db connection
                CloseDBConnection();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error verifying if overlapping exists: " + ex.Message);

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
           Function updateRecordInDB:

           Param1 = Effect_Num
           Value1 = Num_Leds, Value2 = LED_Position_Array, Value3 = LED_Color_Array, Value4 = Lighting_Effect,
           Value5 = Effect_Start, Value6 = Effect_Duration
        */
        public int updateRecordInDB(String Table, String Param1, String Value1, String Value2, String Value3, String Value4, String Value5)
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
                    case "LED_EFFECT":
                        cmd.CommandText = "Update LED_Effect Set LED_Position_Array = @LPA, LED_Color_Array = @LCA, "+
                            "Lighting_Effect = @LEffect, Effect_Start = @EStart, Effect_Duration = @EDuration where Effect_Num = @ENum";
                        cmd.Parameters.AddWithValue("@ENum", int.Parse(Param1));
                        cmd.Parameters.AddWithValue("@LPA", Value1);
                        cmd.Parameters.AddWithValue("@LCA", Value2);
                        cmd.Parameters.AddWithValue("@LEffect", int.Parse(Value3));
                        cmd.Parameters.AddWithValue("@EStart", int.Parse(Value4));
                        cmd.Parameters.AddWithValue("@EDuration", int.Parse(Value5));
                        break;
                    case "LED_PROJECT":

                        break;
                    case "LIGHTING_EFFECTS":

                        break;
                    case "MCU":

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
                MessageBox.Show("Error updating record in table " + Table + ": " + ex.Message);

            }

            return iret;
        }

        /*
        */
        public int deleteRecordFromDB(String Table, String Param1)
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
                    case "LED_EFFECT":
                        cmd.CommandText = "Delete from LED_Effect where Effect_Num = @ENum";
                        cmd.Parameters.AddWithValue("@ENum", int.Parse(Param1));
                        break;
                    case "LED_PROJECT":
                        
                        break;
                    case "LIGHTING_EFFECTS":
                        
                        break;
                    case "MCU":
                        
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
                MessageBox.Show("Error deleting record from table " + Table + ": " + ex.Message);

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
        public void writeProjectFromDB2StructsLocalFile(String ProjectName, FileStream StripSetupFile, FileStream LEffectsFile)
        {
            //Declare variables
            MySqlCommand cmd, cmd2;
            MySqlDataReader rdr, rdr2;
            StreamWriter stripSetupW1 = new StreamWriter(StripSetupFile);
            StreamWriter lEffectsW2 = new StreamWriter(LEffectsFile);
            String projectName = "", pinSetup = "";
            int stripCount = 0, lEffectsCount = 0, loopCount1 = -1, loopCount2 = -1;
            bool first1 = true, first2 = true;

            try
            {
                //Open db connection
                OpenDBConnection();
                cmd = con.CreateCommand();

                //Conjure query string
                cmd.CommandText = "Select Distinct LE.PROJECT_NAME, LE.PIN_SETUP, LE.NUM_LEDS, MP.DATA_PIN, MP.CLOCK_PIN from Led_Effect LE, MCU_Pins MP where MP.PIN_SETUP = LE.PIN_SETUP "+
                    "and LE.Project_Name = @PName order by LE.Pin_Setup";
                cmd.Parameters.AddWithValue("@PName", ProjectName);

                rdr = cmd.ExecuteReader();
                while(rdr.Read())
                {
                    //Get count of strips to be setup
                    projectName = rdr["PROJECT_NAME"].ToString().Trim();
                    pinSetup = rdr["PIN_SETUP"].ToString().Trim();

                    //Add 1 to loopCount1
                    loopCount1 += 1;
                    if (loopCount1 > 0)
                    {
                        first1 = false;
                    }
                    else
                    {
                        stripCount = getInstancesCount("LED_EFFECT", "STRIPS", projectName, pinSetup);
                    }

                    //Write Strip setup information to StripSetupFile
                    stripSetupW1.Write(createStructInfo("STRIP", first1, false, stripCount.ToString(), loopCount1.ToString(), rdr["NUM_LEDS"].ToString().Trim(), rdr["DATA_PIN"].ToString().Trim(), rdr["CLOCK_PIN"].ToString().Trim(),"","","","","","",""));
                    stripSetupW1.Flush();

                    //Loop through all distinct lighting effects and write to LEffectsFile
                    OpenDBConnection3();
                    cmd2 = con3.CreateCommand();
                    cmd2.CommandText = "Select PROJECT_NAME, PIN_SETUP, LIGHTING_EFFECT, NUM_LEDS, LED_POSITION_ARRAY, LED_COLOR_ARRAY, EFFECT_START, EFFECT_DURATION " +
                    "from Led_Effect where Project_Name = @PName and Pin_Setup = @PSetup order by Pin_Setup, Effect_Start";
                    cmd2.Parameters.AddWithValue("@PName", projectName);
                    cmd2.Parameters.AddWithValue("@PSetup", int.Parse(pinSetup));

                    rdr2 = cmd2.ExecuteReader();
                    while (rdr2.Read())
                    {
                        //Get count of lighting effects
                        projectName = rdr2["PROJECT_NAME"].ToString().Trim();
                        pinSetup = rdr2["PIN_SETUP"].ToString().Trim();

                        //Add 1 to loopCount2
                        loopCount2 += 1;
                        if (loopCount2 > 0)
                        {
                            first1 = false;
                            first2 = false;
                        }
                        else
                        {
                            lEffectsCount = getInstancesCount("LED_EFFECT", "LEFFECTS", projectName, pinSetup);
                        }

                        //Write Lighting Effects information to LEffectsFile
                        lEffectsW2.Write(createStructInfo("LEFFECTS", first1, first2, stripCount.ToString(), lEffectsCount.ToString(), loopCount1.ToString(), loopCount2.ToString(), rdr2["Lighting_Effect"].ToString().Trim(), rdr2["NUM_LEDS"].ToString().Trim(), rdr2["LED_Position_Array"].ToString().Trim(), rdr2["LED_Color_Array"].ToString().Trim(), rdr2["Effect_Start"].ToString().Trim(), rdr2["Effect_Duration"].ToString().Trim(),"0","0"));
                        lEffectsW2.Flush();
                    }
                    //Close datareader and db connection
                    rdr2.Close();
                    CloseDBConnection3();

                    //Reset first2 and loopCount2
                    first2 = true;
                    loopCount2 = -1;

                    //Write new line to separate next strip
                    lEffectsW2.Write(System.Environment.NewLine);
                }
                //Close datareader and db connection
                rdr.Close();
                CloseDBConnection();

                //Notify user
                MessageBox.Show("Project successfully written to file as structs...");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error writing Project from DB to structs within file...: " + ex.Message);
                CloseDBConnection();
                CloseDBConnection3();
            }
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

        /*
         * 
        */
        public int getNumLEDs(String ProjectName, String MCUName, int PinSetup)
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
                cmd.CommandText = "Select Distinct Num_Leds from LED_Effect where Project_Name = @PName and MCU_Name = @CName and Pin_Setup = @Pin order by Num_Leds desc";
                cmd.Parameters.AddWithValue("@PName", ProjectName);
                cmd.Parameters.AddWithValue("@CName", MCUName);
                cmd.Parameters.AddWithValue("@Pin", PinSetup);

                rdr = cmd.ExecuteReader();
                if (rdr.Read())
                {
                    iret = rdr.GetInt32(0);
                }
                rdr.Close();

                //Close db connection
                CloseDBConnection();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error getting true Pin Setup value...: " + ex.Message);
            }

            return iret;
        }

        /*
        */
        public string getValueFromDB(String Table, String Option, String Param1, String Param2, String Param3)
        {
            //Declare variables
            MySqlCommand cmd;
            MySqlDataReader rdr;
            String sret = "";

            try
            {
                //Open db connection
                OpenDBConnection();
                cmd = con.CreateCommand();

                switch (Table)
                {
                    case "LED_EFFECT":
                        if (Option.Equals("LEDPARRAY"))
                        {
                            //Conjure query string
                            cmd.CommandText = "Select LED_Position_Array from LED_Effect where Project_Name = @PName and MCU_Name = @MName and Pin_Setup = @PSetup";
                            cmd.Parameters.AddWithValue("@PName", Param1);
                            cmd.Parameters.AddWithValue("@MName", Param2);
                            cmd.Parameters.AddWithValue("@PSetup", int.Parse(Param3));
                        }
                        else
                        {
                            //Conjure query string
                            cmd.CommandText = "Select LED_Color_Array from LED_Effect where Project_Name = @PName and MCU_Name = @MName and Pin_Setup = @PSetup";
                            cmd.Parameters.AddWithValue("@PName", Param1);
                            cmd.Parameters.AddWithValue("@MName", Param2);
                            cmd.Parameters.AddWithValue("@PSetup", int.Parse(Param3));
                        }
                        break;
                }

                rdr = cmd.ExecuteReader();
                if (rdr.Read())
                {
                    sret = rdr.GetString(0);
                }
                rdr.Close();

                //Close db connection
                CloseDBConnection();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error getting " + Option + " from " + Table + ": " + ex.Message);
            }

            return sret;
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
        private bool OpenDBConnection2()
        {
            bool bret = false;

            try
            {
                //Connect to database
                con2 = new MySqlConnection(connString);
                con2.Open();
                bret = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error connecting to database: " + ex.Message);
            }

            return bret;
        }

        /*
        */
        private bool OpenDBConnection3()
        {
            bool bret = false;

            try
            {
                //Connect to database
                con3 = new MySqlConnection(connString);
                con3.Open();
                bret = true;
            }
            catch (Exception ex)
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

        /*
        */
        private bool CloseDBConnection2()
        {
            bool bret = false;

            try
            {
                //Disconnect from database
                con2.Close();
                bret = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error disconnecting from database: " + ex.Message);
            }

            return bret;
        }

        /*
        */
        private bool CloseDBConnection3()
        {
            bool bret = false;

            try
            {
                //Disconnect from database
                con3.Close();
                bret = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error disconnecting from database: " + ex.Message);
            }

            return bret;
        }

        /*
           Function createStructInfo:
           This function will create struct like info for writing to file for passed info
        */
        private String createStructInfo(String Type, bool Initialize1, bool Initialize2, String Value1, String Value2, String Value3, String Value4, String Value5, String Value6, String Value7, String Value8, String Value9, String Value10, String Value11, String Value12)
        {
            //Declare variables
            String sRet = "";

            //Conjure string by type
            switch (Type)
            {
                case "STRIP":
                    if (Initialize1)
                    {
                        sRet += "strips = (StripInfo**)calloc(" + Value1 + ", sizeof(StripInfo*))" + System.Environment.NewLine + System.Environment.NewLine;
                    }
                    sRet += "strips["+ Value2 +"] = (StripInfo*)calloc(1, sizeof(StripInfo))" + System.Environment.NewLine;
                    sRet += "strips[" + Value2 + "]->NumPixels = " + Value3 + System.Environment.NewLine;
                    sRet += "strips[" + Value2 + "]->Datapin = " + Value4 + System.Environment.NewLine;
                    sRet += "strips[" + Value2 + "]->ClockPin = " + Value5 + System.Environment.NewLine + System.Environment.NewLine;

                    break;
                case "LEFFECTS":
                    if (Initialize1)
                    {
                        //Initialize array to hold values for all strips
                        sRet += "leffects = (LightingEffect**)calloc(" + Value1 + ", sizeof(LightingEffect*))" + System.Environment.NewLine;
                    }
                    if (Initialize2)
                    {
                        //Initialize 2nd dimension of array to hold all lighting effects that pertain to this particular strip
                        sRet += "leffects[" + Value3 + "] = (LightingEffect*)calloc(" + Value2 + ", sizeof(LightingEffect))" + System.Environment.NewLine;
                    }
                    //Add lighting effect info to strip
                    sRet += "leffects[" + Value3 + "][" + Value4 + "] = {" + Value5 + ", " + Value6 + ", \"" + Value7 + "\", \"" + Value8 + "\", " + Value9 + ", " + Value10 + ", " + Value11 + ", " + Value12 + "}" + System.Environment.NewLine;

                    break;
            }

            return sRet;
        }

        /*
        */
        public int getInstancesCount(String Table, String Option, String Param1, String Param2)
        {
            //Declare variables
            MySqlCommand cmd;
            MySqlDataReader rdr;
            int iret = -1;

            try
            {
                //Open db connection
                OpenDBConnection2();

                cmd = con2.CreateCommand();

                //Conjure query string
                switch (Table)
                {
                    case "LED_EFFECT":
                        if (Option.Equals("STRIPS"))
                        {
                            cmd.CommandText = "Select Count(*) from LED_Effect where Project_Name = @PName and Pin_Setup = @Pin group by Pin_Setup";
                            cmd.Parameters.AddWithValue("@PName", Param1);
                            cmd.Parameters.AddWithValue("@Pin", Param2);
                        }else
                        {
                            cmd.CommandText = "Select Count(*) from LED_Effect where Project_Name = @PName and Pin_Setup = @Pin";
                            cmd.Parameters.AddWithValue("@PName", Param1);
                            cmd.Parameters.AddWithValue("@Pin", Param2);
                        }
                        break;
                }

                rdr = cmd.ExecuteReader();
                if (rdr.Read())
                {
                    if (rdr.GetString(0).Trim().Equals(""))
                    {
                        iret = 0;
                    }
                    else
                    {
                        iret = rdr.GetInt32(0);
                    }
                }
                rdr.Close();

                //Close db connection
                CloseDBConnection2();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error getting true Pin Setup value...: " + ex.Message);
            }

            return iret;
        }

        #endregion Private Methods


        #region Screen Events

        /*
        */
        public void dgvProjectData_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            btnEditRecord_Click(null, null);
        }

        /*
        */
        public void btnEditRecord_Click(object sender, EventArgs e)
        {
            //Declare variables
            int selectedRow, numOfLEDs, effectStart, effectDuration, effectNum;
            String projectName, mcuName, pinSetup, lightingEffect, currentSongPath;
            List<int> ledPArray = new List<int>();
            List<String> ledCArray = new List<string>();
            String[] temp = { "" };

            //Verify a record is selected within grid
            if (projectGrid.CurrentRow != null && projectGrid.CurrentCell != null)
            {

                //Get values from selected record
                selectedRow = projectGrid.CurrentRow.Index;
                currentSongPath = "";
                projectName = projectGrid.Rows[selectedRow].Cells["PROJECT_NAME"].Value.ToString().Trim() + " - " + projectGrid.Rows[selectedRow].Cells["PROJECT_DESC"].Value.ToString().Trim();
                mcuName = projectGrid.Rows[selectedRow].Cells["MCU_NAME"].Value.ToString().Trim() + " - " + projectGrid.Rows[selectedRow].Cells["MCU_DESC"].Value.ToString().Trim();
                pinSetup = projectGrid.Rows[selectedRow].Cells["DATA_PIN"].Value.ToString().Trim() + ";" + projectGrid.Rows[selectedRow].Cells["CLOCK_PIN"].Value.ToString().Trim() + ";";
                numOfLEDs = int.Parse(projectGrid.Rows[selectedRow].Cells["NUM_LEDS"].Value.ToString().Trim());

                temp = projectGrid.Rows[selectedRow].Cells["LED_POSITION_ARRAY"].Value.ToString().Split(';');
                foreach(String s in temp)
                {
                    ledPArray.Add(int.Parse(s));
                }
                temp = projectGrid.Rows[selectedRow].Cells["LED_COLOR_ARRAY"].Value.ToString().Split(';');
                foreach(String s in temp)
                {
                    ledCArray.Add(s);
                }
                lightingEffect = projectGrid.Rows[selectedRow].Cells["LIGHTING_EFFECT"].Value.ToString().Trim() + " - " + projectGrid.Rows[selectedRow].Cells["DESCRIPTION"].Value.ToString().Trim();
                effectStart = int.Parse(projectGrid.Rows[selectedRow].Cells["EFFECT_START"].Value.ToString().Trim());
                effectDuration = int.Parse(projectGrid.Rows[selectedRow].Cells["EFFECT_DURATION"].Value.ToString().Trim());
                effectNum = int.Parse(projectGrid.Rows[selectedRow].Cells["EFFECT_NUM"].Value.ToString().Trim());

                //Open project form with initial values for timer and song file path
                Project pj = new Project(this, projectName, mcuName, pinSetup, numOfLEDs, ledPArray, ledCArray, lightingEffect, effectStart, effectDuration, effectNum, currentSongPath, false);
                //pj.Owner = (LEDLightingComposerCS)sender;
                pj.Show();
            }
        }

        /*
        */
        public void btnAdd2Project_Click(object sender, EventArgs e, LEDLightingComposerCS LLC, int TimerVal, String CurrentSongPath)
        {
            //Open project form with initial values for timer and song file path
            Project pj = new Project(this, TimerVal, CurrentSongPath, true);
            pj.Owner = LLC;
            pj.Show();
        }

        /*
        */
        public void btnOpenProject_Click(object sender, EventArgs e, LEDLightingComposerCS LLC, DrawingManager DManager)
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
           Function btnSend2LocalFile_Click:
           This function will write all project led effects to user designated file as structs for copying into microcontroller code
        */
        public void btnSend2LocalFile_Click(object sender, EventArgs e)
        {
            //Declare variables
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            FileStream stripSetupFile = null, leffectsFile = null;
            String projectName = "", stripSetupFilename = "", leffectsFilename = "";

            //Verify records exist within grid
            if (projectGrid.Rows.Count <= 0)
            {
                //Notify user records must be within grid before sending to local file
                MessageBox.Show("You must load a project into grid before sending microcontroller information to file...");
                return;
            }

            //Allow user to choose file path and name
            saveFileDialog1.Filter = "Notepad|*.txt";
            saveFileDialog1.Title = "Save Microcontroller Lighting Effect Text File";
            saveFileDialog1.ShowDialog();

            // If the file name is not an empty string open it for saving.
            if (saveFileDialog1.FileName != "")
            {
                //Conjure filenames for Strip and LightingEffects files
                stripSetupFilename = saveFileDialog1.FileName.Split('.')[0] + "-STRIP" + ".txt";
                leffectsFilename = saveFileDialog1.FileName.Split('.')[0] + "-LEFFECTS" + ".txt";

                // Saves the Text via a FileStream created by the OpenFile method.
                saveFileDialog1.FileName = stripSetupFilename;
                stripSetupFile = (System.IO.FileStream)saveFileDialog1.OpenFile();
                saveFileDialog1.FileName = leffectsFilename;
                leffectsFile = (System.IO.FileStream)saveFileDialog1.OpenFile();
            }
            else
            {
                //Notify user file was not created successfully
                MessageBox.Show("File was not created successfully.  Please try again...");
                return;
            }

            //Get project name from datagridview
            projectName = projectGrid.Rows[0].Cells["PROJECT_NAME"].Value.ToString().Trim();

            //Loop through LED_Effect table for project and write structs to user selected file path and name
            writeProjectFromDB2StructsLocalFile(projectName, stripSetupFile, leffectsFile);

            //Close file
            stripSetupFile.Close();
            leffectsFile.Close();
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

        #region Getters & Setters

        public String LED_Project
        {
            get { return LED_PROJECT; }
        }

        public String Lighting_Effects
        {
            get { return LIGHTING_EFFECTS; }
        }

        public String Mcu
        {
            get { return MCU; }
        }

        public String MCU_Pins
        {
            get { return MCU_PINS; }
        }

        public String MCU_Pins_Option1
        {
            get { return MCU_PINS_OPTION1; }
        }

        public DataGridView ProjectGrid
        {
            get { return this.projectGrid; }
        }

        #endregion Getters & Setters
    }
}
