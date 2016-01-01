using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace Designer.Components
{
    public class WarningToColorConverter : IValueConverter
    {

        public Brush ErrorFill { get; set; }
        public Brush WarningFill { get; set; }


        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool fIsWarning = false;

            if (value == null)
                return ErrorFill;

            if (bool.TryParse(value.ToString(), out fIsWarning))
            {
                return fIsWarning ? WarningFill : ErrorFill;
            }

            return ErrorFill;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
