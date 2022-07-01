using System.Collections.Generic;

namespace DiskInformer.Model
{
	public class PhisicalDisk
	{
		public string Name { get; set; }
		public int Size { get; set; }
		public int Partitions { get; set; }
		public int TotalCylinders { get; set; }
		public int TotalSectors { get; set; }
		public string SerialNumber { get; set; }
		public string InterfaceType { get; set; }
		public List<LogicalDisk> LogicalDisks { get; set; }
		public PhisicalDisk(string name, int size, int partitions, int totalCylinders, int totalSectors, string serialNumber, string interfaceType, ICollection<LogicalDisk> logicalDisks = null)
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
