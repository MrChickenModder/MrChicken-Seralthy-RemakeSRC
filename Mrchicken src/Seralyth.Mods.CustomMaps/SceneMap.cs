namespace Seralyth.Mods.CustomMaps;

public class SceneMap
{
	public long MapID { get; }

	public string SceneName { get; }

	public SceneMap(long mapID, string sceneName)
	{
		MapID = mapID;
		SceneName = sceneName;
	}
}
