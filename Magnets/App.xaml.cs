using System;
using System.IO;
using System.Windows;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Xml;
using System.Configuration;
using AdventureAuthor.Utils;

namespace AdventureAuthor.Ideas
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		private MagnetBoardViewer viewer = null;
			
		
		public App()
		{
//			this.Startup += delegate (object sender, StartupEventArgs e) {
//				Say.Information(e.Args.ToString());
//			};
			try {			
				Process[] fridgeMagnetApps = Process.GetProcessesByName(Process.GetCurrentProcess().ProcessName);			
				if (fridgeMagnetApps.Length > 1) {
					Say.Information("Fridge Magnets is already running.\n\n" +
					                "Look for the lightbulb icon in your system tray, " +
					                "and double-click it to bring up Fridge Magnets.");
					Application.Current.Shutdown();
				}						
				
				viewer = new MagnetBoardViewer();
				viewer.Show();
			}
			catch (Exception x) {
				Say.Error("Something went wrong while loading Fridge Magnets.",x);
			}
		}
		
		
		private void appStarted(object sender, StartupEventArgs e)
		{
			// check whether the app was started by double-clicking a file:
			if (e.Args.Length > 0) {				
				string arg0 = e.Args[0];
				if (File.Exists(arg0)) {
					viewer.Open(arg0);
				}
			}
		}
	}
}