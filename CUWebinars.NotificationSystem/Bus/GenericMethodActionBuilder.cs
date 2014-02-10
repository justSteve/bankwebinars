using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace CUWebinars.NotificationSystem.Bus
{
    public class GenericMethodActionBuilder<TargetBase, ParamBase>
    {
        readonly Dictionary<Type, Action<TargetBase, ParamBase>> _actionCache = new Dictionary<Type, Action<TargetBase, ParamBase>>();
        readonly Type _targetType;
        readonly string _method;

        public GenericMethodActionBuilder(Type targetType, string method)
        {
            _targetType = targetType;
            _method = method;
        }

        public Action<TargetBase, ParamBase> GetAction(ParamBase paramInstance)
        {
            var paramType = paramInstance.GetType();

            if (!_actionCache.ContainsKey(paramType))
            {
                _actionCache.Add(paramType, BuildActionForMethod(paramType));
            }

            return _actionCache[paramType];
        }

        private Action<TargetBase, ParamBase> BuildActionForMethod(Type paramType)
        {
            var handlerType = _targetType.MakeGenericType(paramType);

            var ehParam = Expression.Parameter(typeof(TargetBase));
            var evtParam = Expression.Parameter(typeof(ParamBase));
            var invocationExpression =
                Expression.Lambda(
                    Expression.Block(
                        Expression.Call(
                            Expression.Convert(ehParam, handlerType),
                            handlerType.GetMethod(_method),
                            Expression.Convert(evtParam, paramType))),
                    ehParam, evtParam);

            return (Action<TargetBase, ParamBase>)invocationExpression.Compile();
        }
    }
}
