using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthInstitution.Class
{
    public class TimeRange
    {
        public DateOnly Start { get; set; }
        public TimeOnly TimeStart { get; set; }
        public DateOnly End { get; set; }
        public TimeOnly TimeEnd { get; set; }

        public TimeRange(DateOnly start, DateOnly end, TimeOnly timeStart, TimeOnly timeEnd)
        {
            if (start > end || timeStart >= timeEnd) throw new ArgumentException("Constructor of TimeRange - start is after end attribute.");
            Start = start;
            End = end;
            TimeStart = timeStart;
            TimeEnd = timeEnd;
        }

        public void Set24Hours()
        {
            TimeStart = new TimeOnly(0, 0);
            TimeEnd = new TimeOnly(23, 59);
        }

        public TimeRange GetRangeAfterDays(int i)
        {
            return new TimeRange(End.AddDays(i), End.AddDays(i), TimeStart, TimeEnd);
        }

        public Timeslot? GenerateNextOption(DateTime start)
        {
            DateTime end;
            DateOnly startDate = DateOnly.FromDateTime(start);
            while (startDate.CompareTo(End) <= 0)
            {
                end = start.Add(Timeslot.ExaminationDuration.TimeSpan);
                if (end <= startDate.ToDateTime(TimeEnd))
                    return Timeslot.ForExamination(start);
                
                startDate = startDate.AddDays(1);
                start = startDate.ToDateTime(TimeStart);
            }
            return null;
        }

    }
}
