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
    	
    	public ScriptParametersWindow(ref object[] parameters, string title)
        {
    		this.parameters = parameters;    		
            InitializeComponent();
    		this.Title = title;
        }        

        public void AddBooleanQuestion(string question)
        {
        	BooleanQuestionPanel panel = new BooleanQuestionPanel(question);
        	QuestionsPanel.Children.Add(panel);
        }
        
        public void AddBooleanQuestion(string question, string trueText, string falseText)
        {
        	BooleanQuestionPanel panel = new BooleanQuestionPanel(question,trueText,falseText);
        	QuestionsPanel.Children.Add(panel);
        }
        
        public void AddEnumQuestion(string question, Type enumType)
        {
        	EnumQuestionPanel panel = new EnumQuestionPanel(question,enumType);
        	QuestionsPanel.Children.Add(panel);
        }
        
        public void AddIntegerQuestion(string question)
        {
        	IntegerQuestionPanel panel = new IntegerQuestionPanel(question);
        	QuestionsPanel.Children.Add(panel);
        }
        
        public void AddIntegerQuestion(string question, int? min, int? max)
        {
        	IntegerQuestionPanel panel = new IntegerQuestionPanel(question,min,max);
        	QuestionsPanel.Children.Add(panel);
        }
        
        public void AddStringQuestion(string question)
        {
        	StringQuestionPanel panel = new StringQuestionPanel(question);
        	QuestionsPanel.Children.Add(panel);
        }
        
        public void AddTagQuestion(string question, TagHelper.TagType tagType)
        {
        	TagHelper.TagType[] tagTypes = new TagHelper.TagType[]{tagType};
        	AddTagQuestion(question,tagTypes);
        }
        
        public void AddTagQuestion(string question, TagHelper.TagType[] tagTypes)
        {
        	TagQuestionPanel panel = new TagQuestionPanel(question,tagTypes);
        	QuestionsPanel.Children.Add(panel);
        }
        
        private void OnClick_OK(object sender, RoutedEventArgs rea)
        {
        	if (QuestionsPanel.Children.Count != parameters.Length) {
        		throw new ArgumentOutOfRangeException("Found " + QuestionsPanel.Children.Count + 
        		                                      " panels and " + parameters.Length + " parameters.");
        	}
        	
        	bool incomplete = false;
        	for (int i = 0; i < QuestionsPanel.Children.Count; i++) {
        		IQuestionPanel panel = (IQuestionPanel)QuestionsPanel.Children[i];
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
