using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Management;
using DiskInformer.Model;
using DiskInformer.Basic;

namespace DiskInformer.ViewModel
{
	public class MainWindowViewModel : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		private ObservableCollection<PhisicalDisk> _phisicalDisks;
		private PhisicalDisk _selectedPhisicalDisk;
		private LogicalDisk _selectedLogicalDisk;
		#region property
		public ObservableCollection<PhisicalDisk> PhisicalDisks
		{
			get => _phisicalDisks;
			set
			{
				_phisicalDisks = value;
				OnPropertyChanged(nameof(PhisicalDisks));
			}
		}
		public PhisicalDisk SelectedPhisicalDisk
		{
			get => _selectedPhisicalDisk;
			set
			{
				_selectedPhisicalDisk = value;
				OnPropertyChanged(nameof(SelectedPhisicalDisk));
			}
		}
		public LogicalDisk SelectedLogicalDisk
		{
			get => _selectedLogicalDisk;
			set
			{
				_selectedLogicalDisk = value;
				OnPropertyChanged(nameof(SelectedLogicalDisk));
			}
		}
		#endregion
		#region Commands
		public RelayCommand DisplayStatisticsWindow
		{
			get => new RelayCommand(() =>
			{
				View.StatisticsWindow statisticsWindow = new View.StatisticsWindow(5, 102400,256,_phisicalDisks);
				statisticsWindow.Show();
				statisticsWindow.Focus();
			});
		}
		#endregion

		#region constructors
		public MainWindowViewModel()
		{
			_phisicalDisks = new ObservableCollection<PhisicalDisk>();
			GetDisksInfo();
			if (_phisicalDisks.Count != 0)
				SelectedPhisicalDisk = _phisicalDisks[0];
			if (SelectedPhisicalDisk.LogicalDisks.Count != 0)
				SelectedLogicalDisk = SelectedPhisicalDisk.LogicalDisks[0];
		}
		#endregion

		#region protected methods
		private void GetDisksInfo()
		{
			ManagementObjectSearcher managment = new ManagementObjectSearcher("SELECT * FROM Win32_DiskDrive");
			foreach (ManagementObject mo in managment.Get())
			{
				//получение информации о физическом диске
				PhisicalDisk phisicalDisk = new PhisicalDisk(
				(string)mo["Model"],
				(int)(Convert.ToInt64(mo["Size"]) / 1073741824),
				(int)Convert.ToInt64(mo["Partitions"]),
				(int)Convert.ToInt64(mo["TotalCylinders"]),
				(int)Convert.ToInt64(mo["TotalSectors"]),
				(string)mo["SerialNumber"],
				(string)mo["InterfaceType"]
				);

				foreach (ManagementObject b in mo.GetRelated("Win32_DiskPartition"))
				{
					foreach (ManagementBaseObject c in b.GetRelated("Win32_LogicalDisk"))
					{
						//получение информации о локальном диске
						DriveInfo dr = new DriveInfo((string)c["Name"]);

						phisicalDisk.LogicalDisks.Add(new LogicalDisk(
						(string)c["Name"],
						(int)(Convert.ToInt64(c["Size"]) / 1073741824),
						(int)(Convert.ToInt64(dr.AvailableFreeSpace) / 1073741824),
						(int)(Convert.ToInt64(c["Size"]) / 1073741824) - (int)(Convert.ToInt64(dr.AvailableFreeSpace) / 1073741824),
						(string)c["Description"],
						(string)c["FileSystem"]
						));
					}
				}
				_phisicalDisks.Add(phisicalDisk);
			}
		}
		private void OnPropertyChanged(string propertyName)
		{
			if (PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}
		#endregion
	}
}
