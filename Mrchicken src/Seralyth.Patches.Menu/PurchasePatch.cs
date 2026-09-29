using GorillaNetworking;
using HarmonyLib;

namespace Seralyth.Patches.Menu;

[HarmonyPatch(typeof(CosmeticsController), "PurchaseItem")]
public class PurchasePatch
{
	public static bool enabled;

	private static bool Prefix()
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Invalid comparison between Unknown and I4
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		if (enabled)
		{
			CosmeticItem itemFromDict = ((CosmeticsController)CosmeticsController.instance).GetItemFromDict(((CosmeticsController)CosmeticsController.instance).itemToBuy.itemName);
			if ((int)itemFromDict.itemCategory == 13)
			{
				((CosmeticsController)CosmeticsController.instance).UnlockItem(((CosmeticsController)CosmeticsController.instance).itemToBuy.itemName, false);
				string[] bundledItems = itemFromDict.bundledItems;
				foreach (string text in bundledItems)
				{
					((CosmeticsController)CosmeticsController.instance).UnlockItem(text, false);
				}
			}
			else
			{
				((CosmeticsController)CosmeticsController.instance).UnlockItem(((CosmeticsController)CosmeticsController.instance).itemToBuy.itemName, false);
			}
			((CosmeticsController)CosmeticsController.instance).UpdateMyCosmetics();
			((CosmeticsController)CosmeticsController.instance).currentPurchaseItemStage = (PurchaseItemStages)6;
			((CosmeticsController)CosmeticsController.instance).UpdateShoppingCart();
			((CosmeticsController)CosmeticsController.instance).ProcessPurchaseItemState((string)null, ((CosmeticsController)CosmeticsController.instance).isLastHandTouchedLeft);
			return false;
		}
		return true;
	}
}
