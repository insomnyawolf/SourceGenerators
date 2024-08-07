namespace TemplateEnumNamespaceTemplate;

public static partial class EnumHelperTemplateEnumNameTemplate
{
    public static string GetFullName(this TemplateEnumNameTemplate value)
    {
        var res = value switch
        {
TemplateFullNameContentTemplate
            _ => throw new System.InvalidCastException($"The enum 'TemplateEnumNameTemplate' does not contain the variant => {value}.")
        }; ;

        return res;
    }
}