using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using AdventureAuthor.Evaluation.Viewer;
using AdventureAuthor.Utils;
using Microsoft.Win32;

namespace AdventureAuthor.Evaluation.Viewer
{
    public partial class EvidenceControl : OptionalWorksheetPartControl
    {    	
    	#region Constants
    	
    	private readonly string[] PICTURE_EXTENSIONS = new string[] {".jpg",".jpeg",".bmp",".gif"};
    	
    	#endregion
    	
    	#region Fields
    	    	
    	private string filename;    	
		public string Filename {
			get { return filename; }
			set { 			
				filename = value;
				if (filename == null || filename == String.Empty) {
					Status = EvidenceStatus.NoLink;
				}
				else if (File.Exists(filename)) {
					Status = EvidenceStatus.Link;
				}
				else {
					Status = EvidenceStatus.BrokenLink;
				}
				OnChanged(new EventArgs());
			}
		}
    	
    	protected enum EvidenceStatus {
    		NoLink,
    		Link,
    		BrokenLink
    	}
    	
    	private EvidenceStatus status;
    	protected EvidenceStatus Status {
    		get {
    			return status;
    		}
    		set {
    			status = value;
    			switch (status) {
    				case EvidenceStatus.NoLink:   
    					ViewLink.Visibility = Visibility.Collapsed;
    					ViewLink.ToolTip = String.Empty; 					
    					SelectLinkText.Text = "Attach evidence...";
    					ClearLink.Visibility = Visibility.Collapsed;            
			            SelectLink.ToolTip = "Add a piece of evidence which " +
			            					 "supports your other answers.";
    					break;
    				case EvidenceStatus.Link:
    					ViewLink.Visibility = Visibility.Visible;
    					ViewLink.ToolTip = filename;
    					ViewLinkText.Text = ".../" + Path.GetFileName(filename);
    					ViewLinkText.TextDecorations.Clear();
    					ViewLinkText.TextDecorations.Add(TextDecorations.Underline);
    					SelectLinkText.Text = "Change";
    					ClearLink.Visibility = Visibility.Visible;
			            ViewLink.ToolTip = "View supporting evidence\n" +
			            				   "(" + Filename + ")";
			            SelectLink.ToolTip = "Choose a different piece\n" +
			            					 "of supporting evidence.";
    					break;
    				case EvidenceStatus.BrokenLink:
    					ViewLink.Visibility = Visibility.Visible;
    					ViewLink.ToolTip = filename;
    					ViewLinkText.Text = ".../" + Path.GetFileName(filename);
    					ViewLinkText.TextDecorations.Clear();
    					ViewLinkText.TextDecorations.Add(TextDecorations.Strikethrough);
    					SelectLinkText.Text = "Change";
    					ClearLink.Visibility = Visibility.Visible;
			            ViewLink.ToolTip = "View supporting evidence - link broken\n" +
			            				   "(" + Filename + ")";
			            SelectLink.ToolTip = "Choose a different piece\n" +
			            					 "of supporting evidence.";
    					break;
    				default:
    					break;
    			}
    		}
    	}
    	
    	#endregion
    	    			    	
    	#region Constructors
    			
        public EvidenceControl(Evidence evidence)
        {        
    		if (evidence == null) {
        		evidence = new Evidence();
    		}
    		
            InitializeComponent();
            SetInitialActiveStatus(evidence);
            
            if (evidence == null) {
            	Filename = null;
            }
            else {
            	Filename = evidence.Value;
            }
			
            ClearLink.ToolTip = "Remove supporting evidence";
        }
        
        #endregion
        
        #region Event handlers
		
