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

namespace AdventureAuthor.Evaluation.Viewer
{
	/// <summary>
	/// A control that contains an answer control, and activates or 
	/// deactivates it according to user preference.
	/// </summary>
    public partial class ActivatedAnswerControl : IAnswerControl
    {
    	private IAnswerControl answerControl;
    	
    	
    	private bool isActive;    	
		public bool IsActive {
			get { return isActive; }
			set { 
				if (((bool)ActivateCheckBox.IsChecked) != value) {
					ActivateCheckBox.IsChecked = value;
				}
				isActive = value; 
				
				if (isActive) {
					((UIElement)answerControl).Opacity = 1.0f;
				}
				else {
					((UIElement)answerControl).Opacity = 0.2f;
				}
			}
		}
    	
    	
        public ActivatedAnswerControl(IAnswerControl answerControl, bool active)
        {
        	if (!(answerControl is UIElement)) {
        		throw new ArgumentException("IAnswerControl object must be a UIElement.");
        	}
        	UIElement answerControlElement = (UIElement)answerControl;
        	answerControlElement.IsEnabled = false;
            InitializeComponent();
            Grid.SetRow(answerControlElement,0);
            Grid.SetColumn(answerControlElement,0);
            MainGrid.Children.Add(answerControlElement);
            this.answerControl = answerControl;
            answerControl.AnswerChanged += delegate { OnAnswerChanged(new EventArgs()); };            
            IsActive = active;
        }
        
        
        private void OnChecked(object sender, EventArgs e)
        {
        	IsActive = true;
        	OnAnswerChanged(new EventArgs());
        }
        
        
        private void OnUnchecked(object sender, EventArgs e)
        {
        	IsActive = false;
        	OnAnswerChanged(new EventArgs());
        }
    	
        
		public event EventHandler AnswerChanged;     	
		
		protected virtual void OnAnswerChanged(EventArgs e)
		{
			EventHandler handler = AnswerChanged;
			if (handler != null) {
				handler(this,e);
			}
		}
    	
		
		public Answer GetAnswer()
		{
			Answer answer = answerControl.GetAnswer();
			answer.Include = IsActive;
			return answer;
		}
    }
}