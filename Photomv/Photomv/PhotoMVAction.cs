using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Photomv
{
    class PhotoMVAction
    {
        private static Logger log = Logger.GetInstance(Common.TRCFILE, true);
        string src, dest;
        List<Photo> photos = new List<Photo>();
        List<Video> videos = new List<Video>();

        public PhotoMVAction(string indir, string outdir)
        {
            bool isRename = PhotoMVSingleton.GetInstance().IsRename;
            log.Info("PhotoMVAction Version={0}", Common.VERSION);
            log.Info("PhotoMVAction.PhotoMVAction() in={0} out={1} isRename={2}",
                indir, outdir, isRename);
            this.src = indir;
            this.dest = outdir;

            // statistics initialize
            PhotoMVStat stat = PhotoMVStat.GetInstance();
            stat.Initialize();
        }
            
        private void SearchDir(string path)
        {
            if (!Directory.Exists(path))
            {
                log.Error("PhotoMVAction.searchDir() not exsits {0}", path);
                return;
            }
            string[] files = Directory.GetFileSystemEntries(path);

            // str is full path
            foreach (string str in files)
            {
                string ext = Path.GetExtension(str);
                log.Debug("PhotoMVAction.searchDir() name={0} ext={1}", str, ext);

                /*
                 *  JPEG file processing
                 */
                if ((ext == ".jpg") || (ext == ".JPG"))
                {
                    log.Debug("PhotoMVAction.searchDir() JPEG target={0} filename={1}",
                             str, Path.GetFileName(str));

                    string target;
                    if (dest.EndsWith("\\")) {
                        target = dest + Path.GetFileName(str);
                    } else {
                        target = dest + "\\" + Path.GetFileName(str);
                    }
                    Photo photo = new Photo(str, Path.GetFileName(str));
                    photos.Add(photo);

                    log.Debug("PhotoMVAction.searchDir() dest={0}", target);
                }
                /*
                 *  MOV file processing
                 */
                else if ((ext == ".mov") || (ext == ".MOV"))
                {
                    log.Debug("PhotoMVAction.searchDir() MOV target={0} filename={1}",
                             str, Path.GetFileName(str));

                    string target;
                    if (dest.EndsWith("\\"))
                    {
                        target = dest + Path.GetFileName(str);
                    }
                    else
                    {
                        target = dest + "\\" + Path.GetFileName(str);
                    }
                    Video video = new Video(str, Path.GetFileName(str));
                    videos.Add(video);

                    log.Debug("PhotoMVAction.searchDir() dest={0}", target);


                }
                else {
                    if (Directory.Exists(str)) {
                        // child directory recursive
                        SearchDir(str);
                    }
                }
            }
        }

        public int Execute()
        {
            log.Info("PhotoMVAction.execute() start in={0} out={1}", this.src, this.dest);

            int pid = System.Diagnostics.Process.GetCurrentProcess().Id;
            int tid = System.Threading.Thread.CurrentThread.ManagedThreadId;
            log.Debug("PhotoMVAction.execute() start PID={0} TID={1}", pid, tid);

            SearchDir(src);

            foreach(Photo photo in photos)
            {
                photo.Execute(dest);
                //img.ResultMessage;
            }

            foreach (Video video in videos)
            {
                video.Execute(dest);
                //img.ResultMessage;
            }

            PhotoMVStat stat = PhotoMVStat.GetInstance();
            stat.Output();
            log.Info("PhotoMVAction.execute() end");

            // set result
            PhotoMVSingleton.GetInstance().Result = 0;

            return 0;
        }
    }
}
