/*
 * PhotoMVSingleton.cs
 * 
 * Copyright (c) 2019 vottie <tsuboi23@gmail.com>
 * 
 * This software is released under the MIT License.
 * see https://opensource.org/licenses/MIT
 */
using System;

namespace Photomv
{
    class PhotoMVSingleton
    {
        private static PhotoMVSingleton _singleton = new PhotoMVSingleton();

        private string mode;
        private bool isRename;
        private string logfile;
        private string errfile;
        private int result;

        public static PhotoMVSingleton GetInstance()
        {
            return _singleton;
        }

        private PhotoMVSingleton()
        {

        }

        public string Mode { get => mode; set => mode = value; }
        public bool IsRename { get => isRename; set => isRename = value; }
        public string Logfile { get => logfile; set => logfile = value; }
        public string Errfile { get => errfile; set => errfile = value; }
        public int Result { get => result; set => result = value; }

    }
}
