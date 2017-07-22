﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NLua;

namespace MapLua
{
	public partial class SaveLua
	{

		#region Marker
		//[System.Serializable]
		public class Marker
		{
			public string Name = "";
			public MarkerTypes MarkerType;
			public MarkerLayers MarkerLayer;
			public Markers.MarkerObject MarkerObj;
			public List<Chain> ConnectedToChains;

			public float size = 1f;
			public bool resource = false;
			public float amount = 100;
			public string color = "ff808080";
			public string type = "";
			public string prop = "";
			public Vector3 orientation = Vector3.zero;
			public Vector3 position = Vector3.zero;
			public string editorIcon = "";
			public bool hint;

			public string graph = "";
			public string adjacentTo = "";
			public List<Marker> AdjacentToMarker = new List<Marker>();

			public float zoom = 30;
			public bool canSetCamera = true;
			public bool canSyncCamera = true;


			public const string KEY_SIZE = "size";
			public const string KEY_RESOURCE = "resource";
			public const string KEY_AMOUNT = "amount";
			public const string KEY_COLOR = "color";
			public const string KEY_TYPE = "type";
			public const string KEY_PROP = "prop";
			public const string KEY_ORIENTATION = "orientation";
			public const string KEY_POSITION = "position";

			public const string KEY_EDITORICON = "editorIcon";
			public const string KEY_HINT = "hint";

			public const string KEY_ZOOM = "zoom";
			public const string KEY_CANSETCAMERA = "canSetCamera";
			public const string KEY_CANSYNCCAMERA = "canSyncCamera";

			public const string KEY_GRAPH = "graph";
			public const string KEY_ADJACENTTO = "adjacentTo";

			public enum MarkerTypes
			{
				None,
				Mass, Hydrocarbon, BlankMarker, CameraInfo,
				CombatZone,
				DefensivePoint, NavalDefensivePoint,
				ProtectedExperimentalConstruction,
				ExpansionArea, LargeExpansionArea, NavalArea,
				RallyPoint, NavalRallyPoint,
				LandPathNode, AirPathNode, WaterPathNode, AmphibiousPathNode,
				NavalLink,
				TransportMarker,
				Island,
				Count
			}

			public enum MarkerLayers
			{
				All, NoAI, Land, Air, Naval, AnyPath, Other
			}

			public static string MarkerTypeToString(MarkerTypes MType)
			{
				string str1 = MType.ToString();
				string newstring = "";
				for (int i = 0; i < str1.Length; i++)
				{
					if (i > 0 && char.IsUpper(str1[i]))
						newstring += " ";
					newstring += str1[i].ToString();
				}
				return newstring;
			}

			public bool AllowByType(string Key)
			{
				if (MarkerType == MarkerTypes.Mass)
					return Key == KEY_SIZE || Key == KEY_RESOURCE || Key == KEY_AMOUNT || Key == KEY_EDITORICON;
				else if (MarkerType == MarkerTypes.Hydrocarbon)
					return Key == KEY_SIZE || Key == KEY_RESOURCE || Key == KEY_AMOUNT;
				else if (MarkerType == MarkerTypes.BlankMarker)
					return false;
				else if (MarkerType == MarkerTypes.LandPathNode || MarkerType == MarkerTypes.AirPathNode || MarkerType == MarkerTypes.WaterPathNode || MarkerType == MarkerTypes.AmphibiousPathNode)
					return Key == KEY_HINT || Key == KEY_GRAPH || Key == KEY_ADJACENTTO;
				else if (MarkerType == MarkerTypes.NavalLink)
					return false;
				else if (MarkerType == MarkerTypes.CameraInfo)
					return Key == KEY_ZOOM || Key == KEY_CANSETCAMERA || Key == KEY_CANSYNCCAMERA;
				else //Unknown
					return Key == KEY_HINT;
			}

			public MarkerLayers LayerByType(MarkerTypes Type)
			{
				if (Type == MarkerTypes.BlankMarker || Type == MarkerTypes.Mass || Type == MarkerTypes.Hydrocarbon || Type == MarkerTypes.CameraInfo)
					return MarkerLayers.NoAI;
				else if (Type == MarkerTypes.LandPathNode || Type == MarkerTypes.RallyPoint || Type == MarkerTypes.AmphibiousPathNode)
					return MarkerLayers.Land;
				else if (Type == MarkerTypes.WaterPathNode || Type == MarkerTypes.NavalRallyPoint || Type == MarkerTypes.NavalLink)
					return MarkerLayers.Naval;
				else if (Type == MarkerTypes.AirPathNode)
					return MarkerLayers.Air;
				else
					return MarkerLayers.Other;
			}

			public Marker()
			{
			}

