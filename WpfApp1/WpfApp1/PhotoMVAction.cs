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
        private static Logger log = Logger.GetInstance("./PhotomvLog.txt", true);
        string src, dest;
        List<Image> list = new List<Image>();

        public PhotoMVAction(string indir, string outdir)
        {
            log.Info("PhotoMVAction.PhotoMVAction() in={0} out={1}", indir, outdir);
            this.src = indir;
            this.dest = outdir;
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
            foreach(string str in files)
            {
                string ext = Path.GetExtension(str);
                log.Info("PhotoMVAction.searchDir() name={0} ext={1}", str, ext);

                if ((ext == ".jpg") || (ext == ".JPG"))
                {
                    log.Info("PhotoMVAction.searchDir() target={0} filename={1}",
                             str, Path.GetFileName(str));

                    string target;
                    if (dest.EndsWith("\\")) {
                        target = dest + Path.GetFileName(str);
                    } else {
                        target = dest + "\\" + Path.GetFileName(str);
                    }
                    Image img = new Image(str, Path.GetFileName(str));
                    list.Add(img);
                    log.Info("PhotoMVAction.searchDir() dest={0}", target);
                } else {
                    if (Directory.Exists(str)) {
                        // child directory recursive
                        SearchDir(str);
                    }
                }
            }
        }

        public void Execute()
        {
            log.Info("PhotoMVAction.execute() start in={0} out={1}", this.src, this.dest);
            SearchDir(src);

            foreach(Image img in list)
            {
                img.Execute(dest);
                //img.ResultMessage;
            }
            log.Info("PhotoMVAction.execute() end");
        }
    }
}
