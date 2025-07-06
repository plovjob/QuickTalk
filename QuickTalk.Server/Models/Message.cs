namespace QuickTalk.Server.Models
{
    //убрать сеттеры в публичных
    public class Message
    {
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public string Text { get; set; }

        //SentAt DateTimeOffset
        public string TimeOfSend{ get; set; }
    }
}
