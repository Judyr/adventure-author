using System;
using System.IO;
using System.Windows;
using AdventureAuthor.Utils;
using NWN2Toolset.NWN2.Data;
using AdventureAuthor.Evaluation.UI;
using AdventureAuthor.Scripts.UI;
using Microsoft.Win32;

namespace AdventureAuthor.Evaluation.UI
{
	/// <summary>
	/// A window which provides a number of criteria for evaluating a module
	/// and elicits a response from the user as to how well they have been met.
	/// </summary>
    public partial class WorksheetWindow : Window
    {    	
    	#region Fields
    	
    	private string filename;    	
		public string Filename {
			get { return filename; }
			set { filename = value; }
		}
    	
    	/// <summary>
    	/// True if the worksheet fields have been changed since the last save; false otherwise.
    	/// </summary>
    	private bool dirty;
    	
    	#endregion
    	
    	#region Constants
    	
    	private const string XMLFILTER = "XML files (*.xml)|*.xml|All files (*.*)|*.*";
    	
    	#endregion
    	
    	#region Constructors    	   	
    	
    	public WorksheetWindow()
    	{
    		InitializeComponent();
    		dirty = false;
    	}
    	
    	
    	public WorksheetWindow(Worksheet worksheet) : this()
    	{
    		Open(worksheet);
    	}
    	
    	
    	public WorksheetWindow(string filename) : this()
    	{
    		Open(filename);
    	}
    	
    	#endregion
    	
    	#region Methods
    	
    	public void Open(Worksheet worksheet)
    	{
    		Clear();
    		
    		Title = worksheet.Title;
    		DateField.Text = worksheet.Date;
    		NameField.Text = worksheet.Name;
    		    		
    		foreach (Section section in worksheet.Sections) {
    			SectionControl sectionControl = new SectionControl(section);
    			EvaluationSectionsPanel.Children.Add(sectionControl);
    		}
    	}  
    	
    	
    	public void Open(string filename)
    	{
    		if (!File.Exists(filename)) {
    			Say.Error(filename + " could not be found.");
    			return;
    		}
    			
    		try {
	    		object o = AdventureAuthor.Utils.Serialization.Deserialize(filename,typeof(Worksheet));
	    		Worksheet worksheet = (Worksheet)o;
    			Clear();    		
	    		Open(worksheet);
	    		this.filename = filename;
    		}
    		catch (Exception e) {
    			Say.Error("The selected file was not a valid worksheet.",e);
    			Clear();
    			this.filename = null;
    		}
    	}
    	
    	
    	public void Save()
    	{
    		if (filename == null) {
    			throw new InvalidOperationException("Should not have called Save without setting a filename.");
    		}
    		
    		try {
    			AdventureAuthor.Utils.Serialization.Serialize(filename,GetWorksheet());
    		}
    		catch (Exception e) {
    			Say.Error("Failed to save worksheet.",e);
    		}
    	}
    	
    	
    	public void Clear()
    	{
    		filename = null;
    		EvaluationSectionsPanel.Children.Clear();
    	}
    	
    	
    	public Worksheet GetWorksheet()
    	{
    		Worksheet ws = new Worksheet(Title,NameField.Text,DateField.Text);    		
    		foreach (SectionControl sc in EvaluationSectionsPanel.Children) {
    			ws.Sections.Add(sc.GetSection());
    		}
    		return ws;
    	}
    	
    	#endregion
    	
    	#region Event handlers
    	
    	private void OnClick_Open(object sender, EventArgs e)
    	{
			OpenFileDialog openFileDialog = new OpenFileDialog();
			openFileDialog.ValidateNames = true;
    		openFileDialog.DefaultExt = XMLFILTER;
    		openFileDialog.Filter = XMLFILTER;
			openFileDialog.Title = "Select a worksheet file";
			openFileDialog.Multiselect = false;
			//openFileDialog.InitialDirectory = form.App.Module.Repository.DirectoryName;
			openFileDialog.RestoreDirectory = false;			
			openFileDialog.ShowDialog();	
			
			Open(openFileDialog.FileName);
		}
    	
    	
    	private void OnClick_Save(object sender, EventArgs e)
    	{
    		// Get a filename to save to if there isn't one already:
    		if (filename == null) { 
    			SaveFileDialog saveFileDialog = new SaveFileDialog();
    			saveFileDialog.AddExtension = true;
    			saveFileDialog.CheckPathExists = true;
    			saveFileDialog.DefaultExt = XMLFILTER;
    			saveFileDialog.Filter = XMLFILTER;
  				saveFileDialog.ValidateNames = true;
  				bool cancelled = !(bool)saveFileDialog.ShowDialog();  				
  				if (cancelled) {
  					return;
  				}  				
  				filename = saveFileDialog.FileName;
    		}
    		
    		Save();
    	}
    	
    	
    	private void OnClick_Close(object sender, EventArgs e)
    	{
    		// TODO: if changes have been made, ask to save first
    		Clear();
    	}
    	
    	#endregion
    }
}
