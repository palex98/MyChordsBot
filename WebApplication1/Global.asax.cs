using System;
using System.Collections.Generic;
using System.ComponentModel;
using Telegram.Bot.Types;
using System.Threading.Tasks;
using Telegram.Bot.Types.Enums;
using System.Web.Mvc;
using System.Web.Routing;
using System.Threading;
using Telegram.Bot.Types.ReplyMarkups;
using System.Web.Http;

namespace WebApplication1
{
    public class MvcApplication : System.Web.HttpApplication
    {

        static public Telegram.Bot.TelegramBotClient Bot;
        //string key = "470217259:AAG9PcQDPNbs5-iWa5roWG2PUZ0QyZBNn0U";
        string key = "536276992:AAGoT7xS79tuq-WN7JJ4Ei21rk-aKyUyaDU";

        protected async void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            Answer.DictionaryInitialization();

            Bot = new Telegram.Bot.TelegramBotClient(key);
            await Bot.SetWebhookAsync("https://webapplication1201101.azurewebsites.net:443/api/message/update");
        }
    }
}
