using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Flatmate.Models.IRepositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Flatmate.Models.Repositories
{
    /// <summary>
    /// Generic Repository class independant on application
    /// </summary>
    /// <typeparam name="TEntity">Entity Model type</typeparam>
    public class Repository<TEntity>: IRepository<TEntity> where TEntity : class
    {
        protected readonly DbContext Context;
        private DbSet<TEntity> _entities;

        public Repository(DbContext context)
        {
            Context = context;
            _entities = Context.Set<TEntity>();
        }

        public TEntity GetById(int id) {
            return _entities.Find(id);
        }
        public IEnumerable<TEntity> GetAll() {
            return _entities.ToList();
        }
        public IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> predicate) {
            return _entities.Where(predicate);
        }      
        public void Add(TEntity entity) {
            _entities.Add(entity);
        }

        public void AddRange(IEnumerable<TEntity> entities) {
            _entities.AddRange(entities);
        }

        public void Update(TEntity entity) {
            _entities.Update(entity);
            DisplayStates(Context.ChangeTracker.Entries());
        }

        private static void DisplayStates(IEnumerable<EntityEntry> entries) {
            foreach (var entry in entries) {
                System.Diagnostics.Debug.WriteLine($"Entity: {entry.Entity.GetType().Name},State: { entry.State.ToString()}");
            }
        }

        public void Update(TEntity entity, params string[] propertiesToUpdate) {
            // this approach is unable of changing child Entities of root Entity Graph
            // EntryState can't be set to 'Modified' value for them - use Update method
            // more: https://www.entityframeworktutorial.net/efcore/working-with-disconnected-entity-graph-ef-core.aspx
            _entities.Attach(entity);
            var entry = Context.Entry(entity);
            foreach (var property in propertiesToUpdate)
            {
                var collectionEntry = entry.Collection(property);
                collectionEntry.IsModified = true;
            }
            entry.State = EntityState.Modified;
        }

        public void Remove(TEntity entity) {
            _entities.Remove(entity);
        }

        public void RemoveRange(IEnumerable<TEntity> entities) {
            _entities.RemoveRange(entities);
        }

    }
}
