namespace RPG.Engine.Components {
	using Core;
	using Interfaces;
	using Serialization;

	public abstract class AbstractComponent : IComponent {


		#region Constructor

		public AbstractComponent() {
			//TODO: Be able to pass in guid
			this.Guid = Guid.NewGuid();
			GuidDatabase.Instance.ComponentMap.Add(this.Guid, this);
		}

		#endregion
		

		#region IComponent

		public Node Node {
			get;
			set;
		}
		
		public Guid Guid {
			get;
			private set;
		}

		#endregion
		
	}
}