using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HuffmanExercise
{
    class LogUtil
    {
        public static RichTextBox LogTextBox;

        public static void Log(string message)
        {
            LogTextBox.AppendText(message + "\n");
            LogTextBox.Select(LogTextBox.Text.Length - 1, 0);
            LogTextBox.ScrollToCaret();
            LogTextBox.Refresh();
        }
    }
}
