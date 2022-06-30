using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Management;
using DiskInformer.Basic;
using DiskInformer.Model;

namespace DiskInformer.ViewModel
{
	internal class MainWindowViewModel : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		private ObservableCollection<PhisicalDisk> _phisicalDisks;

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
		#endregion
		#region command
		public RelayCommand OpenStatisticWindow
		{
			get => new RelayCommand(()=>
			{
				View.StatisticsWindow statisticsWindow = new View.StatisticsWindow(5,100);
				statisticsWindow.Show();
				statisticsWindow.Focus();
			});
		}
		#endregion

		#region constructors
		public MainWindowViewModel()
		{
			_phisicalDisks = new ObservableCollection<PhisicalDisk>();
			GetDiskInfo();
		}
		#endregion

		#region protected methods
		/// <summary>
		/// Geting current computer disks info
		/// </summary>
		private void GetDiskInfo()
		{
			ManagementObjectSearcher managment = new ManagementObjectSearcher("SELECT * FROM Win32_DiskDrive");
			foreach (ManagementObject mo in managment.Get())
			{
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
						DriveInfo dr = new DriveInfo((string)c["Name"]);
						phisicalDisk.LogicalDisks.Add(new LogicalDisk(
						(string)c["Name"],
						(int)(Convert.ToInt64(c["Size"]) / 1073741824),
						(int)(Convert.ToInt64(dr.AvailableFreeSpace) / 1073741824),
						((int)(Convert.ToInt64(c["Size"]) / 1073741824) - (int)(Convert.ToInt64(dr.AvailableFreeSpace) / 1073741824)),
						(string)c["SystemName"],
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
				PropertyChanged.Invoke(propertyName, new PropertyChangedEventArgs(propertyName));
		}
		#endregion
	}
}
