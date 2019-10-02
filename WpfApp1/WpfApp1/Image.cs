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
        private string day;
        private string hour;
        private string minute;
        private string second;
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
        public string Day { get => day; set => day = value; }
        public string Hour { get => hour; set => hour = value; }
        public string Minute { get => minute; set => minute = value; }
        public string Second { get => second; set => second = value; }
        public string Filename { get => filename; set => filename = value; }
        public string DestFilename { get => destfilename; set => destfilename = value; }
        public string ResultMessage { get => resultMessage; set => resultMessage = value; }


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
                    // i's value is '2'
                    if ((buff[i + 1] == '0') && (buff[i + 4] == ':') && (buff[i + 7] == ':'))
                    {
                        string year = new string(buff, i, 4);
                        string month = new string(buff, i + 5, 2);
                        string day = new string(buff, i + 8, 2);

                        Year = year;
                        Month = month;
                        Day = day;

                        if ((buff[i + 10] == 0x20) && (buff[i + 13] == ':') && (buff[i + 16] == ':'))
                        {
                            string hour = new string(buff, i + 11, 2);
                            string minute = new string(buff, i + 14, 2);
                            string second = new string(buff, i + 17, 2);

                            Hour = hour;
                            Minute = minute;
                            Second = second;
                            result = true;
                            break;
                        }
                    }
                }
                log.Debug("Image.Parse() {0}/{1}/{2} {3}:{4}:{5}", year, month, day, hour, minute, second);
            }
            catch (System.IndexOutOfRangeException e)
            {
                log.Error("Image.Parse() IndexOutOfRangeException occured. {0}", e.GetType());
            }
            log.Info("Image.Parse() end");

            return result;
        }

        public bool ParseFromTail(char[] buff)
        {
            return Parse(buff);
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
                    PhotoMVStat.already_exists++;
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
