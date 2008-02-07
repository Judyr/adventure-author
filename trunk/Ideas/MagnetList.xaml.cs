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
    	
    	private List<Idea> ideas = null;
    	
    	#endregion
    	
    	
        public MagnetList()
        {
            InitializeComponent();
        }
        
        
        public MagnetList(List<Idea> ideas) : this()
        {        	
        	Populate(ideas);
        }
        
        
        public void Populate(List<Idea> ideas)
        {
        	this.ideas = ideas;
        	foreach (Idea idea in ideas) {
        		// create an idea control representing this idea
        		// add it
        	}
        }
        
        
        public void Remove(Idea idea)
        {
        	
        }
        
        
        public void Remove(List<Idea> ideasToRemove)
        {
        	List<Idea> removing = new List<Idea>(ideasToRemove.Count);
        	foreach (Idea ideaToRemove in ideasToRemove) {
        		foreach (Idea idea in this.ideas) {
        			if (idea == ideaToRemove) {
        				removing.Add(idea);
        				continue;
        			}
        		}
        	}
        	foreach (Idea removable in removing) {
        		ideas.Remove(removable);
        	}
        }
        
        
        public void Show(string category)
        {
        	
        }
        
        
        public void Hide(string category)
        {
        	
        }
        
        
        public void Add(Idea idea)
        {
        	
        }
        
        
        public void Add(List<Idea> ideas)
        {
        	foreach (Idea idea in ideas) {
        		Add(idea);
        	}
        }
    }
}