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

namespace Magnets
{
	/// <summary>
	/// Interaction logic for Window1.xaml
	/// </summary>
	public partial class Window1 : Window
	{
		public static MessageQueue queue = null;
		
		
		public Window1()
		{
			InitializeComponent();
			try {
				 
			
				string queueName = System.IO.Path.Combine(System.Environment.MachineName,"otheronefollows");
				
				if (MessageQueue.Exists(queueName))
				queue = new MessageQueue(queueName);
				else
				queue = MessageQueue.Create(queueName, false);
			}
			catch (Exception e) {
				MessageBox.Show(e.ToString());
			}
		}
		
		
		public void OnClickButton(object sender, EventArgs e)
		{
			queue.Send("This is the message body","I am a message - welcome me");
		}
	}
}