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
using System.IO;
using AdventureAuthor.Utils;
using AdventureAuthor.Core;

namespace AdventureAuthor.Ideas
{
    /// <summary>
    /// Interaction logic for MagnetList.xaml
    /// </summary>

    public partial class MagnetList : UnserializableControl
    {
    	#region Constants
    	
    	/// <summary>
    	/// The maximum number of degrees a magnet may deviate from an angle of 0 (in either direction.)
    	/// </summary>    	
    	public const double MAXIMUM_ANGLE_IN_EITHER_DIRECTION = 6;
    	
    	#endregion
    	
    	#region Fields  
    	
    	private string filename;    	
		public string Filename {
			get { return filename; }
			set { 
				filename = value;
			}
		}     	
    	
    	
    	/// <summary>
    	/// The default location to check for a magnet list to open.
    	/// </summary>
    	private static string defaultFilename = System.IO.Path.Combine(ModuleHelper.AdventureAuthorDir,"ideas.xml");
		public static string DefaultFilename {
			get { return defaultFilename; }
			set { defaultFilename = value; }
		}
    	
    	
    	private List<IdeaCategory> visibleCategories;
    	
    	
    	private SortCriterion sortBy = SortCriterion.Alphabetically;
		public SortCriterion SortBy {
			get { return sortBy; }
			set { 
				sortBy = value;
			}
		}
    	
    	
    	/// <summary>
    	/// True to save automatically whenever a magnet is added or deleted.
    	/// </summary>
    	/// <remarks>Should only set this to true if a filename has been provided,
    	/// otherwise a crash will occur</remarks>
    	private bool saveAutomatically = false;    	
		public bool SaveAutomatically {
			get { return saveAutomatically; }
			set { 
				if (value == true && (Filename == null || Filename == String.Empty)) {
					throw new InvalidOperationException("Cannot set this list to save changes automatically " +
					                                    "without first providing a filename.");
				}
				saveAutomatically = value; 
			}
		}
    	
    	
    	/// <summary>
    	/// True to place magnets on the list imperfectly; false to place them perfectly straight.
    	/// </summary>
    	private bool useWonkyMagnets = false;    	
		public bool UseWonkyMagnets {
			get { return useWonkyMagnets; }
			set { 
				if (useWonkyMagnets != value) {
					useWonkyMagnets = value; 
					if (useWonkyMagnets) {
						AngleMagnets(MAXIMUM_ANGLE_IN_EITHER_DIRECTION);
					}
					else {
						StraightenMagnets();
					}
				}
			}
		}
        
        
        private MouseButtonEventHandler magnetControl_MouseDoubleClickHandler;        
        private EventHandler<MagnetEventArgs> magnetControl_SelectedHandler;        
        private DragEventHandler magnetControl_DropHandler;
        private EventHandler<MagnetEventArgs> magnetListChangedHandler;
    	
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
    	
    	
    	public event EventHandler<MagnetEventArgs> MagnetDeleted;    	
    	protected virtual void OnMagnetDeleted(MagnetEventArgs e)
    	{
    		EventHandler<MagnetEventArgs> handler = MagnetDeleted;
    		if (handler != null) {
    			handler(this,e);
    		}
    	}
    	
    	
    	public event EventHandler<MagnetEventArgs> MagnetTransferredOut;    	
    	protected virtual void OnMagnetTransferredOut(MagnetEventArgs e)
    	{
    		EventHandler<MagnetEventArgs> handler = MagnetTransferredOut;
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
		
    	
    	public event EventHandler Scattered;   	
		protected virtual void OnScattered(EventArgs e)
		{
			EventHandler handler = Scattered;
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
        	magnetListChangedHandler = new EventHandler<MagnetEventArgs>(MagnetListChanged_SaveAutomatically);
        		
            InitializeComponent();
            
            visibleCategories = new List<IdeaCategory>(Idea.IDEA_CATEGORIES.Length);
            foreach (IdeaCategory category in Idea.IDEA_CATEGORIES) {
            	visibleCategories.Add(category);
            }
            
            // All changes in the magnet list should be serialized automatically:
            MagnetAdded += magnetListChangedHandler;
            MagnetDeleted += magnetListChangedHandler;
        }
        
        
        public MagnetList(MagnetListInfo magnetListInfo) : this()
        {        	
        	Open(magnetListInfo);
        }
        
        #endregion
        
        #region Methods

        /// <summary>
        /// Open a magnet list.
        /// </summary>
        /// <param name="filename">The filename of the magnet list serialized data</param>
    	public void Open(string filename)
    	{
    		if (!File.Exists(filename)) {
    			Say.Error(filename + " could not be found.");
    			return;
    		}
    			
    		try {
	    		object o = AdventureAuthor.Utils.Serialization.Deserialize(filename,typeof(MagnetListInfo));
	    		MagnetListInfo magnetListInfo = (MagnetListInfo)o;
	    		Open(magnetListInfo);
	    		Filename = filename;
    		}
    		catch (InvalidCastException ec) {
    			Say.Error("The file you tried to open was not a valid magnet list file: " + filename,ec);
    			Clear();
    			this.Filename = null;
    		}
    		catch (Exception e) {
    			Say.Error("Was unable to open this magnet list.",e);
    			Clear();
    			this.Filename = null;
    		}
    	}
    	
    	
    	/// <summary>
    	/// Open a magnet list.
    	/// </summary>
    	/// <param name="magnetListInfo">Serialized data to represent</param>
    	/// <remarks>Must either set a filename on this magnet list or set SaveAutomatically to false</remarks>
    	internal void Open(MagnetListInfo magnetListInfo)
    	{
	        Clear();
	    	foreach (MagnetInfo magnetInfo in magnetListInfo.Magnets) {
	    		MagnetControl magnet = (MagnetControl)magnetInfo.GetControl();
	    		ShowAllCategories(); // set all categories to be shown before adding magnets (faster)
	    		AddMagnet(magnet,false);
	    	}   
    	}
        
        
        /// <summary>
        /// Clear the magnet list.
        /// </summary>
        /// <remarks>This is intended for UI functions, rather than to delete all magnets in a magnet list,
        /// so no events are fired</remarks>
        public void Clear()
        {
        	magnetsPanel.Children.Clear();
        }
        
        
    	public void Save()
    	{
    		if (filename == null) {
    			throw new InvalidOperationException("Save failed: Should not have called Save without first setting a filename.");
    		}
    		else {
	    		AdventureAuthor.Utils.Serialization.Serialize(Filename,this.GetSerializable());
    		}
    	} 
        
        
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
        
        
        /// <summary>
        /// Add a magnet representing an idea to the list
        /// </summary>
        /// <param name="magnet">The magnet to add</param>
        /// <param name="forceShow">True to force this magnet's category to be shown; false otherwise</param>
        /// <remarks>All Add methods ultimately call this method</remarks>
        public void AddMagnet(MagnetControl magnet, bool forceShow)
        {
        	AddHandlers(magnet);
        	
        	magnet.Margin = new Thickness(5);
        	magnet.bringToFrontMenuItem.IsEnabled = false;
        	magnet.sendToBackMenuItem.IsEnabled = false;
        	magnet.removeDeleteMagnetMenuItem.Header = "Delete"; // more permanent than removing from a magnet board
        	
        	magnetsPanel.Children.Add(magnet);
        	
        	// Angle the magnet consistently with current policy:
        	if (useWonkyMagnets) {
	        	bool angleToLeft = magnetsPanel.Children.IndexOf(magnet) % 2 == 0;
	        	magnet.RandomiseAngle(MAXIMUM_ANGLE_IN_EITHER_DIRECTION,angleToLeft);
        	}
        	else {
        		magnet.Angle = 0;
        	}
        	
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
        /// Add magnet list event handlers when a magnet is added
        /// </summary>
        /// <param name="magnet">The magnet to add handlers for</param>
        private void AddHandlers(MagnetControl magnet)
        {
        	try {
        		magnet.MouseDoubleClick += magnetControl_MouseDoubleClickHandler;
        		magnet.Selected += magnetControl_SelectedHandler;
        		magnet.Drop += magnetControl_DropHandler;
        		magnet.Edited += magnetListChangedHandler;
        	}
        	catch (Exception e) {
        		Say.Error("Failed to add handlers to magnet.",e);
        	}
        }
        
        
        /// <summary>
        /// Remove magnet list event handlers when a magnet is removed (as it may be going elsewhere)
        /// </summary>
        /// <param name="magnet">The magnent to remove handlers from</param>
        private void RemoveHandlers(MagnetControl magnet)
        {
        	try {
        		magnet.MouseDoubleClick -= magnetControl_MouseDoubleClickHandler;
        		magnet.Selected -= magnetControl_SelectedHandler;
        		magnet.Drop -= magnetControl_DropHandler;
        		magnet.Edited -= magnetListChangedHandler;
        	}
        	catch (Exception e) {
        		Say.Error("Failed to remove handlers from magnet.",e);
        	}
        }
        
        
        /// <summary>
        /// Indicate that a magnet should be transferred to the magnet board
        /// </summary>
        /// <param name="magnet">The magnet to be transferred</param>
        public void TransferMagnet(MagnetControl magnet)
        {       	        	
	    	OnMagnetTransferredOut(new MagnetEventArgs(magnet));
        }
                
        
        /// <summary>
        /// Delete a magnet from this list
        /// </summary>
        /// <param name="magnet">The magnet to remove</param>
        public void DeleteMagnet(MagnetControl magnet)
        {
        	magnetsPanel.Children.Remove(magnet);        	
        	RemoveHandlers(magnet);	        	
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
        	
        	OnVisibleMagnetsChanged(new EventArgs());
        }
                
        
        /// <summary>
        /// Show all idea categories.
        /// </summary>
        public void ShowAllCategories()
        {
        	visibleCategories.Clear();
        	foreach (IdeaCategory category in Idea.IDEA_CATEGORIES) {
        		visibleCategories.Add(category);
        	}
        	foreach (MagnetControl magnet in magnetsPanel.Children) {
        		magnet.Show();
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
    	        
        
        public List<MagnetControl> GetMagnets(bool visibleOnly)
        {
        	List<MagnetControl> magnets = new List<MagnetControl>(magnetsPanel.Children.Count);
        	foreach (MagnetControl magnet in magnetsPanel.Children) {
        		if (!visibleOnly || magnet.IsVisible) {
        			magnets.Add(magnet);
        		}
        	}
        	return magnets;
        }     	
                
        
        /// <summary>
        /// Set the angle of each magnet in the list to 0. 
        /// </summary>
        private void StraightenMagnets()
        {
        	foreach (MagnetControl magnet in magnetsPanel.Children) {
        		magnet.Angle = 0;
        	}
        }  
        
        
        private void AngleMagnets(double maxAngle)
        {
        	if (maxAngle == 0) {
        		StraightenMagnets();
        		return;
        	}
        	
        	foreach (MagnetControl magnet in magnetsPanel.Children) {
        		bool angleToLeft = magnetsPanel.Children.IndexOf(magnet) % 2 == 0;
        		magnet.RandomiseAngle(maxAngle,angleToLeft);
        	}
        }
            	
        
        public MagnetControl[] GetMagnets() 
        {
    		MagnetControl[] magnets = new MagnetControl[magnetsPanel.Children.Count];
    		foreach (MagnetControl magnet in magnetsPanel.Children) {
    			magnets[magnetsPanel.Children.IndexOf(magnet)] = magnet;
    		}
    		return magnets;
    	}    		
        
        
        public void Scatter()
        {
        	OnScattered(new EventArgs());
        }
    	
    	    	
        /// <summary>
    	/// Check whether a given magnet exists in this list.
    	/// </summary>
    	/// <param name="magnet">The magnet to check for</param>
    	/// <returns>True if the magnet object exists within this list; false otherwise</returns>
    	/// <remarks>This checks for the actual magnet object - to check for an equivalent magnet
    	/// (i.e. a magnet with identical field values) call HasEquivalentMagnet instead</remarks>
    	public bool HasMagnet(MagnetControl magnet)
    	{
    		return MagnetList.HasMagnet(magnetsPanel.Children,magnet);
    	}
    	
    	        
    	/// <summary>
    	/// Check whether an equivalent magnet to the given magnet exists in this list.
    	/// </summary>
    	/// <param name="magnet">The magnet to check for</param>
    	/// <returns>True if a magnet with identical field values to the given magnet exists
    	/// within this list; false otherwise</returns>
    	/// <remarks>This checks for an equivalent magnet (i.e. a magnet with identical field values) - 
    	/// to check whether the collection contains the actual object, call HasMagnet instead</remarks>
    	public bool HasEquivalentMagnet(MagnetControl magnet)
    	{
    		return MagnetList.HasEquivalentMagnet(magnetsPanel.Children,magnet);
    	}
    	
    	
    	/// <summary>
    	/// Check whether a given magnet exists within a collection of UI elements.
    	/// </summary>
    	/// <param name="elements">The UI element collection to check</param>
    	/// <param name="magnet">The magnet to check for</param>
    	/// <returns>True if the magnet object exists within this collection; false otherwise</returns>
    	/// <remarks>This checks for the actual magnet object - to check for an equivalent magnet
    	/// (i.e. a magnet with identical field values) call HasEquivalentMagnet instead</remarks>
    	public static bool HasMagnet(UIElementCollection elements, MagnetControl magnet)
    	{
    		return elements.Contains(magnet);
    	}
    	
    	    	
    	/// <summary>
    	/// Check whether an equivalent magnet to the given magnet exists within a collection of UI elements.
    	/// </summary>
    	/// <param name="elements">The UI element collection to check</param>
    	/// <param name="magnet">The magnet to check for</param>
    	/// <returns>True if a magnet with identical field values to the given magnet exists
    	/// within this collection; false otherwise</returns>
    	/// <remarks>This checks for an equivalent magnet (i.e. a magnet with identical field values) - 
    	/// to check whether the collection contains the actual object, call HasMagnet instead</remarks>
    	public static bool HasEquivalentMagnet(UIElementCollection elements, MagnetControl magnet)
    	{
    		if (HasMagnet(elements,magnet)) { // return true if the actual object is found
    			return true;
    		}
    		foreach (UIElement element in elements) { // return true if an equivalent object is found
    			if (element is MagnetControl) {
    				MagnetControl magnet2 = (MagnetControl)element;
    				if (magnet.Idea.Equals(magnet2.Idea)) {
    					return true;
    				}
    			}
    		}
    		return false;
    	}
    	
    	
    	/// <summary>
    	/// Check whether a given category of idea is currently being displayed.
    	/// </summary>
    	/// <param name="category">The category to check for visibility</param>
    	/// <returns>True if the category is currently set to be displayed; false otherwise</returns>
    	public bool CategoryIsVisible(IdeaCategory category)
    	{
    		return visibleCategories.Contains(category);
    	}
    	
    	
    	public void Sort()
    	{
    		MagnetControl[] magnets = GetMagnets();
    		
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
    	
		
    	public override ISerializableData GetSerializable()
    	{
    		return new MagnetListInfo(this);
    	}	
                
        #endregion
        
        #region Event handlers
        
    	private void magnetControl_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    	{
    		MagnetControl magnet = (MagnetControl)e.Source;
    		if (magnetsPanel.Children.Contains(magnet)) {
    			TransferMagnet(magnet);
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
        
        
        private void MagnetListChanged_SaveAutomatically(object sender, MagnetEventArgs e)
        {
        	if (saveAutomatically) {
        		Save();
        	}
        }
        
        #endregion
    }
}