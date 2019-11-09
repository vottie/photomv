/*
 * Common.cs
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
    class Common
    {
        // Version
        public static readonly string VERSION = "0.9.2";

        // File
        public static readonly string LOGILE  = "photomv_success.txt";
        public static readonly string ERRFILE = "photomv_error.txt";
        public static readonly string INIFILE = "./photomv.ini";
        public static readonly string TRCFILE = "./photomv.log";

        // result
        public static readonly int NORMAL_END = 0;
        public static readonly int IO_ERR = -1;
        public static readonly int UNAUTH_ACCESS_ERR = -2;
        public static readonly int PARSE_FAILURE = -3;




    }
}
