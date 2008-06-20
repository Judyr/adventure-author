using System;
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
		public App()
		{
			// Abort if another process with the same name is already running. Note that if you
			// rename Fridge Magnets.exe to something else and run that, it won't stop you -
			// but I can't imagine why anyone would bother.
			if (!(Process.GetProcessesByName(Process.GetCurrentProcess().ProcessName).Length == 1)) {
				Say.Information("Fridge Magnets is already running!");
				Application.Current.Shutdown();
			}
			
			StartupUri = new Uri("MagnetBoardViewer.xaml",UriKind.Relative);
		}
	}
}