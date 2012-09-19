using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace GrzegorzKozub.VisualStudioExtensions.TotalCommanderLauncher
{
    internal static class EnumExtensions
    {
        internal static TAttribute GetFirstAttribute<TEnum, TAttribute>(this TEnum enumValue) where TEnum : struct
        {
            var enumType = enumValue.GetType();

            if (!enumType.IsEnum)
                throw new ArgumentException();

            var field = enumType.GetField(enumValue.ToString());
            var attributes = field.GetCustomAttributes(typeof(TAttribute), false) as TAttribute[];

            return attributes != null ? attributes.FirstOrDefault() : default(TAttribute);
        }

        internal static string GetDescription<TEnum>(this TEnum enumValue) where TEnum : struct
        {
            var attribute = enumValue.GetFirstAttribute<TEnum, DescriptionAttribute>();
            return attribute != null ? attribute.Description : enumValue.ToString();
        }
    }
}
