﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Zhaoxi.AirCompression.Components
{
    /// <summary>
    /// AirCompressor.xaml 的交互逻辑
    /// </summary>
    public partial class AirCompressor : ComponentBase
    {
        public AirCompressor()
        {
            InitializeComponent();
        }

        private void Path_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Command?.Execute(new object[] { this.DataContext, this.ComponentIndex, this.ComponentId });
        }
    }
}