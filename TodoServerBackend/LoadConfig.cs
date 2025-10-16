public static class Config
{

    public static void readConfig()
    {
        bool dev = false;
        try
        {
            dev = File.ReadAllLines("config.ini")[0].Contains("Development");
        }
        catch
        {
            //do nothing, just fail silently
        }
        Development = dev;
    }

    public static bool Development = false;
}