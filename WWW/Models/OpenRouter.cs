namespace FiszkiNetNauka.Models
{
    public class Message
    {
        public string role { get; set; }
        public string content { get; set; }
    }

    public class ChatRequest
    {
        public string Model { get; set; }
        public List<Message> messages { get; set; }
        public double temperature { get; set; } = 0.7;
    }

    public class Choice
    {
        public Choice(Message message)
        {
            Message = message;
        }

        public Message Message { get; set; }
    }

    public class ChatResponse
    {
        public List<Choice> choices { get; set; }
    }
}
