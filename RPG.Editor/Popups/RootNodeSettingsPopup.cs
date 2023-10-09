namespace RPG.DearImGUI.Popups {
	using Engine.Core;
	using Engine.Modules;
	using Engine.Serialization;
	using Engine.Settings;
	using ImGuiNET;

	public class RootNodeSettingsPopup : AbstractPopup {

		#region Properties

		private Node Root {
			get;
			set;
		}

		private string OriginalName {
			get;
			set;
		}

		private bool NameChanged => this.OriginalName != this.Root.Name;

		#endregion
		
		
		#region Overrides

		public override string Name => "Root Node Settings";

		protected override void OnOpen() {
			SceneGraphModule sceneGraphModule = Application.Instance.Get<SceneGraphModule>();
			if (sceneGraphModule != null) {
				this.Root = Application.Instance.Get<SceneGraphModule>().RootNode;
				this.OriginalName = this.Root.Name;
			}
		}

		protected override void OnRenderGui() {
			if (this.Root == null) {
				return;
			}
			
			//Name
			string name = this.Root.Name;
			if (ImGui.InputText($"Name", ref name, 128)) {
				this.Root.Name = name;
			}
		}

		public override void Close() {
			if (this.Root == null) {
				return;
			}
			
			//Remove old node asset from disk
			if (this.NameChanged) {
				Serializer.Instance.Remove(new SimpleSerializedNode(this.OriginalName));
			}

			//Check if the Node we are renaming if the starting set node so we dont cause any errors during opening
			if (ProjectSettings.Instance.StartingNode == this.OriginalName) {
				ProjectSettings.Instance.StartingNode = this.Root.Name;
				ProjectSettings.Instance.Save();
			}
			
			//TODO: FUTURE: We will need to scan other node files to see if this is referenced so it can also be changed
			
			//Save new root to disk
			Serializer.Instance.Serialize(this.Root);
		}

		#endregion
		
	}
}