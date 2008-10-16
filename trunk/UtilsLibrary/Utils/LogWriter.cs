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

namespace AdventureAuthor.Utils
{
	/// <summary>
	/// Description of LogWriter.
	/// </summary>
	public static class LogWriter
	{
		private static StreamWriter writer = null;				
				
		
		private static string logDirectory;
		public static string LogDirectory {
			get { return logDirectory; }
			set { logDirectory = value; }
		}
		
		
		public static void StartRecording()
		{			
			try {
				System.Diagnostics.Debug.WriteLine("LogWriter started recording.");
				string preferredFilename = User.GetCurrentUserName() + " " + Tools.GetDateStamp(false) + " " + Tools.GetTimeStamp(true) + " toolset";
				string path = GetUnusedPath(logDirectory,preferredFilename,"log");						
				FileInfo f = new FileInfo(path);				
				Stream s = f.Open(FileMode.Create);
				writer = new StreamWriter(s);
				writer.AutoFlush = true;
				Log.Message += new EventHandler<LogEventArgs>(LogMessage);
			}
			catch (Exception e) {
				Say.Error(e.ToString());
			}
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
			System.Diagnostics.Debug.WriteLine("LogMessage() called.");
			if (writer != null) {
				writer.WriteLine(e.Message);
				writer.Flush();
			}
			System.Diagnostics.Debug.WriteLine("Message logged.");
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
