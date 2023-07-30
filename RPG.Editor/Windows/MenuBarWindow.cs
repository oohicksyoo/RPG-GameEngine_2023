namespace RPG.DearImGUI.Windows {
	using System.Numerics;
	using Engine.Core;
	using Engine.Input;
	using Engine.Modules;
	using Engine.Utility;
	using ImGuiNET;
	using Popups;

	public class MenuBarWindow : AbstractWindow {
		

		#region NonSerialized Fields
		
		[NonSerialized]
		private Dictionary<string, List<MenuItemData>> menuItemDataMap;

		[NonSerialized]
		private Dictionary<string, AbstractPopup> openedPopupsMap;

		#endregion
		

		#region Constructor

		public MenuBarWindow() : base(true) {
			SubscribeToMenuBar("File", "New", KeyboardKeyMod.CTRL, KeyboardKeys.n, OnNewNode);
			SubscribeToMenuBar("File", "Quit", OnQuit);
			
			SubscribeToMenuBar("Project", "Settings", KeyboardKeyMod.CTRL, KeyboardKeys.p, OnProjectSettings);
			SubscribeToMenuBar("Editor", "Settings", OnEditorSettings);
		}

		#endregion


		#region Properties

		private Dictionary<string, List<MenuItemData>> MenuItemDataMap {
			get {
				return menuItemDataMap ??= new Dictionary<string, List<MenuItemData>>();
			}
		}

		private Dictionary<string, AbstractPopup> OpenedPopupsMap {
			get {
				return openedPopupsMap ??= new Dictionary<string, AbstractPopup>();
			}
		}

		private string OpenPopupKey {
			get;
			set;
		}

		#endregion


		#region AbstractWindow

		public override string Name => "Menu Bar";

		public override void Render(uint dockId) {
			OnRenderGui();
			RenderPopups();
		}

		protected override void OnRenderGui() {
			if (ImGui.BeginMainMenuBar()) {
				RenderMainMenuBar();
				ImGui.EndMainMenuBar();
			}
		}

		#endregion


		#region Public Methods

		public void SubscribeToMenuBar(string menuName, string name, Action onClickAction) {
			MenuItemData menuItem = new MenuItemData();
			menuItem.name = name;
			menuItem.shortcut = String.Empty;
			menuItem.onClickAction = onClickAction;
			menuItem.hasShortcut = false;
			
			SubscribeToMenuBar(menuName, menuItem);
		}

		public void SubscribeToMenuBar(string menuName, string name, KeyboardKeyMod mod, KeyboardKeys key, Action onClickAction) {
			MenuItemData menuItem = new MenuItemData();
			menuItem.name = name;
			menuItem.shortcut = $"{mod.ToString().ToUpper()}+{key.ToString().ToUpper()}";
			menuItem.onClickAction = onClickAction;
			menuItem.mod = (KeyboardKeys)mod;
			menuItem.key = key;
			menuItem.hasShortcut = true;
			
			SubscribeToMenuBar(menuName, menuItem);
		}

		public void CheckShortcuts() {
			Keyboard keyboard = Application.Instance.InputModule.Keyboard;
			foreach (List<MenuItemData> menuItems in this.MenuItemDataMap.Values) {
				foreach (MenuItemData menuItem in menuItems) {
					if (menuItem.hasShortcut) {
						if (keyboard.IsDown(menuItem.mod) && keyboard.IsPressed(menuItem.key)) {
							menuItem.onClickAction?.Invoke();
						}
					}
				}
			}
			
		}
		
		//TODO: Expose this more so others in their project can use this functionality to open popups
		public void OpenPopup(AbstractPopup abstractPopup) {
			this.OpenPopupKey = abstractPopup.Name;
			this.OpenedPopupsMap.TryAdd(abstractPopup.Name, abstractPopup);
		}

		#endregion


		#region Private Methods

		private void RenderPopups() {
			if (!string.IsNullOrEmpty(this.OpenPopupKey)) {
				if (ImGui.IsPopupOpen(this.OpenPopupKey)) {
					this.OpenedPopupsMap.Remove(this.OpenPopupKey);
					ImGui.CloseCurrentPopup();
				} else {
					ImGui.OpenPopup(this.OpenPopupKey);
				}
				this.OpenPopupKey = string.Empty;
			}

			foreach (AbstractPopup abstractPopup in this.OpenedPopupsMap.Values) {
				if (!abstractPopup.IsOpen) {
					//Popup x was clicked and we need to cleanup
					abstractPopup.Close();
					this.OpenedPopupsMap.Remove(abstractPopup.Name);
				} else {
					abstractPopup?.Render();
				}
			}
		}

		private void SubscribeToMenuBar(string menuName, MenuItemData menuItem) {
			//Doesnt contain this menu name yet
			if (!this.MenuItemDataMap.ContainsKey(menuName)) {
				this.MenuItemDataMap.Add(menuName, new List<MenuItemData>() {menuItem});
				return;
			}
			
			//Menu Item Does exist
			List<MenuItemData> menuItems = this.MenuItemDataMap[menuName];

			if (!menuItems.Any(x => x.name == menuItem.name)) {
				menuItems.Add(menuItem);
			} else {
				Debug.Warning(GetType().Name, $"MenuItem ({menuItem.name}) already exists and we can not add in the new one");
			}
			
			this.MenuItemDataMap[menuName] = menuItems;
		}

		private void RenderMainMenuBar() {
			foreach (KeyValuePair<string,List<MenuItemData>> keyValuePair in this.MenuItemDataMap) {
				RenderMenu(keyValuePair.Key, keyValuePair.Value);
			}
		}

		private void RenderMenu(string menuName, List<MenuItemData> menuItems) {
			if (ImGui.BeginMenu(menuName)) {
				foreach (MenuItemData menuItem in menuItems) {
					if (ImGui.MenuItem(menuItem.name, menuItem.shortcut)) {
						menuItem.onClickAction?.Invoke();
					}
				}
				ImGui.EndMenu();
			}
		}

		#endregion
		

		#region MenuItem Actions

		private void OnNewNode() {
			Debug.Log(GetType().Name,"New");
		}

		private void OnQuit() {
			Application.Instance.RequestShutdown();
		}

		private void OnProjectSettings() {
			OpenPopup(new ProjectSettingsPopup());
		}

		private void OnEditorSettings() {
			OpenPopup(new EditorSettingsPopup());
		}

		#endregion
	}
}