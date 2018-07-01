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
    public abstract class BaseDefaultingRule<T, Tproperty> : IDefaultingRule<T>
        where T : INotifyPropertyChanged
    {
        public List<string> RelyOnPropNames { get; set; }
        public Predicate<T> ExecuteCondition { get; set; }
        public Func<T, string, Tproperty> RuleFunc { get; set; }
        public Action<T, string> RuleAction { get; set; }

        public BaseDefaultingRule()
        {
            RelyOnPropNames = new List<string>();
        }

        public virtual void RunRuleAction(T notifyObject, string changedPropName)
        {
            if (RuleAction == null) return;
            RuleAction(notifyObject, changedPropName);
        }

        #region RelyOn
        /// <summary>
        /// Set the properties of defaulting logic rely on.(To be wathched notify properties)
        /// </summary>
        /// <param name="properties">expression (chained property is supported)</param>
        /// <returns></returns>
        public BaseDefaultingRule<T, Tproperty> RelyOn(params Expression<Func<T, object>>[] properties)
        {
            var propNames = properties.Select(s => s.GetPropetyName()).ToList();
            RelyOn(propNames);
            return this;
        }

        /// <summary>
        /// Set the properties of defaulting logic rely on.(To be wathched notify properties)
        /// </summary>
        /// <param name="properties">string (e.g. ID, User.Profile.FirstName)</param>
        /// <returns></returns>
        public BaseDefaultingRule<T, Tproperty> RelyOn(params string[] properties)
        {
            RelyOn(properties.ToList());
            return this;
        }

        public void RelyOn(List<string> properties)
        {
            RelyOnPropNames.AddRange(properties);
            RelyOnPropNames = RelyOnPropNames.Distinct().ToList();
        }
        #endregion

        #region SetValue
        /// <summary>
        /// Set the property value directly
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public BaseDefaultingRule<T, Tproperty> SetValue(Tproperty value)
        {
            RuleFunc = (_, __) => value;
            return this;
        }

        /// <summary>
        /// Set the property value function
        /// </summary>
        /// <param name="valueFun">Input: NotifyObject, Output: PropertyValue</param>
        /// <returns></returns>
        public BaseDefaultingRule<T, Tproperty> SetValue(Func<T, Tproperty> valueFun)
        {
            RuleFunc = (t, _) => valueFun(t);
            return this;
        }

        /// <summary>
        /// Set the property value function
        /// </summary>
        /// <param name="valueFun">Input: NotifyObject, ChangedPropertyName, Output: PropertyValue</param>
        /// <returns></returns>
        public BaseDefaultingRule<T, Tproperty> SetValue(Func<T, string, Tproperty> valueFun)
        {
            RuleFunc = valueFun;
            return this;
        }
        #endregion

        #region SetAction
        public BaseDefaultingRule<T, Tproperty> SetAction(Action<T, string> action)
        {
            RuleAction = action;
            return this;
        }
        #endregion

        #region When
        /// <summary>
        /// Set Execute Condition for defaulting rule
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public BaseDefaultingRule<T, Tproperty> When(Expression<Func<T, bool>> condition)
        {
            RelyOn(condition.GetAllPropertyNames().ToArray());

            var condFun = condition.Compile();
            ExecuteCondition = (T nodifyObj) => condFun(nodifyObj);

            return this;
        }
        #endregion

    }
}
