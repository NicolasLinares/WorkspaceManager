using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using WorkspaceManagerTool.Models;
using WorkspaceManagerTool.Properties;

namespace WorkspaceManagerTool.Utils {
    public class ImageConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            switch (value) {
                case QuickAccessType.FILE:
                    return "/WorkspaceManagerTool;component/Views/Images/file.png";
                default:
                    return "/WorkspaceManagerTool;component/Views/Images/folder.png";
            }
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}
