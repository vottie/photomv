using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Photomv
{
    class PhotoMVStat
    {
        private static Logger log = Logger.GetInstance("./PhotomvLog.txt", true);

        private static PhotoMVStat _singleton = new PhotoMVStat();

        public static uint copy_success_times;
        public static uint copy_fail_times;

        public static PhotoMVStat GetInstance()
        {
            return _singleton;
        }

        private PhotoMVStat()
        {
        }

        public void Initialize()
        {
            copy_success_times = 0;
            copy_fail_times = 0;
        }

        public void Output()
        {
            log.Info("========= PhotoMV Statistics ========");
            log.Info("copy successed : {0} times", copy_success_times);
            log.Info("copy failed    : {0} times", copy_fail_times);
            log.Info("========= PhotoMV Statistics ========");
        }
    }
}
