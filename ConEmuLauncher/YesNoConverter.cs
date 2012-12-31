using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;

namespace GrzegorzKozub.VisualStudioExtensions.ConEmuLauncher
{
    public class YesNoConverter : BooleanConverter
    {
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
                if (val.Equals("Yes", StringComparison.OrdinalIgnoreCase))
                    return true;
                else if (val.Equals("No", StringComparison.OrdinalIgnoreCase))
                    return false;
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
            if (value is bool && destinationType == typeof(string))
                return (bool)value ? "Yes" : "No";

            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}
