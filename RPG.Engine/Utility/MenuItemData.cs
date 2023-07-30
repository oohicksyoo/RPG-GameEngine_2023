namespace RPG.Engine.Utility {
	using Input;

	/// <summary>
	/// Editor Specific MenuItemData: Requires to be in Engine due to the need to be in IEditorModule
	/// </summary>
	public struct MenuItemData {
		public string name;
		public string shortcut;
		public Action onClickAction;
		public KeyboardKeys mod;
		public KeyboardKeys key;
		public bool hasShortcut;
	}
}