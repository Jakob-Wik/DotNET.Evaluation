using Evry.Evaluation.Interfaces.BusinessLogic;
using Evry.Evaluation.Models;
using Evry.Evaluation.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Evry.Evaluation.BusinessLogic
{
    public class Processor : IProcessor
    {
        public IEnumerable<ProcessedPersonResult> ProcessEvents(IEnumerable<Event> events, PeriodClass periodClass, IEnumerable<EventType> eventTypes = null)
        {
            // EVAL Optional: Modify to apply event type multipliers.

            var result = new List<ProcessedPersonResult>();

            var currentStart = DateTime.MinValue;
            var currentEnd = DateTime.MinValue;
            var prevStart = DateTime.MinValue;
            var prevEnd = DateTime.MinValue;
            var nextStart = DateTime.MinValue;
            var nextEnd = DateTime.MinValue;

            // EVAL: Get dates for previous and next periods
            DateTime WeekStart(DateTime date)
            {
                // Sunday = 0 ---- Saturday = 6 => Monday = 1 ---- Sunday = 7
                var weekday = (int)date.DayOfWeek != 0 ? (int)date.DayOfWeek : 7;
                return date.AddDays((weekday - 1) * -1);
            }
            DateTime MonthStart(DateTime date)
            {
                return date.AddDays((date.Day - 1) * -1);
            }
            DateTime QuarterStart(DateTime date)
            {
                var currentQuarter = ((date.Month - 1) / 3) + 1;
                return new DateTime(DateTime.Now.Year, (currentQuarter - 1) * 3 + 1, 1);
            }
            switch (periodClass)
            {
                case PeriodClass.Week:
                    // Current week
                    currentStart = WeekStart(DateTime.Now);
                    // EVAL: Get end of week
                    currentEnd = currentStart.AddDays(6);
                    // Previous week
                    prevStart = WeekStart(DateTime.Now.AddDays(-7));
                    prevEnd = prevStart.AddDays(6);
                    // Next week
                    nextStart = WeekStart(DateTime.Now.AddDays(7)); ;
                    nextEnd = nextStart.AddDays(6);
                    break;

                case PeriodClass.Month:
                    // Current Month
                    currentStart = MonthStart(DateTime.Now);
                    currentEnd = new DateTime(currentStart.Year, currentStart.Month+1, 1).AddDays(-1);
                    // Previous Month
                    prevStart = MonthStart(new DateTime(DateTime.Now.Year, DateTime.Now.Month - 1, 1));
                    prevEnd = new DateTime(prevStart.Year, prevStart.Month + 1, 1).AddDays(-1);
                    // Next Month
                    nextStart = MonthStart(new DateTime(DateTime.Now.Year, DateTime.Now.Month + 1, 1));
                    nextEnd = new DateTime(nextStart.Year, nextStart.Month + 1, 1).AddDays(-1);
                    break;

                case PeriodClass.Quarter:
                    // EVAL
                    // Current quarter
                    currentStart = QuarterStart(DateTime.Now);
                    currentEnd = currentStart.AddMonths(3).AddDays(-1);
                    // Previous quarter
                    prevStart = QuarterStart(currentEnd.AddMonths(1));
                    prevEnd = prevStart.AddMonths(3).AddDays(-1);
                    // Next quarter
                    nextStart = QuarterStart(currentStart.AddMonths(-1));
                    nextEnd = nextStart.AddMonths(3).AddDays(-1);
                    break;

                case PeriodClass.Year:
                    // Current Year
                    currentStart = new DateTime(DateTime.Now.Year, 1, 1);
                    currentEnd = new DateTime(DateTime.Now.Year + 1, 1, 1).AddDays(-1);
                    // Previous Year
                    prevStart = new DateTime(DateTime.Now.Year-1, 1, 1);
                    prevEnd = new DateTime(prevStart.Year + 1, 1, 1).AddDays(-1);
                    // Next Year
                    nextStart = new DateTime(DateTime.Now.Year - 1, 1, 1);
                    nextEnd = new DateTime(nextStart.Year + 1, 1, 1).AddDays(-1);
                    break;
            }

            var persons = events.Select(x => x.PersonID).ToList();

            persons.ForEach(person =>
            {
                var personResult = new ProcessedPersonResult
                // EVAL: Fill personal details
                {
                    ID = Guid.NewGuid(),
                    PersonID = person
                };


                var takeCurrent = new List<int>();
                var takePrevious = new List<int>();
                var takeNext = new List<int>();

                for (var i = 0; i < events.Count(); i++)
                {
                    if (events.ToArray()[i].Time >= currentStart)
                    {
                        if (events.ToArray()[i].Time <= currentEnd)
                            takeCurrent.Add(i);
                    }
                    else if (events.ToArray()[i].Time >= prevStart)
                    {
                        if (events.ToArray()[i].Time <= prevEnd)
                            takePrevious.Add(i);
                    }
                    else if (events.ToArray()[i].Time >= nextStart)
                    {
                        if (events.ToArray()[i].Time <= nextEnd)
                            takeNext.Add(i);
                    }
                }

                var currentTotal = 0d;
                for (var i = 0; i < takeCurrent.Count; i++)
                {
                    for (var k = 0; k < events.Count(); k++)
                    {
                        if (k == takeCurrent[i])
                        {
                            if (eventTypes != null)
                            {
                                if (eventTypes.Select(x => x.ID).Contains(events.ToArray()[k].TypeID))
                                {
                                    currentTotal += events.ToArray()[k].Amount;
                                }
                            }
                            else
                            {
                                currentTotal += events.ToArray()[k].Amount;
                            }
                        }
                    }
                }
                personResult.PeriodTotals.Add(PeriodType.Current, currentTotal);

                var prevTotal = 0d;
                for (var i = 0; i < takePrevious.Count; i++)
                {
                    for (var k = 0; k < events.Count(); k++)
                    {
                        if (k == takePrevious[i])
                        {
                            if (eventTypes != null)
                            {
                                if (eventTypes.Select(x => x.ID).Contains(events.ToArray()[k].TypeID))
                                {
                                    prevTotal += events.ToArray()[k].Amount;
                                }
                            }
                            else
                            {
                                prevTotal += events.ToArray()[k].Amount;
                            }
                        }
                    }
                }
                personResult.PeriodTotals.Add(PeriodType.Previous, prevTotal);

                var nextTotal = 0d;
                for (var i = 0; i < takeNext.Count; i++)
                {
                    for (var k = 0; k < events.Count(); k++)
                    {
                        if (k == takeNext[i])
                        {
                            if (eventTypes != null)
                            {
                                if (eventTypes.Select(x => x.ID).Contains(events.ToArray()[k].TypeID))
                                {
                                    nextTotal += events.ToArray()[k].Amount;
                                }
                            }
                            else
                            {
                                nextTotal += events.ToArray()[k].Amount;
                            }
                        }
                    }
                }
                personResult.PeriodTotals.Add(PeriodType.Next, nextTotal);
            });

            return result;
        }
    }
}
