namespace Dispatcher;
public class FormData
{
    public string? SenderOrganization { get; set; }
    public string? SenderOrganizationPerson { get; set; }

    public string? RecipientOrganization { get; set; }
    public string? RecipientOrganizationPerson { get; set; }

    public DateTime? FormDate { get; set; }

    public MessageType? Type { get; set; }
    public MessageFormat? Format { get; set; }
}
public enum MessageFormat
{
    One,
    TwoPointFive,
    Three
}
public enum MessageType
{
    Document,
    Receipt,
    Notification
}
public enum MessageDirection
{
    Input,
    Output
}
public enum Result
{
    Success,
    Warn,
    Error
}