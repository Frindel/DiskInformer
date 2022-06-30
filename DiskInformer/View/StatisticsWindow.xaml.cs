using System.Windows;

namespace DiskInformer.View
{
	public partial class StatisticsWindow : Window
	{
		public StatisticsWindow(int updateTime, int dataSize)
		{
			InitializeComponent();
			DataContext = new ViewModel.StatisticsWindowViewModel(this,updateTime,dataSize);
		}
	}
}
