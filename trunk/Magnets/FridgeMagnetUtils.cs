/*
 * Created by SharpDevelop.
 * User: Kirn
 * Date: 17/07/2008
 * Time: 09:39
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.IO;
using System.Diagnostics;
using System.Windows.Forms;
using System.Xml.Serialization;
using AdventureAuthor.Ideas;
using AdventureAuthor.Utils;

namespace AdventureAuthor.Ideas
{
	/// <summary>
	/// Description of FridgeMagnetUtils.
	/// </summary>
	public static class FridgeMagnetUtils
	{
		public static void MagnetBoxToPlainText(string magnetBoxPath, string plainTextPath, bool openFile)
		{
			try {
				if (!File.Exists(magnetBoxPath)) {
					Say.Error("There is no file at location " + magnetBoxPath);
					return;
				}
				object o = Serialization.Deserialize(magnetBoxPath,typeof(MagnetBoxInfo));
				if (o == null || !(o is MagnetBoxInfo)) {
					Say.Error(magnetBoxPath + " is not a valid Magnet Box file.");
					return;
				}
				MagnetBoxInfo magnetBoxInfo = (MagnetBoxInfo)o;
				MagnetBoxToPlainText(magnetBoxInfo,plainTextPath,openFile);
			}
			catch (Exception e) {
				Say.Error("Something went wrong when trying to export a Magnet Box to a plain text file.",e);
			}
		}
		
		
		public static void MagnetBoxToPlainText(MagnetBoxInfo magnetBoxInfo, string plainTextPath, bool openFile)
		{
			if (magnetBoxInfo == null) {
				throw new ArgumentNullException("You need to pass a MagnetBoxInfo argument.");
			}
			
			try {				
				FileInfo fileInfo = new FileInfo(plainTextPath);
				
				using (StreamWriter writer = fileInfo.CreateText())
				{
					writer.AutoFlush = false;
					writer.WriteLine("--- Magnet Box contents ---");
					foreach (MagnetControlInfo magnet in magnetBoxInfo.Magnets) {
						Idea idea = magnet.Idea;
						writer.WriteLine();
						writer.WriteLine("    Idea: " + idea.Text);
						writer.WriteLine("Category: " + idea.Category);
						if (idea.IsStarred) {
							writer.WriteLine("* This idea has been marked as a favourite.");
						}						
					}
				}
				
				if (openFile) {
					Process.Start(plainTextPath);
				}
			}
			catch (Exception e) {
				Say.Error("Something went wrong when trying to export a Magnet Box to a plain text file.",e);
			}
		}
		
		
		public static void ConvertAllMagnetBoxesToPlainText(string directory)
		{
			if (!Directory.Exists(directory)) {
				throw new ArgumentNullException("You passed a null directory.");
			}
			
			int count = 0;
			XmlSerializer xml = new XmlSerializer(typeof(MagnetBoxInfo));
			
			DirectoryInfo dir = new DirectoryInfo(directory);
			foreach (FileInfo file in dir.GetFiles("*.xml",SearchOption.AllDirectories)) {	
				try {
					object obj = xml.Deserialize(file.Open(FileMode.Open));
					if (obj != null) {
						MagnetBoxInfo magnetBox = (MagnetBoxInfo)obj;
						string plainTextPath = file.FullName+".txt";
						MagnetBoxToPlainText(magnetBox,plainTextPath,false);
						count++;
					}
				}
				catch (Exception e) {
					Say.Debug("Oops! " + e.ToString());
				}
			}
			
			Say.Information(count + " Magnet Box(es) converted to plain text.");
		}
		
		
		public static void ConvertAllMagnetBoxesToPlainTextDialog()
		{
			FolderBrowserDialog dialog = new FolderBrowserDialog();
			dialog.Description = "Select the directory containing Magnet Boxes.";
			dialog.ShowNewFolderButton = false;
			DialogResult result = dialog.ShowDialog();
			if (result != DialogResult.OK) {
				return;
			}
			
			try {
				ConvertAllMagnetBoxesToPlainText(dialog.SelectedPath);
			}
			catch (Exception e) {
				Say.Error("Failed to convert contents of directory.",e);
			}			
		}
	}
}
