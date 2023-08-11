namespace RPG.Engine.Utility {
	using System.Runtime.InteropServices;

	public static class ObjectHandleExtensions {
		public static IntPtr ToIntPtr(this object target) {
			return GCHandle.Alloc(target).ToIntPtr();
		}
		
		public static IntPtr ToIntPtr(this GCHandle target) {
			return GCHandle.ToIntPtr(target);
		}

		public static GCHandle ToGcHandle(this IntPtr target) {
			return GCHandle.FromIntPtr(target);
		}

		public static T FromIntPtr<T>(this IntPtr target) {
			return (T)ToGcHandle(target).Target;
		}
		
	}
}