/*
 * Created by SharpDevelop.
 * User: Kirn
 * Date: 07/03/2008
 * Time: 00:07
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using AdventureAuthor.Utils;

namespace AdventureAuthor.Conversations.UI
{
	/// <summary>
	/// Description of SpeakerButton.
	/// </summary>
	public class SpeakerButton : Button
	{
		private TextBlock speakerTextBlock;
		private TextBlock grammarTextBlock;
		private Button button;
		
		private Speaker speaker;		
		public Speaker Speaker {
			get { return speaker; }
			set { 
				speaker = value;
				UpdateSpeakerName();
			}
		}
		
		
		public SpeakerButton(Speaker speaker)
		{
			Margin = new Thickness(2);
			TextBlock textBlock = new TextBlock();
			grammarTextBlock = new TextBlock();
			speakerTextBlock = new TextBlock();
			TextBlock tb2 = new TextBlock();
			grammarTextBlock.Foreground = Brushes.Black;
			tb2.Foreground = Brushes.Black;
			tb2.Text = " line";
			textBlock.Inlines.Add(grammarTextBlock);
			textBlock.Inlines.Add(speakerTextBlock);
			textBlock.Inlines.Add(tb2);
			
			Speaker = speaker;
						
			Content = textBlock;	
		}
		
		
		public void UpdateSpeakerName()
		{
			speakerTextBlock.Text = speaker.Name.ToUpper();
			speakerTextBlock.Foreground = speaker.Colour;	
			if (speaker.Name.Length > 0 && Tools.StartsWithVowel(speaker.Name)) {
				grammarTextBlock.Text = "Add an ";
			}
			else {
				grammarTextBlock.Text = "Add a ";
			}
		}
	}
}
