/*
 * PhotoMVStat.cs
 * 
 * Copyright (c) 2019 vottie <tsuboi23@gmail.com>
 * 
 * This software is released under the MIT License.
 * see https://opensource.org/licenses/MIT
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Photomv
{
    class PhotoMVStat
    {
        private static Logger log = Logger.GetInstance(Common.TRCFILE, true);

        private static PhotoMVStat _singleton = new PhotoMVStat();

        public static uint copy_success_photos;
        public static uint copy_success_videos;
        public static uint copy_fail_times;
        public static uint already_exists;

        public static PhotoMVStat GetInstance()
        {
            return _singleton;
        }

        private PhotoMVStat()
        {
        }

        public void Initialize()
        {
            copy_success_photos = 0;
            copy_success_videos = 0;
            copy_fail_times = 0;
            already_exists = 0;
        }

        public void Output()
        {
            log.Info("========= PhotoMV Statistics ========");
            log.Info("copy successed photos : {0} times", copy_success_photos);
            log.Info("copy successed videos : {0} times", copy_success_videos);
            log.Info("copy failed           : {0} times", copy_fail_times);
            log.Info("already exists        : {0} times", already_exists);
            log.Info("========= PhotoMV Statistics ========");
        }
    }
}
