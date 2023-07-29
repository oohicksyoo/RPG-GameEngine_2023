namespace RPG.DearImGUI.Windows {
	using Engine.Core;
	using Engine.Input;
	using Engine.Modules;
	using Engine.Utility;
	using ImGuiNET;

	public class MenuBarWindow : AbstractWindow {


		#region Types

		private struct MenuItem {
			public string name;
			public string shortcut;
			public Action onClickAction;
			public KeyboardKeys mod;
			public KeyboardKeys key;
			public bool hasShortcut;
		}

		#endregion
		

		#region NonSerialized Fields
		
		[NonSerialized]
		private Dictionary<string, List<MenuItem>> menuMap;

		#endregion
		

		#region Constructor

		public MenuBarWindow() : base(true) {
			SubscribeToMenuBar("File", "New", KeyboardKeyMod.CTRL, KeyboardKeys.n, OnNewNode);
			SubscribeToMenuBar("File", "Quit", OnQuit);
		}

		#endregion


		#region Properties

		private Dictionary<string, List<MenuItem>> MenuMap {
			get {
				return menuMap ??= new Dictionary<string, List<MenuItem>>();
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
				ImGui.EndMainMenuBar();
			}
		}

		#endregion


		#region Public Methods

		public void SubscribeToMenuBar(string menuName, string name, Action onClickAction) {
			MenuItem menuItem = new MenuItem();
			menuItem.name = name;
			menuItem.shortcut = String.Empty;
			menuItem.onClickAction = onClickAction;
			menuItem.hasShortcut = false;
			
			SubscribeToMenuBar(menuName, menuItem);
		}

		public void SubscribeToMenuBar(string menuName, string name, KeyboardKeyMod mod, KeyboardKeys key, Action onClickAction) {
			MenuItem menuItem = new MenuItem();
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
			foreach (List<MenuItem> menuItems in this.MenuMap.Values) {
				foreach (MenuItem menuItem in menuItems) {
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

		private void SubscribeToMenuBar(string menuName, MenuItem menuItem) {
			//Doesnt contain this menu name yet
			if (!this.MenuMap.ContainsKey(menuName)) {
				this.MenuMap.Add(menuName, new List<MenuItem>() {menuItem});
				return;
			}
			
			//Menu Item Does exist
			List<MenuItem> menuItems = this.MenuMap[menuName];

			if (!menuItems.Any(x => x.name == menuItem.name)) {
				menuItems.Add(menuItem);
			} else {
				Debug.Warning(GetType().Name, $"MenuItem ({menuItem.name}) already exists and we can not add in the new one");
			}
			
			this.MenuMap[menuName] = menuItems;
		}

		private void RenderMainMenuBar() {
			foreach (KeyValuePair<string,List<MenuItem>> keyValuePair in this.MenuMap) {
				RenderMenu(keyValuePair.Key, keyValuePair.Value);
			}
		}

		private void RenderMenu(string menuName, List<MenuItem> menuItems) {
			if (ImGui.BeginMenu(menuName)) {
				
				foreach (MenuItem menuItem in menuItems) {
					RenderMenuItem(menuItem.name, menuItem.shortcut, menuItem.onClickAction);
				}
				
				ImGui.EndMenu();
			}
		}

		private void RenderMenuItem(string menuItemName, string shortcut, Action onClickAction) {
			if (ImGui.MenuItem(menuItemName, shortcut)) {
				onClickAction?.Invoke();
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

		#endregion
	}
}