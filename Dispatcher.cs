using Dispatcher.DbModels;
using System.Text.Json;

namespace Dispatcher;
internal class Program
{
    public static string WorkFolder = "C:\\Users\\memes\\Desktop\\homework\\rgu\\Diplom\\TestFolders\\Dispatcher\\Work";
    public static string DispatcherFolder = "Dispatcher";
    private static void Main(string[] args)
    {
        var output = "C:\\Users\\memes\\Desktop\\homework\\rgu\\Diplom\\TestFolders\\Dispatcher\\Out";
        var input = "C:\\Users\\memes\\Desktop\\homework\\rgu\\Diplom\\TestFolders\\Dispatcher\\In";

        using (var context = new MyDbContext())
        {
            context.Database.EnsureCreated();
        }
        ProcessInput(input);
        ProcessOutput(output);
    }
    public static void ProcessInput(string input)
    {
        var packs = Directory.GetDirectories(input);
        foreach (var pack in packs)
        {
            Console.WriteLine("Обработка пакета {0}", pack);
            var files = Directory.GetFiles(pack);
            var pass = files.FirstOrDefault(p => Path.GetFileName(p) == "passport.json");
            if (pass == null)
            {
                var errMes = "Ошибка при обработке пакета, паспорт не найден";
                var errorFolder = Path.Combine(WorkFolder, "Error", "In");
                Console.WriteLine(errMes);
                ProcessError(MessageDirection.Input, errorFolder, errMes);
                MoveToDirectory(pack, errorFolder);
                continue;
            }
            var content = File.ReadAllText(pass);
            FormData? data;
            try
            {
                data = JsonSerializer.Deserialize<FormData>(content);
                if (data == null)
                    throw new Exception("Файл паспорта прочитан неверно");

            }
            catch (Exception e)
            {
                var errMes = String.Format("Произошла ошибка при диссериализации файла {0}", e.ToString());
                Console.WriteLine(errMes);
                var errorFolder = Path.Combine(WorkFolder, "Error", "In");
                ProcessError(MessageDirection.Input, errorFolder, errMes);
                MoveToDirectory(pack, errorFolder);
                continue;
            }
            var folder = FindClientFolder(data);
            if (folder == null)
            {
                var warnMes = "Произошла ошибка при попытке найти клиента, пакет будет перемещен к диспетчеру";
                Console.WriteLine(warnMes);
                var dispatcherFolder = Path.Combine(WorkFolder, DispatcherFolder, "In");
                ProcessWarning(data, MessageDirection.Input, dispatcherFolder, warnMes);
                MoveToDirectory(pack, dispatcherFolder);
                continue;
            }
            var succFolder = Path.Combine(WorkFolder, folder, "In");
            ProcessSuccess(data, MessageDirection.Input, succFolder, "Пакет обработан успешно, сообщение передано клиенту");
            MoveToDirectory(pack, succFolder);
        }
    }

    public static void MoveToDirectory(string source, string destination)
    {
        destination = Path.Combine(destination, Guid.NewGuid().ToString());
        if (!Directory.Exists(destination))
            Directory.Move(source, destination);
    }
    private static void ProcessWarning(FormData data, MessageDirection direction, string folder, string comment)
    {
        using var context = new MyDbContext();
        var record = new JournalModel
        {
            Result = Result.Warn,
            MsgId = Guid.NewGuid().ToString(),
            Direction = direction,
            Comment = comment,
            LocalFolder = folder,
            ReceiverOrg = data.RecipientOrganization,
            SenderOrg = data.SenderOrganization,
            Type = data.Type,
            Format = data.Format
        };
        context.Journals.Add(record);
        context.SaveChanges();
    }

    private static void ProcessSuccess(FormData data, MessageDirection direction, string folder, string comment)
    {
        using var context = new MyDbContext();
        var record = new JournalModel
        {
            Result = Result.Success,
            MsgId = Guid.NewGuid().ToString(),
            Direction = direction,
            Comment = comment,
            LocalFolder = folder,
            ReceiverOrg = data.RecipientOrganization,
            SenderOrg = data.SenderOrganization,
            Type = data.Type,
            Format = data.Format
        };
        context.Journals.Add(record);
        context.SaveChanges();
    }

    private static string? FindClientFolder(FormData data)
    {
        using var context = new MyDbContext();
        var client = context.Clients.FirstOrDefault(c => c.OrgName == data.RecipientOrganization);
        if (client == null)
            return default(string);

        return client.Folder;
    }

    private static void ProcessError(MessageDirection direction, string folder, string comment)
    {
        using var context = new MyDbContext();
        var record = new JournalModel
        {
            Result = Result.Error,
            Direction = direction,
            Comment = comment,
            LocalFolder = folder
        };
        context.Journals.Add(record);
        context.SaveChanges();
    }

    public static void ProcessOutput(string output)
    {
        List<string> outputFolders;
        using (var context = new MyDbContext())
        {
            outputFolders = context.Clients.Select(c => c.Folder).ToList();
        }
        foreach (var outputFolder in outputFolders)
        {
            var directory = Path.Combine(WorkFolder, outputFolder, "Out");
            Directory.CreateDirectory(directory);
            var packs = Directory.GetDirectories(directory);
            foreach (var pack in packs)
            {
                Console.WriteLine("Обработка пакета {0}", pack);
                var files = Directory.GetFiles(pack);
                var pass = files.FirstOrDefault(p => Path.GetFileName(p) == "passport.json");
                if (pass == null)
                {
                    var errMes = "Ошибка при обработке пакета, паспорт не найден";
                    var errorFolder = Path.Combine(WorkFolder, "Error", "Out");
                    Console.WriteLine(errMes);
                    ProcessError(MessageDirection.Output, errorFolder, errMes);
                    MoveToDirectory(pack, errorFolder);
                    continue;
                }
                var content = File.ReadAllText(pass);
                FormData? data;
                try
                {
                    data = JsonSerializer.Deserialize<FormData>(content);
                    if (data == null)
                        throw new Exception("Файл паспорта прочитан неверно");

                }
                catch (Exception e)
                {
                    var errMes = String.Format("Произошла ошибка при диссериализации файла {0}", e.ToString());
                    Console.WriteLine(errMes);
                    var errorFolder = Path.Combine(WorkFolder, "Error", "Out");
                    ProcessError(MessageDirection.Output, errorFolder, errMes);
                    MoveToDirectory(pack, errorFolder);
                    continue;
                }
                ProcessSuccess(data, MessageDirection.Input, output, "Пакет обработан успешно, сообщение передано в МЭДО");
                MoveToDirectory(pack, output);
            }
        }
    }
}