namespace RPG.Engine.Core {
	
	using Attributes;
	using Components;
	using Components.Interfaces;
	using Interfaces;
	using Newtonsoft.Json.Linq;
	using Serialization;
	using Serialization.Interfaces;
	using Utility;

	/// <summary>
	/// Nodes are the base building block, nodes can contain components and have other children nodes under them.
	/// Nodes Always come with a TransformComponent
	/// </summary>
	public class Node : ISerialize, IGuidDatabase, IRunnable {


		#region Constructor

		public Node(string name = "") {
			this.Name = name;
			this.Guid = Guid.NewGuid();
			this.IsEnabled = true;
			this.Tag = String.Empty;
			AddToGuidDatabase();
			AddComponent<Transform>();
		}

		#endregion


		#region Non Serialized

		[NonSerialized]
		private List<IComponent> components;

		[NonSerialized]
		private List<Node> childrenNodes;

		#endregion


		#region Properties

		public string Name {
			get;
			set;
		}

		public Guid Guid {
			get;
			private set;
		}

		public bool IsEnabled {
			get;
			set;
		}

		public string Tag {
			get;
			set;
		}

		public List<IComponent> Components {
			get {
				return components ??= new List<IComponent>();
			}
		}

		public List<Node> Children {
			get {
				return childrenNodes ??= new List<Node>();
			}
		}

		#endregion


		#region Public Methods

		public void Add(Node node) {
			if (!Contains(node)) {
				this.Children.Add(node);
			}
		}

		public void Remove(Node node) {
			this.Children.Remove(node);
		}

		public bool Contains(Node node) {
			return this.Children.Contains(node);
		}

		public bool HasComponent<T>() where T : IComponent {
			return this.Components.Any(x => x is T);
		}
		
		public T GetComponent<T>() where T : IComponent {
			return (T)this.Components.FirstOrDefault(x => x is T);
		}

		public void RemoveComponent<T>() where T : IComponent {
			if (HasComponent<T>() && typeof(T) != typeof(Transform)) {
				this.Components.Remove(GetComponent<T>());
			}
		}

		public void AddComponent<T>() where T : IComponent {
			bool isSingular = typeof(T).IsDefined(typeof(Singular), true);
			if (!isSingular || (isSingular && !HasComponent<T>())) {
				IComponent component = Activator.CreateInstance<T>();
				component.Node = this;
				this.Components.Add(component);
			} else if (isSingular) {
				Debug.Error(GetType().Name, $"Node ({this.Name}) already contains the component ({typeof(T).Name}) and it was marked as singular.");
			}
		}

		#endregion


		#region ISerialize
		
		public string AssetName => $"{this.Name}";

		public string AssetExtension => "node";
		
		public string SpecialFolder => $"Assets/Nodes";

		/// <summary>
		/// Gets the assembly type for serialization
		/// </summary>
		private string Type => GetType().AssemblyQualifiedName;

		public JObject Serialize() {
			JObject jsonObject = new JObject();

			//Name
			jsonObject[nameof(this.Name)] = this.Name;
			
			//Guid
			jsonObject[nameof(this.Guid)] = this.Guid;
			
			//Assembly Type
			jsonObject[nameof(this.Type)] = this.Type;
			
			//IsEnabled
			jsonObject[nameof(this.IsEnabled)] = this.IsEnabled;
			
			//Tag
			jsonObject[nameof(this.Tag)] = this.Tag;
			
			//Components
			JArray components = new JArray();
			foreach (IComponent component in this.Components) {
				components.Add(component.Serialize());
			}
			jsonObject[nameof(this.Components)] = components;

			//Children
			JArray children = new JArray();
			foreach (Node child in this.Children) {
				children.Add(child.Serialize());
			}
			jsonObject[nameof(this.Children)] = children;

			return jsonObject;
		}

		public void Deserialize(JObject jsonObject) {
			//Remove Node From NodeDatabase
			RemoveFromGuidDatabase();
			
			//Name
			this.Name = (string)jsonObject[nameof(this.Name)];
			
			//Guid
			this.Guid = Guid.Parse((string)jsonObject[nameof(this.Guid)]);
			
			//Readd this Node back to the database
			AddToGuidDatabase();
			
			//Assembly Type - Not Needed
			//this.Type = (string)jsonObject[nameof(this.Type)];
			
			//IsEnabled
			this.IsEnabled = (bool)jsonObject[nameof(this.IsEnabled)];
			
			//Tag
			this.Tag = (string)jsonObject[nameof(this.Tag)];
			
			//Components
			//Pre Work to get all possible component types from our different assemblies
			List<Type> types = ComponentsHelper.GetAllAvailableComponentTypes();
			JArray components = (JArray)jsonObject[nameof(this.Components)];
			this.Components.Clear(); //Clear all components because we are going to fill in Transform ourselves
			foreach (JObject componentObject in components) {
				string assemblyType = (string)componentObject["Type"];
				Type type = types.SingleOrDefault(x => x.AssemblyQualifiedName == assemblyType);
				if (type != null) {
					IComponent component = (IComponent)Activator.CreateInstance(type);
					component.Deserialize(componentObject);
					component.Node = this;
					this.Components.Add(component);
				} else {
					Debug.Warning(GetType().Name, $"Could not find type of ({assemblyType}) to add to component.");
				}
			}

			//Children
			JArray children = (JArray)jsonObject[nameof(this.Children)];
			foreach (JObject child in children) {
				Node childNode = new Node();
				childNode.Deserialize(child);
				this.Children.Add(childNode);
			}
			
			//Hook up my serialized values
		}

		#endregion


		#region IGuidDatabase

		public void AddToGuidDatabase() {
			GuidDatabase.Instance.NodeMap.Add(this.Guid, this);
		}

		public void RemoveFromGuidDatabase() {
			GuidDatabase.Instance.NodeMap.Remove(this.Guid);

			foreach (IGuidDatabase component in components) {
				component.RemoveFromGuidDatabase();
			}

			foreach (IGuidDatabase child in this.Children) {
				child.RemoveFromGuidDatabase();
			}
		}

		#endregion


		#region IRunnable

		public void Awake() {
			foreach (IRunnable child in this.Children) {
				child.Awake();
			}

			foreach (IRunnable component in this.Components) {
				component.Awake();
			}
		}

		public void Start() {
			foreach (IRunnable child in this.Children) {
				child.Start();
			}

			foreach (IRunnable component in this.Components) {
				component.Start();
			}
		}

		public void Update() {
			foreach (IRunnable child in this.Children) {
				child.Update();
			}

			foreach (IRunnable component in this.Components) {
				component.Update();
			}
		}

		#endregion

	}
}