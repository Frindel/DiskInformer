using DiskInformer.Basic;
using DiskInformer.Model;
using DiskInformer.View;
using ScottPlot;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DiskInformer.ViewModel
{
	public class StatisticsWindowViewModel : INotifyPropertyChanged
	{
		public enum TestType { Read, Write }

		public event PropertyChangedEventHandler PropertyChanged;

		private const int bytesInKilobytes = 1024;

		private StatisticsWindow _window;
		private readonly int _updateTime;
		private readonly int _dataSize; //в килобайтах 
		private readonly int _step; //в килобайтах 

		private ObservableCollection<LogicalDisk> _logicalDisks;
		private LogicalDisk _selectedLogicalDisk;
		private TestType _testTypeAction;
		private bool _testActive;
		private List<double> _plotPoints;

		private double _maxSpeed;
		private double _currentSpeed;
		private double _avgSpeed;

		private string _fileURI;
		#region property
		public double MaxSpeed
		{
			get => _maxSpeed;
			set
			{
				_maxSpeed = value;
				OnPropertyChanged(nameof(MaxSpeed));
			}
		}
		public double CurrentSpeed
		{
			get => _currentSpeed;
			set
			{
				_currentSpeed = value;
				OnPropertyChanged(nameof(CurrentSpeed));
			}
		}
		public double AvgSpeed
		{
			get => _avgSpeed;
			set
			{
				_avgSpeed = value;
				OnPropertyChanged(nameof(AvgSpeed));
			}
		}
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
			get => new RelayCommand(async () =>
			{
				_plotPoints.Clear();
				MaxSpeed = AvgSpeed = CurrentSpeed = 0;

				//проверка существования файла
				if (_testTypeAction == TestType.Read)
				{
					//создание файла, необходимого для тестирования записи
					string systemDisk = Path.GetPathRoot(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData));
					if (systemDisk== _selectedLogicalDisk.Name + "\\")
						_fileURI = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "FileForTestDisk");
					else 
						_fileURI = Path.Combine(_selectedLogicalDisk.Name+"\\\\", "FileForTestDisk");

					using (FileStream fs = new FileStream(_fileURI, FileMode.Create, FileAccess.ReadWrite, FileShare.None, _step, (FileOptions)0x20000000))
					{
						byte[] writingValues = new byte[bytesInKilobytes * _dataSize];

						new Random().NextBytes(writingValues);
						fs.Write(writingValues, 0, writingValues.Length);
					}
				}

				_testActive = true;

				await Task.Run(() =>
				{
					for (int i = 0; i < 20; i++)
					{
						while (Monitor.IsEntered(this)) { }
							ChangePlot();
						Task.Delay(_updateTime * 1000).Wait();
					}
					_testActive = false;
				});

			},
			() => !_testActive && _selectedLogicalDisk != null);
		}
		public RelayCommand StopTest
		{
			get => new RelayCommand(() =>
			{
				_testActive = false;
			},
			() => _testActive);
		}
		#endregion

		#region constructors
		public StatisticsWindowViewModel(StatisticsWindow window, int updateTime, int dataSize, int step, ICollection<PhisicalDisk> phisicalDisks)
		{
			_testActive = false;
			_plotPoints = new List<double>();
			_window = window;
			_updateTime = updateTime;
			_dataSize = dataSize;
			_step = step;
			_logicalDisks = new ObservableCollection<LogicalDisk>();

			//получение локальных дисков устройства
			foreach (PhisicalDisk a in phisicalDisks)
				foreach (LogicalDisk b in a.LogicalDisks)
					_logicalDisks.Add(b);

			//выделение первого устройства в списке
			if (_logicalDisks.Count > 0)
				SelectedLogialDisk = _logicalDisks[0];

			MaxSpeed = AvgSpeed = CurrentSpeed = 0;

			//настройка графика
			WpfPlot plot = window.plot;
			plot.Plot.SetOuterViewLimits(0, 20.5, 0, 5000);
			plot.Plot.YLabel("МБ/c");
			plot.Plot.XAxis.ManualTickSpacing(1);
		}
		#endregion

		#region protected methods
		private void ChangePlot()
		{
			//получение информации о выделенном локальном диске
			Stopwatch sw = new Stopwatch();
			//тест чтения
			if (_testTypeAction == TestType.Read)
			{
				Monitor.Enter(this);
				try
				{
					using (FileStream fs = new FileStream(_fileURI, FileMode.Open, FileAccess.Read, FileShare.None, 4096, (FileOptions)0x20000000))
					{
						byte[] buffer = new byte[fs.Length];

						sw.Restart();

						fs.Read(buffer, 0, (int)fs.Length);
					}
					sw.Stop();
				}
				finally
				{
					Monitor.Exit(this);
				}
			}
			//тест записи
			if (_testTypeAction == TestType.Write)
			{
				Monitor.Enter(this);
				try
				{
					using (FileStream fs = new FileStream(_fileURI, FileMode.Create, FileAccess.Write, FileShare.None, 4096, (FileOptions)0x20000000 | FileOptions.WriteThrough))
					{
						byte[] writingValues = new byte[bytesInKilobytes * _dataSize];
						new Random().NextBytes(writingValues);

						sw.Restart();

						fs.Write(writingValues, 0, writingValues.Length);
					}
					sw.Stop();
				}
				finally
				{
					Monitor.Exit(this);
				}
			}

			//подсчет скорости
			double rez = Math.Round((_dataSize / 1024) / (sw.ElapsedMilliseconds * 0.001), 2);

			_plotPoints.Add(rez);
			CurrentSpeed = rez;
			if (MaxSpeed < CurrentSpeed)
				MaxSpeed = CurrentSpeed;
			AvgSpeed = Math.Round(_plotPoints.Sum() / _plotPoints.Count, 2);

			//отображение новой информации
			WpfPlot plot = _window.plot;
			plot.Plot.Clear();
			double[] dataY = _plotPoints.ToArray();
			double[] dataX = new double[_plotPoints.Count];
			for (int i = 0; i < _plotPoints.Count; i++)
				dataX[i] = i + 1;

			plot.Plot.AddScatter(dataX, dataY);

			_window.Dispatcher.Invoke(() => plot.Refresh());
		}
		private void OnPropertyChanged(string propertyName)
		{
			if (PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}
		#endregion
	}
}
