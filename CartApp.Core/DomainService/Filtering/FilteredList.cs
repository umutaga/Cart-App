using System.Collections.Generic;

namespace CartApp.Core.Entity
{
	public class FilteredList<T>
	{
		public IEnumerable<T> List { get; set; }
		public int Count { get; set; }
	}
}
