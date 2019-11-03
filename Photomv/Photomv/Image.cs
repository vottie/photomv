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
        public static readonly char YEAR_PREFIX   = '2';
        public static readonly char DELIM_COLON   = ':';
        public static readonly char ZERO          = '0';
        public static readonly char DELIM_HYPHEN  = '-';

        private static Logger log = Logger.GetInstance(Common.TRCFILE, true);

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
            log.Debug("Image.Parse() start byteLen={0}", buff.Length);
            bool result = false;

            try
            {
                for (int i = 0; i < buff.Length - 1; i++)
                {
                    if (buff[i] != YEAR_PREFIX)
                        continue;
                    // search 2yyy:mm:dd
                    // i's value is '2'
                    if ((buff[i + 1] == ZERO) && (buff[i + 4] == DELIM_COLON) && (buff[i + 7] == DELIM_COLON))
                    {
                        string year = new string(buff, i, 4);
                        string month = new string(buff, i + 5, 2);
                        string day = new string(buff, i + 8, 2);

                        Year = year;
                        Month = month;
                        Day = day;

                        if ((buff[i + 10] == 0x20) && (buff[i + 13] == DELIM_COLON) && (buff[i + 16] == DELIM_COLON))
                        {
                            string hour = new string(buff, i + 11, 2);
                            string minute = new string(buff, i + 14, 2);
                            string second = new string(buff, i + 17, 2);

                            Hour = hour;
                            Minute = minute;
                            Second = second;
                            result = true;
                            log.Debug("Image.Parse() {0}/{1}/{2} {3}:{4}:{5}", year, month, day, hour, minute, second);
                            break;
                        }
                    }
                }
            }
            catch (System.IndexOutOfRangeException e)
            {
                log.Error("Image.Parse() IndexOutOfRangeException occured. {0}", e.GetType());
            }
            log.Debug("Image.Parse() end");

            return result;
        }

        public bool ParseFromTail(char[] buff)
        {
            log.Debug("Image.ParseFromTail() start byteLen={0}", buff.Length);
            bool result = false;

            try
            {
                for (int i = 0; i < buff.Length - 1; i++)
                {
                    if (buff[i] != YEAR_PREFIX)
                        continue;
                    // search 2yyy-mm-dd
                    // i's value is '2'
                    if ((buff[i + 1] == ZERO) && (buff[i + 4] == DELIM_HYPHEN) && (buff[i + 7] == DELIM_HYPHEN))
                    {
                        string year = new string(buff, i, 4);
                        string month = new string(buff, i + 5, 2);
                        string day = new string(buff, i + 8, 2);

                        Year = year;
                        Month = month;
                        Day = day;
                        // 2yyyy-mm-ddThh:mm:ss (T is 0x54)
                        if ((buff[i + 10] == 0x54) && (buff[i + 13] == DELIM_COLON) && (buff[i + 16] == DELIM_COLON))
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
                log.Debug("Image.ParseFromTail() {0}/{1}/{2} {3}:{4}:{5}", year, month, day, hour, minute, second);
            }
            catch (System.IndexOutOfRangeException e)
            {
                log.Error("Image.ParseFromTail() IndexOutOfRangeException occured. {0}", e.GetType());
            }
            log.Debug("Image.ParseFromTail() end");

            return result;
        }

        public bool PrepareCopyFile(string outDir)
        {
            try
            {
                log.Debug("Image.PrepareCopyFile() start outdir={0}", outDir);
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
                if (PhotoMVSingleton.GetInstance().IsRename)
                {
                    string ext = Path.GetExtension(Filename);
                    string new_name = Year + "-" + Month + "-" + Day + "_" + Hour + Minute + Second + ext;
                    buffer2.Append(new_name);
                    log.Debug("Image.Rename() New Name = {0}", new_name);
                } else
                {
                    buffer2.Append(Filename);
                }
                destfilename = buffer2.ToString();

                log.Debug("Image.PreparedCopyFile() end dest={0}", destfilename);
                if ((File.Exists(destfilename)))
                {
                    log.Info("Image.PreparedCopyFile() Already exsists {0}", destfilename);
                    PhotoMVStat.already_exists++;
                    string newname = Rename(destfilename);
                    if (newname != "")
                    {
                        this.DestFilename = newname;
                        return true;
                    } else
                    {
                        PhotoMVStat.copy_fail_times++;

                        string errfile = PhotoMVSingleton.GetInstance().Errfile;
                        string errmsg = "original=" + OrgPath + Environment.NewLine;
                        File.AppendAllText(errfile, errmsg);

                        return false;
                    }
                }
                return true;
            }
            catch (IOException e)
            {
                log.Error("Image.PreparedCopyFile IOException occured. {0}", e.GetType());
                return false;
            }
        }

        //
        // Rename arguments fname is full path
        //
        public string Rename(string fname)
        {
            log.Info("Image.Rename start");
            // separate file name, file extension
            string ext = Path.GetExtension(fname);
            string tmpname = Path.GetFileNameWithoutExtension(fname);
            int last = tmpname.Length;
            log.Info("Image.Rename tmpname length {0}", last);

            // file name check(already numbering?)
            // rename try. append _1
            // check
            // already exists?
            // increment

            string dir = Path.GetDirectoryName(fname);
            string[] files = Directory.GetFileSystemEntries(dir);
            for (int i = 0; i < files.Length; i++)
            {
                log.Debug("Image.Rename files[{0}]={1}", i, files[i]);
            }

            string result = "";
            result = dir + "\\" + tmpname + "_1" + ext;
            if (!File.Exists(result))
            {
                log.Info("Image.Rename orig:{0} new:{1}", fname, result);
                // tmp name is not exists.
                return result;
            } else
            {
                log.Debug("Image.Rename Already exists, another numbering {0}", result);

            }

            // _1 is already exists.
            log.Debug("Image.Rename tmpname {0}", tmpname);
            if (tmpname[last - 2] == '_')
            {
                char number = tmpname[last - 1];
                int n = Convert.ToInt32(number);
                if ((n == 0) || (n >= 10))
                {
                    log.Error("Image.Rename cannot rename file {0}", fname);
                    return "";
                }



                for (int i = n; i < 10; i++)
                {
                    string newname = tmpname.Substring(0, last - 2);
                    string numbering = "_" + number;
                    newname = dir + "\\" + newname + numbering + ext;
                    log.Debug("Image.Rename() newname is {0}", newname);
                    if (!File.Exists(newname))
                    {
                        log.Debug("Image.Rename() rename numbering {0}", newname);
                        return newname;
                    }
                }
                log.Debug("Image.Rename 0x0001");
            }
            else
            {
                for (int i = 2; i < 10; i++)
                {
                    // The name of file '_' is not used.
                    string re_rename = dir + "\\" + tmpname + "_" + Convert.ToString(i) + ext;
                    if (!File.Exists(re_rename))
                    {
                        log.Debug("Image.Rename() rename numbering {0}", re_rename);
                        return re_rename;
                    }
                }
            }
            log.Debug("Image.Rename 0x0002");
            return "";
        }
    }
}
