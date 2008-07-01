﻿using System;
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
			this.Startup += delegate (object sender, StartupEventArgs e) {
				Say.Information(e.Args.ToString());
			};
			
			// If another process with the same name is already running, bring it to the fore. If you have been
			// passed a filename, attempt to open that file in the existing process window. Regardless, following
			// this, Shutdown the new application.
			Process[] fridgeMagnetApps = Process.GetProcessesByName(Process.GetCurrentProcess().ProcessName);			
			if (fridgeMagnetApps.Length > 0) {
				fridgeMagnetApps[0].WaitForInputIdle();
				//fridgeMagnetApps[0].MainWindowHandle.
				Say.Information("Already running: " + fridgeMagnetApps[0].Container.ToString());
				Say.Information("Already running: " + ((Window)fridgeMagnetApps[0].Container).Title);
				
				//Say.Information("Fridge Magnets is already running!");
				Application.Current.Shutdown();
			}
			
			//StartupUri = new Uri("MagnetBoardViewer.xaml",UriKind.Relative);
			
			
			viewer = new MagnetBoardViewer();
			viewer.Show();
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