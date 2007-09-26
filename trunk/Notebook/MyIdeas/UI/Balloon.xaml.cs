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

namespace AdventureAuthor.Notebook.MyIdeas.UI
{
    /// <summary>
    /// Interaction logic for Balloon.xaml
    /// </summary>

    public partial class Balloon : UserControl
    {		
    	private Idea myIdea;
		public Idea MyIdea {
			get { return myIdea; }
		}
		
		private bool isPopped;
		public bool IsPopped {
			get { return isPopped; }
			set { isPopped = value; } 
		}		
		
    	public Balloon()
        {
            InitializeComponent();	
            this.myIdea = new Idea();
			this.isPopped = false;			
        }

    	public Balloon(Idea idea)
    	{
            InitializeComponent();	
            this.myIdea = idea;
			this.isPopped = false;				
    	}		
    	    	
    	public void Pop()
    	{
    		// play 'pop' animation
    		this.isPopped = true;
    		this.Visibility = Visibility.Hidden;
    	}
    	
    	public void Unpop()
    	{
    		// play 'pop' animation in reverse
    		this.isPopped = false;
    		this.Visibility = Visibility.Visible;
    	}
    	    	
    	public void SetColour(Brush colour)
    	{
    		this.BalloonEllipse.Fill = colour;
    	}
    	
    	public void SetDrift()
		{
			throw new NotImplementedException();
		}
    }
}
