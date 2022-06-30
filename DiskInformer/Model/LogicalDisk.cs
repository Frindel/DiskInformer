using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiskInformer.Model
{
	internal class LogicalDisk
	{
		public string Name { get; private set; }
		/// <summary>
		/// size in gigabytes
		/// </summary>
		public int Size { get; private set; }
		/// <summary>
		/// space in gigabytes
		/// </summary>
		public int FreeSpace { get; private set; }
		/// <summary>
		/// occupied place in gigabytes
		/// </summary>
		public int OccupiedPlace { get; private set; }



		public string FileSystemName { get; private set; }
		public string FileSystem { get; private set; }
		public LogicalDisk(string name, int size, int freeSpace, int occupiedPlace, string fileSystemName, string fileSystem)
		{
			Name = name;
			Size = size;
			FreeSpace = freeSpace;
			OccupiedPlace = occupiedPlace;
			FileSystemName = fileSystemName;
			FileSystem = fileSystem;
		}
	}
}
