using System;
using System.Collections.Generic;

public static class ListExtensions
{
	private static Random random = new Random();
	
	public static void Shuffle<T>(this IList<T> list)  
	{  
	    int n = list.Count;  
	    while (n > 1) {  
	        n--;  
	        int k = random.Next(n + 1);  
	        T value = list[k];  
	        list[k] = list[n];  
	        list[n] = value;  
	    }  
	}

	public static void Set<T>(this IList<T> list, int index, T t)
	{
		if(index >= list.Count)
		{
			list.GrowTo(index);
		}
		
		list[index] = t;
	}
	
	public static void GrowTo<T>(this IList<T> list, int index)
	{
		var count = index - list.Count + 1;
		for(var i = 0; i < count; i++)
		{
			list.Add(default(T));
		}
	}
}
