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

namespace AdventureAuthor.Ideas
{
    /// <summary>
    /// Interaction logic for MagnetBoard.xaml
    /// </summary>

    public partial class MagnetBoard : Board
    {
    	#region Fields
    	
    	private List<BoardObject> boardObjects = new List<BoardObject>();
		protected override List<BoardObject> BoardObjects {
			get {
    			return boardObjects;    			
			}
		}
            	
    	#endregion
    	
    	#region Constructors
    	
    	public MagnetBoard()
    	{
    		InitializeComponent();
    	}
        
        #endregion
        
        #region Methods
        
        public void Add(BoardObject boardObject)
        {
        	if (boardObject == null) {
        		throw new ArgumentNullException("Tried to add a null object to the board.");
        	}
        	
//        	if (Canvas.GetLeft(boardObject) > MagneticCanvas.Width) {
//        		Canvas.SetLeft(boardObject,MagneticCanvas.Width);
//        	}
//        	if (Canvas.GetTop(boardObject) > MagneticCanvas.Height) {
//        		Canvas.SetTop(boardObject,MagneticCanvas.Height);
//        	}
        	MagneticCanvas.Children.Add(boardObject);
        }
        
//        public void AddNote(Note note)
//        {
//        	
//        }
        
        #endregion
        

    	
    }
}