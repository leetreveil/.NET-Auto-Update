using System.IO;

namespace NAppUpdate.Framework.Utils
{
	public static class FileSystem
	{
		public static void CreateDirectoryStructure(string path)
		{
			CreateDirectoryStructure(path, true);
		}

		public static void CreateDirectoryStructure(string path, bool pathIncludeFile)
		{
			string[] paths = path.Split(Path.DirectorySeparatorChar);

			// ignore the last split because its the filename
			int loopCount = paths.Length;
			if (pathIncludeFile)
				loopCount--;

			for (int ix = 0; ix < loopCount; ix++)
			{
				string newPath = paths[0] + @"\";
				for (int add = 1; add <= ix; add++)
					newPath = Path.Combine(newPath, paths[add]);
				if (!Directory.Exists(newPath))
					Directory.CreateDirectory(newPath);
			}
		}
	}
}
