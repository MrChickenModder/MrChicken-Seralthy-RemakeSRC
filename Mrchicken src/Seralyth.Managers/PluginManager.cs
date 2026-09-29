using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using Seralyth.Classes.Menu;
using Seralyth.Menu;
using Seralyth.Mods;
using Seralyth.Utilities;

namespace Seralyth.Managers;

public class PluginManager
{
	public class Plugin
	{
		public string FileName;

		public bool Enabled;

		public string Name;

		public string Description;

		public Assembly Assembly;
	}

	public static readonly List<Plugin> Plugins = new List<Plugin>();

	private static readonly Dictionary<string, Assembly> cacheAssembly = new Dictionary<string, Assembly>();

	private static readonly Dictionary<Assembly, MethodInfo[]> cacheOnGUI = new Dictionary<Assembly, MethodInfo[]>();

	private static readonly Dictionary<Assembly, MethodInfo[]> cacheUpdate = new Dictionary<Assembly, MethodInfo[]>();

	public static void LoadPlugins()
	{
		Buttons.buttons[Buttons.GetCategory("Plugin Settings")] = new ButtonInfo[1]
		{
			new ButtonInfo
			{
				buttonText = "Exit Plugin Settings",
				method = delegate
				{
					Buttons.CurrentCategoryName = "Settings";
				},
				isTogglable = false,
				toolTip = "Returns you back to the settings menu."
			}
		};
		if (Plugins.Count > 0)
		{
			foreach (Plugin item in Plugins.Where((Plugin plugin2) => plugin2.Enabled))
			{
				DisablePlugin(item.Assembly);
			}
		}
		cacheAssembly.Clear();
		cacheUpdate.Clear();
		cacheOnGUI.Clear();
		Plugins.Clear();
		if (!Directory.Exists("SeralythMenu/Plugins"))
		{
			Directory.CreateDirectory("SeralythMenu/Plugins");
		}
		string[] source = new string[0];
		if (!File.Exists("SeralythMenu/Plugins/DisabledPlugins.txt"))
		{
			File.WriteAllText("SeralythMenu/Plugins/DisabledPlugins.txt", "");
		}
		else
		{
			string text = File.ReadAllText("SeralythMenu/Plugins/DisabledPlugins.txt");
			if (text.Length > 1)
			{
				source = text.Split("\n");
			}
		}
		string[] files = Directory.GetFiles("SeralythMenu/Plugins");
		string[] array = files;
		foreach (string text2 in array)
		{
			try
			{
				if (!(FileUtilities.GetFileExtension(text2) != "dll"))
				{
					string text3 = text2.Replace("SeralythMenu/Plugins/", "");
					Assembly assembly = GetAssembly(text2);
					string[] pluginInfo = GetPluginInfo(assembly);
					Plugin plugin = new Plugin
					{
						FileName = text3,
						Name = pluginInfo[0],
						Description = pluginInfo[1],
						Assembly = GetAssembly(text2),
						Enabled = !source.Contains(text3)
					};
					if (plugin.Enabled)
					{
						EnablePlugin(plugin.Assembly);
					}
					Plugins.Add(plugin);
				}
			}
			catch (Exception ex)
			{
				LogManager.Log("Error with loading plugin " + text2 + ": " + ex);
			}
		}
		foreach (Plugin Plugin in Plugins)
		{
			try
			{
				Buttons.AddButton(Buttons.GetCategory("Plugin Settings"), new ButtonInfo
				{
					buttonText = Plugin.FileName,
					overlapText = (Plugin.Enabled ? "<color=grey>[</color><color=green>ON</color><color=grey>]</color>" : "<color=grey>[</color><color=red>OFF</color><color=grey>]</color>") + " " + Plugin.Name,
					method = delegate
					{
						TogglePlugin(Plugin);
					},
					isTogglable = false,
					toolTip = Plugin.Description
				});
			}
			catch (Exception ex2)
			{
				LogManager.Log("Error with enabling plugin " + Plugin.Name + ": " + ex2);
			}
		}
		Buttons.AddButton(Buttons.GetCategory("Plugin Settings"), new ButtonInfo
		{
			buttonText = "Open Plugins Folder",
			method = OpenPluginsFolder,
			isTogglable = false,
			toolTip = "Opens a folder containing all of your plugins."
		});
		Buttons.AddButton(Buttons.GetCategory("Plugin Settings"), new ButtonInfo
		{
			buttonText = "Reload Plugins",
			method = ReloadPlugins,
			isTogglable = false,
			toolTip = "Reloads all of your plugins."
		});
		Buttons.AddButton(Buttons.GetCategory("Plugin Settings"), new ButtonInfo
		{
			buttonText = "Get More Plugins",
			method = LoadPluginLibrary,
			isTogglable = false,
			toolTip = "Opens a public plugin library, where you can download your own plugins."
		});
	}

