using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace photomv
{
    class PhotoMVAction
    {
        string src, dest;
        List<Image> list = new List<Image>();


        public PhotoMVAction(string indir, string outdir)
        {
            Console.WriteLine("Photomv in={0} out={1}", indir, outdir);
            this.src = indir;
            this.dest = outdir;
        }
            
        private void searchDir(string path)
        {
            if (!Directory.Exists(path))
            {
                Console.WriteLine("Photomv not exists {0}", path);
                return;
            }
            string[] files = Directory.GetFileSystemEntries(path);

            // str is full path
            foreach(string str in files)
            {
                string ext = Path.GetExtension(str);
                Console.WriteLine("name : {0} ext : {1}", str, ext);

                if ((ext == ".jpg") || (ext == ".JPG"))
                {
                    Console.WriteLine("target {0} filename {1}", str, Path.GetFileName(str));

                    string target;
                    if (dest.EndsWith("\\")) {
                        target = dest + Path.GetFileName(str);
                    } else {
                        target = dest + "\\" + Path.GetFileName(str);
                    }
                    Image img = new Image(str, Path.GetFileName(str));
                    list.Add(img);
                    Console.WriteLine("destination {0}", target);
                } else {
                    if (Directory.Exists(str)) {
                        // child directory recursive
                        searchDir(str);
                    }
                }
            }
        }

        public void execute()
        {
            Console.WriteLine("PhotoMVAction execute start in={0} out={1}", this.src, this.dest);
            searchDir(src);

            foreach(Image img in list)
            {
                //img.execute(dest);
                //img.ResultMessage;
            }
            Console.WriteLine("PhotoMVAction execute completed");
        }
    }
}
