using System.ComponentModel;
using BepInEx;
using Seralyth.Managers;
using Seralyth.Menu;

namespace Seralyth;

[Description("Community powered mod menu for Gorilla Tag.")]
[BepInPlugin("org.seralyth.gorillatag.seralythmenu", "MrChicken Menu", "10.0.2")]
public class PluginBepInEx : BaseUnityPlugin
{
	public static bool FirstLaunch;

	private void Awake()
	{
		LogManager.SetLogger(delegate(Level level, string msg)
		{
			switch (level)
			{
			case Level.Error:
				((BaseUnityPlugin)this).Logger.LogError((object)msg);
				break;
			case Level.Warning:
				((BaseUnityPlugin)this).Logger.LogWarning((object)msg);
				break;
			case Level.Debug:
				((BaseUnityPlugin)this).Logger.LogDebug((object)msg);
				break;
			default:
				((BaseUnityPlugin)this).Logger.LogInfo((object)msg);
				break;
			}
		});
		Bootstrapper.Initialize();
	}

	private void OnDestroy()
	{
		Main.UnloadMenu();
	}
}
