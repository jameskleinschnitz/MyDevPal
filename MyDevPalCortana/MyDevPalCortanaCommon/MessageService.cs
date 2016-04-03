using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyDevPalCortanaCommon
{
    public class MessageService
    {
        public static async Task SendMessage(string source, string message, int mood, int vitality, int focus)
        {
            var Client = new MoodTracker.Client.Action.Container(new Uri("https://devstg6.cireson.com/api"));

            await Client.ApplyDelta("KatieJ@BuildDemo126.onmicrosoft.com", mood, vitality, focus, message, source).GetValueAsync();
        }
    }
}
