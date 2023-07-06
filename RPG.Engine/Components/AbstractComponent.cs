namespace RPG.Engine.Components {
	using Core;
	using Interfaces;

	public abstract class AbstractComponent : IComponent {


		#region IComponent

		public Node Node {
			get;
			set;
		}

		#endregion
		
	}
}