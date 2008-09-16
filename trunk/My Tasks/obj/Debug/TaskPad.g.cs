﻿#pragma checksum "..\..\TaskPad.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "A70ECCE17A88E8CFEC20A5871DE0EFEA"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.1433
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using AdventureAuthor.Tasks;
using AdventureAuthor.Utils;
using System;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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


namespace AdventureAuthor.Tasks {
    
    
    /// <summary>
    /// TaskPad
    /// </summary>
    public partial class TaskPad : System.Windows.Controls.UserControl, System.Windows.Markup.IComponentConnector, System.Windows.Markup.IStyleConnector {
        
        
        #line 11 "..\..\TaskPad.xaml"
        internal AdventureAuthor.Tasks.TaskPad rarface;
        
        #line default
        #line hidden
        
        
        #line 122 "..\..\TaskPad.xaml"
        internal System.Windows.Controls.Grid MainGrid;
        
        #line default
        #line hidden
        
        
        #line 130 "..\..\TaskPad.xaml"
        internal System.Windows.Controls.TextBlock headingTextBlock;
        
        #line default
        #line hidden
        
        
        #line 150 "..\..\TaskPad.xaml"
        internal System.Windows.Controls.ListBox taskListBox;
        
        #line default
        #line hidden
        
        
        #line 199 "..\..\TaskPad.xaml"
        internal System.Windows.Controls.Button DeleteTaskButton;
        
        #line default
        #line hidden
        
        
        #line 203 "..\..\TaskPad.xaml"
        internal System.Windows.Controls.Button MoveTaskUpButton;
        
        #line default
        #line hidden
        
        
        #line 207 "..\..\TaskPad.xaml"
        internal System.Windows.Controls.Button MoveTaskDownButton;
        
        #line default
        #line hidden
        
        
        #line 218 "..\..\TaskPad.xaml"
        internal System.Windows.Controls.Button AddTaskButton;
        
        #line default
        #line hidden
        
        
        #line 223 "..\..\TaskPad.xaml"
        internal System.Windows.Controls.StackPanel filterControlsPanel;
        
        #line default
        #line hidden
        
        
        #line 255 "..\..\TaskPad.xaml"
        internal System.Windows.Controls.RadioButton showAllTasksRadioButton;
        
        #line default
        #line hidden
        
        
        #line 271 "..\..\TaskPad.xaml"
        internal System.Windows.Controls.TextBox searchStringTextBox;
        
        #line default
        #line hidden
        
        
        #line 282 "..\..\TaskPad.xaml"
        internal System.Windows.Controls.ListView tagFilterListView;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/My Tasks;component/taskpad.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\TaskPad.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.rarface = ((AdventureAuthor.Tasks.TaskPad)(target));
            
            #line 8 "..\..\TaskPad.xaml"
            this.rarface.KeyDown += new System.Windows.Input.KeyEventHandler(this.HandleKeyPresses);
            
            #line default
            #line hidden
            return;
            case 5:
            this.MainGrid = ((System.Windows.Controls.Grid)(target));
            return;
            case 6:
            this.headingTextBlock = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 7:
            this.taskListBox = ((System.Windows.Controls.ListBox)(target));
            return;
            case 8:
            this.DeleteTaskButton = ((System.Windows.Controls.Button)(target));
            
            #line 200 "..\..\TaskPad.xaml"
            this.DeleteTaskButton.Click += new System.Windows.RoutedEventHandler(this.DeleteSelectedTask);
            
            #line default
            #line hidden
            return;
            case 9:
            this.MoveTaskUpButton = ((System.Windows.Controls.Button)(target));
            
            #line 204 "..\..\TaskPad.xaml"
            this.MoveTaskUpButton.Click += new System.Windows.RoutedEventHandler(this.MoveSelectedTaskUp);
            
            #line default
            #line hidden
            return;
            case 10:
            this.MoveTaskDownButton = ((System.Windows.Controls.Button)(target));
            
            #line 208 "..\..\TaskPad.xaml"
            this.MoveTaskDownButton.Click += new System.Windows.RoutedEventHandler(this.MoveSelectedTaskDown);
            
            #line default
            #line hidden
            return;
            case 11:
            this.AddTaskButton = ((System.Windows.Controls.Button)(target));
            return;
            case 12:
            this.filterControlsPanel = ((System.Windows.Controls.StackPanel)(target));
            return;
            case 13:
            this.showAllTasksRadioButton = ((System.Windows.Controls.RadioButton)(target));
            
            #line 257 "..\..\TaskPad.xaml"
            this.showAllTasksRadioButton.Checked += new System.Windows.RoutedEventHandler(this.ShowBothCompletedAndUncompletedTasks);
            
            #line default
            #line hidden
            return;
            case 14:
            
            #line 260 "..\..\TaskPad.xaml"
            ((System.Windows.Controls.RadioButton)(target)).Checked += new System.Windows.RoutedEventHandler(this.ShowOnlyUncompletedTasks);
            
            #line default
            #line hidden
            return;
            case 15:
            
            #line 262 "..\..\TaskPad.xaml"
            ((System.Windows.Controls.RadioButton)(target)).Checked += new System.Windows.RoutedEventHandler(this.ShowOnlyCompletedTasks);
            
            #line default
            #line hidden
            return;
            case 16:
            this.searchStringTextBox = ((System.Windows.Controls.TextBox)(target));
            
            #line 272 "..\..\TaskPad.xaml"
            this.searchStringTextBox.TextChanged += new System.Windows.Controls.TextChangedEventHandler(this.ShowOnlyTasksContainingSearchString);
            
            #line default
            #line hidden
            return;
            case 17:
            this.tagFilterListView = ((System.Windows.Controls.ListView)(target));
            
            #line 283 "..\..\TaskPad.xaml"
            this.tagFilterListView.SelectionChanged += new System.Windows.Controls.SelectionChangedEventHandler(this.OnlyShowTasksWithSelectedTag);
            
            #line default
            #line hidden
            return;
            case 18:
            
            #line 330 "..\..\TaskPad.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.ClearTagFilter);
            
            #line default
            #line hidden
            return;
            case 19:
            
            #line 334 "..\..\TaskPad.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.ClearAllFilters);
            
            #line default
            #line hidden
            return;
            case 20:
            
            #line 338 "..\..\TaskPad.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.ShowOrHideFilteringControls);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        void System.Windows.Markup.IStyleConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 2:
            
            #line 74 "..\..\TaskPad.xaml"
            ((System.Windows.Controls.MenuItem)(target)).Click += new System.Windows.RoutedEventHandler(this.MoveSelectedTaskDown);
            
            #line default
            #line hidden
            break;
            case 3:
            
            #line 76 "..\..\TaskPad.xaml"
            ((System.Windows.Controls.MenuItem)(target)).Click += new System.Windows.RoutedEventHandler(this.MoveSelectedTaskUp);
            
            #line default
            #line hidden
            break;
            case 4:
            
            #line 79 "..\..\TaskPad.xaml"
            ((System.Windows.Controls.MenuItem)(target)).Click += new System.Windows.RoutedEventHandler(this.DeleteSelectedTask);
            
            #line default
            #line hidden
            break;
            }
        }
    }
}

