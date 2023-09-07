using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WikiForm
{
    internal class FileBrowser
    {
        public string QueryLoadFile()
        {
            OpenFileDialog opf = new OpenFileDialog()
            {
                Title = "Load record",
                InitialDirectory = Application.UserAppDataPath,
                Filter = "Binary files (*.dat)|*.dat|All files (*.*)|*.*",
                FilterIndex = 0,
                CheckFileExists = true,
                CheckPathExists = true,
                Multiselect = false
            };

            opf.ShowDialog();

            return opf.FileName;
        }

        public string QuerySaveFile()
        {
            SaveFileDialog sfd = new SaveFileDialog()
            {
                Title = "Save Record",
                InitialDirectory = Application.UserAppDataPath,
                Filter = "Binary files (*.dat)|*.dat",
                FilterIndex = 0,
                CheckFileExists = false,
                CheckPathExists = true,
            };

            sfd.ShowDialog();

            return sfd.FileName;
        }
    }
}