		private void OnClick_SelectEvidence(object sender, RoutedEventArgs e)
		{
			SelectEvidenceDialog();
		}
		
		
		private void SelectEvidenceDialog()
		{
			OpenFileDialog openFileDialog = new OpenFileDialog();
			openFileDialog.ValidateNames = true;
    		openFileDialog.DefaultExt = Filters.WORKSHEET_EVIDENCE;
    		openFileDialog.Filter = Filters.WORKSHEET_EVIDENCE;
			openFileDialog.Title = "Select a file to attach as evidence";
			openFileDialog.Multiselect = false;
			openFileDialog.RestoreDirectory = false;
			
			if (status == EvidenceStatus.Link) {
				try {
					openFileDialog.InitialDirectory = Path.GetDirectoryName(filename);
				}
				catch (Exception e) {
					Say.Error(Path.GetDirectoryName(filename),e);
				}
			}
						
  			bool ok = (bool)openFileDialog.ShowDialog();  				
  			if (ok) {
  				Filename = openFileDialog.FileName;
  			}
		}
        
		
		private void OnClick_ViewEvidence(object sender, RoutedEventArgs e)
		{
			if (status == EvidenceStatus.NoLink) {
				throw new InvalidOperationException("'View evidence' button should not have been visible.");
			}
			
			if (status == EvidenceStatus.Link) {
				try {
					OpenEvidence(filename);
					return; // only get this far if no exception has been raised
				}
				catch (IOException) {
					// Open the evidence if it's there, and return. If trying to open it throws an IOException,
					// it isn't there, so DON'T return - go on to the BrokenLink handling code instead.
					Status = EvidenceStatus.BrokenLink;
				}
			}
			
			MessageBoxResult result = MessageBox.Show("There doesn't appear to be a file at this location:\n" +
    				                                  filename +"\n\n" +
    				                                  "Do you want to browse to locate the file?",
    				                                  "Evidence file missing", 
    				                                  MessageBoxButton.OKCancel);    				
			if (result == MessageBoxResult.OK) {
    			SelectEvidenceDialog();
    		}    		
		}
		
		
		private void OpenEvidence(string filename)
		{
			if (!File.Exists(filename)) {
				throw new IOException(filename + " could not be found.");
			}
			
			if (IsImage(filename)) {
				switch (EvaluationOptions.ApplicationToOpenImages) {
					case EvaluationOptions.ImageApps.Default:
						Process.Start(filename);
						break;
					case EvaluationOptions.ImageApps.MicrosoftPaint:
						try {
							Process.Start("mspaint.exe",filename);
						}
						catch (Exception) {
							Say.Information("Couldn't find Microsoft Paint on this system - opening with " +
							                "default application instead.");
							Process.Start(filename);
						}
						break;
				}				
			}
			else {
				Process.Start(filename);
			}
			
		}
		
		
		private bool IsImage(string filename)
		{			
			FileInfo fileInfo = new FileInfo(filename);
			string ext = fileInfo.Extension.ToLower();
			for (int i = 0; i < PICTURE_EXTENSIONS.Length; i++) {
				if (ext == PICTURE_EXTENSIONS[i]) {
					return true;
				}
			}
			return false;
		}
		
		
		private void OnClick_ClearEvidence(object sender, EventArgs e)
		{
			MessageBoxResult result = MessageBox.Show(Filename+"\n\nRemove this evidence?",
			                                          "Remove evidence?", 
			                                          MessageBoxButton.OKCancel);
			if (result == MessageBoxResult.OK) {
				Filename = String.Empty;				
			}
		}
        
        
        private void OnChecked(object sender, EventArgs e)
        {
        	Activate();
        }
        
        
        private void OnUnchecked(object sender, EventArgs e)
        {
        	Deactivate(false);
        }
		
		#endregion
				
		#region Methods
		
    	protected override void PerformEnable()
    	{    		
    		ActivatableControl.EnableElement(ViewLink);
    		ActivatableControl.EnableElement(SelectLink);
    		ActivatableControl.EnableElement(ClearLink);
    	}
    	
    	    	
    	protected override void PerformActivate()
    	{	
    		ActivatableControl.ActivateElement(ViewLink);
    		ActivatableControl.ActivateElement(SelectLink);
    		ActivatableControl.ActivateElement(ClearLink);
    		ActivatableControl.EnableElement(ActivateCheckBox);
    		if ((bool)!ActivateCheckBox.IsChecked) {
    			ActivateCheckBox.IsChecked = true;
    		}
    		ActivateCheckBox.ToolTip = "Click to deactivate this answer field\n(will not appear in worksheet)";
    	}
    	
        
    	protected override void PerformDeactivate(bool parentIsDeactivated)
    	{
    		ActivatableControl.DeactivateElement(ViewLink);
    		ActivatableControl.DeactivateElement(SelectLink);
    		ActivatableControl.DeactivateElement(ClearLink);
    		if ((bool)ActivateCheckBox.IsChecked) {
    			ActivateCheckBox.IsChecked = false;
    		}
    		if (parentIsDeactivated) {
    			ActivatableControl.DeactivateElement(ActivateCheckBox);
    		}
    		ActivateCheckBox.ToolTip = "Click to activate this answer field\n(will not appear in worksheet)";
    	}
    	
		public override void ShowActivationControls()
		{
			ActivateCheckBox.Visibility = Visibility.Visible;
		}
		
		public override void HideActivationControls()
		{
			ActivateCheckBox.Visibility = Visibility.Collapsed;
		}
    	
        
        protected override OptionalWorksheetPart GetWorksheetPartObject()
		{
			return new Evidence(filename);
		}
		
		#endregion
    }
}