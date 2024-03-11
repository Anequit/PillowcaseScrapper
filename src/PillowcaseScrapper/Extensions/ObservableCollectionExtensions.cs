using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PillowcaseScrapper.Extensions;
internal static class ObservableCollectionExtensions
{
	public static void Populate<T>(this ObservableCollection<T> collection, IEnumerable<T> newCollection)
	{
		IEnumerator<T> enumerator = newCollection.GetEnumerator();

		while(enumerator.MoveNext())
		{
			collection.Add(enumerator.Current);
		}
	}
}
