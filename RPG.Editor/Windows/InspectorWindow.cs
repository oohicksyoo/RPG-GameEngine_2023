namespace RPG.DearImGUI.Windows {
	using System.Diagnostics;
	using System.Numerics;
	using System.Reflection;
	using Engine.Attributes;
	using Engine.Components;
	using Engine.Components.Interfaces;
	using Engine.Core;
	using Engine.Core.Interfaces;
	using ImGuiNET;
	using Debug = Engine.Utility.Debug;

	public class InspectorWindow : AbstractWindow {

		private EditorModule EditorModule {
			get;
			set;
		}
		
		public InspectorWindow(bool isOpen = true) : base(isOpen) {
			this.EditorModule = Application.Instance.Get<EditorModule>();
		}

		public override string Name => "Inspector";

		protected override void OnRenderGui() {
			if (this.EditorModule == null) {
				return;
			}

			Node node = this.EditorModule.SelectedNode;
			if (node == null) {
				ImGui.Text($"Please select a Node");
				return;
			}
			
			//Enabled
			bool isEnabled = node.IsEnabled;
			if (ImGui.Checkbox("##IsEnabled", ref isEnabled)) {
				node.IsEnabled = isEnabled;
			}

			ImGui.SameLine();

			//Name
			string name = node.Name;
			ImGui.InputText("Name", ref name, 32);
			node.Name = name;
			
			//Tag
			ImGui.SetNextItemWidth(ImGui.GetItemRectSize().X / 1.5f);
			string tag = node.Tag;
			if (ImGui.InputText("Tag", ref tag, 24)) {
				node.Tag = tag;
			}
			
			//Guid
			ImGui.TextDisabled(node.Guid.ToString());
			ImGui.SameLine();
			ImGui.Text("Guid");
			
			//Component Rendering
			RenderComponents(node, node.Components);
		}

		private void RenderComponents(Node node, List<IComponent> components) {
			foreach (var component in components) {
				bool isOpen = ImGui.CollapsingHeader(component.GetType().Name, ImGuiTreeNodeFlags.DefaultOpen);
				bool wasModified = false;

				if (!isOpen) {
					continue;
				}

				if (ImGui.IsItemClicked(ImGuiMouseButton.Right)) {
					ImGui.OpenPopup($"Component Context##{component.Guid}");
				}
				
				//Find all properties in the class marked as Inspector
				PropertyInfo[] properties = component.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

				foreach (PropertyInfo propertyInfo in properties) {
					if (!Attribute.IsDefined(propertyInfo, typeof(Inspector))) {
						continue;
					}
					
					if (!propertyInfo.CanWrite || !propertyInfo.CanRead) {
						continue;
					}
					
					MethodInfo mget = propertyInfo.GetGetMethod(false);
					MethodInfo mset = propertyInfo.GetSetMethod(false);

					// Get and set methods have to be public
					if (mget == null) {
						continue;
					}
					if (mset == null) {
						continue;
					}

					if (!Attribute.IsDefined(propertyInfo, typeof(Inspector))) {
						continue;
					}

					Type t = propertyInfo.PropertyType;
					Action<object, PropertyInfo> inspectorRenderingAction = EditorModule.Get(propertyInfo.PropertyType);
					inspectorRenderingAction?.Invoke(component, propertyInfo);
				}
				
				//Render Add Component Popup
				if (ImGui.BeginPopup($"Component Context##{component.Guid}")) {
					ImGui.TextColored(new Vector4(0,0,0,1), "Add Component");
					ImGui.Separator();
					
					//Use Reflection to grab all the types derived from Component
					Type componentType = typeof(AbstractComponent);
					List<Type> types = new List<Type>();
					types.AddRange(Assembly.GetAssembly(componentType)
						.GetTypes()
						.Where(myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(componentType)));

					Type projectAssembly = Application.Instance.Project.GetType();
					types.AddRange(Assembly.GetAssembly(projectAssembly)
						.GetTypes()
						.Where(x => x.IsClass && !x.IsAbstract && x.IsSubclassOf(componentType)));
					
					foreach (var type in types) {
						if (ImGui.Selectable(type.Name)) {
							wasModified = true;
							node.GetType()
								.GetMethod("AddComponent")
								.MakeGenericMethod(type)
								.Invoke(node, null);
						}
					}
					
					ImGui.Separator();
					bool isTransform = component.GetType() == typeof(Transform);
					if (isTransform) {
						ImGui.BeginDisabled();
					}
					if (ImGui.Selectable("Delete")) {
						wasModified = true;
						node.GetType()
							.GetMethod("RemoveComponent")
							.MakeGenericMethod(component.GetType())
							.Invoke(node, null);
					}
					if (isTransform) {
						ImGui.EndDisabled();
					}

					ImGui.EndPopup();
				}

				if (wasModified) {
					//Return early because collection was modified
					return;
				}
			}
		}
	}
}