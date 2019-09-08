using System;

namespace WpfApp1
{
    class PhotoMVSingleton
    {
        private static PhotoMVSingleton _singleton = new PhotoMVSingleton();

        public static PhotoMVSingleton GetInstance()
        {
            return _singleton;
        }

        private PhotoMVSingleton()
        {

        }
    }
}
