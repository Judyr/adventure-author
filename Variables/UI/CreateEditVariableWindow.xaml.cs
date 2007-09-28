using System;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using NWN2Toolset.NWN2.Data;
using AdventureAuthor.Core;
using AdventureAuthor.Utils;

namespace AdventureAuthor.Variables.UI
{
    public partial class CreateEditVariableWindow : Window
    {
    	#region Constants
    	
    	private const string STRING_TERM = "String";
    	private const string INTEGER_TERM = "Integer";
    	private readonly string[] VARIABLE_TYPES = new string[]{STRING_TERM,INTEGER_TERM};
    	
    	#endregion
    		
    	#region Fields
    	
    	/// <summary>
    	/// The variable the window is exposing.
    	/// </summary>
    	private NWN2ScriptVariable var = null;    	
    	
    	private string originalName = null;
    	private string originalStringValue = null;
    	private int? originalIntValue = null;
    	
    	#endregion
    	
        /// <summary>
        /// Create a new instance of the CreateEditVariableWindow, to create or edit an existing variable
        /// </summary>
        /// <param name="variable">The variable to edit, or null to create a new variable</param>
        public CreateEditVariableWindow(ref NWN2ScriptVariable variable)
        {
            Resources.Add("vartypes",VARIABLE_TYPES);
            
            InitializeComponent();
            
            if (variable == null) {
            	var = new NWN2ScriptVariable();
            }
            else {
            	var = variable; 
            	originalName = variable.Name;            	
            	VariableNameTextBox.Text = var.Name;
            	VariableTypeComboBox.IsEnabled = false;
            	if (variable.VariableType == NWN2ScriptVariableType.String) {
            		originalStringValue = variable.ValueString;
            		VariableTypeComboBox.SelectedItem = STRING_TERM;
            		if (var.ValueString != null) {
            			VariableStartingValueTextBox.Text = var.ValueString;
            		}
            	}
            	else if (variable.VariableType == NWN2ScriptVariableType.Int) {
            		originalIntValue = variable.ValueInt;
            		VariableTypeComboBox.SelectedItem = INTEGER_TERM;
            		VariableStartingValueTextBox.Text = var.ValueInt.ToString();
            	}
            	else {
            		throw new ArgumentException("Currently you can only add " + STRING_TERM + " and " + INTEGER_TERM + " variables - " 
            		                            + var.VariableType.ToString() + " is invalid.");            		
            	}  
            }
        }
        
        /// <summary>
        /// Close the variable operation, returning a variable object.
        /// </summary>
        private void OnClick_OK(object sender, EventArgs ea)
        {
        	if (VariableNameTextBox.Text == String.Empty) {
        		Say.Warning("You haven't given this variable a name.");
        	}
        	else if (!IsValid(VariableNameTextBox.Text)) {
        		Say.Warning("You have chosen an invalid variable name. Names must be under " + Adventure.MAX_RESOURCE_NAME_LENGTH + 
        		            " characters long, and contain none of the following characters: \" < > | : * ? " + @"\ / ");
        	}
        	else if ((originalName == null && !IsAvailable(VariableNameTextBox.Text)) || // TODO reword when less tired
        	         (originalName != null && originalName != VariableNameTextBox.Text && !IsAvailable(VariableNameTextBox.Text))) {
        		Say.Warning("A variable called " + VariableNameTextBox.Text + " already exists - try something else.");
        	}
        	else if (VariableTypeComboBox.SelectedItem == null) {
        		Say.Warning("You haven't selected what type of variable you're creating.");
        	}
        	else if (VariableStartingValueTextBox.Text == String.Empty) {
        		Say.Warning("You haven't entered the starting value for this variable.");
        	}
        	else {
        		if ((string)VariableTypeComboBox.SelectedItem == INTEGER_TERM) {
        			try {
        				int val = int.Parse(VariableStartingValueTextBox.Text);
        				var.Name = VariableNameTextBox.Text;
        				var.VariableType = NWN2ScriptVariableType.Int;
        				var.ValueInt = val;
        				Close();
        			}
        			catch (FormatException) {
        				Say.Warning("You didn't enter a valid number for the starting value.");
        			}
        			catch (OverflowException) {
        				Say.Warning("The number you entered was too big! Enter a number between " 
        				            + int.MinValue + " and " + int.MaxValue + ".");
        			}
        		}
        		else if ((string)VariableTypeComboBox.SelectedItem == STRING_TERM) {
 					var.Name = VariableNameTextBox.Text;
 					var.VariableType = NWN2ScriptVariableType.String;
 					var.ValueString = VariableStartingValueTextBox.Text;
 					Close();
        		}
            	else {
            		throw new ArgumentException("Currently you can only add " + STRING_TERM + " and " + INTEGER_TERM + " variables - " 
            		                            + var.VariableType.ToString() + " is invalid.");            		
            	}
        	}
        }
        
        /// <summary>
        /// Cancel the variable operation. If editing a variable, restore the initial values of that variable before exiting.
        /// </summary>
        private void OnClick_Cancel(object sender, EventArgs ea)
        {
        	if (originalName != null) {
        		var.Name = originalName;
        		if (originalStringValue != null) {
        			var.ValueString = originalStringValue;
        			var.VariableType = NWN2ScriptVariableType.String;
        		}
        		else if (originalIntValue != null) {
        			var.ValueInt = (int)originalIntValue;
        			var.VariableType = NWN2ScriptVariableType.Int;
        		}
        		else {
        			throw new ArgumentNullException("Failed to record the initial state of the variable - could not restore " +
        			                                "the original values upon cancellation.");
        		}
        	}
        	Close();
        }
        
        /// <summary>
        /// Check whether this variable name is taken.
        /// </summary>
        /// <param name="variableName">The variable name to check for</param>
        /// <returns>True if the name is available, false if there is already a variable by that name</returns>
        private bool IsAvailable(string variableName)
        {
        	foreach (NWN2ScriptVariable variable in Adventure.CurrentAdventure.Module.ModuleInfo.Variables) {
        		if (variable.Name == variableName) {
        			return false;
        		}
        	}
        	return true;
        }
        
        /// <summary>
        /// Check whether this is a valid name for a variable, based on length and the absence of invalid characters.
        /// </summary>
        /// <param name="variableName">The variable name to check</param>
        /// <returns>True if this is a valid name, false otherwise</returns>
        private bool IsValid(string variableName)
        {
        	foreach (char c in Path.GetInvalidFileNameChars()) {
        		if (variableName.Contains(c.ToString())) {
        			return false;
        		}
        	}
        	return variableName.Length <= Adventure.MAX_RESOURCE_NAME_LENGTH;
        }
    }
}
