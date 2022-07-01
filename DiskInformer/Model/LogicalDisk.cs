namespace DiskInformer.Model
{
	public class LogicalDisk
	{
		public string Name { get; set; }
		public int Size { get; set; }
		public int FreeSpace { get; set; }
		public int OccupiedPlace { get; set; }
		public string Description { get; set; }
		public string FileSystem { get; set; }
		public LogicalDisk(string name, int size, int freeSpace, int occupiedPlace, string description, string fileSystem)
		{
			Name = name;
			Size = size;
			FreeSpace = freeSpace;
			OccupiedPlace = occupiedPlace;
			Description = description;
			FileSystem = fileSystem;
		}
	}
}

