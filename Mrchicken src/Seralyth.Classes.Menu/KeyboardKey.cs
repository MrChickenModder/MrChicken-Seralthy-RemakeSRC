using System.Collections.Generic;
using Seralyth.Menu;
using UnityEngine;

namespace Seralyth.Classes.Menu;

public class KeyboardKey : MonoBehaviour
{
	public static readonly Dictionary<string, KeyboardKey> keyLookupDictionary = new Dictionary<string, KeyboardKey>();

	public string key;

	public static float delay;

	public void Start()
	{
		keyLookupDictionary[((Object)((Component)this).gameObject).name] = this;
	}

	public void OnTriggerEnter(Collider collider)
	{
		if ((!((Object)(object)collider != (Object)(object)Main.lKeyCollider) || !((Object)(object)collider != (Object)(object)Main.rKeyCollider)) && !((Object)(object)Main.menu == (Object)null) && Time.time > delay)
		{
			if (!Buttons.GetIndex("Disable Keyboard Delay").enabled)
			{
				delay = Time.time + 0.1f;
			}
			if (Main.doButtonsVibrate)
			{
				GorillaTagger.Instance.StartVibration((Object)(object)collider == (Object)(object)Main.lKeyCollider, GorillaTagger.Instance.tagHapticStrength / 2f, GorillaTagger.Instance.tagHapticDuration / 2f);
			}
			VRRig.LocalRig.PlayHandTapLocal(66, (Object)(object)collider == (Object)(object)Main.lKeyCollider, (float)Main.buttonClickVolume / 10f);
			Main.PressKeyboardKey(key);
		}
	}
}
