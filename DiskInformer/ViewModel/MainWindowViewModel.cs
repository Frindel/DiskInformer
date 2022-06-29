using System.Collections.ObjectModel;
using System.ComponentModel;
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
				
			});
		}
		#endregion

		#region constructors
		public MainWindowViewModel()
		{
			GetDiskInfo();
		}
		#endregion

		#region protected methods
		/// <summary>
		/// Geting current computer disks info
		/// </summary>
		private void GetDiskInfo()
		{
		
		}
		private void OnPropertyChanged(string propertyName)
		{
			if (PropertyChanged != null)
				PropertyChanged.Invoke(propertyName, new PropertyChangedEventArgs(propertyName));
		}
		#endregion
	}
}
