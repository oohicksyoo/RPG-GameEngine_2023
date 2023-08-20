namespace RPG.Engine.Serialization {

	using Components.Interfaces;
	using Core;
	using Utility;

	public class GuidDatabase : Singleton<GuidDatabase> {


		#region Non Serialized

		[NonSerialized]
		private Dictionary<Guid, IComponent> componentMap;
		
		[NonSerialized]
		private Dictionary<Guid, Node> nodeMap;

		#endregion


		#region Properties

		public Dictionary<Guid, IComponent> ComponentMap {
			get {
				return componentMap ??= new Dictionary<Guid, IComponent>();
			}
		}
		
		public Dictionary<Guid, Node> NodeMap {
			get {
				return nodeMap ??= new Dictionary<Guid, Node>();
			}
		}

		#endregion


	}
}