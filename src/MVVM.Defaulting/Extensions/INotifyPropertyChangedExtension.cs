using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MVVM.Defaulting.Extensions
{
    public static class INotifyPropertyChangedExtension
    {
        public static MulticastDelegate GetPropertyChangedDelegate(this INotifyPropertyChanged notifyObj)
        {
            MulticastDelegate multiDlg = null;
            var type = notifyObj.GetType();
            while (type != null)
            {
                var fi = type.GetField("PropertyChanged", BindingFlags.NonPublic | BindingFlags.Instance);
                type = type.BaseType;
                if (fi == null) continue;

                multiDlg = fi.GetValue(notifyObj) as MulticastDelegate;
            }

            return multiDlg;
        }
    }
}
