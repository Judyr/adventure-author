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
    	
    	/// <summary>
    	/// True if editing an existing variable, false otherwise.
    	/// </summary>
    	private bool edit;
    	
    	/// <summary>
    	/// The original value of the variable being edited.
    	/// </summary>
    	private object originalValue = null;    	    	
    	
    	#endregion
    	
        /// <summary>
        /// Create a new instance of the CreateEditVariableWindow, to create or edit an existing variable
        /// </summary>
        /// <param name="variable">The variable to edit></param>
        /// <param name="edit">True if we are editing an existing variable, false if we are creating a new variable></param>
        public CreateEditVariableWindow(ref NWN2ScriptVariable variable, bool edit)
        {
            Resources.Add("vartypes",VARIABLE_TYPES);
            
            InitializeComponent();
            
            this.var = variable;
            this.edit = edit;
            VariableNameTextBox.Text = var.Name;
            	
            if (edit) { // edit an existing variable   
            	if (var.VariableType == NWN2ScriptVariableType.String) {
            		originalValue = var.ValueString; // remember the original value, to change back if the user cancels
            		VariableStartingValueTextBox.Text = var.ValueString;
            		VariableTypeComboBox.SelectedItem = STRING_TERM;
            	}
            	else if (variable.VariableType == NWN2ScriptVariableType.Int) {
            		originalValue = (object)var.ValueInt;
            		VariableStartingValueTextBox.Text = var.ValueInt.ToString();
            		VariableTypeComboBox.SelectedItem = INTEGER_TERM;
            	}
            	else {
            		throw new ArgumentException("Currently you can only add " + STRING_TERM + " and " + INTEGER_TERM + " variables - " 
            		                            + var.VariableType.ToString() + " is invalid.");            		
            	}
            	VariableNameTextBox.IsEnabled = false;  // do not allow users to change an existing variable's name
            	VariableTypeComboBox.IsEnabled = false; // or type, as this will cause problems with existing SetVariable scripts
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
        	else if (!edit && !IsAvailable(VariableNameTextBox.Text)) {
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
        				if (!edit) { // when editing these are fixed anyway
	        				var.Name = VariableNameTextBox.Text;
	        				var.VariableType = NWN2ScriptVariableType.Int;
        				}
        				var.ValueInt = val;
        				this.DialogResult = true;
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
        			if (!edit) { // when editing these are fixed anyway
	 					var.Name = VariableNameTextBox.Text;
	 					var.VariableType = NWN2ScriptVariableType.String;
        			}
 					var.ValueString = VariableStartingValueTextBox.Text;
 					this.DialogResult = true;
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
        	this.DialogResult = false;
        	if (edit) {
        		try {
	        		if (var.VariableType == NWN2ScriptVariableType.String) { // restore string value
        				var.ValueString = (string)originalValue;
	        		}
        			else if (var.VariableType == NWN2ScriptVariableType.Int) { // restore int value
        				var.ValueInt = (int)(object)originalValue;
	        		}
        		}
        		catch (NullReferenceException e)  {
        			throw new ArgumentNullException("Failed to restore the variable's original starting value.",e);
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
        	return Adventure.CurrentAdventure.Module.ModuleInfo.Variables.GetVariable(variableName) == null;
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
