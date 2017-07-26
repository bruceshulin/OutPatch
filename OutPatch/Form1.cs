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

namespace OutPatch
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        String SaveDir = "";
        String SourceDir = "";

        private string SelectDir(string default_path)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            if (fbd.ShowDialog() == DialogResult.OK)
            {
                //MessageBox.Show("选择的目录：" + fbd.SelectedPath);
                return fbd.SelectedPath;
            }
            //string temppath = @"D:\OutPatch_"+DateTime.Today.ToShortDateString()+@"\";
           // MessageBox.Show("选择的目录：" + temppath);
            return default_path;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (txtSaveDir.Text == "")
            {
                SaveDir = "";
            }
            else 
            {
                SaveDir = txtSaveDir.Text;
            }
            if (SaveDir == "")
            {
                SaveDir = SelectDir(txtSaveDir.Text);
            }
            if (txtSource.Text == "")
            {
                SourceDir = "";
            }
            else
            {
                SourceDir = txtSource.Text;
            }
            if (SourceDir == "")
            {
                SourceDir = SelectDir(txtSource.Text);
            }
          //1 循环文件或目录
            List<string> listfiles = this.txtFiles.Text.Split('\n').ToList();
            //复制到指定的目录
            for (int i = 0; i < listfiles.Count; i++)
            {
                string file_tmp = listfiles[i].Replace('\r', ' ').Trim().Replace("/", "\\");
                string file = SourceDir + "\\" + file_tmp;
                if (File.Exists( file) == true)
                {
                    string filename = System.IO.Path.GetFileName(file);// getfileName(file);
                    //创建目录
                    string dir = createDir(file_tmp, true);
                    string destFile =SaveDir + "\\" + dir + filename;
                    System.IO.File.Copy(file, destFile, true);
                }
                else 
                {
                    string dir = createDir(file, false);
                    string destPath = SaveDir + "\\" + dir;
                    if (Directory.Exists(file) && Directory.Exists(destPath))
                    {
                        CopyDirectory(file, destPath);
                    }
                    else
                    {
                        MessageBox.Show(file + "目录" + destPath);
                    }
                }
            }
            MessageBox.Show("导出完成");
        }

        private string createDir(string path,bool isfile)
        {
            List<string> list_dir = path.Replace('\r', ' ').Trim().Split('\\').ToList();
            StringBuilder sb = new StringBuilder();

            foreach (var item in list_dir)
            {
                if (item.Contains(":"))
                {
                    continue;
                }
                if (isfile)
                {
                    //是文件需要判断是不是最后一个
                    if (item == list_dir[list_dir.Count - 1])
                    {
                        break;
                    }
                }
                else
                {
                    //全是目录
                }
                sb.Append(item + "\\");
                //创建目录
                string dir = SaveDir + "\\" + sb.ToString();
                if (Directory.Exists(dir) == false)
                {
                    Directory.CreateDirectory(dir);
                }
            }

            return sb.ToString();
        }

        private string getfileName(string path)
        {
            string[] dir_files = path.Replace('\r', ' ').Trim().Split('\\');
            return dir_files[dir_files.Length - 1];
        }

        private void button2_Click(object sender, EventArgs e)
        {
            SaveDir = txtSaveDir.Text = SelectDir(txtSaveDir.Text);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            SourceDir = txtSource.Text = SelectDir(txtSource.Text);
        }

        public static void CopyDirectory(string srcPath, string destPath)
        {
            try
            {
                DirectoryInfo dir = new DirectoryInfo(srcPath);
                FileSystemInfo[] fileinfo = dir.GetFileSystemInfos();  //获取目录下（不包含子目录）的文件和子目录
                foreach (FileSystemInfo i in fileinfo)
                {
                    if (i is DirectoryInfo)     //判断是否文件夹
                    {
                        if (!Directory.Exists(destPath + "\\" + i.Name))
                        {
                            Directory.CreateDirectory(destPath + "\\" + i.Name);   //目标目录下不存在此文件夹即创建子文件夹
                        }
                        System.IO.File.Copy(i.FullName, destPath + "\\" + i.Name);    //递归调用复制子文件夹
                    }
                    else
                    {
                        File.Copy(i.FullName, destPath + "\\" + i.Name, true);      //不是文件夹即复制文件，true表示可以覆盖同名文件
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                //throw;
            }
        }
    }
}
