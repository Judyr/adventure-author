/*
 * Created by SharpDevelop.
 * User: Kirn
 * Date: 12/11/2007
 * Time: 20:54
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.IO;
using AdventureAuthor.Utils;
using AdventureAuthor.Core;

namespace AdventureAuthor.Utils
{
	/// <summary>
	/// Description of LogWriter.
	/// </summary>
	public static class LogWriter
	{
		private static StreamWriter writer = null;				
		
		
		public static void StartRecording()
		{			
			string preferredFilename = User.GetCurrentUserName() + " " + Tools.GetDateStamp(false) + " " + Tools.GetTimeStamp(true);
			string path = GetUnusedPath(ModuleHelper.UserLogDirectory,preferredFilename,"log");						
			FileInfo f = new FileInfo(path);				
			Stream s = f.Open(FileMode.Create);
			writer = new StreamWriter(s);
			writer.AutoFlush = true;
			Log.Message += new EventHandler<LogEventArgs>(LogMessage);
		}
		
		
		public static void StopRecording()
		{
			if (writer != null) {
				writer.Flush();
				writer.Close();
			}
		}
		
		
		private static void LogMessage(object sender, LogEventArgs e)
		{
			if (writer != null) {
				writer.WriteLine(e.Message);
				writer.Flush();
			}
		}
		
		
		public static string GetUnusedPath(string directory, string preferredFilename, string fileExtension)
		{
			int count = 1;
			string path = Path.Combine(directory,preferredFilename+"."+fileExtension);
			while (File.Exists(path)) {
				count++;
				string newfilename = preferredFilename + " (" + count + ")";
				path = Path.Combine(directory,newfilename+"."+fileExtension);						
			}			
			return path;
		}
	}
}