			public Marker(MarkerTypes Type)
			{
				ConnectedToChains = new List<Chain>();
				AdjacentToMarker = new List<Marker>();

				Name = GetLowestName(Type);
				RegisterMarkerName(Name);
				size = 1;
				position = Vector3.zero;
				orientation = Vector3.zero;
				prop = "/env/common/props/markers/M_Blank_prop.bp";

				MarkerType = Type;
				type = MarkerTypeToString(Type);

				if (Type == MarkerTypes.Mass)
				{
					resource = true;
					amount = 100;
					prop = "/env/common/props/markers/M_Mass_prop.bp";
					color = "ff808080";
				}
				else if (Type == MarkerTypes.Hydrocarbon)
				{
					size = 3;
					resource = true;
					amount = 100;
					prop = "/env/common/props/markers/M_Hydrocarbon_prop.bp";
					color = "ff808080";
				}
				else if (Type == MarkerTypes.CameraInfo)
				{
					canSyncCamera = true;
					canSetCamera = true;
					zoom = 30;
				}
			}

			public Marker(string name, LuaTable Table)
			{
				// Create marker from table
				Name = name;
				RegisterMarkerName(Name);
				ConnectedToChains = new List<Chain>();
				AdjacentToMarker = new List<Marker>();
				string[] Keys = LuaParser.Read.GetTableKeys(Table);

				for (int k = 0; k < Keys.Length; k++)
				{
					switch (Keys[k])
					{
						case KEY_POSITION:
							position = LuaParser.Read.Vector3FromTable(Table, KEY_POSITION);
							break;
						case KEY_ORIENTATION:
							orientation = LuaParser.Read.Vector3FromTable(Table, KEY_ORIENTATION);
							break;
						case KEY_SIZE:
							size = LuaParser.Read.FloatFromTable(Table, KEY_SIZE);
							break;
						case KEY_RESOURCE:
							resource = LuaParser.Read.BoolFromTable(Table, KEY_RESOURCE);
							break;
						case KEY_AMOUNT:
							amount = LuaParser.Read.FloatFromTable(Table, KEY_AMOUNT);
							break;
						case KEY_COLOR:
							color = LuaParser.Read.StringFromTable(Table, KEY_COLOR);
							break;
						case KEY_TYPE:
							type = LuaParser.Read.StringFromTable(Table, KEY_TYPE);
							break;
						case KEY_PROP:
							prop = LuaParser.Read.StringFromTable(Table, KEY_PROP);
							break;
						case KEY_EDITORICON:
							editorIcon = LuaParser.Read.StringFromTable(Table, KEY_EDITORICON);
							break;
						case KEY_HINT:
							hint = LuaParser.Read.BoolFromTable(Table, KEY_HINT);
							break;
						case KEY_ZOOM:
							zoom = LuaParser.Read.FloatFromTable(Table, KEY_ZOOM);
							break;
						case KEY_ADJACENTTO:
							adjacentTo = LuaParser.Read.StringFromTable(Table, KEY_ADJACENTTO);
							break;
						case KEY_CANSETCAMERA:
							canSetCamera = LuaParser.Read.BoolFromTable(Table, KEY_CANSETCAMERA);
							break;
						case KEY_CANSYNCCAMERA:
							canSyncCamera = LuaParser.Read.BoolFromTable(Table, KEY_CANSYNCCAMERA);
							break;
					}
				}

				if (string.IsNullOrEmpty(type))
					MarkerType = MarkerTypes.BlankMarker;
				else
				{
					MarkerType = StringToMarkerType(type);
				}
			}

			public static MarkerTypes StringToMarkerType(string value)
			{
				return (MarkerTypes)System.Enum.Parse(typeof(MarkerTypes), value.Replace(" ", ""));
			}

