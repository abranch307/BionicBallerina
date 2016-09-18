using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LEDLightingComposer
{
    public partial class SearchBrowser : Form
    {
        //Global Variables
        private DataGridView projectGrid;
        private DatabaseManager dbmanager;
        private LEDLightingComposerCS llc;
        private DrawingManager dmanager;

        public SearchBrowser(DataGridView ProjectGrid, DatabaseManager DBManager, LEDLightingComposerCS LLC, DrawingManager DManager)
        {
            InitializeComponent();

            //Set global to passed
            this.projectGrid = ProjectGrid;
            this.dbmanager = DBManager;
            this.llc = LLC;
            this.dmanager = DManager;

            //Load all projects into native datagrid
            dbmanager.loadAllProjects2SearchGrid(this.dataGridView1);
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            dataGridView1_CellDoubleClick(null, null);
        }

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            //Verify current row selected
            if (dataGridView1.CurrentRow != null && dataGridView1.CurrentCell != null)
            {
                //Load project into Project Grid
                if (dbmanager.loadProjects2ProjectGrid(dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells[0].Value.ToString().Trim(), projectGrid) > -1)
                {
                    //Clear drawing manager's led strip array
                    dmanager.LedStrips.Clear();

                    //Load led strips and effects into drawing manager
                    dbmanager.loadLEDStripEffectsIntoDrawingManager(projectGrid, dmanager, llc.getDrawingBottom(), llc.getDrawingRight());

                    //Close browser since Project loaded successfully
                    this.Close();
                }
            }
        }
    }
}
