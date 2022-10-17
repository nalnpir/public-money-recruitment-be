namespace VacationRental.Services.Utilities;

internal class DateTimeUtility
{
    public static bool AreDatesOverlapping(DateTime desiredDate, int desiredDays, DateTime targetDate, int targetDays)
    {
        return (targetDate <= desiredDate.Date && targetDate.AddDays(targetDays) > desiredDate.Date)
                               || (targetDate < desiredDate.AddDays(desiredDays) && targetDate.AddDays(targetDays) >= desiredDate.AddDays(desiredDays))
                               || (targetDate > desiredDate && targetDate.AddDays(targetDays) < desiredDate.AddDays(desiredDays));
    }

    /* TODO
     This function check overlapping dates, ask for more details of how it should work.
     It was not implemented because it could break consumer app
    */
    //public static bool AreDatesOverlapping(DateTime desiredDate, int desiredDays, DateTime targetDate, int targetDays)
    //{
    //    var initialDay = desiredDate;
    //    var finalDay = desiredDate.AddDays(desiredDays);
    //    return (IsBetweenTwoDates(initialDay, targetDate, targetDate.AddDays(targetDays))
    //            || IsBetweenTwoDates(finalDay, targetDate, targetDate.AddDays(targetDays)));
    //}

    public static bool IsBetweenTwoDates(DateTime targetDate, DateTime startDate, DateTime endDate)
    {
        return targetDate >= startDate && targetDate < endDate;
    }
}