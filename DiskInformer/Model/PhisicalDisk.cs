using System.Collections.Generic;

namespace DiskInformer.Model
{
	internal class PhisicalDisk
	{
		public string Name { get; private set; }
		/// <summary>
		/// size in bytes
		/// </summary>
		public int Size { get; private set; }
		/// <summary>
		/// space in bytes
		/// </summary>
		public int FreeSpace { get; private set; }
		/// <summary>
		/// occupied place in bytes
		/// </summary>
		public int OccupiedPlace { get; private set; }
		/// <summary>
		/// weight in gram
		/// </summary>
		public int Weight { get; private set; }
		public int? RotationSpeed { get; private set; }
		public string InterfaceType { get; private set; }
		public int DataSpeed { get; private set; }
		/// <summary>
		/// byffer size in bytes
		/// </summary>
		public int BufferSize { get; private set; } 
		public List<LogicalDisk> LogicalDisks { get; private set; }

		public PhisicalDisk(string name, int size, int freeSpace, int occupiedPlace, int weight, int? rotationSpeed,string interfaceType, int dataSpeed, int bufferSize, ICollection<LogicalDisk> logicalDisks = null)
		{
			Name = name;
			Size = size;
			FreeSpace = freeSpace;
			OccupiedPlace = occupiedPlace;
			Weight = weight;
			RotationSpeed = rotationSpeed;
			InterfaceType = interfaceType;
			DataSpeed = dataSpeed;
			BufferSize = bufferSize;
			LogicalDisks = (logicalDisks is null) ? new List<LogicalDisk>() : new List<LogicalDisk>(logicalDisks);
		}

	}
}
