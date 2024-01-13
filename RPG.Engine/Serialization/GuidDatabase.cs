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

		[NonSerialized]
		private Dictionary<Guid, List<Action<IComponent>>> componentHookupMap;

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
		
		private Dictionary<Guid, List<Action<IComponent>>> ComponentHookupMap {
			get {
				return componentHookupMap ??= new Dictionary<Guid, List<Action<IComponent>>>();
			}
		}

		#endregion


		#region Public Methods

		public void Add(Guid guid, IComponent component) {
			bool wasAdded = GuidDatabase.Instance.ComponentMap.TryAdd(guid, component);
			
			if (!wasAdded) {
				//TODO: Failed to add to GuidDatabase because this guid already exists, we need to filter through this and cycle out guids for things attached
			} else if (this.ComponentHookupMap.ContainsKey(guid)) {
				//Fire off call back event for the hookups that were looking for this connection but already intialized
				foreach (Action<IComponent> action in this.ComponentHookupMap[guid]) {
					action?.Invoke(component);
				}

				this.ComponentHookupMap.Remove(guid); //Hookup map no longer needs this guid
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

		/// <summary>
		/// Hook into a callback for a component that is not yet initialized yet
		/// </summary>
		public void ComponentInitializedCallback(Guid guid, Action<IComponent> componentCallbackAction) {
			List<Action<IComponent>> list = new List<Action<IComponent>>();
			bool guidAdded = this.ComponentHookupMap.ContainsKey(guid);
			
			//Grab current list
			if (guidAdded) {
				list = this.ComponentHookupMap[guid];
			}

			//Add callback to list
			list.Add(componentCallbackAction);

			//Setup list in dictionary
			if (!guidAdded) {
				this.ComponentHookupMap.Add(guid, list);
			} else {
				this.ComponentHookupMap[guid] = list;
			}
		}

		#endregion


	}
}