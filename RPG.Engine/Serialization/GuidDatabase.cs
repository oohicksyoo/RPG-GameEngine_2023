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

		//TODO: Convert to private
		public Dictionary<Guid, IComponent> ComponentMap {
			get {
				return componentMap ??= new Dictionary<Guid, IComponent>();
			}
		}
		
		//TODO: Convert to private
		public Dictionary<Guid, Node> NodeMap {
			get {
				return nodeMap ??= new Dictionary<Guid, Node>();
			}
		}

		#endregion


		#region Public Methods

		public void Add(Guid guid, IComponent component) {
			if (!GuidDatabase.Instance.ComponentMap.TryAdd(guid, component)) {
				//TODO: Failed to add to GuidDatabase because this guid already exists, we need to filter through this and cycle out guids for things attached
			}
		}

		public void Add(Guid guid, Node node) {
			if (!GuidDatabase.Instance.NodeMap.TryAdd(guid, node)) {
				//TODO: Failed to add to GuidDatabase because this guid already exists, we need to filter through this and cycle out guids for things attached
			}
		}
		
		//TODO: Exist Method: Check if a GUID is already in place so things can happen to give them new guids
		
		//TODO: Get Methods
		
		//TODO: Remove Methods

		#endregion


	}
}