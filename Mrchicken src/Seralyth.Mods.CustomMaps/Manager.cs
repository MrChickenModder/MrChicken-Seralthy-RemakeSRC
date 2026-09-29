using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using GorillaTagScripts.VirtualStumpCustomMaps;
using Modio.Mods;
using Seralyth.Classes.Menu;
using Seralyth.Managers;
using Seralyth.Menu;
using Seralyth.Utilities;

namespace Seralyth.Mods.CustomMaps;

public static class Manager
{
	public static readonly Dictionary<long, string> mapScriptArchives = new Dictionary<long, string>();

	public static readonly Dictionary<long, CustomMap> mapCache = new Dictionary<long, CustomMap>();

	public static long? currentMapId;

	public static void UpdateCustomMapsTab(long? overwriteId = null)
	{
		currentMapId = overwriteId;
		int category = Buttons.GetCategory("Custom Maps");
		List<ButtonInfo> list = new List<ButtonInfo>
		{
			new ButtonInfo
			{
				buttonText = "Exit Custom Maps",
				method = delegate
				{
					Buttons.CurrentCategoryName = "Main";
				},
				isTogglable = false,
				toolTip = "Returns you back to the main page."
			}
		};
		if (overwriteId.HasValue)
		{
			long value = overwriteId.Value;
			if (!mapScriptArchives.ContainsKey(value))
			{
				mapScriptArchives.Add(value, CustomGameMode.LuaScript);
			}
			CustomMap mapByID = GetMapByID(value);
			if (mapByID == null)
			{
				list.Add(new ButtonInfo
				{
					buttonText = "This map is not supported yet.",
					label = true
				});
			}
			else
			{
				list.AddRange(mapByID.Buttons);
			}
			list.AddRange(new ButtonInfo[4]
			{
				new ButtonInfo
				{
					buttonText = " ",
					label = true
				},
				new ButtonInfo
				{
					buttonText = "Edit Custom Script",
					method = EditUserScript,
					isTogglable = false,
					toolTip = "Opens your custom script for this map."
				},
				new ButtonInfo
				{
					buttonText = "Delete Custom Script",
					method = DeleteUserScript,
					isTogglable = false,
					toolTip = "Deletes your custom script for this map."
				},
				new ButtonInfo
				{
					buttonText = "Run Custom Script",
					enableMethod = StartUserScript,
					disableMethod = StopUserScript,
					toolTip = "Runs your custom script for this map."
				}
			});
		}
		else
		{
			list.Add(new ButtonInfo
			{
				buttonText = "You have not loaded a map.",
				label = true
			});
		}
		Buttons.buttons[category] = list.ToArray();
	}

	public static void ModifyCustomScript(Dictionary<int, string> replacements)
	{
		string luaScript = CustomGameMode.LuaScript;
		string[] array = luaScript.Split(new string[2] { "\r\n", "\n" }, StringSplitOptions.None);
		foreach (var (num2, text2) in replacements)
		{
			if (num2 >= 0 && num2 < array.Length)
			{
				LogManager.Log("Replacing " + array[num2] + " with " + text2);
				array[num2] = text2;
			}
		}
		CustomGameMode.LuaScript = string.Join(Environment.NewLine, array);
		if (NetworkSystem.Instance.InRoom)
		{
			LuauHud.Instance.RestartLuauScript();
		}
		CustomMapManager.ReturnToVirtualStump();
	}

	public static void EditUserScript()
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		string text = FileUtilities.GetGamePath() + string.Format("/{0}/CustomScripts/{1}.luau", "SeralythMenu", CustomMapLoader.LoadedMapModId);
		if (!File.Exists(text))
		{
			File.WriteAllText(text, mapScriptArchives[ModId.op_Implicit(CustomMapManager.currentRoomMapModId)]);
		}
		Process.Start(text);
	}

	public static void DeleteUserScript()
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		string path = FileUtilities.GetGamePath() + string.Format("/{0}/CustomScripts/{1}.luau", "SeralythMenu", CustomMapLoader.LoadedMapModId);
		if (File.Exists(path))
		{
			File.Delete(path);
		}
	}

	public static void StartUserScript()
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		string path = FileUtilities.GetGamePath() + string.Format("/{0}/CustomScripts/{1}.luau", "SeralythMenu", CustomMapLoader.LoadedMapModId);
		if (File.Exists(path))
		{
			CustomGameMode.LuaScript = File.ReadAllText(path);
		}
		if (NetworkSystem.Instance.InRoom)
		{
			LuauHud.Instance.RestartLuauScript();
		}
		CustomMapManager.ReturnToVirtualStump();
	}

	public static void StopUserScript()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		CustomGameMode.LuaScript = mapScriptArchives[ModId.op_Implicit(CustomMapManager.currentRoomMapModId)];
		if (NetworkSystem.Instance.InRoom)
		{
			LuauHud.Instance.RestartLuauScript();
		}
		CustomMapManager.ReturnToVirtualStump();
	}

	public static void RevertCustomScript(int[] lines)
	{
		Dictionary<int, string> replacements = lines.ToDictionary((int line) => line, (int line) => mapScriptArchives[ModId.op_Implicit(CustomMapManager.currentRoomMapModId)].Split(new string[2] { "\r\n", "\n" }, StringSplitOptions.None)[line]);
		ModifyCustomScript(replacements);
	}

	public static void RevertCustomScript(int line)
	{
		RevertCustomScript(new int[1] { line });
	}

	public static CustomMap GetMapByID(long id)
	{
		if (mapCache.TryGetValue(id, out var value))
		{
			return value;
		}
		IEnumerable<Type> enumerable = from t in Assembly.GetExecutingAssembly().GetTypes()
			where t.IsClass && !t.IsAbstract && t.IsSubclassOf(typeof(CustomMap))
			select t;
		foreach (Type item in enumerable)
		{
			CustomMap customMap = (CustomMap)Activator.CreateInstance(item);
			if (customMap.MapID != id)
			{
				continue;
			}
			value = customMap;
			break;
		}
		mapCache[id] = value;
		return value;
	}
}
