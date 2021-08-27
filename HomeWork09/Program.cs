using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

namespace HomeWork09
{
    class Program
    {
        private static TelegramBotClient tgBot;
        //static InlineKeyboardMarkup buttons;
        static string path = @"Download\";

        static void Main(string[] args)
        {
            string token = "";

            tgBot = new(token) { Timeout = TimeSpan.FromSeconds(1) };
            //InlineKeyboardButton chooseDocument = new InlineKeyboardButton();
            //chooseDocument.Text = "Документ";
            //chooseDocument.CallbackData = "/getallfiles";
            //buttons = new InlineKeyboardMarkup(chooseDocument);

            tgBot.OnMessage += TgBot_OnMessage;
            tgBot.StartReceiving();



            Console.ReadLine();
        }

        public static void TgBot_OnMessage(object sender, Telegram.Bot.Args.MessageEventArgs e)
        {

            var chatId = e.Message.Chat.Id;


            //Console.WriteLine($"Полученное сообщение: {text}");
            //await tgBot.SendTextMessageAsync(
            //    chatId: e.Message.Chat, 
            //    text: $"Вы написали {text}");
            if (e.Message.Type == Telegram.Bot.Types.Enums.MessageType.Text)
            {
                var messageText = e?.Message?.Text; if (messageText == null) return;
                MessageHandler(chatId, messageText);
            }

            if (e.Message.Type == Telegram.Bot.Types.Enums.MessageType.Document)
            {
                Console.WriteLine(e.Message.Document.FileSize);
                DownloadDocumentOnLocalStorage(e.Message.Document.FileId, e.Message.Document.FileName);
                Console.WriteLine($"Файл {e.Message.Document.FileName} сохранен");
            }
            if (e.Message.Type == Telegram.Bot.Types.Enums.MessageType.Photo)
            {
                DownloadDocumentOnLocalStorage(e.Message.Photo[e.Message.Photo.Length - 1].FileId, e.Message.Photo[e.Message.Photo.Length - 1].FileId);
                Console.WriteLine("Фото загружено");
            }

        }



        /// <summary>
        /// Загрузка документа на локальное храниилище
        /// </summary>
        /// <param name="fileId"></param>
        /// <param name="fileName"></param>
        private static async void DownloadDocumentOnLocalStorage(string fileId, string fileName)
        {
            var file = await tgBot.GetFileAsync(fileId);

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            using (FileStream fs = new(path + fileName, FileMode.Create))
            {
                await tgBot.DownloadFileAsync(file.FilePath, fs);
            }
        }
        /// <summary>
        /// Показывает все имена файлов на локальном хранилище
        /// </summary>
        /// <param name="chatId"></param>
        private static void GetAllFiles(long chatId)
        {
            var message = "Все доступные файлы:\n";

            if (Directory.Exists(path))
            {
                var files = Directory.EnumerateFiles(path);
                foreach (var file in files)
                {
                    message += file.Remove(0, path.Length) + "\n";
                }
            }
            else
            {
                message += "ничего не найдено";
            }

            tgBot.SendTextMessageAsync(chatId, message);
        }


        private static void DownloadImg()
        {

        }

        /// <summary>
        /// Обработка и отправка сообщений
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="userMessage"></param>
        private static void MessageHandler(long userId, string userMessage)
        {
            Random r = new();
            string[] greetingMessages = { "Привет", "Привет Привет", "Салют", "Я пришёл с миром" };
            string randomGreeting = greetingMessages[r.Next(greetingMessages.Length)];
            //var userGreeting = greetingMessages.Select(g => g == userMessage).ToString();
            //Console.WriteLine(userMessage);
            //Console.WriteLine(userGreeting);
            if (userMessage.ToLower() == "/start")
            {
                tgBot.SendTextMessageAsync(chatId: userId, $"{randomGreeting}! Я телеграм бот и я могу сохранять файлы");
            }
            else if (userMessage == "/getallfiles")
            {
                GetAllFiles(userId);
            }
            else if (userMessage.ToLower() == "привет")
            {
                tgBot.SendTextMessageAsync(chatId: userId, randomGreeting);
            }
            else
            {
                tgBot.SendTextMessageAsync(chatId: userId, "Не понял");
            }
        }
    }
}
