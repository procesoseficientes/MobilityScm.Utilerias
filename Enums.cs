using System;
using System.Linq;
using System.Reflection;
using MobilityScm.Modelo.Tipos;

namespace MobilityScm.Utilerias
{
    public static class EnumsOperations
    {
        public static T GetEnumValueFromStringValue<T>(string description)
        {
            var type = typeof(T);
            if (!type.IsEnum)
                throw new ArgumentException(type.IsEnum.ToString());
            var fields = type.GetFields();
            var field = fields
                            .SelectMany(f => f.GetCustomAttributes(
                                typeof(StringValueAttribute), false), (
                                    f, a) => new { Field = f, Att = a }).SingleOrDefault(a => ((StringValueAttribute)a.Att)
                                .Value == description);
            return field == null ? default(T) : (T)field.Field.GetRawConstantValue();
        }
    }
}
