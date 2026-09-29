using UnityEngine.SceneManagement;

namespace Seralyth.Mods.CustomMaps;

public static class SceneMapLoader
{
	public static void Init()
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		SceneMapRegistry.FillRegistry();
		SceneManager.activeSceneChanged += OnSceneChanged;
		Scene activeScene = SceneManager.GetActiveScene();
		CheckSceneForMap(((Scene)(ref activeScene)).name);
	}

	public static void OnSceneChanged(Scene oldScene, Scene newScene)
	{
		CheckSceneForMap(((Scene)(ref newScene)).name);
	}

	public static void CheckSceneForMap(string sceneName)
	{
		SceneMap mapForScene = SceneMapRegistry.GetMapForScene(sceneName);
		if (mapForScene != null)
		{
			Manager.UpdateCustomMapsTab(mapForScene.MapID);
		}
		else
		{
			Manager.UpdateCustomMapsTab();
		}
	}
}
