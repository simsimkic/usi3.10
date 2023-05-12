using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace HealthInstitution.Class
{
    public class Timeslot
    {

        public static Duration ExaminationDuration = new Duration(new(0, 15, 0));

        public DateTime Start { get; set; }
        public Duration DurationOf { get; set; }

        public Timeslot(DateTime start, Duration duration) {
            this.Start = start;
            this.DurationOf = duration;
        }


        public static Timeslot ForExamination(DateTime start)
        {
            return new Timeslot(start, ExaminationDuration);
        }

        public static Timeslot ForOperation(DateTime start, int hours, int minutes)
        {
            return new Timeslot(start, new Duration(new(hours, minutes, 0)));
        }

        public bool OverlapsWith(Timeslot other)
        {
            DateTime end = Start.Add(DurationOf.TimeSpan);
            DateTime otherEnd = other.Start.Add(other.DurationOf.TimeSpan);
            
            return !((otherEnd <= Start) || (end <= other.Start));
        }

        public bool IsValidTimeForAdmission()
        {
            if (DateTime.Now.AddMinutes(15) >= Start)
            {
                return true;
            }
            return false;
        }

    }
}
