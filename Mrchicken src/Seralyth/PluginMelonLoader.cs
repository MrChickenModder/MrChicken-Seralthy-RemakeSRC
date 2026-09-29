using MelonLoader;
using Seralyth.Managers;
using Seralyth.Menu;

namespace Seralyth;

public class PluginMelonLoader : MelonMod
{
	public override void OnInitializeMelon()
	{
		LogManager.SetLogger(delegate(Level level, string msg)
		{
			switch (level)
			{
			case Level.Error:
				((MelonBase)this).LoggerInstance.Error(msg);
				break;
			case Level.Warning:
				((MelonBase)this).LoggerInstance.Warning(msg);
				break;
			default:
				((MelonBase)this).LoggerInstance.Msg(msg);
				break;
			}
		});
		Bootstrapper.Initialize();
	}

	public override void OnDeinitializeMelon()
	{
		Main.UnloadMenu();
	}
}
