namespace RPG.Engine.Core {
	public static class Time {

		
		#region Properties

		public static float Delta => Application.Instance.SystemModule.Delta;

		public static ulong ElapsedDuration => Application.Instance.SystemModule.ElapsedDuration;

		#endregion

	}
}