using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Collections.ObjectModel;
using WpfVanillaChat.Core;
using WpfVanillaChat.MVVM.Model;
using WpfVanillaChat.Net;
using WpfVanillaChat.Firebase;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using System.Runtime.CompilerServices;

namespace WpfVanillaChat.MVVM.ViewModel
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<UserModel> Users { get; set; } = new ObservableCollection<UserModel>();
        public ObservableCollection<string> Chats { get; set; } = new ObservableCollection<string>();
        public ObservableCollection<ContactModel> Contacts { get; set; } = new ObservableCollection<ContactModel>();
        public ObservableCollection<MessageModel> Messages { get; set; } = new ObservableCollection<MessageModel>();

        /* Commands */

        public ICommand ConnectToServerCommand { get; set; }
        public ICommand SendMessageCommand { get; set; }
        //public RelayCommand SendCommand { get; set; }

        /* Username 선언 */

        private string _username = string.Empty;
        public string Username
        {
            get => _username;
            set
            {
                if (_username != value)
                {
                    _username = value;
                    OnPropertyChanged();
                }
            }
        }

        /* Chat 선언 */
        private string _chat = string.Empty;
        public string Chat
        {
            get => _chat;
            set
            {
                if (_chat != value)
                {
                    _chat = value;
                    OnPropertyChanged();
                }
            }
        }

        /* ContactModel 선언 */
        private ContactModel _selectedcontact = new ContactModel();

        public ContactModel Selectedcontact
        {
            get { return _selectedcontact; }
            set
            {
                _selectedcontact = value;
                OnPropertyChanged();
            }
        }

        /* 서버&파이어베이스 선언 */
        private readonly Server _server;
        private readonly FirebaseHelper _firebaseHelper;

        public MainViewModel()
        {
            _server = new Server();
            _firebaseHelper = new FirebaseHelper();

            _server.UserConnectedEvent += UserConnected;
            _server.MsgReceivedEvent += MessageReceived;
            _server.UserDisconnectedEvent += RemoveUser;

            ConnectToServerCommand = new RelayCommand(async o => await ConnectToServer(), o => !string.IsNullOrEmpty(Username));
            SendMessageCommand = new RelayCommand(async o => await SendMessage(), o => !string.IsNullOrEmpty(Chat));
        }
        private async Task ConnectToServer()
        {
            _server.ConnectToServer(Username);
            await LoadMessages();
        }

        /* Chat Load&Send */
        private async Task LoadMessages()
        {
            if (Users.Count == 2)
            {
                var roomname = GetRoomName(Users[0].Username, Users[1].Username);
                var chats = await _firebaseHelper.LoadMessagesAsync(roomname);
                Application.Current.Dispatcher.Invoke(() =>
                {
                    Chats.Clear();
                    foreach (var chat in chats)
                    {
                        Chats.Add(chat);
                    }
                });
            }
        }

        private async Task SendMessage()
        {
            var roomname = GetRoomName(Users[0].Username, Users[1].Username);
            await _server.SendMessageToServer(Chat, roomname);
            await _firebaseHelper.SaveMessageAsync(roomname, Username, Chat);
            Chat = string.Empty; // 메시지 전송 후 입력창 비우기
        }

        /* Action */
        private async void UserConnected(string username, string uid)
        {
            var user = new UserModel
            {
                Username = username,
                UID = uid,
            };

            if (!Users.Any(x => x.UID == user.UID))
            {
                Application.Current.Dispatcher.Invoke(() => Users.Add(user));
                await LoadMessages(); // 사용자 연결 시 메시지 불러오기 시도
            }
        }

        private void MessageReceived()
        {
            if (_server.PacketReader != null)
            {
                var msg = _server.PacketReader.ReadMessage();
                Application.Current.Dispatcher.Invoke(() => Chats.Add(msg));
            }
        }

        private void RemoveUser()
        {
            if (_server.PacketReader != null)
            {
                var uid = _server.PacketReader.ReadMessage();
                var user = Users.FirstOrDefault(x => x.UID == uid);
                if (user != null)
                {
                    Application.Current.Dispatcher.Invoke(() => Users.Remove(user));
                }
            }
        }

        /* RelayCommend 추가 */
        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private string GetRoomName(string user1, string user2)
        {
            var users = new List<string> { user1, user2 };
            users.Sort();
            return $"ChatRoom_{users[0]}_{users[1]}";
        }

        //Profiles = new ObservableCollection<string>();
        //Contacts = new ObservableCollection<ContactModel>();

        //SendCommand = new RelayCommand(o =>
        //{
        //    Messages.Add(new MessageModel
        //    {
        //        Message = Message,
        //        firstMessage = false
        //    });

        //    Message = "";
        //});

        //Messages.Add(new MessageModel
        //{
        //    Username = "강동우",
        //    UsernameColor = "#409aff",
        //    ImageSource = "https://i.imgur.com/yMWvLXd.png",
        //    Message = "Test",
        //    Time = DateTime.Now,
        //    IsNativeOrigin = false,
        //    firstMessage = true,
        //});

        //for (int i = 0; i < 4; i++)
        //{
        //    Messages.Add(new MessageModel
        //    {
        //        Username = "정영도",
        //        UsernameColor = "#409aff",
        //        ImageSource = "https://i.imgur.com/yMWvLXd.png",
        //        Message = "Test",
        //        Time = DateTime.Now,
        //        IsNativeOrigin = true,
        //    });
        //}

        //Messages.Add(new MessageModel
        //{
        //    Username = "박승준",
        //    UsernameColor = "#409aff",
        //    ImageSource = "https://i.imgur.com/yMWvLXd.png",
        //    Message = "Last",
        //    Time = DateTime.Now,
        //    IsNativeOrigin = true,
        //});

        //for (int i = 0; i < 5; i++)
        //{
        //    Contacts.Add(new ContactModel
        //    {
        //        Username = $"강동우 {i}",
        //        ImageSource = "https://i.imgur.com/yMWvLXd.png",
        //        Messages = Messages,
        //    }); 
        //}
    }
}
