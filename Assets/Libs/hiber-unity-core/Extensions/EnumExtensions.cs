using System;
using System.Collections.Generic;
using System.Reflection;
using System.ComponentModel;

public static class EnumExtensions
{
	public static string GetEnumDescription(Enum value)
	{
	    FieldInfo fi = value.GetType().GetField(value.ToString());
	
	    DescriptionAttribute[] attributes = 
	        (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
	
	    if (attributes != null && attributes.Length > 0)
	        return attributes[0].Description;
	    else
	        return value.ToString();
	}
	
	public static int GetLength(this Enum value) {
		return Enum.GetValues(value.GetType()).Length;
	}
	
	public static int GetLength<T>(){
		return Enum.GetValues(typeof(T)).Length;
	}
}