namespace EccsGuiBuilder.Client.Layouts.Helper
{
	public readonly struct IndexHelper
	{
		// This is inverted, so that the default struct counts from the beginning.
		private readonly bool countFromBack;
		private readonly int rawIndex;
		
		private IndexHelper(bool countFromBack, int rawIndex)
		{
			this.countFromBack = countFromBack;
			this.rawIndex = rawIndex;
		}
		
		public bool getCountFromFront() => !countFromBack;
		public int getIndex() => rawIndex;
		
		/// <summary> Selects the first index to grow. </summary>
		/// <remarks> This is the default. Equal to <code>.nth(0)</code> </remarks>
		public static readonly IndexHelper First = new IndexHelper(false, 0);
		/// <summary> Selects the nth index to grow. </summary>
		/// <remarks> Includes index 0, so the 1-nth is the second index. </remarks>
		public static IndexHelper nth(int index) => new IndexHelper(false, index);
		
		/// <summary> Selects the last index to grow. </summary>
		/// <remarks> Equal to <code>.nthLast(0)</code> </remarks>
		public static readonly IndexHelper Last = new IndexHelper(true, 0);
		/// <summary> Selects the nth last index to grow. </summary>
		/// <remarks> Includes index 0, so the 1-nth-last is the second last index. </remarks>
		public static IndexHelper nthLast(int index) => new IndexHelper(true, index);
	}
}
