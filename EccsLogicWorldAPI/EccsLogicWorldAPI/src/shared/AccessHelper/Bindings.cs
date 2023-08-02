using System.Reflection;

namespace EccsLogicWorldAPI.Shared.AccessHelper
{
	public static class Bindings
	{
		public const BindingFlags any = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;
		public const BindingFlags privateStatic = BindingFlags.NonPublic | BindingFlags.Static;
		public const BindingFlags privateInst = BindingFlags.NonPublic | BindingFlags.Instance;
		public const BindingFlags publicStatic = BindingFlags.Public | BindingFlags.Static;
		public const BindingFlags publicInst = BindingFlags.Public | BindingFlags.Instance;
		public const BindingFlags ppStatic = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static;
		public const BindingFlags ppInst = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance;
	}
}
