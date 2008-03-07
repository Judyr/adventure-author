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
			// Filename:
			// Date_Username.log
			// e.g. 05_05_07_JackStuart.log
			// e.g. 05_05_07_JackStuart2.log
			// Place in AdventureAuthor/logs
			
			string filename = Tools.GetDateStamp();// + "_" + username
			string logpath = Path.Combine(ModuleHelper.LogDir,filename+".log");
			
			// If the filename is already taken, add numbering ("<filename>2.log", "<filename>3.log" etc.):
			int count = 1;
			while (File.Exists(logpath)) {
				count++;
				string newfilename = filename + count.ToString();
				logpath = Path.Combine(ModuleHelper.LogDir,newfilename+".log");								
			}			
			
			FileInfo f = new FileInfo(logpath);				
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
	}
}
