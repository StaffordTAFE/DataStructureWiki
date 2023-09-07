using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WikiForm
{
    internal class ConfirmBox
    {
        public bool QueryConfirmation(string popupName, string labelText, string trueText, string falseText)
        {
            Form form = new Form() // create the form
            {
                Text = popupName,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                ShowIcon = false,
                ShowInTaskbar = false,
                StartPosition = FormStartPosition.CenterParent,
                MinimizeBox = false,
                MaximizeBox = false,
                Size = new Size(210, 142)
            };

            Label label = new Label() // create label
            {
                Text= labelText,
                Location = new Point(12,9),
                Size = new Size(107,20)
            };

            Button trueButton = new Button() // create true button
            {
                Text = trueText,
                DialogResult = DialogResult.Yes,
                Location = new Point(12, 32),
                Size = new System.Drawing.Size(81, 41),
            }; 
            trueButton.DialogResult = DialogResult.Yes;
            form.AcceptButton = trueButton;

            Button falseButton = new Button() // create false button
            {
                Text = falseText,
                DialogResult = DialogResult.No,
                Location = new Point(99, 32),
                Size = new System.Drawing.Size(81, 41),
            };
            form.CancelButton = falseButton;

            form.Controls.Add(label);
            form.Controls.Add(trueButton);
            form.Controls.Add(falseButton);

            DialogResult dialogResult = form.ShowDialog();
            if (dialogResult == DialogResult.Yes)
            {
                return true;
            }
            else
            {
                form.Close (); 
                return false;
            }
        }
    }
}
