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
    public abstract class AbstaractDefaultor<T> : IDisposable
        where T : INotifyPropertyChanged
    {
        #region Fields
        public List<IDefaultingRule<T>> Rules = new List<IDefaultingRule<T>>();
        #endregion

        #region RuleFor
        public DefaultingRule<T, Tproperty> RuleFor<Tproperty>(Expression<Func<T, Tproperty>> property)
        {
            var rule = new DefaultingRule<T, Tproperty>(property);
            Rules.Add(rule);
            return rule;
        }

        public MultiPropertiesDefaultingRule<T> RuleFor(params Expression<Func<T, object>>[] properties)
        {
            var rule = new MultiPropertiesDefaultingRule<T>(properties.ToList());
            Rules.Add(rule);
            return rule;
        }
        #endregion

        #region When/Else
        public AbstaractDefaultor<T> When(Expression<Predicate<T>> condition, Action action)
        {
            var exeCondition = condition.Compile();
            var relayOnNames = condition.GetAllPropertyNames();

            var subRules = GetSubRules(action);
            subRules.ForEach(s => s.ExecuteCondition = s.ExecuteCondition == null ? exeCondition : s.ExecuteCondition.And(exeCondition));
            subRules.ForEach(s => s.RelyOn(relayOnNames));

            var whenRule = new WhenRule<T>
            {
                ExecuteCondition = exeCondition,
                RelyOnPropNames = relayOnNames
            };
            Rules.Add(whenRule);

            return this;
        }

        public AbstaractDefaultor<T> Else(Action action)
        {
            var whenRule = Rules.LastOrDefault();
            if (whenRule == null || !(whenRule is WhenRule<T>))
                throw new InvalidOperationException("Else must follow a When expression");

            Predicate<T> exeCondition = s => !whenRule.ExecuteCondition(s);
            var subRules = GetSubRules(action);
            subRules.ForEach(s => s.ExecuteCondition = s.ExecuteCondition == null ? exeCondition : s.ExecuteCondition.And(exeCondition));
            subRules.ForEach(s => s.RelyOn(whenRule.RelyOnPropNames));

            Rules.Remove(whenRule);
            return this;
        }
        #endregion

        #region Member of IDisposable
        public void Dispose()
        {
            Rules.Clear();
        }
        #endregion

        #region Private Members
        private List<IDefaultingRule<T>> GetSubRules(Action action)
        {
            var preRules = Rules.ToList();
            action();
            return Rules.Except(preRules).ToList();
        }

        #region Private Class: WhenRule
        private class WhenRule<T> : IDefaultingRule<T> where T : INotifyPropertyChanged
        {

            public List<string> RelyOnPropNames { get; set; }

            public Predicate<T> ExecuteCondition { get; set; }

            public void RunRuleAction(T notifyObject, string changedPropName)
            {

            }

            public void RelyOn(List<string> properties)
            {
                RelyOnPropNames = properties;
            }
        }
        #endregion
        #endregion

    }
}
