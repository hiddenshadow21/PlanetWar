public static class PlayerInfo
{
    private static int id = generateRandomId();
    private static string name = generateRandomKey();
    private static string eMail;

    public static int ID
    {
        get => id;
        set => id = value;
    }

    public static string Name
    {
        get => name;
        set => name = value;
    }

    public static string EMail
    {
        get => name + "@mail.pl";
        set => eMail = value;
    }

    private static int generateRandomId()
    {
        const int N = 100000;
        var random = new System.Random();
        return random.Next(N);
    }

    private static string generateRandomKey()
    {
        string tempName = "";
        const int N = 7;
        const string chars = "abcdefghijklmnopqrstuvwxyz";
        var random = new System.Random();

        for (int i = 0; i < N; i++)
            tempName += chars[random.Next(chars.Length)];
        return tempName;
    }
}