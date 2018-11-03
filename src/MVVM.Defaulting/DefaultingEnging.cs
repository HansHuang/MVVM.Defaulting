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
                if (defaultor.PreventDeadLoop && defaultor.ChangedProperties.Contains((e.PropertyName))) return;
                defaultor.ChangedProperties.Add((e.PropertyName));

                defaultor.Rules.ForEach(r =>
                {
                    if (defaultor.IsDisabled) return;

                    //Support chained property
                    if (!r.RelyOnPropNames.Contains(e.PropertyName) && !r.RelyOnPropNames.Any(x => x.StartsWith(e.PropertyName + "."))) return;
                    if (r.ExecuteCondition != null && !r.ExecuteCondition(notifyObject)) return;

                    r.RunRuleAction(notifyObject, e.PropertyName);
                });

                defaultor.ChangedProperties.Remove((e.PropertyName));
            };

            return defaultor;
        }

    }
}
