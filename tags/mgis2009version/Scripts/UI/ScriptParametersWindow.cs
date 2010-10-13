using System;
using System.Windows;
using AdventureAuthor.Utils;
using NWN2Toolset.NWN2.Data;

namespace AdventureAuthor.Scripts.UI
{
	/// <summary>
	/// A window which poses a series of questions to the user in order to elicit values for game script parameters.
	/// </summary>
    public partial class ScriptParametersWindow : Window
    {    	
    	#region Fields
    	
    	/// <summary>
    	/// A referenced array which holds the answers to the questions posed. 
    	/// Must be of an equal length to the number of question panels ultimately added.
    	/// </summary>
    	private object[] parameters;
    	
    	#endregion
    	
    	#region Constructors
    	
    	/// <summary>
    	/// Create a new instance of a script parameters window, 
    	/// which poses a series of questions to the user in order to elicit values for game script parameters.
    	/// </summary>
    	/// <param name="parameters">The array to hold the answers (i.e. parameter values) in; passed by reference</param>
    	/// <param name="title">The title text to display in the window - usually identifies the game script (e.g. 'Add a henchman')</param>
    	public ScriptParametersWindow(ref object[] parameters, string title)
        {
    		this.parameters = parameters;    		
            InitializeComponent();
    		this.Title = title;
    		this.ResizeMode = ResizeMode.NoResize;
    		this.Loaded += delegate {
    			Height = (QuestionsPanel.Children.Count * 120) + 85;
    		};
        }      

    	#endregion
    	
    	#region Methods
    	
    	/// <summary>
    	/// Add a question which will be answered by a boolean value.
    	/// </summary>
    	/// <param name="question">The question to ask the user</param>
        public void AddBooleanQuestion(string question)
        {
        	BooleanQuestionPanel panel = new BooleanQuestionPanel(question);
        	QuestionsPanel.Children.Add(panel);
        }
        
        
    	/// <summary>
    	/// Add a question which will be answered by a boolean value.
    	/// </summary>
    	/// <param name="question">The question to ask the user</param>
    	/// <param name="trueText">The text to display instead of "Yes" for the 'true' answer</param>
    	/// <param name="falseText">The text to display instead of "No" for the 'false' answer</param>
        public void AddBooleanQuestion(string question, string trueText, string falseText)
        {
        	BooleanQuestionPanel panel = new BooleanQuestionPanel(question,trueText,falseText);
        	QuestionsPanel.Children.Add(panel);
        }
        
        
    	/// <summary>
    	/// Add a question which will be answered by a boolean value.
    	/// </summary>
    	/// <param name="question">The question to ask the user</param>
    	/// <param name="trueText">The text to display instead of "Yes" for the 'true' answer</param>
    	/// <param name="falseText">The text to display instead of "No" for the 'false' answer</param>
        /// <param name="defaultValue">The default value of this answer on loading the window</param>
        public void AddBooleanQuestion(string question, string trueText, string falseText, bool defaultValue)
        {
        	BooleanQuestionPanel panel = new BooleanQuestionPanel(question,trueText,falseText,defaultValue);
        	QuestionsPanel.Children.Add(panel);
        }
        
        
    	/// <summary>
    	/// Add a question which will be answered by an enum value.
    	/// </summary>
    	/// <param name="question">The question to ask the user</param>
        /// <param name="enumType">The enum the user will answer from</param>
        public void AddEnumQuestion(string question, Type enumType)
        {
        	EnumQuestionPanel panel = new EnumQuestionPanel(question,enumType);
        	QuestionsPanel.Children.Add(panel);
        }        
        
        
    	/// <summary>
    	/// Add a question which will be answered by an enum value.
    	/// </summary>
    	/// <param name="question">The question to ask the user</param>
        /// <param name="enumType">The enum the user will answer from</param>
        /// <param name="defaultValue">The default value of this answer on loading the window</param>
        public void AddEnumQuestion(string question, Type enumType, string defaultValue)
        {
        	EnumQuestionPanel panel = new EnumQuestionPanel(question,enumType,defaultValue);
        	QuestionsPanel.Children.Add(panel);
        }        
        
        
    	/// <summary>
    	/// Add a question which will be answered by a float value.
    	/// </summary>
    	/// <param name="question">The question to ask the user</param>
        public void AddFloatQuestion(string question)
        {
        	FloatQuestionPanel panel = new FloatQuestionPanel(question);
        	QuestionsPanel.Children.Add(panel);
        }
        
        
    	/// <summary>
    	/// Add a question which will be answered by a float value.
    	/// </summary>
    	/// <param name="question">The question to ask the user</param>
    	/// <param name="min">The minimum allowed value for this answer</param>
    	/// <param name="max">The maximum allowed value for this answer</param>
    	/// <remarks>Min/max are int values in order to hide the float representation</remarks>
        public void AddFloatQuestion(string question, int? min, int? max) 
        {
        	FloatQuestionPanel panel = new FloatQuestionPanel(question,min,max);
        	QuestionsPanel.Children.Add(panel);
        }      
        
                
    	/// <summary>
    	/// Add a question which will be answered by a float value.
    	/// </summary>
    	/// <param name="question">The question to ask the user</param>
    	/// <param name="min">The minimum allowed value for this answer</param>
    	/// <param name="max">The maximum allowed value for this answer</param>
        /// <param name="defaultValue">The default value of this answer on loading the window</param>
    	/// <remarks>Min/max are int values in order to hide the float representation</remarks>
        public void AddFloatQuestion(string question, int? min, int? max, int defaultValue)
        {
        	FloatQuestionPanel panel = new FloatQuestionPanel(question,min,max,defaultValue);
        	QuestionsPanel.Children.Add(panel);
        }
        
        
    	/// <summary>
    	/// Add a question which will be answered by an int value.
    	/// </summary>
    	/// <param name="question">The question to ask the user</param>
        public void AddIntegerQuestion(string question)
        {
        	IntegerQuestionPanel panel = new IntegerQuestionPanel(question);
        	QuestionsPanel.Children.Add(panel);
        }
        
        
    	/// <summary>
    	/// Add a question which will be answered by an int value.
    	/// </summary>
    	/// <param name="question">The question to ask the user</param>
    	/// <param name="min">The minimum allowed value for this answer</param>
    	/// <param name="max">The maximum allowed value for this answer</param>
        public void AddIntegerQuestion(string question, int? min, int? max)
        {
        	IntegerQuestionPanel panel = new IntegerQuestionPanel(question,min,max);
        	QuestionsPanel.Children.Add(panel);
        }
        
        
    	/// <summary>
    	/// Add a question which will be answered by an int value.
    	/// </summary>
    	/// <param name="question">The question to ask the user</param>
    	/// <param name="min">The minimum allowed value for this answer</param>
    	/// <param name="max">The maximum allowed value for this answer</param>
        /// <param name="defaultValue">The default value of this answer on loading the window</param>    
        public void AddIntegerQuestion(string question, int? min, int? max, int defaultValue)
        {
        	IntegerQuestionPanel panel = new IntegerQuestionPanel(question,min,max,defaultValue);
        	QuestionsPanel.Children.Add(panel);
        }
               
        
    	/// <summary>
    	/// Add a question which will be answered by a string value.
    	/// </summary>
    	/// <param name="question">The question to ask the user</param>
        public void AddStringQuestion(string question)
        {
        	StringQuestionPanel panel = new StringQuestionPanel(question);
        	QuestionsPanel.Children.Add(panel);
        }    	
        
        
        
