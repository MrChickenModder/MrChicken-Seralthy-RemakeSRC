using System.Collections.Generic;
using Seralyth.Classes.Menu;
using Seralyth.Menu;

namespace Seralyth.Classes.Mods;

public static class Changelog
{
	private static List<ChangelogEntry> entries = new List<ChangelogEntry>();

	private static bool populated;

	private static readonly Dictionary<string, string> typeDisplayNames = new Dictionary<string, string>
	{
		{ "ADDED", "ADDED" },
		{ "REMOVED", "REMOVED" },
		{ "UPDATED", "UPDATED" },
		{ "FIXED", "FIXED" }
	};

	public static List<ChangelogEntry> Entries
	{
		get
		{
			if (!populated)
			{
				AutoChangelogEntries.Populate();
				populated = true;
			}
			return entries;
		}
	}

	public static string GetTypeDisplayName(string type)
	{
		if (typeDisplayNames.TryGetValue(type, out var value))
		{
			return value;
		}
		return type;
	}

	public static void Add(string type, string description)
	{
		entries.Add(new ChangelogEntry
		{
			type = type,
			description = description
		});
	}

	public static void Clear()
	{
		entries.Clear();
		populated = false;
	}

	public static void RefreshCategory()
	{
		if (!populated)
		{
			AutoChangelogEntries.Populate();
			populated = true;
		}
		int category = Buttons.GetCategory("Update Category");
		if (category >= 0)
		{
			Buttons.buttons[category] = new ButtonInfo[1]
			{
				new ButtonInfo
				{
					buttonText = "Exit Update Category",
					method = delegate
					{
						Buttons.CurrentCategoryName = "Main";
					},
					isTogglable = false,
					toolTip = "Returns you back to the main page."
				}
			};
			for (int num = entries.Count - 1; num >= 0; num--)
			{
				ChangelogEntry changelogEntry = entries[num];
				string text = ((changelogEntry.type == "ADDED") ? "green" : ((changelogEntry.type == "REMOVED") ? "red" : ((changelogEntry.type == "UPDATED") ? "yellow" : "purple")));
				string buttonText = "<color=" + text + ">[" + GetTypeDisplayName(changelogEntry.type) + "]</color> " + changelogEntry.description;
				Buttons.AddButton(category, new ButtonInfo
				{
					buttonText = buttonText,
					label = true
				}, 1);
			}
		}
	}
}
