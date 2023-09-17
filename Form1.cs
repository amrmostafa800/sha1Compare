using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace sha1Compare
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private string _GetSha1(string filePath)
        {
            if (File.Exists(filePath))
            {
                using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    using (SHA1 sha1 = SHA1.Create())
                    {
                        byte[] hashBytes = sha1.ComputeHash(fileStream);
                        string sha1Hash = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
                        return sha1Hash;
                    }
                }
            }
            else
            {
                return null;
            }
        }

        private FileInfo[] _GetFileInfoOfAllFiles(string[] filesPath)
        {
            var fileInfos = new List<FileInfo>();
            foreach (var filePath in filesPath)
            {
                FileInfo fileInfo = new FileInfo()
                {
                    FileName = Path.GetFileName(filePath),
                    FilePath = filePath,
                    FileSha1 = _GetSha1(filePath)
                };
                fileInfos.Add(fileInfo);
            }
            return fileInfos.ToArray();
        }
        
        private string[] _GetAllFilesInDirectory(string folderPath)
        {
            // Get all files in the current folder.
            string[] files = Directory.GetFiles(folderPath);

            // Recursively get files from subfolders.
            foreach (string subfolder in Directory.GetDirectories(folderPath))
            {
                string[] subfolderFiles = _GetAllFilesInDirectory(subfolder);
                files = files.Concat(subfolderFiles).ToArray();
            }

            return files;
        }

        private FileInfo[] _compareBetweenTwoFileInfos(FileInfo[] filesInfo1,FileInfo[] filesInfo2)
        {
            var fileInfoComparer = new FileInfoComparer();
            var inArray1NotInArray2 = filesInfo1.Except(filesInfo2,fileInfoComparer).ToArray();
            return inArray1NotInArray2;
        }
        
        private string[] _ShowDialogOfExtractAllFilesPathFromFolder(int number = 0)
        {
            using (var folderBrowserDialog = new FolderBrowserDialog())
            {
                if (number != 0)
                {
                    folderBrowserDialog.Description = $"Select a folder {number.ToString()}:";
                }
                else
                {
                    folderBrowserDialog.Description = "Select a folder:";
                }
                folderBrowserDialog.RootFolder = Environment.SpecialFolder.Desktop;

                if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
                {
                    string[] files = _GetAllFilesInDirectory(folderBrowserDialog.SelectedPath);

                    return files;
                }
            }

            return null;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Title = "Open File";
                openFileDialog.Filter = "All Files|*.*"; // You can customize the file filter as needed.

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string fileHash = _GetSha1(openFileDialog.FileName);
                    MessageBox.Show(fileHash);
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog())
            {
                folderBrowserDialog.Description = "Select a folder:";
                folderBrowserDialog.RootFolder = Environment.SpecialFolder.Desktop;

                if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
                {
                    string[] files = _GetAllFilesInDirectory(folderBrowserDialog.SelectedPath);
 
                    MessageBox.Show(string.Join("\n",files));
                    
                }
            }
        }

        private void btn_Start_Click(object sender, EventArgs e)
        {
            var files1 = _ShowDialogOfExtractAllFilesPathFromFolder(1);
            var files2 = _ShowDialogOfExtractAllFilesPathFromFolder(2);

            var filesInfo1 = _GetFileInfoOfAllFiles(files1);
            var filesInfo2 = _GetFileInfoOfAllFiles(files2);

            var differentFiles = _compareBetweenTwoFileInfos(filesInfo1, filesInfo2);

            dataGridView1.AutoGenerateColumns = true;
            dataGridView1.DataSource = differentFiles;
        }
    }
}