﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using AdventureAuthor.Utils;

namespace AdventureAuthor.Notebook.Worksheets.UI
{
    /// <summary>
    /// Interaction logic for StarRating.xaml
    /// </summary>

    public partial class StarRating : UserControl
    {
    	public string TitleText {
    		get {
    			return TitleBlock.Text;
    		}
    		set {
    			TitleBlock.Text = value;
    		}
    	}
    	
    	
    	public object Tip {
    		get {
    			return TitleBlock.ToolTip;
    		}
    		set {
    			TitleBlock.ToolTip = value;
    		}
    	}
    	
    	    	
    	public StarRating() : this(5)
    	{
        }
        
        
        public StarRating(int stars)
        {
            InitializeComponent();
        	for (int i = 0; i < stars; i++) {
        		CheckBox button = new CheckBox();
        		button.Template = (ControlTemplate)Resources["StarTemplate"];
        		button.Click += new RoutedEventHandler(OnClick);
        		StarsPanel.Children.Add(button);
        	}
        }
        
        
        public StarRating(int stars, string subject) : this(stars)
        {
        	TitleText = subject;
        }
        
        
        private void OnClick(object sender, RoutedEventArgs e)
        {
        	// Highlight all the stars prior to this one:
        	bool shouldCheck = true;
        	foreach (UIElement uielement in StarsPanel.Children) {
        		if (uielement is CheckBox) {        		
	        		CheckBox star = (CheckBox)uielement;
	        		star.IsChecked = shouldCheck;	        			
		        	if (shouldCheck && star == sender) {
		        		shouldCheck = false;
		        	}
        		}
        	}
        }
           
        
        public int Rating
        {
        	get {    
        		int rating = 0;        		
	        	foreach (UIElement uielement in StarsPanel.Children) {
	        		if (uielement is CheckBox) {        		
		        		CheckBox star = (CheckBox)uielement;
		        		if ((bool)star.IsChecked) {
		        			rating++;
		        		}
	        		}
	        	}        		
        		return rating;
        	}
        	set {
        		int starsChecked = 0;
        		bool shouldCheck = true;
        		
	        	foreach (UIElement uielement in StarsPanel.Children) {
	        		if (uielement is CheckBox) {
	        			if (shouldCheck && starsChecked == value) {
	        				shouldCheck = false;
	        			}     		
		        		CheckBox star = (CheckBox)uielement;
		        		star.IsChecked = shouldCheck;
		        		if ((bool)star.IsChecked) {
		        			starsChecked++;
		        		}
	        		}
	        	}
        	}
        }
    }
}