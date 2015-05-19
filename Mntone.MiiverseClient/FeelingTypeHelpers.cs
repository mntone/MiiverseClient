using System;

namespace Mntone.MiiverseClient
{
	public static class FeelingTypeHelpers
	{
		public static FeelingType DetectFeelingTypeFromIconUri(Uri iconUri)
		{
			if (iconUri.AbsolutePath.EndsWith("_normal_face.png"))
			{
				return FeelingType.Normal;
			}
			else if (iconUri.AbsolutePath.EndsWith("_happy_face.png"))
			{
				return FeelingType.Happy;
			}
			else if (iconUri.AbsolutePath.EndsWith("_like_face.png"))
			{
				return FeelingType.Like;
			}
			else if (iconUri.AbsolutePath.EndsWith("_surprised_face.png"))
			{
				return FeelingType.Surprised;
			}
			else if (iconUri.AbsolutePath.EndsWith("_frustrated_face.png"))
			{
				return FeelingType.Frustrated;
			}
			else if (iconUri.AbsolutePath.EndsWith("_puzzled_face.png"))
			{
				return FeelingType.Puzzled;
			}

			throw new Exception();
		}
	}
}
