using DiskInformer.View;
using OxyPlot;
using ScottPlot;
using System.ComponentModel;
using System.Timers;
using LiveCharts;

namespace DiskInformer.ViewModel
{
	internal class StatisticsWindowViewModel : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;
		private StatisticsWindow _window;
		private readonly int _updateTime;
		//in kilobytes
		private readonly int _dataSize;
		#region property

		#endregion

		#region constructors
		public StatisticsWindowViewModel(StatisticsWindow window, int updateTime, int dataSize)
		{
			_window = window;
			_updateTime = updateTime;
			_dataSize = dataSize;



			WpfPlot plot = window.plot;
			plot.Plot.SetOuterViewLimits(0, 100, 0, 500);

			Timer timer = new Timer(updateTime * 1000);
			timer.AutoReset = true;
			timer.Elapsed += (sender, e) => ChangeSchedule();
			timer.Start();
		}
		#endregion

		#region protected methods
		private async void ChangeSchedule()
		{
			//get current disk info


			//edit plot
			WpfPlot plot = _window.plot;
			plot.Plot.Clear();
			double[] dataX = new double[] { 20, 40 };
			double[] dataY = new double[] { 1, 4 };
			plot.Plot.AddScatter(dataX, dataY);

			_window.Dispatcher.Invoke(()=>plot.Refresh());
		}
		private void OnPropertyChanged(string propertyName)
		{
			if (PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}
		#endregion
	}
}
