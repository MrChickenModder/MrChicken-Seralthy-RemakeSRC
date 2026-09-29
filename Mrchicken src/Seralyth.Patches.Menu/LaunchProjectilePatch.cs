using HarmonyLib;
using Seralyth.Extensions;
using Seralyth.Menu;
using Seralyth.Utilities;
using UnityEngine;

namespace Seralyth.Patches.Menu;

[HarmonyPatch(typeof(ProjectileWeapon), "LaunchProjectile")]
public class LaunchProjectilePatch
{
	public static bool enabled;

	public static void Prefix(ProjectileWeapon __instance)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		if (!enabled)
		{
			return;
		}
		GorillaTagger.Instance.rigidbody.linearVelocity = __instance.GetLaunchVelocity();
		if (Main.dynamicSounds)
		{
			AssetUtilities.LoadSoundFromURL("https://raw.githubusercontent.com/Seralyth/Seralyth-Menu/master/Resources/Server/Audio/Mods/Fun/AngryBirds/launch.ogg", "Audio/Mods/Fun/AngryBirds/launch.ogg", delegate(AudioClip clip)
			{
				clip.Play((float)Main.buttonClickVolume / 10f);
			});
		}
	}
}
