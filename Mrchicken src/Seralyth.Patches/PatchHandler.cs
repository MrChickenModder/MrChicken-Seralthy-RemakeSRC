using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using Seralyth.Managers;
using Seralyth.Patches.Safety;

namespace Seralyth.Patches;

public class PatchHandler
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
	public class SecurityPatch : Attribute
	{
	}

	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
	public class PatchOnAwake : Attribute
	{
	}

	internal static Harmony instance;

	public const string InstanceId = "org.seralyth.gorillatag.seralythmenu";

	public static bool IsPatched { get; internal set; }

	public static int PatchErrors { get; internal set; }

	public static bool CriticalPatchFailed { get; internal set; }

	public static void PatchAll(bool awake = false)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Expected O, but got Unknown
		if (IsPatched)
		{
			return;
		}
		if (instance == null)
		{
			instance = new Harmony("org.seralyth.gorillatag.seralythmenu");
		}
		Type[] source;
		try
		{
			source = Assembly.GetExecutingAssembly().GetTypes();
		}
		catch (ReflectionTypeLoadException ex)
		{
			source = ex.Types.Where((Type t) => t != null).ToArray();
		}
		foreach (Type item in source.Where((Type t) => t.IsClass && ((MemberInfo)t).GetCustomAttribute<HarmonyPatch>() != null && t.GetCustomAttribute<PatchOnAwake>() != null == awake))
		{
			try
			{
				instance.CreateClassProcessor(item).Patch();
			}
			catch (Exception arg)
			{
				PatchErrors++;
				if (item.GetCustomAttribute<SecurityPatch>() != null)
				{
					CriticalPatchFailed = true;
				}
				LogManager.LogError($"Failed to patch {item.FullName}: {arg}");
			}
		}
		LogManager.Log($"Patched with {PatchErrors} errors");
		IsPatched = !awake;
	}

	public static void UnpatchAll()
	{
		if (instance != null && IsPatched)
		{
			instance.UnpatchSelf();
			IsPatched = false;
			instance = null;
		}
	}

	public static void ApplyPatch(Type targetClass, string methodName, MethodInfo prefix = null, MethodInfo postfix = null, Type[] parameterTypes = null)
	{
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		MethodInfo methodInfo = ((parameterTypes == null) ? targetClass.GetMethod(methodName, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic) : targetClass.GetMethod(methodName, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic, null, parameterTypes, null)) ?? throw new Exception("Method '" + methodName + "' not found on " + targetClass.FullName);
		instance.Patch((MethodBase)methodInfo, (!(prefix != null)) ? ((HarmonyMethod)null) : new HarmonyMethod(prefix), (!(postfix != null)) ? ((HarmonyMethod)null) : new HarmonyMethod(postfix), (HarmonyMethod)null, (HarmonyMethod)null, (HarmonyMethod)null);
	}

	public static void RemovePatch(Type targetClass, string methodName, Type[] parameterTypes = null)
	{
		MethodInfo methodInfo = ((parameterTypes == null) ? targetClass.GetMethod(methodName, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic) : targetClass.GetMethod(methodName, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic, null, parameterTypes, null)) ?? throw new Exception("Method '" + methodName + "' not found on " + targetClass.FullName);
		instance.Unpatch((MethodBase)methodInfo, (HarmonyPatchType)0, instance.Id);
	}

	private static MethodInfo[] ImagineTamperingWithThisLoser()
	{
		return new MethodInfo[4]
		{
			typeof(URLBlocker).GetMethod("IsBanned", BindingFlags.Static | BindingFlags.NonPublic),
			typeof(URLBlocker).GetMethod("IsBlockedProcess", BindingFlags.Static | BindingFlags.NonPublic),
			typeof(URLBlocker).GetMethod("ExtractUrls", BindingFlags.Static | BindingFlags.NonPublic),
			typeof(URLBlocker).GetMethod("ExtractAndDecodeBase64", BindingFlags.Static | BindingFlags.NonPublic)
		};
	}

	public static void PatchIntegrityCheck()
	{
		if (instance == null)
		{
			return;
		}
		IEnumerable<MethodInfo> first = from m in Assembly.GetExecutingAssembly().GetTypes().SelectMany((Type t) => t.GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
			where m.GetCustomAttributes(typeof(HarmonyPatch), inherit: false).Length != 0
			select m;
		MethodInfo[] second = ImagineTamperingWithThisLoser();
		IEnumerable<MethodInfo> enumerable = first.Concat(second).Distinct();
		foreach (MethodInfo item in enumerable)
		{
			Patches patchInfo = Harmony.GetPatchInfo((MethodBase)item);
			if (patchInfo == null)
			{
				continue;
			}
			IEnumerable<Patch> source = patchInfo.Prefixes.Concat(patchInfo.Postfixes).Concat(patchInfo.Transpilers).Concat(patchInfo.Finalizers);
			List<string> list = (from p in source
				select p.owner into owner
				where owner != null && owner != instance.Id
				select owner).Distinct().ToList();
			foreach (string item2 in list)
			{
				try
				{
					instance.Unpatch((MethodBase)item, (HarmonyPatchType)0, item2);
				}
				catch
				{
				}
			}
		}
	}
}
