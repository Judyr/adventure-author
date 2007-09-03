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
using AdventureAuthor.UI.Controls;
using AdventureAuthor.Utils;

namespace AdventureAuthor.UI.Windows
{
    /// <summary>
    /// Interaction logic for GenericParameterWindow.xaml
    /// </summary>
    public partial class ScriptParametersWindow : Window
    {    	
    	private object[] parameters;
    	
    	public ScriptParametersWindow(ref object[] parameters)
        {
    		this.parameters = parameters;
    		
            InitializeComponent();
            
            StackPanel OKCancelPanel = new StackPanel();
            Button OKButton = new Button();
            OKButton.Content = "OK";
            OKButton.Click += new RoutedEventHandler(OnClick_OK);
            Button CancelButton = new Button();
            CancelButton.Content = "Cancel";
            CancelButton.Click += new RoutedEventHandler(OnClick_Cancel);
            OKCancelPanel.Children.Add(OKButton);
            OKCancelPanel.Children.Add(CancelButton);            
            MainPanel.Children.Add(OKCancelPanel);
        }        

        public void AddBooleanQuestion(string question)
        {
        	BooleanQuestionPanel panel = new BooleanQuestionPanel(question);
        	MainPanel.Children.Insert(MainPanel.Children.Count-1,panel);
        }
        
        public void AddIntegerQuestion(string question)
        {
        	IntegerQuestionPanel panel = new IntegerQuestionPanel(question);
        	MainPanel.Children.Insert(MainPanel.Children.Count-1,panel);
        }
        
        public void AddIntegerQuestion(string question, int? min, int? max)
        {
        	IntegerQuestionPanel panel = new IntegerQuestionPanel(question,min,max);
        	MainPanel.Children.Insert(MainPanel.Children.Count-1,panel);
        }
        
        public void AddStringQuestion(string question)
        {
        	StringQuestionPanel panel = new StringQuestionPanel(question);
        	MainPanel.Children.Insert(MainPanel.Children.Count-1,panel);
        }
        
        public void AddTagQuestion(string question, TagHelper.TagType[] tagTypes)
        {
        	TagQuestionPanel panel = new TagQuestionPanel(question,tagTypes);
        	MainPanel.Children.Insert(MainPanel.Children.Count-1,panel);
        }
        
        private void OnClick_OK(object sender, RoutedEventArgs rea)
        {
        	int paramsCount = MainPanel.Children.Count - 1; // (ignore the OK/Cancel button panel)
        	if (paramsCount != parameters.Length) {
        		throw new ArgumentOutOfRangeException("The number of question panels doesn't equal the number of parameters to return.");
        	}
        	
        	bool incomplete = false;
        	for (int i = 0; i < MainPanel.Children.Count - 1; i++) {
        		QuestionPanel panel = (QuestionPanel)MainPanel.Children[i];
         		if (panel.Answer == null) {        			
        			incomplete = true;
        			break;
        		}
        		else {
        			parameters[i] = panel.Answer;
        		}       		
        	}
        	
        	if (incomplete) {
        		Say.Warning("You need to give a valid answer to every question on this screen before continuing.");
        		for (int i = 0; i < parameters.Length; i++) {
        			parameters[i] = null;
        		}        		
        	}
        	else {
        		this.DialogResult = true;
        		Close();
        	}
        }
        
        private void OnClick_Cancel(object sender, RoutedEventArgs rea)
        {
        	this.DialogResult = false;
        	Close();
        }
    }
}
