/*
 * Created by SharpDevelop.
 * User: Kirn
 * Date: 31/03/2008
 * Time: 10:59
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using NWN2Toolset.NWN2.Data.Blueprints;
using NWN2Toolset.NWN2.Data.Templates;
using OEIShared.Utils;
using AdventureAuthor.Utils;

namespace AdventureAuthor.Ideas
{
	/// <summary>
	/// Description of BlueprintMagnetControl.
	/// </summary>
	public class BlueprintMagnetControl : MagnetControl
	{
		protected string blueprintIdentifier;
		public string BlueprintIdentifier {
			get { return blueprintIdentifier; }
			set { 
				blueprintIdentifier = value; 
				resRefTextBlock.Text = blueprintIdentifier;
			}
		}
		
		
		protected TextBlock resRefTextBlock = null;
		
		
		protected BlueprintMagnetControl() : base() 
		{
			resRefTextBlock = new TextBlock();
			resRefTextBlock.Name = "resrefTextBlock";
			resRefTextBlock.FontStyle = FontStyles.Italic;
			resRefTextBlock.ToolTip = "This is the resref - a special name\n" +
				"that uniquely identifies this particular blueprint.";
			resRefTextBlock.Margin = new Thickness(2);
			resRefTextBlock.HorizontalAlignment = HorizontalAlignment.Center;
			magnetMainStackPanel.Children.Insert(0,resRefTextBlock);
		}
		
		
		public BlueprintMagnetControl(INWN2Blueprint blueprint) : this()
		{
			RepresentBlueprint(blueprint);
		}
		
		
		public BlueprintMagnetControl(BlueprintMagnetControlInfo magnetInfo) : this()
		{
    		X = magnetInfo.X;
    		Y = magnetInfo.Y;
    		Angle = magnetInfo.Angle;
    		Idea = magnetInfo.Idea;
			BlueprintIdentifier = magnetInfo.BlueprintIdentifier;
			
			InvalidateVisual();
		}
		
        
		/// <summary>
		/// Get a serializable object representing serializable data in an unserializable control.
		/// </summary>
		/// <returns>A serializable object</returns>
    	public override ISerializableData GetSerializable()
    	{
    		return new BlueprintMagnetControlInfo(this);
    	}
        
        
        /// <summary>
        /// Get an identical copy of this MagnetControl.
        /// </summary>
        /// <returns>An identical copy of this MagnetControl</returns>
		public override object Clone()
		{
			BlueprintMagnetControlInfo info = (BlueprintMagnetControlInfo)GetSerializable();
			info.Idea = (Idea)info.Idea.Clone();
			return new BlueprintMagnetControl(info);
		}
		
		
		public void RepresentBlueprint(INWN2Blueprint blueprint)
		{
			try {				 
				blueprint.OEIUnserialize(blueprint.Resource.GetStream(false)); // fetch from disk
				
				if (blueprint is NWN2EncounterBlueprint) {
					NWN2EncounterBlueprint encounter = (NWN2EncounterBlueprint)blueprint;
					BlueprintIdentifier = encounter.Tag;
				}
				else if (blueprint is NWN2StoreBlueprint) {
					NWN2StoreBlueprint store = (NWN2StoreBlueprint)blueprint;
					BlueprintIdentifier = store.Tag;
				}
				else {
					BlueprintIdentifier = blueprint.TemplateResRef.Value;
				}				
				
				string magnetText = GetTextForBlueprintMagnet(blueprint);
				if (Idea == null) {
					Idea = new Idea(magnetText,IdeaCategory.Toolset,User.GetCurrentUserName(),DateTime.Now);
				}		
				else {
					Text = magnetText;
				}
				
				blueprint.Resource.Release();
			}
			catch (Exception e) {
				Say.Error("Failed to make this magnet represent a blueprint.",e);
			}
		}
				
		
//		/// <summary>
//		/// 
//		/// </summary>
//		/// <param name="obj">A game object</param>
//		/// <returns></returns>
//		private static string GetTagOrResref(INWN2Object obj)
//		{
//			return "tag: " + obj.Tag;
//		}
//		
//		
//		/// <summary>
//		/// 
//		/// </summary>
//		/// <param name="blueprint">A game blueprint</param>
//		/// <returns></returns>
//		private static string GetTagOrResref(INWN2Blueprint blueprint)
//		{
//			return "resref: " + blueprint.TemplateResRef.Value;
//		}
		
		
		/// <summary>
		/// Get a piece of text that appropriately describes a given blueprint,
		/// for use in creating a blueprint magnet.
		/// </summary>
		/// <param name="blueprint">The blueprint to describe</param>
		/// <returns>A description of a given blueprint</returns>
		private static string GetTextForBlueprintMagnet(INWN2Blueprint blueprint)
		{
			string text;
			
			if (blueprint is NWN2CreatureTemplate) {
				NWN2CreatureTemplate creature = (NWN2CreatureTemplate)blueprint;
				text = //"resref: " + blueprint.TemplateResRef.Value + "\n" +					
					GetNameForBlueprintMagnet(creature.FirstName + " " + creature.LastName) + " (" + 
					"Creature" + ")";
			}
			else if (blueprint is NWN2DoorTemplate) {
				NWN2DoorTemplate door = (NWN2DoorTemplate)blueprint;
				text = //"resref: " + blueprint.TemplateResRef.Value + "\n" +
					GetNameForBlueprintMagnet(door.LocalizedName) + " (" + 
					"Door" + ")";
			}
			else if (blueprint is NWN2EncounterTemplate) { // for some reason encounters have a Tag rather than Resref
				NWN2EncounterTemplate encounter = (NWN2EncounterTemplate)blueprint;
				text = //"tag: " + encounter.Tag + "\n" +
					GetNameForBlueprintMagnet(encounter.LocalizedName) + " (" + 
					"Encounter" + ")";
			}
			else if (blueprint is NWN2EnvironmentTemplate) {
				NWN2EnvironmentTemplate environment = (NWN2EnvironmentTemplate)blueprint;
				text = //"resref: " + blueprint.TemplateResRef.Value + "\n" +
					GetNameForBlueprintMagnet(environment.LocalizedName) + " (" + 
					"Environment" + ")";
			}
			else if (blueprint is NWN2ItemTemplate) {
				NWN2ItemTemplate item = (NWN2ItemTemplate)blueprint;
				text = //"resref: " + blueprint.TemplateResRef.Value + "\n" +
					GetNameForBlueprintMagnet(item.LocalizedName) + " (" + 
					"Item" + ")";
			}
			else if (blueprint is NWN2LightTemplate) {
				NWN2LightTemplate light = (NWN2LightTemplate)blueprint;
				text = //"resref: " + blueprint.TemplateResRef.Value + "\n" +
					GetNameForBlueprintMagnet(light.LocalizedName) + " (" + 
					"Light" + ")";
			}
			else if (blueprint is NWN2PlaceableTemplate) {
				NWN2PlaceableTemplate placeable = (NWN2PlaceableTemplate)blueprint;
				text = //"resref: " + blueprint.TemplateResRef.Value + "\n" +
					GetNameForBlueprintMagnet(placeable.LocalizedName) + " (" + 
					"Placeable" + ")";
			}
			else if (blueprint is NWN2PlacedEffectTemplate) {
				NWN2PlacedEffectTemplate effect = (NWN2PlacedEffectTemplate)blueprint;
				text = //"resref: " + blueprint.TemplateResRef.Value + "\n" +
					GetNameForBlueprintMagnet(effect.LocalizedName) + " (" + 
					"Effect" + ")";
			}
			else if (blueprint is NWN2SoundTemplate) {
				NWN2SoundTemplate sound = (NWN2SoundTemplate)blueprint;
				text = //"resref: " + blueprint.TemplateResRef.Value + "\n" +
					GetNameForBlueprintMagnet(sound.LocalizedName) + " (" + 
					"Sound" + ")";
			}
			else if (blueprint is NWN2StaticCameraTemplate) {
				NWN2StaticCameraTemplate camera = (NWN2StaticCameraTemplate)blueprint;
				text = //"resref: " + blueprint.TemplateResRef.Value + "\n" +
					GetNameForBlueprintMagnet(camera.LocalizedName) + " (" + 
					"Camera" + ")";
			}
			else if (blueprint is NWN2StoreTemplate) { // for some reason stores have a Tag rather than Resref
				NWN2StoreTemplate store = (NWN2StoreTemplate)blueprint;
				text = //"tag: " + store.Tag + "\n" +
					GetNameForBlueprintMagnet(store.LocalizedName) + " (" + 
					"Store" + ")";
			}
			else if (blueprint is NWN2TreeTemplate) { // trees are better described by Comment (though this is inconsistent)
				//NWN2TreeTemplate tree = (NWN2TreeTemplate)blueprint;
				text = //"resref: " + blueprint.TemplateResRef.Value + "\n" +
					GetNameForBlueprintMagnet(blueprint.Comment) + " (" + 
					"Tree" + ")";
			}
			else if (blueprint is NWN2TriggerTemplate) {
				NWN2TriggerTemplate trigger = (NWN2TriggerTemplate)blueprint;
				text = //"resref: " + blueprint.TemplateResRef.Value + "\n" +
					GetNameForBlueprintMagnet(trigger.LocalizedName) + " (" + 
					"Trigger" + ")";
			}
			else if (blueprint is NWN2WaypointTemplate) {
				NWN2WaypointTemplate waypoint = (NWN2WaypointTemplate)blueprint;
				text = //"resref: " + blueprint.TemplateResRef.Value + "\n" +
					GetNameForBlueprintMagnet(waypoint.LocalizedName) + " (" + 
					"Waypoint" + ")";
			}
			else {
				text = "No description";
			}
				
			return text;
		}
		
		
		/// <summary>
		/// Returns a version of a blueprint name appropriate for a magnet, 
		/// based on the original blueprint name. The quotation marks which 
		/// surround placeable blueprint names are stripped,
		/// and a null or blank name will become 'Unnamed'.
		/// </summary>
		/// <param name="name">The name of the blueprint</param>
		/// <returns>The name to give the magnet</returns>
		private static string GetNameForBlueprintMagnet(OEIExoLocString name)
		{
			return GetNameForBlueprintMagnet(name.ToString());
		}
		
		
		/// <summary>
		/// Returns a version of a blueprint name appropriate for a magnet, 
		/// based on the original blueprint name. The quotation marks which 
		/// surround placeable blueprint names are stripped,
		/// and a null or blank name will become 'Unnamed'.
		/// </summary>
		/// <param name="name">The name of the blueprint</param>
		/// <returns>The name to give the magnet</returns>
		private static string GetNameForBlueprintMagnet(string name)
		{
			if (name == null || name == String.Empty) {
				return "Unnamed";
			}
			else {
				for (int i = 0; i < name.Length; i++) {
					if (name[i] != ' ') {	
						// Placeable names start with '"' and end with '", ' 
						// so remove them if you find them:
						string originalName = name;
						try {
							if (name.StartsWith("\"") && name.EndsWith("\", ")) {
								name = name.Substring(1,name.Length-4);
							}				
						}
						catch (Exception e) {
							Say.Debug("There was an error when trying to strip extraneous characters " +
							          "from a placeable name for a magnet.\n" + e);
							name = originalName;
						}
						return name;
					}
				}
				return "Unnamed";
			}
		}
	}
}
