using Seralyth.Managers;
using Seralyth.Menu;
using UnityEngine;

namespace Seralyth.Classes.Menu;

public class ButtonCollider : MonoBehaviour
{
	public string relatedText;

	public bool incremental;

	public bool positive;

	public void OnTriggerEnter(Collider collider)
	{
		if (Time.time > Main.buttonCooldown && (!((Object)(object)collider != (Object)(object)Main.buttonCollider) || !((Object)(object)collider != (Object)(object)Main.lKeyCollider) || !((Object)(object)collider != (Object)(object)Main.rKeyCollider)) && !Main.joystickMenu && !((Object)(object)Main.menu == (Object)null))
		{
			Main.buttonCooldown = Time.time + 0.2f;
			if (relatedText != "Global Return")
			{
				SoundManager.Play(SoundManager.DefaultSounds["Button"], null, null, relatedText);
			}
			if (Main.annoyingMode && Random.Range(1, 5) == 2)
			{
				NotificationManager.SendNotification("Error");
			}
			else if (incremental)
			{
				Main.ToggleIncremental(relatedText, positive);
			}
			else
			{
				Main.Toggle(relatedText, fromMenu: true);
			}
		}
	}
}
