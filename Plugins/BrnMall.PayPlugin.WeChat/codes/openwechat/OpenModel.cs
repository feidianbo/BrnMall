﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BrnMall.PayPlugin.OpenWeChat
{
    public class OpenModel
    {

        public string access_token { get; set; }
        public string expires_in { get; set; }
        public string refresh_token { get; set; }
        public string openid { get; set; }
        public string scope { get; set; }

    }
}
