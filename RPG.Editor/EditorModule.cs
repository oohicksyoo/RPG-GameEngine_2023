namespace RPG.DearImGUI {
	using System.Drawing;
	using System.Numerics;
	using System.Reflection;
	using Windows;
	using Engine.Components.Interfaces;
	using Engine.Core;
	using Engine.Graphics;
	using Engine.Modules;
	using Engine.Modules.Interfaces;
	using Engine.Serialization;
	using Engine.Utility;
	using ImGuiNET;

	public class EditorModule : IEditorModule {


		#region NonSerialized Fields

		[NonSerialized]
		private List<AbstractWindow> windows;

		[NonSerialized]
		private static Dictionary<Type, Action<object, PropertyInfo>> inspectorTypeRendering;

		#endregion


		#region Properties

		private List<AbstractWindow> Windows {
			get {
				return windows ??= new List<AbstractWindow>();
			}
		}

		public Node SelectedNode {
			get;
			set;
		}

		private static Dictionary<Type, Action<object, PropertyInfo>> InspectorTypeRendering {
			get {
				return inspectorTypeRendering ??= new Dictionary<Type, Action<object, PropertyInfo>>();
			}
		}

		private string Text {
			get;
			set;
		}

		#endregion
		
		
		#region IEditorModule

		public string ModuleName => GetType().Name;

		public string Name => "Editor";

		public Version Version => new Version(0, 1, 0);

		public int Priority => int.MaxValue - 4;

		public void Awake() {
			Initialize();
			
			ImGUISystem.Initialize();
		}

		public void Start() {
			//Configure main node for HierarchyWindow, double check if SceneGraphModule is in fact being used
			SceneGraphModule sceneGraphModule = Application.Instance.Get<SceneGraphModule>();
			Node node = new Node("Empty");
			if (sceneGraphModule != null) {
				node = sceneGraphModule.RootNode;
			}
			
			this.Windows.Add(new MenuBarWindow());
			this.Windows.Add(new HierarchyWindow(node));
			this.Windows.Add(new ConsoleWindow());
			this.Windows.Add(new RenderTargetWindow("Game", Application.Instance.GameFramebuffer.RenderTextureId));
			this.Windows.Add(new RenderTargetWindow("Scene", Application.Instance.SceneFramebuffer.RenderTextureId));
			this.Windows.Add(new InspectorWindow());
			this.Windows.Add(new AsepriteWindow(true));
			this.Windows.Add(new DemoWindow(true));
		}

		public void Update() {
			ImGUISystem.Update();
		}

		public void Shutdown() {
			
		}

		public void Render() {
			uint dockSpaceId = ImGui.DockSpaceOverViewport();
			foreach (AbstractWindow window in this.Windows) {
				window.Render(dockSpaceId);
			}

			ImGUISystem.Render();
		}

		#endregion


		#region Public Static Methods

		/// <summary>
		/// Adds type to Dictionary so it can be rendered, if you add a type that already exists by default you will override all use cases
		/// </summary>
		public static void Register<T>(Action<object, PropertyInfo> inspectorRenderingAction) {
			//Only add if doesnt exist; Project specific entries are added before the default fallbacks
			if (!InspectorTypeRendering.ContainsKey(typeof(T))) {
				InspectorTypeRendering.Add(typeof(T), inspectorRenderingAction);
			}
		}

		public static Action<object, PropertyInfo> Get(Type type) {
			if (InspectorTypeRendering.ContainsKey(type)) {
				return InspectorTypeRendering[type];
			}

			if (typeof(IComponent).IsAssignableFrom(type)) {
				return InspectorTypeRendering[typeof(IComponent)];
			}
			
			//Default return should be like GUID storing?
			return (component, propertyInfo) => {
				ImGui.Text($"Missing inspector rendering action for type({type} | {propertyInfo.PropertyType.Name})");
			};
		}

		#endregion


		#region Private Methods

		private void Initialize() {
			Register<int>(InspectorRenderInt);
			Register<float>(InspectorRenderFloat);
			Register<Vector2>(InspectorRenderVector2);
			Register<Vector3>(InspectorRenderVector3);
			Register<Vector4>(InspectorRenderVector4);
			Register<Color>(InspectorRenderColor);
			Register<bool>(InspectorRenderBool);
			Register<string>(InspectorRenderString);
			Register<IComponent>(InspectorRenderIComponent);
			Register<AsepriteAssetFile>(InspectorRenderAsepriteAssetFile);
		}
		
		private void InspectorRenderInt(object component, PropertyInfo propertyInfo) {
			int value = (int)propertyInfo.GetValue(component);
			ImGui.DragInt($"{propertyInfo.Name}##{propertyInfo.PropertyType}", ref value);
			propertyInfo.SetValue(component, value);
		}
		
		private void InspectorRenderFloat(object component, PropertyInfo propertyInfo) {
			float value = (float)propertyInfo.GetValue(component);
			ImGui.DragFloat($"{propertyInfo.Name}##{propertyInfo.PropertyType}", ref value);
			propertyInfo.SetValue(component, value);
		}

		private void InspectorRenderVector2(object component, PropertyInfo propertyInfo) {
			Vector2 value = (Vector2)propertyInfo.GetValue(component);
			ImGui.DragFloat2($"{propertyInfo.Name}##{propertyInfo.PropertyType}", ref value);
			propertyInfo.SetValue(component, value);
		}
		
		private void InspectorRenderVector3(object component, PropertyInfo propertyInfo) {
			Vector3 value = (Vector3)propertyInfo.GetValue(component);
			ImGui.DragFloat3($"{propertyInfo.Name}##{propertyInfo.PropertyType}", ref value);
			propertyInfo.SetValue(component, value);
		}
		
		private void InspectorRenderVector4(object component, PropertyInfo propertyInfo) {
			Vector4 value = (Vector4)propertyInfo.GetValue(component);
			ImGui.DragFloat4($"{propertyInfo.Name}##{propertyInfo.PropertyType}", ref value);
			propertyInfo.SetValue(component, value);
		}
		
		private void InspectorRenderColor(object component, PropertyInfo propertyInfo) {
			Vector4 value = ((Color)propertyInfo.GetValue(component)).ToVector4();
			ImGui.ColorEdit4($"{propertyInfo.Name}##{propertyInfo.PropertyType}", ref value);
			propertyInfo.SetValue(component, value);
		}
		
		private void InspectorRenderBool(object component, PropertyInfo propertyInfo) {
			bool value = (bool)propertyInfo.GetValue(component);
			ImGui.Checkbox($"{propertyInfo.Name}##{propertyInfo.PropertyType}", ref value);
			propertyInfo.SetValue(component, value);
		}
		
		private void InspectorRenderString(object component, PropertyInfo propertyInfo) {
			string value = (string)propertyInfo.GetValue(component);
			if (ImGui.InputText($"{propertyInfo.Name}##{propertyInfo.PropertyType}", ref value, 128)) {
				propertyInfo.SetValue(component, value);
			}
		}
		
		private void InspectorRenderIComponent(object component, PropertyInfo propertyInfo) {
			object obj = propertyInfo.GetValue(component);
			string value = string.Empty;
			bool isMissing = obj == null;
			if (obj != null) {
				value = ((IComponent)propertyInfo.GetValue(component)).Guid.ToString();
			} else {
				value = Guid.Empty.ToString();
			}

			
			
			if (isMissing) {
				Color color = Color.DarkRed;
				Vector4 chosenColor = new Vector4(color.R, color.G, color.B, color.A);
				ImGui.PushStyleColor(ImGuiCol.Text, chosenColor);
			}
			
			ImGui.Text(value);
			
			if (isMissing) {
				ImGui.PopStyleColor();
			}
			
			ImGui.SameLine();
			ImGui.Text("Guid");

			//TODO: Add drag and drop support of IComponent onto this field
			
			//TODO: Do we find that IComponent that was set and set it for them now?
			//propertyInfo.SetValue(component, value);
		}

		public void InspectorRenderAsepriteAssetFile(object component, PropertyInfo propertyInfo) {
			
		}

		#endregion
		
	}
}