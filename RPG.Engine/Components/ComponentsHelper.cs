namespace RPG.Engine.Components {
	using System.Reflection;
	using Core;

	/// <summary>
	/// Helper methods for Components
	/// </summary>
	public static class ComponentsHelper {

		public static List<Type> GetAllAvailableComponentTypes() {
			//Use Reflection to grab all the types derived from Component
			Type componentType = typeof(AbstractComponent);
			List<Type> types = new List<Type>();
			types.AddRange(Assembly.GetAssembly(componentType)
				.GetTypes()
				.Where(myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(componentType)));

			Type projectAssembly = Application.Instance.Project.GetType();
			types.AddRange(Assembly.GetAssembly(projectAssembly)
				.GetTypes()
				.Where(x => x.IsClass && !x.IsAbstract && x.IsSubclassOf(componentType)));

			return types;
		}
		
	}
}