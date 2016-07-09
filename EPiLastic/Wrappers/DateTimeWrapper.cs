using System;

namespace EpiLastic.Wrappers
{
    public interface IDateTimeWrapper
    {
        DateTime Now { get; }
    }

    public class DateTimeWrapper : IDateTimeWrapper
    {
        public DateTime Now
        {
            get { return DateTime.Now; }
        }
    }
}
