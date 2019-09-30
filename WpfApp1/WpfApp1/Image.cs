using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;

namespace Photomv
{
    public class Image
    {
        private static Logger log = Logger.GetInstance("./PhotomvLog.txt", true);

        private string orgPath;
        private string year;
        private string month;
        private string date;
        private string filename;
        private string destfilename;
        private string resultMessage;

        public Image(string path, string name)
        {
            orgPath = path;
            filename = name;
        }

        public string OrgPath { get => orgPath; set => orgPath = value; }
        public string Year { get => year; set => year = value; }
        public string Month { get => month; set => month = value; }
        public string Date { get => date; set => date = value; }
        public string Filename { get => filename; set => filename = value; }
        public string DestFilename { get => destfilename; set => destfilename = value; }
        public string ResultMessage { get => resultMessage; set => resultMessage = value; }

        public void Execute(string dest)
        {
            log.Info("Image.Execute() dest={0} orig={1} filename={2}", dest, OrgPath, Filename);
            try
            {
                FileStream fs;
                fs = new FileStream(OrgPath,
                                    FileMode.Open,
                                    FileAccess.Read,
                                    FileShare.ReadWrite);
                // read 1024byte from file's head 
                byte[] buff = new byte[1024];
                fs.Read(buff, 0, 1024);
                fs.Close();

                char[] cBuff = System.Text.Encoding.GetEncoding(932).GetString(buff).ToCharArray();
                if (!Parse(cBuff))
                {
                    log.Error("Fail {0}", OrgPath);
                    return;
                }

                if (PrepareCopyFile(dest))
                {
                    if (PhotoMVSingleton.GetInstance().Mode == "debug") {
                        log.Info("Image.Execute pseudo");
                        PhotoMVStat.copy_success_times++;
                        return;
                    }
                    File.Copy(OrgPath, DestFilename, false);
                }
            }
            catch (System.IO.IOException e)
            {
                log.Error("IOException occured. {0}", e.HResult);
                log.Error("IOException occured. {0}", e.GetType());
            }
            catch (UnauthorizedAccessException e)
            {
                /*
                 * If target is not file
                 * (in the case, Directory has specified),
                 * UnauthorizedAccessException occured.
                 */
                log.Error("Error {0}", e.GetType());
            }
        }

        public bool Parse(char[] buff)
        {
            log.Info("Image.Parse() start byteLen={0}", buff.Length);
            bool result = false;

            try
            {
                for (int i = 0; i < buff.Length - 1; i++)
                {
                    if (buff[i] != '2')
                        continue;
                    // search 2yyy:mm:dd
                    if ((buff[i + 1] == '0') && (buff[i + 4] == ':'))
                    {
                        string year = new string(buff, i, 4);
                        string month = new string(buff, i + 5, 2);
                        string date = new string(buff, i, 10);

                        //Console.WriteLine("byte {0}", buff[i]);
                        log.Info("Image.Parse() {0}/{1}/{2}", year, month, date);
                        Year = year;
                        Month = month;
                        Date = date;
                        result = true;
                    }
                }
            }
            catch (System.IndexOutOfRangeException e)
            {
                log.Error("Image.Parse() IndexOutOfRangeException occured. {0}", e.GetType());
            }
            log.Info("Image.Parse() end");

            return result;
        }

        public bool PrepareCopyFile(string outDir)
        {
            try
            {
                log.Info("Image.PrepareCopyFile() start outdir={0}", outDir);
                if (!(Directory.Exists(outDir)))
                {
                    Directory.CreateDirectory(outDir);
                }

                /**
                 * create year direcotry
                 */
                StringBuilder buffer = new System.Text.StringBuilder(outDir);
                buffer.Append("\\");
                buffer.Append(Year);
                string yearDir = buffer.ToString();
                if (!(Directory.Exists(yearDir)))
                {
                    Directory.CreateDirectory(yearDir);
                }
                /**
                 * create month direcotry
                 */
                StringBuilder buffer2 = new System.Text.StringBuilder(yearDir);
                buffer2.Append("\\");
                buffer2.Append(Year);
                buffer2.Append(Month);
                string monthDir = buffer2.ToString();
                if (!(Directory.Exists(monthDir)))
                {
                    Directory.CreateDirectory(monthDir);
                }
                /**
                 * create dest file path
                 */
                buffer2.Append("\\");
                buffer2.Append(Filename);
                destfilename = buffer2.ToString();
                log.Info("Image.PreparedCopyFile() end dest={0}", destfilename);
                if ((File.Exists(destfilename)))
                {
                    log.Info("Image.PreparedCopyFile() Already exsists {0}", destfilename);
                    string newname = Rename(destfilename);
                    return false;
                }
                return true;
            }
            catch (IOException e)
            {
                log.Error("Image.PreparedCopyFile IOException occured. {0}", e.GetType());
                return false;
            }
        }

        public string Rename(string fname)
        {
            log.Info("Image.Rename start");
            // separate file name, file extension
            string ext = Path.GetExtension(fname);
            string tmpname = Path.GetFileNameWithoutExtension(fname);
            int last = tmpname.Length;
            log.Info("Image.Rename tmpname length {0}", last);

            string result = "";
            if (tmpname[last - 2] == '_')
            {
                Console.WriteLine("hit");
                for (int i = 0; i < 10; i++)
                {
                    if (tmpname[last - 1] == (char)i)
                    {
                        log.Info("Image.Rename already exsists file {0}", tmpname);
                        continue;
                    } else
                    {
                        tmpname.Remove(last - 1, 1).Insert(last - 1, ((char)i).ToString());
                        log.Info("Image.Rename done {0}", tmpname);
                    }
                }
            } else
            {
                // TODO fname separete with name and suffix
                result = tmpname + "_0" + ext;
                if (!File.Exists(result))
                {
                    log.Info("Image.Rename orig:{0} new:{1}", fname, tmpname);
                    // tmp name is not exists.
                    return result;
                }
            }
            return "";
        }
    }
}
