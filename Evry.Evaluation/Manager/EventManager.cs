using Evry.Evaluation.Models;
using Evry.Evaluation.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Evry.Evaluation.Manager
{
    public class EventManager
    {
        public List<EventViewModel> GetEventList()
        {
            var result = new List<EventViewModel>();
            using (var repo = new DataRepository())
            {
                // EVAL: Get data from repository and fill view models
                var regions = repo.GetRegions();
                var persons = repo.GetPeople();
                var eventtypes = repo.GetEventTypes();

                foreach (Event data in repo.GetEvents())
                {
                    var evm = new EventViewModel
                    {
                        ID = data.ID,
                        TypeID = data.TypeID,
                        PersonID = data.PersonID,
                        Time = data.Time,
                        Amount = data.Amount
                    };

                    var personIDs = persons.Where(x => x.ID == data.PersonID);
                    if (personIDs.Count() == 1)
                    {
                        evm.PersonName = $" {personIDs.First().Firstname} {personIDs.First().Lastname} ";

                        var regionIDs = regions.Where(x => x.ID == personIDs.First().RegionID);
                        if (regionIDs.Count() == 1)
                        {
                            evm.Region = regionIDs.First().Name;
                        }
                    }
                    
                    var eventtypeIDs = eventtypes.Where(x => x.ID == data.TypeID);
                    if (eventtypeIDs.Count() == 1)
                    {
                        evm.TypeName = eventtypeIDs.First().Name;
                    }
                    
                    // Sum ??
                    evm.Sum = 1.0;

                    result.Add(evm);
                }
            }
            return result;
        }
    }
}