using School.Domain.Enums;

namespace School.Web.Helpers;

/// <summary>
/// Single source of English/Bangla labels for category enums, so the admin
/// forms and the public filters always offer the same values.
/// </summary>
public static class CategoryHelper
{
    private static readonly Dictionary<NoticeCategory, (string En, string Bn)> NoticeLabels = new()
    {
        [NoticeCategory.Academic] = ("Academic", "একাডেমিক"),
        [NoticeCategory.Admission] = ("Admission", "ভর্তি"),
        [NoticeCategory.Government] = ("Government", "সরকারি"),
        [NoticeCategory.Holiday] = ("Holiday", "ছুটি"),
        [NoticeCategory.Tender] = ("Tender", "টেন্ডার"),
        [NoticeCategory.General] = ("General", "সাধারণ"),
        [NoticeCategory.Emergency] = ("Emergency", "জরুরি")
    };

    private static readonly Dictionary<AchievementCategory, (string En, string Bn)> AchievementLabels = new()
    {
        [AchievementCategory.Student] = ("Student", "শিক্ষার্থী"),
        [AchievementCategory.Teacher] = ("Teacher", "শিক্ষক"),
        [AchievementCategory.Institution] = ("Institution", "প্রতিষ্ঠান"),
        [AchievementCategory.Sports] = ("Sports", "খেলাধুলা"),
        [AchievementCategory.NationalAward] = ("National Award", "জাতীয় পুরস্কার")
    };

    private static readonly Dictionary<NewsCategory, (string En, string Bn)> NewsLabels = new()
    {
        [NewsCategory.Academic] = ("Academic", "একাডেমিক"),
        [NewsCategory.Achievement] = ("Achievement", "অর্জন"),
        [NewsCategory.Sports] = ("Sports", "খেলাধুলা"),
        [NewsCategory.Cultural] = ("Cultural", "সাংস্কৃতিক"),
        [NewsCategory.Event] = ("Event", "অনুষ্ঠান"),
        [NewsCategory.General] = ("General", "সাধারণ"),
        [NewsCategory.Notice] = ("Notice", "নোটিশ"),
        [NewsCategory.Admission] = ("Admission", "ভর্তি")
    };

    private static readonly Dictionary<Designation, (string En, string Bn)> DesignationLabels = new()
    {
        [Designation.HeadTeacher] = ("Head Teacher", "প্রধান শিক্ষক"),
        [Designation.AssistantHeadTeacher] = ("Assistant Head Teacher", "সহকারী প্রধান শিক্ষক"),
        [Designation.SeniorTeacher] = ("Senior Teacher", "সিনিয়র শিক্ষক"),
        [Designation.JuniorTeacher] = ("Assistant Teacher", "সহকারী শিক্ষক"),
        [Designation.Staff] = ("Staff", "কর্মচারী")
    };

    public static string Label(NoticeCategory category) => Localize(NoticeLabels, category);
    public static string Label(Designation designation) => Localize(DesignationLabels, designation);
    public static string Label(AchievementCategory category) => Localize(AchievementLabels, category);
    public static string Label(NewsCategory category) => Localize(NewsLabels, category);

    /// <summary>Label for a category stored as its enum name (as the DTOs expose it).</summary>
    public static string Label<TEnum>(string? name) where TEnum : struct, Enum
    {
        if (!Enum.TryParse<TEnum>(name, out var value))
            return name ?? string.Empty;

        return value switch
        {
            NoticeCategory n => Label(n),
            AchievementCategory a => Label(a),
            NewsCategory c => Label(c),
            Designation d => Label(d),
            _ => value.ToString()
        };
    }

    /// <summary>English label regardless of the current culture (admin UI is English-only).</summary>
    public static string EnglishLabel<TEnum>(TEnum value) where TEnum : struct, Enum => value switch
    {
        NoticeCategory n => NoticeLabels[n].En,
        AchievementCategory a => AchievementLabels[a].En,
        NewsCategory c => NewsLabels[c].En,
        Designation d => DesignationLabels[d].En,
        _ => value.ToString()
    };

    private static string Localize<TEnum>(Dictionary<TEnum, (string En, string Bn)> labels, TEnum value) where TEnum : notnull
        => labels.TryGetValue(value, out var label)
            ? LocalizationHelper.GetLocalizedContent(label.En, label.Bn)
            : value.ToString() ?? string.Empty;
}
