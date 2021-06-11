using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Telegram.Bot;

namespace HomeWork09
{
    class Program
    {
        private static TelegramBotClient tgBot;

        static void Main(string[] args)
        {
            string token = "1849814420:AAFOifoC5c5ePYcOeATTdRWHSNdW0PrO2lA";

            tgBot = new (token) { Timeout = TimeSpan.FromSeconds(1) };

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
                DownloadDocumentOnLocalStorage(e.Message.Photo[e.Message.Photo.Length-1].FileId, e.Message.Photo[e.Message.Photo.Length - 1].FileId);
                Console.WriteLine("Фото загружено");
            }

        }

        

        /// <summary>
        /// Загрузка документа на локальное храниилище
        /// </summary>
        /// <param name="fileId"></param>
        /// <param name="path"></param>
        private static async void DownloadDocumentOnLocalStorage(string fileId, string path)
        {
            var file = tgBot.GetFileAsync(fileId);
            using (FileStream fs = new($@"Download\{path}", FileMode.Create))
            {
                await tgBot.DownloadFileAsync(file.Result.FilePath, fs);
            }
        }
        /// <summary>
        /// Показывает все имена файлов на локальном хранилище
        /// </summary>
        /// <param name="chatId"></param>
        private static void GetAllFiles(long chatId)
        {
            var localpath = @"Download";
            var files = Directory.EnumerateFiles(localpath);
            var message = "Все доступные файлы:\n";
            foreach (var file in files)
            {
                message += file.Remove(0, localpath.Length+1)+"\n";
                
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
            string [] greetingMessages = { "Привет", "Привет Привет", "Салют", "Я пришёл с миром" };
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