			public void SaveMarkerValues(LuaParser.Creator LuaFile)
			{
				position = ScmapEditor.WorldPosToScmap(MarkerObj.transform.position);
				orientation = MarkerObj.transform.eulerAngles;
				//position = ScmapEditor.MapWorldPosInSave(MarkerObj.transform.position);

				if (AllowByType(KEY_SIZE))
					LuaFile.AddLine(LuaParser.Write.FloatToLuaFunction(LuaParser.Write.PropertiveToLua(KEY_SIZE), size));
				if (AllowByType(KEY_RESOURCE))
					LuaFile.AddLine(LuaParser.Write.BoolToLuaFunction(LuaParser.Write.PropertiveToLua(KEY_RESOURCE), resource));
				if (AllowByType(KEY_AMOUNT))
					LuaFile.AddLine(LuaParser.Write.FloatToLuaFunction(LuaParser.Write.PropertiveToLua(KEY_AMOUNT), amount));

				LuaFile.AddLine(LuaParser.Write.StringToLuaFunction(LuaParser.Write.PropertiveToLua(KEY_COLOR), color));

				if (AllowByType(KEY_EDITORICON))
					LuaFile.AddLine(LuaParser.Write.StringToLuaFunction(LuaParser.Write.PropertiveToLua(KEY_EDITORICON), editorIcon));
				if (AllowByType(KEY_HINT))
					LuaFile.AddLine(LuaParser.Write.BoolToLuaFunction(LuaParser.Write.PropertiveToLua(KEY_HINT), hint));

				if (AllowByType(KEY_ADJACENTTO))
				{
					adjacentTo = "";
					for(int i = 0; i < AdjacentToMarker.Count; i++)
					{
						if (i > 0)
							adjacentTo += " ";
						adjacentTo += AdjacentToMarker[i].Name;
					}

					LuaFile.AddLine(LuaParser.Write.StringToLuaFunction(LuaParser.Write.PropertiveToLua(KEY_ADJACENTTO), adjacentTo));

				}

				//Type
				LuaFile.AddLine(LuaParser.Write.StringToLuaFunction(LuaParser.Write.PropertiveToLua(KEY_TYPE), MarkerTypeToString(MarkerType)));
				LuaFile.AddLine(LuaParser.Write.StringToLuaFunction(LuaParser.Write.PropertiveToLua(KEY_PROP), prop));

				if (AllowByType(KEY_ZOOM))
					LuaFile.AddLine(LuaParser.Write.FloatToLuaFunction(LuaParser.Write.PropertiveToLua(KEY_ZOOM), zoom));
				if (AllowByType(KEY_CANSETCAMERA))
					LuaFile.AddLine(LuaParser.Write.BoolToLuaFunction(LuaParser.Write.PropertiveToLua(KEY_CANSETCAMERA), canSetCamera));
				if (AllowByType(KEY_CANSYNCCAMERA))
					LuaFile.AddLine(LuaParser.Write.BoolToLuaFunction(LuaParser.Write.PropertiveToLua(KEY_CANSYNCCAMERA), canSyncCamera));

				//Transform
				LuaFile.AddLine(LuaParser.Write.Vector3ToLuaFunction(LuaParser.Write.PropertiveToLua(KEY_POSITION), position));
				LuaFile.AddLine(LuaParser.Write.Vector3ToLuaFunction(LuaParser.Write.PropertiveToLua(KEY_ORIENTATION), orientation));
			}


		}
		#endregion


		static List<string> AllExistingNames = new List<string>();

		public static void RegisterMarkerName(string MarkerName)
		{
			if (!AllExistingNames.Contains(MarkerName))
				AllExistingNames.Add(MarkerName);
		}

		public static void RemoveMarkerName(string MarkerName)
		{
			AllExistingNames.Remove(MarkerName);

		}

		public static bool NameExist(string name)
		{
			return AllExistingNames.Contains(name);
		}

		public static string GetLowestName(Marker.MarkerTypes Type)
		{
			string prefix = "";
			if (Type == Marker.MarkerTypes.BlankMarker || Type == Marker.MarkerTypes.Mass || Type == Marker.MarkerTypes.Hydrocarbon)
				prefix = Marker.MarkerTypeToString(Type) + " ";
			else
			{
				prefix = Type.ToString() + "_";
			}

			int ID = 0;
			while (AllExistingNames.Contains(prefix + ID.ToString()))
				ID++;

			return prefix + ID.ToString();
		}



		private void ConnectAdjacentMarkers()
		{
			int mc = 0;
			int Mcount = MapLuaParser.Current.SaveLuaFile.Data.MasterChains[mc].Markers.Count;

			for (int m = 0; m < Mcount; m++)
			{
				if (MapLuaParser.Current.SaveLuaFile.Data.MasterChains[mc].Markers[m].adjacentTo.Length > 0)
				{
					string[] Names = MapLuaParser.Current.SaveLuaFile.Data.MasterChains[mc].Markers[m].adjacentTo.Split(" ".ToCharArray());
					//Transform Tr = MapLuaParser.Current.SaveLuaFile.Data.MasterChains[mc].Markers[m].MarkerObj.Tr;

					for (int e = 0; e < Names.Length; e++)
					{
						int ConM = MarkerIdByName(mc, Names[e], Mcount);

						if(ConM >= 0)
						{

							MapLuaParser.Current.SaveLuaFile.Data.MasterChains[mc].Markers[m].AdjacentToMarker.Add(MapLuaParser.Current.SaveLuaFile.Data.MasterChains[mc].Markers[ConM]);

						}

					}
				}
			}
		}

		int MarkerIdByName(int mc, string SearchName, int Mcount)
		{
			for (int m = 0; m < Mcount; m++)
			{
				if (MapLuaParser.Current.SaveLuaFile.Data.MasterChains[mc].Markers[m].MarkerObj.name == SearchName)
					return m;
			}
			return -1;
		}

	}
}
