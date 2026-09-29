using System.Collections.Generic;
using Seralyth.Classes.Menu;

namespace Seralyth.Mods.CustomMaps.Maps;

public class MiningSimulator : CustomMap
{
	public override long MapID => 4977315L;

	public override ButtonInfo[] Buttons => new ButtonInfo[3]
	{
		new ButtonInfo
		{
			buttonText = "Instant Mine",
			enableMethod = InstantMine,
			disableMethod = DisableInstantMine,
			toolTip = "Instantly mines any blocks with your pickaxe."
		},
		new ButtonInfo
		{
			buttonText = "Mine Anything",
			enableMethod = MineAnything,
			disableMethod = DisableMineAnything,
			toolTip = "Lets you mine any block."
		},
		new ButtonInfo
		{
			buttonText = "Infinite Backpack",
			enableMethod = InfiniteBackpack,
			disableMethod = DisableInfiniteBackpack,
			toolTip = "Lets you mine more blocks even if your inventory is full."
		}
	};

	public static void InstantMine()
	{
		Manager.ModifyCustomScript(new Dictionary<int, string> { { 974, "lastMinedTime = 0" } });
	}

	public static void DisableInstantMine()
	{
		Manager.RevertCustomScript(974);
	}

	public static void MineAnything()
	{
		Manager.ModifyCustomScript(new Dictionary<int, string> { { 965, "if true then" } });
	}

	public static void DisableMineAnything()
	{
		Manager.RevertCustomScript(965);
	}

	public static void InfiniteBackpack()
	{
		Manager.ModifyCustomScript(new Dictionary<int, string> { { 981, "if false then" } });
	}

	public static void DisableInfiniteBackpack()
	{
		Manager.RevertCustomScript(981);
	}
}
