using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InlineKeyboardButtons;
using Telegram.Bot.Types.ReplyMarkups;

namespace WebApplication1
{
    public class Answer
    {
        public static Update update;
        public static Dictionary<string, string> Chords = new Dictionary<string, string>();
        public static Dictionary<string, string> UkuleleChords = new Dictionary<string, string>();
        public static bool Parsing;
        public static Message SendedMessage;


        public Answer(Update upd)
        {
            update = upd;
        }

        public async Task<string> Work()
        {
            Message message;

            if (update.CallbackQuery != null)
            {
                try
                {
                    var transponator = new Tranponator(update.CallbackQuery.Message.Text);
                    string[] callbackData = update.CallbackQuery.Data.Split('_');
                    Int32.TryParse(callbackData[1], out var messageId);
                    var editKeyboard = new InlineKeyboardMarkup(
                                        new[]
                                        {
                                                            new InlineKeyboardButton[] {
                                                                new InlineKeyboardCallbackButton("🔽", "down_" + messageId),
                                                                new InlineKeyboardCallbackButton("🔼", "up_" + messageId)
                                                            }
                                        }
                                    );
                    if (callbackData[0] == "up")
                    {
                        await MvcApplication.Bot.EditMessageTextAsync(update.CallbackQuery.Message.Chat.Id, messageId, transponator.WorkUp(), ParseMode.Markdown, false, editKeyboard);
                        await MvcApplication.Bot.SendTextMessageAsync(866103901, update.CallbackQuery.From.FirstName + " " + update.CallbackQuery.From.LastName + "\n<b>TONE+</b>", ParseMode.Html);

                    }
                    else if (callbackData[0] == "down")
                    {
                        await MvcApplication.Bot.EditMessageTextAsync(update.CallbackQuery.Message.Chat.Id, messageId, transponator.WorkDown(), ParseMode.Markdown, false, editKeyboard);
                        await MvcApplication.Bot.SendTextMessageAsync(866103901, update.CallbackQuery.From.FirstName + " " + update.CallbackQuery.From.LastName + "\n<b>TONE-</b>", ParseMode.Html);
                    }
                    await MvcApplication.Bot.AnswerCallbackQueryAsync(update.CallbackQuery.Id);
                    Thread.Sleep(1200);
                }
                catch
                {
                    Console.WriteLine("EXEPTION");
                    await MvcApplication.Bot.SendTextMessageAsync(866103901, update.CallbackQuery.From.FirstName + " " + update.CallbackQuery.From.LastName + "\n<b>TONE EXEPTION</b>", ParseMode.Html);

                    try
                    {
                        await MvcApplication.Bot.AnswerCallbackQueryAsync(update.CallbackQuery.Id, "Слишком много запросов на смену тональности!");
                        Thread.Sleep(2000);
                    }
                    catch
                    {
                        Console.WriteLine("BANG");
                    }
                }
                return "";
            }

            if (update.Message != null)
            {
                message = update.Message;
            }
            else
            {
                return "";
            }

            /*if (message.Type == MessageType.AudioMessage)
            {
                Console.WriteLine(message.Audio.Performer);
            }*/

            if (message.Type == MessageType.TextMessage)
            {

                if (await IfChord(message)) return "";

                if (await IfUkuleleChord(message)) return "";

                if (message.Text == "/start")
                {
                    await MvcApplication.Bot.SendTextMessageAsync(message.Chat.Id, "Привет, " + message.From.FirstName + '.');
                    await MvcApplication.Bot.SendTextMessageAsync(message.Chat.Id, "С помощью этого бота ты легко можешь находить аккорды. " +
                        "Просто отправь мне название песни.");
                    await MvcApplication.Bot.SendTextMessageAsync(866103901, message.From.FirstName + " " + message.From.LastName + "\n<b>START</b>\n" +
                        "UserId: " + message.Chat.Id  + " @" + message.From.Username, ParseMode.Html);

                    return "";
                }

                if (message.Text == "/help")
                {
                    await MvcApplication.Bot.SendTextMessageAsync(message.Chat.Id, "Для того, чтобы найти аккорды песни, просто отправь мне ее название.\n\n" +
                        "Если же хочешь узнать аппликатуру неизвестного аккорда, отправь название аккорда.\nЧтобы узнать аппликатуру аккорда для укулеле, отправь название аккорда" +
                        " + u. <i>Например: Am7 u</i>.\n\n" +
                        "Свои пожелания и предложения можно писать сюда - @pyzzzh", ParseMode.Html);
                    await MvcApplication.Bot.SendTextMessageAsync(866103901, message.From.FirstName + " " + message.From.LastName + "\n<b>HELP</b>\n", ParseMode.Html);

                    return "";
                }

                try
                {
                    SendedMessage = await MvcApplication.Bot.SendTextMessageAsync(message.Chat.Id, "<i>Поиск...</i> ⏳", ParseMode.Html);
                    Parsing = true;
                    Thread anim = new Thread(Animation);
                    anim.Start();
                    string searchString;
                    if (message.Text == "/random")
                    {
                        PopularParser popularParser = new PopularParser();
                        await popularParser.PopularParserInit();
                        searchString = popularParser.Get() + " аккорды amdm";
                    }
                    else searchString = message.Text + " аккорды amdm";
                    var googleParser = new GoogleParser(searchString);
                    var amDmParser = new AmDmParser(await googleParser.GetLinkToAmDm());
                    var keyboard = new InlineKeyboardMarkup(
                                        new[]
                                        {
                                                            new InlineKeyboardButton[] {
                                                                new InlineKeyboardCallbackButton("🔽", "down_" + SendedMessage.MessageId),
                                                                new InlineKeyboardCallbackButton("🔼", "up_" + SendedMessage.MessageId)
                                                            }
                                        }
                                    );
                    Parsing = false;
                    var textToSend = await amDmParser.GetData();
                    BoldChords(ref textToSend);
                    try
                    {
                        await MvcApplication.Bot.EditMessageTextAsync(SendedMessage.Chat.Id, SendedMessage.MessageId, textToSend, ParseMode.Markdown, false, keyboard);
                    }
                    catch
                    {
                        await MvcApplication.Bot.DeleteMessageAsync(SendedMessage.Chat.Id, SendedMessage.MessageId);
                        await MvcApplication.Bot.SendTextMessageAsync(message.Chat.Id, textToSend, ParseMode.Markdown);
                        await MvcApplication.Bot.SendTextMessageAsync(866103901, message.From.FirstName + " " + message.From.LastName + "\n<b>" + "429" + "</b>", ParseMode.Html);
                    }
                    await MvcApplication.Bot.SendTextMessageAsync(866103901, message.From.FirstName + " " + message.From.LastName + "\n<b>" + message.Text + "</b>", ParseMode.Html);
                }
                catch (Exception ex)
                {
                    try
                    {

                        if (ex.Message.IndexOf("429") > 0)
                        {
                            await MvcApplication.Bot.SendTextMessageAsync(message.Chat.Id, "_Бот занят! Попробуй чуть позже._", parseMode: ParseMode.Markdown);
                        }
                        else
                        {
                            await MvcApplication.Bot.DeleteMessageAsync(SendedMessage.Chat.Id, SendedMessage.MessageId);
                            await MvcApplication.Bot.SendTextMessageAsync(message.Chat.Id, "_Не удалось найти твою песню. Попробуй еще раз!_", parseMode: ParseMode.Markdown);
                        }
                        Thread.Sleep(1000);
                        await MvcApplication.Bot.SendTextMessageAsync(866103901, message.From.FirstName + " " + message.From.LastName + "\n<b>" + "ERROR" + "</b>" + "\n" + message.Text, ParseMode.Html);
                    }
                    catch
                    {

                    }
                }
                return "";
            }
            return "";
        }

