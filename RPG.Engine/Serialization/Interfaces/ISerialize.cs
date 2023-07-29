namespace RPG.Engine.Serialization.Interfaces {
	using Newtonsoft.Json.Linq;

	public interface ISerialize<T> {

		public JObject Serialize();
		
	}
}