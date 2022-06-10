using System.Globalization;


namespace RepetitivePackage
{
    public static class Repetitive
    {
        public sealed class Dates
        {
            public string StartDate { get; set; }
            public string EndDate { get; set; }
        }

        private static string[] SortDaysPattern(string[] daysInPattern)
        {
            string[] sortedDays = new string[daysInPattern.Length];
            int index = 0;
            for (int day = 0; day < 7; day++)
            {
                var weekDay = Enum.GetName(typeof(DayOfWeek), day);
                if (daysInPattern.Contains(weekDay))
                {
                    sortedDays[index] = weekDay;
                    index++;
                }
            }
            return sortedDays;
        }
        public static List<Dates> Create(string strDateTime, string strEndDateTime, string[] daysInPattern, int interval = 1)
        {
            try
            {
                daysInPattern = SortDaysPattern(daysInPattern);
                List<Dates> events = new List<Dates>();
                DateTime startDateTime, endDateTime;
                startDateTime = DateTime.Parse(strDateTime.Split("T")[0]);
                endDateTime = DateTime.Parse(strEndDateTime.Split("T")[0]);
                string startTime = strDateTime.Split("T")[1];
                string endTime = strEndDateTime.Split("T")[1];
                var recurringDates = GetRecurringDates(startDateTime, endDateTime, interval, daysInPattern);
                recurringDates.ForEach(date =>
                {
                    events.Add(new Dates() { StartDate = date + "T" + startTime, EndDate = date + "T" + endTime });
                });
                return events;
            }
            catch (Exception ex)
            {
                throw;
            }
       
        }
        private static List<string> GetRecurringDates(DateTime startDate, DateTime endDate, int interval, string[] daysInPattern)
        {
            List<string> seriesDates = new List<string>();
            int weekDays = 7;
            int weekNo = WeekOfYearISO8601(startDate);
            if (IsDayExistsInPattern(startDate, daysInPattern))
                seriesDates.Add(startDate.ToString("yyyy-MM-dd"));
            while (startDate <= endDate)
            {
                startDate = startDate.AddDays(1);
                int currentWeek = WeekOfYearISO8601(startDate);
                if (weekNo != currentWeek)
                {
                    if (interval > 1)
                    {
                        int daysToSkip = (weekDays * (interval - 1));
                        startDate = startDate.AddDays(daysToSkip);
                        weekNo = WeekOfYearISO8601(startDate);
                    }
                }
                if (startDate <= endDate && IsDayExistsInPattern(startDate, daysInPattern))
                    seriesDates.Add(startDate.ToString("yyyy-MM-dd"));
            }
            return seriesDates;
        }
        private static bool IsDayExistsInPattern(DateTime date, string[] daysInPattern)
        {
            var dayOfTheWeek = date.DayOfWeek.ToString();
            bool isExistsInPattern = Array.IndexOf(daysInPattern, dayOfTheWeek) >= 0;
            return isExistsInPattern;
        }
        private static int WeekOfYearISO8601(DateTime date)
        {
            var day = (int)CultureInfo.CurrentCulture.Calendar.GetDayOfWeek(date);
            return CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(date.AddDays(4 - (day == 0 ? 7 : day)), CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
        }
    }
}

