namespace CompleteExample;

public class TelemetrySource
{
    private const string DATA = "We're no strangers to love\r\nYou know the rules and so do I (do I)\r\nA full commitment's what I'm thinking of\r\nYou wouldn't get this from any other guy\r\nI just wanna tell you how I'm feeling\r\nGotta make you understand\r\nNever gonna give you up\r\nNever gonna let you down\r\nNever gonna run around and desert you\r\nNever gonna make you cry\r\nNever gonna say goodbye\r\nNever gonna tell a lie and hurt you\r\nWe've known each other for so long\r\nYour heart's been aching, but you're too shy to say it (say it)\r\nInside, we both know what's been going on (going on)\r\nWe know the game and we're gonna play it\r\nAnd if you ask me how I'm feeling\r\nDon't tell me you're too blind to see\r\nNever gonna give you up\r\nNever gonna let you down\r\nNever gonna run around and desert you\r\nNever gonna make you cry\r\nNever gonna say goodbye\r\nNever gonna tell a lie and hurt you\r\nNever gonna give you up\r\nNever gonna let you down\r\nNever gonna run around and desert you\r\nNever gonna make you cry\r\nNever gonna say goodbye\r\nNever gonna tell a lie and hurt you\r\nWe've known each other for so long\r\nYour heart's been aching, but you're too shy to say it (to say it)\r\nInside, we both know what's been going on (going on)\r\nWe know the game and we're gonna play it\r\nI just wanna tell you how I'm feeling\r\nGotta make you understand\r\nNever gonna give you up\r\nNever gonna let you down\r\nNever gonna run around and desert you\r\nNever gonna make you cry\r\nNever gonna say goodbye\r\nNever gonna tell a lie and hurt you\r\nNever gonna give you up\r\nNever gonna let you down\r\nNever gonna run around and desert you\r\nNever gonna make you cry\r\nNever gonna say goodbye\r\nNever gonna tell a lie and hurt you\r\nNever gonna give you up\r\nNever gonna let you down\r\nNever gonna run around and desert you\r\nNever gonna make you cry\r\nNever gonna say goodbye\r\nNever gonna tell a lie and hurt you";
    private string[] _rows;
    private int _index = 0;

    public TelemetrySource()
    {
        _rows = DATA.Split("\r\n");
    }

    public Telemetry Next()
    {
        if (_index == _rows.Length)
        {
            _index = 0;
        }

        return new Telemetry
        {
            Timestamp = DateTimeOffset.Now,
            Type = "Jam",
            Unit = "Line",
            Value = _rows[_index++],
        };
    }
}