	public static void DownloadPlugin(string name, string url)
	{
		if (name.Contains(".."))
		{
			name = name.Replace("..", "");
		}
		string text = url.Split("/")[^1];
		if (File.Exists("SeralythMenu/Plugins/" + text))
		{
			File.Delete("SeralythMenu/Plugins/" + text);
		}
		WebClient webClient = new WebClient();
		webClient.DownloadFile(url, "SeralythMenu/Plugins/" + text);
		LoadPlugins();
		NotificationManager.SendNotification("<color=grey>[</color><color=green>SUCCESS</color><color=grey>]</color> Successfully downloaded " + name + " to your plugins.");
	}

	public static void TogglePlugin(Plugin plugin)
	{
		if (plugin.Enabled)
		{
			DisablePlugin(plugin.Assembly);
		}
		else
		{
			EnablePlugin(plugin.Assembly);
		}
		plugin.Enabled = !plugin.Enabled;
		string contents = (from plugin2 in Plugins
			where !plugin2.Enabled
			select plugin2.FileName).Aggregate("", (string current, string disabledPlugin) => current + disabledPlugin + "\n");
		File.WriteAllText("SeralythMenu/Plugins/DisabledPlugins.txt", contents);
		Buttons.GetIndex(plugin.FileName).overlapText = (plugin.Enabled ? "<color=grey>[</color><color=green>ON</color><color=grey>]</color>" : "<color=grey>[</color><color=red>OFF</color><color=grey>]</color>") + " " + plugin.Name;
	}

	public static void ExecuteUpdate()
	{
		foreach (Plugin item in Plugins.Where((Plugin plugin) => plugin.Enabled))
		{
			try
			{
				PluginUpdate(item.Assembly);
			}
			catch (Exception ex)
			{
				LogManager.Log("Error with Update() with plugin " + item.Name + ": " + ex);
			}
		}
	}

	public static void ExecuteOnGUI()
	{
		foreach (Plugin item in Plugins.Where((Plugin plugin) => plugin.Enabled))
		{
			try
			{
				PluginOnGUI(item.Assembly);
			}
			catch (Exception ex)
			{
				LogManager.Log("Error with OnGUI() with plugin " + item.Name + ": " + ex);
			}
		}
	}

	private static Assembly GetAssembly(string dllName)
	{
		if (cacheAssembly.TryGetValue(dllName, out var value))
		{
			return value;
		}
		Assembly assembly = Assembly.Load(File.ReadAllBytes(dllName.Replace("/", "\\")));
		cacheAssembly.Add(dllName, assembly);
		return assembly;
	}

	private static string[] GetPluginInfo(Assembly Assembly)
	{
		Type[] types = Assembly.GetTypes();
		Type[] array = types;
		foreach (Type type in array)
		{
			FieldInfo field = type.GetField("Name", BindingFlags.Static | BindingFlags.Public);
			FieldInfo field2 = type.GetField("Description", BindingFlags.Static | BindingFlags.Public);
			if (field != null && field2 != null)
			{
				return new string[2]
				{
					(string)field.GetValue(null),
					(string)field2.GetValue(null)
				};
			}
		}
		return new string[2] { "null", "null" };
	}

