namespace RPG.Engine.Core {
	using Attributes;
	using Components;
	using Components.Interfaces;
	using Utility;

	/// <summary>
	/// Nodes are the base building block, nodes can contain components and have other children nodes under them.
	/// Nodes Always come with a TransformComponent
	/// </summary>
	public class Node {


		#region Constructor

		public Node(string name = "") {
			this.Name = name;
			this.Guid = Guid.NewGuid();
			this.IsEnabled = true;
			this.Tag = String.Empty;
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

	}
}