namespace RPG.DearImGUI.Windows {
	using System.Numerics;
	using Engine.Core;
	using Engine.Input;
	using Engine.Modules;
	using Engine.Modules.Interfaces;
	using Engine.Serialization;
	using Engine.Utility;
	using ImGuiNET;
	using Popups;

	public class MenuBarWindow : AbstractWindow {
		

		#region NonSerialized Fields
		
		[NonSerialized]
		private Dictionary<string, List<MenuItemData>> menuItemDataMap;

		#endregion
		

		#region Constructor

		public MenuBarWindow() : base(true) {
			SubscribeToMenuBar("File", "New Node Group", KeyboardKeyMod.CTRL, KeyboardKeys.n, OnNewNodeGroup);
			SubscribeToMenuBar("File", "Save Node Group", KeyboardKeyMod.CTRL, KeyboardKeys.s, OnSaveNodeGroup);
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

		#endregion


		#region AbstractWindow

		public override string Name => "Menu Bar";

		public override void Render(uint dockId) {
			OnRenderGui();
		}

		protected override void OnRenderGui() {
			if (ImGui.BeginMainMenuBar()) {
				RenderMainMenuBar();
				
				//Render Play Stop button
				string text = Application.Instance.IsGameRunning ? "Stop" : "Start";
				if (ImGui.MenuItem(text)) {
					Application.Instance.IsGameRunning = !Application.Instance.IsGameRunning;
				}
				
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

		#endregion


		#region Private Methods

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

		private void OnNewNodeGroup() {
			//TODO: Detect if the current SceneGraphModule RootNode is dirty to prompt the user to maybe save
			SceneGraphModule sceneGraphModule = Application.Instance.Get<SceneGraphModule>();
			if (sceneGraphModule != null) {
				sceneGraphModule.SetRootNode(new Node("New Node"));
			}
		}

		private void OnSaveNodeGroup() {
			SceneGraphModule sceneGraphModule = Application.Instance.Get<SceneGraphModule>();
			if (sceneGraphModule != null) {
				Serializer.Instance.Serialize(sceneGraphModule.RootNode);
			}
		}

		private void OnQuit() {
			Application.Instance.RequestShutdown();
		}

		private void OnProjectSettings() {
			Application.Instance.EditorModule?.OpenPopup(new ProjectSettingsPopup());
		}

		private void OnEditorSettings() {
			Application.Instance.EditorModule?.OpenPopup(new EditorSettingsPopup());
		}

		#endregion
	}
}