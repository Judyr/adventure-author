using System;
using System.Windows;
using AdventureAuthor.Utils;
using NWN2Toolset.NWN2.Data;
using AdventureAuthor.Evaluation.UI;

namespace AdventureAuthor.Evaluation.UI
{
	/// <summary>
	/// A window which provides a number of criteria for evaluating a module
	/// and elicits a response from the user as to how well they have been met.
	/// </summary>
    public partial class EvaluationWindow : Window
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
    	/// Create a new instance of an evaluation window.
    	/// </summary>
    	/// <param name="parameters">The array to hold the answers (i.e. parameter values) in; passed by reference</param>
    	/// <param name="title">The window title - usually identifies the worksheet tempalte (e.g. 'Intermediate 2 worksheet')</param>
    	public EvaluationWindow(ref object[] parameters, string title)
        {
    		this.parameters = parameters;    		
            InitializeComponent();
    		this.Title = title;
    		//this.ResizeMode = ResizeMode.NoResize;
    		this.Loaded += delegate
    		{      			
    			//Height = (QuestionsPanel.Children.Count * 120) + 85;
    		};
    		    		
    		EvaluationSection dialogue = new EvaluationSection("Dialogue");
    		dialogue.AddField(new StatementQuestionPanel("A very controversial statement",true));
    		AddSection(dialogue);
    		
    		EvaluationSection characters = new EvaluationSection("Characters");
    		characters.AddField(new StatementQuestionPanel("There is a variety of characters"));
    		characters.AddField(new StatementQuestionPanel("Thought has been given to the appearance of the characters"));
    		characters.AddField(new StatementQuestionPanel("Animations have been used to bring characters to life"));
    		characters.AddField(new StatementQuestionPanel("The characters are relevant to the story"));
    		characters.AddField(new StatementQuestionPanel("The characters have a range of roles",true));
    		characters.AddField(new StatementQuestionPanel("The characters' personalities are shown through dialogue/behaviour/appearance"));
    		characters.AddField(new StatementQuestionPanel("Relationships have been established between the characters"));
    		characters.AddField(new StatementQuestionPanel("The characters have differing points of view"));
    		characters.AddField(new StatementQuestionPanel("The characters reveal their motivations",true));
    		characters.AddField(new StatementQuestionPanel("The characters have backstories"));
    		characters.AddField(new StatementQuestionPanel("The characters evoke an emotional response"));
    		AddSection(characters);
        }      
    	
    	
    	public void AddSection(EvaluationSection section)
    	{
    		EvaluationSectionsPanel.Children.Add(section);
    	}

    	#endregion
    }
}
