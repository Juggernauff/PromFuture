using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataApiService.Utils
{
    /// <summary>
    /// Простейшие расширения
    /// </summary>
    public static class Extends
    {
        /// <summary>
        /// Формирует строку запроса для HTTP Get из словаря параметров с предшествующим &
        /// </summary>
        /// <remarks>
        ///     Словарь {
        ///         {"page",1},
        ///         {"data","some_date"}
        ///     }
        ///     Результат:
        ///          &page=1&data=some_date
        ///     Если словарь пуст, то просто пустая строка
        /// </remarks>
        /// <param name="pars">Параметры</param>
        /// <returns>Строка для запроса</returns>
        public static string ToGetParameters(this Dictionary<string, string> pars)
        {
            if (pars == null || pars.Count==0)
            {
                return "";
            }
            var paramString = "&" + string.Join("&", pars.Select(x => $"{x.Key}={x.Value}"));
            return paramString;
        }

    }
}
