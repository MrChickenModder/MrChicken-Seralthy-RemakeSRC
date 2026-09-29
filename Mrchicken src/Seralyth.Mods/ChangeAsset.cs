using Seralyth.Menu;
using UnityEngine;

namespace Seralyth.Mods;

public class ChangeAsset
{
	public static ChangeAsset Instance = new ChangeAsset();

	public int IncrementalValue;

	public static AssetEntry[] Assets = new AssetEntry[3]
	{
		new AssetEntry
		{
			file = "consolehamburburassets",
			prefabName = "burger",
			position = Vector3.zero,
			rotation = Vector3.zero,
			scale = Vector3.one
		},
		new AssetEntry
		{
			file = "consolehamburburassets",
			prefabName = "carti",
			position = new Vector3(0f, -0.1f, 0f),
			rotation = Vector3.zero,
			scale = Vector3.one * 0.5f
		},
		new AssetEntry
		{
			file = "consolehamburburassets",
			prefabName = "shrek",
			position = new Vector3(0f, 0f, 0.5f),
			rotation = Vector3.zero,
			scale = Vector3.one * 0.3f
		}
	};

	public static void ChangeValue(bool positive = true)
	{
		if (positive)
		{
			Instance.IncrementalValue++;
			if (Instance.IncrementalValue >= Assets.Length)
			{
				Instance.IncrementalValue = 0;
			}
		}
		else
		{
			Instance.IncrementalValue--;
			if (Instance.IncrementalValue < 0)
			{
				Instance.IncrementalValue = Assets.Length - 1;
			}
		}
		Buttons.GetIndex("Asset: ").overlapText = "Asset: <color=grey>[</color><color=green>" + Assets[Instance.IncrementalValue].prefabName + "</color><color=grey>]</color>";
	}
}
