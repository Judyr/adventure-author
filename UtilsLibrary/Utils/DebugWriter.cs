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
	/// Description of DebugWriter.
	/// </summary>
	public static class DebugWriter
	{
		private static StreamWriter writer = null;				
				
		
		private static string debugDirectory;
		public static string DebugDirectory {
			get { return debugDirectory; }
			set { debugDirectory = value; }
		}			
		
		
		public static void StartRecording()
		{			
			string preferredFilename = User.GetCurrentUserName() + " " + Tools.GetDateStamp(false) + " " 
				+ Tools.GetTimeStamp(true);
			string path = LogWriter.GetUnusedPath(debugDirectory,preferredFilename,"debug");
			FileInfo f = new FileInfo(path);			
			Stream s = f.Open(FileMode.Create);
			writer = new StreamWriter(s);
			writer.AutoFlush = true;
			DebugLog.Message += new EventHandler<DebugLogEventArgs>(LogMessage);
		}

		
		private static void LogMessage(object sender, DebugLogEventArgs e)
		{
			object loggable = null;
			if (e.Message != null) {
				loggable = e.Message;
			}
			else if (e.ThrownException != null) {
				loggable = e.ThrownException;
			}
			else {
				loggable = "Empty debug message.";
			}
			
			writer.WriteLine(Tools.GetTimeStamp(false) + ": " + loggable);
		}
		
		
		public static void StopRecording()
		{
			if (writer != null) {
				writer.Flush();
				writer.Close();
			}
		}
	}
}