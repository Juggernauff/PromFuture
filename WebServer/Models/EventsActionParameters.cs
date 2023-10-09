using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;

namespace WebServer.Models
{
    /// <summary>
    /// Набор параметров для вывода событий
    /// </summary>
    public class EventsActionParameters
    {
        //Для проверки даты
        Regex _regex=new Regex(@"^(\d{2})\.(\d{2})\.(\d{4})$");

        /// <summary>
        /// Фильтр записей
        /// </summary>
        public string Str_filter { get; set; }

        /// <summary>
        /// Поле сортировки
        /// </summary>
        public int Order_num { get; set; }

        /// <summary>
        /// Порядок сортировки
        /// </summary>
        public string Order_direction { get; set; }

        /// <summary>
        /// Идентификатор аппарата ??
        /// </summary>
        public string Machine_id { get; set; }

        /// <summary>
        /// Хранение даты в формате ГГГГ-ММ-ДД
        /// </summary>
        private string _since = "";
        
        /// <summary>
        /// Дата начала интервала события
        /// </summary>
        public string Since
        {
            get=>_since;
            set
            {
                if (value == null)
                {
                    _since = "";
                    return;
                }
                //Дата приходит (должна) в человеческом формате
                //ДД.ММ.ГГГГ
                var mt = _regex.Match(value);
                if (!mt.Success)
                {
                    _since = "";
                    return;
                }
                //Можно конечно сделать через try catch и Convert или DateTime.TryParce
                //Мне кажется так проще
                _since = $"{mt.Groups[3]}-{mt.Groups[2]}-{mt.Groups[1]}";
            }
        }

        /// <summary>
        /// Хранение даты в формате ГГГГ-ММ-ДД
        /// </summary>
        private string _until = "";
        
        /// <summary>
        /// Дата окончания интервала события
        /// </summary>
        public string Until
        {
            get=>_until;
            set
            {
                //Дата приходит (должна) в человеческом формате
                //ДД.ММ.ГГГГ
                if (value == null)
                {
                    _until = "";
                    return;
                }
                var mt = _regex.Match(value);
                if (!mt.Success)
                {
                    _until = "";
                    return;
                }
                //Можно конечно сделать через try catch и Convert или DateTime.TryParce
                //Мне кажется так проще
                _until = $"{mt.Groups[3]}-{mt.Groups[2]}-{mt.Groups[1]}";
            }
        }

        
        
    }

    public static class EventsActionParametersExtention
    {
        /// <summary>
        /// Конвертер данныхиз параметров в словарь
        /// </summary>
        /// <remarks>
        /// Условно рабочий, при добавлении сложных свойств
        /// метод ToString для значения может вернуть некорректное значение
        /// </remarks>
        /// <param name="pars">Параметры</param>
        /// <returns>Словарь</returns>
        public static Dictionary<string, string> ToDictionary(this EventsActionParameters pars)
        {
            var result = new Dictionary<string, string>();
            //Перебираем свойства
            foreach (var propertyInfo in pars.GetType().GetProperties())
            {
                //Значение текущего свойства
                var value = propertyInfo.GetValue(pars);
                //Если значение есть, т.е. не null, не пустая строка, тогда берем
                if (value != null && !string.IsNullOrEmpty(value.ToString()))
                {
                    result.Add(propertyInfo.Name, propertyInfo.GetValue(pars).ToString());
                }
            }

            return result;
        }
    }
}
