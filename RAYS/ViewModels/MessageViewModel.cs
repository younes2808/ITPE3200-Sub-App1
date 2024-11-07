using RAYS.Models;
namespace RAYS.ViewModels
{
    public class MessageViewModel
    {
        public int SenderId { get; set; }
        public int ReceiverId { get; set; }

        // Liste av meldinger for å vise flere meldinger i konversasjonen
        public List<Message> Messages { get; set; }

        // Ekstra felter for visning, som kan være nyttige for å vise navn på sender og mottaker
        public string SenderName { get; set; }
        public string ReceiverName { get; set; }
    }
}
