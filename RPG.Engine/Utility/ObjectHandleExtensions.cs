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
		
		//Array Handling for GPU data
		public static IntPtr ArrayToIntPtr(this float[] target) {
			IntPtr data = Marshal.AllocHGlobal(sizeof(float) * target.Length);
			Marshal.Copy(target, 0, data, target.Length);
			return data;
		}
		
		public static IntPtr ArrayToIntPtr(this int[] target) {
			IntPtr data = Marshal.AllocHGlobal(sizeof(int) * target.Length);
			Marshal.Copy(target, 0, data, target.Length);
			return data;
		}
		
	}
}