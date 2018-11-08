using System.Drawing.Drawing2D;
using System.Web.Http;
using System.Web.Http.Results;
using Telegram.Bot.Types;
using System.Threading.Tasks;

namespace WebApplication1.Controllers
{
    public class MessageController : ApiController
    {
        public static bool isWork = true;
        Answer answer;

        [Route(@"api/message/update")]
        public async Task<OkResult> Update([FromBody] Update update)
        {

            answer = new Answer(update);
            await answer.Work();
           
            return Ok();
        }
    }
}
