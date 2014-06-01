namespace Comical.Core
{
	public class Spread
	{
		public Spread() { }

		public Spread(ImageReference fill)
		{
			Left = fill;
			FillSpread = true;
		}

		public ImageReference Left { get; set; }

		public ImageReference Right { get; set; }

		public bool FillSpread { get; private set; }
	}
}
