using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ExitGames.Client.Photon;
using GorillaExtensions;
using GorillaLocomotion;
using GorillaTag;
using Photon.Pun;
using Photon.Realtime;
using Seralyth.Extensions;
using Seralyth.Managers;
using Seralyth.Menu;
using Seralyth.Patches.Menu;
using Seralyth.Utilities;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Seralyth.Mods;

public static class Projectiles
{
	public static readonly string[] ProjectileObjectNames = new string[78]
	{
		"SnowballLeftAnchor", "SnowballRightAnchor", "GrowingSnowballLeftAnchor", "GrowingSnowballRightAnchor", "WaterBalloonLeftAnchor", "WaterBalloonRightAnchor", "LavaRockAnchor", "LavaRockAnchor", "BucketGiftFunctionalAnchor_Left", "BucketGiftFunctionalAnchor_Right",
		"ScienceCandyLeftAnchor", "ScienceCandyRightAnchor", "FishFoodLeftAnchor", "FishFoodRightAnchor", "AppleLeftAnchor", "AppleRightAnchor", "TrickTreatFunctionalAnchor", "TrickTreatFunctionalAnchorRIGHT Variant", "VotingRockAnchor_LEFT", "VotingRockAnchor_RIGHT",
		"BookLeftAnchor", "BookRightAnchor", "CoinLeftAnchor", "CoinRightAnchor", "EggLeftHand_Anchor Variant", "EggRightHand_Anchor Variant", "IceCreamLeftAnchor", "IceCreamRightAnchor", "HotDogLeftAnchor", "HotDogRightAnchor",
		"Fireworks_Anchor Variant_Left Hand", "Fireworks_Anchor Variant_Right Hand", "Papers_Anchor Variant_Left Hand", "Papers_Anchor Variant_Right Hand", "IceCreamScoopLeftAnchor", "IceCreamScoopRightAnchor", "ChipsLeftAnchor", "ChipsRightAnchor", "SalsaLeftAnchor", "SalsaRightAnchor",
		"ApplePieLeftAnchor", "ApplePieRightAnchor", "GrowingMashedPotatoLeftAnchor", "GrowingMashedPotatoRightAnchor", "BerryPieLeftAnchor", "BerryPieRightAnchor", "LayerDipLeftAnchor", "LayerDipRightAnchor", "PumpkinPieLeftAnchor", "PumpkinPieRightAnchor",
		"GrowingStuffingLeftAnchor", "GrowingStuffingRightAnchor", "CornLeftAnchor", "CornRightAnchor", "TurkeyLegLeftAnchor", "TurkeyLegRightAnchor", "GoalpostFootball_Anchor_LeftHand", "GoalpostFootball_Anchor_RightHand", "PopcornBall_Anchor_Left", "PopcornBall_Anchor_Right",
		"CrackedPlate_Lump_Projectile_Anchor_LEFT", "CrackedPlate_Lump_Projectile_Anchor_RIGHT", "PortableBonfire_Sticks_Anchor_LeftHand", "PortableBonfire_Sticks_Anchor_RightHand", "Walnut_Anchor_Left", "Walnut_Anchor_Right", "HotCocoaCup_Anchor_LEFT", "HotCocoaCup_Anchor_RIGHT", "SlingshotProjectile", "SlingshotProjectile",
		"PillowProjectile_Anchor_LEFT", "PillowProjectile_Anchor_RIGHT", "CakePieces_Anchor_LEFT", "CakePieces_Anchor_RIGHT", "BalloonAnimalProjectileAnchor_LEFT", "BalloonAnimalProjectileAnchor_RIGHT", "EnergyWafer_Anchor_LEFT", "EnergyWafer_Anchor_RIGHT"
	};

	public static string SnowballName = "GrowingSnowball";

	public static Coroutine RigCoroutine;

	public static Coroutine DisableCoroutine;

	public static bool friendSided;

	public static int friendProjectileScale = 1;

	public static bool clientSided;

	private static float lastProjectileErrorTime;

	public static int projMode;

	public static int snowballIndex;

	public static int targetProjectileIndex;

	public static int shootCycle = 1;

	public static int red = 10;

	public static int green = 5;

	public static int blue;

	public static float projDebounce;

	public static float projDebounceType;

	public static int projDebounceIndex = 2;

	private static float lastProjSpamNotify;

	private static readonly Dictionary<bool, bool> previousGripHeld = new Dictionary<bool, bool>();

	public static IEnumerator EnableRig()
	{
		yield return (object)new WaitForSeconds(projDebounceType + 0.2f);
		((Behaviour)VRRig.LocalRig).enabled = true;
	}

	public static IEnumerator DisableProjectile(SnowballThrowable Throwable)
	{
		yield return (object)new WaitForSeconds(projDebounceType + 0.2f);
		Throwable.SetSnowballActiveLocal(false);
	}

	public static void FriendProjectileScale(bool positive = true)
	{
		if (positive)
		{
			friendProjectileScale++;
		}
		else
		{
			friendProjectileScale--;
		}
		if (friendProjectileScale > 5)
		{
			friendProjectileScale = 1;
		}
		if (friendProjectileScale < 1)
		{
			friendProjectileScale = 5;
		}
		Buttons.GetIndex("Friend Projectile Scale").overlapText = "Friend Projectile Scale <color=grey>[</color><color=green>" + friendProjectileScale + "</color><color=grey>]</color>";
	}

