using HealthInstitution.Class;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthInstitution.Repository
{
    public static class NotificationRepository
    {
        private static readonly string _filenameNotifications = "../../../Data/Users/notifications.json";
        public static List<Notification> Notifications { get; set; } = new List<Notification>();

        static NotificationRepository()
        {
            
            if (File.Exists(_filenameNotifications))
            {
                var notifications = (List<Notification>?)FileLoader.Deserialize<Notification>(_filenameNotifications);

                if (notifications != null)
                {
                    Notifications = notifications;
                }

            }
        }

        public static void Save()
        {
            FileLoader.Serialize<Notification>(Notifications,_filenameNotifications);
        }

        public static List<Notification> FindAllNotifications(string username)
        {
            return Notifications.Where(x => x.Username == username).OrderByDescending(x => x.Time).ToList();
        }

        public static Notification FindNotification(int id)
        {
            return Notifications.Where(n => n.Id == id).ToList()[0];
        }

        public static void AddNotification(Notification newNotification)
        {
            Notifications.Add(newNotification);
            Save();
        }

        public static void SetToRead(int id)
        {
            FindNotification(id).IsRead = true;
            Save();
        }

        public static int CreateNewId()
        {
            var ids = Notifications.Select(n => n.Id).ToList();
            if (ids.Count == 0)
                return 1;

            return ids.Max() + 1;
        }
    }
}
