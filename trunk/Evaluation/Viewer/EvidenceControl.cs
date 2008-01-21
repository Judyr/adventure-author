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
    public partial class EvidenceControl : UserControl, IAnswerControl
    {    	
    	#region Constants
    	
    	private const string EVIDENCE_FILTER = "Pictures (*.jpg;*.jpeg;*.bmp;*.gif)|*.jpg;*.jpeg;*.bmp;*.gif" + "|" +
    										   "All files (*.*)|*.*";
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
				OnAnswerChanged(new EventArgs());
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
    					break;
    				case EvidenceStatus.Link:
    					ViewLink.Visibility = Visibility.Visible;
    					ViewLink.ToolTip = filename;
    					ViewLinkText.Text = ".../" + Path.GetFileName(filename);
    					ViewLinkText.TextDecorations.Clear();
    					ViewLinkText.TextDecorations.Add(TextDecorations.Underline);
    					SelectLinkText.Text = "Change";
    					ClearLink.Visibility = Visibility.Visible;
    					break;
    				case EvidenceStatus.BrokenLink:
    					ViewLink.Visibility = Visibility.Visible;
    					ViewLink.ToolTip = filename;
    					ViewLinkText.Text = ".../" + Path.GetFileName(filename);
    					ViewLinkText.TextDecorations.Clear();
    					ViewLinkText.TextDecorations.Add(TextDecorations.Strikethrough);
    					SelectLinkText.Text = "Change";
    					ClearLink.Visibility = Visibility.Visible;
    					break;
    				default:
    					break;
    			}
    		}
    	}
    	
    	#endregion
    	    	
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
		    	
    	#region Constructors
		
        public EvidenceControl()
        {
            InitializeComponent();
            Filename = null;
        }

        
        public EvidenceControl(string location)
        {
            InitializeComponent();
        	Filename = location;
        }
        
        
        public EvidenceControl(Evidence evidence)
        {        
            InitializeComponent();
        	Filename = evidence.Value;
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
    		openFileDialog.DefaultExt = EVIDENCE_FILTER;
    		openFileDialog.Filter = EVIDENCE_FILTER;
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
    		switch (status) {
    			case EvidenceStatus.NoLink:
					Say.Error("'View evidence' button should not have been visible.");
					break;
    			case EvidenceStatus.Link:
					OpenEvidence(filename);
    				break;
    			case EvidenceStatus.BrokenLink:
    				MessageBoxResult result = MessageBox.Show("There doesn't appear to be a file at this location:\n" +
    				                                          filename +"\n\n" +
    				                                          "Do you want to browse to locate the file?",
    				                                          "Evidence file missing", 
    				                                          MessageBoxButton.OKCancel);
    				if (result == MessageBoxResult.OK) {
    					SelectEvidenceDialog();
    				}    				                                          
    				break;
    			default:
    				break;
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
		
		#endregion
				
		#region Methods
		
		public Answer GetAnswer() 
		{
			return new Evidence(filename);
		}
		
		#endregion
    }
}