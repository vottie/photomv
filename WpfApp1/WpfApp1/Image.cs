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
            Console.WriteLine("Image execute start dest : {0} orig : {1} filename : {2}",
                              dest, OrgPath, Filename);
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
                    Console.WriteLine("Fail {0}", OrgPath);
                    return;
                }

                if (PrepareCopyFile(dest))
                {
                    File.Copy(OrgPath, DestFilename, false);
                }
            }
            catch (System.IO.IOException e)
            {
                Console.WriteLine("IOException occured. {0}", e.HResult);
                Console.WriteLine("IOException occured. {0}", e.GetType());
            }
            catch (UnauthorizedAccessException e)
            {
                /*
                 * If target is not file
                 * (in the case, Directory has specified),
                 * UnauthorizedAccessException occured.
                 */
                Console.WriteLine("Error {0}", e.GetType());
            }
        }

        public bool Parse(char[] buff)
        {
            Console.WriteLine("Image parse start");
            Console.WriteLine("byte length {0}", buff.Length);

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
                        Console.WriteLine("parse {0}/{1}/{2}", year, month, date);
                        Year = year;
                        Month = month;
                        Date = date;
                        result = true;
                    }
                }
            }
            catch (System.IndexOutOfRangeException e)
            {
                Console.WriteLine("IndexOutOfRangeException occured. {0}", e.GetType());
            }
            Console.WriteLine("Image parse end");

            return result;
        }

        public bool PrepareCopyFile(string outDir)
        {
            try
            {
                Console.WriteLine("prepareCopyFile {0}", outDir);
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
                Console.WriteLine("preparedCopyFile end {0}", destfilename);
                if ((File.Exists(destfilename)))
                {
                    Console.WriteLine("Already exsists {0}", destfilename);
                    string newname = Rename(destfilename);
                    return false;
                }
                return true;
            }
            catch (IOException e)
            {
                Console.WriteLine("preparedCopyFile IOException occured. {0}", e.GetType());
                return false;
            }
        }

        public string Rename(string fname)
        {
            Console.WriteLine("rename() start");
            // separate file name, file extension
            string ext = Path.GetExtension(fname);
            string tmpname = Path.GetFileNameWithoutExtension(fname);
            int last = tmpname.Length;
            Console.WriteLine("tmpname length {0}", last);

            string result = "";
            if (tmpname[last - 2] == '_')
            {
                Console.WriteLine("hit");
                for (int i = 0; i < 10; i++)
                {
                    if (tmpname[last - 1] == (char)i)
                    {
                        Console.WriteLine("Already exsits file {0} ", tmpname);
                        continue;
                    } else
                    {
                        tmpname.Remove(last - 1, 1).Insert(last - 1, ((char)i).ToString());
                        Console.WriteLine("Renamed {0} ", tmpname);
                    }
                }
            } else
            {
                // TODO fname separete with name and suffix
                result = tmpname + "_0" + ext;
                if (!File.Exists(result))
                {
                    Console.WriteLine("Rename orig:{0} new:{1}", fname, tmpname);
                    // tmp name is not exists.
                    return result;
                }
            }
            return "";
        }
    }
}
