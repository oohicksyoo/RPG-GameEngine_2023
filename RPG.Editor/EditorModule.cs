namespace RPG.DearImGUI {
	
	using System.Drawing;
	using System.Numerics;
	using System.Reflection;
	using System.Runtime.InteropServices;
	using Windows;
	using DragDrop;
	using DragDrop.Interfaces;
	using Engine.Aseprite;
	using Engine.Components;
	using Engine.Components.Interfaces;
	using Engine.Core;
	using Engine.Graphics;
	using Engine.Input;
	using Engine.Modules;
	using Engine.Modules.Interfaces;
	using Engine.Serialization;
	using Engine.Utility;
	using ImGuiNET;
	using Utility;

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

		private MenuBarWindow MenuBarWindow {
			get;
			set;
		}

		private HierarchyWindow HierarchyWindow {
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
			
		}

		public void Update() {
			ImGUISystem.Update();
			
			//Check for Registered Shortcuts
			this.MenuBarWindow.CheckShortcuts();
		}

		public void Shutdown() {
			
		}

		public void Render() {
			uint dockSpaceId = ImGui.DockSpaceOverViewport();
			foreach (AbstractWindow window in this.Windows) {
				window.Render(dockSpaceId);
			}
			
			//TODO: Should move popup rendering out of the MenuBarWindow rendering so really anything can start a popup and know it will open for them

			ImGUISystem.Render();
		}

		public void SubscribeToMenuBar(string menuName, string name, KeyboardKeyMod mod, KeyboardKeys key, Action onClickAction) {
			this.MenuBarWindow.SubscribeToMenuBar(menuName, name, mod, key, onClickAction);
		}

		public void SubscribeToMenuBar(string menuName, string name, Action onClickAction) {
			this.MenuBarWindow.SubscribeToMenuBar(menuName, name, onClickAction);
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
			
			//DefaultAseprite return should be like GUID storing?
			return (component, propertyInfo) => {
				ImGui.Text($"Missing inspector rendering action for type({type} | {propertyInfo.PropertyType.Name})");
			};
		}

		#endregion

		
		#region Private Methods
		
		private void Initialize() {
			InitializeWindows();
			InitializeTypeRendering();
		}

		private void InitializeWindows() {
			this.MenuBarWindow = new MenuBarWindow();
			this.HierarchyWindow = new HierarchyWindow();
			
			this.Windows.Add(this.MenuBarWindow);
			this.Windows.Add(this.HierarchyWindow );
			this.Windows.Add(new ConsoleWindow());
			this.Windows.Add(new RenderTargetWindow("Game", Application.Instance.GameFramebuffer.RenderTextureId));
			this.Windows.Add(new RenderTargetWindow("Scene", Application.Instance.SceneFramebuffer.RenderTextureId));
			this.Windows.Add(new InspectorWindow());
			this.Windows.Add(new AssetWindow());
			this.Windows.Add(new CacheWindow());
			this.Windows.Add(new StatisticsWindows());
			//this.Windows.Add(new DemoWindow(true));
		}

		#endregion


		#region Private Methods - Type Rendering

		private void InitializeTypeRendering() {
			Register<int>(InspectorRenderInt);
			Register<float>(InspectorRenderFloat);
			Register<Vector2>(InspectorRenderVector2);
			Register<Vector3>(InspectorRenderVector3);
			Register<Vector4>(InspectorRenderVector4);
			Register<Color>(InspectorRenderColor);
			Register<bool>(InspectorRenderBool);
			Register<string>(InspectorRenderString);
			Register<IComponent>(InspectorRenderIComponent);
			Register<AsepriteFile>(InspectorRenderAsepriteAssetFile);
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
				IComponent com = (IComponent)propertyInfo.GetValue(component);
				if (com.Node != null) {
					value = $"{com.Node.Name}";
				} else {
					value = $"{((IComponent)propertyInfo.GetValue(component)).Guid}";
				}
			} else {
				value = Guid.Empty.ToString();
			}
			
			if (isMissing) {
				Color color = Color.DarkRed;
				Vector4 chosenColor = new Vector4(color.R, color.G, color.B, color.A);
				ImGui.PushStyleColor(ImGuiCol.Text, chosenColor);
			}
			
			ImGui.Text(value);

			//Node Guid Drop Target: Serialized Field
			DropTarget nodeGuidDropTarget = ImGuiHelpers.DropTarget<NodeDragDropGuid>();
			if (nodeGuidDropTarget.HasDragDropAsset) {
				IDragDropGuid dragDropGuid = (IDragDropGuid)nodeGuidDropTarget.DragDropAsset;
				Guid guid = Guid.Parse(dragDropGuid.Guid);
				if (GuidDatabase.Instance.NodeMap.ContainsKey(guid)) {
					Node nodeValue = GuidDatabase.Instance.NodeMap[guid];
					IComponent componentValue = (IComponent)nodeValue.GetType()
						.GetMethod("GetComponent")
						.MakeGenericMethod(propertyInfo.PropertyType)
						.Invoke(nodeValue, null);

					if (componentValue != null) {
						propertyInfo.SetValue(component, componentValue);
					}
				}
			}
			
			//Component Guid Drop Target: Serialized Field
			DropTarget componentGuidDropTarget = ImGuiHelpers.DropTarget<ComponentDragDropGuid>();
			if (componentGuidDropTarget.HasDragDropAsset) {
				IDragDropGuid dragDropGuid = (IDragDropGuid)componentGuidDropTarget.DragDropAsset;
				Guid guid = Guid.Parse(dragDropGuid.Guid);
				if (GuidDatabase.Instance.ComponentMap.ContainsKey(guid)) {
					IComponent componentValue = GuidDatabase.Instance.ComponentMap[guid];
					//Double check it is the correct type for the property
					if (componentValue.GetType() == propertyInfo.PropertyType) {
						propertyInfo.SetValue(component, componentValue);
					}
				}
			}
			
			if (isMissing) {
				ImGui.PopStyleColor();
			}
			
			ImGui.SameLine();
			ImGui.Text($"{propertyInfo.Name}");
		}

		public void InspectorRenderAsepriteAssetFile(object component, PropertyInfo propertyInfo) {
			object obj = propertyInfo.GetValue(component);
			string value = "NULL";
			bool isMissing = obj == null;
			if (obj != null) {
				AsepriteFile asepriteFile = (AsepriteFile)propertyInfo.GetValue(component);
				if (asepriteFile != null) {
					value = asepriteFile.FilePath;
				}
			}
			
			if (isMissing) {
				Color color = Color.DarkRed;
				Vector4 chosenColor = new Vector4(color.R, color.G, color.B, color.A);
				ImGui.PushStyleColor(ImGuiCol.Text, chosenColor);
			}
			
			ImGui.Text(value);

			DropTarget nodeGuidDropTarget = ImGuiHelpers.DropTarget<AsepriteDragDropAsset>();
			if (nodeGuidDropTarget.HasDragDropAsset) {
				IDragDropAsset dragDropAsset = (IDragDropAsset)nodeGuidDropTarget.DragDropAsset;
				propertyInfo.SetValue(component, new AsepriteFile(dragDropAsset.FilePath));

			}
			
			if (isMissing) {
				ImGui.PopStyleColor();
			}
			
			ImGui.SameLine();
			ImGui.Text($"{propertyInfo.Name}");

			AsepriteComponent asepriteComponent = (AsepriteComponent)component;
			if (!isMissing && asepriteComponent.Texture != null) {
				ImGui.Image(
					(IntPtr)asepriteComponent.Texture.ID, 
					new Vector2(asepriteComponent.Texture.Width * 5, asepriteComponent.Texture.Height * 5), 
					new Vector2(0, 0), 
					new Vector2(1, 1), 
					Vector4.One, 
					Vector4.One
				);
			}
		}

		#endregion
		
	}
}