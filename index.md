---
emoji: 💻
title: WPF TCP 채팅 프로젝트
date: '2024-06-24'
author: 심우진
tags: WPF TCP Socket 채팅
categories: WPF PROJECT
---


## 프로젝트 개요

### 프로젝트명
어플리케이션 기반의 채팅서버 프로그램 (C# WPF TCP)

### 수행기간
- 2024.6.18 - 2024.6.30

## 프로젝트 수행 계획 (요구사항, 기능 명세)

![Plan_01.png](./Plan_01.png)

![Plan_02.png](./Plan_02.png)

## 주요 기능
- 닉네임 기반 사용자 연결
- 메시지 전송 및 수신
- Firebase를 연동하여 채팅로그 저장
- 채팅방 별로 이전 채팅 불러오기
- MVVM 구조로 MainWindow가 MainViewModel을 호출

## TCP 서버-클라이언트 통신을 위한 로직 이해

![ChatFlow.png](./ChatFlow.png)

![SocketDiagram.png](./SocketDiagram.png)

## 코드 설명

### ViewModel 구성 요소

```csharp
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

        /* Chat Load&Send 1:N */
        private async Task LoadMessages()
        {
            if (Users.Count > 1)
            {
                var roomname = GetRoomName(Users.Select(u => u.Username).ToList());
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
            if (Users.Count > 1)
            {
                var roomname = GetRoomName(Users.Select(u => u.Username).ToList());
                await _server.SendMessageToServer(Chat, roomname);
                await _firebaseHelper.SaveMessageAsync(roomname, Username, Chat);
                Chat = string.Empty; // 메시지 전송 후 입력창 비우기
            }
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

        private string GetRoomName(List<string> usernames)
        {
            var users = usernames;
            users.Sort();
            return $"ChatRoom_{string.Join("_", users)}";
        }
    }
}            
```

- 옵저버블컬렉션으로 리스트 관리
Users: 현재 연결된 사용자 목록을 관리하는 ObservableCollection<UserModel>.
Chats: 현재 채팅방의 메시지 목록을 관리하는 ObservableCollection<string>.
Contacts: 사용자의 연락처 목록을 관리하는 ObservableCollection<ContactModel>.
Messages: 개별 메시지 모델을 관리하는 ObservableCollection<MessageModel>.

- 명령(Command) 및 데이터 바인딩
ConnectToServerCommand: 서버에 연결하는 명령.
SendMessageCommand: 메시지를 전송하는 명령.

- 사용자 이름 및 채팅 내용 관리
Username: 사용자의 이름을 저장하고 변경을 알리는 속성.
Chat: 현재 입력된 채팅 메시지를 저장하고 변경을 알리는 속성.

### Firebase 연동

```csharp
namespace WpfVanillaChat.Firebase
{
    public class FirebaseHelper
    {
        private static readonly string FirebaseDatabaseUrl = "파이어베이스 URL";
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
```

리얼타임데이터베이스에 채팅로그가 저장되고 같은 유저끼리 채팅할 때 내용이 호출되게끔 구성되었다.

## 결과물

![VanillaChat.png](./VanillaChat.png)

배울게 정말 많은 프로젝트 였음!


```toc

```

