using AutoMapper;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace DevBook.API.Mapping;

internal sealed class DevBookMappingProfile : Profile
{
	private readonly string mappingMethodName = nameof(IMappableTo<object>.Mapping);

	public DevBookMappingProfile()
	{
		ApplyMappingsFromAssembly(Assembly.GetExecutingAssembly());
	}

	private void ApplyMappingsFromAssembly(Assembly assembly)
	{
		var types = assembly.GetExportedTypes()
			.Where(t => t.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IMappableTo<>)));

		foreach (var type in types)
		{
			// Bypass constructor(scary) that is not needed for map creation
			var instance = RuntimeHelpers.GetUninitializedObject(type);

			// Get method with custom mappings first if there is one, otherwise use the default one on the Interface
			var methodInfo = type.GetMethod(mappingMethodName)
				?? type.GetInterface($"{nameof(IMappableTo<object>)}`1")!.GetMethod(mappingMethodName);

			methodInfo?.Invoke(instance, [this]);
		}
	}
}
