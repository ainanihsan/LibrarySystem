using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibrarySystem.Shared.Domain
{
    public class LendingRecord
    {
        public DateTime BorrowDate { get; set; }
        public DateTime ReturnDate { get; set; }
        public int Pages { get; set; } 
    }
}
