using log4net;
using log4net.Appender;
using log4net.Repository.Hierarchy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Log4netTest
{
    public class ListBoxAppender : IAppender
    {
        private ListBox _listBox;
        private readonly object _lockObj = new object();

        public ListBoxAppender(ListBox listBox)
        {
            var frm = listBox.FindForm();
            if (frm == null)
                return;

            frm.FormClosing += delegate { Close(); };

            _listBox = listBox;
            Name = "ListBoxAppender";
        }

        public string Name { get; set; }

        public static void ConfigureListBoxAppender(ListBox listBox)
        {
            var hierarchy = (Hierarchy)LogManager.GetRepository();
            var appender = new ListBoxAppender(listBox);
            hierarchy.Root.AddAppender(appender);
        }

        public void Close()
        {
            try
            {
                // This locking is required to avoid null reference exceptions
                // in situations where DoAppend() is writing to the ListBox while
                // Close() is nulling out the ListBox.
                lock (_lockObj)
                {
                    _listBox = null;
                }

                var hierarchy = (Hierarchy)LogManager.GetRepository();
                hierarchy.Root.RemoveAppender(this);
            }
            catch
            {
                // There is not much that can be done here, and
                // swallowing the error is desired in my situation.
            }
        }

        public void DoAppend(log4net.Core.LoggingEvent loggingEvent)
        {
            try
            {
                if (_listBox == null)
                    return;

                // For my situation, this quick and dirt filter was all that was 
                // needed. Depending on your situation, you may decide to delete 
                // this logic, modify it slightly, or implement something a 
                // little more sophisticated.
                if (loggingEvent.LoggerName.Contains("NHibernate"))
                    return;

                // Again, my requirements were simple; displaying the message was
                // all that was needed. Depending on your circumstances, you may
                // decide to add information to the displayed message 
                // (e.g. log level) or implement something a little more 
                // dynamic.
                var msg = string.Concat(loggingEvent.RenderedMessage, "\r\n");

                lock (_lockObj)
                {
                    // This check is required a second time because this class 
                    // is executing on multiple threads.
                    if (_listBox == null)
                        return;

                    // Because the logging is running on a different thread than
                    // the GUI, the control's "BeginInvoke" method has to be
                    // leveraged in order to append the message. Otherwise, a 
                    // threading exception will be thrown. 
                    var del = new Action<string>(s =>
                    {
                        string strTime = DateTime.Now.ToString("HH:mm:ss.fff");
                        string strMsg = String.Format("[{0}] {1}", strTime, s);
                        _listBox.Items.Add(strMsg);
                    });
                    _listBox.BeginInvoke(del, msg);
                }
            }
            catch
            {
                // There is not much that can be done here, and
                // swallowing the error is desired in my situation.
            }
        }
    }
}
