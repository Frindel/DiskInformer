using System.Collections.Generic;
using System.Windows;
using DiskInformer.Model;

namespace DiskInformer.View
{
	public partial class StatisticsWindow : Window
	{
		//размер данных и шага представляют в килобайтах
		public StatisticsWindow(int updateTime, int dataSize, int step, ICollection<PhisicalDisk> phisicalDisks)
		{
			InitializeComponent();
			DataContext = new ViewModel.StatisticsWindowViewModel(this,updateTime,dataSize, step,phisicalDisks);
		}
	}
}
