using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Timers;
using System.Reflection;
using System.Resources;
using System.Windows.Threading;
using AdventureAuthor.Utils;

namespace AdventureAuthor.Setup
{
	/// <summary>
	/// A panel that cycles through messages to display to the user.
	/// </summary>
    public partial class MessagePanel : UserControl
    {
    	#region Constants
    	
    	/// <summary>
    	/// The number of past messages available to show the user.
    	/// </summary>
    	public const byte OLDMESSAGESTORECALL = 3;    	
    	
    	/// <summary>
    	/// The default amount of time to display a message for, in milliseconds.
    	/// </summary>
    	public const double DEFAULTDISPLAYTIME = 5000;
    	
    	#endregion
    	
    	#region Properties and fields
    	
    	protected List<IMessageGenerator> messageGenerators = new List<IMessageGenerator>();
		/// <summary>
		/// The message generators used to generate random messages.
		/// </summary>
    	public List<IMessageGenerator> MessageGenerators {
			get { return messageGenerators; }
		}
    	    	
    	
		/// <summary>
		/// A list of previously displayed messages.
		/// </summary>
    	protected List<string> oldMessages = new List<string>(OLDMESSAGESTORECALL);
    	
    	
    	/// <summary>
    	/// Used for locking.
    	/// </summary>
    	protected object padlock = new object();
    	
    	
    	/// <summary>
    	/// A timer used to decide when to display a new message.
    	/// </summary>
    	protected Timer timer = new Timer();
    	
    	
    	/// <summary>
    	/// The picture to display next to this message panel.
    	/// </summary>
    	public Image Picture {
    		get { return picture; }
    		set { picture = value; }
    	}
    	
    	
    	protected string defaultMessage;
    	/// <summary>
    	/// The default message to use when
    	/// there are no other messages to display.
    	/// </summary>
		public string DefaultMessage {
			get { return defaultMessage; }
			set { defaultMessage = value; }
		}
    	
    	
    	protected Delegate hyperlinkMethod = null;
    	
    	#endregion
    	
    	#region Constructors
    	
    	/// <summary>
    	/// A panel that cycles through messages to display to the user.
    	/// </summary>
    	/// <param name="defaultMessage">The default message to use when
    	/// there are no other messages to display.</param>
        public MessagePanel(string defaultMessage)
        {      
            if (defaultMessage == null) {
            	this.defaultMessage = String.Empty;
            }
            else {
            	this.defaultMessage = defaultMessage;
            }
        	
        	// Once you've finished displaying a message, pick another
        	// message to display at random:
        	timer.AutoReset = true;        	
        	timer.Elapsed += MessageTimerElapsed;
        	
            InitializeComponent();
            
            SetMessageToDefault();  
            
            ResourceManager manager = new ResourceManager("AdventureAuthor.Utils.images",
                                                          Assembly.GetAssembly(typeof(EditableTextBox)));
           
            using (System.Drawing.Bitmap bitmap = (System.Drawing.Bitmap)manager.GetObject("aalogo")) {
	           	BitmapSource source = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
																			 bitmap.GetHbitmap(),
						                                                     IntPtr.Zero,
				                                                             Int32Rect.Empty,
				                                                             BitmapSizeOptions.FromEmptyOptions());            	
           		Picture.Source = source;
            }
        }
        
        
    	/// <summary>
    	/// A panel that cycles through messages to display to the user.
    	/// </summary>
        public MessagePanel() : this(String.Empty)
        {        	
        }
        
        #endregion
        
        #region Methods
        
        /// <summary>
        /// Display a message for some length of time.
        /// </summary>
        /// <param name="message">The message to display.</param>
        /// <param name="displayTime">The time to display the message for in milliseconds.</param>
        /// <remarks>If a new message is received, the display of this message will be cut short.</remarks>
        public void SetMessage(string message, double displayTime)
        {        	
        	lock (padlock) {
        		if (timer.Enabled) {
        			timer.Stop();
        		}
        		timer.Interval = displayTime;
        		Dispatcher.Invoke(DispatcherPriority.Normal,
        	                  	  new SetTextDelegate(SetText),message);
        		timer.Start();
        	}
        }
        
        
        /// <summary>
        /// Display a message for some length of time.
        /// </summary>
        /// <param name="message">The message to display.</param>
        public void SetMessage(string message)
        {        	
        	SetMessage(message,DEFAULTDISPLAYTIME);
        }
        
        
        /// <summary>
        ///  Set the text to display to the user.
        /// </summary>
        protected delegate void SetTextDelegate(object message);
        
                
        /// <summary>
        /// Set the text to display to the user.
        /// </summary>
        /// <param name="message">The message to display.</param>
        protected void SetText(object message)
        {
        	if (!(message is string)) {
        		throw new ArgumentException("message","message must be a string.");
        	}
        	messageBlock.Text = (string)message;
        }
        
        
        /// <summary>
        /// Display the default message. Used when there is currently
        /// no other message to display. 
        /// </summary>
        /// <remarks>The default message will continue to display until
        /// another message is set.</remarks>
        public void SetMessageToDefault()
        {
        	lock (padlock) {
        		if (timer.Enabled) {
        			timer.Stop();
        		}        		
        		if (GetCurrentMessage() != DefaultMessage) {
	        		Dispatcher.Invoke(DispatcherPriority.Normal,
	        	                  	  new SetTextDelegate(SetText),DefaultMessage);
        		}
        	}
        }
        
        
        /// <summary>
        /// Displays a random message from one of the message generators.
        /// </summary>
        /// <param name="displayTime">The time to display the message for in milliseconds.</param>
        public void SetMessageAtRandom(double displayTime)
        {	    
        	string message = GetRandomMessage();
        	
        	if (message == null || message == String.Empty) {
        		SetMessageToDefault();
        	}
        	else {
        		SetMessage(message,displayTime);
        	}
        }
                
        
        /// <summary>
        /// Displays a randomly chosen message from one of the message generators.
        /// </summary>
        public void SetMessageAtRandom()
        {	        
        	SetMessageAtRandom(DEFAULTDISPLAYTIME);
        }
                
        
        /// <summary>
        /// Randomly select a message generator and return a message from it.
        /// </summary>
        /// <returns>A randomly selected message.</returns>
        public string GetRandomMessage()
        {
        	IMessageGenerator messageGenerator;
        	lock (padlock) {
	        	if (messageGenerators.Count == 0) {
	        		return String.Empty;
	        	}
	        	else {
        			Random random = new Random();
        			int index = random.Next(0,messageGenerators.Count-1);
        			messageGenerator = messageGenerators[index];
	        	}
        	}
        	return messageGenerator.GetMessage();
        }
        
        
        /// <summary>
        /// Get the current message being displayed.
        /// </summary>
        /// <returns>The message currently being displayed.</returns>
        public string GetCurrentMessage()
        {
        	return messageBlock.Text;
        }
        
        #endregion
        
        #region Event handlers
        
        /// <summary>
        /// Once a message has been displayed for a certain amount of time,
        /// display another message at random.
        /// </summary>
        private void MessageTimerElapsed(object sender, ElapsedEventArgs e)
        {
        	SetMessageAtRandom();
        }
        
        
        private void RunHyperlinkMethod(object sender, MouseEventArgs e)
        {
        	lock (padlock) {
	        	if (hyperlinkMethod != null) {
	        		//hyperlinkMethod.Method.Invoke
	        	}
        	}
        }
        
        #endregion
    }
}