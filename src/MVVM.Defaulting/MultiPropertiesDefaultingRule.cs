using MVVM.Defaulting.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MVVM.Defaulting
{
    public class MultiPropertiesDefaultingRule<T> : BaseDefaultingRule<T, IEnumerable<object>>
        where T : INotifyPropertyChanged
    {
        public List<Expression<Func<T, object>>> Properties { get; set; }
        public List<Action<T, object>> PropSetters { get; private set; }

        public MultiPropertiesDefaultingRule(List<Expression<Func<T, object>>> props)
        {
            Properties = props;
            PropSetters = props.Select(s => s.GetPropertySetter()).ToList();
        }

        public override void RunRuleAction(T notifyObject, string changedPropName)
        {
            base.RunRuleAction(notifyObject, changedPropName);

            if (RuleFunc == null) return;
            var values = RuleFunc(notifyObject, changedPropName).ToList();
            PropSetters.ForEach((setter, i) => setter(notifyObject, values[i]));
        }

    }
}
