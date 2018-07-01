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
    public class DefaultingRule<T, Tproperty> : BaseDefaultingRule<T, Tproperty>
        where T : INotifyPropertyChanged
    {
        public Expression<Func<T, Tproperty>> Property { get; private set; }
        public Action<T, Tproperty> PropSetter { get; private set; }

        public DefaultingRule(Expression<Func<T, Tproperty>> prop)
        {
            RelyOnPropNames = new List<string>();
            PropSetter = prop.GetPropertySetter();
        }

        public override void RunRuleAction(T notifyObject, string changedPropName)
        {
            base.RunRuleAction(notifyObject, changedPropName);

            if (RuleFunc == null) return;
            var value = RuleFunc(notifyObject, changedPropName);
            PropSetter(notifyObject, value);
        }

        #region OneWayBinding
        /// <summary>
        /// One way binding the value of another notify property
        /// </summary>
        /// <param name="target">To watch notify property</param>
        /// <returns></returns>
        public DefaultingRule<T, Tproperty> OneWayBinding(Expression<Func<T, Tproperty>> target)
        {
            var targetGetter = target.Compile();
            RelyOn(target.GetPropetyName());
            RuleFunc = (t, _) => targetGetter(t);

            return this;
        }
        #endregion

    }
}
