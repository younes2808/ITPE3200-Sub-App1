using System.Collections.Generic;

namespace RAYS.ViewModels
{
       public class ConversationsViewModel
    {
        public List<ConversationSummaryViewModel> Conversations { get; set; } = new List<ConversationSummaryViewModel>(); // List of conversations
    }
}
