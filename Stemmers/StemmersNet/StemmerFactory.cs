using System;
using System.Linq;
using System.Reflection;

namespace SIL.Stemmers
{
	public static class StemmerFactory
	{
		public static IStemmer GetStemmer(string icuLocale)
		{
			var iStemmerType = typeof(IStemmer);
			var stemmerAttributeType = typeof(StemmerAttribute);
			foreach (var stemmerType in Assembly.GetExecutingAssembly().GetTypes()
				.Where(t => iStemmerType.IsAssignableFrom(t) && !t.IsInterface))
			{
				var b = (stemmerType.GetCustomAttributes(stemmerAttributeType, true).Cast<StemmerAttribute>()).Single();
				if (b.IcuLocale == icuLocale)
				{
					var constructor = stemmerType.GetConstructor(Type.EmptyTypes);
					if (constructor != null)
					return (IStemmer)(constructor.Invoke(null));
				}
			}
			return null;
		}
	}
}
