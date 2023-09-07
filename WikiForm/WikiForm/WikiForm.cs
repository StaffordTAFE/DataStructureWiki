using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace WikiForm
{
    public partial class WikiForm : Form
    {
        //initialise array
        static readonly int rows = 12;
        static readonly int columns = 4;

        string[,] wikiTable = new string[rows, columns];

        // save cache
        string[,] wikiTableCache = new string[rows, columns];

        // display variables
        List<string> displayList = new List<string>(); // create list for name variables
        List<int> indexCache = new List<int>();

        //instance save system
        SaveSystem saveSystem = new SaveSystem();

        // search value
        string search;
        public WikiForm()
        {
            InitializeComponent();

            deleteToolStripMenuItem.Enabled = false; // disabled on start
        }
        

        //
        // Misc UI functions
        //

        private void DisplayRecords()
        {
            int index = recordListBox.SelectedIndex;

            //
            // Rearrange list to remove gaps
            //

            for (int r = 1; r < rows; r++) // for each row, Ignore first record
            {
                if (wikiTable[r - 1, 0] == null) // check if the record above is empty
                {
                    for (int c = 0; c < columns; c++) // if true shift record up
                    {
                        wikiTable[r - 1, c] = wikiTable[r, c];
                        wikiTable[r, c] = null;
                    }
                }
            }

            //
            // Sort table
            //

            wikiTable = SortTable(wikiTable);

            //
            // Display records
            //

            recordListBox.Items.Clear(); // remove old values
            displayList.Clear();
            indexCache.Clear();

            for (int i = 0; i < rows; i++) // populate list
            {
                displayList.Add(wikiTable[i, 0]);
                indexCache.Add(i); // store the index of the item in the 2d array
            }

            for (int i = displayList.Count - 1; i > -1; i--) // for each record, starting at last record
            {
                if (search != "" && search != null && displayList[i] != null)
                {
                    if (!new Regex(search, RegexOptions.IgnoreCase).Match(displayList[i]).Success) // regex check for search criteria
                    {
                        displayList.RemoveAt(i); // remove if search criteria is not met
                        indexCache.RemoveAt(i);
                    }
                    else
                    {
                        string.Concat(displayList[i], indexCache[i]);
                    }
                }
            }


            try // ignore if displayList is empty
            {
                recordListBox.Items.AddRange(displayList.ToArray()); // add name variables to the listview
            }
            catch (Exception e)
            {
                Debug.WriteLine($"Failed to populate listview: {e}");
            }
        }
        private void UpdateFields() // fill information into text boxes
        {
            if (recordListBox.SelectedIndex != -1)
            {
                name.Text = wikiTable[indexCache[recordListBox.SelectedIndex], 0];
                category.Text = wikiTable[indexCache[recordListBox.SelectedIndex], 1];
                structure.Text = wikiTable[indexCache[recordListBox.SelectedIndex], 2];
                definition.Text = wikiTable[indexCache[recordListBox.SelectedIndex], 3];
            }
        }
        private void ClearFields() // clear fields
        {
            name.Clear();
            structure.Clear();
            category.Clear();
            definition.Clear();
        }
        private void recordListBox_SelectedIndexChanged(object sender, EventArgs e) // called when record is selected
        {
            UpdateFields();

            if (recordListBox.SelectedIndex == -1) // enable or disable delete button if record is selected or not. Clear text boxes when nothing is selected
            {
                deleteToolStripMenuItem.Enabled = false;
                ClearFields();
            }
            else
            {
                deleteToolStripMenuItem.Enabled = true;
            }
        }
        private bool CompareRecords(string entry1, string entry2) // Compare by alphabetical and return if they are out of order
        {
            if (string.Compare(entry1, entry2) > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        private string[,] SortTable(string[,] table) // logic to sort the records
        {
            while (true)
            {
                for (int i = 1; i < rows; i++) // for each row, ignore first row
                {
                    if (table[i, 0] != null && table[i - 1, 0] != null) // make sure that there is a record
                    {
                        bool swapRecords = CompareRecords(table[i - 1, 0], table[i, 0]); // compare record to record above

                        if (swapRecords)
                        {
                            // store first record
                            string index0 = table[i - 1, 0];
                            string index1 = table[i - 1, 1];
                            string index2 = table[i - 1, 2];
                            string index3 = table[i - 1, 3];

                            for (int a = 0; a < 4; a++) // move record 2 up one space
                            {
                                table[i - 1, a] = table[i, a];
                            }
                            // restore record 1 and enter it into record 2 index
                            table[i, 0] = index0;
                            table[i, 1] = index1;
                            table[i, 2] = index2;
                            table[i, 3] = index3;
                        }
                    }
                }

                // iterate through while until all records have been swapped correctly
                bool iterate = false;

                for (int i = 1; i < rows; i++) // for each row, ignore first row
                {
                    if (table[i, 0] != null && table[i - 1, 0] != null) // make sure that there is a record
                    {
                        bool swapRecords = CompareRecords(table[i - 1, 0], table[i, 0]); // compare records

                        if (swapRecords)
                        {
                            iterate = true; 
                        }
                    }
                }

                if (!iterate)
                {
                    break;
                }
            }
            return table;
        }
        protected override void OnFormClosing(FormClosingEventArgs e) // check if table has been saved and prompt user
        {
            if (wikiTable != wikiTableCache) // if table has been changed
            {
                ConfirmBox exitConfirm = new ConfirmBox();

                if (!exitConfirm.QueryConfirmation("Unsaved data", "Exit without saving?", "Yes","No"))
                {
                    e.Cancel = true;
                }
            }
        }
        
        
        //
        // Text field logic
        //

        private void name_DoubleClick(object sender, EventArgs e) // double click on name text box
        {
            name.Clear();
            category.Clear();
            structure.Clear();
            definition.Clear();

            status.Text = "Cleared record values";
        }

        // leave focus
        private void name_Leave(object sender, EventArgs e)
        {
            if (recordListBox.SelectedIndex != -1) // make sure something is selected
            {
                int index = recordListBox.SelectedIndex;
                wikiTable[indexCache[recordListBox.SelectedIndex], 0] = name.Text; // save value into table
                DisplayRecords();
                recordListBox.SetSelected(index, true); // reselect record after refresh
            }
        }
        private void category_Leave(object sender, EventArgs e)
        {
            if (recordListBox.SelectedIndex != -1) // make sure something is selected
            {
                wikiTable[indexCache[recordListBox.SelectedIndex], 1] = category.Text; // save value into table
            }
        }
        private void structure_Leave(object sender, EventArgs e)
        {
            if (recordListBox.SelectedIndex != -1) // make sure something is selected
            {
                wikiTable[indexCache[recordListBox.SelectedIndex], 2] = structure.Text; // save value into table
            }
        }
        private void definition_Leave(object sender, EventArgs e)
        {
            if (recordListBox.SelectedIndex != -1) // make sure something is selected
            {
                wikiTable[indexCache[recordListBox.SelectedIndex], 3] = definition.Text; // save value into table
            }
        }

        // search
        private void searchBox_TextChanged(object sender, EventArgs e)
        {
            search = searchBox.Text;
            DisplayRecords();
        }
        private void searchBox_KeyPress(object sender, KeyPressEventArgs e) // filter search characters
        {
            var regex = new Regex(@"[^a-zA-Z0-9\b\s]");
            if (regex.IsMatch(e.KeyChar.ToString()))
            {
                e.Handled = true;
            }
        }

        // navigation controls
        private void name_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)
            {
                e.Handled = true;
                category.Focus();
            }
        }
        private void category_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)
            {
                e.Handled = true;
                structure.Focus();
            }
        }
        private void structure_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)
            {
                e.Handled = true;
                definition.Focus();
            }
        }


        //
        // Tool strip buttons
        //

        private void saveToolStripMenuItem_click(object sender, EventArgs e)
        {
            FileBrowser fileBrowser = new FileBrowser();

            string saveFile = fileBrowser.QuerySaveFile();

            if (saveFile != "")
            {
                saveSystem.Save(wikiTable, saveFile);
                status.Text = "Successfully saved wiki";
                wikiTableCache = wikiTable;
            }
        }
        private void loadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FileBrowser fileBrowser = new FileBrowser();

            string loadFile = fileBrowser.QueryLoadFile();

            if (loadFile != "")
            {
                wikiTable = saveSystem.Load(loadFile);
                DisplayRecords();
                status.Text = "Successfully loaded wiki";
                wikiTableCache = wikiTable;
            }
        }
        private void addToolStripMenuItem_Click(object sender, EventArgs e) // add record
        {
            string defaultName = "New Record";

            for (int i = 0; i < rows; i++)
            {
                if (wikiTable[i,0] == null)
                {
                    wikiTable[i, 0] = defaultName; // enter data into table
                    break;
                }
            }
            DisplayRecords();

            status.Text = "Added record";
        }
        private void deleteToolStripMenuItem_Click(object sender, EventArgs e) // delete record
        {
            ConfirmBox confirmationBox = new ConfirmBox();

            if (confirmationBox.QueryConfirmation("Confirm Delete", "Are you sure?", "Delete", "Cancel"))
            {
                for (int i = 0; i < columns; i++) // clear record
                {
                    if (recordListBox.SelectedIndex != -1) // if record is selected
                    {
                        wikiTable[indexCache[recordListBox.SelectedIndex], i] = null;
                    }
                }
                DisplayRecords();
                ClearFields();
                deleteToolStripMenuItem.Enabled = false;

                status.Text = "Removed record";
            }
        }
    }
}
