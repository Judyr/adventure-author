/*
 * Created by SharpDevelop.
 * User: Kirn
 * Date: 11/01/2008
 * Time: 11:24
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Text;
using AdventureAuthor.Utils;

namespace AdventureAuthor.Evaluation.UI
{
	/// <summary>
	/// Description of StatementQuestionPanel.
	/// </summary>
	public class StatementQuestionPanel : CompositeQuestionPanel
	{
    	#region Constants
    	
    	private const int NUMBER_OF_STARS = 5;
    	
    	#endregion
    	
		public StatementQuestionPanel(string question) : base(question)
		{
			SubQuestionsPanel.Children.Add(new StarRating(NUMBER_OF_STARS));
		}
		
		
		public StatementQuestionPanel(string question, bool provideEvidence) : this(question)
		{
			if (provideEvidence) {
				SubQuestionsPanel.Children.Add(new CommentBox("Comment: "));
				SubQuestionsPanel.Children.Add(new EvidenceButton());
				
				Button temp = new Button();
				temp.Content = "report answer";
				temp.Click += delegate { 
					StringBuilder sb = new StringBuilder("Composite answer:\n\n");
					List<object> answers = (List<object>)this.Answer;
					foreach (object answer in answers) {
						try {
						sb.Append(answer.ToString());
						}
						catch (Exception e) {
							Say.Error(e);
						}
					}
					Say.Information(sb.ToString());
				};
				SubQuestionsPanel.Children.Add(temp);
			}
		}
	}
}
