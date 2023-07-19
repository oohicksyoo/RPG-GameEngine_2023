namespace RPG.Engine.Components.Interfaces {
	using Core;

	public interface IComponent {

		public Node Node {
			get;
			set;
		}

		public Guid Guid {
			get;
		}
		
	}
}