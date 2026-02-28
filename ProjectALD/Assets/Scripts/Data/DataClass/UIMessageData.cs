using Newtonsoft.Json.Linq;

public class UIMessageData
{
    public JObject messageData;
    
    public UIMessageData()
    {
        messageData = JsonHandler.Load("Messages");
    }
}
