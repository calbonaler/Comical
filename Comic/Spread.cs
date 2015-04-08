namespace Comical.Core
{
	public class Spread
	{
		public Spread() { }

		bool _fillSpread;

		public ImageReference Left { get; private set; }

		public ImageReference Right { get; private set; }

		public ImageReference Fill { get { return _fillSpread ? Left : null; } }

		public bool FillSpread { get { return _fillSpread; } }

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
				_fillSpread = true;
			}
			else if (mode == ImageViewMode.Left)
			{
				Left = value;
				_fillSpread = false;
			}
			else if (mode == ImageViewMode.Right)
			{
				Right = value;
				_fillSpread = false;
			}
		}
	}
}
