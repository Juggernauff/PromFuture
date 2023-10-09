using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using DataApiService.Utils;

namespace DataApiService
{
    /// <summary>
    /// Интерфейс менеджера работы с WebApi
    /// </summary>
    public interface IDataManager
    {
        /// <summary>
        /// Запрос списка элементов
        /// </summary>
        /// <typeparam name="T">Возвращаемый тип</typeparam>
        /// <param name="pointName">Название точки доступа</param>
        /// <param name="getParams">Набор параметров Get запроса</param>
        /// <returns>Набор значений событий API</returns>
        Task<IEnumerable<T>> GetItems<T>(string pointName, Dictionary<string, string> getParams = null);

        /// <summary>
        /// Запрос элемента
        /// </summary>
        /// <typeparam name="T">Возвращаемый тип</typeparam>
        /// <param name="pointName">Название точки доступа</param>
        /// <param name="getParams">Набор параметров Get запроса</param>
        /// <returns>Элемент</returns>
        Task<T> GetItem<T>(string pointName, Dictionary<string, string> getParams = null);

        /// <summary>
        /// Отправка элементов
        /// </summary>
        /// <typeparam name="T">Возвращаемый тип</typeparam>
        /// <param name="pointName">Название точки доступа</param>
        /// <param name="getParams">Набор параметров Get запроса</param>
        /// <returns>Измененные элементы</returns>
        Task<IEnumerable<T>> PostItems<T>(string pointName, Dictionary<string, string> postParams = null);

        /// <summary>
        ///  элемента
        /// </summary>
        /// <typeparam name="T">Возвращаемый тип</typeparam>
        /// <param name="pointName">Название точки доступа</param>
        /// <param name="getParams">Набор параметров Get запроса</param>
        /// <returns>Элемент</returns>
        Task<T> PostItem<T>(string pointName, Dictionary<string, string> postParams = null);

        Task<T> PostItem<T>(string pointName, string json);

        /// <summary>
        /// Аутентификация на сервисе
        /// </summary>
        /// <param name="login">Логин</param>
        /// <param name="password">Пароль</param>
        /// <returns>Устанавливает токен идентификации, или бросает ошибку в случае отказа сервера</returns>
        void Auth(string login, string password);
    }

    /// <summary>
    /// Настройки подключения
    /// </summary>
    public class BaseApiOptions
    {
        /// <summary>
        /// Токент сервиса
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// Адрес веб-сервера (полный URL)
        /// </summary>
        /// <remarks>
        /// Свойство обределяется при старте сервера, значение берется из файла настроек
        /// appsettings.json
        /// ключ ApiWebAddress
        /// по умолчанию равен http://178.57.218.210:398, если в конфигурации нет настройки используется значение по умолчанию
        /// </remarks>
        public string BaseUrl { get; set; }

        /// <summary>
        /// Адрес точки с токеном
        /// </summary>
        /// <param name="controllerName">Контроллер точки</param>
        /// <returns>Строка URL</returns>
        public string GetUrlApiService(string controllerName)
        {
            if (string.IsNullOrEmpty(Token))
            {
                throw new ArgumentNullException("Token не получен");
            }

            return $"{BaseUrl}/{controllerName}?token={Token}";
        }
    }


    /// <summary>
    /// Объект ответа на запрос токена
    /// </summary>
    public class TokenResponse
    {
        /// <summary>
        /// Строка токена
        /// </summary>
        [JsonPropertyName("token")]
        public string Token { get; set; }

        /// <summary>
        /// Идентификатор ??
        /// </summary>
        [JsonPropertyName("owner_id")]
        public int Owner_id { get; set; }

        /// <summary>
        /// Строка для случая ошибочной аутентификации
        /// </summary>
        [JsonPropertyName("result")]
        public string Result { get; set; }
    }


    /// <summary>
    /// Менеджер работы с WebApi
    /// </summary>
    public class DataManager : IDataManager
    {
        private BaseApiOptions _options;
        private WebClient _client;

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="options">Настройки подключения</param>
        public DataManager(BaseApiOptions options)
        {
            _options = options;

            //Класс веб клиента
            _client = new WebClient();
            _client.Headers.Add(HttpRequestHeader.Accept, "application/json");
        }

        /// <summary>
        /// Аутентификация на сервисе
        /// </summary>
        /// <param name="login">Логин</param>
        /// <param name="password">Пароль</param>
        /// <returns>Устанавливает токен идентификации, или бросает ошибку в случае отказа сервера</returns>
        public void Auth(string login, string password)
        {
            _options.Token = GetServiceToken(_options.BaseUrl, login, password).Result;
        }

        /// <summary>
        /// Аутентификация на сервисе
        /// TODO по идее под аутентификацию нужен свой класс или сервис
        /// </summary>
        /// <param name="login">Имя пользователя</param>
        /// <param name="password">Пароль</param>
        /// <returns>Токен доступа к сервису</returns>
        private async Task<string> GetServiceToken(string url, string login, string password)
        {
            var authUrl = $"{url}/token";
            var responce = await GetRequestAsync<TokenResponse>(authUrl, new Dictionary<string, string>()
            {
                {"login",login },
                {"password",password }
            });
            //Если result есть в ответе, значит ошибка аутентификации
            if (!string.IsNullOrEmpty(responce.Result))
            {
                throw new Exception(responce.Result);
            }
            //Полученный с сервера токен
            return responce.Token;
        }

