using System.Collections.Generic;
using System.Windows;
using DiskInformer.Model;

namespace DiskInformer.View
{
	public partial class StatisticsWindow : Window
	{
		public StatisticsWindow(int updateTime, int dataSize, ICollection<PhisicalDisk> phisicalDisks)
		{
			InitializeComponent();
			DataContext = new ViewModel.StatisticsWindowViewModel(this,updateTime,dataSize,phisicalDisks);
		}
	}
}
