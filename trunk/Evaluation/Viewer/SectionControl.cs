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
using AdventureAuthor.Evaluation;
using AdventureAuthor.Evaluation.Viewer;
using AdventureAuthor.Utils;

namespace AdventureAuthor.Evaluation.Viewer
{
    public partial class SectionControl : OptionalWorksheetPartControl
    {        
    	#region Events
    	
    	public event EventHandler<DeletingEventArgs> Deleting;  
    	
		protected virtual void OnDeleting(DeletingEventArgs e)
		{
			EventHandler<DeletingEventArgs> handler = Deleting;
			if (handler != null) {
				handler(this,e);
			}
		}
    	
    	#endregion
    	
    	
    	public string SectionTitle {
    		get {    			
	   			return SectionTitleTextBlock.Text;
    		}
    		set {
    			SectionTitleTextBlock.Text = value;
    		}
    	}
    	
        
        public SectionControl(Section section, bool designerMode) 	
        {
        	if (section == null) {
        		section = new Section();
        	}
        	
            InitializeComponent();            
            SectionTitle = section.Title;   
            
        	foreach (Question question in section.Questions) {
        		if (WorksheetViewer.DesignerMode || question.Include) {	
		        	AddQuestionField(question);
        		}
        	}
                   
            if (designerMode) { // show 'Active?' control, and assume that control is Active to begin with
            	ActivateCheckBox.Visibility = Visibility.Visible;
	    		if (section.Include) {
	    			ActivationStatus = ControlStatus.Active;
	    		}
	    		else {
	    			ActivationStatus = ControlStatus.Inactive;
	    		}
            }
            else { // hide 'Active?' control, and set status to Not Applicable
        		ActivationStatus = ControlStatus.NA;
            }
        }         
        
        
        private void AddQuestionField(QuestionControl control)
        {
        	if (control == null) {
        		throw new ArgumentNullException("Can't add a null question field.");
        	}   
        	
        	if (QuestionsPanel.Children.Count % 2 == 0) {
        		control.Background = (Brush)Resources["Stripe1Brush"];
        	}
        	else {
        		control.Background = (Brush)Resources["Stripe2Brush"];
        	}         	
        	QuestionsPanel.Children.Add(control);
        }
			        
			
        public void AddQuestionField(Question question)
        {
        	if (question == null) {
        		throw new ArgumentNullException("Can't add a null question field.");
        	}
        	
        	QuestionControl control = (QuestionControl)question.GetControl(WorksheetViewer.DesignerMode);
        	AddQuestionField(control);
        }
        
        
        private void OnClick_DeleteSection(object sender, EventArgs e)
        {
        	if (!WorksheetViewer.DesignerMode) {
        		throw new InvalidOperationException("Should not have been possible to try to delete a section.");
        	}
        	
        	MessageBoxResult result = MessageBox.Show("This section contains " + QuestionsPanel.Children.Count +
        	                                          " questions - are you sure you want to permanently delete it?",
        	                                          "Delete section?",
        	                                          MessageBoxButton.OKCancel,
        	                                          MessageBoxImage.Warning,
        	                                          MessageBoxResult.Cancel,
        	                                          MessageBoxOptions.None);
        	if (result == MessageBoxResult.OK) {
        		OnDeleting(new DeletingEventArgs(this));
        	}
        }        
        
        
        private void OnChecked(object sender, EventArgs e)
        {
        	ActivationStatus = ControlStatus.Active;
        }
        
        
        private void OnUnchecked(object sender, EventArgs e)
        {
        	ActivationStatus = ControlStatus.Inactive;
        }

        
    	protected override void Enable()
    	{    		
    		ActivatableControl.Enable(this);
    	}
    	
    	
    	protected override void Activate()
    	{	
    		ActivatableControl.Activate(SectionTitleTextBlock);
    		ActivatableControl.Activate(QuestionsPanel);
    		ActivatableControl.Enable(ActivateCheckBox);
    		if ((bool)!ActivateCheckBox.IsChecked) {
    			ActivateCheckBox.IsChecked = true;
    		}
    	}
    	
        
    	protected override void Deactivate()
    	{
    		ActivatableControl.Deactivate(SectionTitleTextBlock);
    		ActivatableControl.Deactivate(QuestionsPanel);
    		ActivatableControl.Enable(ActivateCheckBox);
    		if ((bool)ActivateCheckBox.IsChecked) {
    			ActivateCheckBox.IsChecked = false;
    		}
    	}
        
        
        protected override OptionalWorksheetPart GetWorksheetPartObject()
        {
        	Section section = new Section(SectionTitle);   			
   			foreach (UIElement element in QuestionsPanel.Children) {
   				QuestionControl qc = element as QuestionControl;
   				if (qc != null) {
   					Question question = (Question)qc.GetWorksheetPart();
   					section.Questions.Add(question);
   				}
   			}   			
   			return section;
        }
        
        
        protected override List<Control> GetActivationControls()
        {
        	List<Control> activationControls = new List<Control>(1);
        	activationControls.Add(ActivateCheckBox);
        	return activationControls;
        }
    }
}