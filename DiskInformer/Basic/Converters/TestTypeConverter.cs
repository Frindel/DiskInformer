using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using DiskInformer.ViewModel;

namespace DiskInformer.Basic.Converters
{
	internal class TestTypeConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			StatisticsWindowViewModel.TestType testTypeAction = (StatisticsWindowViewModel.TestType)value;
			return testTypeAction.ToString();
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return Enum.Parse(typeof(StatisticsWindowViewModel.TestType), (string)value);
		}
	}
}
