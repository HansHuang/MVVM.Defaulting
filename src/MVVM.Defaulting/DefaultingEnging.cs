using MVVM.Defaulting.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVVM.Defaulting
{
    public static class DefaultingEnging
    {
        public static D ApplyDefaulting<T, D>(this T notifyObject)
            where T : INotifyPropertyChanged
            where D : AbstaractDefaultor<T>, new()
        {
            var defaultor = new D();

            notifyObject.PropertyChanged += (s, e) =>
            {
                defaultor.Rules.ForEach(r =>
                {
                    //Support chained property
                    if (!r.RelyOnPropNames.Contains(e.PropertyName) && !r.RelyOnPropNames.Any(x=> x.StartsWith(e.PropertyName + "."))) return;
                    if (r.ExecuteCondition != null && !r.ExecuteCondition(notifyObject)) return;

                    r.RunRuleAction(notifyObject, e.PropertyName);
                });
            };

            return defaultor;
        }

    }
}
