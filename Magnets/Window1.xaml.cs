using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Messaging;
using System.IO;
using System.IO.Pipes;
using System.Threading;
using AdventureAuthor.Utils;

namespace Magnets
{
	/// <summary>
	/// Interaction logic for Window1.xaml
	/// </summary>
	public partial class Window1 : Window
	{		
		private static string pipeName = "magnets";
		
		
		public Window1()
		{
			InitializeComponent();					
			
			Loaded += delegate { 
				Thread thread = new Thread(new ThreadStart(listenForConnection));
				thread.Priority = ThreadPriority.BelowNormal;
				thread.Start(); 
			};
		}
		
		
		private void listenForConnection()
		{
			using (NamedPipeServerStream server = new NamedPipeServerStream(pipeName,PipeDirection.In))
			{
				server.WaitForConnection();
				
				string message;			
				using (StreamReader reader = new StreamReader(server)) // when this is disposed, will it dispose the Server?
				{
					while ((message = reader.ReadLine()) != null) {
						Console.WriteLine("Received: " + message);
					}
					if (server.IsConnected) {
						server.Disconnect();
					}
				}				
			}
				
			listenForConnection();
		}
	}
}