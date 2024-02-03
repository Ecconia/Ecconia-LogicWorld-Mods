namespace CustomChatManager.Server
{
	public static class ChatColors
	{
		public static readonly string highlight = wrap("fa0");
		public static readonly string background = wrap("ccc");
		public static readonly string failure = wrap("ff6464");
		public static readonly string close = "</color>";
		
		private static string wrap(string colorCode)
		{
			return "<#" + colorCode + ">";
		}
	}
}
