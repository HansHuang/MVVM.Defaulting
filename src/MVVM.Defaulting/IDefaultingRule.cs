using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVVM.Defaulting
{
    public interface IDefaultingRule<T> where T : INotifyPropertyChanged
    {
        List<string> RelyOnPropNames { get; set; }
        Predicate<T> ExecuteCondition { get; set; }

        void RunRuleAction(T notifyObject, string changedPropName);
        void RelyOn(List<string> properties);
    }
}
