using System.ComponentModel.DataAnnotations;

namespace Dispatcher.DbModels
{
    internal class JournalModel
    {
        [Key]
        public int Id { get; set; }
        public string? MsgId { get; set; }
        public string? ReceiverOrg { get; set; }
        public string? SenderOrg { get; set; }
        public Result Result { get; set; }
        public MessageType? Type { get; set; }
        public MessageFormat? Format { get; set; }
        public MessageDirection Direction { get; set; }
        public string? LocalFolder { get; set; }
        public string? Comment { get; set; }
    }
}
