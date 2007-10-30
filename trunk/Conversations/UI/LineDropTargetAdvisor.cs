/*
 *   This file is part of Adventure Author.
 *
 *   Adventure Author is copyright Heriot-Watt University 2006-2007.
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
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media.Animation;
using System.Xml;
using Samples.DragDrop;
using AdventureAuthor.Conversations.UI.Controls;
using AdventureAuthor.Utils;

namespace AdventureAuthor.Conversations.UI
{
	public class LineDropTargetAdvisor : IDropTargetAdvisor
	{
		private static DataFormat SupportedFormat = DataFormats.GetDataFormat("ABCFormat");
		private UIElement _targetUI;

		public bool IsValidDataObject(IDataObject obj)
		{
        	Say.Debug("IsValidDataObject");
        	bool isValid = obj.GetDataPresent(SupportedFormat.Name);
        	Say.Debug(isValid.ToString());
			return isValid;
		}

		public void OnDropCompleted(IDataObject obj, Point dropPoint)
		{
        	Say.Debug("OnDropCompleted");
			UIElement elt = ExtractElement(obj);
			
			

			(TargetUI as StackPanel).Children.Add(elt);
			Say.Debug("Dunned OnDropCompleted");
		}

		public UIElement TargetUI
		{
			get
			{
				return _targetUI;
			}
			set
			{
				_targetUI = value;
			}
		}
		public UIElement GetVisualFeedback(IDataObject obj)
		{
        	Say.Debug("GetVisualFeedback");
        	LineControl elt;
        	
        	if (obj is Line) {
        		elt = ExtractElement(obj) as Line;
        	}
        	else if (obj is BranchLine) {
        		elt = ExtractElement(obj) as BranchLine;
        	}
        	else {
        		throw new ArgumentException();
        	}
//			LineControl elt = ExtractElement(obj) as Line;
						
			elt.Width = 350;
			elt.Height = 60;
			elt.Opacity = 0.5;
			elt.IsHitTestVisible = false;

			DoubleAnimation anim = new DoubleAnimation(0.75, new Duration(TimeSpan.FromMilliseconds(500)));
			anim.From = 0.25;
			anim.AutoReverse = true;
			anim.RepeatBehavior = RepeatBehavior.Forever;
			elt.BeginAnimation(UIElement.OpacityProperty, anim);
			Say.Debug("Dunned GetVisualFeedback");

			return elt;
		}

        public Point GetOffsetPoint(IDataObject obj)
        {
        	Say.Debug("GetOffsetPoint");
            Point p = (Point)obj.GetData("point");
            return p;

        }

		private UIElement ExtractElement(IDataObject obj)
		{
        	Say.Debug("ExtractElement");
			string xamlString = obj.GetData("ABCFormat") as string;
			XmlReader reader = XmlReader.Create(new StringReader(xamlString));
			UIElement elt = XamlReader.Load(reader) as UIElement;

			return elt;
		}
	}
}
