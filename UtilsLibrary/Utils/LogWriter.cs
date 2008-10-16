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
using System.Text;
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
		
		
		static LogWriter()
		{
			string myDocumentsFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
			string publicUserDirectory = Path.Combine(myDocumentsFolder,"Adventure Author");
			logDirectory = Path.Combine(publicUserDirectory,"User logs");
		}
		
			
		/// <summary>
		/// Start writing log messages to file. 
		/// </summary>
		/// <param name="identifier">An optional identifier string to place on the end of a
		/// filename, after the username and date/time. For example, passing a value of
		/// "toolset" gives a log file called 'Kirn 16.10.2008 1253 toolset.log'.</param>
		public static void StartRecording(string identifier)
		{			
			try {
				System.Diagnostics.Debug.WriteLine("LogWriter started recording.");
				Tools.EnsureDirectoryExists(LogDirectory);
				StringBuilder preferredFilename = new StringBuilder(User.GetCurrentUserName() + 
				                         " " + Tools.GetDateStamp(false) + " " + Tools.GetTimeStamp(true));
				if (identifier != null && identifier.Length > 0) {
					preferredFilename.Append(" " + identifier);
				}
				string path = GetUnusedPath(logDirectory,preferredFilename.ToString(),"log");
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
			try {
				if (writer != null) {
					writer.Flush();
					writer.Close();
				}
			}
			catch (ObjectDisposedException) {
				//Say.Error("The log writer was already disposed by the time it was told to stop recording.");
			}
		}
		
		
		private static void LogMessage(object sender, LogEventArgs e)
		{
			try {
				if (writer != null) {
					writer.WriteLine(e.Message);
					writer.Flush();
				}
			}
			catch (ObjectDisposedException) {
				//Say.Error("The log writer was already disposed by the time it was told to log message '" + e.Message + "'.\n\n"+Environment.StackTrace);
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
