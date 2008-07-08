/*
 * Created by SharpDevelop.
 * User: Kirn
 * Date: 19/06/2008
 * Time: 13:19
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.ComponentModel;
using System.IO;
using System.IO.Pipes;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Threading;
using AdventureAuthor.Ideas;
using AdventureAuthor.Utils;

namespace AdventureAuthor.Ideas
{
	/// <summary>
	/// Description of SystemTray.
	/// </summary>
	public partial class MagnetBoardViewer : System.Windows.Window
	{				
		#region Constants
				
    	/// <summary>
    	/// A string constant that, when sent as a message to the magnets pipe, will close the thread
    	/// that listens for connections.
    	/// </summary>
    	private const string ABORTMESSAGE = "!_Application exiting: ABORT LISTENING THREAD ..._!";
		
		#endregion
		
		#region Fields
		
		private static string litbulbPath = Path.Combine(FridgeMagnetPreferences.Instance.InstallDirectory,"litbulb.ico");
		private static string unlitbulbPath = Path.Combine(FridgeMagnetPreferences.Instance.InstallDirectory,"unlitbulb.ico");
		private static System.Drawing.Icon litIcon = new System.Drawing.Icon(litbulbPath);
		private static System.Drawing.Icon unlitIcon = new System.Drawing.Icon(unlitbulbPath);
		private System.Windows.Forms.NotifyIcon trayIcon;	
		
		#endregion
		
    	#region Methods
    	
    	/// <summary>
    	/// Places a system tray icon representing the Fridge magnets application into
    	/// the system tray.
    	/// </summary>
    	private void LaunchInSystemTray()
    	{    		
    		trayIcon = new System.Windows.Forms.NotifyIcon();
    		SwitchBulbOff();
    		trayIcon.Visible = true; 
    		UpdateMagnetCountToolTip();	
    		
    		// Set up context menu:			
			ToolStripMenuItem openFridgeMagnets = new ToolStripMenuItem("View Fridge Magnets");
			openFridgeMagnets.Font = new System.Drawing.Font(openFridgeMagnets.Font,System.Drawing.FontStyle.Bold);
			openFridgeMagnets.Click += new EventHandler(ShowFridgeMagnets);
			
			ToolStripMenuItem addNewIdea = new ToolStripMenuItem("Add a new idea");
			addNewIdea.Click += new EventHandler(AddIdeaFromSystemTray);
			
//			ToolStripMenuItem options = new ToolStripMenuItem("Options");
//			options.CheckOnClick = true;
//			options.CheckStateChanged += new EventHandler(DisplayOptionsScreen);
			
			ToolStripMenuItem displayAboutScreen = new ToolStripMenuItem("About");
			displayAboutScreen.Click += delegate { MagnetBoardViewer.DisplayAboutScreen(); };
			
			ToolStripMenuItem exit = new ToolStripMenuItem("Exit");
			exit.Click += new EventHandler(ForceExit);	
			
			ContextMenuStrip strip = new ContextMenuStrip();	
			strip.Items.Add(openFridgeMagnets);				
			strip.Items.Add(addNewIdea);			
			strip.Items.Add(new ToolStripSeparator());
//			strip.Items.Add(options);
			strip.Items.Add(displayAboutScreen);
			strip.Items.Add(new ToolStripSeparator());
			strip.Items.Add(exit);			
			
			trayIcon.ContextMenuStrip = strip;
			
			// If the user attempts to close the window, simply hide it instead, so that
			// magnets can still be sent to it through the toolset and system tray:
			Closing += new CancelEventHandler(HideWindowOnClosing);
			
			// If the user double-clicks the system tray icon, ensure that the 
			// main window becomes visible (if it wasn't already):
			trayIcon.MouseDoubleClick += new MouseEventHandler(ShowFridgeMagnets);
			
			// If the user hovers the mouse over the system tray icon, display
			// a balloon tooltip informing them of their total magnet count:
			trayIcon.MouseMove += delegate { UpdateMagnetCountToolTip(); };	
    		trayIcon.BalloonTipClosed += delegate { SwitchBulbOff(); };
    		trayIcon.BalloonTipClicked += delegate { SwitchBulbOff(); };
    		trayIcon.BalloonTipShown += delegate { SwitchBulbOn(); };
			
			ShowBalloon("Double-click me to start working with Fridge Magnets!",12000);
    	} 
		
    	
    	/// <summary>
    	/// Provides access to the HideWindow method, allowing the main window to be hidden instead of closed.
    	/// </summary>
		public delegate void HideWindowDelegate();	
		
		
		/// <summary>
		/// Hide the window.
		/// </summary>
		/// <remarks>Intended for use with HideWindowDelegate.</remarks>
		private void HideWindow()
		{
			Hide();
		}
		
		
		/// <summary>
		/// Update the system tray icon's tool tip to display the correct number of saved magnets.
		/// </summary>
    	private void UpdateMagnetCountToolTip()
    	{    
    		trayIcon.Text = "Fridge Magnets\n" + "" +
    			"You have " + magnetList.magnetsPanel.Children.Count + " ideas saved.";
    	}
    	
    	
    	/// <summary>
    	/// The system tray lightbulb icon is 'switched on'.
    	/// </summary>
		public void SwitchBulbOn()
		{
			trayIcon.Icon = litIcon;
		}
    	
		
    	/// <summary>
    	/// The system tray lightbulb icon is 'switched off'.
    	/// </summary>
		public void SwitchBulbOff()
		{
			trayIcon.Icon = unlitIcon;
		}    
		
		
    	/// <summary>
    	/// When this method is run, the system tray lightbulb icon will light up, and
    	/// display a balloon tooltip informing the user that their idea has been saved.
    	/// </summary>
    	private void ShowBalloon(string message, int illuminationTime)
    	{
    		trayIcon.ShowBalloonTip(illuminationTime,null,message,ToolTipIcon.None);
    	}
    	
    	#endregion
    	    	
    	#region Event handlers    	

    	private void lightbulbTimer_ReachedZero(object sender, EventArgs e)
    	{
    		SwitchBulbOff();
    	}
    	

    	private void lightbulbTimer_StartedCountdown(object sender, EventArgs e)
    	{
    		SwitchBulbOn();
    	}
    	
    	    	
    	/// <summary>
    	/// Hide the main window when the user attempts to close it.
    	/// </summary>
    	private void HideWindowOnClosing(object sender, CancelEventArgs e)
    	{
			e.Cancel = true; // don't actually close the window - if the user doesn't cancel, then hide it instead
			
    		if (!CloseDialog()) { // return if the user cancels
    			return;
    		}
    		
			System.Windows.Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background,
			                                                          new HideWindowDelegate(HideWindow));
    	}	    	
    	
    	
    	/// <summary>
    	/// Show the main window - regardless of whether it is currently hidden,
    	/// minimised or out of view.
    	/// </summary>
    	private void ShowFridgeMagnets(object sender, EventArgs e)
    	{
    		if (!IsVisible) { // if the window has been 'closed' (actually hidden):
    			Show();
    			WindowState = System.Windows.WindowState.Maximized;
    		}    			    		
    		if (WindowState == System.Windows.WindowState.Minimized) {
    			WindowState = System.Windows.WindowState.Normal;
    		}
    		if (!IsActive) {
    			Activate(); // bring the window into view
    		}    		
    	}
    	
    				
    	/// <summary>
    	/// When the user clicks 'Add a new idea', bring up the Add Idea window.
    	/// </summary>
    	private void AddIdeaFromSystemTray(object sender, EventArgs e)
    	{
        	EditMagnetWindow window = new EditMagnetWindow();
        	window.MagnetCreated += new EventHandler<MagnetEventArgs>(newMagnetCreated);
        	window.ShowDialog();
    	}
    	
    	
    	/// <summary>
    	/// Add the newly created idea to the magnet list, and since the user pulled this up
    	/// from the system tray, display the system tray lightbulb balloon tooltip.
    	/// </summary>
        private void newMagnetCreated(object sender, MagnetEventArgs e)
        {
        	magnetList.AddMagnet(e.Magnet,true);
    		ShowBalloon("Your idea was saved.",10000);
			Log.WriteAction(LogAction.added,"idea",e.Magnet.Idea.ToString() + " ... added from system tray");
        }    
    	    	
    	
    	private void DisplayOptionsScreen(object sender, EventArgs e)
    	{
    		ToolStripMenuItem runOnStartup = (ToolStripMenuItem)sender;
			if (runOnStartup.CheckState == CheckState.Checked) {
				// TODO: add registry key to Run registry, if it's not already there
			}
			else {
				// TODO: if the registry key is present in the Run registry, remove it
			}
    	}
    	    	
    	
    	/// <summary>
    	/// Exits the application.
    	/// </summary>
    	/// <remarks>Use this method rather than Close, since Close is automatically cancelled and
    	/// the window hidden instead. This also tells the app to stop listening for connections.</remarks>
    	private void ForceExit(object sender, EventArgs e)
    	{
    		if (!CloseDialog()) { // return if user has an unsaved board and cancels
    			return;
    		}
    		
    		
    		using (NamedPipeClientStream client = new NamedPipeClientStream(".","magnets",PipeDirection.Out))
    		{
    			try {
    				client.Connect(100);
	    			using (StreamWriter writer = new StreamWriter(client))
	    			{
	    				writer.WriteLine(ABORTMESSAGE);
	    				writer.Flush();
	    			}
    			}
    			catch (TimeoutException) {
    				Console.WriteLine("Couldn't connect to pipe");
    			}
    		}
    		
    		
    		try {
	    		// Serialize the user's preferences:
	    		Serialization.Serialize(FridgeMagnetPreferences.DefaultFridgeMagnetPreferencesPath,
	    		                        FridgeMagnetPreferences.Instance);
    		}
    		catch (Exception ex) {
    			Say.Error("Something went wrong when trying to save your preferences - the choices " +
    				      "you have made may not have been saved.",ex);
    		}
    			
    		Log.WriteAction(LogAction.exited,"magnets");
    		
    		System.Windows.Application.Current.Shutdown();
    	}
    	
    	#endregion    	
	}
}
