/*
 *   This file is part of Adventure Author.
 *
 *   Adventure Author is copyright Heriot-Watt University 2006-2008.
 *
 *   This copyright and licence apply to all source code, compiled code,
 *   documentation, graphics and auxiliary files, except where otherwise stated.
 *
 *   Adventure Author is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 2 of the License, or
 *   (at your option) any later version.
 *
 *   Adventure Author is distributed in the hope that it will be useful,
 *   but WITHOUT ANY WARRANTY; without even the implied warranty of
 *   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *   GNU General Public License for more details.
 * 
 *   Adventure Author is a plugin for Atari's Neverwinter Nights 2, a COMMERCIAL
 *   product. Permission is given to link this GPL-covered plug-in with the 
 *   non-free main program. 
 *
 *   You should have received a copy of the GNU General Public License
 *   along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.Collections;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace AdventureAuthor.Tasks
{
	/// <summary>
	/// Display an appropriate message about an empty task list, depending on whether or
	/// not filters have been applied.
	/// </summary>
	public class GenerateMessageAboutFilters : IMultiValueConverter
	{
		public GenerateMessageAboutFilters()
		{
		}
		
		
		public object Convert(object[] value, Type targetType, object parameter, CultureInfo culture)
		{
			System.Diagnostics.Debug.WriteLine("Converter called");
			if (targetType != typeof(string)) {
				throw new ArgumentException("targetType must be string");
			}
			
			/* I had this checking each filter individually, before realising that I
			 * could just check whether any tasks were open (since the message only
			 * displays when the task list is blank, they must be filtered out.) I've
			 * left it as a multi value converter in case I want to make the message
			 * filter-specific at a later date. */
			
//			bool filteringByStatus = !(bool)value[0]; // cast from Nullable<bool>, and inverted
//			string searchString = (string)value[1];
//			bool filteringByTag = (bool)value[2]; // cast from Nullable<bool>
//			string selectedTag = (string)value[3];
			TaskCollection tasks = (TaskCollection)value[4];
			
			// If the task collection is non-empty, tasks have been filtered out:
			if (tasks != null && tasks.Count > 0) {// && (filteringByStatus || searchString.Length > 0 || (filteringByTag && selectedTag != null))) {
				return "There are no tasks matching what you're looking for.\n\n" +
					   "Click 'Show all' to view your whole task list.";
			}
			else {				
				return "Your task list is empty.\n\n" +
					   "Click the button with the green plus sign to add a task.";
			}
		}
		
		
		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
