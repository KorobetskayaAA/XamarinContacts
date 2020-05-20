using System;
using System.Collections.Generic;
using System.Text;

using SQLite;

namespace XamarinContacts
{
    [Table("Contact")]
    public class Contact
    {
        [PrimaryKey, AutoIncrement, Column("_id")]
        public int Id { get; set; }
        [Unique, NotNull]
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
    }
}
