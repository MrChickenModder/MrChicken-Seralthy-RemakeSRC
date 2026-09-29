using HarmonyLib;
using Seralyth.Menu;

namespace Seralyth.Patches.Menu;

[HarmonyPatch(typeof(NetworkSystemPUN), "SetupVoice")]
public class SetupVoicePatch
{
	public static void Postfix()
	{
		if (Buttons.GetIndex("High Quality Microphone").enabled)
		{
			Buttons.GetIndex("High Quality Microphone").enableMethod();
		}
	}
}
