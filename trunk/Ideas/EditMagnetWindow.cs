using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using AdventureAuthor.Utils;

namespace AdventureAuthor.Ideas
{
    public partial class EditMagnetWindow : Window
    {    	
    	#region Fields
    	
    	private MagnetControl magnet;
    	
    	#endregion
    	
    	#region Events
    	
    	public EventHandler<MagnetEventArgs> MagnetEdited;    	
    	protected virtual void OnMagnetEdited(MagnetEventArgs e)
    	{
    		EventHandler<MagnetEventArgs> handler = MagnetEdited;
    		if (handler != null) {
    			handler(this,e);
    		}
    	}
    	
    	#endregion
    	
    	#region Constructors
    	
    	public EditMagnetWindow() : this(null)
    	{
    		
    	}
    	
    	
        public EditMagnetWindow(MagnetControl magnet) 
        {
        	this.magnet = magnet;
        	
            InitializeComponent();
            ideaTextBox.MaxLength = Idea.MAX_IDEA_LENGTH;
           	ideaCategoryComboBox.ItemsSource = Idea.IDEA_CATEGORIES;  
           	
           	if (magnet != null) {
	        	ideaTextBox.Text = magnet.Idea.Text; 
	        	ideaCategoryComboBox.SelectedItem = magnet.Idea.Category;             		
           	}
        }

        #endregion
        
        #region Event handlers
        
        private void OnClick_OK(object sender, EventArgs e)
        {
        	if (ideaTextBox.Text == String.Empty) {
        		Say.Warning("You can't create a blank idea.");
        	}
        	else {
        		IdeaCategory category;
        		if (ideaCategoryComboBox.SelectedItem != null) {
	        		category = (IdeaCategory)ideaCategoryComboBox.SelectedItem;
	        	}
	        	else {
	        		category = IdeaCategory.Other;
	        	}
        		        		
        		if (magnet == null) {
        			Idea idea = new Idea(ideaTextBox.Text,category,User.GetCurrentUser(),DateTime.Now);
        			magnet = new MagnetControl(idea);
        		}
        		else {
        			magnet.Text = ideaTextBox.Text;
        			magnet.Category = category;
        		}
        		
        		OnMagnetEdited(new MagnetEventArgs(magnet));
	        	Close();
        	}
        }
        
        
        private void OnClick_Cancel(object sender, EventArgs e)
        {
        	Close();
        }
        
        #endregion
    }
}