using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;

namespace GrzegorzKozub.VisualStudioExtensions.TotalCommanderLauncher
{
    public class EnumConverter<TEnum> : EnumConverter where TEnum : struct
    {
        public EnumConverter()
            : base(typeof(TEnum))
        { }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string))
                return true;

            return base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            var val = value as string;

            if (val != null)
            {
                foreach (TEnum item in Enum.GetValues(typeof(TEnum)))
                {
                    if (item.GetDescription() == val)
                        return item;
                }                
            }

            return base.ConvertFrom(context, culture, value);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(string))
                return true;
            
            return base.CanConvertTo(context, destinationType);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (value is TEnum && destinationType == typeof(string))
                return ((TEnum)value).GetDescription();

            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}
