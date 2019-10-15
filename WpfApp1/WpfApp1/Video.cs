using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Photomv
{
    class Video : Image
    {
        private static Logger log = Logger.GetInstance("./PhotomvLog.txt", true);
        public Video(string path, string name) : base(path, name)
        {
        }
        public void Execute(string dest)
        {
            log.Info("Video.Execute() dest={0} orig={1} filename={2}", dest, OrgPath, Filename);
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

                char[] cBuff = System.Text.Encoding.GetEncoding(932).GetString(buff).ToCharArray();
                if (!Parse(cBuff))
                {
                    /*
                     *  iPhone
                     *    format : yyyy-mm-ddThh:mm:ss
                     */
                    byte[] rbuff = new byte[4096]; // reverse buffer
                    long offset = fs.Seek(-4096, SeekOrigin.End);
                    // FileInfo file = new FileInfo(OrgPath);
                    // fs.Position = file.Length - 4096;
                    // log.Debug("Video.Execute() seek offset = {0} finfo.Length = {1}", offset, file.Length);

                    fs.Read(rbuff, 0, 4096);
                    char[] rcBuff = System.Text.Encoding.GetEncoding(932).GetString(rbuff).ToCharArray();

                    if (!ParseFromTail(rcBuff))
                    {
                        log.Error("Video.Execute() Parse Fail {0}", OrgPath);
                        return;
                    }
                }

                fs.Close();


                if (PrepareCopyFile(dest))
                {
                    if (PhotoMVSingleton.GetInstance().Mode == "debug")
                    {
                        log.Debug("Video.Execute() pseudo");
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
    }
}
