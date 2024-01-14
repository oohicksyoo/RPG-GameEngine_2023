namespace RPG.Engine.Modules.Interfaces {
	using Components.Interfaces;
	using Graphics;

	public interface IRender {
		public List<IComponentRenderable> Render();
	}
}