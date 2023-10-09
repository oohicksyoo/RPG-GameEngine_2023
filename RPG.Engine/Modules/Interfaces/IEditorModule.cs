namespace RPG.Engine.Modules.Interfaces {

	using Core;
	using Input;
	using Utility;

	public interface IEditorModule : IModule, IRender {
		
		public Node SelectedNode {
			get;
			set;
		}
		
		public void SubscribeToMenuBar(string menuName, string name, KeyboardKeyMod mod, KeyboardKeys key, Action  onClickAction);
		public void SubscribeToMenuBar(string menuName, string name, Action  onClickAction);
		//TODO: Allow people to subscribe their own AbstractWindows to the editor for use

		public void OpenPopup(IEditorPopup popup);
	}
}