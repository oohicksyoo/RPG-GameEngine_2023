namespace RPG.Engine.Utility {
	public static class MathHelper {

		public static float ToRadians(float degrees) {
			return (float)((Math.PI / 180f) * degrees);
		}

		public static float ToDegrees(float radians) {
			return (float)((180f / Math.PI) * radians);
		}
		
		public static float Remap(float value, float a1, float a2, float b1, float b2) {
			return b1 + (value - a1) * (b2 - b1) / (a2 - a1);
		}
		
		public static float Lerp(float a, float b, float t) {
			return a + (b - a) * t;
		}
		
	}
}