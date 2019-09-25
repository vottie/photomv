using System;

namespace Photomv
{
    class PhotoMVSingleton
    {
        private static PhotoMVSingleton _singleton = new PhotoMVSingleton();

        private string mode;
        public static PhotoMVSingleton GetInstance()
        {
            return _singleton;
        }

        private PhotoMVSingleton()
        {

        }

        public string Mode { get => mode; set => mode = value; }
    }
}
