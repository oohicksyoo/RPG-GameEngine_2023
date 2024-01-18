namespace RPG.Engine.Attributes {
	using Serialization.Interfaces;

	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
	public class Inspector : Attribute {}
}