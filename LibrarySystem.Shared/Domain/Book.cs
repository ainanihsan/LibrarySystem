using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibrarySystem.Shared.Domain
{
    public class Book
    {
        public int Id { get; set; }  // Primary key
        public required string Title { get; set; }
        public required string Author { get; set; }
        public int Pages { get; set; }
        public int CopiesTotal { get; set; }
    }

}
