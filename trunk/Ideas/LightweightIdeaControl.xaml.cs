using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Windows.Controls;
using System.Windows.Input;
using System.Xml.Serialization;
using form = NWN2Toolset.NWN2ToolsetMainForm;
using AdventureAuthor.Utils;
using AdventureAuthor.Core;

namespace AdventureAuthor.Ideas
{
    /// <summary>
    /// Interaction logic for LightweightIdeaControl.xaml
    /// </summary>

    public partial class LightweightIdeaControl : UserControl
    {
    	[XmlElement]
    	private List<Idea> ideas;
    	
    	
        public LightweightIdeaControl()
        {       
        	// TODO obviously doesn't work cos there's no module open when this loads
        	try {
	        	object o = Serialization.Deserialize(Path.Combine(form.App.Module.Repository.DirectoryName,"ideas.xml"),
	        	                                     typeof(List<Idea>));
	        	ideas = (List<Idea>)o;
        	}
        	catch (Exception) {
        		ideas = new List<Idea>();
        	}
            InitializeComponent();
        }
        
        
        private void OnClick_RecordIdea(object sender, EventArgs e)
        {
        	RecordIdea();
        }
        
        
        private void OnClick_ClearIdea(object sender, EventArgs e)
        {
        	IdeaTextBox.Clear();
        }
        
        
        private void OnClick_OpenIdeaScrapbook(object sender, EventArgs e)
        {
        	
        	
        	
        	StringBuilder s = new StringBuilder("Ideas:\n");
        	foreach (Idea idea in ideas) {
        		s.Append("\n" + idea.Text);
        	}
        	Say.Information(s.ToString());
        }
        
        
        private void OnKeyDown_IdeaTextBox(object sender, KeyEventArgs e)
        {
        	if (e.Key == Key.Return) {
        		RecordIdea();
        	}
        }
        
        
        private void RecordIdea()
        {
        	if (ModuleHelper.ModuleIsOpen() && IdeaTextBox.Text != String.Empty) {
        		Idea idea = new Idea(IdeaTextBox.Text);
        		ideas.Add(idea);
        		IdeaTextBox.Clear();
        		IdeaTextBox.Focus();
        		
        		// TODO: Open Scrapbook should flash to indicate an idea has 'gone in'
        		
        		// Save idea:
        		
        		Serialization.Serialize(Path.Combine(form.App.Module.Repository.DirectoryName,"ideas.xml"),ideas);
        	}
        }
    }
}