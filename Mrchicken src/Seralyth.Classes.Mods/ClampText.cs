using UnityEngine;
using UnityEngine.UI;

namespace Seralyth.Classes.Mods;

public class ClampText : MonoBehaviour
{
	public Text currentText;

	public Text targetText;

	public void Start()
	{
		currentText = ((Component)this).GetComponent<Text>();
		LateUpdate();
	}

	public void LateUpdate()
	{
		currentText.text = targetText.text;
	}
}
