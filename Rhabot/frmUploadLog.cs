using System.Windows.Forms;

namespace ISXBotHelper
{
    public partial class frmUploadLog : Form
    {
        public frmUploadLog()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Sets the caption text
        /// </summary>
        /// <param name="DisplayText"></param>
        public void SetText(string DisplayText)
        {
            this.Text = DisplayText;
        }

        /// <summary>
        /// sets the max value of the progressbar
        /// </summary>
        /// <param name="MaxValue"></param>
        public void SetMaxValue(int MaxValue)
        {
            this.progressBar1.Maximum = MaxValue;
        }

        /// <summary>
        /// Sets the value of the progress bar
        /// </summary>
        /// <param name="Value"></param>
        public void SetValue(int Value)
        {
            // only set if not past the max value
            if (Value <= this.progressBar1.Maximum)
                this.progressBar1.Value = Value;
            this.progressBar1.Refresh();
        }

        /// <summary>
        /// Sets the text of the label
        /// </summary>
        /// <param name="LabelText"></param>
        public void SetLabelText(string LabelText)
        {
            this.label1.Text = LabelText;
            this.label1.Refresh();
        }
    }
}