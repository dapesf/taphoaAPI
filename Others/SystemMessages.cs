using System.Reflection;
using System.Resources;

namespace App
{
	public static class SystemMessages
	{
		private static readonly ResourceManager _resourceManager =
			new ResourceManager("taphoa.Resources.Messages", Assembly.GetExecutingAssembly());

		public static string Get(string key)
		{
			return _resourceManager.GetString(key) ?? $"[{key}]";
		}
	}
}
