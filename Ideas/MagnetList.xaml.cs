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
using System.Windows.Navigation;
using System.Windows.Shapes;
using AdventureAuthor.Utils;

namespace AdventureAuthor.Ideas
{
    /// <summary>
    /// Interaction logic for MagnetList.xaml
    /// </summary>

    public partial class MagnetList : UserControl
    {
    	#region Fields   
    	
    	private List<IdeaCategory> visibleCategories;
    	
    	#endregion
    	
    	#region Events
    	
    	public event EventHandler<MagnetEventArgs> MagnetAdded;    	
    	protected virtual void OnMagnetAdded(MagnetEventArgs e)
    	{
    		EventHandler<MagnetEventArgs> handler = MagnetAdded;
    		if (handler != null) {
    			handler(this,e);
    		}
    	}
    	
    	
    	public event EventHandler<MagnetEventArgs> MagnetDeployed;    	
    	protected virtual void OnMagnetDeployed(MagnetEventArgs e)
    	{
    		EventHandler<MagnetEventArgs> handler = MagnetDeployed;
    		if (handler != null) {
    			handler(this,e);
    		}
    	}
    	
    	
    	public event EventHandler<MagnetEventArgs> MagnetDeleted;    	
    	protected virtual void OnMagnetDeleted(MagnetEventArgs e)
    	{
    		EventHandler<MagnetEventArgs> handler = MagnetDeleted;
    		if (handler != null) {
    			handler(this,e);
    		}
    	}
    	    	
    	
    	public event EventHandler<MagnetEventArgs> MagnetSelected;    	
		protected virtual void OnMagnetSelected(MagnetEventArgs e)
		{
			EventHandler<MagnetEventArgs> handler = MagnetSelected;
			if (handler != null) {
				handler(this, e);
			}
		}
		
    	
    	public event EventHandler VisibleCategoriesChanged;   	
		protected virtual void OnVisibleCategoriesChanged(EventArgs e)
		{
			EventHandler handler = VisibleCategoriesChanged;
			if (handler != null) {
				handler(this, e);
			}
		}
    	
    	#endregion
    	
    	#region Constructors
    	
        public MagnetList()
        {
            InitializeComponent();
            visibleCategories = new List<IdeaCategory>(Idea.IDEA_CATEGORIES.Length);
            foreach (IdeaCategory category in Idea.IDEA_CATEGORIES) {
            	visibleCategories.Add(category);
            }
        }
        
        
        public MagnetList(List<Idea> ideas) : this()
        {        	
        	AddIdeas(ideas);
        }
        
        #endregion
        
        #region Methods
        
