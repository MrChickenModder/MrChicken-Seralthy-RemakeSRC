using Seralyth.Managers;
using Seralyth.Menu;
using UnityEngine;

namespace Seralyth;

public static class Plugin
{
	private sealed class Injector : MonoBehaviour
	{
		private void Awake()
		{
			LogManager.SetLogger(delegate(Level level, string msg)
			{
				switch (level)
				{
				case Level.Error:
					Debug.LogError((object)msg);
					break;
				case Level.Warning:
					Debug.LogWarning((object)msg);
					break;
				default:
					Debug.Log((object)msg);
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

	public static void Inject()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Expected O, but got Unknown
		GameObject val = new GameObject("Seralyth");
		val.AddComponent<Injector>();
	}

	public static void InjectDontDestroy()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Expected O, but got Unknown
		GameObject val = new GameObject("Seralyth");
		Object.DontDestroyOnLoad((Object)(object)val);
		val.AddComponent<Injector>();
	}
}
