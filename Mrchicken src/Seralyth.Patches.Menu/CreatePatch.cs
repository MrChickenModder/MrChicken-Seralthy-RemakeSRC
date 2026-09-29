using GorillaTagScripts;
using HarmonyLib;
using Seralyth.Mods;

namespace Seralyth.Patches.Menu;

[HarmonyPatch(typeof(BuilderTableNetworking), "PieceCreatedByShelfRPC")]
public class CreatePatch
{
	public static bool enabled;

	public static int pieceTypeSearch;

	private static void Postfix(int pieceType, int pieceId)
	{
		if (enabled && pieceTypeSearch == pieceType)
		{
			Fun.pieceId = pieceId;
			enabled = false;
		}
	}
}
