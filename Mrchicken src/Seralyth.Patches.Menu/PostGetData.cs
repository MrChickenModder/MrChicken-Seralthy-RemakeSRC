using GorillaNetworking;
using GorillaNetworking.Store;
using HarmonyLib;
using Seralyth.Menu;

namespace Seralyth.Patches.Menu;

[HarmonyPatch(typeof(BundleManager), "CheckIfBundlesOwned")]
public class PostGetData
{
	public static bool CosmeticsInitialized;

	private static void Postfix()
	{
		CosmeticsInitialized = true;
		Main.CosmeticsOwned = ((CosmeticsController)CosmeticsController.instance).concatStringCosmeticsAllowed;
	}
}
