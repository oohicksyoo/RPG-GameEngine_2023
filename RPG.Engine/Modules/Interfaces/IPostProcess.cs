namespace RPG.Engine.Modules.Interfaces {
	using Graphics;

	public interface IPostProcess {
		//TODO: Maybe we are given the final framebuffer image in order to preform post processing?
		public void PostProcess();
	}
}