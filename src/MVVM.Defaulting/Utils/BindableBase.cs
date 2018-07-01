using MVVM.Defaulting.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;

namespace MVVM.Defaulting.Utils
{
    public abstract class BindableBase : INotifyPropertyChanged, IDisposable
    {
        #region INotifyPropertyChanged Members
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="action"></param>
        protected void OnPropertyChanged<T>(Expression<Func<T>> action)
        {
            var propertyName = GetPropertyName(action);
            OnPropertyChanged(propertyName);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="action"></param>
        /// <returns></returns>
        private static string GetPropertyName<T>(Expression<Func<T>> action)
        {
            var expression = (MemberExpression)action.Body;
            var propertyName = expression.Member.Name;
            return propertyName;
        }
        protected bool SetProperty<T>(ref T backingField, T Value, Expression<Func<T>> propertyExpression)
        {
            var changed = !EqualityComparer<T>.Default.Equals(backingField, Value);

            if (changed)
            {
                backingField = Value;
                this.OnPropertyChanged(GetPropertyName(propertyExpression));
            }

            return changed;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="propertyName"></param>
        public void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler == null) return;

            var e = new PropertyChangedEventArgs(propertyName);
            handler(this, e);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region IDisposable Member
        public virtual void Dispose()
        {
            //Clear all PropertyChanged Handlers
            if (PropertyChanged == null) return;

            PropertyChanged.GetInvocationList().OfType<PropertyChangedEventHandler>().ToList().ForEach(s => PropertyChanged -= s);
        }
        #endregion

        #region BubblePropertyChanged
        public void ClearBubblePropertyChanged(Expression<Func<INotifyPropertyChanged>> property)
        {
            property.GetChainNodes().ForEach(prop =>
            {
                var notifyObj = prop.GetGetter()() as INotifyPropertyChanged;
                if (notifyObj == null) return;

                var multiDlg = notifyObj.GetPropertyChangedDelegate();
                if (multiDlg == null) return;

                var handlers = multiDlg.GetInvocationList().Where(s => s.Method.Name.StartsWith("<RegisterBubbleChanged>")).ToList();
                handlers.ForEach(s => notifyObj.PropertyChanged -= (PropertyChangedEventHandler)s);
            });
        }

        public void BubblePropertyChanged(params Expression<Func<INotifyPropertyChanged>>[] properties)
        {
            var handlerList = new List<PropertyChangedEventHandler>();
            var prefixList = new Dictionary<Expression<Func<INotifyPropertyChanged>>, List<string>>();
            properties.ForEach(prop =>
            {
                prefixList.Add(prop, new List<string>());

                var chainStack = new Stack<MemberExpression>(prop.GetChainNodes());
                while (chainStack.Count > 0)
                {
                    var prefix = chainStack.Select(s => s.Member.Name).Reverse().Aggregate((a, b) => string.Join(".", a, b));
                    var node = chainStack.Pop();
                    var getter = node.GetGetter();

                    RegisterBubbleChanged(getter, prefix, handlerList);
                    prefixList[prop].Add(prefix);
                }
            });

            //Re-register bubblePropertyChanged for changed member
            PropertyChanged += (_, e) =>
            {
                var item = prefixList.FirstOrDefault(s => s.Value.Contains(e.PropertyName));
                if (item.Key == null) return;

                ClearBubblePropertyChanged(item.Key);
                BubblePropertyChanged(item.Key);
            };
        }

        private void RegisterBubbleChanged(Func<object> getter, string prefix, List<PropertyChangedEventHandler> handlerList)
        {
            var notifyObj = getter() as INotifyPropertyChanged;
            if (notifyObj == null || string.IsNullOrWhiteSpace(prefix)) return;

            //lazy-implementation for the handler so that can unregister itself from inside
            var actions = new List<Action<object, string>>();
            PropertyChangedEventHandler handler = (s, e) => actions.ForEach(x => x(s, e.PropertyName));
            actions.Add((s, e) =>
            {
                object latestNode = null;
                try
                {
                    latestNode = getter();
                }
                catch //(NullReferenceException)
                {
                    notifyObj.PropertyChanged -= handler;
                }
                //verify current instance is still in bubble chain or not
                if (Equals(s, latestNode)) OnPropertyChanged(string.Join(".", prefix, e));
                else notifyObj.PropertyChanged -= handler;
            });

            //Prevent duplicated handler register
            handlerList.ForEach(s => notifyObj.PropertyChanged -= s);
            notifyObj.PropertyChanged += handler;
            handlerList.Add(handler);
        }
        #endregion

        #region Processors
        protected List<MemberExpression> GetChainNodes(Expression longExp)
        {
            var list = new List<MemberExpression>();

            var member = longExp as MemberExpression;
            if (member == null)
            {
                var unary = longExp as UnaryExpression;
                if (unary == null) return list;

                member = unary.Operand as MemberExpression;
            }
            if (member == null) return list;

            list.Add(member);
            list.AddRange(GetChainNodes(member.Expression));
            return list;
        }
        #endregion
    }
}
