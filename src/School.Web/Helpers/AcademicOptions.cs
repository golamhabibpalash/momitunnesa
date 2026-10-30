namespace School.Web.Helpers;

/// <summary>Shared option lists for the academic admin forms and public pages.</summary>
public static class AcademicOptions
{
    // Bangladeshi schools run Saturday–Thursday; Friday is the weekly holiday.
    public static readonly string[] Days = { "Saturday", "Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday" };

    public static readonly Dictionary<string, string> DayNamesBn = new()
    {
        ["Saturday"] = "শনিবার",
        ["Sunday"] = "রবিবার",
        ["Monday"] = "সোমবার",
        ["Tuesday"] = "মঙ্গলবার",
        ["Wednesday"] = "বুধবার",
        ["Thursday"] = "বৃহস্পতিবার",
        ["Friday"] = "শুক্রবার"
    };

    public static readonly string[] ClassSuggestions = { "Class 6", "Class 7", "Class 8", "Class 9", "Class 10" };
}
