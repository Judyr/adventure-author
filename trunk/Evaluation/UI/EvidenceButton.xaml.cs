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
using AdventureAuthor.Scripts.UI;

namespace AdventureAuthor.Evaluation.UI
{
    public partial class EvidenceButton : UserControl, IQuestionPanel
    {
        public EvidenceButton()
        {
            InitializeComponent();
        }

		
		private void OnClick_AddEvidence(object sender, RoutedEventArgs e)
		{
			// TODO: allow user to browse to a file
		}
		
		
		public object Answer {
			get {
				return null; // TODO: return a reference to a file
			}
		}
    }
}