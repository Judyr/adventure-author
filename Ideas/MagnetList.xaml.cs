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

namespace AdventureAuthor.Ideas
{
    /// <summary>
    /// Interaction logic for MagnetList.xaml
    /// </summary>

    public partial class MagnetList : UserControl
    {
    	#region Fields
    	    	
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
    	
    	public event EventHandler<MagnetEventArgs> MagnetLeavingList;    	
    	protected virtual void OnMagnetLeavingList(MagnetEventArgs e)
    	{
    		EventHandler<MagnetEventArgs> handler = MagnetLeavingList;
    		if (handler != null) {
    			handler(this,e);
    		}
    	}
    	
    	#endregion
    	
    	#region Constructors
    	
        public MagnetList()
        {
            InitializeComponent();
        }
        
        
        public MagnetList(List<Idea> ideas) : this()
        {        	
        	Add(ideas);
        }
        
        #endregion
        
        #region Methods
        
        /// <summary>
        /// Add an idea to the list.
        /// </summary>
        /// <param name="idea">The idea to add</param>
        public void Add(Idea idea)
        {
        	MagnetControl magnet = new MagnetControl(idea);
        	Add(magnet);
        }
        
        
        /// <summary>
        /// Add a range of ideas to the list.
        /// </summary>
        /// <param name="ideas">The range of ideas to add</param>
        public void Add(List<Idea> ideas)
        {
        	foreach (Idea idea in ideas) {
        		Add(idea);
        	}
        }
        
        
        /// <summary>
        /// Add a magnet representing an idea to the list
        /// </summary>
        /// <param name="magnet">The magnet to add</param>
        public void Add(MagnetControl magnet)
        {
        	magnetsPanel.Children.Add(magnet);
        	magnet.MouseDoubleClick += new MouseButtonEventHandler(MagnetControl_MouseDoubleClick);
        	OnMagnetAdded(new MagnetEventArgs(magnet));
        }
        
        
        /// <summary>
        /// Add a range of magnets representing ideas to the list
        /// </summary>
        /// <param name="magnets">The range of magnets to add</param>
        public void Add(List<MagnetControl> magnets)
        {
        	foreach (MagnetControl magnet in magnets) {
        		Add(magnet);
        	}
        }
        
        
        public void Remove(MagnetControl magnet)
        {
        	magnetsPanel.Children.Remove(magnet); // TODO careful about adding MagnetRemoved event cos
        	// you might be moving it to the main screen rather than deleting it
        }
                
        
        /// <summary>
        /// Show ideas belonging to a particular category.
        /// </summary>
        /// <param name="category">The category of ideas to show</param>
        public void ShowCategory(IdeaCategory category)
        {
        	ShowCategory(category);
        }
        
        
        /// <summary>
        /// Show ideas belonging to a particular category.
        /// </summary>
        /// <param name="category">The category of ideas to show</param>
        /// <param name="only">True to show only the given category, and hide everything else; 
        /// false to show the given category and ignore everything else</param>
        public void ShowCategory(IdeaCategory category, bool only)
        {
        	foreach (MagnetControl magnet in magnetsPanel.Children) {
        		if (magnet.Idea.Category == category) {
        			magnet.Show();
        		}
        		else if (only) {
        			magnet.Hide();
        		}
        	}
        }
                
        
        /// <summary>
        /// Hide ideas belonging to a particular category.
        /// </summary>
        /// <param name="category">The category of ideas to hide</param>
        public void HideCategory(IdeaCategory category)
        {
        	HideCategory(category);
        }
        
                
        /// <summary>
        /// Hide ideas belonging to a particular category.
        /// </summary>
        /// <param name="category">The category of ideas to hide</param>
        /// <param name="only">True to hide only the given category, and show everything else; 
        /// false to hide the given category and ignore everything else</param>
        public void HideCategory(IdeaCategory category, bool only)
        {
        	foreach (MagnetControl magnet in magnetsPanel.Children) {
        		if (magnet.Idea.Category == category) {
        			magnet.Hide();
        		}
        		else if (only) {
        			magnet.Show();
        		}
        	}
        }
                
        #endregion
        
        #region Event handlers
        
    	private void MagnetControl_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    	{
    		MagnetControl magnet = (MagnetControl)e.Source;
    		Remove(magnet);
    		OnMagnetLeavingList(new MagnetEventArgs(magnet));
    	}
    		
        #endregion
    }
}