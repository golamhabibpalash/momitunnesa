namespace School.Domain.Interfaces;

public interface ITranslationService
{
    string GetLocalizedString(string en, string bn);
}
