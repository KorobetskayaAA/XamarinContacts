using System;
using System.Collections.Generic;
using System.Text;

using SQLite;

namespace XamarinContacts
{
    public class ContactRepository
    {
        SQLiteConnection database;

        public ContactRepository(string databasePath)
        {
            database = new SQLiteConnection(databasePath);
            database.CreateTable<Contact>();
        }

        public IEnumerable<Contact> GetContacts()
        {
            return database.Table<Contact>().ToList();
        }

        public Contact GetContact(int id)
        {
            return database.Get<Contact>(id);
        }

        public int DeleteContact(int id)
        {
            return database.Delete<Contact>(id);
        }

        public int SaveContact(Contact contact)
        {
            // у контакта есть id - значит он уже добавлен в БД
            if (contact.Id != 0)
            {
                database.Update(contact);
                return contact.Id;
            }
            // у нового контакта id нет - надо вставить новую запись
            return database.Insert(contact);
        }
    }
}
