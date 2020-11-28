using log4net;
using log4net.Config;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Log4netTest
{
    public partial class Form1 : Form
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(Form1));
        public Form1()
        {
            InitializeComponent();
            _log.Info("=============  Started application  =============");
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            XmlConfigurator.Configure();
            ListBoxAppender.ConfigureListBoxAppender(listBox1);

            _log.Info("This is just an example of how this appender works...");
        }

        private void btnTest_Click(object sender, EventArgs e)
        {
            _log.Info("Test button clicked!");
        }
    }
}
