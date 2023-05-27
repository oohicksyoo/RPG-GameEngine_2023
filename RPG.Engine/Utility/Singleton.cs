namespace RPG.Engine.Utility {
	public abstract class Singleton<T> where T : class, new() {
		protected Singleton() { }

		private static readonly Lazy<T> instance = new Lazy<T>(() => new T());

		public static T Instance {
			get {
				return instance.Value;
			}
		}
	}
}