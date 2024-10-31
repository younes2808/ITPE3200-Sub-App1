using Microsoft.EntityFrameworkCore;
using RAYS.DAL;
using RAYS.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RAYS.Repositories
{
    public class FriendRepository : IFriendRepository
    {
        private readonly ServerAPIContext _context;

        public FriendRepository(ServerAPIContext context)
        {
            _context = context;
        }

        public async Task<bool> SendFriendRequestAsync(Friend friend)
        {
            // Sjekk om sender og mottaker er den samme
            if (friend.SenderId == friend.ReceiverId)
                return false; // Sender og mottaker kan ikke være den samme

            // Sjekk om en eksisterende vennforespørsel allerede finnes
            var existingRequest = await _context.Friends
                .FirstOrDefaultAsync(f => 
                    (f.SenderId == friend.SenderId && f.ReceiverId == friend.ReceiverId) ||
                    (f.SenderId == friend.ReceiverId && f.ReceiverId == friend.SenderId));

            if (existingRequest != null)
                return false; // En vennforespørsel eksisterer allerede

            // Sett status til "Pending"
            friend.Status = "Pending";

            // Legg til ny vennforespørsel i databasen
            _context.Friends.Add(friend);
            await _context.SaveChangesAsync();
            return true; // Vennforespørsel sendt
        }

        public async Task<IEnumerable<Friend>> GetFriendRequestsAsync(int userId)
        {
            // Hent vennforespørslene for brukeren
            return await _context.Friends
                .Where(f => (f.ReceiverId == userId || f.SenderId == userId) && f.Status == "Pending")
                .ToListAsync();
        }

        public async Task<bool> AcceptFriendRequestAsync(int id)
        {
            var request = await _context.Friends.FindAsync(id);
            if (request == null) return false; // Forespørsel ikke funnet

            // Oppdater statusen til "Accepted"
            request.Status = "Accepted";
            await _context.SaveChangesAsync();
            return true; // Vennforespørsel akseptert
        }

        public async Task<bool> RejectFriendRequestAsync(int id)
        {
            var request = await _context.Friends.FindAsync(id);
            if (request == null) return false; // Forespørsel ikke funnet

            // Fjern vennforespørselen fra databasen
            _context.Friends.Remove(request);
            await _context.SaveChangesAsync();
            return true; // Vennforespørsel avvist
        }

        public async Task<IEnumerable<Friend>> GetFriendsAsync(int userId)
        {
            // Hent alle venner for brukeren
            return await _context.Friends
                .Where(f => (f.SenderId == userId || f.ReceiverId == userId) && f.Status == "Accepted")
                .ToListAsync();
        }

        public async Task<bool> DeleteFriendAsync(int userId, int friendId)
        {
            var friendRelationship = await _context.Friends
                .FirstOrDefaultAsync(f => 
                    (f.SenderId == userId && f.ReceiverId == friendId) ||
                    (f.SenderId == friendId && f.ReceiverId == userId) &&
                    f.Status == "Accepted");

            if (friendRelationship == null) return false; // Vennforhold ikke funnet

            // Fjern vennforholdet fra databasen
            _context.Friends.Remove(friendRelationship);
            await _context.SaveChangesAsync();
            return true; // Venn slettet
        }
    }
}
