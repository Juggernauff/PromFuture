using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataApiService;
using DataApiService.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using WebServer.Models;

namespace WebServer.Controllers
{
    public class EventsController:BaseController
    {
        private readonly ILogger<EventsController> _logger;
        private IDataManager _dataManager;

        public EventsController(ILogger<EventsController> logger, IDataManager dataManager)
        {
            _logger = logger;
            _dataManager = dataManager;
            _dataManager.Auth("user2", "password2");
        }

        /// <summary>
        /// Список всех событий по умолчанию
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Index()
        {
            //Заполняем дроп лист выбора автоматов
            await SetMachinesDropList();

            //Формируем запрос событий на сегодня
            var model =await _dataManager.GetItems<EventResults>("events",new Dictionary<string, string>()
            {
                {"Since",$"{DateTime.Now:yyyy-MM-dd}" }
            });
            return View(model);
        }

        /// <summary>
        /// Список событий по фильтру
        /// </summary>
        /// <param name="pars">Набор параметров</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Index(EventsActionParameters pars)
        {
            //Проверка параметров запроса
            if (!ModelState.IsValid)
            {
                //Можно формировать сообщение или отправить на страницу ошибок, пока так
                //TODO Не в продакшен
                return BadRequest();
            }

            //Заполняем дроп лист выбора автоматов
            await SetMachinesDropList();

            //Формируем запрос событий
            //Конвертация даты из формата ДД.ММ.ГГГГ в ГГГГ-ММ-ДД происходит при маппинге параметров в классе EventsActionParameters
            var model =await _dataManager.GetItems<EventResults>("events",pars.ToDictionary());

            //Сохраняем выбранный в фильтре аппарат
            ViewData["Select_Machine_Id"] = pars.Machine_id ?? "";
            

            return View(model);
        }

        /// <summary>
        /// Заполнение списка доступных автоматов (машин)
        /// </summary>
        /// <remarks>
        /// Заполняет список в ViewData["Machines"]
        /// </remarks>
        /// <returns></returns>
        private async Task SetMachinesDropList()
        {
            //Запрос машин
            var machines =await _dataManager.GetItems<Machine>("machines");

            //Мапинг в дроп лист
            var resultList = machines.Select(x => new SelectListItem( $"{x.Machine_name} | {x.Machine_address} | {x.Machine_model}", x.Machine_id.ToString())).ToList();
            
            //Значение для "Все"
            resultList.Insert(0,new SelectListItem(" Все ",""));
            
            ViewData["Machines"] = resultList;
        } 
    }
}
