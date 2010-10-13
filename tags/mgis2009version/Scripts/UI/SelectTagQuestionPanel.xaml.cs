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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AdventureAuthor.Scripts.UI
{
    public partial class SelectTagQuestionPanel : ParameterPanel
    {
    	#region Fields
    	
		public override object Answer {
			get	{
    			if ((bool)radioButtonSelectTag.IsChecked && listBoxTagSelection.SelectedItem != null) {
    				return listBoxTagSelection.SelectedValue;
    			}
    			else if ((bool)radioButtonEnterTag.IsChecked && textBoxTagEntry.Text != String.Empty) {
    				return textBoxTagEntry.Text;
    			}
    			else {
    				return null;
    			}
			}
		}
    	
    	#endregion
    	
    	#region Constructors
    	
        public SelectTagQuestionPanel()
        {
        	
        	
            InitializeComponent();            
            
//            foreach (TaggedType type in Enum.GetValues(typeof(TaggedType))) {
//            	CheckBox checkBox = new CheckBox();
//            	checkBox.Content = type;
//            	objectTypesPanel.Children.Add(checkBox);
//            	
//            	foreach (string tagvalue in ScriptHelper.GetTags(type,currentArea)) {
//            		Tag tagtuple = new Tag();
//            		tagtuple.
//            	}
//            }
            
            
//            ScriptHelper.GetTags(TaggedType.
//            
//            SortedList<string,string> tags = new SortedList<string,string>();
//            foreach (TaggedType type in objectTypes) {
//            	foreach (string tag in ScriptHelper.GetTags(type).Keys) {
//            		if (!tags.ContainsKey(tag)) {
//            			tags.Add(tag,null);
//            		}
//            	}
//            }
//            AnswerBox.ItemsSource = tags.Keys;
        }

        #endregion
    }
}