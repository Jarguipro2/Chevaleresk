using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.Data.Entity;
using System.Data.Entity.Migrations;

namespace EFA_DEMO.Models
{
    public static class Extensions
    {
        public static string GetEnumDescription<TEnum>(this TEnum item)
            => item.GetType()
                   .GetField(item.ToString())
                   .GetCustomAttributes(typeof(DescriptionAttribute), false)
                   .Cast<DescriptionAttribute>()
                   .FirstOrDefault()?.Description ?? string.Empty;

        public static void SeedEnumValues<T, TEnum>(this IDbSet<T> dbSet, Func<TEnum, T> converter)
            where T : class => Enum.GetValues(typeof(TEnum))
                                   .Cast<object>()
                                   .Select(value => converter((TEnum)value))
                                   .ToList()
                                   .ForEach(instance => dbSet.AddOrUpdate(instance));
    }
}