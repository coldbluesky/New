﻿#pragma checksum "..\..\..\AirCompressor.xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "EC590F4DA6CD8092EF7352CE684C306D2A287553"
//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
//     运行时版本:4.0.30319.42000
//
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Controls.Ribbon;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;
using Zhaoxi.DigitaPlatform.Components;
using Zhaoxi.DigitaPlatform.Components.Converter;


namespace Zhaoxi.DigitaPlatform.Components {
    
    
    /// <summary>
    /// AirCompressor
    /// </summary>
    public partial class AirCompressor : Zhaoxi.DigitaPlatform.Components.ComponentBase, System.Windows.Markup.IComponentConnector {
        
        
        #line 8 "..\..\..\AirCompressor.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Zhaoxi.DigitaPlatform.Components.AirCompressor root;
        
        #line default
        #line hidden
        
        
        #line 48 "..\..\..\AirCompressor.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.VisualState WarningState;
        
        #line default
        #line hidden
        
        
        #line 77 "..\..\..\AirCompressor.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Media.Effects.DropShadowEffect dse;
        
        #line default
        #line hidden
        
        
        #line 84 "..\..\..\AirCompressor.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Border grid;
        
        #line default
        #line hidden
        
        
        #line 189 "..\..\..\AirCompressor.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Primitives.ToggleButton btnInfo;
        
        #line default
        #line hidden
        
        
        #line 246 "..\..\..\AirCompressor.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Primitives.ToggleButton btnControl;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "7.0.1.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/Zhaoxi.DigitaPlatform.Components;component/aircompressor.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\AirCompressor.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "7.0.1.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.root = ((Zhaoxi.DigitaPlatform.Components.AirCompressor)(target));
            return;
            case 2:
            this.WarningState = ((System.Windows.VisualState)(target));
            return;
            case 3:
            this.dse = ((System.Windows.Media.Effects.DropShadowEffect)(target));
            return;
            case 4:
            this.grid = ((System.Windows.Controls.Border)(target));
            return;
            case 5:
            
            #line 143 "..\..\..\AirCompressor.xaml"
            ((System.Windows.Shapes.Ellipse)(target)).MouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(this.Ellipse_MouseLeftButtonDown);
            
            #line default
            #line hidden
            
            #line 144 "..\..\..\AirCompressor.xaml"
            ((System.Windows.Shapes.Ellipse)(target)).MouseMove += new System.Windows.Input.MouseEventHandler(this.Ellipse_MouseMove);
            
            #line default
            #line hidden
            
            #line 145 "..\..\..\AirCompressor.xaml"
            ((System.Windows.Shapes.Ellipse)(target)).MouseLeftButtonUp += new System.Windows.Input.MouseButtonEventHandler(this.Ellipse_MouseLeftButtonUp);
            
            #line default
            #line hidden
            return;
            case 6:
            
            #line 148 "..\..\..\AirCompressor.xaml"
            ((System.Windows.Shapes.Ellipse)(target)).MouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(this.Ellipse_MouseLeftButtonDown);
            
            #line default
            #line hidden
            
            #line 149 "..\..\..\AirCompressor.xaml"
            ((System.Windows.Shapes.Ellipse)(target)).MouseMove += new System.Windows.Input.MouseEventHandler(this.Ellipse_MouseMove);
            
            #line default
            #line hidden
            
            #line 150 "..\..\..\AirCompressor.xaml"
            ((System.Windows.Shapes.Ellipse)(target)).MouseLeftButtonUp += new System.Windows.Input.MouseButtonEventHandler(this.Ellipse_MouseLeftButtonUp);
            
            #line default
            #line hidden
            return;
            case 7:
            
            #line 153 "..\..\..\AirCompressor.xaml"
            ((System.Windows.Shapes.Ellipse)(target)).MouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(this.Ellipse_MouseLeftButtonDown);
            
            #line default
            #line hidden
            
            #line 154 "..\..\..\AirCompressor.xaml"
            ((System.Windows.Shapes.Ellipse)(target)).MouseMove += new System.Windows.Input.MouseEventHandler(this.Ellipse_MouseMove);
            
            #line default
            #line hidden
            
            #line 155 "..\..\..\AirCompressor.xaml"
            ((System.Windows.Shapes.Ellipse)(target)).MouseLeftButtonUp += new System.Windows.Input.MouseButtonEventHandler(this.Ellipse_MouseLeftButtonUp);
            
            #line default
            #line hidden
            return;
            case 8:
            
            #line 162 "..\..\..\AirCompressor.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.Button_Click);
            
            #line default
            #line hidden
            return;
            case 9:
            this.btnInfo = ((System.Windows.Controls.Primitives.ToggleButton)(target));
            return;
            case 10:
            this.btnControl = ((System.Windows.Controls.Primitives.ToggleButton)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}

