namespace RPG.Engine.Components.Interfaces {
	using Core;
	using Serialization.Interfaces;

	public interface IComponent : ISerialize {

		public Node Node {
			get;
			set;
		}

		public Guid Guid {
			get;
		}
		
	}
}