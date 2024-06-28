using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfVanillaChat.MVVM.Model
{
    public class MessageModel
    {
        public string Username { get; set; } = string.Empty;
        public string UsernameColor { get; set; } = string.Empty;
        public string ImageSource { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public DateTime Time { get; set; }

        public bool IsNativeOrigin { get; set; }
        public bool? firstMessage { get; set; }

    }
}
