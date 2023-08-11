namespace RPG.DearImGUI.Utility {
	using System.Runtime.CompilerServices;
	using System.Runtime.InteropServices;
	using DragDrop;
	using DragDrop.Interfaces;
	using Engine.Utility;
	using ImGuiNET;

	public static class ImGuiHelpers {

		public static void DragSource<T>(T dragDropAsset) where T : IDragDropAsset {
			if (ImGui.BeginDragDropSource()) {
				IntPtr intPtr = dragDropAsset.ToIntPtr();
				uint size = (uint)Marshal.SizeOf(intPtr);
				ImGui.SetDragDropPayload($"{dragDropAsset.GetType().Name}", intPtr, size);
				ImGui.Text($"Asset - {dragDropAsset.Type}");
				ImGui.EndDragDropSource();
			}
		}
		
		public static DropTarget DropTarget<T>() where T : IDragDropAsset {
			DropTarget dropTarget = default;
			
			if (ImGui.BeginDragDropTarget()) {
				ImGuiPayloadPtr payloadPtr = ImGui.AcceptDragDropPayload($"{typeof(T).Name}");
				if (!payloadPtr.Equals(default(ImGuiPayloadPtr))) {
					dropTarget.HasDragDropAsset = true;
					dropTarget.DragDropAsset = payloadPtr.Data.FromIntPtr<T>();
				}
			}

			return dropTarget;
		}
		
	}
}