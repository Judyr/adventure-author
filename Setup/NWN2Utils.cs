/*
 * Created by SharpDevelop.
 * User: Kirn
 * Date: 16/05/2008
 * Time: 08:57
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using NWN2Toolset.Data;
using NWN2Toolset.NWN2.Data;
using NWN2Toolset.NWN2.Data.Instances;
using NWN2Toolset.NWN2.Views;
using AdventureAuthor.Utils;

namespace AdventureAuthor.Setup
{
	/// <summary>
	/// Description of NWN2Utils.
	/// </summary>
	public static class NWN2Utils
	{
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
					return "environment";
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
