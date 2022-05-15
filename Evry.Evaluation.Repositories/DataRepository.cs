using Evry.Evaluation.Data;
using Evry.Evaluation.Interfaces.Data;
using Evry.Evaluation.Interfaces.Repositories;
using Evry.Evaluation.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Evry.Evaluation.Repositories
{
    /// <summary>
    /// Data access repository
    /// </summary>
    public class DataRepository : IDataRepository
    {
        private IDataSession _session;

        public DataRepository()
        {
            _session = DataHelper.GetTestData();
        }

        /// <summary>
        /// Initialize repository with custom data session
        /// </summary>
        /// <param name="session"></param>
        public DataRepository(IDataSession session)
        {
            _session = session;
        }

        /// <summary>
        /// Get all events
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Event> GetEvents()
        {
            return _session.Set<Event>();
        }

        /// <summary>
        /// Get events matching date period
        /// </summary>
        /// <param name="start">Start of period</param>
        /// <param name="end">End of period</param>
        /// <returns></returns>
        public IEnumerable<Event> GetEventsByDateRange(DateTime start, DateTime end)
        {
            // EVAL
            IEnumerable<Person> persons = GetPeople().Where(x => x.Starts >= start && x.Leaves <= end);
            IEnumerable<Guid> tmp = new List<Guid>();
            foreach (Person person in persons)
            {
                tmp.Append<Guid>(person.ID);
            }

            return _session.Set<Event>().Where(x => tmp.Contains(x.PersonID));
        }

        /// <summary>
        /// Get events for specified person
        /// </summary>
        /// <param name="personId"></param>
        /// <returns></returns>
        public IEnumerable<Event> GetEventsByPerson(Guid personId)
        {
            return _session.Set<Event>().Where(x => x.PersonID == personId);
        }

        /// <summary>
        /// Get events for specified region
        /// </summary>
        /// <param name="regionId"></param>
        /// <returns></returns>
        public IEnumerable<Event> GetEventsByRegion(Guid regionId)
        {
            // EVAL
            IEnumerable<Person> persons = GetPeople().Where(x => x.RegionID == regionId);
            IEnumerable<Guid> tmp = new List<Guid>();
            foreach (Person person in persons)
            {
                tmp.Append<Guid>(person.ID);
            }

            return _session.Set<Event>().Where(x => tmp.Contains(x.PersonID));
        }

        /// <summary>
        /// Get events of specified type
        /// </summary>
        /// <param name="typeId"></param>
        /// <returns></returns>
        public IEnumerable<Event> GetEventsByType(Guid typeId)
        {
            return _session.Set<Event>().Where(x => x.TypeID == typeId);
        }

        /// <summary>
        /// Get all event types
        /// </summary>
        /// <returns></returns>
        public IEnumerable<EventType> GetEventTypes()
        {
            return _session.Set<EventType>();
        }

        /// <summary>
        /// Get all people
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Person> GetPeople()
        {
            return _session.Set<Person>();
        }

        /// <summary>
        /// Get people for specified region
        /// </summary>
        /// <param name="regionId"></param>
        /// <returns></returns>
        public IEnumerable<Person> GetPeopleByRegion(Guid regionId)
        {
            return _session.Set<Person>().Where(x => x.RegionID == regionId);
        }

        /// <summary>
        /// Get all regions
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Region> GetRegions()
        {
            // EVAL
            return _session.Set<Region>();
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if (_session != null)
                    {
                        _session.Dispose();
                        _session = null;
                    }
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~DataRepository() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
