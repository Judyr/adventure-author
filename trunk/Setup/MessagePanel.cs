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
using System.Windows.Threading;
using AdventureAuthor.Utils;

namespace AdventureAuthor.Setup
{
    public partial class MessagePanel : UserControl
    {
    	#region Constants
    	
    	private const byte OLDMESSAGESTORECALL = 3;    	
    	private const double DEFAULTDISPLAYTIME = 5000;
    	
    	#endregion
    	
    	#region Properties and fields
    	
    	protected List<IMessageGenerator> messageGenerators = new List<IMessageGenerator>();
		public List<IMessageGenerator> MessageGenerators {
			get { return messageGenerators; }
		}
    	    	
    	
    	protected List<string> oldMessages = new List<string>(OLDMESSAGESTORECALL);
    	
    	
    	protected object padlock = new object();
    	
    	
    	protected Timer timer = new Timer();
    	
    	
    	protected string defaultMessage;
		public string DefaultMessage {
			get { return defaultMessage; }
			set { defaultMessage = value; }
		}
    	
    	#endregion
    	
    	#region Constructors
    	
        public MessagePanel(string defaultMessage)
        {
        	if (defaultMessage == null) {
        		throw new ArgumentNullException("defaultMessage","defaultMessage cannot be null");
        	}
        	        	
        	// Once you've finished displaying a message, pick another
        	// message to display at random:
        	timer.AutoReset = true;        	
        	timer.Elapsed += MessageTimerElapsed;
        	
            InitializeComponent();            
        	
        	this.defaultMessage = defaultMessage;        	
            SetMessageToDefault();
            
            MouseDoubleClick += delegate { SetMessage("Clicked at " + DateTime.Now.ToShortTimeString() + ".", 
                                                      DEFAULTDISPLAYTIME); };
        }
        
        
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
        
        
        public delegate void SetTextDelegate(object message);
        private void SetText(object message)
        {
        	messageBlock.Text = (string)message;
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
        		if (GetMessage() != DefaultMessage) {
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
        /// Displays a random message from one of the message generators.
        /// </summary>
        public void SetMessageAtRandom()
        {	        
        	SetMessageAtRandom(DEFAULTDISPLAYTIME);
        }
                
        
        protected string GetRandomMessage()
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
        
        
        public string GetMessage()
        {
        	return messageBlock.Text;
        }
        
        #endregion
        
        #region Event handlers
        
        private void MessageTimerElapsed(object sender, ElapsedEventArgs e)
        {
        	SetMessageAtRandom();
        }
        
        #endregion
    }
}