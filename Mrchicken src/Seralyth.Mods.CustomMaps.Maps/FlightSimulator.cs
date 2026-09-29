using System.Collections.Generic;
using Seralyth.Classes.Menu;

namespace Seralyth.Mods.CustomMaps.Maps;

public class FlightSimulator : CustomMap
{
	public override long MapID => 5024157L;

	public override ButtonInfo[] Buttons => new ButtonInfo[1]
	{
		new ButtonInfo
		{
			buttonText = "Steal Pilot",
			enableMethod = StealPilot,
			disableMethod = DisableStealPilot,
			toolTip = "Allows you to steal the pilot position from other people's planes."
		}
	};

	public static void StealPilot()
	{
		Manager.ModifyCustomScript(new Dictionary<int, string> { { 373, "if isButtonPressed(pilotClaimButtonJet) then" } });
	}

	public static void DisableStealPilot()
	{
		Manager.RevertCustomScript(373);
	}
}
