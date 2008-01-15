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
using AdventureAuthor.Evaluation.UI;

namespace AdventureAuthor.Evaluation.UI
{
    public partial class EvidenceButton : UserControl, IAnswerControl
    {    	
    	#region Events
    	
    	public event EventHandler AnswerChanged;  
    	
		protected virtual void OnAnswerChanged(EventArgs e)
		{
			EventHandler handler = AnswerChanged;
			if (handler != null) {
				handler(this,e);
			}
		}
    	
    	#endregion
		
		
        public EvidenceButton()
        {
            InitializeComponent();
        }

        
        public EvidenceButton(string location) : this()
        {
        	// TODO: check that file exists, and display either a broken link icon or a link icon
        }
        
        
        public EvidenceButton(Evidence evidence) : this(evidence.Value)
        {        	
        }
        
		
		private void OnClick_AddEvidence(object sender, RoutedEventArgs e)
		{
			// TODO: allow user to browse to a file
			
			
        	// TODO: if appropriate call OnAnswerChanged(new EventArgs());
		}
		
		
		public Answer GetAnswer() 
		{
			return new Evidence(String.Empty); // TODO: return a reference to a file
		}
    }
}