	public static void LaunchLocalProjectile(Vector3 position, Vector3 velocity, int projectileType, int index, bool overrideColor, Color32 color, int scale, int projectileHash, VRRig rig)
	{
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			if (projectileType == 0)
			{
				ProjectileWeapon projectileWeapon = rig.projectileWeapon;
				if (GTExt.IsNotNull((Object)(object)projectileWeapon))
				{
					GameObject val = ObjectPools.instance.Instantiate(projectileWeapon.projectilePrefab, true);
					SlingshotProjectile component = val.GetComponent<SlingshotProjectile>();
					component.Launch(position, velocity, (NetPlayer)null, false, false, index, (float)scale, overrideColor, Color32.op_Implicit(color));
				}
			}
			else
			{
				GameObject val2 = ObjectPools.instance.Instantiate(projectileHash, true);
				SlingshotProjectile component2 = val2.GetComponent<SlingshotProjectile>();
				component2.Launch(position, velocity, (NetPlayer)null, false, false, index, (float)scale, overrideColor, Color32.op_Implicit(color));
			}
		}
		catch (Exception ex)
		{
			LogManager.LogError($"Friend Projectile error: {ex.Message}. Full exception:\n{ex}");
			if (Time.time > lastProjectileErrorTime + 2f)
			{
				NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> LaunchLocalProjectile: " + ex.Message);
				lastProjectileErrorTime = Time.time;
			}
		}
	}

	public static void BetaFireProjectile(string projectileName, Vector3 position, Vector3 velocity, Color color, RaiseEventOptions options = null, bool bypassTeleport = false)
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Expected O, but got Unknown
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0204: Unknown result type (might be due to invalid IL or missing references)
		//IL_020a: Invalid comparison between Unknown and I4
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_023e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0244: Invalid comparison between Unknown and I4
		//IL_04b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_04cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_024f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0500: Unknown result type (might be due to invalid IL or missing references)
		//IL_050d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0543: Unknown result type (might be due to invalid IL or missing references)
		//IL_0556: Unknown result type (might be due to invalid IL or missing references)
		//IL_0569: Unknown result type (might be due to invalid IL or missing references)
		//IL_057c: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_031b: Unknown result type (might be due to invalid IL or missing references)
		//IL_031c: Unknown result type (might be due to invalid IL or missing references)
		//IL_032f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0633: Unknown result type (might be due to invalid IL or missing references)
		//IL_0634: Unknown result type (might be due to invalid IL or missing references)
		//IL_063a: Unknown result type (might be due to invalid IL or missing references)
		//IL_042b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0442: Unknown result type (might be due to invalid IL or missing references)
		//IL_046b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0474: Unknown result type (might be due to invalid IL or missing references)
		//IL_0489: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0362: Unknown result type (might be due to invalid IL or missing references)
		//IL_0363: Unknown result type (might be due to invalid IL or missing references)
		//IL_0368: Unknown result type (might be due to invalid IL or missing references)
		//IL_037e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0388: Unknown result type (might be due to invalid IL or missing references)
		//IL_0392: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_068c: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			color.a = 1f;
			if (((Vector3)(ref velocity)).magnitude > 9999f)
			{
				velocity = ((Vector3)(ref velocity)).normalized * 9999f;
			}
			if (options == null)
			{
				options = new RaiseEventOptions();
			}
			options.Receivers = (ReceiverGroup)1;
			SnowballThrowable projectile = Main.GetProjectile(projectileName);
			if (projectileName != "SlingshotProjectile")
			{
				if ((Object)(object)projectile == (Object)null)
				{
					throw new Exception("Throwable is null");
				}
				if (!((Component)projectile).gameObject.activeSelf)
				{
					projectile.SetSnowballActiveLocal(true);
					((Component)projectile).transform.position = GorillaTagger.Instance.leftHandTransform.position;
					((Component)projectile).transform.rotation = GorillaTagger.Instance.leftHandTransform.rotation;
					if (Buttons.GetIndex("Random Projectile").enabled)
					{
						((MonoBehaviour)CoroutineManager.instance).StartCoroutine(DisableProjectile(projectile));
					}
					else
					{
						if (DisableCoroutine != null)
						{
							((MonoBehaviour)CoroutineManager.instance).StopCoroutine(DisableCoroutine);
						}
						DisableCoroutine = ((MonoBehaviour)CoroutineManager.instance).StartCoroutine(DisableProjectile(projectile));
					}
				}
			}
			if (!(Time.time > projDebounce))
			{
				return;
			}
			if (!bypassTeleport && (Vector3.Distance(((Component)GorillaTagger.Instance.bodyCollider).transform.position, position) > 3.9f || (!clientSided && !friendSided)))
			{
				((Behaviour)VRRig.LocalRig).enabled = false;
				((Component)VRRig.LocalRig).transform.position = position + new Vector3(0f, (velocity.y > 0f) ? (-3f) : 3f, 0f);
				if (RigCoroutine != null)
				{
					((MonoBehaviour)CoroutineManager.instance).StopCoroutine(RigCoroutine);
				}
				RigCoroutine = ((MonoBehaviour)CoroutineManager.instance).StartCoroutine(EnableRig());
			}
			bool flag = (int)options.Receivers == 1 || (options.TargetActors != null && Extensions.Contains(options.TargetActors, PhotonNetwork.LocalPlayer.ActorNumber));
			if (flag)
			{
				if ((int)options.Receivers == 1)
				{
					options.Receivers = (ReceiverGroup)0;
				}
				if (options.TargetActors != null && Extensions.Contains(options.TargetActors, PhotonNetwork.LocalPlayer.ActorNumber))
				{
					List<int> list = options.TargetActors.ToList();
					list.Remove(PhotonNetwork.LocalPlayer.ActorNumber);
					options.TargetActors = list.ToArray();
				}
			}
			if (projectileName.Contains(SnowballName))
			{
				int num = (friendSided ? Math.Max(Overpowered.snowballScale, friendProjectileScale) : Overpowered.snowballScale);
				GrowingSnowballThrowable val = (GrowingSnowballThrowable)(object)((projectile is GrowingSnowballThrowable) ? projectile : null);
				int projectileIncrement = Overpowered.GetProjectileIncrement(position, velocity, ((Component)projectile).transform.lossyScale.x);
				if (flag)
				{
					SlingshotProjectile val2 = val.SpawnGrowingSnowball(ref velocity, (float)num);
					val2.Launch(position, velocity, NetworkSystem.Instance.LocalPlayer, false, false, projectileIncrement, (float)num, true, color);
				}
				if (PhotonNetwork.InRoom && !clientSided)
				{
					if (friendSided)
					{
						Color32 val3 = Color32.op_Implicit(color);
						PhotonNetwork.RaiseEvent((byte)53, (object)new object[8]
						{
							"sendSnowball",
							position,
							velocity,
							val3.r,
							val3.g,
							val3.b,
							val.snowballSizeLevels[num].snowballScale,
							projectileIncrement
						}, options, SendOptions.SendUnreliable);
					}
					else
					{
						object[] obj = new object[2]
						{
							val.changeSizeEvent._eventId,
							num
						};
						RaiseEventOptions obj2 = options;
						SendOptions val4 = default(SendOptions);
						((SendOptions)(ref val4)).Reliability = false;
						val4.Encrypt = true;
						PhotonNetwork.RaiseEvent((byte)176, (object)obj, obj2, val4);
						object[] obj3 = new object[4]
						{
							val.snowballThrowEvent._eventId,
							position,
							velocity,
							projectileIncrement
						};
						RaiseEventOptions obj4 = options;
						val4 = default(SendOptions);
						((SendOptions)(ref val4)).Reliability = false;
						val4.Encrypt = true;
						PhotonNetwork.RaiseEvent((byte)176, (object)obj3, obj4, val4);
					}
				}
			}
			else
			{
				int projectileIncrement2 = Overpowered.GetProjectileIncrement(position, velocity, ((Component)projectile).transform.lossyScale.x);
				Color32 val5 = Color32.op_Implicit(color);
				int num2 = ((!(projectileName == "SlingshotProjectile")) ? (projectileName.ToLower().Contains("left") ? 1 : 2) : 0);
				List<object> list2 = new List<object> { position, velocity, num2, projectileIncrement2, true, val5.r, val5.g, val5.b, val5.a };
				List<object> list3 = new List<object>();
				if (friendSided || clientSided)
				{
					list2.Add(friendProjectileScale);
					list2.Add(projectile.ProjectileHash);
					list3.Add("sendProjectile");
					list3.Add(list2.ToArray());
				}
				else
				{
					list3.Add(NetworkSystem.Instance.ServerTimestamp);
					list3.Add(0);
					list3.Add(list2.ToArray());
				}
				if (flag)
				{
					LaunchLocalProjectile(position, velocity, num2, projectileIncrement2, overrideColor: true, val5, (!friendSided) ? 1 : friendProjectileScale, projectile.ProjectileHash, VRRig.LocalRig);
				}
				if (!clientSided && NetworkSystem.Instance.InRoom)
				{
					PhotonNetwork.RaiseEvent((byte)(friendSided ? 53 : 3), (object)list3.ToArray(), options, SendOptions.SendReliable);
					Main.RPCProtection();
				}
			}
			if (projDebounceType > 0f)
			{
				projDebounce = Time.time + projDebounceType;
			}
		}
		catch (Exception ex)
		{
			LogManager.LogError($"Projectile error: {ex.Message}. Full exception:\n{ex}");
			if (Time.time > lastProjectileErrorTime + 2f)
			{
				NotificationManager.SendNotification("<color=grey>[</color><color=red>ERROR</color><color=grey>]</color> Projectile: " + ex.Message);
				lastProjectileErrorTime = Time.time;
			}
		}
	}

	public static void BetaFireImpact(Vector3 position, Color color)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Expected O, but got Unknown
		if (Time.time > projDebounce)
		{
			object[] array = new object[6] { position, color.r, color.g, color.b, 1f, 1 };
			PhotonNetwork.RaiseEvent((byte)3, (object)new object[3]
			{
				PhotonNetwork.ServerTimestamp,
				(byte)1,
				array
			}, new RaiseEventOptions
			{
				Receivers = (ReceiverGroup)1
			}, SendOptions.SendUnreliable);
			if (projDebounceType > 0f)
			{
				projDebounce = Time.time + 0.1f;
			}
		}
	}

	public static void ChangeProjectile(bool positive = true)
	{
		string[] array = new string[39]
		{
			"Snowball", "Growing Snowball", "Water Balloon", "Lava Rock", "Present", "Science Candy", "Fish Food", "Apple", "Candy Corn", "Voting Rock",
			"Book", "Coin", "Egg", "Ice Cream", "Hot Dog", "Fireworks", "Paper", "Ice Cream Scoop", "Chips", "Salsa",
			"Apple Pie", "Mashed Potatoes", "Berry Pie", "Layer Dip", "Pumpkin Pie", "Stuffing", "Corn", "Turkey Leg", "Football", "Popcorn Ball",
			"Plate", "Stick", "Walnut", "Hot Cocoa", "Slingshot", "Pillow", "Cake", "Balloon Animal", "Energy Wafer"
		};
		if (!Settings.isLoadingPreferences)
		{
			if (positive)
			{
				projMode++;
			}
			else
			{
				projMode--;
			}
		}
		projMode %= array.Length;
		if (projMode < 0)
		{
			projMode = array.Length - 1;
		}
		Buttons.GetIndex("Change Projectile").overlapText = "Change Projectile <color=grey>[</color><color=green>" + array[projMode] + "</color><color=grey>]</color>";
	}

	public static void ChangeGrowingProjectile(bool positive = true)
	{
		string[] array = new string[3] { "Growing Snowball", "Mashed Potatoes", "Stuffing" };
		string[] array2 = new string[3] { "GrowingSnowball", "GrowingMashedPotato", "GrowingStuffing" };
		if (!Settings.isLoadingPreferences)
		{
			if (positive)
			{
				snowballIndex++;
			}
			else
			{
				snowballIndex--;
			}
		}
		snowballIndex %= array.Length;
		if (snowballIndex < 0)
		{
			snowballIndex = array.Length - 1;
		}
		Buttons.GetIndex("Change Growing Projectile").overlapText = "Change Growing Projectile <color=grey>[</color><color=green>" + array[snowballIndex] + "</color><color=grey>]</color>";
		SnowballName = array2[snowballIndex];
	}

	public static void ChangeProjectileIndex(bool positive = true)
	{
		if (!Settings.isLoadingPreferences)
		{
			if (positive)
			{
				targetProjectileIndex++;
			}
			else
			{
				targetProjectileIndex--;
			}
		}
		targetProjectileIndex %= 16;
		if (targetProjectileIndex < 0)
		{
			targetProjectileIndex = 15;
		}
		Buttons.GetIndex("Change Projectile Index").overlapText = "Change Projectile Index <color=grey>[</color><color=green>" + (targetProjectileIndex + 1) + "</color><color=grey>]</color>";
	}

	public static void ChangeShootSpeed(bool positive = true)
	{
		float[] array = new float[5] { 9.72f, 19.44f, 38.88f, 200f, 1000000f };
		string[] array2 = new string[5] { "Slow", "Medium", "Fast", "Ultra Fast", "Instant" };
		if (!Settings.isLoadingPreferences)
		{
			if (positive)
			{
				shootCycle++;
			}
			else
			{
				shootCycle--;
			}
		}
		shootCycle %= array.Length;
		if (shootCycle < 0)
		{
			shootCycle = array.Length - 1;
		}
		Main.ShootStrength = array[shootCycle];
		Buttons.GetIndex("Change Shoot Speed").overlapText = "Change Shoot Speed <color=grey>[</color><color=green>" + array2[shootCycle] + "</color><color=grey>]</color>";
	}

	public static void IncreaseRed(bool positive = true)
	{
		if (!Settings.isLoadingPreferences)
		{
			if (positive)
			{
				red++;
			}
			else
			{
				red--;
			}
		}
		red %= 11;
		if (red < 0)
		{
			red = 10;
		}
		Buttons.GetIndex("RedProj").overlapText = "Red <color=grey>[</color><color=green>" + red + "</color><color=grey>]</color>";
	}

	public static void IncreaseGreen(bool positive = true)
	{
		if (!Settings.isLoadingPreferences)
		{
			if (positive)
			{
				green++;
			}
			else
			{
				green--;
			}
		}
		green %= 11;
		if (green < 0)
		{
			green = 10;
		}
		Buttons.GetIndex("GreenProj").overlapText = "Green <color=grey>[</color><color=green>" + green + "</color><color=grey>]</color>";
	}

	public static void IncreaseBlue(bool positive = true)
	{
		if (!Settings.isLoadingPreferences)
		{
			if (positive)
			{
				blue++;
			}
			else
			{
				blue--;
			}
		}
		blue %= 11;
		if (blue < 0)
		{
			blue = 10;
		}
		Buttons.GetIndex("BlueProj").overlapText = "Blue <color=grey>[</color><color=green>" + blue + "</color><color=grey>]</color>";
	}

	public static void ChangeProjectileDelay(bool positive = true, bool fromMenu = false)
	{
		if (!Settings.isLoadingPreferences)
		{
			if (positive)
			{
				projDebounceIndex++;
			}
			else
			{
				projDebounceIndex--;
			}
		}
		projDebounceIndex %= 21;
		if (projDebounceIndex < 0)
		{
			projDebounceIndex = 20;
		}
		if (projDebounceIndex < 8 && fromMenu && (!Buttons.GetIndex("Friend Sided Projectiles").enabled || !Buttons.GetIndex("Client Sided Projectiles").enabled))
		{
			NotificationManager.SendNotification("<color=grey>[</color><color=red>WARNING</color><color=grey>]</color> Using a projectile delay lower than 0.8 could get you banned. Use at your own caution.", 5000);
		}
		projDebounceType = (float)projDebounceIndex / 20f;
		Overpowered.SnowballSpawnDelay = projDebounceType;
		Buttons.GetIndex("Change Projectile Delay").overlapText = "Change Projectile Delay <color=grey>[</color><color=green>" + projDebounceType + "</color><color=grey>]</color>";
	}

	public static Color CalculateProjectileColor()
	{
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		byte b = byte.MaxValue;
		byte b2 = byte.MaxValue;
		byte b3 = byte.MaxValue;
		if (Buttons.GetIndex("Random Color").enabled)
		{
			b = (byte)Random.Range(0, 255);
			b2 = (byte)Random.Range(0, 255);
			b3 = (byte)Random.Range(0, 255);
		}
		if (Buttons.GetIndex("Rainbow Projectiles").enabled)
		{
			float num = (float)Time.frameCount / 180f % 1f;
			Color val = Color.HSVToRGB(num, 1f, 1f);
			b = (byte)(val.r * 255f);
			b2 = (byte)(val.g * 255f);
			b3 = (byte)(val.b * 255f);
		}
		if (Buttons.GetIndex("Hard Rainbow Projectiles").enabled)
		{
			float num2 = (float)Time.frameCount / 180f % 1f;
			Color val2 = Color.HSVToRGB(num2, 1f, 1f);
			b = (byte)(Mathf.Floor(val2.r * 2f) / 2f * 255f);
			b2 = (byte)(Mathf.Floor(val2.g * 2f) / 2f * 255f);
			b3 = (byte)(Mathf.Floor(val2.b * 2f) / 2f * 255f);
		}
		if (Buttons.GetIndex("Custom Colored Projectiles").enabled)
		{
			b = (byte)((float)red / 10f * 255f);
			b2 = (byte)((float)green / 10f * 255f);
			b3 = (byte)((float)blue / 10f * 255f);
		}
		return Color32.op_Implicit(new Color32(b, b2, b3, byte.MaxValue));
	}

	public static void ProjectileSpam()
	{
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_023e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0243: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_026e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0273: Unknown result type (might be due to invalid IL or missing references)
		//IL_0278: Unknown result type (might be due to invalid IL or missing references)
		//IL_027d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0202: Unknown result type (might be due to invalid IL or missing references)
		//IL_0206: Unknown result type (might be due to invalid IL or missing references)
		//IL_0210: Unknown result type (might be due to invalid IL or missing references)
		//IL_021a: Unknown result type (might be due to invalid IL or missing references)
		//IL_021f: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02de: Unknown result type (might be due to invalid IL or missing references)
		//IL_030e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0332: Unknown result type (might be due to invalid IL or missing references)
		//IL_0337: Unknown result type (might be due to invalid IL or missing references)
		//IL_033c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0364: Unknown result type (might be due to invalid IL or missing references)
		//IL_036e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0373: Unknown result type (might be due to invalid IL or missing references)
		//IL_0378: Unknown result type (might be due to invalid IL or missing references)
		//IL_037f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0384: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_0454: Unknown result type (might be due to invalid IL or missing references)
		//IL_0456: Unknown result type (might be due to invalid IL or missing references)
		//IL_0458: Unknown result type (might be due to invalid IL or missing references)
		//IL_043a: Unknown result type (might be due to invalid IL or missing references)
		//IL_044b: Unknown result type (might be due to invalid IL or missing references)
		//IL_041d: Unknown result type (might be due to invalid IL or missing references)
		//IL_042e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0450: Unknown result type (might be due to invalid IL or missing references)
		if (Time.time > lastProjSpamNotify + 3f)
		{
			NotificationManager.SendNotification("<color=grey>[</color><color=yellow>DEBUG</color><color=grey>]</color> ProjectileSpam called. rightGrab=" + Main.rightGrab + " leftGrab=" + Main.leftGrab);
			lastProjSpamNotify = Time.time;
		}
		int num = projMode * 2;
		bool flag = (Buttons.GetIndex("Left Handed Projectiles").enabled || Buttons.GetIndex("Both Handed Projectiles").enabled) && Main.leftGrab;
		bool flag2 = Main.rightGrab || (Mouse.current != null && Mouse.current.leftButton.isPressed);
		if (Buttons.GetIndex("Both Handed Projectiles").enabled)
		{
			flag = Main.leftGrab;
			flag2 = Main.rightGrab;
		}
		if (!(flag || flag2))
		{
			return;
		}
		if (Buttons.GetIndex("Random Projectile").enabled)
		{
			num = Random.Range(0, ProjectileObjectNames.Length);
		}
		string projectileName = ProjectileObjectNames[num];
		Transform[] array = (Transform[])(object)new Transform[2]
		{
			GorillaTagger.Instance.leftHandTransform,
			GorillaTagger.Instance.rightHandTransform
		};
		bool[] array2 = new bool[2] { flag, flag2 };
		RaycastHit val3 = default(RaycastHit);
		for (int i = 0; i < 2; i++)
		{
			if (!array2[i])
			{
				continue;
			}
			Vector3 position = array[i].position;
			Vector3 val = GTPlayer.Instance.RigidbodyVelocity;
			if (Buttons.GetIndex("Shoot Projectiles").enabled)
			{
				val += Main.GetGunDirection(array[i]) * Main.ShootStrength;
				if (Mouse.current.leftButton.isPressed)
				{
					Ray val2 = Main.TPC.ScreenPointToRay(Vector2.op_Implicit(((InputControl<Vector2>)(object)((Pointer)Mouse.current).position).ReadValue()));
					if (Physics.Raycast(val2, ref val3, 512f, Main.NoInvisLayerMask()))
					{
						Vector3 val4 = ((RaycastHit)(ref val3)).point - array[i].position;
						val = ((Vector3)(ref val4)).normalized * Main.ShootStrength * 2f;
					}
				}
			}
			if (Buttons.GetIndex("Random Direction").enabled)
			{
				val = RandomUtilities.RandomVector3(100f);
			}
			if (Buttons.GetIndex("Above Players").enabled)
			{
				VRRig targetPlayer = RigUtilities.GetTargetPlayer();
				position = ((Component)targetPlayer).transform.position + Vector3.up;
			}
			if (Buttons.GetIndex("Rain Projectiles").enabled)
			{
				position = ((Component)GorillaTagger.Instance.headCollider).transform.position + new Vector3(Random.Range(-2f, 2f), 2f, Random.Range(-2f, 2f));
				val = Vector3.zero;
			}
			if (Buttons.GetIndex("Projectile Aura").enabled)
			{
				float num2 = Time.frameCount;
				position = ((Component)GorillaTagger.Instance.headCollider).transform.position + new Vector3(MathF.Cos(num2 / 20f), 2f, MathF.Sin(num2 / 20f));
			}
			if (Buttons.GetIndex("True Projectile Aura").enabled)
			{
				position = ((Component)GorillaTagger.Instance.headCollider).transform.position + RandomUtilities.RandomVector3();
				val = RandomUtilities.RandomVector3(10f);
			}
			if (Buttons.GetIndex("Projectile Fountain").enabled)
			{
				position = ((Component)GorillaTagger.Instance.headCollider).transform.position + new Vector3(0f, 1f, 0f);
				((Vector3)(ref val))._002Ector((float)Random.Range(-10, 10), 15f, (float)Random.Range(-10, 10));
			}
			if (Buttons.GetIndex("Include Hand Velocity").enabled)
			{
				val = (((Object)(object)array[i] == (Object)(object)GorillaTagger.Instance.rightHandTransform) ? GTPlayer.Instance.RightHand.velocityTracker.GetAverageVelocity(true, 0f, false) : GTPlayer.Instance.LeftHand.velocityTracker.GetAverageVelocity(true, 0f, false));
			}
			BetaFireProjectile(projectileName, position, val, CalculateProjectileColor(), null, bypassTeleport: true);
		}
	}

	public static void ProjectileGun()
	{
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0208: Unknown result type (might be due to invalid IL or missing references)
		//IL_020d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0212: Unknown result type (might be due to invalid IL or missing references)
		//IL_0214: Unknown result type (might be due to invalid IL or missing references)
		//IL_0219: Unknown result type (might be due to invalid IL or missing references)
		//IL_0249: Unknown result type (might be due to invalid IL or missing references)
		//IL_026d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0272: Unknown result type (might be due to invalid IL or missing references)
		//IL_0277: Unknown result type (might be due to invalid IL or missing references)
		//IL_029f: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0300: Unknown result type (might be due to invalid IL or missing references)
		//IL_0305: Unknown result type (might be due to invalid IL or missing references)
		//IL_035c: Unknown result type (might be due to invalid IL or missing references)
		//IL_035e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0360: Unknown result type (might be due to invalid IL or missing references)
		//IL_0342: Unknown result type (might be due to invalid IL or missing references)
		//IL_0353: Unknown result type (might be due to invalid IL or missing references)
		//IL_0358: Unknown result type (might be due to invalid IL or missing references)
		int num = projMode * 2;
		if (!Main.GetGunInput(isShooting: false))
		{
			return;
		}
		GameObject item = Main.RenderGun().NewPointer;
		if (!Main.GetGunInput(isShooting: true))
		{
			return;
		}
		if (Buttons.GetIndex("Random Projectile").enabled)
		{
			num = Random.Range(0, ProjectileObjectNames.Length);
		}
		string projectileName = ProjectileObjectNames[num];
		Vector3 position = item.transform.position + Vector3.up;
		Vector3 val = Vector3.up * 30f;
		if (Buttons.GetIndex("Shoot Projectiles").enabled)
		{
			val = GTPlayer.Instance.RigidbodyVelocity + Main.GetGunDirection(GorillaTagger.Instance.rightHandTransform) * Main.ShootStrength;
			if (Mouse.current.leftButton.isPressed)
			{
				Ray val2 = Main.TPC.ScreenPointToRay(Vector2.op_Implicit(((InputControl<Vector2>)(object)((Pointer)Mouse.current).position).ReadValue()));
				RaycastHit val3 = default(RaycastHit);
				Physics.Raycast(val2, ref val3, 512f, Main.NoInvisLayerMask());
				val = ((RaycastHit)(ref val3)).point - ((Component)GorillaTagger.Instance.rightHandTransform).transform.position;
				((Vector3)(ref val)).Normalize();
				val *= Main.ShootStrength * 2f;
			}
		}
		if (Buttons.GetIndex("Random Direction").enabled)
		{
			val = RandomUtilities.RandomVector3(100f);
		}
		if (Buttons.GetIndex("Above Players").enabled)
		{
			VRRig targetPlayer = RigUtilities.GetTargetPlayer();
			position = ((Component)targetPlayer).transform.position + Vector3.up;
		}
		if (Buttons.GetIndex("Rain Projectiles").enabled)
		{
			position = ((Component)GorillaTagger.Instance.headCollider).transform.position + new Vector3(Random.Range(-2f, 2f), 2f, Random.Range(-2f, 2f));
			val = Vector3.zero;
		}
		if (Buttons.GetIndex("Projectile Aura").enabled)
		{
			float num2 = Time.frameCount;
			position = ((Component)GorillaTagger.Instance.headCollider).transform.position + new Vector3(MathF.Cos(num2 / 20f), 2f, MathF.Sin(num2 / 20f));
		}
		if (Buttons.GetIndex("True Projectile Aura").enabled)
		{
			position = ((Component)GorillaTagger.Instance.headCollider).transform.position + RandomUtilities.RandomVector3();
			val = RandomUtilities.RandomVector3(10f);
		}
		if (Buttons.GetIndex("Projectile Fountain").enabled)
		{
			position = ((Component)GorillaTagger.Instance.headCollider).transform.position + new Vector3(0f, 1f, 0f);
			((Vector3)(ref val))._002Ector((float)Random.Range(-10, 10), 15f, (float)Random.Range(-10, 10));
		}
		if (Buttons.GetIndex("Include Hand Velocity").enabled)
		{
			val = GTPlayer.Instance.RightHand.velocityTracker.GetAverageVelocity(true, 0f, false);
		}
		BetaFireProjectile(projectileName, position, val, CalculateProjectileColor());
	}

	public static void LazerSpam()
	{
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0204: Unknown result type (might be due to invalid IL or missing references)
		//IL_0209: Unknown result type (might be due to invalid IL or missing references)
		//IL_020a: Unknown result type (might be due to invalid IL or missing references)
		//IL_020f: Unknown result type (might be due to invalid IL or missing references)
		//IL_023f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0263: Unknown result type (might be due to invalid IL or missing references)
		//IL_0268: Unknown result type (might be due to invalid IL or missing references)
		//IL_026d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0294: Unknown result type (might be due to invalid IL or missing references)
		//IL_029e: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02db: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_034e: Unknown result type (might be due to invalid IL or missing references)
		//IL_034f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0351: Unknown result type (might be due to invalid IL or missing references)
		//IL_0335: Unknown result type (might be due to invalid IL or missing references)
		//IL_0346: Unknown result type (might be due to invalid IL or missing references)
		//IL_034b: Unknown result type (might be due to invalid IL or missing references)
		int num = projMode * 2;
		if (!Main.rightGrab && !Mouse.current.leftButton.isPressed)
		{
			return;
		}
		if (Buttons.GetIndex("Random Projectile").enabled)
		{
			num = Random.Range(0, ProjectileObjectNames.Length);
		}
		string projectileName = ProjectileObjectNames[num];
		Vector3 position = ((Component)GorillaTagger.Instance.headCollider).transform.position;
		Vector3 val = ((Component)GorillaTagger.Instance.headCollider).transform.forward * 30f;
		if (Buttons.GetIndex("Shoot Projectiles").enabled)
		{
			val = GTPlayer.Instance.RigidbodyVelocity + Main.GetGunDirection(GorillaTagger.Instance.rightHandTransform) * Main.ShootStrength;
			if (Mouse.current.leftButton.isPressed)
			{
				Ray val2 = Main.TPC.ScreenPointToRay(Vector2.op_Implicit(((InputControl<Vector2>)(object)((Pointer)Mouse.current).position).ReadValue()));
				RaycastHit val3 = default(RaycastHit);
				Physics.Raycast(val2, ref val3, 512f, Main.NoInvisLayerMask());
				val = ((RaycastHit)(ref val3)).point - ((Component)GorillaTagger.Instance.rightHandTransform).transform.position;
				((Vector3)(ref val)).Normalize();
				val *= Main.ShootStrength * 2f;
			}
		}
		if (Buttons.GetIndex("Random Direction").enabled)
		{
			val = RandomUtilities.RandomVector3(100f);
		}
		if (Buttons.GetIndex("Above Players").enabled)
		{
			VRRig targetPlayer = RigUtilities.GetTargetPlayer();
			position = ((Component)targetPlayer).transform.position + Vector3.up;
		}
		if (Buttons.GetIndex("Rain Projectiles").enabled)
		{
			position = ((Component)GorillaTagger.Instance.headCollider).transform.position + new Vector3(Random.Range(-2f, 2f), 2f, Random.Range(-2f, 2f));
			val = Vector3.zero;
		}
		if (Buttons.GetIndex("Projectile Aura").enabled)
		{
			float num2 = Time.frameCount;
			position = ((Component)GorillaTagger.Instance.headCollider).transform.position + new Vector3(MathF.Cos(num2 / 20f), 2f, MathF.Sin(num2 / 20f));
		}
		if (Buttons.GetIndex("True Projectile Aura").enabled)
		{
			position = ((Component)GorillaTagger.Instance.headCollider).transform.position + RandomUtilities.RandomVector3();
			val = RandomUtilities.RandomVector3(10f);
		}
		if (Buttons.GetIndex("Projectile Fountain").enabled)
		{
			position = ((Component)GorillaTagger.Instance.headCollider).transform.position + new Vector3(0f, 1f, 0f);
			((Vector3)(ref val))._002Ector((float)Random.Range(-10, 10), 15f, (float)Random.Range(-10, 10));
		}
		if (Buttons.GetIndex("Include Hand Velocity").enabled)
		{
			val = GTPlayer.Instance.RightHand.velocityTracker.GetAverageVelocity(true, 0f, false);
		}
		BetaFireProjectile(projectileName, position, val, CalculateProjectileColor());
	}

	public static void GiveProjectileSpamGun()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0221: Unknown result type (might be due to invalid IL or missing references)
		//IL_022b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0230: Unknown result type (might be due to invalid IL or missing references)
		//IL_0235: Unknown result type (might be due to invalid IL or missing references)
		//IL_023c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0241: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0269: Unknown result type (might be due to invalid IL or missing references)
		//IL_027d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0282: Unknown result type (might be due to invalid IL or missing references)
		//IL_0287: Unknown result type (might be due to invalid IL or missing references)
		if (Main.GetGunInput(isShooting: false))
		{
			var (val, _) = Main.RenderGun();
			if (Main.gunLocked && (Object)(object)Main.lockTarget != (Object)null)
			{
				int num = projMode * 2;
				if (Buttons.GetIndex("Random Projectile").enabled)
				{
					num = Random.Range(0, ProjectileObjectNames.Length);
				}
				string projectileName = ProjectileObjectNames[num];
				Vector3 position = Main.lockTarget.rightHandTransform.position;
				Vector3 velocity = Vector3.zero;
				if (Buttons.GetIndex("Shoot Projectiles").enabled)
				{
					velocity = ((Component)Main.lockTarget.rightHandTransform).transform.forward * Main.ShootStrength;
				}
				if (Buttons.GetIndex("Random Direction").enabled)
				{
					((Vector3)(ref velocity))._002Ector((float)Random.Range(-33, 33), (float)Random.Range(-33, 33), (float)Random.Range(-33, 33));
				}
				if (Buttons.GetIndex("Above Players").enabled)
				{
					VRRig targetPlayer = RigUtilities.GetTargetPlayer();
					position = ((Component)targetPlayer).transform.position + Vector3.up;
				}
				if (Buttons.GetIndex("Rain Projectiles").enabled)
				{
					position = Main.lockTarget.headMesh.transform.position + new Vector3(Random.Range(-3f, 3f), 3f, Random.Range(-3f, 3f));
					velocity = Vector3.zero;
				}
				if (Buttons.GetIndex("Projectile Aura").enabled)
				{
					float num2 = Time.frameCount;
					position = Main.lockTarget.headMesh.transform.position + new Vector3(MathF.Cos(num2 / 20f), 2f, MathF.Sin(num2 / 20f));
				}
				if (Buttons.GetIndex("True Projectile Aura").enabled)
				{
					position = ((Component)GorillaTagger.Instance.headCollider).transform.position + RandomUtilities.RandomVector3();
					velocity = RandomUtilities.RandomVector3(10f);
				}
				if (Buttons.GetIndex("Projectile Fountain").enabled)
				{
					position = Main.lockTarget.headMesh.transform.position + new Vector3(0f, 1f, 0f);
					((Vector3)(ref velocity))._002Ector((float)Random.Range(-10, 10), -15f, (float)Random.Range(-10, 10));
				}
				BetaFireProjectile(projectileName, position, velocity, CalculateProjectileColor());
			}
			if (Main.GetGunInput(isShooting: true))
			{
				VRRig componentInParent = ((Component)((RaycastHit)(ref val)).collider).GetComponentInParent<VRRig>();
				if (Object.op_Implicit((Object)(object)componentInParent) && !componentInParent.IsLocal())
				{
					Main.gunLocked = true;
					Main.lockTarget = componentInParent;
				}
			}
		}
		else if (Main.gunLocked)
		{
			Main.gunLocked = false;
			((Behaviour)VRRig.LocalRig).enabled = true;
		}
	}

	public static void ImpactSpam()
	{
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0202: Unknown result type (might be due to invalid IL or missing references)
		//IL_0247: Unknown result type (might be due to invalid IL or missing references)
		//IL_0248: Unknown result type (might be due to invalid IL or missing references)
		//IL_0228: Unknown result type (might be due to invalid IL or missing references)
		//IL_023c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0241: Unknown result type (might be due to invalid IL or missing references)
		//IL_0246: Unknown result type (might be due to invalid IL or missing references)
		if ((!Main.rightGrab && !Mouse.current.leftButton.isPressed) || !(Time.time > projDebounce))
		{
			return;
		}
		Vector3 position = GorillaTagger.Instance.rightHandTransform.position;
		if (Buttons.GetIndex("Shoot Projectiles").enabled)
		{
			RaycastHit val = default(RaycastHit);
			Physics.Raycast(GorillaTagger.Instance.rightHandTransform.position, Main.GetGunDirection(GorillaTagger.Instance.rightHandTransform), ref val, 512f, Main.NoInvisLayerMask());
			if (Mouse.current.leftButton.isPressed)
			{
				Ray val2 = Main.TPC.ScreenPointToRay(Vector2.op_Implicit(((InputControl<Vector2>)(object)((Pointer)Mouse.current).position).ReadValue()));
				Physics.Raycast(val2, ref val, 512f, Main.NoInvisLayerMask());
			}
			position = ((RaycastHit)(ref val)).point;
		}
		if (Buttons.GetIndex("Above Players").enabled)
		{
			VRRig targetPlayer = RigUtilities.GetTargetPlayer();
			position = ((Component)targetPlayer).transform.position + Vector3.up;
		}
		if (Buttons.GetIndex("Rain Projectiles").enabled)
		{
			position = ((Component)GorillaTagger.Instance.headCollider).transform.position + new Vector3(Random.Range(-3f, 3f), 3f, Random.Range(-3f, 3f));
		}
		if (Buttons.GetIndex("Projectile Aura").enabled)
		{
			float num = Time.frameCount;
			position = ((Component)GorillaTagger.Instance.headCollider).transform.position + new Vector3(MathF.Cos(num / 20f), 2f, MathF.Sin(num / 20f));
		}
		if (Buttons.GetIndex("True Projectile Aura").enabled)
		{
			position = ((Component)GorillaTagger.Instance.headCollider).transform.position + RandomUtilities.RandomVector3();
		}
		if (Buttons.GetIndex("Projectile Fountain").enabled)
		{
			position = ((Component)GorillaTagger.Instance.headCollider).transform.position + new Vector3(0f, 1f, 0f);
		}
		BetaFireImpact(position, CalculateProjectileColor());
		Main.RPCProtection();
		if (projDebounceType > 0f)
		{
			projDebounce = Time.time + projDebounceType + 0.05f;
		}
	}

	private static void HandleGrabProjectile(bool leftHand)
	{
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		SnowballMaker val = (leftHand ? SnowballMaker.leftHandInstance : SnowballMaker.rightHandInstance);
		bool flag = (leftHand ? Main.leftGrab : Main.rightGrab);
		previousGripHeld.TryGetValue(leftHand, out var value);
		if (flag && !value)
		{
			int num = projMode * 2;
			if (Buttons.GetIndex("Random Projectile").enabled)
			{
				num = Random.Range(0, ProjectileObjectNames.Length / 2) * 2;
			}
			SnowballThrowable projectile = Main.GetProjectile(ProjectileObjectNames[num + ((!leftHand) ? 1 : 0)]);
			if (!((Component)projectile).gameObject.activeSelf)
			{
				projectile.SetSnowballActiveLocal(true);
				projectile.velocityEstimator = val.velocityEstimator;
				Transform handTransform = val.handTransform;
				((Component)projectile).transform.position = handTransform.TransformPoint(projectile.SpawnOffset.pos);
				Transform transform = ((Component)projectile).transform;
				Quaternion rotation = handTransform.rotation;
				XformOffset spawnOffset = projectile.SpawnOffset;
				transform.rotation = rotation * ((XformOffset)(ref spawnOffset)).rot;
				Color val2 = CalculateProjectileColor();
				VRRig.LocalRig.SetThrowableProjectileColor(true, Color32.op_Implicit(CalculateProjectileColor()));
				bool randomizeColor = projectile.randomizeColor;
				projectile.randomizeColor = true;
				projectile.ApplyColor(val2);
				projectile.randomizeColor = randomizeColor;
			}
		}
		previousGripHeld[leftHand] = flag;
	}

	public static void GrabProjectile()
	{
		HandleGrabProjectile(leftHand: true);
		HandleGrabProjectile(leftHand: false);
	}

	public static void Urine()
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		if (Main.rightGrab || Mouse.current.leftButton.isPressed)
		{
			Vector3 position = ((Component)GorillaTagger.Instance.bodyCollider).transform.position + new Vector3(0f, -0.15f, 0f);
			Vector3 velocity = ((Component)GorillaTagger.Instance.bodyCollider).transform.forward * 8.33f;
			BetaFireProjectile("ScienceCandyLeftAnchor", position, velocity, Color.yellow);
		}
	}

	public static void Feces()
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		if (Main.rightGrab || Mouse.current.leftButton.isPressed)
		{
			Vector3 position = ((Component)GorillaTagger.Instance.bodyCollider).transform.position + new Vector3(0f, -0.3f, 0f);
			Vector3 zero = Vector3.zero;
			BetaFireProjectile("FishFoodLeftAnchor", position, zero, Color.brown);
		}
	}

	public static void Period()
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		if (Main.rightGrab || Mouse.current.leftButton.isPressed)
		{
			Vector3 position = ((Component)GorillaTagger.Instance.bodyCollider).transform.position + new Vector3(0f, -0.3f, 0f);
			Vector3 zero = Vector3.zero;
			BetaFireProjectile("IceCreamScoopRightAnchor", position, zero, Color.red);
		}
	}

	public static void Semen()
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		if (Main.rightGrab || Mouse.current.leftButton.isPressed)
		{
			Vector3 position = ((Component)GorillaTagger.Instance.bodyCollider).transform.position + new Vector3(0f, -0.15f, 0f);
			Vector3 velocity = ((Component)GorillaTagger.Instance.bodyCollider).transform.forward * 8.33f;
			BetaFireProjectile("ScienceCandyLeftAnchor", position, velocity, Color.ghostWhite);
		}
	}

	public static void Vomit()
	{
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		if (Main.rightGrab || Mouse.current.leftButton.isPressed)
		{
			Vector3 position = ((Component)GorillaTagger.Instance.headCollider).transform.position + ((Component)GorillaTagger.Instance.headCollider).transform.forward * 0.1f + ((Component)GorillaTagger.Instance.headCollider).transform.up * -0.15f;
			Vector3 velocity = ((Component)GorillaTagger.Instance.headCollider).transform.forward * 8.33f;
			BetaFireProjectile("FishFoodLeftAnchor", position, velocity, Color.green);
		}
	}

	public static void Spit()
	{
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		if (Main.rightGrab || Mouse.current.leftButton.isPressed)
		{
			Vector3 position = ((Component)GorillaTagger.Instance.headCollider).transform.position + ((Component)GorillaTagger.Instance.headCollider).transform.forward * 0.1f + ((Component)GorillaTagger.Instance.headCollider).transform.up * -0.15f;
			Vector3 velocity = ((Component)GorillaTagger.Instance.headCollider).transform.forward * 8.33f;
			BetaFireProjectile("WaterBalloonLeftAnchor", position, velocity, Color.cyan);
		}
	}

	public static void LazerEyes()
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		if (Main.rightGrab || Mouse.current.leftButton.isPressed)
		{
			Vector3 position = ((Component)GorillaTagger.Instance.headCollider).transform.position;
			Vector3 velocity = ((Component)GorillaTagger.Instance.headCollider).transform.forward * 30f;
			BetaFireProjectile("Walnut_Anchor_Right", position, velocity, Color.red);
		}
	}

	public static void UrineGun()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		if (Main.GetGunInput(isShooting: false))
		{
			var (val, _) = Main.RenderGun();
			if (Main.gunLocked && (Object)(object)Main.lockTarget != (Object)null)
			{
				Vector3 position = ((Component)Main.lockTarget).transform.position + new Vector3(0f, -0.4f, 0f) + ((Component)Main.lockTarget).transform.forward * 0.2f;
				Vector3 velocity = ((Component)Main.lockTarget).transform.forward * 8.33f;
				BetaFireProjectile("ScienceCandyLeftAnchor", position, velocity, Color.yellow);
			}
			if (Main.GetGunInput(isShooting: true))
			{
				VRRig componentInParent = ((Component)((RaycastHit)(ref val)).collider).GetComponentInParent<VRRig>();
				if (Object.op_Implicit((Object)(object)componentInParent) && !componentInParent.IsLocal())
				{
					Main.gunLocked = true;
					Main.lockTarget = componentInParent;
				}
			}
		}
		else if (Main.gunLocked)
		{
			Main.gunLocked = false;
			((Behaviour)VRRig.LocalRig).enabled = true;
		}
	}

	public static void FecesGun()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		if (Main.GetGunInput(isShooting: false))
		{
			var (val, _) = Main.RenderGun();
			if (Main.gunLocked && (Object)(object)Main.lockTarget != (Object)null)
			{
				Vector3 position = ((Component)Main.lockTarget).transform.position + new Vector3(0f, -0.65f, 0f);
				Vector3 zero = Vector3.zero;
				BetaFireProjectile("FishFoodLeftAnchor", position, zero, Color.brown);
			}
			if (Main.GetGunInput(isShooting: true))
			{
				VRRig componentInParent = ((Component)((RaycastHit)(ref val)).collider).GetComponentInParent<VRRig>();
				if (Object.op_Implicit((Object)(object)componentInParent) && !componentInParent.IsLocal())
				{
					Main.gunLocked = true;
					Main.lockTarget = componentInParent;
				}
			}
		}
		else if (Main.gunLocked)
		{
			Main.gunLocked = false;
			((Behaviour)VRRig.LocalRig).enabled = true;
		}
	}

	public static void PeriodGun()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		if (Main.GetGunInput(isShooting: false))
		{
			var (val, _) = Main.RenderGun();
			if (Main.gunLocked && (Object)(object)Main.lockTarget != (Object)null)
			{
				Vector3 position = ((Component)Main.lockTarget).transform.position + new Vector3(0f, -0.65f, 0f);
				Vector3 zero = Vector3.zero;
				BetaFireProjectile("IceCreamScoopRightAnchor", position, zero, Color.red);
			}
			if (Main.GetGunInput(isShooting: true))
			{
				VRRig componentInParent = ((Component)((RaycastHit)(ref val)).collider).GetComponentInParent<VRRig>();
				if (Object.op_Implicit((Object)(object)componentInParent) && !componentInParent.IsLocal())
				{
					Main.gunLocked = true;
					Main.lockTarget = componentInParent;
				}
			}
		}
		else if (Main.gunLocked)
		{
			Main.gunLocked = false;
			((Behaviour)VRRig.LocalRig).enabled = true;
		}
	}

	public static void SemenGun()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		if (Main.GetGunInput(isShooting: false))
		{
			var (val, _) = Main.RenderGun();
			if (Main.gunLocked && (Object)(object)Main.lockTarget != (Object)null)
			{
				Vector3 position = ((Component)Main.lockTarget).transform.position + new Vector3(0f, -0.4f, 0f) + ((Component)Main.lockTarget).transform.forward * 0.2f;
				Vector3 velocity = ((Component)Main.lockTarget).transform.forward * 8.33f;
				BetaFireProjectile("ScienceCandyLeftAnchor", position, velocity, Color.ghostWhite);
			}
			if (Main.GetGunInput(isShooting: true))
			{
				VRRig componentInParent = ((Component)((RaycastHit)(ref val)).collider).GetComponentInParent<VRRig>();
				if (Object.op_Implicit((Object)(object)componentInParent) && !componentInParent.IsLocal())
				{
					Main.gunLocked = true;
					Main.lockTarget = componentInParent;
				}
			}
		}
		else if (Main.gunLocked)
		{
			Main.gunLocked = false;
			((Behaviour)VRRig.LocalRig).enabled = true;
		}
	}

	public static void VomitGun()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		if (Main.GetGunInput(isShooting: false))
		{
			var (val, _) = Main.RenderGun();
			if (Main.gunLocked && (Object)(object)Main.lockTarget != (Object)null)
			{
				Vector3 position = Main.lockTarget.headMesh.transform.position + Main.lockTarget.headMesh.transform.forward * 0.4f + Main.lockTarget.headMesh.transform.up * -0.05f;
				Vector3 velocity = Main.lockTarget.headMesh.transform.forward * 8.33f;
				BetaFireProjectile("FishFoodLeftAnchor", position, velocity, Color.green);
			}
			if (Main.GetGunInput(isShooting: true))
			{
				VRRig componentInParent = ((Component)((RaycastHit)(ref val)).collider).GetComponentInParent<VRRig>();
				if (Object.op_Implicit((Object)(object)componentInParent) && !componentInParent.IsLocal())
				{
					Main.gunLocked = true;
					Main.lockTarget = componentInParent;
				}
			}
		}
		else if (Main.gunLocked)
		{
			Main.gunLocked = false;
			((Behaviour)VRRig.LocalRig).enabled = true;
		}
	}

	public static void SpitGun()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		if (Main.GetGunInput(isShooting: false))
		{
			var (val, _) = Main.RenderGun();
			if (Main.gunLocked && (Object)(object)Main.lockTarget != (Object)null)
			{
				Vector3 position = Main.lockTarget.headMesh.transform.position + Main.lockTarget.headMesh.transform.forward * 0.4f + Main.lockTarget.headMesh.transform.up * -0.05f;
				Vector3 velocity = Main.lockTarget.headMesh.transform.forward * 8.33f;
				BetaFireProjectile("WaterBalloonLeftAnchor", position, velocity, Color.cyan);
			}
			if (Main.GetGunInput(isShooting: true))
			{
				VRRig componentInParent = ((Component)((RaycastHit)(ref val)).collider).GetComponentInParent<VRRig>();
				if (Object.op_Implicit((Object)(object)componentInParent) && !componentInParent.IsLocal())
				{
					Main.gunLocked = true;
					Main.lockTarget = componentInParent;
				}
			}
		}
		else if (Main.gunLocked)
		{
			Main.gunLocked = false;
			((Behaviour)VRRig.LocalRig).enabled = true;
		}
	}

	public static void LazerEyesGun()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		if (Main.GetGunInput(isShooting: false))
		{
			var (val, _) = Main.RenderGun();
			if (Main.gunLocked && (Object)(object)Main.lockTarget != (Object)null)
			{
				Vector3 position = Main.lockTarget.headMesh.transform.position + Main.lockTarget.headMesh.transform.forward * 0.4f + Main.lockTarget.headMesh.transform.up * -0.05f;
				Vector3 velocity = Main.lockTarget.headMesh.transform.forward * 30f;
				BetaFireProjectile("Walnut_Anchor_Right", position, velocity, Color.red);
			}
			if (Main.GetGunInput(isShooting: true))
			{
				VRRig componentInParent = ((Component)((RaycastHit)(ref val)).collider).GetComponentInParent<VRRig>();
				if (Object.op_Implicit((Object)(object)componentInParent) && !componentInParent.IsLocal())
				{
					Main.gunLocked = true;
					Main.lockTarget = componentInParent;
				}
			}
		}
		else if (Main.gunLocked)
		{
			Main.gunLocked = false;
			((Behaviour)VRRig.LocalRig).enabled = true;
		}
	}

	public static void ProjectileBlindGun()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		if (Main.GetGunInput(isShooting: false))
		{
			var (val, _) = Main.RenderGun();
			if (Main.gunLocked && (Object)(object)Main.lockTarget != (Object)null)
			{
				ProjectileBlindPlayer(Main.lockTarget);
			}
			if (Main.GetGunInput(isShooting: true))
			{
				VRRig componentInParent = ((Component)((RaycastHit)(ref val)).collider).GetComponentInParent<VRRig>();
				if (Object.op_Implicit((Object)(object)componentInParent) && !componentInParent.IsLocal())
				{
					Main.gunLocked = true;
					Main.lockTarget = componentInParent;
				}
			}
		}
		else if (Main.gunLocked)
		{
			Main.gunLocked = false;
		}
	}

	public static void ProjectileBlindAll()
	{
		SerializePatch.OverrideSerialization = delegate
		{
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Expected O, but got Unknown
			//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0104: Unknown result type (might be due to invalid IL or missing references)
			//IL_0109: Unknown result type (might be due to invalid IL or missing references)
			//IL_010e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0115: Expected O, but got Unknown
			//IL_0169: Unknown result type (might be due to invalid IL or missing references)
			if (PhotonNetwork.InRoom)
			{
				Main.MassSerialize(exclude: true, (PhotonView[])(object)new PhotonView[1] { VRRig.LocalRig.GetPhotonView() });
				Vector3 position = ((Component)VRRig.LocalRig).transform.position;
				NetPlayer[] playerListOthers = NetworkSystem.Instance.PlayerListOthers;
				foreach (NetPlayer val in playerListOthers)
				{
					VRRig vRRigFromPlayer = RigUtilities.GetVRRigFromPlayer(val);
					((Component)VRRig.LocalRig).transform.position = ((Component)vRRigFromPlayer).transform.position - Vector3.one * 3f;
					PhotonView photonView = VRRig.LocalRig.GetPhotonView();
					RaiseEventOptions val2 = new RaiseEventOptions();
					val2.TargetActors = new int[1] { val.ActorNumber };
					Main.SendSerialize(photonView, val2);
					Vector3 position2 = vRRigFromPlayer.headMesh.transform.position + new Vector3(0f, 0.1f, 0f);
					Vector3 velocity = new Vector3(0f, -15f, 0f);
					Color black = Color.black;
					val2 = new RaiseEventOptions();
					val2.TargetActors = new int[1] { RigUtilities.NetPlayerToPlayer(RigUtilities.GetPlayerFromVRRig(vRRigFromPlayer)).ActorNumber };
					BetaFireProjectile("EggLeftHand_Anchor Variant", position2, velocity, black, val2, bypassTeleport: true);
				}
				Main.RPCProtection();
				((Behaviour)VRRig.LocalRig).enabled = true;
				((Component)VRRig.LocalRig).transform.position = position;
				return false;
			}
			return true;
		};
	}

	public static void ProjectileBlindPlayer(NetPlayer player)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Expected O, but got Unknown
		VRRig vRRigFromPlayer = RigUtilities.GetVRRigFromPlayer(player);
		Vector3 position = vRRigFromPlayer.headMesh.transform.position + new Vector3(0f, 0.1f, 0f);
		Vector3 velocity = new Vector3(0f, -15f, 0f);
		Color black = Color.black;
		RaiseEventOptions val = new RaiseEventOptions();
		val.TargetActors = new int[1] { RigUtilities.NetPlayerToPlayer(RigUtilities.GetPlayerFromVRRig(vRRigFromPlayer)).ActorNumber };
		BetaFireProjectile("EggLeftHand_Anchor Variant", position, velocity, black, val);
	}

	public static void ProjectileBlindPlayer(VRRig player)
	{
		ProjectileBlindPlayer(RigUtilities.GetPlayerFromVRRig(player));
	}

	public static void ProjectileLagGun()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		if (Main.GetGunInput(isShooting: false))
		{
			var (val, _) = Main.RenderGun();
			if (Main.gunLocked && (Object)(object)Main.lockTarget != (Object)null)
			{
				ProjectileLagPlayer(Main.lockTarget);
			}
			if (Main.GetGunInput(isShooting: true))
			{
				VRRig componentInParent = ((Component)((RaycastHit)(ref val)).collider).GetComponentInParent<VRRig>();
				if (Object.op_Implicit((Object)(object)componentInParent) && !componentInParent.IsLocal())
				{
					Main.gunLocked = true;
					Main.lockTarget = componentInParent;
				}
			}
		}
		else if (Main.gunLocked)
		{
			Main.gunLocked = false;
		}
	}

	public static void ProjectileLagAll()
	{
		SerializePatch.OverrideSerialization = delegate
		{
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Expected O, but got Unknown
			//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0101: Unknown result type (might be due to invalid IL or missing references)
			//IL_010b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0110: Unknown result type (might be due to invalid IL or missing references)
			//IL_0124: Unknown result type (might be due to invalid IL or missing references)
			//IL_0129: Unknown result type (might be due to invalid IL or missing references)
			//IL_012e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0135: Expected O, but got Unknown
			//IL_0189: Unknown result type (might be due to invalid IL or missing references)
			if (PhotonNetwork.InRoom)
			{
				Main.MassSerialize(exclude: true, (PhotonView[])(object)new PhotonView[1] { VRRig.LocalRig.GetPhotonView() });
				Vector3 position = ((Component)VRRig.LocalRig).transform.position;
				NetPlayer[] playerListOthers = NetworkSystem.Instance.PlayerListOthers;
				foreach (NetPlayer val in playerListOthers)
				{
					VRRig vRRigFromPlayer = RigUtilities.GetVRRigFromPlayer(val);
					((Component)VRRig.LocalRig).transform.position = ((Component)vRRigFromPlayer).transform.position - Vector3.one * 3f;
					PhotonView photonView = VRRig.LocalRig.GetPhotonView();
					RaiseEventOptions val2 = new RaiseEventOptions();
					val2.TargetActors = new int[1] { val.ActorNumber };
					Main.SendSerialize(photonView, val2);
					Vector3 position2 = vRRigFromPlayer.headMesh.transform.position + new Vector3(0f, 0.1f, 0f) + vRRigFromPlayer.headMesh.transform.forward * -0.7f;
					Vector3 velocity = new Vector3(0f, 15f, 0f);
					Color black = Color.black;
					val2 = new RaiseEventOptions();
					val2.TargetActors = new int[1] { RigUtilities.NetPlayerToPlayer(RigUtilities.GetPlayerFromVRRig(vRRigFromPlayer)).ActorNumber };
					BetaFireProjectile("Fireworks_Anchor Variant_Left Hand", position2, velocity, black, val2, bypassTeleport: true);
				}
				Main.RPCProtection();
				((Behaviour)VRRig.LocalRig).enabled = true;
				((Component)VRRig.LocalRig).transform.position = position;
				return false;
			}
			return true;
		};
	}

	public static void ProjectileLagPlayer(NetPlayer player)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Expected O, but got Unknown
		VRRig vRRigFromPlayer = RigUtilities.GetVRRigFromPlayer(player);
		Vector3 position = vRRigFromPlayer.headMesh.transform.position + new Vector3(0f, 0.1f, 0f) + vRRigFromPlayer.headMesh.transform.forward * -0.7f;
		Vector3 velocity = new Vector3(0f, 15f, 0f);
		Color black = Color.black;
		RaiseEventOptions val = new RaiseEventOptions();
		val.TargetActors = new int[1] { RigUtilities.NetPlayerToPlayer(RigUtilities.GetPlayerFromVRRig(vRRigFromPlayer)).ActorNumber };
		BetaFireProjectile("Fireworks_Anchor Variant_Left Hand", position, velocity, black, val);
	}

	public static void ProjectileLagPlayer(VRRig player)
	{
		ProjectileLagPlayer(RigUtilities.GetPlayerFromVRRig(player));
	}
}
