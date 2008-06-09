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
		public MagnetControl Magnet	{
			get { return magnet; }
		}
    	
    	#endregion
    	
    	#region Events
    	
    	public EventHandler<MagnetEditedEventArgs> MagnetEdited;    	
    	protected virtual void OnMagnetEdited(MagnetEditedEventArgs e)
    	{
    		EventHandler<MagnetEditedEventArgs> handler = MagnetEdited;
    		if (handler != null) {
    			handler(this,e);
    		}
    	}
    	
    	
    	public EventHandler<MagnetEventArgs> MagnetCreated;    	
    	protected virtual void OnMagnetCreated(MagnetEventArgs e)
    	{
    		EventHandler<MagnetEventArgs> handler = MagnetCreated;
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
            
            List<IdeaCategory> validCategories;
            if (magnet != null && magnet is BlueprintMagnetControl) {
            	validCategories = new List<IdeaCategory>(1);
            	validCategories.Add((IdeaCategory)Enum.Parse(typeof(IdeaCategory),"Toolset",true));
            	ideaCategoryComboBox.IsEnabled = false; // toolset magnets cannot change categories
            }
            else { // do this for new magnets, or existing magnets of any kind other than toolset magnets
            	validCategories = new List<IdeaCategory>(Idea.IDEA_CATEGORIES.Length - 1);
            	foreach (IdeaCategory category in Idea.IDEA_CATEGORIES) {
	            	if (category != IdeaCategory.Toolset) {
	            		validCategories.Add(category);
	            	}
           		}
            }
            
           	ideaCategoryComboBox.ItemsSource = validCategories;  
           	
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
        		Say.Warning("You haven't written an idea yet.");
        	}
        	else if (ideaCategoryComboBox.SelectedItem == null) {
        		Say.Warning("You haven't chosen a category for your idea yet.");
        	}
        	else {
        		IdeaCategory category = (IdeaCategory)ideaCategoryComboBox.SelectedItem;
        		if (magnet == null) {
        			Idea idea = new Idea(ideaTextBox.Text,category,User.GetCurrentUserName(),DateTime.Now);
        			magnet = new MagnetControl(idea);
        			OnMagnetCreated(new MagnetEventArgs(magnet));
        		}
        		else {
        			string originalText = magnet.Text;
        			IdeaCategory originalCategory = magnet.Category;        			
        			magnet.Text = ideaTextBox.Text;
        			magnet.Category = category;
        			OnMagnetEdited(new MagnetEditedEventArgs(magnet,originalText,originalCategory));
        		}
        		
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