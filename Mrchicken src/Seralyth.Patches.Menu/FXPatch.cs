using System;
using HarmonyLib;
using Seralyth.Menu;

namespace Seralyth.Patches.Menu;

[HarmonyPatch(typeof(FXSystem), "PlayFXForRig", new Type[]
{
	typeof(FXType),
	typeof(IFXContext),
	typeof(PhotonMessageInfoWrapped)
})]
public class FXPatch
{
	public static bool Prefix(FXType fxType, IFXContext context, PhotonMessageInfoWrapped info = default(PhotonMessageInfoWrapped))
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		NetPlayer sender = info.Sender;
		if (sender != null && Main.ShouldBypassChecks(sender))
		{
			context.OnPlayFX();
			return false;
		}
		return true;
	}
}
