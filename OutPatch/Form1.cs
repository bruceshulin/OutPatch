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
            SaveHistory(SourceDir, SaveDir);
            //1 循环文件或目录
            List<string> listfiles = this.txtFiles.Text.Split('\n').ToList();
            //复制到指定的目录
            for (int i = 0; i < listfiles.Count; i++)
            {
                if (listfiles[i].Length < 1) //空目录不复制
                {
                    continue;
                }

                string file_tmp = listfiles[i].Replace('\r', ' ').Trim().Replace("/", "\\");
                string file = SourceDir + "\\" + file_tmp;
                if (File.Exists(file) == true)
                {
                    string filename = System.IO.Path.GetFileName(file);// getfileName(file);
                    //创建目录
                    string dir = createDir(file_tmp, true);
                    string destFile = SaveDir + "\\" + dir + filename;
                    System.IO.File.Copy(file, destFile, true);
                }
                else
                {
                    string dir = createDir(file_tmp, false);
                    string destPath = SaveDir + "\\" + dir;
                    if (Directory.Exists(file) && Directory.Exists(destPath))
                    {
                        CopyDirectory(file, destPath);　//这个只能复制一层目录
                    }
                    else
                    {
                        MessageBox.Show(file + "目录" + destPath);
                    }
                }
            }
            MessageBox.Show("导出完成");
        }



        private string createDir(string path, bool isfile)
        {
            List<string> list_dir = path.Replace('\r', ' ').Trim().Split('\\').ToList();
            StringBuilder sb = new StringBuilder();

            foreach (var item in list_dir)
            {
                if (item == "")
                    continue;
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
               

                FileInfo[] files = dir.GetFiles();
                foreach (FileInfo file in files) 
                {
                    if (file is DirectoryInfo)     //判断是否文件夹
                    {
                        continue;
                    }
                    else
                    {
                        File.Copy(file.FullName, destPath + "\\" + file.Name, true);      //不是文件夹即复制文件，true表示可以覆盖同名文件
                    }
                }

                FileSystemInfo[] fileinfo = dir.GetDirectories();  //获取目录下（不包含子目录）的文件和子目录
                foreach (FileSystemInfo chiledir in fileinfo)
                {
                    string tmp_path = destPath;
                    if (chiledir is DirectoryInfo)     //判断是否文件夹
                    {

                        if (!Directory.Exists(tmp_path + "\\" + chiledir.Name))
                        {
                            Directory.CreateDirectory(tmp_path + "\\" + chiledir.Name);   //目标目录下不存在此文件夹即创建子文件夹
                        }
                        if (destPath.EndsWith("\\") == true)
                        {
                            tmp_path += chiledir.Name;    //递归调用复制子文件夹
                        }
                        else
                        {
                            tmp_path += "\\" + chiledir.Name;    //递归调用复制子文件夹
                        }
                        CopyDirectory(chiledir.FullName, tmp_path);
                    }
                    
                   
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                //throw;
            }
        }
        private void SaveHistory(string SourceDir, string SaveDir)
        {
            string path = "history.h";
            string contentpath = File.ReadAllText(path);
            if (contentpath.Contains(SourceDir) == false)
            {
                File.AppendAllText(path, SourceDir + "\r\n");
            }
            if (contentpath.Contains(SaveDir) == false)
            {
                File.AppendAllText(path, SaveDir + "\r\n");
            }
        }
        private void btnHistory_Click(object sender, EventArgs e)
        {
            string path = "history.h";
            if (File.Exists(path) == true)
            {
                string cmd = "notepad " + path;

                System.Diagnostics.Process p = new System.Diagnostics.Process();
                p.StartInfo.FileName = "cmd.exe";
                p.StartInfo.UseShellExecute = false;    //是否使用操作系统shell启动
                p.StartInfo.RedirectStandardInput = true;//接受来自调用程序的输入信息
                p.StartInfo.RedirectStandardOutput = true;//由调用程序获取输出信息
                p.StartInfo.RedirectStandardError = true;//重定向标准错误输出
                p.StartInfo.CreateNoWindow = true;//不显示程序窗口
                p.Start();//启动程序

                //向cmd窗口发送输入信息
                p.StandardInput.WriteLine(cmd + "&exit");

                p.StandardInput.AutoFlush = true;
                //p.StandardInput.WriteLine("exit");
                //向标准输入写入要执行的命令。这里使用&是批处理命令的符号，表示前面一个命令不管是否执行成功都执行后面(exit)命令，如果不执行exit命令，后面调用ReadToEnd()方法会假死
                //同类的符号还有&&和||前者表示必须前一个命令执行成功才会执行后面的命令，后者表示必须前一个命令执行失败才会执行后面的命令



                //获取cmd窗口的输出信息
                //string output = p.StandardOutput.ReadToEnd();

                //StreamReader reader = p.StandardOutput;
                //string line=reader.ReadLine();
                //while (!reader.EndOfStream)
                //{
                //    str += line + "  ";
                //    line = reader.ReadLine();
                //}

                p.WaitForExit();//等待程序执行完退出进程
                p.Close();

            }
            else
            {
                File.Create(path);
            }
        }
    }
}
