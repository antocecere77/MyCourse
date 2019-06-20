Func<DateTime, bool> canDrive = dob => {
    return dob.AddYears(18) <= DateTime.Today;
};

DateTime dob = new DateTime(2000, 12, 25);

bool result = canDrive(dob);
Console.WriteLine(result);

Action<DateTime> printDate = date => Console.WriteLine(date);

DateTime date = DateTime.Today;
printDate(date);