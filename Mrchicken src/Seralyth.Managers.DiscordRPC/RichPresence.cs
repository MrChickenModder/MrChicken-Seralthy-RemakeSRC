using Valve.Newtonsoft.Json;

namespace Seralyth.Managers.DiscordRPC;

public sealed class RichPresence : BaseRichPresence
{
	[JsonProperty(/*Could not decode attribute arguments.*/)]
	public Button[] Buttons { get; set; }

	public bool HasButtons()
	{
		return Buttons != null && Buttons.Length != 0;
	}

	public RichPresence WithState(string state)
	{
		base.State = state;
		return this;
	}

	public RichPresence WithStateUrl(string stateUrl)
	{
		base.StateUrl = stateUrl;
		return this;
	}

	public RichPresence WithDetails(string details)
	{
		base.Details = details;
		return this;
	}

	public RichPresence WithDetailsUrl(string detailsUrl)
	{
		base.DetailsUrl = detailsUrl;
		return this;
	}

	public RichPresence WithType(ActivityType type)
	{
		base.Type = type;
		return this;
	}

	public RichPresence WithStatusDisplay(StatusDisplayType statusDisplay)
	{
		base.StatusDisplay = statusDisplay;
		return this;
	}

	public RichPresence WithTimestamps(Timestamps timestamps)
	{
		base.Timestamps = timestamps;
		return this;
	}

	public RichPresence WithAssets(Assets assets)
	{
		base.Assets = assets;
		return this;
	}

	public RichPresence WithParty(Party party)
	{
		base.Party = party;
		return this;
	}

	public RichPresence WithSecrets(Secrets secrets)
	{
		base.Secrets = secrets;
		return this;
	}

	public RichPresence WithButtons(Button topButton, Button bottomButton = null)
	{
		Buttons = ((topButton == null || bottomButton == null) ? ((topButton == null && bottomButton == null) ? null : new Button[1] { topButton ?? bottomButton }) : new Button[2] { topButton, bottomButton });
		return this;
	}

	public RichPresence Clone()
	{
		RichPresence richPresence = new RichPresence();
		richPresence.State = ((_state != null) ? (_state.Clone() as string) : null);
		richPresence.StateUrl = ((_stateUrl != null) ? (_stateUrl.Clone() as string) : null);
		richPresence.Details = ((_details != null) ? (_details.Clone() as string) : null);
		richPresence.DetailsUrl = ((_detailsUrl != null) ? (_detailsUrl.Clone() as string) : null);
		richPresence.Type = base.Type;
		richPresence.StatusDisplay = base.StatusDisplay;
		richPresence.Buttons = ((!HasButtons()) ? null : (Buttons.Clone() as Button[]));
		richPresence.Secrets = ((!HasSecrets()) ? null : new Secrets
		{
			Join = ((base.Secrets.Join != null) ? (base.Secrets.Join.Clone() as string) : null),
			SpectateSecret = ((base.Secrets.SpectateSecret != null) ? (base.Secrets.SpectateSecret.Clone() as string) : null)
		});
		richPresence.Timestamps = ((!HasTimestamps()) ? null : new Timestamps
		{
			Start = base.Timestamps.Start,
			End = base.Timestamps.End
		});
		richPresence.Assets = ((!HasAssets()) ? null : new Assets
		{
			LargeImageKey = ((base.Assets.LargeImageKey != null) ? (base.Assets.LargeImageKey.Clone() as string) : null),
			LargeImageText = ((base.Assets.LargeImageText != null) ? (base.Assets.LargeImageText.Clone() as string) : null),
			LargeImageUrl = ((base.Assets.LargeImageUrl != null) ? (base.Assets.LargeImageUrl.Clone() as string) : null),
			SmallImageKey = ((base.Assets.SmallImageKey != null) ? (base.Assets.SmallImageKey.Clone() as string) : null),
			SmallImageText = ((base.Assets.SmallImageText != null) ? (base.Assets.SmallImageText.Clone() as string) : null),
			SmallImageUrl = ((base.Assets.SmallImageUrl != null) ? (base.Assets.SmallImageUrl.Clone() as string) : null)
		});
		richPresence.Party = ((!HasParty()) ? null : new Party
		{
			ID = base.Party.ID,
			Size = base.Party.Size,
			Max = base.Party.Max,
			Privacy = base.Party.Privacy
		});
		return richPresence;
	}

	internal RichPresence Merge(BaseRichPresence presence)
	{
		_state = presence.State;
		_stateUrl = presence.StateUrl;
		_details = presence.Details;
		_detailsUrl = presence.DetailsUrl;
		base.Type = presence.Type;
		base.StatusDisplay = presence.StatusDisplay;
		base.Party = presence.Party;
		base.Timestamps = presence.Timestamps;
		base.Secrets = presence.Secrets;
		if (presence.HasAssets())
		{
			if (!HasAssets())
			{
				base.Assets = presence.Assets;
			}
			else
			{
				base.Assets.Merge(presence.Assets);
			}
		}
		else
		{
			base.Assets = null;
		}
		return this;
	}

	internal override bool Matches(RichPresence other)
	{
		if (!base.Matches(other))
		{
			return false;
		}
		if ((Buttons == null) ^ (other.Buttons == null))
		{
			return false;
		}
		if (Buttons != null)
		{
			if (Buttons.Length != other.Buttons.Length)
			{
				return false;
			}
			for (int i = 0; i < Buttons.Length; i++)
			{
				Button button = Buttons[i];
				Button button2 = other.Buttons[i];
				if (button.Label != button2.Label || button.Url != button2.Url)
				{
					return false;
				}
			}
		}
		return true;
	}

	public static implicit operator bool(RichPresence presesnce)
	{
		return presesnce != null;
	}
}