        public static async void Animation()
        {
            while (Parsing)
            {
                if (Parsing) await MvcApplication.Bot.EditMessageTextAsync(SendedMessage.Chat.Id, SendedMessage.MessageId, "<i>Поиск...</i> ⌛", ParseMode.Html);
                if (Parsing) await MvcApplication.Bot.EditMessageTextAsync(SendedMessage.Chat.Id, SendedMessage.MessageId, "<i>Поиск...</i> ⏳", ParseMode.Html);
            }
        }

        public static async Task<bool> IfUkuleleChord(Message message)
        {
            foreach(var chord in UkuleleChords)
            {
                if (chord.Key == message.Text)
                {
                    await MvcApplication.Bot.SendPhotoAsync(message.Chat.Id, new FileToSend(chord.Value));
                    return true;
                }
            }
            return false;
        }

        public static async Task<bool> IfChord(Message message)
        {
            try
            {
                await MvcApplication.Bot.SendPhotoAsync(message.Chat.Id, new FileToSend("https://amdm.ru/images/chords/" + message.Text.Replace('+', 'p')
                    .Replace('-', 'z').Replace('#', 'w').Replace('/', 's').Replace("А", "A").Replace("В", "B").Replace("С", "C").Replace("Е", "E") + "_0.gif"));
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static void BoldChords(ref string text)
        {
            Console.WriteLine("BoldChords");
            foreach (var chord in Chords)
            {
                text = text.Replace(chord.Key, "*" + chord.Key + "*");
            }
        }

        public static void DictionaryInitialization()
        {
            Console.WriteLine("DictionaryInitialization START");

            Chords.Add("A#m", "");
            Chords.Add("B#m", "");
            Chords.Add("C#m", "");
            Chords.Add("D#m", "");
            Chords.Add("E#m", "");
            Chords.Add("F#m", "");
            Chords.Add("G#m", "");
            Chords.Add("Am", "");
            Chords.Add("Bm", "");
            Chords.Add("Cm", "");
            Chords.Add("Dm", "");
            Chords.Add("Em", "");
            Chords.Add("Fm", "");
            Chords.Add("Gm", "");
            Chords.Add("A ", "");
            Chords.Add("B ", "");
            Chords.Add("C ", "");
            Chords.Add("D ", "");
            Chords.Add("E ", "");
            Chords.Add("F ", "");
            Chords.Add("G ", "");
            /*chords.Add("A/D", "https://www.imbf.org/akkordy/images/aslashdchord-nn0022-1.png");
            chords.Add("A11", "https://www.imbf.org/akkordy/images/a11chord-n42433-1.png");
            chords.Add("A13", "https://www.imbf.org/akkordy/images/a13chord-n01231-5.png");
            chords.Add("A4", "https://www.imbf.org/akkordy/images/a4chord-002200-1.png");
            chords.Add("A5", "https://www.otsema.ru/accords/grafic/accord_a5_ot_5_otkr_str.gif");
            chords.Add("A6", "https://www.imbf.org/akkordy/images/a6chord-nn2222-1.png");
            chords.Add("A7sus4", "https://www.imbf.org/akkordy/images/a7sus4chord-002030-1.png");
            chords.Add("A7", "https://www.imbf.org/akkordy/images/a7chord-n02020-1.png");
            chords.Add("A9", "https://www.imbf.org/akkordy/images/a9chord-n02100-1.png");
            chords.Add("Ab+", "https://www.imbf.org/akkordy/images/abpluschord-nn2110-1.png");
            chords.Add("Ab4", "https://www.imbf.org/akkordy/images/ab4chord-nn1124-1.png");
            chords.Add("Ab7", "https://www.imbf.org/akkordy/images/ab7chord-nn1112-1.png");
            chords.Add("Abdim", "https://www.imbf.org/akkordy/images/abdimchord-nn0101-1.png");
            chords.Add("Abm7", "https://www.imbf.org/akkordy/images/abm7chord-nn1111-4.png");
            chords.Add("Abmaj7", "https://www.imbf.org/akkordy/images/abmaj7chord-nn1113-1.png");
            chords.Add("Abm", "https://www.imbf.org/akkordy/images/abmchord-133111-4.png");
            chords.Add("Ab ", "https://www.imbf.org/akkordy/images/abchord-133211-4.png");
            chords.Add("Adim", "https://www.imbf.org/akkordy/images/adimchord-nn1212-1.png");
            chords.Add("Am/G", "https://www.imbf.org/akkordy/images/amslashgchord-302210-1.png");
            chords.Add("Am6", "https://www.imbf.org/akkordy/images/am6chord-n02212-1.png");
            chords.Add("Am7", "https://www.imbf.org/akkordy/images/am7chord-n02213-1.png");
            chords.Add("Am7sus4", "https://www.imbf.org/akkordy/images/am7sus4chord-000030-1.png");
            chords.Add("Am9", "https://www.imbf.org/akkordy/images/am9chord-n01113-5.png");
            chords.Add("Amaj7", "https://www.imbf.org/akkordy/images/amaj7chord-n02120-1.png");
            chords.Add("Am", "https://www.imbf.org/akkordy/images/amchord-n02210-1.png");
            chords.Add("Asus", "https://www.imbf.org/akkordy/images/asuschord-nn2230-1.png");
            chords.Add("B+", "https://www.imbf.org/akkordy/images/bpluschord-nn1004-1.png");
            chords.Add("B/F#", "https://www.imbf.org/akkordy/images/bslashfsharpchord-022200-2.png");
            chords.Add("B11/13", "https://www.imbf.org/akkordy/images/b11slash13chord-004444-1.png");
            chords.Add("B11", "https://www.imbf.org/akkordy/images/b11chord-133200-7.png");
            chords.Add("B13", "https://www.imbf.org/akkordy/images/b13chord-n21204-1.png");
            chords.Add("B4", "https://www.imbf.org/akkordy/images/b4chord-nn3341-2.png");
            chords.Add("B7+", "https://www.imbf.org/akkordy/images/b7pluschord-n21203-1.png");
            chords.Add("B7", "https://www.imbf.org/akkordy/images/b7chord-021202-1.png");
            chords.Add("Bb+", "https://www.imbf.org/akkordy/images/bbpluschord-nn0332-1.png");
            chords.Add("Bb6", "https://www.imbf.org/akkordy/images/bb6chord-nn3333-1.png");
            chords.Add("Bbm9", "https://www.imbf.org/akkordy/images/bbm9chord-nnn113-6.png");
            chords.Add("Bm(maj7)", "https://www.imbf.org/akkordy/images/bmmaj7chord-n20332-1.png");
            chords.Add("Bm6", "https://www.imbf.org/akkordy/images/bm6chord-nn4434-1.png");
            chords.Add("Bm7", "https://www.imbf.org/akkordy/images/bm7chord-n13121-2.png");
            chords.Add("Bmaj", "https://www.imbf.org/akkordy/images/bmajchord-n2434n-1.png");
            chords.Add("Bmsus9", "https://www.imbf.org/akkordy/images/bmsus9chord-n34402-1.png");
            chords.Add("Bm", "https://www.imbf.org/akkordy/images/bmchord-n24432-1.png");
            chords.Add("B ", "https://www.imbf.org/akkordy/images/bchord-n24442-1.png");
            chords.Add("C#4", "https://www.imbf.org/akkordy/images/csharp4chord-nn3341-4.png");
            chords.Add("C#7", "https://www.imbf.org/akkordy/images/csharp7chord-nn3424-1.png");
            chords.Add("C#m7", "https://www.imbf.org/akkordy/images/csharpm7chord-nn2424-1.png");
            chords.Add("C#maj", "https://www.imbf.org/akkordy/images/csharpmajchord-n43111-1.png");
            chords.Add("C#m", "https://www.imbf.org/akkordy/images/csharpmchord-nn2120-1.png");
            chords.Add("C#", "https://www.imbf.org/akkordy/images/csharpchord-nn3121-1.png");
            chords.Add("C/B", "https://www.imbf.org/akkordy/images/cslashbchord-n22010-1.png");
            chords.Add("C11", "https://www.imbf.org/akkordy/images/c11chord-n13141-3.png");
            chords.Add("C4", "https://www.imbf.org/akkordy/images/c4chord-nn3013-1.png");
            chords.Add("C5", "https://www.otsema.ru/accords/grafic/accord_c5_ot_5str.gif");
            chords.Add("C6", "http://guitarlesson.ru/wp-content/uploads/2013/02/C6.png");
            chords.Add("C7", "https://www.imbf.org/akkordy/images/c7chord-032310-1.png");
            chords.Add("C9", "https://www.imbf.org/akkordy/images/c9chord-131213-8.png");
            chords.Add("Cmaj", "https://www.imbf.org/akkordy/images/cmajchord-032010-1.png");
            chords.Add("Cmaj7", "https://www.imbf.org/akkordy/images/cmaj7chord-n32000-1.png");
            chords.Add("Csus2", "https://www.imbf.org/akkordy/images/csus2chord-n3001n-1.png");
            chords.Add("Csus9", "https://www.imbf.org/akkordy/images/csus9chord-nn4124-7.png");
            chords.Add("Cm", "http://guitarlesson.ru/wp-content/uploads/2013/02/Cm.png");
            chords.Add("C ", "https://www.imbf.org/akkordy/images/cchord-n32010-1.png");
            chords.Add("D#4", "https://www.imbf.org/akkordy/images/dsharp4chord-nn1344-1.png");
            chords.Add("D#7", "https://www.imbf.org/akkordy/images/dsharp7chord-nn1323-1.png");
            chords.Add("D#m7", "https://www.imbf.org/akkordy/images/dsharpm7chord-nn1322-1.png");
            chords.Add("D#maj7", "https://www.imbf.org/akkordy/images/dsharpmaj7chord-nn1333-1.png");
            chords.Add("D#", "https://www.imbf.org/akkordy/images/dsharpchord-nn3121-3.png");
            chords.Add("Dadd9", "https://www.imbf.org/akkordy/images/dadd9chord-000232-1.png");
            chords.Add("D/A", "https://www.imbf.org/akkordy/images/dslashachord-n04232-1.png");
            chords.Add("D/B", "https://www.imbf.org/akkordy/images/dslashbchord-n20232-1.png");
            chords.Add("D/C", "https://www.imbf.org/akkordy/images/dslashcchord-n30232-1.png");
            chords.Add("D/C#", "https://www.imbf.org/akkordy/images/dslashcsharpchord-n40232-1.png");
            chords.Add("D/E", "https://www.imbf.org/akkordy/images/dslashechord-n1111n-7.png");
            chords.Add("D/G", "https://www.imbf.org/akkordy/images/dslashgchord-3n0232-1.png");
            chords.Add("D11", "https://www.imbf.org/akkordy/images/d11chord-300210-1.png");
            chords.Add("D4", "https://www.imbf.org/akkordy/images/d4chord-nn0233-1.png");
            chords.Add("D5/E", "https://www.imbf.org/akkordy/images/d5slashechord-0111nn-7.png");
            chords.Add("D6", "https://www.imbf.org/akkordy/images/d6chord-n00202-1.png");
            chords.Add("D7sus2", "https://www.imbf.org/akkordy/images/d7sus2chord-n00210-1.png");
            chords.Add("D7sus4", "https://www.imbf.org/akkordy/images/d7sus4chord-n00213-1.png");
            chords.Add("D7", "https://www.imbf.org/akkordy/images/d7chord-nn0212-1.png");
            chords.Add("D9", "https://www.imbf.org/akkordy/images/d9chord-011112-7.png");
            chords.Add("Dm#7", "https://www.imbf.org/akkordy/images/dmsharp7chord-nn0221-1.png");
            chords.Add("Dm/A", "https://www.imbf.org/akkordy/images/dmslashachord-n00231-1.png");
            chords.Add("Dm/B", "https://www.imbf.org/akkordy/images/dmslashbchord-n20231-1.png");
            chords.Add("Dm/C", "https://www.imbf.org/akkordy/images/dmslashcchord-n30231-1.png");
            chords.Add("Dm/C#", "https://www.imbf.org/akkordy/images/dmslashcsharpchord-n40231-1.png");
            chords.Add("Dm7", "https://www.imbf.org/akkordy/images/dm7chord-nn0211-1.png");
            chords.Add("Dm9", "https://www.imbf.org/akkordy/images/dm9chord-nn3210-1.png");
            chords.Add("Dmaj7", "https://www.imbf.org/akkordy/images/dmaj7chord-nn0222-1.png");
            chords.Add("Dsus2", "https://www.imbf.org/akkordy/images/dsus2chord-000230-1.png");
            chords.Add("Dm", "https://www.imbf.org/akkordy/images/dmchord-nn0231-1.png");
            chords.Add("D ", "https://www.imbf.org/akkordy/images/dchord-nn0232-1.png");
            chords.Add("E11", "https://www.imbf.org/akkordy/images/e11chord-111122-1.png");
            chords.Add("E5", "https://www.imbf.org/akkordy/images/e5chord-0133nn-7.png");
            chords.Add("E6", "https://www.imbf.org/akkordy/images/e6chord-nn3333-9.png");
            chords.Add("E7", "https://www.imbf.org/akkordy/images/e7chord-022130-1.png");
            chords.Add("E9", "https://www.imbf.org/akkordy/images/e9chord-131213-1.png");
            chords.Add("Em(add9)", "https://www.imbf.org/akkordy/images/emadd9chord-024000-1.png");
            chords.Add("Em(sus4)", "https://www.imbf.org/akkordy/images/emsus4chord-002000-1.png");
            chords.Add("Em/B", "https://www.imbf.org/akkordy/images/emslashbchord-n22000-1.png");
            chords.Add("Em/D", "https://www.imbf.org/akkordy/images/emslashdchord-nn0000-1.png");
            chords.Add("Em6", "https://www.imbf.org/akkordy/images/em6chord-022020-1.png");
            chords.Add("Em7", "https://www.imbf.org/akkordy/images/em7chord-022030-1.png");
            chords.Add("Emaj7", "https://www.imbf.org/akkordy/images/emaj7chord-02110n-1.png");
            chords.Add("Esus4", "https://www.imbf.org/akkordy/images/esus4chord-022200-0.png");
            chords.Add("Esus", "https://www.imbf.org/akkordy/images/esuschord-022200-1.png");
            chords.Add("Em", "https://www.imbf.org/akkordy/images/emchord-022000-1.png");
            chords.Add("E ", "https://www.imbf.org/akkordy/images/echord-022100-1.png");
            chords.Add("F#+", "https://www.imbf.org/akkordy/images/fsharppluschord-nn4332-1.png");
            chords.Add("F#/E", "https://www.imbf.org/akkordy/images/fsharpslashechord-044322-1.png");
            chords.Add("F#11", "https://www.imbf.org/akkordy/images/fsharp11chord-242422-1.png");
            chords.Add("F#4", "https://www.imbf.org/akkordy/images/fsharp4chord-nn4422-1.png");
            chords.Add("F#7", "https://www.imbf.org/akkordy/images/fsharp7chord-nn4320-1.png");
            chords.Add("F#9", "https://www.imbf.org/akkordy/images/fsharp9chord-n12122-1.png");
            chords.Add("F#m", "https://www.imbf.org/akkordy/images/fsharpmchord-244222-1.png");
            chords.Add("F#m6", "https://www.imbf.org/akkordy/images/fsharpm6chord-nn1222-1.png");
            chords.Add("F#maj", "https://www.imbf.org/akkordy/images/fsharpmajchord-244322-0.png");
            chords.Add("F#maj7", "https://www.imbf.org/akkordy/images/fsharpmaj7chord-nn4321-1.png");
            chords.Add("F(add9)", "https://www.imbf.org/akkordy/images/fadd9chord-303211-1.png");
            chords.Add("F/A", "https://www.imbf.org/akkordy/images/fslashachord-n03211-1.png");
            chords.Add("F/C", "https://www.imbf.org/akkordy/images/fslashcchord-nn3211-1.png");
            chords.Add("F/G", "https://www.imbf.org/akkordy/images/fslashgchord-333211-1.png");
            chords.Add("F11", "https://www.imbf.org/akkordy/images/f11chord-131311-1.png");
            chords.Add("F4", "https://www.imbf.org/akkordy/images/f4chord-nn3311-1.png");
            chords.Add("F5", "http://soldiez.com/assets/images/chords/akkord_F5__712.png");
            chords.Add("F6", "https://www.imbf.org/akkordy/images/f6chord-n3323n-1.png");
            chords.Add("F7", "https://www.imbf.org/akkordy/images/f7chord-131211-1.png");
            chords.Add("F7/A", "https://www.imbf.org/akkordy/images/f7slashachord-n01211-1.png");
            chords.Add("F9", "https://www.imbf.org/akkordy/images/f9chord-242324-1.png");
            chords.Add("Fm6", "https://www.imbf.org/akkordy/images/fm6chord-nn0111-1.png");
            chords.Add("Fm7", "https://www.imbf.org/akkordy/images/fm7chord-131111-1.png");
            chords.Add("Fmaj7", "https://www.imbf.org/akkordy/images/fmaj7chord-n33210-1.png");
            chords.Add("Fmaj7/A", "https://www.imbf.org/akkordy/images/fmaj7slashachord-n03210-1.png");
            chords.Add("Fmmaj7", "https://www.imbf.org/akkordy/images/fmmaj7chord-n33110-1.png");
            chords.Add("Fm", "https://www.imbf.org/akkordy/images/fmchord-133111-1.png");
            chords.Add("F#", "https://www.imbf.org/akkordy/images/fsharpchord-244322-1.png");
            chords.Add("F ", "https://www.imbf.org/akkordy/images/fchord-133211-1.png");
            chords.Add("G#m6", "https://www.imbf.org/akkordy/images/gsharpm6chord-nn1101-1.png");
            chords.Add("G/A", "https://www.imbf.org/akkordy/images/gslashachord-300003-1.png");
            chords.Add("G/B", "https://www.imbf.org/akkordy/images/gslashbchord-n20003-1.png");
            chords.Add("G/D", "https://www.imbf.org/akkordy/images/gslashdchord-n22100-4.png");
            chords.Add("G/F#", "https://www.imbf.org/akkordy/images/gslashfsharpchord-220003-1.png");
            chords.Add("G11", "https://www.imbf.org/akkordy/images/g11chord-3n0211-1.png");
            chords.Add("G4", "https://www.imbf.org/akkordy/images/g4chord-nn0013-1.png");
            chords.Add("G5", "http://soldiez.com/assets/images/chords/akkord_G5__780.png");
            chords.Add("G6(sus4)", "https://www.imbf.org/akkordy/images/g6sus4chord-020010-1.png");
            chords.Add("G6", "https://www.imbf.org/akkordy/images/g6chord-3n0000-1.png");
            chords.Add("G7#9", "https://www.imbf.org/akkordy/images/g7sharp9chord-13n244-3.png");
            chords.Add("G7(sus4)", "https://www.imbf.org/akkordy/images/g7sus4chord-330011-1.png");
            chords.Add("G7", "https://www.imbf.org/akkordy/images/g7chord-320001-1.png");
            chords.Add("G9(11)", "https://www.imbf.org/akkordy/images/g911chord-120001-1.png");
            chords.Add("G9", "https://www.imbf.org/akkordy/images/g9chord-3n0201-1.png");
            chords.Add("Gm/Bb", "https://www.imbf.org/akkordy/images/gmslashbbchord-3221nn-4.png");
            chords.Add("Gm6", "https://www.imbf.org/akkordy/images/gm6chord-nn2333-1.png");
            chords.Add("Gm7", "https://www.imbf.org/akkordy/images/gm7chord-131111-3.png");
            chords.Add("Gmaj7", "https://www.imbf.org/akkordy/images/gmaj7chord-nn4321-2.png");
            chords.Add("Gmaj9", "https://www.imbf.org/akkordy/images/gmaj9chord-114121-2.png");
            chords.Add("Gsus4", "https://www.imbf.org/akkordy/images/gsus4chord-nn0011-1.png");
            chords.Add("Gm", "https://www.imbf.org/akkordy/images/gmchord-133111-3.png");
            chords.Add("G ", "https://www.imbf.org/akkordy/images/gchord-320003-1.png");
            chords.Add("A ", "https://www.imbf.org/akkordy/images/achord-n02220-1.png");*/

            UkuleleChords.Add("A u", "https://pp.userapi.com/c824604/v824604391/cdad3/SwZBX7_xS_4.jpg");
            UkuleleChords.Add("B u", "https://pp.userapi.com/c824604/v824604391/cdada/TQRF1lwBIWI.jpg");
            UkuleleChords.Add("C u", "https://pp.userapi.com/c824604/v824604391/cdab0/GLAAeTy5ImY.jpg");
            UkuleleChords.Add("D u", "https://pp.userapi.com/c824604/v824604391/cdab7/7zdoeuy89ho.jpg");
            UkuleleChords.Add("E u", "https://pp.userapi.com/c824604/v824604391/cdabe/BMLuYMCazKg.jpg");
            UkuleleChords.Add("F u", "https://pp.userapi.com/c824604/v824604391/cdac5/r_UUHLi3xw4.jpg");
            UkuleleChords.Add("G u", "https://pp.userapi.com/c824604/v824604391/cdacc/7eRvbAxq2tU.jpg");
            UkuleleChords.Add("Am u", "https://pp.userapi.com/c824604/v824604391/cdb04/6enxkqaZjKI.jpg");
            UkuleleChords.Add("Bm u", "https://pp.userapi.com/c824604/v824604391/cdb0b/1nPS_buZgUE.jpg");
            UkuleleChords.Add("Cm u", "https://pp.userapi.com/c824604/v824604391/cdae1/igOHiNsfRoo.jpg");
            UkuleleChords.Add("Dm u", "https://pp.userapi.com/c824604/v824604391/cdae8/gRV6ls5Z__I.jpg");
            UkuleleChords.Add("Em u", "https://pp.userapi.com/c824604/v824604391/cdaef/VZWP0hiqHuQ.jpg");
            UkuleleChords.Add("Fm u", "https://pp.userapi.com/c824604/v824604391/cdaf6/m4mfqS99TeQ.jpg");
            UkuleleChords.Add("Gm u", "https://pp.userapi.com/c824604/v824604391/cdafd/Zqkz2e9F4ic.jpg");
            UkuleleChords.Add("A7 u", "https://pp.userapi.com/c824604/v824604391/cdb35/QeiU39PXr6o.jpg");
            UkuleleChords.Add("B7 u", "https://pp.userapi.com/c824604/v824604391/cdb3c/NY1ActeXsMQ.jpg");
            UkuleleChords.Add("C7 u", "https://pp.userapi.com/c824604/v824604391/cdb12/GTenSRhqHPk.jpg");
            UkuleleChords.Add("D7 u", "https://pp.userapi.com/c824604/v824604391/cdb19/2KIdXiRlo1w.jpg");
            UkuleleChords.Add("E7 u", "https://pp.userapi.com/c824604/v824604391/cdb20/NqA8AH90Gos.jpg");
            UkuleleChords.Add("F7 u", "https://pp.userapi.com/c824604/v824604391/cdb27/a4iJ-3Bs6cw.jpg");
            UkuleleChords.Add("G7 u", "https://pp.userapi.com/c824604/v824604391/cdb2e/CkI1-acD5Gs.jpg");
            UkuleleChords.Add("Am7 u", "https://pp.userapi.com/c824604/v824604391/cdba5/smp8Ef2fMkY.jpg");
            UkuleleChords.Add("Bm7 u", "https://pp.userapi.com/c824604/v824604391/cdbac/jggm-6_x0sU.jpg");
            UkuleleChords.Add("Cm7 u", "https://pp.userapi.com/c824604/v824604391/cdb82/jzd_DSdbvIs.jpg");
            UkuleleChords.Add("Dm7 u", "https://pp.userapi.com/c824604/v824604391/cdb89/B93eAk-Ddas.jpg");
            UkuleleChords.Add("Em7 u", "https://pp.userapi.com/c824604/v824604391/cdb90/zZxG4JUit0o.jpg");
            UkuleleChords.Add("Fm7 u", "https://pp.userapi.com/c824604/v824604391/cdb97/VgwfHIhi2aM.jpg");
            UkuleleChords.Add("Gm7 u", "https://pp.userapi.com/c824604/v824604391/cdb9e/KjiQWKP0-10.jpg");
            UkuleleChords.Add("Amaj7 u", "https://pp.userapi.com/c824604/v824604391/cdb74/hYS3nIZKCaE.jpg");
            UkuleleChords.Add("Bmaj7 u", "https://pp.userapi.com/c824604/v824604391/cdb7b/5kqmHWZEj6o.jpg");
            UkuleleChords.Add("Cmaj7 u", "https://pp.userapi.com/c824604/v824604391/cdb51/uE0UvkcqiAw.jpg");
            UkuleleChords.Add("Dmaj7 u", "https://pp.userapi.com/c824604/v824604391/cdb58/3Snq9-2AHAk.jpg");
            UkuleleChords.Add("Emaj7 u", "https://pp.userapi.com/c824604/v824604391/cdb5f/E0FDC5CsYns.jpg");
            UkuleleChords.Add("Fmaj7 u", "https://pp.userapi.com/c824604/v824604391/cdb51/uE0UvkcqiAw.jpg");
            UkuleleChords.Add("Gmaj7 u", "https://pp.userapi.com/c824604/v824604391/cdb6d/VYpGylBax8g.jpg");
        }
    }


}