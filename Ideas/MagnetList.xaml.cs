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
    	
    	
    	private SortCriterion sortBy = SortCriterion.Alphabetically;
		public SortCriterion SortBy {
			get { return sortBy; }
			set { 
				sortBy = value;
			}
		}
        
        
        private MouseButtonEventHandler magnetControl_MouseDoubleClickHandler;        
        private EventHandler<MagnetEventArgs> magnetControl_SelectedHandler;        
        private DragEventHandler magnetControl_DropHandler;
    	
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
    	
    	
    	public event EventHandler<MagnetRemovedEventArgs> MagnetRemoved;    	
    	protected virtual void OnMagnetRemoved(MagnetRemovedEventArgs e)
    	{
    		EventHandler<MagnetRemovedEventArgs> handler = MagnetRemoved;
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
		
    	
    	public event EventHandler VisibleMagnetsChanged;   	
		protected virtual void OnVisibleMagnetsChanged(EventArgs e)
		{
			EventHandler handler = VisibleMagnetsChanged;
			if (handler != null) {
				handler(this, e);
			}
		}
    	
    	#endregion
    	
    	#region Constructors
    	
        public MagnetList()
        {
        	magnetControl_MouseDoubleClickHandler = new MouseButtonEventHandler(magnetControl_MouseDoubleClick);
        	magnetControl_SelectedHandler = new EventHandler<MagnetEventArgs>(magnetControl_Selected);
        	magnetControl_DropHandler = new DragEventHandler(magnetControl_Drop);
        	
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
        	AddMagnet(magnet,true);
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
        
        
        private void AddHandlers(MagnetControl magnet)
        {
        	try {
        		magnet.MouseDoubleClick += magnetControl_MouseDoubleClickHandler;
        		magnet.Selected += magnetControl_SelectedHandler;
        		magnet.Drop += magnetControl_DropHandler;
        	}
        	catch (Exception e) {
        		Say.Error("Failed to add handlers to magnet.",e);
        	}
        }
        
        
        private void RemoveHandlers(MagnetControl magnet)
        {
        	try {
        		magnet.MouseDoubleClick -= magnetControl_MouseDoubleClickHandler;
        		magnet.Selected -= magnetControl_SelectedHandler;
        		magnet.Drop -= magnetControl_DropHandler;
        	}
        	catch (Exception e) {
        		Say.Error("Failed to remove handlers from magnet.",e);
        	}
        }
        
        
        /// <summary>
        /// Add a magnet representing an idea to the list
        /// </summary>
        /// <param name="magnet">The magnet to add</param>
        /// <param name="forceShow">True to force this magnet's category to be shown; false otherwise</param>
        /// <remarks>All Add methods ultimately call this method</remarks>
        public void AddMagnet(MagnetControl magnet, bool forceShow)
        {
        	AddHandlers(magnet);
        	
        	magnet.Angle = 0;
        	magnet.Margin = new Thickness(5);
        	magnet.bringToFrontMenuItem.IsEnabled = false;
        	magnet.sendToBackMenuItem.IsEnabled = false;
        	
        	magnetsPanel.Children.Add(magnet);
        	
        	// If the magnet is newly created, ensure that it is shown, even if it is in a category which
        	// is currently hidden. If it's been transferred back to the magnet list after being deleted
        	// from a board, then it can retain the same shown/hidden state as its category.
        	bool categoryIsVisible = CategoryIsVisible(magnet.Idea.Category);
        	if (forceShow) {
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
        /// <param name="magnet">The magnet to remove</param>
        public void RemoveMagnet(MagnetControl magnet)
        {
        	magnetsPanel.Children.Remove(magnet);        	
        	RemoveHandlers(magnet);	        	
	    	OnMagnetRemoved(new MagnetRemovedEventArgs(magnet,true));
        }
        
        
        /// <summary>
        /// Delete a magnet from this list
        /// </summary>
        /// <param name="magnet">The magnet to remove</param>
        public void DeleteMagnet(MagnetControl magnet)
        {
        	magnetsPanel.Children.Remove(magnet);        	
        	RemoveHandlers(magnet);	        	
	    	OnMagnetRemoved(new MagnetRemovedEventArgs(magnet,false));
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
        	
        	OnVisibleMagnetsChanged(new EventArgs());
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
        	
        	OnVisibleMagnetsChanged(new EventArgs());
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
    	
        
        public MagnetControl[] GetMagnetsArray() 
        {
    		MagnetControl[] magnets = new MagnetControl[magnetsPanel.Children.Count];
    		foreach (MagnetControl magnet in magnetsPanel.Children) {
    			magnets[magnetsPanel.Children.IndexOf(magnet)] = magnet;
    		}
    		return magnets;
    	}    		
        
        
        public void Scatter()
        {
        	List<MagnetControl> magnets = GetVisibleMagnets();
        	foreach (MagnetControl magnet in magnets) {
        		RemoveMagnet(magnet);
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
    	
    	
    	public void Sort()
    	{
    		MagnetControl[] magnets = GetMagnetsArray();
    		
    		switch (SortBy) {
    			case SortCriterion.Alphabetically:
    				Array.Sort(magnets,MagnetControl.SortAlphabeticallyAscending);
    				magnetsPanel.Children.Clear();
    				foreach (MagnetControl magnet in magnets) {
    					magnetsPanel.Children.Add(magnet);
    				}
    				break;
    			case SortCriterion.Category:
    				break;
    			case SortCriterion.Created:
    				break;
    			case SortCriterion.Stars:
    				break;
    			case SortCriterion.Used:
    				break;    				
    		}
    	}
                
        #endregion
        
        #region Event handlers
        
    	private void magnetControl_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    	{
    		MagnetControl magnet = (MagnetControl)e.Source;
    		if (magnetsPanel.Children.Contains(magnet)) {
    			RemoveMagnet(magnet);
    		}
    	}
    		
        
    	private void magnetControl_Selected(object sender, MagnetEventArgs e)
    	{
    		OnMagnetSelected(e);
    	}
    		
        
        /// <summary>
        /// Treat drop events on magnets as you would drop events on the list itself.
        /// </summary>
        private void magnetControl_Drop(object sender, DragEventArgs e)
        {
        	OnDrop(e);
        }
        
        #endregion
    }
}