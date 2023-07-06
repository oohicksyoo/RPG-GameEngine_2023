﻿namespace RPG.Engine.Core {
	using Components;
	using Components.Interfaces;

	/// <summary>
	/// Nodes are the base building block, nodes can contain components and have other children nodes under them.
	/// Nodes Always come with a TransformComponent
	/// </summary>
	public class Node {


		#region Constructor

		public Node(string name = "") {
			this.Name = name;
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
			private set;
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
			if (HasComponent<T>()) {
				this.Components.Remove(GetComponent<T>());
			}
		}

		public void AddComponent<T>() where T : IComponent {
			IComponent component = Activator.CreateInstance<T>();
			component.Node = this;
			this.Components.Add(component);
		}

		#endregion

	}
}