namespace RPG.Engine.Serialization {
	using System.Numerics;
	using Newtonsoft.Json.Linq;

	public static class SerializationExtensions {

		public static JObject ToJObject(this Vector2 vector2) {
			JObject jsonObject = new JObject();

			jsonObject[nameof(vector2.X)] = vector2.X;
			jsonObject[nameof(vector2.Y)] = vector2.Y;
			
			return jsonObject;
		}

		public static Vector2 FromJObject(this JObject jsonObject) {
			Vector2 vector2 = Vector2.Zero;

			vector2.X = (float)jsonObject[nameof(vector2.X)];
			vector2.Y = (float)jsonObject[nameof(vector2.Y)];
			
			return vector2;
		}
		
	}
}