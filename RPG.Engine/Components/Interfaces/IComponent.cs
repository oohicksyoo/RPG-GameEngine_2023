namespace RPG.Engine.Components.Interfaces {
	using Core;
	using Core.Interfaces;
	using Serialization.Interfaces;

	public interface IComponent : ISerialize, IRunnable {

		public Node Node {
			get;
			set;
		}

		public Guid Guid {
			get;
		}
	}
}