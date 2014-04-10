using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using CUWebinars.Business.Models;

namespace CUWebinars.Business.Core
{
    public class DisconnectedPropertyChangeHelper
    {
        private readonly TTSWebinarsContext _context;

        public DisconnectedPropertyChangeHelper(DbContext context)
        {
            _context = (TTSWebinarsContext) context;
        }

        public void ApplyChanges<TEntity>(TEntity root)
            where TEntity : class, IObjectWithState
        {
            _context.Set<TEntity>().Add(root);
            CheckForEntitiesWithoutStateInterface(_context);

            foreach (var entry in _context.ChangeTracker.Entries<IObjectWithState>())
            {
                IObjectWithState entityWithStateInfo = entry.Entity;
                entry.State = ConvertState(entityWithStateInfo.DomainEntityState);
                if (entityWithStateInfo.DomainEntityState == State.Unchanged)
                {
                    ApplyPropertyChanges(entry.OriginalValues, entityWithStateInfo.OriginalValues);
                }
            }
            _context.SaveChanges();
        }

        public EntityState ConvertState(State state)
        {
            switch (state)
            {
                case State.Added:
                    return EntityState.Added;
                case State.Deleted:
                    return EntityState.Deleted;
                default:
                    return EntityState.Unchanged;
            }
        }

        private void ApplyPropertyChanges(DbPropertyValues values, Dictionary<string, object> originalValues)
        {
            foreach (var originalValue in originalValues)
            {
                if (originalValue is Dictionary<string, object>)
                {
                    ApplyPropertyChanges(
                        (DbPropertyValues)values[originalValue.Key],
                        (Dictionary<string, object>)originalValue.Value
                        );
                }
                else
                {
                    values[originalValue.Key] = originalValue.Value;
                }
            }
        }

        private void CheckForEntitiesWithoutStateInterface(DbContext context)
        {
            var entitiesWithoutState = from entry in _context.ChangeTracker.Entries()
                                       where !(entry.Entity is IObjectWithState)
                                       select entry;

            if (entitiesWithoutState.Any())
            {
                throw new NotSupportedException("All entities must implement IObjectWithState");
            }
        }

        public void MaterializeObject(IObjectWithState objectWithState)
        {
            if (ReferenceEquals(objectWithState, null)) return;
            objectWithState.DomainEntityState = State.Unchanged;
            objectWithState.OriginalValues = BuildOriginalValues(_context.Entry(objectWithState).OriginalValues);            
        }

        private Dictionary<string, object> BuildOriginalValues(DbPropertyValues originalValues)
        {
            var result = new Dictionary<string, object>();

            foreach (var propertyName in originalValues.PropertyNames)
            {
                var value = originalValues[propertyName];

                if (value is DbPropertyValues)
                {
                    result[propertyName] = BuildOriginalValues((DbPropertyValues)value);
                }
                else
                {
                    result[propertyName] = value;
                }
            }
            return result;
        }
    }
}