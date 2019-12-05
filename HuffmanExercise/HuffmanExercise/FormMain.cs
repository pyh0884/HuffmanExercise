using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HuffmanExercise
{
    public partial class FormMain : Form
    {
        private const int MAX_FILE_SIZE = 67108864; // max file size is 64MB
        private const string COMPRESSED_FILE_EXTENTION = ".comp";

        private FileInfo _openedFile;

        public FormMain()
        {
            InitializeComponent();
            LogUtil.LogTextBox = textBoxLog;
        }

        private void buttonCompress_Click(object sender, EventArgs e)
        {
            OpenFile("All Files(*.*)|*.*");

            if (_openedFile == null)
            {
                return;
            }

            try
            {
                byte[] data = CompressUtil.Compress(_openedFile);
                string validPath = GetValidFilePath(_openedFile.FullName + COMPRESSED_FILE_EXTENTION);
                LogUtil.Log("Writing file to disk at: " + validPath);
                File.WriteAllBytes(validPath, data);
                LogUtil.Log("Compressing finished! File size:" + new FileInfo(validPath).Length + "\n");
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.ToString());
                LogUtil.Log("Error happend! Working proccess stopped.\n");
            }
        }

        private void buttonDecompress_Click(object sender, EventArgs e)
        {
            OpenFile("Compressed File(*" + COMPRESSED_FILE_EXTENTION + "|*" + COMPRESSED_FILE_EXTENTION);

            if (_openedFile == null)
            {
                return;
            }

            try
            {
                byte[] data = CompressUtil.Decompress(_openedFile);
                // try write file to original file name
                string validPath = GetValidFilePath(_openedFile.FullName.Substring(0, _openedFile.FullName.Length - 5));
                LogUtil.Log("Writing file to disk at: " + validPath);
                File.WriteAllBytes(validPath, data);
                LogUtil.Log("Decompressing finished!");
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.ToString());
                LogUtil.Log("Error happend! Working proccess stopped.\n");
            }
        }

        // Open a file
        private void OpenFile(string fileFilter)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Multiselect = false;
            fileDialog.Title = "Please select your file";
            fileDialog.Filter = fileFilter;

            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                string fileName = fileDialog.FileName;
                if (!CheckFileValid(fileName))
                {
                    _openedFile = null;
                }
                _openedFile = new FileInfo(fileName);
                LogUtil.Log("File selected: " + _openedFile.FullName);
            }
        }

        private bool CheckFileValid(string path)
        {
            FileInfo file = new FileInfo(path);

            // Check file's size
            if (file.Length > MAX_FILE_SIZE)
            {
                MessageBox.Show("Please make sure your file is smaller than 64MB");
                return false;
            }

            return true;
        }

        private string GetValidFilePath(string path)
        {
            if (File.Exists(path))
            {
                int dotIndex = path.LastIndexOf(".");
                string fileExt = "";
                string fileName = path;
                if (dotIndex != -1)
                {
                    fileExt = path.Substring(dotIndex);
                    fileName = path.Substring(0, dotIndex);
                }

                int countNum = 1;
                string newFilePath = fileName + "(" + countNum + ")" + fileExt;
                while (File.Exists(newFilePath))
                {
                    countNum++;
                    newFilePath = fileName + "(" + countNum + ")" + fileExt;
                }

                path = newFilePath;
            }

            return path;
        }

        private void buttonClearLog_Click(object sender, EventArgs e)
        {
            textBoxLog.Clear();
        }

        private void quitQToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
