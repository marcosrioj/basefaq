using System.ComponentModel;

namespace BaseFaq.Models.Enums.Helpers;

public static class EnumHelper
{
    public static string GetDescription(this Enum genericEnum)
    {
        var genericEnumType = genericEnum.GetType();

        var memberInfo = genericEnumType.GetMember(genericEnum.ToString());

        if (memberInfo.Length <= 0)
        {
            return genericEnum.ToString();
        }

        var attribs = memberInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);

        return attribs.Any() ? ((DescriptionAttribute)attribs.ElementAt(0)).Description : genericEnum.ToString();
    }
}