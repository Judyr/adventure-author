/*
 * Created by SharpDevelop.
 * User: Kirn
 * Date: 16/05/2008
 * Time: 08:57
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.IO;
using System.Reflection;
using NWN2Toolset.Data;
using NWN2Toolset.NWN2.Data;
using NWN2Toolset.NWN2.Data.TypedCollections;
using NWN2Toolset.NWN2.Data.Blueprints;
using NWN2Toolset.NWN2.Data.Instances;
using NWN2Toolset.NWN2.Data.Templates;
using NWN2Toolset.NWN2.Views;
using AdventureAuthor.Conversations;
using AdventureAuthor.Utils;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using OEIShared.IO;
using OEIShared.Utils;
using form = NWN2Toolset.NWN2ToolsetMainForm;

namespace AdventureAuthor.Setup
{
	public static class NWN2Utils
	{	
		public static string GetNarrativeVehicleText(NWN2GameArea area, bool onlyText)
		{
			if (area == null) {
				throw new ArgumentNullException("Tried to get narrative vehicle text from a null area.");
			}
			
			area.Demand();
						
			string header = "Area: " + area.Name + System.Environment.NewLine;
			StringBuilder report = new StringBuilder();
			
			foreach (NWN2InstanceCollection collection in area.AllInstances) {				
				foreach (INWN2Instance instance in collection) {
					
					if (instance.Template == null) {
						if (!onlyText) {
							report.Append("(???) " + instance.Name.ToString() + 
							              " (" + instance.ObjectType + "): ");
						} 
						report.Append(((INWN2Object)instance).LocalizedDescription + System.Environment.NewLine);
						if (!onlyText) {
							report.Append("(" + instance.Name + " had a null template.)" + System.Environment.NewLine);
						}
						continue;
					}
									
					// Get the blueprint that this instance was created from:	
					INWN2Blueprint blueprint = NWN2GlobalBlueprintManager.FindBlueprint(instance.ObjectType,
				                                                                    	instance.Template.ResRef);
					if (blueprint == null) {
						if (!onlyText) {
							report.Append("(???) " + instance.Name.ToString() + 
							              " (" + instance.ObjectType + "): ");
						} 
						report.Append(((INWN2Object)instance).LocalizedDescription + System.Environment.NewLine);
						if (!onlyText) {
							report.Append("(" + instance.Name + " blueprint could not be found.)" + System.Environment.NewLine);
						}
						continue;
					}
					
					blueprint.OEIUnserialize(blueprint.Resource.GetStream(false)); // fetch from disk
					
					// Cast to INWN2Object to compare the LocalizedDescription field:
					INWN2Object original = blueprint as INWN2Object;
					INWN2Object copy = instance as INWN2Object;
					string originalDescription = original.LocalizedDescription.ToString();
					string copyDescription = copy.LocalizedDescription.ToString();
					
					if (copyDescription != originalDescription) {
						if (!onlyText) {
							report.Append(instance.Name.ToString() + 
							              " (" + instance.ObjectType + "): ");
						} 
						report.Append(copyDescription + System.Environment.NewLine);
					}
					
					// If you're dealing with Items, also check the LocalizedDescriptionIdentified field:
					if (collection == area.Items) {
						NWN2ItemBlueprint originalItem = blueprint as NWN2ItemBlueprint;
						NWN2ItemInstance copyOfItem = instance as NWN2ItemInstance;	
						string originalIdentifiedDescription = originalItem.LocalizedDescriptionIdentified.ToString();
						string copyIdentifiedDescription = copyOfItem.LocalizedDescriptionIdentified.ToString();					
						if (copyIdentifiedDescription != originalIdentifiedDescription &&
						    copyIdentifiedDescription != copyDescription) {
							if (!onlyText) {
								report.Append(instance.Name.ToString() + 
								              " (" + instance.ObjectType + "): ");
							} 
							report.Append(copyDescription + System.Environment.NewLine);
						}
					}	
				}
			}
			
			area.Release();			
			
			if (report.ToString() == String.Empty) {
				return String.Empty;	
			}
			else {
				if (!onlyText) {
					report.Insert(0,header,1);
				}
				return report.ToString() + System.Environment.NewLine + System.Environment.NewLine;
			}
		}
		
				
		public static string GetNarrativeVehicleText(NWN2GameModule module, bool onlyText)
		{
			string header = "Module: " + module.Name + System.Environment.NewLine;
			StringBuilder report = new StringBuilder();
			
			foreach (NWN2GameArea area in module.Areas.Values) {
				report.Append(GetNarrativeVehicleText(area,onlyText));
			}
			
			if (report.ToString() == String.Empty) {
				return String.Empty;	
			}
			else {
				if (!onlyText) {
					report.Insert(0,header,1);
				}
				return report.ToString() + System.Environment.NewLine + System.Environment.NewLine;
			}
		}
		
		
		public static int GetTotalWordCountOfConversations(NWN2GameModule module)
		{
			int totalWordCount = 0;
			foreach (NWN2GameConversation conv in module.Conversations.Values) {
				conv.Demand();
				Conversation conversation = new Conversation(conv);
				totalWordCount += conversation.GetWordLinePageCounts().Words;
				conv.Release();
			}
			return totalWordCount;
		}
		
		
		public static int GetApproximateTotalWordCountOfNarrativeVehicles(NWN2GameModule module)
		{
			string narrativeVehicleText = GetNarrativeVehicleText(module,true);
			Log.WriteMessage(narrativeVehicleText);
			int count = 0;
			foreach (string s in narrativeVehicleText.Split(new char[]{' ','\n','\r'},StringSplitOptions.RemoveEmptyEntries)) {
				if (s != null && s != String.Empty && s != System.Environment.NewLine && s != "\n" && s != " ") {
					Log.WriteMessage("__"+s+"__");
					count++;	
				}
			}
			return count;
		}
		
		
		public static void WriteTotalWordCountForAllModulesToFile()
		{
			FileInfo fi = new FileInfo(@"C:\To burn\totalwordcounts.txt");
			using (StreamWriter sw = fi.CreateText())
			{				
				sw.AutoFlush = true;
				DirectoryInfo modulesDirectory = new DirectoryInfo(form.ModulesDirectory);
				foreach (DirectoryInfo module in modulesDirectory.GetDirectories()) {
					if (module.Name.StartsWith("temp")) {
						continue;
					}
					form.App.Module.OpenModuleDirectory(module.Name);
					int conversations = GetTotalWordCountOfConversations(form.App.Module);
					int narrativeVehicles = GetApproximateTotalWordCountOfNarrativeVehicles(form.App.Module);
					int total = conversations + narrativeVehicles;
					sw.WriteLine();
					sw.WriteLine(" ** " + form.App.Module.Name + " ** ");
					sw.WriteLine("Word count (conversations): " + conversations);
					sw.WriteLine("Word count (descriptive text): " + narrativeVehicles);
					sw.WriteLine("Total: " + total);
				}
			}
		}
		
		
		public static void WritePropertyChangeToLog(NWN2PropertyValueChangedEventArgs e)
		{
			foreach (object o in e.ChangedObjects) {
				string name = GetNWN2Name(o);
				string type = GetNWN2TypeName(o);
				string message;
				if (e.PropertyName.ToLower() == "tag") {
					message = "'" + e.PropertyName + "' on " + type + " '" + 
						e.OldValue + "' to " + e.NewValue + " (was " + e.OldValue + ")";
				}
				else {
					message = "'" + e.PropertyName + "' on " + type + " '" + 
						name + "' to " + e.NewValue + " (was " + e.OldValue + ")";
				}
				
				Log.WriteAction(LogAction.set,"property",message);
			}
		}
		
		
		public static string GetNWN2Name(object o)
		{
			IGameArea area = o as IGameArea;
			IGameModule module = o as IGameModule;
			INWN2Instance instance = o as INWN2Instance;
			if (area != null) {
				return area.Name;
			}
			else if (module != null) {
				return module.Name;
			}
			else if (instance != null) {
				return instance.Name;				
			}
			else {
				return "<no name>";
			}
		}
		
		
		public static string GetNWN2TypeName(object o)
		{
			if (o is IGameArea) {
				return "area";
			}
			else if (o is IGameConversation) {
				return "conversation";
			}
			else if (o is IGameModule) {
				return "module";
			}
			else if (o is IGameScript) {
				return "script";
			}
			else if (o is INWN2Instance) {
				if (o is NWN2CreatureInstance) {
					return "creature";
				}
				else if (o is NWN2DoorInstance) {
					return "door";
				}
				else if (o is NWN2EncounterInstance) {
					return "encounter";
				}
				else if (o is NWN2EnvironmentInstance) {
					return "environmentobject";
				}
				else if (o is NWN2ItemInstance) {
					return "item";
				}
				else if (o is NWN2LightInstance) {
					return "light";
				}
				else if (o is NWN2PlaceableInstance) {
					return "placeable";
				}
				else if (o is NWN2PlacedEffectInstance) {
					return "placedeffect";
				}
				else if (o is NWN2SoundInstance) {
					return "sound";
				}
				else if (o is NWN2StaticCameraInstance) {
					return "camera";
				}
				else if (o is NWN2StoreInstance) {
					return "store";
				}
				else if (o is NWN2TreeInstance) {
					return "tree";
				}
				else if (o is NWN2TriggerInstance) {
					return "trigger";
				}
				else if (o is NWN2WaypointInstance) {
					return "waypoint";
				}
				else {
					return o.GetType().ToString();
				}
			}
			else if (o is NWN2GameAreaTileData) {
				return "tile";
			}
			else {
				return o.GetType().ToString();
			}
		}
	}
}
