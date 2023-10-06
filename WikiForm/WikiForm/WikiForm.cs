using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace WikiForm
{
    public partial class WikiForm : Form
    {
        //initialise array
        static readonly int rows = 12;
        static readonly int columns = 4;

        private string[,] wikiTable = new string[rows, columns];

        // current selected index
        private int selectedRecord;

        // dummy value
        private readonly string placeholder = "--";

        // save cache
        private bool modified = false;

        // display variables
        //List<string> displayList = new List<string>(); // create list for name variables

        // search value
        private string search;

        // Form properties
        public WikiForm()
        {
            InitializeComponent();

            deleteToolStripMenuItem.Enabled = false; // disabled on start

            DisplayRecords();
        }


        //
        // Misc UI functions
        //

        private void DisplayRecords()
        {
            int index = selectedRecord;

            //
            // Placeholder value
            //

            for (int i = 0; i < rows; i++)
            {
                if (wikiTable[i, 0] == null || wikiTable[i, 0] == "") // check if the record above is empty
                {
                    wikiTable[i, 0] = placeholder;
                }
            }


            //
            // Sort table
            //

            wikiTable = SortTable(wikiTable);

            //
            // Display records
            //

            List<string> displayName = new List<string>(); // create a list to display names
            List<string> displayCategory = new List<string>(); // create a list to display category

            for (int r = 0; r < rows; r++)
            {
                try
                {
                    displayCategory.Add(wikiTable[r, 1]);
                    displayName.Add(wikiTable[r, 0]);
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e);
                }
            }


            try // ignore if listview is empty
            {
                RecordView.Items.Clear();

                for (int i = 0; i < rows; i++)
                {
                    if (wikiTable[i, 0] != placeholder)
                    {
                        ListViewItem lvi = new ListViewItem(wikiTable[i, 0]);
                        lvi.SubItems.Add(wikiTable[i, 1]);
                        RecordView.Items.Add(lvi);
                    }
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }

            try
            {
                RecordView.Items[index].Selected = true;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
        }
        private void UpdateFields() // fill information into text boxes
        {
            if (selectedRecord != -1)
            {
                if (wikiTable[selectedRecord, 0] == placeholder)
                {
                    name.Text = "";
                }
                else
                {
                    name.Text = wikiTable[selectedRecord, 0];
                }
                category.Text = wikiTable[selectedRecord, 1];
                structure.Text = wikiTable[selectedRecord, 2];
                definition.Text = wikiTable[selectedRecord, 3];
            }
        }
        private void ClearFields() // clear fields
        {
            name.Clear();
            structure.Clear();
            category.Clear();
            definition.Clear();
        }
        private void RecordView_SelectedIndexChanged(object sender, EventArgs e)
        {
            foreach (int index in RecordView.SelectedIndices)
            {
                selectedRecord = index;
            }
            UpdateFields();
        }
        private string[,] SortTable(string[,] table) // logic to sort the records
        {
            while (true)
            {
                bool iterate = false;

                for (int i = 1; i < rows; i++) // for each row, ignore first row
                {
                    if (table[i, 0] != placeholder && table[i - 1, 0] != placeholder) // make sure that there is a record
                    {
                        if (string.Compare(table[i - 1, 0], table[i, 0], true) > 0)
                        {
                            iterate = true;

                            // store first record
                            string index0 = table[i - 1, 0];
                            string index1 = table[i - 1, 1];
                            string index2 = table[i - 1, 2];
                            string index3 = table[i - 1, 3];

                            for (int a = 0; a < columns; a++) // move record 2 up one space
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

                if (!iterate)
                {
                    break;
                }
            }
            return table;
        }
        protected override void OnFormClosing(FormClosingEventArgs e) // check if table has been saved and prompt user
        {
            if (modified) // if table has been changed
            {
                ConfirmBox exitConfirm = new ConfirmBox();

                if (!exitConfirm.QueryConfirmation("Unsaved data", "Exit without saving?", "Yes", "No"))
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
            if (selectedRecord != -1) // make sure something is selected
            {
                wikiTable[selectedRecord, 0] = name.Text; // save value into table
                modified = true;
                DisplayRecords();
            }
        }
        private void category_Leave(object sender, EventArgs e)
        {
            if (selectedRecord != -1) // make sure something is selected
            {
                wikiTable[selectedRecord, 1] = category.Text; // save value into table
                modified = true;
                DisplayRecords();
            }
        }
        private void structure_Leave(object sender, EventArgs e)
        {
            if (selectedRecord != -1) // make sure something is selected
            {
                wikiTable[selectedRecord, 2] = structure.Text; // save value into table
                modified = true;
            }
        }
        private void definition_Leave(object sender, EventArgs e)
        {
            if (selectedRecord != -1) // make sure something is selected
            {
                definition.Text = definition.Text.ToString(); // removes formatting
                wikiTable[selectedRecord, 3] = definition.Text; // save value into table
                modified = true;
            }
        }

        // search
        private void searchBox_TextChanged(object sender, EventArgs e)
        {
            search = searchBox.Text;
        }
        private void searchButton_Click(object sender, EventArgs e)
        {
            if (search != null)
            {
                BinarySearch(search.ToString(), wikiTable);
            }
        }
        private void searchBox_KeyPress(object sender, KeyPressEventArgs e) // filter search characters
        {
            var regex = new Regex(@"[^a-zA-Z0-9-\b\s]");
            if (regex.IsMatch(e.KeyChar.ToString()))
            {
                e.Handled = true;
            }

            if (e.KeyChar == (char)13 && search != null) // if hit enter
            {
                e.Handled = true;
                BinarySearch(search.ToString(), wikiTable);
            }
        }
        private void BinarySearch(string searchCriteria, string[,] table)
        {
            int mid;
            int first = 0;
            int last = table.GetLength(0);

            for (int i = 0; i < rows; i++) // for each row
            {
                if (wikiTable[i, 0] == placeholder) // if value is blank move last back to where there is a record
                {
                    last--;
                }
            }

            while (first <= last)
            {
                mid = (first + last) / 2;

                try
                {
                    if (string.Compare(searchCriteria, table[mid, 0], true) > 0)
                    {
                        first = mid + 1;
                    }
                    else if (string.Compare(searchCriteria, table[mid, 0], true) < 0)
                    {
                        last = mid - 1;
                    }
                    else
                    {
                        RecordView.Items[mid].Selected = true;
                        break;
                    }
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e);

                    status.Text = "Failed to find a match";

                    return;
                }
            }


            // Linear search function
            // I cant bring myself to delete it, I spent way too much time getting this to work. :(

            /* for (int i = displayList.Count - 1; i > -1; i--) // for each record, starting at last record
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
             }*/
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
                Save(wikiTable, saveFile);
                status.Text = "Successfully saved wiki";
                modified = false;
            }
        }
        private void loadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FileBrowser fileBrowser = new FileBrowser();

            string loadFile = fileBrowser.QueryLoadFile();

            if (loadFile != "")
            {
                Load(loadFile);
                DisplayRecords();
                status.Text = "Successfully loaded wiki";
                modified = false;
            }
        }
        private void addToolStripMenuItem_Click(object sender, EventArgs e) // add record
        {
            string defaultName = "New Record";

            for (int i = 0; i < rows; i++)
            {
                if (wikiTable[i, 0] == placeholder)
                {
                    wikiTable[i, 0] = defaultName;
                    modified = true;
                    break;
                }
            }

            DisplayRecords();

            status.Text = "Added record";
        }
        private void deleteToolStripMenuItem_Click(object sender, EventArgs e) // delete record
        {
            ConfirmBox confirmationBox = new ConfirmBox();
            if (wikiTable[selectedRecord, 0] != placeholder)
            {
                if (confirmationBox.QueryConfirmation("Confirm Delete", "Are you sure?", "Delete", "Cancel"))
                {
                    for (int i = 0; i < columns; i++) // clear record
                    {
                        if (selectedRecord != -1) // if record is selected
                        {
                            wikiTable[selectedRecord, i] = null;
                        }
                    }

                    modified = true;

                    DisplayRecords();
                    ClearFields();

                    status.Text = "Removed record";
                }
            }
        }

        /// <summary>
        /// Save function
        /// </summary>
        /// <param name="data"></param>
        /// <param name="filePath"></param>
        public void Save(string[,] data, string filePath)
        {
            using (FileStream fileStream = new FileStream(filePath, FileMode.OpenOrCreate))
            {
                using (BinaryWriter binaryWriter = new BinaryWriter(fileStream))
                {
                    for (int r = 0; r < rows; r++)
                    {
                        for (int c = 0; c < columns; c++)
                        {
                            binaryWriter.Write(data[r, c]);
                        }
                    }
                }
            }


            // Old save function
            /*            if (!File.Exists(filePath)) // if file doesnt exist, create it
                        {
                            var newFile = File.Create(filePath);
                            newFile.Close();
                        }

                        Byte[] byteData = SerializeData(data);

                        File.WriteAllBytes(filePath, byteData);*/
        }// save file

        /// <summary>
        /// Load File
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        private void Load(string filePath)
        {
            using (FileStream fileStream = new FileStream(filePath, FileMode.Open))
            {
                using (BinaryReader binaryReader = new BinaryReader(fileStream))
                {
                    while (binaryReader.BaseStream.Position < binaryReader.BaseStream.Length)
                    {
                        for (int r = 0; r < rows; r++)
                        {
                            for (int c = 0; c < columns; c++)
                            {
                                wikiTable[r, c] = binaryReader.ReadString();
                            }
                        }
                    }
                }
            }

            // Old load function

            /*Byte[] byteData = File.ReadAllBytes(filePath);

            BinaryFormatter bf = new BinaryFormatter();

            using (MemoryStream ms = new MemoryStream(byteData))
            {
                return bf.Deserialize(ms) as string[,];
            }*/
        }// load file
    }
}
