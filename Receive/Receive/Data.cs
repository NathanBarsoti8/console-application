﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Receive
{
    [Serializable]
    public class Data
    {
        public DateTime Date { get; set; }
        public string Ip { get; set; }
        public string Url { get; set; }
    }
}
