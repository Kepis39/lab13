using System;
using System.Collections.Generic;
using System.Text;

namespace lab13.Model
{
    [Serializable]
    public class Book
    {
        public string? Author { get; set; }
        public string? Title { get; set; }
        public int Circulation { get; set; }
        public decimal Price { get; set; }
        public int Year { get; set; }
    }
}
