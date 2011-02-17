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
using System.Collections.Generic;
using AdventureAuthor.Ideas;
using AdventureAuthor.Utils;

namespace AdventureAuthor.Ideas
{
	/// <summary>
	/// Description of FridgeMagnetUtils.
	/// </summary>
	public static class FridgeMagnetUtils
	{
		public static void MagnetFileToPlainText(string magnetCollectionPath, string plainTextPath, bool openFile)
		{
			try {
				if (!File.Exists(magnetCollectionPath)) {
					Say.Error("There is no file at location " + magnetCollectionPath);
					return;
				}				
				
				List<MagnetControlInfo> magnets = null;
				
				try {
					object box = Serialization.Deserialize(magnetCollectionPath,typeof(MagnetBoxInfo));
					if (box != null) {
						MagnetBoxInfo magnetBox = (MagnetBoxInfo)box;
						magnets = magnetBox.Magnets;
						MagnetsToPlainText(magnets,plainTextPath,openFile);
					}
				}
				catch (Exception) {
					object board = Serialization.Deserialize(magnetCollectionPath,typeof(MagnetBoardInfo));
					if (board != null) {
						MagnetBoardInfo magnetBoard = (MagnetBoardInfo)board;
						magnets = magnetBoard.Magnets;
						MagnetsToPlainText(magnets,plainTextPath,openFile);
					}
				}
								
				
			}
			catch (Exception e) {
				Say.Error("Something went wrong when trying to export a Magnet Box to a plain text file.",e);
			}
		}
		
		
		public static void MagnetsToPlainText(List<MagnetControlInfo> magnets, string plainTextPath, bool openFile)
		{
			try {				
				FileInfo fileInfo = new FileInfo(plainTextPath);
				
				using (StreamWriter writer = fileInfo.CreateText())
				{
					writer.AutoFlush = false;
					writer.WriteLine("--- Magnets ---");
					foreach (MagnetControlInfo magnet in magnets) {
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
		
		
		public static void ConvertAllMagnetCollectionsToPlainText(string directory)
		{
			if (!Directory.Exists(directory)) {
				throw new ArgumentNullException("You passed a null directory.");
			}
						
			DirectoryInfo dir = new DirectoryInfo(directory);
			FileInfo[] xmlFiles = dir.GetFiles("*.xml",SearchOption.AllDirectories);
			foreach (FileInfo file in xmlFiles) {	
				try {
					string plainTextPath = file.FullName+".txt";
					MagnetFileToPlainText(file.FullName,plainTextPath,false);
				}
				catch (Exception e) {
					Say.Debug("Oops! " + e.ToString());
				}
			}
			
			Say.Information("Finished - checked " + xmlFiles.Length + " files.");
		}
		
		
		public static void ConvertAllMagnetBoxesToPlainTextDialog()
		{
			FolderBrowserDialog dialog = new FolderBrowserDialog();
			dialog.Description = "Select the directory containing Magnet Boxes/Boards.";
			dialog.ShowNewFolderButton = false;
			DialogResult result = dialog.ShowDialog();
			if (result != DialogResult.OK) {
				return;
			}
			
			try {
				ConvertAllMagnetCollectionsToPlainText(dialog.SelectedPath);
			}
			catch (Exception e) {
				Say.Error("Failed to convert contents of directory.",e);
			}			
		}
	}
}
