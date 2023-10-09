using DataApiService;
using DataApiService.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using WebServer.Models;

namespace WebServer.Controllers
{
    public class SendingCommandToTerminalController : BaseController
    {
        private readonly ILogger<SendingCommandToTerminalController> _logger;
        private IDataManager _dataManager;

        public SendingCommandToTerminalController(ILogger<SendingCommandToTerminalController> logger, IDataManager dataManager)
        {
            _logger = logger;
            _dataManager = dataManager;
            _dataManager.Auth("user2", "password2");
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            await SetCommandDropList();

            string jsonSentCommands = Request.Cookies["SentCommands"] ?? "";

            List<SentCommand> sentCommands = GetSentCommandsToCookie();

            return View(sentCommands);
        }

        [HttpPost]
        public async Task<IActionResult> Index(SentCommand sentCommand, string terminalIds)
        {
            try
            {
                await SetCommandDropList();

                if (string.IsNullOrWhiteSpace(terminalIds)) return View();

                string[] ids = terminalIds.Split(new char[] { ' ', ',' });

                var result = ids.Length switch
                {
                    0 => null,
                    1 => SendCommandToTerminal(sentCommand, ids[0]),
                    _ => SendCommandToTerminals(sentCommand, ids)
                };

                List<SentCommand> sentCommands = new List<SentCommand>();
                if (result.Result != null)
                {
                    result.Result.CommandName = sentCommand.CommandName;
                    result.Result.TimeCreated = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    sentCommands = AddSentCommandAndSaveToCookie(result.Result);
                }

                return View(sentCommands.OrderByDescending(c => c.TimeCreated).ToList());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                ViewData["ErrorMessage"] = ex.Message;
                return View(GetSentCommandsToCookie());
            }
        }

        private async Task SetCommandDropList()
        {
            var responseObject = await _dataManager.GetItem<ResponseObject<Command>>("commands/types");
            var commands = responseObject.Items;
            var result = commands
                            .Select(c => new SelectListItem($"{c.Name}", $"{c}"))
                            .ToList();
            ViewData["Commands"] = result;
        }

        #region Cookie (Add/Get/Save)
        private List<SentCommand> GetSentCommandsToCookie()
        {
            List<SentCommand> sentCommands = new List<SentCommand>();
            string jsonSentCommands = Request.Cookies["SentCommands"] ?? "";

            sentCommands = jsonSentCommands switch
            {
                "" => new List<SentCommand>(),
                _ => JsonSerializer.Deserialize<List<SentCommand>>(jsonSentCommands)
            };

            return sentCommands.OrderByDescending(c => c.TimeCreated).ToList();
        }

        private List<SentCommand> AddSentCommandAndSaveToCookie(SentCommand sentCommand)
        {
            List<SentCommand> sentCommands = GetSentCommandsToCookie();
            sentCommands.Add(sentCommand);
            Response.Cookies.Append("SentCommands", JsonSerializer.Serialize(sentCommands));

            return sentCommands;
        }
        #endregion

        #region SendToTerminal(s)
        private async Task<SentCommand> SendCommandToTerminal(SentCommand sentCommand, string terminalId)
        {
            try
            {
                var sentCommandInfo = await _dataManager.PostItem<ResponseObject<SentCommand>>(
                $"terminals/{terminalId}/commands",
                new Dictionary<string, string>()
                {
                    { "terminal_id", terminalId },
                    { "command_id", sentCommand.CommandId.ToString() },
                    { "parameter1", sentCommand.CommandParameterValue1.ToString() },
                    { "parameter2", sentCommand.CommandParameterValue2.ToString() },
                    { "parameter3", sentCommand.CommandParameterValue3.ToString() },
                });
                return sentCommandInfo.Item;
            }
            catch
            {
                throw;
            }
        }

        private async Task<SentCommand> SendCommandToTerminals(SentCommand sentCommand, string[] terminalIds)
        {
            try
            {
                var parameters = new
                {
                    ids = terminalIds.Select(id => int.Parse(id)).ToList(),
                    command_id = sentCommand.CommandId,
                    parameter1 = sentCommand.CommandParameterValue1,
                    parameter2 = sentCommand.CommandParameterValue2,
                    parameter3 = sentCommand.CommandParameterValue3
                };

                string jsonParameters = JsonSerializer.Serialize(parameters);

                var sentCommandInfo = await _dataManager.PostItem<ResponseObject<SentCommand>>(
                $"terminals/commands",
                jsonParameters);
                return sentCommandInfo.Item;
            }
            catch
            {
                throw;
            }
        }
        #endregion
    }
}