        /// <summary>
        /// Add an idea to the list.
        /// </summary>
        /// <param name="idea">The idea to add</param>
        public void AddIdea(Idea idea)
        {
        	MagnetControl magnet = new MagnetControl(idea);
        	AddNewMagnet(magnet);
        }
        
        
        /// <summary>
        /// Add a range of ideas to the list.
        /// </summary>
        /// <param name="ideas">The range of ideas to add</param>
        public void AddIdeas(List<Idea> ideas)
        {
        	foreach (Idea idea in ideas) {
        		AddIdea(idea);
        	}
        }
        
        
        internal void TransferMagnetFromBoard(MagnetControl magnet)
        {
        	AddMagnet(magnet,false);
        }
        
        
        public void AddNewMagnet(MagnetControl magnet)
        {
        	AddMagnet(magnet,true);
        }
        
        
        /// <summary>
        /// Add a magnet representing an idea to the list
        /// </summary>
        /// <param name="magnet">The magnet to add</param>
        /// <param name="">True if the magnet has just been created; false otherwise</param>
        /// <remarks>All Add methods ultimately call this method</remarks>
        private void AddMagnet(MagnetControl magnet, bool newlyCreated)
        {
        	if (newlyCreated) {
	        	magnet.MouseDoubleClick += new MouseButtonEventHandler(MagnetControl_MouseDoubleClick);
	        	magnet.Selected += delegate(object sender, MagnetEventArgs e) { OnMagnetSelected(e); };
        	}
        	else {
        		magnet.Angle = 0;
        	}
        	magnet.Margin = new Thickness(5);
        	
        	magnetsPanel.Children.Add(magnet);
        	
        	// If the magnet is newly created, ensure that it is shown, even if it is in a category which
        	// is currently hidden. If it's been transferred back to the magnet list after being deleted
        	// from a board, then it can retain the same shown/hidden state as its category.
        	bool categoryIsVisible = CategoryIsVisible(magnet.Idea.Category);
        	if (newlyCreated) {
	        	if (!categoryIsVisible) {
	        		ShowCategory(magnet.Idea.Category);
	        	}
	        	magnet.BringIntoView();
        	}
        	else {
        		if (!categoryIsVisible) {
        			magnet.Hide();
        		}
        	}
        	OnMagnetAdded(new MagnetEventArgs(magnet));
        }
        
        
        /// <summary>
        /// Remove a magnet from the list
        /// </summary>
        /// <param name="magnet"></param>
        /// <remarks>A magnet being 'deployed' rather than 'deleted' means it has been moved
        /// to a magnet board rather than being deleted permanently - MagnetLeavingList event is launched</remarks>
        public void DeployMagnet(MagnetControl magnet)
        {
        	magnetsPanel.Children.Remove(magnet);
    		OnMagnetDeployed(new MagnetEventArgs(magnet));
        }
        
        
        /// <summary>
        /// Delete a magnet from the list
        /// </summary>
        /// <param name="magnet"></param>
        /// <remarks>A magnet being 'deleted' rather than 'removed' means it has been deleted permanently rather
        /// than being moved to a magnet board - MagnetDeleted event is launched</remarks>
        public void DeleteMagnet(MagnetControl magnet)
        {
        	magnetsPanel.Children.Remove(magnet);
    		OnMagnetDeleted(new MagnetEventArgs(magnet));
        }
                
        
        /// <summary>
        /// Show ideas belonging to a particular category.
        /// </summary>
        /// <param name="category">The category of ideas to show</param>
        public void ShowCategory(IdeaCategory category)
        {
        	ShowCategory(category,false);
        }
        
        
        /// <summary>
        /// Show ideas belonging to a particular category.
        /// </summary>
        /// <param name="category">The category of ideas to show</param>
        /// <param name="only">True to show only the given category, and hide everything else; 
        /// false to show the given category and ignore everything else</param>
        public void ShowCategory(IdeaCategory category, bool only)
        {
        	// Update list of visible categories:
        	if (only) {
        		visibleCategories.Clear();
        	}
        	if (!visibleCategories.Contains(category)) {
        		visibleCategories.Add(category);
        	}
        	
        	foreach (MagnetControl magnet in magnetsPanel.Children) {
        		if (magnet.Idea.Category == category) {
        			magnet.Show();
        		}
        		else if (only) {
        			magnet.Hide();
        		}
        	}
        	
        	OnVisibleCategoriesChanged(new EventArgs());
        }
                
        
        /// <summary>
        /// Hide ideas belonging to a particular category.
        /// </summary>
        /// <param name="category">The category of ideas to hide</param>
        public void HideCategory(IdeaCategory category)
        {
        	HideCategory(category,false);
        }
        
                
        /// <summary>
        /// Hide ideas belonging to a particular category.
        /// </summary>
        /// <param name="category">The category of ideas to hide</param>
        /// <param name="only">True to hide only the given category, and show everything else; 
        /// false to hide the given category and ignore everything else</param>
        public void HideCategory(IdeaCategory category, bool only)
        {
        	// Update list of visible categories:
        	if (only) {
        		visibleCategories.Clear();
        		foreach (IdeaCategory cat in Idea.IDEA_CATEGORIES) {
        			visibleCategories.Add(cat);
        		}
        	}
        	if (visibleCategories.Contains(category)) {
        		visibleCategories.Remove(category);
        	}        	
        	
        	foreach (MagnetControl magnet in magnetsPanel.Children) {
        		if (magnet.Idea.Category == category) {
        			magnet.Hide();
        		}
        		else if (only) {
        			magnet.Show();
        		}
        	}
        	
        	OnVisibleCategoriesChanged(new EventArgs());
        }
        
        
        public List<MagnetControl> GetVisibleMagnets()
        {
        	List<MagnetControl> magnets = new List<MagnetControl>(magnetsPanel.Children.Count);
        	foreach (MagnetControl magnet in magnetsPanel.Children) {
        		if (magnet.IsVisible) {
        			magnets.Add(magnet);
        		}
        	}
        	return magnets;
        }
        
        
        public void Scatter()
        {
        	List<MagnetControl> magnets = GetVisibleMagnets();
        	foreach (MagnetControl magnet in magnets) {
        		DeployMagnet(magnet);
        	}
        }
    	
    	
    	public bool HasMagnet(MagnetControl magnet)
    	{
    		return magnetsPanel.Children.Contains(magnet);
    	}
    	
    	
    	public bool CategoryIsVisible(IdeaCategory category)
    	{
    		return visibleCategories.Contains(category);
    	}
                
        #endregion
        
        #region Event handlers
        
    	private void MagnetControl_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    	{
    		MagnetControl magnet = (MagnetControl)e.Source;
    		if (magnetsPanel.Children.Contains(magnet)) {
    			DeployMagnet(magnet);
    		}
    	}
    		
        #endregion
    }
}