using HealthInstitution.Repository;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthInstitution.Class
{
    public class Notification
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public bool IsRead { get; set; }
        public string Username { get; set; }
        public DateTime Time { get; set; }

        [JsonConstructor]
        public Notification(int id, string text, bool isRead, string username, DateTime time)
        {
            Id = id;
            Text = text;
            IsRead = isRead;
            Username = username;
            Time = time;
        }

        public Notification(string text, string username)
        {
            Id = NotificationRepository.CreateNewId();
            Text=text;
            Username=username;
            IsRead = false;
            Time = DateTime.Now;
        }
    }
}
