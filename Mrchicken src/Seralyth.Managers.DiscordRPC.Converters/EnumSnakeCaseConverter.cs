using System;
using System.Linq;
using System.Reflection;
using Valve.Newtonsoft.Json;

namespace Seralyth.Managers.DiscordRPC.Converters;

internal class EnumSnakeCaseConverter : JsonConverter
{
	public override bool CanConvert(Type objectType)
	{
		return objectType.IsEnum;
	}

	public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
	{
		if (reader.Value == null)
		{
			return null;
		}
		object obj;
		return TryParseEnum(objectType, (string)reader.Value, out obj) ? obj : existingValue;
	}

	public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
	{
		Type type = value.GetType();
		string text = Enum.GetName(type, value);
		MemberInfo[] members = type.GetMembers(BindingFlags.Static | BindingFlags.Public);
		MemberInfo[] array = members;
		foreach (MemberInfo memberInfo in array)
		{
			if (memberInfo.Name.Equals(text))
			{
				object[] customAttributes = memberInfo.GetCustomAttributes(typeof(EnumValueAttribute), inherit: true);
				if (customAttributes.Length != 0)
				{
					text = ((EnumValueAttribute)customAttributes[0]).Value;
				}
			}
		}
		writer.WriteValue(text);
	}

	public bool TryParseEnum(Type enumType, string str, out object obj)
	{
		if (str == null)
		{
			obj = null;
			return false;
		}
		Type type = enumType;
		if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
		{
			type = type.GetGenericArguments().First();
		}
		if (!type.IsEnum)
		{
			obj = null;
			return false;
		}
		MemberInfo[] members = type.GetMembers(BindingFlags.Static | BindingFlags.Public);
		MemberInfo[] array = members;
		foreach (MemberInfo memberInfo in array)
		{
			object[] customAttributes = memberInfo.GetCustomAttributes(typeof(EnumValueAttribute), inherit: true);
			if (customAttributes.Cast<EnumValueAttribute>().Any((EnumValueAttribute enumValue) => str.Equals(enumValue.Value)))
			{
				obj = Enum.Parse(type, memberInfo.Name, ignoreCase: true);
				return true;
			}
		}
		obj = null;
		return false;
	}
}
