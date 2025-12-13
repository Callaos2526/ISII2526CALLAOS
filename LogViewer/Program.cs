namespace LogViewer;

public class Program
{
    public static void Main(string[] args)
    {
        //Recibir por consola para suscribirse a un topic específico
        string topic = args.Length > 0 ? args[0] : AskTopicFromConsole();
        var subscriber = new Subscriber(topic);
        subscriber.Start();
    }
    private static string AskTopicFromConsole()
    {
        Console.WriteLine("Introduce el topic al que suscribirse (p.ej. log.error, log.information, log.*):");
        var input = Console.ReadLine();
        return string.IsNullOrWhiteSpace(input) ? "log.*" : input.Trim();
    }
}
