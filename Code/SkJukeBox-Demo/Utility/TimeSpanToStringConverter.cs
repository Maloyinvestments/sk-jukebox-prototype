using System;
using System.Windows;
using System.Windows.Data;


namespace SkJukeBox_Demo.Utility
{
    class TimeSpanToStringConverter : IValueConverter
    {

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            TimeSpan t = new TimeSpan();
            if (value is TimeSpan)
            {
                t = (TimeSpan)value;
            }
            else if(value is Duration)
            {
                if (((Duration)value).HasTimeSpan)
                {
                    t = ((Duration) value).TimeSpan;
                }
            }

            string minutes = t.Minutes.ToString();
            if (t.Minutes < 10) minutes = "0" + minutes;

            string seconds = t.Seconds.ToString();
            if (t.Seconds < 10) seconds = "0" + seconds;

            return minutes + ":" + seconds;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }

        #endregion
    }
}
