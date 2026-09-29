using System.Collections.Generic;

namespace Seralyth.Mods.CustomMaps;

public static class SceneMapRegistry
{
	public static readonly Dictionary<string, SceneMap> sceneMapLookup = new Dictionary<string, SceneMap>();

	public static void RegisterMap(long mapID, string sceneName)
	{
		if (!sceneMapLookup.ContainsKey(sceneName))
		{
			sceneMapLookup.Add(sceneName, new SceneMap(mapID, sceneName));
		}
	}

	public static SceneMap GetMapForScene(string sceneName)
	{
		sceneMapLookup.TryGetValue(sceneName, out var value);
		return value;
	}

	public static void FillRegistry()
	{
		RegisterMap(5107228L, "monke-magic-halloween-alt");
		RegisterMap(5135423L, "Guns");
		RegisterMap(5024157L, "Flight-Simulator");
		RegisterMap(4977315L, "MiningSimulator");
	}
}