        /// <summary>
        /// HTTP GET на сервер
        /// </summary>
        /// <typeparam name="T">Возвращаемый тип</typeparam>
        /// <param name="url">Строка URL сервиса</param>
        /// <param name="pars">Набор параметров</param>
        /// <returns>Полученный ответ с сервера десериализуется из JSON в тип T</returns>
        private async Task<T> GetRequestAsync<T>(string url, Dictionary<string, string> pars)
        {
            var paramString = pars.ToGetParameters();
            var destUrl = $"{url}?{paramString}";
            var responseData = await _client.DownloadDataTaskAsync(new Uri(destUrl));
            var result = JsonSerializer.Deserialize<T>(System.Text.Encoding.UTF8.GetString(responseData));
            return result;
        }

        /// <summary>
        /// HTTP POST на сервер
        /// </summary>
        /// <typeparam name="T">Возвращаемый тип</typeparam>
        /// <param name="url">Строка URL сервиса</param>
        /// <param name="pars">Набор параметров</param>
        /// <returns>Полученный ответ с сервера десериализуется из JSON в тип T</returns>
        private async Task<T> PostRequestAsync<T>(string url, Dictionary<string, string> data)
        {
            _client.Headers.Add(HttpRequestHeader.ContentType, "application/json-patch+json");

            string jsonBody = JsonSerializer.Serialize(data);
            byte[] bodyBytes = Encoding.UTF8.GetBytes(jsonBody);
            byte[] responseData = await _client.UploadDataTaskAsync(url, "POST", bodyBytes);
            T result = JsonSerializer.Deserialize<T>(Encoding.UTF8.GetString(responseData));
            return result;
        }

        private async Task<T> PostRequestAsync<T>(string url, string json)
        {
            _client.Headers.Add(HttpRequestHeader.ContentType, "application/json-patch+json");

            byte[] bodyBytes = Encoding.UTF8.GetBytes(json);
            byte[] responseData = await _client.UploadDataTaskAsync(url, "POST", bodyBytes);
            T result = JsonSerializer.Deserialize<T>(Encoding.UTF8.GetString(responseData));
            return result;
        }

        /// <summary>
        /// Запрос списка элементов
        /// </summary>
        /// <remarks>
        /// Ошибки пробрасываются наверх, вызывающему
        /// </remarks>
        /// <typeparam name="T">Возвращаемый тип</typeparam>
        /// <param name="pointName">Название точки доступа</param>
        /// <param name="getParams">Набор параметров Get запроса</param>
        /// <returns>Набор значений событий API</returns>
        public async Task<IEnumerable<T>> GetItems<T>(string pointName, Dictionary<string, string> getParams = null)
        {
            try
            {
                //Адрес сервиса с токеном
                string urlService = _options.GetUrlApiService(pointName);
                var paramString = getParams.ToGetParameters();

                var url = new Uri($"{urlService}{paramString}");

                var responseData = await _client.DownloadDataTaskAsync(url);
                var jsonStr = System.Text.Encoding.UTF8.GetString(responseData);
                var result = JsonSerializer.Deserialize<IEnumerable<T>>(jsonStr);
                return result;
            }
            catch (Exception)
            {

                throw;
            }
        }

        /// <summary>
        /// Запрос элементов
        /// </summary>
        /// <remarks>
        /// Ошибки пробрасываются наверх, вызывающему
        /// </remarks>
        /// <typeparam name="T">Возвращаемый тип</typeparam>
        /// <param name="pointName">Название точки доступа</param>
        /// <param name="getParams">Набор параметров Get запроса</param>
        /// <returns>Значение события API</returns>
        public async Task<T> GetItem<T>(string pointName, Dictionary<string, string> getParams = null)
        {
            try
            {
                string urlService = _options.GetUrlApiService(pointName);
                var paramString = getParams.ToGetParameters();

                var url = new Uri($"{urlService}{paramString}");

                var responseData = await _client.DownloadDataTaskAsync(url);
                var jsonStr = Encoding.UTF8.GetString(responseData);
                var result = JsonSerializer.Deserialize<T>(jsonStr);
                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Отправка элементов
        /// </summary>
        /// <remarks>
        /// Ошибки пробрасываются наверх, вызывающему
        /// </remarks>
        /// <typeparam name="T">Возвращаемый тип</typeparam>
        /// <param name="pointName">Название точки доступа</param>
        /// <param name="getParams">Набор параметров Get запроса</param>
        /// <returns>Измененные элементы</returns>
        public async Task<IEnumerable<T>> PostItems<T>(string pointName, Dictionary<string, string> postParams = null)
        {
            try
            {
                string urlService = _options.GetUrlApiService(pointName);
                var result = await PostRequestAsync<IEnumerable<T>>($"{urlService}", postParams);
                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Отправка элемента
        /// </summary>
        /// <remarks>
        /// Ошибки пробрасываются наверх, вызывающему
        /// </remarks>
        /// <typeparam name="T">Возвращаемый тип</typeparam>
        /// <param name="pointName">Название точки доступа</param>
        /// <param name="getParams">Набор параметров Get запроса</param>
        /// <returns>Измененный элемент</returns>
        public async Task<T> PostItem<T>(string pointName, Dictionary<string, string> postParams = null)
        {
            try
            {
                string urlService = _options.GetUrlApiService(pointName);
                var result = await PostRequestAsync<T>($"{urlService}", postParams);
                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<T> PostItem<T>(string pointName, string json)
        {
            try
            {
                string urlService = _options.GetUrlApiService(pointName);
                var result = await PostRequestAsync<T>($"{urlService}", json);
                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
