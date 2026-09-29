using GorillaTagScripts;
using Seralyth.Menu;
using UnityEngine;

namespace Seralyth.Utilities;

public class ManagerRegistry
{
	public class GhostReactor
	{
		public static GhostReactorManager GhostReactorManager => GhostReactor.instance.grManager;

		public static GameEntityManager GameEntityManager => GameEntityManager.GetManagerForZone(GhostReactor.instance.zone);
	}

	public class SuperInfection
	{
		public static SuperInfectionManager SuperInfectionManager => SuperInfectionManager.activeSuperInfectionManager;

		public static SuperInfection ZoneSuperInfection => SuperInfectionManager.zoneSuperInfection;

		public static GameEntityManager GameEntityManager => SuperInfectionManager.gameEntityManager;
	}

	public class CustomMaps
	{
		public static CustomMapsGameManager CustomMapsGameManager => CustomMapsGameManager.instance;

		public static GameEntityManager GameEntityManager => CustomMapsGameManager.gameEntityManager;
	}

	private static LightningManager _lightningManager;

	public static BuilderTable BuilderTable => GetBuilderTable();

	public static LightningManager LightningManager
	{
		get
		{
			if ((Object)(object)_lightningManager == (Object)null)
			{
				_lightningManager = Main.GetObject("Environment Objects/05Maze_PersistentObjects/2025_Halloween1_PersistentObjects/LightningManager").GetComponent<LightningManager>();
			}
			return _lightningManager;
		}
		set
		{
			_lightningManager = value;
		}
	}

	private static BuilderTable GetBuilderTable()
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		BuilderTable result = default(BuilderTable);
		BuilderTable.TryGetBuilderTableForZone(VRRig.LocalRig.zoneEntity.currentZone, ref result);
		return result;
	}
}
