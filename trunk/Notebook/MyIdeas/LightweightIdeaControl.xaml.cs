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
using System.Timers;
using AdventureAuthor.Utils;
using AdventureAuthor.Notebook.MyIdeas;

namespace AdventureAuthor.Notebook.MyIdeas
{
    /// <summary>
    /// Interaction logic for LightweightIdeaControl.xaml
    /// </summary>

    public partial class LightweightIdeaControl : UserControl
    {
    	private List<IdeaFragment> ideas = new List<IdeaFragment>();
    	
    	
        public LightweightIdeaControl()
        {        	
            InitializeComponent();
            
//            Timer timer = new Timer(4000);
//            timer.Elapsed += delegate {  };
//            
//        	timer.Elapsed += delegate	
//        	{  
//        		Log.WriteMessage("Tick");
//        		opacity = Math.Max(opacity - 0.1,0.0);
//        		Log.WriteMessage("Opacity is now " + opacity);
//        		if (opacity <= 0.0) {
//        			Log.WriteMessage("Opacity reached 0.0 - stopping timer");
//        			timer.Stop();
//        			IdeaSavedLabel.Opacity = 0.0;
//        			opacity = 2.0;
//        		}
//        		else if (opacity <= 1.0) {
//        			lock (padlock) {
//        				Log.WriteMessage("Opacity less than 1.0. About to set real opacity: " + IdeaSavedLabel.Opacity + " to " +
//        				                 " calculated opacity: " + opacity);
//        				IdeaSavedLabel.Opacity = opacity;
//        				Log.WriteMessage("Done.");
//        			}
//        		}
//        	};
        }
        
        
        private void OnClick_RecordIdea(object sender, EventArgs e)
        {
        	if (IdeaTextBox.Text != String.Empty) {
        		IdeaFragment idea = new IdeaFragment(IdeaTextBox.Text);
        		ideas.Add(idea);
        		IdeaTextBox.Clear();
        		IdeaTextBox.Focus();
        	}
        }
        
        
        private void OnClick_ClearIdea(object sender, EventArgs e)
        {
        	IdeaTextBox.Clear();
        }
        
        
        private void OnClick_OpenIdeaScrapbook(object sender, EventArgs e)
        {
        	StringBuilder s = new StringBuilder("Ideas:\n");
        	foreach (IdeaFragment idea in ideas) {
        		s.Append("\n" + idea.Text);
        	}
        	Say.Information(s.ToString());
        }
    }
}