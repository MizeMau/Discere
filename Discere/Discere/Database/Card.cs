using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Discere.Database
{
    public class Card
    {
        [Table("Card")]
        public class Model
        {
            [Key]
            public long CardID { get; set; }
            public long Number { get; set; }
            public string Question { get; set; }
            public string Answer { get; set; }
            public int Difficulty { get; set; }
        }
        public class DTO : Model
        {
            public string UserAnswer { get; set; }
        }
        public class Service : Service<Model>
        {

        }
    }
}
