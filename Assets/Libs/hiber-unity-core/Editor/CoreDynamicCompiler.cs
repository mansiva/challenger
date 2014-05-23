using Microsoft.CSharp;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

/// <summary>
/// This class can be used to compile a string of source code. Eventually, this could be used to create
/// a runtime console that could inject code for debugging purposes.
/// </summary>
public static class CoreDynamicCompiler
{
	/// <summary>
	/// For each type passed as a parameter, the assembly is found and
	/// added to a list of unique assemblies. Duplicates are ignored.
	/// </summary>
	public static Assembly[] ConvertTypesToAssemblies(params Type[] types)
	{
		HashSet<Assembly> assemblies = new HashSet<Assembly>();
		foreach(var type in types)
		{
			if(type != null)
			{
				assemblies.Add(type.Assembly);
			}
		}

		Assembly[] array = new Assembly[assemblies.Count];
		assemblies.CopyTo(array);
		return array;
	}

	/// <summary>
	/// Compiles a string of source code with the use of the provided assemblies.
	/// </summary>
	public static CompilerResults CompileScript(string source, Assembly[] assemblies)
	{
		// Define standard compilation parameters.
		CompilerParameters parameters = new CompilerParameters();
		parameters.GenerateExecutable = false;
		parameters.GenerateInMemory = true;
		parameters.IncludeDebugInformation = false;

		// Add the assemblies to the parameters.
		foreach(var assembly in assemblies)
		{
			Uri uri = new Uri(assembly.CodeBase);
			parameters.ReferencedAssemblies.Add(uri.LocalPath);
		}

		// Compile
		CodeDomProvider compiler = CSharpCodeProvider.CreateProvider("CSharp");
		return compiler.CompileAssemblyFromSource(parameters, source);
	}
}
