using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfVanillaChat.MVVM.Model
{
    public class ContactModel
    {
        public string Username { get; set; } = string.Empty;
        public string ImageSource { get; set; } = string.Empty;

        public ObservableCollection<MessageModel> Messages { get; set; } = new ObservableCollection<MessageModel>();

        public string LastMessage => Messages.Last().Message;
    }
}
