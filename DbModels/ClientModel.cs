using System.ComponentModel.DataAnnotations;

namespace Dispatcher.DbModels
{
    public class ClientModel
    {
        [Key]
        public int Id { get; set; }
        public string OrgName { get; set; }
        public string Folder { get; set; }
        public string Password { get; set; }
        public DateTime CreationDate { get; set; }

    }
}