        /// <summary>
    	/// Add a question which will be answered by a string value.
    	/// </summary>
    	/// <param name="question">The question to ask the user</param>
        /// <param name="defaultValue">The default value of this answer on loading the window</param>    
        public void AddStringQuestion(string question, string defaultValue)
        {
        	StringQuestionPanel panel = new StringQuestionPanel(question,defaultValue);
        	QuestionsPanel.Children.Add(panel);
        }
        
                
        /// <summary>
        /// Add a question which will be answered by a string value corresponding to the tag of a game object.
        /// </summary>
        /// <param name="question">The question to ask the user</param>
        /// <param name="tagType">The type of object to populate the tag list from (e.g. Creature, Placeable, or All)</param>
        public void AddTagQuestion(string question, TaggedType tagType)
        {
        	TaggedType[] tagTypes = new TaggedType[]{tagType};
        	AddTagQuestion(question,tagTypes);
        }
        
        
        /// <summary>
        /// Add a question which will be answered by a string value corresponding to the tag of a game object.
        /// </summary>
        /// <param name="question">The question to ask the user</param>
        /// <param name="tagType">The (multiple) types of object to populate the tag list from</param>
        public void AddTagQuestion(string question, TaggedType[] tagTypes)
        {
        	TagQuestionPanel panel = new TagQuestionPanel(question,tagTypes);
        	QuestionsPanel.Children.Add(panel);
        }
                       
        
        /// <summary>
        /// Add a question which will be answered by a string value corresponding to the resref of a game object blueprint.
        /// </summary>
        /// <param name="question">The question to ask the user</param>
        /// <param name="resrefType">The type of blueprint to populate the resref list from (e.g. Creature, Placeable, or All)</param>
        public void AddResRefQuestion(string question, TaggedType resrefType)
        {
        	TaggedType[] resrefTypes = new TaggedType[]{resrefType};
        	AddResRefQuestion(question,resrefTypes);
        }
        
        
        /// <summary>
        /// Add a question which will be answered by a string value corresponding to the resref of a game object blueprint.
        /// </summary>
        /// <param name="question">The question to ask the user</param>
        /// <param name="resrefTypes">The (multiple) types of blueprint to populate the resref list from</param>
        public void AddResRefQuestion(string question, TaggedType[] resrefTypes)
        {
        	ResRefQuestionPanel panel = new ResRefQuestionPanel(question,resrefTypes);
        	QuestionsPanel.Children.Add(panel);
        }

        
        /// <summary>
        /// Add a question which will be answered by a string value corresponding to the name of a game variable.
        /// </summary>
        /// <param name="question">The question to ask the user</param>
        /// <param name="variableType">The type of variable which the user can select from (e.g. int, string) </param>
        public void AddVariableQuestion(string question, NWN2ScriptVariableType variableType)
        {
        	VariableQuestionPanel panel = new VariableQuestionPanel(question,variableType);
        	QuestionsPanel.Children.Add(panel);
        }
        
        #endregion Methods
        
        #region Event handlers
        
        /// <summary>
        /// Attempt to return an answer from the window, warning if the answers are incomplete.
        /// </summary>
        private void OnClick_OK(object sender, RoutedEventArgs rea)
        {
        	if (QuestionsPanel.Children.Count != parameters.Length) {
        		throw new ArgumentOutOfRangeException("Found " + QuestionsPanel.Children.Count + 
        		                                      " panels and " + parameters.Length + " parameters.");
        	}
        	
        	bool incomplete = false;
        	for (int i = 0; i < QuestionsPanel.Children.Count; i++) {
        		ParameterPanel panel = (ParameterPanel)QuestionsPanel.Children[i];
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
        
        
        /// <summary>
        /// Cancel the window.
        /// </summary>
        private void OnClick_Cancel(object sender, RoutedEventArgs rea)
        {
        	this.DialogResult = false;
        	Close();
        }
        
        #endregion
    }
}
