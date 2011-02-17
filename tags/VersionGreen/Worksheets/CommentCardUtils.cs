/*
 * Created by SharpDevelop.
 * User: Kirn
 * Date: 17/07/2008
 * Time: 14:31
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using System.Xml.Serialization;
using AdventureAuthor.Utils;

namespace AdventureAuthor.Evaluation
{
	/// <summary>
	/// Description of CommentCardUtils.
	/// </summary>
	public static class CommentCardUtils
	{		
    	/// <summary>
    	/// Export a given Comment Card to plain text.
    	/// </summary>
    	/// <param name="card"></param>
    	/// <param name="plainTextPath"></param>
		public static void ExportToTextFile(Card card, string plainTextPath, bool openFile)
		{
			FileInfo fi = new FileInfo(plainTextPath);
			
			using (StreamWriter sw = fi.CreateText())
			{
				sw.AutoFlush = false;
				sw.WriteLine("Comment Card:\t" + card.Title);
				sw.WriteLine("Game designer:\t" + card.DesignerName);
				sw.WriteLine("Evaluator:\t" + card.EvaluatorName);
				sw.WriteLine("Filled in on:\t" + card.Date);
				sw.WriteLine();
				sw.WriteLine();
							
				foreach (Section section in card.Sections) {
					// Check that a section has not been excluded, either because it has been excluded explicitly
					// or because it has no questions which are to be included:
					if (!section.Include) {
						continue;
					}
					else {
						bool sectionHasQuestions = false;
						foreach (Question question in section.Questions) {
							if (question.Include) {
								sectionHasQuestions = true;
								break;
							}
						}
						if (!sectionHasQuestions) {
							continue;
						}
					}
					
					int questionCount = 0;
					sw.WriteLine(">>>>> " + section.Title + " <<<<<");
					sw.WriteLine();
					foreach (Question question in section.Questions) {
						if (question.Include) {
							questionCount++;
							sw.WriteLine("Q" + questionCount + ": " + question.Text);
							sw.WriteLine();
							sw.WriteLine("Answer(s):");
							foreach (Answer answer in question.Answers) {
								if (answer.Include) {
									sw.WriteLine("- " + answer.ToString());
								}
							}
							if (question.Replies.Count > 0) {
								sw.WriteLine();
								sw.WriteLine("Replies:");
								foreach (Reply reply in question.Replies) {
									sw.WriteLine("- " + reply.ToString());
								}
							}
							sw.WriteLine();
							sw.WriteLine();
						}
					}
				}
				sw.Flush();
			}
			
			if (openFile) {
				Process.Start(plainTextPath);
			}
		}		
		
		
		public static void ConvertAllCommentCardsToPlainText(string directory)
		{
			if (!Directory.Exists(directory)) {
				throw new ArgumentNullException("You passed a null directory.");
			}
			
			int count = 0;
			XmlSerializer xml = new XmlSerializer(typeof(Card));
			
			DirectoryInfo dir = new DirectoryInfo(directory);
			foreach (FileInfo file in dir.GetFiles("*.xml",SearchOption.AllDirectories)) {	
				try {
					object obj = xml.Deserialize(file.Open(FileMode.Open));
					if (obj != null) {
						Card card = (Card)obj;
						string plainTextPath = file.FullName+".txt";
						ExportToTextFile(card,plainTextPath,false);
						count++;
					}
				}
				catch (Exception e) {
					Say.Debug("Oops! " + e.ToString());
				}
			}
			
			Say.Information(count + " Comment Card(s) converted to plain text.");
		}
		
		
		public static void ConvertAllCommentCardsToPlainTextDialog()
		{
			FolderBrowserDialog dialog = new FolderBrowserDialog();
			dialog.Description = "Select the directory containing Comment Cards.";
			dialog.ShowNewFolderButton = false;
			DialogResult result = dialog.ShowDialog();
			if (result != DialogResult.OK) {
				return;
			}
			
			try {
				ConvertAllCommentCardsToPlainText(dialog.SelectedPath);
			}
			catch (Exception e) {
				Say.Error("Failed to convert contents of directory.",e);
			}			
		}
	}
}
