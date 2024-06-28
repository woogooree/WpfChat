using Firebase.Database;
using Firebase.Database.Query;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WpfVanillaChat.Firebase
{
    public class FirebaseHelper
    {
        private static readonly string FirebaseDatabaseUrl = "https://wpfvanillachat-default-rtdb.firebaseio.com/";
        private readonly FirebaseClient _firebaseClient;

        public FirebaseHelper()
        {
            _firebaseClient = new FirebaseClient(FirebaseDatabaseUrl);
        }

        public async Task SaveMessageAsync(string room, string username, string message)
        {
            var chatMessage = new
            {
                Username = username,
                Message = message,
                Timestamp = DateTime.UtcNow
            };

            await _firebaseClient
                .Child("ChatRooms")
                .Child(room)
                .Child("Messages")
                .PostAsync(chatMessage);
        }

        public async Task<List<string>> LoadMessagesAsync(string room)
        {
            var messages = await _firebaseClient
                .Child("ChatRooms")
                .Child(room)
                .Child("Messages")
                .OrderByKey()
                .OnceAsync<Dictionary<string, object>>();


            List<string> chatMessages = new List<string>();
            foreach (var message in messages)
            {
                var msg = message.Object;
                chatMessages.Add($"{msg["Username"]}: {msg["Message"]}");
            }

            return chatMessages;
        }
    }
}
