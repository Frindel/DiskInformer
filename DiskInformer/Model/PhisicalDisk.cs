using System.Collections.Generic;

namespace DiskInformer.Model
{
	internal class PhisicalDisk
	{
		public string Name { get; private set; }
		/// <summary>
		/// size in gigabytes
		/// </summary>
		public int Size { get; private set; }
		/// <summary>
		/// space in gigabytes
		/// </summary>
		public int Partitions { get; private set; }
		/// <summary>
		/// occupied place in gigabytes
		/// </summary>
		public int TotalCylinders { get; private set; }
		/// <summary>
		/// weight in gram
		/// </summary>
		public int TotalSectors { get; private set; }
		public string SerialNumber { get; private set; }
		public string InterfaceType { get; private set; }
		public List<LogicalDisk> LogicalDisks { get; private set; }

		public PhisicalDisk(string name, int size, int partitions, int totalCylinders, int totalSectors, string serialNumber, string interfaceType, ICollection<LogicalDisk> logicalDisks=null)
		{
			Name = name;
			Size = size;
			Partitions = partitions;
			TotalCylinders = totalCylinders;
			TotalSectors = totalSectors;
			SerialNumber = serialNumber;
			InterfaceType = interfaceType;
			LogicalDisks = (logicalDisks is null) ? new List<LogicalDisk>() : new List<LogicalDisk>(logicalDisks);
		}

	}
}
