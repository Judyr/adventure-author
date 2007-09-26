using System;
using System.Windows;
using AdventureAuthor.Utils;
using NWN2Toolset.NWN2.Data;

namespace AdventureAuthor.Scripts.UI
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
    		this.ResizeMode = ResizeMode.NoResize;
    		this.Loaded += delegate 
    		{  
    			Height = (QuestionsPanel.Children.Count * 120) + 85;
    		};
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
        
        public void AddFloatQuestion(string question)
        {
        	FloatQuestionPanel panel = new FloatQuestionPanel(question);
        	QuestionsPanel.Children.Add(panel);
        }
        
        public void AddFloatQuestion(string question, int? min, int? max) // int min/max because we are hiding float representation
        {
        	FloatQuestionPanel panel = new FloatQuestionPanel(question,min,max);
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
        
        public void AddTagQuestion(string question, ScriptHelper.ObjectType tagType)
        {
        	ScriptHelper.ObjectType[] tagTypes = new ScriptHelper.ObjectType[]{tagType};
        	AddTagQuestion(question,tagTypes);
        }
        
        public void AddTagQuestion(string question, ScriptHelper.ObjectType[] tagTypes)
        {
        	TagQuestionPanel panel = new TagQuestionPanel(question,tagTypes);
        	QuestionsPanel.Children.Add(panel);
        }
        
        public void AddResRefQuestion(string question, ScriptHelper.ObjectType resrefType)
        {
        	ScriptHelper.ObjectType[] resrefTypes = new ScriptHelper.ObjectType[]{resrefType};
        	AddResRefQuestion(question,resrefTypes);
        }
        
        public void AddResRefQuestion(string question, ScriptHelper.ObjectType[] resrefTypes)
        {
        	ResRefQuestionPanel panel = new ResRefQuestionPanel(question,resrefTypes);
        	QuestionsPanel.Children.Add(panel);
        }

        public void AddVariableQuestion(string question, NWN2ScriptVariableType variableType)
        {
        	VariableQuestionPanel panel = new VariableQuestionPanel(question,variableType);
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
