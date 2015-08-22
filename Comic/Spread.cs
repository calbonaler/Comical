namespace Comical.Core
{
	public class Spread
	{
		public Spread() { }
		
		public ImageReference Left { get; private set; }

		public ImageReference Right { get; private set; }

		public ImageReference Fill => FillSpread ? Left : null;

		public bool FillSpread { get; private set; }

		public ImageReference GetImage(ImageViewMode mode)
		{
			if (mode == ImageViewMode.Right)
				return Right;
			else
				return Left;
		}

		public void SetImage(ImageReference value, ImageViewMode mode)
		{
			if (mode == ImageViewMode.Default)
			{
				Left = value;
				Right = null;
				FillSpread = true;
			}
			else if (mode == ImageViewMode.Left)
			{
				Left = value;
				FillSpread = false;
			}
			else if (mode == ImageViewMode.Right)
			{
				Right = value;
				FillSpread = false;
			}
		}
	}
}
