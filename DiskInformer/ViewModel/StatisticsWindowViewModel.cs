using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiskInformer.ViewModel
{
	internal class StatisticsWindowViewModel : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		#region property

		#endregion

		#region constructors
		public StatisticsWindowViewModel()
		{
		
		}
		#endregion

		#region protected methods
		private void OnPropertyChanged(string propertyName)
		{
			if (PropertyChanged !=null)
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}
		#endregion
	}
}
