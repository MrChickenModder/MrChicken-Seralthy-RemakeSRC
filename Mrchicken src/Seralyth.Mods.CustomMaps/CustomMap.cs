using Seralyth.Classes.Menu;

namespace Seralyth.Mods.CustomMaps;

public abstract class CustomMap
{
	public abstract long MapID { get; }

	public abstract ButtonInfo[] Buttons { get; }
}
