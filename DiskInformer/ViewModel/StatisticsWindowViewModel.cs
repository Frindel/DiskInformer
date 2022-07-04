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
using System.Text;

namespace DiskInformer.ViewModel
{
	public class StatisticsWindowViewModel : INotifyPropertyChanged
	{
		public enum TestType { Read, Write }

		public event PropertyChangedEventHandler PropertyChanged;

		private const int ByteInKilobytes = 1024;

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
			get => new RelayCommand(() =>
			{
				_plotPoints.Clear();
				MaxSpeed = AvgSpeed = CurrentSpeed = 0;

				//проверка существования файла
				if (_testTypeAction == TestType.Read &&
				(!File.Exists($"{_selectedLogicalDisk.Name}\\\\FileForTestDisk") ||
				new FileInfo($"{_selectedLogicalDisk.Name}\\\\FileForTestDisk").Length != _dataSize))
				{
					//создание файла, необходимого для тестирования записи
					using (StreamWriter sw = new StreamWriter($"{_selectedLogicalDisk.Name}\\\\FileForTestDisk", true, Encoding.ASCII))
					{
						for (int i = 0; i < _dataSize / _step; i++)
						{
							sw.Write(new string('t', ByteInKilobytes * _step));
						}
					}
				}

				System.Timers.Timer timer = new System.Timers.Timer(_updateTime * 1000);
				timer.AutoReset = true;
				timer.Elapsed += (sender, e) =>
				{
					//условие завершения работы теста
					if (_plotPoints.Count == 20 || !_testActive)
					{
						((System.Timers.Timer)sender).Stop();
						_testActive = false;
						return;
					}

					//проверка на отсутствие подключений к тестируемому файлу
					if (!FileBusy($"{_selectedLogicalDisk.Name}\\\\FileForTestDisk"))
						ChangePlot();
				};
				timer.Start();

				_testActive = true;
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
			plot.Plot.SetOuterViewLimits(0, 20.5, 0, 500);
			plot.Plot.YLabel("МБ/c");
			plot.Plot.XAxis.ManualTickSpacing(1);
		}
		#endregion

		#region protected methods
		private void ChangePlot()
		{
			//получение информации о выделенном локальном диске
			Stopwatch sv = new Stopwatch();
			//тест чтения
			if (_testTypeAction == TestType.Read)
			{
				using (StreamReader sr = new StreamReader(new FileStream(_selectedLogicalDisk.Name + "\\\\FileForTestDisk", FileMode.Open, FileAccess.Read), Encoding.ASCII))
				{

					char[] buffer = new char[3];

					sv.Start();

					for (int i = 0; i < _dataSize / 3; i++)
					{
						sr.Read(buffer, 0, 3);
					}
					sv.Stop();
				}
			}
			//тест записи
			if (_testTypeAction == TestType.Write)
			{
				using (StreamWriter sw = new StreamWriter(_selectedLogicalDisk.Name + "\\\\FileForTestDisk", false, Encoding.ASCII))
				{

					string recordedValue = new string('t', ByteInKilobytes * _step);

					sv.Start();

					for (int i = 0; i < _dataSize / _step; i++)
					{
						sw.Write(recordedValue);
					}
					sv.Stop();
				}
			}

			//подсчет скорости
			double rez = Math.Round((_dataSize / 1024) / (sv.ElapsedMilliseconds * 0.001), 2);

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
		
		private bool FileBusy(string path)
		{
			try
			{
				StreamWriter sw = new StreamWriter(path);
				sw.Close();
				return false;
			}
			catch (IOException)
			{
				return true;
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
