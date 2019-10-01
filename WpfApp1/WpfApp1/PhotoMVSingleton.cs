using System;

namespace Photomv
{
    class PhotoMVSingleton
    {
        private static PhotoMVSingleton _singleton = new PhotoMVSingleton();

        private string mode;
        private bool isRename;

        public static PhotoMVSingleton GetInstance()
        {
            return _singleton;
        }

        private PhotoMVSingleton()
        {

        }

        public string Mode { get => mode; set => mode = value; }

        public bool IsRename { get => isRename; set => isRename = value; }

    }
}
