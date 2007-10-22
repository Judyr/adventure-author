
using System; 
using System.Windows;

namespace AdventureAuthor.Conversations.UI
{
	public partial class App : Application 
	{
		[STAThread]
		public static void Main() 
		{
			// Initialize and run the application
			App application = new App();
			application.Run();
		}
	}
}
