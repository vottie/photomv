using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;

namespace Photomv
{
    class Photo : Image
    {
        private static Logger log = Logger.GetInstance("./PhotomvLog.txt", true);

        public Photo(string path, string name) : base(path, name)
        {
        }
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
                    if (PhotoMVSingleton.GetInstance().Mode == "debug")
                    {
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

    }
}