	private static void EnablePlugin(Assembly Assembly)
	{
		Type[] types = Assembly.GetTypes();
		Type[] array = types;
		foreach (Type type in array)
		{
			try
			{
				type.GetMethod("OnEnable", BindingFlags.Static | BindingFlags.Public)?.Invoke(null, null);
			}
			catch
			{
			}
		}
	}

	private static void DisablePlugin(Assembly Assembly)
	{
		Type[] types = Assembly.GetTypes();
		Type[] array = types;
		foreach (Type type in array)
		{
			try
			{
				type.GetMethod("OnDisable", BindingFlags.Static | BindingFlags.Public)?.Invoke(null, null);
			}
			catch
			{
			}
		}
	}

	private static void PluginOnGUI(Assembly Assembly)
	{
		if (cacheOnGUI.TryGetValue(Assembly, out var value))
		{
			MethodInfo[] array = value;
			foreach (MethodInfo methodInfo in array)
			{
				methodInfo.Invoke(null, null);
			}
			return;
		}
		Type[] types = Assembly.GetTypes();
		List<MethodInfo> list = (from Type in types
			select Type.GetMethod("OnGUI", BindingFlags.Static | BindingFlags.Public) into Method
			where Method != null
			select Method).ToList();
		cacheOnGUI.Add(Assembly, list.ToArray());
		foreach (MethodInfo item in list)
		{
			item.Invoke(null, null);
		}
	}

	private static void PluginUpdate(Assembly Assembly)
	{
		if (cacheUpdate.TryGetValue(Assembly, out var value))
		{
			MethodInfo[] array = value;
			foreach (MethodInfo methodInfo in array)
			{
				methodInfo.Invoke(null, null);
			}
			return;
		}
		Type[] types = Assembly.GetTypes();
		List<MethodInfo> list = (from Type in types
			select Type.GetMethod("Update", BindingFlags.Static | BindingFlags.Public) into Method
			where Method != null
			select Method).ToList();
		cacheUpdate.Add(Assembly, list.ToArray());
		foreach (MethodInfo item in list)
		{
			item.Invoke(null, null);
		}
	}

	public static void ReloadPlugins()
	{
		Settings.SavePreferences();
		LoadPlugins();
		Settings.LoadPreferences();
		if (Main.isSearching)
		{
			Settings.Search();
		}
		Buttons.CurrentCategoryName = "Main";
	}

	public static void OpenPluginsFolder()
	{
		Process.Start(FileUtilities.GetGamePath() + "/SeralythMenu/Plugins");
	}

	public static void LoadPluginLibrary()
	{
		string http = Main.GetHttp("https://raw.githubusercontent.com/Seralyth/Seralyth-Menu/master/Resources/Server/Plugins/PluginLibrary.txt");
		string[] array = Main.AlphabetizeNoSkip(http.Split("\n"));
		List<ButtonInfo> list = new List<ButtonInfo>
		{
			new ButtonInfo
			{
				buttonText = "Exit Plugin Library",
				method = delegate
				{
					Buttons.CurrentCategoryName = "Plugin Settings";
				},
				isTogglable = false,
				toolTip = "Returns you back to the plugin settings."
			}
		};
		int num = 0;
		string[] array2 = array;
		foreach (string text in array2)
		{
			if (text.Length > 2)
			{
				num++;
				string[] Data = text.Split(";");
				list.Add(new ButtonInfo
				{
					buttonText = "PluginDownload" + num,
					overlapText = Data[0],
					method = delegate
					{
						DownloadPlugin(Data[0], Data[2]);
					},
					isTogglable = false,
					toolTip = Data[1]
				});
			}
		}
		Buttons.buttons[Buttons.GetCategory("Temporary Category")] = list.ToArray();
		Buttons.CurrentCategoryName = "Temporary Category";
	}
}
