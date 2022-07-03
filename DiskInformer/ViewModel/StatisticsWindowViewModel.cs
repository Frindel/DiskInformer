using DiskInformer.View;
using ScottPlot;
using System.ComponentModel;
using System.Timers;
using System.Collections.Generic;
using DiskInformer.Model;
using System.IO;
using System.Collections.ObjectModel;
using DiskInformer.Basic;
using System.Text;
using System.Diagnostics;
using System;

namespace DiskInformer.ViewModel
{
	public class StatisticsWindowViewModel : INotifyPropertyChanged
	{
		public enum TestType { Read,Write} 

		public event PropertyChangedEventHandler PropertyChanged;

		private StatisticsWindow _window;
		private readonly int _updateTime;
		//in kilobytes
		private readonly int _dataSize;
		private ObservableCollection<LogicalDisk> _logicalDisks;
		private LogicalDisk _selectedLogicalDisk;
		private TestType _testTypeAction;
		private bool _testActive;
		private List<double> _plotPoints; 

		#region property
		public ObservableCollection<LogicalDisk> LogicalDisks
		{
			get => _logicalDisks;
			set
			{
				_logicalDisks = value;
				OnPropertyChanged(nameof(LogicalDisk));
			}
		}
		public LogicalDisk SelectedLogialDisk
		{
			get => _selectedLogicalDisk;
			set
			{
				_selectedLogicalDisk = value;
				OnPropertyChanged(nameof(SelectedLogialDisk));
			}
		}
		public TestType TestTypeAction
		{
			get => _testTypeAction;
			set
			{
				_testTypeAction = value;
				OnPropertyChanged(nameof(TestTypeAction));
			}
		}
		#endregion

		#region Command
		public RelayCommand StartTest
		{
			get => new RelayCommand(()=>
			{
				_plotPoints.Clear();

				//проверка существования файла
				if (_testTypeAction == TestType.Read&&!File.Exists(_selectedLogicalDisk.Name+"\\\\FileForTestDisk"))
				
				{
					//создание файла, необходимого для тестирования записи
					using (StreamWriter sw = new StreamWriter(_selectedLogicalDisk.Name+ "\\\\FileForTestDisk",true,Encoding.ASCII))
					{
						//производится запись файла, размерностью 1 МБ с шагом в 8 байт
						for (int i = 0; i < 131072; i++)
						{
							sw.Write(new string('t',8));
						}
					}
				}

				Timer timer = new Timer(_updateTime * 1000);
				timer.AutoReset = true;
				timer.Elapsed += (sender, e) =>
				{
					if (_plotPoints.Count == 10 || !_testActive)
					{
						((Timer)sender).Stop();
						_testActive = false;
					}
					ChangePlot();
				};
				timer.Start();

				_testActive = true;
			},
			()=> _selectedLogicalDisk != null);
		}
		public RelayCommand StopTest
		{
			get => new RelayCommand(()=>
			{
				_testActive = false;
			},
			()=>_testActive);
		}
		#endregion

		#region constructors
		public StatisticsWindowViewModel(StatisticsWindow window, int updateTime, int dataSize,ICollection<PhisicalDisk> phisicalDisks)
		{
			_testActive = false;
			_plotPoints = new List<double>();
			_window = window;
			_updateTime = updateTime;
			_dataSize = dataSize;
			_logicalDisks = new ObservableCollection<LogicalDisk>();
			foreach (PhisicalDisk a in phisicalDisks)
				foreach (LogicalDisk b in a.LogicalDisks)
					_logicalDisks.Add(b);

			if (_logicalDisks.Count > 0)
				SelectedLogialDisk = _logicalDisks[0];

			//настройка графика
			WpfPlot plot = window.plot;
			plot.Plot.SetOuterViewLimits(0, 10, 0, 500);
			plot.Plot.YLabel("байт/c");
			plot.Plot.XAxis.ManualTickSpacing(1);
		}
		#endregion

		#region protected methods
		private async void ChangePlot()
		{
			//get current disk info
			Stopwatch sv = new Stopwatch();
			//тест чтения
			if (_testTypeAction == TestType.Read)
			{
				using (StreamReader sr = new StreamReader(new FileStream(_selectedLogicalDisk.Name + "\\\\FileForTestDisk", FileMode.Open, FileAccess.Read)))
				{
					sv.Start();
					//производится чтение файла, размерностью 1 МБ с шагом в 8 байт
					for (int i = 0; i < 131072; i++)
					{
						sr.Read(new char[8], 0, 8);
					}
					sv.Stop();
				}
			}
			//тест записи
			if (_testTypeAction == TestType.Write)
			{
				using (StreamWriter sw = new StreamWriter(_selectedLogicalDisk.Name + "\\\\FileForTestDisk", false, Encoding.ASCII))
				{
					sv.Start();
					//производится запись файла, размерностью 1 МБ с шагом в 8 байт
					for (int i = 0; i < 131072; i++)
					{
						sw.Write(new string('t', 8));
					}
					sv.Stop();
				}
			}
			double rez = 1048576 / sv.Elapsed.TotalMilliseconds * 0.001;
			_plotPoints.Add(Math.Round(rez,1));

			//edit plot
			WpfPlot plot = _window.plot;
			plot.Plot.Clear();
			double[] dataY = _plotPoints.ToArray();
			double[] dataX = new double[_plotPoints.Count];
			for (int i = 0; i < _plotPoints.Count; i++)
				dataX[i] = i+1;

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
