﻿using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zhaoxi.AirCompression.Model
{
    public class UserModel : ObservableObject
    {
        public string UserId { get; set; } = "admin";

        private string _userName="admin";

        public string UserName
        {
            get { return _userName; }
            set { Set<string>(ref _userName, value); }
        }

        private string _password = "123456";

        public string Password
        {
            get { return _password; }
            set { Set<string>(ref _password, value); }
        }

        public string TeamNum { get; set; }
        public string TeamName { get; set; }
    }
}
